using System;
using System.Text.Json.Serialization;

namespace ComprovantesPagamento.Domain.Responses
{
    public class RegisterResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("crypt_pass")]
        public string CryptPass { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("register_date")]
        public DateTime RegisterDate { get; set; }
    }
}
