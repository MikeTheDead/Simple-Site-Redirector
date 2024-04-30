namespace altsite.MongoControllers;

public interface IMongoController<T>
{
    Task<T?> Get(string id);
    Task<List<T>> GetCollection();
    Task Set(T value);
    Task Update(T value);
    Task Remove(T value);
}