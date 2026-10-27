using System;

using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var indexKeys = Builders<UserEntity>.IndexKeys.Ascending(user => user.Login);
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(indexKeys, new CreateIndexOptions {Unique = true}));
        }

        public UserEntity Insert(UserEntity user)
        {
            var entity = new UserEntity(Guid.NewGuid(), user.Login, user.LastName, user.FirstName, user.GamesPlayed, user.CurrentGameId);
            userCollection.InsertOne(entity);
            return entity;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(user => user.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var update = Builders<UserEntity>.Update.SetOnInsert(user => user.Id, Guid.NewGuid());
            var options = new FindOneAndUpdateOptions<UserEntity>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            return userCollection.FindOneAndUpdate<UserEntity>(user => user.Login == login, update, options);
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(entity => entity.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(user => user.Id == id);
        }

        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var totalCount = userCollection.CountDocuments(FilterDefinition<UserEntity>.Empty);
            var items = userCollection.Find(FilterDefinition<UserEntity>.Empty)
                .SortBy(user => user.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
            return new PageList<UserEntity>(items, totalCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}
