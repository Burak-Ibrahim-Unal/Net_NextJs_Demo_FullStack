
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using System.Text.Json;

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

            if (count == 0)
            {
                Console.WriteLine("No Search Data...");

                var itemData = await File.ReadAllTextAsync("Data/auctions.json");
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

                await DB.SaveAsync(items);
            }
        }
    }
}
