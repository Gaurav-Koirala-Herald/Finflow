using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using FinFlowAPI.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.Models;
using FinFlowAPI.Services.Role;

namespace FinFlowAPI. API.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Include(r => r. CreatedBy)
                .Include(r => r. RoleFunctions)
                .Include(r => r.RolePrivileges)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r. Description,
                    IsSystemRole = r.IsSystemRole,
                    CreatedBy = r.CreatedBy != null ? r.CreatedBy. FullName : "System",
                    CreatedAt = r. CreatedAt,
                    FunctionIds = r. RoleFunctions. Select(rf => rf.FunctionId).ToList(),
                    PrivilegeIds = r.RolePrivileges.Select(rp => rp.PrivilegeId).ToList()
                })
                .ToListAsync();
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.CreatedBy)
                .Include(r => r. RoleFunctions)
                .Include(r => r.RolePrivileges)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role. Description,
                IsSystemRole = role. IsSystemRole,
                CreatedBy = role.CreatedBy?. FullName ?? "System",
                CreatedAt = role. CreatedAt,
                FunctionIds = role.RoleFunctions.Select(rf => rf.FunctionId).ToList(),
                PrivilegeIds = role. RolePrivileges.Select(rp => rp. PrivilegeId). ToList()
            };
        }

        public async Task<RoleDto?> CreateRoleAsync(CreateRoleDto dto, int createdById)
        {
            if (await _context. Roles.AnyAsync(r => r.Name == dto. Name))
                return null;

            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedById = createdById,
                CreatedAt = DateTime. UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return new RoleDto
            {
                Id = role.Id,
                Name = role. Name,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                CreatedAt = role.CreatedAt
            };
        }

        public async Task<RoleDto? > UpdateRoleAsync(int id, UpdateRoleDto dto)
        {
            var role = await _context.Roles. FindAsync(id);
            if (role == null || role.IsSystemRole) return null;

            role.Name = dto. Name;
            role.Description = dto. Description;
            role.UpdatedAt = DateTime. UtcNow;

            await _context.SaveChangesAsync();

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role. Description,
                IsSystemRole = role. IsSystemRole,
                CreatedAt = role.CreatedAt
            };
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context. Roles.FindAsync(id);
            if (role == null || role.IsSystemRole) return false;

            _context. Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignFunctionsAsync(AssignFunctionsDto dto)
        {
            string connectionString = _context.Database.GetDbConnection().ConnectionString;
            var connection = new SqlConnection(connectionString);
            if(dto.FunctionIds.Count == 0) dto.FunctionIds.Add(0);
            string sp = "assign_functions_to_role";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", dto.RoleId);
            parameters.Add("@FunctionIds", string.Join(',',dto.FunctionIds));

            // DataTable dataTable = new();
            // dataTable.Columns.Add("FunctionId",typeof(int));
            // foreach (var functionId in dto.FunctionIds)
            // {
            //     dataTable.Rows.Add(functionId);
            // }
            // parameters.Add("@FunctionIds", dataTable.AsTableValuedParameter("dbo.FunctionListTypes"));
            //
            var dbResp = await connection.QueryFirstOrDefaultAsync<int>(sp, parameters,commandType:CommandType.StoredProcedure);
            return dbResp == 1;
            
            
            
            
            
            // var role = await _context. Roles
            //     .Include(r => r.RoleFunctions)
            //     .FirstOrDefaultAsync(r => r.Id == dto. RoleId);
            //
            // if (role == null) return false;
            //
            // // Remove existing functions
            // _context.RoleFunctions.RemoveRange(role.RoleFunctions);
            //
            // // Add new functions
            // foreach (var functionId in dto.FunctionIds)
            // {
            //     _context.RoleFunctions.Add(new RoleFunction
            //     {
            //         RoleId = dto.RoleId,
            //         FunctionId = functionId,
            //         AssignedAt = DateTime. UtcNow
            //     });
            // }
            //
            // await _context.SaveChangesAsync();
            // return true;
            }

        public async Task<bool> AssignPrivilegesAsync(AssignPrivilegesDto dto)
        {
            var role = await _context. Roles
                . Include(r => r.RolePrivileges)
                . FirstOrDefaultAsync(r => r.Id == dto.RoleId);

            if (role == null) return false;

            // Remove existing privileges
            _context.RolePrivileges. RemoveRange(role.RolePrivileges);

            // Add new privileges
            foreach (var privilegeId in dto. PrivilegeIds)
            {
                _context.RolePrivileges.Add(new RolePrivilege
                {
                    RoleId = dto.RoleId,
                    PrivilegeId = privilegeId,
                    AssignedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ModuleDto>> GetModulesWithFunctionsAndPrivilegesAsync()
        {
            return await _context. Modules
                .Where(m => m. ParentModuleId == null && m.IsActive)
                .Include(m => m.Functions. Where(f => f. IsActive))
                .Include(m => m.Privileges. Where(p => p.IsActive))
                .Include(m => m. SubModules)
                    .ThenInclude(sm => sm.Functions. Where(f => f.IsActive))
                . Include(m => m. SubModules)
                    .ThenInclude(sm => sm.Privileges. Where(p => p.IsActive))
                .OrderBy(m => m.DisplayOrder)
                .Select(m => new ModuleDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Icon = m.Icon,
                    DisplayOrder = m.DisplayOrder,
                    Functions = m.Functions.Select(f => new FunctionDto
                    {
                        Id = f. Id,
                        Name = f.Name,
                        Code = f.Code,
                        Description = f.Description,
                        ModuleId = f.ModuleId
                    }).ToList(),
                    Privileges = m. Privileges.Select(p => new PrivilegeDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code,
                        Description = p.Description,
                        ModuleId = p.ModuleId
                    }). ToList(),
                    SubModules = m.SubModules.Select(sm => new ModuleDto
                    {
                        Id = sm. Id,
                        Name = sm.Name,
                        Description = sm.Description,
                        Functions = sm.Functions. Select(f => new FunctionDto
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Code = f.Code,
                            ModuleId = f. ModuleId
                        }).ToList(),
                        Privileges = sm.Privileges.Select(p => new PrivilegeDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Code = p. Code,
                            ModuleId = p.ModuleId
                        }).ToList()
                    }). ToList()
                })
                .ToListAsync();
        }

        public async Task<List<FunctionDto>> GetFunctionsByRoleAsync(int roleId)
        {
            var existingRole = await _context.Roles.SingleOrDefaultAsync(e=>e.Id == roleId);
            
            if(existingRole == null) return null;
            
            var roleFunctions = await _context.RoleFunctions.Where(w => w.RoleId == roleId)
                .Include(roleFunction => roleFunction.Function).ToListAsync();
            
            return roleFunctions.Select(s => new FunctionDto
            {
                Id = s.FunctionId,
                Name = s.Function.Name,
                Code = s.Function.Code,
                Description = s.Function.Description,
                ModuleId = s.Function.ModuleId
            }).ToList();
        }
    }
}