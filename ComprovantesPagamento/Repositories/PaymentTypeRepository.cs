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


        public PaymentType GetByCode(string Code)
        {
            try
            {
                var type = Collection.Find(Filter.Eq("code", Code))
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
