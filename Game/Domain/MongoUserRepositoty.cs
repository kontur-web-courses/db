using System;
using JetBrains.Annotations;
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
            userCollection.Indexes.CreateOne(
                new CreateIndexModel<UserEntity>(
                    Builders<UserEntity>.IndexKeys.Ascending(nameof(UserEntity.Login)),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.FindSync(u => u.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var update = Builders<UserEntity>.Update
                .SetOnInsert(nameof(UserEntity.Id), Guid.NewGuid())
                .SetOnInsert(nameof(UserEntity.Login), login)
                .SetOnInsert(nameof(UserEntity.LastName), (string) null)
                .SetOnInsert(nameof(UserEntity.FirstName), (string) null)
                .SetOnInsert(nameof(UserEntity.GamesPlayed), 0)
                .SetOnInsert(nameof(UserEntity.CurrentGameId), Guid.Empty);

            return userCollection.FindOneAndUpdate<UserEntity, UserEntity>(u => u.Login == login, update,
                new FindOneAndUpdateOptions<UserEntity, UserEntity>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                });
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
            var entities = userCollection
                .Find(_ => true)
                .SortBy(u => u.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
            return new PageList<UserEntity>(entities, userCollection.CountDocuments(_ => true), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}