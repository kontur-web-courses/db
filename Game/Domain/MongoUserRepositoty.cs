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
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var keys = Builders<UserEntity>.IndexKeys.Ascending(x => x.Login);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var model = new CreateIndexModel<UserEntity>(keys, indexOptions);
            userCollection.Indexes.CreateOne(model);
        }

        public UserEntity Insert(UserEntity user)
        {
            //TODO: Ищи в документации InsertXXX.
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            //TODO: Ищи в документации FindXXX
            return userCollection
                .Find(user => user.Id == id)
                .FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            //TODO: Это Find или Insert
            var user = userCollection
                .Find(user => user.Login == login)
                .FirstOrDefault();

            if (user != null)
                return user;

            user = new UserEntity(Guid.NewGuid()) { Login = login };
            return Insert(user);
        }

        public void Update(UserEntity user)
        {
            //TODO: Ищи в документации ReplaceXXX
            userCollection.ReplaceOne(x => x.Id == user.Id, user);
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
            var users = userCollection
                .Find(new BsonDocument())
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .SortBy(x => x.Login)
                .ToList();

            return new PageList<UserEntity>(users, userCollection.CountDocuments(new BsonDocument()), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}