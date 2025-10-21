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
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys.Ascending(u => u.Login),
                new() { Unique = true }));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
            => userCollection.Find(u => u.Id == id).SingleOrDefault();

        public UserEntity GetOrCreateByLogin(string login) =>
            userCollection.FindOneAndUpdate(u => u.Login == login,
                Builders<UserEntity>.Update
                    .SetOnInsert(u => u.Id, Guid.NewGuid())
                    .SetOnInsert(u => u.Login, login)
                , new()
                {
                    ReturnDocument = ReturnDocument.After,
                    IsUpsert = true,
                });

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(u=> u.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(u => u.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var users = userCollection.Find(FilterDefinition<UserEntity>.Empty)
                .SortBy(u => u.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize).ToList();
            var totalCount = userCollection.CountDocuments(FilterDefinition<UserEntity>.Empty);
            return new(users, totalCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}