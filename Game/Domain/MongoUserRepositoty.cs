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
            
            var indexKeysDefinition = Builders<UserEntity>.IndexKeys.Ascending(u => u.Login);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<UserEntity>(indexKeysDefinition, indexOptions);
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
            var filter = Builders<UserEntity>.Filter.Eq(u => u.Login, login);
            var update = Builders<UserEntity>.Update
                .SetOnInsert(u => u.Id, Guid.NewGuid())
                .SetOnInsert(u => u.Login, login)
                .SetOnInsert(u => u.LastName, "")
                .SetOnInsert(u => u.FirstName, "")
                .SetOnInsert(u => u.GamesPlayed, 0)
                .SetOnInsert(u => u.CurrentGameId, Guid.NewGuid());
            
            var options = new FindOneAndUpdateOptions<UserEntity>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            return userCollection.FindOneAndUpdate(filter, update, options);
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
            var totalCount = userCollection.CountDocuments(u => true);
            var users = userCollection
                .Find(u => true)
                .SortBy(u => u.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            return new PageList<UserEntity>(
                users, 
                (int)totalCount, 
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