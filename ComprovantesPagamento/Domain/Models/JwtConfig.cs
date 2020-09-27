using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComprovantesPagamento.Domain.Models
{
    public class JwtConfig
    {
        public string Key { get; set; }
        public int ExpiresInMinutes { get; set; }
        public int RefreshExpireInMinutes { get; set; }
    }
}
