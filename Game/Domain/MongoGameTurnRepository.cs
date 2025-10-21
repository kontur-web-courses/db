using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;
        private const string CollectionName = "game_turns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            gameTurnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            gameTurnCollection.Indexes.CreateOne(
                new CreateIndexModel<GameTurnEntity>(
                    Builders<GameTurnEntity>.IndexKeys
                        .Ascending(t => t.GameId)
                        .Descending(t => t.TurnIndex)
                )
            );
        }
        public GameTurnEntity Insert(GameTurnEntity entity)
        {
            gameTurnCollection.InsertOne(entity);
            return entity;
        }

        public List<GameTurnEntity> GetLastTurns(Guid gameId, int maxCountTurns) =>
            gameTurnCollection.Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnIndex)
                .Limit(maxCountTurns)
                .ToList();
    }
}