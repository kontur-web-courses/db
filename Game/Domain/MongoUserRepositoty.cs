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
            var indexKeysDefinition = Builders<UserEntity>.IndexKeys.Ascending(user => user.Login);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModelDefinition = new CreateIndexModel<UserEntity>(indexKeysDefinition, indexOptions);
            userCollection.Indexes.CreateOne(indexModelDefinition);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var user = userCollection.Find(x => x.Id == id);
            return user?.FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            lock (login)
            {
                var user = userCollection.Find(x => x.Login == login).FirstOrDefault();
                if (user == null)
                {
                    user = Insert(new UserEntity { Login = login });
                }
                else
                {
                    userCollection.ReplaceOne(x => x.Login == login, user);
                }
               
                return user;
            }
            
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
            var usersCollection = userCollection.Database.GetCollection<UserEntity>(CollectionName);
            var userCount = usersCollection.Count(_ => true);
            var users = usersCollection.Find(_ => true)
                                                    .SortBy(x => x.Login)
                                                    .Skip((pageNumber-1) * pageSize)
                                                    .Limit(pageSize)
                                                    .ToList();
            return new PageList<UserEntity>(users, userCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}