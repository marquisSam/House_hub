using HouseHub.Models;

namespace HouseHub.Services
{
    public interface ITodoUserService
    {
        Task<TodoUser> AssignUserToTodoAsync(Guid todoId, Guid userId);
        Task<bool> RemoveUserFromTodoAsync(Guid todoId, Guid userId);
        Task<IEnumerable<User>> GetTodoUsersAsync(Guid todoId);
        Task<IEnumerable<Todo>> GetUserTodosAsync(Guid userId);
        Task<TodoUser?> GetTodoUserAssignmentAsync(Guid todoId, Guid userId);
    }
}