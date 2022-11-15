using System;
using System.Linq;
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
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            
            var options = new CreateIndexOptions { Unique = true };
            userCollection.Indexes.CreateOne(new BsonDocument("Login", 1), options);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var user = userCollection.FindSync(user => user.Id == id).FirstOrDefault();
            return user;
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection.FindSync(user => user.Login.Equals(login)).FirstOrDefault();
            if (user == null)
                try
                {
                    user = Insert(new UserEntity {Login = login});
                }
                catch (MongoWriteException e) when (e.WriteError.Code == 11000)
                {
                    return userCollection.FindSync(u => u.Login == login).First();
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
            userCollection.DeleteOne(user => user.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var users = userCollection
                .Aggregate()
                .Sort(new BsonDocument("Login", 1))
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            var count = userCollection.CountDocuments(new BsonDocument());

            return new PageList<UserEntity>(users, count, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}