using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TotechsIdentity.AppSettings
{
    public class JwtTokenConfig
    {
        public string Key { get; set; } = String.Empty;
        public string Issuer { get; set; } = String.Empty;
    }
}
