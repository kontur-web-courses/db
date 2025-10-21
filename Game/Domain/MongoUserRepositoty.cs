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
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys.Ascending("Login"), options));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id) => 
            userCollection
            .Find(u => u.Id == id)
            .FirstOrDefault();

        public UserEntity GetOrCreateByLogin(string login)
        {
            var foundedUser = userCollection.Find(u => u.Login == login).FirstOrDefault();
            if (foundedUser is not null)
                return foundedUser;
            
            var user = new UserEntity(Guid.NewGuid()) { Login = login };
            userCollection.InsertOne(user);
            return user;
        }

        public void Update(UserEntity user) 
            => userCollection.ReplaceOne(u => u.Id == user.Id, user);

        public void Delete(Guid id) 
            => userCollection.DeleteOne(u => u.Id == id);

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var page = userCollection
                .Find(u => true)
                .SortBy(u => u.Login)
                .Skip(pageSize * (pageNumber - 1))
                .Limit(pageSize)
                .ToList();
            return new PageList<UserEntity>(page, userCollection.CountDocuments(u => true), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}