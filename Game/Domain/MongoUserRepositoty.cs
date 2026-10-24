using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain;

public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserEntity> userCollection;
    public const string CollectionName = "users";

    public MongoUserRepository(IMongoDatabase db)
    {
        userCollection = db.GetCollection<UserEntity>(CollectionName);
        var indexModel = new CreateIndexModel<UserEntity>(
            Builders<UserEntity>.IndexKeys.Ascending(u => u.Login),
            new CreateIndexOptions { Unique = true, Name = "Login" });
        userCollection.Indexes.CreateOne(indexModel);
    }

    public UserEntity Insert(UserEntity user)
    {
        var id = Guid.NewGuid();
        var withId = new UserEntity(id, user.Login, user.LastName, user.FirstName, user.GamesPlayed, user.CurrentGameId);
        userCollection.InsertOne(withId);
        return withId;
    }

    public UserEntity FindById(Guid id) => userCollection.Find(u => u.Id == id).FirstOrDefaultAsync().Result;

    public UserEntity GetOrCreateByLogin(string login)
    { 
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Login, login);
        var option = new FindOneAndUpdateOptions<UserEntity> { ReturnDocument = ReturnDocument.After, IsUpsert = true };
        var update = Builders<UserEntity>.Update
            .SetOnInsert(u => u.Id, Guid.NewGuid())
            .SetOnInsert(u => u.Login, login);

        try
        {
            return userCollection.FindOneAndUpdate(filter, update, option);
        }
        catch (MongoCommandException ex) when (ex.Code == 11000)    // E11000 код для "DuplicateKey"
        {
            return userCollection.Find(u => u.Login == login).FirstOrDefault();
        }
    }

    public void Update(UserEntity user) => userCollection.ReplaceOne(u => u.Id == user.Id, user);

    public void Delete(Guid id) => userCollection.DeleteOne(u => u.Id == id);

    public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
    {
        var totalCount = userCollection.CountDocuments(new BsonDocument());
        var items = userCollection
            .Find(new BsonDocument())
            .SortBy(u => u.Login)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToList();
        return new PageList<UserEntity>(items, totalCount, pageNumber, pageSize);
    }

    public void UpdateOrInsert(UserEntity user, out bool isInserted)
    {
        throw new NotImplementedException();
    }
}