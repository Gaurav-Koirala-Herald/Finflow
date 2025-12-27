using Microsoft. EntityFrameworkCore;
using FinFlowAPI.Models;

namespace FinFlowAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleFunction> RoleFunctions { get; set; }
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get;set; }
        public DbSet<PostInteraction> PostInteractions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================
            // USER CONFIGURATION
            // ============================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                
                entity.HasIndex(u => u.Username). IsUnique();
                entity.HasIndex(u => u. Email).IsUnique();
                
                entity.Property(u => u.Username).IsRequired(). HasMaxLength(50);
                entity. Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u. FullName).HasMaxLength(100);
            });
            modelBuilder.Entity<PostInteraction>()
                .HasIndex(pi => new { pi.PostId, pi.UserId, pi.Type })
                .IsUnique(); // 1 like per user per post
            // ============================================
            // ROLE CONFIGURATION
            // ============================================
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                
                entity.HasIndex(r => r.Name).IsUnique();
                
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
                entity.Property(r => r. Description).HasMaxLength(200);

                // Role -> CreatedBy User relationship
                entity. HasOne(r => r.CreatedBy)
                    .WithMany()
                    .HasForeignKey(r => r.CreatedById)
                    . OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            // ============================================
            // MODULE CONFIGURATION
            // ============================================
            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(m => m.Id);
                
                entity.Property(m => m. Name).IsRequired(). HasMaxLength(100);
                entity. Property(m => m.Description).HasMaxLength(200);
                entity.Property(m => m.Icon).HasMaxLength(100);

                // Self-referencing relationship for parent/child modules
                entity.HasOne(m => m.ParentModule)
                    .WithMany(m => m.SubModules)
                    .HasForeignKey(m => m.ParentModuleId)
                    .OnDelete(DeleteBehavior. Restrict)
                    . IsRequired(false);
            });

            // ============================================
            // FUNCTION CONFIGURATION
            // ============================================
            modelBuilder. Entity<Function>(entity =>
            {
                entity.HasKey(f => f. Id);
                
                entity.HasIndex(f => f.Code).IsUnique();
                
                entity.Property(f => f.Name).IsRequired().HasMaxLength(100);
                entity.Property(f => f. Code).IsRequired(). HasMaxLength(100);
                entity. Property(f => f.Description).HasMaxLength(200);

                // Function -> Module relationship
                entity. HasOne(f => f.Module)
                    .WithMany(m => m. Functions)
                    .HasForeignKey(f => f. ModuleId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ============================================
            // PRIVILEGE CONFIGURATION
            // ============================================
            modelBuilder.Entity<Privilege>(entity =>
            {
                entity.HasKey(p => p.Id);
                
                entity.HasIndex(p => p.Code).IsUnique();
                
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity. Property(p => p.Code).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(200);

                // Privilege -> Module relationship
                entity. HasOne(p => p.Module)
                    .WithMany(m => m. Privileges)
                    .HasForeignKey(p => p.ModuleId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ============================================
            // USER ROLE (JOIN TABLE) CONFIGURATION
            // ============================================
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => ur.Id);
                
                // Composite unique constraint
                entity.HasIndex(ur => new { ur.UserId, ur.RoleId }). IsUnique();

                // UserRole -> User relationship
                entity.HasOne(ur => ur. User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur. UserId)
                    .OnDelete(DeleteBehavior. NoAction);

                // UserRole -> Role relationship
                entity. HasOne(ur => ur.Role)
                    .WithMany(r => r. UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    . OnDelete(DeleteBehavior.NoAction);

                // UserRole -> AssignedBy User relationship (FIXED!)
                entity.HasOne(ur => ur.AssignedBy)
                    . WithMany() // No navigation property on User side
                    .HasForeignKey(ur => ur.AssignedById)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            // ============================================
            // ROLE FUNCTION (JOIN TABLE) CONFIGURATION
            // ============================================
            modelBuilder.Entity<RoleFunction>(entity =>
            {
                entity.HasKey(rf => rf.Id);
                
                // Composite unique constraint
                entity.HasIndex(rf => new { rf. RoleId, rf.FunctionId }). IsUnique();

                // RoleFunction -> Role relationship
                entity.HasOne(rf => rf.Role)
                    . WithMany(r => r.RoleFunctions)
                    .HasForeignKey(rf => rf. RoleId)
                    .OnDelete(DeleteBehavior.NoAction);

                // RoleFunction -> Function relationship
                entity.HasOne(rf => rf.Function)
                    . WithMany(f => f.RoleFunctions)
                    .HasForeignKey(rf => rf. FunctionId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ============================================
            // ROLE PRIVILEGE (JOIN TABLE) CONFIGURATION
            // ============================================
            modelBuilder.Entity<RolePrivilege>(entity =>
            {
                entity.HasKey(rp => rp.Id);
                
                // Composite unique constraint
                entity.HasIndex(rp => new { rp.RoleId, rp.PrivilegeId }).IsUnique();

                // RolePrivilege -> Role relationship
                entity.HasOne(rp => rp. Role)
                    .WithMany(r => r.RolePrivileges)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior. NoAction);

                // RolePrivilege -> Privilege relationship
                entity.HasOne(rp => rp.Privilege)
                    . WithMany(p => p.RolePrivileges)
                    .HasForeignKey(rp => rp.PrivilegeId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // ============================================
            // SEED MODULES
            // ============================================
            modelBuilder.Entity<Module>(). HasData(
                new Module { Id = 1, Name = "Dashboard Management", Description = "Dashboard related features", DisplayOrder = 1, IsActive = true },
                new Module { Id = 2, Name = "Staff Management", Description = "Manage staff/teachers", DisplayOrder = 2, IsActive = true },
                new Module { Id = 3, Name = "Student Management", Description = "Manage students", DisplayOrder = 3, IsActive = true },
                new Module { Id = 4, Name = "Location Management", Description = "Manage locations", DisplayOrder = 4, IsActive = true },
                new Module { Id = 5, Name = "Reports", Description = "View and generate reports", DisplayOrder = 5, IsActive = true },
                new Module { Id = 6, Name = "Roles Management", Description = "Manage roles and permissions", DisplayOrder = 6, IsActive = true }
            );

            // ============================================
            // SEED FUNCTIONS
            // ============================================
            modelBuilder.Entity<Function>().HasData(
                // Dashboard Functions
                new Function { Id = 1, Name = "Preview Dashboard", Code = "PREVIEW_DASHBOARD", Description = "View dashboard previews", ModuleId = 1, IsActive = true },
                new Function { Id = 2, Name = "Filter Date", Code = "FILTER_DATE", Description = "Filter data by date", ModuleId = 1, IsActive = true },
                new Function { Id = 3, Name = "Download", Code = "DOWNLOAD", Description = "Download reports/data", ModuleId = 1, IsActive = true },
                
                // Staff Functions
                new Function { Id = 4, Name = "Add Staff", Code = "ADD_STAFF", Description = "Add new staff member", ModuleId = 2, IsActive = true },
                new Function { Id = 5, Name = "Edit Staff", Code = "EDIT_STAFF", Description = "Edit staff information", ModuleId = 2, IsActive = true },
                new Function { Id = 6, Name = "Delete Staff", Code = "DELETE_STAFF", Description = "Delete staff member", ModuleId = 2, IsActive = true },
                
                // Student Functions
                new Function { Id = 7, Name = "Add Student", Code = "ADD_STUDENT", Description = "Add new student", ModuleId = 3, IsActive = true },
                new Function { Id = 8, Name = "Edit Student", Code = "EDIT_STUDENT", Description = "Edit student information", ModuleId = 3, IsActive = true },
                new Function { Id = 9, Name = "Delete Student", Code = "DELETE_STUDENT", Description = "Delete student", ModuleId = 3, IsActive = true },
                
                // Location Functions
                new Function { Id = 10, Name = "Add Location", Code = "ADD_LOCATION", Description = "Add new location", ModuleId = 4, IsActive = true },
                new Function { Id = 11, Name = "Edit Location", Code = "EDIT_LOCATION", Description = "Edit location information", ModuleId = 4, IsActive = true },
                new Function { Id = 12, Name = "Delete Location", Code = "DELETE_LOCATION", Description = "Delete location", ModuleId = 4, IsActive = true }
            );

            // ============================================
            // SEED PRIVILEGES
            // ============================================
            modelBuilder.Entity<Privilege>().HasData(
                // Dashboard Privileges
                new Privilege { Id = 1, Name = "View Statistics", Code = "VIEW_STATISTICS", Description = "View dashboard statistics", ModuleId = 1, IsActive = true },
                new Privilege { Id = 2, Name = "View Enrollment Analytics", Code = "VIEW_ENROLLMENT_ANALYTICS", Description = "View enrollment analytics", ModuleId = 1, IsActive = true },
                new Privilege { Id = 3, Name = "View Application Overview", Code = "VIEW_APPLICATION_OVERVIEW", Description = "View application overview", ModuleId = 1, IsActive = true },
                new Privilege { Id = 4, Name = "View Intakes Status", Code = "VIEW_INTAKES_STATUS", Description = "View intake status", ModuleId = 1, IsActive = true },
                
                // Staff Privileges
                new Privilege { Id = 5, Name = "View Staff List", Code = "VIEW_STAFF_LIST", Description = "View list of staff", ModuleId = 2, IsActive = true },
                new Privilege { Id = 6, Name = "View Staff Details", Code = "VIEW_STAFF_DETAILS", Description = "View staff details", ModuleId = 2, IsActive = true },
                new Privilege { Id = 7, Name = "View Teacher Report", Code = "VIEW_TEACHER_REPORT", Description = "View teacher reports", ModuleId = 2, IsActive = true },
                
                // Student Privileges
                new Privilege { Id = 8, Name = "View Student List", Code = "VIEW_STUDENT_LIST", Description = "View list of students", ModuleId = 3, IsActive = true },
                new Privilege { Id = 9, Name = "View Student Details", Code = "VIEW_STUDENT_DETAILS", Description = "View student details", ModuleId = 3, IsActive = true },
                new Privilege { Id = 10, Name = "View Student Report", Code = "VIEW_STUDENT_REPORT", Description = "View student reports", ModuleId = 3, IsActive = true },
                
                // Location Privileges
                new Privilege { Id = 11, Name = "View Location List", Code = "VIEW_LOCATION_LIST", Description = "View list of locations", ModuleId = 4, IsActive = true },
                new Privilege { Id = 12, Name = "View Location Details", Code = "VIEW_LOCATION_DETAILS", Description = "View location details", ModuleId = 4, IsActive = true },
                
                // Role Privileges
                new Privilege { Id = 13, Name = "View Roles", Code = "VIEW_ROLES", Description = "View all roles", ModuleId = 6, IsActive = true }
            );

            // ============================================
            // SEED ROLES
            // ============================================
            modelBuilder.Entity<Role>().HasData(
                new Role 
                { 
                    Id = 1, 
                    Name = "SuperAdmin", 
                    Description = "Full system access - can manage everything", 
                    IsSystemRole = true, 
                    CreatedById = null,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind. Utc)
                },
                new Role 
                { 
                    Id = 2, 
                    Name = "Teacher", 
                    Description = "Teacher role - can manage students, view own reports", 
                    IsSystemRole = false,
                    CreatedById = null,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind. Utc)
                },
                new Role 
                { 
                    Id = 3, 
                    Name = "SchoolAdministration", 
                    Description = "School admin - can view students and teachers, but cannot delete", 
                    IsSystemRole = false,
                    CreatedById = null,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // ============================================
            // SEED SUPER ADMIN USER (Password: Admin@123)
            // ============================================
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "superadmin",
                    Email = "superadmin@ums.com",
                    PasswordHash = "Admin@123", // BCrypt hash of "Admin@123"
                    FullName = "Super Administrator",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // ============================================
            // ASSIGN SUPERADMIN ROLE TO SUPERADMIN USER
            // ============================================
            modelBuilder.Entity<UserRole>(). HasData(
                new UserRole 
                { 
                    Id = 1, 
                    UserId = 1, 
                    RoleId = 1, 
                    AssignedById = null,
                    AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind. Utc)
                }
            );

            // ============================================
            // ASSIGN ALL FUNCTIONS TO SUPERADMIN ROLE
            // ============================================
            var roleFunctions = new List<RoleFunction>();
            for (int i = 1; i <= 12; i++)
            {
                roleFunctions.Add(new RoleFunction 
                { 
                    Id = i, 
                    RoleId = 1, 
                    FunctionId = i, 
                    AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            }
            modelBuilder.Entity<RoleFunction>().HasData(roleFunctions);

            // ============================================
            // ASSIGN ALL PRIVILEGES TO SUPERADMIN ROLE
            // ============================================
            var rolePrivileges = new List<RolePrivilege>();
            for (int i = 1; i <= 13; i++)
            {
                rolePrivileges. Add(new RolePrivilege 
                { 
                    Id = i, 
                    RoleId = 1, 
                    PrivilegeId = i, 
                    AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            }
            modelBuilder.Entity<RolePrivilege>().HasData(rolePrivileges);

            // ============================================
            // ASSIGN PERMISSIONS TO TEACHER ROLE
            // ============================================
            // Teacher can: manage students, view student reports, but NOT view teacher reports
            modelBuilder.Entity<RoleFunction>().HasData(
                new RoleFunction { Id = 13, RoleId = 2, FunctionId = 7, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }, // ADD_STUDENT
                new RoleFunction { Id = 14, RoleId = 2, FunctionId = 8, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind. Utc) }  // EDIT_STUDENT
            );

            modelBuilder.Entity<RolePrivilege>().HasData(
                new RolePrivilege { Id = 14, RoleId = 2, PrivilegeId = 8, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },  // VIEW_STUDENT_LIST
                new RolePrivilege { Id = 15, RoleId = 2, PrivilegeId = 9, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },  // VIEW_STUDENT_DETAILS
                new RolePrivilege { Id = 16, RoleId = 2, PrivilegeId = 10, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind. Utc) }  // VIEW_STUDENT_REPORT
            );

            // ============================================
            // ASSIGN PERMISSIONS TO SCHOOL ADMINISTRATION ROLE
            // ============================================
            // School Admin can: view students & teachers, view reports, but NOT delete
            modelBuilder.Entity<RolePrivilege>().HasData(
                new RolePrivilege { Id = 17, RoleId = 3, PrivilegeId = 5, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },  // VIEW_STAFF_LIST
                new RolePrivilege { Id = 18, RoleId = 3, PrivilegeId = 6, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },  // VIEW_STAFF_DETAILS
                new RolePrivilege { Id = 19, RoleId = 3, PrivilegeId = 7, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },  // VIEW_TEACHER_REPORT
                new RolePrivilege { Id = 20, RoleId = 3, PrivilegeId = 8, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },  // VIEW_STUDENT_LIST
                new RolePrivilege { Id = 21, RoleId = 3, PrivilegeId = 9, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },  // VIEW_STUDENT_DETAILS
                new RolePrivilege { Id = 22, RoleId = 3, PrivilegeId = 10, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }  // VIEW_STUDENT_REPORT
            );
        }
    }
}