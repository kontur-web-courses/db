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
            userCollection.Indexes
                .CreateOne(
                    new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys
                        .Ascending(x => x.Login), new CreateIndexOptions { Unique = true }));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var user = userCollection.Find(x => x.Id == id).FirstOrDefault();
            return user;
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection.Find(x => x.Login == login).FirstOrDefault();
            if (user is null)
            {
                var newUser = new UserEntity(Guid.NewGuid())
                {
                    Login = login
                };
                Insert(newUser);
                
                return newUser;
            }

            return user;
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
            var users = userCollection
                .Find(_ => true)
                .SortBy(x => x.Login)
                .Skip(pageSize * (pageNumber - 1))
                .Limit(pageSize);

            return new PageList<UserEntity>(
                users.ToList(),
                userCollection.CountDocuments(_ => true),
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