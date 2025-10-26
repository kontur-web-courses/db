using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private IMongoDatabase _db;
        private readonly IMongoCollection<GameTurnEntity> _collection;

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            _db = db;
            _collection = _db.GetCollection<GameTurnEntity>("GameTurn");
            var indexDef = Builders<GameTurnEntity>.IndexKeys.Descending(x => x.TurnNumber);
            _collection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(indexDef));
        }

        public void Insert(GameTurnEntity gameTurn)
        {
            _collection.InsertOne(gameTurn);
        }

        public IEnumerable<GameTurnEntity> GetLastTurns(Guid gameId)
        {
            const int lastTrunsNumber = 5;
            return _collection.AsQueryable()
                .Where(ent => ent.GameId == gameId)
                .OrderByDescending(ent => ent.TurnNumber)
                .Take(lastTrunsNumber)
                .AsEnumerable()
                .Reverse();
        }
    }
}