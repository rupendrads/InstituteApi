namespace InstituteApi.Models;

public class RoyaltyDistribution 
{
    public long Id { get; set; }

    public long AdmissionId { get; set; }

    public virtual Admission Admission { get; set; }

    public DateTime? DateOfExecution { get; set; }

    public virtual ICollection<RoyaltyDistributionDetail> RoyaltyDistributionDetails { get; set; }
}