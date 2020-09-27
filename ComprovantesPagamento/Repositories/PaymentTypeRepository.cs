using ComprovantesPagamento.Contracts;
using ComprovantesPagamento.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComprovantesPagamento.Repositories
{
    public class PaymentTypeRepository : BaseRepository<PaymentType>
    {
        public PaymentTypeRepository(IDbService dbService) : base(dbService, "payment_types")
        {

        }

        public IEnumerable<PaymentType> List(string userId)
        {
            try
            {
                return Collection.Find(Filter.Eq("user_id", userId))
                    .ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public PaymentType GetByUserID(string userId, string id)
        {
            try
            {
                var type = Collection.Find(Filter.And(FilterUserID(userId), FilterId(id)))
                    .FirstOrDefault();

                return type;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public PaymentType GetByCode(string userId, string Code)
        {
            try
            {
                var type = Collection.Find(Filter.And(Filter.Eq("user_id", userId), Filter.Eq("code", Code)))
                    .FirstOrDefault();

                return type;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
