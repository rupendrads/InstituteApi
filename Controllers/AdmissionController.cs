using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;

namespace InstituteApi.Controllers;

[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class AdmissionController: ControllerBase 
{
    private readonly InstituteContext _context;

    public AdmissionController(InstituteContext context)
    {
        _context = context;
    }

    // GET: api/Admission
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdmissionDto>>> GetAdmissions()
    {
        return await _context.Admissions
            .Select(x => ItemToDTO(x))
            .ToListAsync();
    }


    // GET: api/admission/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Admission>> GetAdmission(long id)
    {
        var admission = await _context.Admissions.FindAsync(id);

        if (admission == null)
        {
            return NotFound();
        }

        return admission;
    }

    // POST: api/admission
    [HttpPost]  
    public async Task<IActionResult> PostAdmission([FromBody] AdmissionDto admissionDto)  
    {
        var admission = new Admission 
        {
            UserId = admissionDto.UserId,
            InstituteId = admissionDto.InstituteId,
            CourseId = admissionDto.CourseId,
            DateOfAdmission = admissionDto.DateOfAdmission,
            RefId = admissionDto.RefId
        };

        _context.Admissions.Add(admission);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAdmission),
            new { id = admission.Id },
            ItemToDTO(admission));
    }

    private static AdmissionDto ItemToDTO(Admission admission) =>
    new AdmissionDto
    {
        Id = admission.Id,
        UserId = admission.UserId,
        InstituteId = admission.InstituteId,
        CourseId = admission.CourseId,
        DateOfAdmission = admission.DateOfAdmission,
        RefId = admission.RefId
    };
}