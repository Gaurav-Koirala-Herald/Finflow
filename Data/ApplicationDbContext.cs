using Microsoft.EntityFrameworkCore;
using FinflowAPI. Models.Entities;

namespace FinflowAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<LoginHistory> LoginHistories { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }
    public DbSet<TransactionCategory> TransactionCategories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder. Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e. Email).HasMaxLength(255).IsRequired();
            entity. Property(e => e. PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(150).IsRequired();
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e. Gender).HasMaxLength(20);
            entity. Property(e => e.ProfilePicture).HasMaxLength(500);
            entity.Property(e => e.PreferredLanguage).HasMaxLength(10).HasDefaultValue("en");
        });

        // UserSettings Configuration
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e. SettingId);
            entity.HasIndex(e => e. UserId).IsUnique();
            entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("NPR");
            entity.Property(e => e.DateFormat).HasMaxLength(20).HasDefaultValue("YYYY-MM-DD");
            entity.Property(e => e.TimeFormat).HasMaxLength(10).HasDefaultValue("24h");
            entity.Property(e => e.FontSize).HasMaxLength(20).HasDefaultValue("Medium");

            entity.HasOne(e => e. User)
                .WithOne(e => e.UserSettings)
                .HasForeignKey<UserSettings>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Role Configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity. HasKey(e => e.RoleId);
            entity. HasIndex(e => e.RoleName).IsUnique();
            entity. Property(e => e.RoleName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        // UserRole Configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId);
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();

            entity.HasOne(e => e. User)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior. Restrict);
        });

        // RefreshToken Configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);
            entity. HasIndex(e => e.Token);
            entity.Property(e => e. Token).HasMaxLength(500).IsRequired();

            entity.HasOne(e => e. User)
                .WithMany(e => e.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // LoginHistory Configuration
        modelBuilder. Entity<LoginHistory>(entity =>
        {
            entity.HasKey(e => e.LoginId);
            entity.HasIndex(e => e. UserId);
            entity.Property(e => e. IPAddress).HasMaxLength(50);
            entity. Property(e => e.DeviceInfo).HasMaxLength(500);

            entity.HasOne(e => e. User)
                .WithMany(e => e.LoginHistories)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TransactionType Configuration
        modelBuilder. Entity<TransactionType>(entity =>
        {
            entity. HasKey(e => e.TransactionTypeId);
            entity.HasIndex(e => e.TypeName).IsUnique();
            entity.Property(e => e. TypeName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        // TransactionCategory Configuration
        modelBuilder.Entity<TransactionCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);
            entity. Property(e => e. CategoryName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e. Icon).HasMaxLength(100);

            entity. HasOne(e => e.TransactionType)
                .WithMany(e => e.TransactionCategories)
                .HasForeignKey(e => e.TransactionTypeId)
                .OnDelete(DeleteBehavior. Restrict);
        });

        // Transaction Configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity. HasKey(e => e.TransactionId);
            entity. HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.CategoryId);
            entity.Property(e => e. Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);

            entity.HasOne(e => e. User)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e. TransactionType)
                .WithMany(e => e. Transactions)
                .HasForeignKey(e => e.TransactionTypeId)
                .OnDelete(DeleteBehavior. Restrict);

            entity.HasOne(e => e.Category)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior. Restrict);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "Admin", Description = "System administrator with full access", IsActive = true },
            new Role { RoleId = 2, RoleName = "User", Description = "Regular application user", IsActive = true }
        );

        // Seed Transaction Types
        modelBuilder.Entity<TransactionType>().HasData(
            new TransactionType { TransactionTypeId = 1, TypeName = "Income", Description = "Money received or earned", IsActive = true },
            new TransactionType { TransactionTypeId = 2, TypeName = "Expense", Description = "Money spent or paid out", IsActive = true }
        );

        // Seed Transaction Categories - Income
        modelBuilder. Entity<TransactionCategory>().HasData(
            new TransactionCategory { CategoryId = 1, TransactionTypeId = 1, CategoryName = "Salary", Description = "Monthly salary income", Icon = "wallet", IsActive = true },
            new TransactionCategory { CategoryId = 2, TransactionTypeId = 1, CategoryName = "Business", Description = "Business income", Icon = "briefcase", IsActive = true },
            new TransactionCategory { CategoryId = 3, TransactionTypeId = 1, CategoryName = "Investment", Description = "Returns from investments", Icon = "trending-up", IsActive = true },
            new TransactionCategory { CategoryId = 4, TransactionTypeId = 1, CategoryName = "Gift", Description = "Gift received", Icon = "gift", IsActive = true },
            new TransactionCategory { CategoryId = 5, TransactionTypeId = 1, CategoryName = "Other Income", Description = "Miscellaneous income", Icon = "plus-circle", IsActive = true },
            // Expense Categories
            new TransactionCategory { CategoryId = 6, TransactionTypeId = 2, CategoryName = "Food & Dining", Description = "Meals and groceries", Icon = "utensils", IsActive = true },
            new TransactionCategory { CategoryId = 7, TransactionTypeId = 2, CategoryName = "Transportation", Description = "Travel and commute", Icon = "car", IsActive = true },
            new TransactionCategory { CategoryId = 8, TransactionTypeId = 2, CategoryName = "Shopping", Description = "Retail purchases", Icon = "shopping-bag", IsActive = true },
            new TransactionCategory { CategoryId = 9, TransactionTypeId = 2, CategoryName = "Bills & Utilities", Description = "Monthly bills", Icon = "file-text", IsActive = true },
            new TransactionCategory { CategoryId = 10, TransactionTypeId = 2, CategoryName = "Entertainment", Description = "Movies, games, etc.", Icon = "film", IsActive = true },
            new TransactionCategory { CategoryId = 11, TransactionTypeId = 2, CategoryName = "Healthcare", Description = "Medical expenses", Icon = "heart", IsActive = true },
            new TransactionCategory { CategoryId = 12, TransactionTypeId = 2, CategoryName = "Education", Description = "Learning and courses", Icon = "book", IsActive = true },
            new TransactionCategory { CategoryId = 13, TransactionTypeId = 2, CategoryName = "Rent", Description = "Housing rent", Icon = "home", IsActive = true },
            new TransactionCategory { CategoryId = 14, TransactionTypeId = 2, CategoryName = "Other Expense", Description = "Miscellaneous expenses", Icon = "minus-circle", IsActive = true }
        );
    }
}