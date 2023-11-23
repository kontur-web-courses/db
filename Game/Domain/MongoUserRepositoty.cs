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
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexKeys = Builders<UserEntity>.IndexKeys.Ascending(x => x.Login);
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(indexKeys, indexOptions));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(x => x.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection.Find(x => x.Login == login).FirstOrDefault();
            if (user != null) return user;
            user = new UserEntity
            {
                Login = login
            };
            userCollection.InsertOne(user);

            return user;
        }

        public void Update(UserEntity user)
        {
            userCollection
                .ReplaceOne(x => x.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(x => x.Id == id);
        }

        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var totalUsers = userCollection.CountDocuments(x => true);
            var totalPages = totalUsers % pageSize == 0
                ? totalUsers / pageSize
                : totalUsers / pageSize + 1;

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            else if (pageNumber > totalPages)
            {
                pageNumber = (int)totalPages;
            }

            var users = userCollection.Find(x => true)
                                      .SortBy(x => x.Login)
                                      .Skip((pageNumber - 1) * pageSize)
                                      .Limit(pageSize)
                                      .ToList();

            return new PageList<UserEntity>(users, totalUsers, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}