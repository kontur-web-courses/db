using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> turnCollection;
        public const string CollectionName = "gameTurns";

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            turnCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            
            // Create index on GameId and TurnIndex for fast queries of last turns
            var indexKeysDefinition = Builders<GameTurnEntity>.IndexKeys
                .Ascending(t => t.GameId)
                .Descending(t => t.TurnIndex);
            var indexModel = new CreateIndexModel<GameTurnEntity>(indexKeysDefinition);
            turnCollection.Indexes.CreateOne(indexModel);
        }

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            if (turn.Id == Guid.Empty)
            {
                turn = new GameTurnEntity(
                    Guid.NewGuid(),
                    turn.GameId,
                    turn.TurnIndex,
                    turn.Player1Id,
                    turn.Player1Decision,
                    turn.Player2Id,
                    turn.Player2Decision,
                    turn.WinnerId,
                    turn.Timestamp
                );
            }
            
            turnCollection.InsertOne(turn);
            return turn;
        }

        public IList<GameTurnEntity> GetLastTurns(Guid gameId, int count)
        {
            return turnCollection
                .Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnIndex)
                .Limit(count)
                .ToList();
        }
    }
}