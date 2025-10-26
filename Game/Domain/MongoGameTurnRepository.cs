using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> turnsCollection;
        public const string CollectionName = "gameTurns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            turnsCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            
            var indexKeysDefinition = Builders<GameTurnEntity>.IndexKeys
                .Ascending(t => t.GameId)
                .Descending(t => t.TurnIndex);
            var indexModel = new CreateIndexModel<GameTurnEntity>(indexKeysDefinition);
            turnsCollection.Indexes.CreateOne(indexModel);
        }

        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            turnsCollection.InsertOne(gameTurn);
            return gameTurn;
        }

        public List<GameTurnEntity> GetLastTurns(Guid gameId, int turnsCount)
        {
            return turnsCollection
                .Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnIndex)
                .Limit(turnsCount)
                .ToList();
        }

        public List<GameTurnEntity> FindByGameId(Guid gameId)
        {
            return turnsCollection
                .Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnIndex)
                .ToList();
        }
    }
}
