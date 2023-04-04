using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;

namespace InstituteApi.Controllers;

[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class InstituteController: ControllerBase
{
    private readonly InstituteContext _context;

    public InstituteController(InstituteContext context)
    {
        _context = context;
    }

    // GET: api/Institute
    [HttpGet("{id}")]
    public async Task<ActionResult<InstituteDto>> GetInstitute(long id)
    {
        var institute = await _context.Institutes.FindAsync(id);

        if (institute == null)
        {
            return NotFound();
        }

        return ItemToDTO(institute);
    }

    private static InstituteDto ItemToDTO(Institute institute) =>
    new InstituteDto
    {
        Id = institute.Id,
        Name = institute.Name
    };
}