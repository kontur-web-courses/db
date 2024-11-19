using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> userCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            userCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            userCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var game = userCollection.FindSync(game => game.Id == gameId).FirstOrDefault();
            return game;
        }

        public void Update(GameEntity game)
        {
            var filter = new BsonDocument("_id", game.Id);
            userCollection.ReplaceOne(filter, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return userCollection
                .Find(new BsonDocument("Status", (int)GameStatus.WaitingToStart))
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var filter = new BsonDocument("_id", game.Id).Add(new BsonDocument("Status", (int)GameStatus.WaitingToStart));
            var result = userCollection.ReplaceOne(filter, game);
            return result.IsAcknowledged && result.MatchedCount != 0;
        }
    }
}