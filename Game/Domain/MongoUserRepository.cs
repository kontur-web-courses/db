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
            var indexModel = new CreateIndexModel<UserEntity>(
                Builders<UserEntity>.IndexKeys.Ascending(x => x.Login),
                new CreateIndexOptions { Unique = true }
            );
            userCollection.Indexes.CreateOne(indexModel);
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
            // var foundByLogin =  userCollection.Find(x => x.Login == login).FirstOrDefault();
            // if (foundByLogin != null)
            //     return foundByLogin;
            // var newUser = new UserEntity {Login = login};
            // userCollection.InsertOne(newUser);
            // return newUser;
            
            var options = new FindOneAndUpdateOptions<UserEntity>
            {
                IsUpsert = true, 
                ReturnDocument = ReturnDocument.After
            };
            
            var update = Builders<UserEntity>.Update
                .SetOnInsert(x => x.Login, login)
                .SetOnInsert(x => x.Id, Guid.NewGuid());
            
            var result = userCollection.FindOneAndUpdate<UserEntity, UserEntity>(x => x.Login == login, update, options);
            return result;
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
            var totalCount = userCollection.CountDocuments(FilterDefinition<UserEntity>.Empty);
            
            var users = userCollection.Find(FilterDefinition<UserEntity>.Empty)
                .SortBy(x => x.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
                
            return new PageList<UserEntity>(users, totalCount, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}