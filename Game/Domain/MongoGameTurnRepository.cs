using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> collection;
        private const string CollectionName = "turns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            collection = db.GetCollection<GameTurnEntity>(CollectionName);
            collection.Indexes.CreateOne(
                new CreateIndexModel<GameTurnEntity>(
                    new IndexKeysDefinitionBuilder<GameTurnEntity>()
                    .Ascending(document => document.GameId)
                    .Descending(document => document.Index))
                );
        }
        
        public GameTurnEntity FindById(Guid gameId)
        {
            return collection.FindSync(x => x.Id == gameId).FirstOrDefault();
        }

        public List<GameTurnEntity> FindByGameId(Guid gameId, int count)
        {
            return collection.FindSync(turn => turn.GameId == gameId, new FindOptions<GameTurnEntity>()
            {
                Sort = new SortDefinitionBuilder<GameTurnEntity>().Descending(t => t.Index),
                Limit = count
            }).ToList();
        }

        public GameTurnEntity Insert(GameTurnEntity game)
        {
            collection.InsertOne(game);
            return FindById(game.Id);
        }
    }
}