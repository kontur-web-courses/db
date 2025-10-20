using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;
        public const string CollectionName = "turns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            gameTurnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            var indexKeysDefinition = Builders<GameTurnEntity>.IndexKeys.Ascending(t => t.FinishedAt);
            gameTurnCollection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(indexKeysDefinition));
        }

        public GameTurnEntity Insert(GameTurnEntity game)
        {
            gameTurnCollection.InsertOne(game);
            return game;
        }

        public List<GameTurnEntity> GetLastTurns(int limit)
        {
            return gameTurnCollection
                .Find(_ => true)
                .SortByDescending(g => g.FinishedAt)
                .Limit(limit)
                .SortBy(g => g.FinishedAt)
                .ToList();
        }
    }
}