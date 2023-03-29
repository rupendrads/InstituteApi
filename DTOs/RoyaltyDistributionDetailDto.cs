namespace InstituteApi.DTOs;

public class RoyaltyDistributionDetailDto 
{
    public long Id { get; set; }

    public long RoyaltyLevelDetailId { get; set; }    

    public long UserId { get; set; }

    public string FirstName {get; set;}

    public string LastName {get; set;}

    public double RoyaltyAmount { get; set; }

    public bool PayoutFlag { get; set; }

    public DateTime? PayoutDate { get; set; }
}