using System;
using MongoDB.Driver;

namespace Tests
{
    public static class TestMongoDatabase
    {
        public static IMongoDatabase Create()
        {
            // var mongoConnectionString = Environment.GetEnvironmentVariable("PROJECT5100_MONGO_CONNECTION_STRING")
                                        // ?? "mongodb://localhost:27017";
            var mongoConnectionString = "mongodb+srv://admin:admin@cluster0.cuqy0.mongodb.net/test?retryWrite=true&retryRead=true";                             
            var mongoClient = new MongoClient(mongoConnectionString);
            return mongoClient.GetDatabase("game-tests");
        }
    }
}