using Camino.Core.Domain.Entities.DonViTinhs;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.XuatKhos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauDichVuKyThuatTiemChung : BaseEntity
    {
        public long DuocPhamBenhVienId { get; set; }
        public string TenDuocPham { get; set; }
        public string TenDuocPhamTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? STTHoatChat { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat LoaiThuocHoacHoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public long DuongDungId { get; set; }
        public string HamLuong { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string DangBaoChe { get; set; }
        public long DonViTinhId { get; set; }
        public string HuongDan { get; set; }
        public string MoTa { get; set; }
        public string ChiDinh { get; set; }
        public string ChongChiDinh { get; set; }
        public string LieuLuongCachDung { get; set; }
        public string TacDungPhu { get; set; }
        public string ChuYdePhong { get; set; }
        public long? HopDongThauDuocPhamId { get; set; }
        public long? NhaThauId { get; set; }
        public string SoHopDongThau { get; set; }
        public string SoQuyetDinhThau { get; set; }
        public Enums.EnumLoaiThau? LoaiThau { get; set; }
        public Enums.EnumLoaiThuocThau? LoaiThuocThau { get; set; }
        public string NhomThau { get; set; }
        public string GoiThau { get; set; }
        public int? NamThau { get; set; }
        public double SoLuong { get; set; }
        public long? XuatKhoDuocPhamChiTietId { get; set; }
        public Enums.ViTriTiem ViTriTiem { get; set; }
        public int MuiSo { get; set; }
        public Enums.TrangThaiTiemChung TrangThaiTiemChung { get; set; }
        public long? NhanVienTiemId { get; set; }
        public DateTime? ThoiDiemTiem { get; set; }
        public string LieuLuong { get; set; }

        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual DuongDung DuongDung { get; set; }
        public virtual DonViTinh DonViTinh { get; set; }
        public virtual HopDongThauDuocPham HopDongThauDuocPham { get; set; }
        public virtual NhaThau NhaThau { get; set; }
        public virtual XuatKhoDuocPhamChiTiet XuatKhoDuocPhamChiTiet { get; set; }
        public virtual NhanVien NhanVienTiem { get; set; }

        #region clone
        public YeuCauDichVuKyThuatTiemChung Clone()
        {
            return (YeuCauDichVuKyThuatTiemChung)this.MemberwiseClone();
        }
        #endregion
    }
}
