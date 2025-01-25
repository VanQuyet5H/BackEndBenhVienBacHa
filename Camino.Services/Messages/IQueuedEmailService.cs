using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Messages;

namespace Camino.Services.Messages
{
    public interface IQueuedEmailService : IMasterFileService<QueuedEmail>
    {
    }
}
