using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;

namespace Camino.Core.Domain.Entities.DuyetBaoHiems
{
    public class DuyetBaoHiemChiTiet : BaseEntity
    {
        public long DuyetBaoHiemId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? DonThuocThanhToanChiTietId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienChiPhiBHYTId { get; set; }
        public double SoLuong { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public long? YeuCauTiepNhanTheBHYTId { get; set; }
        public string MaSoTheBHYT { get; set; }

        public virtual YeuCauTiepNhanTheBHYT YeuCauTiepNhanTheBHYT { get; set; }
        public virtual DuyetBaoHiem DuyetBaoHiem { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual YeuCauDuocPhamBenhVien YeuCauDuocPhamBenhVien { get; set; }
        public virtual YeuCauVatTuBenhVien YeuCauVatTuBenhVien { get; set; }
        public virtual YeuCauDichVuGiuongBenhVien YeuCauDichVuGiuongBenhVien { get; set; }
        public virtual DonThuocThanhToanChiTiet DonThuocThanhToanChiTiet { get; set; }
        public virtual YeuCauTruyenMau YeuCauTruyenMau { get; set; }
        public virtual YeuCauDichVuGiuongBenhVienChiPhiBHYT YeuCauDichVuGiuongBenhVienChiPhiBHYT { get; set; }
        
    }
}
