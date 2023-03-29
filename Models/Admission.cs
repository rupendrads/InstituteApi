namespace InstituteApi.Models;

public class Admission 
{
    public long Id {get; set;}

    public long UserId {get; set;}

    public virtual User User { get; set; }

    public long InstituteId {get; set;}

    public long CourseId {get; set;}

    public virtual Course Course {get; set;}

    public DateTime DateOfAdmission { get; set; }

    public long? RefId { get; set; }
}