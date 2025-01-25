using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XacNhanBhytNoiTru
{
    public class DanhSachHuongBhytNoiTruVo : GridItem
    {
        // id dành cho đơn thuốc thanh toán
        public bool CheckedDefault { get; set; }
        public string MaSoTheBHYT { get; set; }
        public long? TheBHYTId { get; set; }
        public long? IdDatabaseDonThuocThanhToan { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public string Nhom => GroupType.GetDescription();

        public bool IsDaXacNhan => BaoHiemChiTra == true;

        public Enums.EnumNhomGoiDichVu GroupType { get; set; }

        public string Khoa { get; set; }

        public DateTime NgayPhatSinh { get; set; }

        public string NgayPhatSinhDiplay => NgayPhatSinh.ApplyFormatDate();

        public string MaDichVu { get; set; }

        public long? DichVuId { get; set; }

        public string TenDichVu { get; set; }

        public string NhanVienChiDinh { get; set; }

        public string NoiChiDinh { get; set; }

        public string LoaiGia { get; set; }

        public decimal SoLuong { get; set; }

        public decimal? DonGiaBenhVien { get; set; }

        public decimal? ThanhTienBenhVien => DonGiaBenhVien * SoLuong;

        public decimal? DGBHYTThamKhao { get; set; }

        public decimal? ThanhTienBHYTThamKhao => (decimal)((double)DGBHYTThamKhao.GetValueOrDefault() * (double)SoLuong);

        public bool DuocHuongBaoHiem { get; set; }

        public bool? BaoHiemChiTra { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }

        public bool HuongBhyt => BaoHiemChiTra != false;

        public string IcdChinh { get; set; }

        public string GhiChuIcdChinh { get; set; }

        public int? MucHuongSystem { get; set; }

        public int? MucHuongDaDuyet { get; set; }

        public int? TiLeDv { get; set; }

        public Enums.LoaiDichVuKyThuat LoaiKt { get; set; }

        public bool IsPttt => LoaiKt == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat;

        public List<IcdKemTheoVo> IcdKemTheos { get; set; }

        public bool CanModify => TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan;

        public bool ShowHistory { get; set; }

        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }

        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiKhamBenh { get; set; }

        public Enums.EnumTrangThaiYeuCauDichVuKyThuat TrangThaiDichVuKyThuat { get; set; }
        public Enums.EnumTrangThaiYeuCauTruyenMau TrangThaiYeuCauTruyenMau { get; set; }

        public Enums.EnumYeuCauDuocPhamBenhVien TrangThaiDuocPhamBenhVien { get; set; }

        public Enums.EnumTrangThaiGiuongBenh TrangThaiGiuongBenh { get; set; }

        public Enums.TrangThaiDonThuocThanhToan TrangThaiDonThuocThanhToan { get; set; }

        public Enums.EnumYeuCauVatTuBenhVien TrangThaiVatTuBenhVien { get; set; }
    }
}
