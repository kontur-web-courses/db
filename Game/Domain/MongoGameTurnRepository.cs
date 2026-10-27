using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        public const string CollectionName = "gameTurns";
        private readonly IMongoCollection<GameTurnEntity> collection;

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            collection = db.GetCollection<GameTurnEntity>(CollectionName);

            var indexKeys = Builders<GameTurnEntity>.IndexKeys
                .Ascending(t => t.GameId)
                .Descending(t => t.TurnIndex);
            collection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(indexKeys));
        }

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            if (turn.Id == Guid.Empty)
                turn = new GameTurnEntity(Guid.NewGuid(), turn.GameId, turn.TurnIndex, turn.WinnerId, new List<PlayerTurnDecision>(turn.Decisions));
            
            collection.InsertOne(turn);
            return turn;
        }

        public IReadOnlyList<GameTurnEntity> FindLastTurns(Guid gameId, int maxCount = 5)
        {
            return collection
                .Find(t => t.GameId == gameId)
                .SortByDescending(t => t.TurnIndex)
                .Limit(maxCount)
                .ToList();
        }
    }
}