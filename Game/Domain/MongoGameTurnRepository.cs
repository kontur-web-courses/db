using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> turnsCollection;
        public const string CollectionName = "turns";

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            turnsCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            turnsCollection.Indexes.CreateOne(new CreateIndexModel<GameTurnEntity>(
                Builders<GameTurnEntity>.IndexKeys.Descending(turn => turn.TurnIndex),
                new CreateIndexOptions { Unique = true }));
        }


    }
}