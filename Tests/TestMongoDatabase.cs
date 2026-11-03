using System;
using MongoDB.Driver;

namespace Tests
{
    public static class TestMongoDatabase
    {
        public static IMongoDatabase Create()
        {
            var mongoConnectionString = Environment.GetEnvironmentVariable("PROJECT5100_MONGO_CONNECTION_STRING")
                                        ?? "mongodb://localhost:27017?maxConnecting=100";
            var settings = MongoClientSettings.FromConnectionString(mongoConnectionString);
            settings.WriteConcern = WriteConcern.Unacknowledged;
            var mongoClient = new MongoClient(settings);
            return mongoClient.GetDatabase("game-tests");
        }
    }
}
