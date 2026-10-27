using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";
        private readonly IMongoCollection<GameEntity> _collection;

        public MongoGameRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            _collection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
            => _collection
                .Find(x => x.Id == gameId)
                .FirstOrDefault();


        public void Update(GameEntity game)
            => _collection
                .ReplaceOne(x => x.Id == game.Id, game);


        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
            => _collection
                .Find(x => x.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var entity = _collection.Find(x => x.Id == game.Id).FirstOrDefault();
            if (entity is not { Status: GameStatus.WaitingToStart })
                return false;

            var result = _collection.ReplaceOne(x => x.Id == game.Id, game);
            return result.IsAcknowledged && result.ModifiedCount == 1;
        }
    }
}