using HouseHub.Models;
using HouseHub.AppDataContext;
using HouseHub.Interface;
using Microsoft.EntityFrameworkCore;

namespace HouseHub.Services
{
    /// <summary>
    /// Service class responsible for managing the many-to-many relationship between Todos and Users.
    /// Provides operations for assigning users to todos, removing assignments, and querying relationships.
    /// </summary>
    /// <remarks>
    /// This service manages the TodoUser join table and ensures referential integrity
    /// by validating that both Todo and User entities exist before creating assignments.
    /// </remarks>
    public class TodoUserService : ITodoUserService
    {
        private readonly ItemDbContext _context;
        private readonly ILogger<TodoUserService> _logger;

        /// <summary>
        /// Initializes a new instance of the TodoUserService class.
        /// </summary>
        /// <param name="context">The database context for accessing todo-user relationship data.</param>
        /// <param name="logger">The logger for recording service operations and errors.</param>
        public TodoUserService(ItemDbContext context, ILogger<TodoUserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Assigns a user to a todo by creating a TodoUser relationship.
        /// </summary>
        /// <param name="todoId">The unique identifier of the todo.</param>
        /// <param name="userId">The unique identifier of the user to assign.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the newly created TodoUser assignment entity.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the user is already assigned to the specified todo.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 500 response.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when either the todo or user with the specified ID does not exist.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 400 response.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method performs the following validation steps:
        /// 1. Checks if the assignment already exists (prevents duplicates)
        /// 2. Verifies that the todo exists in the database
        /// 3. Verifies that the user exists in the database
        /// 4. Creates the assignment with the current UTC timestamp
        /// </para>
        /// <para>
        /// The AssignedAt timestamp is automatically set to the current UTC time.
        /// </para>
        /// </remarks>
        public async Task<TodoUser> AssignUserToTodoAsync(Guid todoId, Guid userId)
        {
            // Check if assignment already exists
            TodoUser? existingAssignment = await _context.TodoUsers
                .FirstOrDefaultAsync(tu => tu.TodoId == todoId && tu.UserId == userId);

            if (existingAssignment != null)
            {
                throw new InvalidOperationException("User is already assigned to this todo");
            }

            // Verify todo and user exist
            bool todoExists = await _context.Todos.AnyAsync(t => t.Id == todoId);
            bool userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!todoExists) throw new ArgumentException("Todo not found", nameof(todoId));
            if (!userExists) throw new ArgumentException("User not found", nameof(userId));

            TodoUser? todoUser = new TodoUser
            {
                TodoId = todoId,
                UserId = userId,
                AssignedAt = DateTime.UtcNow
            };

            _context.TodoUsers.Add(todoUser);
            await _context.SaveChangesAsync();

            return todoUser;
        }

        /// <summary>
        /// Removes a user assignment from a todo by deleting the TodoUser relationship.
        /// </summary>
        /// <param name="todoId">The unique identifier of the todo.</param>
        /// <param name="userId">The unique identifier of the user to unassign.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains true if the assignment was found and removed, false if the assignment did not exist.
        /// </returns>
        /// <remarks>
        /// This method is idempotent - calling it multiple times with the same parameters
        /// will not cause an error. If the assignment does not exist, the method returns false
        /// without throwing an exception.
        /// </remarks>
        public async Task<bool> RemoveUserFromTodoAsync(Guid todoId, Guid userId)
        {
            TodoUser? todoUser = await _context.TodoUsers
                .FirstOrDefaultAsync(tu => tu.TodoId == todoId && tu.UserId == userId);

            if (todoUser == null) return false;

            _context.TodoUsers.Remove(todoUser);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Retrieves all users assigned to a specific todo.
        /// </summary>
        /// <param name="todoId">The unique identifier of the todo.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a collection of User entities assigned to the todo.
        /// Returns an empty collection if no users are assigned.
        /// </returns>
        /// <remarks>
        /// This method eagerly loads the User navigation property from the TodoUsers join table.
        /// The returned users are fully populated User entities, not just IDs.
        /// </remarks>
        public async Task<IEnumerable<User>> GetTodoUsersAsync(Guid todoId)
        {
            return await _context.TodoUsers
                .Where(tu => tu.TodoId == todoId)
                .Include(tu => tu.User)
                .Select(tu => tu.User)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all todos assigned to a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a collection of Todo entities assigned to the user.
        /// Returns an empty collection if no todos are assigned.
        /// </returns>
        /// <remarks>
        /// This method eagerly loads the Todo navigation property from the TodoUsers join table.
        /// The returned todos are fully populated Todo entities, not just IDs.
        /// </remarks>
        public async Task<IEnumerable<Todo>> GetUserTodosAsync(Guid userId)
        {
            return await _context.TodoUsers
                .Where(tu => tu.UserId == userId)
                .Include(tu => tu.Todo)
                .Select(tu => tu.Todo)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific todo-user assignment relationship.
        /// </summary>
        /// <param name="todoId">The unique identifier of the todo.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the TodoUser assignment entity if found, or null if the assignment does not exist.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method eagerly loads both the Todo and User navigation properties,
        /// providing complete information about the assignment relationship.
        /// </para>
        /// <para>
        /// Use this method when you need detailed information about a specific assignment,
        /// including the AssignedAt timestamp and full entity details.
        /// </para>
        /// </remarks>
        public async Task<TodoUser?> GetTodoUserAssignmentAsync(Guid todoId, Guid userId)
        {
            return await _context.TodoUsers
                .Include(tu => tu.Todo)
                .Include(tu => tu.User)
                .FirstOrDefaultAsync(tu => tu.TodoId == todoId && tu.UserId == userId);
        }
    }
}