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

        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var startIndex = pageSize * (pageNumber - 1);
            var page = userCollection
                .Find(FilterDefinition<UserEntity>.Empty)
                .SortBy(x => x.Login)
                .Skip(startIndex)
                .Limit(pageSize)
                .ToList();
            var pageList = new PageList<UserEntity>(
                page,
                userCollection.CountDocuments(FilterDefinition<UserEntity>.Empty),
                pageNumber,
                pageSize);
            return pageList;
        }

        public void UpdateOrInsert(UserEntity user, out bool isInserted) => throw new NotImplementedException();

        private UserEntity FindByLogin(string login) => userCollection.Find(x => x.Login == login).SingleOrDefault();
    }
}