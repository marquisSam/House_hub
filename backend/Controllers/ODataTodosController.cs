using HouseHub.Interface;
using HouseHub.Contracts;
using HouseHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;

namespace HouseHub.Controllers
{
    public class TodosController : ODataController
    {
        private readonly ITodoServices _todoServices;
        private readonly ILogger<TodosController> _logger;

        public TodosController(ITodoServices todoServices, ILogger<TodosController> logger)
        {
            _todoServices = todoServices;
            _logger = logger;
        }

        // GET: odata/Todos
        [EnableQuery(PageSize = 20)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var todos = await _todoServices.GetAllTodos();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting todos");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // GET: odata/Todos(guid)
        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            try
            {
                var todo = await _todoServices.GetByIdAsync(key);
                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting todo with id: {key}");
                return NotFound($"Todo with id {key} not found.");
            }
        }

        // POST: odata/Todos
        public async Task<IActionResult> Post([FromBody] CreateTodosRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var todo = await _todoServices.CreateAsync(request);
                return Created(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating todo");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: odata/Todos(guid)
        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] UpdateTodoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var todo = await _todoServices.UpdateAsync(key, request);
                return Updated(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating todo with id: {key}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: odata/Todos(guid)
        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            try
            {
                var todo = await _todoServices.DeleteAsync(key);
                _logger.LogInformation($"Todo deleted successfully: {todo.Title}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting todo with id: {key}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}