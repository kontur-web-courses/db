using System;
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
            var options = new CreateIndexOptions
            {
                Unique = true,
                Name = "login_index"
            };
            userCollection.Indexes.CreateOne(new IndexKeysDefinitionBuilder<UserEntity>().Ascending(nameof(UserEntity.Login)), options);

        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return FindById(user.Id);
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(userEntity => userEntity.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection.Find(userEntity => userEntity.Login == login).FirstOrDefault();
            if (user == null)
            {
                user = Insert(new UserEntity()
                {
                    Login = login
                });
            }

            return user;
        }

        public void Update(UserEntity user)
        {
            var foundUser = FindById(user.Id);
            if (foundUser != null)
            {
                userCollection.ReplaceOne(userEntity => userEntity.Id == user.Id, user);
            }
            else
            {
                Insert(user);
            }
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(userEntity => userEntity.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var users = userCollection
                .Find(_ => true)
                .SortBy(userEntity => userEntity.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            return new PageList<UserEntity>(users, userCollection
                .Find(_ => true).CountDocuments(), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}