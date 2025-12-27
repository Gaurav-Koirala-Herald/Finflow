using Microsoft. EntityFrameworkCore. Storage;
using FinflowAPI. Data;
using FinflowAPI. DbImplementation. Interfaces;
using FinflowAPI. DbImplementation.Interfaces;

namespace FinflowAPI.DbImplementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction?  _transaction;

    private IUserRepository?  _users;
    // private IRoleRepository? _roles;
    // private IRefreshTokenRepository? _refreshTokens;
    // private ILoginHistoryRepository? _loginHistories;
    // private ITransactionRepository?  _transactions;
    // private ITransactionTypeRepository? _transactionTypes;
    // private ITransactionCategoryRepository? _transactionCategories;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // public IUserRepository Users =>
    //     _users ??= new UserRepository(_context);
    //
    // public IRoleRepository Roles =>
    //     _roles ??= new RoleRepository(_context);
    //
    // public IRefreshTokenRepository RefreshTokens =>
    //     _refreshTokens ??= new RefreshTokenRepository(_context);
    //
    // public ILoginHistoryRepository LoginHistories =>
    //     _loginHistories ??= new LoginHistoryRepository(_context);
    //
    // public ITransactionRepository Transactions =>
    //     _transactions ??= new TransactionRepository(_context);
    //
    // public ITransactionTypeRepository TransactionTypes =>
    //     _transactionTypes ??= new TransactionTypeRepository(_context);
    //
    // public ITransactionCategoryRepository TransactionCategories =>
    //     _transactionCategories ??= new TransactionCategoryRepository(_context);

    public IUserRepository Users
    { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context. Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction. DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}