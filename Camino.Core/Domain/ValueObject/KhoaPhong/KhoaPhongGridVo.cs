using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.KhoaPhong
{
    public class KhoaPhongGridVo : GridItem
    {
        public string Ten { get; set; }

        public string Ma { get; set; }

        public Enums.EnumLoaiKhoaPhong LoaiKhoaPhong { get; set; }

        public bool? CoKhamNgoaiTru { get; set; }
        public bool? CoKhamNoiTru { get; set; }
        public bool? IsDisabled { get; set; }

        public long? SoTienThuTamUng { get; set; }

        public string MoTa { get; set; }

        public string TenKhoa { get; set; }

        public string TenLoaiKhoaPhongDisplayName { get; set; }
        public string KieuKhamDisplay
        {
            get
            {
                var result = string.Empty;
                if (CoKhamNoiTru == true)
                {
                    result += "Nội Trú";
                }
                if (CoKhamNgoaiTru == true)
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += "; ";
                    }
                    result += "Ngoại Trú";
                }
                return result;
            }
        }

    }
}
