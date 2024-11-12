using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            gameCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;        }

        public GameEntity FindById(Guid gameId)
        {
            return gameCollection.Find(game => game.Id == gameId).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            var result = gameCollection.ReplaceOne(g => g.Id == game.Id, game);
            if (!result.IsAcknowledged)
            {
                throw new Exception("Update operation was not acknowledged.");
            }        
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            //TODO: Используй Find и Limit
            return gameCollection.Find(game => game.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            //TODO: Для проверки успешности используй IsAcknowledged и ModifiedCount из результата
            var result = gameCollection.UpdateOne(
                g => g.Id == game.Id && g.Status == GameStatus.WaitingToStart,
                Builders<GameEntity>.Update.Set(g => g.Status, game.Status)
            );

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}