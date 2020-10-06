using System.Text.Json.Serialization;

namespace ComprovantesPagamento.Domain.Requests
{
    public class RegisterRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("pass")]
        public string Pass { get; set; }
    }
}
