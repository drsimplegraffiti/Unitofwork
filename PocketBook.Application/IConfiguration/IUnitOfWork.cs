using PocketBook.Application.IRepositories;

namespace PocketBook.Application.IConfiguration
{
    public interface IUnitOfWork
    {
         IUserRepository Users { get; }
    Task CompleteAsync();
    }
}