using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";
        private readonly IMongoCollection<GameEntity> collection;

        public MongoGameRepository(IMongoDatabase db)
        {
            collection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            collection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            return collection
                .Find(game => game.Id == gameId)
                .SingleOrDefault();
        }

        public void Update(GameEntity game)
        {
            collection.ReplaceOne(gameEntity => gameEntity.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return collection
                .Find(game => game.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var result = collection.ReplaceOne(gameEntity => gameEntity.Id == game.Id && gameEntity.Status == GameStatus.WaitingToStart, game);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}