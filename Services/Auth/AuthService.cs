using System.IdentityModel.Tokens. Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FinFlowAPI.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.Models;

namespace FinFlowAPI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur. Role)
                        .ThenInclude(r => r. RoleFunctions)
                            .ThenInclude(rf => rf. Function)
                . Include(u => u. UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePrivileges)
                            .ThenInclude(rp => rp. Privilege)
                . FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

            if (user == null || !request.Password.Contains(user.PasswordHash))
                return null;

            var roles = user.UserRoles.Select(ur => ur.Role. Name). Distinct().ToList();
            var functions = user.UserRoles
                .SelectMany(ur => ur.Role.RoleFunctions)
                .Select(rf => rf.Function.Code)
                . Distinct()
                .ToList();
            var privileges = user.UserRoles
                .SelectMany(ur => ur.Role.RolePrivileges)
                .Select(rp => rp. Privilege.Code)
                .Distinct()
                .ToList();

            var token = GenerateJwtToken(user. Id, user.Username,user.IsActive, roles, functions, privileges);

            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user. Email,
                FullName = user. FullName,
                Token = token,
                Roles = roles,
                Functions = functions,
                Privileges = privileges
            };
        }

        public async Task<UserDto?> RegisterAsync(RegisterRequestDto request, int createdById)
        {
            if (await _context. Users.AnyAsync(u => u. Username == request.Username || u.Email == request.Email))
                return null;

            var user = new Models.User
            {
                Username = request.Username,
                Email = request. Email,
                PasswordHash = request. Password,
                FullName = request. FullName,
                IsActive = true,
                CreatedAt = DateTime. UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign roles
            if (request.RoleIds. Any())
            {
                foreach (var roleId in request. RoleIds)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId,
                        AssignedById = createdById,
                        AssignedAt = DateTime. UtcNow
                    });
                }
                await _context. SaveChangesAsync();
            }

            return new UserDto
            {
                Id = user.Id,
                Username = user. Username,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public string GenerateJwtToken(int userId, string username,bool IsActive, List<string> roles, List<string> functions, List<string> privileges)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]! ));
            var credentials = new SigningCredentials(key, SecurityAlgorithms. HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(functions.Select(func => new Claim("function", func)));
            claims.AddRange(privileges.Select(priv => new Claim("privilege", priv)));
            claims.Add(new Claim("Status", IsActive.ToString()));
            

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime. UtcNow. AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}