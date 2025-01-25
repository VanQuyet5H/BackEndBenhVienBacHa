using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using System.Globalization;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Services.Helpers;
using Camino.Services.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.KhoDuocPhams;
using Camino.Services.QuanHeThanNhan;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhieuNghiDuongThai;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Infrastructure;

namespace Camino.Services.YeuCauKhamBenh
{
    [ScopedDependency(ServiceType = typeof(IYeuCauKhamBenhService))]


    public partial class YeuCauKhamBenhService : MasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>, IYeuCauKhamBenhService
    {

        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhanLichSuKhamBHYT> _yeuCauTiepNhanLichSuKhamBHYTRepository;
        private readonly IRepository<Core.Domain.Entities.DoiTuongUuTienKhamChuaBenhs.DoiTuongUuTienKhamChuaBenh> _doiTuongRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.Users.User> _bacSiChiDinhRepository;
        private readonly IRepository<ICD> _iCDRepository;
        private readonly IRepository<YeuCauKhamBenhICDKhac> _yeuCauKhamBenhICDKhacRepository;
        private readonly IRepository<YeuCauKhamBenhChanDoanPhanBiet> _yeuCauKhamBenhChanDoanPhanBietRepository;
        private readonly IRepository<Core.Domain.Entities.Localization.LocaleStringResource> _localeStringResourceRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> _hoatDongNhanVienRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;

        private readonly IRepository<Core.Domain.Entities.Thuocs.ADR> _aDRRepository;
        private readonly IRepository<Core.Domain.Entities.Users.User> _userRepository;
        private readonly IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> _inputStringStoredRepository;

        private readonly IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.NhomGiaDichVuKhamBenhBenhVien> _nhomGiaDichVuKhamBenhBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.Thuocs.DuocPham> _duocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.Thuocs.DuongDung> _duongDungRepository;
        private readonly IRepository<Core.Domain.Entities.Thuocs.ToaThuocMau> _toaThuocMauRepository;
        private readonly IRepository<Core.Domain.Entities.Thuocs.ToaThuocMauChiTiet> _toaThuocMauChiTietRepository;

        private readonly IRepository<Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc> _benhNhanDiUngThuocRepository;
        private readonly IRepository<Core.Domain.Entities.NhapKhoDuocPhams.NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        //private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVienGiaBaoHiem> _duocPhamBenhVienGiaBaoHiemRepository;

        private readonly IRepository<PhongBenhVienHangDoi> _phongBenhVienHangDoiRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<Core.Domain.Template> _templateRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChuanDoan> _yeuCauKhamBenhChuanDoanRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuocChiTiet> _yeuCauKhamBenhDonThuocChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu> _ketQuaSinhHieuRepository;
        private readonly IRepository<Core.Domain.Entities.BenhNhans.BenhNhanTienSuBenh> _benhNhanTienSuBenhRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhTrieuChung> _yeuCauKhamBenhTrieuChungRepository;
        private readonly IRepository<Core.Domain.Entities.TrieuChungs.TrieuChung> _trieuChungRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBaoHiem> _dichVuKhamBenhBenhVienGiaBaoHiemGiaRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVienGiaBaoHiem> _dichVuGiuongBenhVienGiaBaoHiemRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVienGiaBenhVien> _dichVuGiuongBenhVienGiaBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.BenhNhans.BenhNhan> _benhNhanRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh> _dichVuKhamBenhRepository;
        private readonly IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private readonly IRepository<DichVuKhamBenhBenhVienNoiThucHien> _dichVuKhamBenhBenhVienNoiThucHienRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private readonly IRepository<Core.Domain.Entities.GoiDichVus.GoiDichVu> _goiDichVuRepository;
        private readonly IRepository<Core.Domain.Entities.BenhVien.BenhVien> _benhVienRepository;

        private readonly IRepository<YeuCauKhamBenhDonThuoc> _yeuCauKhamBenhDonThuocRepository;
        private readonly IRepository<Core.Domain.Entities.DoiTuongUuDais.DoiTuongUuDaiDichVuKhamBenhBenhVien> _doiTuongUuDaiDichVuKhamBenhBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhLichSuTrangThai> _yeuCauKhamBenhLichSuTrangThaiRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.ICDs.ChuanDoan> _chuanDoanRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        private readonly IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> _quanHeThanNhanRepository;
        private readonly IDuocPhamVaVatTuBenhVienService _duocPhamVaVatTuBenhVienService;
        private readonly IRepository<Core.Domain.Entities.DonVatTus.YeuCauKhamBenhDonVTYT> _yeuCauKhamBenhDonVTYTRepository;
        private readonly IRepository<Core.Domain.Entities.DonVatTus.YeuCauKhamBenhDonVTYTChiTiet> _yeuCauKhamBenhDonVTYTChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.DonThuocThanhToans.DonThuocThanhToan> _donThuocThanhToanRepository;
        private readonly IRepository<Core.Domain.Entities.DonThuocThanhToans.DonThuocThanhToanChiTiet> _donThuocThanhToanChiTietRepository;
        private readonly IRepository<DonVTYTThanhToan> _donVTYTThanhToanRepository;
        private readonly IRepository<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IRepository<TuVanThuocKhamSucKhoe> _tuVanThuocKhamSucKhoeRepository;
        private readonly IRepository<ThuocHoacHoatChat> _thuocHoacHoatChatRepository;

        private readonly IRepository<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;

        IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhomRepository;
        private readonly IRepository<NoiDungMauKhamBenh> _noiDungMauKhamBenhRepository;
        private readonly IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository;
        private readonly ILoggerManager _logger;

        public YeuCauKhamBenhService(IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> repository, IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
          IRepository<Core.Domain.Entities.DoiTuongUuTienKhamChuaBenhs.DoiTuongUuTienKhamChuaBenh> doiTuongRepository,
          IRepository<Core.Domain.Entities.DoiTuongUuDais.DoiTuongUuDaiDichVuKhamBenhBenhVien> doiTuongUuDaiDichVuKhamBenhBenhVienRepository,
          IRepository<Core.Domain.Entities.LyDoKhamBenhs.LyDoKhamBenh> lyDoKhamBenhRepository,
          IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
          IRepository<YeuCauKhamBenhDonThuocChiTiet> yeuCauKhamBenhDonThuocChiTietRepository,
          IRepository<YeuCauKhamBenhDonThuoc> yeuCauKhamBenhDonThuocRepository,
          IRepository<Core.Domain.Entities.Users.User> bacSiChiDinhRepository,
          IRepository<Core.Domain.Entities.ICDs.ICD> iCDRepository,
          IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.NhomGiaDichVuKhamBenhBenhVien> nhomGiaDichVuKhamBenhBenhVienRepository,
          IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository,
          IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository,
          IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
          //IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVienGiaBaoHiem> duocPhamBenhVienGiaBaoHiemRepository,
          IRepository<Core.Domain.Entities.Thuocs.DuocPham> duocPhamRepository,
          IRepository<Core.Domain.Entities.Thuocs.DuongDung> duongDungRepository,
          IRepository<Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc> benhNhanDiUngThuocRepository,
          IRepository<Core.Domain.Entities.NhapKhoDuocPhams.NhapKhoDuocPham> nhapKhoDuocPhamRepository,
          IRepository<Core.Domain.Entities.Thuocs.ADR> aDRRepository,
          IRepository<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh> dichVuKhamBenhRepository,
          IRepository<Core.Domain.Entities.BenhVien.BenhVien> benhVienRepository,
          IRepository<Template> templateRepository,
          IRepository<Core.Domain.Entities.Users.User> userRepository,
          IRepository<YeuCauKhamBenhICDKhac> yeuCauKhamBenhICDKhacRepository,
          IRepository<YeuCauKhamBenhChanDoanPhanBiet> yeuCauKhamBenhChanDoanPhanBietRepository,
          IRepository<PhongBenhVienHangDoi> phongBenhVienHangDoiRepository,
          IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> yeuCauTiepNhanRepository,
          IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChuanDoan> yeuCauKhamBenhChuanDoanRepository,
          IRepository<Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu> ketQuaSinhHieuRepository,
          IRepository<Core.Domain.Entities.BenhNhans.BenhNhanTienSuBenh> benhNhanTienSuBenhRepository,
          IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhTrieuChung> yeuCauKhamBenhTrieuChungRepository,
          IRepository<Core.Domain.Entities.TrieuChungs.TrieuChung> trieuChungRepository,
          IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
          IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauVatTuBenhVien> yeuCauVatTuBenhVienRepository,
          IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVienGiaBaoHiem> dichVuKhamBenhBenhVienGiaBaoHiemGiaRepository,
          IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuGiuongBenhVien> yeuCauDichVuGiuongBenhVienRepository,
          IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVienGiaBaoHiem> dichVuGiuongBenhVienGiaBaoHiemRepository,
          IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVienGiaBenhVien> dichVuGiuongBenhVienGiaBenhVienRepository,
          IRepository<Core.Domain.Entities.BenhNhans.BenhNhan> benhNhanRepository,
          IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauGoiDichVu> yeuCauGoiDichVuRepository,
          IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> hoatDongNhanVienRepository,
          IUserAgentHelper userAgentHelper,
          IRepository<Core.Domain.Entities.GoiDichVus.GoiDichVu> goiDichVuRepository,
          IRepository<Core.Domain.Entities.Thuocs.ToaThuocMau> toaThuocMauRepository,
          IRepository<Core.Domain.Entities.Thuocs.ToaThuocMauChiTiet> toaThuocMauChiTietRepository,
          IRepository<DichVuKhamBenhBenhVienNoiThucHien> dichVuKhamBenhBenhVienNoiThucHienRepository,
          IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
          IRepository<Core.Domain.Entities.Localization.LocaleStringResource> localeStringResourceRepository,
          IRepository<Core.Domain.Entities.ICDs.ChuanDoan> chuanDoanRepository,
          ICauHinhService cauHinhService,
          ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
          IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhanLichSuKhamBHYT> yeuCauTiepNhanLichSuKhamBHYTRepository,
          IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
          IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhLichSuTrangThai> yeuCauKhamBenhLichSuTrangThaiRepository,
          IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> quanHeThanNhanRepository,
          IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> inputStringStoredRepository,
          IRepository<VatTuBenhVien> vatTuBenhVienRepository,
          IRepository<Core.Domain.Entities.DonVatTus.YeuCauKhamBenhDonVTYT> yeuCauKhamBenhDonVTYTRepository,
          IRepository<Core.Domain.Entities.DonVatTus.YeuCauKhamBenhDonVTYTChiTiet> yeuCauKhamBenhDonVTYTChiTietRepository,
          IDuocPhamVaVatTuBenhVienService duocPhamVaVatTuBenhVienService,
          IRepository<Core.Domain.Entities.DonThuocThanhToans.DonThuocThanhToan> donThuocThanhToanRepository,
          IRepository<Core.Domain.Entities.DonThuocThanhToans.DonThuocThanhToanChiTiet> donThuocThanhToanChiTietRepository,
          IRepository<DonVTYTThanhToan> donVTYTThanhToanRepository,
          IRepository<DonVTYTThanhToanChiTiet> donVTYTThanhToanChiTietRepository,
          IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
          ILocalizationService localizationService,
          IYeuCauTiepNhanService yeuCauTiepNhanService,
          IRepository<TuVanThuocKhamSucKhoe> tuVanThuocKhamSucKhoeRepository,
          IRepository<ThuocHoacHoatChat> thuocHoacHoatChatRepository,
          IRepository<DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository,
          IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhomRepository,
          IRepository<NoiDungMauKhamBenh> noiDungMauKhamBenhRepository,
          IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepository,
          ILoggerManager logger,
          IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository,
          IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository,
          IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository
        ) : base(repository)
        {
            _logger = logger;
            _benhVienRepository = benhVienRepository;
            _toaThuocMauRepository = toaThuocMauRepository;
            _toaThuocMauChiTietRepository = toaThuocMauChiTietRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _doiTuongRepository = doiTuongRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _bacSiChiDinhRepository = bacSiChiDinhRepository;
            _phongBenhVienHangDoiRepository = phongBenhVienHangDoiRepository;
            _templateRepository = templateRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _yeuCauKhamBenhChuanDoanRepository = yeuCauKhamBenhChuanDoanRepository;
            _yeuCauKhamBenhDonThuocChiTietRepository = yeuCauKhamBenhDonThuocChiTietRepository;
            _iCDRepository = iCDRepository;
            _ketQuaSinhHieuRepository = ketQuaSinhHieuRepository;
            _benhNhanDiUngThuocRepository = benhNhanDiUngThuocRepository;
            _duocPhamRepository = duocPhamRepository;
            _benhNhanTienSuBenhRepository = benhNhanTienSuBenhRepository;
            _yeuCauKhamBenhTrieuChungRepository = yeuCauKhamBenhTrieuChungRepository;
            _trieuChungRepository = trieuChungRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _yeuCauVatTuBenhVienRepository = yeuCauVatTuBenhVienRepository;
            _aDRRepository = aDRRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            //_duocPhamBenhVienGiaBaoHiemRepository = duocPhamBenhVienGiaBaoHiemRepository;
            _duocPhamRepository = duocPhamRepository;
            _benhNhanDiUngThuocRepository = benhNhanDiUngThuocRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _yeuCauKhamBenhDonThuocChiTietRepository = yeuCauKhamBenhDonThuocChiTietRepository;
            _yeuCauKhamBenhDonThuocRepository = yeuCauKhamBenhDonThuocRepository;
            _nhomGiaDichVuKhamBenhBenhVienRepository = nhomGiaDichVuKhamBenhBenhVienRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _dichVuKhamBenhBenhVienGiaBaoHiemGiaRepository = dichVuKhamBenhBenhVienGiaBaoHiemGiaRepository;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _doiTuongUuDaiDichVuKhamBenhBenhVienRepository = doiTuongUuDaiDichVuKhamBenhBenhVienRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _goiDichVuRepository = goiDichVuRepository;
            _userAgentHelper = userAgentHelper;
            _duongDungRepository = duongDungRepository;
            _yeuCauKhamBenhICDKhacRepository = yeuCauKhamBenhICDKhacRepository;
            _yeuCauKhamBenhChanDoanPhanBietRepository = yeuCauKhamBenhChanDoanPhanBietRepository;
            _dichVuKhamBenhRepository = dichVuKhamBenhRepository;
            _localeStringResourceRepository = localeStringResourceRepository;
            _dichVuKhamBenhBenhVienNoiThucHienRepository = dichVuKhamBenhBenhVienNoiThucHienRepository;
            _chuanDoanRepository = chuanDoanRepository;
            _hoatDongNhanVienRepository = hoatDongNhanVienRepository;
            _nhanVienRepository = nhanVienRepository;
            _yeuCauTiepNhanLichSuKhamBHYTRepository = yeuCauTiepNhanLichSuKhamBHYTRepository;
            _cauHinhService = cauHinhService;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _userRepository = userRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _yeuCauDichVuGiuongBenhVienRepository = yeuCauDichVuGiuongBenhVienRepository;
            _yeuCauKhamBenhLichSuTrangThaiRepository = yeuCauKhamBenhLichSuTrangThaiRepository;
            _quanHeThanNhanRepository = quanHeThanNhanRepository;
            _inputStringStoredRepository = inputStringStoredRepository;
            _duocPhamVaVatTuBenhVienService = duocPhamVaVatTuBenhVienService;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _yeuCauKhamBenhDonVTYTChiTietRepository = yeuCauKhamBenhDonVTYTChiTietRepository;
            _yeuCauKhamBenhDonVTYTRepository = yeuCauKhamBenhDonVTYTRepository;
            _donThuocThanhToanRepository = donThuocThanhToanRepository;
            _donThuocThanhToanChiTietRepository = donThuocThanhToanChiTietRepository;
            _donVTYTThanhToanRepository = donVTYTThanhToanRepository;
            _donVTYTThanhToanChiTietRepository = donVTYTThanhToanChiTietRepository;
            _cauHinhRepository = cauHinhRepository;
            _localizationService = localizationService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _tuVanThuocKhamSucKhoeRepository = tuVanThuocKhamSucKhoeRepository;
            _thuocHoacHoatChatRepository = thuocHoacHoatChatRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
            _duocPhamBenhVienPhanNhomRepository = duocPhamBenhVienPhanNhomRepository;
            _noiDungMauKhamBenhRepository = noiDungMauKhamBenhRepository;
            _noiTruPhieuDieuTriRepository = noiTruPhieuDieuTriRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository;
        }

        public string GetBodyByName(string ten)
        {
            var result = _templateRepository.Table.AsNoTracking()
                .OrderByDescending(k => k.Version)
                .Where(o => o.Name == ten)
                .Select(o => o.Body)
                .FirstOrDefault();
            return result;
        }

        public string GetResourceValueByResourceName(string ten)
        {
            var result = _localeStringResourceRepository.Table.AsNoTracking()
                .Where(o => o.ResourceName.Contains(ten))
                .Select(o => o.ResourceValue)
                .FirstOrDefault();
            return result;
        }

        public async Task<List<ICDTemplateVo>> GetListICDBaoGomItemDaChonAsync(DropDownListRequestModel queryInfo)
        {
            var lstICDs = new List<ICDTemplateVo>();
            var lstColumnNameSearch = new List<string>
            {
                "Ma",
                "TenTiengViet"
            };

            var chanDoanSoBoICDId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                //var chanDoanSoBoICDId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
                lstICDs = await _iCDRepository.TableNoTracking
                    //.ApplyFulltext(queryInfo.Query, nameof(ICD), lstColumnNameSearch)
                    .Where(x => x.Id == chanDoanSoBoICDId || x.HieuLuc == true)
                    .Select(item => new ICDTemplateVo
                    {
                        DisplayName = item.Ma + " - " + item.TenTiengViet, //item.TenTiengViet,
                        KeyId = item.Id,
                        Ten = item.TenTiengViet,
                        Ma = item.Ma,
                    })
                    .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                    .OrderByDescending(x => x.KeyId == chanDoanSoBoICDId).ThenBy(x => x.KeyId)
                    .Take(queryInfo.Take).ToListAsync();
            }
            else
            {
                var lstICDId = await _iCDRepository
                    .ApplyFulltext(queryInfo.Query, nameof(ICD), lstColumnNameSearch)
                    .Where(x => x.Id == chanDoanSoBoICDId || x.HieuLuc == true)
                    .Select(x => x.Id)
                    .ToListAsync();
                lstICDs = await _iCDRepository.TableNoTracking
                    //.ApplyFulltext(queryInfo.Query, nameof(ICD), lstColumnNameSearch)
                    .Where(x => lstICDId.Contains(x.Id)) // &&  (x.Id == chanDoanSoBoICDId || x.HieuLuc == true))
                    .OrderByDescending(x => x.Id == chanDoanSoBoICDId)
                    .ThenBy(p => lstICDId.IndexOf(p.Id) != -1 ? lstICDId.IndexOf(p.Id) : queryInfo.Take + 1)
                    .Take(queryInfo.Take)
                    .Select(item => new ICDTemplateVo
                    {
                        DisplayName = item.Ma + " - " + item.TenTiengViet, //item.TenTiengViet,
                        KeyId = item.Id,
                        Ten = item.TenTiengViet,
                        Ma = item.Ma,
                    }).ToListAsync();
            }

            //var lstICDs = await _iCDRepository.TableNoTracking
            //    .ApplyLike(queryInfo.Query, o => o.Ma, o => o.TenTiengViet, o => o.TenTiengAnh)
            //    .Take(queryInfo.Take)
            //    .Select(item => new ICDTemplateVo
            //    {
            //        DisplayName = item.Ma + " - " + item.TenTiengViet,
            //        KeyId = item.Id,
            //        Ten = item.TenTiengViet,
            //        Ma = item.Ma,
            //    }).ToListAsync();

            //if (queryInfo.Id != 0)
            //{
            //    var icdDaChon = await _iCDRepository.TableNoTracking
            //        .Where(x => x.Id == queryInfo.Id)
            //        .Select(item => new ICDTemplateVo()
            //        {
            //            DisplayName = item.Ma + " - " + item.TenTiengViet,
            //            KeyId = item.Id,
            //            Ten = item.TenTiengViet,
            //            Ma = item.Ma,
            //        }).FirstOrDefaultAsync();

            //    if (icdDaChon != null && !lstICDs.Any(x => x.KeyId == icdDaChon.KeyId))
            //    {
            //        lstICDs.Add(icdDaChon);
            //    }
            //}

            return lstICDs;
        }

        public async Task KiemTraDatayeuCauKhamBenhAsync(long yeuCauKhamBenhId, long phongBenhVienHangDoiId = 0, Enums.EnumTrangThaiHangDoi trangThaiHangDoi = Enums.EnumTrangThaiHangDoi.DangKham)
        {
            //todo: có update bỏ await
            var resourceName = "";
            var yeuCauKhamBenh = BaseRepository.TableNoTracking
                .Include(x => x.PhongBenhVienHangDois)
                .FirstOrDefault(x => x.Id == yeuCauKhamBenhId);

            if (yeuCauKhamBenh == null)
            {
                resourceName = "KhamBenh.YeuCauKhamBenh.NotExists";
            }
            else
            {
                switch (yeuCauKhamBenh.TrangThai)
                {
                    case Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham:
                        resourceName = "KhamBenh.YeuCauKhamBenh.DaHuy"; break;
                    case Enums.EnumTrangThaiYeuCauKhamBenh.DaKham:
                        resourceName = "KhamBenh.YeuCauKhamBenh.DaHoanThanhKham"; break;

                    default:
                        resourceName = ""; break;
                }

                // kiểm tra trạng thái hàng đợi, nếu hàng đợi ko phải đang khám thì báo lỗi thay đổi trạng thái
                if (trangThaiHangDoi == Enums.EnumTrangThaiHangDoi.DangKham
                    && string.IsNullOrEmpty(resourceName)
                    && yeuCauKhamBenh.PhongBenhVienHangDois.Any(x => x.YeuCauDichVuKyThuatId == null && (phongBenhVienHangDoiId == 0 || x.Id == phongBenhVienHangDoiId)))
                {
                    var phongBenhVienHangDoiHienTai =
                        yeuCauKhamBenh.PhongBenhVienHangDois.First(x => x.YeuCauDichVuKyThuatId == null && (phongBenhVienHangDoiId == 0 || x.Id == phongBenhVienHangDoiId));

                    if (phongBenhVienHangDoiHienTai.TrangThai != Enums.EnumTrangThaiHangDoi.DangKham)
                    {
                        resourceName = "KhamBenh.PhongBenhVienHangDoi.TrangThaiBiThayDoi";
                    }

                }
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                var currentUserLanguge = _userAgentHelper.GetUserLanguage();
                var mess = _localeStringResourceRepository.TableNoTracking
                    .Where(x => x.ResourceName == resourceName && x.Language == (int)currentUserLanguge)
                    .Select(x => x.ResourceValue).FirstOrDefault();
                throw new Exception(mess ?? resourceName);
            }
        }

        public void KiemTraChanDoanSoBoKhiThemDichVu(YeuCauTiepNhan yeuCauTiepNhan,long yeuCauKhamBenhId)
        {
            if (yeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
            {
                var yeuCauKhamDangKham =
                    yeuCauTiepNhan.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauKhamBenhId);
                if (yeuCauKhamDangKham != null && yeuCauKhamDangKham.ChanDoanSoBoICDId == null)
                {
                    throw new Exception(_localizationService.GetResource("ChiDinhDichVu.ChanDoanSoBoICD.Required"));
                }
            }
        }

        public async Task<List<NoiDungMauKhamBenhLookupItemVo>> GetListNoiDungMauKhamBenhTheoBacSiAsync(DropDownListRequestModel queryInfo)
        {
            var bacSiHienTaiId = _userAgentHelper.GetCurrentUserId();
            var lstNoiDungMau = await _noiDungMauKhamBenhRepository.TableNoTracking
                .Where(x => x.BacSiId == bacSiHienTaiId)
                .OrderByDescending(x => x.Id == queryInfo.Id).ThenBy(x => x.CreatedOn)
                .Select(item => new NoiDungMauKhamBenhLookupItemVo()
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                    Ten = item.Ten,
                    Ma = item.Ma,
                    NoiDung = item.NoiDung
                })
                .ApplyLike(queryInfo.Query, x => x.Ma, x => x.Ten)
                .Take(queryInfo.Take).ToListAsync();
            return lstNoiDungMau;
        }
        #region BVHD-3698
        public void SaveCapNhatNgayDuongThai(long yeuCauKhamBenhId, DateTime tuNgay, DateTime denNgay)
        {
            var yckb = BaseRepository.GetById(yeuCauKhamBenhId);
            yckb.DuongThaiTuNgay = tuNgay;
            yckb.DuongThaiDenNgay = denNgay;

            long userId = _userAgentHelper.GetCurrentUserId();
            yckb.DuongThaiNguoiInId = userId;
            yckb.DuongThaiNgayIn = DateTime.Now;

            BaseRepository.Context.SaveChanges();
        }
        public async Task<string> PhieuNghiDuongThai(long yeuCauKhamBenhId)
        {
            var content = string.Empty;
            
            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuNghiDuongThai"));

            var emt = BaseRepository.TableNoTracking.Where(d => d.Id == yeuCauKhamBenhId).ToList();

            var thongTinBnInfo = BaseRepository.TableNoTracking.Where(d => d.Id == yeuCauKhamBenhId)
                .Select(d => new {
                    HoVaTen = d.YeuCauTiepNhan.HoTen,
                    NgaySinh  = DateHelper.DOBFormat(d.YeuCauTiepNhan.NgaySinh, d.YeuCauTiepNhan.ThangSinh, d.YeuCauTiepNhan.NamSinh),
                    MaBHXH = d.YeuCauTiepNhan.BHYTMaSoThe,// ngoài trú
                    DonViLamViec = d.YeuCauTiepNhan.NoiLamViec,
                    ChanDoan = MaskHelper.ICDDisplay(d.Icdchinh.Ma,d.Icdchinh.TenTiengViet,d.GhiChuICDChinh, Enums.KieuHienThiICD.MaGachNgangGhiChu),
                    TuNgay = d.DuongThaiTuNgay ,
                    DenNgay =d.DuongThaiDenNgay,
                }).FirstOrDefault();
            var soNgay = string.Empty;
           if(thongTinBnInfo.TuNgay != null && thongTinBnInfo.DenNgay != null)
            {
                DateTime start = new DateTime(thongTinBnInfo.TuNgay.Value.Year, thongTinBnInfo.TuNgay.Value.Month, thongTinBnInfo.TuNgay.Value.Day);
                DateTime end = new DateTime(thongTinBnInfo.DenNgay.Value.Year, thongTinBnInfo.DenNgay.Value.Month, thongTinBnInfo.DenNgay.Value.Day);

                List<string> returnHtml = new List<string>();
                TimeSpan difference = end - start;
                soNgay = difference.Days + "";
            }
           
            var data = new 
            {
                HoVaTen = thongTinBnInfo.HoVaTen,
                NgaySinh = thongTinBnInfo.NgaySinh,
                MaSoBHXH= thongTinBnInfo.MaBHXH,
                DonViLamViec= thongTinBnInfo.DonViLamViec,
                ChanDoan = thongTinBnInfo.ChanDoan,
                SoNgay = soNgay,
                DayNow = DateTime.Now.Day,
                MonthNow = DateTime.Now.Month,
                YearNow = DateTime.Now.Year,
                TuNgay = thongTinBnInfo.TuNgay.Value.ApplyFormatDate(),
                DenNgay = thongTinBnInfo.DenNgay.Value.ApplyFormatDate()
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        public NgayDuongThaiYeuCauKhamBenhVo GetNgayDuongThaiYeuCauKhamBenh(long yeuCauKhamBenhId)
        {
            var vo = BaseRepository.TableNoTracking.Where(d => d.Id == yeuCauKhamBenhId)
                .Select(d => new NgayDuongThaiYeuCauKhamBenhVo()
                {
                    TuNgay = d.DuongThaiTuNgay, 
                    DenNgay = d.DuongThaiDenNgay,
                    YeuCauKhamBenhId = yeuCauKhamBenhId
                }).FirstOrDefault();
            return vo;
        }
        #endregion
    }
}
