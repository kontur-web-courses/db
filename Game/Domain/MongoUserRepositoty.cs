using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database, bool dropCollection = true)
        {
            if (dropCollection)
                database.DropCollection(CollectionName);

            userCollection = database.GetCollection<UserEntity>(CollectionName);
            userCollection.Indexes.CreateOne(Builders<UserEntity>.IndexKeys.Ascending(e => e.Login), new CreateIndexOptions { Unique = true });
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var filter = new BsonDocument("_id", id);
            return userCollection.Find(filter).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var filter = new BsonDocument("Login", login);
            var user = userCollection.Find(filter).FirstOrDefault();
            if (user is null)
            {
                user = new UserEntity { Login = login };
                try
                {
                    userCollection.InsertOne(user);
                }
                catch (MongoWriteException)
                {
                    user = userCollection.Find(filter).FirstOrDefault();
                }
            }

            return user;
        }

        public void Update(UserEntity user)
        {
            var filter = new BsonDocument("_id", user.Id);
            userCollection.ReplaceOne(filter, user);
        }

        public void Delete(Guid id)
        {
            var filter = new BsonDocument("_id", id);
            userCollection.DeleteOne(filter);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var findResult = userCollection.Find(FilterDefinition<UserEntity>.Empty)
                .SortBy(entity => entity.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize);
            return new PageList<UserEntity>(
                findResult.ToList(),
                userCollection.CountDocuments(FilterDefinition<UserEntity>.Empty),
                pageNumber,
                pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}