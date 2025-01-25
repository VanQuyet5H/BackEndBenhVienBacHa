using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.GiayPhanUngThuoc
{
    public class GiayPhanUngThuocViewModel :BaseViewModel
    {
        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }


        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string ChanDoan { get; set; }
        public bool? DongYThuPhanUngThuoc   { get; set; }

        public string BSChiDinh { get; set; }

        public long? ThuocId { get; set; }
        public string TenThuocText { get; set; }

        public long KetQuaId { get; set; }

        public string KetQuaText { get; set; }

        public DateTime? ThoiGianLamPhieu { get; set; }
        public string ThoiGianLamPhieuString { get; set; }

        public DateTime? ThoiGianThuPhanUng { get; set; }
        public string ThoiGianThuPhanUngString { get; set; }

        public long? DieuDuongThucHienId { get; set; }
        public string DieuDuongThucHienText { get; set; }

        public DateTime? ThoiGianDocPhanUng { get; set; }
        public string ThoiGianDocPhanUngString { get; set; }
        public long? BSDocPhanUngId { get; set; }
        public string BSDocPhanUngText { get; set; }
        public long? IdNoiTruHoSo { get; set; }
        public bool CheckCreateOrCapNhat { get; set; }
        public string NuocSX { get; set; }
        public string SoLo { get; set; }
    }
    public class GiayPhanUngThuocInfo : BaseViewModel
    {
        public string KetQuaText { get; set; }

        public string NgayPhanUngThuoc { get; set; }
        public string TenThuoc { get; set; }

    }

}
