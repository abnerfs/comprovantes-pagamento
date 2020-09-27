using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComprovantesPagamento.Domain.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("crypt_pass")]
        public string CryptPass { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("register_date")]
        public DateTime? RegisterDate { get; set; } = null;
    }
}
