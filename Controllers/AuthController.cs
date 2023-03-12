using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;

namespace InstituteApi.Controllers;

[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly InstituteContext _context;

    public AuthController(InstituteContext context)
    {
        _context = context;
    }

    [HttpPost]  
    public async Task<IActionResult> PostLogin([FromBody] LoginDto loginDto)  
    {  
        var user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == loginDto.UserName && user.Password == loginDto.Password);

        bool loggedIn = user == null ? false: true;

        return Ok(new {
            loggedIn= loggedIn,
            userId = user?.Id
        });
    }
}

