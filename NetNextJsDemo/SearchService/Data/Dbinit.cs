
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using System.Text.Json;
using SearchService.Services;

namespace SearchService.Data
{
    public class Dbinit
    {
        public static async Task InitDb(WebApplication webApplication)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings
                .FromConnectionString(webApplication.Configuration.GetConnectionString("MongoDb")));

            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();

            await using var scope = webApplication.Services.CreateAsyncScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();
            var items = await httpClient.GetItemsForSearchDn();
            Console.WriteLine(items.Count + "returned from the Auction Service");

            if (items.Count > 0)
                await DB.SaveAsync(items);

            #region Test

            //if (count == 0)
            //{
            //    Console.WriteLine("No Search Data...");

            //    var itemData = await File.ReadAllTextAsync("Data/auctions.json");
            //    var options = new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true,
            //    };
            //    var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

            //    await DB.SaveAsync(items);
            //} 

            #endregion
        }
    }
}
