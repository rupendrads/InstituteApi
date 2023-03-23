namespace InstituteApi.DTOs;

public class RoyaltyLevelDto
{
    public long Id { get; set; }

    public byte RLevel { get; set; }

    public ICollection<RoyaltyLevelDetailDto> RoyaltyLevelDetails { get; set; }
}