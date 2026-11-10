using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> turnCollection;
        public const string CollectionName = "gameTurns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            turnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            turnCollection.Indexes.CreateOne(
                new CreateIndexModel<GameTurnEntity>(
                    Builders<GameTurnEntity>.IndexKeys
                        .Ascending(t => t.GameId)
                        .Descending(t => t.TurnIndex)));
        }

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            turnCollection.InsertOne(turn);
            return turn;
        }

        public IList<GameTurnEntity> FindLatestByGameId(Guid gameId, int limit)
        {
            return turnCollection
                .Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnIndex)
                .Limit(limit)
                .ToList();
        }
    }
}
