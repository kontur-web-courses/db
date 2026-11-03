using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        public const string CollectionName = "game-turns";
        private readonly IMongoCollection<GameTurnEntity> collection;

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            collection = database.GetCollection<GameTurnEntity>(CollectionName);
            collection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(Builders<GameTurnEntity>.IndexKeys.Ascending(t => t.GameId).Descending(t => t.TurnNumber)));
        }

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            collection.InsertOne(turn);
            return turn;
        }

        public IList<GameTurnEntity> GetLast(Guid gameId, int limit)
        {
            return collection.Find(t => t.GameId == gameId).SortByDescending(t => t.TurnNumber).Limit(limit).ToList();
        }
    }
}
