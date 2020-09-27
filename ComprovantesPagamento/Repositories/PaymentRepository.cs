using ComprovantesPagamento.Contracts;
using ComprovantesPagamento.Domain.Models;

namespace ComprovantesPagamento.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>
    {
        public PaymentRepository(IDbService dbService) : base(dbService, "payments")
        {

        }
    }
}
