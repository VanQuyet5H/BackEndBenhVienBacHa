using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruBenhAn : BaseEntity
    {
        public long BenhNhanId { get; set; }
        public string SoBenhAn { get; set; }
        public string SoLuuTru { get; set; }
        public string ThuTuSapXepLuuTru { get; set; }
        public long? NhanVienLuuTruId { get; set; }
        public DateTime? NgayLuuTru { get; set; }
        public long KhoaPhongNhapVienId { get; set; }
        public DateTime ThoiDiemNhapVien { get; set; }
        public DateTime? ThoiDiemRaVien { get; set; }
        public bool LaCapCuu { get; set; }
        public Enums.LoaiBenhAn LoaiBenhAn { get; set; }
        public DateTime ThoiDiemTaoBenhAn { get; set; }
        public long NhanVienTaoBenhAnId { get; set; }
        public int? SoLanVaoVienDoBenhNay { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTri { get; set; }
        public Enums.EnumTinhTrangRaVien? TinhTrangRaVien { get; set; }
        public Enums.EnumHinhThucRaVien? HinhThucRaVien { get; set; }
        public bool? CoThuThuat { get; set; }
        public bool? CoPhauThuat { get; set; }
        public DateTime? NgayTaiKham { get; set; }
        public string GhiChuTaiKham { get; set; }
        public bool? CoChuyenVien { get; set; }
        public Enums.LoaiChuyenTuyen? LoaiChuyenTuyen { get; set; }
        public long? ChuyenDenBenhVienId { get; set; }
        public DateTime? ThoiDiemChuyenVien { get; set; }
        public string ThongTinTaiNanThuongTich { get; set; }
        public string ThongTinBenhAn { get; set; }
        public string ThongTinTongKetBenhAn { get; set; }
        public string ThongTinRaVien { get; set; }
        public long? ChanDoanChinhRaVienICDId { get; set; }
        public string ChanDoanChinhRaVienGhiChu { get; set; }
        public bool? DaQuyetToan { get; set; }
        public string DanhSachChanDoanKemTheoRaVienICDId { get; set; }
        public string DanhSachChanDoanKemTheoRaVienGhiChu { get; set; }
        public DateTime? ThoiDiemTongHopYLenhDichVuKyThuat { get; set; }
        public DateTime? ThoiDiemTongHopYLenhTruyenMau { get; set; }
        public DateTime? ThoiDiemTongHopYLenhVatTu { get; set; }
        public DateTime? ThoiDiemTongHopYLenhDuocPham { get; set; }
        public decimal? SoNgayDieuTriBenhAnSoSinh { get; set; }

        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual NhanVien NhanVienLuuTru { get; set; }
        public virtual KhoaPhong KhoaPhongNhapVien { get; set; }
        public virtual NhanVien NhanVienTaoBenhAn { get; set; }
        public virtual BenhVien.BenhVien ChuyenDenBenhVien { get; set; }
        public virtual ICD ChanDoanChinhRaVienICD { get; set; }

        //BVHD-3575
        public DateTime? ThoiDiemTongHopYLenhKhamBenh { get; set; }

        private ICollection<NoiTruEkipDieuTri> _noiTruEkipDieuTris;
        public virtual ICollection<NoiTruEkipDieuTri> NoiTruEkipDieuTris
        {
            get => _noiTruEkipDieuTris ?? (_noiTruEkipDieuTris = new List<NoiTruEkipDieuTri>());
            protected set => _noiTruEkipDieuTris = value;
        }

        private ICollection<NoiTruKhoaPhongDieuTri> _noiTruKhoaPhongDieuTris;
        public virtual ICollection<NoiTruKhoaPhongDieuTri> NoiTruKhoaPhongDieuTris
        {
            get => _noiTruKhoaPhongDieuTris ?? (_noiTruKhoaPhongDieuTris = new List<NoiTruKhoaPhongDieuTri>());
            protected set => _noiTruKhoaPhongDieuTris = value;
        }

        private ICollection<NoiTruPhieuDieuTri> _noiTruPhieuDieuTris;
        public virtual ICollection<NoiTruPhieuDieuTri> NoiTruPhieuDieuTris
        {
            get => _noiTruPhieuDieuTris ?? (_noiTruPhieuDieuTris = new List<NoiTruPhieuDieuTri>());
            protected set => _noiTruPhieuDieuTris = value;
        }

        #region //BVHD-3312
        private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _noiTruPhieuDieuTriChiTietYLenhs;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NoiTruPhieuDieuTriChiTietYLenhs
        {
            get => _noiTruPhieuDieuTriChiTietYLenhs ?? (_noiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
            protected set => _noiTruPhieuDieuTriChiTietYLenhs = value;
        }

        private ICollection<NoiTruPhieuDieuTriChiTietDienBien> _noiTruPhieuDieuTriChiTietDienBiens;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietDienBien> NoiTruPhieuDieuTriChiTietDienBiens
        {
            get => _noiTruPhieuDieuTriChiTietDienBiens ?? (_noiTruPhieuDieuTriChiTietDienBiens = new List<NoiTruPhieuDieuTriChiTietDienBien>());
            protected set => _noiTruPhieuDieuTriChiTietDienBiens = value;
        }
        #endregion

        private ICollection<NoiTruThoiGianDieuTriBenhAnSoSinh> _noiTruThoiGianDieuTriBenhAnSoSinhs;
        public virtual ICollection<NoiTruThoiGianDieuTriBenhAnSoSinh> NoiTruThoiGianDieuTriBenhAnSoSinhs
        {
            get => _noiTruThoiGianDieuTriBenhAnSoSinhs ?? (_noiTruThoiGianDieuTriBenhAnSoSinhs = new List<NoiTruThoiGianDieuTriBenhAnSoSinh>());
            protected set => _noiTruThoiGianDieuTriBenhAnSoSinhs = value;
        }

        private ICollection<NoiTruChiDinhPhaThuocTiem> _noiTruChiDinhPhaThuocTiems;
        public virtual ICollection<NoiTruChiDinhPhaThuocTiem> NoiTruChiDinhPhaThuocTiems
        {
            get => _noiTruChiDinhPhaThuocTiems ?? (_noiTruChiDinhPhaThuocTiems = new List<NoiTruChiDinhPhaThuocTiem>());
            protected set => _noiTruChiDinhPhaThuocTiems = value;
        }

        private ICollection<NoiTruChiDinhPhaThuocTruyen> _noiTruChiDinhPhaThuocTruyens;
        public virtual ICollection<NoiTruChiDinhPhaThuocTruyen> NoiTruChiDinhPhaThuocTruyens
        {
            get => _noiTruChiDinhPhaThuocTruyens ?? (_noiTruChiDinhPhaThuocTruyens = new List<NoiTruChiDinhPhaThuocTruyen>());
            protected set => _noiTruChiDinhPhaThuocTruyens = value;
        }
    }
}
