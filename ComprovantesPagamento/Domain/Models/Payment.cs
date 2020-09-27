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


        [BsonElement("payment_type")]
        public string PaymentType { get; set; }


        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("payed")]
        public bool Payed { get; set; }


        [BsonElement("payment_date")]
        public DateTime? PaymentDate { get; set; } = null;


        [BsonElement("create_date")]
        public DateTime CreateDate { get; set; }
    }
}
