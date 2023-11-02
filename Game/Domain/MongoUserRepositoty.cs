using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        [Obsolete("Obsolete")]
        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var options = new CreateIndexOptions {Unique = true};
            userCollection.Indexes.CreateOne("{Login: 1}", options);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection
                .Find(x => x.Id == id)
                .FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var userEntity = userCollection.Find(x => x.Login == login).FirstOrDefault();
            if (userEntity != null)
                return userEntity;
            userEntity = new UserEntity(Guid.NewGuid())
            {
                Login = login
            };
            userCollection.InsertOne(userEntity);
            return userEntity;
        }

        public void Update(UserEntity user)
        {
            userCollection
                .FindOneAndReplace(x => x.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection
                .FindOneAndDelete(x => x.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var pageList = userCollection
                .Aggregate()
                .SortBy(x => x.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
            return new PageList<UserEntity>(pageList, userCollection.CountDocuments(x => true), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}