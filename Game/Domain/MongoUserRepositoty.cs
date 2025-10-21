using System;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        private static object Locker = new object();
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var options = new CreateIndexOptions{Unique = true};
            userCollection.Indexes.CreateOne("{ Login : 1 }", options);
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
            lock (Locker)
            {
                var user = userCollection.Find(x => x.Login == login).FirstOrDefault();
                if (user is null)
                {
                    user = new UserEntity { Login = login };
                    userCollection.InsertOne(user);
                }

                return user;
            }
        }

        public void Update(UserEntity user) => userCollection.ReplaceOne(x => x.Login == user.Login, user);

        public void Delete(Guid id) => userCollection.DeleteOne(x => x.Id == id);

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var sortedCollection = userCollection
                .Find(x => true)
                .SortBy(x => x.Login).ToList();
            var totalCount = sortedCollection.Count;
            var collection = sortedCollection
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PageList<UserEntity>(collection, totalCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}