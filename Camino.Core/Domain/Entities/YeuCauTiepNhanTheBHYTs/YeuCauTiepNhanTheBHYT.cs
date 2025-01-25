using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs
{
    public class YeuCauTiepNhanTheBHYT : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public string MaSoThe { get; set; }
        public int MucHuong { get; set; }
        public string MaDKBD { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string DiaChi { get; set; }
        public string CoQuanBHXH { get; set; }
        public DateTime? NgayDu5Nam { get; set; }
        public DateTime? NgayDuocMienCungChiTra { get; set; }
        public string MaKhuVuc { get; set; }
        public bool? DuocMienCungChiTra { get; set; }
        public long? GiayMienCungChiTraId { get; set; }
        public Enums.EnumTinhTrangThe? TinhTrangThe { get; set; }
        public bool? IsCheckedBHYT { get; set; }
        public bool? DuocGiaHanThe { get; set; }


        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual GiayMienCungChiTra GiayMienCungChiTra { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }
        private ICollection<YeuCauTruyenMau> _yeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> YeuCauTruyenMaus
        {
            get => _yeuCauTruyenMaus ?? (_yeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _yeuCauTruyenMaus = value;
        }
        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }
        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }
        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }
        private ICollection<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _yeuCauDichVuGiuongBenhVienChiPhiBHYTs;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhiBHYT> YeuCauDichVuGiuongBenhVienChiPhiBHYTs
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhiBHYTs ?? (_yeuCauDichVuGiuongBenhVienChiPhiBHYTs = new List<YeuCauDichVuGiuongBenhVienChiPhiBHYT>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhiBHYTs = value;
        }
        private ICollection<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTiets;
        public virtual ICollection<DuyetBaoHiemChiTiet> DuyetBaoHiemChiTiets
        {
            get => _duyetBaoHiemChiTiets ?? (_duyetBaoHiemChiTiets = new List<DuyetBaoHiemChiTiet>());
            protected set => _duyetBaoHiemChiTiets = value;
        }
        private ICollection<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTiets;
        public virtual ICollection<DonThuocThanhToanChiTiet> DonThuocThanhToanChiTiets
        {
            get => _donThuocThanhToanChiTiets ?? (_donThuocThanhToanChiTiets = new List<DonThuocThanhToanChiTiet>());
            protected set => _donThuocThanhToanChiTiets = value;
        }
    }
}
