using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;

namespace InstituteApi.Controllers;

[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly InstituteContext _context;

    public UsersController(InstituteContext context)
    {   
        _context = context;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        return await _context.Users
            .Select(x => ItemToDTO(x))
            .ToListAsync();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(long id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return ItemToDTO(user);
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<UserDto>> PostUser(UserDto userDto)
    {
        var user = new User
        {
            InstituteId = userDto.InstituteId,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            UserName = userDto.UserName,
            Password = userDto.Password,
            Birthdate = userDto.Birthdate,
            Gender = userDto.Gender,
            PhoneNo = userDto.PhoneNo,
            Email = userDto.Email,
            Address = userDto.Address,
            UserType = userDto.UserType
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            ItemToDTO(user));
    }

    // PUT: api/Users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(long id,[FromBody] UserDto userDto)
    {
        if (id != userDto.Id)
        {
            return BadRequest();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.Birthdate = userDto.Birthdate;
        user.Gender = userDto.Gender;
        user.PhoneNo = userDto.PhoneNo;
        user.Email = userDto.Email;
        user.Address = userDto.Address;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!UserExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    private bool UserExists(long id)
    {
        return _context.Users.Any(e => e.Id == id);
    }

    private static UserDto ItemToDTO(User user) =>
    new UserDto
    {
        Id = user.Id,
        InstituteId = user.InstituteId,
        FirstName = user.FirstName,
        LastName = user.LastName,
        UserName = user.UserName,
        Password = user.Password,
        Birthdate = user.Birthdate,
        Gender = user.Gender,
        PhoneNo = user.PhoneNo,
        Email = user.Email,
        Address = user.Address,
        UserType = user.UserType
    };
}