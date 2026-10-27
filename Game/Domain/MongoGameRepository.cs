using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
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
            return collection.Find(game => game.Id == gameId).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            collection.ReplaceOne(item => item.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            //TODO: Используй Find и Limit
            return collection
                .Find(game => game.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            //TODO: Для проверки успешности используй IsAcknowledged и ModifiedCount из результата
            var result = collection.ReplaceOne(item => item.Id == game.Id && 
                                                       item.Status == GameStatus.WaitingToStart, game);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}