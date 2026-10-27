using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> turnCollection;
        public const string CollectionName = "turns";

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            turnCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            var index = Builders<GameTurnEntity>
                .IndexKeys
                .Ascending(t => t.GameId)
                .Descending(t => t.TurnNumber);
            turnCollection.Indexes.CreateOne(new  CreateIndexModel<GameTurnEntity>(index));
        }

        public void Insert(GameTurnEntity turn)
        {
            turnCollection.InsertOne(turn);
        }

        public IList<GameTurnEntity> FindLastTurns(Guid gameId, int limit)
        {
            return turnCollection
                .Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnNumber)
                .Limit(limit)
                .ToList();
        }
    }
}