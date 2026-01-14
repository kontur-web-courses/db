using System;
using MongoDB.Driver;

namespace ConsoleApp
{
    public static class MongoDatabase
    {
        public static IMongoDatabase Create()
        {
            var mongoConnectionString = Environment.GetEnvironmentVariable("PROJECT5100_MONGO_CONNECTION_STRING")
                                        ?? "mongodb://localhost:27017?maxConnecting=100";
            var mongoClient = new MongoClient(mongoConnectionString);
            return mongoClient.GetDatabase("rock-scissors-paper");
        }
    }
}