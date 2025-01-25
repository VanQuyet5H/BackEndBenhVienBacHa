using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.DichVuBenhVienTongHops;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.TemplateKhamBenhTheoDichVus;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ChuanDoan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ICDs;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.KhamBenhs
{
    [ScopedDependency(ServiceType = typeof(IKhamBenhService))]
    public partial class KhamBenhService : MasterFileService<ICD>, IKhamBenhService
    {
        private IRepository<ThuocHoacHoatChat> _thuocHoacHoatChatRepository;
        private IRepository<Core.Domain.Entities.TrieuChungs.TrieuChung> _trieuChungRepository;
        private IRepository<ChuanDoan> _chuanDoanRepository;
        private IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private IRepository<Core.Domain.Entities.DichVuKyThuats.NhomDichVuKyThuat> _nhomDichVuKyThuatRepository;
        private IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongrepository;
        private IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> _khoaPhongNhanVienrepository;
        private IRepository<YeuCauKhamBenhTrieuChung> _yeuCauKhamBenhTrieuChungRepository;
        private IRepository<YeuCauKhamBenhChuanDoan> _yeuCauKhamBenhChuanDoanRepository;
        private IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private IRepository<TemplateKhamBenhTheoDichVu> _templateKhamBenhTheoDichVuRepository;
        private IRepository<User> _userRepository;
        private IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private IRepository<GoiDichVu> _goiDichVuRepository;
        private IRepository<DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;
        private IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> _dichVuGiuongBenhVienRepository;
        private IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienService;
        private IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Template> _templateRepository;
        private IRepository<NhomGiaDichVuKyThuatBenhVien> _nhomGiaDichVuKyThuatBenhVienRepository;
        private IRepository<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;
        private IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private IRepository<GiuongBenh> _giuongBenhRepository;
        private IRepository<PhongBenhVienHangDoi> _phongBenhVienHangDoiRepository;
        private IRepository<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongRepository;
        private IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> _hoatDongNhanVienRepository;
        private IRepository<Kho> _khoRepository;
        private IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private IRepository<DuocPham> _duocPhamRepository;
        private IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;
        private IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        private ILocalizationService _localizationService;
        private IRepository<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienRepository;
        private IRepository<XuatKhoDuocPham> _xuatKhoDuocPhamRepository;
        private IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;
        private IDuocPhamVaVatTuBenhVienService _duocPhamVaVatTuBenhVienService;
        private ICauHinhService _cauHinhService;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private IRepository<DichVuBenhVienTongHop> _dichVuBenhVienTongHopRepository;
        private IRepository<GoiDichVuChiTietDichVuKhamBenh> _goiDichVuChiTietDichVuKhamBenhRepository;
        private IRepository<GoiDichVuChiTietDichVuKyThuat> _goiDichVuChiTietDichVuKyThuatRepository;
        private IRepository<GoiDichVuChiTietDichVuGiuong> _goiDichVuChiTietDichVuGiuongRepository;
        private IRepository<ChuongTrinhGoiDichVu> _chuongTrinhGoiDichVuRepository;
        private IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> _chuongTrinhGoiDichVuKhamBenhRepository;
        private IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> _chuongTrinhGoiDichVuKyThuatRepository;
        private IRepository<ChuongTrinhGoiDichVuDichVuGiuong> _chuongTrinhGoiDichVuGiuongRepository;
        private readonly IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> _inputStringStoredRepository;
        private IRepository<DichVuKhamBenhBenhVienNoiThucHien> _dichVuKhamBenhBenhVienNoiThucHienRepository;
        private IRepository<DichVuKyThuatBenhVienNoiThucHien> _dichVuKyThuatBenhVienNoiThucHienRepository;
        private IRepository<DichVuKyThuatBenhVienNoiThucHienUuTien> _dichVuKyThuatBenhVienNoiThucHienUuTienRepository;

        private readonly IRepository<Core.Domain.Entities.ICDs.ICD> _iCDRepository;
        private IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> _hocViHocHamRepository;
        private IRepository<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienRepository;
        private IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepository;
        private IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
        private IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
        private IRepository<MienGiamChiPhi> _mienGiamChiPhiRepository;
        private IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiRepository;
        private IRepository<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private IRepository<DichVuKhamBenhBenhVienGiaBaoHiem> _dichVuKhamBenhBenhVienGiaBaoHiemRepository;
        private IRepository<DichVuKyThuatBenhVienGiaBaoHiem> _dichVuKyThuatBenhVienGiaBaoHiemRepository;
        private IRepository<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository;
        public KhamBenhService
            (
                IRepository<ICD> repository,
                IRepository<ThuocHoacHoatChat> thuocHoacHoatChatRepository,
                IRepository<Core.Domain.Entities.TrieuChungs.TrieuChung> trieuChungRepository,
                IRepository<ChuanDoan> chuanDoanRepository,
                IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
                IRepository<Core.Domain.Entities.DichVuKyThuats.NhomDichVuKyThuat> nhomDichVuKyThuatRepository,
                IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongrepository,
                IRepository<YeuCauKhamBenhTrieuChung> yeuCauKhamBenhTrieuChungRepository,
                IRepository<YeuCauKhamBenhChuanDoan> yeuCauKhamBenhChuanDoanRepository,
                IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
                IRepository<TemplateKhamBenhTheoDichVu> templateKhamBenhTheoDichVuRepository,
                IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
                IRepository<GoiDichVu> goiDichVuRepository,
                IRepository<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository,
                IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> dichVuGiuongBenhVienRepository,
                IRepository<VatTuBenhVien> vatTuBenhVienRepository,
                IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository,
                IRepository<User> userRepository,
                IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienService,
                IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
                IUserAgentHelper userAgentHelper,
                IRepository<Template> templateRepository,
                IRepository<NhomGiaDichVuKyThuatBenhVien> nhomGiaDichVuKyThuatBenhVienRepository,
                IRepository<DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository,
                IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
                IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
                IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> khoaPhongNhanVienrepository,
                IRepository<GiuongBenh> giuongBenhRepository,
                IRepository<PhongBenhVienHangDoi> phongBenhVienHangDoiRepository,
                IRepository<YeuCauDichVuGiuongBenhVien> yeuCauDichVuGiuongRepository,
                IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> hoatDongNhanVienRepository,
                IRepository<Kho> khoRepository,
                IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
                IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
                IRepository<DuocPham> duocPhamRepository,
                IRepository<Core.Domain.Entities.VatTus.VatTu> vatTuRepository,
                ILocalizationService localizationService,
                IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository,
                IRepository<YeuCauVatTuBenhVien> yeuCauVatTuBenhVienRepository,
                IRepository<XuatKhoDuocPham> xuatKhoDuocPhamRepository,
                IRepository<XuatKhoVatTu> xuatKhoVatTuRepository,
                ICauHinhService cauHinhService,
                IDuocPhamVaVatTuBenhVienService duocPhamVaVatTuBenhVienService,
                IRepository<DichVuBenhVienTongHop> dichVuBenhVienTongHopRepository,
                IRepository<GoiDichVuChiTietDichVuKhamBenh> goiDichVuChiTietDichVuKhamBenhRepository,
                IRepository<GoiDichVuChiTietDichVuKyThuat> goiDichVuChiTietDichVuKyThuatRepository,
                IRepository<GoiDichVuChiTietDichVuGiuong> goiDichVuChiTietDichVuGiuongRepository,
                IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
                IRepository<ChuongTrinhGoiDichVu> chuongTrinhGoiDichVuRepository,
                IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> chuongTrinhGoiDichVuKhamBenhRepository,
                IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> chuongTrinhGoiDichVuKyThuatRepository,
                IRepository<ChuongTrinhGoiDichVuDichVuGiuong> chuongTrinhGoiDichVuGiuongRepository,
                IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> inputStringStoredRepository,
                IRepository<DichVuKhamBenhBenhVienNoiThucHien> dichVuKhamBenhBenhVienNoiThucHienRepository,
                IRepository<DichVuKyThuatBenhVienNoiThucHien> dichVuKyThuatBenhVienNoiThucHienRepository,
                IRepository<DichVuKyThuatBenhVienNoiThucHienUuTien> dichVuKyThuatBenhVienNoiThucHienUuTienRepository,
                IRepository<Core.Domain.Entities.ICDs.ICD> iCDRepository,
                IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> hocViHocHamRepository,
                IRepository<YeuCauDichVuGiuongBenhVien> yeuCauDichVuGiuongBenhVienRepository,
                IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepository,
                IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository,
                IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository,
                IRepository<MienGiamChiPhi> mienGiamChiPhiRepository,
                IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChiRepository,
                IRepository<DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository,
                IRepository<DichVuKhamBenhBenhVienGiaBaoHiem> dichVuKhamBenhBenhVienGiaBaoHiemRepository,
                IRepository<DichVuKyThuatBenhVienGiaBaoHiem> dichVuKyThuatBenhVienGiaBaoHiemRepository,
                IRepository<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository
            ) : base(repository)

        {
            _thuocHoacHoatChatRepository = thuocHoacHoatChatRepository;
            _trieuChungRepository = trieuChungRepository;
            _chuanDoanRepository = chuanDoanRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _nhomDichVuKyThuatRepository = nhomDichVuKyThuatRepository;
            _khoaPhongrepository = khoaPhongrepository;
            _yeuCauKhamBenhTrieuChungRepository = yeuCauKhamBenhTrieuChungRepository;
            _yeuCauKhamBenhChuanDoanRepository = yeuCauKhamBenhChuanDoanRepository;
            _templateKhamBenhTheoDichVuRepository = templateKhamBenhTheoDichVuRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _goiDichVuRepository = goiDichVuRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _dichVuGiuongBenhVienRepository = dichVuGiuongBenhVienRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _userRepository = userRepository;
            _duocPhamBenhVienService = duocPhamBenhVienService;
            _phongBenhVienRepository = phongBenhVienRepository;
            _userAgentHelper = userAgentHelper;
            _templateRepository = templateRepository;
            _nhomGiaDichVuKyThuatBenhVienRepository = nhomGiaDichVuKyThuatBenhVienRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _khoaPhongNhanVienrepository = khoaPhongNhanVienrepository;
            _giuongBenhRepository = giuongBenhRepository;
            _phongBenhVienHangDoiRepository = phongBenhVienHangDoiRepository;
            _yeuCauDichVuGiuongRepository = yeuCauDichVuGiuongRepository;
            _hoatDongNhanVienRepository = hoatDongNhanVienRepository;
            _khoRepository = khoRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _duocPhamRepository = duocPhamRepository;
            _vatTuRepository = vatTuRepository;
            _localizationService = localizationService;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _yeuCauVatTuBenhVienRepository = yeuCauVatTuBenhVienRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _duocPhamVaVatTuBenhVienService = duocPhamVaVatTuBenhVienService;
            _cauHinhService = cauHinhService;
            _dichVuBenhVienTongHopRepository = dichVuBenhVienTongHopRepository;
            _goiDichVuChiTietDichVuKhamBenhRepository = goiDichVuChiTietDichVuKhamBenhRepository;
            _goiDichVuChiTietDichVuKyThuatRepository = goiDichVuChiTietDichVuKyThuatRepository;
            _goiDichVuChiTietDichVuGiuongRepository = goiDichVuChiTietDichVuGiuongRepository;
            _cauHinhRepository = cauHinhRepository;
            _chuongTrinhGoiDichVuRepository = chuongTrinhGoiDichVuRepository;
            _chuongTrinhGoiDichVuKhamBenhRepository = chuongTrinhGoiDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuKyThuatRepository = chuongTrinhGoiDichVuKyThuatRepository;
            _chuongTrinhGoiDichVuGiuongRepository = chuongTrinhGoiDichVuGiuongRepository;
            _inputStringStoredRepository = inputStringStoredRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _dichVuKyThuatBenhVienNoiThucHienRepository = dichVuKyThuatBenhVienNoiThucHienRepository;
            _dichVuKyThuatBenhVienNoiThucHienUuTienRepository = dichVuKyThuatBenhVienNoiThucHienUuTienRepository;
            _iCDRepository = iCDRepository;
            _hocViHocHamRepository = hocViHocHamRepository;
            _yeuCauDichVuGiuongBenhVienRepository = yeuCauDichVuGiuongBenhVienRepository;
            _noiTruPhieuDieuTriRepository = noiTruPhieuDieuTriRepository;
            _dichVuKhamBenhBenhVienNoiThucHienRepository = dichVuKhamBenhBenhVienNoiThucHienRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
            _mienGiamChiPhiRepository = mienGiamChiPhiRepository;
            _taiKhoanBenhNhanChiRepository = taiKhoanBenhNhanChiRepository;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _dichVuKhamBenhBenhVienGiaBaoHiemRepository = dichVuKhamBenhBenhVienGiaBaoHiemRepository;
            _dichVuKyThuatBenhVienGiaBaoHiemRepository = dichVuKyThuatBenhVienGiaBaoHiemRepository;
            _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository = yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository;
        }

        public async Task<List<ICDTemplateVo>> GetListBenh(DropDownListRequestModel model)
        {
            var listBenh = await BaseRepository.TableNoTracking
                .Where(p => p.TenTiengViet.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = listBenh.Select(item => new ICDTemplateVo
            {
                KeyId = item.Id,
                TenBenh = item.TenTiengViet,
                Ma = item.Ma
            }).ToList();

            return query;
        }

        public string GetTenBenh(int id)
        {
            var ma = BaseRepository.TableNoTracking.Where(p => p.Id == id).Select(p => p.Ma)
                .First().ToString();

            var tenTiengViet = BaseRepository.TableNoTracking.Where(p => p.Id == id).Select(p => p.TenTiengViet)
                .First().ToString();

            return ma + " - " + tenTiengViet;
        }

        public async Task<List<ThuocHoacHoatChatTemplateVo>> GetListThuoc(DropDownListRequestModel model)
        {
            //var listDuocPham = await _thuocHoacHoatChatRepository.TableNoTracking
            //    .Include(p => p.DuongDung)
            //    .Where(p => p.Ma.Contains(model.Query ?? "") || p.Ten.Contains(model.Query ?? ""))
            //    .Take(model.Take)
            //    .ToListAsync();
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var listDuocPham = await _thuocHoacHoatChatRepository.TableNoTracking
                    .Select(item => new ThuocHoacHoatChatTemplateVo
                    {
                        DisplayName = item.Ten,//item.Ma + " - " + item.Ten,
                        KeyId = item.Id,
                        TenThuoc = item.Ten,
                        HoatChat = item.Ma,
                        DuongDung = item.DuongDung.Ten
                    })
                    .ApplyLike(model.Query, x => x.DisplayName, x => x.DuongDung)
                    .Take(model.Take)
                    .ToListAsync();
                return listDuocPham;
            }
            else
            {
                var lstColumnNameSearch = new List<string>
                {
                    "Ma",
                    "Ten"
                };

                var lstDuocPhamId = await _thuocHoacHoatChatRepository
                    .ApplyFulltext(model.Query, nameof(ThuocHoacHoatChat), lstColumnNameSearch)
                    .Select(x => x.Id)
                    .Take(model.Take)
                    .ToListAsync();

                var listDuocPham = await _thuocHoacHoatChatRepository.TableNoTracking
                    .Where(x => lstDuocPhamId.Any(i => i == x.Id) || x.DuongDung.Ten.ToLower().Trim().Contains(model.Query ?? ""))
                    .OrderBy(x => lstDuocPhamId.Any(i => i == x.Id) ? lstDuocPhamId.IndexOf(x.Id) : lstDuocPhamId.Count)
                    .Take(model.Take)
                    .Select(item => new ThuocHoacHoatChatTemplateVo
                    {
                        //DisplayName = item.Ma + " - " + item.Ten,
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        TenThuoc = item.Ten,
                        HoatChat = item.Ma,
                        DuongDung = item.DuongDung.Ten
                    })
                    .ToListAsync();
                return listDuocPham;
            }
        }
        // namTest
        public List<LookupItemVo> getListLoaiDiUng()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.LoaiDiUng>().Select(item => new LookupItemVo()
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item)
            }).ToList();
            return listEnum;
        }

        public async Task<string> GetTenThuoc(long id)
        {
            if (id == 0)
            {
                return null;
            }

            var tenThuoc = await _thuocHoacHoatChatRepository.TableNoTracking.Where(p => p.Id == id)
                .Select(p => p.Ten).FirstOrDefaultAsync();

            return tenThuoc;
        }

        public List<LookupItemVo> GetListNhomMau(LookupQueryInfo queryInfo)
        {
            var listEnumNhomMau = EnumHelper.GetListEnum<Enums.EnumNhomMau>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumNhomMau = listEnumNhomMau.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                                                             .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumNhomMau;
        }

        public string GetTenNhomMau(int id)
        {
            var enumNhomMau = (Enums.EnumNhomMau)id;
            var tenNhomMau = enumNhomMau.GetDescription();
            return tenNhomMau;
        }

        public async Task<List<LookupItemVo>> GetListTrieuChung(DropDownListRequestModel model)
        {
            var listKhoaPhong = await _trieuChungRepository.TableNoTracking
                .Where(p => p.Ten.Contains(model.Query ?? ""))
                .ApplyLike(model.Query, g => g.Ten)
                .Take(model.Take)
                .ToListAsync();

            var query = listKhoaPhong.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id
            }).ToList();

            return query;
        }

        public async Task<List<ChuanDoanTemplateVo>> GetListChuanDoan(DropDownListRequestModel model)
        {
            var listChuanDoan = await _chuanDoanRepository.TableNoTracking
                .Where(p => p.TenTiengViet.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? ""))
                .Take(model.Take)
                .ToListAsync();

            var query = listChuanDoan.Select(item => new ChuanDoanTemplateVo
            {
                DisplayName = item.Ma + " - " + item.TenTiengViet,
                KeyId = item.Id,
                Ten = item.TenTiengViet,
                Ma = item.Ma
            }).ToList();

            return query;
        }

        public long GetIdYeuCauTiepNhan(long idTrieuChung, long idYeuCauTiepNhan)
        {
            var getId = Convert.ToInt64(_yeuCauKhamBenhTrieuChungRepository.TableNoTracking
                .Where(p => p.TrieuChungId == idTrieuChung && p.YeuCauKhamBenhId == idYeuCauTiepNhan)
                .Select(p => p.Id)
                .First());
            return getId;
        }

        public long GetIdYeuCauChuanDoan(long idChuanDoan, long idYeuCauTiepNhan)
        {
            var getId = Convert.ToInt64(_yeuCauKhamBenhChuanDoanRepository.TableNoTracking
                .Where(p => p.ChuanDoanId == idChuanDoan && p.YeuCauKhamBenhId == idYeuCauTiepNhan)
                .Select(p => p.Id)
                .First());
            return getId;
        }

        public List<long> DeleteAllTrieuChungByYctn(long idYeuCauTiepNhan)
        {
            var listTrieuChungByYctn = _yeuCauKhamBenhTrieuChungRepository.TableNoTracking
                .Where(p => p.YeuCauKhamBenhId == idYeuCauTiepNhan).Select(p => p.Id).ToList();

            return listTrieuChungByYctn;
        }

        public List<long> DeleteAllChuanDoanByYeuCauTiepNhan(long idYeuCauTiepNhan)
        {
            var listChuanDoanByYctn = _yeuCauKhamBenhChuanDoanRepository.TableNoTracking
                .Where(p => p.YeuCauKhamBenhId == idYeuCauTiepNhan).Select(p => p.Id).ToList();

            return listChuanDoanByYctn;
        }

        public string GetTenNhanVien(long id)
        {
            var ten = _userRepository.TableNoTracking.Where(p => p.Id == id)
                .Select(p => p.HoTen)
                .FirstOrDefault();

            return ten;
        }

        public async Task<TemplateKhamBenhTheoDichVu> GetKhoaPhong(long idDichVuKhamBenh)
        {
            var template = await _templateKhamBenhTheoDichVuRepository.TableNoTracking
                .Where(p => p.DichVuKhamBenhBenhVienId == idDichVuKhamBenh)
                .FirstOrDefaultAsync();

            return template;
        }

        public async Task<string> GetLoiDan(long id)
        {
            var loiDan = await BaseRepository.TableNoTracking.Where(p => p.Id == id)
                .Select(p => p.LoiDanCuaBacSi).FirstOrDefaultAsync();
            return loiDan;
        }

        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanByIdAsync(long yeuCauTiepNhanId)
        {
            // todo: có cập nhật: bỏ await
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.Table.Where(x => x.Id == yeuCauTiepNhanId)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.DichVuKhamBenhBenhVien).ThenInclude(z => z.DichVuKhamBenhBenhVienGiaBaoHiems)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.DichVuKhamBenhBenhVien).ThenInclude(z => z.DichVuKhamBenhBenhVienGiaBenhViens)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.YeuCauKhamBenhLichSuTrangThais)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.PhongBenhVienHangDois)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.YeuCauKhamBenhICDKhacs).ThenInclude(z => z.ICD)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
                .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.DonThuocThanhToans)
                .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTu)
                .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet)
                .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.DonVTYTThanhToans)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.TaiKhoanBenhNhanChis)

                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien).ThenInclude(z => z.DichVuKyThuatBenhVienGiaBaoHiems)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien).ThenInclude(z => z.DichVuKyThuatVuBenhVienGiaBenhViens)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.TaiKhoanBenhNhanChis)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.MienGiamChiPhis)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.CongTyBaoHiemTuNhanCongNos)

                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(y => y.DichVuGiuongBenhVien).ThenInclude(z => z.DichVuGiuongBenhVienGiaBaoHiems)
                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(y => y.DichVuGiuongBenhVien).ThenInclude(z => z.DichVuGiuongBenhVienGiaBenhViens)
                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(y => y.HoatDongGiuongBenhs)
                .Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(y => y.DuocPhamBenhVien)
                .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(y => y.VatTuBenhVien)

                //TODO update entity kho on 9/9/2020
                .Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                .Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTris).ThenInclude(z => z.NhapKhoDuocPhamChiTiet)

                //TODO update entity kho on 9/9/2020
                .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTu)
                .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTuChiTietViTris).ThenInclude(x => x.NhapKhoVatTuChiTiet)

                .Include(x => x.DonThuocThanhToans).ThenInclude(y => y.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuyetBaoHiemChiTiets)
                .Include(x => x.DoiTuongUuDai).ThenInclude(y => y.DoiTuongUuDaiDichVuKyThuatBenhViens)
                .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.NoiTruPhieuDieuTris).ThenInclude(p => p.YeuCauDuocPhamBenhViens)
                .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruChiDinhDuocPhams).ThenInclude(p => p.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.NhapKhoDuocPhamChiTiets).ThenInclude(z => z.NhapKhoDuocPhams)
                .Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(y => y.YeuCauTraDuocPhamTuBenhNhanChiTiets).ThenInclude(y => y.YeuCauTraDuocPhamTuBenhNhan)
                .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruChiDinhDuocPhams).ThenInclude(z => z.NoiTruChiDinhPhaThuocTiem)
                .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruChiDinhDuocPhams).ThenInclude(z => z.NoiTruChiDinhPhaThuocTruyen)

                 .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.NoiTruPhieuDieuTris).ThenInclude(p => p.YeuCauVatTuBenhViens).ThenInclude(z => z.VatTuBenhVien).ThenInclude(z => z.NhapKhoVatTuChiTiets).ThenInclude(z => z.NhapKhoVatTu)
                .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(y => y.YeuCauTraVatTuTuBenhNhanChiTiets).ThenInclude(y => y.YeuCauTraVatTuTuBenhNhan)
                .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(y => y.VatTuBenhVien).ThenInclude(y => y.NhapKhoVatTuChiTiets)

                .Include(x => x.YeuCauNhapVien)
                .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.NoiTruKhoaPhongDieuTris)
                .Include(x => x.NoiTruBenhAn).ThenInclude(y => y.NoiTruEkipDieuTris)

                .Include(x => x.YeuCauTruyenMaus).ThenInclude(z => z.NhapKhoMauChiTiets)
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris).ThenInclude(x => x.YeuCauTruyenMaus).ThenInclude(z => z.NhapKhoMauChiTiets)
                .Include(x => x.NoiTruHoSoKhacs)

                // khám đoàn
                //BVHD-3668
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)

                // gói dịch vụ giường
                .Include(x => x.BenhNhan).ThenInclude(y => y.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(t => t.ChuongTrinhGoiDichVuDichVuGiuongs)
                .Include(x => x.BenhNhan).ThenInclude(y => y.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(t => t.ChuongTrinhGoiDichVuDichVuGiuongs)

                //ghi nhận VTT
                .Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(x => x.YeuCauLinhDuocPhamChiTiets).ThenInclude(x => x.YeuCauLinhDuocPham)
                .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(x => x.YeuCauLinhVatTuChiTiets).ThenInclude(x => x.YeuCauLinhVatTu)


                //BVHD-3825
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                .First();
            return yeuCauTiepNhan;
        }

        public List<LookupItemVo> getListLoaiTienSuBenh()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumLoaiTienSuBenh>().Select(item => new LookupItemVo()
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item)
            }).ToList();
            return listEnum;
        }

        public async Task<string> GetTemplateKhamBenhTheoDichVuKham(long dichVuKhamBenhId)
        {
            //var template =
            //    await _templateKhamBenhTheoDichVuRepository.TableNoTracking.ToArrayAsync();

            //Cập nhật: 01/12/2022: bỏ await
            var template =
                 _templateKhamBenhTheoDichVuRepository.TableNoTracking
                 .Select(x => new
                 {
                     Id = x.Id,
                     DichVuKhamBenhBenhVienId = x.DichVuKhamBenhBenhVienId,
                     ComponentDynamics = x.ComponentDynamics
                 })
                 .ToList();

            var result = template.Any(x => x.DichVuKhamBenhBenhVienId == dichVuKhamBenhId)
                ? template.Where(x => x.DichVuKhamBenhBenhVienId == dichVuKhamBenhId).OrderByDescending(x => x.Id).First() : template.First();
            return result != null ? result.ComponentDynamics : null;
        }

        public decimal GetMucTranChiPhi()
        {
            var vaLue = _cauHinhRepository.TableNoTracking.Where(p => p.Name == "CauHinhTiepNhan.MucTranChiPhiKeToa").Select(p => p.Value).First();
            return decimal.Parse(vaLue);
        }

        //Cập nhật 17/03/2022: get thông tin YCTN khi thêm dịch vụ
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.Table.Where(x => x.Id == yeuCauTiepNhanId)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhLichSuTrangThais)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauDuocPhamBenhViens)
                .Include(x => x.YeuCauVatTuBenhViens)
                .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                .Include(x => x.DoiTuongUuDai).ThenInclude(x => x.DoiTuongUuDaiDichVuKyThuatBenhViens)
                .Include(x => x.DoiTuongUuDai).ThenInclude(x => x.DoiTuongUuDaiDichVuKhamBenhBenhViens)

                // KSK
                .Include(x => x.HopDongKhamSucKhoeNhanVien)
                .First();
            return yeuCauTiepNhan;
        }

        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaDichVuNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.TaiKhoanBenhNhanChis)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TaiKhoanBenhNhanChis)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaNhieuDichVuNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaVTTHThuocNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(x => x.YeuCauLinhDuocPhamChiTiets).ThenInclude(x => x.YeuCauLinhDuocPham)
                    .Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTris).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                    .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(x => x.YeuCauLinhVatTuChiTiets).ThenInclude(x => x.YeuCauLinhVatTu)
                    .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(x => x.XuatKhoVatTuChiTiet).ThenInclude(x => x.XuatKhoVatTuChiTietViTris).ThenInclude(x => x.NhapKhoVatTuChiTiet)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiSuaXoaDVKyThuatNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TaiKhoanBenhNhanChis)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDvThuongDungNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDVTrongGoiNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanValidationChuyenDichVuKhamNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.DonThuocThanhToans)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.DonVTYTThanhToans)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT)
                    .Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(x => x.XuatKhoDuocPhamChiTiet)
                    .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(x => x.XuatKhoVatTuChiTiet));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiChuyenDichVuKhamNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhLichSuTrangThais)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonVTYTs).ThenInclude(x => x.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(x => x.DonVTYTThanhToanChiTiets)
                        .ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.NhapKhoVatTuChiTiet)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonVTYTs).ThenInclude(x => x.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(x => x.DonVTYTThanhToanChiTiets)
                        .ThenInclude(x => x.XuatKhoVatTuChiTietViTri).ThenInclude(x => x.XuatKhoVatTuChiTiet).ThenInclude(x => x.XuatKhoVatTu)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs).ThenInclude(x => x.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                        .ThenInclude(x => x.DuyetBaoHiemChiTiets)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs).ThenInclude(x => x.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                        .ThenInclude(x => x.XuatKhoDuocPhamChiTietViTri).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs).ThenInclude(x => x.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                        .ThenInclude(x => x.XuatKhoDuocPhamChiTietViTri).ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPham)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs).ThenInclude(x => x.DonThuocThanhToans)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT)
                    .Include(x => x.YeuCauDuocPhamBenhViens).ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTris).ThenInclude(x => x.NhapKhoDuocPhamChiTiet)
                    .Include(x => x.YeuCauVatTuBenhViens).ThenInclude(x => x.XuatKhoVatTuChiTiet).ThenInclude(x => x.XuatKhoVatTuChiTietViTris).ThenInclude(x => x.NhapKhoVatTuChiTiet)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDVKhamSucKhoeByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiSuaXoaDVKhamSucKhoeByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                    .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaRiengDichVuKhamNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.TaiKhoanBenhNhanChis)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaRiengDichVuKhamTabChiDinhByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaRiengDichVuKyThuatNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauDichVuKyThuats)//.ThenInclude(x => x.TaiKhoanBenhNhanChis)
                                                         //.Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiCapNhatRiengDichVuKyThuatTiemChungByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauKhamBenhs)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuKyThuatKhuyenMaiNgoaiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.Table.Where(x => x.Id == yeuCauTiepNhanId)
                .Include(x => x.YeuCauKhamBenhs)
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhLichSuTrangThais)
                .Include(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauDuocPhamBenhViens)
                .Include(x => x.YeuCauVatTuBenhViens)
                .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                .First();
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuKhamNoiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.Table.Where(x => x.Id == yeuCauTiepNhanId)
                .Include(x => x.YeuCauKhamBenhs)
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                .First();
            return yeuCauTiepNhan;
        }
        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuKyThuatNoiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.Table.Where(x => x.Id == yeuCauTiepNhanId)
                .Include(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                .First();
            return yeuCauTiepNhan;
        }

        public async Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaRiengDichVuKyThuatNoiTruByIdAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, a =>
                a.Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TaiKhoanBenhNhanChis)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.TaiKhoanBenhNhanThu)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.NoiTruPhieuDieuTri));
            return yeuCauTiepNhan;
        }

        #region cập nhật 5/12/2022
        public async Task<MienGiamChiPhi> GetMienGiamChiPhiTrongGoiTheoDichVu(long? yeuCauKhamBenhId = null, long? yeuCauKyThuatId = null)
        {
            MienGiamChiPhi mienGiam = null;
            if (yeuCauKhamBenhId.GetValueOrDefault() != 0)
            {
                mienGiam = _mienGiamChiPhiRepository.Table
                        .Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId
                                    && x.DaHuy != true
                                    && x.YeuCauGoiDichVuId != null
                                    && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true))
                        .FirstOrDefault();
            }
            else if (yeuCauKyThuatId.GetValueOrDefault() != 0)
            {
                mienGiam = _mienGiamChiPhiRepository.Table
                    .Where(x => x.YeuCauDichVuKyThuatId == yeuCauKyThuatId
                                && x.DaHuy != true
                                && x.YeuCauGoiDichVuId != null
                                && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true))
                    .FirstOrDefault();
            }
            return mienGiam;
        }

        public async Task<bool> KiemTraTaiKhoanBenhNhanChiTheoDichVu(long? yeuCauKhamBenhId = null, long? yeuCauKyThuatId = null)
        {
            var result = false;
            if (yeuCauKhamBenhId.GetValueOrDefault() != 0)
            {
                result = _taiKhoanBenhNhanChiRepository.TableNoTracking
                            .Any(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId);
            }
            else if (yeuCauKyThuatId.GetValueOrDefault() != 0)
            {
                result = _taiKhoanBenhNhanChiRepository.Table
                            .Any(x => x.YeuCauDichVuKyThuatId == yeuCauKyThuatId);
            }
            return result;
        }

        public async Task<List<long>> KiemTraTaiKhoanBenhNhanChiTheoDichVus(List<long> yeuCauKhamBenhIds = null, List<long> yeuCauKyThuatIds = null)
        {
            var results = new List<long>();
            if (yeuCauKhamBenhIds != null && yeuCauKhamBenhIds.Any())
            {
                results = _taiKhoanBenhNhanChiRepository.TableNoTracking
                            .Where(x => x.YeuCauKhamBenhId != null
                                    && yeuCauKhamBenhIds.Contains(x.YeuCauKhamBenhId.Value))
                            .Select(x => x.YeuCauKhamBenhId.Value)
                            .Distinct().ToList();
            }
            else if (yeuCauKyThuatIds != null && yeuCauKyThuatIds.Any())
            {
                results = _taiKhoanBenhNhanChiRepository.TableNoTracking
                            .Where(x => x.YeuCauDichVuKyThuatId != null
                                    && yeuCauKyThuatIds.Contains(x.YeuCauDichVuKyThuatId.Value))
                            .Select(x => x.YeuCauDichVuKyThuatId.Value)
                            .Distinct().ToList();
            }
            return results;
        }
        #endregion
    }
}
