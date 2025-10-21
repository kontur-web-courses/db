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

        public MongoGameRepository(IMongoDatabase database)
        {
            gameCollection = database.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
            => gameCollection.Find(u => u.Id == gameId).SingleOrDefault();

        public void Update(GameEntity game) => gameCollection.ReplaceOne(u=> u.Id == game.Id, game);

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit) =>
            gameCollection
                .Find(g => g.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var updated = gameCollection.ReplaceOne(
                g => g.Id == game.Id && g.Status == GameStatus.WaitingToStart,
                game);
            return updated.IsAcknowledged && updated.ModifiedCount > 0;
        }
    }
}