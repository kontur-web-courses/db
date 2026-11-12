using System;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Linq;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";
        private readonly IMongoCollection<GameEntity> gameCollection;

        public MongoGameRepository(IMongoDatabase db)
        {
            gameCollection = db.GetCollection<GameEntity>(CollectionName);
            var keys = Builders<GameEntity>.IndexKeys.Ascending(g => g.Status);
            gameCollection.Indexes.CreateOne(new CreateIndexModel<GameEntity>(keys));
        }

        public GameEntity Insert(GameEntity game)
        {
            if (game.Id != Guid.Empty)
                throw new InvalidOperationException();

            var id = Guid.NewGuid();
            var entity = new GameEntity(id, game.Status, game.TurnsCount,
                game.CurrentTurnIndex, game.Players.ToList());
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

        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection
                .Find(g => g.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
        }
        
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var filter = Builders<GameEntity>.Filter.And(
                Builders<GameEntity>.Filter.Eq(g => g.Id, game.Id),
                Builders<GameEntity>.Filter.Eq(g => g.Status, GameStatus.WaitingToStart));

            var result = gameCollection.ReplaceOne(filter, game);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}