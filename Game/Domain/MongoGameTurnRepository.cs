using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        public const string CollectionName = "gameTurns";
        public const string IndexName = "GameId_TurnIndex";

        private readonly IMongoCollection<GameTurnEntity> turnCollection;

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            turnCollection = db.GetCollection<GameTurnEntity>(CollectionName);

            var keys = Builders<GameTurnEntity>.IndexKeys
                .Ascending(t => t.GameId)
                .Descending(t => t.TurnIndex);
            turnCollection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(
                keys, new CreateIndexOptions { Name = IndexName, Unique = true }));
        }

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            turnCollection.InsertOne(turn);
            return turn;
        }

        public IReadOnlyList<GameTurnEntity> GetLastTurns(Guid gameId, int count)
        {
            return turnCollection
                .Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnIndex)
                .Limit(count)
                .ToList();
        }
    }
}