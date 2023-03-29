namespace InstituteApi.DTOs;

public class RoyaltyDistributionDto 
{
    public long Id { get; set; }

    public long AdmissionId { get; set; }

    public DateTime? DateOfAdmission { get; set; }

    public long UserId {get; set;}

    public string FirstName {get; set;}

    public string LastName {get; set;}

    public DateTime? DateOfExecution { get; set; }

    public ICollection<RoyaltyDistributionDetailDto> RoyaltyDistributionDetails { get; set; }
    
}