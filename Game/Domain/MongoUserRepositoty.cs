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
            var indexKeys = Builders<UserEntity>.IndexKeys.Ascending(x => x.Login);

            var indexOptions = new CreateIndexOptions { Unique = true };

            var indexModel = new CreateIndexModel<UserEntity>(indexKeys, indexOptions);

            userCollection.Indexes.CreateOne(indexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id) => userCollection.Find(x => x.Id == id).FirstOrDefault();

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection.Find(x => x.Login == login).FirstOrDefault();
            if (user != null)
                return user;
            var newUser = new UserEntity{Login = login};
            try
            {
                return Insert(newUser);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                return userCollection.Find(x => x.Login == login).First();
            }
        }

        public void Update(UserEntity user) => userCollection.ReplaceOne(x => x.Id == user.Id, user);

        public void Delete(Guid id) => userCollection.DeleteOne(x => x.Id == id);

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var totalCount = userCollection.CountDocuments(_ => true);
            var skip = (pageNumber - 1) * pageSize;
            var items = userCollection
                .Find(_ => true)
                .SortBy(x => x.Login)
                .Skip(skip)
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