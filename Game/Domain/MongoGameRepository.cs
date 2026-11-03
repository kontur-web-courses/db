using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(GameEntity)))
            {
                BsonClassMap.RegisterClassMap<GameEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(g => g.Id);
                });
            }

            gameCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            return gameCollection.Find(g => g.Id == gameId).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(g => g.Id == game.Id, game);
        }

        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection.Find(g => g.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
        }

        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var filter = Builders<GameEntity>.Filter.Eq(g => g.Id, game.Id) 
                         & Builders<GameEntity>.Filter.Eq(g => g.Status, GameStatus.WaitingToStart);
            var result = gameCollection.FindOneAndReplace(filter, game);

            return result != null;
        }
    }
}
