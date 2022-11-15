using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db) => gameCollection = db.GetCollection<GameEntity>(CollectionName);

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId) => gameCollection.Find(g => g.Id == gameId).FirstOrDefault();

        public void Update(GameEntity game) => gameCollection.ReplaceOne(g => g.Id == game.Id, game);

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit) =>
            gameCollection.Find(g => g.Status == GameStatus.WaitingToStart).Limit(limit).ToList();

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            if (gameCollection.Find(g => g.Id == game.Id).FirstOrDefault() is var a &&
                a?.Status is not GameStatus.WaitingToStart or null)
                return false;

            var replaceOneResult = gameCollection.ReplaceOne(g => g.Id == game.Id, game);

            return replaceOneResult.IsAcknowledged && replaceOneResult.ModifiedCount == 1;
        }
    }
}