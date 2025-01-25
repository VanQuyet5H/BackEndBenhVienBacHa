using System;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.LichPhanCongNgoaiTru
{
    public class LichPhanCongNgoaiTruGridVo : GridItem
    {
        
        public long IdKhoa { get; set; }

        public string TenKhoa { get; set; }

        public int SoTuan { get; set; }

        public string Tuan1Display { get; set; }

        public string Tuan2Display { get; set; }

        public string Tuan3Display { get; set; }

        public string Tuan4Display { get; set; }

        public string Tuan5Display { get; set; }

        public string Tuan6Display { get; set; }

        public bool XepLichTuan1 { get; set; }

        public bool XepLichTuan2 { get; set; }

        public bool XepLichTuan3 { get; set; }

        public bool XepLichTuan4 { get; set; }

        public bool XepLichTuan5 { get; set; }

        public bool XepLichTuan6 { get; set; }

        public DateTime NgayStartTuan1 { get; set; }

        public DateTime NgayStartTuan2 { get; set; }

        public DateTime NgayStartTuan3 { get; set; }

        public DateTime NgayStartTuan4 { get; set; }

        public DateTime NgayStartTuan5 { get; set; }

        public DateTime NgayStartTuan6 { get; set; }

        public DateTime NgayEndTuan1 { get; set; }

        public DateTime NgayEndTuan2 { get; set; }
                            
        public DateTime NgayEndTuan3 { get; set; }
                            
        public DateTime NgayEndTuan4 { get; set; }
                            
        public DateTime NgayEndTuan5 { get; set; }
                            
        public DateTime NgayEndTuan6 { get; set; }

        public DateTime Now { get; set; }
    }
}
