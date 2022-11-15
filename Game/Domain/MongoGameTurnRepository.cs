using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;
        public const string CollectionName = "turns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            gameTurnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
        }

        public void Insert(GameTurnEntity gameTurn)
        {
            gameTurnCollection.InsertOne(gameTurn);
        }

        public List<GameTurnEntity> GetTurns(Guid gameId)
        {
            return gameTurnCollection.Find(t => t.GameId == gameId).SortByDescending(t => t.TurnNumber).Limit(5).ToList();
        }
    }
}