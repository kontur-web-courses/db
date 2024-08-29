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
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            return gameCollection.Find(x => x.Id == gameId).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(x => x.Id == game.Id, game);
        }

        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection.Find(x => x.Status == GameStatus.WaitingToStart).Limit(limit).ToList();
        }

        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var g = gameCollection.Find(e => e.Id == game.Id).FirstOrDefault();
            if (g is not { Status: GameStatus.WaitingToStart }) 
                return false;
            {
                var result = gameCollection.ReplaceOne(e => e.Id == game.Id, game);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
        }
    }
}