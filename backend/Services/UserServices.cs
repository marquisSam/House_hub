using HouseHub.Contracts;
using HouseHub.Models;
using HouseHub.Interface;
using HouseHub.AppDataContext;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HouseHub.Services
{
    public class UserServices : IUserServices
    {
        private readonly ItemDbContext _context;
        private readonly ILogger<UserServices> _logger;
        private readonly IMapper _mapper;

        public UserServices(ItemDbContext context, ILogger<UserServices> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            if (users == null)
            {
                throw new Exception("No users found");
            }
            return users;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }

        public async Task<User> CreateAsync(CreateUserRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Check if email already exists
            if (!string.IsNullOrWhiteSpace(request.Email) && await EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("A user with this email already exists");
            }

            var user = _mapper.Map<User>(request);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error saving user to database");
                throw new InvalidOperationException("Failed to create user", ex);
            }

            return user;
        }

        public async Task<User> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            // Check if email already exists for another user
            if (!string.IsNullOrEmpty(request.Email) && await EmailExistsAsync(request.Email, id))
            {
                throw new InvalidOperationException("A user with this email already exists");
            }

            _mapper.Map(request, existingUser);
            existingUser.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating user in database");
                throw new InvalidOperationException("Failed to update user", ex);
            }

            return existingUser;
        }

        public async Task<User> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting user from database");
                throw new InvalidOperationException("Failed to delete user", ex);
            }

            return user;
        }

        public async Task<bool> EmailExistsAsync(string? email, Guid? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var query = _context.Users.Where(u => u.Email == email);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }
    }
}