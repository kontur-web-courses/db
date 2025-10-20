using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var indexKeysDefinition = Builders<UserEntity>.IndexKeys.Ascending(u => u.Login);
            var indexOptions = new CreateIndexOptions { Unique = true };
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(indexKeysDefinition, indexOptions));
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
                .Find(u => u.Id == id)
                .FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            //TODO: Это Find или Insert
            var filter = Builders<UserEntity>.Filter.Eq(u => u.Login, login);
            var update = Builders<UserEntity>.Update
                .SetOnInsert(u => u.Id, Guid.NewGuid());

            var options = new FindOneAndUpdateOptions<UserEntity>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            return userCollection.FindOneAndUpdate(filter, update, options);
        }

        public void Update(UserEntity user)
        {
            //TODO: Ищи в документации ReplaceXXX
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
            //TODO: Тебе понадобятся SortBy, Skip и Limit
            var totalCount = userCollection.CountDocuments(_ => true);
            var page = userCollection
                .Find(_ => true)
                .SortBy(u => u.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            return new PageList<UserEntity>(page, totalCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}