namespace InstituteApi.Models;

public class RoyaltyDistributionDetail
{
    public long Id { get; set; }

    public long RoyaltyDistributionId { get; set; }

    public virtual RoyaltyDistribution RoyaltyDistribution { get; set; }

    public long RoyaltyLevelDetailId { get; set; }

    public virtual RoyaltyLevelDetail RoyaltyLevelDetail { get; set; }

    public long UserId { get; set; }

    public virtual User User { get; set; }

    public double RoyaltyAmount { get; set; }

    public bool PayoutFlag { get; set; }

    public DateTime? PayoutDate { get; set; }
}