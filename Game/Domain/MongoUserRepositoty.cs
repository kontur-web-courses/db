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
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>("{ Login : 1 }", new CreateIndexOptions(){Unique = true}));
        }

        public UserEntity Insert(UserEntity user)
        {
            var found = userCollection.Find(x => x.Login == user.Login).FirstOrDefault();
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var user = userCollection.Find(x => (x.Id == id)).FirstOrDefault();
            return user;
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            lock (login)
            {
                var user = userCollection.Find(x => x.Login == login).FirstOrDefault()
                           ?? Insert(new UserEntity(Guid.NewGuid()) { Login = login });
                return user;
            }
        }

        public void Update(UserEntity user)
        {
            //var update = new BsonDocument("$set", new BsonDocument(user));
            var result = userCollection.ReplaceOne(x => x.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(x => x.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var list = userCollection.Aggregate().SortBy(x => x.Login).Skip(pageSize * (pageNumber - 1)).Limit(pageSize).ToList();
            return new PageList<UserEntity>(list, userCollection.CountDocuments(x => true), pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            //нуууууууууу ладно
            throw new NotImplementedException();
        }
    }
}