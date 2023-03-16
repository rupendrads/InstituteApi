using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;

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

        course.Subjects = new List<Subject>();
        courseDto.Subjects.ToList().ForEach(subject => {
            course.Subjects.Add(new Subject {
                SubjectName = subject.SubjectName
            });
        });

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCourse),
            new { id = course.CourseId },
            ItemToDTO(course));
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