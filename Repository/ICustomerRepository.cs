using HotelCustomerAPI.Entities;

namespace HotelCustomerAPI.Repository
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(string id);
        Task<IEnumerable<Customer>> GetManyAsync(int start, int count);
        Task AddOneAsync(Customer customer);
        Task PutOneAsync(string id, Customer customer);
        Task DeleteOneAsync(string id);
    }
}
