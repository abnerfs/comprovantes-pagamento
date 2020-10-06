using System;
using System.Text.Json.Serialization;

namespace ComprovantesPagamento.Domain.Responses
{

    public class JwtResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("expire_date")]
        public DateTime ExpireDate { get; set; }
    }
}
