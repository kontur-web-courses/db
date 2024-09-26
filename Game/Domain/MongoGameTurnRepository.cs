using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        public const string CollectionName = "gameTurns";
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;
        public MongoGameTurnRepository(IMongoDatabase db)
        {
            gameTurnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
        }

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            gameTurnCollection.InsertOne(turn);
            return turn;
        }

        public List<GameTurnEntity> GetLastTurns(Guid gameId)
        {
            var filter = new BsonDocument("GameId", gameId);
            return gameTurnCollection.Find(filter)
                .SortByDescending(t => t.TurnNumber)
                .Limit(5)
                .ToList();
        }
    }
}