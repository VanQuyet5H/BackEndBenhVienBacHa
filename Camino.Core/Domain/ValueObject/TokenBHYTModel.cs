using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class TokenBHYTModel
    {
        public string token { get; set; }
        public string id_token { get; set; }
    }
    public class FileDownLoad
    {
        public string StringValue { get; set; }
        public int size { get; set; }
    }
    public class SoThuTuModel { 
        public int STTXMl1 { get; set; }
        public int STTXMl2 { get; set; }
        public int STTXMl3 { get; set; }
        public int STTXMl4{ get; set; }
        public int STTXMl5 { get; set; }
    }
}
