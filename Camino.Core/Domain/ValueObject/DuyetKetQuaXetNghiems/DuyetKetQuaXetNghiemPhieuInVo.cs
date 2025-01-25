using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems
{
    public class DuyetKetQuaXetNghiemPhieuInVo : GridItem
    {
        public DuyetKetQuaXetNghiemPhieuInVo()
        {
            NhomDichVuBenhVienIds = new List<long>();
        }
        public string HostingName { get; set; }

        public bool? Header { get; set; }
        public List<long> NhomDichVuBenhVienIds { get; set; }
        public long? LoaiIn { get; set; }
        public List<DuyetKqXetNghiemChiTietModel> ListIn { get; set; }
    }
    public class PhieuInXetNghiemViewModel
    {
        public string Html { get; set; }
        public string TenFile { get; set; }
        public bool? NoFooter { get; set; }
    }

    public class PhieuInXetNghiemTestCovidViewModel
    {
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinhString { get; set; }
        public string DiaChi { get; set; }
        public string SoTheBHYT { get; set; }
        public string ThoiGianLayMau { get; set; }
        public string NoiLayMau { get; set; }
        public string NguoiLayMau { get; set; }
        public string LoaiBenhPham { get; set; }
        public string DanhSach { get; set; }
        public string Gio { get; set; }
        public string Phut { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string LanhDaoDonVi { get; set; }
        public string LogoUrl { get; set; }
    }
    

    public class CauHinhDichVuTestSarsCovid
    {
        public long DichVuKyThuatBenhVienId { get; set; }
        public string TenDichVu { get; set; }
    }

    public class CauHinhLoaiKitTestSarsCovid
    {
        public long LoaiKitThuId { get; set; }
        public string LoaiKitThu { get; set; }
    }

    public class KetQuaXetNghiemVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long DichVuXetNghiemId { get; set; }
        public string DichVuXetNghiemTen { get; set; }
        public long? KetQuaXetNghiemChiTietId { get; set; }
    }
}
