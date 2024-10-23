using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private const string CollectionName = "turns";
        private readonly IMongoCollection<GameTurnEntity> dbCollection;

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            dbCollection.Indexes
                .CreateOne(new CreateIndexModel<GameTurnEntity>(
                    Builders<GameTurnEntity>
                        .IndexKeys
                        .Ascending(gameTurnEntity => gameTurnEntity.GameId)
                        .Ascending(gameTurnEntity => gameTurnEntity.TurnIndex))
            );
        }

        public IReadOnlyList<GameTurnEntity> FindLastTurns(Guid gameId, int maxCount)
        {
            var findResult = dbCollection
                .Find(gameTurnEntity => gameTurnEntity.GameId == gameId)
                .SortByDescending(gameTurnEntity => gameTurnEntity.TurnIndex)
                .Limit(maxCount)
                .ToList();
            
            findResult.Reverse();
            return findResult;
        }

        public GameTurnEntity Insert(GameTurnEntity gameTurnEntity)
        {
            dbCollection.InsertOne(gameTurnEntity);
            
            return gameTurnEntity;
        }
    }
}