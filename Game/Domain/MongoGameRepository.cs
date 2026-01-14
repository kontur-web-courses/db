using System;
using System.Collections.Generic;
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
            if (game.Id != Guid.Empty)
                throw new Exception();

            game = new GameEntity(
                id: Guid.NewGuid(),
                status: game.Status,
                turnsCount: game.TurnsCount,
                currentTurnIndex: game.CurrentTurnIndex,
                players: new List<Player>(game.Players)
            );
            gameCollection.InsertOne(game);

            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var game = gameCollection.Find(entity => entity.Id == gameId).FirstOrDefault();
            return game;
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(entity => entity.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            var games = gameCollection.Find(entity => entity.Status == GameStatus.WaitingToStart).Limit(limit).ToList();
            return games;
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var g = FindById(game.Id);
            if (g is not { Status: GameStatus.WaitingToStart })
                return false;
            Update(game);
            return true;
            //TODO: Для проверки успешности используй IsAcknowledged и ModifiedCount из результата
        }
    }
}