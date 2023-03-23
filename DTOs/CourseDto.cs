namespace InstituteApi.DTOs;

public class CourseDto 
{
    public long CourseId { get; set; }

    public string CourseName { get; set; }

    public long InstituteId { get; set; }

    public double CourseFee { get; set; }

    public short CourseDuration { get; set; }

    public string? RoyaltyType { get; set; }

    public double RoyaltyValue { get; set; }

    public ICollection<SubjectDto> Subjects { get; set; }
}