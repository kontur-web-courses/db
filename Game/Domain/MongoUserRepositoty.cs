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
            var indexModel = new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys.Ascending(x => x.Login), 
                new CreateIndexOptions { Unique = true });
            userCollection.Indexes.CreateOne(indexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var user = userCollection.Find(x => x.Id == id)
                                     .FirstOrDefault();
            return user;
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
            userCollection.ReplaceOne(x => x.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(x => x.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var entity = userCollection.Find(Builders<UserEntity>.Filter.Empty)
                .SortBy(x => x.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize);
            return new PageList<UserEntity>(entity.ToList(), userCollection.CountDocuments(Builders<UserEntity>.Filter.Empty), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}