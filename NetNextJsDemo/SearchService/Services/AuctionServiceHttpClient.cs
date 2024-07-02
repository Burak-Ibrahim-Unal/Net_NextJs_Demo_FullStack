using System.Globalization;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services
{
    public class AuctionServiceHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        public async Task<List<Item>> GetItemsForSearchDn()
        {
            var lastUpdated = await DB.Find<Item, string>()
                .Sort(x => x.Descending(x => x.UpdatedAt))
                .Project(x => x.UpdatedAt.ToString(CultureInfo.CreateSpecificCulture("tr-TR")))
                .ExecuteFirstAsync();

            return (await httpClient.GetFromJsonAsync<List<Item>>(configuration["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated))!;
        }
    }
}
