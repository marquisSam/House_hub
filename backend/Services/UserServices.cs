using HouseHub.Contracts;
using HouseHub.Models;
using HouseHub.Interface;
using HouseHub.AppDataContext;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HouseHub.Services
{
    /// <summary>
    /// Service class responsible for managing User entities and their associated operations.
    /// Provides CRUD operations, email validation, and duplicate checking.
    /// </summary>
    /// <remarks>
    /// This service ensures email uniqueness across the user database and provides
    /// comprehensive user management functionality with proper error handling.
    /// </remarks>
    public class UserServices : IUserServices
    {
        private readonly ItemDbContext _context;
        private readonly ILogger<UserServices> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the UserServices class.
        /// </summary>
        /// <param name="context">The database context for accessing user data.</param>
        /// <param name="logger">The logger for recording service operations and errors.</param>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        public UserServices(ItemDbContext context, ILogger<UserServices> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains an enumerable collection of all users.
        /// Returns an empty collection if no users exist.
        /// </returns>
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the user to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the user entity.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no user with the specified ID exists in the database.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 404 response.
        /// </exception>
        public async Task<User> GetByIdAsync(Guid id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID '{id}' not found");
            }
            return user;
        }

        /// <summary>
        /// Retrieves a specific user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the user entity.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no user with the specified email exists in the database.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 404 response.
        /// </exception>
        /// <remarks>
        /// Email comparison is case-sensitive based on database collation settings.
        /// </remarks>
        public async Task<User> GetByEmailAsync(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email '{email}' not found");
            }
            return user;
        }

        /// <summary>
        /// Creates a new user with email uniqueness validation.
        /// </summary>
        /// <param name="request">The request object containing user details.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the newly created user entity.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the request parameter is null.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 400 response.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a user with the specified email already exists or when a database error occurs.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 500 response.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// 1. Validates that the request is not null
        /// 2. Checks if the email already exists in the database (if email is provided)
        /// 3. Maps the request to a User entity using AutoMapper
        /// 4. Sets CreatedAt and UpdatedAt timestamps to current UTC time
        /// 5. Saves the user to the database
        /// </para>
        /// <para>
        /// Email uniqueness is enforced at the application level. If an email is provided
        /// and already exists, the operation will fail before attempting to save to the database.
        /// </para>
        /// </remarks>
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

            User? user = _mapper.Map<User>(request);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating user");
                throw new InvalidOperationException("Failed to create user. Please try again.", ex);
            }

            return user;
        }

        /// <summary>
        /// Updates an existing user with email uniqueness validation.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the user to update.</param>
        /// <param name="request">The request object containing updated user details.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the updated user entity.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the request parameter is null.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 400 response.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no user with the specified ID exists in the database.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 404 response.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the email already exists for another user or when a database error occurs.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 500 response.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// 1. Validates that the request is not null
        /// 2. Finds the existing user by ID
        /// 3. Checks if the email already exists for a different user (if email is being changed)
        /// 4. Maps the request properties to the user entity using AutoMapper
        /// 5. Updates the UpdatedAt timestamp to current UTC time
        /// 6. Saves the changes to the database
        /// </para>
        /// <para>
        /// When checking email uniqueness, the current user's ID is excluded from the check,
        /// allowing users to keep their existing email address during updates.
        /// </para>
        /// </remarks>
        public async Task<User> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            User? existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID '{id}' not found");
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
                _logger.LogError(ex, "Database error while updating user with ID {UserId}", id);
                throw new InvalidOperationException("Failed to update user. Please try again.", ex);
            }

            return existingUser;
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the user to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the deleted user entity.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no user with the specified ID exists in the database.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 404 response.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a database error occurs during deletion (e.g., foreign key constraint violations).
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 500 response.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method permanently removes the user from the database.
        /// If the user has associated TodoUser relationships, the delete behavior depends on
        /// the cascade delete configuration in the database model.
        /// </para>
        /// <para>
        /// Consider the implications of deleting users who are assigned to todos.
        /// You may want to implement soft deletes or handle orphaned todo assignments.
        /// </para>
        /// </remarks>
        public async Task<User> DeleteAsync(Guid id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID '{id}' not found");
            }

            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted user with ID {UserId}", id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting user with ID {UserId}", id);
                throw new InvalidOperationException("Failed to delete user. The user may have associated data that must be removed first.", ex);
            }

            return user;
        }

        /// <summary>
        /// Checks if an email address already exists in the user database.
        /// </summary>
        /// <param name="email">The email address to check. Can be null or whitespace.</param>
        /// <param name="excludeUserId">Optional user ID to exclude from the check (used when updating a user).</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains true if the email exists (for a different user), false otherwise.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is used for email uniqueness validation during create and update operations.
        /// If the email parameter is null or whitespace, the method returns false (no conflict).
        /// </para>
        /// <para>
        /// When excludeUserId is provided, the method checks if the email exists for any user
        /// except the one with the specified ID. This allows users to keep their existing email
        /// during update operations without triggering a false duplicate error.
        /// </para>
        /// </remarks>
        public async Task<bool> EmailExistsAsync(string? email, Guid? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            IQueryable<User>? query = _context.Users.Where(u => u.Email == email);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }
    }
}