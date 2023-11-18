using PocketBook.Core.IRepositories;

namespace PocketBook.Core.IConfiguration;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    Task CompleteAsync();

    // others
    IDisposable BeginTransaction();

    void CommitTransaction();

    void RollbackTransaction();

    void Dispose();

    Task<int> SaveChangesAsync();

    int SaveChanges();

    void SaveChangesWithNoReturn();


}