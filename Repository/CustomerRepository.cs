using HotelCustomerAPI.Entities;
using HotelCustomerAPI.HotelDbContext;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HotelCustomerAPI.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _collection;

        public CustomerRepository(IMongoDbContext context)
        {
            _collection = context.GetCollection<Customer>("Customers");
        }

        public async Task<Customer> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                throw new ArgumentException("Invalid ID format", nameof(id));
            }

            var filter = Builders<Customer>.Filter.Eq(c => c.Id, objectId.ToString());
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Customer>> GetManyAsync(int start, int count)
        {
            var filter = Builders<Customer>.Filter.Empty;
            var customers = await _collection.Find(filter).Skip(start).Limit(count).ToListAsync();
            return customers;
        }

        public async Task AddOneAsync(Customer customer)
        {
            await _collection.InsertOneAsync(customer);
        }

        public async Task PutOneAsync(string id, Customer customer)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);

            var update = Builders<Customer>.Update
                .Set(c => c.Name, customer.Name)
                .Set(c => c.Email, customer.Email)
                .Set(c => c.BookingId, customer.BookingId);

            _collection.UpdateOneAsync(filter, update);
        }


        public async Task DeleteOneAsync(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);

            _collection.DeleteOne(filter);
        }
    }
}
