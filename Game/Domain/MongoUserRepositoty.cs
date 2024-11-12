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
            var indexKeysDefinition = Builders<UserEntity>.IndexKeys.Ascending(user => user.Login);
            var options = new CreateIndexOptions
            {
                Unique = true,
                Name = "login_index"
            };
            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(indexKeysDefinition, options));
        }

        public UserEntity Insert(UserEntity user)
        {
            //TODO: Ищи в документации InsertXXX.
            /*            var foundUser = userCollection.Find(u => u.Login == user.Login).FirstOrDefault();
                        if (foundUser != null)
                            throw new MongoWriteException();*/
            userCollection.InsertOne(user);
            /*            try
                        {

                        }
                        catch (Exception ex) 
                        {
                            throw;
                        }*/
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(user => user.Id == id).FirstOrDefault();
            //TODO: Ищи в документации FindXXX
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            lock (userCollection)
            {
                var user = userCollection.Find(u => u.Login == login, new FindOptions
                {
                    Hint = "login_index"
                }).FirstOrDefault();

                if (user != null) return user;
                user = new UserEntity(Guid.NewGuid()) { Login = login };
                userCollection.InsertOne(user);

                return user;
            }
        }
        
        public void Update(UserEntity user)
        {
            //TODO: Ищи в документации ReplaceXXX
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
            //TODO: Тебе понадобятся SortBy, Skip и Limit
            var items = userCollection.Find(_ => true).SortBy(u => u.Login).Skip((pageNumber - 1) * pageSize).Limit(pageSize).ToList();
            var totalCount = userCollection.CountDocuments(_ => true);
            return new PageList<UserEntity>(items, totalCount, pageNumber, pageSize);

        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}