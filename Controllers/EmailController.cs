using System.Net;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;

    public EmailController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
    {
        var result = await _emailService.SendOtpEmail(
            request.Email
        );

        if (result.code != HttpStatusCode.OK)
            return StatusCode(500, "Email failed");

        return Ok("Email sent");
    }
}

public class OtpRequest
{
    public string Email { get; set; }
}
