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
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys.Ascending(u => u.Login),
                new CreateIndexOptions { Unique = true }));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return FindById(user.Id);
        }

        public UserEntity FindById(Guid id) => userCollection.Find(user => user.Id == id).FirstOrDefault();

        public UserEntity GetOrCreateByLogin(string login) =>
            userCollection.Find(user => user.Login == login).FirstOrDefault() ?? Insert(new UserEntity {Login = login});

        public void Update(UserEntity user) => userCollection.ReplaceOne(userEntity => userEntity.Id == user.Id, user);

        public void Delete(Guid id) => userCollection.DeleteOne(user => user.Id == id);
        
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var items = userCollection
                .Find(user => true)
                .SortBy(user => user.Login)
                .Skip(pageSize * (pageNumber - 1))
                .Limit(pageSize)
                .ToList();

            var totalCount = userCollection.CountDocuments(d => true);
            return new PageList<UserEntity>(items, totalCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}