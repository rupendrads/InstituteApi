using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;

namespace InstituteApi.Controllers;

[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class RoyaltyReportController: ControllerBase 
{
    private readonly InstituteContext _context;

    public RoyaltyReportController(InstituteContext context)
    {
        _context = context;
    }

    // GET: api/RoyaltyReport?instituteId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoyaltyDistributionDto>>> GetRoyaltyPayouts(long instituteId)
    {
        return await _context.RoyaltyDistributions.Include(rd => rd.Admission).Include(rd => rd.Admission.User)
                    .Include(rd => rd.RoyaltyDistributionDetails).ThenInclude(rdd => rdd.User)
                    .Where(rd => rd.Admission.InstituteId == instituteId && rd.RoyaltyDistributionDetails.Any(rdd => rdd.PayoutFlag == true))
                    .Select(x => RoyaltyDistributionItemToDto(x))
                    .ToListAsync();
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