using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private IMongoCollection<GameTurnEntity> turnsCollection;
        public const string CollectionName = "turns";

        public MongoGameTurnRepository(IMongoDatabase db, bool dropCollection = true)
        {
            if (dropCollection)
                db.DropCollection(CollectionName);

            turnsCollection = db.GetCollection<GameTurnEntity>(CollectionName);

            var indexBuilder = Builders<GameTurnEntity>.IndexKeys.Ascending(entity => entity.GameId);
            var createIndexModel = new CreateIndexModel<GameTurnEntity>(indexBuilder);
            turnsCollection.Indexes.CreateOne(createIndexModel);
            indexBuilder = Builders<GameTurnEntity>.IndexKeys.Descending(entity => entity.TurnIndex);
            createIndexModel = new CreateIndexModel<GameTurnEntity>(indexBuilder);
            turnsCollection.Indexes.CreateOne(createIndexModel);
        }


        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            turnsCollection.InsertOne(turn);
            return turn;
        }

        public IList<GameTurnEntity> FindLastTurns(Guid gameId, int turnsCount = 5)
        {
            var filterBuilder = Builders<GameTurnEntity>.Filter;
            var filter = filterBuilder.Eq(entity => entity.GameId, gameId);
            var lastTurns = turnsCollection.Find(filter).SortByDescending(t => t.TurnIndex).Limit(turnsCount).ToList();
            lastTurns.Reverse();
            return lastTurns;
        }
    }
}