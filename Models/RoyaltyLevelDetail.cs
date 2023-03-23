namespace InstituteApi.Models;

public class RoyaltyLevelDetail 
{
    public long Id { get; set; }

    public long RoyaltyLevelId { get; set; }

    public byte RLevelNo {get; set;}

    public byte RLevelPercent { get; set; }

    public RoyaltyLevel RoyaltyLevel { get; set; }
}