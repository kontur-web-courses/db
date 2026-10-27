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
            var indexKeys = Builders<UserEntity>.IndexKeys.Ascending(u => u.Login);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<UserEntity>(indexKeys, indexOptions);
            userCollection.Indexes.CreateOne(indexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var userEntity = userCollection.Find(Builders<UserEntity>.Filter.Eq(u => u.Id, id)).FirstOrDefault();
            return userEntity;
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var userEntity = new UserEntity { Login = login };
            try
            {
                userCollection.InsertOne(userEntity);
                return userEntity;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return userCollection.Find(u => u.Login == login).First();
            }
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(Builders<UserEntity>.Filter.Eq(u => u.Id, user.Id), user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(Builders<UserEntity>.Filter.Eq(u => u.Id, id));
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var totalCount = userCollection.CountDocuments(Builders<UserEntity>.Filter.Empty);

            var items = userCollection.Find(Builders<UserEntity>.Filter.Empty)
                .SortBy(u => u.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
            
            return new PageList<UserEntity>(items, totalCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}