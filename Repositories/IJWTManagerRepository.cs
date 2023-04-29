using InstituteApi.Models;

namespace InstituteApi.Repositories;

public interface IJWTManagerRepository
{
    JwtToken Authenticate(User user); 
}   
