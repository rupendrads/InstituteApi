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
public class RoyaltyLevelController: ControllerBase
{
    private readonly InstituteContext _context;

    public RoyaltyLevelController(InstituteContext context)
    {
        _context = context;
    }

    // GET: api/RoyaltyLevel
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoyaltyLevelDto>>> GetRoyaltyLevels()
    {
        return await _context.RoyaltyLevels.Include(rl => rl.RoyaltyLevelDetails)
            .Select(x => ItemToDTO(x))
            .ToListAsync();
    }

    // GET: api/RoyaltyLevel/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RoyaltyLevelDto>> GetRoyaltyLevel(long id)
    {
        var royaltyLevel = await _context.RoyaltyLevels.Include(rl => rl.RoyaltyLevelDetails).FirstOrDefaultAsync(rl => rl.Id == id);

        if (royaltyLevel == null)
        {
            return NotFound();
        }

        return ItemToDTO(royaltyLevel);
    }

    // POST: api/RoyaltyLevel
    [HttpPost]
    public async Task<ActionResult<RoyaltyLevelDto>> PostRoyaltyLevel(RoyaltyLevelDto royaltyLevelDto)
    {
        var royaltyLevel = new RoyaltyLevel {
            Id = royaltyLevelDto.Id,
            RLevel = royaltyLevelDto.RLevel
        };

        royaltyLevel.RoyaltyLevelDetails = new List<RoyaltyLevelDetail>();
        royaltyLevelDto.RoyaltyLevelDetails.ToList().ForEach(rld => {
            royaltyLevel.RoyaltyLevelDetails.Add(new RoyaltyLevelDetail {
                RoyaltyLevelId = royaltyLevelDto.Id,
                RLevelNo = rld.RLevelNo,
                RLevelPercent = rld.RLevelPercent
            });
        });

        _context.RoyaltyLevels.Add(royaltyLevel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRoyaltyLevel),
            new { id = royaltyLevel.Id },
            ItemToDTO(royaltyLevel));
    }

    // PUT: api/RoyaltyLevel/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRoyaltyLevel(long id,[FromBody] RoyaltyLevelDto royaltyLevelDto)
    {
        if (id != royaltyLevelDto.Id)
        {
            return BadRequest();
        }

        var royaltyLevel = await _context.RoyaltyLevels.Include(rl => rl.RoyaltyLevelDetails).FirstOrDefaultAsync(rl => rl.Id == id);
        if (royaltyLevel == null)
        {
            return NotFound();
        }

        royaltyLevel.RLevel = royaltyLevelDto.RLevel;

        // updating existing details
        var deletedRoyaltyLevelDetails = new List<RoyaltyLevelDetail>();        
        royaltyLevel.RoyaltyLevelDetails.ToList().ForEach(rld => {
            var rldt = royaltyLevelDto.RoyaltyLevelDetails.FirstOrDefault(rldt => rldt.Id == rld.Id);
            if(rldt != null){
                rld.RLevelNo = rldt.RLevelNo;
                rld.RLevelPercent =  rldt.RLevelPercent;
            }
            else 
            {
                deletedRoyaltyLevelDetails.Add(rld);
            }
        }); 

        // removing deleted details
        deletedRoyaltyLevelDetails.ToList().ForEach(rld => {
            royaltyLevel.RoyaltyLevelDetails.Remove(rld);
        });

        // adding new details
        royaltyLevelDto.RoyaltyLevelDetails.ToList().ForEach(rldt => {
            if(royaltyLevel.RoyaltyLevelDetails.Any(rld => rld.Id == rldt.Id) == false)
            {
                royaltyLevel.RoyaltyLevelDetails.Add(new RoyaltyLevelDetail {
                    RLevelNo = rldt.RLevelNo,
                    RLevelPercent = rldt.RLevelPercent,
                    RoyaltyLevelId = id
                });
            }
        });        

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!RoyaltyLevelExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/RoyaltyLevel/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoyaltyLevel(long id)
    {
        var royaltyLevel = await _context.RoyaltyLevels.FindAsync(id);
        if (royaltyLevel == null)
        {
            return NotFound();
        }

        _context.RoyaltyLevels.Remove(royaltyLevel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RoyaltyLevelExists(long id)
    {
        return _context.RoyaltyLevels.Any(rl => rl.Id == id);
    }

    private static RoyaltyLevelDto ItemToDTO(RoyaltyLevel royaltyLevel) 
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
}