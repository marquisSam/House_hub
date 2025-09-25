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
            try
            {
                var users = await _userServices.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // GET: odata/Users(guid)
        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            try
            {
                var user = await _userServices.GetByIdAsync(key);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting user with id: {key}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // POST: odata/Users
        public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userServices.CreateAsync(request);
                return Created(user);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("email already exists"))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // PUT: odata/Users(guid)
        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userServices.UpdateAsync(key, request);
                return Updated(user);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("email already exists"))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex) when (ex.Message == "User not found")
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating user with id: {key}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // DELETE: odata/Users(guid)
        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            try
            {
                var user = await _userServices.DeleteAsync(key);
                return Ok(user);
            }
            catch (Exception ex) when (ex.Message == "User not found")
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting user with id: {key}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // GET: odata/Users/GetByEmail?email=test@example.com
        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            try
            {
                var user = await _userServices.GetByEmailAsync(email);
                return Ok(user);
            }
            catch (Exception ex) when (ex.Message == "User not found")
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting user with email: {email}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}