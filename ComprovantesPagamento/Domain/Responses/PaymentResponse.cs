using System;
using System.Text.Json.Serialization;

namespace ComprovantesPagamento.Domain.Responses
{
    public class PaymentResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("payment_receipt")]
        public string PaymentReceipt { get; set; }

        [JsonPropertyName("payment_document")]
        public string PaymentDocument { get; set; }

        [JsonPropertyName("payment_type")]
        public string PaymentType { get; set; }

        [JsonPropertyName("payment_type_code")]
        public string PaymentTypeCode { get; set; }


        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("month")]
        public int Month { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("create_date")]
        public DateTime CreateDate { get; set; }
    }
}
