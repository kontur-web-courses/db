using System;
using MongoDB.Driver;

namespace Tests
{
    public static class TestMongoDatabase
    {
        public static IMongoDatabase Create()
        {
            var mongoConnectionString = "mongodb+srv://cmbbigis:8UnHIHU9gzDY5Byz@kontur-backend-course.ijouauu.mongodb.net/";
            var mongoClient = new MongoClient(mongoConnectionString);
            return mongoClient.GetDatabase("game-tests");
        }
    }
}