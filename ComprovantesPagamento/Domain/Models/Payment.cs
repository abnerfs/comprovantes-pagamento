using MongoDB.Bson;
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
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("payment_receipt")]
        public string PaymentReceipt { get; set; }

        [BsonElement("payment_document")]
        public string PaymentDocument { get; set; }

        [BsonElement("create_date")]
        public DateTime CreateDate { get; set; }
    }
}
