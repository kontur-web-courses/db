using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;
        public const string CollectionName = "turns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            gameTurnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
            gameTurnCollection.Indexes.CreateOne("{gameId: 1, time: -1}");
        }
        
        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            //TODO: Ищи в документации InsertXXX.
            gameTurnCollection.InsertOne(gameTurn);
            return gameTurn;
        }
        
        public List<GameTurnEntity> FindLast(Guid gameId, int count)
        {
            //TODO: Ищи в документации FindXXX
            return gameTurnCollection
                .Find(gt => gt.gameId == gameId)
                .SortByDescending(g => g.time)
                .Limit(5).ToList();
        }
    }
}