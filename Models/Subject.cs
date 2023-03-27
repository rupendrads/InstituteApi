namespace InstituteApi.Models;

public class Subject 
{
    public long? SubjectId { get; set; }

    public string SubjectName { get; set; }

    public long InstituteId { get; set; }

    public Institute Institute { get; set; }

    public ICollection<Course> Courses { get; set; }
}