using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain;

public class MongoGameTurnRepository : IGameTurnRepository
{
    public const string CollectionName = "game-turns";
    private readonly IMongoCollection<GameTurnEntity> turnCollection;

    public MongoGameTurnRepository(IMongoDatabase database)
    {
        turnCollection = database.GetCollection<GameTurnEntity>(CollectionName);
        turnCollection.Indexes.CreateOne(
            new CreateIndexModel<GameTurnEntity>(Builders<GameTurnEntity>.IndexKeys.Hashed("GameId").Descending("Index")));
    }

    public GameTurnEntity Insert(GameTurnEntity turn)
    {
        turnCollection.InsertOne(turn);
        return turn;
    }

    public IList<GameTurnEntity> FindLastTurns(Guid gameId, int limit)
    {
        return turnCollection.Find(turn => turn.GameId == gameId).SortByDescending(turn => turn.Index).Limit(limit).ToList();
    }
}