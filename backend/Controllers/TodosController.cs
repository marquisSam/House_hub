using HouseHub.Interface;
using HouseHub.Contracts;
using Microsoft.AspNetCore.Mvc;


namespace HouseHub.Controllers.Todos
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoServices _todoServices;
        private readonly ILogger<TodosController> _logger;
        public TodosController(ITodoServices todoServices, ILogger<TodosController> logger)
        {
            _todoServices = todoServices;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodoAsync(CreateTodosRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var todo = await _todoServices.CreateAsync(request);
                return Ok(new { message = $"Successfully created todo.", data = todo });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllTodosAsync()
        {
            try
            {
                var todos = await _todoServices.GetAllTodos();
                return Ok(new { message = $"Successfully retrieved todos.", data = todos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting todos");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTodoAsync(Guid id)
        {
            try
            {
                var todo = await _todoServices.DeleteAsync(id);
                _logger.LogInformation($"Todo deleted successfully: {todo.Title}");
                return Ok(new { message = $"Successfully deleted todo {todo.Title}.", data = todo });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting todo: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTodoAsync(Guid id, UpdateTodoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var todo = await _todoServices.UpdateAsync(id, request);
                return Ok(new { message = $"Successfully updated todo.", data = todo });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Task failed successfully : {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}