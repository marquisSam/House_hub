using HouseHub.Contracts;
using HouseHub.Models;
using HouseHub.Interface;
using HouseHub.AppDataContext;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HouseHub.Services
{
    /// <summary>
    /// Service class responsible for managing Todo entities and their associated operations.
    /// Provides CRUD operations, user assignment management, and transaction handling.
    /// </summary>
    /// <remarks>
    /// This service coordinates between the Todo entities, user assignments, and database context.
    /// All create and update operations are wrapped in transactions to ensure data consistency.
    /// </remarks>
    public class TodoServices : ITodoServices
    {
        private readonly ItemDbContext _context;
        private readonly ILogger<TodoServices> _logger;
        private readonly IMapper _mapper;
        private readonly ITodoUserService _todoUserService;

        /// <summary>
        /// Initializes a new instance of the TodoServices class.
        /// </summary>
        /// <param name="context">The database context for accessing todo data.</param>
        /// <param name="logger">The logger for recording service operations and errors.</param>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        /// <param name="todoUserService">The service for managing todo-user relationships.</param>
        public TodoServices(ItemDbContext context, ILogger<TodoServices> logger, IMapper mapper, ITodoUserService todoUserService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _todoUserService = todoUserService;
        }

        /// <summary>
        /// Retrieves all todos from the database including their assigned users.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains an enumerable collection of all todos with their assigned users.
        /// Returns an empty collection if no todos exist.
        /// </returns>
        /// <remarks>
        /// This method eagerly loads the Users navigation property using Include.
        /// </remarks>
        public async Task<IEnumerable<Todo>> GetAllTodos()
        {
            return await _context.Todos
                .Include(t => t.Users)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific todo by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the todo to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the todo entity with its assigned users.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no todo with the specified ID exists in the database.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 404 response.
        /// </exception>
        /// <remarks>
        /// This method eagerly loads the Users navigation property.
        /// </remarks>
        public async Task<Todo> GetByIdAsync(Guid id)
        {
            Todo? todo = await _context.Todos
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (todo == null)
            {
                throw new KeyNotFoundException($"Todo with ID '{id}' not found");
            }
            
            return todo;
        }

        /// <summary>
        /// Creates a new todo with optional user assignments in a transactional operation.
        /// </summary>
        /// <param name="request">The request object containing todo details and optional user assignments.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the newly created todo with all assigned users loaded.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the request parameter is null.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 400 response.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a database update error occurs during todo creation or user assignment.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 500 response.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method executes the following steps within a database transaction:
        /// 1. Maps the request to a Todo entity using AutoMapper
        /// 2. Sets CreatedAt and UpdatedAt timestamps to current UTC time
        /// 3. Saves the todo to the database
        /// 4. Assigns users to the todo if AssignedUserIds are provided
        /// 5. Commits the transaction if all operations succeed
        /// </para>
        /// <para>
        /// If any step fails, the entire transaction is rolled back to maintain data consistency.
        /// The todo is reloaded after creation to include all navigation properties.
        /// </para>
        /// </remarks>
        public async Task<Todo> CreateAsync(CreateTodosRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                Todo? todo = _mapper.Map<Todo>(request);
                todo.CreatedAt = DateTime.UtcNow;
                todo.UpdatedAt = DateTime.UtcNow;
                
                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();
                
                // Handle user assignments
                await AssignUsersToTodoAsync(todo.Id, request.AssignedUserIds);
                
                await transaction.CommitAsync();
                
                // Reload the todo with assigned users
                return await GetByIdAsync(todo.Id);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error while creating todo");
                throw new InvalidOperationException("Failed to create todo. Please try again.", ex);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Updates an existing todo with new values and optionally manages user assignments in a transactional operation.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the todo to update.</param>
        /// <param name="request">The request object containing updated todo details and optional user assignments.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the updated todo with all assigned users loaded.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no todo with the specified ID exists in the database.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 404 response.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a database update error occurs during todo update or user assignment changes.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 500 response.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method executes the following steps within a database transaction:
        /// 1. Finds the existing todo by ID
        /// 2. Maps the request properties to the todo entity using AutoMapper
        /// 3. Updates the UpdatedAt timestamp to current UTC time
        /// 4. Handles completion status and sets/clears CompletedAt timestamp accordingly
        /// 5. Saves todo changes to the database
        /// 6. Updates user assignments if AssignedUserIds are provided (adds new assignments and removes unassigned users)
        /// 7. Commits the transaction if all operations succeed
        /// </para>
        /// <para>
        /// The CompletedAt timestamp is managed as follows:
        /// - Set to current UTC time when IsCompleted changes from false to true
        /// - Cleared (set to null) when IsCompleted changes from true to false
        /// - Preserved if already set and IsCompleted remains true
        /// </para>
        /// <para>
        /// If any step fails, the entire transaction is rolled back to maintain data consistency.
        /// The todo is reloaded after update to include all navigation properties.
        /// </para>
        /// </remarks>
        public async Task<Todo> UpdateAsync(Guid id, UpdateTodoRequest request)
        {
            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                Todo? todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    throw new KeyNotFoundException($"Todo with ID '{id}' not found");
                }

                _mapper.Map(request, todo);
                todo.UpdatedAt = DateTime.UtcNow;

                // Handle completion timestamp
                if (request.IsCompleted.HasValue)
                {
                    todo.CompletedAt = request.IsCompleted.Value && todo.CompletedAt == null
                        ? DateTime.UtcNow
                        : !request.IsCompleted.Value ? null : todo.CompletedAt;
                }

                await _context.SaveChangesAsync();
                
                // Handle user assignments if provided
                if (request.AssignedUserIds != null)
                {
                    await UpdateTodoUserAssignmentsAsync(id, request.AssignedUserIds);
                }
                
                await transaction.CommitAsync();
                
                // Reload the todo with assigned users
                return await GetByIdAsync(id);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error while updating todo with ID {TodoId}", id);
                throw new InvalidOperationException("Failed to update todo. Please try again.", ex);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Deletes a todo and all its associated relationships from the database.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the todo to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the deleted todo entity.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no todo with the specified ID exists in the database.
        /// This exception is handled by the GlobalExceptionHandler middleware to return a 404 response.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method permanently removes the todo from the database.
        /// Associated TodoUser relationships are automatically deleted due to cascade delete configuration.
        /// </para>
        /// <para>
        /// The operation is logged upon successful completion.
        /// </para>
        /// </remarks>
        public async Task<Todo> DeleteAsync(Guid id)
        {
            Todo? todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                throw new KeyNotFoundException($"Todo with ID '{id}' not found");
            }
            
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted todo with ID {TodoId}", id);
            return todo;
        }

        #region Private Helper Methods

        /// <summary>
        /// Assigns multiple users to a todo by creating TodoUser relationships.
        /// </summary>
        /// <param name="todoId">The unique identifier of the todo to assign users to.</param>
        /// <param name="userIds">A collection of user IDs to assign. Can be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// <para>
        /// This method iterates through the provided user IDs and creates assignments.
        /// If a user is already assigned to the todo, the InvalidOperationException is caught and ignored.
        /// This allows the method to be idempotent and safely handle duplicate assignment requests.
        /// </para>
        /// <para>
        /// If userIds is null or empty, the method returns immediately without performing any operations.
        /// </para>
        /// </remarks>
        private async Task AssignUsersToTodoAsync(Guid todoId, IEnumerable<Guid>? userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return;
            }

            foreach (Guid userId in userIds)
            {
                try
                {
                    await _todoUserService.AssignUserToTodoAsync(todoId, userId);
                }
                catch (InvalidOperationException)
                {
                    // User already assigned, skip
                }
            }
        }

        /// <summary>
        /// Synchronizes user assignments for a todo by comparing current assignments with the desired state.
        /// </summary>
        /// <param name="todoId">The unique identifier of the todo to update assignments for.</param>
        /// <param name="assignedUserIds">The complete list of user IDs that should be assigned to the todo.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// <para>
        /// This method performs a differential update of user assignments:
        /// 1. Retrieves the current list of users assigned to the todo
        /// 2. Identifies users that need to be removed (currently assigned but not in the new list)
        /// 3. Identifies users that need to be added (in the new list but not currently assigned)
        /// 4. Removes outdated assignments
        /// 5. Creates new assignments
        /// </para>
        /// <para>
        /// This approach ensures that the final state matches the provided assignedUserIds list exactly,
        /// while minimizing database operations by only modifying assignments that have changed.
        /// </para>
        /// </remarks>
        private async Task UpdateTodoUserAssignmentsAsync(Guid todoId, IEnumerable<Guid> assignedUserIds)
        {
            IEnumerable<User>? currentUsers = await _todoUserService.GetTodoUsersAsync(todoId);
            HashSet<Guid>? currentUserIds = currentUsers.Select(u => u.Id).ToHashSet();
            
            // Remove users that are no longer assigned
            List<Guid>? usersToRemove = currentUserIds.Except(assignedUserIds).ToList();
            foreach (Guid userIdToRemove in usersToRemove)
            {
                await _todoUserService.RemoveUserFromTodoAsync(todoId, userIdToRemove);
            }
            
            // Add new user assignments
            List<Guid>? usersToAdd = assignedUserIds.Except(currentUserIds).ToList();
            await AssignUsersToTodoAsync(todoId, usersToAdd);
        }

        #endregion
    }
}