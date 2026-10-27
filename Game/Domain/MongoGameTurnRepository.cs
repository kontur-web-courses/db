using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private const string CollectionName = "gameTurns";
        private readonly IMongoCollection<GameTurnEntity> _collection;

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<GameTurnEntity>(CollectionName);

            _collection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(
                Builders<GameTurnEntity>.IndexKeys.Descending(gt => gt.FinishedAt)));
        }
        public List<GameTurnEntity> GetTurns(int count = 5)
        {
            var gameTurns = _collection.Find(new BsonDocument())
                .SortByDescending(x => x.FinishedAt)
                .Limit(count)
                .ToList();

            return gameTurns;
        }

        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            _collection.InsertOne(gameTurn);

            return gameTurn;
        }
    }
}