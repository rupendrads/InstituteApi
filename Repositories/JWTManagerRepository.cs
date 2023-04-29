using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InstituteApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace InstituteApi.Repositories;

public class JWTManagerRepository : IJWTManagerRepository
{     
    private readonly IConfiguration iconfiguration;

    public JWTManagerRepository()
    {
    }

    public JWTManagerRepository(IConfiguration iconfiguration)
    {
        this.iconfiguration = iconfiguration;
    }

    public JwtToken Authenticate(User user)
    {        
		// Else we generate JSON Web Token
		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
		  Subject = new ClaimsIdentity(new Claim[]
		  {
			 new Claim(ClaimTypes.Name, user.FirstName)                    
		  }),
		   Expires = DateTime.UtcNow.AddMinutes(20),
		   SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),SecurityAlgorithms.HmacSha256Signature)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return new JwtToken { Token = tokenHandler.WriteToken(token) };
    }
}