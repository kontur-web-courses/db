using System;
using MongoDB.Driver;

namespace Tests
{
    public static class TestMongoDatabase
    {
        public static IMongoDatabase Create()
        {
            var mongoConnectionString = "mongodb://localhost:27017"
                                        ?? "mongodb+srv://admin:R4hFG5ckl3TcaT3E@backendfiitdb.ewnbiqr.mongodb.net/";
            var mongoClient = new MongoClient(mongoConnectionString);
            return mongoClient.GetDatabase("game-tests");
        }
    }
}