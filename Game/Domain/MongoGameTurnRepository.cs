using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> turnCollection;
        public const string CollectionName = "game-turns";

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            turnCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            var indexKey = Builders<GameTurnEntity>.IndexKeys
                .Ascending(x => x.GameId)
                .Descending(x => x.TurnNumber);
            turnCollection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(indexKey));
        }

        public void Insert(GameTurnEntity turn)
        {
            turnCollection.InsertOne(turn);
        }

        public IList<GameTurnEntity> FindLastTurns(Guid gameId, int limit)
        {
            return turnCollection.Find(x => x.GameId == gameId)
                .SortByDescending(x => x.TurnNumber)
                .Limit(limit)
                .ToList();
        }
    }
}
