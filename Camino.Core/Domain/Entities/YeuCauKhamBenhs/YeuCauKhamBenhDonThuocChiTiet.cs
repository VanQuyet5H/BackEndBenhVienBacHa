using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.Thuocs;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonViTinhs;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenhDonThuocChiTiet : BaseEntity
    {
        public long YeuCauKhamBenhDonThuocId { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDuocPhamBenhVien { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? StthoatChat { get; set; }
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
        public double SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool BenhNhanMuaNgoai { get; set; }
        public string GhiChu { get; set; }
        /// <summary>
        /// Update 30/03/2020
        /// </summary>
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }

        /// <summary>
        /// Update 20/09/2021
        /// </summary>
        public double? SoLanTrenVien { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public int? SoThuTu { get; set; }

        public virtual DuocPham DuocPham { get; set; }
        public virtual YeuCauKhamBenhDonThuoc YeuCauKhamBenhDonThuoc { get; set; }
        public virtual DuongDung DuongDung { get; set; }
        public virtual DonViTinh DonViTinh { get; set; }

        private ICollection<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTiets;
        public virtual ICollection<DonThuocThanhToanChiTiet> DonThuocThanhToanChiTiets
        {
            get => _donThuocThanhToanChiTiets ?? (_donThuocThanhToanChiTiets = new List<DonThuocThanhToanChiTiet>());
            protected set => _donThuocThanhToanChiTiets = value;
        }
    }
}
