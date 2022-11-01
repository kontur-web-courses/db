using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";
        private readonly IMongoCollection<GameEntity> gamesCollection;

        public MongoGameRepository(IMongoDatabase db)
        {
            gamesCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gamesCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var cursor = gamesCollection.FindSync(Builders<GameEntity>.Filter.Eq(x => x.Id, gameId));
            return cursor.SingleOrDefault();
        }

        public void Update(GameEntity game)
        {
            gamesCollection.ReplaceOne(Builders<GameEntity>.Filter.Eq(x => x.Id, game.Id), game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gamesCollection.FindSync(FilterDefinition<GameEntity>.Empty)
                .ToEnumerable()
                .Where(x => x.Status == GameStatus.WaitingToStart)
                .Take(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            if ((game.Status != GameStatus.WaitingToStart && game.Status != GameStatus.Playing) || game.Players.Count < 1)
                return false;
            var updated = new GameEntity(game.Id, GameStatus.Playing, game.TurnsCount, game.CurrentTurnIndex, game.Players.ToList());
            Update(updated);
            return true;
        }
    }
}