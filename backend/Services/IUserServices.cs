using HouseHub.Contracts;
using HouseHub.Models;

namespace HouseHub.Interface
{
    public interface IUserServices
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<User> CreateAsync(CreateUserRequest request);
        Task<User> UpdateAsync(Guid id, UpdateUserRequest request);
        Task<User> DeleteAsync(Guid id);
        Task<bool> EmailExistsAsync(string? email, Guid? excludeUserId = null);
    }
}