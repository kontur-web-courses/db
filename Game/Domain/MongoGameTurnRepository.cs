using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        public const string CollectionName = "gameTurns";
        private readonly IMongoCollection<GameTurnEntity> turnCollection;

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            turnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            var indexKeys = Builders<GameTurnEntity>.IndexKeys
                .Ascending(x => x.GameId)
                .Descending(x => x.TurnIndex);
            var indexModel = new CreateIndexModel<GameTurnEntity>(indexKeys);
            turnCollection.Indexes.CreateOne(indexModel);
        }

        public void Insert(GameTurnEntity turn)
        {
            turnCollection.InsertOne(turn);
        }

        public List<GameTurnEntity> GetLastTurns(Guid gameId, int limit = 5)
        {
            return turnCollection
                .Find(x => x.GameId == gameId)
                .SortByDescending(x => x.TurnIndex)
                .Limit(limit)
                .ToList();
        }
    }
}
