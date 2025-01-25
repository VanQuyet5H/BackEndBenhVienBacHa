using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Configuration
{
    public class EmailConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string From { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public int MaxRetries { get; set; }
    }
}
