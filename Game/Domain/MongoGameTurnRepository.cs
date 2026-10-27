using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> turnCollection;
        private const string CollectionName = "gameTurns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            turnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            turnCollection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(
                Builders<GameTurnEntity>.IndexKeys
                    .Ascending(t => t.GameId)
                    .Descending(t => t.TurnIndex)));
        }

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            turnCollection.InsertOne(turn);
            return turn;
        }

        public IList<GameTurnEntity> GetLastTurns(Guid gameId, int count)
        {
            var turns = turnCollection.Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnIndex)
                .Limit(count)
                .ToList();
            turns.Reverse();
            return turns;
        }
    }
}