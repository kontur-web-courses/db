using System;
using System.Linq;
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
                Builders<UserEntity>.IndexKeys.Descending(user => user.Login), 
                new CreateIndexOptions { Unique = true }));
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
            lock (login)
            {
                var users = userCollection.FindSync(user => user.Login == login).ToList();
                if (users.Count > 0)
                    return users.First();
                var user = new UserEntity(Guid.NewGuid())
                {
                    Login = login
                };
                userCollection.InsertOne(user);

                return user;
            }
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(usr => usr.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(user => user.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var users = userCollection.AsQueryable()
                .OrderBy(usr => usr.Login)
                .Skip((pageNumber  - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PageList<UserEntity>(users, userCollection.Count(x => true), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}