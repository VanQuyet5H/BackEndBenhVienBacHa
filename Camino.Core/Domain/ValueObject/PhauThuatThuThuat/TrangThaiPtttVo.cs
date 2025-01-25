using System;
using Camino.Core.Domain.ValueObject.Grid;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class LookupTrangThaiPtttVo
    {
        public long KeyId { get; set; }

        public string TenDv { get; set; }
        public long? BacSiChinhId { get; set; }
        public string BacSi { get; set; }

        public bool IsDaTuongTrinh { get; set; }
        public bool IsKhongTuongTrinh { get; set; }


        public bool IsPhauThuatVienChinh { get; set; }

        public string TuongTrinhDisplay => IsDaTuongTrinh ?
            "Đã tường trình" : "Đang tường trình";

        public string DisplayName => TenDv;

        public string LoaiPhauThuatThuThuat { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public string LoaiPTTT { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat TrangThaiYeuCauDichVuKyThuat { get; set; } //Update tường trình lại
        //public string LoaiPTTT => !string.IsNullOrEmpty(LoaiPhauThuatThuThuat) && LoaiPhauThuatThuThuat.Substring(0, 1).ToLower().Contains("p") ? "Phẫu thuật" : "Thủ thuật";

        //Cập nhật 13/12/2022
        public long? NhomDichVuBenhVienKiemTraId { get; set; }
    }
}
