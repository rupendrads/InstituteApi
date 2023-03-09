using InstituteApi.Models;

namespace InstituteApi.DTOs;
public class UserDto {
    public long Id {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string UserName {get; set;}
    public string Password {get; set;}
    public DateTime Birthdate {get; set;}
    public string Gender {get; set;}
    public string PhoneNo {get; set;}
    public string Email {get; set;}
    public string Address {get; set;}
}