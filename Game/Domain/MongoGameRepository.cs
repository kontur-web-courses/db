using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";
        private readonly IMongoCollection<GameEntity> gameCollection;

        public MongoGameRepository(IMongoDatabase db)
        {
            gameCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return FindById(game.Id);
        }

        public GameEntity FindById(Guid gameId)
        {
            return gameCollection.Find(gameEntity => gameEntity.Id == gameId).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            var foundGame = FindById(game.Id);
            if (foundGame != null)
            {
                gameCollection.ReplaceOne(gameEntity => gameEntity.Id == game.Id, game);
            }
            else
            {
                Insert(game);
            }
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection
                .Find(gameEntity => gameEntity.Status == GameStatus.WaitingToStart)
                .SortBy(gameEntity => gameEntity.Id)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var foundGame = FindById(game.Id);
            if (foundGame != null && foundGame.Status == GameStatus.WaitingToStart)
            {
                Update(game);
                return true;
            }

            return false;
        }
    }
}