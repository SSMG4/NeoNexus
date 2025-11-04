#pragma once
#include <vector>
#include <string>
#include <cstdint>

struct PkgEntry {
    std::string path;
    uint64_t size;
};

/// ParsePkgFile tries to read package file metadata and list entries.
/// Current implementation:
/// - If the file has a .zip or .vpk extension, it attempts a simple ZIP central-directory parse (best-effort).
/// - Otherwise returns an empty vector (caller should handle that gracefully).
std::vector<PkgEntry> ParsePkgFile(const char* path);
