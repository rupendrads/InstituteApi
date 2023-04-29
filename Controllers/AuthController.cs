using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;
using InstituteApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace InstituteApi.Controllers;

[AllowAnonymous]
[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly InstituteContext _context;
    private readonly IJWTManagerRepository _jWTManagerRepository;

    public AuthController(InstituteContext context, IJWTManagerRepository jWTManagerRepository)
    {
        _context = context;
        _jWTManagerRepository = jWTManagerRepository;
    }

    [HttpPost]  
    public async Task<IActionResult> PostLogin([FromBody] LoginDto loginDto)  
    {  
        var user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == loginDto.UserName && user.Password == loginDto.Password);

        bool loggedIn = user == null ? false: true;

        JwtToken token = null;
        if(loggedIn == true)
        {
            token = _jWTManagerRepository.Authenticate(user);            
        } 
        else 
        {
            return Unauthorized();
        }

        return Ok(new {
            loggedIn= loggedIn,
            userId = user?.Id,
            userType = user?.UserType,
            instituteId = user?.InstituteId,
            token = token
        });
    }
}

