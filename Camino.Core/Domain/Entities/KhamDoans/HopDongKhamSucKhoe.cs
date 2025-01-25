using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class HopDongKhamSucKhoe : BaseEntity
    {
        public long CongTyKhamSucKhoeId { get; set; }
        public string SoHopDong { get; set; }
        public DateTime NgayHopDong { get; set; }
        public int LoaiHopDong { get; set; }
        public int SoNguoiKham { get; set; }
        public decimal GiaTriHopDong { get; set; }
        public decimal? ThanhToanPhatSinh { get; set; }
        public decimal? TiLeChietKhau { get; set; }
        public decimal? SoTienTamUng { get; set; }
        public decimal? SoTienChietKhau { get; set; }
        public decimal? SoTienPhatSinhThucTe { get; set; }
        public decimal? SoTienChiChoNhanVien { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public bool DaKetThuc { get; set; }
        public string NguoiKyHopDong { get; set; }
        public string ChucDanhNguoiKy { get; set; }
        public string NguoiGioiThieu { get; set; }
        public string DienGiai { get; set; }
               
        public string LyDoMoLaiHopHopDong { get; set; }
        public long? NhanVienMoLaiHopDongId  { get; set; }
        public DateTime? NgayMoLaiHopDong { get; set; }
        public decimal? GiaTriThucTe { get; set; }

        public virtual CongTyKhamSucKhoe CongTyKhamSucKhoe { get; set; }

        private ICollection<HopDongKhamSucKhoeDiaDiem> _hopDongKhamSucKhoeDiaDiems;
        public virtual ICollection<HopDongKhamSucKhoeDiaDiem> HopDongKhamSucKhoeDiaDiems
        {
            get => _hopDongKhamSucKhoeDiaDiems ?? (_hopDongKhamSucKhoeDiaDiems = new List<HopDongKhamSucKhoeDiaDiem>());
            protected set => _hopDongKhamSucKhoeDiaDiems = value;
        }

        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeNhanViens
        {
            get => _hopDongKhamSucKhoeNhanViens ?? (_hopDongKhamSucKhoeNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeNhanViens = value;
        }

        private ICollection<GoiKhamSucKhoe> _goiKhamSucKhoes;
        public virtual ICollection<GoiKhamSucKhoe> GoiKhamSucKhoes
        {
            get => _goiKhamSucKhoes ?? (_goiKhamSucKhoes = new List<GoiKhamSucKhoe>());
            protected set => _goiKhamSucKhoes = value;
        }

        private ICollection<PhongBenhVien> _phongBenhViens;
        public virtual ICollection<PhongBenhVien> PhongBenhViens
        {
            get => _phongBenhViens ?? (_phongBenhViens = new List<PhongBenhVien>());
            protected set => _phongBenhViens = value;
        }

        private ICollection<YeuCauNhanSuKhamSucKhoe> _yeuCauNhanSuKhamSucKhoes;
        public virtual ICollection<YeuCauNhanSuKhamSucKhoe> YeuCauNhanSuKhamSucKhoes
        {
            get => _yeuCauNhanSuKhamSucKhoes ?? (_yeuCauNhanSuKhamSucKhoes = new List<YeuCauNhanSuKhamSucKhoe>());
            protected set => _yeuCauNhanSuKhamSucKhoes = value;
        }

        public virtual NhanVien NhanVienMoLaiHopDong { get; set; }
    }
}
