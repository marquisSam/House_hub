using HouseHub.Models;
using HouseHub.AppDataContext;
using Microsoft.EntityFrameworkCore;

namespace HouseHub.Services
{
    public class TodoUserService : ITodoUserService
    {
        private readonly ItemDbContext _context;
        private readonly ILogger<TodoUserService> _logger;

        public TodoUserService(ItemDbContext context, ILogger<TodoUserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TodoUser> AssignUserToTodoAsync(Guid todoId, Guid userId)
        {
            // Check if assignment already exists
            var existingAssignment = await _context.TodoUsers
                .FirstOrDefaultAsync(tu => tu.TodoId == todoId && tu.UserId == userId);

            if (existingAssignment != null)
            {
                throw new InvalidOperationException("User is already assigned to this todo");
            }

            // Verify todo and user exist
            var todoExists = await _context.Todos.AnyAsync(t => t.Id == todoId);
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!todoExists) throw new ArgumentException("Todo not found", nameof(todoId));
            if (!userExists) throw new ArgumentException("User not found", nameof(userId));

            var todoUser = new TodoUser
            {
                TodoId = todoId,
                UserId = userId,
                AssignedAt = DateTime.UtcNow
            };

            _context.TodoUsers.Add(todoUser);
            await _context.SaveChangesAsync();

            return todoUser;
        }

        public async Task<bool> RemoveUserFromTodoAsync(Guid todoId, Guid userId)
        {
            var todoUser = await _context.TodoUsers
                .FirstOrDefaultAsync(tu => tu.TodoId == todoId && tu.UserId == userId);

            if (todoUser == null) return false;

            _context.TodoUsers.Remove(todoUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetTodoUsersAsync(Guid todoId)
        {
            return await _context.TodoUsers
                .Where(tu => tu.TodoId == todoId)
                .Include(tu => tu.User)
                .Select(tu => tu.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Todo>> GetUserTodosAsync(Guid userId)
        {
            return await _context.TodoUsers
                .Where(tu => tu.UserId == userId)
                .Include(tu => tu.Todo)
                .Select(tu => tu.Todo)
                .ToListAsync();
        }

        public async Task<TodoUser?> GetTodoUserAssignmentAsync(Guid todoId, Guid userId)
        {
            return await _context.TodoUsers
                .Include(tu => tu.Todo)
                .Include(tu => tu.User)
                .FirstOrDefaultAsync(tu => tu.TodoId == todoId && tu.UserId == userId);
        }
    }
}