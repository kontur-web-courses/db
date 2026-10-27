using System;
using MongoDB.Driver;

namespace ConsoleApp;

public class MongoDatabase
{
    public static IMongoDatabase Create()
    {
        var mongoConnectionString = "mongodb://localhost:27017?maxConnecting=100";
        var mongoClient = new MongoClient(mongoConnectionString);
        return mongoClient.GetDatabase("game");
    }
}