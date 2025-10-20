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
            return userCollection.Find(x => x.Id == id).FirstOrDefault();
        }

        
        public UserEntity GetOrCreateByLogin(string login)
        {
            var filter = Builders<UserEntity>.Filter.Eq(x => x.Login, login);
            var update = Builders<UserEntity>.Update
                .SetOnInsert(x => x.Id, Guid.NewGuid())
                .SetOnInsert(x => x.Login, login);
    
            var options = new FindOneAndUpdateOptions<UserEntity>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
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
            var page = userCollection
                .Find(x => true)
                .SortBy(x => x.Login)
                .Skip(pageSize * (pageNumber - 1))
                .Limit(pageSize)
                .ToList();
            return new PageList<UserEntity>(page, userCollection.CountDocuments(x => true), pageNumber,  pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}