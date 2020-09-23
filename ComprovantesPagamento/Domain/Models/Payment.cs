using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComprovantesPagamento.Domain.Models
{
    public class Payment
    {
        [BsonId]
        public string Id { get; set; }
        public PaymentType Type { get; set; }
        public DateTime Date { get; set; }
        public string PaymentReceipt { get; set; }
        public string PaymentDocument { get; set; }
    }
}
