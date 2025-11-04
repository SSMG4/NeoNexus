using System.Collections.Generic;
using NeoNexus.WinDemo.Services;
using Xunit;

namespace NeoNexus.WinDemo.Tests
{
    public class CbpsCsvParserTests
    {
        [Fact]
        public void ParseCsv_SimpleSample_ParsesExpectedEntries()
        {
            // Small CSV sample, includes comma inside quoted field to ensure parser handles quoted commas
            var csv = "id,title,download_url,type,visible\n" +
                      "GCTK10000,GCToolKit,https://example.com/GcToolKit.vpk,VPK,True\n" +
                      "OHCL00001,\"Open, Hydra Castle Labyrinth\",https://example.com/OpenHCL_Vita.vpk,VPK,True\n";

            var list = CbpsService.ParseCsv(csv);
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);

            var first = list[0];
            Assert.Equal("GCTK10000", first.Id);
            Assert.Equal("GCToolKit", first.Title);
            Assert.Equal("VPK", first.Type);

            var second = list[1];
            Assert.Equal("OHCL00001", second.Id);
            Assert.Equal("Open, Hydra Castle Labyrinth", second.Title);
        }
    }
}
