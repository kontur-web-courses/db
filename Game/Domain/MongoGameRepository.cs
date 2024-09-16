using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> collection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            collection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            collection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            collection.ReplaceOne(x => x.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return collection.Find(x => x.Status == GameStatus.WaitingToStart).Limit(limit).ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            //TODO: Для проверки успешности используй IsAcknowledged и ModifiedCount из результата
            var replaceOneResult = collection.ReplaceOne(x => x.Id == game.Id && x.Status == GameStatus.WaitingToStart, game);
            return replaceOneResult.IsAcknowledged && replaceOneResult.ModifiedCount > 0;
        }
    }
}