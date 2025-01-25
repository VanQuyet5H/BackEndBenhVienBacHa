using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum DeviceType
        {
            Web = 1,
            Android = 2,
            IOS = 3
        }
        public enum MessagePriority
        {
            Normal = 1,
            High = 2
        }
        public enum MessagingType
        {
            Task = 1,
            Notification = 2,
            SMS = 3,
            Email = 4
        }
        public enum TaskStatus
        {
            Running = 1,
            Completed = 2,
            Aborted = 3
        }
    }
}
