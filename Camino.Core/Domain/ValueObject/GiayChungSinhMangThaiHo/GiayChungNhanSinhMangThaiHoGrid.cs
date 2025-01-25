using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.GiayChungSinhMangThaiHo
{
    public class GiayChungNhanSinhMangThaiHoGrid : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public string NhanVienThucHien { get; set; }
        public long NoiThucHienId { get; set; }
    }
    public class DieuTriNoiTruTongKetBenhAnKhoaSanGrid
    {
        public List<DacDiemTreSoSinhGridVo> DacDiemTreSoSinhs { get; set; }
    }
    public class DacDiemTreSoSinhGridVo :GridItem
    {
        public DateTime? DeLuc { get; set; }
        public string DeLucDisplayName => DeLuc?.ToString("dd/MM/yyy hh:mm:ss");

        public EnumGioiTinh? GioiTinhId { get; set; }
        public string GioiTinh { get; set; }
        public EnumTrangThaiSong? TinhTrangId { get; set; }
        public string TenTinhTrang { get; set; }
        public string DiTat { get; set; }
        public double? CanNang { get; set; }
        public double? Cao { get; set; }
        public int? VongDau { get; set; }
        public bool? CoHauMon { get; set; }

        //khách hàng muốn nhập 1 lúc 3 chỉ số => 1,5,10 phut 
        public int? ApGar => 1;
        public int? ChiSoApGar { get; set; }

        public int? ApGar5 => 2;
        public int? ChiSoApGar5 { get; set; }

        public int? ApGar10 => 3;
        public int? ChiSoApGar10 { get; set; }

        public string TinhTrang { get; set; }
        public string KetQuaXuLy { get; set; }

        public long? YeuCauTiepNhanConId { get; set; }
    }
    public class ThongTinGiayChungNhanSinhMangThaiHo
    {
        public ThongTinGiayChungNhanSinhMangThaiHo() {
            DacDiemTreSoSinhs = new List<ThongTinDacDiemTreSoSinhGridVo>();
        }
        public string So { get; set; }
        public string QuyenSo { get; set; }
        public string HoTenVoNhoMangThaiHo { get; set; }
        public int? NamSinhVoNhoMangThaiHo { get; set; }
        public string CMNDHoChieuVoNhomangThaiHo { get; set; }
        public string DanTocVoNhoMangThaiHo { get; set; }
        public string NoiDangKyThuongTruVoNhoMangThaiHo { get; set; }
        public string HoTenChongNhoMangThaiHo { get; set; }
        public int? NamSinhChongNhoMangThaiHo { get; set; }
        public string CMNDHoChieuChongNhomangThaiHo { get; set; }
        public string DanTocChongNhoMangThaiHo { get; set; }
        public string NoiDangKyThuongTruChongNhoMangThaiHo { get; set; }
        public string HoTenVoMangThaiHo { get; set; }
        public int? NamSinhVoMangThaiHo { get; set; }
        public string CMNDHoChieuVomangThaiHo { get; set; }
        public string DanTocVoMangThaiHo { get; set; }
        public string NoiDangKyThuongTruVoMangThaiHo { get; set; }
        public string HoTenChongMangThaiHo { get; set; }
        public int? NamSinhChongMangThaiHo { get; set; }
        public string CMNDHoChieuChongmangThaiHo { get; set; }
        public string DanTocChongMangThaiHo { get; set; }
        public string NoiDangKyThuongTruChongMangThaiHo { get; set; }
        public string GioiTinh { get; set; }
        public string CanNang { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienDisplay { get; set; }
        public string TaiKhoanDangNhap { get; set; }
        public string NguoiDoDe { get; set; }
        public string ThuTruongCSYT { get; set; }
        public List<ThongTinDacDiemTreSoSinhGridVo> DacDiemTreSoSinhs { get; set; }
        public int? SoLanSinh { get; set; }
        public int? SoConHienSong { get; set; }
        public string NgayThucHienString { get; set; }
    }
    public class ThongTinDacDiemTreSoSinhGridVo:GridItem
    {
        public string GioiTinh { get; set; }
        public double? CanNang { get; set; }
        public string DuDinhDatTenCon { get; set; }
        public string TinhTrang { get; set; }
        public DateTime?  DeLuc { get; set; }
        public string DeLucDisplayName { get; set; }
    }
    public class XacNhanInPhieuGiaySinhMangThaiHo
    {
        public long NoiTruHoSoKhacId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
    }
    public class ThongTinRaVien
    {
        public DateTime? ThoiGianRaVien { get; set; }
    }
    public class GiayChunGSinhMangThaiHo
    {
        public string  HTMLGiayChungSinh1 { get; set; }
        public string HTMLGiayChungSinh2 { get; set; }
    }
}
