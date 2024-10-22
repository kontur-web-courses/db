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
            
            var indexKeys = Builders<GameEntity>.IndexKeys.Ascending(g => g.Id);
            var indexOptions = new CreateIndexOptions();
            var indexModel = new CreateIndexModel<GameEntity>(indexKeys, indexOptions);

            gameCollection.Indexes.CreateOne(indexModel);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var filter = Builders<GameEntity>.Filter.Eq(g => g.Id, gameId);
            return gameCollection.Find(filter).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            var filter = Builders<GameEntity>.Filter.Eq(g => g.Id, game.Id);
            gameCollection.ReplaceOne(filter, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            //TODO: Используй Find и Limit
            var filter = Builders<GameEntity>.Filter.Where(g => g.Status == GameStatus.WaitingToStart);
            return gameCollection.Find(filter).Limit(limit).ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            
            var filter = Builders<GameEntity>.Filter.Eq(g => g.Id, game.Id);
            var result = gameCollection.Find(filter).FirstOrDefault();
            if (result?.Status == GameStatus.WaitingToStart)
            {
                var update = gameCollection.ReplaceOne(filter, game);
                return update.IsAcknowledged && update.ModifiedCount > 0;
            }
            
            return false;
        }
    }
}