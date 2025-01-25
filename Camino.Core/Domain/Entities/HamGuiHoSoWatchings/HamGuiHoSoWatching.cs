using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.HamGuiHoSoWatchings
{
   public class HamGuiHoSoWatching:BaseEntity
    {
        public DateTime? TimeSend { get; set; }
        public string DataJson { get; set; }
        public string XMLJson { get; set; }
        public string XMLError { get; set; }
        public string APIError{ get; set; }
    }
}
