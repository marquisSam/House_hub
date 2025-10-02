using HouseHub.Contracts;
using HouseHub.Models;

namespace HouseHub.Interface
{
    public interface ITodoServices
    {
        Task<IEnumerable<Todo>> GetAllTodos();
        Task<Todo> GetByIdAsync(Guid id);
        Task<Todo> CreateAsync(CreateTodosRequest request);
        Task<Todo> UpdateAsync(Guid id, UpdateTodoRequest request);
        Task<Todo> DeleteAsync(Guid id);
    }
}