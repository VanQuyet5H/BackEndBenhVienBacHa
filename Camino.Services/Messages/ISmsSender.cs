using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Services.Messages
{
    public interface ISmsSender
    {
        bool SendSms(string to, string msg);
    }
}
