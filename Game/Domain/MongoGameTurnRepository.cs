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
            var indexModel = new CreateIndexModel<GameTurnEntity>(
                Builders<GameTurnEntity>.IndexKeys
                    .Ascending(t => t.GameId)
                    .Descending(t => t.TurnIndex),
                new CreateIndexOptions { Unique = true, Name = "GameId_TurnIndex" });
            turnCollection.Indexes.CreateOne(indexModel);
        }

        public void Insert(GameTurnEntity turn) => turnCollection.InsertOne(turn);

        public List<GameTurnEntity> GetLastTurns(Guid gameId, int limit) => turnCollection
            .Find(t => t.GameId == gameId)
            .SortByDescending(t => t.TurnIndex)
            .Limit(limit)
            .ToList();
    }
}
