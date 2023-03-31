using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstituteApi.Models;
using InstituteApi.DTOs;
using Microsoft.AspNetCore.Cors;

namespace InstituteApi.Controllers;

[EnableCors("_myAllowSpecificOrigins")]
[ApiController]
[Route("api/[controller]")]
public class RoyaltyDistributionController : ControllerBase
{
    private readonly InstituteContext _context;

    public RoyaltyDistributionController(InstituteContext context)
    {   
        _context = context;
    }

    // GET: api/RoyaltyDistribution?instituteId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoyaltyDistributionDto>>> GetRoyaltyDistributions(long instituteId)
    {
        // get royalty levels
        var royaltyLevel = await _context.RoyaltyLevels.Include(rl => rl.RoyaltyLevelDetails).FirstOrDefaultAsync(rl => rl.Id == instituteId);
        if (royaltyLevel == null)
        {
            return NotFound();
        }
        var royaltyLevelDto = RoyaltyLevelItemToDTO(royaltyLevel);        

        //get admissions with referrals
        var admissions = _context.Admissions.Where(ad => ad.RefId!= null && ad.InstituteId == instituteId)
                        .Include(adm => adm.User)
                        .Include(adm => adm.Course)
                        .Include(adm => adm.RoyaltyDistribution); 
        var admissionsList = await admissions.ToListAsync();               

        // get referrals for each admission
        List<RoyaltyDistributionDto> royaltyDistributionList = new List<RoyaltyDistributionDto>();
        admissionsList.ForEach(ad => {  
            if(ad.RoyaltyDistribution ==null)
            {
            var royaltyDistribution = new RoyaltyDistributionDto {
                            Id = -1,            
                            AdmissionId = ad.Id,
                            DateOfAdmission = ad.DateOfAdmission,                            
                            UserId = ad.UserId, 
                            FirstName = ad.User.FirstName,
                            LastName = ad.User.LastName,
                            DateOfExecution = null,
                            RoyaltyDistributionDetails = new List<RoyaltyDistributionDetailDto>()
                        };
            var admission = ad;            
            royaltyLevelDto.RoyaltyLevelDetails.ToList().ForEach(rld => {
                if(admission!=null)
                {
                    double totalDistributableRoyalty = 0;
                    if(admission.Course!=null)
                    {
                        if(admission.Course.RoyaltyType == "Percentage")
                        {
                            double courseFee = admission.Course.CourseFee;
                            double percentage = admission.Course.RoyaltyValue;
                            totalDistributableRoyalty = (percentage/courseFee)*100;
                        }
                        else 
                        {
                            totalDistributableRoyalty = admission.Course.RoyaltyValue;
                        }
                    }
                    var referral = _context.Admissions.Include(adm => adm.User).FirstOrDefault(adm => adm.UserId == admission.RefId && adm.InstituteId == instituteId);
                    if(referral != null){
                        royaltyDistribution.RoyaltyDistributionDetails.Add(new RoyaltyDistributionDetailDto {
                            Id = -1,                        
                            PayoutFlag = false,
                            PayoutDate = null,
                            RoyaltyLevelDetailId = rld.Id,                        
                            RoyaltyAmount = totalDistributableRoyalty> 0 ? ((double)rld.RLevelPercent/totalDistributableRoyalty)*100: 0,
                            UserId = referral.UserId,
                            FirstName = referral.User.FirstName,
                            LastName = referral.User.LastName
                        });
                    }      
                    admission = referral;          
                }
            });
            royaltyDistributionList.Add(royaltyDistribution);
            }
        });
        return royaltyDistributionList;       
    }

    // GET: api/RoyaltyDistribution/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RoyaltyDistributionDto>> GetRoyaltyDistribution(long id)
    {
        var royaltyDistribution = await _context.RoyaltyDistributions.Include(rd => rd.RoyaltyDistributionDetails).FirstOrDefaultAsync(rl => rl.Id == id);

        if (royaltyDistribution == null)
        {
            return NotFound();
        }

        return RoyaltyDistributionItemToDto(royaltyDistribution);
    }

    // POST: api/RoyaltyDistribution
    [HttpPost]
    public async Task<ActionResult<RoyaltyDistributionDto>> PostRoyaltyDistribution(RoyaltyDistributionDto royaltyDistributionDto)
    {        
        var royaltyDistribution = new RoyaltyDistribution {
            AdmissionId = royaltyDistributionDto.AdmissionId,
            DateOfExecution = DateTime.Now,            
        };

        royaltyDistribution.RoyaltyDistributionDetails = new List<RoyaltyDistributionDetail>();
        royaltyDistributionDto.RoyaltyDistributionDetails.ToList().ForEach(rdd => {
            royaltyDistribution.RoyaltyDistributionDetails.Add(new RoyaltyDistributionDetail {
                PayoutDate = null,
                PayoutFlag = false,
                RoyaltyAmount = rdd.RoyaltyAmount,
                RoyaltyLevelDetailId = rdd.RoyaltyLevelDetailId,
                UserId = rdd.UserId
            });
        });

        _context.RoyaltyDistributions.Add(royaltyDistribution);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRoyaltyDistribution),
            new { id = royaltyDistribution.Id });
    }

    private static RoyaltyDistributionDto RoyaltyDistributionItemToDto(RoyaltyDistribution royaltyDistribution)
    {
        RoyaltyDistributionDto royaltyDistributionDto = new RoyaltyDistributionDto {
            Id = royaltyDistribution.Id,
            AdmissionId = royaltyDistribution.AdmissionId,
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
                    PayoutDate = rdd.PayoutDate,
                    PayoutFlag = rdd.PayoutFlag
                });
            });
        }
        return royaltyDistributionDto;
    }

    private static RoyaltyLevelDto RoyaltyLevelItemToDTO(RoyaltyLevel royaltyLevel) 
    {
        RoyaltyLevelDto royaltyLevelDto = new RoyaltyLevelDto {
            Id = royaltyLevel.Id,
            RLevel = royaltyLevel.RLevel
        };
        
        if(royaltyLevelDto.RoyaltyLevelDetails == null)
        {
            royaltyLevelDto.RoyaltyLevelDetails = new List<RoyaltyLevelDetailDto>(); 
        }

        if(royaltyLevel.RoyaltyLevelDetails!=null)
        {
            royaltyLevel.RoyaltyLevelDetails.ToList().ForEach(royaltyLevelDetail => {
                royaltyLevelDto.RoyaltyLevelDetails.Add(
                    new RoyaltyLevelDetailDto {
                        Id = royaltyLevelDetail.Id,
                        RLevelNo = royaltyLevelDetail.RLevelNo,
                        RLevelPercent = royaltyLevelDetail.RLevelPercent,
                        RoyaltyLevelId = royaltyLevelDetail.RoyaltyLevelId
                    }
                );                
            });
        }
        return royaltyLevelDto;
    }

    private static UserDto UserItemToDTO(User user) =>
    new UserDto
    {
        Id = user.Id,
        InstituteId = user.InstituteId,
        FirstName = user.FirstName,
        LastName = user.LastName,
        UserName = user.UserName,
        Password = user.Password,
        Birthdate = user.Birthdate,
        Gender = user.Gender,
        PhoneNo = user.PhoneNo,
        Email = user.Email,
        Address = user.Address,
        UserType = user.UserType
    };
}