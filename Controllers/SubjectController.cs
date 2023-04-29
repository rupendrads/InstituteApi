using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace InstituteApi.Controllers;

[Authorize]
[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class SubjectController: ControllerBase
{
    private readonly InstituteContext _context;

    public SubjectController(InstituteContext context)
    {
        _context = context;
    }

    // GET: api/Subject
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects(long instituteId)
    {
        return await _context.Subjects.Where(s => s.InstituteId == instituteId)
            .Select(x => ItemToDTO(x))
            .ToListAsync();
    }

    // GET: api/Subject/5
    [HttpGet("{subjectId}")]
    public async Task<ActionResult<SubjectDto>> Get(long subjectId)
    {
        var subject = await _context.Subjects.FindAsync(subjectId);

        if (subject == null)
        {
            return NotFound();
        }

        return ItemToDTO(subject);
    }

    // POST: api/Subject
    [HttpPost]
    public async Task<ActionResult<SubjectDto>> PostSubject(SubjectDto subjectDto)
    {
        var subject = new Subject
        {
            SubjectName = subjectDto.SubjectName,
            InstituteId = subjectDto.InstituteId
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetSubjects),
            new { id = subject.SubjectId },
            ItemToDTO(subject));
    }

    // PUT: api/Subject/5
    [HttpPut("{subjectId}")]
    public async Task<IActionResult> PutSubject(long subjectId,[FromBody] SubjectDto subjectDto)
    {
        if (subjectId != subjectDto.SubjectId)
        {
            return BadRequest();
        }

        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null)
        {
            return NotFound();
        }

        subject.SubjectName = subjectDto.SubjectName;        

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!SubjectExists(subjectId))
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Subject/5
    [HttpDelete("{subjectId}")]
    public async Task<IActionResult> DeleteSubject(long subjectId)
    {
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null)
        {
            return NotFound();
        }

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SubjectExists(long subjectId)
    {
        return _context.Subjects.Any(e => e.SubjectId == subjectId);
    }

    private static SubjectDto ItemToDTO(Subject subject) =>
    new SubjectDto
    {
        SubjectId = subject.SubjectId,
        SubjectName = subject.SubjectName,
        InstituteId = subject.InstituteId
    };
}