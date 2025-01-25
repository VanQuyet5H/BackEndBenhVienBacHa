using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.BenhVien.Khoas;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.CongTyUuDais;
using Camino.Core.Domain.Entities.DichVuBenhVienTongHops;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Camino.Core.Domain.Entities.DoiTuongUuTienKhamChuaBenhs;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.HinhThucDens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DanhMucMarketing;
using Camino.Core.Domain.ValueObject.DichVuKhuyenMai;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.LyDoTiepNhan;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.Users;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.TiepNhanBenhNhan
{
    [ScopedDependency(ServiceType = typeof(ITiepNhanBenhNhanService))]
    public partial class TiepNhanBenhNhanService : YeuCauTiepNhanBaseService, ITiepNhanBenhNhanService
    {
        private readonly IRepository<Core.Domain.Entities.LyDoKhamBenhs.LyDoKhamBenh> _lyDoKhamBenhRepository;
        private readonly IRepository<Khoa> _khoaRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IRepository<DoiTuongUuTienKhamChuaBenh> _doiTuongUuTienKhamChuaBenhRepository;

        private readonly IRepository<Core.Domain.Entities.QuocGias.QuocGia> _quocGiaRepository;
        private readonly IRepository<Core.Domain.Entities.DanTocs.DanToc> _danTocRepository;
        private readonly IRepository<Core.Domain.Entities.NgheNghieps.NgheNghiep> _ngheNghiepRepository;
        private readonly IRepository<Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh> _donViHanhChinhRepository;

        private readonly IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> _quanHeThanNhanRepository;
        private readonly IRepository<BenhNhan> _benhNhanRepository;

        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;

        private readonly IRepository<CongTyUuDai> _congTyUuDaiRepository;
        private readonly IRepository<DoiTuongUuDai> _doiTuongUuDaiRepository;
        private readonly IRepository<HinhThucDen> _hinhThucDenRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu> _noiGioiThieuRepository;
        private readonly IRepository<Core.Domain.Entities.NguoiGioiThieus.NguoiGioiThieu> _nguoiGioiThieuRepository;
        private readonly IRepository<Core.Domain.Entities.TrieuChungs.TrieuChung> _trieuChungRepository;
        private readonly IRepository<NhomGiaDichVuKhamBenhBenhVien> _nhomGiaDichVuKhamBenhRepository;
        private readonly IRepository<NhomGiaDichVuKyThuatBenhVien> _nhomGiaDichVuKyThuatBenhVienRepository;
        private readonly IRepository<NhomGiaDichVuGiuongBenhVien> _nhomGiaDichVuGiuongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh> _dichVuKhamBenhRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> _dichVuKyThuatRepository;

        private readonly IRepository<DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;

        private readonly IRepository<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private readonly IRepository<DichVuKhamBenhBenhVienGiaBaoHiem> _dichVuKhamBenhBenhVienGiaBaoHiemRepository;

        private readonly IRepository<DichVuKyThuatBenhVienGiaBaoHiem> _dichVuKyThuatBenhVienGiaBaoHiemRepository;
        private readonly IRepository<DichVuGiuongBenhVienGiaBaoHiem> _dichVuGiuongBenhVienGiaBaoHiemRepository;

        private readonly IRepository<DoiTuongUuDaiDichVuKhamBenhBenhVien> _doiTuongUuDaiDichVuKhamBenhBenhVienRepository;
        private readonly IRepository<DoiTuongUuDaiDichVuKyThuatBenhVien> _doiTuongUuDaiDichVuKyThuatBenhVienRepository;

        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;

        private readonly IRepository<Core.Domain.Entities.BenhVien.BenhVien> _benhVienRepository;

        private readonly IRepository<CongTyBaoHiemTuNhan> _congTyBaoHiemTuNhanRepository;

        private readonly IRepository<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;

        private readonly IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> _dichVuGiuongBenhVienRepository;

        private readonly IRepository<DichVuGiuongBenhVienGiaBenhVien> _dichVuGiuongBenhVienGiaBenhVienRepository;

        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;

        private readonly IRepository<GoiDichVu> _goiDichVuRepository;

        private readonly IUserAgentHelper _userAgentHelper;

        private readonly IRepository<LoaiHoSoYeuCauTiepNhan> _loaiHoSoYeuCauTiepNhanRepository;

        private readonly IRepository<CongTyBaoHiemTuNhanCongNo> _congTuBaoHiemTuNhanCongNoRepository;

        private readonly IRepository<DichVuGiuongBenhVienNoiThucHien> _dichVuGiuongBenhVienNoiThucHienRepository;
        private readonly IRepository<DichVuKhamBenhBenhVienNoiThucHien> _dichVuKhamBenhBenhVienNoiThucHienRepository;
        private readonly IRepository<DichVuKyThuatBenhVienNoiThucHien> _dichVuKyThuatBenhVienNoiThucHienRepository;
        private readonly IRepository<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan> _lyDoTiepNhanRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;

        private readonly IRepository<DichVuBenhVienTongHop> _dichVuBenhVienTongHopRepository;

        private readonly IRepository<ChuongTrinhGoiDichVu> _chuongTrinhGoiDichVuRepository;
        private readonly IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private readonly IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepository;

        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
        private readonly IRepository<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienRepository;

        private readonly IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> _hoatDongNhanVienRepository;
        private readonly IRepository<GoiDichVuChiTietDichVuKhamBenh> _goiDichVuChiTietDichVuKhamBenhRepository;
        private readonly IRepository<GoiDichVuChiTietDichVuKyThuat> _goiDichVuChiTietDichVuKyThuatRepository;
        private readonly IRepository<GoiDichVuChiTietDichVuGiuong> _goiDichVuChiTietDichVuGiuongRepository;

        private readonly IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> _noiTruBenhAnRepository;


        //service
        private readonly ICauHinhService _cauHinhService;
        private readonly ILocalizationService _localizationService;
        private readonly IRoleService _roleService;

        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;

        private readonly IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        private readonly IRepository<DonThuocThanhToan> _donThuocThanhToanRepository;
        private readonly IRepository<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienRepository;

        private readonly IRepository<YeuCauTiepNhanCongTyBaoHiemTuNhan> _yeuCauTiepNhanCongTyBaoHiemTuNhanRepository;
        private readonly IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiRepository;
        private readonly IRepository<DichVuKyThuatBenhVienNoiThucHienUuTien> _dichVuKyThuatBenhVienNoiThucHienUuTienRepository;

        private readonly IRepository<MienGiamChiPhi> _mienGiamChiPhiRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> _chuongTrinhGoiDichVuDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> _chuongTrinhGoiDichVuDichVuKyThuatRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuGiuong> _chuongTrinhGoiDichVuDichVuGiuongRepository;

        private readonly IRepository<Camino.Core.Domain.Entities.HocViHocHams.HocViHocHam> _hocViHocHamRepository;
        private readonly IRepository<Template> _templateRepository;

        public TiepNhanBenhNhanService(IRepository<YeuCauTiepNhan> repository
            , IRepository<ChuongTrinhGoiDichVu> chuongTrinhGoiDichVuRepository, IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository
            , IRepository<Core.Domain.Entities.LyDoKhamBenhs.LyDoKhamBenh> lyDoKhamBenhRepository, IRepository<Khoa> khoaRepository
            , IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository
            , IRepository<DoiTuongUuTienKhamChuaBenh> doiTuongUuTienKhamChuaBenhRepository
            , IRepository<Core.Domain.Entities.QuocGias.QuocGia> quocGiaRepository
            , IRepository<Core.Domain.Entities.DanTocs.DanToc> danTocRepository
            , IRepository<Core.Domain.Entities.NgheNghieps.NgheNghiep> ngheNghiepRepository
            , IRepository<Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh> donViHanhChinhRepository
            , IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> quanHeThanNhanRepository
            , IRepository<BenhNhan> benhNhanRepository
            , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
            , IRepository<CongTyUuDai> congTyUuDaiRepository, IRepository<DoiTuongUuDai> doiTuongUuDaiRepository
            , IRepository<HinhThucDen> hinhThucDenRepository, IRepository<Camino.Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu> noiGioiThieuRepository
            , IRepository<Core.Domain.Entities.TrieuChungs.TrieuChung> trieuChungRepository
            , IRepository<NhomGiaDichVuKhamBenhBenhVien> nhomGiaDichVuKhamBenhRepository
            , IRepository<NhomGiaDichVuKyThuatBenhVien> nhomGiaDichVuKyThuatBenhVienRepository
            , IRepository<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh> dichVuKhamBenhRepository
            , IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> dichVuKyThuatRepository
            , IRepository<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository
            , IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository
            , IRepository<DichVuKhamBenhBenhVienGiaBaoHiem> dichVuKhamBenhBenhVienGiaBaoHiemRepository

            , IRepository<DichVuKyThuatBenhVienGiaBaoHiem> dichVuKyThuatBenhVienGiaBaoHiemRepository
            , IRepository<DichVuGiuongBenhVienGiaBaoHiem> dichVuGiuongBenhVienGiaBaoHiemRepository

            , IRepository<DichVuGiuongBenhVienNoiThucHien> dichVuGiuongBenhVienNoiThucHienRepository
            , IRepository<DichVuKhamBenhBenhVienNoiThucHien> dichVuKhamBenhBenhVienNoiThucHienRepository
            , IRepository<DichVuKyThuatBenhVienNoiThucHien> dichVuKyThuatBenhVienNoiThucHienRepository
            , IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository
            , IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository
            , IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository
            , IRepository<DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository
            , IRepository<DoiTuongUuDaiDichVuKhamBenhBenhVien> doiTuongUuDaiDichVuKhamBenhBenhVienRepository
            , IRepository<DoiTuongUuDaiDichVuKyThuatBenhVien> doiTuongUuDaiDichVuKyThuatBenhVienRepository
            , ICauHinhService cauHinhService
            , IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository
            , IRepository<Core.Domain.Entities.BenhVien.BenhVien> benhVienRepository
            , IRepository<CongTyBaoHiemTuNhan> congTyBaoHiemTuNhanRepository
            , IRepository<DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository
            , IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> dichVuGiuongBenhVienRepository
            , IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository
            , IUserAgentHelper userAgentHelper, IRepository<LoaiHoSoYeuCauTiepNhan> loaiHoSoYeuCauTiepNhanRepository
            , IRepository<DichVuGiuongBenhVienGiaBenhVien> dichVuGiuongBenhVienGiaBenhVienRepository, IRepository<GoiDichVu> goiDichVuRepository
            , ILocalizationService localizationService
            , ITaiKhoanBenhNhanService taiKhoanBenhNhanService
            , IRepository<CongTyBaoHiemTuNhanCongNo> congTuBaoHiemTuNhanCongNoRepository
            , IRepository<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan> lyDoTiepNhanRepository
            , IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository
            , IRepository<Core.Domain.Entities.NguoiGioiThieus.NguoiGioiThieu> nguoiGioiThieuRepository
            , IRepository<DichVuBenhVienTongHop> dichVuBenhVienTongHopRepository
            , IRepository<YeuCauDichVuGiuongBenhVien> yeuCauDichVuGiuongBenhVienRepository
            , IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository
            , IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepository
            , IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> hoatDongNhanVienRepository
            , IRepository<GoiDichVuChiTietDichVuKhamBenh> goiDichVuChiTietDichVuKhamBenhRepository
            , IRepository<GoiDichVuChiTietDichVuKyThuat> goiDichVuChiTietDichVuKyThuatRepository
            , IRepository<GoiDichVuChiTietDichVuGiuong> goiDichVuChiTietDichVuGiuongRepository
            , IRoleService roleService,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository
            , IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> noiTruBenhAnRepository
            , IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository
            , IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository
            , IRepository<DonThuocThanhToan> donThuocThanhToanRepository
            , IRepository<YeuCauVatTuBenhVien> yeuCauVatTuBenhVienRepository
            , IRepository<YeuCauTiepNhanCongTyBaoHiemTuNhan> yeuCauTiepNhanCongTyBaoHiemTuNhanRepository
            , IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChiRepository
            , IRepository<DichVuKyThuatBenhVienNoiThucHienUuTien> dichVuKyThuatBenhVienNoiThucHienUuTienRepository
            , IRepository<MienGiamChiPhi> mienGiamChiPhiRepository
            , IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> chuongTrinhGoiDichVuDichVuKhamBenhRepository
            , IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> chuongTrinhGoiDichVuDichVuKyThuatRepository
            , IRepository<ChuongTrinhGoiDichVuDichVuGiuong> chuongTrinhGoiDichVuDichVuGiuongRepository
            , IRepository<Camino.Core.Domain.Entities.HocViHocHams.HocViHocHam> hocViHocHamRepository
            , IRepository<Template> templateRepository
            ) : base(repository, userAgentHelper, cauHinhService, localizationService, taiKhoanBenhNhanService)
        {
            _chuongTrinhGoiDichVuRepository = chuongTrinhGoiDichVuRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _lyDoKhamBenhRepository = lyDoKhamBenhRepository;
            _khoaRepository = khoaRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _doiTuongUuTienKhamChuaBenhRepository = doiTuongUuTienKhamChuaBenhRepository;
            _quocGiaRepository = quocGiaRepository;
            _danTocRepository = danTocRepository;
            _ngheNghiepRepository = ngheNghiepRepository;
            _donViHanhChinhRepository = donViHanhChinhRepository;
            _quanHeThanNhanRepository = quanHeThanNhanRepository;
            _benhNhanRepository = benhNhanRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _congTyUuDaiRepository = congTyUuDaiRepository;
            _doiTuongUuDaiRepository = doiTuongUuDaiRepository;
            _hinhThucDenRepository = hinhThucDenRepository;
            _noiGioiThieuRepository = noiGioiThieuRepository;
            _trieuChungRepository = trieuChungRepository;
            _nhomGiaDichVuKhamBenhRepository = nhomGiaDichVuKhamBenhRepository;
            _nhomGiaDichVuKyThuatBenhVienRepository = nhomGiaDichVuKyThuatBenhVienRepository;
            _dichVuKhamBenhRepository = dichVuKhamBenhRepository;
            _dichVuKyThuatRepository = dichVuKyThuatRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _dichVuKhamBenhBenhVienGiaBaoHiemRepository = dichVuKhamBenhBenhVienGiaBaoHiemRepository;

            _dichVuKyThuatBenhVienGiaBaoHiemRepository = dichVuKyThuatBenhVienGiaBaoHiemRepository;
            _dichVuGiuongBenhVienGiaBaoHiemRepository = dichVuGiuongBenhVienGiaBaoHiemRepository;

            _dichVuGiuongBenhVienNoiThucHienRepository = dichVuGiuongBenhVienNoiThucHienRepository;
            _dichVuKhamBenhBenhVienNoiThucHienRepository = dichVuKhamBenhBenhVienNoiThucHienRepository;
            _dichVuKyThuatBenhVienNoiThucHienRepository = dichVuKyThuatBenhVienNoiThucHienRepository;

            _doiTuongUuDaiDichVuKhamBenhBenhVienRepository = doiTuongUuDaiDichVuKhamBenhBenhVienRepository;
            _doiTuongUuDaiDichVuKyThuatBenhVienRepository = doiTuongUuDaiDichVuKyThuatBenhVienRepository;
            _cauHinhService = cauHinhService;
            _nhanVienRepository = nhanVienRepository;
            _benhVienRepository = benhVienRepository;
            _congTyBaoHiemTuNhanRepository = congTyBaoHiemTuNhanRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
            _dichVuGiuongBenhVienRepository = dichVuGiuongBenhVienRepository;
            _dichVuGiuongBenhVienGiaBenhVienRepository = dichVuGiuongBenhVienGiaBenhVienRepository;
            _goiDichVuRepository = goiDichVuRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _loaiHoSoYeuCauTiepNhanRepository = loaiHoSoYeuCauTiepNhanRepository;
            _userAgentHelper = userAgentHelper;
            _congTuBaoHiemTuNhanCongNoRepository = congTuBaoHiemTuNhanCongNoRepository;
            _lyDoTiepNhanRepository = lyDoTiepNhanRepository;
            _nguoiGioiThieuRepository = nguoiGioiThieuRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;

            _dichVuBenhVienTongHopRepository = dichVuBenhVienTongHopRepository;
            _localizationService = localizationService;
            _noiTruPhieuDieuTriRepository = noiTruPhieuDieuTriRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
            _yeuCauDichVuGiuongBenhVienRepository = yeuCauDichVuGiuongBenhVienRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;

            _hoatDongNhanVienRepository = hoatDongNhanVienRepository;

            _goiDichVuChiTietDichVuKhamBenhRepository = goiDichVuChiTietDichVuKhamBenhRepository;
            _goiDichVuChiTietDichVuKyThuatRepository = goiDichVuChiTietDichVuKyThuatRepository;
            _goiDichVuChiTietDichVuGiuongRepository = goiDichVuChiTietDichVuGiuongRepository;

            _noiTruBenhAnRepository = noiTruBenhAnRepository;
            _roleService = roleService;
            _cauHinhRepository = cauHinhRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _donThuocThanhToanRepository = donThuocThanhToanRepository;
            _yeuCauVatTuBenhVienRepository = yeuCauVatTuBenhVienRepository;
            _yeuCauTiepNhanCongTyBaoHiemTuNhanRepository = yeuCauTiepNhanCongTyBaoHiemTuNhanRepository;
            _taiKhoanBenhNhanChiRepository = taiKhoanBenhNhanChiRepository;
            _dichVuKyThuatBenhVienNoiThucHienUuTienRepository = dichVuKyThuatBenhVienNoiThucHienUuTienRepository;
            _mienGiamChiPhiRepository = mienGiamChiPhiRepository;

            _chuongTrinhGoiDichVuDichVuKhamBenhRepository = chuongTrinhGoiDichVuDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuDichVuKyThuatRepository = chuongTrinhGoiDichVuDichVuKyThuatRepository;
            _chuongTrinhGoiDichVuDichVuGiuongRepository = chuongTrinhGoiDichVuDichVuGiuongRepository;
            _hocViHocHamRepository = hocViHocHamRepository;
            _templateRepository = templateRepository;
        }

        //public async Task<List<LookupItemVo>> GetLyDoKhamBenh(DropDownListRequestModel queryInfo)
        //{
        //    var lstLyDoKhamBenh = await _lyDoKhamBenhRepository.TableNoTracking
        //        .Where(p => p.IsDisabled != true && p.Ten.Contains(queryInfo.Query ?? ""))
        //        .Take(queryInfo.Take)
        //        .ToListAsync();

        //    var query = lstLyDoKhamBenh.Select(item => new LookupItemVo
        //    {
        //        DisplayName = item.Ten,
        //        KeyId = item.Id,
        //    }).ToList();

        //    return query;
        //}

        public async Task<List<KhoaKhamTemplateVo>> GetKhoaKham(DropDownListRequestModel model)
        {
            var lstKhoaKham = await _khoaPhongRepository.TableNoTracking
                .Where(p => p.IsDisabled != true && p.CoKhamNgoaiTru != null && p.CoKhamNgoaiTru == true
                            && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")))
                .Take(model.Take)
                .ToListAsync();

            var query = lstKhoaKham.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma,
            }).ToList();

            return query;
        }

        public async Task<List<PhongKhamTemplateVo>> GetPhongKham(DropDownListRequestModel model, int tongSoNguoiKham)
        {
            var dichVuKhamBenhBenhVienId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (dichVuKhamBenhBenhVienId == 0)
            {
                return new List<PhongKhamTemplateVo>();
            }
            //var dichVuKhamBenhBenhVienEntity =
            //    await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh).FirstOrDefaultAsync(p => p.Id == dichVuKhamBenhBenhVienId);
            var lstKhoaPhongKhamId = new List<long>();
            var lstPhongBenhVienId = new List<long>();
            //if (dichVuKhamBenhBenhVienEntity != null)
            //{
            //    //khoaPhongKhamId = dichVuKhamBenhBenhVienEntity.KhoaPhongId;
            //}

            var noiThucHien = await _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking.Where(p => p.DichVuKhamBenhBenhVienId == dichVuKhamBenhBenhVienId).ToListAsync();

            var lstPhongKham = new List<Core.Domain.Entities.PhongBenhViens.PhongBenhVien>();

            if (noiThucHien.Any())
            {
                foreach (var item in noiThucHien)
                {
                    if (item.KhoaPhongId != null)
                    {
                        lstKhoaPhongKhamId.Add(item.KhoaPhongId ?? 0);
                    }
                    if (item.PhongBenhVienId != null)
                    {
                        lstPhongBenhVienId.Add(item.PhongBenhVienId ?? 0);
                    }
                }
            }
            else
            {
                lstPhongBenhVienId.AddRange(_phongBenhVienRepository.TableNoTracking.Where(p => p.IsDisabled != true).Select(p => p.Id));
            }

            //var checkTime = TimeSpan.Compare(DateTime.Now.TimeOfDay, Convert.ToDateTime("12:00:00 PM").TimeOfDay);
            //var lstPhongKham = await _phongBenhVienRepository.TableNoTracking
            //    .Include(p => p.LichPhanCongNgoaiTrus).ThenInclude(p => p.NhanVien).ThenInclude(p => p.User)
            //    .Include(p => p.LichPhanCongNgoaiTrus).ThenInclude(p => p.NhanVien).ThenInclude(p => p.ChucDanh).ThenInclude(p => p.NhomChucDanh)
            //    .Where(p => p.IsDisabled != null && p.IsDisabled == false && p.KhoaPhongId == khoaPhongKhamId
            //                && p.LichPhanCongNgoaiTrus.Any(x => x.NgayPhanCong.Date == DateTime.Now.Date
            //                && ( (x.LoaiThoiGianPhanCong == Enums.EnumLoaiThoiGianPhanCong.Chieu && (checkTime == 1  || checkTime == 0) 
            //                            || (x.LoaiThoiGianPhanCong == Enums.EnumLoaiThoiGianPhanCong.Sang && checkTime == -1) ))
            //                //Phong kham co bac si
            //                && (x.NhanVien.ChucDanh.NhomChucDanh.Id == (long)Enums.EnumNhomChucDanh.BacSi) ))
            //    .Take(model.Take)
            //    .ToListAsync();

            if (lstKhoaPhongKhamId.Any())
            {
                foreach (var khoaPhongKhamId in lstKhoaPhongKhamId)
                {
                    var lst = await _phongBenhVienRepository.TableNoTracking
                        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.User)
                        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.ChucDanh)
                        .Where(p => p.IsDisabled != true && p.KhoaPhongId == khoaPhongKhamId
                                    && ((!p.HoatDongNhanViens.Any() && p.Ma.Contains(model.Query ?? "")) || p.HoatDongNhanViens.Any(x => x.NhanVien.User.HoTen.Contains(model.Query ?? ""))
                                    || p.Ma.Contains(model.Query ?? "") || p.Ten.Contains(model.Query ?? "")))
                        .Take(200)
                        .ToListAsync();
                    lstPhongKham.AddRange(lst);
                }
            }
            if (lstPhongBenhVienId.Any())
            {
                foreach (var phongBenhVienId in lstPhongBenhVienId)
                {
                    var lst = await _phongBenhVienRepository.TableNoTracking
                        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.User)
                        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.ChucDanh)
                        .Where(p => p.IsDisabled != true && p.Id == phongBenhVienId
                                    && ((!p.HoatDongNhanViens.Any() && p.Ma.Contains(model.Query ?? "")) || p.HoatDongNhanViens.Any(x => x.NhanVien.User.HoTen.Contains(model.Query ?? ""))
                                    || p.Ma.Contains(model.Query ?? "") || p.Ten.Contains(model.Query ?? "")))
                        .Take(200)
                        .ToListAsync();
                    lstPhongKham.AddRange(lst);
                }
            }


            var result = new List<PhongKhamTemplateVo>();
            foreach (var item in lstPhongKham)
            {
                if (item.HoatDongNhanViens.Any(p => p.NhanVien.ChucDanh != null && p.NhanVien.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi))
                {
                    foreach (var hoatDongNhanVien in item.HoatDongNhanViens)
                    {
                        var keyId = item.Id + "," + hoatDongNhanVien.NhanVienId;
                        if (hoatDongNhanVien.NhanVien.ChucDanh == null
                            || (hoatDongNhanVien.NhanVien.ChucDanh != null && hoatDongNhanVien.NhanVien.ChucDanh.NhomChucDanhId != (long)Enums.EnumNhomChucDanh.BacSi || result.Any(p => p.KeyId == keyId)))
                        {
                            continue;
                        }
                        //if (result.Any(p => p.KeyId == keyId))
                        //{
                        //    continue;
                        //}
                        //query
                        if (hoatDongNhanVien.NhanVien.User.HoTen.ToUpper().ConvertUnicodeString().ConvertToUnSign()
                                                .Contains(!string.IsNullOrEmpty(model.Query) ? model.Query.ToUpper().ConvertUnicodeString().ConvertToUnSign() : "")
                            || item.Ma.ToUpper().ConvertUnicodeString().ConvertToUnSign()
                                                .Contains(!string.IsNullOrEmpty(model.Query) ? model.Query.ToUpper().ConvertUnicodeString().ConvertToUnSign() : "")
                                                || item.Ten.ToUpper().ConvertUnicodeString().ConvertToUnSign()
                                                .Contains(!string.IsNullOrEmpty(model.Query) ? model.Query.ToUpper().ConvertUnicodeString().ConvertToUnSign() : ""))
                        {
                            //update 10/11/2022 bỏ phân luồng
                            ////Tram update: Tổng= tổng yêu cầu khám dc chỉ định hoặc thực hiện tại phòng này vào ngày hôm nay và có trạng thái khác Hủy
                            //var tong = _yeuCauKhamBenhRepository.TableNoTracking.Where(p => ((p.NoiThucHienId != null && p.NoiThucHienId == item.Id) ||
                            //                                                                 (p.NoiThucHienId == null && p.NoiDangKyId != null && p.NoiDangKyId == item.Id)) &&
                            //                                                                 p.ThoiDiemChiDinh.Date == DateTime.Now.Date && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
                            ////Tram update: Tổng= tổng yêu cầu khám dc chỉ định hoặc thực hiện của BS này vào ngày hôm nay và có trạng thái khác Hủy
                            //var tongYeuCauBacSiThucHien = _yeuCauKhamBenhRepository.TableNoTracking.Where(p => ((p.BacSiThucHienId != null && p.BacSiThucHienId == hoatDongNhanVien.NhanVienId) ||
                            //                                                                                    (p.BacSiThucHienId == null && p.BacSiDangKyId != null && p.BacSiDangKyId == hoatDongNhanVien.NhanVienId)) &&
                            //                                                                                    p.ThoiDiemChiDinh.Date == DateTime.Now.Date && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
                            var resultNeedAdd = new PhongKhamTemplateVo
                            {
                                BacSiId = hoatDongNhanVien.NhanVienId,
                                PhongKhamId = item.Id,
                                KeyId = keyId,
                                MaPhong = item.Ma,
                                TenBacSi = hoatDongNhanVien.NhanVien.User.HoTen,
                                //Tong = tong.Count(),
                                //DangKham = tong.Count(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham),
                                TongSoKhamGioiHan = tongSoNguoiKham,
                                //IsWarning = tongYeuCauBacSiThucHien.Count() >= tongSoNguoiKham,
                                DisplayName = item.Ten,
                                TenPhong = item.Ten
                            };
                            result.Add(resultNeedAdd);

                        }
                    }
                }
                else
                {
                    //update 10/11/2022 bỏ phân luồng
                    //Tram update: Tổng= tổng yêu cầu khám dc chỉ định hoặc thực hiện tại phòng này vào ngày hôm nay và có trạng thái khác Hủy
                    //var tong = _yeuCauKhamBenhRepository.TableNoTracking.Where(p => ((p.NoiThucHienId != null && p.NoiThucHienId == item.Id) ||
                    //                                                                 (p.NoiThucHienId == null && p.NoiDangKyId != null && p.NoiDangKyId == item.Id)) &&
                    //                                                                 p.ThoiDiemChiDinh.Date == DateTime.Now.Date && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
                    var keyId = item.Id + "," + 0;
                    var resultNeedAdd = new PhongKhamTemplateVo
                    {
                        BacSiId = 0,
                        PhongKhamId = item.Id,
                        KeyId = keyId,
                        MaPhong = item.Ma,
                        TenBacSi = "",
                        //Tong = tong.Count(),
                        //DangKham = tong.Count(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham),
                        TongSoKhamGioiHan = tongSoNguoiKham,
                        IsWarning = 0 >= tongSoNguoiKham,
                        DisplayName = item.Ten,
                        TenPhong = item.Ten
                    };
                    result.Add(resultNeedAdd);

                }

            }

            result = result.OrderBy(p => p.Tong).ToList();
            //var query = lstPhongKham.Select(item => new LookupItemTextVo
            //{
            //    DisplayName = item.Ten,
            //    KeyId = item.Id + "",
            //}).ToList();

            return result.Distinct().ToList();
        }

        public async Task<List<PhongKhamDVKTTemplateVo>> GetPhongKhamKyThuat(DropDownListRequestModel model, int tongSoNguoiKham, int loai)
        {
            var lstKhoaPhongKhamId = new List<long>();
            var lstPhongBenhVienId = new List<long>();
            var lstPhongKham = new List<Core.Domain.Entities.PhongBenhViens.PhongBenhVien>();

            if (loai == 1)
            {
                var dichVuKyThuatBenhVienId = CommonHelper.GetIdFromRequestDropDownList(model);
                //var dichVuKyThuatBenhVien =
                //    await _dichVuKyThuatBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                //        p.Id == dichVuKyThuatBenhVienId);
                //if (dichVuKyThuatBenhVien != null)
                //{
                //    khoaPhongKhamId = dichVuKyThuatBenhVien.KhoaPhongId ?? 0;
                //}
                var noiThucHien = _dichVuKyThuatBenhVienNoiThucHienRepository.TableNoTracking.Where(p => p.DichVuKyThuatBenhVienId == dichVuKyThuatBenhVienId).ToList();
                if (noiThucHien.Any())
                {
                    foreach (var item in noiThucHien)
                    {
                        if (item.KhoaPhongId != null)
                        {
                            lstKhoaPhongKhamId.Add(item.KhoaPhongId ?? 0);
                        }
                        if (item.PhongBenhVienId != null)
                        {
                            lstPhongBenhVienId.Add(item.PhongBenhVienId ?? 0);
                        }
                    }
                }
                else
                {
                    lstPhongBenhVienId.AddRange(_phongBenhVienRepository.TableNoTracking.Where(p => p.IsDisabled != true).Select(p => p.Id));
                }
            }
            else if (loai == 2)
            {
                var dichVuGiuongBenhVienId = CommonHelper.GetIdFromRequestDropDownList(model);
                //var dichVuGiuongBenhVien =
                //    await _dichVuGiuongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                //        p.Id == dichVuGiuongBenhVienId);
                //if (dichVuGiuongBenhVien != null)
                //{
                //    khoaPhongKhamId = dichVuGiuongBenhVien.KhoaPhongId ?? 0;
                //}
                var noiThucHien = await _dichVuGiuongBenhVienNoiThucHienRepository.TableNoTracking.Where(p => p.DichVuGiuongBenhVienId == dichVuGiuongBenhVienId).ToListAsync();
                if (noiThucHien.Any())
                {
                    foreach (var item in noiThucHien)
                    {
                        if (item.KhoaPhongId != null)
                        {
                            lstKhoaPhongKhamId.Add(item.KhoaPhongId ?? 0);
                        }
                        if (item.PhongBenhVienId != null)
                        {
                            lstPhongBenhVienId.Add(item.PhongBenhVienId ?? 0);
                        }
                    }
                }
                else
                {
                    lstPhongBenhVienId.AddRange(_phongBenhVienRepository.TableNoTracking.Where(p => p.IsDisabled != true).Select(p => p.Id));
                }
            }

            var phongKhamTheoDichVus = _phongBenhVienRepository.TableNoTracking
                //.Include(p => p.YeuCauDichVuKyThuatNoiThucHiens).Include(p => p.YeuCauDichVuGiuongBenhVienNoiThucHiens)
                .Where(p => p.IsDisabled != true
                            && ((!p.HoatDongNhanViens.Any() && (p.Ma.Contains(model.Query ?? "") || p.Ten.Contains(model.Query ?? ""))) || p.HoatDongNhanViens.Any(x => x.NhanVien.User.HoTen.Contains(model.Query ?? "")) || p.Ma.Contains(model.Query ?? "") || p.Ten.Contains(model.Query ?? ""))
                            && (lstKhoaPhongKhamId.Contains(p.KhoaPhongId) || lstPhongBenhVienId.Contains(p.Id)))
                .Take(model.Take).ToList();
            lstPhongKham.AddRange(phongKhamTheoDichVus);

            var lstPhongKhamId = lstPhongKham.Select(o => o.Id).ToList();

            //update 10/11/2022 bỏ phân luồng
            //var soDvDangThucHienTheoPhongs = new List<long>();
            //if (loai == 1)
            //{
            //    soDvDangThucHienTheoPhongs = _yeuCauDichVuKyThuatRepository.TableNoTracking
            //    .Where(p => lstPhongKhamId.Contains(p.NoiThucHienId.GetValueOrDefault()) && (p.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || p.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien))
            //    .Select(p => p.NoiThucHienId.GetValueOrDefault()).ToList();
            //}
            //else if (loai == 2)
            //{
            //    soDvDangThucHienTheoPhongs = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking
            //    .Where(p => lstPhongKhamId.Contains(p.NoiThucHienId.GetValueOrDefault()) && (p.TrangThai == Enums.EnumTrangThaiGiuongBenh.ChuaThucHien || p.TrangThai == Enums.EnumTrangThaiGiuongBenh.DangThucHien))
            //    .Select(p => p.NoiThucHienId.GetValueOrDefault()).ToList();
            //}

            var result = new List<PhongKhamDVKTTemplateVo>();

            result = lstPhongKham.Select(item => new PhongKhamDVKTTemplateVo()
            {
                DisplayName = item.Ten,
                TenPhong = item.Ten,
                MaPhong = item.Ma,
                KeyId = item.Id.ToString(),
                //Tong = soDvDangThucHienTheoPhongs.Count(o => o == item.Id)
                //Tong = loai == 1 ? item.YeuCauDichVuKyThuatNoiThucHiens
                //        .Where(p => p.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || p.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien).Count()
                //    : item.YeuCauDichVuGiuongBenhVienNoiThucHiens
                //        .Where(p => p.TrangThai == Enums.EnumTrangThaiGiuongBenh.ChuaThucHien || p.TrangThai == Enums.EnumTrangThaiGiuongBenh.DangThucHien).Count(),
            }).Distinct().ToList();

            result = result.OrderBy(p => p.Tong)
                .Take(model.Take).ToList();

            return result;
        }

        public async Task<List<PhongKhamTemplateVo>> SettPhongKham(long khoaPhongKhamId, int tongSoNguoiKham)
        {
            var lstPhongKham = await _phongBenhVienRepository.TableNoTracking
                .Include(p => p.LichPhanCongNgoaiTrus).ThenInclude(p => p.NhanVien).ThenInclude(p => p.User)
                .Include(p => p.LichPhanCongNgoaiTrus).ThenInclude(p => p.NhanVien).ThenInclude(p => p.ChucDanh).ThenInclude(p => p.NhomChucDanh)
                .Where(p => p.IsDisabled != true && p.KhoaPhongId == khoaPhongKhamId
                            && p.LichPhanCongNgoaiTrus.Any(x => x.NgayPhanCong.Date == DateTime.Now.Date
                            //Phong kham co bac si
                            && (x.NhanVien.ChucDanh.NhomChucDanh.Id == (long)Enums.EnumNhomChucDanh.BacSi || x.NhanVien.ChucDanh.NhomChucDanh.Id == (long)Enums.EnumNhomChucDanh.BacSiDuPhong)))
                //.Take(model.Take)
                .ToListAsync();

            var result = new List<PhongKhamTemplateVo>();
            foreach (var item in lstPhongKham)
            {
                foreach (var lichPhanCong in item.LichPhanCongNgoaiTrus)
                {
                    //query
                    //if (lichPhanCong.NhanVien.User.HoTen.Contains(model.Query ?? "") || item.Ma.Contains(model.Query ?? ""))
                    //{
                    //var tong = BaseRepository.TableNoTracking.Where(p => p.BacSiChiDinhId == lichPhanCong.NhanVienId
                    //                                                     && p.ThoiGianTiepNhan.Date == DateTime.Now.Date);
                    var resultNeedAdd = new PhongKhamTemplateVo
                    {
                        BacSiId = lichPhanCong.NhanVienId,
                        PhongKhamId = item.Id,
                        KeyId = item.Id + "," + lichPhanCong.NhanVienId,
                        MaPhong = item.Ma,
                        TenBacSi = lichPhanCong.NhanVien.User.HoTen,
                        //Tong = tong.Count(),
                        //DangKham = tong.Count(p => p.TrangThaiYeuCauKhamBenh == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham),
                        TongSoKhamGioiHan = tongSoNguoiKham,
                        //IsWarning = tong.Count() >= tongSoNguoiKham,
                        DisplayName = item.Ma + " - " + lichPhanCong.NhanVien.User.HoTen
                    };
                    result.Add(resultNeedAdd);
                    //}

                }
            }

            result = result.OrderBy(p => p.Tong).ToList();
            //var query = lstPhongKham.Select(item => new LookupItemTextVo
            //{
            //    DisplayName = item.Ten,
            //    KeyId = item.Id + "",
            //}).ToList();

            return result;
        }

        public async Task<List<LookupItemVo>> GetDoiTuongKhamChuaBenhUuTien(DropDownListRequestModel model)
        {
            var lstDoiTuong = await _doiTuongUuTienKhamChuaBenhRepository.TableNoTracking.Where(p => p.IsDisabled != true && p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();
            var query = lstDoiTuong.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetDanToc(DropDownListRequestModel model)
        {
            var lstDanToc = 
                await _danTocRepository.TableNoTracking
                    .Where(p => (model.Id != 0 && p.Id == model.Id) || (model.Id == 0 && p.IsDisabled != true && p.Ten.Contains(model.Query ?? "")))
                .Take(model.Take)
                .ToListAsync();
            var query = lstDanToc.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;

        }

        public async Task<List<LookupItemVo>> GetQuocTich(DropDownListRequestModel model)
        {
            //Cập nhật  01/07/2022: bổ sung where thêm id hiện tại đang chọn, fix bug item > 50
            var lstQuocTich = await _quocGiaRepository.TableNoTracking
                .Where(p => p.IsDisabled != true 
                            && (p.Id == model.Id || p.Ten.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Id)
                .Take(model.Take)
                .ToListAsync();
            var query = lstQuocTich.Select(item => new LookupItemVo
            {
                DisplayName = item.QuocTich,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<TinhThanhTemplateVo>> GetTinhThanh(DropDownListRequestModel model, long? quanHuyenId)
        {
            if (quanHuyenId != null && quanHuyenId != 0)
            {
                var donViHanhChinhTrucThuoc =
                    await _donViHanhChinhRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == quanHuyenId);
                if (donViHanhChinhTrucThuoc != null)
                {
                    var donViHanhChinhTrucThuocId = donViHanhChinhTrucThuoc.TrucThuocDonViHanhChinhId;
                    var lstTinhThanh = await _donViHanhChinhRepository.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.TinhThanhPho && p.Id == donViHanhChinhTrucThuocId
                                                                                                  && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
                        .Take(100)
                        .Select(p => new TinhThanhTemplateVo
                        {
                            DisplayName = p.Ten,
                            Ten = p.Ten,
                            Ma = p.Ma,
                            KeyId = p.Id,
                        })
                        .ToListAsync();

                    return lstTinhThanh;
                }
                else
                {
                    return new List<TinhThanhTemplateVo>();
                }
            }
            else
            {
                var lstTinhThanh = await _donViHanhChinhRepository.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.TinhThanhPho
                                                                                              && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
                    .Take(100)
                    .Select(p => new TinhThanhTemplateVo
                    {
                        DisplayName = p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        KeyId = p.Id,
                    })
                    .ToListAsync();

                return lstTinhThanh;
            }

        }

        public async Task<List<TinhThanhTemplateVo>> GetQuanHuyen(DropDownListRequestModel model, long? phuongXaId)
        {
            if (phuongXaId != 0 && phuongXaId != null)
            {
                var donViHanhChinhTrucThuoc =
                    await _donViHanhChinhRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == phuongXaId);
                if (donViHanhChinhTrucThuoc != null)
                {
                    var donViHanhChinhTrucThuocId = donViHanhChinhTrucThuoc.TrucThuocDonViHanhChinhId;

                    var lstQuanHuyen = await _donViHanhChinhRepository.TableNoTracking.Where(p =>
                            p.CapHanhChinh == Enums.CapHanhChinh.QuanHuyen && p.Id == donViHanhChinhTrucThuocId
                            && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") ||
                                p.TenVietTat.Contains(model.Query ?? "")))
                        .Take(200)
                        .Select(p => new TinhThanhTemplateVo
                        {
                            DisplayName = p.Ten,
                            Ten = p.Ten,
                            Ma = p.Ma,
                            KeyId = p.Id,
                        })
                        .ToListAsync();

                    return lstQuanHuyen;
                }
                else
                {
                    return new List<TinhThanhTemplateVo>();
                }
            }
            else
            {
                var subId = CommonHelper.GetIdFromRequestDropDownList(model);

                if (subId == 0) return new List<TinhThanhTemplateVo>();

                var lstQuanHuyen = await _donViHanhChinhRepository.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.QuanHuyen
                                                                                              && p.TrucThuocDonViHanhChinhId == subId
                                                                                              && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
                    .Take(200)
                    .Select(p => new TinhThanhTemplateVo
                    {
                        DisplayName = p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        KeyId = p.Id,
                    })
                    .ToListAsync();

                return lstQuanHuyen;
            }

        }

        public async Task<List<TinhThanhTemplateVo>> GetPhuongXa(DropDownListRequestModel model)
        {
            var subId = CommonHelper.GetIdFromRequestDropDownList(model);

            if (subId == 0)
            {
                var lst = await _donViHanhChinhRepository.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.PhuongXa
                                                                                             && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
                    .Take(200)
                    .Select(p => new TinhThanhTemplateVo
                    {
                        DisplayName = p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        KeyId = p.Id,
                    })
                    .ToListAsync();
                return lst;
            }

            var lstPhuongXa = await _donViHanhChinhRepository.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.PhuongXa
                                                                                         && p.TrucThuocDonViHanhChinhId == subId
                                                                                         && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
                .Take(200)
                .Select(p => new TinhThanhTemplateVo
                {
                    DisplayName = p.Ten,
                    Ten = p.Ten,
                    Ma = p.Ma,
                    KeyId = p.Id,
                })
                .ToListAsync();
            //var query = lstPhuongXa.Select(item => new LookupItemVo
            //{
            //    DisplayName = item.Ten,
            //    KeyId = item.Id,
            //}).ToList();

            return lstPhuongXa;
        }

        public async Task<List<LookupItemVo>> GetNgheNghiep(DropDownListRequestModel model)
        {
            var lstNgheNghiep = await _ngheNghiepRepository.TableNoTracking.Where(p => p.IsDisabled != true && p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();
            var query = lstNgheNghiep.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetQuanHe(DropDownListRequestModel model)
        {
            var lstNgheNghiep = await _quanHeThanNhanRepository.TableNoTracking.Where(p => p.IsDisabled != true && p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();
            var query = lstNgheNghiep.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetDoiTuongUuDai(DropDownListRequestModel model)
        {
            var lstDoiTuongUuDai = await _doiTuongUuDaiRepository.TableNoTracking.Where(p =>
                    p.IsDisabled != true && p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = lstDoiTuongUuDai.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetCongTyUuDai(DropDownListRequestModel model)
        {
            var lstEntity = await _congTyUuDaiRepository.TableNoTracking.Where(p =>
                    p.IsDisabled != true && p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public Task<List<LookupItemVo>> GetLyDoVaoVien(DropDownListRequestModel model)
        {
            var lstEnums = EnumHelper.GetListEnum<Enums.EnumYeuCauTiepNhan>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(model.Query))
            {
                lstEnums = lstEnums.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                                 .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            return Task.FromResult(lstEnums);

        }

        public async Task<List<LookupTreeItemVo>> GetLyDoTiepNhanTreeView(DropDownListRequestModel model)
        {
            var query = await _lyDoTiepNhanRepository.TableNoTracking
                .Select(s => new LookupTreeItemVo
                {
                    KeyId = s.Id,
                    DisplayName = s.Ten,
                    Level = s.CapNhom,
                    ParentId = s.LyDoTiepNhanChaId
                }).ToListAsync();

            var list = query.Select(s => new LookupTreeItemVo
            {
                KeyId = s.KeyId,
                DisplayName = s.DisplayName,
                Level = s.Level + 1,
                ParentId = s.ParentId,
                Items = GetChildrenTree(query, s.KeyId, s.Level, model.Query.RemoveDiacritics(), s.DisplayName)
            }).Where(x =>
                x.ParentId == null && (string.IsNullOrEmpty(model.Query) ||
                                       !string.IsNullOrEmpty(model.Query) && (x.Items.Any()
                                       || x.DisplayName.RemoveDiacritics().Trim().ToLower().Contains(model.Query.RemoveDiacritics().Trim().ToLower()))))
                .Take(model.Take)
                .ToList();

            return list;
        }

        public static List<LookupTreeItemVo> GetChildrenTree(List<LookupTreeItemVo> comments, long Id, long level, string queryString, string parentDisplay) //todo: cần xóa
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id && c.Level == level + 1)
                .Select(c => new LookupTreeItemVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Items = GetChildrenTree(comments, c.KeyId, c.Level, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString) || !string.IsNullOrEmpty(queryString) && (parentDisplay.RemoveDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.RemoveDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any()))
                .ToList();
            return query;
        }

        public async Task<List<LookupItemVo>> GetHinhThucDen(DropDownListRequestModel model)
        {
            var lstEntity = await _hinhThucDenRepository.TableNoTracking.Where(p =>
                    p.IsDisabled != true && p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public Task<List<LookupItemVo>> GetGioiTinh(DropDownListRequestModel model)
        {
            var lstEnums = EnumHelper.GetListEnum<Enums.LoaiGioiTinh>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(model.Query))
            {
                lstEnums = lstEnums.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                   .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            return Task.FromResult(lstEnums);
        }

        public async Task<List<LookupItemVo>> GetLoaiTaiLieuDinhKem(DropDownListRequestModel model)
        {
            var lstEntity = await _loaiHoSoYeuCauTiepNhanRepository.TableNoTracking
                .Take(model.Take)
                .ToListAsync();

            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<string> GetNameLoaiTaiLieuDinhKem(long id)
        {
            var entity = await _loaiHoSoYeuCauTiepNhanRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return entity?.Ten;
        }

        public async Task<List<MaDichVuTemplateVo>> NoiGioiThieu(DropDownListRequestModel model)
        {
            var noiGioiThieus = await _noiGioiThieuRepository.TableNoTracking.Where(p =>
                    p.IsDisabled != true)
                .Select(p => new MaDichVuTemplateVo
                {
                    DisplayName = p.DonVi != null ? p.Ten + " - " + p.DonVi : p.Ten,
                    Ten = p.Ten,
                    Ma = p.DonVi,
                    KeyId = p.Id
                })
                .ApplyLike(model.Query, s => s.Ten, s => s.Ma, s => s.DisplayName)
                .Take(model.Take)
                .ToListAsync();
            if (model.Id > 0 && !noiGioiThieus.Any(o => o.KeyId == model.Id))
            {
                var item = _noiGioiThieuRepository.TableNoTracking.Where(p =>
                       p.Id == model.Id)
                .Select(p => new MaDichVuTemplateVo
                {
                    DisplayName = p.DonVi != null ? p.Ten + " - " + p.DonVi : p.Ten,
                    Ten = p.Ten,
                    Ma = p.DonVi,
                    KeyId = p.Id
                }).FirstOrDefault();
                if (item != null)
                {
                    noiGioiThieus.Add(item); 
                }
            }
            return noiGioiThieus;
        }

        public async Task<List<LookupItemVo>> GetGoiKham(DropDownListRequestModel model)
        {
            //update goi dv 10/21
            var lstEntity = await _goiDichVuRepository.TableNoTracking.Where(p => //!p.CoChietKhau && p.NgayBatDau.Date <= DateTime.Now.Date && (p.NgayKetThuc == null || p.NgayKetThuc.GetValueOrDefault() >= DateTime.Now.Date) && p.IsDisabled != true && 
            p.IsDisabled != true && p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetGoiKhamCoChietKhau(DropDownListRequestModel model)
        {
            //update goi dv 10/21
            var lstEntity = await _goiDichVuRepository.TableNoTracking.Where(p => //p.CoChietKhau && p.NgayBatDau.Date <= DateTime.Now.Date && (p.NgayKetThuc == null || p.NgayKetThuc.GetValueOrDefault() >= DateTime.Now.Date) && p.IsDisabled != true && 
                p.IsDisabled != true && p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<MaDichVuTemplateVo>> GetDichVuKhamBenh(DropDownListRequestModel model)
        {
            var lstEntity = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh)
                .Where(p => p.HieuLuc && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")))
                .Take(model.Take)
                .Select(p => new MaDichVuTemplateVo
                {
                    DisplayName = p.Ten,
                    Ten = p.Ten,
                    Ma = p.Ma,
                    KeyId = p.Id,
                })
                .ToListAsync();

            return lstEntity;
        }

        public async Task<List<MaDichVuTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model)
        {
            var lstEntity = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Include(p => p.DichVuKyThuat)
                .Where(p => p.HieuLuc && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")))
                .Take(model.Take)
                .Select(p => new MaDichVuTemplateVo
                {
                    DisplayName = p.Ma + " - " + p.Ten,
                    Ten = p.Ten,
                    Ma = p.Ma,
                    KeyId = p.Id,
                })
                .ToListAsync();

            return lstEntity;
        }

        public async Task<List<MaDichVuTemplateVo>> GetDichVuGiuongBenh(DropDownListRequestModel model)
        {
            var lstEntity = await _dichVuGiuongBenhVienRepository.TableNoTracking.Include(p => p.DichVuGiuong)
                .Where(p => p.HieuLuc && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")))
                .Take(model.Take)
                .Select(p => new MaDichVuTemplateVo
                {
                    DisplayName = p.Ma + " - " + p.Ten,
                    Ten = p.Ten,
                    Ma = p.Ma,
                    KeyId = p.Id,
                })
                .ToListAsync();

            return lstEntity;
        }

        public async Task<List<LookupItemVo>> GetLoaiGiaKhamBenh(DropDownListRequestModel model)
        {
            var lstEntity = await _nhomGiaDichVuKhamBenhRepository.TableNoTracking.Where(p => p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<BenhNhan> GetBenhNhan(string maSoTheBHYT)
        {
            return await _benhNhanRepository.TableNoTracking.Where(p => p.BHYTMaSoThe.Equals(maSoTheBHYT))
                .LastOrDefaultAsync();
        }



        public async Task<bool> CheckBenhNhanTNBNExists(string maSoTheBHYT)
        {
            var benhNhan = await _benhNhanRepository.TableNoTracking.Include(p => p.YeuCauTiepNhans).Where(p => p.BHYTMaSoThe.Equals(maSoTheBHYT))
                .LastOrDefaultAsync();
            return benhNhan?.YeuCauTiepNhans.Any(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien) ?? false;
        }

        public Task<BenhNhan> CreateNewBenhNhan(YeuCauTiepNhan model)
        {
            var benhNhan = new BenhNhan
            {
                BHYTMaDKBD = model.BHYTMaDKBD,
                BHYTDiaChi = model.BHYTDiaChi,
                BHYTMaSoThe = model.BHYTMaSoThe,
                BHYTNgayDu5Nam = model.BHYTNgayDu5Nam,
                BHYTDuocMienCungChiTra = model.BHYTDuocMienCungChiTra,
                BHYTNgayHieuLuc = model.BHYTNgayHieuLuc,
                BHYTNgayHetHan = model.BHYTNgayHetHan,
                BHYTCoQuanBHXH = model.BHYTCoQuanBHXH,
                CoBHTN = model.CoBHTN,
                CoBHYT = model.CoBHYT,
                BHYTgiayMienCungChiTra = model.BHYTDuocMienCungChiTra == true ? model.BHYTGiayMienCungChiTra : null,
                HoTen = model.HoTen,
                NgaySinh = model.NgaySinh,
                NamSinh = model.NamSinh,
                NguoiLienHeHoTen = model.NguoiLienHeHoTen,
                NguoiLienHeEmail = model.NguoiLienHeEmail,
                NguoiLienHeDiaChi = model.NguoiLienHeDiaChi,
                NguoiLienHePhuongXaId = model.NguoiLienHePhuongXaId,
                NguoiLienHeQuanHeNhanThanId = model.NguoiLienHeQuanHeNhanThanId,
                NguoiLienHeQuanHuyenId = model.NguoiLienHeQuanHuyenId,
                NguoiLienHeSoDienThoai = model.NguoiLienHeSoDienThoai,
                NguoiLienHeTinhThanhId = model.NguoiLienHeTinhThanhId,
                PhuongXaId = model.PhuongXaId,
                TinhThanhId = model.TinhThanhId,
                QuanHuyenId = model.QuanHuyenId,
                QuocTichId = model.QuocTichId,
                DanTocId = model.DanTocId,
                DiaChi = model.DiaChi,
                SoDienThoai = model.SoDienThoai,
                SoChungMinhThu = model.SoChungMinhThu,
                Email = model.Email,
                NgheNghiepId = model.NgheNghiepId,
                GioiTinh = model.GioiTinh,
                BHYTMaKhuVuc = model.BHYTMaKhuVuc,
                BHYTNgayDuocMienCungChiTra = model.BHYTNgayDuocMienCungChiTra,
                NoiLamViec = model.NoiLamViec,
                ThangSinh = model.ThangSinh,
                NhomMau = model.NhomMau,
            };

            if (model.CoBHTN == true)
            {
                foreach (var bhtn in model.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                {
                    var benhNhanBHTN = new BenhNhanCongTyBaoHiemTuNhan
                    {
                        CongTyBaoHiemTuNhanId = bhtn.CongTyBaoHiemTuNhanId,
                        DiaChi = bhtn.DiaChi,
                        MaSoThe = bhtn.MaSoThe,
                        NgayHetHan = bhtn.NgayHetHan,
                        NgayHieuLuc = bhtn.NgayHieuLuc,
                        SoDienThoai = bhtn.SoDienThoai,
                    };
                    benhNhan.BenhNhanCongTyBaoHiemTuNhans.Add(benhNhanBHTN);
                }
            }

            return Task.FromResult(benhNhan);
        }
        public async Task UpdateBenhNhanForUpdateView(long id, YeuCauTiepNhan model)
        {
            var benhNhan = await _benhNhanRepository.Table.Include(p => p.BenhNhanCongTyBaoHiemTuNhans).FirstOrDefaultAsync(p => p.Id == id);
            if (benhNhan != null)
            {
                // chỉ user có quyền mới đc cập nhật thông tin hành chính người bệnh
                if (_roleService.IsHavePermissionForUpdateInformationTNBN())
                {
                    benhNhan.HoTen = model.HoTen;
                    benhNhan.NgaySinh = model.NgaySinh;
                    benhNhan.NamSinh = model.NamSinh;
                    benhNhan.PhuongXaId = model.PhuongXaId;
                    benhNhan.TinhThanhId = model.TinhThanhId;
                    benhNhan.QuanHuyenId = model.QuanHuyenId;
                    benhNhan.QuocTichId = model.QuocTichId;
                    benhNhan.DanTocId = model.DanTocId;
                    benhNhan.DiaChi = model.DiaChi;
                    benhNhan.SoDienThoai = model.SoDienThoai;
                    benhNhan.SoChungMinhThu = model.SoChungMinhThu;
                    benhNhan.Email = model.Email;
                    benhNhan.NgheNghiepId = model.NgheNghiepId;
                    benhNhan.GioiTinh = model.GioiTinh;
                    benhNhan.NoiLamViec = model.NoiLamViec;
                    benhNhan.ThangSinh = model.ThangSinh;

                    if (!string.IsNullOrEmpty(model.NguoiLienHeHoTen)
                        || (model.NguoiLienHeQuanHeNhanThanId != 0 && model.NguoiLienHeQuanHeNhanThanId != null)
                        || !string.IsNullOrEmpty(model.NguoiLienHeSoDienThoai)
                        || !string.IsNullOrEmpty(model.NguoiLienHeEmail)
                        || (model.NguoiLienHeTinhThanhId != 0 && model.NguoiLienHeTinhThanhId != null)
                        || (model.NguoiLienHeQuanHuyenId != 0 && model.NguoiLienHeQuanHuyenId != null)
                        || (model.NguoiLienHePhuongXaId != 0 && model.NguoiLienHePhuongXaId != null)
                        || !string.IsNullOrEmpty(model.NguoiLienHeDiaChi)
                    )
                    {
                        benhNhan.NguoiLienHeHoTen = model.NguoiLienHeHoTen;
                        benhNhan.NguoiLienHeQuanHeNhanThanId = model.NguoiLienHeQuanHeNhanThanId;
                        benhNhan.NguoiLienHeSoDienThoai = model.NguoiLienHeSoDienThoai;
                        benhNhan.NguoiLienHeEmail = model.NguoiLienHeEmail;
                        benhNhan.NguoiLienHeTinhThanhId = model.NguoiLienHeTinhThanhId;
                        benhNhan.NguoiLienHeQuanHuyenId = model.NguoiLienHeQuanHuyenId;
                        benhNhan.NguoiLienHePhuongXaId = model.NguoiLienHePhuongXaId;
                        benhNhan.NguoiLienHeDiaChi = model.NguoiLienHeDiaChi;
                    }
                }

                //if (benhNhan.CoBHYT == false && model.CoBHYT == true) <= code vô trách nhiệm
                if (benhNhan.CoBHYT != true && model.CoBHYT == true)
                {
                    benhNhan.CoBHYT = model.CoBHYT;

                    benhNhan.BHYTMaDKBD = model.BHYTMaDKBD;
                    benhNhan.BHYTDiaChi = model.BHYTDiaChi;
                    benhNhan.BHYTMaSoThe = model.BHYTMaSoThe;
                    benhNhan.BHYTNgayDu5Nam = model.BHYTNgayDu5Nam;
                    benhNhan.BHYTDuocMienCungChiTra = model.BHYTDuocMienCungChiTra;
                    benhNhan.BHYTNgayHieuLuc = model.BHYTNgayHieuLuc;
                    benhNhan.BHYTNgayHetHan = model.BHYTNgayHetHan;
                    benhNhan.BHYTCoQuanBHXH = model.BHYTCoQuanBHXH;
                    benhNhan.BHYTMaKhuVuc = model.BHYTMaKhuVuc;
                    benhNhan.BHYTNgayDuocMienCungChiTra = model.BHYTNgayDuocMienCungChiTra;
                }

                //update bao hiem tu nhan
                if (model.CoBHTN == true)
                {
                    foreach (var bhtn in benhNhan.BenhNhanCongTyBaoHiemTuNhans)
                    {
                        if (!model.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any(p => p.CongTyBaoHiemTuNhanId == bhtn.CongTyBaoHiemTuNhanId))
                        {
                            bhtn.WillDelete = true;
                        }
                    }
                    foreach (var bhtn in model.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                    {
                        if (!benhNhan.BenhNhanCongTyBaoHiemTuNhans.Any(p => p.CongTyBaoHiemTuNhanId == bhtn.CongTyBaoHiemTuNhanId))
                        {
                            var benhNhanBHTN = new BenhNhanCongTyBaoHiemTuNhan
                            {
                                CongTyBaoHiemTuNhanId = bhtn.CongTyBaoHiemTuNhanId,
                                DiaChi = bhtn.DiaChi,
                                MaSoThe = bhtn.MaSoThe,
                                NgayHetHan = bhtn.NgayHetHan,
                                NgayHieuLuc = bhtn.NgayHieuLuc,
                                SoDienThoai = bhtn.SoDienThoai,
                            };
                            benhNhan.BenhNhanCongTyBaoHiemTuNhans.Add(benhNhanBHTN);
                        }
                        else
                        {
                            if (benhNhan.BenhNhanCongTyBaoHiemTuNhans.Any())
                            {
                                var result = benhNhan.BenhNhanCongTyBaoHiemTuNhans.Single(c => c.CongTyBaoHiemTuNhanId == bhtn.CongTyBaoHiemTuNhanId);

                                result.BenhNhanId = result.BenhNhanId;
                                result.CongTyBaoHiemTuNhanId = bhtn.CongTyBaoHiemTuNhanId;
                                result.DiaChi = bhtn.DiaChi;
                                result.MaSoThe = bhtn.MaSoThe;
                                result.NgayHetHan = bhtn.NgayHetHan;
                                result.NgayHieuLuc = bhtn.NgayHieuLuc;
                                result.SoDienThoai = bhtn.SoDienThoai;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var bhtn in benhNhan.BenhNhanCongTyBaoHiemTuNhans)
                    {
                        bhtn.WillDelete = true;
                    }
                }

                await _benhNhanRepository.UpdateAsync(benhNhan);
            }
        }

        public async Task UpdateBenhNhan(long id, YeuCauTiepNhan model)
        {
            var benhNhan = await _benhNhanRepository.Table.Include(p => p.BenhNhanCongTyBaoHiemTuNhans).FirstOrDefaultAsync(p => p.Id == id);
            if (benhNhan != null)
            {
                benhNhan.HoTen = model.HoTen;
                benhNhan.NgaySinh = model.NgaySinh;
                benhNhan.NamSinh = model.NamSinh;
                benhNhan.PhuongXaId = model.PhuongXaId;
                benhNhan.TinhThanhId = model.TinhThanhId;
                benhNhan.QuanHuyenId = model.QuanHuyenId;
                benhNhan.QuocTichId = model.QuocTichId;
                benhNhan.DanTocId = model.DanTocId;
                benhNhan.DiaChi = model.DiaChi;
                benhNhan.SoDienThoai = model.SoDienThoai;
                benhNhan.SoChungMinhThu = model.SoChungMinhThu;
                benhNhan.Email = model.Email;
                benhNhan.NgheNghiepId = model.NgheNghiepId;
                benhNhan.GioiTinh = model.GioiTinh;
                benhNhan.NoiLamViec = model.NoiLamViec;
                benhNhan.ThangSinh = model.ThangSinh;

                if (!string.IsNullOrEmpty(model.NguoiLienHeHoTen)
                    || (model.NguoiLienHeQuanHeNhanThanId != 0 && model.NguoiLienHeQuanHeNhanThanId != null)
                    || !string.IsNullOrEmpty(model.NguoiLienHeSoDienThoai)
                    || !string.IsNullOrEmpty(model.NguoiLienHeEmail)
                    || (model.NguoiLienHeTinhThanhId != 0 && model.NguoiLienHeTinhThanhId != null)
                    || (model.NguoiLienHeQuanHuyenId != 0 && model.NguoiLienHeQuanHuyenId != null)
                    || (model.NguoiLienHePhuongXaId != 0 && model.NguoiLienHePhuongXaId != null)
                    || !string.IsNullOrEmpty(model.NguoiLienHeDiaChi)
                )
                {
                    benhNhan.NguoiLienHeHoTen = model.NguoiLienHeHoTen;
                    benhNhan.NguoiLienHeQuanHeNhanThanId = model.NguoiLienHeQuanHeNhanThanId;
                    benhNhan.NguoiLienHeSoDienThoai = model.NguoiLienHeSoDienThoai;
                    benhNhan.NguoiLienHeEmail = model.NguoiLienHeEmail;
                    benhNhan.NguoiLienHeTinhThanhId = model.NguoiLienHeTinhThanhId;
                    benhNhan.NguoiLienHeQuanHuyenId = model.NguoiLienHeQuanHuyenId;
                    benhNhan.NguoiLienHePhuongXaId = model.NguoiLienHePhuongXaId;
                    benhNhan.NguoiLienHeDiaChi = model.NguoiLienHeDiaChi;
                }

                //update bao hiem tu nhan
                if (model.CoBHTN == true)
                {
                    foreach (var bhtn in benhNhan.BenhNhanCongTyBaoHiemTuNhans)
                    {
                        if (!model.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any(p => p.CongTyBaoHiemTuNhanId == bhtn.CongTyBaoHiemTuNhanId))
                        {
                            bhtn.WillDelete = true;
                        }
                    }
                    foreach (var bhtn in model.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                    {
                        if (!benhNhan.BenhNhanCongTyBaoHiemTuNhans.Any(p => p.CongTyBaoHiemTuNhanId == bhtn.CongTyBaoHiemTuNhanId))
                        {
                            var benhNhanBHTN = new BenhNhanCongTyBaoHiemTuNhan
                            {
                                CongTyBaoHiemTuNhanId = bhtn.CongTyBaoHiemTuNhanId,
                                DiaChi = bhtn.DiaChi,
                                MaSoThe = bhtn.MaSoThe,
                                NgayHetHan = bhtn.NgayHetHan,
                                NgayHieuLuc = bhtn.NgayHieuLuc,
                                SoDienThoai = bhtn.SoDienThoai,
                            };
                            benhNhan.BenhNhanCongTyBaoHiemTuNhans.Add(benhNhanBHTN);
                        }
                        else
                        {
                            if (benhNhan.BenhNhanCongTyBaoHiemTuNhans.Any())
                            {
                                var result = benhNhan.BenhNhanCongTyBaoHiemTuNhans.Single(c => c.CongTyBaoHiemTuNhanId == bhtn.CongTyBaoHiemTuNhanId);

                                result.BenhNhanId = result.BenhNhanId;
                                result.CongTyBaoHiemTuNhanId = bhtn.CongTyBaoHiemTuNhanId;
                                result.DiaChi = bhtn.DiaChi;
                                result.MaSoThe = bhtn.MaSoThe;
                                result.NgayHetHan = bhtn.NgayHetHan;
                                result.NgayHieuLuc = bhtn.NgayHieuLuc;
                                result.SoDienThoai = bhtn.SoDienThoai;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var bhtn in benhNhan.BenhNhanCongTyBaoHiemTuNhans)
                    {
                        bhtn.WillDelete = true;
                    }
                }

                await _benhNhanRepository.UpdateAsync(benhNhan);
            }
        }

        public Task<BenhNhan> CreateNewBenhNhanKhongBHYT(YeuCauTiepNhan model)
        {
            var benhNhan = new BenhNhan
            {
                BHYTMaDKBD = model.BHYTMaDKBD,
                BHYTDiaChi = model.BHYTDiaChi,
                BHYTMaSoThe = model.BHYTMaSoThe,
                BHYTNgayDu5Nam = model.BHYTNgayDu5Nam,
                BHYTDuocMienCungChiTra = model.BHYTDuocMienCungChiTra,
                BHYTNgayHieuLuc = model.BHYTNgayHieuLuc,
                BHYTNgayHetHan = model.BHYTNgayHetHan,
                BHYTCoQuanBHXH = model.BHYTCoQuanBHXH,
                CoBHTN = model.CoBHTN,
                CoBHYT = model.CoBHYT,
                BHYTgiayMienCungChiTra = model.BHYTDuocMienCungChiTra == true ? model.BHYTGiayMienCungChiTra : null,
                HoTen = model.HoTen,
                NgaySinh = model.NgaySinh,
                NamSinh = model.NamSinh,
                NguoiLienHeHoTen = model.NguoiLienHeHoTen,
                NguoiLienHeEmail = model.NguoiLienHeEmail,
                NguoiLienHeDiaChi = model.NguoiLienHeDiaChi,
                NguoiLienHePhuongXaId = model.NguoiLienHePhuongXaId,
                NguoiLienHeQuanHeNhanThanId = model.NguoiLienHeQuanHeNhanThanId,
                NguoiLienHeQuanHuyenId = model.NguoiLienHeQuanHuyenId,
                NguoiLienHeSoDienThoai = model.NguoiLienHeSoDienThoai,
                NguoiLienHeTinhThanhId = model.NguoiLienHeTinhThanhId,
                PhuongXaId = model.PhuongXaId,
                TinhThanhId = model.TinhThanhId,
                QuanHuyenId = model.QuanHuyenId,
                QuocTichId = model.QuocTichId,
                DanTocId = model.DanTocId,
                DiaChi = model.DiaChi,
                SoDienThoai = model.SoDienThoai,
                SoChungMinhThu = model.SoChungMinhThu,
                Email = model.Email,
                NgheNghiepId = model.NgheNghiepId,
                GioiTinh = model.GioiTinh,
                BHYTMaKhuVuc = model.BHYTMaKhuVuc,
                BHYTNgayDuocMienCungChiTra = model.BHYTNgayDuocMienCungChiTra,
                NoiLamViec = model.NoiLamViec,
                ThangSinh = model.ThangSinh,
                NhomMau = model.NhomMau,
            };

            if (model.CoBHTN == true)
            {
                foreach (var bhtn in model.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                {
                    var benhNhanBHTN = new BenhNhanCongTyBaoHiemTuNhan
                    {
                        CongTyBaoHiemTuNhanId = bhtn.CongTyBaoHiemTuNhanId,
                        DiaChi = bhtn.DiaChi,
                        MaSoThe = bhtn.MaSoThe,
                        NgayHetHan = bhtn.NgayHetHan,
                        NgayHieuLuc = bhtn.NgayHieuLuc,
                        SoDienThoai = bhtn.SoDienThoai,
                    };
                    benhNhan.BenhNhanCongTyBaoHiemTuNhans.Add(benhNhanBHTN);
                }
            }

            return Task.FromResult(benhNhan);
        }

        public async Task<List<string>> GetListTenTrieuChungKhamAsync()
        {
            var lstTenNuocSanXuat =
                await _trieuChungRepository.TableNoTracking
                    .Where(x => !string.IsNullOrEmpty(x.Ten.Trim()))
                    .Select(x => x.Ten).ToListAsync();
            return lstTenNuocSanXuat;
        }

        public async Task<YeuCauTiepNhan> GetDataForDichVuKyThuatOrGiuongForUpdateViewGridAsync(ThemDichVuKhamBenhVo model, int loai)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var locationId = _userAgentHelper.GetCurrentNoiLLamViecId();


            long phongBenhVienId = 0;
            long nhanVienId = 0;
            if (model.NoiThucHienId.IndexOf(",") == -1)
            {
                phongBenhVienId = long.Parse(model.NoiThucHienId);
            }
            else
            {
                var lstNoiThucHienId = model.NoiThucHienId.Split(",");
                phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                nhanVienId = long.Parse(lstNoiThucHienId[1]);
            }

            //decimal bhytThanhToan = 0;

            if (loai == 1)
            {
                var dichVuKyThuatBenhVien = _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Include(p => p.DichVuKyThuat)
                    .FirstOrDefault(p => p.Id == model.MaDichVuId);

                var dichVuKyThuatBenhVienGiaBaoHiem =
                     _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == model.MaDichVuId
                    && p.TuNgay.Date <= DateTime.Now.Date
                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                //bhytThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0) *
                //                (dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong ?? 0) * (model.BHYTMucHuong ?? 100) / 100;

                //if (!model.DuocHuongBHYT)
                //{
                //    bhytThanhToan = 0;
                //}

                #region cập nhật 23/12/2022
                //var entity = await GetByIdHaveInclude(model.YeuCauTiepNhanId ?? 0);
                var entity = await GetByIdHaveIncludeForAdddichVu(model.YeuCauTiepNhanId ?? 0);
                #endregion

                if (entity != null)
                {
                    //add new dich vu ky thuat into yeucautiepnhan
                    var yeuCau = new YeuCauDichVuKyThuat
                    {
                        MaDichVu = dichVuKyThuatBenhVien.Ma,
                        Ma4350DichVu = dichVuKyThuatBenhVien.DichVuKyThuat?.Ma4350,
                        MaGiaDichVu = dichVuKyThuatBenhVien.DichVuKyThuat?.MaGia,
                        TenDichVu = dichVuKyThuatBenhVien.Ten,
                        DuocHuongBaoHiem = model.DuocHuongBHYT,
                        BaoHiemChiTra = null,
                        Gia = (decimal)model.DonGia,
                        //GiaBaoHiemThanhToan = bhytThanhToan,
                        //TiLeUuDai = Convert.ToInt32(item.TLMG),
                        NoiChiDinhId = locationId,
                        NoiThucHienId = phongBenhVienId,
                        //NhanVienThucHienId = nhanVienId,
                        //BacSiThucHienId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                        ThoiDiemChiDinh = DateTime.Now,
                        SoLan = model.SoLuong ?? 1,
                        ThoiDiemDangKy = DateTime.Now,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,


                        DichVuKyThuatBenhVienId = dichVuKyThuatBenhVien.Id,
                        NhomGiaDichVuKyThuatBenhVienId = model.LoaiGiaId ?? 0,

                        NhomChiPhi = dichVuKyThuatBenhVien.DichVuKyThuat?.NhomChiPhi ?? Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                        NhomDichVuBenhVienId = dichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                        LoaiDichVuKyThuat = GetLoaiDichVuKyThuat(dichVuKyThuatBenhVien.NhomDichVuBenhVienId),
                        //
                        DonGiaBaoHiem = dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0,
                        TiLeBaoHiemThanhToan = dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,
                        //
                    };

                    entity.YeuCauDichVuKyThuats.Add(yeuCau);

                    //set lai gia cho dich vu
                    //var lstDichVuOld = new List<DichVuOld>();
                    //lstDichVuOld = GetListDichVuOld(entity);

                    //Set default BHYT thanh toan and set new value
                    //entity = await SetValueDefaultAndSetValueBHYTWithNewLogic(entity, model.BHYTMucHuong ?? 0);

                    //Set trang thai thanh toan cho thu ngan
                    //entity = SetTrangThaiThanhToanForThuNganAgain(entity, lstDichVuOld);

                    return entity;
                }
            }
            else
            {
                var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .Include(p => p.DichVuGiuong)
                    .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);

                var dichVuGiuongBenhVienGiaBaoHiem =
                await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuGiuongBenhVienId == model.MaDichVuId
                    && p.TuNgay.Date <= DateTime.Now.Date
                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                //bhytThanhToan = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
                //                (dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong ?? 0) * (model.BHYTMucHuong ?? 100) / 100;

                //if (!model.DuocHuongBHYT)
                //{
                //    bhytThanhToan = 0;
                //}

                var entity = await GetByIdHaveInclude(model.YeuCauTiepNhanId ?? 0);
                if (entity != null)
                {
                    //add new dich vu giuong into yeucautiepnhan
                    var yeuCau = new YeuCauDichVuGiuongBenhVien
                    {
                        Ma = dichVuGiuongBenhVien.Ma,
                        MaTT37 = dichVuGiuongBenhVien.DichVuGiuong?.MaTT37,
                        Ten = dichVuGiuongBenhVien.Ten,
                        DuocHuongBaoHiem = model.DuocHuongBHYT,
                        BaoHiemChiTra = null,
                        Gia = (decimal)model.DonGia,
                        //GiaBaoHiemThanhToan = bhytThanhToan,
                        NoiChiDinhId = locationId,
                        NoiThucHienId = phongBenhVienId,
                        //nhanvien = nhanVienId,
                        //BacSiThucHienId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiGiuongBenh.ChuaThucHien,
                        ThoiDiemChiDinh = DateTime.Now,
                        LoaiGiuong = dichVuGiuongBenhVien.LoaiGiuong,
                        MaGiuong = dichVuGiuongBenhVien.Ma,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                        NhomGiaDichVuGiuongBenhVienId = model.LoaiGiaId,
                        DichVuGiuongBenhVienId = dichVuGiuongBenhVien.Id,
                        //
                        DonGiaBaoHiem = dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0,
                        TiLeBaoHiemThanhToan = dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,
                        //
                    };
                    entity.YeuCauDichVuGiuongBenhViens.Add(yeuCau);

                    //set lai gia cho dich vu
                    //var lstDichVuOld = new List<DichVuOld>();
                    //lstDichVuOld = GetListDichVuOld(entity);

                    //Set default BHYT thanh toan and set new value
                    //entity = await SetValueDefaultAndSetValueBHYTWithNewLogic(entity, model.BHYTMucHuong ?? 0);

                    //Set trang thai thanh toan cho thu ngan
                    //entity = SetTrangThaiThanhToanForThuNganAgain(entity, lstDichVuOld);

                    return entity;
                }
            }

            return null;
        }

        public async Task<YeuCauTiepNhan> NoiThucHienChange(long id, string nhom, long yeuCauTiepNhanId, int? mucHuong, string noiThucHienId)
        {
            #region Cập nhật 21/12/2022
            //var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);
            var entity = await GetByIdHaveIncludeForAdddichVu(yeuCauTiepNhanId);
            #endregion

            long phongBenhVienId = 0;
            long nhanVienId = 0;
            if (noiThucHienId.IndexOf(",") == -1)
            {
                phongBenhVienId = long.Parse(noiThucHienId);
            }
            else
            {
                var lstNoiThucHienId = noiThucHienId.Split(",");
                phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                nhanVienId = long.Parse(lstNoiThucHienId[1]);
            }

            //change noi thuc hien
            if (nhom == Constants.NhomDichVu.DichVuKhamBenh)
            {

                var item = entity.YeuCauKhamBenhs.FirstOrDefault(p => p.Id == id);
                if (item != null)
                {
                    item.NoiDangKyId = phongBenhVienId;
                    if (nhanVienId != 0)
                    {
                        item.BacSiDangKyId = nhanVienId;
                    }
                    else
                    {
                        item.BacSiDangKyId = null;
                    }
                }
            }
            else if (nhom == Constants.NhomDichVu.DichVuKyThuat)
            {
                var item = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == id);
                if (item != null)
                {
                    item.NoiThucHienId = phongBenhVienId;
                }
            }
            else if (nhom == Constants.NhomDichVu.DichVuGiuong)
            {
                var item = entity.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.Id == id);
                if (item != null)
                {
                    item.NoiThucHienId = phongBenhVienId;
                }
            }

            //set lai gia cho dich vu
            //var lstDichVuOld = new List<DichVuOld>();
            //lstDichVuOld = GetListDichVuOld(entity);

            //Set default BHYT thanh toan and set new value
            //entity = await SetValueDefaultAndSetValueBHYTWithNewLogic(entity, mucHuong ?? 0);

            //Set trang thai thanh toan cho thu ngan
            //entity = SetTrangThaiThanhToanForThuNganAgain(entity, lstDichVuOld);

            return entity;
        }

        public async Task<YeuCauTiepNhan> RemoveDichVuCoChietKhau(long goiCoChietKhauId, string nhom, long yeuCauTiepNhanId, int? mucHuong)
        {
            var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);
            //TODO: need update goi dv
            //var goiChietKhau = entity.YeuCauGoiDichVus.FirstOrDefault(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId && p.GoiDichVuId == goiCoChietKhauId);
            //if (goiChietKhau != null)
            //{
            //    goiChietKhau.WillDelete = true;
            //    if (goiChietKhau.YeuCauKhamBenhs.Any())
            //    {
            //        foreach (var item in goiChietKhau.YeuCauKhamBenhs)
            //        {
            //            item.WillDelete = true;
            //        }
            //    }
            //    if (goiChietKhau.YeuCauDichVuKyThuats.Any())
            //    {
            //        foreach (var item in goiChietKhau.YeuCauDichVuKyThuats)
            //        {
            //            item.WillDelete = true;
            //        }
            //    }
            //    if (goiChietKhau.YeuCauDichVuGiuongBenhViens.Any())
            //    {
            //        foreach (var item in goiChietKhau.YeuCauDichVuGiuongBenhViens)
            //        {
            //            item.WillDelete = true;
            //        }
            //    }
            //    if (goiChietKhau.YeuCauDuocPhamBenhViens.Any())
            //    {
            //        foreach (var item in goiChietKhau.YeuCauDuocPhamBenhViens)
            //        {
            //            item.WillDelete = true;
            //        }
            //    }
            //    if (goiChietKhau.YeuCauVatTuBenhViens.Any())
            //    {
            //        foreach (var item in goiChietKhau.YeuCauVatTuBenhViens)
            //        {
            //            item.WillDelete = true;
            //        }
            //    }
            //}

            return entity;
        }

        public async Task<YeuCauTiepNhan> RemoveDichVu(long id, string nhom, long yeuCauTiepNhanId, int? mucHuong, string lyDoHuy = null)
        {
            #region Cập nhật 21/12/2022
            //var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);
            var entity = await GetByIdHaveIncludeMienGiam(yeuCauTiepNhanId);
            #endregion

            //if (entity.YeuCauKhamBenhs.Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Count()
            //    + entity.YeuCauDichVuKyThuats.Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Count()
            //    + entity.YeuCauDichVuGiuongBenhViens.Where(p => p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy).Count()
            //    + entity.YeuCauDuocPhamBenhViens.Where(p => p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).Count()
            //    + entity.YeuCauVatTuBenhViens.Where(p => p.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy).Count() <= 1)
            //{
            //    return null;
            //}

            //remove dich vu

            if (nhom == Constants.NhomDichVu.DichVuKhamBenh)
            {
                var itemRemove = entity.YeuCauKhamBenhs.FirstOrDefault(p => p.Id == id);
                if (itemRemove != null)
                {
                    itemRemove.WillDelete = true;
                    if (!string.IsNullOrEmpty(lyDoHuy))
                    {
                        if (itemRemove.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new Exception(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                        }
                        itemRemove.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
                        itemRemove.LyDoHuyDichVu = lyDoHuy;
                    }
                    else
                    {
                        var anyTaiKhoanBenhNhanChi = _taiKhoanBenhNhanChiRepository.TableNoTracking.Any(o => o.YeuCauKhamBenhId == itemRemove.Id);
                        if (itemRemove.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && anyTaiKhoanBenhNhanChi)
                        {
                            throw new Exception(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                        }
                    }

                    //BVHD-3825
                    var mienGiam = itemRemove.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                    if (mienGiam != null)
                    {
                        mienGiam.DaHuy = true;
                        mienGiam.WillDelete = true;

                        var giamSoTienMienGiam = itemRemove.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                        if (giamSoTienMienGiam < 0)
                        {
                            giamSoTienMienGiam = 0;
                        }
                        itemRemove.SoTienMienGiam = giamSoTienMienGiam;
                    }

                    // trường hợp dịch vụ nội trú
                    await XuLyXoaYLenhKhiXoaDichVuAsync(EnumNhomGoiDichVu.DichVuKhamBenh, itemRemove.Id);
                }
            }
            else if (nhom == Constants.NhomDichVu.DichVuKyThuat)
            {
                var itemRemove = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == id);
                if (itemRemove != null)
                {
                    itemRemove.WillDelete = true;
                    if (!string.IsNullOrEmpty(lyDoHuy))
                    {
                        if (itemRemove.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new Exception(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                        }
                        itemRemove.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
                        itemRemove.LyDoHuyDichVu = lyDoHuy;
                    }
                    else
                    {
                        var anyTaiKhoanBenhNhanChi = _taiKhoanBenhNhanChiRepository.TableNoTracking.Any(o => o.YeuCauDichVuKyThuatId == itemRemove.Id);

                        if (itemRemove.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && anyTaiKhoanBenhNhanChi)
                        {
                            throw new Exception(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                        }
                    }

                    //BVHD-3825
                    var mienGiam = itemRemove.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                    if (mienGiam != null)
                    {
                        mienGiam.DaHuy = true;
                        mienGiam.WillDelete = true;

                        var giamSoTienMienGiam = itemRemove.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                        if (giamSoTienMienGiam < 0)
                        {
                            giamSoTienMienGiam = 0;
                        }
                        itemRemove.SoTienMienGiam = giamSoTienMienGiam;
                    }

                    // trường hợp dịch vụ nội trú
                    await XuLyXoaYLenhKhiXoaDichVuAsync(EnumNhomGoiDichVu.DichVuKyThuat, itemRemove.Id);
                }
            }
            else if (nhom == Constants.NhomDichVu.DichVuGiuong)
            {
                var itemRemove = entity.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.Id == id);
                if (itemRemove != null)
                {
                    itemRemove.WillDelete = true;
                }
            }
            else if (nhom == Constants.NhomDichVu.DuocPham)
            {
                var itemRemove = entity.YeuCauDuocPhamBenhViens.FirstOrDefault(p => p.Id == id);
                if (itemRemove != null)
                {
                    itemRemove.WillDelete = true;
                }
            }
            else if (nhom == Constants.NhomDichVu.VatTuTieuHao)
            {
                var itemRemove = entity.YeuCauVatTuBenhViens.FirstOrDefault(p => p.Id == id);
                if (itemRemove != null)
                {
                    itemRemove.WillDelete = true;

                }
            }

            //set lai gia cho dich vu
            //var lstDichVuOld = new List<DichVuOld>();
            //lstDichVuOld = GetListDichVuOld(entity);

            //Set default BHYT thanh toan and set new value
            //entity = await SetValueDefaultAndSetValueBHYTWithNewLogic(entity, mucHuong ?? 0);

            //Set trang thai thanh toan cho thu ngan
            //entity = SetTrangThaiThanhToanForThuNganAgain(entity, lstDichVuOld);

            return entity;
        }

        public async Task<YeuCauTiepNhan> ThemGoiCoChietKhauPopup(List<ChiDinhDichVuGridVo> lstGoi, long yeuCauTiepNhanId, int? mucHuong)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var locationId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var goiDichVu = await _goiDichVuRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == lstGoi[0].GoiCoChietKhauId);

            var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);


            //Them goi co chiet khau
            //TODO: need update goi dv
            var goiCoChietKhau = new YeuCauGoiDichVu
            {
                //CoChietKhau = true,
                //update goi dv 10/21
                //ChiPhiGoiDichVu = goiDichVu.ChiPhiGoiDichVu ?? 0,
                //LoaiGoiDichVu = goiDichVu.LoaiGoiDichVu,
                //GoiDichVuId = lstGoi[0].GoiCoChietKhauId,
                NhanVienChiDinhId = currentUserId,
                NoiChiDinhId = locationId,
                //Ten = lstGoi[0].TenGoiChietKhau,
                ThoiDiemChiDinh = DateTime.Now,
                TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien,
                TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
            };

            foreach (var model in lstGoi)
            {
                if (model.Nhom == Constants.NhomDichVu.DichVuKhamBenh)
                {
                    var lstNoiThucHienId = model.NoiThucHienId.Split(",");
                    var phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                    var nhanVienId = long.Parse(lstNoiThucHienId[1]);

                    var dichVuKhamBenhBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh).FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);

                    //add new dich vu kham benh into yeucautiepnhan
                    var yeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh
                    {
                        MaDichVu = dichVuKhamBenhBenhVien.Ma,
                        MaDichVuTT37 = dichVuKhamBenhBenhVien.DichVuKhamBenh?.MaTT37,
                        TenDichVu = dichVuKhamBenhBenhVien.Ten,
                        DuocHuongBaoHiem = model.DuocHuongBHYTPopup,
                        BaoHiemChiTra = null,
                        Gia = (decimal)model.DonGia,
                        //GiaBaoHiemThanhToan = 0,
                        //TiLeUuDai = Convert.ToInt32(item.TLMG),
                        NoiChiDinhId = locationId,
                        NoiDangKyId = phongBenhVienId,
                        //BacSiThucHienId = nhanVienId,
                        //BacSiDangKyId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                        ThoiDiemChiDinh = DateTime.Now,
                        ThoiDiemDangKy = DateTime.Now,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                        DichVuKhamBenhBenhVienId = dichVuKhamBenhBenhVien.Id,

                        NhomGiaDichVuKhamBenhBenhVienId = model.LoaiGiaId,

                        //
                        YeuCauTiepNhan = entity,
                    };
                    if (nhanVienId != 0)
                    {
                        yeuCauKhamBenh.BacSiDangKyId = nhanVienId;
                    }
                    else
                    {
                        yeuCauKhamBenh.BacSiDangKyId = null;
                    }
                    goiCoChietKhau.YeuCauKhamBenhs.Add(yeuCauKhamBenh);
                }
                else if (model.Nhom == Constants.NhomDichVu.DichVuKyThuat)
                {
                    long phongBenhVienId = 0;
                    long nhanVienId = 0;
                    if (model.NoiThucHienId.IndexOf(",") == -1)
                    {
                        phongBenhVienId = long.Parse(model.NoiThucHienId);
                    }
                    else
                    {
                        var lstNoiThucHienId = model.NoiThucHienId.Split(",");
                        phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                        nhanVienId = long.Parse(lstNoiThucHienId[1]);
                    }

                    var dichVuKyThuatBenhVien = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Include(p => p.DichVuKyThuat)
                    .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);

                    //add new dich vu ky thuat into yeucautiepnhan
                    var yeuCau = new YeuCauDichVuKyThuat
                    {
                        MaDichVu = dichVuKyThuatBenhVien.Ma,
                        Ma4350DichVu = dichVuKyThuatBenhVien.DichVuKyThuat?.Ma4350,
                        MaGiaDichVu = dichVuKyThuatBenhVien.DichVuKyThuat?.MaGia,
                        TenDichVu = dichVuKyThuatBenhVien.Ten,
                        DuocHuongBaoHiem = model.DuocHuongBHYTPopup,
                        BaoHiemChiTra = null,
                        Gia = (decimal)model.DonGia,
                        //GiaBaoHiemThanhToan = 0,
                        //TiLeUuDai = Convert.ToInt32(item.TLMG),
                        NoiChiDinhId = locationId,
                        NoiThucHienId = phongBenhVienId,
                        //NhanVienThucHienId = nhanVienId,
                        //BacSiThucHienId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                        ThoiDiemChiDinh = DateTime.Now,
                        SoLan = model.SoLuong ?? 0,
                        ThoiDiemDangKy = DateTime.Now,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,


                        DichVuKyThuatBenhVienId = dichVuKyThuatBenhVien.Id,
                        NhomGiaDichVuKyThuatBenhVienId = model.LoaiGiaId,

                        NhomChiPhi = dichVuKyThuatBenhVien.DichVuKyThuat?.NhomChiPhi ?? Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                        NhomDichVuBenhVienId = dichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                        LoaiDichVuKyThuat = GetLoaiDichVuKyThuat(dichVuKyThuatBenhVien.NhomDichVuBenhVienId),
                        //
                        YeuCauTiepNhan = entity,
                    };

                    goiCoChietKhau.YeuCauDichVuKyThuats.Add(yeuCau);
                }
                else
                {
                    long phongBenhVienId = 0;
                    long nhanVienId = 0;
                    if (model.NoiThucHienId.IndexOf(",") == -1)
                    {
                        phongBenhVienId = long.Parse(model.NoiThucHienId);
                    }
                    else
                    {
                        var lstNoiThucHienId = model.NoiThucHienId.Split(",");
                        phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                        nhanVienId = long.Parse(lstNoiThucHienId[1]);
                    }
                    var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .Include(p => p.DichVuGiuong)
                    .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);

                    var yeuCau = new YeuCauDichVuGiuongBenhVien
                    {
                        Ma = dichVuGiuongBenhVien.Ma,
                        MaTT37 = dichVuGiuongBenhVien.DichVuGiuong?.MaTT37,
                        Ten = dichVuGiuongBenhVien.Ten,
                        DuocHuongBaoHiem = model.DuocHuongBHYTPopup,
                        BaoHiemChiTra = null,
                        Gia = (decimal)model.DonGia,
                        //GiaBaoHiemThanhToan = 0,
                        NoiChiDinhId = locationId,
                        NoiThucHienId = phongBenhVienId,
                        //nhanvien = nhanVienId,
                        //BacSiThucHienId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiGiuongBenh.ChuaThucHien,
                        ThoiDiemChiDinh = DateTime.Now,
                        LoaiGiuong = dichVuGiuongBenhVien.LoaiGiuong,
                        MaGiuong = dichVuGiuongBenhVien.Ma,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                        NhomGiaDichVuGiuongBenhVienId = model.LoaiGiaId,
                        DichVuGiuongBenhVienId = dichVuGiuongBenhVien.Id,

                        //
                        YeuCauTiepNhan = entity,
                    };
                    goiCoChietKhau.YeuCauDichVuGiuongBenhViens.Add(yeuCau);
                }
            }
            //TODO: need update goi dv
            //entity.YeuCauGoiDichVus.Add(goiCoChietKhau);

            return entity;
        }

        public async Task<YeuCauTiepNhan> ThemGoiKhongChietKhauPopup(List<ChiDinhDichVuGridVo> lstGoi, long yeuCauTiepNhanId, int? mucHuong)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var locationId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);

            foreach (var model in lstGoi)
            {
                if (model.Nhom == Constants.NhomDichVu.DichVuKhamBenh)
                {
                    if (entity.YeuCauKhamBenhs.Any(p => p.DichVuKhamBenhBenhVienId == model.MaDichVuId)) continue;
                    var lstNoiThucHienId = model.NoiThucHienId.Split(",");
                    var phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                    var nhanVienId = long.Parse(lstNoiThucHienId[1]);

                    var dichVuKhamBenhBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh).FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);
                    var dichVuKhamBenhBenhVienGiaBaoHiem =
                        await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKhamBenhBenhVienId == model.MaDichVuId
                            && p.TuNgay.Date <= DateTime.Now.Date
                            && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                    //var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                    //                    (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong) * (mucHuong ?? 100) / 100;
                    //var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                    //                                  (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong);
                    //if (!model.DuocHuongBHYTPopup)
                    //{
                    //    bhytThanhToan = 0;
                    //    bhytThanhToanChuaBaoGomMucHuong = 0;
                    //}
                    //add new dich vu kham benh into yeucautiepnhan
                    var yeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh
                    {
                        MaDichVu = dichVuKhamBenhBenhVien.Ma,
                        MaDichVuTT37 = dichVuKhamBenhBenhVien.DichVuKhamBenh?.MaTT37,
                        TenDichVu = dichVuKhamBenhBenhVien.Ten,
                        DuocHuongBaoHiem = model.DuocHuongBHYTPopup,
                        BaoHiemChiTra = null,
                        Gia = (decimal)model.DonGia,
                        //GiaBaoHiemThanhToan = bhytThanhToan,
                        //TiLeUuDai = Convert.ToInt32(item.TLMG),
                        NoiChiDinhId = locationId,
                        NoiDangKyId = phongBenhVienId,
                        //BacSiThucHienId = nhanVienId,
                        //BacSiDangKyId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                        ThoiDiemChiDinh = DateTime.Now,
                        ThoiDiemDangKy = DateTime.Now,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                        DichVuKhamBenhBenhVienId = dichVuKhamBenhBenhVien.Id,
                        //
                        DonGiaBaoHiem = dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0,
                        TiLeBaoHiemThanhToan = dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,
                        //
                        NhomGiaDichVuKhamBenhBenhVienId = model.LoaiGiaId,

                    };
                    if (nhanVienId != 0)
                    {
                        yeuCauKhamBenh.BacSiDangKyId = nhanVienId;
                    }
                    else
                    {
                        yeuCauKhamBenh.BacSiDangKyId = null;
                    }
                    entity.YeuCauKhamBenhs.Add(yeuCauKhamBenh);
                }
                else if (model.Nhom == Constants.NhomDichVu.DichVuKyThuat)
                {
                    if (entity.YeuCauDichVuKyThuats.Any(p => p.DichVuKyThuatBenhVienId == model.MaDichVuId)) continue;

                    long phongBenhVienId = 0;
                    long nhanVienId = 0;
                    if (model.NoiThucHienId.IndexOf(",") == -1)
                    {
                        phongBenhVienId = long.Parse(model.NoiThucHienId);
                    }
                    else
                    {
                        var lstNoiThucHienId = model.NoiThucHienId.Split(",");
                        phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                        nhanVienId = long.Parse(lstNoiThucHienId[1]);
                    }

                    var dichVuKyThuatBenhVien = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Include(p => p.DichVuKyThuat)
                    .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);

                    var dichVuKyThuatBenhVienGiaBaoHiem =
                    await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKyThuatBenhVienId == model.MaDichVuId
                        && p.TuNgay.Date <= DateTime.Now.Date
                        && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                    //var bhytThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0) *
                    //                (dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong) * (mucHuong ?? 100) / 100;

                    //if (!model.DuocHuongBHYTPopup)
                    //{
                    //    bhytThanhToan = 0;
                    //}

                    //add new dich vu ky thuat into yeucautiepnhan
                    var yeuCau = new YeuCauDichVuKyThuat
                    {
                        MaDichVu = dichVuKyThuatBenhVien.Ma,
                        Ma4350DichVu = dichVuKyThuatBenhVien.DichVuKyThuat?.Ma4350,
                        MaGiaDichVu = dichVuKyThuatBenhVien.DichVuKyThuat?.MaGia,
                        TenDichVu = dichVuKyThuatBenhVien.Ten,
                        DuocHuongBaoHiem = model.DuocHuongBHYTPopup,
                        BaoHiemChiTra = null,
                        Gia = (decimal)model.DonGia,
                        //GiaBaoHiemThanhToan = bhytThanhToan,
                        //TiLeUuDai = Convert.ToInt32(item.TLMG),
                        NoiChiDinhId = locationId,
                        NoiThucHienId = phongBenhVienId,
                        //NhanVienThucHienId = nhanVienId,
                        //BacSiThucHienId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                        ThoiDiemChiDinh = DateTime.Now,
                        SoLan = model.SoLuong ?? 0,
                        ThoiDiemDangKy = DateTime.Now,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,


                        DichVuKyThuatBenhVienId = dichVuKyThuatBenhVien.Id,
                        NhomGiaDichVuKyThuatBenhVienId = model.LoaiGiaId,

                        NhomChiPhi = dichVuKyThuatBenhVien.DichVuKyThuat?.NhomChiPhi ?? Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                        NhomDichVuBenhVienId = dichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                        LoaiDichVuKyThuat = GetLoaiDichVuKyThuat(dichVuKyThuatBenhVien.NhomDichVuBenhVienId),
                        //
                        DonGiaBaoHiem = dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0,
                        TiLeBaoHiemThanhToan = dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,
                        //
                    };

                    entity.YeuCauDichVuKyThuats.Add(yeuCau);
                }
                else
                {
                    if (entity.YeuCauDichVuGiuongBenhViens.Any(p => p.DichVuGiuongBenhVienId == model.MaDichVuId)) continue;

                    long phongBenhVienId = 0;
                    long nhanVienId = 0;
                    if (model.NoiThucHienId.IndexOf(",") == -1)
                    {
                        phongBenhVienId = long.Parse(model.NoiThucHienId);
                    }
                    else
                    {
                        var lstNoiThucHienId = model.NoiThucHienId.Split(",");
                        phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                        nhanVienId = long.Parse(lstNoiThucHienId[1]);
                    }
                    var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .Include(p => p.DichVuGiuong)
                    .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);

                    var dichVuGiuongBenhVienGiaBaoHiem =
                    await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuGiuongBenhVienId == model.MaDichVuId
                        && p.TuNgay.Date <= DateTime.Now.Date
                        && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                    //var bhytThanhToan = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
                    //                (dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong) * (mucHuong ?? 100) / 100;

                    //if (!model.DuocHuongBHYTPopup)
                    //{
                    //    bhytThanhToan = 0;
                    //}

                    var yeuCau = new YeuCauDichVuGiuongBenhVien
                    {
                        Ma = dichVuGiuongBenhVien.Ma,
                        MaTT37 = dichVuGiuongBenhVien.DichVuGiuong?.MaTT37,
                        Ten = dichVuGiuongBenhVien.Ten,
                        DuocHuongBaoHiem = model.DuocHuongBHYTPopup,
                        BaoHiemChiTra = null,
                        Gia = (decimal)model.DonGia,
                        //GiaBaoHiemThanhToan = bhytThanhToan,
                        NoiChiDinhId = locationId,
                        NoiThucHienId = phongBenhVienId,
                        //nhanvien = nhanVienId,
                        //BacSiThucHienId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiGiuongBenh.ChuaThucHien,
                        ThoiDiemChiDinh = DateTime.Now,
                        LoaiGiuong = dichVuGiuongBenhVien.LoaiGiuong,
                        MaGiuong = dichVuGiuongBenhVien.Ma,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                        NhomGiaDichVuGiuongBenhVienId = model.LoaiGiaId,
                        DichVuGiuongBenhVienId = dichVuGiuongBenhVien.Id,
                        //
                        DonGiaBaoHiem = dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0,
                        TiLeBaoHiemThanhToan = dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,
                        //

                    };
                    entity.YeuCauDichVuGiuongBenhViens.Add(yeuCau);
                }
            }

            //set lai gia cho dich vu
            //var lstDichVuOld = new List<DichVuOld>();
            //lstDichVuOld = GetListDichVuOld(entity);

            //Set default BHYT thanh toan and set new value
            //entity = await SetValueDefaultAndSetValueBHYTWithNewLogic(entity, mucHuong ?? 0);

            //Set trang thai thanh toan cho thu ngan
            //entity = SetTrangThaiThanhToanForThuNganAgain(entity, lstDichVuOld);

            return entity;
        }

        public async Task<YeuCauTiepNhan> GetDataForDichVuKhamBenhForUpdateViewGridAsync(ThemDichVuKhamBenhVo model)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var locationId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var lstNoiThucHienId = model.NoiThucHienId.Split(",");
            var phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
            var nhanVienId = long.Parse(lstNoiThucHienId[1]);
            //var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.Include(p => p.KhoaPhong).FirstOrDefaultAsync(p => p.Id == phongBenhVienId);
            //var nhanVien =
            //    await _nhanVienRepository.TableNoTracking.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == nhanVienId);
            //var loaiGiaBenhVien = await _nhomGiaDichVuKhamBenhRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.LoaiGiaId);
            var dichVuKhamBenhBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh).FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);
            var dichVuKhamBenhBenhVienGiaBaoHiem =
                await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKhamBenhBenhVienId == model.MaDichVuId
                    && p.TuNgay.Date <= DateTime.Now.Date
                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

            //var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
            //                    (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong ?? 0) * (model.BHYTMucHuong ?? 100) / 100;
            //var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
            //                                  (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong ?? 0);
            //if (!model.DuocHuongBHYT)
            //{
            //    bhytThanhToan = 0;
            //    bhytThanhToanChuaBaoGomMucHuong = 0;
            //}

            #region cập nhật 23/12/2022
            //var entity = await GetByIdHaveInclude(model.YeuCauTiepNhanId ?? 0);
            var entity = await GetByIdHaveIncludeForAdddichVu(model.YeuCauTiepNhanId ?? 0);
            #endregion

            if (entity != null)
            {
                //add new dich vu kham benh into yeucautiepnhan
                var yeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh
                {
                    MaDichVu = dichVuKhamBenhBenhVien.Ma,
                    MaDichVuTT37 = dichVuKhamBenhBenhVien.DichVuKhamBenh?.MaTT37,
                    TenDichVu = dichVuKhamBenhBenhVien.Ten,
                    DuocHuongBaoHiem = model.DuocHuongBHYT,
                    BaoHiemChiTra = null,
                    Gia = (decimal)model.DonGia,
                    //GiaBaoHiemThanhToan = bhytThanhToan,
                    //TiLeUuDai = Convert.ToInt32(item.TLMG),
                    NoiChiDinhId = locationId,
                    NoiDangKyId = phongBenhVienId,
                    //BacSiThucHienId = nhanVienId,
                    //BacSiDangKyId = nhanVienId,
                    NhanVienChiDinhId = currentUserId,
                    TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                    ThoiDiemChiDinh = DateTime.Now,
                    ThoiDiemDangKy = DateTime.Now,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                    DichVuKhamBenhBenhVienId = dichVuKhamBenhBenhVien.Id,

                    NhomGiaDichVuKhamBenhBenhVienId = model.LoaiGiaId ?? 0,

                    //
                    DonGiaBaoHiem = dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0,
                    TiLeBaoHiemThanhToan = dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,
                    //

                };
                if (nhanVienId != 0)
                {
                    yeuCauKhamBenh.BacSiDangKyId = nhanVienId;
                }
                else
                {
                    yeuCauKhamBenh.BacSiDangKyId = null;
                }
                entity.YeuCauKhamBenhs.Add(yeuCauKhamBenh);

                //set lai gia cho dich vu
                //var lstDichVuOld = new List<DichVuOld>();
                //lstDichVuOld = GetListDichVuOld(entity);

                //Set default BHYT thanh toan and set new value
                //entity = await SetValueDefaultAndSetValueBHYTWithNewLogic(entity, model.BHYTMucHuong ?? 0);

                //Set trang thai thanh toan cho thu ngan
                //entity = SetTrangThaiThanhToanForThuNganAgain(entity, lstDichVuOld);

            }

            return entity;
        }

        public async Task<ChiDinhDichVuGridVo> GetDataForDichVuKhamBenhGridAsync(ThemDichVuKhamBenhVo model)
        {
            if (string.IsNullOrEmpty(model.NoiThucHienId))
            {
                return null;
            }
            //var mucBHYTChiTra = await _cauHinhTheoThoiGianService.SoTienBHYTSeThanhToanToanBo();
            var lstNoiThucHienId = model.NoiThucHienId.Split(",");
            var phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
            var nhanVienId = long.Parse(lstNoiThucHienId[1]);
            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.Include(p => p.KhoaPhong).FirstOrDefaultAsync(p => p.Id == phongBenhVienId);
            var nhanVien =
                await _nhanVienRepository.TableNoTracking.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == nhanVienId);
            var loaiGiaBenhVien = await _nhomGiaDichVuKhamBenhRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.LoaiGiaId);
            var dichVuKhamBenhBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh).FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);
            //var dichVuKhamBenhBenhVienGiaBenhVien =
            //    await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKhamBenhBenhVienId == model.MaDichVuId
            //        && p.NhomGiaDichVuKhamBenhBenhVienId == model.LoaiGiaId && p.TuNgay.Date <= DateTime.Now.Date 
            //        && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date ) ) );
            var dichVuKhamBenhBenhVienGiaBaoHiem =
                await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKhamBenhBenhVienId == model.MaDichVuId
                    && p.TuNgay.Date <= DateTime.Now.Date
                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
            //var doiTuongUuDaiDichVuKhamBenhBenhVien =
            //    await _doiTuongUuDaiDichVuKhamBenhBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
            //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
            //        && p.DichVuKhamBenhBenhVienId == model.MaDichVuId);
            //var donGia = dichVuKhamBenhBenhVienGiaBenhVien?.Gia ?? 0;
            //var thanhTien = donGia * (model.SoLuong ?? 0);
            var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong ?? 0) * (model.BHYTMucHuong ?? 100) / 100;
            var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                              (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (model.SoLuong ?? 0);

            if (!model.DuocHuongBHYT)
            {
                bhytThanhToan = 0;
                bhytThanhToanChuaBaoGomMucHuong = 0;
            }

            var result = new ChiDinhDichVuGridVo
            {
                MaDichVuId = model.MaDichVuId ?? 0,
                Ma = dichVuKhamBenhBenhVien != null ? dichVuKhamBenhBenhVien.Ma : "",
                TenDichVu = dichVuKhamBenhBenhVien != null ? dichVuKhamBenhBenhVien.Ten : "",
                LoaiGia = loaiGiaBenhVien != null ? loaiGiaBenhVien.Ten : "",
                LoaiGiaId = model.LoaiGiaId ?? 0,
                SoLuong = model.SoLuong ?? 0,
                DonGiaDisplay = (model.DonGia ?? 0).ApplyNumber(),
                DonGia = model.DonGia ?? 0,
                ThanhTienDisplay = ((model.DonGia ?? 0) * (model.SoLuong ?? 0)).ApplyNumber(),
                ThanhTien = (model.DonGia ?? 0) * (model.SoLuong ?? 0),
                BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber(),
                BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong,
                BHYTThanhToan = bhytThanhToan,
                //TLMGDisplay = doiTuongUuDaiDichVuKhamBenhBenhVien != null ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %" : "",
                //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai : 0,

                Nhom = Constants.NhomDichVu.DichVuKhamBenh,
                NoiThucHienId = model.NoiThucHienId,
                KhoaPhongId = phongBenhVienId,
                DuocHuongBHYT = model.DuocHuongBHYT,
                GiaBHYT = dichVuKhamBenhBenhVienGiaBaoHiem != null ? (double)dichVuKhamBenhBenhVienGiaBaoHiem.Gia : 0,
                TiLeBaoHiemThanhToan = dichVuKhamBenhBenhVienGiaBaoHiem != null ? dichVuKhamBenhBenhVienGiaBaoHiem.TiLeBaoHiemThanhToan : 0,

            };

            //result.SoTienMG = (result.ThanhTien - (double) result.BHYTThanhToan) * result.TLMG / 100;
            //result.SoTienMGDisplay = result.SoTienMG.ApplyNumber();
            result.BnThanhToan = result.ThanhTien - (double)result.BHYTThanhToan;
            result.BnThanhToanDisplay = result.BnThanhToan.ApplyNumber();
            if (phongBenhVien != null && nhanVien != null)
            {
                result.NoiThucHienDisplay =
                    phongBenhVien.KhoaPhong.Ma + " - " + phongBenhVien.Ten + " - " + nhanVien.User.HoTen;
                result.NoiThucHienId = model.NoiThucHienId;
            }

            return result;
        }

        public async Task<List<ChiDinhDichVuGridVo>> GetDataForGoiCoHoacKhongChietKhauGridForUpdateAsync(ThemDichVuKhamBenhVo model, bool isCoChietKhau, List<ListDichVuCheckTruocDo> ListDichVuCheckTruocDos)
        {
            var result = new List<ChiDinhDichVuGridVo>();

            if (!isCoChietKhau)
            {
                var goiDichVu = await _goiDichVuRepository.TableNoTracking
                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                    .ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                    .ThenInclude(p => p.DichVuGiuong)
                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)

                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                    .ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                    .ThenInclude(p => p.DichVuKhamBenh)
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                    .ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                    .ThenInclude(p => p.DichVuKyThuat)
                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)


                    .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);
                if (goiDichVu != null)
                {
                    if (goiDichVu.GoiDichVuChiTietDichVuGiuongs.Any())
                    {
                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuGiuongs)
                        {
                            var dichVuGiuongBenhVienGiaBaoHiem =
                                await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
                                    .FirstOrDefaultAsync(p =>
                                        p.DichVuGiuongBenhVienId == item.DichVuGiuongBenhVienId
                                        && p.TuNgay.Date <= DateTime.Now.Date
                                        && (p.DenNgay == null ||
                                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                            var bhytThanhToan = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                (dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1 *
                                                (100) / 100;
                            //(model.BHYTMucHuong ?? 100) / 100;

                            var donGiaEntity = item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuGiuongBenhVienId,
                                Ma = item.DichVuGiuongBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuGiuongBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuGiuongBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuGiuongBenhVien?.Id ?? 0,
                                SoLuong = 1,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                TLMGDisplay = "0 %",
                                TLMG = 0,

                                Nhom = Constants.NhomDichVu.DichVuGiuong,

                                //DuocHuongBHYT = model.DuocHuongBHYT,

                                GiaBHYT = (double)(dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0),
                                TiLeBaoHiemThanhToan = dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0
                            };
                            child.DuocHuongBHYT = model.BHYTMucHuong != null && child.GiaBHYT != 0;
                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.DonGiaDisplay;
                            child.BHYTThanhToanDisplay = "0";

                            child.BHYTThanhToan = 0;
                            child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                            child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                            child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan - child.SoTienMG;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;
                            result.Add(child);
                        }
                    }

                    if (goiDichVu.GoiDichVuChiTietDichVuKhamBenhs.Any())
                    {

                        //var doiTuongUuDaiDichVuKhamBenhBenhVien =
                        //    await _doiTuongUuDaiDichVuKhamBenhBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                        //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
                        //        && p.DichVuKhamBenhBenhVienId == model.MaDichVuId);

                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuKhamBenhs)
                        {
                            var dichVuKhamBenhBenhVienGiaBaoHiem =
                                await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                                    .FirstOrDefaultAsync(p =>
                                        p.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId
                                        && p.TuNgay.Date <= DateTime.Now.Date
                                        && (p.DenNgay == null ||
                                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                            var donGiaEntity = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1 *
                                                (model.BHYTMucHuong ?? 100) / 100;
                            var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                                  (dichVuKhamBenhBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
                            //if (!model.DuocHuongBHYT)
                            //{
                            //    bhytThanhToan = 0;
                            //    bhytThanhToanChuaBaoGomMucHuong = 0;
                            //}

                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuKhamBenhBenhVienId,
                                Ma = item.DichVuKhamBenhBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuKhamBenhBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId,
                                SoLuong = 1,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                //TLMGDisplay =
                                //    doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //        ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %"
                                //        : "",
                                //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //    ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai
                                //    : 0,

                                Nhom = Constants.NhomDichVu.DichVuKhamBenh,
                                //DuocHuongBHYT = model.DuocHuongBHYT,

                                GiaBHYT = (double)(dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0),
                                TiLeBaoHiemThanhToan = dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0
                            };
                            child.DuocHuongBHYT = model.BHYTMucHuong != null && child.GiaBHYT != 0;

                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.DonGiaDisplay;

                            child.BHYTThanhToan = bhytThanhToan;
                            child.BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber();
                            child.BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong;

                            child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                            child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan - child.SoTienMG;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;
                            result.Add(child);
                        }
                    }

                    if (goiDichVu.GoiDichVuChiTietDichVuKyThuats.Any())
                    {

                        //var doiTuongUuDaiDichVuKhamBenhBenhVien =
                        //    await _doiTuongUuDaiDichVuKyThuatBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                        //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
                        //        && p.DichVuKyThuatBenhVienId == model.MaDichVuId);

                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuKyThuats)
                        {


                            var dichVuKyThuatVienGiaBaoHiem =
                                    await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                                        .FirstOrDefaultAsync(p =>
                                            p.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId
                                            && p.TuNgay.Date <= DateTime.Now.Date
                                            && (p.DenNgay == null ||
                                                (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                            var bhytThanhToan = (dichVuKyThuatVienGiaBaoHiem?.Gia ?? 0) *
                                                (dichVuKyThuatVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1 *
                                                (100) / 100;


                            var donGiaEntity = item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuKyThuatBenhVienId,
                                Ma = item.DichVuKyThuatBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuKyThuatBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuKyThuatBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                                SoLuong = item.SoLan,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                //TLMGDisplay =
                                //    doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //        ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %"
                                //        : "",
                                //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //    ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai
                                //    : 0,

                                Nhom = Constants.NhomDichVu.DichVuKyThuat,

                                GiaBHYT = (double)(dichVuKyThuatVienGiaBaoHiem?.Gia ?? 0),
                                TiLeBaoHiemThanhToan = dichVuKyThuatVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,
                            };
                            child.DuocHuongBHYT = model.BHYTMucHuong != null && child.GiaBHYT != 0;

                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.DonGiaDisplay;
                            child.BHYTThanhToanDisplay = "0";

                            child.BHYTThanhToan = 0;
                            child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                            child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                            child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan - child.SoTienMG;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;
                            result.Add(child);

                        }
                    }
                }
            }
            else
            {
                var goiDichVu = await _goiDichVuRepository.TableNoTracking
                   .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                   .ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                   .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                   .ThenInclude(p => p.DichVuGiuong)
                   .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)

                   .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                   .ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                   .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                   .ThenInclude(p => p.DichVuKhamBenh)
                   .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                   .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                   .ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                   .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                   .ThenInclude(p => p.DichVuKyThuat)
                   .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)


                   .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);
                if (goiDichVu != null)
                {
                    double tongThanhTien = 0;
                    if (goiDichVu.GoiDichVuChiTietDichVuGiuongs.Any())
                    {
                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuGiuongs)
                        {
                            var donGiaEntity = item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));
                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuGiuongBenhVienId,
                                Ma = item.DichVuGiuongBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuGiuongBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuGiuongBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuGiuongBenhVien?.Id ?? 0,
                                SoLuong = 1,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                TLMGDisplay = "0 %",
                                TLMG = 0,

                                Nhom = Constants.NhomDichVu.DichVuGiuong,

                                //goi co chiet khau
                                IsGoiCoChietKhau = true,
                                GoiCoChietKhauId = goiDichVu.Id,
                                TenGoiChietKhau = goiDichVu.Ten,
                                //update goi dv 10/21
                                //TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu ?? 0,
                                DuocHuongBHYT = model.DuocHuongBHYT,
                            };
                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.ThanhTien.ApplyNumber();
                            child.BHYTThanhToanDisplay = "0";

                            child.BHYTThanhToan = 0;
                            child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                            child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                            child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan - child.SoTienMG;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;
                            result.Add(child);

                            tongThanhTien = tongThanhTien + child.ThanhTien;
                        }
                    }

                    if (goiDichVu.GoiDichVuChiTietDichVuKhamBenhs.Any())
                    {

                        //var doiTuongUuDaiDichVuKhamBenhBenhVien =
                        //    await _doiTuongUuDaiDichVuKhamBenhBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                        //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
                        //        && p.DichVuKhamBenhBenhVienId == model.MaDichVuId);

                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuKhamBenhs)
                        {
                            var dichVuKhamBenhBenhVienGiaBaoHiem =
                                await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                                    .FirstOrDefaultAsync(p =>
                                        p.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId
                                        && p.TuNgay.Date <= DateTime.Now.Date
                                        && (p.DenNgay == null ||
                                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                            var donGiaEntity = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1 *
                                                (model.BHYTMucHuong ?? 100) / 100;
                            var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                                  (dichVuKhamBenhBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;


                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuKhamBenhBenhVienId,
                                Ma = item.DichVuKhamBenhBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuKhamBenhBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId,
                                SoLuong = 1,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                //TLMGDisplay =
                                //    doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //        ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %"
                                //        : "",
                                //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //    ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai
                                //    : 0,

                                Nhom = Constants.NhomDichVu.DichVuKhamBenh,
                                //goi co chiet khau
                                IsGoiCoChietKhau = true,
                                GoiCoChietKhauId = goiDichVu.Id,
                                TenGoiChietKhau = goiDichVu.Ten,
                                //update goi dv 10/21
                                //TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu ?? 0,
                                DuocHuongBHYT = model.DuocHuongBHYT,

                            };
                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.ThanhTien.ApplyNumber();

                            child.BHYTThanhToan = 0;
                            child.BHYTThanhToanDisplay = "0";
                            child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                            child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                            child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan - child.SoTienMG;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;
                            result.Add(child);

                            tongThanhTien = tongThanhTien + child.ThanhTien;
                        }
                    }

                    if (goiDichVu.GoiDichVuChiTietDichVuKyThuats.Any())
                    {

                        //var doiTuongUuDaiDichVuKhamBenhBenhVien =
                        //    await _doiTuongUuDaiDichVuKyThuatBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                        //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
                        //        && p.DichVuKyThuatBenhVienId == model.MaDichVuId);

                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuKyThuats)
                        {
                            var donGiaEntity = item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuKyThuatBenhVienId,
                                Ma = item.DichVuKyThuatBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuKyThuatBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuKyThuatBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                                SoLuong = item.SoLan,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                //TLMGDisplay =
                                //    doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //        ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %"
                                //        : "",
                                //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //    ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai
                                //    : 0,

                                Nhom = Constants.NhomDichVu.DichVuKyThuat,

                                //goi co chiet khau
                                IsGoiCoChietKhau = true,
                                GoiCoChietKhauId = goiDichVu.Id,
                                TenGoiChietKhau = goiDichVu.Ten,
                                //update goi dv 10/21
                                //TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu ?? 0,
                                DuocHuongBHYT = model.DuocHuongBHYT,
                            };
                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.DonGiaDisplay;
                            child.BHYTThanhToanDisplay = "0";

                            child.BHYTThanhToan = 0;
                            child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                            child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                            child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan - child.SoTienMG;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;
                            result.Add(child);

                            tongThanhTien = tongThanhTien + child.ThanhTien;
                        }
                    }

                    foreach (var item in result)
                    {
                        item.TyLeChietKhau = Math.Round((tongThanhTien - item.TongChiPhiGoi) / tongThanhTien * 100);
                        item.TyLeChietKhauDisplay = item.TyLeChietKhau + "%";
                        item.DuocGiamTrongGoi = item.DonGia - (((tongThanhTien - item.TongChiPhiGoi) / tongThanhTien * 100) * item.DonGia / 100);
                        item.ThanhTienTrongGoi = item.DuocGiamTrongGoi * item.SoLuong ?? 0;
                    }
                }
            }

            return result;
        }

        public async Task<List<ChiDinhDichVuGridVo>> GetDataForGoiCoHoacKhongChietKhauGridAsync(ThemDichVuKhamBenhVo model, bool isCoChietKhau, List<ListDichVuCheckTruocDo> ListDichVuCheckTruocDos)
        {
            var result = new List<ChiDinhDichVuGridVo>();

            if (!isCoChietKhau)
            {
                var goiDichVu = await _goiDichVuRepository.TableNoTracking
                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                    .ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                    .ThenInclude(p => p.DichVuGiuong)
                    .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)

                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                    .ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                    .ThenInclude(p => p.DichVuKhamBenh)
                    .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                    .ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                    .ThenInclude(p => p.DichVuKyThuat)
                    .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)


                    .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);
                if (goiDichVu != null)
                {
                    if (goiDichVu.GoiDichVuChiTietDichVuGiuongs.Any())
                    {
                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuGiuongs)
                        {
                            var dichVuKhamGiuongBenhVienGiaBaoHiem =
                                await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
                                    .FirstOrDefaultAsync(p =>
                                        p.DichVuGiuongBenhVienId == item.DichVuGiuongBenhVienId
                                        && p.TuNgay.Date <= DateTime.Now.Date
                                        && (p.DenNgay == null ||
                                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                            var donGiaEntity = item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuGiuongBenhVienId,
                                Ma = item.DichVuGiuongBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuGiuongBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuGiuongBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuGiuongBenhVien?.Id ?? 0,
                                SoLuong = 1,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                TLMGDisplay = "0 %",
                                TLMG = 0,

                                Nhom = Constants.NhomDichVu.DichVuGiuong,
                                DuocHuongBHYT = model.DuocHuongBHYT,
                            };
                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.DonGiaDisplay;
                            child.BHYTThanhToanDisplay = "0";

                            child.BHYTThanhToan = 0;
                            child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                            child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                            child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan - child.SoTienMG;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;

                            //
                            child.CoGiaBHYT = DuocHuongBHYT(item.DichVuGiuongBenhVienId, 3).Result;
                            child.GiaBHYT = (double)(dichVuKhamGiuongBenhVienGiaBaoHiem?.Gia ?? 0);
                            child.GiaBHYTDislay = child.GiaBHYT.ApplyNumber();
                            child.TiLeBaoHiemThanhToan = (dichVuKhamGiuongBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0);

                            child.DuocHuongBHYTPopup = child.CoGiaBHYT && child.DuocHuongBHYT;

                            result.Add(child);
                        }
                    }

                    if (goiDichVu.GoiDichVuChiTietDichVuKhamBenhs.Any())
                    {

                        //var doiTuongUuDaiDichVuKhamBenhBenhVien =
                        //    await _doiTuongUuDaiDichVuKhamBenhBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                        //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
                        //        && p.DichVuKhamBenhBenhVienId == model.MaDichVuId);

                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuKhamBenhs)
                        {
                            var dichVuKhamBenhBenhVienGiaBaoHiem =
                                await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                                    .FirstOrDefaultAsync(p =>
                                        p.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId
                                        && p.TuNgay.Date <= DateTime.Now.Date
                                        && (p.DenNgay == null ||
                                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                            var donGiaEntity = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1 *
                                                (model.BHYTMucHuong ?? 100) / 100;
                            var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                                  (dichVuKhamBenhBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
                            if (!model.DuocHuongBHYT)
                            {
                                bhytThanhToan = 0;
                                bhytThanhToanChuaBaoGomMucHuong = 0;
                            }

                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuKhamBenhBenhVienId,
                                Ma = item.DichVuKhamBenhBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuKhamBenhBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId,
                                SoLuong = 1,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                //TLMGDisplay =
                                //    doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //        ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %"
                                //        : "",
                                //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //    ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai
                                //    : 0,

                                Nhom = Constants.NhomDichVu.DichVuKhamBenh,
                                DuocHuongBHYT = model.DuocHuongBHYT,
                            };
                            child.GiaBHYT = (double)(dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0);
                            child.GiaBHYTDislay = child.GiaBHYT.ApplyNumber();
                            child.TiLeBaoHiemThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0);

                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.DonGiaDisplay;

                            child.BHYTThanhToan = bhytThanhToan;
                            child.BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber();
                            child.BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong;

                            //child.SoTienMG = (child.ThanhTien - (double) child.BHYTThanhToan) * child.TLMG / 100;
                            //child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;

                            //
                            child.CoGiaBHYT = DuocHuongBHYT(item.DichVuKhamBenhBenhVienId, 1).Result;

                            child.DuocHuongBHYTPopup = child.CoGiaBHYT && child.DuocHuongBHYT;

                            result.Add(child);
                        }
                    }

                    if (goiDichVu.GoiDichVuChiTietDichVuKyThuats.Any())
                    {
                        //var doiTuongUuDaiDichVuKhamBenhBenhVien =
                        //    await _doiTuongUuDaiDichVuKyThuatBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                        //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
                        //        && p.DichVuKyThuatBenhVienId == model.MaDichVuId);

                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuKyThuats)
                        {

                            var dichVuKyThuatBenhVienGiaBaoHiem =
                                    await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                                        .FirstOrDefaultAsync(p =>
                                            p.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId
                                            && p.TuNgay.Date <= DateTime.Now.Date
                                            && (p.DenNgay == null ||
                                                (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                            var donGiaEntity = item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuKyThuatBenhVienId,
                                Ma = item.DichVuKyThuatBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuKyThuatBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuKyThuatBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                                SoLuong = item.SoLan,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                //TLMGDisplay =
                                //    doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //        ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %"
                                //        : "",
                                //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //    ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai
                                //    : 0,

                                Nhom = Constants.NhomDichVu.DichVuKyThuat,

                                //update 10/8/2020 k nho logic
                                DuocHuongBHYT = model.DuocHuongBHYT,
                            };
                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.DonGiaDisplay;
                            child.BHYTThanhToanDisplay = "0";

                            child.BHYTThanhToan = 0;
                            child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                            //child.SoTienMG = (child.ThanhTien - (double) child.BHYTThanhToan) * child.TLMG / 100;
                            //child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;

                            //
                            child.CoGiaBHYT = DuocHuongBHYT(item.DichVuKyThuatBenhVienId, 2).Result;
                            child.GiaBHYT = (double)(dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0);
                            child.GiaBHYTDislay = child.GiaBHYT.ApplyNumber();
                            child.TiLeBaoHiemThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0);

                            child.DuocHuongBHYTPopup = child.CoGiaBHYT && child.DuocHuongBHYT;

                            result.Add(child);

                        }
                    }
                }
            }
            else
            {
                var goiDichVu = await _goiDichVuRepository.TableNoTracking
                   .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                   .ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                   .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                   .ThenInclude(p => p.DichVuGiuong)
                   .Include(p => p.GoiDichVuChiTietDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)

                   .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                   .ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                   .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                   .ThenInclude(p => p.DichVuKhamBenh)
                   .Include(p => p.GoiDichVuChiTietDichVuKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                   .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                   .ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                   .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                   .ThenInclude(p => p.DichVuKyThuat)
                   .Include(p => p.GoiDichVuChiTietDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)


                   .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);
                if (goiDichVu != null)
                {
                    double tongThanhTien = 0;
                    if (goiDichVu.GoiDichVuChiTietDichVuGiuongs.Any())
                    {
                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuGiuongs)
                        {
                            var dichVuKhamGiuongBenhVienGiaBaoHiem =
                               await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
                                   .FirstOrDefaultAsync(p =>
                                       p.DichVuGiuongBenhVienId == item.DichVuGiuongBenhVienId
                                       && p.TuNgay.Date <= DateTime.Now.Date
                                       && (p.DenNgay == null ||
                                           (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                            var donGiaEntity = item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));
                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuGiuongBenhVienId,
                                Ma = item.DichVuGiuongBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuGiuongBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuGiuongBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuGiuongBenhVien?.Id ?? 0,
                                SoLuong = 1,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                TLMGDisplay = "0 %",
                                TLMG = 0,

                                Nhom = Constants.NhomDichVu.DichVuGiuong,

                                //goi co chiet khau
                                IsGoiCoChietKhau = true,
                                GoiCoChietKhauId = goiDichVu.Id,
                                TenGoiChietKhau = goiDichVu.Ten,
                                //update goi dv 10/21
                                //TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu ?? 0,
                                DuocHuongBHYT = model.DuocHuongBHYT,
                            };
                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.ThanhTien.ApplyNumber();
                            child.BHYTThanhToanDisplay = "0";

                            child.BHYTThanhToan = 0;
                            child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                            child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                            child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan - child.SoTienMG;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;

                            //
                            child.GiaBHYT = (double)(dichVuKhamGiuongBenhVienGiaBaoHiem?.Gia ?? 0);
                            child.GiaBHYTDislay = child.GiaBHYT.ApplyNumber();
                            child.TiLeBaoHiemThanhToan = (dichVuKhamGiuongBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0);

                            result.Add(child);

                            tongThanhTien = tongThanhTien + child.ThanhTien;
                        }
                    }

                    if (goiDichVu.GoiDichVuChiTietDichVuKhamBenhs.Any())
                    {

                        //var doiTuongUuDaiDichVuKhamBenhBenhVien =
                        //    await _doiTuongUuDaiDichVuKhamBenhBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                        //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
                        //        && p.DichVuKhamBenhBenhVienId == model.MaDichVuId);

                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuKhamBenhs)
                        {
                            var dichVuKhamBenhBenhVienGiaBaoHiem =
                                await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                                    .FirstOrDefaultAsync(p =>
                                        p.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId
                                        && p.TuNgay.Date <= DateTime.Now.Date
                                        && (p.DenNgay == null ||
                                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                            var donGiaEntity = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1 *
                                                (model.BHYTMucHuong ?? 100) / 100;
                            var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                                  (dichVuKhamBenhBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;


                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuKhamBenhBenhVienId,
                                Ma = item.DichVuKhamBenhBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuKhamBenhBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId,
                                SoLuong = 1,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                //TLMGDisplay =
                                //    doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //        ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %"
                                //        : "",
                                //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //    ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai
                                //    : 0,

                                Nhom = Constants.NhomDichVu.DichVuKhamBenh,
                                //goi co chiet khau
                                IsGoiCoChietKhau = true,
                                GoiCoChietKhauId = goiDichVu.Id,
                                TenGoiChietKhau = goiDichVu.Ten,
                                //update goi dv 10/21
                                //TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu ?? 0,
                                DuocHuongBHYT = model.DuocHuongBHYT,

                            };
                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.ThanhTien.ApplyNumber();

                            child.BHYTThanhToan = bhytThanhToan;
                            child.BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber();
                            child.BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong;

                            child.GiaBHYT = (double)(dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0);
                            child.GiaBHYTDislay = child.GiaBHYT.ApplyNumber();
                            child.TiLeBaoHiemThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0);

                            //child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                            //child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;

                            result.Add(child);

                            tongThanhTien = tongThanhTien + child.ThanhTien;
                        }
                    }

                    if (goiDichVu.GoiDichVuChiTietDichVuKyThuats.Any())
                    {

                        //var doiTuongUuDaiDichVuKhamBenhBenhVien =
                        //    await _doiTuongUuDaiDichVuKyThuatBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                        //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
                        //        && p.DichVuKyThuatBenhVienId == model.MaDichVuId);

                        foreach (var item in goiDichVu.GoiDichVuChiTietDichVuKyThuats)
                        {
                            var dichVuKyThuatBenhVienGiaBaoHiem =
                                    await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                                        .FirstOrDefaultAsync(p =>
                                            p.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId
                                            && p.TuNgay.Date <= DateTime.Now.Date
                                            && (p.DenNgay == null ||
                                                (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                            var donGiaEntity = item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens
                                .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                                                     && (p.DenNgay == null ||
                                                         (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                                                          DateTime.Now.Date)));

                            var child = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = item.DichVuKyThuatBenhVienId,
                                Ma = item.DichVuKyThuatBenhVien?.Ma ?? "",
                                TenDichVu = item.DichVuKyThuatBenhVien?.Ten ?? "",
                                LoaiGia = item.NhomGiaDichVuKyThuatBenhVien?.Ten ?? "",
                                LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                                SoLuong = item.SoLan,
                                DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                                DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                                //TLMGDisplay =
                                //    doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //        ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %"
                                //        : "",
                                //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null
                                //    ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai
                                //    : 0,

                                Nhom = Constants.NhomDichVu.DichVuKyThuat,

                                //goi co chiet khau
                                IsGoiCoChietKhau = true,
                                GoiCoChietKhauId = goiDichVu.Id,
                                TenGoiChietKhau = goiDichVu.Ten,
                                //update goi dv 10/21
                                //TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu ?? 0,
                                DuocHuongBHYT = model.DuocHuongBHYT,
                            };
                            child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                            child.ThanhTienDisplay = child.DonGiaDisplay;
                            child.BHYTThanhToanDisplay = "0";

                            child.BHYTThanhToan = 0;
                            child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                            //child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100; 
                            //child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                            child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan;
                            child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                            child.IsHaveNoiThucHien = true;

                            //
                            child.GiaBHYT = (double)(dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0);
                            child.GiaBHYTDislay = child.GiaBHYT.ApplyNumber();
                            child.TiLeBaoHiemThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem
                                                                       ?.TiLeBaoHiemThanhToan ?? 0);


                            result.Add(child);

                            tongThanhTien = tongThanhTien + child.ThanhTien;
                        }
                    }

                    foreach (var item in result)
                    {
                        item.TyLeChietKhau = Math.Round((tongThanhTien - item.TongChiPhiGoi) / tongThanhTien * 100);
                        item.TyLeChietKhauDisplay = item.TyLeChietKhau + "%";
                        item.DuocGiamTrongGoi = item.DonGia - (((tongThanhTien - item.TongChiPhiGoi) / tongThanhTien * 100) * item.DonGia / 100);
                        item.ThanhTienTrongGoi = item.DuocGiamTrongGoi * item.SoLuong ?? 0;
                    }
                }
            }


            return result;
        }

        public async Task<ChiDinhDichVuKyThuatGridVo> GetDataForDichVuKyThuatGridAsync(ThemDichVuKyThuatVo model, int loai)
        {
            var loaiGiaBenhVien = await _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == (model.LoaiGiaId ?? 1));

            //var lstNoiThucHienId = model.NoiThucHienId.Split(",");
            var phongBenhVienId = long.Parse(model.NoiThucHienId);
            //var nhanVienId = long.Parse(lstNoiThucHienId[1]);
            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.Include(p => p.KhoaPhong).FirstOrDefaultAsync(p => p.Id == phongBenhVienId);
            //var nhanVien =
            //    await _nhanVienRepository.TableNoTracking.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == nhanVienId);

            //var doiTuongUuDaiDichVuKhamBenhBenhVien =
            //    await _doiTuongUuDaiDichVuKyThuatBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
            //        p.DoiTuongUuDaiId == model.DoiTuongUuDaiId
            //        && p.DichVuKyThuatBenhVienId == model.MaDichVuId);

            var result = new ChiDinhDichVuKyThuatGridVo();


            //var dichVuKyThuatBenhVienGiaBenhVien =
            //    await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
            //    .FirstOrDefaultAsync(p => p.DichVuKyThuatBenhVienId == model.MaDichVuId
            //                              && p.TuNgay.Date <= DateTime.Now.Date
            //                              && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

            if (loai == 1)
            {
                var dichVuKyThuatBenhVien = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Include(p => p.DichVuKyThuat)
                    .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);

                var dichVuKyThuatBenhVienGiaBaoHiem =
                await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKyThuatBenhVienId == model.MaDichVuId
                    && p.TuNgay.Date <= DateTime.Now.Date
                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                result = new ChiDinhDichVuKyThuatGridVo
                {
                    MaDichVuId = model.MaDichVuId ?? 0,
                    Ma = dichVuKyThuatBenhVien != null ? dichVuKyThuatBenhVien.Ma : "",
                    TenDichVu = dichVuKyThuatBenhVien != null ? dichVuKyThuatBenhVien.Ten : "",
                    SoLuong = model.SoLuong ?? 0,
                    DonGiaDisplay = (model.DonGia ?? 0).ApplyNumber(),
                    DonGia = model.DonGia ?? 0,
                    ThanhTienDisplay = ((model.DonGia ?? 0) * (model.SoLuong ?? 0)).ApplyNumber(),
                    ThanhTien = (model.DonGia ?? 0) * (model.SoLuong ?? 0),
                    //TLMGDisplay = doiTuongUuDaiDichVuKhamBenhBenhVien != null ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %" : "",
                    //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai : 0,

                    LoaiGia = loaiGiaBenhVien != null ? loaiGiaBenhVien.Ten : "",
                    LoaiGiaId = loaiGiaBenhVien != null ? loaiGiaBenhVien.Id : 0,

                    Nhom = Constants.NhomDichVu.DichVuKyThuat,
                    NoiThucHienId = model.NoiThucHienId.ToString(),
                    KhoaPhongId = phongBenhVienId,

                    GiaBHYT = dichVuKyThuatBenhVienGiaBaoHiem != null ? (double)dichVuKyThuatBenhVienGiaBaoHiem.Gia : 0,
                    TiLeBaoHiemThanhToan = dichVuKyThuatBenhVienGiaBaoHiem != null ? dichVuKyThuatBenhVienGiaBaoHiem.TiLeBaoHiemThanhToan : 0,
                    DuocHuongBHYT = false,
                };
            }
            else if (loai == 2)
            {
                var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienRepository.TableNoTracking
                    .Include(p => p.DichVuGiuong)
                    .FirstOrDefaultAsync(p => p.Id == model.MaDichVuId);

                var dichVuGiuongBenhVienGiaBaoHiem =
                await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuGiuongBenhVienId == model.MaDichVuId
                    && p.TuNgay.Date <= DateTime.Now.Date
                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                result = new ChiDinhDichVuKyThuatGridVo
                {
                    MaDichVuId = model.MaDichVuId ?? 0,
                    Ma = dichVuGiuongBenhVien != null ? dichVuGiuongBenhVien.Ma : "",
                    TenDichVu = dichVuGiuongBenhVien != null ? dichVuGiuongBenhVien.Ten : "",
                    SoLuong = model.SoLuong ?? 0,
                    DonGiaDisplay = (model.DonGia ?? 0).ApplyNumber(),
                    DonGia = model.DonGia ?? 0,
                    ThanhTienDisplay = ((model.DonGia ?? 0) * (model.SoLuong ?? 0)).ApplyNumber(),
                    ThanhTien = (model.DonGia ?? 0) * (model.SoLuong ?? 0),
                    //TLMGDisplay = doiTuongUuDaiDichVuKhamBenhBenhVien != null ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %" : "",
                    //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai : 0,

                    LoaiGia = loaiGiaBenhVien != null ? loaiGiaBenhVien.Ten : "",
                    LoaiGiaId = loaiGiaBenhVien != null ? loaiGiaBenhVien.Id : 0,

                    Nhom = Constants.NhomDichVu.DichVuGiuong,
                    NoiThucHienId = model.NoiThucHienId.ToString(),
                    KhoaPhongId = phongBenhVienId,

                    GiaBHYT = dichVuGiuongBenhVienGiaBaoHiem != null ? (double)dichVuGiuongBenhVienGiaBaoHiem.Gia : 0,
                    TiLeBaoHiemThanhToan = dichVuGiuongBenhVienGiaBaoHiem != null ? dichVuGiuongBenhVienGiaBaoHiem.TiLeBaoHiemThanhToan : 0,
                    DuocHuongBHYT = false,
                };
            }

            //result.SoTienMG = (result.ThanhTien - result.BnThanhToan) * result.TLMG / 100;
            result.BnThanhToan = result.ThanhTien;
            result.BnThanhToanDisplay = result.BnThanhToan.ApplyNumber();
            if (phongBenhVien != null)
            {
                result.NoiThucHienDisplay =
                    phongBenhVien.KhoaPhong.Ma + " - " + phongBenhVien.Ten;
                result.NoiThucHienId = model.NoiThucHienId.ToString();
            }

            return result;

        }

        public async Task<double> GetDonGia(GetDonGiaVo model)
        {
            var dichVuKhamBenhBenhVienGiaBenhVien =
                await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKhamBenhBenhVienId == model.DichVuKhamBenhBenhVienId
                    && p.NhomGiaDichVuKhamBenhBenhVienId == model.NhomGiaDichVuKhamBenhBenhVienId && p.TuNgay.Date <= DateTime.Now.Date
                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
            return dichVuKhamBenhBenhVienGiaBenhVien != null ? (double)dichVuKhamBenhBenhVienGiaBenhVien.Gia : 0;
        }

        public async Task<GetDonGiaVo> GetDonGiaKyThuat(GetDonGiaVo model)
        {
            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var dichVuKyThuatBenhVienGiaBenhVien = await _dichVuKyThuatBenhVienGiaBenhVienRepository
                .TableNoTracking
                .OrderByDescending(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId)
                .ThenBy(x => x.CreatedOn)
                .Where(p => p.DichVuKyThuatBenhVienId == model.DichVuKhamBenhBenhVienId 
                                          && (model.NhomGiaDichVuKhamBenhBenhVienId == null || model.NhomGiaDichVuKhamBenhBenhVienId == 0 || p.NhomGiaDichVuKyThuatBenhVienId == model.NhomGiaDichVuKhamBenhBenhVienId)
                                                           && p.TuNgay.Date <= DateTime.Now.Date
                                                           && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)))
                .Select(item => new GetDonGiaVo()
                {
                    NhomGiaDichVuKhamBenhBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                    DonGia = item.Gia
                })
                .FirstOrDefaultAsync();
            if (model.IsCheckValidDonGia == true && dichVuKyThuatBenhVienGiaBenhVien == null)
            {
                throw new Exception(_localizationService.GetResource("ChiDinhDichVu.DonGiaDichVu.HetHan"));
            }
            //return dichVuKyThuatBenhVienGiaBenhVien != null ? (double)dichVuKyThuatBenhVienGiaBenhVien.Gia : 0;
            return dichVuKyThuatBenhVienGiaBenhVien;
        }

        public async Task<double> GetDonGiaGiuongBenh(GetDonGiaVo model)
        {
            var entity = await _dichVuGiuongBenhVienGiaBenhVienRepository
                .TableNoTracking.FirstOrDefaultAsync(p => p.DichVuGiuongBenhVienId == model.DichVuKhamBenhBenhVienId && p.NhomGiaDichVuGiuongBenhVienId == model.NhomGiaDichVuKhamBenhBenhVienId
                                                          && p.TuNgay.Date <= DateTime.Now.Date
                                                          && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
            return entity != null ? (double)entity.Gia : 0;
        }

        public async Task<List<LookupItemVo>> GetTuyenChuyen(DropDownListRequestModel model)
        {
            if (model.Id != 0)
            {
                var lst = await _benhVienRepository.TableNoTracking.Where(p => p.Id == model.Id)
                .Take(model.Take)
                .ToListAsync();
                var query = lst.Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                }).ToList();

                return query;
            }
            else
            {

                var lst = await _benhVienRepository.TableNoTracking.Where(p => p.Ten.Contains(model.Query ?? ""))
                    .Take(model.Take)
                    .ToListAsync();
                var query = lst.Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id,
                }).ToList();

                return query;
            }
        }

        public async Task<List<LookupItemVo>> GetCongTyBaoHiemTuNhan(DropDownListRequestModel model)
        {
            var lst = await _congTyBaoHiemTuNhanRepository.TableNoTracking.Where(p => p.Ten.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();
            var query = lst.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<ThemBaoHiemTuNhan> GetThongTinBHTN(long congTyBaoHiemTuNhanId)
        {
            var congTyBHTN =
                await _congTyBaoHiemTuNhanRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                    p.Id == congTyBaoHiemTuNhanId);
            var result = new ThemBaoHiemTuNhan();
            result.DiaChi = congTyBHTN?.DiaChi;
            result.SoDienThoai = congTyBHTN?.SoDienThoai;
            result.CongTyBaoHiemTuNhanId = congTyBaoHiemTuNhanId;

            return result;
        }

        public async Task<DiaChiBHYT> GetDiaChiBHYT(DiaChiBHYT model)
        {
            var result = new DiaChiBHYT();
            var tinhThanh = await _donViHanhChinhRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                p.CapHanhChinh == Enums.CapHanhChinh.TinhThanhPho && p.Ten.Contains(model.TinhThanh));
            var tinhThanhId = tinhThanh?.Id ?? 0;
            var quanHuyen = await _donViHanhChinhRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                p.CapHanhChinh == Enums.CapHanhChinh.QuanHuyen && p.Ten.Contains(model.QuanHuyen) && p.TrucThuocDonViHanhChinhId == tinhThanhId);
            var quanHuyenId = quanHuyen?.Id ?? 0;
            var phuongXa = await _donViHanhChinhRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                p.CapHanhChinh == Enums.CapHanhChinh.PhuongXa && p.Ten.Contains(model.PhuongXa) && p.TrucThuocDonViHanhChinhId == quanHuyenId);
            result.TinhThanhId = tinhThanh?.Id;
            result.QuanHuyenId = quanHuyen?.Id;
            result.PhuongXaId = phuongXa?.Id;
            return result;
        }

        public async Task<ThemBaoHiemTuNhanGridVo> ThemThongTinBHTN(ThemBaoHiemTuNhan model)
        {
            var result = new ThemBaoHiemTuNhanGridVo();
            result.CongTyBaoHiemTuNhanId = model.CongTyBaoHiemTuNhanId;
            result.CongTyDisplay =
            (await _congTyBaoHiemTuNhanRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                p.Id == (model.CongTyBaoHiemTuNhanId ?? 0)))?.Ten;
            result.MaSoThe = model.MaSoThe;
            result.NgayHieuLuc = model.NgayHieuLuc;
            result.NgayHieuLucDisplay =
                model.NgayHieuLuc != null ? (model.NgayHieuLuc ?? DateTime.Now).ApplyFormatDate() : null;
            result.NgayHetHan = model.NgayHetHan;
            result.NgayHetHanDisplay = model.NgayHetHan != null ? (model.NgayHetHan ?? DateTime.Now).ApplyFormatDate() : null;
            result.SoDienThoai = model.SoDienThoai;
            result.DiaChi = model.DiaChi;
            result.Id = model.Id ?? 0;

            return result;
        }

        public async Task<List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>> SetListYeuCauKhamBenh(List<ChiDinhDichVuGridVo> model)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var locationId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var lstYeuCauKhamBenh = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            foreach (var item in model)
            {
                var lstNoiThucHienId = item.NoiThucHienId.Split(",");
                var phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                var nhanVienId = long.Parse(lstNoiThucHienId[1]);

                var dichVuKhamBenhBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh).FirstOrDefaultAsync(p => p.Id == item.MaDichVuId);

                var yeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh
                {
                    MaDichVu = item.Ma,
                    MaDichVuTT37 = dichVuKhamBenhBenhVien.DichVuKhamBenh?.MaTT37,
                    TenDichVu = item.TenDichVu,
                    DuocHuongBaoHiem = item.DuocHuongBHYT,
                    BaoHiemChiTra = null,
                    Gia = (decimal)item.DonGia,
                    //GiaBaoHiemThanhToan = item.BHYTThanhToan,
                    TiLeUuDai = Convert.ToInt32(item.TLMG),
                    NoiDangKyId = phongBenhVienId,
                    NoiChiDinhId = locationId,
                    //BacSiThucHienId = nhanVienId,
                    BacSiDangKyId = nhanVienId,
                    NhanVienChiDinhId = currentUserId,
                    TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                    ThoiDiemChiDinh = DateTime.Now,
                    ThoiDiemDangKy = DateTime.Now,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,

                    NhomGiaDichVuKhamBenhBenhVienId = item.LoaiGiaId,
                    DichVuKhamBenhBenhVienId = dichVuKhamBenhBenhVien.Id,
                };
                lstYeuCauKhamBenh.Add(yeuCauKhamBenh);
            }

            return lstYeuCauKhamBenh;
        }

        public async Task<List<YeuCauDichVuKyThuat>> SetListYeuCauKyThuat(List<ChiDinhDichVuGridVo> model)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var locationId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var lstYeuCauDichVuKyThuat = new List<YeuCauDichVuKyThuat>();

            foreach (var item in model)
            {
                var lstNoiThucHienId = item.NoiThucHienId.Split(",");
                var phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                var nhanVienId = long.Parse(lstNoiThucHienId[1]);

                var dichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Include(p => p.DichVuKyThuat).FirstOrDefaultAsync(p => p.Id == item.MaDichVuId);


                var yeuCau = new YeuCauDichVuKyThuat
                {
                    MaDichVu = item.Ma,
                    Ma4350DichVu = dichVuKyThuat.DichVuKyThuat?.Ma4350,
                    MaGiaDichVu = dichVuKyThuat.DichVuKyThuat?.MaGia,
                    TenDichVu = item.TenDichVu,
                    DuocHuongBaoHiem = item.DuocHuongBHYT,
                    BaoHiemChiTra = null,
                    Gia = (decimal)item.DonGia,
                    //GiaBaoHiemThanhToan = item.BHYTThanhToan,
                    TiLeUuDai = Convert.ToInt32(item.TLMG),
                    NoiChiDinhId = locationId,
                    NoiThucHienId = phongBenhVienId,
                    NhanVienThucHienId = nhanVienId,
                    //BacSiThucHienId = nhanVienId,
                    NhanVienChiDinhId = currentUserId,
                    TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                    ThoiDiemChiDinh = DateTime.Now,
                    SoLan = item.SoLuong ?? 0,
                    ThoiDiemDangKy = DateTime.Now,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,

                    DichVuKyThuatBenhVienId = dichVuKyThuat.Id,
                    NhomGiaDichVuKyThuatBenhVienId = item.LoaiGiaId,

                    NhomChiPhi = dichVuKyThuat.DichVuKyThuat?.NhomChiPhi ?? Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                    NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                    LoaiDichVuKyThuat = GetLoaiDichVuKyThuat(dichVuKyThuat.NhomDichVuBenhVienId)
                };
                lstYeuCauDichVuKyThuat.Add(yeuCau);
            }

            return lstYeuCauDichVuKyThuat;

        }

        public async Task<YeuCauTiepNhan> SetListYeuCauKhac(List<ChiDinhDichVuGridVo> model, YeuCauTiepNhan entity)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var locationId = _userAgentHelper.GetCurrentNoiLLamViecId();
            //var lstGoiDichVu = new List<YeuCauGoiDichVu>();
            var lstCTGoiDVAdded = new List<YeuCauGoiDichVu>();
            foreach (var item in model)
            {
                var ycGoiDV = new YeuCauGoiDichVu();
                //long ycGoiDvId = 0;

                long phongBenhVienId = 0;
                long nhanVienId = 0;
                if (item.NoiThucHienId.IndexOf(",") == -1)
                {
                    phongBenhVienId = long.Parse(item.NoiThucHienId);
                }
                else
                {
                    var lstNoiThucHienId = item.NoiThucHienId.Split(",");
                    phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                    nhanVienId = long.Parse(lstNoiThucHienId[1]);
                }


                if (item.IsGoiCoChietKhau)
                {
                    ycGoiDV = _yeuCauGoiDichVuRepository.Table
                        .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                        .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                        .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
                        .FirstOrDefault(p => p.BenhNhanId == entity.BenhNhanId && p.Id == item.GoiCoChietKhauId);
                    if (ycGoiDV != null)
                    {
                        //ycGoiDvId = ycGoiDV.Id;
                    }

                    // trường hợp này hiện tại ko còn xử lý nữa
                    else
                    {
                        if (lstCTGoiDVAdded.Any(p => p.ChuongTrinhGoiDichVuId == item.GoiCoChietKhauId))
                        {
                            ycGoiDV = lstCTGoiDVAdded.First(p => p.ChuongTrinhGoiDichVuId == item.GoiCoChietKhauId);
                        }
                        else
                        {
                            var ct = await _chuongTrinhGoiDichVuRepository.Table
                            .Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                            .Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                            .Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
                            .FirstOrDefaultAsync(p => p.Id == item.GoiCoChietKhauId);

                            ycGoiDV = new YeuCauGoiDichVu();
                            if (entity.BenhNhanId == null || entity.BenhNhanId == 0)
                            {
                                ycGoiDV.BenhNhan = entity.BenhNhan;
                            }
                            else
                            {
                                ycGoiDV.BenhNhanId = entity.BenhNhanId ?? 0;
                            }
                            ycGoiDV.ChuongTrinhGoiDichVuId = item.GoiCoChietKhauId.Value;
                            ycGoiDV.ChuongTrinhGoiDichVu = ct;
                            ycGoiDV.MaChuongTrinh = ct.Ma;
                            ycGoiDV.TenChuongTrinh = ct.Ten;
                            ycGoiDV.GiaTruocChietKhau = ct.GiaTruocChietKhau;
                            ycGoiDV.GiaSauChietKhau = ct.GiaSauChietKhau;
                            ycGoiDV.TenGoiDichVu = ct.TenGoiDichVu;
                            ycGoiDV.MoTaGoiDichVu = ct.MoTaGoiDichVu;
                            ycGoiDV.GoiSoSinh = ct.GoiSoSinh;
                            ycGoiDV.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
                            ycGoiDV.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                            ycGoiDV.ThoiDiemChiDinh = DateTime.Now;
                            ycGoiDV.BoPhanMarketingDangKy = false;
                            ycGoiDV.TrangThai = EnumTrangThaiYeuCauGoiDichVu.DangThucHien; // Enums.EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien; // cập nhật theo logic mới
                            ycGoiDV.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;

                            lstCTGoiDVAdded.Add(ycGoiDV);
                        }

                    }
                }

                if (item.Nhom.Equals(Constants.NhomDichVu.DichVuKhamBenh))
                {
                    var dichVuKhamBenhBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh).FirstOrDefaultAsync(p => p.Id == item.MaDichVuId);

                    var yeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh
                    {
                        MaDichVu = item.Ma,
                        MaDichVuTT37 = dichVuKhamBenhBenhVien.DichVuKhamBenh?.MaTT37,
                        TenDichVu = item.TenDichVu,
                        DuocHuongBaoHiem = item.DuocHuongBHYT,
                        BaoHiemChiTra = null,
                        Gia = (decimal)item.DonGia,
                        //GiaBaoHiemThanhToan = item.BHYTThanhToan,
                        TiLeUuDai = Convert.ToInt32(item.TLMG),
                        NoiChiDinhId = locationId,
                        NoiDangKyId = phongBenhVienId,
                        //BacSiThucHienId = nhanVienId,
                        //BacSiDangKyId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                        ThoiDiemChiDinh = DateTime.Now,
                        ThoiDiemDangKy = DateTime.Now,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                        DichVuKhamBenhBenhVienId = dichVuKhamBenhBenhVien.Id,

                        NhomGiaDichVuKhamBenhBenhVienId = item.LoaiGiaId,
                        //
                        DonGiaBaoHiem = item.GiaBHYT != null ? (decimal)item.GiaBHYT : 0,
                        MucHuongBaoHiem = item.TiLeBaoHiemThanhToan,

                    };
                    if (nhanVienId != 0)
                    {
                        yeuCauKhamBenh.BacSiDangKyId = nhanVienId;
                    }
                    else
                    {
                        yeuCauKhamBenh.BacSiDangKyId = null;
                    }

                    if (item.IsGoiCoChietKhau)
                    {
                        var goiDichVuKhamBenh = ycGoiDV.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(o =>
                            o.DichVuKhamBenhBenhVienId == yeuCauKhamBenh.DichVuKhamBenhBenhVienId &&
                            o.NhomGiaDichVuKhamBenhBenhVienId == yeuCauKhamBenh.NhomGiaDichVuKhamBenhBenhVienId);
                        if (goiDichVuKhamBenh != null)
                        {
                            yeuCauKhamBenh.YeuCauGoiDichVu = ycGoiDV;
                            yeuCauKhamBenh.Gia = goiDichVuKhamBenh.DonGia;
                            yeuCauKhamBenh.DonGiaTruocChietKhau = goiDichVuKhamBenh.DonGiaTruocChietKhau;
                            yeuCauKhamBenh.DonGiaSauChietKhau = goiDichVuKhamBenh.DonGiaSauChietKhau;
                        }
                    }

                    if (item.YeuCauGoiDichVuKhuyenMaiId != null && item.YeuCauGoiDichVuKhuyenMaiId != 0)
                    {
                        yeuCauKhamBenh.SoTienMienGiam = item.SoTienMienGiam;
                        yeuCauKhamBenh.MienGiamChiPhis.Add(new MienGiamChiPhi()
                        {
                            YeuCauTiepNhan = entity,
                            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                            SoTien = item.SoTienMienGiam ?? 0,
                            YeuCauGoiDichVuId = item.YeuCauGoiDichVuKhuyenMaiId
                        });
                    }

                    entity.YeuCauKhamBenhs.Add(yeuCauKhamBenh);
                }
                else if (item.Nhom.Equals(Constants.NhomDichVu.DichVuKyThuat))
                {

                    var dichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Include(p => p.DichVuKyThuat).FirstOrDefaultAsync(p => p.Id == item.MaDichVuId);

                    var yeuCau = new YeuCauDichVuKyThuat
                    {
                        MaDichVu = item.Ma,
                        Ma4350DichVu = dichVuKyThuat.DichVuKyThuat?.Ma4350,
                        MaGiaDichVu = dichVuKyThuat.DichVuKyThuat?.MaGia,
                        TenDichVu = item.TenDichVu,
                        DuocHuongBaoHiem = item.DuocHuongBHYT,
                        BaoHiemChiTra = null,
                        Gia = (decimal)item.DonGia,
                        //GiaBaoHiemThanhToan = item.BHYTThanhToan,
                        TiLeUuDai = Convert.ToInt32(item.TLMG),
                        NoiChiDinhId = locationId,
                        NoiThucHienId = phongBenhVienId,
                        //NhanVienThucHienId = nhanVienId,
                        //BacSiThucHienId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                        ThoiDiemChiDinh = DateTime.Now,
                        SoLan = item.SoLuong ?? 0,
                        ThoiDiemDangKy = DateTime.Now,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,


                        DichVuKyThuatBenhVienId = dichVuKyThuat.Id,
                        NhomGiaDichVuKyThuatBenhVienId = item.LoaiGiaId,

                        NhomChiPhi = dichVuKyThuat.DichVuKyThuat?.NhomChiPhi ?? Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                        NhomDichVuBenhVienId = dichVuKyThuat.NhomDichVuBenhVienId,
                        LoaiDichVuKyThuat = GetLoaiDichVuKyThuat(dichVuKyThuat.NhomDichVuBenhVienId),
                        //
                        DonGiaBaoHiem = item.GiaBHYT != null ? (decimal)item.GiaBHYT : 0,
                        MucHuongBaoHiem = item.TiLeBaoHiemThanhToan,
                    };

                    if (item.IsGoiCoChietKhau)
                    {
                        var goiDichVuKyThuat = ycGoiDV.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(o =>
                            o.DichVuKyThuatBenhVienId == yeuCau.DichVuKyThuatBenhVienId &&
                            o.NhomGiaDichVuKyThuatBenhVienId == yeuCau.NhomGiaDichVuKyThuatBenhVienId);
                        if (goiDichVuKyThuat != null)
                        {
                            yeuCau.YeuCauGoiDichVu = ycGoiDV;
                            yeuCau.Gia = goiDichVuKyThuat.DonGia;
                            yeuCau.DonGiaTruocChietKhau = goiDichVuKyThuat.DonGiaTruocChietKhau;
                            yeuCau.DonGiaSauChietKhau = goiDichVuKyThuat.DonGiaSauChietKhau;
                        }
                    }

                    if (item.YeuCauGoiDichVuKhuyenMaiId != null && item.YeuCauGoiDichVuKhuyenMaiId != 0)
                    {
                        yeuCau.SoTienMienGiam = item.SoTienMienGiam;
                        yeuCau.MienGiamChiPhis.Add(new MienGiamChiPhi()
                        {
                            YeuCauTiepNhan = entity,
                            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                            SoTien = item.SoTienMienGiam ?? 0,
                            YeuCauGoiDichVuId = item.YeuCauGoiDichVuKhuyenMaiId
                        });
                    }

                    entity.YeuCauDichVuKyThuats.Add(yeuCau);
                }
                else if (item.Nhom.Equals(Constants.NhomDichVu.DichVuGiuong))
                {
                    var dichVuGiuong = await _dichVuGiuongBenhVienRepository.TableNoTracking.Include(p => p.DichVuGiuong).FirstOrDefaultAsync(p => p.Id == item.MaDichVuId);

                    var yeuCau = new YeuCauDichVuGiuongBenhVien
                    {
                        Ma = item.Ma,
                        MaTT37 = dichVuGiuong.DichVuGiuong?.MaTT37,
                        Ten = item.TenDichVu,
                        DuocHuongBaoHiem = item.DuocHuongBHYT,
                        BaoHiemChiTra = null,
                        Gia = (decimal)item.DonGia,
                        //GiaBaoHiemThanhToan = item.BHYTThanhToan,
                        NoiChiDinhId = locationId,
                        NoiThucHienId = phongBenhVienId,
                        //nhanvien = nhanVienId,
                        //BacSiThucHienId = nhanVienId,
                        NhanVienChiDinhId = currentUserId,
                        TrangThai = Enums.EnumTrangThaiGiuongBenh.ChuaThucHien,
                        ThoiDiemChiDinh = DateTime.Now,
                        LoaiGiuong = dichVuGiuong.LoaiGiuong,
                        MaGiuong = dichVuGiuong.Ma,
                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                        NhomGiaDichVuGiuongBenhVienId = item.LoaiGiaId,
                        DichVuGiuongBenhVienId = dichVuGiuong.Id,
                        //
                        DonGiaBaoHiem = item.GiaBHYT != null ? (decimal)item.GiaBHYT : 0,
                        MucHuongBaoHiem = item.TiLeBaoHiemThanhToan,

                    };

                    if (item.IsGoiCoChietKhau)
                    {
                        var goiDichVuGiuong = ycGoiDV.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(o =>
                            o.DichVuGiuongBenhVienId == yeuCau.DichVuGiuongBenhVienId &&
                            o.NhomGiaDichVuGiuongBenhVienId == yeuCau.NhomGiaDichVuGiuongBenhVienId);
                        if (goiDichVuGiuong != null)
                        {
                            yeuCau.YeuCauGoiDichVu = ycGoiDV;
                            yeuCau.Gia = goiDichVuGiuong.DonGia;
                            yeuCau.DonGiaTruocChietKhau = goiDichVuGiuong.DonGiaTruocChietKhau;
                            yeuCau.DonGiaSauChietKhau = goiDichVuGiuong.DonGiaSauChietKhau;
                        }
                    }
                    entity.YeuCauDichVuGiuongBenhViens.Add(yeuCau);
                }
            }

            //TODO: need update goi dv
            //if (lstGoiDichVu.Any())
            //{
            //    foreach (var item in lstGoiDichVu)
            //    {
            //        entity.YeuCauGoiDichVus.Add(item);
            //    }
            //}

            return entity;
        }

        private Enums.LoaiDichVuKyThuat GetLoaiDichVuKyThuat(long nhomDichVuBenhVienId)
        {
            return CalculateHelper.GetLoaiDichVuKyThuat(nhomDichVuBenhVienId, _nhomDichVuBenhVienRepository.TableNoTracking.ToList());
        }
        /*Move to ResourceHelper
        public string CreateMaYeuCauTiepNhan()
        {
            var now = DateTime.Now;

            var path = @"Resource\\YeuCauTiepNhan.xml";
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement yeuCauTiepNhanXML = data.Descendants(root + "YeuCauTiepNhan").FirstOrDefault();
            var maYeuCauTiepNhan = (string)yeuCauTiepNhanXML.Element(root + "MaYeuCauTiepNhan");
            var preMaYeuCauTiepNhan = (string)yeuCauTiepNhanXML.Element(root + "PreMaYeuCauTiepNhan");
            //Lấy ra ngày trong năm và format luôn luôn 3 chữ số
            var dayOfYearFormat = now.DayOfYear.ToString();
            switch (now.DayOfYear.ToString().Length)
            {
                case 1:
                    dayOfYearFormat = "00" + now.DayOfYear;
                    break;
                case 2:
                    dayOfYearFormat = "0" + now.DayOfYear;
                    break;
            }
            //Prefix mới của Mã YCTN
            var newPreMaYeuCauTiepNhan = now.Year+ dayOfYearFormat;
            //Tăng suffiex cũa Mã YCTN
            var newMaYeuCauTiepNhan = !string.IsNullOrEmpty(maYeuCauTiepNhan) ? Convert.ToInt32(maYeuCauTiepNhan) + 1 : 1;
            if (newPreMaYeuCauTiepNhan != preMaYeuCauTiepNhan)
            {
                newMaYeuCauTiepNhan = 1;
            }
            //Format suffiex cũa Mã YCTN luôn luôn 4 chữ số
            var maYeuCauTiepNhanFormat = newMaYeuCauTiepNhan.ToString();
            switch (newMaYeuCauTiepNhan.ToString().Length)
            {
                case 1:
                    maYeuCauTiepNhanFormat = "000" + newMaYeuCauTiepNhan;
                    break;
                case 2:
                    maYeuCauTiepNhanFormat = "00" + newMaYeuCauTiepNhan;
                    break;
                case 3:
                    maYeuCauTiepNhanFormat = "0" + newMaYeuCauTiepNhan;
                    break;
            }
            //Cập nhập vào file
            yeuCauTiepNhanXML.Element("MaYeuCauTiepNhan").Value = newMaYeuCauTiepNhan.ToString();
            yeuCauTiepNhanXML.Element("PreMaYeuCauTiepNhan").Value = newPreMaYeuCauTiepNhan;
            data.Save(path);
            return newPreMaYeuCauTiepNhan + maYeuCauTiepNhanFormat;
        }*/
        public string DisplayMaYeuCauTiepNhan(string maYeuCauTiepNhan)
        {
            return !string.IsNullOrEmpty(maYeuCauTiepNhan) && maYeuCauTiepNhan.Length > 7 ? maYeuCauTiepNhan.Substring(7, maYeuCauTiepNhan.Length - 7) : maYeuCauTiepNhan;
        }
        public string ConvertFullMaYeuCauTiepNhan(string maYeuCauTiepNhan)
        {
            var now = DateTime.Now;
            var dayOfYearFormat = now.DayOfYear.ToString();
            switch (now.DayOfYear.ToString().Length)
            {
                case 1:
                    dayOfYearFormat = "00" + now.DayOfYear;
                    break;
                case 2:
                    dayOfYearFormat = "0" + now.DayOfYear;
                    break;
            }
            var preMaYeuCauTiepNhan = now.Year + dayOfYearFormat;
            return preMaYeuCauTiepNhan + maYeuCauTiepNhan;
        }

        public async Task<List<MaDichVuTemplateVo>> GetDichVu(DropDownListRequestModel model)
        {
            var lstEntity = new List<MaDichVuTemplateVo>();

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {

                var lstDichVuKyThuatEntity = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Include(p => p.DichVuKyThuat)
                    .Where(p => p.HieuLuc 
                                && (nhomTiemChungId == null || p.NhomDichVuBenhVienId != nhomTiemChungId))
                    .Select(p => new MaDichVuTemplateVo
                    {
                        DisplayName = p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        Loai = "DVKT",
                        KeyId = p.Id,
                        TenTt43 = p.DichVuKyThuat.TenGia
                    })
                    .Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.Loai.Contains(model.Query ?? ""))
                    //.Where(p => p.Loai.Contains(model.Query ?? ""))
                    .Take(model.Take)
                    .ToListAsync();

                var lstDichVuKhamBenhEntity = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh)
                    .Where(p => p.HieuLuc)
                    .Select(p => new MaDichVuTemplateVo
                    {
                        DisplayName = p.Ten,
                        Ten = p.Ten,
                        Ma = p.Ma,
                        Loai = "DV Khám",
                        KeyId = p.Id,
                    })
                    .Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.Loai.Contains(model.Query ?? ""))
                    //.Where(p => p.Loai.Contains(model.Query ?? ""))
                    .Take(model.Take)
                    .ToListAsync();

                //Dong dich vu giuong va goi kham vao 21/8/2020
                //var lstDichVuGiuongBenhEntity = await _dichVuGiuongBenhVienRepository.TableNoTracking.Include(p => p.DichVuGiuong)
                //    .Where(p => p.HieuLuc)
                //    .Select(p => new MaDichVuTemplateVo
                //    {
                //        DisplayName = p.Ma + " - " + p.Ten,
                //        Ten = p.Ten,
                //        Ma = p.Ma,
                //        Loai = "DV Giường",
                //        KeyId = p.Id,
                //    })
                //    .Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.Loai.Contains(model.Query ?? ""))
                //    //.Where(p => p.Loai.Contains(model.Query ?? ""))
                //    .Take(model.Take)
                //    .ToListAsync();

                lstEntity.AddRange(lstDichVuKyThuatEntity);
                lstEntity.AddRange(lstDichVuKhamBenhEntity);
                //lstEntity.AddRange(lstDichVuGiuongBenhEntity);

                return lstEntity;
            }


            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add("Ten");
            lstColumnNameSearch.Add("Ma");

            var lstDichVuKhamBenhId = new List<long>();
            var lstDichVuKyThuatId = new List<long>();
            var lstDichVuGiuongId = new List<long>();

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var lstDichVuTiemChungId = new List<long>();
            if (nhomTiemChungId != null)
            {
                lstDichVuTiemChungId = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(x => x.NhomDichVuBenhVienId == nhomTiemChungId)
                    .Select(x => x.Id)
                    .ToListAsync();
            }


            //var lstDichVuBenhVienTongHop = await _dichVuBenhVienTongHopRepository
            //    .ApplyFulltext(model.Query, nameof(DichVuBenhVienTongHop), lstColumnNameSearch).Where(p => p.HieuLuc != false).Take(model.Take).ToListAsync();
            var lstDichVuBenhVienTongHop = await _dichVuBenhVienTongHopRepository
                .ApplyFulltext(model.Query, nameof(DichVuBenhVienTongHop), lstColumnNameSearch)
                .Where(p => p.HieuLuc != false && (p.DichVuKyThuatBenhVienId == null || !lstDichVuTiemChungId.Contains(p.DichVuKyThuatBenhVienId.Value)))
                .Take(model.Take).ToListAsync();

            var lstDVKBId = lstDichVuBenhVienTongHop.Where(p =>
                p.LoaiDichVuBenhVien == Enums.EnumDichVuTongHop.KhamBenh && p.HieuLuc).Select(p => p.DichVuKhamBenhBenhVienId).ToList();
            var lstDichVuKhamBenhBenhVien =
                _dichVuKhamBenhBenhVienRepository.TableNoTracking.Where(p => lstDVKBId.Any(m => m == p.Id)).ToList();

            var lstDVKTId = lstDichVuBenhVienTongHop.Where(p =>
                p.LoaiDichVuBenhVien == Enums.EnumDichVuTongHop.KyThuat && p.HieuLuc).Select(p => p.DichVuKyThuatBenhVienId).ToList();
            var lstDichVuKyThuatBenhVien =
                _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => lstDVKTId.Any(m => m == p.Id)).Include(o => o.DichVuKyThuat).ToList();

            var lstDVGBId = lstDichVuBenhVienTongHop.Where(p =>
                p.LoaiDichVuBenhVien == Enums.EnumDichVuTongHop.GiuongBenh && p.HieuLuc).Select(p => p.DichVuGiuongBenhVienId).ToList();
            var lstDichVuGiuongBenhBenhVien =
                _dichVuGiuongBenhVienRepository.TableNoTracking.Where(p => lstDVGBId.Any(m => m == p.Id)).ToList();

            foreach (var item in lstDichVuBenhVienTongHop)
            {
                switch (item.LoaiDichVuBenhVien)
                {
                    case Enums.EnumDichVuTongHop.KhamBenh:
                        //var dichVu = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                        //    .First(p => p.Id == item.DichVuKhamBenhBenhVienId && p.HieuLuc);
                        var dichVu = lstDichVuKhamBenhBenhVien.FirstOrDefault(p => p.Id == item.DichVuKhamBenhBenhVienId);
                        if (dichVu == null) break;
                        var maDichVu = new MaDichVuTemplateVo
                        {
                            DisplayName = dichVu.Ten,
                            Ten = dichVu.Ten,
                            Ma = dichVu.Ma,
                            Loai = "DV Khám",
                            KeyId = dichVu.Id,
                        };
                        lstEntity.Add(maDichVu);
                        break;
                    case Enums.EnumDichVuTongHop.KyThuat:
                        //var dichVu2 = _dichVuKyThuatBenhVienRepository.TableNoTracking
                        //    .First(p => p.Id == item.DichVuKyThuatBenhVienId && p.HieuLuc);
                        var dichVu2 = lstDichVuKyThuatBenhVien.FirstOrDefault(p => p.Id == item.DichVuKyThuatBenhVienId);
                        if (dichVu2 == null) break;
                        var maDichVu2 = new MaDichVuTemplateVo
                        {
                            DisplayName = dichVu2.Ten,
                            Ten = dichVu2.Ten,
                            Ma = dichVu2.Ma,
                            Loai = "DVKT",
                            KeyId = dichVu2.Id,
                            TenTt43 = dichVu2.DichVuKyThuat?.TenGia
                        };
                        lstEntity.Add(maDichVu2);
                        break;
                    case Enums.EnumDichVuTongHop.GiuongBenh:
                        //var dichVu3 = _dichVuGiuongBenhVienRepository.TableNoTracking
                        //    .First(p => p.Id == item.DichVuGiuongBenhVienId && p.HieuLuc);
                        //var dichVu3 = lstDichVuGiuongBenhBenhVien.FirstOrDefault(p => p.Id == item.DichVuGiuongBenhVienId);
                        //if (dichVu3 == null) break;
                        //var maDichVu3 = new MaDichVuTemplateVo
                        //{
                        //    DisplayName = dichVu3.Ma + " - " + dichVu3.Ten,
                        //    Ten = dichVu3.Ten,
                        //    Ma = dichVu3.Ma,
                        //    Loai = "DV Giường",
                        //    KeyId = dichVu3.Id,
                        //};
                        //lstEntity.Add(maDichVu3);
                        break;
                    default:
                        break;
                }
            }

            //if (lstDichVuBenhVienTongHop.Any(p => p.DichVuKhamBenhBenhVienId != null))
            //{
            //    foreach (var item in lstDichVuBenhVienTongHop)
            //    {
            //        if (item.DichVuKhamBenhBenhVienId != null && item.DichVuKhamBenhBenhVienId != 0)
            //        {
            //            lstDichVuKhamBenhId.Add(item.DichVuKhamBenhBenhVienId ?? 0);
            //        }
            //    }
            //}

            //if (lstDichVuBenhVienTongHop.Any(p => p.DichVuKyThuatBenhVienId != null))
            //{
            //    foreach (var item in lstDichVuBenhVienTongHop)
            //    {
            //        if (item.DichVuKyThuatBenhVienId != null && item.DichVuKyThuatBenhVienId != 0)
            //        {
            //            lstDichVuKyThuatId.Add(item.DichVuKyThuatBenhVienId ?? 0);
            //        }
            //    }
            //}

            //if (lstDichVuBenhVienTongHop.Any(p => p.DichVuGiuongBenhVienId != null))
            //{
            //    foreach (var item in lstDichVuBenhVienTongHop)
            //    {
            //        if (item.DichVuGiuongBenhVienId != null && item.DichVuGiuongBenhVienId != 0)
            //        {
            //            lstDichVuGiuongId.Add(item.DichVuGiuongBenhVienId ?? 0);
            //        }
            //    }
            //}

            //#region dvkt

            //var lstDichVuKyThuatEntity1 = await _dichVuKyThuatBenhVienRepository.ApplyFulltext(model.Query, "DichVuKyThuatBenhVien", lstColumnNameSearch).Include(p => p.DichVuKyThuat)
            //    .Where(p => p.HieuLuc)
            //    .Select(p => new MaDichVuTemplateVo
            //    {
            //        DisplayName = p.Ma + " - " + p.Ten,
            //        Ten = p.Ten,
            //        Ma = p.Ma,
            //        Loai = "DVKT",
            //        KeyId = p.Id,
            //    })
            //    .Take(model.Take)
            //    .ToListAsync();

            //var lstDichVuKyThuatEntity2 = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Include(p => p.DichVuKyThuat)
            //    .Where(p => p.HieuLuc)
            //    .Select(p => new MaDichVuTemplateVo
            //    {
            //        DisplayName = p.Ma + " - " + p.Ten,
            //        Ten = p.Ten,
            //        Ma = p.Ma,
            //        Loai = "DVKT",
            //        KeyId = p.Id,
            //    })
            //    //.Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.Loai.Contains(model.Query ?? ""))
            //    .Where(p => p.Loai.Contains(model.Query ?? ""))
            //    .Take(model.Take)
            //    .ToListAsync();

            //var lstDichVuKyThuatEntity = lstDichVuKyThuatEntity1.Concat(lstDichVuKyThuatEntity2).ToList();

            //#endregion dvkt

            //#region dvkb

            //var lstDichVuKhamBenhEntity1 = await _dichVuKhamBenhBenhVienRepository.ApplyFulltext(model.Query, "DichVuKhamBenhBenhVien", lstColumnNameSearch).Include(p => p.DichVuKhamBenh)
            //    .Where(p => p.HieuLuc)
            //    .Select(p => new MaDichVuTemplateVo
            //    {
            //        DisplayName = p.Ma + " - " + p.Ten,
            //        Ten = p.Ten,
            //        Ma = p.Ma,
            //        Loai = "DV Khám",
            //        KeyId = p.Id,
            //    })
            //    .Take(model.Take)
            //    .ToListAsync();

            //var lstDichVuKhamBenhEntity2 = await _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh)
            //    .Where(p => p.HieuLuc)
            //    .Select(p => new MaDichVuTemplateVo
            //    {
            //        DisplayName = p.Ma + " - " + p.Ten,
            //        Ten = p.Ten,
            //        Ma = p.Ma,
            //        Loai = "DV Khám",
            //        KeyId = p.Id,
            //    })
            //    //.Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.Loai.Contains(model.Query ?? ""))
            //    .Where(p => p.Loai.Contains(model.Query ?? ""))
            //    .Take(model.Take)
            //    .ToListAsync();

            //var lstDichVuKhamBenhEntity = lstDichVuKhamBenhEntity1.Concat(lstDichVuKhamBenhEntity2).ToList();

            //#endregion dvkb

            //#region dvg

            //var lstDichVuGiuongBenhEntity1 = await _dichVuGiuongBenhVienRepository.ApplyFulltext(model.Query, "DichVuGiuongBenhVien", lstColumnNameSearch).Include(p => p.DichVuGiuong)
            //    .Where(p => p.HieuLuc)
            //    .Select(p => new MaDichVuTemplateVo
            //    {
            //        DisplayName = p.Ma + " - " + p.Ten,
            //        Ten = p.Ten,
            //        Ma = p.Ma,
            //        Loai = "DV Giường",
            //        KeyId = p.Id,
            //    })
            //    .Take(model.Take)
            //    .ToListAsync();

            //var lstDichVuGiuongBenhEntity2 = await _dichVuGiuongBenhVienRepository.TableNoTracking.Include(p => p.DichVuGiuong)
            //    .Where(p => p.HieuLuc)
            //    .Select(p => new MaDichVuTemplateVo
            //    {
            //        DisplayName = p.Ma + " - " + p.Ten,
            //        Ten = p.Ten,
            //        Ma = p.Ma,
            //        Loai = "DV Giường",
            //        KeyId = p.Id,
            //    })
            //    //.Where(p => p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.Loai.Contains(model.Query ?? ""))
            //    .Where(p => p.Loai.Contains(model.Query ?? ""))
            //    .Take(model.Take)
            //    .ToListAsync();

            //var lstDichVuGiuongBenhEntity = lstDichVuGiuongBenhEntity1.Concat(lstDichVuGiuongBenhEntity2);

            //#endregion dvg



            //var lstEntity = new List<MaDichVuTemplateVo>();

            //lstEntity.AddRange(lstDichVuKyThuatEntity);
            //lstEntity.AddRange(lstDichVuKhamBenhEntity);
            //lstEntity.AddRange(lstDichVuGiuongBenhEntity);

            return lstEntity;
        }

        public async Task<List<GoiDichVuTemplateVo>> GetGoiKhamTongHop(DropDownListRequestModel model)
        {
            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add("Ten");
            //update goi dv 10/21
            var lstEntity = (_goiDichVuRepository.ApplyFulltext(model.Query, "GoiDichVu", lstColumnNameSearch))
                                            .Where(p => //p.NgayBatDau.Date <= DateTime.Now.Date &&
                                             p.LoaiGoiDichVu == Enums.EnumLoaiGoiDichVu.Marketing
                                             //&& (p.NgayKetThuc == null ||
                                             //    (p.NgayKetThuc != null && (p.NgayKetThuc ?? DateTime.Now).Date >= DateTime.Now.Date)
                                             //) && p.IsDisabled != true
                                             && p.IsDisabled != true);


            var query = lstEntity.Select(item => new GoiDichVuTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                //ChietKhau = item.CoChietKhau ? "Có" : "Không",
                //IsCoChietKhau = item.CoChietKhau,
                Ten = item.Ten
            }).Take(model.Take).ToList();

            var query2 = _goiDichVuRepository.TableNoTracking
                    .Where(p => /*p.NgayBatDau.Date <= DateTime.Now.Date &&*/
                                p.LoaiGoiDichVu == Enums.EnumLoaiGoiDichVu.Marketing
                                //&& (p.NgayKetThuc == null ||
                                //    (p.NgayKetThuc != null && (p.NgayKetThuc ?? DateTime.Now).Date >= DateTime.Now.Date)
                                //) && p.IsDisabled != true
                                && p.IsDisabled != true).Select(item => new GoiDichVuTemplateVo
                                {
                                    DisplayName = item.Ten,
                                    KeyId = item.Id,
                                    //ChietKhau = item.CoChietKhau ? "Có" : "Không",
                                    //IsCoChietKhau = item.CoChietKhau,
                                    Ten = item.Ten
                                })
                .Where(p => p.ChietKhau.Contains(model.Query ?? "") && !query.Any(x => x.KeyId == p.KeyId))
                .Take(model.Take).ToList();

            var query3 = query.Concat(query2);

            return query3.ToList();
        }

        public async Task<int?> GetTyLeMienGiamKhamBenh(long doiTuongId, long maDichVuId)
        {
            var doiTuongUuDaiDichVuKhamBenhBenhVien =
                await _doiTuongUuDaiDichVuKhamBenhBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                    p.DoiTuongUuDaiId == doiTuongId
                    && p.DichVuKhamBenhBenhVienId == maDichVuId);
            return doiTuongUuDaiDichVuKhamBenhBenhVien?.TiLeUuDai;
        }

        public async Task<int?> GetTyLeMienGiamKyThuat(long doiTuongId, long maDichVuId)
        {
            var doiTuongUuDaiDichVuKyThuatBenhVien =
                await _doiTuongUuDaiDichVuKyThuatBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                    p.DoiTuongUuDaiId == doiTuongId
                    && p.DichVuKyThuatBenhVienId == maDichVuId);
            return doiTuongUuDaiDichVuKyThuatBenhVien?.TiLeUuDai;
        }

        public async Task<string> GetTenCongTyBaoHiemTuNhan(long congTyBaoHiemTuNhanId)
        {
            var result = (await _congTyBaoHiemTuNhanRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                p.Id == congTyBaoHiemTuNhanId))?.Ten;

            return result;
        }

        public async Task<YeuCauTiepNhan> GetByIdHaveIncludeSimplify(long id)
        {
            var result = BaseRepository.GetById(id, s => s.Include(u => u.BenhNhan)
                .Include(u => u.BenhNhan).ThenInclude(u => u.DanToc)
                .Include(u => u.BenhNhan).ThenInclude(u => u.NgheNghiep)
                .Include(u => u.BenhNhan).ThenInclude(u => u.QuocTich)
                .Include(u => u.BenhNhan).ThenInclude(u => u.TinhThanh)
                .Include(u => u.BenhNhan).ThenInclude(u => u.QuanHuyen)
                .Include(u => u.BenhNhan).ThenInclude(u => u.PhuongXa)
                .Include(u => u.BenhNhan).ThenInclude(u => u.NguoiLienHeQuanHeNhanThan)
                .Include(u => u.BenhNhan).ThenInclude(u => u.BenhNhanCongTyBaoHiemTuNhans)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuGiuongs)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichKhamBenhs)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuKyThuats)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuGiuongs)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichKhamBenhs)
                //.Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Include(u => u.NguoiGioiThieu)
                .Include(u => u.NoiGioiThieu)

                .Include(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                .Include(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans).ThenInclude(u => u.CongTyBaoHiemTuNhan)
                //TODO: need update goi dv
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(u => u.YeuCauGoiDichVu).ThenInclude(u => u.ChuongTrinhGoiDichVu)
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.YeuCauGoiDichVu).ThenInclude(u => u.ChuongTrinhGoiDichVu)
                //.Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(u => u.YeuCauGoiDichVu).ThenInclude(u => u.ChuongTrinhGoiDichVu)

                //.Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuong)
                //.Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                .Include(u => u.YeuCauDuocPhamBenhViens)
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                .Include(u => u.YeuCauVatTuBenhViens)
                .Include(u => u.DonThuocThanhToans).ThenInclude(p => p.DonThuocThanhToanChiTiets)

                .Include(u => u.DoiTuongUuTienKhamChuaBenh)
                .Include(u => u.GiayChuyenVien)
                .Include(u => u.BHYTGiayMienCungChiTra)

                .Include(u => u.HoSoYeuCauTiepNhans)
                .Include(u => u.HoSoYeuCauTiepNhans).ThenInclude(u => u.LoaiHoSoYeuCauTiepNhan)

                .Include(u => u.TheVoucherYeuCauTiepNhans).ThenInclude(u => u.TheVoucher).ThenInclude(u => u.Voucher)

                .Include(u => u.YeuCauTiepNhanLichSuKhamBHYT)
                .Include(u => u.YeuCauTiepNhanLichSuKiemTraTheBHYTs)

                //.Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauVatTuBenhViens).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauDuocPhamBenhViens).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)


                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(c => c.User)
                //update MG
                
                //.Include(u => u.YeuCauKhamBenhs)
                //.Include(u => u.YeuCauDichVuKyThuats)
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.YeuCauDichVuKyThuatKhamSangLocTiemChung)
                //.Include(u => u.TaiKhoanBenhNhanChis)
                //.Include(u => u.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                //.Include(u => u.MienGiamChiPhis).ThenInclude(p => p.TaiKhoanBenhNhanThu)

                // cập nhật gói khám
                //.Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                //.Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                //.Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                

                //BVHD-3825
                //.Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.MienGiamChiPhis).ThenInclude(u => u.TaiKhoanBenhNhanThu)
                //.Include(u => u.YeuCauDichVuKyThuats).ThenInclude(u => u.MienGiamChiPhis).ThenInclude(u => u.TaiKhoanBenhNhanThu)
            );
            return result;
        }

        public async Task<YeuCauTiepNhan> GetByIdHaveInclude(long id)
        {
            var result = BaseRepository.GetById(id, s => s.Include(u => u.BenhNhan)
                .Include(u => u.BenhNhan).ThenInclude(u => u.DanToc)
                .Include(u => u.BenhNhan).ThenInclude(u => u.NgheNghiep)
                .Include(u => u.BenhNhan).ThenInclude(u => u.QuocTich)
                .Include(u => u.BenhNhan).ThenInclude(u => u.TinhThanh)
                .Include(u => u.BenhNhan).ThenInclude(u => u.QuanHuyen)
                .Include(u => u.BenhNhan).ThenInclude(u => u.PhuongXa)
                .Include(u => u.BenhNhan).ThenInclude(u => u.NguoiLienHeQuanHeNhanThan)
                .Include(u => u.BenhNhan).ThenInclude(u => u.BenhNhanCongTyBaoHiemTuNhans)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                .Include(u => u.NguoiGioiThieu)
                .Include(u => u.NoiGioiThieu)

                .Include(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                .Include(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans).ThenInclude(u => u.CongTyBaoHiemTuNhan)
                //TODO: need update goi dv
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(u => u.YeuCauGoiDichVu).ThenInclude(u => u.ChuongTrinhGoiDichVu)
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.YeuCauGoiDichVu).ThenInclude(u => u.ChuongTrinhGoiDichVu)
                .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(u => u.YeuCauGoiDichVu).ThenInclude(u => u.ChuongTrinhGoiDichVu)
                //.Include(u => u.YeuCauGoiDichVus).ThenInclude(u => u.YeuCauDuocPhamBenhViens)
                //.Include(u => u.YeuCauGoiDichVus).ThenInclude(u => u.YeuCauVatTuBenhViens)
                //.Include(u => u.YeuCauGoiDichVus).ThenInclude(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                //.Include(u => u.YeuCauGoiDichVus).ThenInclude(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                //.Include(u => u.YeuCauGoiDichVus).ThenInclude(u => u.YeuCauKhamBenh).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.DichVuGiuongBenhVien).ThenInclude(p => p.DichVuGiuong)
                .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                .Include(u => u.YeuCauDuocPhamBenhViens)
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                .Include(u => u.YeuCauVatTuBenhViens)
                .Include(u => u.DonThuocThanhToans).ThenInclude(p => p.DonThuocThanhToanChiTiets)

                .Include(u => u.DoiTuongUuTienKhamChuaBenh)
                .Include(u => u.GiayChuyenVien)
                .Include(u => u.BHYTGiayMienCungChiTra)

                .Include(u => u.HoSoYeuCauTiepNhans)
                .Include(u => u.HoSoYeuCauTiepNhans).ThenInclude(u => u.LoaiHoSoYeuCauTiepNhan)

                .Include(u => u.TheVoucherYeuCauTiepNhans).ThenInclude(u => u.TheVoucher).ThenInclude(u => u.Voucher)

                .Include(u => u.YeuCauTiepNhanLichSuKhamBHYT)
                .Include(u => u.YeuCauTiepNhanLichSuKiemTraTheBHYTs)

                //.Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauVatTuBenhViens).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauDuocPhamBenhViens).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)


                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(c => c.User)
                //update MG
                .Include(u => u.MienGiamChiPhis)

                .Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.TaiKhoanBenhNhanChis)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.TaiKhoanBenhNhanChis)
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.YeuCauDichVuKyThuatKhamSangLocTiemChung)


                // cập nhật gói khám
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuGiuongs)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuGiuongs)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Include(u => u.BenhNhan).ThenInclude(u => u.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuKyThuats)

                //BVHD-3825
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.MienGiamChiPhis).ThenInclude(u => u.TaiKhoanBenhNhanThu)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(u => u.MienGiamChiPhis).ThenInclude(u => u.TaiKhoanBenhNhanThu)
            );
            return result;
        }

        public async Task<YeuCauTiepNhan> GetByIdHaveIncludeForUpdate(long id)
        {
            var result = await BaseRepository.GetByIdAsync(id, s => s.Include(u => u.BenhNhan)
                .Include(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                .Include(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans).ThenInclude(u => u.CongTyBaoHiemTuNhan)
                .Include(u => u.DoiTuongUuTienKhamChuaBenh)
                .Include(u => u.GiayChuyenVien)
                .Include(u => u.BHYTGiayMienCungChiTra)
                .Include(u => u.HoSoYeuCauTiepNhans)

                .Include(u => u.TheVoucherYeuCauTiepNhans).ThenInclude(u => u.TheVoucher).ThenInclude(u => u.Voucher)

                .Include(u => u.YeuCauTiepNhanLichSuKhamBHYT)
                .Include(u => u.YeuCauTiepNhanLichSuKiemTraTheBHYTs)
            );
            return result;
        }

        //public async Task<List<ChiDinhDichVuGridVo>> SetGiaForGrid(List<ChiDinhDichVuGridVo> lstGrid, int? bhytMucHuong)
        //{
        //    foreach (var item in lstGrid)
        //    {
        //        if (!item.IsGoiCoChietKhau)
        //        {
        //            //KCB
        //            if (item.Nhom == Constants.NhomDichVu.DichVuKhamBenh && item.DuocHuongBHYT)
        //            {
        //                var dichVuKhamBenhBenhVienGiaBaoHiem =
        //                        await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuKhamBenhBenhVienId == item.MaDichVuId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

        //                var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                        (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //                //* (bhytMucHuong ?? 100) / 100;
        //                var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                                      (dichVuKhamBenhBenhVienGiaBaoHiem
        //                                                           ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //                item.ThanhTien = item.DonGia * item.SoLuong ?? 0;
        //                item.ThanhTienDisplay = item.ThanhTien.ApplyNumber();

        //                item.BHYTThanhToan = bhytThanhToan;
        //                item.BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber();
        //                item.BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong;
        //                item.BnThanhToan = item.ThanhTien - (double)item.BHYTThanhToan;
        //                item.BnThanhToanDisplay = item.BnThanhToan.ApplyNumber();
        //                item.GiaBHYT = (double)(dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0);
        //                item.TiLeBaoHiemThanhToan = dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan;
        //            }
        //            //DVKT
        //            if (item.Nhom == Constants.NhomDichVu.DichVuKyThuat && item.DuocHuongBHYT)
        //            {
        //                var dichVuKyThuatBenhVienGiaBaoHiem =
        //                        await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuKyThuatBenhVienId == item.MaDichVuId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

        //                var bhytThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                        (dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //                //* (bhytMucHuong ?? 100) / 100;
        //                var bhytThanhToanChuaBaoGomMucHuong = (dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                                      (dichVuKyThuatBenhVienGiaBaoHiem
        //                                                           ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //                item.ThanhTien = item.DonGia * item.SoLuong ?? 0;
        //                item.ThanhTienDisplay = item.ThanhTien.ApplyNumber();

        //                item.BHYTThanhToan = bhytThanhToan;
        //                item.BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber();
        //                item.BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong;
        //                item.BnThanhToan = item.ThanhTien - (double)item.BHYTThanhToan;
        //                item.BnThanhToanDisplay = item.BnThanhToan.ApplyNumber();
        //                item.GiaBHYT = (double)(dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0);
        //                item.TiLeBaoHiemThanhToan = dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan;
        //            }
        //            //DV giuong
        //            if (item.Nhom == Constants.NhomDichVu.DichVuGiuong && item.DuocHuongBHYT)
        //            {
        //                var dichVuGiuongBenhVienGiaBaoHiem = await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuGiuongBenhVienId == item.MaDichVuId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

        //                var bhytThanhToan = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                        (dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //                //* (bhytMucHuong ?? 100) / 100;
        //                var bhytThanhToanChuaBaoGomMucHuong = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                                      (dichVuGiuongBenhVienGiaBaoHiem
        //                                                           ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //                item.ThanhTien = item.DonGia * item.SoLuong ?? 0;
        //                item.ThanhTienDisplay = item.ThanhTien.ApplyNumber();

        //                item.BHYTThanhToan = bhytThanhToan;
        //                item.BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber();
        //                item.BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong;
        //                item.BnThanhToan = item.ThanhTien - (double)item.BHYTThanhToan;
        //                item.BnThanhToanDisplay = item.BnThanhToan.ApplyNumber();
        //                item.GiaBHYT = (double)(dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0);
        //                item.TiLeBaoHiemThanhToan = dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan;
        //            }
        //            //DV vat tu
        //            if (item.Nhom == Constants.NhomDichVu.VatTuTieuHao && item.DuocHuongBHYT)
        //            {
        //                //var dichVuVatTuTieuHaoGiaBaoHiem 
        //            }
        //            //DV duoc pham
        //            //if (item.Nhom == Constants.NhomDichVu.DuocPham && item.DuocHuongBHYT)
        //            //{
        //            //    var dichVuDuocPhamGiaBaoHiem = await _duocPhamBenhVienGiaBaoHiemRepository.TableNoTracking
        //            //                .FirstOrDefaultAsync(p =>
        //            //                    p.DuocPhamBenhVienId == item.MaDichVuId
        //            //                    && p.TuNgay.Date <= DateTime.Now.Date
        //            //                    && (p.DenNgay == null ||
        //            //                        (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

        //            //    var bhytThanhToan = (dichVuDuocPhamGiaBaoHiem?.Gia ?? 0) *
        //            //                            (dichVuDuocPhamGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //            //    //* (bhytMucHuong ?? 100) / 100;
        //            //    var bhytThanhToanChuaBaoGomMucHuong = (dichVuDuocPhamGiaBaoHiem?.Gia ?? 0) *
        //            //                                          (dichVuDuocPhamGiaBaoHiem
        //            //                                               ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //            //    item.ThanhTien = item.DonGia * item.SoLuong ?? 0;
        //            //    item.ThanhTienDisplay = item.ThanhTien.ApplyNumber();

        //            //    item.BHYTThanhToan = bhytThanhToan;
        //            //    item.BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber();
        //            //    item.BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong;
        //            //    item.BnThanhToan = item.ThanhTien - (double)item.BHYTThanhToan;
        //            //    item.BnThanhToanDisplay = item.BnThanhToan.ApplyNumber();
        //            //    item.GiaBHYT = (double)(dichVuDuocPhamGiaBaoHiem?.Gia ?? 0);
        //            //    item.TiLeBaoHiemThanhToan = dichVuDuocPhamGiaBaoHiem?.TiLeBaoHiemThanhToan;
        //            //}

        //        }
        //    }

        //    var tongBHYTThanhToan = lstGrid.Where(p => !p.IsGoiCoChietKhau).Select(p => p.BHYTThanhToan).Sum();
        //    var soTienBHYTSeThanhToan = await _cauHinhService.SoTienBHYTSeThanhToanToanBo();

        //    var lstSoLanKham = await _cauHinhService.GetTyLeHuongBaoHiem5LanKhamDichVuBHYT();

        //    int soLanKham = 1;

        //    foreach (var item in lstGrid)
        //    {
        //        if (!item.IsGoiCoChietKhau && item.DuocHuongBHYT)
        //        {
        //            if (item.Nhom == Constants.NhomDichVu.DichVuKhamBenh)
        //            {
        //                item.BHYTThanhToan = await TinhTienBHYTVoiSoLanVaMucLuongCoBan(item.BHYTThanhToanChuaBaoGomMucHuong
        //                    , soLanKham, tongBHYTThanhToan, soTienBHYTSeThanhToan, bhytMucHuong ?? 100, lstSoLanKham, null, lstGrid);
        //                item.BHYTThanhToanDisplay = item.BHYTThanhToan.ApplyVietnameseFloatNumber();
        //                soLanKham = soLanKham + 1;

        //                item.BnThanhToan = item.ThanhTien - (double)item.BHYTThanhToan;
        //                item.BnThanhToanDisplay = item.BnThanhToan.ApplyNumber();
        //            }
        //            else
        //            {
        //                item.BHYTThanhToan = await TinhTienBHYTVoiSoLanVaMucLuongCoBan(item.BHYTThanhToanChuaBaoGomMucHuong
        //                    , 1, tongBHYTThanhToan, soTienBHYTSeThanhToan, bhytMucHuong ?? 100, lstSoLanKham, null, lstGrid);
        //                item.BHYTThanhToanDisplay = item.BHYTThanhToan.ApplyVietnameseFloatNumber();

        //                item.BnThanhToan = item.ThanhTien - (double)item.BHYTThanhToan;
        //                item.BnThanhToanDisplay = item.BnThanhToan.ApplyNumber();
        //            }
        //        }
        //    }

        //    return lstGrid;
        //}

        public Task<List<ChiDinhDichVuGridVo>> ConvertDichVuToGridVo(YeuCauTiepNhan entity)
        {
            throw new NotImplementedException();
        }



        public async Task<YeuCauTiepNhan> HuyBHYTChiDinhDichVu(List<ListChiDinhNeedUpdate> lstChiDinhDichVu, long yeuCauTiepNhanId, int? mucHuong)
        {
            var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);

            //var lstDichVuOld = new List<DichVuOld>();
            //lstDichVuOld = GetListDichVuOld(entity);

            foreach (var dichVu in lstChiDinhDichVu)
            {
                if (dichVu.Nhom.Equals(Constants.NhomDichVu.DichVuKhamBenh))
                {
                    var item = entity.YeuCauKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == dichVu.DichVuId);
                    if (item == null) continue;

                    item.DuocHuongBaoHiem = false;
                    //item.GiaBaoHiemThanhToan = 0;

                }
                else if (dichVu.Nhom.Equals(Constants.NhomDichVu.DichVuKyThuat))
                {
                    var item = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == dichVu.DichVuId && p.YeuCauKhamBenhId != null);
                    if (item == null) continue;

                    item.DuocHuongBaoHiem = false;
                    //item.GiaBaoHiemThanhToan = 0;
                }
                else if (dichVu.Nhom.Equals(Constants.NhomDichVu.DichVuGiuong))
                {
                    var item = entity.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.DichVuGiuongBenhVienId == dichVu.DichVuId && p.YeuCauKhamBenhId != null);
                    if (item == null) continue;

                    item.DuocHuongBaoHiem = false;
                    //item.GiaBaoHiemThanhToan = 0;
                }
                else if (dichVu.Nhom.Equals(Constants.NhomDichVu.DuocPham))
                {
                    var item = entity.YeuCauDuocPhamBenhViens.FirstOrDefault(p => p.DuocPhamBenhVienId == dichVu.DichVuId && p.YeuCauKhamBenhId != null);
                    if (item == null) continue;

                    item.DuocHuongBaoHiem = false;
                    //item.GiaBaoHiemThanhToan = 0;
                }
                else if (dichVu.Nhom.Equals(Constants.NhomDichVu.VatTuTieuHao))
                {

                }
            }

            // Set default BHYT thanh toan and set new value
            //entity = await SetValueDefaultAndSetValueBHYTWithNewLogic(entity, mucHuong ?? 0);

            //Set trang thai thanh toan cho thu ngan
            //entity = SetTrangThaiThanhToanForThuNganAgain(entity, lstDichVuOld);

            //await BaseRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<YeuCauTiepNhan> SetMucHuongChoDichVu(long yeuCauTiepNhanId, int mucHuong)
        {
            var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);

            //var lstDichVuOld = new List<DichVuOld>();
            //lstDichVuOld = GetListDichVuOld(entity);

            //Set default BHYT thanh toan and set new value
            //entity = await SetValueDefaultAndSetValueBHYTWithNewLogic(entity, mucHuong);

            //Set trang thai thanh toan cho thu ngan
            //entity = SetTrangThaiThanhToanForThuNganAgain(entity, lstDichVuOld);

            //await BaseRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<YeuCauTiepNhan> RemoveListDichVuToServer(List<DichVuNeedUpdate> lstDichVu, long yeuCauTiepNhanId, bool coBHYT, DateTime ngayBatDau, DateTime ngayHetHan, Enums.EnumLyDoVaoVien lyDoVaoVien)
        {
            var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);

            foreach (var item in lstDichVu)
            {
                if (item.nhom.Equals(Constants.NhomDichVu.DichVuKhamBenh))
                {

                }
                else if (item.nhom.Equals(Constants.NhomDichVu.DichVuKyThuat))
                {

                }
                else if (item.nhom.Equals(Constants.NhomDichVu.DichVuGiuong))
                {

                }
                else if (item.nhom.Equals(Constants.NhomDichVu.DuocPham))
                {

                }
                else if (item.nhom.Equals(Constants.NhomDichVu.VatTuTieuHao))
                {

                }
            }



            return entity;
        }

        public async Task<YeuCauTiepNhan> AddListDichVuToServer(List<DichVuNeedUpdate> lstDichVu, long yeuCauTiepNhanId, bool coBHYT, DateTime ngayBatDau, DateTime ngayHetHan, Enums.EnumLyDoVaoVien lyDoVaoVien)
        {
            var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);

            foreach (var item in lstDichVu)
            {
                if (item.nhom.Equals(Constants.NhomDichVu.DichVuKhamBenh))
                {
                    if (!(await IsUpdateBHYTForDichVu(item.id, item.nhom, coBHYT, ngayBatDau, ngayHetHan, lyDoVaoVien))) continue;

                    var dv = entity.YeuCauKhamBenhs.FirstOrDefault(p => p.Id == item.id);
                    if (dv == null) return entity;

                }
                else if (item.nhom.Equals(Constants.NhomDichVu.DichVuKyThuat))
                {

                }
                else if (item.nhom.Equals(Constants.NhomDichVu.DichVuGiuong))
                {

                }
                else if (item.nhom.Equals(Constants.NhomDichVu.DuocPham))
                {

                }
                else if (item.nhom.Equals(Constants.NhomDichVu.VatTuTieuHao))
                {

                }
            }

            return entity;
        }

        private async Task<bool> IsUpdateBHYTForDichVu(long id, string nhom, bool coBHYT, DateTime ngayBatDau, DateTime ngayHetHan, Enums.EnumLyDoVaoVien lyDoVaoVien)
        {
            var result = true;

            if (nhom.Equals(Constants.NhomDichVu.DichVuKhamBenh))
            {
                var dv = await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                                .FirstOrDefaultAsync(p =>
                                    p.DichVuKhamBenhBenhVienId == id
                                    && p.TuNgay.Date <= DateTime.Now.Date
                                    && (p.DenNgay == null ||
                                        (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                if (dv == null || (dv != null && dv.Gia == 0)
                    || ngayBatDau.Date > DateTime.Now.Date || ngayHetHan.Date < DateTime.Now.Date || lyDoVaoVien != Enums.EnumLyDoVaoVien.DungTuyen || coBHYT != true)
                    return false;
            }
            else if (nhom.Equals(Constants.NhomDichVu.DichVuKyThuat))
            {
                var dv = await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                                .FirstOrDefaultAsync(p =>
                                    p.DichVuKyThuatBenhVienId == id
                                    && p.TuNgay.Date <= DateTime.Now.Date
                                    && (p.DenNgay == null ||
                                        (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                if (dv == null || (dv != null && dv.Gia == 0)
                    || ngayBatDau.Date > DateTime.Now.Date || ngayHetHan.Date < DateTime.Now.Date || lyDoVaoVien != Enums.EnumLyDoVaoVien.DungTuyen || coBHYT != true)
                    return false;
            }
            else if (nhom.Equals(Constants.NhomDichVu.DichVuGiuong))
            {
                var dv = await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
                                .FirstOrDefaultAsync(p =>
                                    p.DichVuGiuongBenhVienId == id
                                    && p.TuNgay.Date <= DateTime.Now.Date
                                    && (p.DenNgay == null ||
                                        (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                if (dv == null || (dv != null && dv.Gia == 0)
                    || ngayBatDau.Date > DateTime.Now.Date || ngayHetHan.Date < DateTime.Now.Date || lyDoVaoVien != Enums.EnumLyDoVaoVien.DungTuyen || coBHYT != true)
                    return false;
            }
            else if (nhom.Equals(Constants.NhomDichVu.DuocPham))
            {
                //var dv = await _duocPhamBenhVienGiaBaoHiemRepository.TableNoTracking
                //                .FirstOrDefaultAsync(p =>
                //                    p.DuocPhamBenhVienId == id
                //                    && p.TuNgay.Date <= DateTime.Now.Date
                //                    && (p.DenNgay == null ||
                //                        (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                //if (dv == null || (dv != null && dv.Gia == 0)
                //    || ngayBatDau.Date > DateTime.Now.Date || ngayHetHan.Date < DateTime.Now.Date || lyDoVaoVien != Enums.EnumLyDoVaoVien.DungTuyen || coBHYT != true)
                return false;
            }
            else if (nhom.Equals(Constants.NhomDichVu.VatTuTieuHao))
            {
                return false;
            }

            return result;
        }

        public async Task<YeuCauTiepNhan> CheckOrUncheckBHYTForDichVu(bool isChecked, long id, string nhom, long yeuCauTiepNhanId, int mucHuong)
        {
            var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);


            //var lstDichVuOld = new List<DichVuOld>();
            //lstDichVuOld = GetListDichVuOld(entity);

            if (nhom.Equals(Constants.NhomDichVu.DichVuKhamBenh))
            {
                var item = entity.YeuCauKhamBenhs.FirstOrDefault(p => p.Id == id);
                if (item == null) return entity;
                //var dichVuKhamBenhBenhVienGiaBaoHiem =
                //                await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                //                    .FirstOrDefaultAsync(p =>
                //                        p.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId
                //                        && p.TuNgay.Date <= DateTime.Now.Date
                //                        && (p.DenNgay == null ||
                //                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                //var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                //                                (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
                ////* (bhytMucHuong ?? 100) / 100;
                //var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                //                                      (dichVuKhamBenhBenhVienGiaBaoHiem
                //                                           ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;


                item.DuocHuongBaoHiem = isChecked;
                //if (isChecked)
                //{
                //    item.GiaBaoHiemThanhToan = bhytThanhToan;
                //}
                //else
                //{
                //    item.GiaBaoHiemThanhToan = 0;
                //}

            }
            else if (nhom.Equals(Constants.NhomDichVu.DichVuKyThuat))
            {
                var item = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == id);
                if (item == null) return entity;

                //var dichVuKyThuatBenhVienGiaBaoHiem =
                //                await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                //                    .FirstOrDefaultAsync(p =>
                //                        p.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId
                //                        && p.TuNgay.Date <= DateTime.Now.Date
                //                        && (p.DenNgay == null ||
                //                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                //var bhytThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0) *
                //                        (dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
                ////* (bhytMucHuong ?? 100) / 100;
                //var bhytThanhToanChuaBaoGomMucHuong = (dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0) *
                //                                      (dichVuKyThuatBenhVienGiaBaoHiem
                //                                           ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;

                item.DuocHuongBaoHiem = isChecked;
                //if (isChecked)
                //{
                //    item.GiaBaoHiemThanhToan = bhytThanhToan;
                //}
                //else
                //{
                //    item.GiaBaoHiemThanhToan = 0;
                //}
            }
            else if (nhom.Equals(Constants.NhomDichVu.DichVuGiuong))
            {
                var item = entity.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.Id == id);
                if (item == null) return entity;
                //var dichVuGiuongBenhVienGiaBaoHiem = await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
                //                    .FirstOrDefaultAsync(p =>
                //                        p.DichVuGiuongBenhVienId == item.DichVuGiuongBenhVienId
                //                        && p.TuNgay.Date <= DateTime.Now.Date
                //                        && (p.DenNgay == null ||
                //                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                //var bhytThanhToan = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
                //                        (dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
                ////* (bhytMucHuong ?? 100) / 100;
                //var bhytThanhToanChuaBaoGomMucHuong = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
                //                                      (dichVuGiuongBenhVienGiaBaoHiem
                //                                           ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;

                item.DuocHuongBaoHiem = isChecked;
                //if (isChecked)
                //{
                //    item.GiaBaoHiemThanhToan = bhytThanhToan;
                //}
                //else
                //{
                //    item.GiaBaoHiemThanhToan = 0;
                //}
            }
            else if (nhom.Equals(Constants.NhomDichVu.DuocPham))
            {
                var item = entity.YeuCauDuocPhamBenhViens.FirstOrDefault(p => p.Id == id);
                if (item == null) return entity;

                //var dichVuDuocPhamGiaBaoHiem = await _duocPhamBenhVienGiaBaoHiemRepository.TableNoTracking
                //                    .FirstOrDefaultAsync(p =>
                //                        p.DuocPhamBenhVienId == item.DuocPhamBenhVienId
                //                        && p.TuNgay.Date <= DateTime.Now.Date
                //                        && (p.DenNgay == null ||
                //                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                //var bhytThanhToan = (dichVuDuocPhamGiaBaoHiem?.Gia ?? 0) *
                //                        (dichVuDuocPhamGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
                ////* (bhytMucHuong ?? 100) / 100;
                //var bhytThanhToanChuaBaoGomMucHuong = (dichVuDuocPhamGiaBaoHiem?.Gia ?? 0) *
                //                                      (dichVuDuocPhamGiaBaoHiem
                //                                           ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;


                item.DuocHuongBaoHiem = isChecked;
                //if (isChecked)
                //{
                //    item.GiaBaoHiemThanhToan = bhytThanhToan;
                //}
                //else
                //{
                //    item.GiaBaoHiemThanhToan = 0;
                //}
            }
            else if (nhom.Equals(Constants.NhomDichVu.VatTuTieuHao))
            {
                //var item = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == maDichVuId);
                //if (item == null) return entity;
                //item.DuocHuongBaoHiem = isChecked;
            }

            //Set default BHYT thanh toan and set new value
            //entity = await SetValueDefaultAndSetValueBHYTWithNewLogic(entity, mucHuong);

            //Set trang thai thanh toan cho thu ngan
            //entity = SetTrangThaiThanhToanForThuNganAgain(entity, lstDichVuOld);

            //await BaseRepository.UpdateAsync(entity);

            return entity;
        }

        public async Task<YeuCauTiepNhan> CapNhatDuocHuongBHYTDichVuAsync(bool isChecked, long id, string nhom, long yeuCauTiepNhanId)
        {
            var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);

            if (nhom.Equals(Constants.NhomDichVu.DichVuKhamBenh))
            {
                var item = entity.YeuCauKhamBenhs.FirstOrDefault(p => p.Id == id);
                if (item == null) return entity;

                if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                {
                    throw new Exception(string.Format(
                        _localizationService.GetResource("TiepNhanBenhNhan.CapNhatDuocHuongBHYT.DichVuDaThanhToan"),
                        item.TenDichVu));
                }

                // xử lý yêu cầu kỹ thuật kèm theo nếu có
                var lstYeuCauKyThuat = entity.YeuCauDichVuKyThuats
                    .Where(x => x.YeuCauKhamBenhId == item.Id
                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .ToList();
                var lstYeuCauKyThuatDaThanhToan = lstYeuCauKyThuat
                    .Where(x => x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    .Select(x => x.TenDichVu)
                    .Distinct().ToList();
                if (lstYeuCauKyThuatDaThanhToan.Any())
                {
                    throw new Exception(string.Format(
                        _localizationService.GetResource("TiepNhanBenhNhan.CapNhatDuocHuongBHYT.DichVuDaThanhToan"),
                        lstYeuCauKyThuatDaThanhToan.Join(", ")));
                }

                if (isChecked)
                {
                    if (item.DonGiaBaoHiem == null)
                    {
                        var giaBH = _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                            .FirstOrDefault(x => x.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId
                                                 && x.TuNgay.Date <= DateTime.Now.Date
                                                 && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                        item.DonGiaBaoHiem = giaBH?.Gia;
                        item.TiLeBaoHiemThanhToan = giaBH?.TiLeBaoHiemThanhToan;
                    }

                    if (item.DonGiaBaoHiem != null)
                    {
                        item.DuocHuongBaoHiem = true;

                        if (lstYeuCauKyThuat.Any())
                        {
                            var lstDichVuKyThuatId = lstYeuCauKyThuat
                                .Where(x => x.DonGiaBaoHiem == null)
                                .Select(x => x.DichVuKyThuatBenhVienId).Distinct().ToList();
                            var giaBHDichVuKyThuats = _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                                .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId)
                                            && x.TuNgay.Date <= DateTime.Now.Date
                                            && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date))
                                .ToList();
                            foreach (var yeuCauDichVuKyThuat in lstYeuCauKyThuat)
                            {
                                if (yeuCauDichVuKyThuat.DuocHuongBaoHiem != true)
                                {
                                    if (yeuCauDichVuKyThuat.DonGiaBaoHiem == null)
                                    {
                                        var giaBHTheoDichVu = giaBHDichVuKyThuats.FirstOrDefault(x =>
                                            x.DichVuKyThuatBenhVienId == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId);
                                        if (giaBHTheoDichVu != null)
                                        {
                                            yeuCauDichVuKyThuat.DonGiaBaoHiem = giaBHTheoDichVu?.Gia;
                                            yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan =
                                                giaBHTheoDichVu?.TiLeBaoHiemThanhToan;
                                        }
                                    }

                                    if (yeuCauDichVuKyThuat.DonGiaBaoHiem != null)
                                    {
                                        yeuCauDichVuKyThuat.DuocHuongBaoHiem = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format(
                            _localizationService.GetResource(
                                "TiepNhanBenhNhan.CapNhatDuocHuongBHYT.DichVuKhongCoGiaBaoHiem"), item.TenDichVu));
                    }
                }
                else
                {
                    if (entity.CoBHYT != true)
                    {
                        item.DuocHuongBaoHiem = false;
                        item.MucHuongBaoHiem = 0;
                        item.BaoHiemChiTra = null;

                        if (lstYeuCauKyThuat.Any())
                        {
                            var lstKyThuatDuocHuongBH = lstYeuCauKyThuat.Where(x => x.DuocHuongBaoHiem == true).ToList();
                            foreach (var yeuCauDichVuKyThuat in lstKyThuatDuocHuongBH)
                            {
                                yeuCauDichVuKyThuat.DuocHuongBaoHiem = false;
                                yeuCauDichVuKyThuat.MucHuongBaoHiem = 0;
                                yeuCauDichVuKyThuat.BaoHiemChiTra = null;
                            }
                        }
                    }
                }
            }
            else if (nhom.Equals(Constants.NhomDichVu.DichVuKyThuat))
            {
                var item = entity.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == id 
                                                                           && p.YeuCauKhamBenhId != null);
                if (item == null) return entity;

                var yeuCauKhamBenhChiDinh = entity.YeuCauKhamBenhs.First(x => x.Id == item.YeuCauKhamBenhId);
                if (entity.CoBHYT == true)
                {
                    if (yeuCauKhamBenhChiDinh.DuocHuongBaoHiem != true)
                    {
                        throw new Exception(string.Format(_localizationService.GetResource("TiepNhanBenhNhan.CapNhatDuocHuongBHYT.DichVuKhamKhongDuocHuongBHYT"), item.TenDichVu, yeuCauKhamBenhChiDinh.TenDichVu));
                    }
                }

                if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                {
                    throw new Exception(string.Format(_localizationService.GetResource("TiepNhanBenhNhan.CapNhatDuocHuongBHYT.DichVuDaThanhToan"), item.TenDichVu));
                }

                if (isChecked)
                {
                    if (item.DonGiaBaoHiem == null)
                    {
                        var giaBH = _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                            .FirstOrDefault(x => x.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId
                                                 && x.TuNgay.Date <= DateTime.Now.Date
                                                 && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));
                        item.DonGiaBaoHiem = giaBH?.Gia;
                        item.TiLeBaoHiemThanhToan = giaBH?.TiLeBaoHiemThanhToan;
                    }

                    if (item.DonGiaBaoHiem != null)
                    {
                        item.DuocHuongBaoHiem = true;
                    }
                    else
                    {
                        throw new Exception(string.Format(_localizationService.GetResource("TiepNhanBenhNhan.CapNhatDuocHuongBHYT.DichVuKhongCoGiaBaoHiem"), item.TenDichVu));
                    }
                }
                else
                {
                    if (entity.CoBHYT != true)
                    {
                        item.DuocHuongBaoHiem = false;
                        item.MucHuongBaoHiem = 0;
                        item.BaoHiemChiTra = null;
                    }
                }
            }

            return entity;
        }

        public async Task<bool> IsHaveCongNo(long yeuCauTiepNhanId, long congTyId)
        {
            var result = false;

            var congNo = await _congTuBaoHiemTuNhanCongNoRepository.TableNoTracking.Include(p => p.TaiKhoanBenhNhanThu)
                            .FirstOrDefaultAsync(p => p.TaiKhoanBenhNhanThu != null && p.TaiKhoanBenhNhanThu.YeuCauTiepNhanId == yeuCauTiepNhanId && p.CongTyBaoHiemTuNhanId == congTyId);

            if (congNo != null)
            {
                result = true;
            }

            return result;
        }

        public async Task<string> NoiThucHienModelText(long noiThucHienId, long? nguoiThucHienId = 0)
        {
            string result = string.Empty;

            if (nguoiThucHienId == 0 || nguoiThucHienId == null)
            {
                var phongEntity =
                    await _phongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == noiThucHienId);

                //hot fix 12/10/2020
                result = phongEntity?.Ten;
            }
            else
            {
                var phongEntity =
                    await _phongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == noiThucHienId);
                var nhanVienEntity = await _nhanVienRepository.TableNoTracking.Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == nguoiThucHienId);

                //hot fix 12/10/2020
                result = phongEntity?.Ten;
            }

            return result;
        }

        public async Task<string> NoiThucHienKyThuatGiuongModelText(long noiThucHienId, long? nguoiThucHienId)
        {
            string result = string.Empty;

            if (nguoiThucHienId == 0 || nguoiThucHienId == null)
            {
                var phongEntity =
                    await _phongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == noiThucHienId);

                //hot fix 12/10/2020
                result = phongEntity?.Ten;
            }
            else
            {
                var phongEntity =
                    await _phongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == noiThucHienId);
                var nhanVienEntity = await _nhanVienRepository.TableNoTracking.Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == nguoiThucHienId);

                //hot fix 12/10/2020
                result = phongEntity?.Ten;
            }

            return result;
        }
        public async Task<long?> GetYeuCauTiepNhanIdOfBenhNhan(string maThe, long? id = 0)
        {
            if (id == 0 || id == null)
            {
                var benhNhanEntity = await _benhNhanRepository.TableNoTracking.Include(p => p.YeuCauTiepNhans)
                    .FirstOrDefaultAsync(p => !string.IsNullOrEmpty(p.BHYTMaSoThe)
               && p.BHYTMaSoThe.Equals(maThe));
                if (benhNhanEntity != null && benhNhanEntity.YeuCauTiepNhans.Any(p => p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru))
                {
                    var yeuCauTiepNhanToDay = benhNhanEntity.YeuCauTiepNhans
                        .FirstOrDefault(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                         && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru);
                    if (yeuCauTiepNhanToDay != null)
                    {
                        return yeuCauTiepNhanToDay.Id;
                    }
                }
            }
            else
            {
                var benhNhanEntity = await _benhNhanRepository.TableNoTracking.Include(p => p.YeuCauTiepNhans).FirstOrDefaultAsync(p => p.Id == id);
                if (benhNhanEntity != null && benhNhanEntity.YeuCauTiepNhans.Any(p => p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru))
                {
                    var yeuCauTiepNhanToDay = benhNhanEntity.YeuCauTiepNhans.FirstOrDefault(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                        && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru);
                    //.FirstOrDefault(p => p.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                    if (yeuCauTiepNhanToDay != null)
                    {
                        return yeuCauTiepNhanToDay.Id;
                    }
                }
            }

            return null;
        }
        public async Task<long?> GetYeuCauTiepNhanIdOfBenhNhanTrongNgay(string maThe, long? id = 0)
        {
            if (id == 0 || id == null)
            {
                var benhNhanEntity = await _benhNhanRepository.TableNoTracking.Include(p => p.YeuCauTiepNhans)
                    .FirstOrDefaultAsync(p => !string.IsNullOrEmpty(p.BHYTMaSoThe)
               && p.BHYTMaSoThe.Equals(maThe));
                if (benhNhanEntity != null && benhNhanEntity.YeuCauTiepNhans.Any(p => p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru))
                {
                    var yeuCauTiepNhanToDay = benhNhanEntity.YeuCauTiepNhans
                        .FirstOrDefault(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                         && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                         && p.CreatedOn.Value.Date == DateTime.Now.Date
                         );
                    if (yeuCauTiepNhanToDay != null)
                    {
                        return yeuCauTiepNhanToDay.Id;
                    }
                }
            }
            else
            {
                var benhNhanEntity = await _benhNhanRepository.TableNoTracking.Include(p => p.YeuCauTiepNhans).FirstOrDefaultAsync(p => p.Id == id);
                if (benhNhanEntity != null && benhNhanEntity.YeuCauTiepNhans.Any(p => p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru))
                {
                    var yeuCauTiepNhanToDay = benhNhanEntity.YeuCauTiepNhans.FirstOrDefault(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                        && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                        && p.CreatedOn.Value.Date == DateTime.Now.Date
                        );
                    //.FirstOrDefault(p => p.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                    if (yeuCauTiepNhanToDay != null)
                    {
                        return yeuCauTiepNhanToDay.Id;
                    }
                }
            }

            return null;
        }

        public async Task<long?> GetYeuCauTiepNhanIdOfBenhNhanNgoaiNgay(string maThe, long? id = 0)
        {
            if (id == 0 || id == null)
            {
                var benhNhanEntity = await _benhNhanRepository.TableNoTracking.Include(p => p.YeuCauTiepNhans)
                    .FirstOrDefaultAsync(p => !string.IsNullOrEmpty(p.BHYTMaSoThe)
               && p.BHYTMaSoThe.Equals(maThe));
                if (benhNhanEntity != null && benhNhanEntity.YeuCauTiepNhans.Any(p => p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru))
                {
                    var yeuCauTiepNhanToDay = benhNhanEntity.YeuCauTiepNhans
                        .FirstOrDefault(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                         && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                         && !string.IsNullOrEmpty(p.BHYTMaSoThe) && p.BHYTMaSoThe.Equals(maThe)
                         && p.CreatedOn.Value.Date != DateTime.Now.Date
                         );
                    if (yeuCauTiepNhanToDay != null)
                    {
                        return yeuCauTiepNhanToDay.Id;
                    }
                }
            }
            else
            {
                var benhNhanEntity = await _benhNhanRepository.TableNoTracking.Include(p => p.YeuCauTiepNhans).FirstOrDefaultAsync(p => p.Id == id);
                //if (benhNhanEntity != null && benhNhanEntity.YeuCauTiepNhans.Any(p => p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru))
                //{
                //    var yeuCauTiepNhanToDay = benhNhanEntity.YeuCauTiepNhans.FirstOrDefault(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                //        && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                //        && (string.IsNullOrEmpty(maThe) || (!string.IsNullOrEmpty(maThe) && p.BHYTMaSoThe.Equals(maThe)))
                //        && p.CreatedOn.Value.Date != DateTime.Now.Date
                //        );
                //    //.FirstOrDefault(p => p.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                //    if (yeuCauTiepNhanToDay != null)
                //    {
                //        return yeuCauTiepNhanToDay.Id;
                //    }
                //}
                if (benhNhanEntity != null && benhNhanEntity.YeuCauTiepNhans.Any(p => p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru))
                {
                    var yeuCauTiepNhanToDay = benhNhanEntity.YeuCauTiepNhans.FirstOrDefault(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                        && p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                        && p.CreatedOn.Value.Date != DateTime.Now.Date
                        );
                    //.FirstOrDefault(p => p.CreatedOn.GetValueOrDefault().Date == DateTime.Now.Date);
                    if (yeuCauTiepNhanToDay != null)
                    {
                        return yeuCauTiepNhanToDay.Id;
                    }
                }
            }

            return null;
        }

        public async Task<string> GetLyDoTiepNhan(long lyDoTiepNhan)
        {
            var lyDo = await _lyDoTiepNhanRepository.TableNoTracking
                .Where(p => p.Id == lyDoTiepNhan)
                .Select(p => p.Ten).FirstOrDefaultAsync();

            return lyDo;
        }

        public async Task<LyDoTiepNhanDefaultDataGridVo> GetLyDoTiepNhanDefault()
        {
            var idChas = await _lyDoTiepNhanRepository.TableNoTracking
                .Where(p => p.LyDoTiepNhanChaId != null).Select(p => p.LyDoTiepNhanChaId).Distinct().ToListAsync();

            var ids = await _lyDoTiepNhanRepository.TableNoTracking
                .Select(p => p.Id).ToListAsync();

            long idDefault = 0;
            var flagMatch = false;

            foreach (var id in ids)
            {
                foreach (var idChaItem in idChas)
                {
                    if (id == idChaItem)
                    {
                        flagMatch = true;
                        break;
                    }
                }

                if (flagMatch != true)
                {
                    idDefault = id;
                    break;
                }

                flagMatch = false;
            }

            var lyDoDefault = await GetLyDoTiepNhan(idDefault);

            return new LyDoTiepNhanDefaultDataGridVo
            {
                Id = idDefault,
                Ten = lyDoDefault
            };
        }



        public async Task<GridDataSource> GetDataForGridAsyncPopupTimKiem(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _benhNhanRepository.TableNoTracking
                .Select(s => new TimKiemBenhNhanPopupGridVo
                {
                    Id = s.Id,
                    MaBN = s.MaBN,
                    BHYTMaSoThe = s.BHYTMaSoThe,
                    HoTen = s.HoTen,
                    NgaySinh = s.NgaySinh,
                    ThangSinh = s.ThangSinh,
                    NamSinh = s.NamSinh,
                    GioiTinh = s.GioiTinh,
                    SoChungMinhThu = s.SoChungMinhThu,
                    DiaChi = s.DiaChiDayDu,
                    SoDienThoai = s.SoDienThoai,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<TimKiemBenhNhanPopupGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.MaBN))
                {
                    query = query.Where(p => p.MaBN.Contains(queryString.MaBN.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.HoTen))
                {
                    query = query.Where(p => p.HoTen.Contains(queryString.HoTen.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.BHYTMaSoThe))
                {
                    query = query.Where(p => p.BHYTMaSoThe.Contains(queryString.BHYTMaSoThe.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.NgaySinhFormat) && (queryString.NamSinh == null || queryString.NamSinh == 0))
                {
                    DateTime.TryParseExact(queryString.NgaySinhFormat, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                        out var ngayThangNamSinh);
                    query = query.Where(p => p.NgaySinh == ngayThangNamSinh.Day && p.ThangSinh == ngayThangNamSinh.Month && p.NamSinh == ngayThangNamSinh.Year);
                }
                if (queryString.NamSinh != null && queryString.NamSinh != 0)
                {
                    query = query.Where(p => p.NamSinh == queryString.NamSinh);
                }
                if (!string.IsNullOrEmpty(queryString.SoChungMinhThu))
                {
                    query = query.Where(p => p.SoChungMinhThu.Contains(queryString.SoChungMinhThu.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.SoDienThoai))
                {
                    query = query.Where(p => p.SoDienThoai.Contains(queryString.SoDienThoai.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.DiaChi))
                {
                    query = query.Where(p => p.DiaChi.Contains(queryString.DiaChi.Trim()));
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncPopupTimKiem(QueryInfo queryInfo)
        {
            var query = _benhNhanRepository.TableNoTracking
                .Select(s => new TimKiemBenhNhanPopupGridVo
                {
                    Id = s.Id,
                    MaBN = s.MaBN,
                    BHYTMaSoThe = s.BHYTMaSoThe,
                    HoTen = s.HoTen,
                    NgaySinh = s.NgaySinh,
                    ThangSinh = s.ThangSinh,
                    NamSinh = s.NamSinh,
                    GioiTinh = s.GioiTinh,
                    SoChungMinhThu = s.SoChungMinhThu,
                    DiaChi = s.DiaChiDayDu,
                    SoDienThoai = s.SoDienThoai,
                    SoDienThoaiDisplay = s.SoDienThoaiDisplay,
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<TimKiemBenhNhanPopupGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.MaBN))
                {
                    query = query.Where(p => p.MaBN.Contains(queryString.MaBN.Trim()));
                }
                if (!string.IsNullOrEmpty(queryString.HoTen))
                {
                    query = query.Where(p => p.HoTen.Contains(queryString.HoTen.Trim()));

                }
                if (!string.IsNullOrEmpty(queryString.BHYTMaSoThe))
                {
                    query = query.Where(p => p.BHYTMaSoThe.Contains(queryString.BHYTMaSoThe.Trim()));

                }
                if (!string.IsNullOrEmpty(queryString.NgaySinhFormat) && (queryString.NamSinh == null || queryString.NamSinh == 0))
                {
                    DateTime.TryParseExact(queryString.NgaySinhFormat, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                        out var ngayThangNamSinh);
                    query = query.Where(p => p.NgaySinh == ngayThangNamSinh.Day && p.ThangSinh == ngayThangNamSinh.Month && p.NamSinh == ngayThangNamSinh.Year);
                }
                if (queryString.NamSinh != null && queryString.NamSinh != 0)
                {
                    query = query.Where(p => p.NamSinh == queryString.NamSinh);
                }
                if (!string.IsNullOrEmpty(queryString.SoChungMinhThu))
                {
                    query = query.Where(p => p.SoChungMinhThu.Contains(queryString.SoChungMinhThu));

                }
                if (!string.IsNullOrEmpty(queryString.SoDienThoai))
                {
                    query = query.Where(p => p.SoDienThoai.Contains(queryString.SoDienThoai.Trim()));

                }
                if (!string.IsNullOrEmpty(queryString.DiaChi))
                {
                    query = query.Where(p => p.DiaChi.Contains(queryString.DiaChi.Trim()));
                }
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridAsyncTaiKham(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.BacSiThucHien).ThenInclude(o => o.User)
                .Where(o => o.YeuCauKhamBenhs.Any(p => p.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && p.CoTaiKham == true)
                && o.BenhNhanId == long.Parse(queryInfo.AdditionalSearchString))
                .SelectMany(p => p.YeuCauKhamBenhs).Where(p => p.CoTaiKham == true)
                .Select(s => new TaiKhamGridVo
                {
                    Id = s.Id,
                    TenDichVu = s.TenDichVu,
                    BacSiThucHien = s.BacSiThucHien != null ? s.BacSiThucHien.User.HoTen : "",
                    GhiChuTaiKham = s.GhiChuTaiKham,
                    NgayTaiKhamDisplay = s.NgayTaiKham != null ? (s.NgayTaiKham ?? DateTime.Now).ApplyFormatDate() : "",
                    NgayTaiKham = s.NgayTaiKham ?? DateTime.Now,
                })
                .OrderByDescending(s => s.NgayTaiKham)
                .ApplyLike(queryInfo.SearchTerms, g => g.TenDichVu, g => g.BacSiThucHien, g => g.GhiChuTaiKham);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //   .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalForGridAsyncTaiKham(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.BacSiThucHien).ThenInclude(o => o.User)
                .Where(o => o.YeuCauKhamBenhs.Any(p => p.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && p.CoTaiKham == true)
                            && o.BenhNhanId == long.Parse(queryInfo.AdditionalSearchString))
                .SelectMany(p => p.YeuCauKhamBenhs).Where(p => p.CoTaiKham == true)
                .Select(s => new TaiKhamGridVo
                {
                    Id = s.Id,
                    TenDichVu = s.TenDichVu,
                    BacSiThucHien = s.BacSiThucHien != null ? s.BacSiThucHien.User.HoTen : "",
                    GhiChuTaiKham = s.GhiChuTaiKham,
                    NgayTaiKhamDisplay = s.NgayTaiKham != null ? (s.NgayTaiKham ?? DateTime.Now).ApplyFormatDate() : "",
                    NgayTaiKham = s.NgayTaiKham ?? DateTime.Now,
                })
                .OrderByDescending(s => s.NgayTaiKham)
                .ApplyLike(queryInfo.SearchTerms, g => g.TenDichVu, g => g.BacSiThucHien, g => g.GhiChuTaiKham);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<QuanHuyenTinhThanhModel> GetTinhThanhQuanHuyen(long phuongXaId)
        {
            var result = new QuanHuyenTinhThanhModel();
            var donViHanhChinhTrucThuoc =
                await _donViHanhChinhRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == phuongXaId);
            if (donViHanhChinhTrucThuoc != null)
            {
                var donViHanhChinhTrucThuocId = donViHanhChinhTrucThuoc.TrucThuocDonViHanhChinhId;
                result.QuanHuyenId =
                (await _donViHanhChinhRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                    p.Id == donViHanhChinhTrucThuocId))?.Id;
            }

            if (result.QuanHuyenId != null && result.QuanHuyenId != 0)
            {
                var donViHanhChinhTrucThuocQuanHuyen =
                    await _donViHanhChinhRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == result.QuanHuyenId);
                var donViHanhChinhTrucThuocId = donViHanhChinhTrucThuocQuanHuyen.TrucThuocDonViHanhChinhId;
                result.TinhThanhId =
                (await _donViHanhChinhRepository.TableNoTracking.FirstOrDefaultAsync(p =>
                    p.Id == donViHanhChinhTrucThuocId))?.Id;
            }

            return result;
        }

        public async Task<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> GetPhongBenhVien(long id)
        {
            return await _phongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> DuocHuongBHYT(long dichVuId, int loai)
        {
            //1: dich vu kham benh, 2: dich vu ky thuat, 3: dich vu giuong, 4: Vat tu, 5: Vat tu
            var result = false;

            switch (loai)
            {
                case 1:
                    var dichVuKhamBenhBenhVienGiaBaoHiem =
                        await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKhamBenhBenhVienId == dichVuId
                                                                                                                   && p.TuNgay.Date <= DateTime.Now.Date
                                                                                                                   && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                    if (dichVuKhamBenhBenhVienGiaBaoHiem != null && dichVuKhamBenhBenhVienGiaBaoHiem.Gia != 0)
                        result = true;
                    break;
                case 2:
                    var dichVuKyThuatBenhVienGiaBaoHiem =
                        await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKyThuatBenhVienId == dichVuId
                                                                                                                   && p.TuNgay.Date <= DateTime.Now.Date
                                                                                                                   && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                    if (dichVuKyThuatBenhVienGiaBaoHiem != null && dichVuKyThuatBenhVienGiaBaoHiem.Gia != 0)
                        result = true;
                    break;
                case 3:
                    var dichVuGiuongBenhVienGiaBaoHiem =
                        await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuGiuongBenhVienId == dichVuId
                                                                                                                  && p.TuNgay.Date <= DateTime.Now.Date
                                                                                                                  && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                    if (dichVuGiuongBenhVienGiaBaoHiem != null && dichVuGiuongBenhVienGiaBaoHiem.Gia != 0)
                        result = true;
                    break;
                case 4:
                    break;
                case 5:
                    //var duocPhamBenhVienGiaBaoHiem =
                    //    await _duocPhamBenhVienGiaBaoHiemRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DuocPhamBenhVienId == dichVuId
                    //                                                                                               && p.TuNgay.Date <= DateTime.Now.Date
                    //                                                                                               && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                    //if (duocPhamBenhVienGiaBaoHiem != null && duocPhamBenhVienGiaBaoHiem.Gia != 0)
                    //    result = true;
                    break;
                default:
                    break;
            }

            return result;
        }


        public async Task<decimal> GetGiaDichVu(long dichVuId, long loaiGiaId, string nhom)
        {
            decimal price = 0;
            switch (nhom)
            {
                case Constants.NhomDichVu.DichVuKhamBenh:
                    var nhomGia = await _nhomGiaDichVuKhamBenhRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == loaiGiaId);
                    if (nhomGia == null) break;
                    var entity = await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKhamBenhBenhVienId == dichVuId
                     && p.NhomGiaDichVuKhamBenhBenhVienId == nhomGia.Id && p.TuNgay.Date <= DateTime.Now.Date
                     && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                    price = entity != null ? entity.Gia : 0;
                    break;
                case Constants.NhomDichVu.DichVuKyThuat:
                    var nhomGiaDVKT = await _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == loaiGiaId);
                    if (nhomGiaDVKT == null) break;
                    var entityDVKT = await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuKyThuatBenhVienId == dichVuId
                     && p.NhomGiaDichVuKyThuatBenhVienId == nhomGiaDVKT.Id && p.TuNgay.Date <= DateTime.Now.Date
                     && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                    price = entityDVKT != null ? entityDVKT.Gia : 0;
                    break;
                case Constants.NhomDichVu.DichVuGiuong:
                    var nhomGiaDVG = await _nhomGiaDichVuGiuongBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == loaiGiaId);
                    if (nhomGiaDVG == null) break;
                    var entityDVG = await _dichVuGiuongBenhVienGiaBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.DichVuGiuongBenhVienId == dichVuId
                     && p.NhomGiaDichVuGiuongBenhVienId == nhomGiaDVG.Id && p.TuNgay.Date <= DateTime.Now.Date
                     && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
                    price = entityDVG != null ? entityDVG.Gia : 0;
                    break;
                case Constants.NhomDichVu.DuocPham:
                    break;
                default:
                    break;
            }

            return price;
        }


        public Task<bool> IsUnder6YearOld(DateTime? dateTime, int? year)
        {
            var now = DateTime.Now;
            if (dateTime != null)
            {
                if (dateTime.GetValueOrDefault().Year == now.Year - 6)
                {
                    if (dateTime.GetValueOrDefault().Month == now.Month)
                    {
                        if (dateTime.GetValueOrDefault().Day >= now.Day)
                        {
                            return Task.FromResult(true);
                        }
                        else
                        {
                            return Task.FromResult(true);
                        }
                    }
                    else if (dateTime.GetValueOrDefault().Month > now.Month)
                    {
                        return Task.FromResult(true);
                    }
                    else
                    {
                        return Task.FromResult(false);
                    }
                }
                else if (dateTime.GetValueOrDefault().Year > now.Year - 6)
                {
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            else
            {
                if (year == now.Year - 6)
                {
                    return Task.FromResult(true);
                }
                else if (year > now.Year - 6)
                {
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(false);
        }

        public async Task<DefaultValueTNBNModel> GetDefaultValueForTNBN()
        {
            var cauHinhTNBN = _cauHinhService.LoadSetting<CauHinhTiepNhan>();

            var danTocId = cauHinhTNBN.DanToc;
            var hinhThucDenId = cauHinhTNBN.HinhThucDen;
            var lyDoTiepNhanId = cauHinhTNBN.LyDoTiepNhan;
            var quocTichId = cauHinhTNBN.QuocTich;
            var tinhThanhPhoId = cauHinhTNBN.TinhThanhPho;

            var danTocEntity = _danTocRepository.TableNoTracking.FirstOrDefault(p => p.Id == danTocId);
            if (danTocEntity == null)
            {
                danTocId = _danTocRepository.TableNoTracking.FirstOrDefault(p => p.IsDisabled != true)?.Id ?? 0;
            }

            var hinhThucDenEntity = _hinhThucDenRepository.TableNoTracking.FirstOrDefault(p => p.Id == hinhThucDenId);
            if (hinhThucDenEntity == null)
            {
                hinhThucDenId = _hinhThucDenRepository.TableNoTracking.FirstOrDefault(p => p.IsDisabled != true)?.Id ?? 0;
            }

            var lyDoTiepNhanEntity = _lyDoTiepNhanRepository.TableNoTracking.FirstOrDefault(p => p.Id == lyDoTiepNhanId);
            if (lyDoTiepNhanEntity == null)
            {
                lyDoTiepNhanId = _lyDoTiepNhanRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            }

            var quocTichEntity = _quocGiaRepository.TableNoTracking.FirstOrDefault(p => p.Id == quocTichId);
            if (quocTichEntity == null)
            {
                quocTichId = _quocGiaRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            }

            var tinhThanhPhoEntity = _quocGiaRepository.TableNoTracking.FirstOrDefault(p => p.Id == tinhThanhPhoId);
            if (tinhThanhPhoEntity == null)
            {
                tinhThanhPhoId = _quocGiaRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            }

            var result = new DefaultValueTNBNModel
            {
                DanTocId = danTocId,
                HinhThucDenId = hinhThucDenId,
                LyDoTiepNhanId = lyDoTiepNhanId,
                QuocTichId = quocTichId,
                TinhThanhPhoId = tinhThanhPhoId,
            };

            return result;
        }


        //public Task<bool> IsUnder6yearsold(DateTime dateTime)
        //{

        //    var under6yearsold = DateHelper.DaysCalculateJson(dateTime);
        //    var model = JsonConvert.DeserializeObject<NgayThangNamSinhTiepNhanBenhNhan>(under6yearsold);
        //    if ( (model.Years == 6 && model.Days == 0 && model.Months == 0) || model.Years < 6)
        //    {
        //        return Task.FromResult(true);
        //    }

        //    return Task.FromResult(false);
        //}

        #region private class
        private YeuCauTiepNhan SetTrangThaiThanhToanForThuNganAgain(YeuCauTiepNhan entity, List<DichVuOld> lstDichVuOld)
        {
            //todo: need remove
            //if (entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Any())
            //{
            //    foreach (var item in entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
            //    {
            //        var valueOldForItem = lstDichVuOld.Where(p => p.Nhom.Equals(Constants.NhomDichVu.DichVuKhamBenh) && p.Id == item.Id)
            //            .Select(p => p.GiaBaoHiemThanhToan).FirstOrDefault();
            //        if (valueOldForItem != item.GiaBaoHiemThanhToan)
            //        {
            //            item.BaoHiemChiTra = null;
            //            if (item.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
            //            {
            //                item.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
            //            }
            //        }
            //    }
            //}
            //if (entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Any())
            //{
            //    foreach (var item in entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            //    {
            //        var valueOldForItem = lstDichVuOld.Where(p => p.Nhom.Equals(Constants.NhomDichVu.DichVuKyThuat) && p.Id == item.Id)
            //            .Select(p => p.GiaBaoHiemThanhToan).FirstOrDefault();
            //        if (valueOldForItem != item.GiaBaoHiemThanhToan)
            //        {
            //            item.BaoHiemChiTra = null;
            //            if (item.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
            //            {
            //                item.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
            //            }
            //        }
            //    }
            //}
            //if (entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy).Any())
            //{
            //    foreach (var item in entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy))
            //    {
            //        var valueOldForItem = lstDichVuOld.Where(p => p.Nhom.Equals(Constants.NhomDichVu.DichVuGiuong) && p.Id == item.Id)
            //            .Select(p => p.GiaBaoHiemThanhToan).FirstOrDefault();
            //        if (valueOldForItem != item.GiaBaoHiemThanhToan)
            //        {
            //            item.BaoHiemChiTra = null;
            //            if (item.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
            //            {
            //                item.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
            //            }
            //        }
            //    }
            //}
            //if (entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).Any())
            //{
            //    foreach (var item in entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
            //    {
            //        var valueOldForItem = lstDichVuOld.Where(p => p.Nhom.Equals(Constants.NhomDichVu.DuocPham) && p.Id == item.Id)
            //            .Select(p => p.GiaBaoHiemThanhToan).FirstOrDefault();
            //        if (valueOldForItem != item.GiaBaoHiemThanhToan)
            //        {
            //            item.BaoHiemChiTra = null;
            //            if (item.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
            //            {
            //                item.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
            //            }
            //        }
            //    }
            //}

            return entity;
        }
        private List<DichVuOld> GetListDichVuOld(YeuCauTiepNhan entity)
        {
            var lstDichVuOld = new List<DichVuOld>();

            if (entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Any())
            {
                foreach (var item in entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
                {
                    var dichVu = new DichVuOld();
                    dichVu.Id = item.Id;
                    //dichVu.GiaBaoHiemThanhToan = item.GiaBaoHiemThanhToan;
                    dichVu.DuocHuongBaoHiem = item.DuocHuongBaoHiem;
                    dichVu.Nhom = Constants.NhomDichVu.DichVuKhamBenh;
                    lstDichVuOld.Add(dichVu);
                }
            }
            if (entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Any())
            {
                foreach (var item in entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                {
                    var dichVu = new DichVuOld();
                    dichVu.Id = item.Id;
                    //dichVu.GiaBaoHiemThanhToan = item.GiaBaoHiemThanhToan;
                    dichVu.DuocHuongBaoHiem = item.DuocHuongBaoHiem;
                    dichVu.Nhom = Constants.NhomDichVu.DichVuKyThuat;
                    lstDichVuOld.Add(dichVu);
                }
            }
            if (entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy).Any())
            {
                foreach (var item in entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy))
                {
                    var dichVu = new DichVuOld();
                    dichVu.Id = item.Id;
                    //dichVu.GiaBaoHiemThanhToan = item.GiaBaoHiemThanhToan;
                    dichVu.DuocHuongBaoHiem = item.DuocHuongBaoHiem;
                    dichVu.Nhom = Constants.NhomDichVu.DichVuGiuong;
                    lstDichVuOld.Add(dichVu);
                }
            }
            if (entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).Any())
            {
                foreach (var item in entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
                {
                    var dichVu = new DichVuOld();
                    dichVu.Id = item.Id;
                    //dichVu.GiaBaoHiemThanhToan = item.GiaBaoHiemThanhToan;
                    dichVu.DuocHuongBaoHiem = item.DuocHuongBaoHiem;
                    dichVu.Nhom = Constants.NhomDichVu.DuocPham;
                    lstDichVuOld.Add(dichVu);
                }
            }
            if (entity.YeuCauVatTuBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy).Any())
            {
                foreach (var item in entity.YeuCauVatTuBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
                {
                    var dichVu = new DichVuOld();
                    dichVu.Id = item.Id;
                    //dichVu.GiaBaoHiemThanhToan = item.GiaBaoHiemThanhToan;
                    dichVu.DuocHuongBaoHiem = item.DuocHuongBaoHiem;
                    dichVu.Nhom = Constants.NhomDichVu.VatTuTieuHao;
                    lstDichVuOld.Add(dichVu);
                }
            }

            return lstDichVuOld;
        }
        //private async Task<YeuCauTiepNhan> SetValueDefaultAndSetValueBHYTWithNewLogic(YeuCauTiepNhan entity, int mucHuong)
        //{
        //    var soTienBHYTSeThanhToan = await _cauHinhService.SoTienBHYTSeThanhToanToanBo();

        //    var lstSoLanKham = await _cauHinhService.GetTyLeHuongBaoHiem5LanKhamDichVuBHYT();

        //    //Set default BHYT thanh toan
        //    if (entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null).Any())
        //    {
        //        foreach (var item in entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null))
        //        {
        //            var dichVuKhamBenhBenhVienGiaBaoHiem =
        //                       await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
        //                           .FirstOrDefaultAsync(p =>
        //                               p.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId
        //                               && p.TuNgay.Date <= DateTime.Now.Date
        //                               && (p.DenNgay == null ||
        //                                   (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
        //            var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                        (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;

        //            item.GiaBaoHiemThanhToan = bhytThanhToan;
        //        }
        //    }

        //    if (entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null).Any())
        //    {
        //        foreach (var item in entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null))
        //        {
        //            var dichVuKyThuatBenhVienGiaBaoHiem =
        //                        await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

        //            var bhytThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                    (dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;

        //            item.GiaBaoHiemThanhToan = bhytThanhToan;
        //        }
        //    }

        //    if (entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null).Any())
        //    {
        //        foreach (var item in entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null))
        //        {
        //            var dichVuGiuongBenhVienGiaBaoHiem = await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuGiuongBenhVienId == item.DichVuGiuongBenhVienId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

        //            var bhytThanhToan = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                    (dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;

        //            item.GiaBaoHiemThanhToan = bhytThanhToan;
        //        }
        //    }

        //    if (entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null).Any())
        //    {
        //        foreach (var item in entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && p.DuocHuongBaoHiem))
        //        {
        //            var dichVuDuocPhamGiaBaoHiem = await _duocPhamBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DuocPhamBenhVienId == item.DuocPhamBenhVienId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

        //            var bhytThanhToan = (dichVuDuocPhamGiaBaoHiem?.Gia ?? 0) *
        //                                    (dichVuDuocPhamGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;

        //            item.GiaBaoHiemThanhToan = bhytThanhToan;

        //        }
        //    }

        //    //Set BHYT with logic
        //    var tongBHYTThanhToan = TinhTongTienBHYTThanhToan(entity);
        //    int soLanKham = 1;

        //    if (entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null).Any())
        //    {
        //        foreach (var item in entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null))
        //        {
        //            item.GiaBaoHiemThanhToan = await TinhTienBHYTVoiSoLanVaMucLuongCoBan(item.GiaBaoHiemThanhToan ?? 0
        //                    , soLanKham, tongBHYTThanhToan, soTienBHYTSeThanhToan, mucHuong, lstSoLanKham, entity, null);
        //            soLanKham = soLanKham + 1;
        //        }
        //    }

        //    if (entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null).Any())
        //    {
        //        foreach (var item in entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null))
        //        {
        //            item.GiaBaoHiemThanhToan = await TinhTienBHYTVoiSoLanVaMucLuongCoBan(item.GiaBaoHiemThanhToan ?? 0
        //                    , 1, tongBHYTThanhToan, soTienBHYTSeThanhToan, mucHuong, lstSoLanKham, entity, null);
        //        }
        //    }

        //    if (entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null).Any())
        //    {
        //        foreach (var item in entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null))
        //        {
        //            item.GiaBaoHiemThanhToan = await TinhTienBHYTVoiSoLanVaMucLuongCoBan(item.GiaBaoHiemThanhToan ?? 0
        //                    , 1, tongBHYTThanhToan, soTienBHYTSeThanhToan, mucHuong, lstSoLanKham, entity, null);
        //        }
        //    }

        //    if (entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null).Any())
        //    {
        //        foreach (var item in entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && p.DuocHuongBaoHiem && p.YeuCauGoiDichVuId == null))
        //        {
        //            item.GiaBaoHiemThanhToan = await TinhTienBHYTVoiSoLanVaMucLuongCoBan(item.GiaBaoHiemThanhToan ?? 0
        //                    , 1, tongBHYTThanhToan, soTienBHYTSeThanhToan, mucHuong, lstSoLanKham, entity, null);

        //        }
        //    }

        //    return entity;
        //}

        //private decimal TinhTongTienBHYTThanhToan(YeuCauTiepNhan entity)
        //{
        //    decimal total = 0;

        //    //if ()
        //    //{

        //    //}
        //    total = total + entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.DuocHuongBaoHiem && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(p => p.GiaBaoHiemThanhToan).Sum() ?? 0;

        //    total = total + entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.DuocHuongBaoHiem && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(p => p.GiaBaoHiemThanhToan).Sum() ?? 0;

        //    total = total + entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.DuocHuongBaoHiem && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy).Select(p => p.GiaBaoHiemThanhToan).Sum() ?? 0;

        //    total = total + entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.DuocHuongBaoHiem && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy).Select(p => p.GiaBaoHiemThanhToan).Sum() ?? 0;

        //    total = total + entity.YeuCauVatTuBenhViens.Where(p => p.WillDelete != true && p.DuocHuongBaoHiem && p.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy).Select(p => p.GiaBaoHiemThanhToan).Sum() ?? 0;

        //    return total;
        //}

        //private async Task<decimal> TinhTienBHYTVoiSoLanVaMucLuongCoBan(decimal bhytThanhToanChuaBaoGomMucHuong, int soLan, decimal tongBHYTThanhToan
        //    , decimal soTienBHYTSeThanhToan, int mucHuong, List<double> lstSoLanKham, YeuCauTiepNhan entity, List<ChiDinhDichVuGridVo> lstGrid)
        //{

        //    //
        //    var tyLeLan1 = lstSoLanKham[0];
        //    var tyLeLan2 = lstSoLanKham[1];
        //    var tyLeLan3 = lstSoLanKham[2];
        //    var tyLeLan4 = lstSoLanKham[3];
        //    var tyLeLan5 = lstSoLanKham[4];

        //    //Tinh tong BHYT thanh toan - cheat
        //    decimal tongBHYTThanhToanSauApplySoLanKham = 0;
        //    if (entity != null)
        //    {
        //        var soLanKham = 1;
        //        foreach (var item in entity.YeuCauKhamBenhs.Where(p => p.WillDelete != true && p.DuocHuongBaoHiem && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.YeuCauGoiDichVuId == null))
        //        {
        //            var dichVuKhamBenhBenhVienGiaBaoHiem =
        //                        await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
        //            var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                        (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //            //(model.BHYTMucHuong ?? 100) / 100;
        //            if (soLanKham == 1)
        //            {
        //                tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan1 / 100;
        //            }
        //            else if (soLanKham == 2)
        //            {
        //                tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan2 / 100;
        //            }
        //            else if (soLanKham == 3)
        //            {
        //                tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan3 / 100;
        //            }
        //            else if (soLanKham == 4)
        //            {
        //                tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan4 / 100;
        //            }
        //            else if (soLanKham == 5)
        //            {
        //                tongBHYTThanhToanSauApplySoLanKham =
        //                    tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan5 / 100;
        //            }
        //            else
        //            {
        //                tongBHYTThanhToanSauApplySoLanKham = 0;
        //            }
        //            soLanKham = soLanKham + 1;
        //        }
        //        foreach (var item in entity.YeuCauDichVuKyThuats.Where(p => p.WillDelete != true && p.DuocHuongBaoHiem && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.YeuCauGoiDichVuId == null))
        //        {
        //            var dichVuKyThuatBenhVienGiaBaoHiem =
        //                        await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
        //            var bhytThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                        (dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * item.SoLan;
        //            tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan;
        //        }
        //        foreach (var item in entity.YeuCauDichVuGiuongBenhViens.Where(p => p.WillDelete != true && p.DuocHuongBaoHiem && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy && p.YeuCauGoiDichVuId == null))
        //        {
        //            var dichVuGiuongBenhVienGiaBaoHiem =
        //                        await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuGiuongBenhVienId == item.DichVuGiuongBenhVienId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
        //            var bhytThanhToan = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                        (dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //            tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan;
        //        }
        //        //foreach (var item in entity.YeuCauDuocPhamBenhViens.Where(p => p.WillDelete != true && p.DuocHuongBaoHiem && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && p.YeuCauGoiDichVuId == null))
        //        //{
        //        //    var duocPhamBenhVienGiaBaoHiem =
        //        //                await _duocPhamBenhVienGiaBaoHiemRepository.TableNoTracking
        //        //                    .FirstOrDefaultAsync(p =>
        //        //                        p.DuocPhamBenhVienId == item.DuocPhamBenhVienId
        //        //                        && p.TuNgay.Date <= DateTime.Now.Date
        //        //                        && (p.DenNgay == null ||
        //        //                            (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
        //        //    var bhytThanhToan = (duocPhamBenhVienGiaBaoHiem?.Gia ?? 0) *
        //        //                                (duocPhamBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (decimal)item.SoLuong;
        //        //    tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan;
        //        //}
        //    }
        //    else if (lstGrid != null)
        //    {
        //        var soLanKham = 1;
        //        var lstCheck = lstGrid.Where(p => p.IsGoiCoChietKhau != true && p.DuocHuongBHYT);
        //        foreach (var item in lstCheck)
        //        {
        //            if (item.Nhom == Constants.NhomDichVu.DichVuKhamBenh)
        //            {
        //                var dichVuKhamBenhBenhVienGiaBaoHiem =
        //                        await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuKhamBenhBenhVienId == item.MaDichVuId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
        //                var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                            (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //                //(model.BHYTMucHuong ?? 100) / 100;
        //                if (soLanKham == 1)
        //                {
        //                    tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan1 / 100;
        //                }
        //                else if (soLanKham == 2)
        //                {
        //                    tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan2 / 100;
        //                }
        //                else if (soLanKham == 3)
        //                {
        //                    tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan3 / 100;
        //                }
        //                else if (soLanKham == 4)
        //                {
        //                    tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan4 / 100;
        //                }
        //                else if (soLanKham == 5)
        //                {
        //                    tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan * (decimal)tyLeLan5 / 100;
        //                }
        //                else
        //                {
        //                    tongBHYTThanhToanSauApplySoLanKham = 0;
        //                }
        //                soLanKham = soLanKham + 1;
        //            }
        //            else if (item.Nhom == Constants.NhomDichVu.DichVuKyThuat)
        //            {
        //                var dichVuKyThuatBenhVienGiaBaoHiem =
        //                        await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuKyThuatBenhVienId == item.MaDichVuId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
        //                var bhytThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                            (dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * item.SoLuong;
        //                tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan ?? 0;
        //            }
        //            else if (item.Nhom == Constants.NhomDichVu.DichVuGiuong)
        //            {
        //                var dichVuGiuongBenhVienGiaBaoHiem =
        //                        await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
        //                            .FirstOrDefaultAsync(p =>
        //                                p.DichVuGiuongBenhVienId == item.MaDichVuId
        //                                && p.TuNgay.Date <= DateTime.Now.Date
        //                                && (p.DenNgay == null ||
        //                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
        //                var bhytThanhToan = (dichVuGiuongBenhVienGiaBaoHiem?.Gia ?? 0) *
        //                                            (dichVuGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;
        //                tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan;
        //            }
        //            //else if (item.Nhom == Constants.NhomDichVu.DuocPham)
        //            //{
        //            //    var duocPhamBenhVienGiaBaoHiem =
        //            //            await _duocPhamBenhVienGiaBaoHiemRepository.TableNoTracking
        //            //                .FirstOrDefaultAsync(p =>
        //            //                    p.DuocPhamBenhVienId == item.MaDichVuId
        //            //                    && p.TuNgay.Date <= DateTime.Now.Date
        //            //                    && (p.DenNgay == null ||
        //            //                        (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));
        //            //    var bhytThanhToan = (duocPhamBenhVienGiaBaoHiem?.Gia ?? 0) *
        //            //                                (duocPhamBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * (decimal)item.SoLuong;
        //            //    tongBHYTThanhToanSauApplySoLanKham = tongBHYTThanhToanSauApplySoLanKham + bhytThanhToan;
        //            //}
        //        }
        //    }


        //    //tinh theo so lan
        //    if (soLan == 1)
        //    {
        //        bhytThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong * (decimal)tyLeLan1 / 100;
        //    }
        //    else if (soLan == 2)
        //    {
        //        bhytThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong * (decimal)tyLeLan2 / 100;
        //    }
        //    else if (soLan == 3)
        //    {
        //        bhytThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong * (decimal)tyLeLan3 / 100;
        //    }
        //    else if (soLan == 4)
        //    {
        //        bhytThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong * (decimal)tyLeLan4 / 100;
        //    }
        //    else if (soLan == 5)
        //    {
        //        bhytThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong * (decimal)tyLeLan5 / 100;
        //    }
        //    else
        //    {
        //        bhytThanhToanChuaBaoGomMucHuong = 0;
        //    }

        //    //tinh theo phan tram luong co ban
        //    if (tongBHYTThanhToanSauApplySoLanKham > soTienBHYTSeThanhToan)
        //    {
        //        bhytThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong * mucHuong / 100;
        //    }

        //    return bhytThanhToanChuaBaoGomMucHuong;
        //}

        #endregion private class

        #region Xóa y lệnh khi xóa dịch vụ

        private async Task XuLyXoaYLenhKhiXoaDichVuAsync(Enums.EnumNhomGoiDichVu loaiDichVu, long yeuCauDichVuId)
        {
            //var phieuDieuTri = await _noiTruPhieuDieuTriRepository.Table
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs)
            //    .Include(x => x.NoiTruPhieuDieuTriChiTietDienBiens)
            //    .Where(x => x.NoiTruPhieuDieuTriChiTietYLenhs.Any(a => (loaiDichVu == EnumNhomGoiDichVu.DichVuKyThuat && a.YeuCauDichVuKyThuatId == yeuCauDichVuId)
            //                                                            || (loaiDichVu == EnumNhomGoiDichVu.TruyenMau && a.YeuCauTruyenMauId == yeuCauDichVuId))) //todo: cần bổ sung thêm dịch vụ nếu cần
            //    .FirstOrDefaultAsync();
            var phieuDieuTri = await _noiTruBenhAnRepository.Table
                .Include(x => x.NoiTruPhieuDieuTriChiTietYLenhs)
                .Include(x => x.NoiTruPhieuDieuTriChiTietDienBiens)
                .Where(x => x.NoiTruPhieuDieuTriChiTietYLenhs.Any(a => (loaiDichVu == EnumNhomGoiDichVu.DichVuKyThuat && a.YeuCauDichVuKyThuatId == yeuCauDichVuId)
                                                                       || (loaiDichVu == EnumNhomGoiDichVu.TruyenMau && a.YeuCauTruyenMauId == yeuCauDichVuId)

                                                                       //BVHD-3575
                                                                       || (loaiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh && a.YeuCauKhamBenhId == yeuCauDichVuId))) //todo: cần bổ sung thêm dịch vụ nếu cần
                .FirstOrDefaultAsync();

            if (phieuDieuTri != null)
            {
                var chiTietYLenhTheoDichVus = phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs
                    .Where(x => (loaiDichVu == EnumNhomGoiDichVu.DichVuKyThuat && x.YeuCauDichVuKyThuatId == yeuCauDichVuId)
                                || (loaiDichVu == EnumNhomGoiDichVu.TruyenMau && x.YeuCauTruyenMauId == yeuCauDichVuId)
                                || (loaiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh && x.YeuCauKhamBenhId == yeuCauDichVuId)).ToList();
                foreach (var yLenh in chiTietYLenhTheoDichVus)
                {
                    yLenh.WillDelete = true;
                }

                var dienBienKhongCoYLenhs = phieuDieuTri.NoiTruPhieuDieuTriChiTietDienBiens.Where(x =>
                    phieuDieuTri.NoiTruPhieuDieuTriChiTietYLenhs.Where(y => !y.WillDelete).All(a => a.GioYLenh != x.GioDienBien)
                    ).ToList();
                foreach (var dienBien in dienBienKhongCoYLenhs)
                {
                    dienBien.WillDelete = true;
                }
            }
        }

        #region BVHD-3575

        public async Task XuLyXoaYLenhVaCapNhatDienBienKhiXoaDichVuAsync(Enums.EnumNhomGoiDichVu nhomDichVu, long yeuCauDichVuId)
        {
            await XuLyXoaYLenhKhiXoaDichVuAsync(nhomDichVu, yeuCauDichVuId);
        }


        #endregion
        #endregion

        public async Task<GridDataSource> GetDataForGridAsyncDichVuKhuyenMais(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }

            //BVHD-3825
            //long benhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            var arrParam = queryInfo.AdditionalSearchString.Split(";");
            var benhNhanId = long.Parse(arrParam[0]);
            bool isCapGiuong = arrParam[1].ToLower() == "true";

            var query = _yeuCauGoiDichVuRepository.TableNoTracking
               .Where(o => ((o.BenhNhanId == benhNhanId && o.GoiSoSinh != true) || (o.BenhNhanSoSinhId == benhNhanId && o.GoiSoSinh == true)) && o.TrangThai != EnumTrangThaiYeuCauGoiDichVu.DaHuy
               && (o.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any()
               || o.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any()
               || o.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
               .Select(s => new DichVuKhuyenMaiGridVo
               {
                   Id = s.Id,
                   Ten = s.ChuongTrinhGoiDichVu.Ten + " - " + s.ChuongTrinhGoiDichVu.TenGoiDichVu,//s.TenGoiDichVu,
                   ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,
                   BenhNhanId = (s.BenhNhanId == benhNhanId && s.GoiSoSinh != true) ? benhNhanId : s.BenhNhanSoSinhId
               });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncDichVuKhuyenMais(QueryInfo queryInfo)
        {
            return null;
        }

        public async Task<GridDataSourceDichVuKhuyenMai> GetDanhSachDichVuKhuyenMaiForGrid(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }

            //BVHD-3825
            //var yeuCauGoiDichVuId = long.Parse(queryInfo.AdditionalSearchString);
            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;
            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            //var yeuCauGoiDichVu = await _yeuCauGoiDichVuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauGoiDichVuId);
            var lstDichVuDangChon = new List<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo>();
            if (!string.IsNullOrEmpty(searchObj[1]) && searchObj[1] != "undefined" && searchObj[1] != "null")
            {
                lstDichVuDangChon = JsonConvert.DeserializeObject<List<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo>>(searchObj[1]);
            }
            var isPTTT = searchObj[2].ToLower() == "true";
            var isCapGiuong = searchObj[3].ToLower() == "true";
            var isDieuTriNoiTru = searchObj.Length >= 3 && searchObj[4].ToLower() == "true";
            var isVacxin = searchObj.Length >= 4 && searchObj[5].ToLower() == "true";
            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();


            List<ChiTietGoiDichVuKhuyenMaiTheoBenhNhanGridVo> danhSachDichVuKhuyenMaiBenhNhanVos = new List<ChiTietGoiDichVuKhuyenMaiTheoBenhNhanGridVo>();
            var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(gdv => gdv.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy && gdv.Id == yeuCauGoiDichVuId)
                .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs).ThenInclude(o => o.NhomGiaDichVuKhamBenhBenhVien)
                .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats).ThenInclude(o => o.NhomGiaDichVuKyThuatBenhVien)
                .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien)
                .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats).ThenInclude(o => o.DichVuKyThuatBenhVien)
                .Include(o => o.MienGiamChiPhis).ThenInclude(o => o.YeuCauDichVuKyThuat)
                .Include(o => o.MienGiamChiPhis).ThenInclude(o => o.YeuCauKhamBenh)
                .Include(o => o.MienGiamChiPhis).ThenInclude(o => o.TaiKhoanBenhNhanThu)
                .First();

            //BVHD-3825
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;

            foreach (var dvKhuyenMai in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
            {
                var dvKhuyenMaiDaDung = yeuCauGoiDichVu.MienGiamChiPhis.Where(o =>
                        o.DaHuy != true &&
                        o.YeuCauKhamBenh != null &&
                        o.YeuCauKhamBenh.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                        o.YeuCauKhamBenh.DichVuKhamBenhBenhVienId == dvKhuyenMai.DichVuKhamBenhBenhVienId &&
                        o.YeuCauKhamBenh.NhomGiaDichVuKhamBenhBenhVienId == dvKhuyenMai.NhomGiaDichVuKhamBenhBenhVienId &&
                        
                        (o.TaiKhoanBenhNhanThuId == null || o.TaiKhoanBenhNhanThu.DaHuy != true))
                    .Select(x => x.YeuCauKhamBenhId)
                    .Distinct().Count();

                //var dichVuTrongGoiKhuyenMaiVo = new CacDichVuKhuyenMaiTrongGoiMarketingGridVo
                //{
                //    Id = dvKhuyenMai.Id,
                //    Ma = dvKhuyenMai.DichVuKhamBenhBenhVien.Ma,
                //    Ten = dvKhuyenMai.DichVuKhamBenhBenhVien.Ten,
                //    NhomId = 1,
                //    SoLan = dvKhuyenMai.SoLan,
                //    LoaiGia = dvKhuyenMai.NhomGiaDichVuKhamBenhBenhVien.Ten,
                //    DonGia = dvKhuyenMai.DonGia,
                //    DonGiaKhuyenMai = dvKhuyenMai.DonGiaKhuyenMai,
                //    GhiChu = dvKhuyenMai.GhiChu,
                //    HanSuDung = DateHelper.CalculateHanSuDung(dvKhuyenMai.SoNgaySuDung, dvKhuyenMai.CreatedOn),
                //    SoLanDaDung = dvKhuyenMaiDaDung
                //};

                var dichVuTrongGoiKhuyenMaiVo = new ChiTietGoiDichVuKhuyenMaiTheoBenhNhanGridVo
                {
                    YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                    TenGoiDichVu = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + yeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu,
                    ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                    ChuongTrinhGoiDichVuChiTietId = dvKhuyenMai.Id,
                    DichVuBenhVienId = dvKhuyenMai.DichVuKhamBenhBenhVienId,
                    MaDichVu = dvKhuyenMai.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = dvKhuyenMai.DichVuKhamBenhBenhVien.Ten,
                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                    TenLoaiGia = dvKhuyenMai.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    NhomGiaId = dvKhuyenMai.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = dvKhuyenMai.SoLan,
                    DonGia = dvKhuyenMai.DonGia,
                    DonGiaKhuyenMai = dvKhuyenMai.DonGiaKhuyenMai,
                    HanSuDung = yeuCauGoiDichVu.ThoiDiemChiDinh.AddDays(dvKhuyenMai.SoNgaySuDung),
                    SoLuongDaDung = dvKhuyenMaiDaDung,
                    SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == dvKhuyenMai.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                               && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                               && a.ChuongTrinhGoiDichVuChiTietId == dvKhuyenMai.Id
                                                                                                                                                               && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh).SoLuongSuDung : 0,
                    IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                           && a.ChuongTrinhGoiDichVuChiTietId == dvKhuyenMai.Id
                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh),
                    IsPTTT = isPTTT,
                    IsDieuTriNoiTru = isDieuTriNoiTru,
                    //LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                    IsNhomTiemChung = false, //nhomTiemChungId == null ? false : dvKhuyenMai.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId
                    IsGoiDaQuyetToan = yeuCauGoiDichVu.DaQuyetToan == true,
                    IsDanhSachVacxin = isVacxin
                };
                danhSachDichVuKhuyenMaiBenhNhanVos.Add(dichVuTrongGoiKhuyenMaiVo);
            }

            foreach (var dvKhuyenMai in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
            {
                var dvKhuyenMaiDaDung = yeuCauGoiDichVu.MienGiamChiPhis
                    .Where(o =>
                        o.DaHuy != true &&
                        o.YeuCauDichVuKyThuat != null &&
                        o.YeuCauDichVuKyThuat.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                        o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId == dvKhuyenMai.DichVuKyThuatBenhVienId &&
                        o.YeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId == dvKhuyenMai.NhomGiaDichVuKyThuatBenhVienId &&

                        (o.TaiKhoanBenhNhanThuId == null || o.TaiKhoanBenhNhanThu.DaHuy != true))
                    .Select(o => new {o.YeuCauDichVuKyThuatId, o.YeuCauDichVuKyThuat.SoLan})
                    .GroupBy(x => x.YeuCauDichVuKyThuatId)
                    .Select(x => x.First().SoLan)
                    .DefaultIfEmpty().Sum();
                //var dichVuTrongGoiKhuyenMaiVo = new CacDichVuKhuyenMaiTrongGoiMarketingGridVo
                //{
                //    Id = dvKhuyenMai.Id,
                //    Ma = dvKhuyenMai.DichVuKyThuatBenhVien.Ma,
                //    Ten = dvKhuyenMai.DichVuKyThuatBenhVien.Ten,
                //    NhomId = 2,
                //    SoLan = dvKhuyenMai.SoLan,
                //    LoaiGia = dvKhuyenMai.NhomGiaDichVuKyThuatBenhVien.Ten,
                //    DonGia = dvKhuyenMai.DonGia,
                //    DonGiaKhuyenMai = dvKhuyenMai.DonGiaKhuyenMai,
                //    GhiChu = dvKhuyenMai.GhiChu,
                //    HanSuDung = DateHelper.CalculateHanSuDung(dvKhuyenMai.SoNgaySuDung, dvKhuyenMai.CreatedOn),
                //    SoLanDaDung = dvKhuyenMaiDaDung
                //};

                var dichVuTrongGoiKhuyenMaiVo = new ChiTietGoiDichVuKhuyenMaiTheoBenhNhanGridVo
                {
                    YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                    TenGoiDichVu = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + yeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu,
                    ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                    ChuongTrinhGoiDichVuChiTietId = dvKhuyenMai.Id,
                    DichVuBenhVienId = dvKhuyenMai.DichVuKyThuatBenhVienId,
                    MaDichVu = dvKhuyenMai.DichVuKyThuatBenhVien.Ma,
                    TenDichVu = dvKhuyenMai.DichVuKyThuatBenhVien.Ten,
                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                    TenLoaiGia = dvKhuyenMai.NhomGiaDichVuKyThuatBenhVien.Ten,
                    NhomGiaId = dvKhuyenMai.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = dvKhuyenMai.SoLan,
                    DonGia = dvKhuyenMai.DonGia,
                    DonGiaKhuyenMai = dvKhuyenMai.DonGiaKhuyenMai,
                    HanSuDung = yeuCauGoiDichVu.ThoiDiemChiDinh.AddDays(dvKhuyenMai.SoNgaySuDung),
                    SoLuongDaDung = dvKhuyenMaiDaDung,
                    SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == dvKhuyenMai.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                               && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                               && a.ChuongTrinhGoiDichVuChiTietId == dvKhuyenMai.Id
                                                                                                                                                               && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat).SoLuongSuDung : 0,
                    IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                           && a.ChuongTrinhGoiDichVuChiTietId == dvKhuyenMai.Id
                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat),
                    IsPTTT = isPTTT,
                    IsDieuTriNoiTru = isDieuTriNoiTru,
                    LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(dvKhuyenMai.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                    IsNhomTiemChung = nhomTiemChungId == null ? false : dvKhuyenMai.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId,
                    IsGoiDaQuyetToan = yeuCauGoiDichVu.DaQuyetToan == true,
                    IsDanhSachVacxin = isVacxin
                };

                danhSachDichVuKhuyenMaiBenhNhanVos.Add(dichVuTrongGoiKhuyenMaiVo);
            }

            //BVHD-3825 fillter
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var searchString = queryInfo.SearchTerms.Trim().ToLower().RemoveVietnameseDiacritics();
                danhSachDichVuKhuyenMaiBenhNhanVos = danhSachDichVuKhuyenMaiBenhNhanVos
                    .Where(x => x.MaDichVu.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchString)
                                || x.TenDichVu.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchString)).ToList();
            }

            var countTask = queryInfo.LazyLoadPage == true ? 0 : danhSachDichVuKhuyenMaiBenhNhanVos.Count();
            var queryTask = danhSachDichVuKhuyenMaiBenhNhanVos.ToArray();
            return new GridDataSourceDichVuKhuyenMai { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDichVuKhuyenMaiChild(QueryInfo queryInfo)
        {
            return null;
        }
        public async Task<(bool, long)> KiemTraTatCaDichVuTrongGoi(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = await BaseRepository.GetByIdAsync(yeuCauTiepNhanId,
                x => x.Include(o => o.BenhNhan).Include(o => o.YeuCauKhamBenhs).Include(o => o.YeuCauDichVuKyThuats));

            var soDichVuKham = yeuCauTiepNhan.YeuCauKhamBenhs.Any(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan && o.YeuCauGoiDichVuId != null);

            var soDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats.Any(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan && o.YeuCauGoiDichVuId != null);

            return (soDichVuKham || soDichVuKyThuat, yeuCauTiepNhan.BenhNhan.Id);
        }

        #region update nhóm dịch vụ thường dùng
        public async Task<ChiDinhDichVuTrongNhomThuongDungVo> ThemYeuGoiDichVuThuongDungTaoMoiYCTNAsync(YeuCauThemGoiDichVuThuongDungVo yeuCauVo)
        {
            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            //var coBHYT = yeuCauTiepNhan.CoBHYT ?? false;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thoiDiemHienTai = DateTime.Now;
            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();
            var result = new ChiDinhDichVuTrongNhomThuongDungVo();

            var lstDichVuKhamBenhTrongGoi = await _goiDichVuChiTietDichVuKhamBenhRepository.TableNoTracking
                .Where(x => yeuCauVo.GoiDichVuIds.Any(a => a == x.GoiDichVuId)
                            && !yeuCauVo.DichVuKhongThems.Any(a => a.GoiDichVuId == x.GoiDichVuId
                                                                   && a.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                                                                   && a.DichVuId == x.DichVuKhamBenhBenhVienId))
                .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                {
                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                    DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                    MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                    MaGiaDichVu = item.DichVuKhamBenhBenhVien.DichVuKhamBenh != null ? item.DichVuKhamBenhBenhVien.DichVuKhamBenh.MaTT37 : "",
                    NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKhamBenhBenhVienId,
                    TenNhomGiaDichVuBenhVien = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    DonGiaBenhVien = item.DichVuKhamBenhBenhVien
                                         .DichVuKhamBenhBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId
                                                                                                 && a.TuNgay.Date <= DateTime.Now.Date
                                                                                                 && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                    item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId
                                                                                                           && a.TuNgay.Date <= DateTime.Now.Date
                                                                                                           && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    DonGiaBaoHiem = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems
                                        .FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                             && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                    item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    //CoBHYT = coBHYT,
                    SoLuong = 1,
                    TiLeBaoHiemThanhToan = item.DichVuKhamBenhBenhVien
                                               .DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                      && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                            item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                })
                .Union(
                    _goiDichVuChiTietDichVuKyThuatRepository.TableNoTracking
                .Where(x => yeuCauVo.GoiDichVuIds.Any(a => a == x.GoiDichVuId)
                            && !yeuCauVo.DichVuKhongThems.Any(a => a.GoiDichVuId == x.GoiDichVuId
                                                                  && a.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                                                                  && a.DichVuId == x.DichVuKyThuatBenhVienId)
                            && (nhomTiemChungId == null || x.DichVuKyThuatBenhVien.NhomDichVuBenhVienId != nhomTiemChungId))
                .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                {
                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                    DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                    MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                    TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                    MaGiaDichVu = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.MaGia : "",
                    TenGia = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.TenGia : "",
                    Ma4350 = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350 : "",
                    NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                    TenNhomGiaDichVuBenhVien = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                    DonGiaBenhVien = item.DichVuKyThuatBenhVien
                                         .DichVuKyThuatVuBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId
                                                                                                  && a.TuNgay.Date <= DateTime.Now.Date
                                                                                                  && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                    item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId
                                                                                                                       && a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                       && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    DonGiaBaoHiem = item.DichVuKyThuatBenhVien
                                        .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                              && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                    item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                    && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    //CoBHYT = coBHYT,
                    SoLuong = item.SoLan,
                    TiLeBaoHiemThanhToan = item.DichVuKyThuatBenhVien
                                               .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                     && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                            item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                            && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                    NhomChiPhiDichVuKyThuat = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.NhomChiPhi : Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                    NhomDichVuBenhVienId = item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId
                }))
                .Union(
                    _goiDichVuChiTietDichVuGiuongRepository.TableNoTracking
                .Where(x => yeuCauVo.GoiDichVuIds.Any(a => a == x.GoiDichVuId)
                            && !yeuCauVo.DichVuKhongThems.Any(a => a.GoiDichVuId == x.GoiDichVuId
                                                              && a.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh
                                                              && a.DichVuId == x.DichVuGiuongBenhVienId))
                .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                {
                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                    DichVuBenhVienId = item.DichVuGiuongBenhVienId,
                    MaDichVu = item.DichVuGiuongBenhVien.Ma,
                    TenDichVu = item.DichVuGiuongBenhVien.Ten,
                    MaGiaDichVu = item.DichVuGiuongBenhVien.DichVuGiuong != null ? item.DichVuGiuongBenhVien.DichVuGiuong.MaTT37 : "",
                    NhomGiaDichVuBenhVienId = item.NhomGiaDichVuGiuongBenhVienId,
                    TenNhomGiaDichVuBenhVien = item.NhomGiaDichVuGiuongBenhVien.Ten,
                    DonGiaBenhVien = item.DichVuGiuongBenhVien
                                         .DichVuGiuongBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuGiuongBenhVienId == item.NhomGiaDichVuGiuongBenhVienId
                                                                                               && a.TuNgay.Date <= DateTime.Now.Date
                                                                                               && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                    item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.FirstOrDefault(a => a.NhomGiaDichVuGiuongBenhVienId == item.NhomGiaDichVuGiuongBenhVienId
                                                                                                                   && a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                   && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    DonGiaBaoHiem = item.DichVuGiuongBenhVien
                                        .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                             && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                    item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                  && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                    //CoBHYT = coBHYT,
                    SoLuong = item.SoLan,
                    TiLeBaoHiemThanhToan = item.DichVuGiuongBenhVien
                                               .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                    && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                            item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                }))
                .ToListAsync();

            if (!lstDichVuKhamBenhTrongGoi.Any())
            {
                throw new Exception(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.DichVu.Required"));
            }

            var bacSiDangKys = await _hoatDongNhanVienRepository.TableNoTracking
                .Include(x => x.NhanVien).ThenInclude(x => x.User)
                            .Where(x => x.NhanVien.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi
                                        && x.NhanVien.User.IsActive).ToListAsync();
            var noiThucHienDVKBs = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
                .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
                .ToListAsync();
            var noiThucHienDVKTs = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).ThenInclude(y => y.PhongBenhVien)
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
                .ToListAsync();
            var noiThucHienDVGs = await _dichVuGiuongBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
                .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
                .ToListAsync();

            foreach (var dichVuBenhVien in lstDichVuKhamBenhTrongGoi)
            {
                switch (dichVuBenhVien.NhomGoiDichVu)
                {
                    case EnumNhomGoiDichVu.DichVuKhamBenh:
                        if (dichVuBenhVien.DonGiaBenhVien == null)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.GiaBenhVien.NotExists"), dichVuBenhVien.TenDichVu));
                        }

                        //var lstPhongId = new List<long>();
                        //var lstNoiThucHienDVKBS = noiThucHienDVKBs.Where(x => x.Id == dichVuBenhVien.DichVuBenhVienId).SelectMany(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ToList();
                        //lstPhongId.AddRange(lstNoiThucHienDVKBS.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList());
                        //lstPhongId.AddRange(lstNoiThucHienDVKBS.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong).SelectMany(x => x.PhongBenhViens).Select(x => x.Id).ToList());
                        //lstPhongId.Sort();
                        //var phongThucHienId = lstPhongId.First();
                        var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();
                        var model = new DropDownListRequestModel()
                        {
                            ParameterDependencies = "{DichVu:" + dichVuBenhVien.DichVuBenhVienId + "}",
                            Take = 50
                        };
                        var noiThucHiens = await GetPhongKham(model, settings.GioiHanSoNguoiKham);
                        var phongThucHienId = noiThucHiens.Select(x => x.KeyId).FirstOrDefault();
                        var lstNoiThucHienId = phongThucHienId.Split(",");
                        var phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                        var nhanVienId = long.Parse(lstNoiThucHienId[1]);
                        var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.Include(p => p.KhoaPhong).FirstOrDefaultAsync(p => p.Id == phongBenhVienId);
                        var bacSiDangKyDVKB =
                            await _nhanVienRepository.TableNoTracking.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == nhanVienId);
                        //var entityYeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh()
                        //{
                        //    YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                        //    YeuCauKhamBenhTruocId = yeuCauVo.YeuCauKhamBenhId == 0 ? (long?)null : yeuCauVo.YeuCauKhamBenhId,
                        //    DichVuKhamBenhBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                        //    MaDichVu = dichVuBenhVien.MaDichVu,
                        //    TenDichVu = dichVuBenhVien.TenDichVu,
                        //    MaDichVuTT37 = dichVuBenhVien.MaGiaDichVu,
                        //    NhomGiaDichVuKhamBenhBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                        //    Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                        //    DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                        //    DuocHuongBaoHiem = dichVuBenhVien.DuocHuongBaoHiem,
                        //    TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,

                        //    TrangThai = EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                        //    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                        //    BaoHiemChiTra = null,

                        //    NhanVienChiDinhId = currentUserId,
                        //    NoiChiDinhId = phongHienTaiId,
                        //    ThoiDiemChiDinh = thoiDiemHienTai,

                        //    BacSiDangKyId = bacSiDangKyDVKB?.NhanVienId,
                        //    ThoiDiemDangKy = thoiDiemHienTai,
                        //    NoiDangKyId = phongThucHienId
                        //};

                        var yeuCauKhamNew = new ChiDinhDichVuGridVo
                        {
                            MaDichVuId = dichVuBenhVien.DichVuBenhVienId,
                            Ma = dichVuBenhVien.MaDichVu,
                            TenDichVu = dichVuBenhVien.TenDichVu,
                            LoaiGia = dichVuBenhVien.TenNhomGiaDichVuBenhVien,
                            LoaiGiaId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                            SoLuong = 1,
                            DonGiaDisplay = ((double)dichVuBenhVien.DonGiaBenhVien.Value).ApplyNumber(),
                            DonGia = (double)dichVuBenhVien.DonGiaBenhVien.Value,
                            ThanhTienDisplay = ((double)dichVuBenhVien.DonGiaBenhVien.Value).ApplyNumber(),
                            ThanhTien = (double)dichVuBenhVien.DonGiaBenhVien.Value,
                            //BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber(),
                            //BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong,
                            //BHYTThanhToan = bhytThanhToan,
                            //TLMGDisplay = doiTuongUuDaiDichVuKhamBenhBenhVien != null ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai + " %" : "",
                            //TLMG = doiTuongUuDaiDichVuKhamBenhBenhVien != null ? doiTuongUuDaiDichVuKhamBenhBenhVien.TiLeUuDai : 0,

                            Nhom = Constants.NhomDichVu.DichVuKhamBenh,
                            NoiThucHienId = phongThucHienId,
                            KhoaPhongId = phongBenhVienId,
                            DuocHuongBHYT = yeuCauVo.DuocHuongBaoHiem ?? false,
                            GiaBHYT = (double?)dichVuBenhVien.DonGiaBaoHiem ?? 0,
                            TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan ?? 0,

                        };

                        yeuCauKhamNew.BnThanhToan = yeuCauKhamNew.ThanhTien - (double)yeuCauKhamNew.BHYTThanhToan;
                        yeuCauKhamNew.BnThanhToanDisplay = yeuCauKhamNew.BnThanhToan.ApplyNumber();
                        if (phongBenhVien != null && bacSiDangKyDVKB != null)
                        {
                            yeuCauKhamNew.NoiThucHienDisplay =
                                phongBenhVien.KhoaPhong.Ma + " - " + phongBenhVien.Ten + " - " + bacSiDangKyDVKB?.User?.HoTen;
                            yeuCauKhamNew.NoiThucHienId = phongThucHienId;
                        }

                        result.DichVuKhamBenhs.Add(yeuCauKhamNew);
                        break;
                    case EnumNhomGoiDichVu.DichVuKyThuat:
                        if (dichVuBenhVien.DonGiaBenhVien == null)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.GiaBenhVien.NotExists"), dichVuBenhVien.TenDichVu));
                        }
                        var lstPhongDVKTId = new List<long>();
                        var noiThucHienDVKTUuTiens = noiThucHienDVKTs.SelectMany(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId)
                            .OrderByDescending(x => x.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung).ThenBy(x => x.CreatedOn).ToList();
                        if (noiThucHienDVKTUuTiens.Any())
                        {
                            lstPhongDVKTId.Add(noiThucHienDVKTUuTiens.Select(x => x.PhongBenhVienId).First());
                        }
                        else
                        {
                            var noiThucHienDVKTByIds = noiThucHienDVKTs.SelectMany(x => x.DichVuKyThuatBenhVienNoiThucHiens).Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            lstPhongDVKTId.AddRange(noiThucHienDVKTByIds.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList());
                            lstPhongDVKTId.AddRange(noiThucHienDVKTByIds.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong).SelectMany(x => x.PhongBenhViens).Select(x => x.Id).ToList());
                            lstPhongDVKTId.Sort();
                        }
                        var phongThucHienDVKTId = lstPhongDVKTId.First();
                        var phongBenhVienDVKT = await _phongBenhVienRepository.TableNoTracking
                            .Include(x => x.KhoaPhong)
                            .FirstOrDefaultAsync(x => x.Id == phongThucHienDVKTId);
                        var bacSiDangKyDVKT = bacSiDangKys.FirstOrDefault(x => x.PhongBenhVienId == phongThucHienDVKTId);

                        //var entityYeuCauDichVuKyThuat = new YeuCauDichVuKyThuat()
                        //{
                        //    YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                        //    YeuCauKhamBenhId = yeuCauVo.YeuCauKhamBenhId == 0 ? (long?)null : yeuCauVo.YeuCauKhamBenhId,
                        //    DichVuKyThuatBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                        //    NhomDichVuBenhVienId = dichVuBenhVien.NhomDichVuBenhVienId,
                        //    MaDichVu = dichVuBenhVien.MaDichVu,
                        //    TenDichVu = dichVuBenhVien.TenDichVu,
                        //    MaGiaDichVu = dichVuBenhVien.MaGiaDichVu,
                        //    TenGiaDichVu = dichVuBenhVien.TenGia,
                        //    Ma4350DichVu = dichVuBenhVien.Ma4350,
                        //    NhomGiaDichVuKyThuatBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                        //    Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                        //    DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                        //    DuocHuongBaoHiem = dichVuBenhVien.DuocHuongBaoHiem,
                        //    TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,
                        //    NoiThucHienId = phongThucHienDVKTId,
                        //    NhomChiPhi = dichVuBenhVien.NhomChiPhiDichVuKyThuat,
                        //    SoLan = dichVuBenhVien.SoLuong,
                        //    LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(dichVuBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),

                        //    TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                        //    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                        //    BaoHiemChiTra = null,

                        //    NhanVienChiDinhId = currentUserId,
                        //    NoiChiDinhId = phongHienTaiId,
                        //    ThoiDiemChiDinh = thoiDiemHienTai,
                        //    NhanVienThucHienId = bacSiDangKyDVKT?.NhanVienId,

                        //    ThoiDiemDangKy = thoiDiemHienTai,
                        //    TiLeUuDai = null // todo: cần kiểm tra lại
                        //};

                        var yeuCauDVKT = new ChiDinhDichVuKyThuatGridVo
                        {
                            MaDichVuId = dichVuBenhVien.DichVuBenhVienId,
                            Ma = dichVuBenhVien.MaDichVu,
                            TenDichVu = dichVuBenhVien.TenDichVu,
                            SoLuong = dichVuBenhVien.SoLuong,
                            DonGiaDisplay = ((double)dichVuBenhVien.DonGiaBenhVien.Value).ApplyNumber(),
                            DonGia = (double)dichVuBenhVien.DonGiaBenhVien.Value,
                            ThanhTienDisplay = ((double)dichVuBenhVien.DonGiaBenhVien.Value * dichVuBenhVien.SoLuong).ApplyNumber(),
                            ThanhTien = (double)dichVuBenhVien.DonGiaBenhVien.Value * dichVuBenhVien.SoLuong,

                            LoaiGia = dichVuBenhVien.TenNhomGiaDichVuBenhVien,
                            LoaiGiaId = dichVuBenhVien.NhomGiaDichVuBenhVienId,

                            Nhom = Constants.NhomDichVu.DichVuKyThuat,
                            NoiThucHienId = phongThucHienDVKTId.ToString(),
                            KhoaPhongId = phongThucHienDVKTId,

                            GiaBHYT = (double?)dichVuBenhVien.DonGiaBaoHiem ?? 0,
                            TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan ?? 0,
                            DuocHuongBHYT = false,
                        };

                        yeuCauDVKT.BnThanhToan = yeuCauDVKT.ThanhTien;
                        yeuCauDVKT.BnThanhToanDisplay = yeuCauDVKT.BnThanhToan.ApplyNumber();
                        if (phongBenhVienDVKT != null)
                        {
                            yeuCauDVKT.NoiThucHienDisplay =
                                phongBenhVienDVKT.KhoaPhong.Ma + " - " + phongBenhVienDVKT.Ten;
                            yeuCauDVKT.NoiThucHienId = phongThucHienDVKTId.ToString();
                        }
                        result.DichVuKyThuats.Add(yeuCauDVKT);
                        break;
                    case EnumNhomGoiDichVu.DichVuGiuongBenh:
                        break;
                }
            }
            return result;
        }

        #endregion

        #region [PHÁT SINH TRIỂN KHAI] [Tiếp đón] BN BHYT: sửa điều kiện được phép tạo yêu cầu tiếp nhận mới
        public async Task<KetQuaKiemTraTaoMoiYeuCauTiepNhanVo> KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(long benhNhanId, bool? laKiemTraCungNgay = false, long? yeuCauTiepNhanCapNhatId = null)
        {
            // Điều kiện để có thể tạo mới YCTN có BHYT
            // 1. Tất cả các dịch vụ của yctn cũ phải đã thực hiện, thanh toán, xuất
            // 2. Ko có YCTN nào có BHYT
            // Nếu YCTN có BHYT: thì chỉ cho phép tạo mới YCTN ko có thẻ BHYT

            var yeuCauTiepNhans = BaseRepository.TableNoTracking
                //.Include(x => x.YeuCauKhamBenhs)
                //.Include(x => x.YeuCauDichVuKyThuats)
                //.Include(x => x.YeuCauDichVuGiuongBenhViens)
                //.Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(y => y.XuatKhoDuocPhamChiTiet)//.ThenInclude(z => z.XuatKhoDuocPham)
                //.Include(x => x.YeuCauVatTuBenhViens).ThenInclude(y => y.XuatKhoVatTuChiTiet)//.ThenInclude(z => z.XuatKhoVatTu)
                //.Include(x => x.DonThuocThanhToans)//.ThenInclude(y => y.DonThuocThanhToanChiTiets).ThenInclude(z => z.XuatKhoDuocPhamChiTietViTri).ThenInclude(t => t.XuatKhoDuocPhamChiTiet).ThenInclude(u => u.XuatKhoDuocPham)
                //.Include(x => x.DonVTYTThanhToans)//.ThenInclude(y => y.DonVTYTThanhToanChiTiets).ThenInclude(z => z.XuatKhoVatTuChiTietViTri).ThenInclude(t => t.XuatKhoVatTuChiTiet).ThenInclude(u => u.XuatKhoVatTu)
                .Include(x => x.NoiTruBenhAn)
                .Where(x => x.BenhNhanId == benhNhanId
                            && x.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien
                            && (x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                || x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru))
                .ToList();

            var errMessage = string.Empty;
            long? yeuCauTiepNhanIdLoi = null;

            var yctnDaTaoCungNgays = yeuCauTiepNhans
                .Where(x => x.ThoiDiemTiepNhan.Date == DateTime.Now.Date)
                .Select(x => x.MaYeuCauTiepNhan)
                .Distinct().ToList();
            if (yctnDaTaoCungNgays.Any() && yeuCauTiepNhanCapNhatId == null)
            {
                errMessage = string.Format(_localizationService.GetResource("TiepNhanBenhNhan.YeuCauTiepNhan.CungNgay"), string.Join(", ", yctnDaTaoCungNgays));
                var yeuCauTiepNhanLoi = yeuCauTiepNhans
                    .Where(x => x.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                && x.ThoiDiemTiepNhan.Date == DateTime.Now.Date)
                    .FirstOrDefault();
                if (yeuCauTiepNhanLoi != null)
                {
                    yeuCauTiepNhanIdLoi = yeuCauTiepNhanLoi.Id;
                }
            }

            // trường hợp yeuCauTiepNhanHienTaiId != null là đang vào xem chi tiết của yctn -> chỉ cần check qua ngày là có thể show button tạo yctn mới -> qua trang tạo mới thì check all
            else //if (laKiemTraCungNgay != true)
            {
                var maYCTNCanCapNhat = yeuCauTiepNhans.Where(x => x.Id == yeuCauTiepNhanCapNhatId).Select(x => x.MaYeuCauTiepNhan).FirstOrDefault();
                var yctnCoBHYTs = yeuCauTiepNhans.Where(x => x.CoBHYT == true && !x.MaYeuCauTiepNhan.Equals(maYCTNCanCapNhat) && x.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).ToList();
                var yctnNoiTrus = yeuCauTiepNhans.Where(x => x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && !x.MaYeuCauTiepNhan.Equals(maYCTNCanCapNhat)).ToList();

                //==================================== TEMPLATE THEO KH YÊU CẦU (NỘI TRÚ)===================================================
                var templateYCTNDangDieuTriNoiTru = _localizationService.GetResource("TiepNhanBenhNhan.YeuCauTiepNhanNoiTru.DangDieuTri");  //"Mã TN {0} đang điều trị nội trú";
                var templateYCTNChuaQuyetToanNoiTru = _localizationService.GetResource("TiepNhanBenhNhan.YeuCauTiepNhanNoiTru.ChuaQuyetToanVienPhi");  //"Mã TN {0} chưa quyết toán viện phí nội trú";
                //==========================================================================================================================

                if (laKiemTraCungNgay != true)
                {
                    // get template mess
                    var templateYCTN = _localizationService.GetResource("TiepNhanBenhNhan.BenhNhan.CoTiepNhan");  //"BN có mã TN {0} "; => update "Mã TN {0} "
                                                                                                                  //var templateMessTiepNhanCoBH = _localizationService.GetResource("TiepNhanBenhNhan.YeuCauTiepNhan.CoBaoHiem"); // "có BHYT chưa kết thúc. ";
                                                                                                                  //var templateMessDichVu = _localizationService.GetResource("TiepNhanBenhNhan.DichVuBaoHiem.ChuaThanhToanHoacThucHien"); // "có {0} chưa thực hiện xong hoặc chưa thanh toán. ";
                                                                                                                  //var templateMessThuocVatTu = _localizationService.GetResource("TiepNhanBenhNhan.ThuocVatTuBaoHiem.ChuaThanhToanHoacXuat"); // "có {0} chưa xuất hoặc chưa thanh toán. ";

                    //==================================== TEMPLATE THEO KH YÊU CẦU (NGOẠI TRÚ)===================================================
                    var templateMessDichVuChuaThucHien = _localizationService.GetResource("TiepNhanBenhNhan.YeuCauTiepNhan.DichVuChuaThucHien"); // "chưa thực hiện xong DV";
                    var templateMessDichVuChuaThanhToan = _localizationService.GetResource("TiepNhanBenhNhan.YeuCauTiepNhan.DichVuChuaThanhToan"); //"chưa quyết toán viện phí ngoại trú";
                    var templateMessThuocChuaXuat = _localizationService.GetResource("TiepNhanBenhNhan.YeuCauTiepNhan.ThuocChuaXuat"); //"chưa xuất thuốc";
                    var templateMessVatTuChuaXuat = _localizationService.GetResource("TiepNhanBenhNhan.YeuCauTiepNhan.VatTuChuaXuat"); //"chưa xuất vật tư";
                    //============================================================================================================================

                    if (yctnCoBHYTs.Any())
                    {
                        var lstYeuCauTiepNhanNgoaiTruId = yctnCoBHYTs
                            .Where(a => a.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                            .Select(x => x.Id)
                            .Distinct().ToList();

                        var lstThongTinThucHienThanhToan = new List<ThongTinThucHienVaThanhToanDichVuVo>();
                        var lstYCTNIdCoYCDPChuaXuat = new List<long>();
                        var lstYCTNIdCoDonThuocChuaXuat = new List<long>();
                        var lstYCTNIdCoVatTuChuaXuat = new List<long>();
                        if (lstYeuCauTiepNhanNgoaiTruId.Any())
                        {
                            #region Kiểm tra chưa thực hiện dv, chưa thanh toán dv

                            lstThongTinThucHienThanhToan = _yeuCauKhamBenhRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                            && lstYeuCauTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId)
                                            && x.DuocHuongBaoHiem == true
                                            && x.KhongTinhPhi != true
                                            && x.BaoHiemChiTra != false)
                                .Select(x => new ThongTinThucHienVaThanhToanDichVuVo()
                                {
                                    YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                                    ChuaThucHien = x.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham,
                                    ChuaThanhToan = x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                }).Distinct().ToList();

                            var lstThongTinThucHienThanhToanKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                            && lstYeuCauTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId)
                                            && x.DuocHuongBaoHiem == true
                                            && x.KhongTinhPhi != true
                                            && x.BaoHiemChiTra != false)
                                .Select(x => new ThongTinThucHienVaThanhToanDichVuVo()
                                {
                                    YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                                    ChuaThucHien = x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                                    ChuaThanhToan = x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                }).Distinct().ToList();

                            var lstThongTinThucHienThanhToanGiuong = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                            && lstYeuCauTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId)
                                            && x.DuocHuongBaoHiem == true
                                            && x.KhongTinhPhi != true
                                            && x.BaoHiemChiTra != false)
                                .Select(x => new ThongTinThucHienVaThanhToanDichVuVo()
                                {
                                    YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                                    ChuaThucHien = x.TrangThai != EnumTrangThaiGiuongBenh.DaThucHien,
                                    ChuaThanhToan = x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                }).Distinct().ToList();

                            var lstThongTinThanhToanYCDuocPham = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && lstYeuCauTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId)
                                            && x.DuocHuongBaoHiem == true
                                            && x.KhongTinhPhi != true
                                            && x.BaoHiemChiTra != false)
                                .Select(x => new ThongTinThucHienVaThanhToanDichVuVo()
                                {
                                    YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                                    ChuaThanhToan = true
                                }).Distinct().ToList();

                            var lstThongTinThanhToanDonThuoc = _donThuocThanhToanRepository.TableNoTracking
                                .Where(x => x.TrangThai != TrangThaiDonThuocThanhToan.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && x.YeuCauTiepNhanId != null
                                            && lstYeuCauTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId.Value)
                                            && x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT)
                                .Select(x => new ThongTinThucHienVaThanhToanDichVuVo()
                                {
                                    YeuCauTiepNhanId = x.YeuCauTiepNhanId.Value,
                                    ChuaThanhToan = true
                                }).Distinct().ToList();

                            var lstThongTinThanhToanVatTu = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                            && lstYeuCauTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId)
                                            && x.DuocHuongBaoHiem == true
                                            && x.KhongTinhPhi != true
                                            && x.BaoHiemChiTra != false)
                                .Select(x => new ThongTinThucHienVaThanhToanDichVuVo()
                                {
                                    YeuCauTiepNhanId = x.YeuCauTiepNhanId,
                                    ChuaThanhToan = true
                                }).Distinct().ToList();

                            lstThongTinThucHienThanhToan
                                .Union(lstThongTinThucHienThanhToanKyThuat)
                                .Union(lstThongTinThucHienThanhToanGiuong)
                                .Union(lstThongTinThanhToanYCDuocPham)
                                .Union(lstThongTinThanhToanDonThuoc)
                                .Union(lstThongTinThanhToanVatTu)
                                .ToList();
                            #endregion

                            #region Kiểm tra thuốc chưa xuất
                            lstYCTNIdCoYCDPChuaXuat = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                            && lstYeuCauTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId)
                                            && (x.XuatKhoDuocPhamChiTiet == null || x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null)
                                            && x.DuocHuongBaoHiem == true
                                            && x.KhongTinhPhi != true
                                            && x.BaoHiemChiTra != false)
                                .Select(x => x.YeuCauTiepNhanId)
                                .Distinct().ToList();

                            lstYCTNIdCoDonThuocChuaXuat = _donThuocThanhToanRepository.TableNoTracking
                                .Where(x => x.TrangThai != TrangThaiDonThuocThanhToan.DaHuy
                                            && x.YeuCauTiepNhanId != null
                                            && lstYeuCauTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId.Value)
                                            && x.TrangThai != TrangThaiDonThuocThanhToan.DaXuatThuoc
                                            && x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT)
                                .Select(x => x.YeuCauTiepNhanId.Value)
                                .Distinct().ToList();
                            #endregion

                            #region Kiểm tra vật tư chưa xuất
                            lstYCTNIdCoVatTuChuaXuat = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                            && lstYeuCauTiepNhanNgoaiTruId.Contains(x.YeuCauTiepNhanId)
                                            && (x.XuatKhoVatTuChiTiet == null || x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null)
                                            && x.DuocHuongBaoHiem == true
                                            && x.KhongTinhPhi != true
                                            && x.BaoHiemChiTra != false)
                                .Select(x => x.YeuCauTiepNhanId)
                                .Distinct().ToList();
                            #endregion
                        }


                        foreach (var yeuCauTiepNhan in yctnCoBHYTs)
                        {
                            List<string> errDichVus = new List<string>();
                            List<string> errThuocVaTus = new List<string>();
                            var err = string.Empty;

                            if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                            {
                                //if (yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien == null || yeuCauTiepNhan.NoiTruBenhAn?.DaQuyetToan != true)
                                //{
                                //    if (yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien == null)
                                //    {
                                //        err = string.Format(templateYCTNDangDieuTriNoiTru, yeuCauTiepNhan.MaYeuCauTiepNhan);
                                //    }
                                //    else if (yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien != null && yeuCauTiepNhan.NoiTruBenhAn?.DaQuyetToan != true)
                                //    {
                                //        err = string.Format(templateYCTNChuaQuyetToanNoiTru, yeuCauTiepNhan.MaYeuCauTiepNhan);
                                //    }
                                //}
                            }
                            else
                            {
                                #region Kiểm tra chưa thực hiện dv
                                //if (yeuCauTiepNhan.YeuCauKhamBenhs.Any(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                //                                            && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham
                                //                                            && x.DuocHuongBaoHiem == true
                                //                                            && x.KhongTinhPhi != true
                                //                                            && x.BaoHiemChiTra != false)
                                //    || yeuCauTiepNhan.YeuCauDichVuKyThuats.Any(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                //                                                    && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                //                                                    && x.DuocHuongBaoHiem == true
                                //                                                    && x.KhongTinhPhi != true
                                //                                                    && x.BaoHiemChiTra != false)
                                //    || yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                //                                                           && x.TrangThai != EnumTrangThaiGiuongBenh.DaThucHien
                                //                                                           && x.DuocHuongBaoHiem == true
                                //                                                           && x.KhongTinhPhi != true
                                //                                                           && x.BaoHiemChiTra != false))
                                //{
                                //    errDichVus.Add(templateMessDichVuChuaThucHien);
                                //}

                                var lstYCTNChuaThucHienId = lstThongTinThucHienThanhToan
                                    .Where(x => x.ChuaThucHien)
                                    .Select(x => x.YeuCauTiepNhanId).Distinct().ToList();
                                if (lstYCTNChuaThucHienId.Contains(yeuCauTiepNhan.Id))
                                {
                                    errDichVus.Add(templateMessDichVuChuaThucHien);
                                }
                                #endregion

                                #region Kiểm tra chưa thanh toán dv
                                //if (yeuCauTiepNhan.YeuCauKhamBenhs.Any(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                //                                            && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                //                                            && x.DuocHuongBaoHiem == true
                                //                                            && x.KhongTinhPhi != true
                                //                                            && x.BaoHiemChiTra != false)
                                //    || yeuCauTiepNhan.YeuCauDichVuKyThuats.Any(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                //                                                    && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                //                                                    && x.DuocHuongBaoHiem == true
                                //                                                    && x.KhongTinhPhi != true
                                //                                                    && x.BaoHiemChiTra != false)
                                //    || yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(x => x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                //                                                           && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                //                                                           && x.DuocHuongBaoHiem == true
                                //                                                           && x.KhongTinhPhi != true
                                //                                                           && x.BaoHiemChiTra != false)
                                //    || yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Any(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                //                                                       && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                //                                                       && x.DuocHuongBaoHiem == true
                                //                                                       && x.KhongTinhPhi != true
                                //                                                       && x.BaoHiemChiTra != false)
                                //    || yeuCauTiepNhan.DonThuocThanhToans.Any(x => x.TrangThai != TrangThaiDonThuocThanhToan.DaHuy
                                //                                                  && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                //                                                  && x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT)
                                //    || yeuCauTiepNhan.YeuCauVatTuBenhViens.Any(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                //                                                    && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                                //                                                    && x.DuocHuongBaoHiem == true
                                //                                                    && x.KhongTinhPhi != true
                                //                                                    && x.BaoHiemChiTra != false))
                                //{
                                //    errDichVus.Add(templateMessDichVuChuaThanhToan);
                                //}

                                var lstYCTNChuaThanhToanId = lstThongTinThucHienThanhToan
                                    .Where(x => x.ChuaThanhToan)
                                    .Select(x => x.YeuCauTiepNhanId).Distinct().ToList();
                                if (lstYCTNChuaThanhToanId.Contains(yeuCauTiepNhan.Id))
                                {
                                    errDichVus.Add(templateMessDichVuChuaThanhToan);
                                }
                                #endregion

                                #region Kiểm tra thuốc chưa xuất
                                //if (yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Any(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                //                                                    && (x.XuatKhoDuocPhamChiTiet == null || x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == null)
                                //                                                    && x.DuocHuongBaoHiem == true
                                //                                                    && x.KhongTinhPhi != true
                                //                                                    && x.BaoHiemChiTra != false)
                                //   || yeuCauTiepNhan.DonThuocThanhToans.Any(x => x.TrangThai != TrangThaiDonThuocThanhToan.DaHuy
                                //                                                 && x.TrangThai != TrangThaiDonThuocThanhToan.DaXuatThuoc
                                //                                                 && x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT))
                                //{
                                //    errThuocVaTus.Add(templateMessThuocChuaXuat);
                                //}

                                if (lstYCTNIdCoYCDPChuaXuat.Contains(yeuCauTiepNhan.Id)
                                    || lstYCTNIdCoDonThuocChuaXuat.Contains(yeuCauTiepNhan.Id))
                                {
                                    errThuocVaTus.Add(templateMessThuocChuaXuat);
                                }
                                #endregion

                                #region Kiểm tra vật tư chưa xuất
                                //if (yeuCauTiepNhan.YeuCauVatTuBenhViens.Any(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                                //                                              && (x.XuatKhoVatTuChiTiet == null || x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == null)
                                //                                              && x.DuocHuongBaoHiem == true
                                //                                              && x.KhongTinhPhi != true
                                //                                              && x.BaoHiemChiTra != false))
                                //{
                                //    errThuocVaTus.Add(templateMessVatTuChuaXuat);
                                //}

                                if (lstYCTNIdCoVatTuChuaXuat.Contains(yeuCauTiepNhan.Id))
                                {
                                    errThuocVaTus.Add(templateMessVatTuChuaXuat);
                                }
                                #endregion

                                
                                //err = string.Format(templateYCTN, yeuCauTiepNhan.MaYeuCauTiepNhan);
                                if (!errDichVus.Any() && !errThuocVaTus.Any())
                                {
                                    //maYCTNCoBHDaHoanTatDichVus.Add(yeuCauTiepNhan.MaYeuCauTiepNhan);
                                }
                                else
                                {
                                    err = string.Format(templateYCTN, yeuCauTiepNhan.MaYeuCauTiepNhan);
                                    if (errDichVus.Any())
                                    {
                                        //err += string.Format(templateMessDichVu, string.Join(", ", errDichVus));
                                        err += string.Join(", ", errDichVus);
                                    }

                                    if (errThuocVaTus.Any())
                                    {
                                        //err += (!string.IsNullOrEmpty(err) ? ", " : "") + string.Format(templateMessThuocVatTu, string.Join(", ", errThuocVaTus));
                                        err += (!string.IsNullOrEmpty(err) ? ", " : "") + string.Join(", ", errThuocVaTus);
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(err))
                            {
                                errMessage += err + ". ";

                                // chỉ lấy yctn ngoại trú
                                if (yeuCauTiepNhanIdLoi == null && yeuCauTiepNhan.LoaiYeuCauTiepNhan != EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                                {
                                    yeuCauTiepNhanIdLoi = yeuCauTiepNhan.Id;
                                }
                            }
                        }
                    }
                }

                errMessage += KiemTraDieuTriNoiTruChuaHoanThanh(yctnNoiTrus, templateYCTNDangDieuTriNoiTru, templateYCTNChuaQuyetToanNoiTru);
            }

            return new KetQuaKiemTraTaoMoiYeuCauTiepNhanVo()
            {
                ErrorMessage = errMessage,
                YeuCauTiepNhanId = yeuCauTiepNhanIdLoi
            };
        }

        private string KiemTraDieuTriNoiTruChuaHoanThanh(List<YeuCauTiepNhan> yctnNoiTrus, string templateYCTNDangDieuTriNoiTru, string templateYCTNChuaQuyetToanNoiTru)
        {
            var strErr = string.Empty;
            if (yctnNoiTrus.Any())
            {
                foreach (var yeuCauTiepNhan in yctnNoiTrus)
                {
                    var err = string.Empty;

                    if (yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien == null || yeuCauTiepNhan.NoiTruBenhAn?.DaQuyetToan != true)
                    {
                        if (yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien == null)
                        {
                            err = string.Format(templateYCTNDangDieuTriNoiTru, yeuCauTiepNhan.MaYeuCauTiepNhan);
                        }
                        else if (yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien != null && yeuCauTiepNhan.NoiTruBenhAn?.DaQuyetToan != true)
                        {
                            err = string.Format(templateYCTNChuaQuyetToanNoiTru, yeuCauTiepNhan.MaYeuCauTiepNhan);
                        }
                    }

                    if (!string.IsNullOrEmpty(err))
                    {
                        strErr += err + ". ";
                    }
                }
            }
            return strErr;
        }

        #endregion

        #region [PHÁT SINH TRIỂN KHAI][THU NGÂN] YÊU CẦU HIỂN THỊ CẢNH BÁO KHI THÊM NB CÓ TÊN, NGÀY SINH VÀ SĐT GIỐNG NHAU
        public async Task<KiemTraNguoiBenhTrongHeThongVo> KiemTraBenhNhanTrongHeThongAsync(KiemTraNguoiBenhTrongHeThongVo kiemTraVo)
        {
            var kiemTraResult = new KiemTraNguoiBenhTrongHeThongVo();
            if (kiemTraVo.CoBHYT == true && !string.IsNullOrEmpty(kiemTraVo.BHYTMaSoThe))
            {
                var benhNhans = await _benhNhanRepository.TableNoTracking
                    .Where(x => x.BHYTMaSoThe.Contains(kiemTraVo.BHYTMaSoThe))
                    .ToListAsync();
                if (benhNhans.Any())
                {
                    if(benhNhans.Any(x => x.HoTen.Contains(kiemTraVo.HoTen) 
                                          && (kiemTraVo.NgayThangNamSinh == null || (x.ThangSinh == kiemTraVo.ThangSinh
                                                                                     && x.NgaySinh == kiemTraVo.NgaySinh
                                                                                     && x.NamSinh == kiemTraVo.NamSinh))
                                          && (kiemTraVo.NamSinh == null || x.NamSinh == kiemTraVo.NamSinh)
                                          && (string.IsNullOrEmpty(kiemTraVo.SoDienThoai) || kiemTraVo.SoDienThoai.Contains(x.SoDienThoai))))
                    {
                        kiemTraResult.Message = _localizationService.GetResource("TiepNhanBenhNhan.NguoiBenh.IsExists");
                    }
                    else
                    {
                        kiemTraResult.Message = _localizationService.GetResource("TiepNhanBenhNhan.MaTheBHYT.IsUsed");
                    }
                }
                else
                {
                    kiemTraResult = null;
                }
            }
            else
            {
                var benhNhans = await _benhNhanRepository.TableNoTracking
                    .Where(x => x.HoTen.Contains(kiemTraVo.HoTen)
                                && (kiemTraVo.NgayThangNamSinh == null || (x.ThangSinh == kiemTraVo.ThangSinh
                                                                           && x.NgaySinh == kiemTraVo.NgaySinh
                                                                           && x.NamSinh == kiemTraVo.NamSinh))
                                && (kiemTraVo.NamSinh == null || x.NamSinh == kiemTraVo.NamSinh)
                                && (string.IsNullOrEmpty(kiemTraVo.SoDienThoai) || kiemTraVo.SoDienThoai.Contains(x.SoDienThoai)))
                    .ToListAsync();
                if (benhNhans.Any())
                {
                    kiemTraResult.Message = _localizationService.GetResource("TiepNhanBenhNhan.NguoiBenh.IsExists");
                }
                else
                {
                    kiemTraResult = null;
                }
            }

            return kiemTraResult;
        }
        #endregion
        #region BVHD-3761
        public async Task<DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham> GetSarsCoVs()
        {
            var lstDichVuSarCoVs = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhTiepNhan.DichVuTestSarsCovid")
                .Select(d=>d.Value).FirstOrDefault();

            var json = JsonConvert.DeserializeObject<List<DichVuKyThuatBenhVienIdsSarsCoV>>(lstDichVuSarCoVs);
            var dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham = new DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham();
            dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham.Ids = json.Select(d => d.DichVuKyThuatBenhVienId).ToList();

            dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham.LoaiMauXetNghiem = Enums.EnumLoaiMauXetNghiem.DichTyHau;
            dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham.LoaiMauXetNghiemText = Enums.EnumLoaiMauXetNghiem.DichTyHau.GetDescription();

            return dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham;
        }
        public async Task<List<long>> GetKiemTraYeuCauDichVuKyThuatThuocNhomSarsCov2(List<long> yeuCauDichVuKyThuatIds)
        {
            var lstDichVuSarCoVs = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhTiepNhan.DichVuTestSarsCovid")
                .Select(d => d.Value).FirstOrDefault();

            var json = JsonConvert.DeserializeObject<List<DichVuKyThuatBenhVienIdsSarsCoV>>(lstDichVuSarCoVs);
            var dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham = new DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham();
            dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham.Ids = json.Select(d => d.DichVuKyThuatBenhVienId).ToList();
            var listIds = _yeuCauDichVuKyThuatRepository.TableNoTracking
                         .Where(d => yeuCauDichVuKyThuatIds.Contains(d.Id) &&
                                    !dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham.Ids.Contains(d.DichVuKyThuatBenhVienId))
                         .Select(d => d.Id).ToList();

            return listIds;
        }
        public async Task<InfoSarsCoVTheoYeuCauTiepNhan> GetInfoSarsCoVTheoYeuCauTiepNhan(long id)
        {
            var info = BaseRepository.TableNoTracking.Where(d => d.Id == id)
                       .Select(d => new InfoSarsCoVTheoYeuCauTiepNhan
                       {
                           BieuHienLamSang = d.BieuHienLamSang,
                           DichTeSarsCoV2 = d.DichTeSarsCoV2,
                           LoaiMauXetNghiem = Enums.EnumLoaiMauXetNghiem.DichTyHau,
                           LoaiMauXetNghiemText = Enums.EnumLoaiMauXetNghiem.DichTyHau.GetDescription()
                       }).First();
            return info;
        }
        public async Task<InfoSarsCoVTheoYeuCauTiepNhan> GetInfoSarsCoVTheoYeuCauTiepNhanNoiTru(long id)
        {
            var info = BaseRepository.TableNoTracking.Where(d => d.Id == id)
                       .Select(d => new InfoSarsCoVTheoYeuCauTiepNhan
                       {
                           BieuHienLamSang = d.BieuHienLamSang,
                           DichTeSarsCoV2 = d.DichTeSarsCoV2,
                           LoaiMauXetNghiem = Enums.EnumLoaiMauXetNghiem.DichTyHau,
                           LoaiMauXetNghiemText = Enums.EnumLoaiMauXetNghiem.DichTyHau.GetDescription()
                       }).First();
            if(string.IsNullOrEmpty(info.BieuHienLamSang) && string.IsNullOrEmpty(info.BieuHienLamSang))
            {
                // kiểm tra tiếp nhận ngoại trú có test sars cov chưa . nếu có lấy show lên trong nội trú
                var YeuCauTiepNhanNgoaiTruId = BaseRepository.TableNoTracking.Where(d => d.Id == id)
                    .Select(d => d.YeuCauTiepNhanNgoaiTruCanQuyetToanId)
                    .First();
                if(YeuCauTiepNhanNgoaiTruId != null)
                {
                    var inFo = BaseRepository.TableNoTracking
                         .Where(d => d.Id == YeuCauTiepNhanNgoaiTruId)
                         .Select(d => new InfoSarsCoVTheoYeuCauTiepNhan
                         {
                             BieuHienLamSang = d.BieuHienLamSang,
                             DichTeSarsCoV2 = d.DichTeSarsCoV2,
                             LoaiMauXetNghiem = Enums.EnumLoaiMauXetNghiem.DichTyHau,
                             LoaiMauXetNghiemText = Enums.EnumLoaiMauXetNghiem.DichTyHau.GetDescription()
                         }).First();
                    return inFo;
                }
                return info;
            }
            return info;
        }
        #endregion


        #region BVHD-3825
        public async Task KiemTraSoLuongConLaiCuaDichVuKhuyenMaiTrongGoiAsync(ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo yeuCauVo)
        {
            var lstDichVuDaChon = yeuCauVo.DichVus;
            var lstDichVuKhamBenhTrongGoi = new List<DichVuBenhVienTheoGoiDichVuVo>();
            var lstDichVuSoLuongConLaiKhongDu = new List<string>();
            var lstDichVuKhongCoTrongGoi = new List<string>();
            var lstDichVuHetHanSuDung = new List<string>();

            var lstYeuCauGoiDichVuIds = lstDichVuDaChon.Select(a => a.YeuCauGoiDichVuId).ToList();
            var yeuCauGoiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs).ThenInclude(z => z.DichVuKhamBenhBenhVien)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien)
                .Include(u => u.MienGiamChiPhis).ThenInclude(o => o.YeuCauDichVuKyThuat)
                .Include(u => u.MienGiamChiPhis).ThenInclude(o => o.YeuCauKhamBenh)
                .Include(u => u.MienGiamChiPhis).ThenInclude(o => o.TaiKhoanBenhNhanThu)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(z => z.DichVuGiuongBenhVien).ThenInclude(t => t.YeuCauDichVuGiuongBenhViens)
                .Where(x => lstYeuCauGoiDichVuIds.Contains(x.Id))
                .ToList();
            foreach (var yeuCauGoiDichVu in yeuCauGoiDichVus)
            {
                // dịch vụ khám
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKhamBenhBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh))
                    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                        TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                        SoLanDaSuDung = yeuCauGoiDichVu.MienGiamChiPhis
                            .Where(o => o.DaHuy != true).Where(o =>
                                o.YeuCauKhamBenh != null &&
                                o.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                                o.YeuCauKhamBenh.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId &&
                                o.YeuCauKhamBenh.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId &&
                                (o.TaiKhoanBenhNhanThuId == null || o.TaiKhoanBenhNhanThu.DaHuy != true))
                            .Select(x => x.YeuCauKhamBenhId)
                            .Distinct().Count(),
                        SoLanTheoGoi = item.SoLan,
                        HanSuDung = yeuCauGoiDichVu.ThoiDiemChiDinh.AddDays(item.SoNgaySuDung)
                    }));

                // dịch vụ kỹ thuật
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKyThuatBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat))
                    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                        TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                        SoLanDaSuDung = yeuCauGoiDichVu.MienGiamChiPhis
                            .Where(o => o.DaHuy != true &&
                                o.YeuCauDichVuKyThuat != null &&
                                o.YeuCauDichVuKyThuat.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId &&
                                o.YeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId &&
                                (o.TaiKhoanBenhNhanThuId == null || o.TaiKhoanBenhNhanThu.DaHuy != true))
                            .Select(o => new {o.YeuCauDichVuKyThuatId, o.YeuCauDichVuKyThuat.SoLan})
                            .GroupBy(x => x.YeuCauDichVuKyThuatId)
                            .Select(x => x.First().SoLan).DefaultIfEmpty().Sum(),
                        SoLanTheoGoi = item.SoLan,
                        HanSuDung = yeuCauGoiDichVu.ThoiDiemChiDinh.AddDays(item.SoNgaySuDung)
                    }));

                //// dịch vụ giường
                //lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                //    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                //                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                //                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                //                                         && a.DichVuBenhVienId == x.DichVuGiuongBenhVienId
                //                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh))
                //    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                //    {
                //        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                //        DichVuBenhVienId = item.DichVuGiuongBenhVienId,
                //        TenDichVu = item.DichVuGiuongBenhVien.Ten,
                //        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                //        SoLanDaSuDung = item.DichVuGiuongBenhVien.YeuCauDichVuGiuongBenhViens.Count(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                //                                                                                 && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id),
                //        SoLanTheoGoi = item.SoLan
                //    }));
            }

            foreach (var dichVuDaChon in lstDichVuDaChon)
            {
                var dichVuTrongGoi = lstDichVuKhamBenhTrongGoi.FirstOrDefault(x => x.YeuCauGoiDichVuId == dichVuDaChon.YeuCauGoiDichVuId
                                                                                   && x.DichVuBenhVienId == dichVuDaChon.DichVuBenhVienId);
                if (dichVuTrongGoi == null)
                {
                    lstDichVuKhongCoTrongGoi.Add(dichVuDaChon.TenDichVu);
                }
                else if (dichVuTrongGoi.SoLanConLai < dichVuDaChon.SoLuongSuDung)
                {
                    lstDichVuSoLuongConLaiKhongDu.Add(dichVuDaChon.TenDichVu);
                }
                else if (!dichVuTrongGoi.ConHanSuDung)
                {
                    lstDichVuHetHanSuDung.Add(dichVuDaChon.TenDichVu);
                }
            }

            if (lstDichVuKhongCoTrongGoi.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.NotExists"), lstDichVuKhongCoTrongGoi.Join(",")));
            }
            if (lstDichVuSoLuongConLaiKhongDu.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVuSoLuongConLai.NotEnough"), lstDichVuSoLuongConLaiKhongDu.Join(",")));
            }

            if (lstDichVuHetHanSuDung.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVuSoLuongConLai.Expired"), lstDichVuHetHanSuDung.Join(",")));
            }

            var lstGoiDaQuyetToan = yeuCauGoiDichVus.Where(x => x.DaQuyetToan == true).ToList();
            if (lstGoiDaQuyetToan.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.DaQuetToan"), lstGoiDaQuyetToan.Select(x => x.TenChuongTrinh).Join(",")));
            }

            var lstGoiDangNgungSuDung = yeuCauGoiDichVus.Where(x => x.NgungSuDung == true).ToList();
            if (lstGoiDangNgungSuDung.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.NgungSuDung"), lstGoiDangNgungSuDung.Select(x => x.TenChuongTrinh).Join(",")));
            }
        }

        public async Task<List<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo>> KiemTraValidationChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(long yeuCauTiepNhanId, List<string> lstGoiDichVuId, long? noiTruPhieuDieuTriId = null)
        {
            var lstDichVuCanhBao = new List<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo>();
            var lstDichVuDaChon = new List<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo>();
            foreach (var dichVu in lstGoiDichVuId)
            {
                var dichVuObj = JsonConvert.DeserializeObject<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo>(dichVu);
                lstDichVuDaChon.Add(dichVuObj);
            }

            var lstTenDichVuBiTrung = new List<ChiDinhGoiDichVuThuongDungDichVuLoiVo>();
            var lstDichVuTrungDaChon = lstDichVuDaChon
                .GroupBy(x => new { x.DichVuBenhVienId, x.NhomGoiDichVu })
                .Select(item => new ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanVo()
                {
                    DichVuBenhVienId = item.First().DichVuBenhVienId,
                    NhomGoiDichVu = item.First().NhomGoiDichVu,
                    TenDichVu = item.First().TenDichVu,
                    SoLuongSuDung = item.Count()
                }).Where(x => x.SoLuongSuDung > 1)
                .ToList();

            DateTime? ngayDieuTri = null;
            long? tiepNhanNgoaiTruId = null;
            if (noiTruPhieuDieuTriId != null)
            {
                var noiTruPhieuDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.YeuCauTiepNhan)
                    .FirstOrDefault(x => x.Id == noiTruPhieuDieuTriId.Value);
                ngayDieuTri = noiTruPhieuDieuTri?.NgayDieuTri.Date;
                tiepNhanNgoaiTruId = noiTruPhieuDieuTri?.NoiTruBenhAn?.YeuCauTiepNhan?.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
            }

            //Cập nhật 02/12/2022
            var lstDichVuKhamIdDaChon = lstDichVuDaChon.Where(x => x.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.DichVuBenhVienId).Distinct().ToList();
            var lstDichVuKyThuatIdDaChon = lstDichVuDaChon.Where(x => x.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.DichVuBenhVienId).Distinct().ToList();

            var lstDichVuTrungDichVuDaThem = new List<ChiTietNhomGoiDichVuThuongDungDangChonVo>();
            if (lstDichVuKyThuatIdDaChon.Any())
            {
                var kts = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                //&& x.NoiTruPhieuDieuTriId == noiTruPhieuDieuTriId
                                && (ngayDieuTri == null || (x.NoiTruPhieuDieuTri != null && x.NoiTruPhieuDieuTri.NgayDieuTri.Date == ngayDieuTri.Value.Date))

                                //Cập nhật 02/12/2022
                                //&& lstDichVuDaChon.Any(a => a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat
                                //                                   && a.DichVuBenhVienId == x.DichVuKyThuatBenhVienId)
                                && lstDichVuKyThuatIdDaChon.Contains(x.DichVuKyThuatBenhVienId)

                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                    {
                        NhomDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        DichVuId = item.DichVuKyThuatBenhVienId,
                        TenDichVu = item.TenDichVu
                    })
                    .ToList();
                if (kts.Any())
                {
                    lstDichVuTrungDichVuDaThem.AddRange(kts);
                }
            }

            if(lstDichVuKhamIdDaChon.Any())
            {
                var khams = _yeuCauKhamBenhRepository.TableNoTracking
                            .Where(x => ((noiTruPhieuDieuTriId == null && x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                                         //BVHD-3575: cập nhật cho phép chỉ định dịch vụ khám từ nội trú
                                         //|| (noiTruPhieuDieuTriId != null && x.Id == 0)) // nội trú thì chỉ kiểm tra dịch vụ kỹ thuật
                                         || (noiTruPhieuDieuTriId != null
                                             && x.YeuCauTiepNhanId == tiepNhanNgoaiTruId
                                             && x.LaChiDinhTuNoiTru != null && x.LaChiDinhTuNoiTru == true
                                             && x.ThoiDiemDangKy.Date == ngayDieuTri))

                                        //Cập nhật 02/12/2022
                                        //&& lstDichVuDaChon.Any(a => a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKhamBenh
                                        //                                   && a.DichVuBenhVienId == x.DichVuKhamBenhBenhVienId)
                                        && lstDichVuKhamIdDaChon.Contains(x.DichVuKhamBenhBenhVienId)

                                        && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                            .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                            {
                                NhomDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                DichVuId = item.DichVuKhamBenhBenhVienId,
                                TenDichVu = item.TenDichVu
                            })
                            .ToList();
                if (khams.Any())
                {
                    lstDichVuTrungDichVuDaThem.AddRange(khams);
                }
            }

            lstDichVuTrungDichVuDaThem = lstDichVuTrungDichVuDaThem.Distinct().ToList();
                    //.Union(
                    //    _yeuCauDichVuGiuongRepository.TableNoTracking
                    //        .Where(x => ((noiTruPhieuDieuTriId == null && x.YeuCauTiepNhanId == yeuCauTiepNhanId) || (noiTruPhieuDieuTriId != null && x.Id == 0)) // nội trú thì chỉ kiểm tra dịch vụ kỹ thuật
                    //                    && lstDichVuDaChon.Any(a => a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuGiuongBenh
                    //                                                       && a.DichVuBenhVienId == x.DichVuGiuongBenhVienId)
                    //                    && x.TrangThai != EnumTrangThaiGiuongBenh.DaHuy)
                    //        .Select(item => new ChiTietNhomGoiDichVuThuongDungDangChonVo()
                    //        {
                    //            NhomDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                    //            DichVuId = item.DichVuGiuongBenhVienId,
                    //            TenDichVu = item.Ten
                    //        })
                    //)
                    //.ToList();

            foreach (var dichVu in lstDichVuDaChon)
            {
                var dichVuCanhBao = lstDichVuCanhBao.FirstOrDefault(x => x.YeuCauGoiDichVuId == dichVu.YeuCauGoiDichVuId
                                                                       && x.ChuongTrinhGoiDichVuId == dichVu.ChuongTrinhGoiDichVuId
                                                                       && x.ChuongTrinhGoiDichVuChiTietId == dichVu.ChuongTrinhGoiDichVuChiTietId
                                                                       && x.DichVuId == dichVu.DichVuBenhVienId
                                                                       && (int)x.NhomGoiDichVu == dichVu.NhomGoiDichVu);
                if (dichVuCanhBao == null)
                {
                    dichVuCanhBao = new ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo()
                    {
                        YeuCauGoiDichVuId = dichVu.YeuCauGoiDichVuId,
                        ChuongTrinhGoiDichVuId = dichVu.ChuongTrinhGoiDichVuId,
                        ChuongTrinhGoiDichVuChiTietId = dichVu.ChuongTrinhGoiDichVuChiTietId,
                        DichVuId = dichVu.DichVuBenhVienId,
                        TenDichVu = dichVu.TenDichVu,
                        NhomGoiDichVuValue = dichVu.NhomGoiDichVu,
                        TenGoiDichVu = dichVu.TenGoiDichVu
                    };
                }

                if (lstDichVuTrungDaChon.Any(x => x.DichVuBenhVienId == dichVu.DichVuBenhVienId
                                                  && x.NhomGoiDichVu == dichVu.NhomGoiDichVu))
                {
                    dichVuCanhBao.LoaiLois.Add(Enums.LoaiLoiGoiDichVu.Trung.GetDescription());
                    lstDichVuCanhBao.Add(dichVuCanhBao);
                }
                else if (lstDichVuTrungDichVuDaThem.Any(x => x.DichVuId == dichVu.DichVuBenhVienId
                                                  && (int)x.NhomDichVu == dichVu.NhomGoiDichVu))
                {
                    dichVuCanhBao.LoaiLois.Add(Enums.LoaiLoiGoiDichVu.Trung.GetDescription());
                    lstDichVuCanhBao.Add(dichVuCanhBao);
                }
            }

            return lstDichVuCanhBao.Distinct().ToList();
        }

        public async Task XuLyThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(YeuCauTiepNhan yeuCauTiepNhan, ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo yeuCauVo)
        {
            //todo: có cập nhật bỏ await
            var coBHYT = yeuCauTiepNhan.CoBHYT ?? false;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thoiDiemHienTai = DateTime.Now;
            var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
            var lstDichVuDaChon = yeuCauVo.DichVus;

            var lstDichVuKhamBenhTrongGoi = new List<DichVuBenhVienKhuyenMaiTheoGoiDichVuVo>();

            //có cập nhật: bỏ await
            var lstYeuCauGoiDichVuIds = lstDichVuDaChon.Select(x => x.YeuCauGoiDichVuId).ToList();
            var yeuCauGoiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs).ThenInclude(z => z.DichVuKhamBenhBenhVien).ThenInclude(t => t.DichVuKhamBenh)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs).ThenInclude(z => z.DichVuKhamBenhBenhVien).ThenInclude(t => t.YeuCauKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs).ThenInclude(z => z.DichVuKhamBenhBenhVien).ThenInclude(t => t.DichVuKhamBenhBenhVienGiaBaoHiems)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien).ThenInclude(t => t.DichVuKyThuat)
               // .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien).ThenInclude(t => t.YeuCauDichVuKyThuats)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien).ThenInclude(t => t.DichVuKyThuatBenhVienGiaBaoHiems)

                #region  Cập nhật 15/12/2022
                //.Include(x => x.MienGiamChiPhis).ThenInclude(y => y.TaiKhoanBenhNhanThu)
                //.Include(x => x.MienGiamChiPhis).ThenInclude(y => y.YeuCauKhamBenh)
                //.Include(x => x.MienGiamChiPhis).ThenInclude(y => y.YeuCauDichVuKyThuat)
                #endregion
                .Where(x => lstYeuCauGoiDichVuIds.Contains(x.Id))
                .ToList();

            #region Cập nhật 15/12/2022
            var dvKhamDaDungs = _mienGiamChiPhiRepository.TableNoTracking
                .Where(x => x.DaHuy != true
                            && x.YeuCauGoiDichVuId != null
                            && x.YeuCauKhamBenhId != null 
                            && x.YeuCauKhamBenh.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)
                            && lstYeuCauGoiDichVuIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                    NhomGiaId = x.YeuCauKhamBenh.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = 1,
                    YeuCauDichVuId = x.YeuCauKhamBenhId
                }).ToList();

            dvKhamDaDungs = dvKhamDaDungs
                .GroupBy(x => new { x.YeuCauDichVuId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.First().YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.First().DichVuBenhVienId,
                    NhomGiaId = x.First().NhomGiaId,
                    SoLuong = 1
                })
                .GroupBy(x => new { x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId})
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.Key.DichVuBenhVienId,
                    NhomGiaId = x.Key.NhomGiaId,
                    SoLuong = x.Sum(a => a.SoLuong)
                })
                .ToList();

            var dvKyThuatDaDungs = _mienGiamChiPhiRepository.TableNoTracking
                .Where(x => x.DaHuy != true
                            && x.YeuCauGoiDichVuId != null
                            && x.YeuCauDichVuKyThuatId != null
                            && x.YeuCauDichVuKyThuat.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)
                            && lstYeuCauGoiDichVuIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = x.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                    NhomGiaId = x.YeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = x.YeuCauDichVuKyThuat.SoLan,
                    YeuCauDichVuId = x.YeuCauDichVuKyThuatId
                }).ToList();

            dvKyThuatDaDungs = dvKyThuatDaDungs
                .GroupBy(x => new { x.YeuCauDichVuId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.First().YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.First().DichVuBenhVienId,
                    NhomGiaId = x.First().NhomGiaId,
                    SoLuong = x.First().SoLuong
                })
                .GroupBy(x => new { x.YeuCauGoiDichVuId, x.DichVuBenhVienId, x.NhomGiaId, x.YeuCauDichVuId })
                .Select(x => new DichVuTrongGoiDaDungVo()
                {
                    YeuCauGoiDichVuId = x.Key.YeuCauGoiDichVuId,
                    DichVuBenhVienId = x.Key.DichVuBenhVienId,
                    NhomGiaId = x.Key.NhomGiaId,
                    SoLuong = x.Sum(a => a.SoLuong)
                })
                .ToList();
            #endregion

            foreach (var yeuCauGoiDichVu in yeuCauGoiDichVus)
            {
                // dịch vụ khám
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKhamBenhBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                                && !yeuCauVo.DichVuKhongThems.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                       && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                                       && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                                       && a.DichVuId == x.DichVuKhamBenhBenhVienId
                                                                       && a.NhomGoiDichVuValue == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh))
                    .Select(item => new DichVuBenhVienKhuyenMaiTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                        MaDichVu = item.DichVuKhamBenhBenhVien.Ma,
                        TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                        MaGiaDichVu = item.DichVuKhamBenhBenhVien.DichVuKhamBenh != null ? item.DichVuKhamBenhBenhVien.DichVuKhamBenh.MaTT37 : "",
                        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKhamBenhBenhVienId,
                        DonGiaBenhVien = item.DonGia,
                        DonGiaKhuyenMai = item.DonGiaKhuyenMai,
                        DonGiaBaoHiem = item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems
                                    .FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                         && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                      && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                        CoBHYT = coBHYT,
                        SoLuong = 1,
                        TiLeBaoHiemThanhToan = item.DichVuKhamBenhBenhVien
                                               .DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                      && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                            item.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                        #region Cập nhật 15/12/2022
                        //SoLanDaSuDung = yeuCauGoiDichVu.MienGiamChiPhis.Where(o => o.DaHuy != true)
                        //    .Where(o =>
                        //        o.YeuCauKhamBenh != null &&
                        //        o.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                        //        o.YeuCauKhamBenh.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId &&
                        //        o.YeuCauKhamBenh.NhomGiaDichVuKhamBenhBenhVienId == item.NhomGiaDichVuKhamBenhBenhVienId &&
                        //        (o.TaiKhoanBenhNhanThuId == null || o.TaiKhoanBenhNhanThu.DaHuy != true))
                        //    .Select(x => x.YeuCauKhamBenhId.Value)
                        //    .Distinct().Count(),
                        SoLanDaSuDung = dvKhamDaDungs.Where(o => o.DichVuBenhVienId == item.DichVuKhamBenhBenhVienId && o.NhomGiaId == item.NhomGiaDichVuKhamBenhBenhVienId).Sum(o => o.SoLuong),
                        #endregion
                        SoLanTheoGoi = item.SoLan,
                        HanSuDung = yeuCauGoiDichVu.ThoiDiemChiDinh.AddDays(item.SoNgaySuDung),
                        DaQuyetToan = yeuCauGoiDichVu.DaQuyetToan == true,
                        TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu
                    }));

                // dịch vụ kỹ thuật
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKyThuatBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                                && !yeuCauVo.DichVuKhongThems.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                        && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                                        && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                                        && a.DichVuId == x.DichVuKyThuatBenhVienId
                                                                        && a.NhomGoiDichVuValue == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat))
                    .Select(item => new DichVuBenhVienKhuyenMaiTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                        MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                        TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                        MaGiaDichVu = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.MaGia : "",
                        TenGia = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.TenGia : "",
                        Ma4350 = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350 : "",
                        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                        DonGiaBenhVien = item.DonGia,
                        DonGiaKhuyenMai = item.DonGiaKhuyenMai,
                        DonGiaBaoHiem = item.DichVuKyThuatBenhVien
                                        .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                              && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                    item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                    && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                        CoBHYT = coBHYT,
                        SoLuong = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                             && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                             && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                             && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                             && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).SoLuongSuDung,
                        TiLeBaoHiemThanhToan = item.DichVuKyThuatBenhVien
                                               .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                     && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                            item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                            && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                        NhomChiPhiDichVuKyThuat = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.NhomChiPhi : Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                        NhomDichVuBenhVienId = item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                        #region Cập nhật 15/12/2022
                        //SoLanDaSuDung = yeuCauGoiDichVu.MienGiamChiPhis
                        //    .Where(o => o.DaHuy != true &&
                        //        o.YeuCauDichVuKyThuat != null &&
                        //        o.YeuCauDichVuKyThuat.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                        //        o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId &&
                        //        o.YeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId == item.NhomGiaDichVuKyThuatBenhVienId &&
                        //        (o.TaiKhoanBenhNhanThuId == null || o.TaiKhoanBenhNhanThu.DaHuy != true))
                        //    .Select(o => new {o.YeuCauDichVuKyThuatId, o.YeuCauDichVuKyThuat.SoLan})
                        //    .GroupBy(x => x.YeuCauDichVuKyThuatId)
                        //    .Select(x => x.First().SoLan).DefaultIfEmpty().Sum(),
                        SoLanDaSuDung = dvKyThuatDaDungs.Where(o => o.DichVuBenhVienId == item.DichVuKyThuatBenhVienId && o.NhomGiaId == item.NhomGiaDichVuKyThuatBenhVienId).Sum(o => o.SoLuong),
                        #endregion
                        SoLanTheoGoi = item.SoLan,
                        HanSuDung = yeuCauGoiDichVu.ThoiDiemChiDinh.AddDays(item.SoNgaySuDung),
                        DaQuyetToan = yeuCauGoiDichVu.DaQuyetToan == true,
                        TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,

                        ViTriTiem = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                        && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                                        && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                        && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                                        && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).ViTriTiem ?? null,
                        MuiSo = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                    && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                                    && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                    && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                                    && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).MuiSo ?? null,
                        LieuLuong = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                        && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                                        && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                        && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                                        && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).LieuLuong ?? null,
                        NoiThucHienId = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                            && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                                            && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                            && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                                            && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).NoiThucHienId ?? 0,
                    }));

                //// dịch vụ giường
                //lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                //    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                //                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                //                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                //                                         && a.DichVuBenhVienId == x.DichVuGiuongBenhVienId
                //                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                //                && !yeuCauVo.DichVuKhongThems.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                //                                                        && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                //                                                        && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                //                                                        && a.DichVuId == x.DichVuGiuongBenhVienId
                //                                                        && a.NhomGoiDichVuValue == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh))
                //    .Select(item => new DichVuBenhVienTheoGoiDichVuVo()
                //    {
                //        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                //        DichVuBenhVienId = item.DichVuGiuongBenhVienId,
                //        MaDichVu = item.DichVuGiuongBenhVien.Ma,
                //        TenDichVu = item.DichVuGiuongBenhVien.Ten,
                //        MaGiaDichVu = item.DichVuGiuongBenhVien.DichVuGiuong != null ? item.DichVuGiuongBenhVien.DichVuGiuong.MaTT37 : "",
                //        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuGiuongBenhVienId,
                //        DonGiaBenhVien = item.DonGia,
                //        DonGiaBaoHiem = item.DichVuGiuongBenhVien
                //                        .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                             && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                //                    item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                                  && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                //        CoBHYT = coBHYT,
                //        SoLuong = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                //                                             && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                //                                             && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                //                                             && b.DichVuBenhVienId == item.DichVuGiuongBenhVienId
                //                                             && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuGiuongBenh).SoLuongSuDung,//item.SoLan,
                //        TiLeBaoHiemThanhToan = item.DichVuGiuongBenhVien
                //                               .DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                    && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                //                            item.DichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                //                                                                                                          && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                //        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                //        SoLanDaSuDung = item.DichVuGiuongBenhVien.YeuCauDichVuGiuongBenhViens.Count(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                //                                                                                 && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id),
                //        SoLanTheoGoi = item.SoLan
                //    }));
            }

            if (!lstDichVuKhamBenhTrongGoi.Any())
            {
                throw new Exception(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.DichVu.Required"));
            }

            var bacSiDangKys = _hoatDongNhanVienRepository.TableNoTracking
                            .Where(x => x.NhanVien.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi
                                        && x.NhanVien.User.IsActive).ToList();
            //var noiThucHienDVKBs = _dichVuKhamBenhBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToList();
            //var noiThucHienDVKTs = _dichVuKyThuatBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToList();
            //var noiThucHienDVGs = await _dichVuGiuongBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuGiuongBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToListAsync();

            #region cập nhật 02/12/2022: xử lý get list noi thực hiện theo dịch vụ
            var noiThucHienDVKBs = new List<DichVuKhamBenhBenhVienNoiThucHien>();
            var noiThucHienDVKTs = new List<DichVuKyThuatBenhVienNoiThucHien>();
            var dvktNoiThucHienUuTiens = new List<DichVuKyThuatBenhVienNoiThucHienUuTien>();
            var lstDichVuKhamId = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh).Select(x => x.DichVuBenhVienId).Distinct().ToList();
            var lstDichVuKyThuatId = lstDichVuKhamBenhTrongGoi.Where(x => x.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat).Select(x => x.DichVuBenhVienId).Distinct().ToList();

            if (lstDichVuKhamId.Any())
            {
                noiThucHienDVKBs = _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking
                    .Include(x => x.PhongBenhVien)
                    .Include(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
                    .Where(x => lstDichVuKhamId.Contains(x.DichVuKhamBenhBenhVienId))
                    .ToList();
            }

            if (lstDichVuKyThuatId.Any())
            {
                dvktNoiThucHienUuTiens = _dichVuKyThuatBenhVienNoiThucHienUuTienRepository.TableNoTracking
                    .Include(x => x.PhongBenhVien)
                    .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId))
                    .ToList();

                if (!dvktNoiThucHienUuTiens.Any())
                {
                    noiThucHienDVKTs = _dichVuKyThuatBenhVienNoiThucHienRepository.TableNoTracking
                        .Include(x => x.PhongBenhVien)
                        .Include(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
                        .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId))
                        .ToList();
                }
            }
            #endregion

            // get nơi thực hiện mặc định
            var noiThucHienTheoDichVuKyThuats = new List<Camino.Core.Domain.ValueObject.KhoaPhongNhanVien.PhongKhamTemplateVo>();

            foreach (var dichVuBenhVien in lstDichVuKhamBenhTrongGoi)
            {
                switch (dichVuBenhVien.NhomGoiDichVu)
                {
                    case EnumNhomGoiDichVu.DichVuKhamBenh:
                        if (dichVuBenhVien.SoLanConLai < dichVuBenhVien.SoLuong)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongConLai.NotEnough"), dichVuBenhVien.TenDichVu));
                        }

                        if (!dichVuBenhVien.ConHanSuDung)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.HanSuDung.Expired"), dichVuBenhVien.TenDichVu));
                        }

                        if (dichVuBenhVien.DaQuyetToan)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.DaQuetToan"), dichVuBenhVien.TenGoiDichVu));
                        }

                        var lstPhongId = new List<long>();
                        //var lstNoiThucHienDVKBS = noiThucHienDVKBs.Where(x => x.Id == dichVuBenhVien.DichVuBenhVienId).SelectMany(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ToList();
                        var lstNoiThucHienDVKBS = noiThucHienDVKBs.Where(x => x.DichVuKhamBenhBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                        lstPhongId.AddRange(lstNoiThucHienDVKBS.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList());
                        lstPhongId.AddRange(lstNoiThucHienDVKBS.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong).SelectMany(x => x.PhongBenhViens).Select(x => x.Id).ToList());
                        lstPhongId.Sort();
                        var phongThucHienId = lstPhongId.Any() ? lstPhongId.First() : phongHienTaiId;

                        var bacSiDangKyDVKB = bacSiDangKys.FirstOrDefault(x => x.PhongBenhVienId == phongThucHienId);
                        var entityYeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh()
                        {
                            YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                            YeuCauKhamBenhTruocId = yeuCauVo.YeuCauKhamBenhId,
                            DichVuKhamBenhBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                            MaDichVu = dichVuBenhVien.MaDichVu,
                            TenDichVu = dichVuBenhVien.TenDichVu,
                            MaDichVuTT37 = dichVuBenhVien.MaGiaDichVu,
                            NhomGiaDichVuKhamBenhBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                            Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                            DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                            DuocHuongBaoHiem = dichVuBenhVien.DuocHuongBaoHiem,
                            TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,

                            TrangThai = EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                            BaoHiemChiTra = null,

                            NhanVienChiDinhId = currentUserId,
                            NoiChiDinhId = phongHienTaiId,
                            ThoiDiemChiDinh = thoiDiemHienTai,

                            BacSiDangKyId = bacSiDangKyDVKB?.NhanVienId,
                            ThoiDiemDangKy = thoiDiemHienTai,
                            NoiDangKyId = phongThucHienId
                        };

                        //BVHD-3575: cập nhật cho phép chỉ định dv khám từ nội trú
                        // trường hợp chỉ định trong nội trú
                        if (yeuCauVo.NoiTruPhieuDieuTriId != null)
                        {
                            var ngayDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == yeuCauVo.NoiTruPhieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;
                            var newThoiDiemDangKy = ngayDieuTri.Date == thoiDiemHienTai.Date ? thoiDiemHienTai : ngayDieuTri;
                            entityYeuCauKhamBenh.ThoiDiemDangKy = newThoiDiemDangKy;
                            entityYeuCauKhamBenh.LaChiDinhTuNoiTru = true;
                        }

                        entityYeuCauKhamBenh.MienGiamChiPhis.Add(new MienGiamChiPhi
                        {
                            YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                            SoTien = dichVuBenhVien.SoTienMG,
                            YeuCauGoiDichVuId = dichVuBenhVien.YeuCauGoiDichVuId
                        });
                        entityYeuCauKhamBenh.SoTienMienGiam = dichVuBenhVien.SoTienMG;

                        yeuCauVo.YeuCauKhamBenhNews.Add(entityYeuCauKhamBenh);

                        //BVHD-3575: cập nhật cho phép chỉ định dv khám từ nội trú, dịch vụ khám sẽ được xử lý lưu vào YEUCAUTIEPNHAN ngoại trú ở chỗ khác
                        if (yeuCauVo.NoiTruPhieuDieuTriId == null)
                        {
                            yeuCauTiepNhan.YeuCauKhamBenhs.Add(entityYeuCauKhamBenh);
                        }

                        if (yeuCauVo.LaThemTam)
                        {
                            var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();
                            var model = new DropDownListRequestModel()
                            {
                                ParameterDependencies = "{DichVu:" + dichVuBenhVien.DichVuBenhVienId + "}",
                                Take = 50
                            };
                            var noiThucHiens = await GetPhongKham(model, settings.GioiHanSoNguoiKham);
                            var phongThucHienIdStr = noiThucHiens.Select(x => x.KeyId).FirstOrDefault();
                            var lstNoiThucHienId = phongThucHienIdStr.Split(",");
                            var phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                            var nhanVienId = long.Parse(lstNoiThucHienId[1]);
                            var phongBenhVien = await _phongBenhVienRepository.TableNoTracking.Include(p => p.KhoaPhong).FirstOrDefaultAsync(p => p.Id == phongBenhVienId);
                            var bacSiDangKyDVKham =
                                await _nhanVienRepository.TableNoTracking.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == nhanVienId);

                            var yeuCauKhamThemTam = new ChiDinhDichVuGridVo
                            {
                                MaDichVuId = dichVuBenhVien.DichVuBenhVienId,
                                Ma = dichVuBenhVien.MaDichVu,
                                TenDichVu = dichVuBenhVien.TenDichVu,
                                LoaiGia = dichVuBenhVien.TenNhomGiaDichVuBenhVien,
                                LoaiGiaId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                                SoLuong = 1,
                                DonGiaDisplay = ((double)dichVuBenhVien.DonGiaBenhVien.Value).ApplyNumber(),
                                DonGia = (double)dichVuBenhVien.DonGiaBenhVien.Value,
                                ThanhTienDisplay = ((double)dichVuBenhVien.DonGiaBenhVien.Value).ApplyNumber(),
                                ThanhTien = (double)dichVuBenhVien.DonGiaBenhVien.Value,

                                Nhom = Constants.NhomDichVu.DichVuKhamBenh,
                                NoiThucHienId = phongThucHienIdStr,
                                KhoaPhongId = phongBenhVienId,
                                DuocHuongBHYT = yeuCauVo.DuocHuongBaoHiemTam ?? false,
                                GiaBHYT = (double?)dichVuBenhVien.DonGiaBaoHiem ?? 0,
                                TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan ?? 0,

                                //SoTienMG = (double)entityYeuCauKhamBenh.SoTienMienGiam,
                                SoTienMienGiam = entityYeuCauKhamBenh.SoTienMienGiam,
                                //SoTienMGDisplay = entityYeuCauKhamBenh.SoTienMienGiam?.ApplyNumber(),
                                YeuCauGoiDichVuKhuyenMaiId = dichVuBenhVien.YeuCauGoiDichVuId,
                                LaDichVuKhuyenMai = true,
                                NhomId = EnumNhomGoiDichVu.DichVuKhamBenh
                            };

                            yeuCauKhamThemTam.BnThanhToan = yeuCauKhamThemTam.ThanhTien - (double)yeuCauKhamThemTam.BHYTThanhToan;
                            yeuCauKhamThemTam.BnThanhToanDisplay = yeuCauKhamThemTam.BnThanhToan.ApplyNumber();
                            if (phongBenhVien != null && bacSiDangKyDVKham != null)
                            {
                                yeuCauKhamThemTam.NoiThucHienDisplay =
                                    phongBenhVien.KhoaPhong.Ma + " - " + phongBenhVien.Ten + " - " + bacSiDangKyDVKham?.User?.HoTen;
                                yeuCauKhamThemTam.NoiThucHienId = phongThucHienIdStr;
                            }

                            if (yeuCauVo.DichVuKhuyenMaiThemTamTuTiepNhan == null)
                            {
                                yeuCauVo.DichVuKhuyenMaiThemTamTuTiepNhan = new ChiDinhDichVuTrongNhomThuongDungVo();
                            }
                            yeuCauVo.DichVuKhuyenMaiThemTamTuTiepNhan.DichVuKhamBenhs.Add(yeuCauKhamThemTam);
                        }
                        break;
                    case EnumNhomGoiDichVu.DichVuKyThuat:
                        if (dichVuBenhVien.SoLanConLai < dichVuBenhVien.SoLuong)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongConLai.NotEnough"), dichVuBenhVien.TenDichVu));
                        }
                        if (!dichVuBenhVien.ConHanSuDung)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.HanSuDung.Expired"), dichVuBenhVien.TenDichVu));
                        }
                        if (dichVuBenhVien.DaQuyetToan)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.DaQuetToan"), dichVuBenhVien.TenGoiDichVu));
                        }

                        var lstPhongDVKTId = new List<long>();
                        //var noiThucHienDVKTUuTiens = noiThucHienDVKTs.SelectMany(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId)
                        //    .OrderByDescending(x => x.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung).ThenBy(x => x.CreatedOn).ToList();
                        var noiThucHienDVKTUuTiens = dvktNoiThucHienUuTiens.Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId)
                                    .OrderByDescending(x => x.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung).ThenBy(x => x.CreatedOn).ToList();
                        if (noiThucHienDVKTUuTiens.Any())
                        {
                            lstPhongDVKTId.Add(noiThucHienDVKTUuTiens.Select(x => x.PhongBenhVienId).First());
                        }
                        else
                        {
                            //var noiThucHienDVKTByIds = noiThucHienDVKTs.SelectMany(x => x.DichVuKyThuatBenhVienNoiThucHiens).Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            var noiThucHienDVKTByIds = noiThucHienDVKTs.Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVien.DichVuBenhVienId).ToList();
                            lstPhongDVKTId.AddRange(noiThucHienDVKTByIds.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList());
                            lstPhongDVKTId.AddRange(noiThucHienDVKTByIds.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong).SelectMany(x => x.PhongBenhViens).Select(x => x.Id).ToList());
                            lstPhongDVKTId.Sort();
                        }

                        // trường hợp dv chưa có nơi thực hiện
                        if (!lstPhongDVKTId.Any())
                        {
                            long noiThucHienId = 0;
                            var query = new DropDownListRequestModel
                            {
                                ParameterDependencies = "{DichVuId: " + dichVuBenhVien.DichVuBenhVienId + "}",
                                Take = 1
                            };

                            if (!noiThucHienTheoDichVuKyThuats.Any())
                            {
                                noiThucHienTheoDichVuKyThuats = _phongBenhVienRepository.TableNoTracking
                                    .Where(x => x.IsDisabled != true)
                                    .Select(item => new Camino.Core.Domain.ValueObject.KhoaPhongNhanVien.PhongKhamTemplateVo
                                    {
                                        DisplayName = item.Ten,
                                        KeyId = item.Id,
                                        TenPhong = item.Ten,
                                        MaPhong = item.Ma,
                                        PhongKhamId = item.Id,
                                    })
                                    .Distinct()
                                    .Take(1)
                                    .ToList();
                            }
                            //var noiThucHienTheoDichVuKyThuats =
                            //    await GetPhongThucHienChiDinhKhamOrDichVuKyThuat(query);
                            if (noiThucHienTheoDichVuKyThuats.Any())
                            {
                                noiThucHienId = noiThucHienTheoDichVuKyThuats.First().KeyId;
                            }

                            lstPhongDVKTId.Add(noiThucHienId);
                        }
                        var phongThucHienDVKTId = lstPhongDVKTId.First();
                        var bacSiDangKyDVKT = bacSiDangKys.FirstOrDefault(x => x.PhongBenhVienId == phongThucHienDVKTId);

                        var entityYeuCauDichVuKyThuat = new YeuCauDichVuKyThuat()
                        {
                            YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                            YeuCauKhamBenhId = yeuCauVo.YeuCauKhamBenhId,
                            DichVuKyThuatBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                            NhomDichVuBenhVienId = dichVuBenhVien.NhomDichVuBenhVienId,
                            MaDichVu = dichVuBenhVien.MaDichVu,
                            TenDichVu = dichVuBenhVien.TenDichVu,
                            MaGiaDichVu = dichVuBenhVien.MaGiaDichVu,
                            TenGiaDichVu = dichVuBenhVien.TenGia,
                            Ma4350DichVu = dichVuBenhVien.Ma4350,
                            NhomGiaDichVuKyThuatBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                            Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                            DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                            DuocHuongBaoHiem = (yeuCauVo.ISPTTT || yeuCauVo.DichVuKyThuatKhongHuongBHYT) ? false : dichVuBenhVien.DuocHuongBaoHiem,
                            TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,
                            NoiThucHienId = phongThucHienDVKTId,
                            NhomChiPhi = dichVuBenhVien.NhomChiPhiDichVuKyThuat,
                            SoLan = dichVuBenhVien.SoLuong,
                            LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(dichVuBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),

                            TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                            BaoHiemChiTra = null,

                            NhanVienChiDinhId = currentUserId,
                            NoiChiDinhId = phongHienTaiId,
                            ThoiDiemChiDinh = thoiDiemHienTai,
                            NhanVienThucHienId = bacSiDangKyDVKT?.NhanVienId,

                            ThoiDiemDangKy = thoiDiemHienTai,
                            TiLeUuDai = null, // todo: cần kiểm tra lại
                        };

                        entityYeuCauDichVuKyThuat.MienGiamChiPhis.Add(new MienGiamChiPhi
                        {
                            YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                            SoTien = dichVuBenhVien.SoTienMG,
                            YeuCauGoiDichVuId = dichVuBenhVien.YeuCauGoiDichVuId
                        });
                        entityYeuCauDichVuKyThuat.SoTienMienGiam = dichVuBenhVien.SoTienMG;

                        // trường hợp chỉ định trong nội trú
                        if (yeuCauVo.NoiTruPhieuDieuTriId != null)
                        {
                            var ngayDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == yeuCauVo.NoiTruPhieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;
                            var newThoiDiemDangKy = ngayDieuTri.Date == thoiDiemHienTai.Date ? thoiDiemHienTai : ngayDieuTri; //ngayDieuTri.Date.AddSeconds(thoiDiemHienTai.Hour * 3600 + thoiDiemHienTai.Minute * 60);
                            entityYeuCauDichVuKyThuat.ThoiDiemDangKy = newThoiDiemDangKy;
                            entityYeuCauDichVuKyThuat.NoiTruPhieuDieuTriId = yeuCauVo.NoiTruPhieuDieuTriId;
                        }


                        //Dùng riêng cho vắc xin
                        if (yeuCauVo.IsVacxin)
                        {
                            entityYeuCauDichVuKyThuat.YeuCauGoiDichVuId = dichVuBenhVien.YeuCauGoiDichVuId;
                            entityYeuCauDichVuKyThuat.DichVuKyThuatBenhVien = await _dichVuKyThuatBenhVienRepository.GetByIdAsync(dichVuBenhVien.DichVuBenhVienId);
                            entityYeuCauDichVuKyThuat.NhanVienChiDinh = _nhanVienRepository.GetById(entityYeuCauDichVuKyThuat.NhanVienChiDinhId, x => x.Include(y => y.User));
                            if (entityYeuCauDichVuKyThuat.NoiThucHienId != null)
                            {
                                entityYeuCauDichVuKyThuat.NoiThucHien = _phongBenhVienRepository.GetById(entityYeuCauDichVuKyThuat.NoiThucHienId.Value);
                            }

                            var bacSiDangKyTiemVacxin = bacSiDangKys.FirstOrDefault(x => x.PhongBenhVienId == dichVuBenhVien.NoiThucHienId);
                            entityYeuCauDichVuKyThuat.NhanVienThucHienId = bacSiDangKyTiemVacxin?.NhanVienId;
                            if (entityYeuCauDichVuKyThuat.NhanVienThucHienId != null)
                            {
                                entityYeuCauDichVuKyThuat.NhanVienThucHien = _nhanVienRepository.GetById(entityYeuCauDichVuKyThuat.NhanVienThucHienId.Value, x => x.Include(y => y.User));
                            }

                            /* Thêm yêu cầu dịch vụ kỹ thuật tiêm chủng (dược phẩm) */
                            var duocPhamBenhVienTiemChung = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.Id == dichVuBenhVien.DichVuBenhVienId)
                                                                                                                  .SelectMany(p => p.DichVuKyThuatBenhVienTiemChungs)
                                                                                                                  .LastOrDefaultAsync();

                            if (duocPhamBenhVienTiemChung == null)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.DichVuKyThuatBenhVienTiemChung.DichVuKyThuatBenhVien.Null"));
                            }

                            var duocPhamBenhVien = await _duocPhamBenhVienRepository.GetByIdAsync(duocPhamBenhVienTiemChung.DuocPhamBenhVienId, o => o.Include(p => p.DuocPham));

                            var newYeuCauDichVuKyThuatTiemChung = new YeuCauDichVuKyThuatTiemChung();
                            newYeuCauDichVuKyThuatTiemChung.DuocPhamBenhVienId = duocPhamBenhVien.Id;
                            newYeuCauDichVuKyThuatTiemChung.TenDuocPham = duocPhamBenhVien.DuocPham.Ten;
                            newYeuCauDichVuKyThuatTiemChung.TenDuocPhamTiengAnh = duocPhamBenhVien.DuocPham.TenTiengAnh;
                            newYeuCauDichVuKyThuatTiemChung.SoDangKy = duocPhamBenhVien.DuocPham.SoDangKy;
                            newYeuCauDichVuKyThuatTiemChung.STTHoatChat = duocPhamBenhVien.DuocPham.STTHoatChat;
                            newYeuCauDichVuKyThuatTiemChung.MaHoatChat = duocPhamBenhVien.DuocPham.MaHoatChat;
                            newYeuCauDichVuKyThuatTiemChung.HoatChat = duocPhamBenhVien.DuocPham.HoatChat;
                            newYeuCauDichVuKyThuatTiemChung.LoaiThuocHoacHoatChat = duocPhamBenhVien.DuocPham.LoaiThuocHoacHoatChat;
                            newYeuCauDichVuKyThuatTiemChung.NhaSanXuat = duocPhamBenhVien.DuocPham.NhaSanXuat;
                            newYeuCauDichVuKyThuatTiemChung.NuocSanXuat = duocPhamBenhVien.DuocPham.NuocSanXuat;
                            newYeuCauDichVuKyThuatTiemChung.DuongDungId = duocPhamBenhVien.DuocPham.DuongDungId;
                            newYeuCauDichVuKyThuatTiemChung.HamLuong = duocPhamBenhVien.DuocPham.HamLuong;
                            newYeuCauDichVuKyThuatTiemChung.QuyCach = duocPhamBenhVien.DuocPham.QuyCach;
                            newYeuCauDichVuKyThuatTiemChung.TieuChuan = duocPhamBenhVien.DuocPham.TieuChuan;
                            newYeuCauDichVuKyThuatTiemChung.DangBaoChe = duocPhamBenhVien.DuocPham.DangBaoChe;
                            newYeuCauDichVuKyThuatTiemChung.DonViTinhId = duocPhamBenhVien.DuocPham.DonViTinhId;
                            newYeuCauDichVuKyThuatTiemChung.HuongDan = duocPhamBenhVien.DuocPham.HuongDan;
                            newYeuCauDichVuKyThuatTiemChung.MoTa = duocPhamBenhVien.DuocPham.MoTa;
                            newYeuCauDichVuKyThuatTiemChung.ChiDinh = duocPhamBenhVien.DuocPham.ChiDinh;
                            newYeuCauDichVuKyThuatTiemChung.ChongChiDinh = duocPhamBenhVien.DuocPham.ChongChiDinh;
                            newYeuCauDichVuKyThuatTiemChung.LieuLuongCachDung = duocPhamBenhVien.DuocPham.LieuLuongCachDung;
                            newYeuCauDichVuKyThuatTiemChung.TacDungPhu = duocPhamBenhVien.DuocPham.TacDungPhu;
                            newYeuCauDichVuKyThuatTiemChung.ChuYdePhong = duocPhamBenhVien.DuocPham.ChuYDePhong;
                            newYeuCauDichVuKyThuatTiemChung.ViTriTiem = dichVuBenhVien.ViTriTiem.Value;
                            newYeuCauDichVuKyThuatTiemChung.MuiSo = dichVuBenhVien.MuiSo.Value;
                            newYeuCauDichVuKyThuatTiemChung.TrangThaiTiemChung = TrangThaiTiemChung.ChuaTiemChung;
                            newYeuCauDichVuKyThuatTiemChung.LieuLuong = dichVuBenhVien.LieuLuong;

                            /***** Xử lý số lượng *****/
                            newYeuCauDichVuKyThuatTiemChung.SoLuong = 1;
                            entityYeuCauDichVuKyThuat.TiemChung = newYeuCauDichVuKyThuatTiemChung;
                        }

                        yeuCauVo.YeuCauDichVuKyThuatNews.Add(entityYeuCauDichVuKyThuat);
                        yeuCauTiepNhan.YeuCauDichVuKyThuats.Add(entityYeuCauDichVuKyThuat);

                        //trường hợp thêm ở tiếp nhận
                        if (yeuCauVo.LaThemTam && !yeuCauVo.IsVacxin)
                        {
                            var phongBenhVienDVKT = await _phongBenhVienRepository.TableNoTracking
                                .Include(x => x.KhoaPhong)
                                .FirstOrDefaultAsync(x => x.Id == entityYeuCauDichVuKyThuat.NoiThucHienId.Value);

                            var yeuCauDVKT = new ChiDinhDichVuKyThuatGridVo
                            {
                                MaDichVuId = dichVuBenhVien.DichVuBenhVienId,
                                Ma = dichVuBenhVien.MaDichVu,
                                TenDichVu = dichVuBenhVien.TenDichVu,
                                SoLuong = dichVuBenhVien.SoLuong,
                                DonGiaDisplay = ((double)dichVuBenhVien.DonGiaBenhVien.Value).ApplyNumber(),
                                DonGia = (double)dichVuBenhVien.DonGiaBenhVien.Value,
                                ThanhTienDisplay = ((double)dichVuBenhVien.DonGiaBenhVien.Value * dichVuBenhVien.SoLuong).ApplyNumber(),
                                ThanhTien = (double)dichVuBenhVien.DonGiaBenhVien.Value * dichVuBenhVien.SoLuong,

                                LoaiGia = dichVuBenhVien.TenNhomGiaDichVuBenhVien,
                                LoaiGiaId = dichVuBenhVien.NhomGiaDichVuBenhVienId,

                                Nhom = Constants.NhomDichVu.DichVuKyThuat,
                                NoiThucHienId = phongThucHienDVKTId.ToString(),
                                KhoaPhongId = phongThucHienDVKTId,

                                GiaBHYT = (double?)dichVuBenhVien.DonGiaBaoHiem ?? 0,
                                TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan ?? 0,
                                DuocHuongBHYT = false,

                                //SoTienMG = (double)entityYeuCauDichVuKyThuat.SoTienMienGiam,
                                //SoTienMGDisplay = entityYeuCauDichVuKyThuat.SoTienMienGiam?.ApplyNumber(),
                                SoTienMienGiam = entityYeuCauDichVuKyThuat.SoTienMienGiam,
                                YeuCauGoiDichVuKhuyenMaiId = dichVuBenhVien.YeuCauGoiDichVuId,
                                LaDichVuKhuyenMai = true,
                                NhomId = EnumNhomGoiDichVu.DichVuKyThuat
                            };

                            yeuCauDVKT.BnThanhToan = yeuCauDVKT.ThanhTien;
                            yeuCauDVKT.BnThanhToanDisplay = yeuCauDVKT.BnThanhToan.ApplyNumber();
                            if (phongBenhVienDVKT != null)
                            {
                                yeuCauDVKT.NoiThucHienDisplay =
                                    phongBenhVienDVKT.KhoaPhong.Ma + " - " + phongBenhVienDVKT.Ten;
                                yeuCauDVKT.NoiThucHienId = phongThucHienDVKTId.ToString();
                            }

                            if (yeuCauVo.DichVuKhuyenMaiThemTamTuTiepNhan == null)
                            {
                                yeuCauVo.DichVuKhuyenMaiThemTamTuTiepNhan = new ChiDinhDichVuTrongNhomThuongDungVo();
                            }
                            yeuCauVo.DichVuKhuyenMaiThemTamTuTiepNhan.DichVuKyThuats.Add(yeuCauDVKT);
                        }
                        
                        break;
                        //case EnumNhomGoiDichVu.DichVuGiuongBenh:
                        //    if (dichVuBenhVien.SoLanConLai < dichVuBenhVien.SoLuong)
                        //    {
                        //        throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongConLai.NotEnough"), dichVuBenhVien.TenDichVu));
                        //    }

                        //    var entityYeuCauGiuongBenh = new YeuCauDichVuGiuongBenhVien()
                        //    {
                        //        YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId,
                        //        YeuCauKhamBenhId = yeuCauVo.YeuCauKhamBenhId,
                        //        DichVuGiuongBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                        //        Ma = dichVuBenhVien.MaDichVu,
                        //        Ten = dichVuBenhVien.TenDichVu,
                        //        MaTT37 = dichVuBenhVien.MaGiaDichVu,
                        //        NhomGiaDichVuGiuongBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                        //        Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                        //        DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                        //        DuocHuongBaoHiem = dichVuBenhVien.DuocHuongBaoHiem,
                        //        BaoHiemChiTra = null,
                        //        TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,
                        //        LoaiGiuong = dichVuBenhVien.LoaiGiuong.Value,

                        //        TrangThai = EnumTrangThaiGiuongBenh.ChuaThucHien,
                        //        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,

                        //        NhanVienChiDinhId = currentUserId,
                        //        NoiChiDinhId = phongHienTaiId,
                        //        ThoiDiemChiDinh = thoiDiemHienTai,

                        //        YeuCauGoiDichVuId = dichVuBenhVien.YeuCauGoiDichVuId
                        //    };

                        //    yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Add(entityYeuCauGiuongBenh);
                        //    yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Add(entityYeuCauGiuongBenh);
                        //    break;
                }
            }

            if (yeuCauVo.YeuCauKhamBenhId != null)
            {
                var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauVo.YeuCauKhamBenhId
                                                                               && x.YeuCauTiepNhanId == yeuCauVo.YeuCauTiepNhanId
                                                                               && x.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham);
                if (yeuCauKhamBenh != null)
                {
                    yeuCauKhamBenh.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                    yeuCauKhamBenh.NoiThucHienId = yeuCauKhamBenh.NoiDangKyId;
                    yeuCauKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                    yeuCauKhamBenh.ThoiDiemThucHien = DateTime.Now;

                    YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                    {
                        TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai,
                        MoTa = yeuCauKhamBenh.TrangThai.GetDescription()
                    };
                    yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
                }
            }
        }

        public async Task GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(ThongTinDichVuTrongGoi thongTinChiDinhVo)
        {
            var yeuCauGoiDichVus = await _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                //.Include(x => x.MienGiamChiPhis).ThenInclude(x => x.YeuCauDichVuKyThuat)
                //.Include(x => x.MienGiamChiPhis).ThenInclude(x => x.YeuCauKhamBenh)
                //.Include(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                .Where(x => x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                            && ((x.BenhNhanId == thongTinChiDinhVo.BenhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == thongTinChiDinhVo.BenhNhanId && x.GoiSoSinh == true))
                            && x.NgungSuDung != true // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
                            
                            // trường hợp cập nhật số lượng hoặc loại giá dịch vụ khuyến mãi
                            && (thongTinChiDinhVo.YeucauGoiDichVuKhuyenMaiId == null || x.Id == thongTinChiDinhVo.YeucauGoiDichVuKhuyenMaiId)
                            )
                .ToListAsync();
            if (!yeuCauGoiDichVus.Any())
            {
                throw new Exception(_localizationService.GetResource("ChiDinh.GoiDichVu.NotExists"));
            }

            var yeuCauGoiDichVuTheoDichVus = yeuCauGoiDichVus
                .Where(x => (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                             && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
                                 .Any(a => a.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId
                                           && a.NhomGiaDichVuKhamBenhBenhVienId == thongTinChiDinhVo.NhomGiaId))
                           || (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                               && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
                                   .Any(a => a.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId
                                             && a.NhomGiaDichVuKyThuatBenhVienId == thongTinChiDinhVo.NhomGiaId))
                            //|| (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh
                            //    && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(a => a.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId)))
                           )
                .ToList();
            if (!yeuCauGoiDichVuTheoDichVus.Any())
            {
                throw new Exception(_localizationService.GetResource("ChiDinh.GoiDichVuCoDichVu.NotExists"));
            }

            #region Cập nhật 21/12/2022: xử lý get riêng số lượng đã dùng của dịch vụ
            var lstYeuCauGoiId = yeuCauGoiDichVuTheoDichVus.Select(x => x.Id).Distinct().ToList();
            long? YeuCauGoiDichVuId = null;
            long? YeuCauDichVuId = null;
            int SoLan = 0;
            var lstMienGiamDichVu = new[] {new { YeuCauGoiDichVuId, YeuCauDichVuId, SoLan } }.ToList();
            if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                lstMienGiamDichVu = _mienGiamChiPhiRepository.TableNoTracking
                    .Where(x => x.DaHuy != true
                                && x.YeuCauGoiDichVuId != null
                                && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)
                                && x.YeuCauKhamBenhId != null
                                && x.YeuCauKhamBenh.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.YeuCauKhamBenh.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId
                                && x.YeuCauKhamBenh.NhomGiaDichVuKhamBenhBenhVienId == thongTinChiDinhVo.NhomGiaId
                                && lstYeuCauGoiId.Contains(x.YeuCauGoiDichVuId.Value)
                                && (thongTinChiDinhVo.YeuCauDichVuCapNhatSoLuongLoaiGiaId == null
                                        || x.YeuCauKhamBenhId != thongTinChiDinhVo.YeuCauDichVuCapNhatSoLuongLoaiGiaId))
                    .Select(x => new { x.YeuCauGoiDichVuId, YeuCauDichVuId = x.YeuCauKhamBenhId, SoLan = 1} )
                    .ToList();
            }
            else if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat)
            {
                lstMienGiamDichVu = _mienGiamChiPhiRepository.TableNoTracking
                    .Where(x => x.DaHuy != true
                                && x.YeuCauGoiDichVuId != null
                                && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)
                                && x.YeuCauDichVuKyThuatId != null
                                && x.YeuCauDichVuKyThuat.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId
                                && x.YeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId == thongTinChiDinhVo.NhomGiaId
                                && lstYeuCauGoiId.Contains(x.YeuCauGoiDichVuId.Value)
                                && (thongTinChiDinhVo.YeuCauDichVuCapNhatSoLuongLoaiGiaId == null
                                        || x.YeuCauDichVuKyThuatId != thongTinChiDinhVo.YeuCauDichVuCapNhatSoLuongLoaiGiaId))
                    .Select(x => new { x.YeuCauGoiDichVuId, YeuCauDichVuId = x.YeuCauDichVuKyThuatId, SoLan = x.YeuCauDichVuKyThuat.SoLan })
                    .ToList();
            }
            #endregion

            var yeuCauGoiDichVu = yeuCauGoiDichVuTheoDichVus
                .Where(x => (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
                                    .Where(a => a.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId
                                                && a.NhomGiaDichVuKhamBenhBenhVienId == thongTinChiDinhVo.NhomGiaId)
                                    .Sum(a => a.SoLan) >= (
                                                            #region Cập nhật 21/12/2022: xử lý get riêng số lượng đã dùng của dịch vụ
                                                            //x.MienGiamChiPhis.Where(o => o.DaHuy != true).Where(o =>
                                                            //   o.YeuCauKhamBenh != null &&
                                                            //   o.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                                                            //   o.YeuCauKhamBenh.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId &&
                                                            //   o.YeuCauKhamBenh.NhomGiaDichVuKhamBenhBenhVienId == thongTinChiDinhVo.NhomGiaId &&
                                                            //   (o.TaiKhoanBenhNhanThuId == null || o.TaiKhoanBenhNhanThu.DaHuy != true) &&
                                                            //   (thongTinChiDinhVo.NhomGoiDichVu != EnumNhomGoiDichVu.DichVuKhamBenh 
                                                            //        || thongTinChiDinhVo.YeuCauDichVuCapNhatSoLuongLoaiGiaId == null 
                                                            //        || o.YeuCauKhamBenhId != thongTinChiDinhVo.YeuCauDichVuCapNhatSoLuongLoaiGiaId))
                                                            //.Select(o => o.YeuCauKhamBenhId.Value).Distinct().Count() + thongTinChiDinhVo.SoLuong)
                                                            lstMienGiamDichVu.Where(a => a.YeuCauGoiDichVuId == x.Id).Select(a => a.YeuCauDichVuId).Distinct().Count() + thongTinChiDinhVo.SoLuong)
                                                            #endregion
                                                            )
                            || (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
                                    .Where(a => a.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId
                                                && a.NhomGiaDichVuKyThuatBenhVienId == thongTinChiDinhVo.NhomGiaId)
                                    .Sum(a => a.SoLan) >= (
                                                            #region Cập nhật 21/12/2022: xử lý get riêng số lượng đã dùng của dịch vụ
                                                            //x.MienGiamChiPhis.Where(o => o.DaHuy != true &&
                                                            //       o.YeuCauDichVuKyThuat != null &&
                                                            //       o.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                                            //       o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId &&
                                                            //       o.YeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId == thongTinChiDinhVo.NhomGiaId &&
                                                            //       (o.TaiKhoanBenhNhanThuId == null || o.TaiKhoanBenhNhanThu.DaHuy != true) &&
                                                            //       (thongTinChiDinhVo.NhomGoiDichVu != EnumNhomGoiDichVu.DichVuKyThuat
                                                            //            || thongTinChiDinhVo.YeuCauDichVuCapNhatSoLuongLoaiGiaId == null 
                                                            //            || o.YeuCauDichVuKyThuatId != thongTinChiDinhVo.YeuCauDichVuCapNhatSoLuongLoaiGiaId))
                                                            //   .Select(o => new{o.YeuCauDichVuKyThuatId, o.YeuCauDichVuKyThuat.SoLan})
                                                            //   .GroupBy(o => o.YeuCauDichVuKyThuatId)
                                                            //   .Select(o => o.First().SoLan).DefaultIfEmpty().Sum() + thongTinChiDinhVo.SoLuong)
                                                            lstMienGiamDichVu.Where(a => a.YeuCauGoiDichVuId == x.Id).Sum(a => a.SoLan) + thongTinChiDinhVo.SoLuong)
                                                            #endregion
                                                            )
                            //|| (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh
                            //    && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                            //        .Where(a => a.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId)
                            //        .Sum(a => a.SoLan) >= (x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Where(a => a.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId
                            //                                                                 && a.YeuCauGoiDichVuId == x.Id)
                            //                                   .Sum(a => a.SoLuong) + thongTinChiDinhVo.SoLuong))
                            )
                .FirstOrDefault();
            if (yeuCauGoiDichVu == null)
            {
                throw new Exception(_localizationService.GetResource("ChiDinh.SoLuongDichVuConLaiTrongGoi.NotEnough"));
            }

            var goiDaQuyetToans = yeuCauGoiDichVuTheoDichVus
                .Where(x => x.DaQuyetToan != null
                            && x.DaQuyetToan == true
                            && ((thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
                                    .Any(a => a.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId
                                              && a.NhomGiaDichVuKhamBenhBenhVienId == thongTinChiDinhVo.NhomGiaId))
                                || (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                                    && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
                                        .Any(a => a.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId
                                                  && a.NhomGiaDichVuKyThuatBenhVienId == thongTinChiDinhVo.NhomGiaId)))
                            )
                .Select(x => x.ChuongTrinhGoiDichVu.Ten + " - " + x.ChuongTrinhGoiDichVu.TenGoiDichVu)
                .Distinct()
                .ToList();
            if (goiDaQuyetToans.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.DaQuetToan"), goiDaQuyetToans.Join(", ")));
            }

            var kiemTraHanSuDung = yeuCauGoiDichVuTheoDichVus
                .Where(x => (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
                                    .Any(a => a.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId
                                              && a.NhomGiaDichVuKhamBenhBenhVienId == thongTinChiDinhVo.NhomGiaId
                                              && x.ThoiDiemChiDinh.AddDays(a.SoNgaySuDung).Date >= DateTime.Now.Date))
                            || (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat
                                && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
                                    .Any(a => a.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId
                                              && a.NhomGiaDichVuKyThuatBenhVienId == thongTinChiDinhVo.NhomGiaId
                                              && x.ThoiDiemChiDinh.AddDays(a.SoNgaySuDung).Date >= DateTime.Now.Date))
                            //|| (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh
                            //    && x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                            //        .Where(a => a.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId)
                            //        .Sum(a => a.SoLan) >= (x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Where(a => a.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId
                            //                                                                 && a.YeuCauGoiDichVuId == x.Id)
                            //                                   .Sum(a => a.SoLuong) + thongTinChiDinhVo.SoLuong))
                            )
                .Any();
            if (!kiemTraHanSuDung)
            {
                throw new Exception(_localizationService.GetResource("ChiDinhCanLamSan.HanSuDung.Expired"));
            }

            if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKyThuat)
            {
                var dichVuTrongGoi =
                    yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
                        .First(x => x.DichVuKyThuatBenhVienId == thongTinChiDinhVo.DichVuId
                                    && x.NhomGiaDichVuKyThuatBenhVienId == thongTinChiDinhVo.NhomGiaId);
                thongTinChiDinhVo.YeuCauGoiDichVuId = yeuCauGoiDichVu.Id;
                thongTinChiDinhVo.DonGia = dichVuTrongGoi.DonGia;
                thongTinChiDinhVo.DonGiaKhuyenMai = dichVuTrongGoi.DonGiaKhuyenMai;
            }
            else if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                var dichVuTrongGoi =
                    yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
                        .First(x => x.DichVuKhamBenhBenhVienId == thongTinChiDinhVo.DichVuId
                                    && x.NhomGiaDichVuKhamBenhBenhVienId == thongTinChiDinhVo.NhomGiaId);
                thongTinChiDinhVo.YeuCauGoiDichVuId = yeuCauGoiDichVu.Id;
                thongTinChiDinhVo.DonGia = dichVuTrongGoi.DonGia;
                thongTinChiDinhVo.DonGiaKhuyenMai = dichVuTrongGoi.DonGiaKhuyenMai;
            }
            //else if (thongTinChiDinhVo.NhomGoiDichVu == EnumNhomGoiDichVu.DichVuGiuongBenh)
            //{
            //    var dichVuTrongGoi =
            //        yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(x => x.DichVuGiuongBenhVienId == thongTinChiDinhVo.DichVuId);
            //    thongTinChiDinhVo.YeuCauGoiDichVuId = yeuCauGoiDichVu.Id;
            //    thongTinChiDinhVo.DonGia = dichVuTrongGoi.DonGia;
            //    thongTinChiDinhVo.DonGiaTruocChietKhau = dichVuTrongGoi.DonGiaTruocChietKhau;
            //    thongTinChiDinhVo.DonGiaSauChietKhau = dichVuTrongGoi.DonGiaSauChietKhau;
            //}
        }
        #endregion

        #region BVHD-3920
        public async Task<bool> KiemTraBatBuocNhapNoiGioiThieuAsync(long? hinhThucDenId, long? noiGioiThieuId)
        {
            if (hinhThucDenId == null || hinhThucDenId == 0)
            {
                return true;
            }

            var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
            long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);

            if (hinhThucDenId == hinhThucDenGioiThieuId)
            {
                return noiGioiThieuId != null && noiGioiThieuId != 0;
            }

            return true;
        }


        #endregion

        #region BVHD-3941
        public async Task<string> GetThongTinBaoHiemTuNhanAsync(long yeuCauTiepNhanId)
        {
            var thongTinBaoHiemTuNhan = string.Empty;

            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId,
                x => x.Include(a => a.NoiTruBenhAn));

            //var lstCongTyBaoHiemTuNhan = _yeuCauTiepNhanCongTyBaoHiemTuNhanRepository.TableNoTracking
            //    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId)
            //    .ToList();

            if (yeuCauTiepNhan.CoBHTN == true)
            {
                var thoiDiemBatDau = yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemNhapVien ?? yeuCauTiepNhan.ThoiDiemTiepNhan;
                var thoiDiemKetThuc = yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien
                                      ?? (yeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat ? yeuCauTiepNhan.ThoiDiemCapNhatTrangThai : (DateTime?)null);
                var thoiDiemHienTai = DateTime.Now.Date;
                var daKetThuc = thoiDiemKetThuc != null;

                var lstCongTyHiemTuNhan = _yeuCauTiepNhanCongTyBaoHiemTuNhanRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                    && ((!daKetThuc
                                            && (x.NgayHieuLuc == null || x.NgayHieuLuc.Value.Date <= thoiDiemHienTai)
                                            && (x.NgayHetHan == null || x.NgayHetHan.Value.Date >= thoiDiemHienTai))
                                        || (
                                            daKetThuc
                                                && (x.NgayHieuLuc == null || x.NgayHieuLuc.Value.Date <= thoiDiemBatDau.Date)
                                                && (x.NgayHetHan == null || x.NgayHetHan.Value.Date >= thoiDiemBatDau.Date))
                                        )
                        )
                    .Select(x => x.CongTyBaoHiemTuNhan.Ten)
                    .ToList();
                thongTinBaoHiemTuNhan = string.Join("; ", lstCongTyHiemTuNhan);
            }

            return thongTinBaoHiemTuNhan;
        }


        #endregion

        #region Cập nhật 16/12/2022

        public List<LookupItemVo> GetListNoiThucHien(List<long> phongIds)
        {
            var results = _phongBenhVienRepository.TableNoTracking
                .Where(x => phongIds.Contains(x.Id))
                .Select(x => new LookupItemVo()
                {
                    KeyId = x.Id,
                    DisplayName = x.Ten
                }).ToList();
            return results;
        }

        public (List<long>, List<long>) KiemTraDichVuCoGiaBaoHiem(List<long> dichVuKhamIds, List<long> dichVuKyThuatIds)
        {
            var lstDichVuKhamCoGiaBHId = new List<long>();
            var lstDichVuKyThuatCoGiaBHId = new List<long>();

            if (dichVuKhamIds.Any())
            {
                lstDichVuKhamCoGiaBHId = _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                    .Where(x => dichVuKhamIds.Contains(x.DichVuKhamBenhBenhVienId)
                                && x.TuNgay.Date <= DateTime.Now.Date
                                && (x.DenNgay == null || (x.DenNgay != null && x.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)))
                    .Select(x => x.DichVuKhamBenhBenhVienId)
                    .Distinct()
                    .ToList();
            }

            if (dichVuKyThuatIds.Any())
            {
                lstDichVuKyThuatCoGiaBHId = _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                    .Where(x => dichVuKyThuatIds.Contains(x.DichVuKyThuatBenhVienId)
                                && x.TuNgay.Date <= DateTime.Now.Date
                                && (x.DenNgay == null || (x.DenNgay != null && x.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)))
                    .Select(x => x.DichVuKyThuatBenhVienId)
                    .Distinct()
                    .ToList();
            }

            return (lstDichVuKhamCoGiaBHId, lstDichVuKyThuatCoGiaBHId);
        }

        public (bool, bool) KiemTraDisableMienGiamGetYCTN(long yctnId)
        {
            var coMienGiam = false;
            var coMienGiamUuDai = false;

            if(yctnId != 0)
            {
                var mienGiams = _mienGiamChiPhiRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhanId == yctnId)
                    .Select(x => x.LoaiMienGiam)
                    .Distinct().ToList();
                coMienGiam = mienGiams.Any();
                coMienGiamUuDai = mienGiams.Any(x => x == LoaiMienGiam.UuDai);
            }

            return (coMienGiam, coMienGiamUuDai);
        }

        public List<long> GetChuongTrinhGoiDichVuIdTheoBenhNhanId(long benhNhanId)
        {
            var chuongTrinhIds = new List<long>();
            if (benhNhanId != 0)
            {
                chuongTrinhIds = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Where(x => x.BenhNhanId == benhNhanId || (x.GoiSoSinh != null && x.GoiSoSinh == true && x.BenhNhanSoSinhId == benhNhanId))
                    .Select(x => x.ChuongTrinhGoiDichVuId)
                    .ToList();
            }
            return chuongTrinhIds.Distinct().ToList();
        }

        public (List<long>, List<long>) GetDichVuIdTrongGoi(List<long> chuongTrinhIds)
        {
            var lstDichVuKhamId = new List<long>();
            var lstDichVuKyThuatId = new List<long>();

            if(chuongTrinhIds.Any())
            {
                lstDichVuKhamId = _chuongTrinhGoiDichVuDichVuKhamBenhRepository.TableNoTracking
                    .Where(x => chuongTrinhIds.Contains(x.ChuongTrinhGoiDichVuId))
                    .Select(x => x.DichVuKhamBenhBenhVienId)
                    .ToList();
                lstDichVuKhamId = lstDichVuKhamId.Distinct().ToList();

                lstDichVuKyThuatId = _chuongTrinhGoiDichVuDichVuKyThuatRepository.TableNoTracking
                    .Where(x => chuongTrinhIds.Contains(x.ChuongTrinhGoiDichVuId))
                    .Select(x => x.DichVuKyThuatBenhVienId)
                    .ToList();
                lstDichVuKyThuatId = lstDichVuKyThuatId.Distinct().ToList();
            }

            return (lstDichVuKhamId, lstDichVuKyThuatId);
        }

        public (List<long>, List<long>) GetDichVuKhuyenMaiIdTrongGoi(List<long> chuongTrinhIds)
        {
            var lstDichVuKhamId = new List<long>();
            var lstDichVuKyThuatId = new List<long>();

            if (chuongTrinhIds.Any())
            {
                lstDichVuKhamId = _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository.TableNoTracking
                    .Where(x => chuongTrinhIds.Contains(x.ChuongTrinhGoiDichVuId))
                    .Select(x => x.DichVuKhamBenhBenhVienId)
                    .ToList();
                lstDichVuKhamId = lstDichVuKhamId.Distinct().ToList();

                lstDichVuKyThuatId = _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository.TableNoTracking
                    .Where(x => chuongTrinhIds.Contains(x.ChuongTrinhGoiDichVuId))
                    .Select(x => x.DichVuKyThuatBenhVienId)
                    .ToList();
                lstDichVuKyThuatId = lstDichVuKyThuatId.Distinct().ToList();
            }

            return (lstDichVuKhamId, lstDichVuKyThuatId);
        }

        public List<LookupItemTemplate> GetYeuCauDichVuLaKhuyenMai(List<long> yeuCauDichVuIds, bool laYeuCauKham = true)
        {
            var results = new List<LookupItemTemplate>();
            if (yeuCauDichVuIds.Any())
            {
                if(laYeuCauKham)
                {
                    results = _mienGiamChiPhiRepository.TableNoTracking
                        .Where(x => x.YeuCauKhamBenhId != null
                                    && x.DaHuy != true
                                    && x.YeuCauGoiDichVuId != null
                                    && yeuCauDichVuIds.Contains(x.YeuCauKhamBenhId.Value))
                        .Select(x => new LookupItemTemplate()
                        {
                            KeyId = x.YeuCauKhamBenhId.Value,
                            NhomChaId = x.YeuCauGoiDichVuId
                        }).ToList();
                }
                else
                {
                    results = _mienGiamChiPhiRepository.TableNoTracking
                        .Where(x => x.YeuCauDichVuKyThuatId != null
                                    && x.DaHuy != true
                                    && x.YeuCauGoiDichVuId != null
                                    && yeuCauDichVuIds.Contains(x.YeuCauDichVuKyThuatId.Value))
                        .Select(x => new LookupItemTemplate()
                        {
                            KeyId = x.YeuCauDichVuKyThuatId.Value,
                            NhomChaId = x.YeuCauGoiDichVuId
                        }).ToList();
                }
            }

            return results;
        }

        public async Task<YeuCauTiepNhan> GetByIdHaveIncludeForAdddichVu(long id)
        {
            var result = await BaseRepository.GetByIdAsync(id, s => 
                    s.Include(a => a.YeuCauKhamBenhs)
                     .Include(a => a.YeuCauDichVuKyThuats)
                     .Include(a => a.YeuCauDichVuGiuongBenhViens)
                     .Include(x => x.YeuCauDuocPhamBenhViens)
                     .Include(x => x.YeuCauVatTuBenhViens)
                     .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
            );
            return result;
        }

        public async Task<YeuCauTiepNhan> GetByIdHaveIncludeMienGiam(long id)
        {
            var result = await BaseRepository.GetByIdAsync(id, s =>
                    s.Include(a => a.YeuCauKhamBenhs).ThenInclude(a => a.MienGiamChiPhis).ThenInclude(a => a.TaiKhoanBenhNhanThu)
                     .Include(a => a.YeuCauDichVuKyThuats).ThenInclude(a => a.MienGiamChiPhis).ThenInclude(a => a.TaiKhoanBenhNhanThu)
                     .Include(a => a.YeuCauDichVuGiuongBenhViens)
                     .Include(x => x.YeuCauDuocPhamBenhViens)
                     .Include(x => x.YeuCauVatTuBenhViens)
                     .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
            );
            return result;
        }

        public async Task<YeuCauTiepNhan> GetByIdHaveIncludeSimplifyChuyenDoiDichVuTrongVaNgoaiGoi(long id)
        {
            var result = BaseRepository.GetById(id, s => s.Include(u => u.BenhNhan)
                .Include(u => u.BenhNhan).ThenInclude(u => u.DanToc)
                .Include(u => u.BenhNhan).ThenInclude(u => u.NgheNghiep)
                .Include(u => u.BenhNhan).ThenInclude(u => u.QuocTich)
                .Include(u => u.BenhNhan).ThenInclude(u => u.TinhThanh)
                .Include(u => u.BenhNhan).ThenInclude(u => u.QuanHuyen)
                .Include(u => u.BenhNhan).ThenInclude(u => u.PhuongXa)
                .Include(u => u.BenhNhan).ThenInclude(u => u.NguoiLienHeQuanHeNhanThan)
                .Include(u => u.BenhNhan).ThenInclude(u => u.BenhNhanCongTyBaoHiemTuNhans)
                .Include(u => u.NguoiGioiThieu)
                .Include(u => u.NoiGioiThieu)

                .Include(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                .Include(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans).ThenInclude(u => u.CongTyBaoHiemTuNhan)
                //TODO: need update goi dv
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(u => u.YeuCauGoiDichVu).ThenInclude(u => u.ChuongTrinhGoiDichVu)
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.YeuCauGoiDichVu).ThenInclude(u => u.ChuongTrinhGoiDichVu)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                .Include(u => u.YeuCauDuocPhamBenhViens)
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                .Include(u => u.YeuCauVatTuBenhViens)
                .Include(u => u.DonThuocThanhToans).ThenInclude(p => p.DonThuocThanhToanChiTiets)

                .Include(u => u.DoiTuongUuTienKhamChuaBenh)
                .Include(u => u.GiayChuyenVien)
                .Include(u => u.BHYTGiayMienCungChiTra)

                .Include(u => u.HoSoYeuCauTiepNhans)
                .Include(u => u.HoSoYeuCauTiepNhans).ThenInclude(u => u.LoaiHoSoYeuCauTiepNhan)

                .Include(u => u.TheVoucherYeuCauTiepNhans).ThenInclude(u => u.TheVoucher).ThenInclude(u => u.Voucher)

                .Include(u => u.YeuCauTiepNhanLichSuKhamBHYT)
                .Include(u => u.YeuCauTiepNhanLichSuKiemTraTheBHYTs)

                .Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauVatTuBenhViens).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)
                //.Include(u => u.YeuCauDuocPhamBenhViens).ThenInclude(p => p.CongTyBaoHiemTuNhanCongNos)


                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(c => c.User)
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(p => p.YeuCauDichVuKyThuatKhamSangLocTiemChung)


                //BVHD-3825
                .Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.MienGiamChiPhis).ThenInclude(u => u.TaiKhoanBenhNhanThu)
                .Include(u => u.YeuCauDichVuKyThuats).ThenInclude(u => u.MienGiamChiPhis).ThenInclude(u => u.TaiKhoanBenhNhanThu)
            );
            return result;
        }

        public async Task<(List<long>, List<long>)> GetDichVuIdHuyThanhToan(List<long> yeuCauKhamIds, List<long> yeuCauKyThuatIds)
        {
            var dichVuKhamIdHuyThanhToans = new List<long>();
            var dichVuKyThuatIdHuyThanhToans = new List<long>();

            if (yeuCauKhamIds.Any())
            {
                dichVuKhamIdHuyThanhToans = _taiKhoanBenhNhanChiRepository.TableNoTracking
                    .Where(x => x.YeuCauKhamBenhId != null
                                && yeuCauKhamIds.Contains(x.YeuCauKhamBenhId.Value))
                    .Select(x => x.YeuCauKhamBenhId.Value)
                    .Distinct().ToList();
            }
            if (yeuCauKyThuatIds.Any())
            {
                dichVuKyThuatIdHuyThanhToans = _taiKhoanBenhNhanChiRepository.TableNoTracking
                    .Where(x => x.YeuCauDichVuKyThuatId != null
                                && yeuCauKyThuatIds.Contains(x.YeuCauDichVuKyThuatId.Value))
                    .Select(x => x.YeuCauDichVuKyThuatId.Value)
                    .Distinct().ToList();
            }

            return (dichVuKhamIdHuyThanhToans, dichVuKyThuatIdHuyThanhToans);
        }
        #endregion

        #region cập nhật 28/12/2022

        public string AddChiDinhKhamBenhTheoNguoiChiDinhVaNhom(long yeuCauTiepNhanId, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
           List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
           List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string ghiChuCLS, string hostingName)
        {
   

            var yeuCauTiepNhan = BaseRepository.TableNoTracking
                .Where(p => p.Id == yeuCauTiepNhanId)
                .Select(o => new
                {
                    LaCapCuu = o.LaCapCuu,
                    LaCapCuuNgoaiTru = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null ? o.YeuCauTiepNhanNgoaiTruCanQuyetToan.LaCapCuu : null,
                    o.MaYeuCauTiepNhan,
                    o.BenhNhan.MaBN,
                    o.HoTen,
                    o.GioiTinh,
                    o.NamSinh,
                    o.DiaChiDayDu,
                    o.SoDienThoai,
                    o.CoBHYT,
                    o.BHYTMucHuong,
                    o.BHYTMaSoThe,
                    o.BHYTNgayHieuLuc,
                    o.BHYTNgayHetHan,
                    o.NguoiLienHeHoTen,
                    TenQuanHeThanNhan = o.NguoiLienHeQuanHeNhanThanId != null ? o.NguoiLienHeQuanHeNhanThan.Ten : null,
                }).FirstOrDefault();

            var laCapCuu = yeuCauTiepNhan.LaCapCuu ?? yeuCauTiepNhan.LaCapCuuNgoaiTru;

            var hocHamHocVis = _dichVuBenhVienTongHopRepository.TableNoTracking.Select(d => new { d.Id,d.Ma}).ToList();

            var nhomDichVus = _nhomDichVuBenhVienRepository.TableNoTracking.Select(q => new { q.Ten , q.Id}).ToList();

            var dienGiaiChanDoanSoBo = new List<string>();
            var chanDoanSoBos = new List<string>();

            //var lstInThuTuTheoNhomDichVu = listDichVuTheoNguoiChiDinh.First().nhomChiDinhId; // kiểm tra list đầu tiền là dịch vụ gì in nhóm dịch vụ đó trước

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;
            content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU IN CHUNG TẤT CẢ DỊCH VỤ</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            // từng item phiếu in theo người  chỉ định => tất cả dịch vụ khám bệnh và dịch vụ kỹ thuật đều cùng 1  người chỉ định
            var nhanVienChiDinh = "";
            // tên người chỉ định theo phiếu in 
            string ngay = "";
            string thang = "";
            string nam = "";


            // BVHD-3939
            decimal tongCong = 0;
            int soLuong = 0;

            var isHave = false;
            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
            var i = 1;

            //DỊCH VỤ KHÁM BỆNH
            var lstDVKB = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
            int indexDVKB = 1;
            var listInDichVuKhamBenh = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();



            foreach (var itx in lstDVKB)
            {
                var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                 && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                  ).OrderBy(x => x.CreatedOn); // to do nam ho;

                if (lstYeuCauKhamBenhChiDinh != null)
                {
                    foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                    {
                        if (itx.dichVuChiDinhId == yckb.Id)
                        {
                            listInDichVuKhamBenh.Add(yckb);
                        }
                    }
                }
            }

            // BVHD-3939
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            var thanhTienDv = listInDichVuKhamBenh
                .Select(d => (d.YeuCauGoiDichVuId != null ? d.DonGiaSauChietKhau * 1 : d.Gia * 1)).Sum(d => d);
            var thanhTienFormat = string.Format(cul, "{0:n0}", thanhTienDv);
            tongCong += thanhTienDv.GetValueOrDefault();

            foreach (var yckb in listInDichVuKhamBenh)
            {
                if (yckb.ChanDoanSoBoICD != null)
                {
                    dienGiaiChanDoanSoBo.Add(yckb.ChanDoanSoBoGhiChu);

                    chanDoanSoBos.Add(yckb.ChanDoanSoBoICD != null ? yckb.ChanDoanSoBoICD?.Ma + "-" + yckb.ChanDoanSoBoICD?.TenTiengViet : "");
                }


                ngay = yckb.ThoiDiemDangKy.Day.ToString();
                thang = yckb.ThoiDiemDangKy.Month.ToString();
                nam = yckb.ThoiDiemDangKy.Year.ToString();
                if (indexDVKB == 1)
                {
                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    // BVHD-3939
                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;position:relative;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;position:relative;text-align: right;'><b>{thanhTienFormat}</b></td>";
                    // end BVHD-3939
                    htmlDanhSachDichVu += " </tr>";
                }

                var maHocHamVi = string.Empty;
                var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                if (maHocHamViId != null)
                {
                    maHocHamVi = hocHamHocVis.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                }

                nhanVienChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);

                if (yckb.ChanDoanSoBoICD != null)
                {
                    dienGiaiChanDoanSoBo.Add(yckb.ChanDoanSoBoGhiChu);

                    chanDoanSoBos.Add(yckb.ChanDoanSoBoICD != null ? yckb.ChanDoanSoBoICD?.Ma + "-" + yckb.ChanDoanSoBoICD?.TenTiengViet : "");
                }

                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                htmlDanhSachDichVu += " </tr>";
                i++;
                indexDVKB++;
                soLuong++;
            }

            // DỊCH VỤ KỸ THUẬT

            int indexDVKT = 1;
            var listInDichVuKyThuat = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat>();
            var lstDVKT = listDichVuTheoNguoiChiDinh.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat);
            foreach (var itx in lstDVKT)
            {
                foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                {
                    if (itx.dichVuChiDinhId == ycdvkt.Id)
                    {

                        listInDichVuKyThuat.Add(ycdvkt);
                    }

                }
            }
            List<ListDichVuChiDinh> lstDichVuCungChidinh = new List<ListDichVuChiDinh>();
            List<ListDichVuChiDinh> lstDichVuChidinhTungPhieu = new List<ListDichVuChiDinh>();

            List<ListDichVuChiDinh> lstDichVuChidinh = new List<ListDichVuChiDinh>();
            foreach (var ycdvkt in listInDichVuKyThuat)
            {
                var lstDichVuKT = new ListDichVuChiDinh();
                var nhomDichVu = nhomDichVus.Where(x => x.Id == ycdvkt.NhomDichVuBenhVienId).First().Ten;
                lstDichVuKT.TenNhom = nhomDichVu;
                lstDichVuKT.nhomChiDinhId = ycdvkt.NhomDichVuBenhVienId;
                lstDichVuKT.dichVuChiDinhId = ycdvkt.Id;
                lstDichVuChidinh.Add(lstDichVuKT);
            }
            foreach (var dv in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
            {

                if (dv.Count() > 1)
                {
                    // BVHD-3939
                    var listDichVuIds = dv.Select(d => d.dichVuChiDinhId).ToList();

                    var thanhTienDvKT = listDVKT.Where(d => listDichVuIds.Contains(d.Id))
                        .Select(d => (d.YeuCauGoiDichVuId != null ? d.DonGiaSauChietKhau * d.SoLan : d.Gia * d.SoLan)).Sum(d => d);
                    CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                    var thanhTienDVKTFormat = string.Format(culDVKT, "{0:n0}", thanhTienDvKT);
                    tongCong += thanhTienDvKT.GetValueOrDefault();
                    // end BVHD-3939

                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                    {
                        if (dv.Where(p => p.dichVuChiDinhId == ycdvkt.Id).Any())
                        {
                            var nhomDichVu = nhomDichVus.Where(x => x.Id == ycdvkt.NhomDichVuBenhVienId).Select(q => q.Ten).FirstOrDefault();

                            var maHocHamVi = string.Empty;
                            var maHocHamViId = ycdvkt.NhanVienChiDinh?.HocHamHocViId;
                            if (maHocHamViId != null)
                            {
                                maHocHamVi = hocHamHocVis.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                            }
                            nhanVienChiDinh = returnStringTen(maHocHamVi, "", ycdvkt?.NhanVienChiDinh?.User?.HoTen);

                            if (ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null)
                            {
                                dienGiaiChanDoanSoBo.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);

                                chanDoanSoBos.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null ? ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet : "");
                            }

                            ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                            thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                            nam = ycdvkt.ThoiDiemDangKy.Year.ToString();
                            if (indexDVKT == 1)
                            {
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienDVKTFormat}</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                            }
                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            indexDVKT++;
                            //nhomDichVu = "";
                            soLuong += ycdvkt.SoLan;
                        }
                    }
                    indexDVKT = 1;
                }
                if (dv.Count() == 1)
                {
                    // BVHD-3939 // == 1 
                    var listDichVuIds = dv.Select(d => d.dichVuChiDinhId).ToList();
                    var thanhTienDVKT = listDVKT.Where(d => d.Id == dv.First().dichVuChiDinhId)
                        .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                        .Sum();
                    CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                    var thanhTienDVKTFormat = string.Format(culDVKT, "{0:n0}", thanhTienDVKT);
                    tongCong += thanhTienDVKT.GetValueOrDefault(); ;
                    // end BVHD-3939
                    foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                    {
                        if (dv.Where(p => p.dichVuChiDinhId == ycdvkt.Id).Any())
                        {
                            if (ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null)
                            {
                                dienGiaiChanDoanSoBo.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoGhiChu);

                                chanDoanSoBos.Add(ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD != null ? ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.Ma + "-" + ycdvkt.YeuCauKhamBenh?.ChanDoanSoBoICD?.TenTiengViet : "");
                            }
                            var maHocHamVi = string.Empty;
                            var maHocHamViId = ycdvkt.NhanVienChiDinh?.HocHamHocViId;
                            if (maHocHamViId != null)
                            {
                                maHocHamVi = hocHamHocVis.Where(d => d.Id == maHocHamViId).Select(d => d.Ma).FirstOrDefault();
                            }
                            nhanVienChiDinh = returnStringTen(maHocHamVi, "", ycdvkt?.NhanVienChiDinh?.User?.HoTen);

                            ngay = ycdvkt.ThoiDiemDangKy.Day.ToString();
                            thang = ycdvkt.ThoiDiemDangKy.Month.ToString();
                            nam = ycdvkt.ThoiDiemDangKy.Year.ToString();

                            var nhomDichVu = nhomDichVus.Where(x => x.Id == ycdvkt.NhomDichVuBenhVienId).Select(q => q.Ten).FirstOrDefault();
                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + nhomDichVu.ToUpper() + "</b></td>";
                            htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienDVKTFormat}</b></td>";
                            htmlDanhSachDichVu += " </tr>";
                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null ? ycdvkt.NoiThucHien?.Ten : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'</td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            indexDVKT++;
                            nhomDichVu = "";
                            soLuong += ycdvkt.SoLan;
                        }
                    }
                    indexDVKT = 1;
                }
            }
            // từng item phiếu in theo người  chỉ định => tất cả dịch vụ khám bệnh và dịch vụ kỹ thuật đều cùng 1  người chỉ định

            // BVHD-3939- page -total
            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
            // BVHD-3939 - số lượng
            htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
            htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND("{0:n0}")}</b></th>";

            htmlDanhSachDichVu += " </tr>";
            // end BVHD-3939

            var data = new
            {
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.MaYeuCauTiepNhan) : "",
                MaTN = yeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBN = yeuCauTiepNhan.MaBN,
                HoTen = yeuCauTiepNhan.HoTen ?? "",
                GioiTinhString = yeuCauTiepNhan.GioiTinh.GetDescription(),
                NamSinh = yeuCauTiepNhan.NamSinh ?? null,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                Ngay = ngay,
                Thang = thang,
                Nam = nam,
                DienThoai = yeuCauTiepNhan.SoDienThoai,
                DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                HanThe = (yeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
                //Now = DateTime.Now.ApplyFormatDateTimeSACH(),
                //NowTime = DateTime.Now.ApplyFormatTime(),,
                NoiYeuCau = tenPhong,

                ChuanDoanSoBo = chanDoanSoBos.Where(s => s != null && s != "" && s != "-").ToList().Distinct().Join(";"), // khám bệnh 
                DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null && s != "").ToList().Distinct().Join(";"), // khám bệnh

                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = nhanVienChiDinh,
                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                TenQuanHeThanNhan = yeuCauTiepNhan.TenQuanHeThanNhan,
                GhiChuCanLamSang = ghiChuCLS,
                NgayThangNam = DateTime.Now.ApplyFormatDateTimeSACH(),
                //BVHD-3800
                CapCuu = laCapCuu == true ? "Cấp cứu".ToUpper() : ""

            };
            var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
            content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
            if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
            {
                var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                var tmpKB = "<tr id=\"NguoiGiamHo\">";
                var test = content.IndexOf(tmp);
                content = content.Replace(tmpKB, tampKB);
            }

            htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
            i = 1;


            return content;

        }
        private string returnStringTen(string maHocHamHocVi, string maNhomChucDanh, string ten)
        {
            var stringTen = string.Empty;
            //chỗ này show theo format: Mã học vị học hàm + dấu cách + Tên bác sĩ
            if (!string.IsNullOrEmpty(maHocHamHocVi))
            {
                stringTen = maHocHamHocVi + " " + ten;
            }
            if (string.IsNullOrEmpty(maHocHamHocVi))
            {
                stringTen = ten;
            }
            return stringTen;
        }

        public List<long> GetListSarsCauHinh()
        {
            var lstDichVuSarCoVs = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhTiepNhan.DichVuTestSarsCovid")
                .Select(d => d.Value).FirstOrDefault();

            var json = JsonConvert.DeserializeObject<List<DichVuKyThuatBenhVienIdsSarsCoV>>(lstDichVuSarCoVs);
            var dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham = new DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham();
            dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham.Ids = json.Select(d => d.DichVuKyThuatBenhVienId).ToList();
            return dichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham.Ids;
        }
        #endregion
    }
}