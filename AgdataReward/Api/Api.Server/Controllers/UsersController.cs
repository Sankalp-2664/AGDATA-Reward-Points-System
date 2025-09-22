using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public IActionResult GetAll() => Ok(_userService.GetAllUsers());

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetUserById(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        var newUser = _userService.AddUser(user);
        return CreatedAtAction(nameof(GetById), new { id = newUser.UserId }, newUser);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, User user)
    {
        if (id != user.UserId) return BadRequest("Id mismatch");
        var updated = _userService.UpdateUser(user);
        return Ok(updated);
    }
}
