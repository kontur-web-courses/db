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

            var options = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<UserEntity>(
                Builders<UserEntity>.IndexKeys.Descending(t => t.Login),
                options);
            userCollection.Indexes.CreateOne(indexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(FilterUserById(id)).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var filter = Builders<UserEntity>.Filter.Eq(user => user.Login, login);
            var user = userCollection.Find(filter).FirstOrDefault();
            if (user is not null)
                return user;
            var newUser = new UserEntity()
            {
                Login = login,
            };
            userCollection.InsertOne(newUser);
            return newUser;
        }

        public void Update(UserEntity user)
        {
            var filter = FilterUserById(user.Id);
            userCollection.ReplaceOne(filter, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(FilterUserById(id));
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var totalCount = userCollection.CountDocuments(Builders<UserEntity>.Filter.Empty);
            var items = userCollection.Find(Builders<UserEntity>.Filter.Empty)
                .SortBy(user => user.Login)
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

        private static FilterDefinition<UserEntity> FilterUserById(Guid id)
        {
            return Builders<UserEntity>.Filter
                .Eq(u => u.Id, id);
        }
    }
}