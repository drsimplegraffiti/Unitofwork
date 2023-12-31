
using Microsoft.EntityFrameworkCore;
using PocketBook.Application.IRepositories;
using PocketBook.Domain;
using PocketBook.Infrastructure;

namespace PocketBook.Application.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context, ILogger logger) : base(context, logger) { }

        public override async Task<IEnumerable<User>> All() // we override the All method from GenericRepository, possible because it was virtual
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(UserRepository));
                return new List<User>();
            }
        }

        public override async Task<bool> Upsert(User entity)
        {
            try
            {
                var existingUser = await dbSet.Where(x => x.Id == entity.Id)
                                                    .FirstOrDefaultAsync();

                if (existingUser == null)
                    return await Add(entity);

                existingUser.FirstName = entity.FirstName;
                existingUser.LastName = entity.LastName;
                existingUser.Email = entity.Email;
                existingUser.Password = entity.Password;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert function error", typeof(UserRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.Id == id)
                                        .FirstOrDefaultAsync();

                if (exist == null) return false;

                dbSet.Remove(exist);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete function error", typeof(UserRepository));
                return false;
            }
        }

        public override async Task<User> GetById(Guid id)
        {
            try
            {
                return await dbSet.Where(x => x.Id == id)
                                        .FirstOrDefaultAsync() ?? throw new Exception("Not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetById function error", typeof(UserRepository));
                return null!;
            }
        }


        // GetByEmail is a custom method that is not in GenericRepository
        public async Task<User> GetByEmail(string email)
        {
            try
            {
                return await dbSet.Where(x => x.Email == email)
                                        .FirstOrDefaultAsync() ?? throw new Exception("Not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetByEmail function error", typeof(UserRepository));
                return null!;
            }
        }


    
        
    }
}