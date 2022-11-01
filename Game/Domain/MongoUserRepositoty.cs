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
            var options = new CreateIndexOptions { Unique = true };
#pragma warning disable CS0618
            userCollection.Indexes.CreateOne(Builders<UserEntity>.IndexKeys.Ascending(x => x.Login), options);
#pragma warning restore CS0618
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var cursor = userCollection.FindSync(Builders<UserEntity>.Filter.Eq(x => x.Id, id));
            return cursor.SingleOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = FindByLogin(login);
            if (user != null)
            {
                return user;
            }
            var userEntity = new UserEntity(Guid.NewGuid(), login, "", "", 0, null);
            return Insert(userEntity);
        }

        private UserEntity FindByLogin(string login)
        {
            var cursor = userCollection.FindSync(Builders<UserEntity>.Filter.Eq(x => x.Login, login));
            return cursor.SingleOrDefault();
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(Builders<UserEntity>.Filter.Eq(x => x.Id, user.Id), user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(Builders<UserEntity>.Filter.Eq(x => x.Id, id));
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var startIndex = pageSize * (pageNumber - 1);
            var userEntities = userCollection.FindSync(FilterDefinition<UserEntity>.Empty)
                .ToList();
            var page=userEntities
                .Skip(startIndex)
                .Take(pageSize)
                .OrderBy(x => x.Login);
            var pageList = new PageList<UserEntity>(page.ToList(), userEntities.Count, pageNumber, pageSize);
            return pageList;
            //TODO: Тебе понадобятся SortBy, Skip и Limit
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}