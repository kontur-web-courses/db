using System;
using System.Linq;
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
            userCollection.Indexes.CreateOne("{ Login : 1 }", new CreateIndexOptions { Unique = true });
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var users= userCollection.Find(x => x.Id == id);
            return users.FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            try
            {
                var user = userCollection.Find(x => x.Login == login).FirstOrDefault();
                if (user != null)
                {
                    return user;
                }

                user = new UserEntity { Login = login };
                Insert(user);
                return user;
            }
            catch (MongoWriteException e) when (e.WriteError.Code == 11000)
            {
                return userCollection.FindSync(u => u.Login == login).First();
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
            //TODO: Тебе понадобятся SortBy, Skip и Limit
            var users = userCollection.Find("{}")
                .SortBy(x => x.Login)
                .Skip((pageNumber-1)*pageSize)
                .Limit(pageSize)
                .ToList();
            return new PageList<UserEntity>(users, userCollection.CountDocuments("{}"), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}