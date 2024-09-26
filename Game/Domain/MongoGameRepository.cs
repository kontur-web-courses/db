using System;
using System.Collections.Generic;
using MongoDB.Bson;
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
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var filter = new BsonDocument("_id", gameId);
            var result = gameCollection.Find(filter)
                .FirstOrDefault();
            return result;
        }

        public void Update(GameEntity game)
        {
            var filter = new BsonDocument("_id", game.Id);
            gameCollection.ReplaceOne(filter, game);
        }
        
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            var filter = new BsonDocument("Status", GameStatus.WaitingToStart);
            return gameCollection.Find(filter)
                .Limit(limit)
                .ToList();
        }
        
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var filter = new BsonDocument("_id", game.Id);
            var gameToUpdate = gameCollection.Find(filter).FirstOrDefault();
            if (gameToUpdate == null)
                return false;
            if (gameToUpdate.Status != GameStatus.WaitingToStart)
                return false;
            var result = gameCollection.ReplaceOne(filter, game);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}