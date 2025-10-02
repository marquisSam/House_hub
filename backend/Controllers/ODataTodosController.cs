using HouseHub.Interface;
using HouseHub.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

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

        [EnableQuery(PageSize = 20)]
        public async Task<IActionResult> Get()
        {
            IEnumerable<Models.Todo>? todos = await _todoServices.GetAllTodos();
            return Ok(todos);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            Models.Todo? todo = await _todoServices.GetByIdAsync(key);
            if (todo == null)
                return NotFound($"Todo with id {key} not found.");
            return Ok(todo);
        }

        [EnableQuery]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTodosRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Models.Todo? todo = await _todoServices.CreateAsync(request);
            return Created(todo);
        }

        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] CreateTodosRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Models.Todo? existingTodo = await _todoServices.GetByIdAsync(key);
            if (existingTodo == null)
                return NotFound($"Todo with id {key} not found.");

            UpdateTodoRequest? updateRequest = new UpdateTodoRequest
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                Priority = request.Priority,
                Category = request.Category,
                IsCompleted = false
            };

            var todo = await _todoServices.UpdateAsync(key, updateRequest);
            return Updated(todo);
        }

        [HttpPatch]
        [EnableQuery]
        public async Task<IActionResult> Patch([FromRoute] Guid key, [FromBody] UpdateTodoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Models.Todo? existingTodo = await _todoServices.GetByIdAsync(key);
            if (existingTodo == null)
                return NotFound($"Todo with id {key} not found.");

            Models.Todo? updatedTodo = await _todoServices.UpdateAsync(key, request);
            if (updatedTodo == null)
                throw new InvalidOperationException("Update operation failed");

            return Ok(updatedTodo);
        }

        [HttpPatch("({key})")]
        [EnableQuery]
        public async Task<IActionResult> PatchAlternative([FromRoute] string key, [FromBody] UpdateTodoRequest request)
        {
            if (!Guid.TryParse(key.Replace("guid'", "").Replace("'", ""), out Guid guidKey))
                return BadRequest($"Invalid GUID format: {key}");

            return await Patch(guidKey, request);
        }

        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            Models.Todo? todo = await _todoServices.DeleteAsync(key);
            if (todo == null)
                return NotFound($"Todo with id {key} not found.");
            
            _logger.LogInformation($"Todo deleted successfully: {todo.Title}");
            return Ok(todo.Id);
        }
    }
}