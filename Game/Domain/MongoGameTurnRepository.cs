using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;
        public const string CollectionName = "gameTurns";

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            gameTurnCollection.InsertOne(turn);
            return turn;
        }

        public List<GameTurnEntity> GetLastTurns(Guid gameId)
        {
            var filter = new BsonDocument("GameId", gameId);
            return gameTurnCollection.Find(filter)
                .SortByDescending(x => x.Turn)
                .Limit(5)
                .ToList();
        }

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            gameTurnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
        }
    }
}