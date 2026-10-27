using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;
        public const string CollectionName = "game_turns";
        
        public MongoGameTurnRepository(IMongoDatabase database)
        {
            gameTurnCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            var indexModel =
                new CreateIndexModel<GameTurnEntity>(Builders<GameTurnEntity>.IndexKeys
                    .Ascending(x => x.Id.GameId)
                    .Ascending(x => x.Id.TurnIndex), new CreateIndexOptions { Unique = true }
                );
            gameTurnCollection.Indexes.CreateOne(indexModel);
        }
        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            gameTurnCollection.InsertOne(gameTurn);
            return gameTurn;
        }
        
        public List<GameTurnEntity> FindAllGamesById(Guid gameId, int count)
        {
            return gameTurnCollection
                .Find(x => x.Id.GameId == gameId)
                .SortByDescending(x => x.Id.TurnIndex)
                .Limit(count).
                ToList();
        }
    }
}