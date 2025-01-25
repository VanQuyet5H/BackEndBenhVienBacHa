using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Newtonsoft.Json;
using NLog;

namespace Camino.Core.Infrastructure
{
    [SingletonDependencyAttribute(ServiceType = typeof(ILoggerManager))]
    public class LoggerManager : ILoggerManager
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public LoggerManager()
        {
        }

        public void LogDebug(string message)
        {
            Logger.Debug(message);
        }

        public void LogError(string message)
        {
            Logger.Error(message);
        }

        public void LogInfo(string message)
        {
            Logger.Info(message);
        }

        public void LogWarn(string message)
        {
            Logger.Warn(message);
        }
        public void LogTrace(string message, params object[] args)
        {
            var serializeObject = string.Empty;
            try
            {
                if(args != null && args.Length > 0)
                {
                    serializeObject = JsonConvert.SerializeObject(args);
                }                
            }
            catch
            { }
            Logger.Trace(message + ". args:" + serializeObject);
        }
    }
}
