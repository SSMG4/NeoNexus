using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NeoNexus.WinDemo.Services
{
    /// <summary>
    /// Robust download manager with resume, progress, cancellation, and simple temporary-file handling.
    /// - Resumes downloads when server supports Range.
    /// - Writes to a temporary ".part" file and moves atomically when complete.
    /// - Reports progress via IProgress&lt;double&gt; (0.0 .. 100.0).
    /// - Ensures only one downloader writes to a given destination (simple file lock).
    /// </summary>
    public class DownloadManager : IDisposable
    {
        private readonly HttpClient _http;
        private readonly string _tempExt;
        private bool _disposed;

        public DownloadManager(string temporaryExtension = null)
        {
            _tempExt = temporaryExtension ?? Config.Current.Download.TemporaryExtension ?? ".part";
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _http = new HttpClient(handler, disposeHandler: true)
            {
                Timeout = TimeSpan.FromMinutes(30)
            };
        }

        /// <summary>
        /// Download a file with resume and progress reporting.
        /// destination: final destination path (full path).
        /// progress: optional progress reporter (percentage 0..100).
        /// cancellationToken: optional cancellation token.
        /// </summary>
        public async Task DownloadFileAsync(string url, string destination, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(destination)) throw new ArgumentNullException(nameof(destination));

            Directory.CreateDirectory(Path.GetDirectoryName(destination) ?? Environment.CurrentDirectory);

            string temp = destination + _tempExt;

            // Simple single-process lock: create a lock file next to temp
            string lockFile = temp + ".lock";
            using (var lockFs = new FileStream(lockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                try
                {
                    // Determine existing length
                    long existingLength = 0;
                    if (File.Exists(temp))
                    {
                        var fi = new FileInfo(temp);
                        existingLength = fi.Length;
                    }

                    // Issue HEAD request to see if server supports ranges and get total length
                    long? totalLength = null;
                    bool acceptRanges = false;

                    using (var headReq = new HttpRequestMessage(HttpMethod.Head, url))
                    using (var headResp = await _http.SendAsync(headReq, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
                    {
                        if (headResp.IsSuccessStatusCode)
                        {
                            if (headResp.Content.Headers.ContentLength.HasValue)
                                totalLength = headResp.Content.Headers.ContentLength.Value;
                            if (headResp.Headers.Contains("Accept-Ranges"))
                                acceptRanges = true;
                            else
                            {
                                // Some servers don't advertise Accept-Ranges. We'll try Range anyway.
                                acceptRanges = true;
                            }
                        }
                    }

                    // If existingLength equals totalLength, simply move to destination
                    if (totalLength.HasValue && existingLength == totalLength.Value && File.Exists(temp))
                    {
                        File.Move(temp, destination, overwrite: true);
                        progress?.Report(100.0);
                        return;
                    }

                    // Prepare the request; ask for range if we have partial file and server supports ranges
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (existingLength > 0 && acceptRanges)
                    {
                        request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(existingLength, null);
                    }

                    using (var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();

                        // Determine final size
                        if (!totalLength.HasValue)
                        {
                            if (response.Content.Headers.ContentLength.HasValue)
                            {
                                // If we requested a range, the Content-Length is the remaining bytes
                                if (existingLength > 0 && response.StatusCode == HttpStatusCode.PartialContent)
                                {
                                    totalLength = existingLength + response.Content.Headers.ContentLength.Value;
                                }
                                else
                                {
                                    totalLength = response.Content.Headers.ContentLength.Value;
                                    existingLength = 0; // overwrite if we can't resume
                                }
                            }
                        }
                        else
                        {
                            // totalLength already has value from HEAD
                            // keep existingLength as-is
                        }

                        // Open file for append (or create)
                        using (var fs = new FileStream(temp, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                        {
                            fs.Seek(existingLength, SeekOrigin.Begin);
                            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                var buffer = new byte[81920];
                                long totalRead = existingLength;
                                int read;
                                var lastReport = DateTime.UtcNow;
                                while ((read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false)) > 0)
                                {
                                    fs.Write(buffer, 0, read);
                                    totalRead += read;

                                    // Report progress no more than 5 times per second to avoid UI lag
                                    if (progress != null && (DateTime.UtcNow - lastReport).TotalMilliseconds > 200)
                                    {
                                        if (totalLength.HasValue && totalLength.Value > 0)
                                        {
                                            double percent = ((double)totalRead / totalLength.Value) * 100.0;
                                            progress.Report(Math.Min(100.0, percent));
                                        }
                                        lastReport = DateTime.UtcNow;
                                    }
                                }
                                await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
                            }
                        }
                    }

                    // At this point download finished successfully; move temp to destination atomically
                    File.Move(temp, destination, overwrite: true);
                    progress?.Report(100.0);
                }
                finally
                {
                    // Release lock by disposing lockFs, then remove lock file
                    lockFs.Close();
                    try { File.Delete(lockFile); } catch { /* best-effort */ }
                }
            }
        }

        /// <summary>
        /// Download with expected SHA256 checksum verification.
        /// Throws InvalidOperationException if checksum mismatch.
        /// </summary>
        public async Task DownloadFileWithChecksumAsync(string url, string destination, string expectedSha256Hex, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            await DownloadFileAsync(url, destination, progress, cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(expectedSha256Hex))
            {
                using (var sha = System.Security.Cryptography.SHA256.Create())
                using (var fs = new FileStream(destination, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var hash = sha.ComputeHash(fs);
                    var actual = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    if (!string.Equals(actual, expectedSha256Hex.Replace("-", "").ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException($"Checksum mismatch for {destination}. Expected {expectedSha256Hex}, got {actual}");
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _http?.Dispose();
            _disposed = true;
        }
    }
}
