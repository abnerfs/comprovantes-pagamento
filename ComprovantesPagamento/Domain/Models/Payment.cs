using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

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

        [BsonElement("payment_type_code")]
        public string PaymentTypeCode { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("month")]
        public int Month { get; set; }

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("payment_date")]
        public DateTime PaymentDate { get; set; }

        [BsonElement("create_date")]
        public DateTime CreateDate { get; set; }


        public string GetFolderName() => $"/{PaymentTypeCode}/{Year}/{Month}";

        public string GetReceiptFileName() => $"{PaymentTypeCode}_receipt_{Year}_{Month}.pdf";

        public string GetDocumentFileName() => $"{PaymentTypeCode}_document_{Year}_{Month}.pdf";

    }
}
