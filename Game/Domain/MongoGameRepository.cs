using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
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
            return FindById(game.Id);
        }

        public GameEntity FindById(Guid gameId)
        {
            return gameCollection.Find(gameCollection => gameCollection.Id == gameId).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            if (FindById(game.Id) == null)
            {
                Insert(game);
            }
            else
            {
                gameCollection.ReplaceOne(gameCollection => gameCollection.Id == game.Id, game);
            }
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection
                .Find(g => g.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var foundGame = FindById(game.Id);
            if (foundGame is not { Status: GameStatus.WaitingToStart }) return false;
            Update(game);
            return true;
        }
    }
}