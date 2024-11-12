using Game.Domain;
using MongoDB.Driver;

namespace ConsoleApp;

public static class MongoDbExtensions
{
    public static IGameRepository GetGameRepository(this IMongoDatabase mongoDatabase)
    {
        mongoDatabase.DropCollection(MongoGameRepository.CollectionName);
        return new MongoGameRepository(mongoDatabase);
    }
    
    public static IUserRepository GetUserRepository(this IMongoDatabase mongoDatabase)
    {
        mongoDatabase.DropCollection(MongoUserRepository.CollectionName);
        return new MongoUserRepository(mongoDatabase);
    }
}