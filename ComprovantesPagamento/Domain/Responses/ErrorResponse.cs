using System.Text.Json.Serialization;

namespace ComprovantesPagamento.Domain.Responses
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}
