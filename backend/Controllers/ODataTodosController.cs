using HouseHub.Interface;
using HouseHub.Contracts;
using HouseHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using System.Linq;

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

        // PUT: odata/Todos(guid) - Full replacement
        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] CreateTodosRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // PUT should replace the entire resource
                var existingTodo = await _todoServices.GetByIdAsync(key);
                if (existingTodo == null)
                {
                    return NotFound($"Todo with id {key} not found.");
                }

                // Create a new todo object with all required fields
                var updateRequest = new UpdateTodoRequest
                {
                    Title = request.Title,
                    Description = request.Description,
                    DueDate = request.DueDate,
                    Priority = request.Priority,
                    Category = request.Category,
                    IsCompleted = false // Reset completion status for full replacement
                };

                var todo = await _todoServices.UpdateAsync(key, updateRequest);
                return Updated(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while replacing todo with id: {key}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PATCH: odata/Todos(guid) - Partial update
        [HttpPatch]
        public async Task<IActionResult> Patch([FromRoute] Guid key, [FromBody] UpdateTodoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingTodo = await _todoServices.GetByIdAsync(key);
                if (existingTodo == null)
                {
                    return NotFound($"Todo with id {key} not found.");
                }

                var updatedTodo = await _todoServices.UpdateAsync(key, request);
                if (updatedTodo == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Update operation failed");
                }
                
                return Ok(updatedTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while partially updating todo with id: {key}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Alternative PATCH route for OData guid format
        [HttpPatch("({key})")]
        public async Task<IActionResult> PatchAlternative([FromRoute] string key, [FromBody] UpdateTodoRequest request)
        {
            // Parse the GUID from string (handles both "guid'value'" and plain "value" formats)
            if (!Guid.TryParse(key.Replace("guid'", "").Replace("'", ""), out Guid guidKey))
            {
                return BadRequest($"Invalid GUID format: {key}");
            }

            return await Patch(guidKey, request);
        }

        // DELETE: odata/Todos(guid)
        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            try
            {
                var todo = await _todoServices.DeleteAsync(key);
                _logger.LogInformation($"Todo deleted successfully: {todo.Title}");
                return todo != null ? Ok(todo.Id) : NotFound($"Todo with id {key} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting todo with id: {key}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}