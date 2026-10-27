using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        public const string CollectionName = "game-turns";
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            gameTurnCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            gameTurnCollection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(
                Builders<GameTurnEntity>.IndexKeys
                    .Ascending(gameTurn => gameTurn.GameId)
                    .Descending(gameTurn => gameTurn.TurnIndex),
                new CreateIndexOptions { Unique = true }));
        }

        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            gameTurnCollection.InsertOne(gameTurn);
            return gameTurn;
        }

        public IList<GameTurnEntity> FindLastByGameId(Guid gameId, int limit = 5)
        {
            return gameTurnCollection.Find(gameTurn => gameTurn.GameId == gameId)
                .SortByDescending(gameTurn => gameTurn.TurnIndex)
                .Limit(limit)
                .ToList();
        }
    }
}
