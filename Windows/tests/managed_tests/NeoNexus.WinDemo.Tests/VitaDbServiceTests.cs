using System.Threading.Tasks;
using Xunit;
using NeoNexus.WinDemo.Services;

namespace NeoNexus.WinDemo.Tests
{
    public class VitaDbServiceTests
    {
        [Fact(Skip = "Integration test — network required")]
        public async Task FetchHomebrews_ReturnsItems()
        {
            var arr = await VitaDbService.FetchListAsync(VitaDbService.HomebrewsUrl);
            Assert.NotNull(arr);
        }
    }
}
