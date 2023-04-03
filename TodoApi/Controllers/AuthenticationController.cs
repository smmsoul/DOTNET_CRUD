using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthenticationController(IConfiguration config) 
    {
        _config = config;
    }

    public record AuthenticationData(string? UserName, string? Password);
    public record UserData(int Id, string FirstName, string LastName, string UserName);

    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<string> Authenticate([FromBody] AuthenticationData data)
    {
        var user = ValidateCredentials(data);
        if(user == null)
        {
            return Unauthorized();
        }

        string token = (string)GenerateToken(user);

        return Ok(token);
    }

    private object GenerateToken(UserData user)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(
                _config.GetValue<string>(key: "Authentication:SecretKey")));

        var signingCredentials = new SigningCredentials(secretKey,SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new();
        claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName));
        claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));
        claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));

        var token = new JwtSecurityToken(
            _config.GetValue<string>(key:"Authentication:Issuer"),
            _config.GetValue<string>(key: "Authentication:Audience"),
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(value:60),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserData? ValidateCredentials(AuthenticationData data)
    {
        //not a production code - replace with a call to ur auth system
        if(CompareValues(data.UserName, expected:"tcorey")&&
            CompareValues(data.Password, expected:"Test123"))
        {
            return new UserData(Id: 1, FirstName: "Tim", LastName: "Corey", data.UserName);
        }

        if (CompareValues(data.UserName, expected: "sstrom") &&
            CompareValues(data.Password, expected: "Test123"))
        {
            return new UserData(Id: 1, FirstName: "Sue", LastName: "Strom", data.UserName);
        }

        if (CompareValues(data.UserName, expected: "string") &&
            CompareValues(data.Password, expected: "string"))
        {
            return new UserData(Id: 1, FirstName: "string", LastName: "string", data.UserName);
        }
        return null;
    }

    private bool CompareValues(string? actual, string? expected)
    {
        if (actual is not null)
        {
            if(actual.Equals(expected))
            {
                return true;
            }
        }
        return false;
    }

}
