using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly IMongoDatabase _database;

    public DataController()
    {
        var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") 
            ?? "mongodb://localhost:27017";
        var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME") 
            ?? "production_db";
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "API C# opérationnelle", database = "connected" });
    }

    [HttpGet("collections")]
    public IActionResult GetCollections()
    {
        var collections = _database.ListCollectionNames().ToList();
        return Ok(collections);
    }

    // TODO: Ajouter vos endpoints d'interaction avec la BDD ici
}
