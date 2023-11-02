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

        public GameEntity FindById(Guid gameId) => gameCollection.Find(game => game.Id == gameId).FirstOrDefault();

        public void Update(GameEntity game) => gameCollection.ReplaceOne(gameEntity => gameEntity.Id == game.Id, game);

        public IList<GameEntity> FindWaitingToStart(int limit) =>
            gameCollection.Find(game => game.Status == GameStatus.WaitingToStart).Limit(limit).ToList();

        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var result = gameCollection
                .ReplaceOne(gameEntity => gameEntity.Id == game.Id && gameEntity.Status == GameStatus.WaitingToStart, game);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}