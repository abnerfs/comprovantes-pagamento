using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ComprovantesPagamento.Domain.Requests
{
    public class LoginRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("pass")]
        public string Pass { get; set; }
    }
}
