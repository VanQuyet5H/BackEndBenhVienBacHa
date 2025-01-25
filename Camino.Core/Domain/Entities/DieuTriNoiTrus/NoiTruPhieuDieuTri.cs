using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.CheDoAns;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruPhieuDieuTri : BaseEntity
    {
        public long NoiTruBenhAnId { get; set; }
        public long NhanVienLapId { get; set; }
        public long KhoaPhongDieuTriId { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public DateTime? ThoiDiemThamKham { get; set; }
        public long? ChanDoanChinhICDId { get; set; }
        public string ChanDoanChinhGhiChu { get; set; }
        public string DienBien { get; set; }
        public Enums.CheDoChamSoc? CheDoChamSoc { get; set; }
        public string GhiChuChamSoc { get; set; }

        public bool? BenhNhanCapCuu { get; set; }
        public long? CheDoAnId { get; set; }

        //BVHD-3916
        public string GhiChuCanLamSang { get; set; }

        public virtual NoiTruBenhAn NoiTruBenhAn { get; set; }
        public virtual NhanVien NhanVienLap { get; set; }
        public virtual KhoaPhong KhoaPhongDieuTri { get; set; }
        public virtual ICD ChanDoanChinhICD { get; set; }
        public virtual CheDoAn CheDoAn { get; set; }

        //private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _noiTruPhieuDieuTriChiTietYLenhs;
        //public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NoiTruPhieuDieuTriChiTietYLenhs
        //{
        //    get => _noiTruPhieuDieuTriChiTietYLenhs ?? (_noiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
        //    protected set => _noiTruPhieuDieuTriChiTietYLenhs = value;
        //}

        private ICollection<NoiTruThamKhamChanDoanKemTheo> _noiTruThamKhamChanDoanKemTheos;
        public virtual ICollection<NoiTruThamKhamChanDoanKemTheo> NoiTruThamKhamChanDoanKemTheos
        {
            get => _noiTruThamKhamChanDoanKemTheos ?? (_noiTruThamKhamChanDoanKemTheos = new List<NoiTruThamKhamChanDoanKemTheo>());
            protected set => _noiTruThamKhamChanDoanKemTheos = value;
        }

        private ICollection<KetQuaSinhHieu> _ketQuaSinhHieus;
        public virtual ICollection<KetQuaSinhHieu> KetQuaSinhHieus
        {
            get => _ketQuaSinhHieus ?? (_ketQuaSinhHieus = new List<KetQuaSinhHieu>());
            protected set => _ketQuaSinhHieus = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }

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
        
        //private ICollection<NoiTruPhieuDieuTriChiTietDienBien> _noiTruPhieuDieuTriChiTietDienBiens;
        //public virtual ICollection<NoiTruPhieuDieuTriChiTietDienBien> NoiTruPhieuDieuTriChiTietDienBiens
        //{
        //    get => _noiTruPhieuDieuTriChiTietDienBiens ?? (_noiTruPhieuDieuTriChiTietDienBiens = new List<NoiTruPhieuDieuTriChiTietDienBien>());
        //    protected set => _noiTruPhieuDieuTriChiTietDienBiens = value;
        //}

        private ICollection<NoiTruChiDinhDuocPham> _noiTruChiDinhDuocPhams;
        public virtual ICollection<NoiTruChiDinhDuocPham> NoiTruChiDinhDuocPhams
        {
            get => _noiTruChiDinhDuocPhams ?? (_noiTruChiDinhDuocPhams = new List<NoiTruChiDinhDuocPham>());
            protected set => _noiTruChiDinhDuocPhams = value;
        }

        private ICollection<NoiTruPhieuDieuTriTuVanThuoc> _noiTruPhieuDieuTriTuVanThuocs;
        public virtual ICollection<NoiTruPhieuDieuTriTuVanThuoc> NoiTruPhieuDieuTriTuVanThuocs
        {
            get => _noiTruPhieuDieuTriTuVanThuocs ?? (_noiTruPhieuDieuTriTuVanThuocs = new List<NoiTruPhieuDieuTriTuVanThuoc>());
            protected set => _noiTruPhieuDieuTriTuVanThuocs = value;
        }
        
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

    public class NoiTruPhieuDieuTriDienBien
    {
        public long IdView { get; set; }
        public string DienBien { get; set; }
        public string YLenh { get; set; }
        public string CheDoChamSoc { get; set; }
        public string CheDoAn { get; set; }
        public DateTime ThoiGian { get; set; }
        public long? DienBienLastUserId { get; set; }
        public long? YLenhLastUserId { get; set; }
    }
}

