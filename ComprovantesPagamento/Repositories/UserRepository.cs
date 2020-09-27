using ComprovantesPagamento.Contracts;
using ComprovantesPagamento.Domain.Models;
using MongoDB.Driver;

namespace ComprovantesPagamento.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(IDbService dbService) : base(dbService, "users")
        {

        }

        public User GetByEmail(string email)
        {
            try
            {
                var user = Collection.Find(Filter.Eq("email", email))
                   .FirstOrDefault();

                return user;
            }
            catch (System.Exception)
            {

                throw;
            }
        }


    }
}
