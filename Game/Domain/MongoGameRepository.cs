using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";
        private readonly IMongoCollection<GameEntity> dbCollection;

        public MongoGameRepository(IMongoDatabase db)
        {
            dbCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            dbCollection.InsertOne(game);

            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            return dbCollection
                .Find(gameEntity => gameEntity.Id == gameId)
                .FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            dbCollection.ReplaceOne(gameEntity => gameEntity.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return dbCollection
                .Find(gameEntity => gameEntity.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var replacementResult = dbCollection.ReplaceOne(gameEntity => 
                    gameEntity.Id == game.Id && 
                    gameEntity.Status == GameStatus.WaitingToStart, game);
            return replacementResult.IsAcknowledged && replacementResult.ModifiedCount > 0;
        }
    }
}