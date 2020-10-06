﻿using System.Text.Json.Serialization;

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
