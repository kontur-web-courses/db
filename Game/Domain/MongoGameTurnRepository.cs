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

            // Этот составной индекс одновременно обеспечивает уникальность тура в игре
            // и позволяет быстро получить последние туры одной игры.
            var indexKeys = Builders<GameTurnEntity>.IndexKeys
                .Ascending(gameTurn => gameTurn.GameId)
                .Descending(gameTurn => gameTurn.TurnIndex);
            gameTurnCollection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(
                indexKeys,
                new CreateIndexOptions { Unique = true }));
        }

        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            if (gameTurn == null) throw new ArgumentNullException(nameof(gameTurn));

            gameTurnCollection.InsertOne(gameTurn);
            return gameTurn;
        }

        public IList<GameTurnEntity> FindLastByGameId(Guid gameId, int limit = 5)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));

            return gameTurnCollection
                .Find(gameTurn => gameTurn.GameId == gameId)
                .SortByDescending(gameTurn => gameTurn.TurnIndex)
                .Limit(limit)
                .ToList();
        }
    }
}
