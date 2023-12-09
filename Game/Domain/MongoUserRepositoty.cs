using System;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
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
            
            var indexKeysDefinition = Builders<UserEntity>.IndexKeys.Descending(user => user.Login);
            userCollection.Indexes.CreateOne(indexKeysDefinition, new CreateIndexOptions { Unique = true });
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(user => user.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection.Find(user => user.Login == login).FirstOrDefault();

            if (user == null)
            {
                user = new UserEntity(){Login = login};

                try
                {
                    userCollection.InsertOne(user);
                }
                catch (MongoWriteException)
                {
                    user = userCollection.Find(userEntity => userEntity.Login == login).FirstOrDefault();
                }
            }

            return user;
        }

        public void Update(UserEntity updatedUser)
        {
            userCollection.ReplaceOne(user => user.Id == updatedUser.Id, updatedUser);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(user => user.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var allUsers = userCollection.Find(_ => true).SortBy(user => user.Login).ToList();
            var usersOnCurrentPage = allUsers.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            return new PageList<UserEntity>(usersOnCurrentPage, allUsers.Count, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}