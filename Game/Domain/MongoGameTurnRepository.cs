using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> collection;

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            collection = database.GetCollection<GameTurnEntity>("game_turns");
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var keys = Builders<GameTurnEntity>.IndexKeys
                .Ascending(x => x.GameId)
                .Descending(x => x.TurnIndex);
            collection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(keys));
        }

        public void SaveTurn(GameTurnEntity turn)
        {
            collection.InsertOne(turn);
        }

        public IReadOnlyList<GameTurnEntity> GetLastTurns(Guid gameId, int count)
        {
            return collection
                .Find(x => x.GameId == gameId)
                .SortByDescending(x => x.TurnIndex)
                .Limit(count)
                .ToList();
        }
    }
}