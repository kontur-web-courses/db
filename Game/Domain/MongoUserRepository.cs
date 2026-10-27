using System;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);

            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(
                Builders<UserEntity>.IndexKeys.Ascending(u => u.Login),
                new CreateIndexOptions { Unique = true }));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);

            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var user = userCollection.Find(user => user.Id == id)
                .FirstOrDefault();

            return user;
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection.Find(u => u.Login == login).FirstOrDefault();
            if (user != null)
                return user;

            user = new UserEntity { Login = login };
            try
            {
                userCollection.InsertOne(user);
                return user;
            }
            catch
            {
                return userCollection.Find(u => u.Login == login).First();
            }
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(user => user.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            //TODO: Тебе понадобятся SortBy, Skip и Limit
            var users = userCollection.Find(new BsonDocument())
                .SortBy(user => user.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            var totalCount = userCollection.CountDocuments(new BsonDocument());

            return new PageList<UserEntity>(users, totalCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}