using ComprovantesPagamento.Contracts;
using ComprovantesPagamento.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace ComprovantesPagamento.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>
    {
        public PaymentRepository(IDbService dbService) : base(dbService, "payments")
        {


        }

        public IEnumerable<Payment> ListPayment(string userId, string typeId)
        {
            try
            {
                return Collection.Find(Filter.And(FilterUserID(userId), Filter.Eq("payment_type", typeId)))
                    .ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Payment GetByUser(string userId, string id)
        {
            try
            {
                return Collection.Find(Filter.And(FilterId(id), FilterUserID(userId)))
                    .FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Payment GetByYearMonth(string type, int year, int month)
        {
            try
            {
                return Collection.Find(Filter.And(Filter.Eq("payment_type", type), Filter.Eq("month", month), Filter.Eq("year", year)))
                    .FirstOrDefault();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
