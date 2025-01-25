using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class HangDoiTiemChungQuyeryInfo
    {
        public long PhongKhamHienTaiId { get; set; }
        public string SearchString { get; set; }
        public Enums.LoaiHangDoiTiemVacxin LoaiHangDoi { get; set; }
    }

    public class HangDoiTiemChungDangKhamQuyeryInfo
    {
        public long? PhongKhamHienTaiId { get; set; }
        public long? YeuCauKhamTiemChungId { get; set; }
        public Enums.EnumTrangThaiHangDoi? TrangThaiHangDoi { get; set; }
        public Enums.LoaiHangDoiTiemVacxin LoaiHangDoi { get; set; }
    }

    public class HangDoiTiemChungGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauTiemVacxinId { get; set; }
        public long? YeuCauTiemChungId { get; set; }
        public int SoThuTu { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public Enums.LoaiGioiTinh GioiTinh { get; set; }
        public string TenGioiTinh { get; set; }
        public int Tuoi { get; set; }
        public DateTime NgayKhamSangLoc { get; set; }

        //Cập nhật 12/07/2022
        public int? Nam { get; set; }
        public List<ThongTinPhongBenhVienHangDoi> ThongTinPhongBenhVienHangDois { get; set; } = new List<ThongTinPhongBenhVienHangDoi>();
    }

    public class ThongTinPhongBenhVienHangDoi
    {
        public long YeuCauDichVuKyThuatId { get; set; }
        public long PhongBenhVienId { get; set; }
        public Enums.EnumTrangThaiHangDoi TrangThai { get; set; }
    }

    public class ThongTinLoTheoXuatChiTietVo
    {
        public long XuatKhoChiTietId { get; set; }
        public long NhapKhoChiTietId { get; set; }
        public string SoLo { get; set; }
    }
}
