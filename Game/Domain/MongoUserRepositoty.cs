using System;
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
            var indexModel = new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys.Ascending(x => x.Login), new CreateIndexOptions()
            {
                Unique = true
            });
            userCollection.Indexes.CreateOne(indexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var filter = new BsonDocument("_id", new BsonBinaryData(id, GuidRepresentation.Standard));
            return userCollection.Find(filter).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var filter = Builders<UserEntity>.Filter.Eq(u => u.Login, login);
            var update = Builders<UserEntity>.Update.SetOnInsert(u => u.Id, Guid.NewGuid());
            var options = new FindOneAndUpdateOptions<UserEntity>()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            return userCollection.FindOneAndUpdate(filter, update, options);
        }

        public void Update(UserEntity user)
        {
            var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, user.Id);
            userCollection.ReplaceOne(filter, user);
        }

        public void Delete(Guid id)
        {
            var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, id);
            userCollection.DeleteOne(filter);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var filter = new BsonDocument();
            var users = userCollection
                .Find(filter)
                .SortBy(x => x.Login);
            var totalCount = users.CountDocuments();
            var pageList = new PageList<UserEntity>(
                    users
                        .Skip((pageNumber - 1) * pageSize)
                        .Limit(pageSize)
                        .ToList(), 
                    totalCount,
                    pageNumber, 
                    pageSize);
            return pageList;
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}