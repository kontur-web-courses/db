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
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(indexKeys, indexOptions));
        }

        public UserEntity Insert(UserEntity user)
        {
            var entity = new UserEntity(
                Guid.NewGuid(),
                user.Login,
                user.LastName,
                user.FirstName,
                user.GamesPlayed,
                user.CurrentGameId);

            userCollection.InsertOne(entity);
            return entity;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(u => u.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            // 1. Сразу пробуем вставить (оптимизация для случая "пользователя нет")
            var newUser = new UserEntity(Guid.NewGuid(), login, "", "", 0, null);
            try
            {
                userCollection.InsertOne(newUser);
                return newUser;
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // 2. Если пользователь уже был — читаем его
                var filter = Builders<UserEntity>.Filter.Eq(u => u.Login, login);
                return userCollection.Find(filter).FirstOrDefault();
            }
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(u => u.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var totalCount = userCollection.CountDocuments(FilterDefinition<UserEntity>.Empty);

            var items = userCollection
                .Find(FilterDefinition<UserEntity>.Empty)
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