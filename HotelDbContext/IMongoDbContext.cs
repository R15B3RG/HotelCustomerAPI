using MongoDB.Driver;

namespace HotelCustomerAPI.HotelDbContext
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
