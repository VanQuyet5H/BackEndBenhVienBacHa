using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonViTinhs;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.Entities.NhomVatTus;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.DonVatTus
{
    public class DonVTYTThanhToanChiTiet : BaseEntity
    {
        public long DonVTYTThanhToanId { get; set; }
        public long? YeuCauKhamBenhDonVTYTChiTietId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public long XuatKhoVatTuChiTietViTriId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long NhomVatTuId { get; set; }

        
        public string DonViTinh { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string MoTa { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal DonGiaBan { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal GiaBan { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public double SoLuong { get; set; }
        public long? HopDongThauVatTuId { get; set; }
        public long? NhaThauId { get; set; }
        public string SoHopDongThau { get; set; }
        public string SoQuyetDinhThau { get; set; }
        public Enums.EnumLoaiThau? LoaiThau { get; set; }
        public string NhomThau { get; set; }
        public string GoiThau { get; set; }
        public int NamThau { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public string GhiChu { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string GhiChuMienGiamThem { get; set; }

        public virtual DonVTYTThanhToan DonVTYTThanhToan { get; set; }
        public virtual YeuCauKhamBenhDonVTYTChiTiet YeuCauKhamBenhDonVTYTChiTiet { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual XuatKhoVatTuChiTietViTri XuatKhoVatTuChiTietViTri { get; set; }
        public virtual NhomVatTu NhomVatTu { get; set; }
        public virtual HopDongThauVatTu HopDongThauVatTu { get; set; }
        public virtual NhaThau NhaThau { get; set; }
        
        private ICollection<CongTyBaoHiemTuNhanCongNo> _baoHiemTuNhanCongNos;
        public virtual ICollection<CongTyBaoHiemTuNhanCongNo> CongTyBaoHiemTuNhanCongNos
        {
            get => _baoHiemTuNhanCongNos ?? (_baoHiemTuNhanCongNos = new List<CongTyBaoHiemTuNhanCongNo>());
            protected set => _baoHiemTuNhanCongNos = value;
        }

        private ICollection<MienGiamChiPhi> _mienGiamChiPhis;
        public virtual ICollection<MienGiamChiPhi> MienGiamChiPhis
        {
            get => _mienGiamChiPhis ?? (_mienGiamChiPhis = new List<MienGiamChiPhi>());
            protected set => _mienGiamChiPhis = value;
        }
        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChis;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChis
        {
            get => _taiKhoanBenhNhanChis ?? (_taiKhoanBenhNhanChis = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChis = value;
        }
    }
}
