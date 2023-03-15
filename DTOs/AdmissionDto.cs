namespace InstituteApi.DTOs;

public class AdmissionDto 
{
    public long Id {get; set;}

    public long UserId {get; set;}

    public long InstituteId {get; set;}

    public long CourseId {get; set;}

    public DateTime DateOfAdmission { get; set; }

    public long? RefId { get; set; }
}