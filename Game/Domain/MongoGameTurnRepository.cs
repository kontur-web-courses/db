using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain;

public class MongoGameTurnRepository : IGameTurnRepository
{
    private readonly IMongoCollection<GameTurnEntity> _collection;
    private const string CollectionName = "game-tours";

    public MongoGameTurnRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<GameTurnEntity>(CollectionName);
    }

    public GameTurnEntity Insert(GameTurnEntity tour)
    {
        _collection.InsertOne(tour);
        return tour;
    }

    public IList<GameTurnEntity> GetGameTours(Guid gameId)
        => _collection.Find(t => t.GameId == gameId).SortBy(t => t.TourIndex).ToList();
}