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
            userCollection.Indexes.CreateOne(
                new CreateIndexModel<UserEntity>(
                    Builders<UserEntity>.IndexKeys.Ascending(user => user.Login),
                    new CreateIndexOptions { Unique = true }
                )
            );
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(user => user.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            try
            {
                return userCollection.FindOneAndUpdate<UserEntity>(
                    user => user.Login == login,
                    Builders<UserEntity>.Update.SetOnInsert(user => user.Id, Guid.NewGuid()),
                    new FindOneAndUpdateOptions<UserEntity, UserEntity> { IsUpsert = true, ReturnDocument = ReturnDocument.After }
                );
            }
            catch (MongoCommandException e) when (e.Code == 11000)
            {
                return userCollection.FindSync(u => u.Login == login).First();
            }
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(curUser => curUser.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(curUser => curUser.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var count = userCollection.CountDocuments(user => true);
            var list = userCollection
                        .Find(user => true)
                        .SortBy(user => user.Login)
                        .Skip(pageSize * (pageNumber - 1))
                        .Limit(pageSize)
                        .ToList();

            return new PageList<UserEntity>(
                list,
                count,
                pageNumber,
                pageSize
            );
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}