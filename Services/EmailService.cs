using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FinFlowAPI.Data;
using FinFlowAPI.Services;
using Microsoft.EntityFrameworkCore;

public class EmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ApplicationDbContext _context;
    private readonly CommonService _commonService;

    public EmailService(CommonService commonService, ApplicationDbContext context, HttpClient httpClient, IConfiguration configuration)
    {
        _commonService = commonService;
        _context = context;
        _httpClient = httpClient;
        _apiKey = configuration["Resend:ApiKey"];
    }

    public async Task<CommonResponseDTO> SendOtpEmail(string userEmail)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");

        var user = await _context.Users
                       .SingleOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
            return new CommonResponseDTO { code = HttpStatusCode.BadRequest, message = "User not found" };

        var otp = _commonService.GenerateOTP();
        user.otp = otp;
        user.OtpCreatedDateTime = DateTime.Now;
        user.IsOtpVerified = false;
        await _context.SaveChangesAsync();
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");


        string html = $@"
<div style='font-family: Helvetica, Arial, sans-serif; background-color:#f3f6fb; padding:30px 15px;'>
  <table width='100%' cellpadding='0' cellspacing='0' 
    style='max-width:600px; margin:0 auto; background:#ffffff; border-radius:10px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.05);'>
    
    <thead>
      <tr>
        <th style='background:#1e3a8a; padding:25px; color:#ffffff; font-size:26px; text-align:center;'>
          FINFLOW
        </th>
      </tr>
    </thead>

    <tbody>
      <tr>
        <td style='padding:35px;'>
          <h2 style='color:#111827; font-size:20px;'>Hello {user.Username},</h2>
          <p style='color:#4b5563; font-size:16px; line-height:1.6;'>
            Use the OTP below to verify your FinFlow account. 
            This code is valid for <strong>5 minutes</strong>.
          </p>

          <div style='text-align:center; margin:30px 0;'>
            <span style='display:inline-block; font-size:28px; font-weight:bold;
              padding:18px 35px; background:#eef2ff; border-radius:8px;
              letter-spacing:8px; border:2px dashed #c7d2fe;'>
              {otp}
            </span>
          </div>

          <p style='color:#6b7280; font-size:14px;'>
            If you did not request this, please ignore this email.
          </p>
        </td>
      </tr>
    </tbody>

    <tfoot>
      <tr>
        <td style='padding:20px; text-align:center; background:#f9fafb;
          color:#9ca3af; font-size:12px;'>
          © {DateTime.Now.Year} FinFlow. All rights reserved.
        </td>
      </tr>
    </tfoot>
  </table>
</div>";

        var body = new
        {
            from = "Finflow <finflow@koiralagaurav.com.np>",
            to = new[] { userEmail },
            subject = "OTP Verification",
            html = html
        };

        request.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return new CommonResponseDTO { code = HttpStatusCode.OK, message = "OTP email sent successfully" };
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return new CommonResponseDTO { code = HttpStatusCode.InternalServerError, message = $"Failed to send OTP email: {errorContent}" };
        }
    }
}
