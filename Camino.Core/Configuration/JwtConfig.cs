using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Configuration
{
    public class JwtConfig
    {
        public string InternalIssuer { get; set; }
        public string PortalIssuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
    }
}
