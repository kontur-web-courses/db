using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain;

public class MongoUserRepository : IUserRepository
{
    public const string CollectionName = "users";
    private readonly IMongoCollection<UserEntity> userCollection;

    public MongoUserRepository(IMongoDatabase database)
    {
        userCollection = database.GetCollection<UserEntity>(CollectionName);
        userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(
            new BsonDocument("Login", 1),
            new CreateIndexOptions {Unique = true}));
    }

    public UserEntity Insert(UserEntity user)
    {
        userCollection.InsertOne(user);
        return user;
    }

    public UserEntity FindById(Guid id)
    {
        var cursor = userCollection.FindSync(user => user.Id == id);
        return cursor.FirstOrDefault();
    }

    public UserEntity GetOrCreateByLogin(string login)
    {
        var cursor = userCollection.FindSync(user => user.Login == login);
        var user = cursor.FirstOrDefault();
        if (user != null)
            return user;
        user = new UserEntity(Guid.NewGuid(), login, "", "", 0, null);
        userCollection.InsertOne(user);
        return user;
    }

    public void Update(UserEntity user)
    {
        userCollection.ReplaceOne(u => u.Id == user.Id, user);
    }

    public void Delete(Guid id)
    {
        userCollection.DeleteOne(user => user.Id == id);
    }

    // Для вывода списка всех пользователей (упорядоченных по логину)
    // страницы нумеруются с единицы
    public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
    {
        var filter = new BsonDocument();
        var totalCount = userCollection.CountDocuments(filter);
        var userEntities = userCollection.Find(filter).SortBy(user => user.Login).Skip((pageNumber - 1) * pageSize).Limit(pageSize).ToList();
        return new PageList<UserEntity>(userEntities, totalCount, pageNumber, pageSize);
    }

    // Не нужно реализовывать этот метод
    public void UpdateOrInsert(UserEntity user, out bool isInserted)
    {
        throw new NotImplementedException();
    }
}