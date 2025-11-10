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
            
            // Create unique index on Login
            var indexKeysDefinition = Builders<UserEntity>.IndexKeys.Ascending(u => u.Login);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<UserEntity>(indexKeysDefinition, indexOptions);
            userCollection.Indexes.CreateOne(indexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            UserEntity userToInsert = user;
            if (user.Id == Guid.Empty)
            {
                userToInsert = new UserEntity(Guid.NewGuid(), user.Login, user.LastName, user.FirstName, user.GamesPlayed, user.CurrentGameId);
            }
            
            userCollection.InsertOne(userToInsert);
            return userToInsert;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(u => u.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var existingUser = userCollection.Find(u => u.Login == login).FirstOrDefault();
            if (existingUser != null)
            {
                return existingUser;
            }

            var newUser = new UserEntity(Guid.NewGuid(), login, "", "", 0, null);
            try
            {
                userCollection.InsertOne(newUser);
                return newUser;
            }
            catch (MongoWriteException)
            {
                // Another thread might have inserted the same login, try to find it
                return userCollection.Find(u => u.Login == login).FirstOrDefault() ?? newUser;
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
            var totalCount = userCollection.CountDocuments(_ => true);
            var items = userCollection
                .Find(_ => true)
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