namespace InstituteApi.Models;

public class RoyaltyLevel 
{
    public long Id { get; set; }

    public byte RLevel { get; set; }

    public virtual Institute Institute { get; set; }

    public ICollection<RoyaltyLevelDetail> RoyaltyLevelDetails { get; set; }
}