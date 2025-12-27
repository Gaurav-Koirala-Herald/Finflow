namespace FinflowAPI.DbImplementation. Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    // IRoleRepository Roles { get; }
    // IRefreshTokenRepository RefreshTokens { get; }
    // ILoginHistoryRepository LoginHistories { get; }
    // ITransactionRepository Transactions { get; }
    // ITransactionTypeRepository TransactionTypes { get; }
    // ITransactionCategoryRepository TransactionCategories { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}