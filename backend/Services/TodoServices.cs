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
            _logger.LogInformation($"UpdateAsync called with id: {id} and request: {System.Text.Json.JsonSerializer.Serialize(request)}");
            
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    _logger.LogWarning($"Todo with id {id} not found in database");
                    throw new Exception("Todo not found");
                }

                _logger.LogInformation($"Found existing todo: {System.Text.Json.JsonSerializer.Serialize(todo)}");
                
                // Log before mapping
                _logger.LogInformation("Applying AutoMapper mapping...");
                _mapper.Map(request, todo);
                
                _logger.LogInformation($"Todo after mapping: {System.Text.Json.JsonSerializer.Serialize(todo)}");
                
                todo.UpdatedAt = DateTime.UtcNow;

                // Handle completion timestamp
                if (request.IsCompleted.HasValue)
                {
                    if (request.IsCompleted.Value && todo.CompletedAt == null)
                    {
                        todo.CompletedAt = DateTime.UtcNow;
                        _logger.LogInformation($"Set CompletedAt to {todo.CompletedAt}");
                    }
                    else if (!request.IsCompleted.Value)
                    {
                        todo.CompletedAt = null;
                        _logger.LogInformation("Cleared CompletedAt");
                    }
                }

                _logger.LogInformation("Saving changes to database...");
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Successfully updated todo: {System.Text.Json.JsonSerializer.Serialize(todo)}");
                return todo;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Task failed successfully: updating todo.");
                throw new Exception($"Task failed successfully: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error in UpdateAsync for id {id}");
                throw;
            }
        }

        public async Task<Todo> DeleteAsync(Guid id)
        {
            Console.WriteLine($"Attempting to delete todo with id: {id}");
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