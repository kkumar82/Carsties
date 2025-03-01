using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;
namespace SearchService.Data;
public class DbInitializer 
{ 
    public static async Task InitDb(WebApplication app) 
    { 
        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));
        
        await DB.Index<Item>()
            .Key(x => x.Make, MongoDB.Entities.KeyType.Text)
            .Key(x => x.Model, MongoDB.Entities.KeyType.Text)
            .Key(x => x.Color, MongoDB.Entities.KeyType.Text)
            .CreateAsync(); 
        
        using var scope = app.Services.CreateScope(); 
        
        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>(); 
        
        var items = await httpClient.GetItemsForSearchDb(); 
        
        Console.WriteLine(items.Count + "returned from the auction service"); 
        
        if (items.Count > 0) await DB.SaveAsync(items); 
    } 
}