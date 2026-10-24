using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";

        private readonly IMongoCollection<GameEntity> gameCollection;

        public MongoGameRepository(IMongoDatabase db) => gameCollection = db.GetCollection<GameEntity>(CollectionName);

        public GameEntity Insert(GameEntity game)
        {
            var entity = new GameEntity(game.Id, game.Status, game.TurnsCount, game.CurrentTurnIndex, game.Players.ToList());
            gameCollection.InsertOne(entity);
            return entity;
        }

        public GameEntity FindById(Guid gameId) => gameCollection.Find(g => g.Id == gameId).FirstOrDefault();

        public void Update(GameEntity game) => gameCollection.ReplaceOne(g => g.Id == game.Id, game);

        public IList<GameEntity> FindWaitingToStart(int limit) => gameCollection
            .Find(g => g.Status == GameStatus.WaitingToStart)
            .Limit(limit)
            .ToList();

        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var filter = Builders<GameEntity>.Filter.And(
                Builders<GameEntity>.Filter.Eq(g => g.Id, game.Id), 
                Builders<GameEntity>.Filter.Eq(g => g.Status, GameStatus.WaitingToStart));
            var result = gameCollection.ReplaceOne(filter, game);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}