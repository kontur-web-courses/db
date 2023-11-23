using System;
using MongoDB.Driver;

namespace Tests
{
    public static class TestMongoDatabase
    {
        public static IMongoDatabase Create()
        {
            var mongoConnectionString = Environment.GetEnvironmentVariable("PROJECT5100_MONGO_CONNECTION_STRING")
                                        ?? "mongodb://myuser:mypassword@localhost:27017/?authSource=db";
            var mongoClient = new MongoClient(mongoConnectionString);
            return mongoClient.GetDatabase("db");
        }
    }
}