using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApi.DTOs;
using WebApi.Errors;
using WebApi.Extensions;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers;

public class AccountController : BaseController
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _configuration;

    public AccountController(IUnitOfWork uow, IConfiguration configuration)
    {
        _uow = uow;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginReqDto loginReq)
    {
        var user = await _uow.UserRepository.Authenticate(loginReq.UserName, loginReq.Password);

        if (user is null)
        {
            var apiError = new ApiError()
            {
                ErrorCode = Unauthorized().StatusCode,
                ErrorMessage = "Invalid user name or password",
                ErrorDetails = "This error appear when provided user id or password does not exists"
            };
            return Unauthorized(apiError);
        }

        var loginRes = new LoginResDto
        {
            UserName = user.Username,
            Token = CreateJwt(user)
        };

        return Ok(loginRes);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(LoginReqDto loginReq)
    {
        var apiError = new ApiError();
        
        if(loginReq.UserName.IsEmpty() || loginReq.Password.IsEmpty()) {
            apiError.ErrorCode=BadRequest().StatusCode;
            apiError.ErrorMessage="User name or password can not be blank";                    
            return BadRequest(apiError);
        }     
        
        if (await _uow.UserRepository.UserAlreadyExists(loginReq.UserName)) {
            apiError.ErrorCode=BadRequest().StatusCode;
            apiError.ErrorMessage="User already exists, please try different user name";
            return BadRequest(apiError);
        }

        _uow.UserRepository.Register(loginReq.UserName, loginReq.Password);
        await _uow.SaveAsync();

        return StatusCode(201);
    }


    private string CreateJwt(User user)
    {
        var secretKey = _configuration.GetSection("AppSettings:Key").Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var claims = new Claim[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(2),
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}