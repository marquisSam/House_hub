using HouseHub.Interface;
using HouseHub.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace HouseHub.Controllers
{
    public class UsersController : ODataController
    {
        private readonly IUserServices _userServices;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserServices userServices, ILogger<UsersController> logger)
        {
            _userServices = userServices;
            _logger = logger;
        }

        // GET: odata/Users
        [EnableQuery(PageSize = 20)]
        public async Task<IActionResult> Get()
        {
            IEnumerable<Models.User>? users = await _userServices.GetAllUsers();
            return Ok(users);
        }

        // GET: api/Users/test - Simple test endpoint
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Users controller is working", timestamp = DateTime.UtcNow });
        }

        // GET: odata/Users(guid)
        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            Models.User? user = await _userServices.GetByIdAsync(key);
            if (user == null)
                return NotFound($"User with id {key} not found.");
            return Ok(user);
        }

        // POST: odata/Users
        public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Models.User? user = await _userServices.CreateAsync(request);
            return Created(user);
        }

        // PUT: odata/Users(guid)
        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Models.User? user = await _userServices.UpdateAsync(key, request);
            return Updated(user);
        }

        // DELETE: odata/Users(guid)
        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            Models.User? user = await _userServices.DeleteAsync(key);
            if (user == null)
                return NotFound($"User with id {key} not found.");
            
            _logger.LogInformation($"User deleted successfully: {user.Email}");
            return Ok(user);
        }

        // GET: odata/Users/GetByEmail?email=test@example.com
        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            Models.User? user = await _userServices.GetByEmailAsync(email);
            if (user == null)
                return NotFound($"User with email {email} not found.");
            return Ok(user);
        }
    }
}