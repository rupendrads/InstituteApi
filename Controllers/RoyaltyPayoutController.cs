using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace InstituteApi.Controllers;

[Authorize]
[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class RoyaltyPayoutController: ControllerBase 
{
    private readonly InstituteContext _context;

    public RoyaltyPayoutController(InstituteContext context)
    {
        _context = context;
    }

    // GET: api/RoyaltyPayout?instituteId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoyaltyDistributionDto>>> GetRoyaltyDistributions(long instituteId)
    {
        return await _context.RoyaltyDistributions.Include(rd => rd.Admission).Include(rd => rd.Admission.User)
                    .Include(rd => rd.RoyaltyDistributionDetails).ThenInclude(rdd => rdd.User)
                    .Where(rd => rd.Admission.InstituteId == instituteId && rd.RoyaltyDistributionDetails.Any(rdd => rdd.PayoutFlag == false))
                    .Select(x => RoyaltyDistributionItemToDto(x))
                    .ToListAsync();
    }

    // POST: api/RoyaltyPayout?instituteId=1
    [HttpPost]
    public async Task<ActionResult<RoyaltyDistributionDto>> PostRoyaltyPayout(long instituteId,ICollection<RoyaltyDistributionDto> royaltyDistributionDtos)
    {
        await _context.RoyaltyDistributions.Include(rd => rd.RoyaltyDistributionDetails)
            .Where(rd => rd.Admission.InstituteId == instituteId && rd.RoyaltyDistributionDetails.Any(rdd => rdd.PayoutFlag == false))
            .LoadAsync();

        royaltyDistributionDtos.ToList().ForEach(async rd => {
            var royaltyDistribution = await _context.RoyaltyDistributions.FindAsync(rd.Id);
            if(royaltyDistribution != null)
            {
                rd.RoyaltyDistributionDetails.ToList().ForEach(async rdd => {
                    var royaltyDistributionDetail = await _context.RoyaltyDistributionDetails.FindAsync(rdd.Id);
                    if(royaltyDistributionDetail != null)
                    {
                        royaltyDistributionDetail.PayoutDate = DateTime.Now;
                        royaltyDistributionDetail.PayoutFlag = true;
                    }
                });
            }
        });

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static RoyaltyDistributionDto RoyaltyDistributionItemToDto(RoyaltyDistribution royaltyDistribution)
    {
        RoyaltyDistributionDto royaltyDistributionDto = new RoyaltyDistributionDto {
            Id = royaltyDistribution.Id,
            AdmissionId = royaltyDistribution.AdmissionId,
            DateOfAdmission = royaltyDistribution.Admission.DateOfAdmission,
            UserId = royaltyDistribution.Admission.UserId,
            FirstName = royaltyDistribution.Admission.User.FirstName,
            LastName = royaltyDistribution.Admission.User.LastName,
            DateOfExecution = royaltyDistribution.DateOfExecution
        };

        if(royaltyDistributionDto.RoyaltyDistributionDetails == null)
        {
            royaltyDistributionDto.RoyaltyDistributionDetails = new List<RoyaltyDistributionDetailDto>();
        }

        if(royaltyDistribution.RoyaltyDistributionDetails!= null)
        {
            royaltyDistribution.RoyaltyDistributionDetails.ToList().ForEach(rdd => {
                royaltyDistributionDto.RoyaltyDistributionDetails.Add(new RoyaltyDistributionDetailDto {
                    Id = rdd.Id,
                    RoyaltyLevelDetailId = rdd.RoyaltyLevelDetailId,
                    RoyaltyAmount = rdd.RoyaltyAmount,
                    UserId = rdd.UserId,
                    FirstName = rdd.User.FirstName,
                    LastName = rdd.User.LastName,
                    PayoutDate = rdd.PayoutDate,
                    PayoutFlag = rdd.PayoutFlag
                });
            });
        }
        return royaltyDistributionDto;
    }
}