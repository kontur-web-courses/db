using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain;

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
        var cursor = gameCollection.FindSync(game => game.Id == gameId);
        return cursor.FirstOrDefault();
    }

    public void Update(GameEntity game)
    {
        gameCollection.ReplaceOne(g => g.Id == game.Id, game);
    }

    // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
    public IList<GameEntity> FindWaitingToStart(int limit)
    {
        var find = gameCollection.Find(game => game.Status == GameStatus.WaitingToStart).Limit(limit);
        return find.ToList();
    }

    // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
    public bool TryUpdateWaitingToStart(GameEntity game)
    {
        var result = gameCollection.ReplaceOne(g => g.Id == game.Id && g.Status == GameStatus.WaitingToStart, game);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}