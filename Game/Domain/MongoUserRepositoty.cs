using System;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        public const string CollectionName = "users";
        private readonly IMongoCollection<UserEntity> userCollection;

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var options = new CreateIndexOptions { Unique = true };
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys.Ascending(x => x.Login), options));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id) => userCollection.Find(x => x.Id == id).SingleOrDefault();

        public UserEntity GetOrCreateByLogin(string login)
        {
            return FindByLogin(login) ??
                   Insert(new UserEntity(Guid.NewGuid(), login, "", "", 0, null));
        }

        public void Update(UserEntity user) => userCollection.ReplaceOne(x => x.Id == user.Id, user);

        public void Delete(Guid id) => userCollection.DeleteOne(Builders<UserEntity>.Filter.Eq(x => x.Id, id));

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var startIndex = pageSize * (pageNumber - 1);
            var userEntities = userCollection.FindSync(FilterDefinition<UserEntity>.Empty)
                .ToList();
            var page = userEntities
                .Skip(startIndex)
                .Take(pageSize)
                .OrderBy(x => x.Login);
            var pageList = new PageList<UserEntity>(page.ToList(), userEntities.Count, pageNumber, pageSize);
            return pageList;
            //TODO: Тебе понадобятся SortBy, Skip и Limit
        }

        public void UpdateOrInsert(UserEntity user, out bool isInserted) => throw new NotImplementedException();

        private UserEntity FindByLogin(string login) => userCollection.Find(x => x.Login == login).SingleOrDefault();
    }
}