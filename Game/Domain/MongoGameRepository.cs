using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db, bool dropCollection = true)
        {
            if (dropCollection)
                db.DropCollection(CollectionName);

            gameCollection = db.GetCollection<GameEntity>(CollectionName);
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
            var filter = Builders<GameEntity>.Filter.Eq(g => g.Status, GameStatus.WaitingToStart);
            return gameCollection.Find(filter)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var filterBuilder = Builders<GameEntity>.Filter;
            var filter = filterBuilder.Eq(g => g.Status, GameStatus.WaitingToStart) & filterBuilder.Eq(g => g.Id, game.Id);
            var result = gameCollection.ReplaceOne(filter, game);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}