using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> turnCollection;
        public const string CollectionName = "turns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            turnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            var indexKeys = Builders<GameTurnEntity>.IndexKeys
                .Ascending(t => t.GameId)
                .Descending(t => t.TurnIndex);
            
            turnCollection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(indexKeys));
        }
        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            turnCollection.InsertOne(gameTurn);
            return gameTurn;
        }

        public IReadOnlyList<GameTurnEntity> GetLastTurns(Guid gameId, int turnsCount)
        {
            return turnCollection
                .Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnIndex)
                .Limit(turnsCount)
                .ToList()
                .AsReadOnly();
        }
    }
}