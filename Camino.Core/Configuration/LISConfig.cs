using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Configuration
{
    public class LISConfig
    {
        public LISConfig()
        {
            PhienXetNghiemSoGioToiDa = 24*10;
        }
        public int PhienXetNghiemSoGioToiDa { get; set; }
        public string ResultFolder { get; set; }
        public int MoveToNotFoundAfterHours { get; set; }
        public string BarCodeNumberPosition { get; set; }
        public string DeviceCodePosition { get; set; }
        public string AssayCodePosition { get; set; }
        public string ResultValuePosition { get; set; }
        public string ResultUnitPosition { get; set; }
        public string ResultTimePosition { get; set; }
        public string DeviceModelId { get; set; }
        public string HumaClotDeviceId { get; set; }

        public string[] BarCodeNumberPositions => BarCodeNumberPosition?.Split(';');
        public string[] DeviceCodePositions => DeviceCodePosition?.Split(';');
        public string[] AssayCodePositions => AssayCodePosition?.Split(';');
        public string[] ResultValuePositions => ResultValuePosition?.Split(';');
        public string[] ResultUnitPositions => ResultUnitPosition?.Split(';');
        public string[] ResultTimePositions => ResultTimePosition?.Split(';');
        public string[] DeviceModelIds => DeviceModelId?.Split(';');
        public string[] HumaClotDeviceIds => HumaClotDeviceId?.Split(';');
    }
}
