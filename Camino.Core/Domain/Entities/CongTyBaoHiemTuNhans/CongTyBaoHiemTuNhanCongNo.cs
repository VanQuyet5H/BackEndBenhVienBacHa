using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans
{
    public class CongTyBaoHiemTuNhanCongNo : BaseEntity
    {
        public long? TaiKhoanBenhNhanThuId { get; set; }
        public long CongTyBaoHiemTuNhanId { get; set; }
        public decimal SoTien { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? DonThuocThanhToanChiTietId { get; set; }
        public long? DonVTYTThanhToanChiTietId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public bool? DaHuy { get; set; }

        public virtual TaiKhoanBenhNhanThu TaiKhoanBenhNhanThu { get; set; }
        public virtual CongTyBaoHiemTuNhan CongTyBaoHiemTuNhan { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual YeuCauDuocPhamBenhVien YeuCauDuocPhamBenhVien { get; set; }
        public virtual YeuCauVatTuBenhVien YeuCauVatTuBenhVien { get; set; }
        public virtual YeuCauDichVuGiuongBenhVien YeuCauDichVuGiuongBenhVien { get; set; }
        public virtual DonThuocThanhToanChiTiet DonThuocThanhToanChiTiet { get; set; }
        public virtual DonVTYTThanhToanChiTiet DonVTYTThanhToanChiTiet { get; set; }
        public virtual YeuCauGoiDichVu YeuCauGoiDichVu { get; set; }
        public virtual YeuCauTruyenMau YeuCauTruyenMau { get; set; }
        public virtual YeuCauDichVuGiuongBenhVienChiPhiBenhVien YeuCauDichVuGiuongBenhVienChiPhiBenhVien { get; set; }

        
    }
}
