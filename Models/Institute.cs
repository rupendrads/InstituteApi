namespace InstituteApi.Models;

public class Institute
{
    public long Id {get; set;}

    public string Name { get; set; }

    public virtual RoyaltyLevel RoyaltyLevel { get; set; }
}