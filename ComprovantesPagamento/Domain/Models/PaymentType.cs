using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComprovantesPagamento.Domain.Models
{
    public class PaymentType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("code")]
        public string Code { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("create_date")]
        public DateTime CreateDate { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; }


        [BsonElement("update_date")]
        public DateTime? UpdateDate { get; set; }
    }
}
