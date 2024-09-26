using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            var options = new CreateIndexOptions { Unique = true };
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var indexModel = new CreateIndexModel<UserEntity>("{ Login : 1 }", options);
            userCollection.Indexes.CreateOne(indexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var filter = new BsonDocument("_id", id);
            var result = userCollection.Find(filter)
                .FirstOrDefault();
            return result;
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var filter = new BsonDocument("Login", login);
            var result = userCollection.Find(filter)
                .FirstOrDefault();
            if (result != null)
                return result;
            
            var user = new UserEntity(Guid.NewGuid(), login, "", "", 0, null);
            Insert(user);
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
            var filter = new BsonDocument();
            var users = userCollection.Find(filter)
                .SortBy(x => x.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
            var page = new PageList<UserEntity>(users, userCollection.CountDocuments(filter), pageNumber, pageSize);
            return page;
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}