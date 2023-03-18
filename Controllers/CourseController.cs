using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;
using System.Linq;

namespace InstituteApi.Controllers;

[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly InstituteContext _context;

    public CourseController(InstituteContext context)
    {   
        _context = context;
    }

    // GET: api/Course
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        return await _context.Courses.Include(c => c.Subjects)
            .Select(x => ItemToDTO(x))
            .ToListAsync();
    }

    // GET: api/Course/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Course>> GetCourse(long id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        return course;
    }

    // POST: api/Course
    [HttpPost]
    public async Task<ActionResult<CourseDto>> PostCourse(CourseDto courseDto)
    {
        var course = new Course
        {
            CourseName = courseDto.CourseName,
            InstituteId = courseDto.InstituteId,
            CourseFee = courseDto.CourseFee,
            CourseDuration = courseDto.CourseDuration,            
        };

        var subjectsArray = courseDto.Subjects.ToArray();
        List<long?> subjectIds = new List<long?>();
        courseDto.Subjects.ToList().ForEach(subject => {
                subjectIds.Add(subject.SubjectId);
        });
        
        var selectedSubjects = from s in _context.Subjects
        where subjectIds.Contains(s.SubjectId.Value)
        select s;

        course.Subjects = selectedSubjects.ToList();

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCourse),
            new { id = course.CourseId },
            ItemToDTO(course));
    }

    // PUT: api/Course/5
    [HttpPut("{courseId}")]
    public async Task<IActionResult> PutCourse(long courseId,[FromBody] CourseDto courseDto)
    {
        if (courseId != courseDto.CourseId)
        {
            return BadRequest();
        }

        var course = await _context.Courses.Include(s => s.Subjects).FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course == null)
        {
            return NotFound();
        }

        course.CourseName = courseDto.CourseName;
        course.CourseDuration = courseDto.CourseDuration;
        course.CourseFee = courseDto.CourseFee;

        var subjectsArray = courseDto.Subjects.ToArray();
        List<long?> subjectIds = new List<long?>();
        courseDto.Subjects.ToList().ForEach(subject => {
                subjectIds.Add(subject.SubjectId);
        });
        
        var selectedSubjects = from s in _context.Subjects
        where subjectIds.Contains(s.SubjectId.Value)
        select s;

        var deletedSubjects = course.Subjects.Except(selectedSubjects);

        // added subjects
        selectedSubjects.ToList().ForEach(selectedSubject => {
            if(course.Subjects.FirstOrDefault(s => s.SubjectId == selectedSubject.SubjectId) == null)
            {
                course.Subjects.Add(selectedSubject);
            }
        });        

        // removed subjects
        deletedSubjects.ToList().ForEach(deletedSubject => {
            if(course.Subjects.FirstOrDefault(s => s.SubjectId == deletedSubject.SubjectId) != null)
            {
                course.Subjects.Remove(deletedSubject);
            }
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!CourseExists(courseId))
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Course/5
    [HttpDelete("{courseId}")]
    public async Task<IActionResult> DeleteCourse(long courseId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
        {
            return NotFound();
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CourseExists(long courseId)
    {
        return _context.Courses.Any(e => e.CourseId == courseId);
    }

    private static CourseDto ItemToDTO(Course course) 
    {
        CourseDto courseDto = new CourseDto
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            InstituteId = course.InstituteId,
            CourseDuration = course.CourseDuration,
            CourseFee = course.CourseFee           
        };
        courseDto.Subjects = new List<SubjectDto>();
        if(course.Subjects!=null)
        {
            course.Subjects.ToList().ForEach(subject => {
                courseDto.Subjects.Add(new SubjectDto {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName
                });
            });
        }
        return courseDto;
    }
}