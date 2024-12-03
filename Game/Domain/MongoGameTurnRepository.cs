using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;
        public const string CollectionName = "gameTurns";

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            gameTurnCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            gameTurnCollection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(
                Builders<GameTurnEntity>.IndexKeys.Ascending(x => x.GameId)
                    .Ascending(x => x.TurnIndex)));
        }

        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            gameTurnCollection.InsertOne(gameTurn);
            return gameTurn;
        }

        public List<GameTurnEntity> GetLastTurns(Guid gameId, int maxCountTurns)
        {
            var lastTurns = gameTurnCollection
                .Find(x => x.GameId == gameId)
                .SortByDescending(x => x.TurnIndex)
                .Limit(maxCountTurns)
                .ToList();
           
            lastTurns.Reverse();
            return lastTurns;
        }
    }
}