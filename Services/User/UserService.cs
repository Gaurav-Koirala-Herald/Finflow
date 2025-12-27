using Microsoft.EntityFrameworkCore;
using FinFlowAPI.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.Models;


namespace FinFlowAPI.Services.User
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur. Role)
                . Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u. Username,
                    Email = u.Email,
                    FullName = u. FullName,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    Roles = u.UserRoles.Select(ur => new RoleDto
                    {
                        Id = ur.Role.Id,
                        Name = ur. Role.Name,
                        Description = ur.Role.Description
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var users = await _context.Users.ToListAsync();
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur. Role)
                . FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user. Email,
                FullName = user. FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = user.UserRoles.Select(ur => new RoleDto
                {
                    Id = ur. Role.Id,
                    Name = ur.Role.Name,
                    Description = ur.Role.Description
                }).ToList()
            };
        }

        public async Task<UserDto?> CreateUserAsync(CreateUserDto dto, int createdById)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username || u. Email == dto.Email))
                return null;

            var user = new Models.User
            {
                Username = dto.Username,
                Email = dto. Email,
                // PasswordHash = dto.Password,
                FullName = dto.FullName,
                IsActive = true,
                CreatedAt = DateTime. UtcNow
            };

            _context.Users. Add(user);
            await _context. SaveChangesAsync();

            // Assign roles
            foreach (var roleId in dto.RoleIds)
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

            return await GetUserByIdAsync(user.Id);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.Email = dto.Email;
            user. FullName = dto. FullName;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime. UtcNow;

            await _context.SaveChangesAsync();
            return await GetUserByIdAsync(id);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur. Role)
                . FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return false;

            // Prevent deleting SuperAdmin
            if (user.UserRoles.Any(ur => ur.Role.IsSystemRole && ur. Role.Name == "SuperAdmin"))
                return false;

            _context.Users. Remove(user);
            await _context. SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignRolesAsync(AssignRolesDto dto, int assignedById)
        {
            var users = await _context.Users.ToListAsync();
            var user = await _context.Users
                .Include(u => u.UserRoles)
                . FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null) return false;

            // Remove existing roles
            _context.UserRoles.RemoveRange(user.UserRoles);
            // Add new roles
            foreach (var roleId in dto.RoleIds)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = dto.UserId,
                    RoleId = roleId,
                    AssignedById = assignedById,
                    AssignedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FunctionDto>> GetFunctionsByUserIdAsync(int Id)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == Id);
            if (existingUser == null)
            {
                return null;
            }

            var userRole = existingUser.UserRoles.ToList();
            
            var userFunctions = _context.RoleFunctions.Where(w => userRole.Any(ur => ur.RoleId == w.RoleId))
                .Include(roleFunction => roleFunction.Function).ToList();
            
            return userFunctions.Select(s => new FunctionDto
            {
                Id = s.FunctionId,
                Code = s.Function.Code,
                Name = s.Function.Name,
                Description = s.Function.Description
            }).ToList();    
            
        }

        public async Task<bool> UpdateUserProfileAsync(UserDto userDto)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(x=>x.Id == userDto.Id);

            if (existingUser == null) return false;
            
            existingUser.FullName = userDto.FullName;
            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.Username = userDto.Username;
            existingUser.Email = userDto.Email;
            
            _context.Users.Update(existingUser);
            
            await _context.SaveChangesAsync();

            return true;
        }
    }
}