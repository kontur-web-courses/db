using System;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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
                new CreateIndexModel<UserEntity>(new IndexKeysDefinitionBuilder<UserEntity>().Ascending(x => x.Login),
                new CreateIndexOptions() { Unique = true })
            );
        }

        public UserEntity Insert(UserEntity user)
        {
            //TODO: Ищи в документации InsertXXX.
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            //TODO: Ищи в документации FindXXX
            return userCollection.Find(x => x.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            //TODO: Это Find или Insert
            return userCollection.FindOneAndUpdate<UserEntity>(x => x.Login == login,
                new UpdateDefinitionBuilder<UserEntity>().SetOnInsert(x => x.Login, login).SetOnInsert(x => x.Id, Guid.NewGuid()),
                new FindOneAndUpdateOptions<UserEntity> { ReturnDocument = ReturnDocument.After, IsUpsert = true });
        }

        public void Update(UserEntity user)
        {
            //TODO: Ищи в документации ReplaceXXX
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
            return new PageList<UserEntity>(
                userCollection
                    .AsQueryable()
                    .OrderBy(x => x.Login)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),
                userCollection.CountDocuments(x => true),
                pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}