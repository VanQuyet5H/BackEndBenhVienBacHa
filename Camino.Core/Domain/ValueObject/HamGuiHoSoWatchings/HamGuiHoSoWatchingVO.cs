using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.HamGuiHoSoWatchings
{
    public class HamGuiHoSoWatchingVO
    {
        public HamGuiHoSoWatchingVO(){
            ByteData = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            TenFileVOs = new List<TenFileVO>();
        }
        public DateTime? TimeSend { get; set; }
        public string DataJson { get; set; }
        public string XMLJson { get; set; }
        public string XMLError { get; set; }
        public string APIError { get; set; }
        public Byte[] ByteData { get; set; }
        public bool? ErrorCheck { get; set; }
        public string NameFileDown { get; set; }
        public int? countFile { get; set; }
        public List<TenFileVO> TenFileVOs { get; set; }
    }
    public class TenFileVO
    {
        public string TenFile { get; set; }
        public string DuLieu { get; set; }
    }
}
