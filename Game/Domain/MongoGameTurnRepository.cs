using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameTurnsCollection;
        private const string CollectionName = "gameTurns";

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            gameTurnsCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            gameTurnsCollection.Indexes.CreateOne(
                new CreateIndexModel<GameTurnEntity>(
                    Builders<GameTurnEntity>.IndexKeys.Descending(x => x.Timestamp)));
        }
        public GameTurnEntity Insert(GameTurnEntity gameTurnEntity)
        {
            gameTurnsCollection.InsertOne(gameTurnEntity);
            return gameTurnEntity;
        }

        public IReadOnlyCollection<GameTurnEntity> FindLatest(int limit = 5)
        {
            return gameTurnsCollection
                .Find(FilterDefinition<GameTurnEntity>.Empty)
                .SortByDescending(x => x.Timestamp)
                .Limit(limit)
                .ToList();
        }
    }
}