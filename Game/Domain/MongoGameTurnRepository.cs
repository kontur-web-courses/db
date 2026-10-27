using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> turnCollection;
        public const string CollectionName = "games-turn";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            turnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            var indexModel = new CreateIndexModel<GameTurnEntity>(Builders<GameTurnEntity>.IndexKeys.Ascending(t => t.GameId).Descending(t => t.TurnNumber));
            turnCollection.Indexes.CreateOne(indexModel);
        }
    
        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            turnCollection.InsertOne(gameTurn);
            return gameTurn;
        }

        public IList<GameTurnEntity> GetLastTurns(int limit, Guid gameId)
        {
            return turnCollection
                .Find(g => g.GameId == gameId)
                .SortByDescending(u => u.TurnNumber)
                .Limit(limit)
                .ToList();
        }
        
    }
}