using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> _collection;

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<GameTurnEntity>("game-turns");
            var indexKeys = Builders<GameTurnEntity>.IndexKeys.Ascending(t => t.GameId).Ascending(t => t.TurnNo);
            _collection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(indexKeys));
        }

        public void Insert(GameTurnEntity turn)
        {
            _collection.InsertOne(turn);
        }

        public List<GameTurnEntity> GetLastTurns(Guid gameId, int count)
        {
            var result = _collection
                .Find(turn => turn.GameId == gameId)
                .SortByDescending(turn => turn.TurnNo)
                .Limit(count)
                .ToList();
            result.Reverse();
            return result;
        }
    }
}