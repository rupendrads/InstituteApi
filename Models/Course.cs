namespace InstituteApi.Models;

public class Course 
{
    public long CourseId { get; set; }

    public string CourseName { get; set; }

    public long InstituteId { get; set; }

    public double CourseFee { get; set; }

    public short CourseDuration { get; set; }

    public ICollection<Subject> Subjects { get; set; }
}