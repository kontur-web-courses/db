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
            var indexKeys = Builders<UserEntity>.IndexKeys.Ascending(u => u.Login);
            var indexModel = new CreateIndexModel<UserEntity>(indexKeys, indexOptions);
            userCollection.Indexes.CreateOne(indexModel);
        }


        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(u => u.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection.Find(u => u.Login == login).FirstOrDefault();
            if (user == null)
            {
                user = new UserEntity(Guid.NewGuid()) { Login = login };
                userCollection.InsertOne(user);
            }
            return user;
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(u => u.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var skipCount = (pageNumber - 1) * pageSize;
            var users = userCollection.Find(Builders<UserEntity>.Filter.Empty)
                .SortBy(u => u.Login)
                .Skip(skipCount)
                .Limit(pageSize)
                .ToList();
        
            var totalCount = userCollection.CountDocuments(Builders<UserEntity>.Filter.Empty);
            return new PageList<UserEntity>(users, totalCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}