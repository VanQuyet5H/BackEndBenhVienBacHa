using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Configuration
{
    public class SmsConfig
    {
        public string PassCodeDault { get; set; }
        public int MaxRetries { get; set; }
        public TwilioConfig Twilio { get; set; }
        public AwsConfig Aws { get; set; }
    }
    public class TwilioConfig
    {
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string Sender { get; set; }
        public string FromNumber { get; set; }
        public bool Active { get; set; }
    }
    public class AwsConfig
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Sender { get; set; }
        public bool Active { get; set; }
    }
}
