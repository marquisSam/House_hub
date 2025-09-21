using HouseHub.Contracts;
using HouseHub.Models;
using HouseHub.Interface;
using HouseHub.AppDataContext;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HouseHub.Services
{
    public class TodoServices : ITodoServices
    {
        private readonly ItemDbContext _context;
        private readonly ILogger<TodoServices> _logger;
        private readonly IMapper _mapper;

        public TodoServices(ItemDbContext context, ILogger<TodoServices> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Todo>> GetAllTodos()
        {
            var todos = await _context.Todos.ToListAsync();
            if (todos == null)
            {
                throw new Exception("No todos found");
            }
            return todos;
        }

        public async Task<Todo> GetByIdAsync(Guid id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                throw new Exception("Todo not found");
            }
            return todo;
        }

        public async Task<Todo> CreateAsync(CreateTodosRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var todo = _mapper.Map<Todo>(request);
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            _context.Todos.Add(todo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Task failed successfully: creating todo.");
                throw new Exception($"Task failed successfully: {ex.Message}");
            }
            return todo;
        }

        public async Task<Todo> UpdateAsync(Guid id, UpdateTodoRequest request)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    throw new Exception("Todo not found");
                }

                _mapper.Map(request, todo);
                todo.UpdatedAt = DateTime.UtcNow;

                // Handle completion timestamp
                if (request.IsCompleted.HasValue)
                {
                    if (request.IsCompleted.Value && todo.CompletedAt == null)
                    {
                        todo.CompletedAt = DateTime.UtcNow;
                    }
                    else if (!request.IsCompleted.Value)
                    {
                        todo.CompletedAt = null;
                    }
                }

                await _context.SaveChangesAsync();
                return todo;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Task failed successfully: updating todo.");
                throw new Exception($"Task failed successfully: {ex.Message}");
            }
        }

        public async Task<Todo> DeleteAsync(Guid id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                throw new Exception("Todo not found");
            }
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return todo;
        }
    }
}