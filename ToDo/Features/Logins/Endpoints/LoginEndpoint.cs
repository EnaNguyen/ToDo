using Microsoft.AspNetCore.Mvc;
using ToDo.Extensions;
using ToDo.Helpers.OTPs;
using ToDo.Helpers.Tokens;
using ToDo.Features.Logins.DTO;
public class LoginEndpoint : IApiEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder route)
    {
        var loginProcess = route.MapGroup("api/Login").WithTags("Login");
        loginProcess.MapPost("/", async (ToDo.Features.Logins.Services.IAuthenticationServices _services, [FromBody] ToDo.Features.Logins.DTO.LoginRequest loginRequest) =>
        {
            try
            {
                var loginResponse = await _services.Login(loginRequest.Username, loginRequest.Password);
                if(loginResponse.Result ==1)
                {
                    return Results.Ok(loginResponse);
                }
                else if(loginResponse.Result==2)
                {
                    return Results.Ok(loginResponse);
                }
                else 
                    return Results.BadRequest();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
            .WithName("UserLogin")
            .WithSummary("User Login")
            .WithDescription("User Login")
            .Produces<ToDo.Features.Logins.DTO.LoginResponse>(StatusCodes.Status200OK);
        loginProcess.MapPost("/otp-verify", async (IOTPServices _otpServices, OTPRequest otpInput, ITokenServices _tokenServices) =>
        {
            try
            {
                var verified = await _otpServices.VerifyTotpAsync(otpInput);
                if (verified.Result==1)
                {
                    return Results.Ok(verified);
                }
                return Results.BadRequest(new { Message = "Invalid OTP" });
            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
        loginProcess.MapGet("/otp-resent/{userId:int}", async (IOTPServices _otpServices,int userId) =>
        {
            try
            {
                var resent = await _otpServices.ReSentOTP(userId);
                if (resent)
                {
                    return Results.Ok(new { Message = "OTP Resent Successfully" });
                }
                return Results.BadRequest(new { Message = "Failed to Resend OTP" });
            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}

