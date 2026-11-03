using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
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
            if (game.Id != Guid.Empty) throw new InvalidOperationException();
            var entity = new GameEntity(Guid.NewGuid(), game.Status, game.TurnsCount, game.CurrentTurnIndex, new List<Player>(game.Players));
            gameCollection.InsertOne(entity);
            return entity;
        }

        public GameEntity FindById(Guid gameId)
        {
            return gameCollection.Find(g => g.Id == gameId).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(g => g.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection.Find(g => g.Status == GameStatus.WaitingToStart).Limit(limit).ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var result = gameCollection.ReplaceOne(g => g.Id == game.Id && g.Status == GameStatus.WaitingToStart, game);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}
