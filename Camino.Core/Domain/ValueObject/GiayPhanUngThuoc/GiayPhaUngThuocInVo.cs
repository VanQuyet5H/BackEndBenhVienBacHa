using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.GiayPhanUngThuoc
{
    public class GiayPhaUngThuocInVo
    {
        public string KhoaCreate { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
        public string TenToiLa { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string SoGiuong { get; set; }
        public string SoBuong { get; set; }
        public string ChanDoan { get; set; }
        public string BSChinhDinh { get; set; }
        public string TenThuoc { get; set; }
        public string NuocSX { get; set; }
        public string SoLo { get; set; }
        public string DieuDuongPhanUng { get; set; }
        public string ChucDanhDieuDuongPhanUng { get; set; }
        public string HoTenBSDocPhanUng { get; set; }
        public string ChucDanhBSDocPhanUng { get; set; }
        public string NgayGioThangnamCamKet { get; set; }
        public string NgayGioDieuDuongPhanUngThu { get; set; }
        public string NgayGioBSDocPhanUng { get; set; }
        public string NguoiViet { get; set; }
        public string DieuDuongThuPhanUng { get; set; }
        public string BacSiDocPhanUng { get; set; }
        public string KetQuaPhanUng1 { get; set; }
        public string KetQuaPhanUng2 { get; set; }
        public string KetQuaPhanUng3 { get; set; }

    }
    public class GiayPhanUngThuocVO 
    {
        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }


        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string ChanDoan { get; set; }
        public bool? DongYThuPhanUngThuoc { get; set; }

        public string BSChiDinh { get; set; }

        public long? ThuocId { get; set; }
        public string TenThuocText { get; set; }

        public long KetQuaId { get; set; }

        public string KetQuaText { get; set; }

        public DateTime? ThoiGianLamPhieu { get; set; }
        public string ThoiGianLamPhieuString { get; set; }

        public DateTime? ThoiGianThuPhanUng { get; set; }
        public string ThoiGianThuPhanUngString { get; set; }
        public long? BSDocPhanUngId { get; set; }
        public string BSDocPhanUngText { get; set; }

        public DateTime? ThoiGianDocPhanUng { get; set; }
        public string ThoiGianDocPhanUngString { get; set; }

        public long? DieuDuongThucHienId { get; set; }
        public string DieuDuongThucHienText { get; set; }
        public long? IdNoiTruHoSo { get; set; }
        public string NuocSX { get; set; }
        public string SoLo { get; set; }
    }

}
