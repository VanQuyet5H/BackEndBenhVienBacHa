using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.DanTocs;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.DonViHanhChinhs;
using Camino.Core.Domain.Entities.GachNos;
using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.NgheNghieps;
using Camino.Core.Domain.Entities.QuanHeThanNhans;
using Camino.Core.Domain.Entities.QuocGias;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;

namespace Camino.Core.Domain.Entities.BenhNhans
{
    public class BenhNhan : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string MaBN { get; private set; }
        public bool? CoBHYT { get; set; }
        public bool? CoBHTN { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NoiLamViec { get; set; }
        public long? QuocTichId { get; set; }
        public long? DanTocId { get; set; }
        public string DiaChi { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public string SoDienThoai { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }
        public string Email { get; set; }
        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeSoDienThoai { get; set; }
        public string NguoiLienHeEmail { get; set; }
        public string NguoiLienHeDiaChi { get; set; }
        public long? NguoiLienHePhuongXaId { get; set; }
        public long? NguoiLienHeQuanHuyenId { get; set; }
        public long? NguoiLienHeTinhThanhId { get; set; }
        public string BHYTMaSoThe { get; set; }
        //public long? BHYTNoiDangKyId { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTDiaChi { get; set; }

        public string BHYTCoQuanBHXH { get; set; }
        public DateTime? BHYTNgayDu5Nam { get; set; }
        public string BHYTMaDKBD { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DiaChiDayDu { get; private set; }

        //private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        //public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        //{
        //    get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
        //    protected set => _yeuCauKhamBenhs = value;
        //}

        public virtual NgheNghiep NgheNghiep { get; set; }
        //public virtual QuocGia QuocGia { get; set; }
        public virtual QuocGia QuocTich { get; set; }
        public virtual DanToc DanToc { get; set; }

        public virtual DonViHanhChinh PhuongXa { get; set; }
        public virtual DonViHanhChinh QuanHuyen { get; set; }
        public virtual DonViHanhChinh TinhThanh { get; set; }

        public virtual QuanHeThanNhan NguoiLienHeQuanHeNhanThan { get; set; }

        public virtual DonViHanhChinh NguoiLienHePhuongXa { get; set; }

        public virtual DonViHanhChinh NguoiLienHeQuanHuyen { get; set; }
        public virtual DonViHanhChinh NguoiLienHeTinhThanh { get; set; }
        //public virtual BenhVien.BenhVien BHYTNoiDangKy { get; set; }

        // update 12/2/2020
        public Enums.EnumNhomMau? NhomMau { get; set; }
        public Enums.EnumYeuToRh? YeuToRh { get; set; }
        public bool? BHYTDuocMienCungChiTra { get; set; }
        public DateTime? BHYTNgayDuocMienCungChiTra { get; set; }
        public string BHYTMaKhuVuc { get; set; }
        public long? BHYTGiayMienCungChiTraId { get; set; }
        //public long? BHTNCongTyBaoHiemId { get; set; }
        //   public string BHTNMaSoThe { get; set; }
        //   public string BHTNDiaChi { get; set; }
        //   public string BHTNSoDienThoai { get; set; }
        //   public DateTime? BHTNNgayHieuLuc { get; set; }
        //   public DateTime? BHTNNgayHetHan { get; set; }
        public string GhiChuTienSuBenh { get; set; }
        public string GhiChuDiUngThuoc { get; set; }

        private ICollection<BenhNhanDiUngThuoc> _benhNhanDiUngThuocs;
        public virtual ICollection<BenhNhanDiUngThuoc> BenhNhanDiUngThuocs
        {
            get => _benhNhanDiUngThuocs ?? (_benhNhanDiUngThuocs = new List<BenhNhanDiUngThuoc>());
            protected set => _benhNhanDiUngThuocs = value;
        }

        private ICollection<BenhNhanTienSuBenh> _benhNhanTienSuBenhs;
        public virtual ICollection<BenhNhanTienSuBenh> BenhNhanTienSuBenhs
        {
            get => _benhNhanTienSuBenhs ?? (_benhNhanTienSuBenhs = new List<BenhNhanTienSuBenh>());
            protected set => _benhNhanTienSuBenhs = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        //public virtual GiayMienCungChiTra BhytgiayMienCungChiTra { get; set; }
        //public virtual CongTyBaoHiemTuNhan BHTNCongTyBaoHiem { get; set; }

        // update 12/2/2020
        public virtual GiayMienCungChiTra BHYTgiayMienCungChiTra { get; set; }

        //update 22/02/2020 - thach
        private ICollection<BenhNhanCongTyBaoHiemTuNhan> _benhNhanCongTyBaoHiemTuNhans;
        public virtual ICollection<BenhNhanCongTyBaoHiemTuNhan> BenhNhanCongTyBaoHiemTuNhans
        {
            get => _benhNhanCongTyBaoHiemTuNhans ?? (_benhNhanCongTyBaoHiemTuNhans = new List<BenhNhanCongTyBaoHiemTuNhan>());
            protected set => _benhNhanCongTyBaoHiemTuNhans = value;
        }

        public virtual TaiKhoanBenhNhan TaiKhoanBenhNhan { get; set; }

        private ICollection<DonThuocThanhToan> _donThuocThanhToans;
        public virtual ICollection<DonThuocThanhToan> DonThuocThanhToans
        {
            get => _donThuocThanhToans ?? (_donThuocThanhToans = new List<DonThuocThanhToan>());
            protected set => _donThuocThanhToans = value;
        }

        private ICollection<TheVoucherYeuCauTiepNhan> _theVoucherYeuCauTiepNhans { get; set; }
        public virtual ICollection<TheVoucherYeuCauTiepNhan> TheVoucherYeuCauTiepNhans
        {
            get => _theVoucherYeuCauTiepNhans ?? (_theVoucherYeuCauTiepNhans = new List<TheVoucherYeuCauTiepNhan>());
            protected set => _theVoucherYeuCauTiepNhans = value;
        }

        private ICollection<DonVTYTThanhToan> _donVTYTThanhToans;
        public virtual ICollection<DonVTYTThanhToan> DonVTYTThanhToans
        {
            get => _donVTYTThanhToans ?? (_donVTYTThanhToans = new List<DonVTYTThanhToan>());
            protected set => _donVTYTThanhToans = value;
        }

        private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVus;
        public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVus
        {
            get => _yeuCauGoiDichVus ?? (_yeuCauGoiDichVus = new List<YeuCauGoiDichVu>());
            protected set => _yeuCauGoiDichVus = value;

        }

        private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVuSoSinhs;
        public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVuSoSinhs
        {
            get => _yeuCauGoiDichVuSoSinhs ?? (_yeuCauGoiDichVuSoSinhs = new List<YeuCauGoiDichVu>());
            protected set => _yeuCauGoiDichVuSoSinhs = value;

        }

        private ICollection<XuatKhoQuaTang> _xuatKhoQuaTangs;
        public virtual ICollection<XuatKhoQuaTang> XuatKhoQuaTangs
        {
            get => _xuatKhoQuaTangs ?? (_xuatKhoQuaTangs = new List<XuatKhoQuaTang>());
            protected set => _xuatKhoQuaTangs = value;
        }

        private ICollection<GachNo> _gachNos;
        public virtual ICollection<GachNo> GachNos
        {
            get => _gachNos ?? (_gachNos = new List<GachNo>());
            protected set => _gachNos = value;
        }

        private ICollection<PhienXetNghiem> _phienXetNghiems;
        public virtual ICollection<PhienXetNghiem> PhienXetNghiems
        {
            get => _phienXetNghiems ?? (_phienXetNghiems = new List<PhienXetNghiem>());
            protected set => _phienXetNghiems = value;
        }

        private ICollection<NoiTruBenhAn> _noiTruBenhAns;
        public virtual ICollection<NoiTruBenhAn> NoiTruBenhAns
        {
            get => _noiTruBenhAns ?? (_noiTruBenhAns = new List<NoiTruBenhAn>());
            protected set => _noiTruBenhAns = value;
        }

        private ICollection<YeuCauNhapVien> _yeuCauNhapViens;
        public virtual ICollection<YeuCauNhapVien> YeuCauNhapViens
        {
            get => _yeuCauNhapViens ?? (_yeuCauNhapViens = new List<YeuCauNhapVien>());
            protected set => _yeuCauNhapViens = value;
        }

        private ICollection<YeuCauTiepNhanTheBHYT> _yeuCauTiepNhanTheBHYTs;
        public virtual ICollection<YeuCauTiepNhanTheBHYT> YeuCauTiepNhanTheBHYTs
        {
            get => _yeuCauTiepNhanTheBHYTs ?? (_yeuCauTiepNhanTheBHYTs = new List<YeuCauTiepNhanTheBHYT>());
            protected set => _yeuCauTiepNhanTheBHYTs = value;
        }

        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeNhanViens
        {
            get => _hopDongKhamSucKhoeNhanViens ?? (_hopDongKhamSucKhoeNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeNhanViens = value;
        }

        private ICollection<ChuongTrinhGoiDichVu> _chuongTrinhGoiDichVus;
        public virtual ICollection<ChuongTrinhGoiDichVu> ChuongTrinhGoiDichVus
        {
            get => _chuongTrinhGoiDichVus ?? (_chuongTrinhGoiDichVus = new List<ChuongTrinhGoiDichVu>());
            protected set => _chuongTrinhGoiDichVus = value;
        }
    }
}