using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Services.Helpers;
using XuatKhoDuocPham = Camino.Core.Domain.Entities.XuatKhos.XuatKhoDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Globalization;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Services.KhoDuocPhams;
using static Camino.Core.Domain.ValueObject.QuayThuoc.DanhSachChoXuatThuocVO;
using Camino.Services.ExportImport.Help;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Services.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Services.QuayThuoc
{
    [ScopedDependency(ServiceType = typeof(IQuayThuocService))]
    public partial class QuayThuocService : MasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>, IQuayThuocService
    {
        IRepository<BenhNhan> _benhNhanRepo;
        IRepository<DuocPham> _duocPhamnRepo;
        IRepository<Camino.Core.Domain.Entities.VatTus.VatTu> _vatTuRepo;
        IRepository<DonThuocThanhToan> _donThuocThanhToanRepo;
        IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepo;
        IRepository<CongTyBaoHiemTuNhan> _congtyBaoHiemTuNhanRepo;
        IRepository<YeuCauTiepNhanCongTyBaoHiemTuNhan> _yeucauTiepNhanCongTyBaoHiemTuNhanRepo;
        IRepository<BenhNhanCongTyBaoHiemTuNhan> _benhNhanCongTyBaoHiemTuNhanRepo;
        IRepository<Template> _templateRepo;
        IRepository<Camino.Core.Domain.Entities.Users.User> _userRepo;
        IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepo;
        IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepo;
        private readonly IRepository<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuRepository;
        private readonly IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiRepository;
        IUserAgentHelper _userAgentHelper;
        IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepo;
        IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuocChiTiet> _yeuCauKhamBenhDonThuocChiTietRepo;
        IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChuanDoan> _yeuCauKhamBenhChuanDoanRepo;
        IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuoc> _yeuCauKhamBenhDonThuocRepo;
        IRepository<Camino.Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri> _khoDuocPhamViTriRepo;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Core.Domain.Entities.Vouchers.Voucher> _voucherRepository;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        IRepository<Camino.Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh> _donViHanhChinhRepo;
        IRepository<Camino.Core.Domain.Entities.Thuocs.ADR> _aDRRepo;
        IRepository<Camino.Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc> _benhNhanDiUngThuocRepo;
        private readonly IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        IRepository<DonVTYTThanhToan> _donVTYTThanhToanRepo;
        IRepository<XuatKhoVatTu> _xuatKhoVatTuRepo;
        IRepository<XuatKhoDuocPham> _xuatKhoDuocPhamRepo;
        private readonly IDuocPhamVaVatTuBenhVienService _duocPhamVaVatTuBenhVienService;
        private readonly IRepository<Core.Domain.Entities.BenhVien.BenhVien> _benhVienRepository;
        private readonly IRepository<Core.Domain.Entities.Users.User> _bacSiChiDinhRepository;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuocChiTiet> _yeuCauKhamBenhDonThuocChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.Users.User> _userRepository;
        IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhomRepository;
        private readonly IRepository<DonThuocThanhToan> _donThuocThanhToanRepository;
        private readonly IRepository<DonVTYTThanhToan> _donVTYTThanhToanRepository;
        private readonly IRepository<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTietRepository;
        private readonly IRepository<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTietRepository;
        public QuayThuocService
        (IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> repository,
            IRepository<Template> templateRepo,
            IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepo,
            IRepository<DuocPham> duocPhamnRepo, IRepository<DonThuocThanhToan> donThuocThanhToanRepo,
            IRepository<BenhNhanCongTyBaoHiemTuNhan> benhNhanCongTyBaoHiemTuNhanRepo,
            IRepository<YeuCauTiepNhanCongTyBaoHiemTuNhan> yeucauTiepNhanCongTyBaoHiemTuNhanRepo,
            IRepository<BenhNhan> benhNhanRepo, IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepo,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepo, IUserAgentHelper userAgentHelper,
            IRepository<CongTyBaoHiemTuNhan> congtyBaoHiemTuNhanRepo,
            IRepository<TaiKhoanBenhNhanThu> taiKhoanBenhNhanThuRepository,
            IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuocChiTiet> yeuCauKhamBenhDonThuocChiTietRepo,
            IRepository<Camino.Core.Domain.Entities.Users.User> userRepo,
            ICauHinhService cauHinhService, IRepository<Core.Domain.Entities.Vouchers.Voucher> voucherRepository,
            ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
            IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChiRepository,
            IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuoc> yeuCauKhamBenhDonThuocRepo,
            IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChuanDoan> yeuCauKhamBenhChuanDoanRepo,
            IRepository<Camino.Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh> donViHanhChinhRepo,
            IRepository<Camino.Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri> khoDuocPhamViTriRepo,
            IRepository<Camino.Core.Domain.Entities.Thuocs.ADR> aDRRepo,
            IRepository<Camino.Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc> benhNhanDiUngThuocRepo,
            IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepo,
            IRepository<Camino.Core.Domain.Entities.VatTus.VatTu> vatTuRepo,
            IRepository<DonVTYTThanhToan> donVTYTThanhToanRepo,
            IRepository<XuatKhoVatTu> xuatKhoVatTuRepo,
            IRepository<XuatKhoDuocPham> xuatKhoDuocPhamRepo,
            IRepository<Core.Domain.Entities.BenhVien.BenhVien> benhVienRepository,
            IDuocPhamVaVatTuBenhVienService duocPhamVaVatTuBenhVienService,
            IRepository<Core.Domain.Entities.Users.User> bacSiChiDinhRepository,
            IYeuCauKhamBenhService yeuCauKhamBenhService,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhDonThuocChiTiet> yeuCauKhamBenhDonThuocChiTietRepository,
            IRepository<Core.Domain.Entities.Users.User> userRepository,
            IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhomRepository,
            IRepository<DonThuocThanhToan> donThuocThanhToanRepository,
            IRepository<DonVTYTThanhToan> donVTYTThanhToanRepository,
            IRepository<DonThuocThanhToanChiTiet> donThuocThanhToanChiTietRepository,
            IRepository<DonVTYTThanhToanChiTiet> donVTYTThanhToanChiTietRepository
            )
            : base(repository)
        {
            _donThuocThanhToanRepo = donThuocThanhToanRepo;
            _benhNhanCongTyBaoHiemTuNhanRepo = benhNhanCongTyBaoHiemTuNhanRepo;
            _yeucauTiepNhanCongTyBaoHiemTuNhanRepo = yeucauTiepNhanCongTyBaoHiemTuNhanRepo;
            _benhNhanRepo = benhNhanRepo;
            _yeuCauKhamBenhRepo = yeuCauKhamBenhRepo;
            _yeuCauTiepNhanRepo = yeuCauTiepNhanRepo;
            _congtyBaoHiemTuNhanRepo = congtyBaoHiemTuNhanRepo;
            _duocPhamnRepo = duocPhamnRepo;
            _nhapKhoDuocPhamChiTietRepo = nhapKhoDuocPhamChiTietRepo;
            _userAgentHelper = userAgentHelper;
            _templateRepo = templateRepo;
            _taiKhoanBenhNhanThuRepository = taiKhoanBenhNhanThuRepository;
            _yeuCauKhamBenhDonThuocChiTietRepo = yeuCauKhamBenhDonThuocChiTietRepo;
            _userRepo = userRepo;
            _cauHinhService = cauHinhService;
            _voucherRepository = voucherRepository;
            _taiKhoanBenhNhanChiRepository = taiKhoanBenhNhanChiRepository;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _yeuCauKhamBenhDonThuocRepo = yeuCauKhamBenhDonThuocRepo;
            _yeuCauKhamBenhChuanDoanRepo = yeuCauKhamBenhChuanDoanRepo;
            _donViHanhChinhRepo = donViHanhChinhRepo;
            _khoDuocPhamViTriRepo = khoDuocPhamViTriRepo;
            _aDRRepo = aDRRepo;
            _benhNhanDiUngThuocRepo = benhNhanDiUngThuocRepo;
            _duocPhamVaVatTuBenhVienService = duocPhamVaVatTuBenhVienService;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _nhapKhoVatTuChiTietRepo = nhapKhoVatTuChiTietRepo;
            _vatTuRepo = vatTuRepo;
            _donVTYTThanhToanRepo = donVTYTThanhToanRepo;
            _xuatKhoVatTuRepo = xuatKhoVatTuRepo;
            _xuatKhoDuocPhamRepo = xuatKhoDuocPhamRepo;
            _benhVienRepository = benhVienRepository;
            _bacSiChiDinhRepository = bacSiChiDinhRepository;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _yeuCauKhamBenhDonThuocChiTietRepository = yeuCauKhamBenhDonThuocChiTietRepository;
            _userRepository = userRepository;
            _duocPhamBenhVienPhanNhomRepository = duocPhamBenhVienPhanNhomRepository;
            _donThuocThanhToanRepository = donThuocThanhToanRepository;
            _donVTYTThanhToanRepository = donVTYTThanhToanRepository;
            _donThuocThanhToanChiTietRepository = donThuocThanhToanChiTietRepository;
            _donVTYTThanhToanChiTietRepository = donVTYTThanhToanChiTietRepository;
        }

        public async Task NguoiBenhKhongMuaDonThuoc(long yeuCauTiepNhanId)
        {
            //huy don thuoc
            var dsDonThuocCanHuys = _donThuocThanhToanRepository.Table
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
                .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet)
                .Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT
                && o.YeuCauTiepNhanId == yeuCauTiepNhanId).ToList();

            foreach (var donthuocCanHuy in dsDonThuocCanHuys)
            {
                foreach (var donThuocThanhToanChiTiet in donthuocCanHuy.DonThuocThanhToanChiTiets)
                {
                    foreach (var congNo in donThuocThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                    {
                        congNo.WillDelete = true;
                    }
                    foreach (var mienGiam in donThuocThanhToanChiTiet.MienGiamChiPhis)
                    {
                        mienGiam.WillDelete = true;
                    }
                    foreach (var taiKhoanBenhNhanChi in donThuocThanhToanChiTiet.TaiKhoanBenhNhanChis)
                    {
                        taiKhoanBenhNhanChi.DonThuocThanhToanChiTietId = null;
                    }
                    donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = Math.Round(donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat, 2);
                    donThuocThanhToanChiTiet.WillDelete = true;
                    donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                    donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                }
                donthuocCanHuy.WillDelete = true;
            }
            if (dsDonThuocCanHuys.Any())
            {
                _donThuocThanhToanRepository.Context.SaveChanges();
            }
            //huy don vat tu
            var dsDonVTYTCanHuys = _donVTYTThanhToanRepository.Table
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet)
                    .Include(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.XuatKhoVatTuChiTiet)
                    .Where(o => o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan
                && o.YeuCauTiepNhanId == yeuCauTiepNhanId).ToList();

            foreach (var donVTYTCanHuy in dsDonVTYTCanHuys)
            {
                foreach (var donVTYTThanhToanChiTiet in donVTYTCanHuy.DonVTYTThanhToanChiTiets)
                {
                    foreach (var congNo in donVTYTThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                    {
                        congNo.WillDelete = true;
                    }
                    foreach (var mienGiam in donVTYTThanhToanChiTiet.MienGiamChiPhis)
                    {
                        mienGiam.WillDelete = true;
                    }
                    foreach (var taiKhoanBenhNhanChi in donVTYTThanhToanChiTiet.TaiKhoanBenhNhanChis)
                    {
                        taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietId = null;
                    }
                    donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat = Math.Round(donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat - donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat, 2);
                    donVTYTThanhToanChiTiet.WillDelete = true;
                    donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                    donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                }
                donVTYTCanHuy.WillDelete = true;
            }
            if (dsDonVTYTCanHuys.Any())
            {
                _donVTYTThanhToanRepository.Context.SaveChanges();
            }
        }

        #region Câp nhật 12/04/2021

        public async Task<List<ThongTinPhieuThuQuayThuocVo>> GetSoPhieu(DropDownListRequestModel model, long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanRepo.GetByIdAsync(yeuCauTiepNhanId,
                o => o.Include(x => x.TaiKhoanBenhNhanThus).ThenInclude(c => c.PhieuHoanUng).Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.TaiKhoanBenhNhanThu));

            var dsPhieuThu = yeuCauTiepNhan.TaiKhoanBenhNhanThus.Where(o =>
                o.LoaiNoiThu == Enums.LoaiNoiThu.NhaThuoc && (o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi));

            var dsPhieu = dsPhieuThu.Select(o => new ThongTinPhieuThuQuayThuocVo
            {
                KeyId = o.Id,
                DisplayName = o.SoPhieuHienThi,
                DaHuy = o.DaHuy ?? false,
                DaThuHoi = o.DaThuHoi,
                NgayLap = o.NgayThu
            }).OrderByDescending(o => o.NgayLap).ToList();

            return dsPhieu;
        }

        public ThongTinPhieuThuQuayThuoc GetThongTinPhieuThuQuayThuoc(long soPhieuId)
        {
            ThongTinPhieuThuQuayThuoc thongTinPhieuThuQuayThuoc = null;
            var taiKhoanBenhNhanThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
                                                                    .Include(x => x.NhanVienThuHoi).ThenInclude(c => c.User)
                                                                    .Include(x => x.NhanVienHuy).ThenInclude(c => c.User)
                                                                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
                                                                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                                                                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                                                                    .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                                                                    .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans)
                                                                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTietTheoPhieuThu).ThenInclude(o => o.DuocPham).ThenInclude(c => c.DonViTinh)
                                                                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DuocPham).ThenInclude(c => c.DonViTinh)
                                                                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonVTYTThanhToanChiTietTheoPhieuThu)
                                                                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonVTYTThanhToanChiTiet)
                                                                    .FirstOrDefault(o => o.Id == soPhieuId);


            if (taiKhoanBenhNhanThu != null)
            {
                var tongBHYTTT = taiKhoanBenhNhanThu.TaiKhoanBenhNhanChis.Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100).DefaultIfEmpty().Sum();
                var tongChiPhiBNTT = taiKhoanBenhNhanThu.TaiKhoanBenhNhanChis.Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(o => o.TienChiPhi.GetValueOrDefault()).DefaultIfEmpty().Sum();
                var tongMienGiam = taiKhoanBenhNhanThu.MienGiamChiPhis.Select(o => o.SoTien).DefaultIfEmpty().Sum();
                var tongCongNoBaoHiemTuNhan = taiKhoanBenhNhanThu.CongTyBaoHiemTuNhanCongNos.Select(o => o.SoTien).DefaultIfEmpty().Sum();

                var tongChiPhi = tongChiPhiBNTT + tongMienGiam + tongCongNoBaoHiemTuNhan + tongBHYTTT;
                var benhNhanThanhToan = tongChiPhiBNTT + tongCongNoBaoHiemTuNhan;

                thongTinPhieuThuQuayThuoc = new ThongTinPhieuThuQuayThuoc
                {
                    Id = taiKhoanBenhNhanThu.Id,
                    SoPhieu = taiKhoanBenhNhanThu.SoPhieuHienThi,
                    DaHuy = taiKhoanBenhNhanThu.DaHuy,

                    TongChiPhi = tongChiPhi,
                    BHYTThanhToan = tongBHYTTT,
                    MienGiam = tongMienGiam,
                    BenhNhanThanhToan = benhNhanThanhToan,
                    TienMat = tongChiPhiBNTT < (taiKhoanBenhNhanThu.TamUng.GetValueOrDefault() + taiKhoanBenhNhanThu.CongNo.GetValueOrDefault())
                                             ? (tongChiPhiBNTT - taiKhoanBenhNhanThu.CongNo.GetValueOrDefault())
                                               : taiKhoanBenhNhanThu.TamUng.GetValueOrDefault() + taiKhoanBenhNhanThu.TienMat.GetValueOrDefault(),
                    ChuyenKhoan = taiKhoanBenhNhanThu.ChuyenKhoan.GetValueOrDefault(),
                    Pos = taiKhoanBenhNhanThu.POS.GetValueOrDefault(),
                    CongNo = taiKhoanBenhNhanThu.CongNo.GetValueOrDefault() + tongCongNoBaoHiemTuNhan,


                    NgayThu = taiKhoanBenhNhanThu.NgayThu,
                    NoiDungThu = taiKhoanBenhNhanThu.NoiDungThu,

                    DaThuHoi = taiKhoanBenhNhanThu.DaThuHoi.GetValueOrDefault(),
                    NgayThuHoi = taiKhoanBenhNhanThu.NgayThuHoi,
                    NguoiThuHoiId = taiKhoanBenhNhanThu.NhanVienThuHoiId,
                    NguoiThuHoi = taiKhoanBenhNhanThu.NhanVienThuHoi?.User.HoTen,
                    NhanVienHuyPhieu = taiKhoanBenhNhanThu.NhanVienHuy?.User.HoTen,
                    NgayHuy = taiKhoanBenhNhanThu.NgayHuy
                };

                foreach (var taiKhoanBenhNhanChi in taiKhoanBenhNhanThu.TaiKhoanBenhNhanChis.Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi))
                {
                    var chiPhiDichVu = new ChiPhiThuocVatTuVo()
                    {
                        TaiKhoanBenhNhanChiId = taiKhoanBenhNhanChi.Id,
                        Soluong = taiKhoanBenhNhanChi.SoLuong.GetValueOrDefault(),
                        DonGia = taiKhoanBenhNhanChi.Gia.GetValueOrDefault(),
                        KhongTinhPhi = false,
                        KiemTraBHYTXacNhan = true,
                        DuocHuongBHYT = taiKhoanBenhNhanChi.MucHuongBaoHiem.GetValueOrDefault() > 0,
                        DonGiaBHYT = taiKhoanBenhNhanChi.DonGiaBaoHiem.GetValueOrDefault(),
                        TiLeBaoHiemThanhToan = taiKhoanBenhNhanChi.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuongBaoHiem = taiKhoanBenhNhanChi.MucHuongBaoHiem.GetValueOrDefault(),
                        SoTienMG = taiKhoanBenhNhanChi.SoTienMienGiam.GetValueOrDefault(),

                        DaThanhToan = taiKhoanBenhNhanChi.TienChiPhi.GetValueOrDefault(),
                        DaHoanThu = taiKhoanBenhNhanThu.DaHuy != true && taiKhoanBenhNhanChi.DaHuy == true,
                        NgayThu = taiKhoanBenhNhanThu.NgayThu
                    };

                    if (taiKhoanBenhNhanChi.DonThuocThanhToanChiTietTheoPhieuThu != null)
                    {
                        chiPhiDichVu.Id = taiKhoanBenhNhanChi.DonThuocThanhToanChiTietTheoPhieuThu.Id;
                        chiPhiDichVu.NgayPhatSinh = taiKhoanBenhNhanChi.DonThuocThanhToanChiTietTheoPhieuThu.NgayPhatSinh;
                        chiPhiDichVu.DichVuBenhVienId = taiKhoanBenhNhanChi.DonThuocThanhToanChiTietTheoPhieuThu.DuocPhamId;
                        chiPhiDichVu.LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.DuocPhamBenhVien;
                        chiPhiDichVu.Ma = string.Empty;
                        chiPhiDichVu.Nhom = LoaiDuocPhamHoacVatTu.DuocPhamBenhVien.GetDescription();
                        chiPhiDichVu.TenDichVu = taiKhoanBenhNhanChi.DonThuocThanhToanChiTietTheoPhieuThu.Ten;
                        chiPhiDichVu.DonViTinh = taiKhoanBenhNhanChi.DonThuocThanhToanChiTietTheoPhieuThu.DonViTinh?.Ten;
                        chiPhiDichVu.LoaiGia = string.Empty;
                        chiPhiDichVu.SoTienBaoHiemTuNhanChiTra = taiKhoanBenhNhanChi.DonThuocThanhToanChiTietTheoPhieuThu.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault();
                        chiPhiDichVu.SoTienMG = taiKhoanBenhNhanChi.DonThuocThanhToanChiTietTheoPhieuThu.SoTienMienGiam.GetValueOrDefault();
                    }
                    else if (taiKhoanBenhNhanChi.DonThuocThanhToanChiTiet != null)
                    {
                        chiPhiDichVu.Id = taiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.Id;
                        chiPhiDichVu.NgayPhatSinh = taiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.CreatedOn;
                        chiPhiDichVu.DichVuBenhVienId = taiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DuocPhamId;
                        chiPhiDichVu.LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.DuocPhamBenhVien;
                        chiPhiDichVu.Ma = string.Empty;
                        chiPhiDichVu.Nhom = LoaiDuocPhamHoacVatTu.DuocPhamBenhVien.GetDescription();
                        chiPhiDichVu.TenDichVu = taiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.Ten;
                        chiPhiDichVu.DonViTinh = taiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DonViTinh?.Ten;
                        chiPhiDichVu.LoaiGia = string.Empty;
                        chiPhiDichVu.SoTienBaoHiemTuNhanChiTra = taiKhoanBenhNhanChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault();
                        chiPhiDichVu.SoTienMG = taiKhoanBenhNhanChi.SoTienMienGiam.GetValueOrDefault();
                    }
                    else if (taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietTheoPhieuThu != null)
                    {
                        chiPhiDichVu.Id = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietTheoPhieuThu.Id;
                        chiPhiDichVu.NgayPhatSinh = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietTheoPhieuThu.NgayPhatSinh;
                        chiPhiDichVu.DichVuBenhVienId = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietTheoPhieuThu.VatTuBenhVienId;
                        chiPhiDichVu.LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.VatTuBenhVien;
                        chiPhiDichVu.Ma = string.Empty;
                        chiPhiDichVu.Nhom = LoaiDuocPhamHoacVatTu.VatTuBenhVien.GetDescription();
                        chiPhiDichVu.TenDichVu = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietTheoPhieuThu.Ten;
                        chiPhiDichVu.DonViTinh = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietTheoPhieuThu.DonViTinh;
                        chiPhiDichVu.LoaiGia = string.Empty;
                        chiPhiDichVu.SoTienBaoHiemTuNhanChiTra = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietTheoPhieuThu.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault();
                        chiPhiDichVu.SoTienMG = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietTheoPhieuThu.SoTienMienGiam.GetValueOrDefault();
                    }
                    else if (taiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet != null)
                    {
                        chiPhiDichVu.Id = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.Id;
                        chiPhiDichVu.NgayPhatSinh = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.CreatedOn;
                        chiPhiDichVu.DichVuBenhVienId = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.VatTuBenhVienId;
                        chiPhiDichVu.LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.VatTuBenhVien;
                        chiPhiDichVu.Ma = string.Empty;
                        chiPhiDichVu.Nhom = LoaiDuocPhamHoacVatTu.VatTuBenhVien.GetDescription();
                        chiPhiDichVu.TenDichVu = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.Ten;
                        chiPhiDichVu.DonViTinh = taiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.DonViTinh;
                        chiPhiDichVu.LoaiGia = string.Empty;
                        chiPhiDichVu.SoTienBaoHiemTuNhanChiTra = taiKhoanBenhNhanChi.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault();
                        chiPhiDichVu.SoTienMG = taiKhoanBenhNhanChi.SoTienMienGiam.GetValueOrDefault();
                    }

                    thongTinPhieuThuQuayThuoc.DanhSachThuPhis.Add(chiPhiDichVu);
                }

            }
            return thongTinPhieuThuQuayThuoc;
        }

        public void HuyPhieuThu(ThongTinHuyPhieuVo thongTinHuyPhieuVo)
        {
            var phieuThu = _taiKhoanBenhNhanThuRepository.GetById(thongTinHuyPhieuVo.SoPhieu,
                o => o.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.DonVTYTThanhToans).ThenInclude(x => x.DonVTYTThanhToanChiTiets)
                    .Include(x => x.TaiKhoanBenhNhan)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.TaiKhoanBenhNhanThus)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.DonThuocThanhToanChiTiet).ThenInclude(ct => ct.DonThuocThanhToan)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.DonVTYTThanhToanChiTiet).ThenInclude(ct => ct.DonVTYTThanhToan)
                    .Include(x => x.CongTyBaoHiemTuNhanCongNos).ThenInclude(ct => ct.CongTyBaoHiemTuNhan)
                    .Include(x => x.MienGiamChiPhis));
            if (phieuThu.DaHuy == true)
            {
                throw new Exception("Phiếu thu đã được hủy");
            }

            foreach (var phieuThuCongTyBaoHiemTuNhanCongNo in phieuThu.CongTyBaoHiemTuNhanCongNos)
            {
                phieuThuCongTyBaoHiemTuNhanCongNo.CongTyBaoHiemTuNhan.CongTyBaoHiemTuNhanCongNos.Add(new CongTyBaoHiemTuNhanCongNo
                {
                    TaiKhoanBenhNhanThuId = null,
                    SoTien = phieuThuCongTyBaoHiemTuNhanCongNo.SoTien,
                    YeuCauKhamBenhId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauKhamBenhId,
                    YeuCauDichVuKyThuatId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauDichVuKyThuatId,
                    YeuCauDuocPhamBenhVienId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauDuocPhamBenhVienId,
                    YeuCauVatTuBenhVienId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauVatTuBenhVienId,
                    YeuCauDichVuGiuongBenhVienId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauDichVuGiuongBenhVienId,
                    DonThuocThanhToanChiTietId = phieuThuCongTyBaoHiemTuNhanCongNo.DonThuocThanhToanChiTietId,
                    YeuCauGoiDichVuId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauGoiDichVuId,
                    DonVTYTThanhToanChiTietId = phieuThuCongTyBaoHiemTuNhanCongNo.DonVTYTThanhToanChiTietId,
                    YeuCauTruyenMauId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauTruyenMauId,
                    YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                });
            }
            foreach (var mienGiamChiPhi in phieuThu.MienGiamChiPhis)
            {
                phieuThu.YeuCauTiepNhan.MienGiamChiPhis.Add(new MienGiamChiPhi
                {
                    TaiKhoanBenhNhanThuId = null,
                    LoaiMienGiam = mienGiamChiPhi.LoaiMienGiam,
                    SoTien = mienGiamChiPhi.SoTien,
                    TheVoucherId = mienGiamChiPhi.TheVoucherId,
                    YeuCauKhamBenhId = mienGiamChiPhi.YeuCauKhamBenhId,
                    YeuCauDichVuKyThuatId = mienGiamChiPhi.YeuCauDichVuKyThuatId,
                    YeuCauDuocPhamBenhVienId = mienGiamChiPhi.YeuCauDuocPhamBenhVienId,
                    YeuCauVatTuBenhVienId = mienGiamChiPhi.YeuCauVatTuBenhVienId,
                    YeuCauDichVuGiuongBenhVienId = mienGiamChiPhi.YeuCauDichVuGiuongBenhVienId,
                    YeuCauGoiDichVuId = mienGiamChiPhi.YeuCauGoiDichVuId,
                    DonThuocThanhToanChiTietId = mienGiamChiPhi.DonThuocThanhToanChiTietId,
                    DonVTYTThanhToanChiTietId = mienGiamChiPhi.DonVTYTThanhToanChiTietId,
                    LoaiChietKhau = mienGiamChiPhi.LoaiChietKhau,
                    TiLe = mienGiamChiPhi.TiLe,
                    MaTheVoucher = mienGiamChiPhi.MaTheVoucher,
                    DoiTuongUuDaiId = mienGiamChiPhi.DoiTuongUuDaiId,
                    YeuCauTruyenMauId = mienGiamChiPhi.YeuCauTruyenMauId,
                    YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = mienGiamChiPhi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId
                });
            }
            foreach (var phieuThuTaiKhoanBenhNhanChi in phieuThu.TaiKhoanBenhNhanChis.Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi))
            {
                phieuThuTaiKhoanBenhNhanChi.DaHuy = true;
                phieuThuTaiKhoanBenhNhanChi.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
                phieuThuTaiKhoanBenhNhanChi.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
                phieuThuTaiKhoanBenhNhanChi.LyDoHuy = thongTinHuyPhieuVo.LyDo;

                if (phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet != null)
                {
                    phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.SoTienBenhNhanDaChi = 0;
                    phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;
                    phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTietId = null;
                }
                if (phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet != null)
                {
                    phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.SoTienBenhNhanDaChi = 0;
                    phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;
                    phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTietId = null;
                }
            }

            phieuThu.DaHuy = true;
            phieuThu.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
            phieuThu.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
            phieuThu.NgayHuy = DateTime.Now;
            phieuThu.LyDoHuy = thongTinHuyPhieuVo.LyDo;
            phieuThu.DaThuHoi = thongTinHuyPhieuVo.ThoiGianThuHoi != null;
            phieuThu.NhanVienThuHoiId = thongTinHuyPhieuVo.NguoiThuHoiId;
            phieuThu.NgayThuHoi = thongTinHuyPhieuVo.ThoiGianThuHoi;

            _taiKhoanBenhNhanThuRepository.Update(phieuThu);
        }

        public void CapnhatNguoiThuHoiPhieuThuThuoc(ThongTinHuyPhieuVo thongTinHuyPhieuVo)
        {
            var phieuThuThuoc = _taiKhoanBenhNhanThuRepository.GetById(thongTinHuyPhieuVo.SoPhieu, o => o.Include(x => x.TaiKhoanBenhNhan));

            phieuThuThuoc.LyDoHuy = thongTinHuyPhieuVo.LyDo;
            phieuThuThuoc.DaThuHoi = thongTinHuyPhieuVo.ThoiGianThuHoi != null;
            phieuThuThuoc.NhanVienThuHoiId = thongTinHuyPhieuVo.NguoiThuHoiId;
            phieuThuThuoc.NgayThuHoi = thongTinHuyPhieuVo.ThoiGianThuHoi;

            _taiKhoanBenhNhanThuRepository.Update(phieuThuThuoc);
        }

        #endregion
        public List<ThongTinBenhNhanGridVo> FindQuayThuoc(string search)
        {
            if (!String.IsNullOrEmpty(search))
            {
                var queryString = JsonConvert.DeserializeObject<QuayThuocGridVo>(search);
                if (queryString.MaTiepNhan == null && queryString.SoDienThoai == null && queryString.HoTen == null && queryString.MaBenhNhan == null && queryString.DateStart == null && queryString.DateEnd == null)
                {
                    return null;
                }
                else
                {
                    DateTime? tuNgayPart = null;
                    DateTime? denNgaysPart = null;
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()))
                    {
                        if (DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay))
                        {
                            tuNgayPart = tuNgay;
                        }
                    }
                    if (!string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        if (DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay))
                        {
                            denNgaysPart = denNgay;
                        }
                    }

                    var query = _yeuCauTiepNhanRepo.TableNoTracking
                                                                   .Where(o => o.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                                                               o.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc) ||
                                                                                                             (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && ((x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc) ||
                                                                                                             (x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan && x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)))
                                                                               ||
                                                                               o.DonVTYTThanhToans.Any(u => (u.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan || u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)))
                                                                               )

                               .Select(s => new ThongTinBenhNhanGridVo
                               {
                                   Id = s.Id,
                                   MaBN = s.BenhNhan != null ? s.BenhNhan.MaBN : "",
                                   BenhNhanId = s.BenhNhanId.GetValueOrDefault(),
                                   MaTN = s.MaYeuCauTiepNhan,
                                   YeuCauTiepNhanId = s.Id,
                                   HoTen = s.HoTen,
                                   NamSinh = s.NamSinh.ToString(),
                                   GioiTinh = s.GioiTinh,
                                   GioiTinhHienThi = s.GioiTinh.GetDescription(),
                                   DiaChi = s.DiaChiDayDu,
                                   SoDienThoai = s.SoDienThoai,
                                   SoDienThoaiDisPlay = s.SoDienThoaiDisplay,
                                   CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                                   TrangThai = s.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) || s.DonVTYTThanhToans.Any(k => (k.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan))) ? TrangThaiThanhToan.ChuaThanhToan : TrangThaiThanhToan.DaThanhToan,
                                   IsDisable = false
                               });
                    if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                    {
                        query = query.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                    {

                        query = query.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                    {
                        query = query.Where(x => x.SoDienThoai == queryString.SoDienThoai.Trim().ApplyFormatPhone());
                    }
                    if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                    {
                        query = query.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day, 0, 0, 0);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        query = query.Where(p => p.CreatedOn.Any(o => o >= TuNgayPart && o <= denNgay));
                        query = query.Select(s => new ThongTinBenhNhanGridVo
                        {
                            Id = s.Id,
                            MaBN = s.MaBN,
                            BenhNhanId = s.BenhNhanId,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh.ToString(),
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChiDayDu,
                            SoDienThoai = s.SoDienThoai,
                            SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                            DoiTuong = s.DoiTuong,
                            TrangThaiHienThi = s.TrangThaiHienThi,
                            TrangThai = s.TrangThai,
                            SoTien = s.SoTien,
                            SoTienNumber = s.SoTienNumber,
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = queryString.DateEnd,
                            IsDisable = false
                        });
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day, 0, 0, 0);
                        query = query.Where(p => p.CreatedOn.Any(o => o >= TuNgayPart));
                        query = query.Select(s => new ThongTinBenhNhanGridVo
                        {
                            Id = s.Id,
                            MaBN = s.MaBN,
                            BenhNhanId = s.BenhNhanId,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh.ToString(),
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChiDayDu,
                            SoDienThoai = s.SoDienThoai,
                            SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                            DoiTuong = s.DoiTuong,
                            TrangThaiHienThi = s.TrangThaiHienThi,
                            TrangThai = s.TrangThai,
                            SoTien = s.SoTien,
                            SoTienNumber = s.SoTienNumber,
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = null,
                            IsDisable = false
                        });
                    }
                    if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        query = query.Where(p => p.CreatedOn.Any(o => o <= denNgay));
                        query = query.Select(s => new ThongTinBenhNhanGridVo
                        {
                            Id = s.Id,
                            MaBN = s.MaBN,
                            BenhNhanId = s.BenhNhanId,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh.ToString(),
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChiDayDu,
                            SoDienThoai = s.SoDienThoai,
                            SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                            DoiTuong = s.DoiTuong,
                            TrangThaiHienThi = s.TrangThaiHienThi,
                            TrangThai = s.TrangThai,
                            SoTien = s.SoTien,
                            SoTienNumber = s.SoTienNumber,
                            CreatedOn = s.CreatedOn,
                            DateStart = null,
                            DateEnd = queryString.DateEnd,
                            IsDisable = false
                        });
                    }
                    var returnData = query.ToList().Distinct().ToList();
                    if (returnData.Any())
                    {
                        return returnData;
                    }
                    else   // toa thuốc cũ
                    {
                        //todo: need improve
                        var querytmp = _yeuCauTiepNhanRepo.TableNoTracking
                                     .Where(o => o.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat)

                            .Select(s => new ThongTinBenhNhanGridVo
                            {
                                MaBN = s.BenhNhan != null ? s.BenhNhan.MaBN : "",
                                BenhNhanId = s.BenhNhanId.GetValueOrDefault(),
                                MaTN = s.MaYeuCauTiepNhan,
                                HoTen = s.HoTen,
                                NamSinh = s.NamSinh.ToString(),
                                GioiTinh = s.GioiTinh,
                                DiaChi = s.DiaChiDayDu,
                                SoDienThoai = s.SoDienThoai,
                                SoDienThoaiDisPlay = s.SoDienThoaiDisplay,
                                ThongTinDonThuocVos = s.DonThuocThanhToans.Select(dt => new ThongTinDonThuocVo
                                {
                                    Id = dt.Id,
                                    TrangThai = dt.TrangThai,
                                    YeuCauKhamBenhId = dt.YeuCauKhamBenhId,
                                    CreatedOn = dt.CreatedOn
                                }).ToList(),
                                //DonThuocThanhToanExits = s.DonThuocThanhToans.Where(x => x.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc && x.YeuCauKhamBenh != null).Any() ? true : false,
                                //CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).ToList(),
                                IsDisable = true
                            });
                        if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                        {
                            querytmp = querytmp.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                        }
                        if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                        {

                            querytmp = querytmp.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                        }
                        if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                        {
                            querytmp = querytmp.Where(x => x.SoDienThoai == queryString.SoDienThoai.Trim());
                        }
                        if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                        {
                            querytmp = querytmp.Where(o =>o.HoTen == queryString.HoTen.Trim());
                        }
                        //if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                        //{
                        //    DateTime TuNgayPart = DateTime.Now;
                        //    DateTime DenNgaysPart = DateTime.Now;
                        //    DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        //    DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        //    querytmp = querytmp.Where(p => p.CreatedOn.Any(o => o >= TuNgayPart && o <= DenNgaysPart));
                        //    querytmp = querytmp.Select(s => new ThongTinBenhNhanGridVo()
                        //    {
                        //        MaBN = s.MaBN,
                        //        BenhNhanId = s.BenhNhanId,
                        //        MaTN = s.MaTN,
                        //        HoTen = s.HoTen,
                        //        NamSinh = s.NamSinh.ToString(),
                        //        GioiTinhHienThi = s.GioiTinhHienThi,
                        //        CreatedOn = s.CreatedOn,
                        //        DateStart = queryString.DateStart,
                        //        DateEnd = queryString.DateEnd,
                        //        SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                        //        IsDisable = true
                        //    });
                        //}
                        //DateTime? tuNgayPart = null;
                        //DateTime? denNgaysPart = null;
                        //if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()))
                        //{
                        //    if(DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay))
                        //    {
                        //        tuNgayPart = tuNgay;
                        //    }
                        //}
                        //if (!string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                        //{                            
                        //    if( DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay))
                        //    {
                        //        denNgaysPart = denNgay;
                        //    }
                        //}

                        var datatmp = querytmp.ToList();


                        return datatmp
                            .Where(p => p.ThongTinDonThuocVos.Any(dt => dt.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc && dt.YeuCauKhamBenhId != null && (tuNgayPart == null || tuNgayPart <= dt.CreatedOn) && (denNgaysPart == null || dt.CreatedOn <= denNgaysPart)))
                            .Select(s => new ThongTinBenhNhanGridVo
                            {
                                MaBN = s.MaBN,
                                BenhNhanId = s.BenhNhanId,
                                MaTN = s.MaTN,
                                HoTen = s.HoTen,
                                NamSinh = s.NamSinh,
                                GioiTinh = s.GioiTinh,
                                GioiTinhHienThi = s.GioiTinh?.GetDescription(),
                                DiaChi = s.DiaChi,
                                SoDienThoai = s.SoDienThoai,
                                SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                                DonThuocThanhToanExits = true,
                                IsDisable = true
                            }).Distinct().ToList();

                    }
                 }
            }
            return null;
        }
        public List<ThongTinBenhNhanGridVo> FindQuayThuocOld(string search)
        {
            if (!String.IsNullOrEmpty(search))
            {
                var queryString = JsonConvert.DeserializeObject<QuayThuocGridVo>(search);
                if (queryString.MaTiepNhan == null && queryString.SoDienThoai == null && queryString.HoTen == null && queryString.MaBenhNhan == null && queryString.DateStart == null && queryString.DateEnd == null)
                {
                    return null;
                }
                else
                {
                    var query = _yeuCauTiepNhanRepo.TableNoTracking
                                                                   .Where(o => o.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                                                               o.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc) ||
                                                                                                             (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && ((x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc) ||
                                                                                                             (x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan && x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)))
                                                                               ||
                                                                               o.DonVTYTThanhToans.Any(u => (u.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan || u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)))
                                                                               )

                               .Select(s => new ThongTinBenhNhanGridVo
                               {
                                   Id = s.Id,
                                   MaBN = s.BenhNhan != null ? s.BenhNhan.MaBN : "",
                                   BenhNhanId = s.BenhNhanId != null ? Convert.ToInt64(s.BenhNhanId) : 0,
                                   MaTN = s.MaYeuCauTiepNhan,
                                   YeuCauTiepNhanId = s.Id,
                                   HoTen = s.HoTen,
                                   NamSinh = s.NamSinh.ToString(),
                                   GioiTinhHienThi = s.GioiTinh.GetDescription(),
                                   DiaChi = s.DiaChiDayDu,
                                   SoDienThoai = s.SoDienThoai,
                                   SoDienThoaiDisPlay = s.SoDienThoai.ApplyFormatPhone(),
                                   CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                                   TrangThai = s.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) || s.DonVTYTThanhToans.Any(k => (k.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan))) ? TrangThaiThanhToan.ChuaThanhToan : TrangThaiThanhToan.DaThanhToan,
                                   IsDisable = false
                               }).Distinct();
                    if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                    {
                        query = query.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                    {

                        query = query.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                    {
                        query = query.Where(x => x.SoDienThoai == queryString.SoDienThoai.Trim().ApplyFormatPhone());
                    }
                    if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                    {
                        query = query.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day, 0, 0, 0);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        query = query.Where(p => p.CreatedOn.Any(o => o >= TuNgayPart && o <= denNgay));
                        query = query.Select(s => new ThongTinBenhNhanGridVo
                        {
                            Id = s.Id,
                            MaBN = s.MaBN,
                            BenhNhanId = s.BenhNhanId,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh.ToString(),
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChiDayDu,
                            SoDienThoai = s.SoDienThoai,
                            SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                            DoiTuong = s.DoiTuong,
                            TrangThaiHienThi = s.TrangThaiHienThi,
                            TrangThai = s.TrangThai,
                            SoTien = s.SoTien,
                            SoTienNumber = s.SoTienNumber,
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = queryString.DateEnd,
                            IsDisable = false
                        });
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day, 0, 0, 0);
                        query = query.Where(p => p.CreatedOn.Any(o => o >= TuNgayPart));
                        query = query.Select(s => new ThongTinBenhNhanGridVo
                        {
                            Id = s.Id,
                            MaBN = s.MaBN,
                            BenhNhanId = s.BenhNhanId,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh.ToString(),
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChiDayDu,
                            SoDienThoai = s.SoDienThoai,
                            SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                            DoiTuong = s.DoiTuong,
                            TrangThaiHienThi = s.TrangThaiHienThi,
                            TrangThai = s.TrangThai,
                            SoTien = s.SoTien,
                            SoTienNumber = s.SoTienNumber,
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = null,
                            IsDisable = false
                        });
                    }
                    if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        query = query.Where(p => p.CreatedOn.Any(o => o <= denNgay));
                        query = query.Select(s => new ThongTinBenhNhanGridVo
                        {
                            Id = s.Id,
                            MaBN = s.MaBN,
                            BenhNhanId = s.BenhNhanId,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh.ToString(),
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChiDayDu,
                            SoDienThoai = s.SoDienThoai,
                            SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                            DoiTuong = s.DoiTuong,
                            TrangThaiHienThi = s.TrangThaiHienThi,
                            TrangThai = s.TrangThai,
                            SoTien = s.SoTien,
                            SoTienNumber = s.SoTienNumber,
                            CreatedOn = s.CreatedOn,
                            DateStart = null,
                            DateEnd = queryString.DateEnd,
                            IsDisable = false
                        });
                    }
                    if (!query.Any())   // toa thuốc cũ
                    {
                        //todo: need improve
                        var querytmp = _yeuCauTiepNhanRepo.TableNoTracking
                                     .Where(o => o.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat && o.DonThuocThanhToans.Any())

                            .Select(s => new ThongTinBenhNhanGridVo
                            {
                                MaBN = s.BenhNhan != null ? s.BenhNhan.MaBN : "",
                                BenhNhanId = s.BenhNhanId != null ? Convert.ToInt64(s.BenhNhanId) : 0,
                                MaTN = s.MaYeuCauTiepNhan,
                                HoTen = s.HoTen,
                                NamSinh = s.NamSinh.ToString(),
                                GioiTinhHienThi = s.GioiTinh.GetDescription(),
                                DiaChi = s.DiaChiDayDu,
                                SoDienThoai = s.SoDienThoai,
                                SoDienThoaiDisPlay = s.SoDienThoai.ApplyFormatPhone(),
                                DonThuocThanhToanExits = s.DonThuocThanhToans.Where(x => x.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc && x.YeuCauKhamBenh != null).Any() ? true : false,
                                CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).ToList(),
                                IsDisable = true
                            }).Distinct();
                        if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                        {
                            querytmp = querytmp.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                        }
                        if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                        {

                            querytmp = querytmp.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                        }
                        if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                        {
                            querytmp = querytmp.Where(x => x.SoDienThoai == queryString.SoDienThoai.Trim());
                        }
                        if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                        {
                            querytmp = querytmp.Where(o => (queryString.HoTen == null || string.IsNullOrEmpty(queryString.HoTen.Trim()) || (!string.IsNullOrEmpty(queryString.HoTen.Trim()) && o.HoTen == queryString.HoTen.Trim())));
                        }
                        if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                        {
                            DateTime TuNgayPart = DateTime.Now;
                            DateTime DenNgaysPart = DateTime.Now;
                            DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                            DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                            querytmp = querytmp.Where(p => p.CreatedOn.Any(o => o >= TuNgayPart && o <= DenNgaysPart));
                            querytmp = querytmp.Select(s => new ThongTinBenhNhanGridVo()
                            {
                                MaBN = s.MaBN,
                                BenhNhanId = s.BenhNhanId,
                                MaTN = s.MaTN,
                                HoTen = s.HoTen,
                                NamSinh = s.NamSinh.ToString(),
                                GioiTinhHienThi = s.GioiTinhHienThi,
                                CreatedOn = s.CreatedOn,
                                DateStart = queryString.DateStart,
                                DateEnd = queryString.DateEnd,
                                SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                                IsDisable = true
                            });
                        }
                        if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                        {
                            DateTime TuNgayPart = DateTime.Now;
                            DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                            querytmp = querytmp.Where(p => p.CreatedOn.Any(o => o >= TuNgayPart));
                            querytmp = querytmp.Select(s => new ThongTinBenhNhanGridVo()
                            {
                                MaBN = s.MaBN,
                                BenhNhanId = s.BenhNhanId,
                                MaTN = s.MaTN,
                                HoTen = s.HoTen,
                                NamSinh = s.NamSinh.ToString(),
                                GioiTinhHienThi = s.GioiTinhHienThi,
                                CreatedOn = s.CreatedOn,
                                DateStart = queryString.DateStart,
                                DateEnd = null,
                                SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                                IsDisable = true
                            });
                        }
                        if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                        {
                            DateTime DenNgaysPart = DateTime.Now;
                            DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                            querytmp = querytmp.Where(p => p.CreatedOn.Any(o => o <= DenNgaysPart));
                            querytmp = querytmp.Select(s => new ThongTinBenhNhanGridVo()
                            {
                                MaBN = s.MaBN,
                                BenhNhanId = s.BenhNhanId,
                                MaTN = s.MaTN,
                                HoTen = s.HoTen,
                                NamSinh = s.NamSinh.ToString(),
                                GioiTinhHienThi = s.GioiTinhHienThi,
                                CreatedOn = s.CreatedOn,
                                DateStart = null,
                                DateEnd = queryString.DateEnd,
                                SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                                IsDisable = true
                            });
                        }
                        query = querytmp.Where(p => p.DonThuocThanhToanExits == true).Select(s => new ThongTinBenhNhanGridVo
                        {
                            MaBN = s.MaBN,
                            BenhNhanId = s.BenhNhanId,
                            MaTN = s.MaTN,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai,
                            SoDienThoaiDisPlay = s.SoDienThoaiDisPlay,
                            DonThuocThanhToanExits = s.DonThuocThanhToanExits,
                            IsDisable = true
                        });

                    }
                    return query.ToList();
                }
            }
            return null;
        }
        public YeuCauTiepNhan GetYeuCauTiepNhan(long tiepNhanId)
        {
            return _yeuCauTiepNhanRepo.TableNoTracking.FirstOrDefault(x => x.Id == tiepNhanId);
        }
        public List<CongTyBaoHiemTuNhanGridVo> CheckBenhNhanExistBaoHiemTuNhan(long id)
        {

            var query = _yeucauTiepNhanCongTyBaoHiemTuNhanRepo.TableNoTracking.Include(x => x.CongTyBaoHiemTuNhan).Where(x => x.YeuCauTiepNhanId == id).Select(s => new CongTyBaoHiemTuNhanGridVo
            {

                DenNgay = s.NgayHetHan != null ? s.NgayHetHan : null,
                TuNgay = s.NgayHieuLuc != null ? s.NgayHieuLuc : null,
                DenNgayHienThi = s.NgayHetHan != null ? Convert.ToDateTime(s.NgayHetHan).ApplyFormatDate() : null,
                TuNgayHienThi = s.NgayHieuLuc != null ? Convert.ToDateTime(s.NgayHieuLuc).ApplyFormatDate() : null,
                DiaChi = s.DiaChi,
                DienThoai = s.SoDienThoai,
                SoThe = s.MaSoThe,
                Id = s.CongTyBaoHiemTuNhanId,
                TenCongTy = s.CongTyBaoHiemTuNhan.Ten,
                SoTien = 0
            }).ToList();
            return query;
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, long yCTiepNhanId)
        {

            BuildDefaultSortExpression(queryInfo);
            var query = _yeucauTiepNhanCongTyBaoHiemTuNhanRepo.TableNoTracking.Include(x => x.CongTyBaoHiemTuNhan).Where(x => x.YeuCauTiepNhanId == yCTiepNhanId)
               .Select(s => new CongTyBaoHiemTuNhanGridVo
               {

                   DenNgay = s.NgayHetHan != null ? s.NgayHetHan : null,
                   TuNgay = s.NgayHieuLuc != null ? s.NgayHieuLuc : null,
                   DenNgayHienThi = s.NgayHetHan != null ? Convert.ToDateTime(s.NgayHetHan).ApplyFormatDate() : null,
                   TuNgayHienThi = s.NgayHieuLuc != null ? Convert.ToDateTime(s.NgayHieuLuc).ApplyFormatDate() : null,
                   DiaChi = s.DiaChi,
                   DienThoai = s.SoDienThoai,
                   SoThe = s.MaSoThe,
                   Id = s.CongTyBaoHiemTuNhanId,
                   TenCongTy = s.CongTyBaoHiemTuNhan.Ten,
                   SoTien = 0
               });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.SoThe);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo, long benhNhanID)
        {
            var query = _benhNhanCongTyBaoHiemTuNhanRepo.TableNoTracking.Include(x => x.CongTyBaoHiemTuNhan).Where(x => x.BenhNhanId == benhNhanID)
              .Select(s => new CongTyBaoHiemTuNhanGridVo
              {

                  DenNgay = s.NgayHetHan != null ? s.NgayHetHan : null,
                  TuNgay = s.NgayHieuLuc != null ? s.NgayHieuLuc : null,
                  DenNgayHienThi = s.NgayHetHan != null ? Convert.ToDateTime(s.NgayHetHan).ApplyFormatDate() : null,
                  TuNgayHienThi = s.NgayHieuLuc != null ? Convert.ToDateTime(s.NgayHieuLuc).ApplyFormatDate() : null,
                  DiaChi = s.DiaChi,
                  DienThoai = s.SoDienThoai,
                  SoThe = s.MaSoThe,
                  Id = s.CongTyBaoHiemTuNhanId,
                  TenCongTy = s.CongTyBaoHiemTuNhan.Ten,
                  SoTien = 0

              });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.SoThe);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuAsync(DropDownListRequestModel queryInfo)
        {
            var duocPhamVaVatTus = await _duocPhamVaVatTuBenhVienService.GetDuocPhamVaVatTuTrongKho(false, queryInfo.Query, (long)Enums.EnumKhoDuocPham.KhoNhaThuoc, queryInfo.Take);

            return duocPhamVaVatTus.Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
            {
                DisplayName = s.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                    ? (s.Ten + "-" + s.HoatChat).ToString().Replace("\n", "")
                    : s.Ten,
                KeyId = s.Id,
                Ten = s.Ten,
                HoatChat = s.HoatChat,
                DonViTinh = s.DonViTinh,
                DuongDung = s.DuongDung,
                SoLuongTon = Math.Round(s.SoLuongTon, 1),
                HanSuDung = s.HanSuDung?.ApplyFormatDate(),
                Loai = s.LoaiDuocPhamHoacVatTu.GetDescription(),
                LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu,
                HamLuong = s.HamLuong,
                NhaSanXuat = s.NhaSanXuat
            }).ToList();
        }

        public async Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuDinhMucAsync(DropDownListRequestModel queryInfo)
        {
            var duocPhamVaVatTus = await _duocPhamVaVatTuBenhVienService.GetDuocPhamVaVatTuTrongKho(false, queryInfo.Query, (long)Enums.EnumKhoDuocPham.KhoNhaThuoc, queryInfo.Take);
            var dpVT = duocPhamVaVatTus.Select(s => new DuocPhamVaVatTuTNhaThuocTemplateVo
            {
                DisplayName = s.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                    ? s.Ten + " - " + s.HoatChat
                    : s.Ten,
                KeyId = s.Id,
                Ten = s.Ten,
                HoatChat = s.HoatChat,
                DonViTinh = s.DonViTinh,
                DuongDung = s.DuongDung,
                SoLuongTon = Math.Round(s.SoLuongTon, 1),
                HanSuDung = s.HanSuDung?.ApplyFormatDate(),
                Loai = s.LoaiDuocPhamHoacVatTu.GetDescription(),
                LoaiDuocPhamHoacVatTu = s.LoaiDuocPhamHoacVatTu,
                HamLuong = s.HamLuong,
                NhaSanXuat = s.NhaSanXuat
            }).ToList();
            if (queryInfo.Id != null && queryInfo.Id != 0)
            {
                return dpVT.Where(d => d.KeyId == queryInfo.Id).ToList();
            }

            return dpVT;
        }
        public async Task<string> GetTenBacSI(long userId)
        {
            //lay ds duoc pham co trong kho ngoai
            var query = _userRepo.TableNoTracking.Where(x => x.Id == userId).FirstOrDefault();
            if (query != null)
            {
                return query.HoTen;
            }
            return null;
        }
        public async Task<string> GetTenBenhNhan(long benhNhanId)
        {
            //lay ds duoc pham co trong kho ngoai
            var query = _benhNhanRepo.TableNoTracking.Where(x => x.MaBN == Convert.ToString(benhNhanId)).FirstOrDefault();
            if (query != null)
            {
                return query.HoTen;
            }
            return null;
        }

        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetThongTinDuocPham(long duocPhamId, long loaiDuocPhamHoacVatTu)
        {
            if (loaiDuocPhamHoacVatTu == 1)
            {
                var resultDuocPham = await _nhapKhoDuocPhamChiTietRepo.TableNoTracking
                .Where(kho =>
                    kho.DuocPhamBenhVienId == duocPhamId &&
                    kho.LaDuocPhamBHYT == false &&
                    kho.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc &&
                    kho.SoLuongDaXuat < kho.SoLuongNhap && kho.HanSuDung >= DateTime.Now)
                .Select(x => new ThongTinDuocPhamQuayThuocVo()
                {
                    DuocPhamId = x.DuocPhamBenhViens.DuocPham.Id,
                    MaHoatChat = x.DuocPhamBenhViens.DuocPham.HoatChat,
                    TenDuocPham = x.DuocPhamBenhViens.DuocPham.Ten,
                    SoLuongTon = x.SoLuongNhap - x.SoLuongDaXuat,
                    NhapKhoDuocPhamChiTietId = x.Id,
                    TenHoatChat = x.DuocPhamBenhViens.DuocPham.HoatChat,
                    DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    //SoLuongToa = ,
                    SoLuongMua = 0,
                    DonGiaNhap = x.DonGiaNhap,
                    DonGia = x.DonGiaBan,
                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                    VAT = x.VAT,
                    Solo = x.Solo,
                    ViTri = x.KhoDuocPhamViTri.Ten,
                    HanSuDung = x.HanSuDung,
                    isNew = true,
                    LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                }).ToListAsync();
                return resultDuocPham.OrderBy(o => o.HanSuDung).ToList();
            }
            else
            {
                var resultVatTu = await _nhapKhoVatTuChiTietRepo.TableNoTracking
                .Where(kho =>
                    kho.VatTuBenhVienId == duocPhamId &&
                    kho.LaVatTuBHYT == false &&
                    kho.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc &&
                    kho.SoLuongDaXuat < kho.SoLuongNhap && kho.LaVatTuBHYT == false && kho.HanSuDung >= DateTime.Now)
               .Select(x => new ThongTinDuocPhamQuayThuocVo()
               {
                   DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                   TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                   SoLuongTon = x.SoLuongNhap - x.SoLuongDaXuat,
                   NhapKhoDuocPhamChiTietId = x.Id,
                   DonViTinh = x.VatTuBenhVien.VatTus.DonViTinh,
                   SoLuongToa = 0,
                   SoLuongMua = 0,
                   DonGiaNhap = x.DonGiaNhap,
                   DonGia = x.DonGiaBan,
                   TiLeTheoThapGia = x.TiLeTheoThapGia,
                   VAT = x.VAT,
                   Solo = x.Solo,
                   ViTri = x.KhoViTri.Ten,
                   HanSuDung = x.HanSuDung,
                   isNew = true,
                   LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien
               }).ToListAsync();
                return resultVatTu.OrderBy(o => o.HanSuDung).ToList();
            }
            return null;
        }

        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocChoThanhToan(long tiepNhanId)
        {
            var result = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                            o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                            o.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan)
                .SelectMany(o => o.DonThuocThanhToanChiTiets)
                .Select(x => new ThongTinDuocPhamQuayThuocVo()
                {
                    Id = x.Id,
                    DuocPhamId = x.DuocPham.Id,
                    MaHoatChat = x.DuocPham.HoatChat,
                    TenDuocPham = x.DuocPham.Ten,
                    NongDoVaHamLuong = x.DuocPham.HamLuong,
                    SoLuongTon = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongNhap - x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat,
                    NhapKhoDuocPhamChiTietId = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Id,
                    TenHoatChat = x.DuocPham.HoatChat,
                    DonViTinh = x.DuocPham.DonViTinh.Ten,
                    SoLuongToa = x.YeuCauKhamBenhDonThuocChiTiet != null ? x.YeuCauKhamBenhDonThuocChiTiet.SoLuong : 0,
                    SoLuongMua = x.SoLuong,
                    DonGiaNhap = x.DonGiaNhap,
                    DonGia = x.DonGiaBan,
                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                    VAT = x.VAT,
                    DanhSachCongNoChoThus = x.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<CongNoChoThuGridVo>() : x.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new CongNoChoThuGridVo { CongNoId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }),
                    DanhSachMienGiamVos = x.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : x.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null).GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                        (k, v) => new DanhSachMienGiamVo
                        {
                            LoaiMienGiam = k.LoaiMienGiam,
                            LoaiChietKhau = k.LoaiChietKhau,
                            TheVoucherId = k.TheVoucherId,
                            MaTheVoucher = k.MaTheVoucher,
                            DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                            TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                            SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                        }).Where(o => o.SoTien != 0),
                    GhiChuMienGiamThem = x.GhiChuMienGiamThem,
                    Solo = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Solo,
                    ViTri = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.KhoDuocPhamViTri != null ? x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.KhoDuocPhamViTri.Ten : null,
                    HanSuDung = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung,
                    isNew = false,
                    BacSiKeToa = x.YeuCauKhamBenhDonThuocChiTiet != null ? x.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen : string.Empty,
                    CheckedDefault = true,
                    LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien,
                });
            var resultVatTu = _donVTYTThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
               .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                           o.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan)
               .SelectMany(o => o.DonVTYTThanhToanChiTiets)
               .Select(x => new ThongTinDuocPhamQuayThuocVo()
               {
                   Id = x.Id,
                   DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                   TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                   SoLuongTon = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongNhap - x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat,
                   NhapKhoDuocPhamChiTietId = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Id,
                   DonViTinh = x.VatTuBenhVien.VatTus.DonViTinh,
                   SoLuongToa = x.YeuCauKhamBenhDonVTYTChiTietId != null ? x.YeuCauKhamBenhDonVTYTChiTiet.SoLuong : 0,
                   SoLuongMua = x.SoLuong,
                   DonGiaNhap = x.DonGiaNhap,
                   DonGia = x.DonGiaBan,
                   TiLeTheoThapGia = x.TiLeTheoThapGia,
                   VAT = x.VAT,
                   DanhSachCongNoChoThus = x.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<CongNoChoThuGridVo>() : x.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new CongNoChoThuGridVo { CongNoId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }),
                   DanhSachMienGiamVos = x.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : x.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null).GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                       (k, v) => new DanhSachMienGiamVo
                       {
                           LoaiMienGiam = k.LoaiMienGiam,
                           LoaiChietKhau = k.LoaiChietKhau,
                           TheVoucherId = k.TheVoucherId,
                           MaTheVoucher = k.MaTheVoucher,
                           DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                           TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                           SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                       }).Where(o => o.SoTien != 0),
                   GhiChuMienGiamThem = x.GhiChuMienGiamThem,
                   Solo = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Solo,
                   ViTri = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTri != null ? x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTri.Ten : null,
                   HanSuDung = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HanSuDung,
                   isNew = false,
                   BacSiKeToa = x.YeuCauKhamBenhDonVTYTChiTietId != null ? x.YeuCauKhamBenhDonVTYTChiTiet.YeuCauKhamBenhDonVTYT.BacSiKeDon.User.HoTen : string.Empty,
                   CheckedDefault = true,
                   LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien,
               });

            //var listThuocChuaThanhToan = result.Union(resultVatTu).OrderBy("Id asc").ToListAsync();
            var listThuocChuaThanhToan = result.Union(resultVatTu).ToList();
            foreach (var itemx in listThuocChuaThanhToan)
            {
                var danhSachMienGiamVos = itemx.DanhSachMienGiamVos.ToList();
                //add MienGiamThem
                if (!danhSachMienGiamVos.Any())
                {
                    var item = new DanhSachMienGiamVo()
                    {
                        LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                        LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                        TiLe = 0,
                        SoTien = 0
                    };
                    danhSachMienGiamVos.Add(item);
                    var item1 = new DanhSachMienGiamVo()
                    {
                        LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                        LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                        SoTien = 0
                    };
                    danhSachMienGiamVos.Add(item1);
                }
                itemx.DanhSachMienGiamVos = danhSachMienGiamVos;
            }

            return listThuocChuaThanhToan.ToList();
        }

        public async Task<List<CongTyBaoHiemTuNhanGridVo>> GetListCongTyBaoHiemTuNhans(long tiepNhanId)
        {
            var thongTinCongTyBaoHiemTuNhans = await _yeucauTiepNhanCongTyBaoHiemTuNhanRepo.TableNoTracking.Include(x => x.CongTyBaoHiemTuNhan)
                .Where(p => p.YeuCauTiepNhanId == tiepNhanId)
                .ToListAsync();

            var query = thongTinCongTyBaoHiemTuNhans.Select(item => new CongTyBaoHiemTuNhanGridVo
            {
                Id = item.Id,
                CongNoId = item.CongTyBaoHiemTuNhanId,
                TenCongTy = item.CongTyBaoHiemTuNhan.Ten,
                SoThe = item.MaSoThe,
                DienThoai = item.SoDienThoai,
                DiaChi = item.DiaChi,
                DenNgay = item.NgayHetHan,
                TuNgay = item.NgayHieuLuc,
                TuNgayHienThi = item.NgayHieuLuc != null ? Convert.ToDateTime(item.NgayHieuLuc).ApplyFormatDate() : null,
                DenNgayHienThi = item.NgayHetHan != null ? Convert.ToDateTime(item.NgayHetHan).ApplyFormatDate() : null
            }).ToList();

            return query;
        }
        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocChoXuatThuocBHYT(long tiepNhanId)
        {
            var result = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                            o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT &&
                            o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)
                .SelectMany(o => o.DonThuocThanhToanChiTiets)
                .Select(x => new ThongTinDuocPhamQuayThuocVo()
                {
                    Id = x.Id,
                    DuocPhamId = x.DuocPham.Id,
                    MaHoatChat = x.DuocPham.HoatChat,
                    NongDoVaHamLuong = x.DuocPham.HamLuong,
                    TenDuocPham = x.DuocPham.Ten,
                    SoLuongTon = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongNhap - x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat,
                    NhapKhoDuocPhamChiTietId = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Id,
                    TenHoatChat = x.DuocPham.HoatChat,
                    DonViTinh = x.DuocPham.DonViTinh.Ten,
                    SoLuongToa = x.YeuCauKhamBenhDonThuocChiTiet != null ? x.YeuCauKhamBenhDonThuocChiTiet.SoLuong : 0,
                    SoLuongMua = x.SoLuong,
                    DonGiaNhap = x.DonGiaNhap,
                    DonGia = x.DonGiaBan,
                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                    VAT = x.VAT,
                    Solo = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Solo,
                    ViTri = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.KhoDuocPhamViTri.Ten,
                    HanSuDung = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung,
                    isNew = false,
                    LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                });
            var resultVatTu = _donVTYTThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                            //o.don == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                            o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)
                .SelectMany(o => o.DonVTYTThanhToanChiTiets)
                .Select(x => new ThongTinDuocPhamQuayThuocVo()
                {
                    Id = x.Id,
                    DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                    TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                    SoLuongTon = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongNhap - x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat,
                    NhapKhoDuocPhamChiTietId = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Id,
                    DonViTinh = x.VatTuBenhVien.VatTus.DonViTinh,
                    SoLuongMua = x.SoLuong,
                    Solo = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Solo,
                    ViTri = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTri.Ten,
                    HanSuDung = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HanSuDung,
                    isNew = false,
                    VatTuBHYT = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                    DonGiaNhap = x.DonGiaNhap,
                    DonGia = x.DonGiaBan,
                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                    VAT = x.VAT,
                    LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien
                }).Where(x => x.VatTuBHYT == true);  // true lấy vật tư k bảo hiểm
            return await result.Union(resultVatTu).ToListAsync();
        }

        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachChoXuatThuocKhachVangLai(long benhNhanId)
        {
            var result = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                .Where(o => o.BenhNhanId == benhNhanId &&
                            o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)
                .SelectMany(o => o.DonThuocThanhToanChiTiets)
                .Select(x => new ThongTinDuocPhamQuayThuocVo
                {
                    Id = x.Id,
                    DuocPhamId = x.DuocPham.Id,
                    MaHoatChat = x.DuocPham.HoatChat,
                    TenDuocPham = x.DuocPham.Ten,
                    SoLuongTon = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongNhap - x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat,
                    NhapKhoDuocPhamChiTietId = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Id,
                    TenHoatChat = x.DuocPham.HoatChat,
                    DonViTinh = x.DuocPham.DonViTinh.Ten,
                    SoLuongToa = x.YeuCauKhamBenhDonThuocChiTiet != null ? x.YeuCauKhamBenhDonThuocChiTiet.SoLuong : 0,
                    SoLuongMua = x.SoLuong,
                    DonGiaNhap = x.DonGiaNhap,
                    DonGia = x.DonGiaBan,
                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                    VAT = x.VAT,
                    Solo = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Solo,
                    ViTri = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.KhoDuocPhamViTri.Ten,
                    HanSuDung = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung,
                    //HanSuDungHientThi = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung.ApplyFormatDate(),
                    isNew = false,
                    BacSiKeToa = x.YeuCauKhamBenhDonThuocChiTiet != null ? x.YeuCauKhamBenhDonThuocChiTiet.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen : string.Empty
                });
            return await result.ToListAsync();
        }

        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocChoXuatThuocKhongBHYT(long tiepNhanId)
        {
            var result = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                            o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                            o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)
                .SelectMany(o => o.DonThuocThanhToanChiTiets)
                .Select(x => new ThongTinDuocPhamQuayThuocVo()
                {
                    Id = x.Id,
                    DuocPhamId = x.DuocPham.Id,
                    MaHoatChat = x.DuocPham.HoatChat,
                    NongDoVaHamLuong = x.DuocPham.HamLuong,
                    TenDuocPham = x.DuocPham.Ten,
                    SoLuongTon = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongNhap - x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat,
                    NhapKhoDuocPhamChiTietId = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Id,
                    TenHoatChat = x.DuocPham.HoatChat,
                    DonViTinh = x.DuocPham.DonViTinh.Ten,
                    SoLuongToa = x.YeuCauKhamBenhDonThuocChiTiet != null ? x.YeuCauKhamBenhDonThuocChiTiet.SoLuong : 0,
                    SoLuongMua = x.SoLuong,
                    DonGiaNhap = x.DonGiaNhap,
                    DonGia = x.DonGiaBan,
                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                    VAT = x.VAT,
                    Solo = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Solo,
                    ViTri = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.KhoDuocPhamViTri.Ten,
                    HanSuDung = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung,
                    isNew = false,
                    LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                });
            var resultVatTu = _donVTYTThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                            o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)
                .SelectMany(o => o.DonVTYTThanhToanChiTiets)
                .Select(x => new ThongTinDuocPhamQuayThuocVo()
                {
                    Id = x.Id,
                    DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                    TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                    SoLuongTon = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongNhap - x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat,
                    NhapKhoDuocPhamChiTietId = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Id,
                    DonViTinh = x.VatTuBenhVien.VatTus.DonViTinh,
                    SoLuongMua = x.SoLuong,
                    Solo = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Solo,
                    ViTri = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTri.Ten,
                    HanSuDung = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HanSuDung,
                    isNew = false,
                    VatTuBHYT = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                    DonGiaNhap = x.DonGiaNhap,
                    DonGia = x.DonGiaBan,
                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                    VAT = x.VAT,
                    LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.VatTuBenhVien
                }).Where(x => x.VatTuBHYT != true);  // true lấy vật tư k bảo hiểm
            return await result.Union(resultVatTu).ToListAsync();
        }
        #region List đơn thuốc trong ngày
        public GridDataSource GetDataForGridTimBenhNhanAsync(QueryInfo queryInfo, bool isPrint)
        {
            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> { new Sort { Field = "TrangThai", Dir = "asc" }, new Sort { Field = "Id", Dir = "asc" } };
            }
            if (queryInfo.AdditionalSearchString != null)
            {
                var queryString = JsonConvert.DeserializeObject<QuayThuocGridVo>(queryInfo.AdditionalSearchString);
                //todo: need improve
                if (queryString.KiemTraThanhToan == "1" || queryString.KiemTraThanhToan == "\"1\"") // 1 đang thanh toán
                {
                    var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                                   .Where(o => o.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                              (o.DonThuocThanhToans.Any(s => s.DonThuocThanhToanChiTiets.Count() != 0) &&
                                              o.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                                            ((x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan &&
                                                                            x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)))))
                                                                            ||
                                               (o.DonVTYTThanhToans.Any(s => s.DonVTYTThanhToanChiTiets.Count() != 0) &&
                                               o.DonVTYTThanhToans.Any(u => u.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan &&
                                                                          u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)))
                     .Select(s => new DonThuocThanhToanGridVo
                     {
                         Id = s.Id,
                         BenhNhanId = s.BenhNhanId,
                         MaBN = s.BenhNhan.MaBN,
                         MaTN = s.MaYeuCauTiepNhan,
                         YeuCauTiepNhanId = s.Id,
                         HoTen = s.HoTen,
                         NamSinh = s.NamSinh,
                         GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                         DiaChi = s.DiaChiDayDu,
                         SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                         DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                         TrangThai = TrangThaiThanhToan.ChuaThanhToan,
                         IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                         TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                               + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                         //update search 22/10/2020
                         CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                     });
                    if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                    {
                        querydata = querydata.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                    {

                        querydata = querydata.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                    {
                        querydata = querydata.Where(x => x.SoDienThoai == queryString.SoDienThoai.Trim().ApplyFormatPhone());
                    }
                    if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                    {
                        querydata = querydata.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay && o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = null

                        });
                    }
                    if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = null,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    var countTask = queryInfo.LazyLoadPage == true ? 0 : querydata.Count();

                    var queryTask = isPrint == true ? querydata.OrderBy(queryInfo.SortString).ToArray() : querydata.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
                else if (queryString.KiemTraThanhToan == "2" || queryString.KiemTraThanhToan == "\"2\"")// 2 đã thanh toán
                {
                    var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                                    .Where(o => o.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                                o.DonThuocThanhToans.Any(s => s.DonThuocThanhToanChiTiets.Count() != 0) &&
                                                o.DonThuocThanhToans.Any(x => (x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                               x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc &&
                                                                               (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT))) // || x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                                                ||
                                                o.DonVTYTThanhToans.Any(s => s.DonVTYTThanhToanChiTiets.Count() != 0) &&
                                                o.DonVTYTThanhToans.Any(u => u.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                          u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT))
                    .Select(s => new DonThuocThanhToanGridVo
                    {
                        Id = s.Id,
                        BenhNhanId = s.BenhNhanId,
                        MaTN = s.MaYeuCauTiepNhan,
                        MaBN = s.BenhNhan.MaBN,
                        HoTen = s.HoTen,
                        NamSinh = s.NamSinh,
                        YeuCauTiepNhanId = s.Id,
                        GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                        DiaChi = s.DiaChiDayDu,
                        SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                        DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                        TrangThai = TrangThaiThanhToan.DaThanhToan,
                        IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                        TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(x => x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                            + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT == true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                        CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                    });
                    if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                    {
                        querydata = querydata.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                    {

                        querydata = querydata.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                    {
                        querydata = querydata.Where(x => x.SoDienThoai == queryString.SoDienThoai.Trim().ApplyFormatPhone());
                    }
                    if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                    {
                        querydata = querydata.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay && o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = null

                        });
                    }
                    if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = null,
                            DateEnd = queryString.DateEnd

                        });
                    }

                    var countTask = queryInfo.LazyLoadPage == true ? 0 : querydata.Count();

                    var queryTask = isPrint == true ? querydata.OrderBy(queryInfo.SortString).ToArray() : querydata.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArray();

                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
                }
                else
                {

                    var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                              .Where(p => p.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                          p.DonVTYTThanhToans.Any(s => s.DonVTYTThanhToanChiTiets.Count() != 0) &&
                                          p.DonVTYTThanhToans.Any(x => (x.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT &&
                                                                        x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) ||
                                                                       (x.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT &&
                                                                        x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan
                                                                        )
                                                                        )
                                                     ||
                                    p.DonThuocThanhToans.Any(s => s.DonThuocThanhToanChiTiets.Count() != 0) &&
                                    p.DonThuocThanhToans.Any(x => (x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc &&
                                                                   x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan &&
                                                                   (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)) || //|| x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                                                                   (x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc &&
                                                                   x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                   (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT) //|| x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT
                                                                    )))
                  .Select(s => new DonThuocThanhToanGridVo
                  {
                      Id = s.Id,
                      BenhNhanId = s.BenhNhanId,
                      MaTN = s.MaYeuCauTiepNhan,
                      MaBN = s.BenhNhan.MaBN,
                      YeuCauTiepNhanId = s.Id,
                      HoTen = s.HoTen,
                      NamSinh = s.NamSinh,
                      GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                      DiaChi = s.DiaChiDayDu,
                      SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                      DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                      IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                      SoTienChoThanhToan = s.DonThuocThanhToans.Where(p => p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan)) + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                      TrangThai = (s.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                                 x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) ||
                                                                 (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                                     x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)) ||
                                   s.DonVTYTThanhToans.Any(l => l.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan ||
                                                               l.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)) ? TrangThaiThanhToan.ChuaThanhToan : TrangThaiThanhToan.DaThanhToan,
                      TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(p => (p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && p.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc) || (p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                           + s.DonVTYTThanhToans.Sum(xc => xc.DonVTYTThanhToanChiTiets.Sum(vt => vt.GiaBan)),
                      CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                  });
                    if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                    {
                        querydata = querydata.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                    {

                        querydata = querydata.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                    {
                        querydata = querydata.Where(x => x.SoDienThoai == queryString.SoDienThoai.Trim().ApplyFormatPhone());
                    }
                    if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                    {
                        querydata = querydata.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay && o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = null

                        });
                    }
                    if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = null,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    var countTask = queryInfo.LazyLoadPage == true ? 0 : querydata.Count();

                    var queryTask = isPrint == true ? querydata.OrderBy(queryInfo.SortString).ToArray() : querydata.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                        .Take(queryInfo.Take).ToArray();
                    return new GridDataSource { Data = queryTask, TotalRowCount = countTask };

                }
            }
            else
            {

                var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                              .Where(p => p.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                          p.DonVTYTThanhToans.Any(s => s.DonVTYTThanhToanChiTiets.Count() != 0) &&
                                          p.DonVTYTThanhToans.Any(x => (x.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT &&
                                                                        x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) ||
                                                                       (x.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT &&
                                                                        x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan
                                                                        )
                                                                        )
                                                     ||
                                    p.DonThuocThanhToans.Any(s => s.DonThuocThanhToanChiTiets.Count() != 0) &&
                                    p.DonThuocThanhToans.Any(x => (x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc &&
                                                                   x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan &&
                                                                   x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT) ||
                                                                   (x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc &&
                                                                   x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                   x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT
                                                                    )))
               .Select(s => new DonThuocThanhToanGridVo
               {
                   Id = s.Id,
                   BenhNhanId = s.BenhNhanId,
                   MaTN = s.MaYeuCauTiepNhan,
                   MaBN = s.BenhNhan.MaBN,
                   YeuCauTiepNhanId = s.Id,
                   HoTen = s.HoTen,
                   NamSinh = s.NamSinh,
                   GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                   DiaChi = s.DiaChiDayDu,
                   SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                   DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                   IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                   SoTienChoThanhToan = s.DonThuocThanhToans.Where(p => p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan)) + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                   TrangThai = (s.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                              x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) ||
                                                              (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                                  x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)) ||
                               s.DonVTYTThanhToans.Any(l => l.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan ||
                                                           l.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)) ? TrangThaiThanhToan.ChuaThanhToan : TrangThaiThanhToan.DaThanhToan,
                   TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(p => (p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && p.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc) || (p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                       + s.DonVTYTThanhToans.Sum(xc => xc.DonVTYTThanhToanChiTiets.Sum(vt => vt.GiaBan))
               });
                var countTask = queryInfo.LazyLoadPage == true ? 0 : querydata.Count();

                var queryTask = isPrint == true ? querydata.OrderBy(queryInfo.SortString).ToArray() : querydata.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }

        }

        public GridDataSource GetTotalPageForGridTimBenhNhanAsync(QueryInfo queryInfo)
        {
            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> { new Sort { Field = "TrangThai", Dir = "asc" }, new Sort { Field = "Id", Dir = "asc" } };
            }
            if (queryInfo.AdditionalSearchString != null)
            {
                var queryString = JsonConvert.DeserializeObject<QuayThuocGridVo>(queryInfo.AdditionalSearchString);
                if (queryString.KiemTraThanhToan == "1" || queryString.KiemTraThanhToan == "\"1\"") // 1 đang thanh toán
                {
                    var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                                   .Where(o => o.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                              (o.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                                            ((x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan &&
                                                                            x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)))))
                                                                            ||
                                               o.DonVTYTThanhToans.Any(u => u.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan &&
                                                                          u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT))
                     .Select(s => new DonThuocThanhToanGridVo
                     {
                         Id = s.Id,
                         BenhNhanId = s.BenhNhanId,
                         MaBN = s.BenhNhan.MaBN,
                         MaTN = s.MaYeuCauTiepNhan,
                         YeuCauTiepNhanId = s.Id,
                         HoTen = s.HoTen,
                         NamSinh = s.NamSinh,
                         GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                         DiaChi = s.DiaChiDayDu,
                         SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                         DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                         TrangThai = TrangThaiThanhToan.ChuaThanhToan,
                         IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                         TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                               + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                         //update search 22/10/2020
                         CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                     });
                    if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                    {
                        querydata = querydata.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                    {

                        querydata = querydata.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                    {
                        querydata = querydata.Where(x => x.SoDienThoai == queryString.SoDienThoai.ApplyFormatPhone().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                    {
                        querydata = querydata.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay && o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = null

                        });
                    }
                    if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = null,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    var countTask = querydata.Count();

                    return new GridDataSource { TotalRowCount = countTask };
                }
                else if (queryString.KiemTraThanhToan == "2" || queryString.KiemTraThanhToan == "\"2\"")// 2 đã thanh toán
                {
                    var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                                    .Where(o => o.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                                o.DonThuocThanhToans.Any(x => (x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                               x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc
                                                                               && x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT))
                                                ||
                                                o.DonVTYTThanhToans.Any(u => u.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                          u.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT))
                    .Select(s => new DonThuocThanhToanGridVo
                    {
                        Id = s.Id,
                        BenhNhanId = s.BenhNhanId,
                        MaTN = s.MaYeuCauTiepNhan,
                        MaBN = s.BenhNhan.MaBN,
                        HoTen = s.HoTen,
                        NamSinh = s.NamSinh,
                        YeuCauTiepNhanId = s.Id,
                        GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                        DiaChi = s.DiaChiDayDu,
                        SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                        DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                        TrangThai = TrangThaiThanhToan.DaThanhToan,
                        IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                        TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(x => x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                            + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT == true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                        CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                    });
                    if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                    {
                        querydata = querydata.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                    {

                        querydata = querydata.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                    {
                        querydata = querydata.Where(x => x.SoDienThoai == queryString.SoDienThoai.ApplyFormatPhone().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                    {
                        querydata = querydata.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay && o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = null

                        });
                    }
                    if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = null,
                            DateEnd = queryString.DateEnd

                        });
                    }

                    var countTask = querydata.Count();

                    return new GridDataSource { TotalRowCount = countTask };
                }
                else
                {

                    var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                                   .Where(p => p.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                               p.DonVTYTThanhToans.Any(x => (x.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT &&
                                                                             x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) ||
                                                                            (x.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT &&
                                                                             x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan
                                                                             ))
                                                          ||
                                         p.DonThuocThanhToans.Any(x => (x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc &&
                                                                        x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan &&
                                                                        x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT) ||
                                                                        (x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc &&
                                                                        x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                        x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)
                                                                         ))
                   .Select(s => new DonThuocThanhToanGridVo
                   {
                       Id = s.Id,
                       BenhNhanId = s.BenhNhanId,
                       MaTN = s.MaYeuCauTiepNhan,
                       MaBN = s.BenhNhan.MaBN,
                       YeuCauTiepNhanId = s.Id,
                       HoTen = s.HoTen,
                       NamSinh = s.NamSinh,
                       GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                       DiaChi = s.DiaChiDayDu,
                       SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                       DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                       IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                       SoTienChoThanhToan = s.DonThuocThanhToans.Where(p => p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan)) + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                       TrangThai = (s.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                                  x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) ||
                                                                  (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                                      x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)) ||
                                   s.DonVTYTThanhToans.Any(l => l.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan ||
                                                               l.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)) ? TrangThaiThanhToan.ChuaThanhToan : TrangThaiThanhToan.DaThanhToan,
                       TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(p => (p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && p.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc) || (p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                           + s.DonVTYTThanhToans.Sum(xc => xc.DonVTYTThanhToanChiTiets.Sum(vt => vt.GiaBan)),
                       CreatedOn = s.DonThuocThanhToans.OrderByDescending(d => d.CreatedOn).Select(x => Convert.ToDateTime(x.CreatedOn)).Concat(s.DonVTYTThanhToans.OrderByDescending(c => c.CreatedOn).Select(z => Convert.ToDateTime(z.CreatedOn))).ToList(),
                   });
                    if (!string.IsNullOrEmpty(queryString.MaBenhNhan?.Trim()))
                    {
                        querydata = querydata.Where(x => x.MaBN == queryString.MaBenhNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.MaTiepNhan?.Trim()))
                    {

                        querydata = querydata.Where(x => x.MaTN == queryString.MaTiepNhan.Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.SoDienThoai?.Trim()))
                    {
                        querydata = querydata.Where(x => x.SoDienThoai == queryString.SoDienThoai.ApplyFormatPhone().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.HoTen?.Trim()))
                    {
                        querydata = querydata.Where(o => o.HoTen.ToLower() == queryString.HoTen.ToLower().Trim());
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay && o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o >= tuNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = queryString.DateStart,
                            DateEnd = null

                        });
                    }
                    if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                    {
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        querydata = querydata.Where(p => p.CreatedOn.Any(o => o <= denNgay));
                        querydata = querydata.Select(s => new DonThuocThanhToanGridVo
                        {
                            Id = s.Id,
                            BenhNhanId = s.BenhNhanId,
                            MaBN = s.MaBN,
                            MaTN = s.MaTN,
                            YeuCauTiepNhanId = s.Id,
                            HoTen = s.HoTen,
                            NamSinh = s.NamSinh,
                            GioiTinhHienThi = s.GioiTinhHienThi,
                            DiaChi = s.DiaChi,
                            SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                            DoiTuong = s.DoiTuong,
                            TrangThai = s.TrangThai,
                            IsDisable = s.IsDisable,
                            TongGiaTriDonThuoc = s.TongGiaTriDonThuoc,
                            //update search 22/10/2020
                            CreatedOn = s.CreatedOn,
                            DateStart = null,
                            DateEnd = queryString.DateEnd

                        });
                    }
                    var countTask = querydata.Count();

                    return new GridDataSource { TotalRowCount = countTask };

                }
            }
            else
            {

                var querydata = _yeuCauTiepNhanRepo.TableNoTracking
                               .Where(p => p.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                                           p.DonVTYTThanhToans.Any(x => (x.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT &&
                                                                         x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) ||
                                                                        (x.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT &&
                                                                         x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan
                                                                         ))
                                                      ||
                                     p.DonThuocThanhToans.Any(x => (x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc &&
                                                                    x.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan &&
                                                                    x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT) ||
                                                                    (x.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc &&
                                                                    x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan &&
                                                                    x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)
                                                                     ))
               .Select(s => new DonThuocThanhToanGridVo
               {
                   Id = s.Id,
                   BenhNhanId = s.BenhNhanId,
                   MaTN = s.MaYeuCauTiepNhan,
                   MaBN = s.BenhNhan.MaBN,
                   YeuCauTiepNhanId = s.Id,
                   HoTen = s.HoTen,
                   NamSinh = s.NamSinh,
                   GioiTinhHienThi = s.BenhNhan.GioiTinh.GetDescription(),
                   DiaChi = s.DiaChiDayDu,
                   SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                   DoiTuong = s.CoBHYT == true ? "BHYT (" + s.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                   IsDisable = (s.DonThuocThanhToans.Where(x => x.YeuCauKhamBenhDonThuocId != null).Any() || s.DonVTYTThanhToans.Where(x => x.YeuCauKhamBenhDonVTYTId != null).Any()) ? false : true,
                   SoTienChoThanhToan = s.DonThuocThanhToans.Where(p => p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan)) + s.DonVTYTThanhToans.Where(w => (w.DonVTYTThanhToanChiTiets.Where(v => v.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true).Any() && w.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan && w.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT)).Sum(v => v.DonVTYTThanhToanChiTiets.Sum(g => g.GiaBan)),
                   TrangThai = (s.DonThuocThanhToans.Any(x => (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                              x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan) ||
                                                              (x.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT &&
                                                                  x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)) ||
                               s.DonVTYTThanhToans.Any(l => l.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan ||
                                                           l.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)) ? TrangThaiThanhToan.ChuaThanhToan : TrangThaiThanhToan.DaThanhToan,
                   TongGiaTriDonThuoc = s.DonThuocThanhToans.Where(p => (p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && p.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc) || (p.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && p.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc)).Sum(o => o.DonThuocThanhToanChiTiets.Sum(g => g.GiaBan))
                                       + s.DonVTYTThanhToans.Sum(xc => xc.DonVTYTThanhToanChiTiets.Sum(vt => vt.GiaBan))
               });
                var countTask = querydata.Count();

                return new GridDataSource { TotalRowCount = countTask };
            }

        }
        public async Task<GridDataSource> GetDanhSachThuocBenhNhanChild(QueryInfo queryInfo)
        {
            var querystring = queryInfo.AdditionalSearchString.Split('-');
            if (querystring[0] != "null")
            {
                var test = _donThuocThanhToanRepo.TableNoTracking
                   .Where(o => o.YeuCauTiepNhanId == long.Parse(querystring[0])).ToList();
                var resultDP = _donThuocThanhToanRepo.TableNoTracking
                   .Where(o => o.YeuCauTiepNhanId == long.Parse(querystring[0]) &&
                               o.TrangThai != TrangThaiDonThuocThanhToan.DaHuy &&
                               o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan &&
                               o.DonThuocThanhToanChiTiets.Any())
                   .Select(x => new DonThuocCuaBenhNhanGridVo()
                   {
                       Id = x.Id,
                       MaDon = x.Id.ToString(),
                       NgayKeDonDate = Convert.ToDateTime(x.CreatedOn),
                       NgayKeDon = x.CreatedOn.Value.ApplyFormatDateTimeSACH(),
                       DVKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "",
                       BSKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.BacSiKetLuan.User.HoTen : "",
                       SoTien = x.DonThuocThanhToanChiTiets.Sum(g => Convert.ToDouble(g.GiaBan)),
                       SoTienDisPlay = x.DonThuocThanhToanChiTiets.Sum(g => Convert.ToDouble(g.GiaBan)).ApplyFormatMoneyVNDToDouble(),
                       TTThanhToan = (int)x.TrangThaiThanhToan,
                       TTXuatThuoc = (int)x.TrangThai,
                       TinhTrang = "",
                       LoaiDonThuoc = x.LoaiDonThuoc.GetDescription(),
                       DonThuocBacSiKeToa = x.YeuCauKhamBenhDonThuocId != null ? false : true,
                       HighLightClass = x.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT ? "bg-row-lightblue" : "bg-row-lightPeru",
                       DonThuocThanhToanId = x.Id,
                       YeuCauKhambenhId = x.YeuCauKhamBenhId,
                   });
                var resultVT = _donVTYTThanhToanRepo.TableNoTracking
                      .Where(o => o.YeuCauTiepNhanId == long.Parse(querystring[0]) &&
                                  o.TrangThai != TrangThaiDonVTYTThanhToan.DaHuy &&
                                  o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan)
                      .Select(x => new DonThuocCuaBenhNhanGridVo()
                      {
                          Id = x.Id,
                          MaDon = x.Id.ToString(),
                          NgayKeDonDate = Convert.ToDateTime(x.CreatedOn),
                          NgayKeDon = x.CreatedOn.Value.ApplyFormatDateTimeSACH(),
                          DVKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "",
                          BSKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.BacSiKetLuan.User.HoTen : "",
                          SoTien = x.DonVTYTThanhToanChiTiets.Sum(g => Convert.ToDouble(g.GiaBan)),
                          SoTienDisPlay = x.DonVTYTThanhToanChiTiets.Sum(g => Convert.ToDouble(g.GiaBan)).ApplyFormatMoneyVNDToDouble(),
                          TTThanhToan = (int)x.TrangThaiThanhToan,
                          TTXuatThuoc = (int)x.TrangThai,
                          TinhTrang = "",
                          YeuCauKhambenhId = x.YeuCauKhamBenhId,
                          //LoaiDonThuoc = x.nhap(),
                          DonThuocBacSiKeToa = x.YeuCauKhamBenhDonVTYTId != null ? false : true,
                          HighLightClass = "bg-row-lightblue", // lun lun la vat tu k y te
                          DonThuocThanhToanId = x.Id,
                      });
                var result = resultDP.Union(resultVT);
                if (querystring.Length > 1)
                {
                    if (querystring[1] != "null" && querystring[2] != "null")
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(querystring[1], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        DateTime.TryParseExact(querystring[2], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        result = result.Where(p => p.NgayKeDonDate >= tuNgay && p.NgayKeDonDate <= denNgay);
                    }
                    if ((querystring[1] != "null" && querystring[2] == "null") || (querystring[1] != null && querystring[2] == null) || (querystring[1] != "" && querystring[2] == ""))
                    {
                        DateTime TuNgayPart = DateTime.Now;
                        DateTime.TryParseExact(querystring[1], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                        var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                        result = result.Where(p => p.NgayKeDonDate >= tuNgay);
                    }

                    if ((querystring[1] == "null" && querystring[2] != "null") || (querystring[1] == null && querystring[2] != null) || (querystring[1] == "" && querystring[2] != ""))
                    {
                        DateTime DenNgaysPart = DateTime.Now;
                        DateTime.TryParseExact(querystring[2], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                        var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                        result = result.Where(p => p.NgayKeDonDate <= denNgay);
                    }
                }
                var stt = 1;
                List<DonThuocCuaBenhNhanGridVo> list = new List<DonThuocCuaBenhNhanGridVo>();
                foreach (var itemx in result)
                {
                    if (itemx.DVKham == null || itemx.DVKham == "")
                    {
                        itemx.DVKham = "Mua thêm";
                    }
                    if (itemx.BSKham == null || itemx.BSKham == "")
                    {
                        itemx.BSKham = "Nhà thuốc";
                    }
                    if (itemx.TTThanhToan == 1 && itemx.TTXuatThuoc == 1)
                    {
                        itemx.TinhTrang = Enums.TrangThaiThanhToan.ChuaThanhToan.GetDescription();
                    }
                    if (itemx.TTThanhToan == 2 && itemx.TTXuatThuoc == 1)
                    {
                        itemx.TinhTrang = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc.GetDescription();
                    }
                    if (itemx.TTThanhToan == 2 && itemx.TTXuatThuoc == 2)
                    {
                        itemx.TinhTrang = Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc.GetDescription();
                    }
                    itemx.STT = stt++;
                    list.Add(itemx);
                }

                var dataOrderBy = list.AsQueryable().OrderBy(queryInfo.SortString);
                var quaythuoc = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };

            }
            else
            {
                var result = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
              .Where(o => o.YeuCauTiepNhanId == long.Parse(querystring[0]) && o.TrangThai != TrangThaiDonThuocThanhToan.DaHuy && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan)
              .Select(x => new DonThuocCuaBenhNhanGridVo()
              {
                  Id = x.Id,
                  MaDon = x.Id.ToString(),
                  NgayKeDonDate = Convert.ToDateTime(x.CreatedOn),
                  NgayKeDon = x.CreatedOn.Value.ApplyFormatDateTimeSACH(),
                  DVKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "",
                  BSKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.BacSiKetLuan.User.HoTen : "",
                  //TODO: Nam fix
                  //SoTien = x.DonThuocThanhToanChiTiets.Sum(op => Convert.ToDouble(op.SoLuong) * Convert.ToDouble(op.Gia)),
                  //SoTienDisPlay = x.DonThuocThanhToanChiTiets.Sum(op => Convert.ToDouble(op.SoLuong) * Convert.ToDouble(op.Gia)).ApplyFormatMoneyVNDToDouble(),
                  TTThanhToan = (int)x.TrangThaiThanhToan,
                  TTXuatThuoc = (int)x.TrangThai,
                  TinhTrang = "",
                  LoaiDonThuoc = x.LoaiDonThuoc.GetDescription(),
                  DonThuocBacSiKeToa = x.YeuCauKhamBenhId != null ? false : true,
                  HighLightClass = x.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT ? "bg-row-lightblue" : "bg-row-lightPeru",
                  DonThuocThanhToanId = x.Id,
                  YeuCauKhambenhId = x.YeuCauKhamBenhId,
              });
                var stt = 1;
                List<DonThuocCuaBenhNhanGridVo> list = new List<DonThuocCuaBenhNhanGridVo>();
                foreach (var itemx in result)
                {
                    if (itemx.DVKham == null || itemx.DVKham == "")
                    {
                        itemx.DVKham = "Mua thêm";
                    }
                    if (itemx.BSKham == null || itemx.BSKham == "")
                    {
                        itemx.BSKham = "Nhà thuốc";
                    }
                    if (itemx.TTThanhToan == 1 && itemx.TTXuatThuoc == 1)
                    {
                        itemx.TinhTrang = Enums.TrangThaiThanhToan.ChuaThanhToan.GetDescription();
                    }
                    if (itemx.TTThanhToan == 2 && itemx.TTXuatThuoc == 1)
                    {
                        itemx.TinhTrang = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc.GetDescription();
                    }
                    if (itemx.TTThanhToan == 2 && itemx.TTXuatThuoc == 2)
                    {
                        itemx.TinhTrang = Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc.GetDescription();
                    }
                    itemx.STT = stt++;
                    list.Add(itemx);
                }
                var dataOrderBy = list.AsQueryable().OrderBy(queryInfo.SortString);
                var quaythuoc = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };

            }
            return null;
        }
        #endregion
        #region toa thuốc người bệnh list child đơn thuốc trong ngày
        public async Task<GridDataSource> GetDanhSachThuocBenhNhan(QueryInfo queryInfo)
        {
            var querystring = queryInfo.AdditionalSearchString.Split('-');
            if (querystring[0] != "null" && querystring[1] != "null" && querystring[2] != "null")
            {
                var resultDP = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
               .Where(o => o.YeuCauTiepNhanId == long.Parse(querystring[0]) && o.TrangThai != TrangThaiDonThuocThanhToan.DaHuy && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan)
               .Select(x => new DonThuocCuaBenhNhanGridVo()
               {
                   Id = x.Id,
                   MaDon = x.Id.ToString(),
                   NgayKeDonDate = Convert.ToDateTime(x.CreatedOn),
                   NgayKeDon = x.CreatedOn.Value.ApplyFormatDateTimeSACH(),
                   DVKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "",
                   BSKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.BacSiKetLuan.User.HoTen : "",
                   //TODO: Nam fix
                   //SoTien = x.DonThuocThanhToanChiTiets.Sum(op => Convert.ToDouble(op.SoLuong) * Convert.ToDouble(op.Gia)),
                   //SoTienDisPlay = x.DonThuocThanhToanChiTiets.Sum(op => Convert.ToDouble(op.SoLuong) * Convert.ToDouble(op.Gia)).ApplyFormatMoneyVNDToDouble(),
                   TTThanhToan = (int)x.TrangThaiThanhToan,
                   TTXuatThuoc = (int)x.TrangThai,
                   TinhTrang = "",
                   LoaiDonThuoc = x.LoaiDonThuoc.GetDescription(),
                   DonThuocBacSiKeToa = x.YeuCauKhamBenhDonThuocId != null ? false : true,
                   HighLightClass = x.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT ? "bg-row-lightblue" : "bg-row-lightPeru",
                   DonThuocThanhToanId = x.Id,
               });
                var resultVT = _donVTYTThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
              .Where(o => o.YeuCauTiepNhanId == long.Parse(querystring[0]) && o.TrangThai != TrangThaiDonVTYTThanhToan.DaHuy && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan)
              .Select(x => new DonThuocCuaBenhNhanGridVo()
              {
                  Id = x.Id,
                  MaDon = x.Id.ToString(),
                  NgayKeDonDate = Convert.ToDateTime(x.CreatedOn),
                  NgayKeDon = x.CreatedOn.Value.ApplyFormatDateTimeSACH(),
                  DVKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "",
                  BSKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.BacSiKetLuan.User.HoTen : "",
                  //TODO: Nam fix
                  //SoTien = x.DonThuocThanhToanChiTiets.Sum(op => Convert.ToDouble(op.SoLuong) * Convert.ToDouble(op.Gia)),
                  //SoTienDisPlay = x.DonThuocThanhToanChiTiets.Sum(op => Convert.ToDouble(op.SoLuong) * Convert.ToDouble(op.Gia)).ApplyFormatMoneyVNDToDouble(),
                  TTThanhToan = (int)x.TrangThaiThanhToan,
                  TTXuatThuoc = (int)x.TrangThai,
                  TinhTrang = "",
                  LoaiDonThuoc = "Thuốc Không BHYT",
                  DonThuocBacSiKeToa = x.YeuCauKhamBenhDonVTYTId != null ? false : true,
                  HighLightClass = "bg-row-lightblue",
                  DonThuocThanhToanId = x.Id,
              });
                var result = resultDP.Union(resultVT);
                if (querystring[1] != null && querystring[2] != null)
                {
                    DateTime TuNgayPart = DateTime.Now;
                    DateTime DenNgaysPart = DateTime.Now;
                    DateTime.TryParseExact(querystring[1], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                    DateTime.TryParseExact(querystring[2], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                    var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                    var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayKeDonDate >= tuNgay && p.NgayKeDonDate <= denNgay);
                }
                if (querystring[1] != null && querystring[2] == null)
                {
                    DateTime TuNgayPart = DateTime.Now;
                    DateTime.TryParseExact(querystring[1], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                    var tuNgay = new DateTime(TuNgayPart.Year, TuNgayPart.Month, TuNgayPart.Day);
                    result = result.Where(p => p.NgayKeDonDate >= tuNgay);
                }
                if (querystring[1] == null && querystring[2] != null)
                {
                    DateTime DenNgaysPart = DateTime.Now;
                    DateTime.TryParseExact(querystring[2], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
                    var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                    result = result.Where(p => p.NgayKeDonDate <= denNgay);
                }
                var stt = 1;
                List<DonThuocCuaBenhNhanGridVo> list = new List<DonThuocCuaBenhNhanGridVo>();
                foreach (var itemx in result)
                {
                    if (itemx.DVKham == null || itemx.DVKham == "")
                    {
                        itemx.DVKham = "Mua thêm";
                    }
                    if (itemx.BSKham == null || itemx.BSKham == "")
                    {
                        itemx.BSKham = "Nhà thuốc";
                    }
                    if (itemx.TTThanhToan == 1 && itemx.TTXuatThuoc == 1)
                    {
                        itemx.TinhTrang = Enums.TrangThaiThanhToan.ChuaThanhToan.GetDescription();
                    }
                    if (itemx.TTThanhToan == 2 && itemx.TTXuatThuoc == 1)
                    {
                        itemx.TinhTrang = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc.GetDescription();
                    }
                    if (itemx.TTThanhToan == 2 && itemx.TTXuatThuoc == 2)
                    {
                        itemx.TinhTrang = Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc.GetDescription();
                    }
                    itemx.STT = stt++;
                    list.Add(itemx);
                }

                var dataOrderBy = list.AsQueryable().OrderBy(queryInfo.SortString);
                var quaythuoc = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };

            }
            else
            {
                var result = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
              .Where(o => o.YeuCauTiepNhanId == long.Parse(querystring[0]) && o.TrangThai != TrangThaiDonThuocThanhToan.DaHuy && o.TrangThaiThanhToan != TrangThaiThanhToan.HuyThanhToan)
              .Select(x => new DonThuocCuaBenhNhanGridVo()
              {
                  Id = x.Id,
                  MaDon = x.Id.ToString(),
                  NgayKeDonDate = Convert.ToDateTime(x.CreatedOn),
                  NgayKeDon = x.CreatedOn.Value.ApplyFormatDateTimeSACH(),
                  DVKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten : "",
                  BSKham = x.YeuCauKhamBenhId != null ? x.YeuCauKhamBenh.BacSiKetLuan.User.HoTen : "",
                  //TODO: Nam fix
                  //SoTien = x.DonThuocThanhToanChiTiets.Sum(op => Convert.ToDouble(op.SoLuong) * Convert.ToDouble(op.Gia)),
                  //SoTienDisPlay = x.DonThuocThanhToanChiTiets.Sum(op => Convert.ToDouble(op.SoLuong) * Convert.ToDouble(op.Gia)).ApplyFormatMoneyVNDToDouble(),
                  TTThanhToan = (int)x.TrangThaiThanhToan,
                  TTXuatThuoc = (int)x.TrangThai,
                  TinhTrang = "",
                  LoaiDonThuoc = x.LoaiDonThuoc.GetDescription(),
                  DonThuocBacSiKeToa = x.YeuCauKhamBenhId != null ? false : true,
                  HighLightClass = x.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT ? "bg-row-lightblue" : "bg-row-lightPeru",
                  DonThuocThanhToanId = x.Id,
              });
                var stt = 1;
                List<DonThuocCuaBenhNhanGridVo> list = new List<DonThuocCuaBenhNhanGridVo>();
                foreach (var itemx in result)
                {
                    if (itemx.DVKham == null || itemx.DVKham == "")
                    {
                        itemx.DVKham = "Mua thêm";
                    }
                    if (itemx.BSKham == null || itemx.BSKham == "")
                    {
                        itemx.BSKham = "Nhà thuốc";
                    }
                    if (itemx.TTThanhToan == 1 && itemx.TTXuatThuoc == 1)
                    {
                        itemx.TinhTrang = Enums.TrangThaiThanhToan.ChuaThanhToan.GetDescription();
                    }
                    if (itemx.TTThanhToan == 2 && itemx.TTXuatThuoc == 1)
                    {
                        itemx.TinhTrang = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc.GetDescription();
                    }
                    if (itemx.TTThanhToan == 2 && itemx.TTXuatThuoc == 2)
                    {
                        itemx.TinhTrang = Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc.GetDescription();
                    }
                    itemx.STT = stt++;
                    list.Add(itemx);
                }
                var dataOrderBy = list.AsQueryable().OrderBy(queryInfo.SortString);
                var quaythuoc = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                var countTask = dataOrderBy.Count();

                return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };

            }
            return null;
        }
        private string TinhTrangDonThuocBenhNhan(int TTThanhToan, int TTXuatThuoc)
        {
            if (TTThanhToan == 1 && TTXuatThuoc == 1)
            {
                return Enums.TrangThaiThanhToan.ChuaThanhToan.GetDescription();
            }
            if (TTThanhToan == 2 && TTXuatThuoc == 1)
            {
                return Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc.GetDescription();
            }
            if (TTThanhToan == 2 && TTXuatThuoc == 2)
            {
                return Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc.GetDescription();
            }
            if (TTThanhToan == null && TTXuatThuoc == null)
            {
                return Enums.TrangThaiThanhToan.ChuaThanhToan.GetDescription();
            }
            return null;
        }

        public async Task<string> InPhieuThuTienThuoc(XacNhanInThuocBenhNhan xacNhanIn)
        {
            var infoBN = ThongTinBenhNhanPhieuThuoc(xacNhanIn.TiepNhanId, xacNhanIn.YeuCauKhambenhId);

            var templateDonThuocBHYT = infoBN.LaTreEm == true ? _templateRepo.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYTTreEm")).First() : _templateRepo.TableNoTracking.Where(x => x.Name.Equals("DonThuocBHYT")).First();
            var templateDonThuocTrongBenhVien = infoBN.LaTreEm == true ? _templateRepo.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYTTreEm")).First() : _templateRepo.TableNoTracking.Where(x => x.Name.Equals("DonThuocKhongBHYT")).First();
            var templateDonThuocThucPhamChucNang = _templateRepo.TableNoTracking.Where(x => x.Name.Equals("DonThuocThucPhamChucNang")).FirstOrDefault();
            var templateDonThuocNgoaiBenhVien = infoBN.LaTreEm == true ? _templateRepo.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBVTreEm")).First() : _templateRepo.TableNoTracking.Where(x => x.Name.Equals("DonThuocNgoaiBV")).First();
            var duocPhamBenhVienPhanNhoms = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToList();

            //var listDonThuocBHYT = inToaThuoc.ListGridThuoc.Where(p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT);
            //var listDonThuocKhongBHYT = inToaThuoc.ListGridThuoc.Where(p => p.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).ToList();
            //Dictionary<long?, int> dictionaryBHYT = new Dictionary<long?, int>();
            //Dictionary<long?, int> dictionaryKhongBHYT = new Dictionary<long?, int>();
            ////sort Grid hiện tại theo Grid truyền vào
            //dictionaryBHYT = listDonThuocBHYT
            //    .Select((id, index) => new
            //    {
            //        key = (long?)id.Id,
            //        rank = index
            //    }).ToDictionary(o => o.key, o => o.rank);
            //dictionaryKhongBHYT = listDonThuocKhongBHYT
            //    .Select((id, index) => new
            //    {
            //        key = (long?)id.Id,
            //        rank = index
            //    }).ToDictionary(o => o.key, o => o.rank);

            //Thuốc trong BV
            var donThuocBHYTChiTiets = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == xacNhanIn.YeuCauKhambenhId && z.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                .Select(s => new InNoiTruDonThuocChiTietVo
                                {
                                    STT = s.SoThuTu,
                                    Id = s.Id,
                                    Ten = s.Ten,
                                    MaHoatChat = s.MaHoatChat,
                                    HoatChat = s.HoatChat,
                                    HamLuong = s.HamLuong,
                                    TenDuongDung = s.DuongDung.Ten,
                                    DVT = s.DonViTinh.Ten,
                                    SoLuong = s.SoLuong,
                                    SoNgayDung = s.SoNgayDung,
                                    ThoiGianDungSang = s.ThoiGianDungSang,
                                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                                    ThoiGianDungToi = s.ThoiGianDungToi,
                                    // thuốc gây nghiện,hướng thần thì cách dùng con số chuyển thành text , ngược lại nếu thuống thường kiểm tra sl kê nhỏ hơn 10 thì thêm 0 phía trước , còn lại bình thường
                                    DungSang = s.DungSang,
                                    DungTrua = s.DungTrua,
                                    DungChieu = s.DungChieu,
                                    DungToi = s.DungToi,
                                    ThoiDiemKeDon = s.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.YeuCauKhamBenhDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.YeuCauKhamBenhDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                    //}).OrderBy(p => dictionaryBHYT.Any(a => a.Key == p.Id) ? dictionaryBHYT[p.Id] : dictionaryBHYT.Count).ToList();
                                }).OrderBy(p => p.STT ?? 0).ToList();


            //var donThuocBHYTsDoc = donThuocBHYTChiTiets.Where(z =>
            //                                                                                      z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && z.LaDuocPhamBenhVien
            //                                                                                   && z.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsTiem = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem
            //                                                             && z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsUong = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong
            //                                                             && z.LaDuocPhamBenhVien
            //                                                              && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsDat = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat
            //                                                            && z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocBHYTsDungNgoai = donThuocBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai
            //                                                                  && z.LaDuocPhamBenhVien
            //                                                                   && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();

            //var donThuocBHYTsKhac = donThuocBHYTChiTiets.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && z.LaDuocPhamBenhVien
            //                                                                                 && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc
            //                                                                                 ).ToList();
            //var donThuocBHYTs = donThuocBHYTsDoc
            //               .Concat(donThuocBHYTsTiem)
            //               .Concat(donThuocBHYTsUong)
            //               .Concat(donThuocBHYTsDat)
            //               .Concat(donThuocBHYTsDungNgoai)
            //               .Concat(donThuocBHYTsKhac);
            var donThuocBHYTs = donThuocBHYTChiTiets;
            var donThuocKhongBHYTChiTiets = _yeuCauKhamBenhDonThuocChiTietRepository.TableNoTracking
                                .Where(z => z.YeuCauKhamBenhDonThuoc.YeuCauKhamBenhId == xacNhanIn.YeuCauKhambenhId && z.YeuCauKhamBenhDonThuoc.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT)
                                .Select(s => new InNoiTruDonThuocChiTietVo
                                {
                                    STT = s.SoThuTu,
                                    Id = s.Id,
                                    Ten = s.Ten,
                                    MaHoatChat = s.MaHoatChat,
                                    HoatChat = s.HoatChat,
                                    HamLuong = s.HamLuong,
                                    TenDuongDung = s.DuongDung.Ten,
                                    DVT = s.DonViTinh.Ten,
                                    SoLuong = s.SoLuong,
                                    SoNgayDung = s.SoNgayDung,
                                    ThoiGianDungSang = s.ThoiGianDungSang,
                                    ThoiGianDungTrua = s.ThoiGianDungTrua,
                                    ThoiGianDungChieu = s.ThoiGianDungChieu,
                                    ThoiGianDungToi = s.ThoiGianDungToi,
                                    DungSang = s.DungSang,
                                    DungTrua = s.DungTrua,
                                    DungChieu = s.DungChieu,
                                    DungToi = s.DungToi,
                                    ThoiDiemKeDon = s.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon,
                                    GhiChu = s.YeuCauKhamBenhDonThuoc.GhiChu,
                                    CachDung = s.GhiChu,
                                    LaDuocPhamBenhVien = s.LaDuocPhamBenhVien,
                                    TenBacSiKeDon = s.YeuCauKhamBenhDonThuoc.BacSiKeDon.User.HoTen,
                                    BacSiKeDonId = s.YeuCauKhamBenhDonThuoc.BacSiKeDonId,
                                    LoaiThuocTheoQuanLy = s.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy,
                                    DuocPhamBenhVienPhanNhomId = s.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                    DuongDungId = s.DuongDungId
                                }).ToList();
            foreach (var thuoc in donThuocKhongBHYTChiTiets)
            {
                thuoc.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(thuoc.DuocPhamBenhVienPhanNhomId.GetValueOrDefault(), duocPhamBenhVienPhanNhoms);
            }

            var userCurrentId = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.First().BacSiKeDonId : (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.First().BacSiKeDonId : 0);

            var tenBacSiKeDon = _userRepository.TableNoTracking
                             .Where(u => u.Id == userCurrentId).Select(u =>
                             (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                           + u.HoTen).FirstOrDefault();

            //var donThuocTrongBVKhongBHYTsDoc = donThuocKhongBHYTChiTiets.Where(z =>
            //                                                                                     z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                  && z.LaDuocPhamBenhVien
            //                                                                                  && z.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsTiem = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem
            //                                                              && z.LaDuocPhamBenhVien
            //                                                              && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsUong = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong
            //                                                             && z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsDat = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat
            //                                                            && z.LaDuocPhamBenhVien
            //                                                            && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocTrongBVKhongBHYTsDungNgoai = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai
            //                                                                  && z.LaDuocPhamBenhVien
            //                                                                  && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();

            //var donThuocTrongBVKhongBHYTsKhac = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && z.LaDuocPhamBenhVien
            //                                                                                 && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc
            //                                                                                 ).ToList();
            //var donThuocTrongBVs = donThuocTrongBVKhongBHYTsDoc
            //               .Concat(donThuocTrongBVKhongBHYTsTiem)
            //               .Concat(donThuocTrongBVKhongBHYTsUong)
            //               .Concat(donThuocTrongBVKhongBHYTsDat)
            //               .Concat(donThuocTrongBVKhongBHYTsDungNgoai)
            //               .Concat(donThuocTrongBVKhongBHYTsKhac);
            var donThuocTrongBVs = donThuocKhongBHYTChiTiets.Where(z => z.LaDuocPhamBenhVien).OrderBy(z => z.STT).ToList();

            //var donThuocNgoaiBVsDoc = donThuocKhongBHYTChiTiets.Where(z =>
            //                                                                                     z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                  && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                  && !z.LaDuocPhamBenhVien
            //                                                                                  && z.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsTiem = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Tiem
            //                                                              && !z.LaDuocPhamBenhVien
            //                                                              && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsUong = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Uong
            //                                                             && !z.LaDuocPhamBenhVien
            //                                                             && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsDat = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.Dat
            //                                                            && !z.LaDuocPhamBenhVien
            //                                                            && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();
            //var donThuocNgoaiBVsDungNgoai = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId == Constants.DuongDungIdSapXep.DungNgoai
            //                                                                  && !z.LaDuocPhamBenhVien
            //                                                                  && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc).ToList();

            //var donThuocNgoaiBVsKhac = donThuocKhongBHYTChiTiets.Where(z => z.DuongDungId != Constants.DuongDungIdSapXep.Tiem
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Uong
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.Dat
            //                                                                                   && z.DuongDungId != Constants.DuongDungIdSapXep.DungNgoai
            //                                                                                   && !z.LaDuocPhamBenhVien
            //                                                                                 && z.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.ThuocDoc
            //                                                                                 ).ToList();
            //var donThuocNgoaiBVs = donThuocNgoaiBVsDoc
            //               .Concat(donThuocNgoaiBVsTiem)
            //               .Concat(donThuocNgoaiBVsUong)
            //               .Concat(donThuocNgoaiBVsDat)
            //               .Concat(donThuocNgoaiBVsDungNgoai)
            //               .Concat(donThuocNgoaiBVsKhac);

            var donThuocNgoaiBVs = donThuocKhongBHYTChiTiets.Where(z => !z.LaDuocPhamBenhVien).OrderBy(z => z.STT).ToList();

            var headerBHYT = string.Empty;
            var headerKhongBHYT = string.Empty;
            var headerThuocNgoaiBV = string.Empty;
            var headerThucPhamChucNang = string.Empty;

            if (xacNhanIn.Header == false)
            {
                headerBHYT = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>TOA THUỐC BẢO HIỂM Y TẾ</th>" +
                         "</p>";
                headerKhongBHYT = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>TOA THUỐC KHÔNG BẢO HIỂM Y TẾ</th>" +
                         "</p>";

                headerThuocNgoaiBV = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                             "<th>TOA THUỐC NGOÀI BỆNH VIỆN</ th>" +
                             "</p>";

                headerThucPhamChucNang = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                                 "<th>ĐƠN TƯ VẤN</ th>" +
                                 "</p>";
            }

            var contentThuocTrongBenhVien = string.Empty;
            var contentThuocNgoaiBenhVien = string.Empty;
            var contentThuocBHYT = string.Empty;
            var contentThucPhamChucNang = string.Empty;

            var resultThuocTrongBenhVien = string.Empty;
            var resultThuocNgoaiBenhVien = string.Empty;
            var resultThuocBHYT = string.Empty;
            var resultThuocThucPhamChucNang = string.Empty;
            var content = string.Empty;
            var sttBHYT = 0;

            var sttKhongBHYTTrongBV = 0;
            var sttKhongBHYTNgoaiBV = 0;
            var sttTPCN = 0;

            if (donThuocBHYTs.Any())
            {
                foreach (var donThuocBHYTChiTiet in donThuocBHYTs)
                {
                    var cd =
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungSangDisplay)
                                 ? "Sáng " + donThuocBHYTChiTiet.DungSang
                                 +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungSangDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungSangDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungTruaDisplay)
                                 ? "Trưa " + donThuocBHYTChiTiet.DungTrua +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungTruaDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungTruaDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                             (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungChieuDisplay)
                                 ? "Chiều " + donThuocBHYTChiTiet.DungChieu +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungChieuDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungChieuDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "") +
                            (!string.IsNullOrEmpty(donThuocBHYTChiTiet.DungToiDisplay)
                                 ? "Tối " + donThuocBHYTChiTiet.DungToi +
                                   (!string.IsNullOrEmpty(donThuocBHYTChiTiet.ThoiGianDungToiDisplay)
                                       ? " " + donThuocBHYTChiTiet.ThoiGianDungToiDisplay
                                       : "") + " " + donThuocBHYTChiTiet.DVT + ","
                                 : "");

                    var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                                 + (!string.IsNullOrEmpty(donThuocBHYTChiTiet.CachDung) ? "<p style='margin:0'><i>" + donThuocBHYTChiTiet.CachDung + " </i></p>" : "");
                    sttBHYT++;
                    resultThuocBHYT += "<tr>";
                    resultThuocBHYT += "<td style='vertical-align: top; text-align: center' >" + sttBHYT + "</td>";
                    resultThuocBHYT += "<td >" + _yeuCauKhamBenhService.FormatTenDuocPham(donThuocBHYTChiTiet.Ten, donThuocBHYTChiTiet.HoatChat, donThuocBHYTChiTiet.HamLuong, donThuocBHYTChiTiet.DuocPhamBenhVienPhanNhomId)
                        + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                        + "</td>";

                    resultThuocBHYT += "<td  style='vertical-align: top;text-align: center' >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocBHYTChiTiet.SoLuong, donThuocBHYTChiTiet.LoaiThuocTheoQuanLy) + " " + donThuocBHYTChiTiet.DVT + "</td>";
                    resultThuocBHYT += "</tr>";
                }

                if (!string.IsNullOrEmpty(resultThuocBHYT))
                {
                    resultThuocBHYT = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocBHYT + "</tbody></table>";
                    var data = new DataYCKBDonThuoc
                    {
                        Header = headerBHYT,
                        TemplateDonThuoc = resultThuocBHYT,
                        LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                        BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                        MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                        HoTen = infoBN.HoTen,
                        Tuoi = infoBN.Tuoi,
                        NamSinhDayDu = infoBN.NamSinhDayDu,
                        CanNang = infoBN.CanNang,
                        GioiTinh = infoBN?.GioiTinh,
                        DiaChi = infoBN?.DiaChi,
                        CMND = infoBN?.CMND,
                        SoTheBHYT = infoBN.BHYTMaSoThe,
                        NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                        NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                        ChuanDoan = infoBN?.ChuanDoan,
                        ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),
                        BacSiKham = tenBacSiKeDon,
                        LoiDan = infoBN.LoiDan,
                        MaBN = infoBN.MaBN,
                        SoDienThoai = infoBN.SoDienThoai,
                        SoThang = infoBN.SoThang,
                        CongKhoan = sttBHYT,
                        //KhoaPhong = khoaPhong
                    };
                    contentThuocBHYT = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocBHYT.Body, data);
                }

            }

            if (donThuocKhongBHYTChiTiets.Any())
            {
                if (donThuocTrongBVs.Any())
                {
                    foreach (var donThuocTrongBV in donThuocTrongBVs)
                    {
                        var cd =
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungSangDisplay)

                                     ? "Sáng " + donThuocTrongBV.DungSang +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungSangDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungSangDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungTruaDisplay)

                                     ? "Trưa " + donThuocTrongBV.DungTrua +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungTruaDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungTruaDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungChieuDisplay)

                                     ? "Chiều " + donThuocTrongBV.DungChieu +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungChieuDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungChieuDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocTrongBV.DungToiDisplay)

                                     ? "Tối " + donThuocTrongBV.DungToi +
                                       (!string.IsNullOrEmpty(donThuocTrongBV.ThoiGianDungToiDisplay)
                                           ? " " + donThuocTrongBV.ThoiGianDungToiDisplay
                                           : "") + " " + donThuocTrongBV.DVT + ","
                                     : "");

                        var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                               + (!string.IsNullOrEmpty(donThuocTrongBV.CachDung) ? "<p style='margin:0'><i>" + donThuocTrongBV.CachDung + " </i></p>" : "");
                        if (donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThucPhamChucNang
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.MyPham
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.VatTuYTe
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThietBiYTe
                             || donThuocTrongBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThuocTuDuocLieu)
                        {
                            sttTPCN++;
                            resultThuocThucPhamChucNang += "<tr>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + sttTPCN + "</td>";
                            resultThuocThucPhamChucNang += "<td >" + "<b>" + donThuocTrongBV.Ten + "</b>"
                             + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";


                        }
                        else
                        {
                            sttKhongBHYTTrongBV++;
                            resultThuocTrongBenhVien += "<tr>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTTrongBV + "</td>";
                            resultThuocTrongBenhVien += "<td >" + _yeuCauKhamBenhService.FormatTenDuocPham(donThuocTrongBV.Ten, donThuocTrongBV.HoatChat, donThuocTrongBV.HamLuong, donThuocTrongBV.DuocPhamBenhVienPhanNhomId)
                                 + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocTrongBenhVien += "<td style='vertical-align: top;text-align: center'  >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocTrongBV.SoLuong, donThuocTrongBV.LoaiThuocTheoQuanLy) + " " + donThuocTrongBV.DVT + "</td>";
                            resultThuocTrongBenhVien += "</tr>";
                        }
                    }
                    if (!string.IsNullOrEmpty(resultThuocTrongBenhVien))
                    {
                        resultThuocTrongBenhVien = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocTrongBenhVien + "</tbody></table>";
                        var data = new DataYCKBDonThuoc
                        {
                            Header = headerKhongBHYT,
                            TemplateDonThuoc = resultThuocTrongBenhVien,
                            LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                            MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                            HoTen = infoBN.HoTen,
                            NamSinhDayDu = infoBN.NamSinhDayDu,

                            Tuoi = infoBN.Tuoi,
                            CMND = infoBN?.CMND,
                            CanNang = infoBN.CanNang,
                            GioiTinh = infoBN?.GioiTinh,
                            DiaChi = infoBN?.DiaChi,
                            SoTheBHYT = infoBN.BHYTMaSoThe,
                            NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                            NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                            ChuanDoan = infoBN?.ChuanDoan,
                            BacSiKham = tenBacSiKeDon,
                            LoiDan = infoBN.LoiDan,
                            NguoiGiamHo = infoBN?.NguoiGiamHo,
                            MaBN = infoBN.MaBN,
                            SoDienThoai = infoBN.SoDienThoai,
                            SoThang = infoBN.SoThang,
                            CongKhoan = sttKhongBHYTTrongBV,
                            //KhoaPhong = khoaPhong,
                            ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                        };
                        contentThuocTrongBenhVien = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocTrongBenhVien.Body, data);
                    }

                }
                if (donThuocNgoaiBVs.Any())
                {
                    foreach (var donThuocNgoaiBV in donThuocNgoaiBVs)
                    {
                        var cd =
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungSangDisplay)

                                     ? "Sáng " + donThuocNgoaiBV.DungSang +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungSangDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungSangDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungTruaDisplay)

                                     ? "Trưa " + donThuocNgoaiBV.DungTrua +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungTruaDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungTruaDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungChieuDisplay)

                                     ? "Chiều " + donThuocNgoaiBV.DungChieu +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungChieuDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungChieuDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "") +
                             (!string.IsNullOrEmpty(donThuocNgoaiBV.DungToiDisplay)
                                     ? "Tối " + donThuocNgoaiBV.DungToi +
                                       (!string.IsNullOrEmpty(donThuocNgoaiBV.ThoiGianDungToiDisplay)
                                           ? " " + donThuocNgoaiBV.ThoiGianDungToiDisplay
                                           : "") + " " + donThuocNgoaiBV.DVT + ","
                                     : "");

                        var cachDung = (!string.IsNullOrEmpty(cd) ? "<i>" + cd.Trim().Remove(cd.Trim().Length - 1) + "<i></br>" : "")
                              + (!string.IsNullOrEmpty(donThuocNgoaiBV.CachDung) ? "<p style='margin:0'><i>" + donThuocNgoaiBV.CachDung + " </i></p>" : "");
                        if (donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThucPhamChucNang
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.MyPham
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.VatTuYTe
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThietBiYTe
                        || donThuocNgoaiBV.DuocPhamBenhVienPhanNhomChaId == (long)Enums.EnumDuocPhamBenhVienPhanNhom.ThuocTuDuocLieu)
                        {
                            sttTPCN++;
                            resultThuocThucPhamChucNang += "<tr>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + sttTPCN + "</td>";
                            resultThuocThucPhamChucNang += "<td >" + "<b>" + donThuocNgoaiBV.Ten + "</b>"
                                + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")

                                + "</td>";
                            resultThuocThucPhamChucNang += "<td style='vertical-align: top;text-align: center' >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
                            resultThuocThucPhamChucNang += "</tr>";
                        }
                        else
                        {
                            sttKhongBHYTNgoaiBV++;
                            resultThuocNgoaiBenhVien += "<tr>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + sttKhongBHYTNgoaiBV + "</td>";
                            resultThuocNgoaiBenhVien += "<td >" + _yeuCauKhamBenhService.FormatTenDuocPham(donThuocNgoaiBV.Ten, donThuocNgoaiBV.HoatChat, donThuocNgoaiBV.HamLuong, donThuocNgoaiBV.DuocPhamBenhVienPhanNhomId)
                                + (!string.IsNullOrEmpty(cachDung) ? "</br> " + cachDung : "")
                                + "</td>";
                            resultThuocNgoaiBenhVien += "<td style='vertical-align: top;text-align: center' >" + _yeuCauKhamBenhService.FormatSoLuong(donThuocNgoaiBV.SoLuong, donThuocNgoaiBV.LoaiThuocTheoQuanLy) + " " + donThuocNgoaiBV.DVT + "</td>";
                            resultThuocNgoaiBenhVien += "</tr>";
                        }
                    }
                    if (!string.IsNullOrEmpty(resultThuocNgoaiBenhVien))
                    {
                        resultThuocNgoaiBenhVien = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên thuốc – Hàm lượng - Liều dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocNgoaiBenhVien + "</tbody></table>";
                        var data = new DataYCKBDonThuoc
                        {
                            Header = headerKhongBHYT,
                            TemplateDonThuoc = resultThuocNgoaiBenhVien,
                            LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                            BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                            MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                            HoTen = infoBN.HoTen,
                            NamSinhDayDu = infoBN.NamSinhDayDu,
                            Tuoi = infoBN.Tuoi,
                            CMND = infoBN?.CMND,
                            CanNang = infoBN.CanNang,
                            GioiTinh = infoBN?.GioiTinh,
                            DiaChi = infoBN?.DiaChi,
                            SoTheBHYT = infoBN.BHYTMaSoThe,
                            NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                            NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                            ChuanDoan = infoBN?.ChuanDoan,
                            BacSiKham = tenBacSiKeDon,
                            LoiDan = infoBN.LoiDan,
                            NguoiGiamHo = infoBN?.NguoiGiamHo,
                            MaBN = infoBN.MaBN,
                            SoDienThoai = infoBN.SoDienThoai,
                            SoThang = infoBN.SoThang,
                            CongKhoan = sttKhongBHYTNgoaiBV,
                            //KhoaPhong = khoaPhong,
                            ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                        };
                        contentThuocNgoaiBenhVien = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocNgoaiBenhVien.Body, data);
                    }
                }
            }
            if (!string.IsNullOrEmpty(resultThuocThucPhamChucNang))
            {
                resultThuocThucPhamChucNang = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>Tên sản phẩm – Cách dùng</th><th>Số lượng</th></tr></thead><tbody>" + resultThuocThucPhamChucNang + "</tbody></table>";
                var data = new DataYCKBDonThuoc
                {
                    Header = headerThucPhamChucNang,
                    TemplateDonThuoc = resultThuocThucPhamChucNang,
                    LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                    BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                    MaTN = "<b>Mã TN: </b>" + "<b>" + infoBN.MaTN + "</b>",
                    HoTen = infoBN.HoTen,
                    NamSinhDayDu = infoBN.NamSinhDayDu,
                    Tuoi = infoBN.Tuoi,
                    CanNang = infoBN.CanNang,
                    GioiTinh = infoBN?.GioiTinh,
                    DiaChi = infoBN?.DiaChi,
                    SoTheBHYT = infoBN.BHYTMaSoThe,
                    ChuanDoan = infoBN?.ChuanDoan,
                    BacSiKham = tenBacSiKeDon,
                    LoiDan = infoBN.LoiDan,
                    MaBN = infoBN.MaBN,
                    SoDienThoai = infoBN.SoDienThoai,
                    SoThang = infoBN.SoThang,
                    CongKhoan = sttTPCN,
                    //KhoaPhong = khoaPhong,
                    ThoiDiemKeDon = donThuocBHYTChiTiets.Any() ? donThuocBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() :
                                (donThuocKhongBHYTChiTiets.Any() ? donThuocKhongBHYTChiTiets.Select(z => z.ThoiDiemKeDon).First() : (DateTime?)null),

                };
                contentThucPhamChucNang = TemplateHelpper.FormatTemplateWithContentTemplate(templateDonThuocThucPhamChucNang.Body, data);
            }
            if (contentThuocBHYT != "")
            {
                contentThuocBHYT = contentThuocBHYT + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThuocTrongBenhVien != "")
            {
                contentThuocTrongBenhVien = contentThuocTrongBenhVien + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThuocNgoaiBenhVien != "")
            {
                contentThuocNgoaiBenhVien = contentThuocNgoaiBenhVien + "<div class=\"pagebreak\"> </div>";
            }
            if (contentThucPhamChucNang != "")
            {
                contentThucPhamChucNang = contentThucPhamChucNang + "<div class=\"pagebreak\"> </div>";
            }
            content = contentThuocBHYT + contentThuocTrongBenhVien + contentThuocNgoaiBenhVien + contentThucPhamChucNang;
            content = content.Remove(content.Length - 30);
            return content;

        }
        //anh thêm vật tư nhé nam 10/02/2020
        public string InPhieuVatTu(XacNhanInThuocBenhNhan xacNhanInThuocBenhNhan)
        {
            var content = string.Empty;
            var contentVatTu = string.Empty;
            var resultVatTu = string.Empty;

            var header = string.Empty;
            if (xacNhanInThuocBenhNhan.Header == true)
            {
                header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>VẬT TƯ Y TẾ</th>" +
                         "</p>";
            }
            var listVatTuYT = xacNhanInThuocBenhNhan.ListGridThuoc;
            Dictionary<long?, int> dictionary = new Dictionary<long?, int>();
            //sort Grid hiện tại theo Grid truyền vào
            dictionary = listVatTuYT
                .Select((id, index) => new
                {
                    key = (long?)id.Id,
                    rank = index
                }).ToDictionary(o => o.key, o => o.rank);
            var templateVatTuYT = _templateRepo.TableNoTracking.Where(x => x.Name.Equals("KhamBenhVatTuYTe")).First();
            var infoBN = ThongTinBenhNhanPhieuThuoc(xacNhanInThuocBenhNhan.TiepNhanId, xacNhanInThuocBenhNhan.YeuCauKhambenhId);
            var getKCBBanDau = string.Empty;
            var getTenNoiThucHien = _yeuCauKhamBenhRepo.TableNoTracking
                               .Where(dt => dt.Id == xacNhanInThuocBenhNhan.YeuCauKhambenhId).Select(x => x.NoiThucHien.KhoaPhong.Ten).FirstOrDefault();
            var getTenNoiKetLuan = _yeuCauKhamBenhRepo.TableNoTracking
                               .Where(dt => dt.Id == xacNhanInThuocBenhNhan.YeuCauKhambenhId).Select(x => x.NoiKetLuan.KhoaPhong.Ten).FirstOrDefault();
            if (infoBN != null && !string.IsNullOrEmpty(infoBN.BHYTMaDKBD))
            {
                getKCBBanDau = _benhVienRepository.TableNoTracking
                            .Where(bv => bv.Ma == infoBN.BHYTMaDKBD).Select(p => p.Ten).FirstOrDefault();
            }

            var getTenBSKham = _bacSiChiDinhRepository.TableNoTracking
                                .Where(bs => bs.Id == _userAgentHelper.GetCurrentUserId())
                                .Select(u =>
                              (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "") + u.HoTen).FirstOrDefault();

            var vTYTChiTiets = _yeuCauKhamBenhRepo.TableNoTracking
                         .Include(yckb => yckb.YeuCauTiepNhan)
                         .Include(yckb => yckb.YeuCauKhamBenhDonVTYTs).ThenInclude(ycdt => ycdt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dtct => dtct.NhomVatTu)
                         .Include(yckb => yckb.YeuCauKhamBenhDonVTYTs).ThenInclude(ycdt => ycdt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dtct => dtct.VatTuBenhVien)
                         .SelectMany(yckb => yckb.YeuCauKhamBenhDonVTYTs)
                                 .Select(vt => vt)
                                 .Where(vt => vt.YeuCauKhamBenhId == xacNhanInThuocBenhNhan.YeuCauKhambenhId)
                         .SelectMany(ycvt => ycvt.YeuCauKhamBenhDonVTYTChiTiets).Include(s => s.YeuCauKhamBenhDonVTYT)
                         .Select(vtct => vtct).Include(dtct => dtct.NhomVatTu).Include(dtct => dtct.VatTuBenhVien)
                         .OrderBy(p => dictionary.Any(a => a.Key == p.Id) ? dictionary[p.Id] : dictionary.Count)
                         .ToList();
            var STT = 0;
            foreach (var item in vTYTChiTiets)
            {
                STT++;
                resultVatTu += "<tr>";
                resultVatTu += "<td rowspan='2' align='center'>" + STT + "</td>";
                resultVatTu += "<td>" + item.Ten + "</td>";
                resultVatTu += "<td rowspan='2' align='center'>" + item.SoLuong + " " + item.DonViTinh + "</td>";
                resultVatTu += "</tr>";
                resultVatTu += "<tr>";
                resultVatTu += "<td><i>" + item.GhiChu + "</i></td>";
                resultVatTu += "</tr>";
            }
            resultVatTu = "<style>.thuoc-table{border-top: 1px solid #000;border-right: 1px solid #000;border-spacing: 0;}.thuoc-table td,.thuoc-table th{border-left: 1px solid #000;border-bottom: 1px solid #000;padding: 5px;}</style><table width='100%' class='thuoc-table'><thead><tr><th>STT</th><th>TÊN VTYT</th><th>SỐ LƯỢNG</th></tr></thead><tbody>" + resultVatTu + "</tbody></table>";

            var data = new DataYCKBVatTu
            {
                Header = header,
                TemplateVatTu = resultVatTu,
                LogoUrl = xacNhanInThuocBenhNhan.Hosting + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(infoBN.MaTN) ? BarcodeHelper.GenerateBarCode(infoBN.MaTN) : "",
                MaTN = infoBN.MaTN,
                HoTen = infoBN.HoTen,
                Tuoi = infoBN.Tuoi,
                CanNang = infoBN.CanNang,
                GioiTinh = infoBN?.GioiTinh,
                DiaChi = infoBN?.DiaChi,
                CMND = infoBN?.CMND,
                ChuanDoan = infoBN?.ChuanDoan,
                SoTheBHYT = infoBN.BHYTMaSoThe,
                NgayHieuLuc = infoBN.BHYTNgayHieuLuc == null ? "" : (infoBN.BHYTNgayHieuLuc.Value).ApplyFormatDate(),
                NgayHetHan = infoBN.BHYTNgayHetHan == null ? "" : (infoBN.BHYTNgayHetHan.Value).ApplyFormatDate(),
                NoiDangKy = getKCBBanDau,
                Ngay = DateTime.Today.Day.ConvertDateToString(),
                Thang = DateTime.Today.Month.ConvertMonthToString(),
                Nam = DateTime.Today.Year.ToString(),
                BacSiKham = getTenBSKham,
                NguoiGiamHo = infoBN?.NguoiGiamHo,
                MaBN = infoBN?.MaBN,
                SoDienThoai = infoBN.SoDienThoai,
                CongKhoan = STT,
                KhoaPhong = getTenNoiKetLuan != null ? getTenNoiKetLuan : getTenNoiThucHien,
                ThoiDiemKeDon = vTYTChiTiets.Any() ? vTYTChiTiets.Select(z => z.YeuCauKhamBenhDonVTYT.ThoiDiemKeDon).First() : (DateTime?)null

            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(templateVatTuYT.Body, data);
            return content;
        }
        private DataBenhNhan ThongTinBenhNhanPhieuThuoc(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            var thongTinBenhNhanPhieuThuoc = _yeuCauTiepNhanRepo.TableNoTracking
                         .Where(s => s.YeuCauKhamBenhs.Any(y => y.Id == yeuCauKhamBenhId))
                         .Select(s => new DataBenhNhan
                         {
                             MaTN = s.MaYeuCauTiepNhan,
                             Id = s.BenhNhan.Id,
                             HoTen = s.HoTen,
                             NamSinh = s.NamSinh,
                             Tuoi = s.NamSinh != null ? (DateTime.Now.Year - s.NamSinh) : null,
                             CanNang = s.KetQuaSinhHieus.Count != 0 && s.KetQuaSinhHieus.FirstOrDefault() != null
                                                ? (s.KetQuaSinhHieus.OrderByDescending(p => p.Id).Where(p => p.CanNang != null).FirstOrDefault().CanNang.ToString() + " kg") : null,
                             GioiTinh = s.GioiTinh.GetDescription(),
                             DiaChi = s.DiaChiDayDu,
                             BHYTMaSoThe = s.BHYTMaSoThe,
                             BHYTMaDKBD = s.BHYTMaDKBD,
                             ChuanDoan = s.YeuCauKhamBenhs.Where(p => p.Id == yeuCauKhamBenhId).Select(p => p.GhiChuICDChinh).FirstOrDefault(),
                             LoiDan = s.YeuCauKhamBenhs.Where(p => p.Id == yeuCauKhamBenhId).SelectMany(p => p.YeuCauKhamBenhDonThuocs).Select(dt => dt.GhiChu).FirstOrDefault().Replace("\n", "<br>"),
                             BHYTNgayHieuLuc = s.BHYTNgayHieuLuc,
                             BHYTNgayHetHan = s.BHYTNgayHetHan,
                             CMND = s.BenhNhan.SoChungMinhThu,
                             NguoiGiamHo = s.NguoiLienHeHoTen,
                             SoDienThoai = s.SoDienThoai,
                             MaBN = s.BenhNhan.MaBN,
                             SoThang = CalculateHelper.TinhTongSoThangCuaTuoi(s.NgaySinh, s.ThangSinh, s.NamSinh)
                         }).FirstOrDefault();
            return thongTinBenhNhanPhieuThuoc;
        }
        // end anh thêm vật tư nhé nam 10/02/2020

        private string GetTuongTac(string MaHoatChat, List<string> lstDuocPham, List<MaHoatChatGridVo> lstADR)
        {
            var TuongTac = string.Empty;
            if (lstADR.Count > 0)
            {
                foreach (var item in lstADR)
                {
                    if (item.MaHoatChat1 == MaHoatChat && lstDuocPham.Where(p => p != MaHoatChat).Contains(item.MaHoatChat2))
                    {
                        TuongTac += item.Ten2 + "; ";
                    }
                    if ((item.MaHoatChat2 == MaHoatChat && lstDuocPham.Where(p => p != MaHoatChat).Contains(item.MaHoatChat1)))
                    {
                        TuongTac += item.Ten1 + "; ";
                    }
                    if (TuongTac == string.Empty)
                    {
                        TuongTac = "Không";
                    }
                }
            }
            else
            {
                if (TuongTac == string.Empty)
                {
                    TuongTac = "Không";
                }
            }

            return TuongTac;
        }
        #endregion

        #region mua đơn thuốc cũ khách vãng lai
        public async Task<List<ThongTinDuocPhamQuayThuocCuVo>> GetDonThuocChiTietCu(long[] yeuCauKhamBenhDonThuocId)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            List<ThongTinDuocPhamQuayThuocCuVo> lstThongTinDuocPhamQuayThuocVo = new List<ThongTinDuocPhamQuayThuocCuVo>();
            List<ThongTinDuocPhamQuayThuocCuVo> lstThongTinDuocPhamCuQuayThuocVo = new List<ThongTinDuocPhamQuayThuocCuVo>();
            foreach (var item in yeuCauKhamBenhDonThuocId)
            {
                var result = _yeuCauKhamBenhDonThuocChiTietRepo.TableNoTracking
               .Where(o => o.YeuCauKhamBenhDonThuocId == item && o.LaDuocPhamBenhVien == true)
               .Select(x => new ThongTinDuocPhamQuayThuocCuVo()
               {
                   DuocPhamId = x.DuocPham.Id,
                   MaHoatChat = x.DuocPham.MaHoatChat != null ? x.DuocPham.MaHoatChat : "",
                   Ma = x.DuocPham.DuocPhamBenhVien != null ? x.DuocPham.DuocPhamBenhVien.Ma : x.DuocPham.MaHoatChat,
                   TenDuocPham = x.DuocPham.Ten != null ? x.DuocPham.Ten : "",
                   TenHoatChat = x.DuocPham.HoatChat != null ? x.DuocPham.HoatChat : "",
                   DonViTinh = x.DuocPham.DonViTinh.Ten != null ? x.DuocPham.DonViTinh.Ten : "",
                   SoLuongMua = x.SoLuong,
                   isNew = false,
                   LoaiDonThuoc = x.YeuCauKhamBenhDonThuoc != null ? x.YeuCauKhamBenhDonThuoc.LoaiDonThuoc : 0,
                   NongDoVaHamLuong = x.DuocPham.HamLuong,
                   LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
               });
                lstThongTinDuocPhamQuayThuocVo.AddRange(result);
            }
            List<ThongTinDuocPhamQuayThuocCuVo> apDungToaThuocChiTietVos = new List<ThongTinDuocPhamQuayThuocCuVo>();
            foreach (var ketoa in lstThongTinDuocPhamQuayThuocVo)
            {

                if (ketoa.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                {
                    ThongTinDuocPhamQuayThuocCuVo toaThuocChiTietVo = new ThongTinDuocPhamQuayThuocCuVo
                    {
                        DuocPhamId = ketoa.DuocPhamId,
                        TenDuocPham = ketoa.TenDuocPham,
                        MaHoatChat = ketoa.MaHoatChat,
                        TenHoatChat = ketoa.TenHoatChat,
                        SoLuongTon = 0,
                        DonViTinh = ketoa.DonViTinh,
                        SoLuongMua = ketoa.SoLuongMua,
                        SoLuongToa = ketoa.SoLuongToa,
                        ThanhTien = 0,
                        DonGia = 0,
                        Solo = "",
                        isNew = false,
                        ViTri = "",
                        LoaiDonThuoc = ketoa.LoaiDonThuoc,
                        NongDoVaHamLuong = ketoa.NongDoVaHamLuong,
                        HighLightClass = ketoa.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT ? "bg-row-lightRed" : "",
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien

                    };
                    lstThongTinDuocPhamCuQuayThuocVo.Add(toaThuocChiTietVo);
                }
                else
                {
                    var result = _nhapKhoDuocPhamChiTietRepo.TableNoTracking
                    .Where(kho =>
                        kho.DuocPhamBenhVienId == ketoa.DuocPhamId &&
                        kho.NhapKhoDuocPhams.KhoId == (long)EnumKhoDuocPham.KhoNhaThuoc && kho.HanSuDung >= DateTime.Now &&
                        kho.SoLuongDaXuat < kho.SoLuongNhap)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Select(x => new ThongTinDuocPhamQuayThuocCuVo()
                    {
                        DuocPhamId = x.DuocPhamBenhViens.DuocPham.Id,
                        MaHoatChat = x.DuocPhamBenhViens.DuocPham.HoatChat,
                        TenDuocPham = x.DuocPhamBenhViens.DuocPham.Ten,
                        SoLuongTon = x.SoLuongNhap - x.SoLuongDaXuat,
                        NhapKhoDuocPhamChiTietId = x.Id,
                        TenHoatChat = x.DuocPhamBenhViens.DuocPham.HoatChat,
                        DonViTinh = x.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        SoLuongToa = 0,
                        SoLuongMua = 0,
                        DonGia = x.DonGiaBan,
                        ThanhTien = x.DonGiaBan * Convert.ToDecimal(ketoa.SoLuongMua),
                        Solo = x.Solo,
                        ViTri = x.KhoDuocPhamViTri.Ten,
                        HanSuDung = x.HanSuDung,
                        //HanSuDungHientThi = x.HanSuDung.ApplyFormatDate(),
                        LoaiDonThuoc = ketoa.LoaiDonThuoc,
                        isNew = true,
                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                    }).OrderBy(s => s.HanSuDung);
                    if (result.Any())
                    {
                        if (result.Count() == 1)
                        {
                            foreach (var itemx in result)
                            {
                                if (itemx.SoLuongTon < ketoa.SoLuongMua)
                                {
                                    itemx.SoLuongMua = itemx.SoLuongTon;
                                    itemx.ThanhTien = Convert.ToDecimal(itemx.DonGia) * Convert.ToDecimal(itemx.SoLuongTon);
                                    lstThongTinDuocPhamCuQuayThuocVo.Add(itemx);
                                    ThongTinDuocPhamQuayThuocCuVo toaThuocChiTietVo = new ThongTinDuocPhamQuayThuocCuVo
                                    {
                                        DuocPhamId = ketoa.DuocPhamId,
                                        TenDuocPham = ketoa.TenDuocPham,
                                        MaHoatChat = ketoa.MaHoatChat,
                                        TenHoatChat = ketoa.TenHoatChat,
                                        SoLuongTon = 0,
                                        DonViTinh = ketoa.DonViTinh,
                                        SoLuongMua = ketoa.SoLuongMua - itemx.SoLuongTon,
                                        SoLuongToa = ketoa.SoLuongToa,
                                        ThanhTien = 0,
                                        DonGia = 0,
                                        Solo = "",
                                        isNew = false,
                                        ViTri = "",
                                        ThuocTrongKhoExit = true,
                                        NongDoVaHamLuong = ketoa.NongDoVaHamLuong,
                                        LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien

                                    };
                                    lstThongTinDuocPhamCuQuayThuocVo.Add(toaThuocChiTietVo);
                                }
                                if (itemx.SoLuongTon >= ketoa.SoLuongMua)
                                {
                                    itemx.SoLuongMua = ketoa.SoLuongMua;
                                    itemx.ThanhTien = Convert.ToDecimal(itemx.DonGia) * Convert.ToDecimal(ketoa.SoLuongMua);
                                    lstThongTinDuocPhamCuQuayThuocVo.Add(itemx);
                                }
                            }
                        }
                        else
                        {
                            var soLuongMua = ketoa.SoLuongMua;
                            foreach (var itemx in result)
                            {
                                if (itemx.SoLuongTon < ketoa.SoLuongMua)
                                {
                                    itemx.SoLuongMua = itemx.SoLuongTon;
                                    itemx.ThanhTien = Convert.ToDecimal(itemx.DonGia) * Convert.ToDecimal(itemx.SoLuongTon);
                                    soLuongMua = ketoa.SoLuongMua - itemx.SoLuongTon;
                                    lstThongTinDuocPhamCuQuayThuocVo.Add(itemx);
                                }
                                if (itemx.SoLuongTon >= ketoa.SoLuongMua)
                                {
                                    itemx.SoLuongMua = soLuongMua;
                                    itemx.ThanhTien = Convert.ToDecimal(itemx.DonGia) * Convert.ToDecimal(soLuongMua);
                                    soLuongMua = ketoa.SoLuongMua - itemx.SoLuongTon;
                                    lstThongTinDuocPhamCuQuayThuocVo.Add(itemx);
                                }
                            }
                            if (soLuongMua > 0)
                            {
                                ThongTinDuocPhamQuayThuocCuVo toaThuocChiTietVo = new ThongTinDuocPhamQuayThuocCuVo
                                {
                                    DuocPhamId = ketoa.DuocPhamId,
                                    TenDuocPham = ketoa.TenDuocPham,
                                    MaHoatChat = ketoa.MaHoatChat,
                                    TenHoatChat = ketoa.TenHoatChat,
                                    SoLuongTon = 0,
                                    DonViTinh = ketoa.DonViTinh,
                                    SoLuongMua = soLuongMua,
                                    SoLuongToa = ketoa.SoLuongToa,
                                    ThanhTien = 0,
                                    DonGia = 0,
                                    Solo = "",
                                    isNew = false,
                                    ViTri = "",
                                    ThuocTrongKhoExit = true,
                                    NongDoVaHamLuong = ketoa.NongDoVaHamLuong,
                                    LoaiDuocPhamHoacVatTu = Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                                };
                                lstThongTinDuocPhamCuQuayThuocVo.Add(toaThuocChiTietVo);
                            }

                        }
                    }
                    else
                    {
                        ThongTinDuocPhamQuayThuocCuVo toaThuocChiTietVo = new ThongTinDuocPhamQuayThuocCuVo
                        {
                            DuocPhamId = ketoa.DuocPhamId,
                            TenDuocPham = ketoa.TenDuocPham,
                            MaHoatChat = ketoa.MaHoatChat,
                            TenHoatChat = ketoa.TenHoatChat,
                            SoLuongTon = 0,
                            DonViTinh = ketoa.DonViTinh,
                            SoLuongMua = ketoa.SoLuongMua,
                            SoLuongToa = ketoa.SoLuongToa,
                            ThanhTien = 0,
                            DonGia = 0,
                            Solo = "",
                            isNew = false,
                            ViTri = "",
                            ThuocTrongKhoExit = true,
                            NongDoVaHamLuong = ketoa.NongDoVaHamLuong,

                        };
                        lstThongTinDuocPhamCuQuayThuocVo.Add(toaThuocChiTietVo);
                    }

                }
            }

            return lstThongTinDuocPhamCuQuayThuocVo;
        }
        #endregion
        #region Get đơn thuốc cũ của người bệnh
        public async Task<GridDataSource> GetDataForGridToaThuocCuAsync(QueryInfo queryInfo, long maBN)
        {
            BuildDefaultSortExpression(queryInfo);
            //var query = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.DichVuKhamBenh).Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauKhamBenhChuanDoans).ThenInclude(x => x.ChuanDoan)
            //    .Include(x => x.YeuCauTiepNhan).Include(x => x.DonThuocThanhToanChiTiets).Where(x => x.YeuCauTiepNhan.BenhNhanId == maBN && x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
            //    .Select(s => new ToaThuocCuVO()
            //    {
            //        Id = s.YeuCauKhamBenh.Id,
            //        BacSiKham = s.YeuCauKhamBenh.BacSiKetLuanId != null ? s.YeuCauKhamBenh.BacSiKetLuan.User.HoTen : "", // cap nhap sau
            //        ChuanDoan =String.Join(",", s.YeuCauKhamBenh.YeuCauKhamBenhChuanDoans.Select(x => x.ChuanDoan.TenTiengViet).ToArray()), // s.YeuCauKhamBenh.YeuCauKhamBenhChuanDoans.Where(z => z.YeuCauKhamBenhId == s.Id).First().ChuanDoanId != null ? s.YeuCauKhamBenh.YeuCauKhamBenhChuanDoans.Where(z => z.YeuCauKhamBenhId == s.Id).FirstOrDefault().ChuanDoan.TenTiengViet : "",
            //        DVKham = s.YeuCauKhamBenh.DichVuKhamBenhBenhVien.DichVuKhamBenh.Ten,
            //        DVKhamId = s.YeuCauKhamBenh.DichVuKhamBenhBenhVien.DichVuKhamBenhId,
            //        NgayKham = s.YeuCauKhamBenh.ThoiDiemDangKy.ApplyFormatDate()

            //    });
            //var u = _yeuCauKhamBenhRepo.TableNoTracking.Include(x => x.DonThuocThanhToans).Include(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.DichVuKhamBenh).Include(x => x.YeuCauKhamBenhChuanDoans).ThenInclude(x => x.ChuanDoan)
            //   .ToList();
            var query = _yeuCauKhamBenhDonThuocRepo.TableNoTracking
                                           .Where(x => x.YeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && x.YeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.MaBN == Convert.ToString(maBN) && x.DonThuocThanhToans.Any() && x.YeuCauKhamBenh.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                                                           .Select(s => new ToaThuocCuVO()
                                                           {
                                                               Id = s.Id,
                                                               BacSiKham = s.YeuCauKhamBenh.BacSiKetLuanId != null ? s.YeuCauKhamBenh.BacSiKetLuan.User.HoTen : "", // cap nhap sau
                                                               ChuanDoan = s.YeuCauKhamBenh.Icdchinh.TenTiengViet + ". " + String.Join(",", s.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(x => x.ICD.TenTiengViet).ToArray()),
                                                               DVKham = s.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                                                               DVKhamId = s.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                                                               NgayKham = s.YeuCauKhamBenh.ThoiDiemDangKy.ApplyFormatDateTimeSACH(),
                                                               NgayKhamDate = s.YeuCauKhamBenh.ThoiDiemDangKy
                                                           });


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<TimKiemThuocToaThuocCuVO>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.ChuanDoan))
                {
                    query = query.Where(p => p.ChuanDoanDiacritics.TrimEnd().TrimStart().ToLower().Contains(queryString.ChuanDoan.RemoveDiacritics().TrimEnd().TrimStart().ToLower()));
                }
                if (!string.IsNullOrEmpty(queryString.DVKham))
                {
                    query = query.Where(p => p.DVKhamDiacritics.TrimEnd().TrimStart().ToLower().Contains(queryString.DVKham.RemoveDiacritics().TrimEnd().TrimStart().ToLower()));
                }
                if (!string.IsNullOrEmpty(queryString.BacSi))
                {
                    query = query.Where(p => p.BacSiKhamDiacritics.TrimEnd().TrimStart().ToLower().Contains(queryString.BacSi.RemoveDiacritics().TrimEnd().TrimStart().ToLower()));
                }
                //if (queryString.NgayKham != null)
                //{
                //    var ngay = Convert.ToDateTime(queryString.NgayKham).ApplyFormatDate();
                //    query = query.Where(x => x.NgayKham.Equals(ngay));
                //}
                if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                {
                    DateTime TuNgayPart = DateTime.Now;
                    DateTime DenNgaysPart = DateTime.Now;
                    DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                    DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);

                    var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                    query = query.Where(p => p.NgayKhamDate >= TuNgayPart && p.NgayKhamDate <= denNgay);
                }
                if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                {
                    DateTime TuNgayPart = DateTime.Now;
                    DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                    query = query.Where(p => p.NgayKhamDate >= TuNgayPart);
                }
                if (string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                {
                    DateTime DenNgaysPart = DateTime.Now;
                    DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);

                    var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                    query = query.Where(p => p.NgayKhamDate <= denNgay);
                }
            }
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            //await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridToaThuocCuAsync(QueryInfo queryInfo, long maBN)
        {
            var query = _yeuCauKhamBenhDonThuocRepo.TableNoTracking
                                           .Where(x => x.YeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && x.YeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.MaBN == Convert.ToString(maBN) && x.DonThuocThanhToans.Any() && x.YeuCauKhamBenh.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                                                           .Select(s => new ToaThuocCuVO()
                                                           {
                                                               Id = s.Id,
                                                               BacSiKham = s.YeuCauKhamBenh.BacSiKetLuanId != null ? s.YeuCauKhamBenh.BacSiKetLuan.User.HoTen : "", // cap nhap sau
                                                               ChuanDoan = s.YeuCauKhamBenh.Icdchinh.TenTiengViet + ". " + String.Join(",", s.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(x => x.ICD.TenTiengViet).ToArray()),
                                                               DVKham = s.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                                                               DVKhamId = s.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,
                                                               NgayKham = s.YeuCauKhamBenh.ThoiDiemDangKy.ApplyFormatDateTimeSACH(),
                                                               NgayKhamDate = s.YeuCauKhamBenh.ThoiDiemDangKy
                                                           });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<TimKiemThuocToaThuocCuVO>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.ChuanDoan))
                {
                    query = query.Where(p => p.ChuanDoanDiacritics.TrimEnd().TrimStart().ToLower().Contains(queryString.ChuanDoan.RemoveDiacritics().TrimEnd().TrimStart().ToLower()));
                }
                if (!string.IsNullOrEmpty(queryString.DVKham))
                {
                    query = query.Where(p => p.DVKhamDiacritics.TrimEnd().TrimStart().ToLower().Contains(queryString.DVKham.RemoveDiacritics().TrimEnd().TrimStart().ToLower()));
                }
                if (!string.IsNullOrEmpty(queryString.BacSi))
                {
                    query = query.Where(p => p.BacSiKhamDiacritics.TrimEnd().TrimStart().ToLower().Contains(queryString.BacSi.RemoveDiacritics().TrimEnd().TrimStart().ToLower()));
                }
                if (!string.IsNullOrEmpty(queryString.DateStart?.Trim()) && !string.IsNullOrEmpty(queryString.DateEnd?.Trim()))
                {
                    DateTime TuNgayPart = DateTime.Now;
                    DateTime DenNgaysPart = DateTime.Now;
                    DateTime.TryParseExact(queryString.DateStart, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                    DateTime.TryParseExact(queryString.DateEnd, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);

                    var denNgay = new DateTime(DenNgaysPart.Year, DenNgaysPart.Month, DenNgaysPart.Day, 23, 59, 59);
                    query = query.Where(p => p.NgayKhamDate >= TuNgayPart && p.NgayKhamDate <= denNgay);
                }
            }
            var countTask = query.Count();
            //await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauKhamBenhDonThuocChiTietRepo.TableNoTracking
                .Where(o => o.YeuCauKhamBenhDonThuocId == long.Parse(queryInfo.SearchTerms))
                .Select(x => new ChiTietThuocToaThuocCuVO()
                {
                    Id = x.Id,
                    TenThuoc = x.DuocPham.Ten,
                    HoatChat = x.DuocPham.HoatChat,
                    DVT = x.DuocPham.DonViTinh.Ten,
                    SoLuong = x.SoLuong.ToString(),
                    LaDuocPhamBenhVien = x.LaDuocPhamBenhVien,
                });
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenThuoc, g => g.HoatChat);

            // query = query.ApplyLike(queryInfo.SearchTerms, g => g.Gia.ToString(), g => g.TiLeBaoHiemThanhToan.ToString());
            //query = query.OrderByDescending(x => x.TuNgay);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            var query = _yeuCauKhamBenhDonThuocChiTietRepo.TableNoTracking
                .Where(o => o.YeuCauKhamBenhDonThuocId == long.Parse(queryInfo.SearchTerms))
                .Select(x => new ChiTietThuocToaThuocCuVO()
                {
                    Id = x.Id,
                    TenThuoc = x.DuocPham.Ten,
                    HoatChat = x.DuocPham.HoatChat,
                    DVT = x.DuocPham.DonViTinh.Ten,
                    SoLuong = x.SoLuong.ToString(),
                    LaDuocPhamBenhVien = x.LaDuocPhamBenhVien,
                });
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenThuoc, g => g.HoatChat);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion
        private async Task<ThongTinMienGiamVo> GetThongTinMienGiam(long yeuCauTiepNhanId)
        {
            var thongTinMienGiamVo = new ThongTinMienGiamVo();

            var yeuCauTiepNhan = await _yeuCauTiepNhanRepo.GetByIdAsync(yeuCauTiepNhanId,
                x => x
                    .Include(o => o.MienGiamChiPhis)
                    .Include(o => o.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauDichVuKyThuats)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens)
                    .Include(o => o.YeuCauDuocPhamBenhViens)
                    .Include(o => o.YeuCauVatTuBenhViens)
                    //TODO: need update goi dv
                    //.Include(o => o.YeuCauGoiDichVus)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets)
                    .Include(o => o.CongTyUuDai)
                    .Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                    .Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKyThuatBenhViens)
                    .Include(o => o.GiayMienGiamThem)
                    .Include(o => o.TheVoucherYeuCauTiepNhans).ThenInclude(v => v.TheVoucher).ThenInclude(v => v.Voucher));

            //Thông tin đối tượng ưu đãi
            if (yeuCauTiepNhan.DoiTuongUuDai != null)
            {
                var thongTinMienGiamTheoDoiTuongUuDaiVo = new ThongTinMienGiamTheoDoiTuongUuDaiVo
                {
                    CongTyUudai = yeuCauTiepNhan.CongTyUuDai?.Ten,
                    DoiTuongUuDai = yeuCauTiepNhan.DoiTuongUuDai.Ten,
                    //DichVuMiemGiamTheoTiLes = new List<DichVuMiemGiamTheoTiLe>()
                };


                //foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                //{
                //    var uuDaiDichVuKhamBenh = yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKhamBenhBenhViens.FirstOrDefault(o => o.DichVuKhamBenhBenhVienId == yeuCauKhamBenh.DichVuKhamBenhBenhVienId);
                //    if (uuDaiDichVuKhamBenh != null)
                //    {
                //        thongTinMienGiamTheoDoiTuongUuDaiVo.DichVuMiemGiamTheoTiLes.Add(new DichVuMiemGiamTheoTiLe
                //        {
                //            LoaiNhom = NhomChiPhiKhamChuaBenh.DichVuKhamBenh,
                //            DichVuId = yeuCauKhamBenh.Id,
                //            TiLe = uuDaiDichVuKhamBenh.TiLeUuDai
                //        });
                //    }
                //}
                //foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                //{
                //    var uuDaiDichVuKyThuat = yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKyThuatBenhViens.FirstOrDefault(o => o.DichVuKyThuatBenhVienId == yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId);
                //    if (uuDaiDichVuKyThuat != null)
                //    {
                //        thongTinMienGiamTheoDoiTuongUuDaiVo.DichVuMiemGiamTheoTiLes.Add(new DichVuMiemGiamTheoTiLe
                //        {
                //            LoaiNhom = NhomChiPhiKhamChuaBenh.DichVuKyThuat,
                //            DichVuId = yeuCauDichVuKyThuat.Id,
                //            TiLe = uuDaiDichVuKyThuat.TiLeUuDai
                //        });
                //    }
                //}

                thongTinMienGiamVo.ThongTinMienGiamTheoDoiTuongUuDaiVo = thongTinMienGiamTheoDoiTuongUuDaiVo;
            }

            //Thông tin miễm giảm thêm
            if (yeuCauTiepNhan.CoMienGiamThem == true)
            {
                decimal soTienDaMienGiam = 0;
                foreach (var mienGiamChiPhi in yeuCauTiepNhan.MienGiamChiPhis.Where(o => o.LoaiMienGiam == LoaiMienGiam.MienGiamThem))
                {
                    if (mienGiamChiPhi.YeuCauKhamBenhId != null && yeuCauTiepNhan.YeuCauKhamBenhs.Any(o => o.Id == mienGiamChiPhi.YeuCauKhamBenhId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    {
                        soTienDaMienGiam += mienGiamChiPhi.SoTien;
                    }
                    if (mienGiamChiPhi.YeuCauDichVuKyThuatId != null && yeuCauTiepNhan.YeuCauDichVuKyThuats.Any(o => o.Id == mienGiamChiPhi.YeuCauDichVuKyThuatId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    {
                        soTienDaMienGiam += mienGiamChiPhi.SoTien;
                    }
                    if (mienGiamChiPhi.YeuCauDichVuGiuongBenhVienId != null && yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(o => o.Id == mienGiamChiPhi.YeuCauDichVuGiuongBenhVienId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    {
                        soTienDaMienGiam += mienGiamChiPhi.SoTien;
                    }
                    if (mienGiamChiPhi.YeuCauDuocPhamBenhVienId != null && yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Any(o => o.Id == mienGiamChiPhi.YeuCauDuocPhamBenhVienId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    {
                        soTienDaMienGiam += mienGiamChiPhi.SoTien;
                    }
                    if (mienGiamChiPhi.YeuCauVatTuBenhVienId != null && yeuCauTiepNhan.YeuCauVatTuBenhViens.Any(o => o.Id == mienGiamChiPhi.YeuCauVatTuBenhVienId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    {
                        soTienDaMienGiam += mienGiamChiPhi.SoTien;
                    }
                    //TODO: need update goi dv
                    //if (mienGiamChiPhi.YeuCauGoiDichVuId != null && yeuCauTiepNhan.YeuCauGoiDichVus.Any(o => o.Id == mienGiamChiPhi.YeuCauGoiDichVuId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    //{
                    //    soTienDaMienGiam += mienGiamChiPhi.SoTien;
                    //}
                    if (mienGiamChiPhi.DonThuocThanhToanChiTietId != null && yeuCauTiepNhan.DonThuocThanhToans.Any(o => o.DonThuocThanhToanChiTiets.Any(ct => ct.Id == mienGiamChiPhi.DonThuocThanhToanChiTietId) && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                    {
                        soTienDaMienGiam += mienGiamChiPhi.SoTien;
                    }
                }
                //thông tin giấy miễm giảm thêm
                thongTinMienGiamVo.ThongTinMienGiamThemVo = new ThongTinMienGiamThemVo
                {
                    //LoaiMienGiamThem = yeuCauTiepNhan.LoaiMienGiamThem.GetValueOrDefault(),
                    //SoTienMG = yeuCauTiepNhan.SoTienMienGiamThem.GetValueOrDefault(),
                    //TiLeMienGiam = yeuCauTiepNhan.TiLeMienGiamThem.GetValueOrDefault(),
                    //SoTienMGConLai = yeuCauTiepNhan.SoTienMienGiamThem.GetValueOrDefault() - soTienDaMienGiam,
                    LyDoMiemGiam = yeuCauTiepNhan.LyDoMienGiamThem,
                    TaiLieuDinhKemGiayMiemGiam = new TaiLieuDinhKemGiayMiemGiam
                    {
                        Ten = yeuCauTiepNhan.GiayMienGiamThem.Ten,
                        DuongDan = yeuCauTiepNhan.GiayMienGiamThem.DuongDan,
                        TenGuid = yeuCauTiepNhan.GiayMienGiamThem.TenGuid,
                        KichThuoc = yeuCauTiepNhan.GiayMienGiamThem.KichThuoc,
                        LoaiTapTin = (int)yeuCauTiepNhan.GiayMienGiamThem.LoaiTapTin
                    }
                };
            }

            //Thông tin miễm giảm theo voucher
            if (yeuCauTiepNhan.TheVoucherYeuCauTiepNhans.Any())
            {
                var thongTinMienGiamVoucherVo = new ThongTinMienGiamVoucherVo
                {
                    //LoaiVoucher = yeuCauTiepNhan.TheVoucherYeuCauTiepNhans.First().TheVoucher.Voucher.LoaiVoucher,
                    MaVouchers = yeuCauTiepNhan.TheVoucherYeuCauTiepNhans.Select(o => new LookupItemVo { DisplayName = o.TheVoucher.Ma, KeyId = o.TheVoucher.Id }).ToList(),
                    DuocPhamMienGiamTheoTiLes = new List<DuocPhamMienGiamTheoTiLe>(),
                    DichVuMiemGiamTheoTiLes = new List<DichVuMiemGiamTheoTiLe>()
                };
                //var cauHinhVoucher = _cauHinhService.LoadSetting<CauHinhVoucher>();
                //decimal soTienVoucherDaMienGiam = 0;
                //foreach (var mienGiamChiPhi in yeuCauTiepNhan.MienGiamChiPhis.Where(o => o.LoaiMienGiam == Enums.LoaiMienGiam.Voucher))
                //{
                //    if (mienGiamChiPhi.YeuCauKhamBenhId != null && yeuCauTiepNhan.YeuCauKhamBenhs.Any(o => o.Id == mienGiamChiPhi.YeuCauKhamBenhId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //    {
                //        soTienVoucherDaMienGiam += mienGiamChiPhi.SoTien;
                //    }
                //    if (mienGiamChiPhi.YeuCauDichVuKyThuatId != null && yeuCauTiepNhan.YeuCauDichVuKyThuats.Any(o => o.Id == mienGiamChiPhi.YeuCauDichVuKyThuatId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //    {
                //        soTienVoucherDaMienGiam += mienGiamChiPhi.SoTien;
                //    }
                //    if (mienGiamChiPhi.YeuCauDichVuGiuongBenhVienId != null && yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(o => o.Id == mienGiamChiPhi.YeuCauDichVuGiuongBenhVienId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //    {
                //        soTienVoucherDaMienGiam += mienGiamChiPhi.SoTien;
                //    }
                //    if (mienGiamChiPhi.YeuCauDuocPhamBenhVienId != null && yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Any(o => o.Id == mienGiamChiPhi.YeuCauDuocPhamBenhVienId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //    {
                //        soTienVoucherDaMienGiam += mienGiamChiPhi.SoTien;
                //    }
                //    if (mienGiamChiPhi.YeuCauVatTuBenhVienId != null && yeuCauTiepNhan.YeuCauVatTuBenhViens.Any(o => o.Id == mienGiamChiPhi.YeuCauVatTuBenhVienId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //    {
                //        soTienVoucherDaMienGiam += mienGiamChiPhi.SoTien;
                //    }
                //    //TODO: need update goi dv
                //    //if (mienGiamChiPhi.YeuCauGoiDichVuId != null && yeuCauTiepNhan.YeuCauGoiDichVus.Any(o => o.Id == mienGiamChiPhi.YeuCauGoiDichVuId && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //    //{
                //    //    soTienVoucherDaMienGiam += mienGiamChiPhi.SoTien;
                //    //}
                //    if (mienGiamChiPhi.DonThuocThanhToanChiTietId != null && yeuCauTiepNhan.DonThuocThanhToans.Any(o => o.DonThuocThanhToanChiTiets.Any(ct => ct.Id == mienGiamChiPhi.DonThuocThanhToanChiTietId) && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //    {
                //        soTienVoucherDaMienGiam += mienGiamChiPhi.SoTien;
                //    }
                //}

                //decimal soTienVoucherConMienGiam = cauHinhVoucher.SoTienToiDaDuocMienGiam - soTienVoucherDaMienGiam;
                //if (thongTinMienGiamVoucherVo.LoaiVoucher == Enums.LoaiVoucher.DungNhieuLan)
                //{
                //    decimal soTienTrenTheVoucher = 0;
                //    foreach (var theVoucherYeuCauTiepNhan in yeuCauTiepNhan.TheVoucherYeuCauTiepNhans.OrderBy(o => o.Id))
                //    {
                //        //TODO: need update MiemGiam
                //        //soTienTrenTheVoucher += theVoucherYeuCauTiepNhan.TheVoucher.TriGia.GetValueOrDefault() - theVoucherYeuCauTiepNhan.TheVoucher.TongGiaTriDaSuDung.GetValueOrDefault();
                //    }

                //    thongTinMienGiamVoucherVo.SoTienVoucherMiemGiam = soTienTrenTheVoucher < soTienVoucherConMienGiam ? soTienTrenTheVoucher : soTienVoucherConMienGiam;
                //}
                //else
                //{
                //    thongTinMienGiamVoucherVo.SoTienVoucherMiemGiam = soTienVoucherConMienGiam;
                //    foreach (var theVoucherYeuCauTiepNhan in yeuCauTiepNhan.TheVoucherYeuCauTiepNhans)
                //    {
                //        var voucher = _voucherRepository.GetById(theVoucherYeuCauTiepNhan.TheVoucher.VoucherId, x => x.Include(o => o.VoucherChiTietMienGiams));

                //        foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                //        {
                //            var uuDaiDichVuKhamBenh = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.DichVuKhamBenhBenhVienId == yeuCauKhamBenh.DichVuKhamBenhBenhVienId);
                //            if (uuDaiDichVuKhamBenh != null)
                //            {
                //                var dichVuMiemGiamTheoTiLe = thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.FirstOrDefault(o => o.DichVuId == yeuCauKhamBenh.Id && o.LoaiNhom == NhomChiPhiKhamChuaBenh.DichVuKhamBenh);
                //                if (dichVuMiemGiamTheoTiLe != null)
                //                {
                //                    //TODO: need update MiemGiam
                //                    //dichVuMiemGiamTheoTiLe.TiLe += uuDaiDichVuKhamBenh.TiLeMienGiam;
                //                }
                //                else
                //                {
                //                    thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.Add(new DichVuMiemGiamTheoTiLe
                //                    {
                //                        LoaiNhom = NhomChiPhiKhamChuaBenh.DichVuKhamBenh,
                //                        DichVuId = yeuCauKhamBenh.Id,
                //                        //TODO: need update MiemGiam
                //                        //TiLe = uuDaiDichVuKhamBenh.TiLeMienGiam
                //                    });
                //                }
                //            }
                //        }

                //        foreach (var yeuCau in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                //        {
                //            var uuDaiDichVu = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.DichVuKyThuatBenhVienId == yeuCau.DichVuKyThuatBenhVienId);
                //            if (uuDaiDichVu != null)
                //            {
                //                var dichVuMiemGiamTheoTiLe = thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.FirstOrDefault(o => o.DichVuId == yeuCau.Id && o.LoaiNhom == NhomChiPhiKhamChuaBenh.DichVuKyThuat);
                //                if (dichVuMiemGiamTheoTiLe != null)
                //                {
                //                    //TODO: need update MiemGiam
                //                    //dichVuMiemGiamTheoTiLe.TiLe += uuDaiDichVu.TiLeMienGiam;
                //                }
                //                else
                //                {
                //                    thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.Add(new DichVuMiemGiamTheoTiLe
                //                    {
                //                        LoaiNhom = NhomChiPhiKhamChuaBenh.DichVuKyThuat,
                //                        DichVuId = yeuCau.Id,
                //                        //TODO: need update MiemGiam
                //                        //TiLe = uuDaiDichVu.TiLeMienGiam
                //                    });
                //                }
                //            }
                //        }
                //        //TODO: need update MiemGiam
                //        //foreach (var yeuCau in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                //        //{
                //        //    var uuDaiDichVu = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.DichVuGiuongBenhVienId == yeuCau.DichVuGiuongBenhVienId);
                //        //    if (uuDaiDichVu != null)
                //        //    {
                //        //        var dichVuMiemGiamTheoTiLe = thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.FirstOrDefault(o => o.DichVuId == yeuCau.Id && o.LoaiNhom == NhomChiPhiKhamChuaBenh.DichVuGiuong);
                //        //        if (dichVuMiemGiamTheoTiLe != null)
                //        //        {
                //        //            dichVuMiemGiamTheoTiLe.TiLe += uuDaiDichVu.TiLeMienGiam;
                //        //        }
                //        //        else
                //        //        {
                //        //            thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.Add(new DichVuMiemGiamTheoTiLe
                //        //            {
                //        //                LoaiNhom = NhomChiPhiKhamChuaBenh.DichVuGiuong,
                //        //                DichVuId = yeuCau.Id,
                //        //                TiLe = uuDaiDichVu.TiLeMienGiam
                //        //            });
                //        //        }
                //        //    }
                //        //}

                //        //foreach (var yeuCau in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                //        //{
                //        //    var uuDaiDichVu = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.DuocPhamBenhVienId == yeuCau.DuocPhamBenhVienId);
                //        //    if (uuDaiDichVu != null)
                //        //    {
                //        //        var dichVuMiemGiamTheoTiLe = thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.FirstOrDefault(o => o.DichVuId == yeuCau.Id && o.LoaiNhom == NhomChiPhiKhamChuaBenh.DuocPham);
                //        //        if (dichVuMiemGiamTheoTiLe != null)
                //        //        {
                //        //            dichVuMiemGiamTheoTiLe.TiLe += uuDaiDichVu.TiLeMienGiam;
                //        //        }
                //        //        else
                //        //        {
                //        //            thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.Add(new DichVuMiemGiamTheoTiLe
                //        //            {
                //        //                LoaiNhom = NhomChiPhiKhamChuaBenh.DuocPham,
                //        //                DichVuId = yeuCau.Id,
                //        //                TiLe = uuDaiDichVu.TiLeMienGiam
                //        //            });
                //        //        }
                //        //    }
                //        //}

                //        //foreach (var yeuCau in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                //        //{
                //        //    var uuDaiDichVu = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.VatTuBenhVienId == yeuCau.VatTuBenhVienId);
                //        //    if (uuDaiDichVu != null)
                //        //    {
                //        //        var dichVuMiemGiamTheoTiLe = thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.FirstOrDefault(o => o.DichVuId == yeuCau.Id && o.LoaiNhom == NhomChiPhiKhamChuaBenh.VatTuTieuHao);
                //        //        if (dichVuMiemGiamTheoTiLe != null)
                //        //        {
                //        //            dichVuMiemGiamTheoTiLe.TiLe += uuDaiDichVu.TiLeMienGiam;
                //        //        }
                //        //        else
                //        //        {
                //        //            thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.Add(new DichVuMiemGiamTheoTiLe
                //        //            {
                //        //                LoaiNhom = NhomChiPhiKhamChuaBenh.VatTuTieuHao,
                //        //                DichVuId = yeuCau.Id,
                //        //                TiLe = uuDaiDichVu.TiLeMienGiam
                //        //            });
                //        //        }
                //        //    }
                //        //}
                //        //TODO: need update goi dv
                //        //foreach (var yeuCau in yeuCauTiepNhan.YeuCauGoiDichVus.Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
                //        //{
                //        //    var uuDaiDichVu = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.GoiDichVuId == yeuCau.GoiDichVuId);
                //        //    if (uuDaiDichVu != null)
                //        //    {
                //        //        var dichVuMiemGiamTheoTiLe = thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.FirstOrDefault(o => o.DichVuId == yeuCau.Id && o.LoaiNhom == NhomChiPhiKhamChuaBenh.GoiDichVu);
                //        //        if (dichVuMiemGiamTheoTiLe != null)
                //        //        {
                //        //            dichVuMiemGiamTheoTiLe.TiLe += uuDaiDichVu.TiLeMienGiam;
                //        //        }
                //        //        else
                //        //        {
                //        //            thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.Add(new DichVuMiemGiamTheoTiLe
                //        //            {
                //        //                LoaiNhom = NhomChiPhiKhamChuaBenh.GoiDichVu,
                //        //                DichVuId = yeuCau.Id,
                //        //                TiLe = uuDaiDichVu.TiLeMienGiam
                //        //            });
                //        //        }
                //        //    }
                //        //}
                //        //TODO: need update MiemGiam
                //        //foreach (var yeuCau in yeuCauTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)).SelectMany(o => o.DonThuocThanhToanChiTiets))
                //        //{
                //        //    var uuDaiDichVu = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.DuocPhamBenhVienId == yeuCau.DuocPhamId);
                //        //    if (uuDaiDichVu != null)
                //        //    {
                //        //        var dichVuMiemGiamTheoTiLe = thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.FirstOrDefault(o => o.DichVuId == yeuCau.Id && o.LoaiNhom == NhomChiPhiKhamChuaBenh.ToaThuoc);
                //        //        if (dichVuMiemGiamTheoTiLe != null)
                //        //        {
                //        //            dichVuMiemGiamTheoTiLe.TiLe += uuDaiDichVu.TiLeMienGiam;
                //        //        }
                //        //        else
                //        //        {
                //        //            thongTinMienGiamVoucherVo.DichVuMiemGiamTheoTiLes.Add(new DichVuMiemGiamTheoTiLe
                //        //            {
                //        //                LoaiNhom = NhomChiPhiKhamChuaBenh.ToaThuoc,
                //        //                DichVuId = yeuCau.Id,
                //        //                TiLe = uuDaiDichVu.TiLeMienGiam
                //        //            });
                //        //        }
                //        //    }
                //        //}
                //        //foreach (var voucherChiTietMienGiam in voucher.VoucherChiTietMienGiams.Where(o => o.DuocPhamBenhVienId != null))
                //        //{
                //        //    var duocPhamMienGiamTheoTiLe = thongTinMienGiamVoucherVo.DuocPhamMienGiamTheoTiLes.FirstOrDefault(o => o.DuocPhamId == voucherChiTietMienGiam.DuocPhamBenhVienId);
                //        //    if (duocPhamMienGiamTheoTiLe != null)
                //        //    {
                //        //        duocPhamMienGiamTheoTiLe.TiLe += voucherChiTietMienGiam.TiLeMienGiam;
                //        //    }
                //        //    else
                //        //    {
                //        //        thongTinMienGiamVoucherVo.DuocPhamMienGiamTheoTiLes.Add(new DuocPhamMienGiamTheoTiLe
                //        //        {
                //        //            DuocPhamId = voucherChiTietMienGiam.DuocPhamBenhVienId.Value,
                //        //            TiLe = voucherChiTietMienGiam.TiLeMienGiam
                //        //        });
                //        //    }
                //        //}
                //    }
                //}

                thongTinMienGiamVo.ThongTinMienGiamVoucherVo = thongTinMienGiamVoucherVo;
            }
            return thongTinMienGiamVo;
        }
        #region Thanh toán đơn thuốc 
        public async Task<KetQuaThemThanhToanDonThuocVo> ThanhToanDonThuoc(ThongTinDonThuocVO thongTinDonThuoc, bool xuatThuoc = false)
        {
            var userId = _userAgentHelper.GetCurrentUserId();
            var userThuNgan = _userRepo.GetById(_userAgentHelper.GetCurrentUserId());
            var maTaiKhoan = userId.ToString();
            if (userThuNgan.Email != null && userThuNgan.Email.IndexOf("@") > 0)
            {
                maTaiKhoan = userThuNgan.Email.Substring(0, userThuNgan.Email.IndexOf("@") > 10 ? 10 : userThuNgan.Email.IndexOf("@"));
            }

            var ycTiepNhan = await _yeuCauTiepNhanRepo.GetByIdAsync(thongTinDonThuoc.Id,
                x => x.Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(g => g.DonThuocThanhToanChiTiets).ThenInclude(g => g.YeuCauKhamBenhDonThuocChiTiet).ThenInclude(g => g.YeuCauKhamBenhDonThuoc)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                    .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(g => g.DonVTYTThanhToanChiTiets).ThenInclude(g => g.YeuCauKhamBenhDonVTYTChiTiet).ThenInclude(g => g.YeuCauKhamBenhDonVTYT)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.CongTyBaoHiemTuNhanCongNos)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.MienGiamChiPhis)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.TaiKhoanBenhNhanChis)
                    .Include(o => o.DonVTYTThanhToans).ThenInclude(o => o.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTu)
                    .Include(o => o.BenhNhan).ThenInclude(o => o.TaiKhoanBenhNhan));
            //kiem tra thong tin thanh toan truoc khi thanh toan
            var ycDonThuocThanhToanChiTietExcept = thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew != true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien)
                .Select(o => o.Id).Except(ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                    .SelectMany(o => o.DonThuocThanhToanChiTiets).Select(o => o.Id));

            var ycDonVTYTThanhToanChiTietExcept = thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew != true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien)
                .Select(o => o.Id).Except(ycTiepNhan.DonVTYTThanhToans.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                    .SelectMany(o => o.DonVTYTThanhToanChiTiets).Select(o => o.Id));

            if (ycDonThuocThanhToanChiTietExcept.Any() || ycDonVTYTThanhToanChiTietExcept.Any())
            {
                return new KetQuaThemThanhToanDonThuocVo
                {
                    ThanhCong = false,
                    Error = "Thông tin dịch vụ thanh toan không hợp lệ, vui lòng tải lại trang"
                };
            }
            foreach (var chiPhiDonThuocVo in thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.CheckedDefault))
            {
                var soTienTruocMienGiam = chiPhiDonThuocVo.ThanhTien - chiPhiDonThuocVo.TongCongNo;

                decimal soTienMienGiamTheoDv = 0;

                foreach (var mienGiamTheoTiLe in chiPhiDonThuocVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe))
                {
                    mienGiamTheoTiLe.SoTien = Math.Round((soTienTruocMienGiam * mienGiamTheoTiLe.TiLe.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                    soTienMienGiamTheoDv += mienGiamTheoTiLe.SoTien;
                }
                foreach (var mienGiamTheoSoTien in chiPhiDonThuocVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien))
                {
                    soTienMienGiamTheoDv += mienGiamTheoSoTien.SoTien;
                }
                if (soTienMienGiamTheoDv > soTienTruocMienGiam)
                {
                    return new KetQuaThemThanhToanDonThuocVo
                    {
                        ThanhCong = false,
                        Error = "Thông tin dịch vụ thanh toan không hợp lệ, vui lòng tải lại trang"
                    };
                }
                chiPhiDonThuocVo.SoTienMG = soTienMienGiamTheoDv;
            }
            //var thongTinMienGiam = await GetThongTinMienGiam(thongTinDonThuoc.Id);
            var duocPhamVuotTonKho = new List<DuocPhamVuotTonKho>();

            var thuPhi = new TaiKhoanBenhNhanThu
            {
                LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi,
                LoaiNoiThu = LoaiNoiThu.NhaThuoc,
                TienMat = thongTinDonThuoc.TienMat,
                ChuyenKhoan = thongTinDonThuoc.ChuyenKhoan,
                POS = thongTinDonThuoc.POS,
                CongNo = thongTinDonThuoc.SoTienCongNo,
                NoiDungThu = thongTinDonThuoc.NoiDungThu,
                GhiChu = thongTinDonThuoc.GhiChu,
                NgayThu = thongTinDonThuoc.NgayThu,
                NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
            };

            // update 

            //cap nhat duoc pham da ke
            foreach (var donThuocThanhToanChiTiet in ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan).SelectMany(o => o.DonThuocThanhToanChiTiets))
            {
                if (thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew != true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select(o => o.Id).Contains(donThuocThanhToanChiTiet.Id))
                {
                    //capnhat
                    var donThuocChiTietCanMua = thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew != true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).First(o => o.Id == donThuocThanhToanChiTiet.Id);
                    if (!donThuocThanhToanChiTiet.SoLuong.AlmostEqual(donThuocChiTietCanMua.SoLuongMua))
                    {
                        if (donThuocChiTietCanMua.SoLuongMua < donThuocThanhToanChiTiet.SoLuong)
                        {
                            var soLuongGiam = donThuocThanhToanChiTiet.SoLuong - donThuocChiTietCanMua.SoLuongMua;
                            donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= soLuongGiam;
                            donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat = donThuocChiTietCanMua.SoLuongMua;
                            donThuocThanhToanChiTiet.SoLuong = donThuocChiTietCanMua.SoLuongMua;
                        }
                        else
                        {
                            var soLuongTang = donThuocChiTietCanMua.SoLuongMua - donThuocThanhToanChiTiet.SoLuong;
                            var soLuongTon = donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongNhap - donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat;

                            if (soLuongTon < soLuongTang)
                            {
                                //tra ve loi
                                duocPhamVuotTonKho.Add(new DuocPhamVuotTonKho { Stt = donThuocChiTietCanMua.STT.GetValueOrDefault(), SoLuongTonKho = soLuongTon + donThuocThanhToanChiTiet.SoLuong });
                            }
                            else
                            {
                                donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongTang;
                                donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat = donThuocChiTietCanMua.SoLuongMua;
                                donThuocThanhToanChiTiet.SoLuong = donThuocChiTietCanMua.SoLuongMua;
                            }
                        }
                    }
                    //them cong no
                    foreach (var ycCongTyBaoHiemTuNhanCongNo in donThuocThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null))
                    {
                        ycCongTyBaoHiemTuNhanCongNo.WillDelete = true;
                    }
                    if (donThuocChiTietCanMua.TongCongNo > 0)
                    {
                        foreach (var chiphiDanhSachCongNoChoThu in donThuocChiTietCanMua.DanhSachCongNoChoThus)
                        {
                            donThuocThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos.Add(new CongTyBaoHiemTuNhanCongNo
                            {
                                TaiKhoanBenhNhanThu = thuPhi,
                                CongTyBaoHiemTuNhanId = chiphiDanhSachCongNoChoThu.CongNoId,
                                SoTien = chiphiDanhSachCongNoChoThu.SoTienCongNoChoThu
                            });
                        }
                    }
                    donThuocThanhToanChiTiet.SoTienBaoHiemTuNhanChiTra = donThuocChiTietCanMua.TongCongNo;

                    //mien giam them theo ti le
                    foreach (var mienGiam in donThuocThanhToanChiTiet.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null))
                    {
                        mienGiam.WillDelete = true;
                    }
                    var mienGiamThemTiLe = donThuocChiTietCanMua.DanhSachMienGiamVos.FirstOrDefault(o => o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem && o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe);
                    if (mienGiamThemTiLe != null && mienGiamThemTiLe.TiLe.GetValueOrDefault() != 0)
                    {
                        donThuocThanhToanChiTiet.MienGiamChiPhis.Add(new MienGiamChiPhi
                        {
                            TaiKhoanBenhNhanThu = thuPhi,
                            YeuCauTiepNhanId = ycTiepNhan.Id,
                            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                            TiLe = mienGiamThemTiLe.TiLe,
                            SoTien = mienGiamThemTiLe.SoTien
                        });
                    }

                    //mien giam them theo so tien
                    var mienGiamThemSoTien = donThuocChiTietCanMua.DanhSachMienGiamVos.FirstOrDefault(o => o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem && o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien);
                    if (mienGiamThemSoTien != null && !mienGiamThemSoTien.SoTien.AlmostEqual(0))
                    {
                        donThuocThanhToanChiTiet.MienGiamChiPhis.Add(new MienGiamChiPhi
                        {
                            TaiKhoanBenhNhanThu = thuPhi,
                            YeuCauTiepNhanId = ycTiepNhan.Id,
                            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                            SoTien = mienGiamThemSoTien.SoTien
                        });
                    }
                    donThuocThanhToanChiTiet.SoTienMienGiam = donThuocChiTietCanMua.SoTienMG;

                    if ((mienGiamThemTiLe?.SoTien ?? 0).AlmostEqual(0) && (mienGiamThemSoTien?.SoTien ?? 0).AlmostEqual(0))
                    {
                        donThuocThanhToanChiTiet.GhiChuMienGiamThem = null;
                    }
                    else
                    {
                        donThuocThanhToanChiTiet.GhiChuMienGiamThem = donThuocChiTietCanMua.GhiChuMienGiamThem;
                    }
                }
                else
                {
                    //xoa
                    foreach (var congNo in donThuocThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                    {
                        congNo.WillDelete = true;
                    }
                    foreach (var mienGiam in donThuocThanhToanChiTiet.MienGiamChiPhis)
                    {
                        mienGiam.WillDelete = true;
                    }
                    foreach (var taiKhoanBenhNhanChi in donThuocThanhToanChiTiet.TaiKhoanBenhNhanChis)
                    {
                        taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietId = null;
                    }
                    donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                    donThuocThanhToanChiTiet.WillDelete = true;
                    donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                    donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                    if (donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;
                }
            }
            //cap nhat vat tu da ke
            foreach (var donVTYTThanhToanChiTiet in ycTiepNhan.DonVTYTThanhToans.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan).SelectMany(o => o.DonVTYTThanhToanChiTiets))
            {
                if (thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew != true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien).Select(o => o.Id).Contains(donVTYTThanhToanChiTiet.Id))
                {
                    //capnhat
                    var donVTYTChiTietCanMua = thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew != true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien).First(o => o.Id == donVTYTThanhToanChiTiet.Id);
                    if (!donVTYTThanhToanChiTiet.SoLuong.AlmostEqual(donVTYTChiTietCanMua.SoLuongMua))
                    {
                        if (donVTYTChiTietCanMua.SoLuongMua < donVTYTThanhToanChiTiet.SoLuong)
                        {
                            var soLuongGiam = donVTYTThanhToanChiTiet.SoLuong - donVTYTChiTietCanMua.SoLuongMua;
                            donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= soLuongGiam;
                            donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat = donVTYTChiTietCanMua.SoLuongMua;
                            donVTYTThanhToanChiTiet.SoLuong = donVTYTChiTietCanMua.SoLuongMua;
                        }
                        else
                        {
                            var soLuongTang = donVTYTChiTietCanMua.SoLuongMua - donVTYTThanhToanChiTiet.SoLuong;
                            var soLuongTon = donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongNhap - donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat;

                            if (soLuongTon < soLuongTang)
                            {
                                //tra ve loi
                                duocPhamVuotTonKho.Add(new DuocPhamVuotTonKho { Stt = donVTYTChiTietCanMua.STT.GetValueOrDefault(), SoLuongTonKho = soLuongTon + donVTYTThanhToanChiTiet.SoLuong });
                            }
                            else
                            {
                                donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat += soLuongTang;
                                donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat = donVTYTChiTietCanMua.SoLuongMua;
                                donVTYTThanhToanChiTiet.SoLuong = donVTYTChiTietCanMua.SoLuongMua;
                            }
                        }
                    }
                    //them cong no
                    foreach (var ycCongTyBaoHiemTuNhanCongNo in donVTYTThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null))
                    {
                        ycCongTyBaoHiemTuNhanCongNo.WillDelete = true;
                    }
                    if (donVTYTChiTietCanMua.TongCongNo > 0)
                    {
                        foreach (var chiphiDanhSachCongNoChoThu in donVTYTChiTietCanMua.DanhSachCongNoChoThus)
                        {
                            donVTYTThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos.Add(new CongTyBaoHiemTuNhanCongNo
                            {
                                TaiKhoanBenhNhanThu = thuPhi,
                                CongTyBaoHiemTuNhanId = chiphiDanhSachCongNoChoThu.CongNoId,
                                SoTien = chiphiDanhSachCongNoChoThu.SoTienCongNoChoThu
                            });
                        }
                    }
                    donVTYTThanhToanChiTiet.SoTienBaoHiemTuNhanChiTra = donVTYTChiTietCanMua.TongCongNo;

                    foreach (var mienGiam in donVTYTThanhToanChiTiet.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null))
                    {
                        mienGiam.WillDelete = true;
                    }
                    //mien giam them theo ti le
                    var mienGiamThemTiLe = donVTYTChiTietCanMua.DanhSachMienGiamVos.FirstOrDefault(o => o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem && o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe);
                    if (mienGiamThemTiLe != null && mienGiamThemTiLe.TiLe.GetValueOrDefault() != 0)
                    {
                        donVTYTThanhToanChiTiet.MienGiamChiPhis.Add(new MienGiamChiPhi
                        {
                            TaiKhoanBenhNhanThu = thuPhi,
                            YeuCauTiepNhanId = ycTiepNhan.Id,
                            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                            TiLe = mienGiamThemTiLe.TiLe,
                            SoTien = mienGiamThemTiLe.SoTien
                        });
                    }

                    //mien giam them theo so tien
                    var mienGiamThemSoTien = donVTYTChiTietCanMua.DanhSachMienGiamVos.FirstOrDefault(o => o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem && o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien);
                    if (mienGiamThemSoTien != null && !mienGiamThemSoTien.SoTien.AlmostEqual(0))
                    {
                        donVTYTThanhToanChiTiet.MienGiamChiPhis.Add(new MienGiamChiPhi
                        {
                            TaiKhoanBenhNhanThu = thuPhi,
                            YeuCauTiepNhanId = ycTiepNhan.Id,
                            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                            SoTien = mienGiamThemSoTien.SoTien
                        });
                    }
                    donVTYTThanhToanChiTiet.SoTienMienGiam = donVTYTChiTietCanMua.SoTienMG;

                    if ((mienGiamThemTiLe?.SoTien ?? 0).AlmostEqual(0) && (mienGiamThemSoTien?.SoTien ?? 0).AlmostEqual(0))
                    {
                        donVTYTThanhToanChiTiet.GhiChuMienGiamThem = null;
                    }
                    else
                    {
                        donVTYTThanhToanChiTiet.GhiChuMienGiamThem = donVTYTChiTietCanMua.GhiChuMienGiamThem;
                    }
                }
                else
                {
                    //xoa
                    foreach (var congNo in donVTYTThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos)
                    {
                        congNo.WillDelete = true;
                    }
                    foreach (var mienGiam in donVTYTThanhToanChiTiet.MienGiamChiPhis)
                    {
                        mienGiam.WillDelete = true;
                    }
                    foreach (var taiKhoanBenhNhanChi in donVTYTThanhToanChiTiet.TaiKhoanBenhNhanChis)
                    {
                        taiKhoanBenhNhanChi.DonVTYTThanhToanChiTietId = null;
                    }
                    donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                    donVTYTThanhToanChiTiet.WillDelete = true;
                    donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                    donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                    if (donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu != null)
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.WillDelete = true;
                }
            }

            //them duoc pham moi
            var danhSachDonThuocMoi = thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew == true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).ToList();
            if (danhSachDonThuocMoi.Count > 0)
            {
                var donThuocThanhToan = new DonThuocThanhToan
                {
                    LoaiDonThuoc = EnumLoaiDonThuoc.ThuocKhongBHYT,
                    BenhNhanId = ycTiepNhan.BenhNhanId,
                    TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                };
                ycTiepNhan.DonThuocThanhToans.Add(donThuocThanhToan);

                foreach (var thongTinDuocPhamMoi in thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew == true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien))
                {
                    var duocPham = _duocPhamnRepo.GetById(thongTinDuocPhamMoi.DuocPhamId,
                    x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets)
                        .ThenInclude(nkct => nkct.HopDongThauDuocPhams)
                        .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets)
                        .ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

                    var nhapKhoDuocPhamChiTiet =
                        duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.First(o =>
                            o.Id == thongTinDuocPhamMoi.NhapKhoDuocPhamChiTietId);
                    var soLuongTon = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                    if (soLuongTon < thongTinDuocPhamMoi.SoLuongMua)
                    {
                        duocPhamVuotTonKho.Add(new DuocPhamVuotTonKho
                        {
                            Stt = thongTinDuocPhamMoi.STT.GetValueOrDefault(),
                            SoLuongTonKho = soLuongTon
                        });
                    }
                    else
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat += thongTinDuocPhamMoi.SoLuongMua;
                        var xuatKhoChiTiet = new XuatKhoDuocPhamChiTietViTri
                        {
                            SoLuongXuat = thongTinDuocPhamMoi.SoLuongMua,
                            NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                            XuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                            {
                                DuocPhamBenhVien = duocPham.DuocPhamBenhVien,
                            }
                        };

                        var dtttChiTiet = new DonThuocThanhToanChiTiet
                        {
                            DuocPhamId = duocPham.Id,
                            XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet,
                            Ten = duocPham.Ten,
                            TenTiengAnh = duocPham.TenTiengAnh,
                            SoDangKy = duocPham.SoDangKy,
                            STTHoatChat = duocPham.STTHoatChat,
                            NhomChiPhi = Enums.EnumDanhMucNhomTheoChiPhi.ThuocThanhToanTheoTyLe,
                            MaHoatChat = duocPham.MaHoatChat,
                            HoatChat = duocPham.HoatChat,
                            LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat,
                            NhaSanXuat = duocPham.NhaSanXuat,
                            NuocSanXuat = duocPham.NuocSanXuat,
                            DuongDungId = duocPham.DuongDungId,
                            HamLuong = duocPham.HamLuong,
                            QuyCach = duocPham.QuyCach,
                            TieuChuan = duocPham.TieuChuan,
                            DangBaoChe = duocPham.DangBaoChe,
                            DonViTinhId = duocPham.DonViTinhId,
                            HuongDan = duocPham.HuongDan,
                            MoTa = duocPham.MoTa,
                            ChiDinh = duocPham.ChiDinh,
                            ChongChiDinh = duocPham.ChongChiDinh,
                            LieuLuongCachDung = duocPham.LieuLuongCachDung,
                            TacDungPhu = duocPham.TacDungPhu,
                            ChuYDePhong = duocPham.ChuYDePhong,
                            HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                            NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                            SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                            SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                            LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                            LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                            NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                            GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                            NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                            DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                            PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                            VAT = nhapKhoDuocPhamChiTiet.VAT,
                            DonGiaBan = CalculateHelper.TinhDonGiaBan(nhapKhoDuocPhamChiTiet.DonGiaNhap, nhapKhoDuocPhamChiTiet.TiLeTheoThapGia, nhapKhoDuocPhamChiTiet.VAT, true, true, nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho),
                            GiaBan = CalculateHelper.TinhDonGiaBan(nhapKhoDuocPhamChiTiet.DonGiaNhap, nhapKhoDuocPhamChiTiet.TiLeTheoThapGia, nhapKhoDuocPhamChiTiet.VAT, true, true, nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho) * (decimal)thongTinDuocPhamMoi.SoLuongMua,
                            SoLuong = thongTinDuocPhamMoi.SoLuongMua,
                            SoTienBenhNhanDaChi = 0,
                            DuocHuongBaoHiem = false
                        };

                        //them cong no
                        if (thongTinDuocPhamMoi.TongCongNo > 0)
                        {
                            foreach (var chiphiDanhSachCongNoChoThu in thongTinDuocPhamMoi.DanhSachCongNoChoThus)
                            {
                                dtttChiTiet.CongTyBaoHiemTuNhanCongNos.Add(new CongTyBaoHiemTuNhanCongNo
                                {
                                    TaiKhoanBenhNhanThu = thuPhi,
                                    CongTyBaoHiemTuNhanId = chiphiDanhSachCongNoChoThu.CongNoId,
                                    SoTien = chiphiDanhSachCongNoChoThu.SoTienCongNoChoThu
                                });
                            }
                        }
                        dtttChiTiet.SoTienBaoHiemTuNhanChiTra = thongTinDuocPhamMoi.TongCongNo;

                        //mien giam them theo ti le
                        var mienGiamThemTiLe = thongTinDuocPhamMoi.DanhSachMienGiamVos.FirstOrDefault(o => o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem && o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe);
                        if (mienGiamThemTiLe != null && mienGiamThemTiLe.TiLe.GetValueOrDefault() != 0)
                        {
                            dtttChiTiet.MienGiamChiPhis.Add(new MienGiamChiPhi
                            {
                                TaiKhoanBenhNhanThu = thuPhi,
                                YeuCauTiepNhanId = ycTiepNhan.Id,
                                LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                                TiLe = mienGiamThemTiLe.TiLe,
                                SoTien = mienGiamThemTiLe.SoTien
                            });
                        }

                        //mien giam them theo so tien
                        var mienGiamThemSoTien = thongTinDuocPhamMoi.DanhSachMienGiamVos.FirstOrDefault(o => o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem && o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien);
                        if (mienGiamThemSoTien != null && !mienGiamThemSoTien.SoTien.AlmostEqual(0))
                        {
                            dtttChiTiet.MienGiamChiPhis.Add(new MienGiamChiPhi
                            {
                                TaiKhoanBenhNhanThu = thuPhi,
                                YeuCauTiepNhanId = ycTiepNhan.Id,
                                LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                SoTien = mienGiamThemSoTien.SoTien
                            });
                        }
                        dtttChiTiet.SoTienMienGiam = thongTinDuocPhamMoi.SoTienMG;

                        if ((mienGiamThemTiLe?.SoTien ?? 0).AlmostEqual(0) && (mienGiamThemSoTien?.SoTien ?? 0).AlmostEqual(0))
                        {
                            dtttChiTiet.GhiChuMienGiamThem = null;
                        }
                        else
                        {
                            dtttChiTiet.GhiChuMienGiamThem = thongTinDuocPhamMoi.GhiChuMienGiamThem;
                        }

                        donThuocThanhToan.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                    }


                }
            }

            //them vat tu moi
            var danhSachDonVTYTMoi = thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew == true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien).ToList();
            if (danhSachDonVTYTMoi.Count > 0)
            {
                // vật tư
                var donVTYTThanhToan = new DonVTYTThanhToan
                {
                    BenhNhanId = ycTiepNhan.BenhNhanId,
                    TrangThai = Enums.TrangThaiDonVTYTThanhToan.ChuaXuatVTYT,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                };
                ycTiepNhan.DonVTYTThanhToans.Add(donVTYTThanhToan);

                foreach (var thongTinVatTuMoi in thongTinDonThuoc.DanhSachDonThuocs.Where(o => o.isNew == true && o.CheckedDefault && o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien))
                {
                    var vatTuBenhVien = _vatTuBenhVienRepository.GetById(thongTinVatTuMoi.DuocPhamId,
                    x => x.Include(vt => vt.VatTus)
                        .Include(vt => vt.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.HopDongThauVatTu)
                        .Include(vt => vt.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.NhapKhoVatTu).ThenInclude(nk => nk.Kho));

                    var nhapKhoVatTuChiTiet = vatTuBenhVien.NhapKhoVatTuChiTiets.First(o => o.Id == thongTinVatTuMoi.NhapKhoDuocPhamChiTietId);
                    var soLuongTon = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;
                    if (soLuongTon < thongTinVatTuMoi.SoLuongMua)
                    {
                        duocPhamVuotTonKho.Add(new DuocPhamVuotTonKho
                        {
                            Stt = thongTinVatTuMoi.STT.GetValueOrDefault(),
                            SoLuongTonKho = soLuongTon
                        });
                    }
                    else
                    {
                        nhapKhoVatTuChiTiet.SoLuongDaXuat += thongTinVatTuMoi.SoLuongMua;
                        var xuatKhoChiTiet = new XuatKhoVatTuChiTietViTri
                        {
                            SoLuongXuat = thongTinVatTuMoi.SoLuongMua,
                            NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                            XuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                            {
                                VatTuBenhVien = vatTuBenhVien
                            }
                        };

                        var donVTYTThanhToanChiTiet = new DonVTYTThanhToanChiTiet
                        {
                            VatTuBenhVienId = vatTuBenhVien.Id,
                            XuatKhoVatTuChiTietViTri = xuatKhoChiTiet,
                            Ten = vatTuBenhVien.VatTus.Ten,
                            Ma = vatTuBenhVien.VatTus.Ma,
                            NhomVatTuId = vatTuBenhVien.VatTus.NhomVatTuId,
                            DonViTinh = vatTuBenhVien.VatTus.DonViTinh,
                            NhaSanXuat = vatTuBenhVien.VatTus.NhaSanXuat,
                            NuocSanXuat = vatTuBenhVien.VatTus.NuocSanXuat,
                            QuyCach = vatTuBenhVien.VatTus.QuyCach,
                            MoTa = vatTuBenhVien.VatTus.MoTa,
                            DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTiet.VAT,
                            HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                            NhaThauId = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhaThauId,
                            SoHopDongThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoHopDong,
                            SoQuyetDinhThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoQuyetDinh,
                            LoaiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.LoaiThau,
                            NhomThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhomThau,
                            GoiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.GoiThau,
                            NamThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.Nam,
                            SoTienBenhNhanDaChi = 0,
                            SoLuong = thongTinVatTuMoi.SoLuongMua,
                            DonGiaBan = CalculateHelper.TinhDonGiaBan(nhapKhoVatTuChiTiet.DonGiaNhap, nhapKhoVatTuChiTiet.TiLeTheoThapGia, nhapKhoVatTuChiTiet.VAT, false, true, nhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho),
                            GiaBan = CalculateHelper.TinhDonGiaBan(nhapKhoVatTuChiTiet.DonGiaNhap, nhapKhoVatTuChiTiet.TiLeTheoThapGia, nhapKhoVatTuChiTiet.VAT, false, true, nhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho) * (decimal)thongTinVatTuMoi.SoLuongMua
                        };

                        //them cong no
                        if (thongTinVatTuMoi.TongCongNo > 0)
                        {
                            foreach (var chiphiDanhSachCongNoChoThu in thongTinVatTuMoi.DanhSachCongNoChoThus)
                            {
                                donVTYTThanhToanChiTiet.CongTyBaoHiemTuNhanCongNos.Add(new CongTyBaoHiemTuNhanCongNo
                                {
                                    TaiKhoanBenhNhanThu = thuPhi,
                                    CongTyBaoHiemTuNhanId = chiphiDanhSachCongNoChoThu.CongNoId,
                                    SoTien = chiphiDanhSachCongNoChoThu.SoTienCongNoChoThu
                                });
                            }
                        }
                        donVTYTThanhToanChiTiet.SoTienBaoHiemTuNhanChiTra = thongTinVatTuMoi.TongCongNo;

                        //mien giam them theo ti le
                        var mienGiamThemTiLe = thongTinVatTuMoi.DanhSachMienGiamVos.FirstOrDefault(o => o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem && o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe);
                        if (mienGiamThemTiLe != null && mienGiamThemTiLe.TiLe.GetValueOrDefault() != 0)
                        {
                            donVTYTThanhToanChiTiet.MienGiamChiPhis.Add(new MienGiamChiPhi
                            {
                                TaiKhoanBenhNhanThu = thuPhi,
                                YeuCauTiepNhanId = ycTiepNhan.Id,
                                LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                                TiLe = mienGiamThemTiLe.TiLe,
                                SoTien = mienGiamThemTiLe.SoTien
                            });
                        }

                        //mien giam them theo so tien
                        var mienGiamThemSoTien = thongTinVatTuMoi.DanhSachMienGiamVos.FirstOrDefault(o => o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem && o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien);
                        if (mienGiamThemSoTien != null && !mienGiamThemSoTien.SoTien.AlmostEqual(0))
                        {
                            donVTYTThanhToanChiTiet.MienGiamChiPhis.Add(new MienGiamChiPhi
                            {
                                TaiKhoanBenhNhanThu = thuPhi,
                                YeuCauTiepNhanId = ycTiepNhan.Id,
                                LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                SoTien = mienGiamThemSoTien.SoTien
                            });
                        }
                        donVTYTThanhToanChiTiet.SoTienMienGiam = thongTinVatTuMoi.SoTienMG;

                        if ((mienGiamThemTiLe?.SoTien ?? 0).AlmostEqual(0) && (mienGiamThemSoTien?.SoTien ?? 0).AlmostEqual(0))
                        {
                            donVTYTThanhToanChiTiet.GhiChuMienGiamThem = null;
                        }
                        else
                        {
                            donVTYTThanhToanChiTiet.GhiChuMienGiamThem = thongTinVatTuMoi.GhiChuMienGiamThem;
                        }

                        donVTYTThanhToan.DonVTYTThanhToanChiTiets.Add(donVTYTThanhToanChiTiet);
                    }
                }
            }

            if (duocPhamVuotTonKho.Count > 0)
            {
                return new KetQuaThemThanhToanDonThuocVo
                {
                    ThanhCong = false,
                    Error = "Có dược phẩm vượt tồn kho",
                    DanhSachDuocPhamVuotTonKho = duocPhamVuotTonKho
                };
            }
            //kiem tra so tien thanh toan
            var tongThu = thongTinDonThuoc.TienMat.GetValueOrDefault() +
                          thongTinDonThuoc.ChuyenKhoan.GetValueOrDefault() +
                           thongTinDonThuoc.SoTienCongNo.GetValueOrDefault() +
                          thongTinDonThuoc.POS.GetValueOrDefault();

            var tk = ycTiepNhan.BenhNhan.TaiKhoanBenhNhan ?? new TaiKhoanBenhNhan
            {
                BenhNhan = ycTiepNhan.BenhNhan,
                SoDuTaiKhoan = 0
            };

            decimal soTienCanThuDuocPham = ycTiepNhan.DonThuocThanhToans
                .Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(o => o.WillDelete != true)
                .Sum(o => o.DonGiaBan * (decimal)o.SoLuong - o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault());

            decimal soTienCanThuVatTu = ycTiepNhan.DonVTYTThanhToans
                .Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                .SelectMany(o => o.DonVTYTThanhToanChiTiets).Where(o => o.WillDelete != true)
                .Sum(o => o.DonGiaBan * (decimal)o.SoLuong - o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault());
            decimal tongSoTienCanThu = Math.Round(soTienCanThuDuocPham + soTienCanThuVatTu, MidpointRounding.AwayFromZero);
            if (!tongSoTienCanThu.SoTienTuongDuong(tongThu))
            {
                return new KetQuaThemThanhToanDonThuocVo
                {
                    ThanhCong = false,
                    Error = "Số tiền thanh toan không hợp lệ",
                    DanhSachDuocPhamVuotTonKho = duocPhamVuotTonKho
                };
            }
            thuPhi.TaiKhoanBenhNhan = tk;
            thuPhi.SoPhieuHienThi = ResourceHelper.CreateSoPhieuThu(userId, maTaiKhoan);

            var xuatKhoDuocPham = new XuatKhoDuocPham()
            {
                KhoXuatId = (int)Enums.EnumKhoDuocPham.KhoNhaThuoc,
                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
                TenNguoiNhan = ycTiepNhan.HoTen,
                NgayXuat = DateTime.Now,
                NguoiXuatId = _userAgentHelper.GetCurrentUserId()
            };
            var xuatKhoVatTu = new XuatKhoVatTu()
            {
                KhoXuatId = (int)Enums.EnumKhoDuocPham.KhoNhaThuoc,
                LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
                TenNguoiNhan = ycTiepNhan.HoTen,
                NgayXuat = DateTime.Now,
                NguoiXuatId = _userAgentHelper.GetCurrentUserId()
            };

            foreach (var thuocThanhToan in ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan))
            {
                thuocThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                if (xuatThuoc)
                {
                    thuocThanhToan.ThoiDiemCapThuoc = DateTime.Now;
                    thuocThanhToan.TrangThai = TrangThaiDonThuocThanhToan.DaXuatThuoc;
                }
                foreach (var donThuocThanhToanChiTiet in thuocThanhToan.DonThuocThanhToanChiTiets.Where(o => o.WillDelete != true))
                {
                    donThuocThanhToanChiTiet.SoTienBenhNhanDaChi = (decimal)donThuocThanhToanChiTiet.SoLuong * donThuocThanhToanChiTiet.DonGiaBan - donThuocThanhToanChiTiet.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - donThuocThanhToanChiTiet.SoTienMienGiam.GetValueOrDefault();

                    var donThuocThanhToanChiTietTheoPhieuThu = donThuocThanhToanChiTiet.Map<DonThuocThanhToanChiTietTheoPhieuThu>();
                    donThuocThanhToanChiTietTheoPhieuThu.NgayPhatSinh = donThuocThanhToanChiTiet.CreatedOn ?? DateTime.Now;
                    donThuocThanhToanChiTietTheoPhieuThu.LoaiDonThuoc = thuocThanhToan.LoaiDonThuoc;
                    var ttNhapThuoc = donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet;
                    donThuocThanhToanChiTietTheoPhieuThu.Solo = ttNhapThuoc.Solo;
                    donThuocThanhToanChiTietTheoPhieuThu.MaVach = ttNhapThuoc.MaVach;
                    donThuocThanhToanChiTietTheoPhieuThu.HanSuDung = ttNhapThuoc.HanSuDung;
                    donThuocThanhToanChiTietTheoPhieuThu.NgayNhapVaoBenhVien = ttNhapThuoc.NgayNhapVaoBenhVien;
                    donThuocThanhToanChiTietTheoPhieuThu.SoLuongToa = donThuocThanhToanChiTiet.YeuCauKhamBenhDonThuocChiTiet?.SoLuong;
                    donThuocThanhToanChiTietTheoPhieuThu.BacSiKeDonId = donThuocThanhToanChiTiet.YeuCauKhamBenhDonThuocChiTiet?.YeuCauKhamBenhDonThuoc.BacSiKeDonId;
                    donThuocThanhToanChiTietTheoPhieuThu.ThoiDiemKeDon = donThuocThanhToanChiTiet.YeuCauKhamBenhDonThuocChiTiet?.YeuCauKhamBenhDonThuoc.ThoiDiemKeDon;

                    thuPhi.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                    {
                        TaiKhoanBenhNhan = tk,
                        LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                        TienChiPhi = donThuocThanhToanChiTiet.SoTienBenhNhanDaChi,
                        NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                        NgayChi = DateTime.Now,
                        SoPhieuHienThi = thuPhi.SoPhieuHienThi,
                        DonThuocThanhToanChiTiet = donThuocThanhToanChiTiet,
                        DonThuocThanhToanChiTietTheoPhieuThu = donThuocThanhToanChiTietTheoPhieuThu,
                        Gia = donThuocThanhToanChiTiet.DonGiaBan,
                        SoLuong = donThuocThanhToanChiTiet.SoLuong,
                        SoTienMienGiam = donThuocThanhToanChiTiet.SoTienMienGiam,
                        SoTienBaoHiemTuNhanChiTra = donThuocThanhToanChiTiet.SoTienBaoHiemTuNhanChiTra,
                        YeuCauTiepNhanId = ycTiepNhan.Id,
                        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                    });
                    if (xuatThuoc)
                    {
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NgayXuat = DateTime.Now;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.NgayXuat = DateTime.Now;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = xuatKhoDuocPham;
                    }
                }
            }
            foreach (var vatTuThanhToan in ycTiepNhan.DonVTYTThanhToans.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan))
            {
                vatTuThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;
                if (xuatThuoc)
                {
                    vatTuThanhToan.ThoiDiemCapVTYT = DateTime.Now;
                    vatTuThanhToan.TrangThai = TrangThaiDonVTYTThanhToan.DaXuatVTYT;
                }
                foreach (var donVTThanhToanChiTiet in vatTuThanhToan.DonVTYTThanhToanChiTiets.Where(o => o.WillDelete != true))
                {
                    donVTThanhToanChiTiet.SoTienBenhNhanDaChi = (decimal)donVTThanhToanChiTiet.SoLuong * donVTThanhToanChiTiet.DonGiaBan - donVTThanhToanChiTiet.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - donVTThanhToanChiTiet.SoTienMienGiam.GetValueOrDefault();

                    var donVTYTThanhToanChiTietTheoPhieuThu = donVTThanhToanChiTiet.Map<DonVTYTThanhToanChiTietTheoPhieuThu>();
                    donVTYTThanhToanChiTietTheoPhieuThu.NgayPhatSinh = donVTThanhToanChiTiet.CreatedOn ?? DateTime.Now;
                    var ttNhapVatTu = donVTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet;
                    donVTYTThanhToanChiTietTheoPhieuThu.Solo = ttNhapVatTu.Solo;
                    donVTYTThanhToanChiTietTheoPhieuThu.MaVach = ttNhapVatTu.MaVach;
                    donVTYTThanhToanChiTietTheoPhieuThu.HanSuDung = ttNhapVatTu.HanSuDung;
                    donVTYTThanhToanChiTietTheoPhieuThu.NgayNhapVaoBenhVien = ttNhapVatTu.NgayNhapVaoBenhVien;
                    donVTYTThanhToanChiTietTheoPhieuThu.SoLuongToa = donVTThanhToanChiTiet.YeuCauKhamBenhDonVTYTChiTiet?.SoLuong;
                    donVTYTThanhToanChiTietTheoPhieuThu.BacSiKeDonId = donVTThanhToanChiTiet.YeuCauKhamBenhDonVTYTChiTiet?.YeuCauKhamBenhDonVTYT.BacSiKeDonId;
                    donVTYTThanhToanChiTietTheoPhieuThu.ThoiDiemKeDon = donVTThanhToanChiTiet.YeuCauKhamBenhDonVTYTChiTiet?.YeuCauKhamBenhDonVTYT.ThoiDiemKeDon;

                    thuPhi.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                    {
                        TaiKhoanBenhNhan = tk,
                        LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                        TienChiPhi = donVTThanhToanChiTiet.SoTienBenhNhanDaChi,
                        NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                        NgayChi = DateTime.Now,
                        SoPhieuHienThi = thuPhi.SoPhieuHienThi,
                        DonVTYTThanhToanChiTiet = donVTThanhToanChiTiet,
                        DonVTYTThanhToanChiTietTheoPhieuThu = donVTYTThanhToanChiTietTheoPhieuThu,
                        Gia = donVTThanhToanChiTiet.DonGiaBan,
                        SoLuong = donVTThanhToanChiTiet.SoLuong,
                        SoTienMienGiam = donVTThanhToanChiTiet.SoTienMienGiam,
                        SoTienBaoHiemTuNhanChiTra = donVTThanhToanChiTiet.SoTienBaoHiemTuNhanChiTra,
                        YeuCauTiepNhanId = ycTiepNhan.Id,
                        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                    });

                    if (xuatThuoc)
                    {
                        donVTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NgayXuat = DateTime.Now;
                        donVTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.NgayXuat = DateTime.Now;
                        donVTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu = xuatKhoVatTu;
                    }
                }
            }
            ycTiepNhan.TaiKhoanBenhNhanThus.Add(thuPhi);
            await _yeuCauTiepNhanRepo.UpdateAsync(ycTiepNhan);
            return new KetQuaThemThanhToanDonThuocVo
            {
                ThanhCong = true,
                TaiKhoanBenhNhanThuId = thuPhi.Id
            };
        }
        #endregion
        #region Thanh toán đơn thuốc khách vãng lai
        public async Task<KetQuaThemThanhToanDonThuocVo> ThanhToanDonThuocKhachVanLai(KhachVangLaiThanhToanDonThuocVo thongTinDonThuoc, bool xuatThuoc = false)
        {

            var userId = _userAgentHelper.GetCurrentUserId();
            var userThuNgan = _userRepo.GetById(_userAgentHelper.GetCurrentUserId());
            var maTaiKhoan = userId.ToString();
            if (userThuNgan.Email != null && userThuNgan.Email.IndexOf("@") > 0)
            {
                maTaiKhoan = userThuNgan.Email.Substring(0, userThuNgan.Email.IndexOf("@") > 10 ? 10 : userThuNgan.Email.IndexOf("@"));
            }

            if (thongTinDonThuoc.DanhSachDonThuoc.Count == 0)
            {
                return new KetQuaThemThanhToanDonThuocVo
                {
                    ThanhCong = false,
                    Error = "Không có dược phẩm trong đơn thuốc"
                };
            }
            var ycTiepNhan = new YeuCauTiepNhan
            {
                LoaiYeuCauTiepNhan = EnumLoaiYeuCauTiepNhan.MuaThuoc,
                ThoiDiemTiepNhan = DateTime.Now,
                ThoiDiemCapNhatTrangThai = DateTime.Now,
                TrangThaiYeuCauTiepNhan = EnumTrangThaiYeuCauTiepNhan.DangThucHien
            };
            BenhNhan benhNhan;
            if (thongTinDonThuoc.ThongTinKhach.BenhNhanId.GetValueOrDefault() != 0)
            {
                benhNhan = _benhNhanRepo.GetById(thongTinDonThuoc.ThongTinKhach.BenhNhanId.GetValueOrDefault());
                if (benhNhan.TaiKhoanBenhNhan == null)
                {
                    benhNhan.TaiKhoanBenhNhan = new TaiKhoanBenhNhan { SoDuTaiKhoan = 0 };
                }
            }
            else
            {
                benhNhan = new BenhNhan
                {
                    TaiKhoanBenhNhan = new TaiKhoanBenhNhan { SoDuTaiKhoan = 0 },
                    HoTen = thongTinDonThuoc.ThongTinKhach.HoTen.ToUpper(),
                    GioiTinh = thongTinDonThuoc.ThongTinKhach.GioiTinh,
                    NamSinh = thongTinDonThuoc.ThongTinKhach.NamSinh,
                    DiaChi = thongTinDonThuoc.ThongTinKhach.DiaChi,
                    SoDienThoai = thongTinDonThuoc.ThongTinKhach.SoDienThoai,
                    TinhThanhId = thongTinDonThuoc.ThongTinKhach.TinhThanhId == 0 ? null : thongTinDonThuoc.ThongTinKhach.TinhThanhId,
                    QuanHuyenId = thongTinDonThuoc.ThongTinKhach.QuanHuyenId == 0 ? null : thongTinDonThuoc.ThongTinKhach.QuanHuyenId,
                    PhuongXaId = thongTinDonThuoc.ThongTinKhach.PhuongXaId == 0 ? null : thongTinDonThuoc.ThongTinKhach.PhuongXaId
                };
            }
            ycTiepNhan.BenhNhan = benhNhan;
            ycTiepNhan.HoTen = benhNhan.HoTen;
            ycTiepNhan.GioiTinh = benhNhan.GioiTinh;
            ycTiepNhan.NamSinh = benhNhan.NamSinh;
            ycTiepNhan.DiaChi = benhNhan.DiaChi;
            ycTiepNhan.SoDienThoai = benhNhan.SoDienThoai;
            ycTiepNhan.TinhThanhId = benhNhan.TinhThanhId;
            ycTiepNhan.QuanHuyenId = benhNhan.QuanHuyenId;
            ycTiepNhan.PhuongXaId = benhNhan.PhuongXaId;

            //kiem tra thong tin thanh toan truoc khi thanh toan
            var duocPhamVuotTonKho = new List<DuocPhamVuotTonKho>();
            // them duoc pham 
            var listDuocPham = thongTinDonThuoc.DanhSachDonThuoc.Where(x => x.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).ToList();
            if (listDuocPham.Count > 0)
            {
                var donThuocThanhToan = new DonThuocThanhToan
                {
                    LoaiDonThuoc = EnumLoaiDonThuoc.ThuocKhongBHYT,
                    BenhNhan = benhNhan,
                    TrangThai = Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                };
                ycTiepNhan.DonThuocThanhToans.Add(donThuocThanhToan);
                foreach (var thongTinDuocPhamMoi in listDuocPham)
                {
                    var duocPham = _duocPhamnRepo.GetById(thongTinDuocPhamMoi.DuocPhamId,
                        x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets)
                            .ThenInclude(nkct => nkct.HopDongThauDuocPhams)
                            .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets)
                            .ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

                    var nhapKhoDuocPhamChiTiet =
                        duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.First(o =>
                            o.Id == thongTinDuocPhamMoi.NhapKhoDuocPhamChiTietId);
                    var soLuongTon = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                    if (soLuongTon < thongTinDuocPhamMoi.SoLuongMua)
                    {
                        duocPhamVuotTonKho.Add(new DuocPhamVuotTonKho
                        {
                            Stt = thongTinDuocPhamMoi.STT.GetValueOrDefault(),
                            SoLuongTonKho = soLuongTon
                        });
                    }
                    else
                    {
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat += thongTinDuocPhamMoi.SoLuongMua;
                        var xuatKhoChiTiet = new XuatKhoDuocPhamChiTietViTri
                        {
                            SoLuongXuat = thongTinDuocPhamMoi.SoLuongMua,
                            NgayXuat = DateTime.Now,
                            NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                            XuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                            {
                                DuocPhamBenhVien = duocPham.DuocPhamBenhVien,
                            }
                        };

                        var dtttChiTiet = new DonThuocThanhToanChiTiet
                        {
                            DuocPhamId = duocPham.Id,
                            XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet,
                            Ten = duocPham.Ten,
                            TenTiengAnh = duocPham.TenTiengAnh,
                            SoDangKy = duocPham.SoDangKy,
                            STTHoatChat = duocPham.STTHoatChat,
                            NhomChiPhi = Enums.EnumDanhMucNhomTheoChiPhi.ThuocThanhToanTheoTyLe,
                            MaHoatChat = duocPham.MaHoatChat,
                            HoatChat = duocPham.HoatChat,
                            LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat,
                            NhaSanXuat = duocPham.NhaSanXuat,
                            NuocSanXuat = duocPham.NuocSanXuat,
                            DuongDungId = duocPham.DuongDungId,
                            HamLuong = duocPham.HamLuong,
                            QuyCach = duocPham.QuyCach,
                            TieuChuan = duocPham.TieuChuan,
                            DangBaoChe = duocPham.DangBaoChe,
                            DonViTinhId = duocPham.DonViTinhId,
                            HuongDan = duocPham.HuongDan,
                            MoTa = duocPham.MoTa,
                            ChiDinh = duocPham.ChiDinh,
                            ChongChiDinh = duocPham.ChongChiDinh,
                            LieuLuongCachDung = duocPham.LieuLuongCachDung,
                            TacDungPhu = duocPham.TacDungPhu,
                            ChuYDePhong = duocPham.ChuYDePhong,
                            HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                            NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId,
                            SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong,
                            SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh,
                            LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau,
                            LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau,
                            NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau,
                            GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau,
                            NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam,
                            DonGiaNhap = nhapKhoDuocPhamChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoDuocPhamChiTiet.TiLeTheoThapGia,
                            PhuongPhapTinhGiaTriTonKho = nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho,
                            VAT = nhapKhoDuocPhamChiTiet.VAT,
                            SoLuong = thongTinDuocPhamMoi.SoLuongMua,
                            SoTienBenhNhanDaChi = 0,
                            DuocHuongBaoHiem = false,
                            DonGiaBan = CalculateHelper.TinhDonGiaBan(nhapKhoDuocPhamChiTiet.DonGiaNhap, nhapKhoDuocPhamChiTiet.TiLeTheoThapGia, nhapKhoDuocPhamChiTiet.VAT, true, true, nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho)
                        };

                        donThuocThanhToan.DonThuocThanhToanChiTiets.Add(dtttChiTiet);
                    }
                }
            }
            // them vat tu
            var listVatTu = thongTinDonThuoc.DanhSachDonThuoc.Where(x => x.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien).ToList();
            if (listVatTu.Count > 0)
            {
                // vật tư
                var donVTYTThanhToan = new DonVTYTThanhToan
                {
                    BenhNhanId = ycTiepNhan.BenhNhanId,
                    TrangThai = Enums.TrangThaiDonVTYTThanhToan.ChuaXuatVTYT,
                    TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan
                };
                ycTiepNhan.DonVTYTThanhToans.Add(donVTYTThanhToan);

                foreach (var thongTinVatTuMoi in listVatTu)
                {
                    var vatTuBenhVien = _vatTuBenhVienRepository.GetById(thongTinVatTuMoi.DuocPhamId,
                    x => x.Include(vt => vt.VatTus)
                        .Include(vt => vt.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.HopDongThauVatTu)
                        .Include(vt => vt.NhapKhoVatTuChiTiets).ThenInclude(nkct => nkct.NhapKhoVatTu).ThenInclude(nk => nk.Kho));

                    var nhapKhoVatTuChiTiet = vatTuBenhVien.NhapKhoVatTuChiTiets.First(o => o.Id == thongTinVatTuMoi.NhapKhoDuocPhamChiTietId);
                    var soLuongTon = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;
                    if (soLuongTon < thongTinVatTuMoi.SoLuongMua)
                    {
                        duocPhamVuotTonKho.Add(new DuocPhamVuotTonKho
                        {
                            Stt = thongTinVatTuMoi.STT.GetValueOrDefault(),
                            SoLuongTonKho = soLuongTon
                        });
                    }
                    else
                    {
                        nhapKhoVatTuChiTiet.SoLuongDaXuat += thongTinVatTuMoi.SoLuongMua;
                        var xuatKhoChiTiet = new XuatKhoVatTuChiTietViTri
                        {
                            SoLuongXuat = thongTinVatTuMoi.SoLuongMua,
                            NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                            XuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                            {
                                VatTuBenhVien = vatTuBenhVien
                            }
                        };

                        var donVTYTThanhToanChiTiet = new DonVTYTThanhToanChiTiet
                        {
                            VatTuBenhVienId = vatTuBenhVien.Id,
                            XuatKhoVatTuChiTietViTri = xuatKhoChiTiet,
                            Ten = vatTuBenhVien.VatTus.Ten,
                            Ma = vatTuBenhVien.VatTus.Ma,
                            NhomVatTuId = vatTuBenhVien.VatTus.NhomVatTuId,
                            DonViTinh = vatTuBenhVien.VatTus.DonViTinh,
                            NhaSanXuat = vatTuBenhVien.VatTus.NhaSanXuat,
                            NuocSanXuat = vatTuBenhVien.VatTus.NuocSanXuat,
                            QuyCach = vatTuBenhVien.VatTus.QuyCach,
                            MoTa = vatTuBenhVien.VatTus.MoTa,
                            DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTiet.VAT,
                            HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                            NhaThauId = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhaThauId,
                            SoHopDongThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoHopDong,
                            SoQuyetDinhThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.SoQuyetDinh,
                            LoaiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.LoaiThau,
                            NhomThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.NhomThau,
                            GoiThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.GoiThau,
                            NamThau = nhapKhoVatTuChiTiet.HopDongThauVatTu.Nam,
                            SoTienBenhNhanDaChi = 0,
                            SoLuong = thongTinVatTuMoi.SoLuongMua,
                            DonGiaBan = CalculateHelper.TinhDonGiaBan(nhapKhoVatTuChiTiet.DonGiaNhap, nhapKhoVatTuChiTiet.TiLeTheoThapGia, nhapKhoVatTuChiTiet.VAT, false, true, nhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho),
                            GiaBan = CalculateHelper.TinhDonGiaBan(nhapKhoVatTuChiTiet.DonGiaNhap, nhapKhoVatTuChiTiet.TiLeTheoThapGia, nhapKhoVatTuChiTiet.VAT, false, true, nhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho) * (decimal)thongTinVatTuMoi.SoLuongMua
                        };
                        donVTYTThanhToan.DonVTYTThanhToanChiTiets.Add(donVTYTThanhToanChiTiet);
                    }
                }
            }
            if (duocPhamVuotTonKho.Count > 0)
            {
                return new KetQuaThemThanhToanDonThuocVo
                {
                    ThanhCong = false,
                    Error = "Có dược phẩm vượt tồn kho",
                    DanhSachDuocPhamVuotTonKho = duocPhamVuotTonKho
                };
            }
            //kiem tra so tien thanh toan
            var tongThu = thongTinDonThuoc.ThongTinThuChi.TienMat.GetValueOrDefault() +
                          thongTinDonThuoc.ThongTinThuChi.ChuyenKhoan.GetValueOrDefault() +
                          thongTinDonThuoc.ThongTinThuChi.POS.GetValueOrDefault() +
                          thongTinDonThuoc.ThongTinThuChi.SoTienCongNo.GetValueOrDefault();

            decimal soTienCanThu = ycTiepNhan.DonThuocThanhToans
                .Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                .SelectMany(o => o.DonThuocThanhToanChiTiets)
                .Sum(o => (decimal)o.SoLuong * o.DonGiaBan);
            decimal soTienCanThuVT = ycTiepNhan.DonVTYTThanhToans
               .Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
               .SelectMany(o => o.DonVTYTThanhToanChiTiets)
               .Sum(o => (decimal)o.SoLuong * o.DonGiaBan);
            decimal tongthuTien = soTienCanThu + soTienCanThuVT;
            if (!tongthuTien.SoTienTuongDuong(tongThu))
            {
                return new KetQuaThemThanhToanDonThuocVo
                {
                    ThanhCong = false,
                    Error = "Số tiền thanh toán không hợp lệ",
                    DanhSachDuocPhamVuotTonKho = duocPhamVuotTonKho
                };
            }
            var tk = ycTiepNhan.BenhNhan.TaiKhoanBenhNhan ?? new TaiKhoanBenhNhan
            {
                BenhNhan = ycTiepNhan.BenhNhan
            };

            var thuPhi = new TaiKhoanBenhNhanThu
            {
                TaiKhoanBenhNhan = tk,
                LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi,
                LoaiNoiThu = LoaiNoiThu.NhaThuoc,
                TienMat = thongTinDonThuoc.ThongTinThuChi.TienMat,
                ChuyenKhoan = thongTinDonThuoc.ThongTinThuChi.ChuyenKhoan,
                POS = thongTinDonThuoc.ThongTinThuChi.POS,
                //update công nợ 25/05/2021 cho khách vẵng lai
                CongNo = thongTinDonThuoc.ThongTinThuChi.SoTienCongNo,
                NoiDungThu = thongTinDonThuoc.ThongTinThuChi.NoiDungThu,
                GhiChu = thongTinDonThuoc.ThongTinThuChi.GhiChu,
                NgayThu = thongTinDonThuoc.ThongTinThuChi.NgayThu,
                SoQuyen = 1,
                NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                SoPhieuHienThi = ResourceHelper.CreateSoPhieuThu(userId, maTaiKhoan),
            };

            var xuatKhoDuocPham = new XuatKhoDuocPham()
            {
                KhoXuatId = (int)Enums.EnumKhoDuocPham.KhoNhaThuoc,
                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
                TenNguoiNhan = ycTiepNhan.HoTen,
                NgayXuat = DateTime.Now,
                NguoiXuatId = _userAgentHelper.GetCurrentUserId()
            };
            var xuatKhoVatTu = new XuatKhoVatTu()
            {
                KhoXuatId = (int)Enums.EnumKhoDuocPham.KhoNhaThuoc,
                LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
                TenNguoiNhan = ycTiepNhan.HoTen,
                NgayXuat = DateTime.Now,
                NguoiXuatId = _userAgentHelper.GetCurrentUserId()
            };

            foreach (var thuocThanhToan in ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan))
            {
                thuocThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;
                if (xuatThuoc)
                {
                    thuocThanhToan.ThoiDiemCapThuoc = DateTime.Now;
                    thuocThanhToan.TrangThai = TrangThaiDonThuocThanhToan.DaXuatThuoc;
                }
                foreach (var donThuocThanhToanChiTiet in thuocThanhToan.DonThuocThanhToanChiTiets)
                {
                    donThuocThanhToanChiTiet.SoTienBenhNhanDaChi = (decimal)donThuocThanhToanChiTiet.SoLuong * donThuocThanhToanChiTiet.DonGiaBan;

                    var donThuocThanhToanChiTietTheoPhieuThu = donThuocThanhToanChiTiet.Map<DonThuocThanhToanChiTietTheoPhieuThu>();
                    donThuocThanhToanChiTietTheoPhieuThu.NgayPhatSinh = donThuocThanhToanChiTiet.CreatedOn ?? DateTime.Now;
                    donThuocThanhToanChiTietTheoPhieuThu.LoaiDonThuoc = thuocThanhToan.LoaiDonThuoc;
                    var ttNhapThuoc = donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet;
                    donThuocThanhToanChiTietTheoPhieuThu.Solo = ttNhapThuoc.Solo;
                    donThuocThanhToanChiTietTheoPhieuThu.MaVach = ttNhapThuoc.MaVach;
                    donThuocThanhToanChiTietTheoPhieuThu.HanSuDung = ttNhapThuoc.HanSuDung;
                    donThuocThanhToanChiTietTheoPhieuThu.NgayNhapVaoBenhVien = ttNhapThuoc.NgayNhapVaoBenhVien;

                    thuPhi.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                    {
                        TaiKhoanBenhNhan = tk,
                        LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                        TienChiPhi = donThuocThanhToanChiTiet.SoTienBenhNhanDaChi,
                        NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                        NgayChi = DateTime.Now,
                        SoPhieuHienThi = thuPhi.SoPhieuHienThi,
                        DonThuocThanhToanChiTiet = donThuocThanhToanChiTiet,
                        DonThuocThanhToanChiTietTheoPhieuThu = donThuocThanhToanChiTietTheoPhieuThu,
                        Gia = donThuocThanhToanChiTiet.DonGiaBan,
                        SoLuong = donThuocThanhToanChiTiet.SoLuong,
                        YeuCauTiepNhan = ycTiepNhan,
                        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId(),

                    });

                    if (xuatThuoc)
                    {
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NgayXuat = DateTime.Now;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.NgayXuat = DateTime.Now;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = xuatKhoDuocPham;
                    }
                }
            }
            foreach (var vatTuThanhToan in ycTiepNhan.DonVTYTThanhToans.Where(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan))
            {
                vatTuThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;
                if (xuatThuoc)
                {
                    vatTuThanhToan.ThoiDiemCapVTYT = DateTime.Now;
                    vatTuThanhToan.TrangThai = TrangThaiDonVTYTThanhToan.DaXuatVTYT;
                }
                foreach (var donVTThanhToanChiTiet in vatTuThanhToan.DonVTYTThanhToanChiTiets)
                {
                    donVTThanhToanChiTiet.SoTienBenhNhanDaChi = (decimal)donVTThanhToanChiTiet.SoLuong * donVTThanhToanChiTiet.DonGiaBan - donVTThanhToanChiTiet.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - donVTThanhToanChiTiet.SoTienMienGiam.GetValueOrDefault();

                    var donVTYTThanhToanChiTietTheoPhieuThu = donVTThanhToanChiTiet.Map<DonVTYTThanhToanChiTietTheoPhieuThu>();
                    donVTYTThanhToanChiTietTheoPhieuThu.NgayPhatSinh = donVTThanhToanChiTiet.CreatedOn ?? DateTime.Now;
                    var ttNhapVatTu = donVTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet;
                    donVTYTThanhToanChiTietTheoPhieuThu.Solo = ttNhapVatTu.Solo;
                    donVTYTThanhToanChiTietTheoPhieuThu.MaVach = ttNhapVatTu.MaVach;
                    donVTYTThanhToanChiTietTheoPhieuThu.HanSuDung = ttNhapVatTu.HanSuDung;
                    donVTYTThanhToanChiTietTheoPhieuThu.NgayNhapVaoBenhVien = ttNhapVatTu.NgayNhapVaoBenhVien;

                    thuPhi.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                    {

                        TaiKhoanBenhNhan = tk,
                        LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                        TienChiPhi = donVTThanhToanChiTiet.SoTienBenhNhanDaChi,
                        NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                        NgayChi = DateTime.Now,
                        SoPhieuHienThi = thuPhi.SoPhieuHienThi,
                        DonVTYTThanhToanChiTietTheoPhieuThu = donVTYTThanhToanChiTietTheoPhieuThu,
                        Gia = donVTThanhToanChiTiet.DonGiaBan,
                        SoLuong = donVTThanhToanChiTiet.SoLuong,
                        DonVTYTThanhToanChiTiet = donVTThanhToanChiTiet,
                        YeuCauTiepNhan = ycTiepNhan,
                        NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                    });
                    if (xuatThuoc)
                    {
                        donVTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NgayXuat = DateTime.Now;
                        donVTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.NgayXuat = DateTime.Now;
                        donVTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu = xuatKhoVatTu;
                    }
                }
            }
            ycTiepNhan.TaiKhoanBenhNhanThus.Add(thuPhi);
            var maTiepNhan = ResourceHelper.CreateMaYeuCauTiepNhan();
            ycTiepNhan.MaYeuCauTiepNhan = maTiepNhan;
            await _yeuCauTiepNhanRepo.AddAsync(ycTiepNhan);
            return new KetQuaThemThanhToanDonThuocVo
            {
                ThanhCong = true,
                TaiKhoanBenhNhanThuId = thuPhi.Id
            };
        }
        #endregion
        private async Task<string> GetUserName(long id)
        {
            var name = await _userRepo.TableNoTracking.Where(p => p.Id == id)
                .Select(p => p.HoTen).FirstOrDefaultAsync();
            return name;
        }
        #region in phiếu thu bảng kê thu tiền thuốc
        public async Task<string> InBaoCaoToaThuocAsync(long id, bool bangKe, bool thuTien, string hostingName)
        {
            var content = "";
            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var currentUserName = await GetUserName(currentUserId);
            var groupVTBHYT = "Vật Tư BHYT";
            var groupVTKhongBHYT = "Vật Tư Không BHYT";

            var headerVTBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupVTBHYT.ToUpper()
                                        + "</b></tr>";
            var headerVTKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupVTKhongBHYT.ToUpper()
                                        + "</b></tr>";
            var groupThuocBHYT = "Dược Phẩm BHYT";
            var groupThuocKhongBHYT = "Dược Phẩm Không BHYT";

            var headerDPBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                        + "</b></tr>";
            var headerDPKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                        + "</b></tr>";

            var thongTinPhieuThuQuayThuoc = GetThongTinPhieuThuQuayThuoc(id);

            if (bangKe && thuTien == false)
            {
                var result = _templateRepo.TableNoTracking
                .FirstOrDefault(x => x.Name.Equals("BangKeThuTienThuoc"));

                var tkThu = _taiKhoanBenhNhanThuRepository.GetById(id,
                    x => x.Include(o => o.CongTyBaoHiemTuNhanCongNos).Include(o => o.MienGiamChiPhis)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.TinhThanh)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.QuanHuyen)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.BenhNhan)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonViTinh)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonThuocThanhToan)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.XuatKhoDuocPhamChiTietViTri)
                                                                    .ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(dt => dt.XuatKhoDuocPham)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonVTYTThanhToanChiTiet).ThenInclude(o => o.DonVTYTThanhToan)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonVTYTThanhToanChiTiet).ThenInclude(o => o.XuatKhoVatTuChiTietViTri)
                                                                    .ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(dt => dt.XuatKhoVatTu));


                var tongTienToaThuoc = string.Empty;
                var soTienBangChu = string.Empty;
                var keToaThuoc = string.Empty;
                var keToaThuocBHKBH = string.Empty;
                var keDuoPham = string.Empty;
                var keDuoPhamKBHYT = string.Empty;
                var keVatTu = string.Empty;
                var keVatTuKBH = string.Empty;
                decimal tongTienPhaiTra = 0;
                int idex = 1;
                //decimal tongChiPhi = 0;

                if (tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTiet).Any())
                {
                    //BVHD-3943: gộp thuốc, VT
                    var thuocThanhToanChiTietBHYTs = tkThu.TaiKhoanBenhNhanChis
                        .Where(p => p.DonThuocThanhToanChiTietId != null && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT)
                        .Select(o => o.DonThuocThanhToanChiTiet).ToList();
                    var groupThuocThanhToanChiTietBHYTs = thuocThanhToanChiTietBHYTs.GroupBy(o => new { o.DuocPhamId, o.DonGiaBan }).ToList();

                    foreach (var thuocThanhToanChiTiet in groupThuocThanhToanChiTietBHYTs)
                    {
                        keDuoPham = keDuoPham + "<tr style='border: 1px solid #020000;'>"
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     idex++
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     GetFormatDuocPham(thuocThanhToanChiTiet.Key.DuocPhamId)
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     thuocThanhToanChiTiet.First().DonViTinh.Ten
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     GetFormatSoLuong(thuocThanhToanChiTiet.Key.DuocPhamId, thuocThanhToanChiTiet.Sum(o=> o.SoLuong))
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Key.DonGiaBan).ApplyFormatMoneyToDouble()
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Sum(o=>o.GiaBan)).ApplyFormatMoneyToDouble()
                                                     + "</tr>";
                        tongTienPhaiTra += thuocThanhToanChiTiet.Sum(o => o.GiaBan);
                    }
                    //foreach (var thuocThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTiet))
                    //{
                    //    if (thuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT)
                    //    {
                    //        if (thuocThanhToanChiTiet != null)
                    //        {
                    //            keDuoPham = keDuoPham + "<tr style='border: 1px solid #020000;'>"
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 idex++
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 thuocThanhToanChiTiet?.DonViTinh.Ten
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuong)
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.DonGiaBan).ApplyFormatMoneyToDouble()
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.GiaBan).ApplyFormatMoneyToDouble()
                    //                                 + "</tr>";
                    //            tongTienPhaiTra += thuocThanhToanChiTiet.GiaBan;
                    //            //tongChiPhi = Convert.ToDecimal(thuocThanhToanChiTiet.SoTienBenhNhanDaChi);
                    //        }
                    //    }
                    //}

                    //BVHD-3943: gộp thuốc, VT
                    var thuocThanhToanChiTietKhongBHYTs = tkThu.TaiKhoanBenhNhanChis
                        .Where(p => p.DonThuocThanhToanChiTietId != null && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)
                        .Select(o => o.DonThuocThanhToanChiTiet).ToList();
                    var groupThuocThanhToanChiTietKhongBHYTs = thuocThanhToanChiTietKhongBHYTs.GroupBy(o => new { o.DuocPhamId, o.DonGiaBan }).ToList();
                    foreach (var thuocThanhToanChiTiet in groupThuocThanhToanChiTietKhongBHYTs)
                    {
                        keDuoPhamKBHYT = keDuoPhamKBHYT + "<tr style='border: 1px solid #020000;'>"
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     idex++
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                      GetFormatDuocPham(thuocThanhToanChiTiet.Key.DuocPhamId)
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     thuocThanhToanChiTiet.First().DonViTinh.Ten
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     GetFormatSoLuong(thuocThanhToanChiTiet.Key.DuocPhamId, thuocThanhToanChiTiet.Sum(o=>o.SoLuong))
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Key.DonGiaBan).ApplyFormatMoneyToDouble()
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Sum(o=>o.GiaBan)).ApplyFormatMoneyToDouble()
                                                     + "</tr>";
                        tongTienPhaiTra += thuocThanhToanChiTiet.Sum(o => o.GiaBan);
                    }


                    //foreach (var thuocThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTiet))
                    //{
                    //    if (thuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)
                    //    {
                    //        if (thuocThanhToanChiTiet != null)
                    //        {
                    //            keDuoPhamKBHYT = keDuoPhamKBHYT + "<tr style='border: 1px solid #020000;'>"
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 idex++
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                  GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 thuocThanhToanChiTiet?.DonViTinh.Ten
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuong)
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.DonGiaBan).ApplyFormatMoneyToDouble()
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.GiaBan).ApplyFormatMoneyToDouble()
                    //                                 + "</tr>";
                    //            tongTienPhaiTra += thuocThanhToanChiTiet.GiaBan;
                    //            //tongChiPhi = Convert.ToDecimal(thuocThanhToanChiTiet.SoTienBenhNhanDaChi);
                    //        }
                    //    }
                    //}
                    if (keDuoPham != "" && keDuoPhamKBHYT != "")
                    {
                        keToaThuocBHKBH = headerDPBHYT + keDuoPham + headerDPKhongBHYT + keDuoPhamKBHYT;
                    }
                    else if (keDuoPham != "" && keDuoPhamKBHYT == "")
                    {
                        keToaThuocBHKBH = headerDPBHYT + keDuoPham;
                    }
                    else if (keDuoPham == "" && keDuoPhamKBHYT != "")
                    {
                        keToaThuocBHKBH = headerDPKhongBHYT + keDuoPhamKBHYT;
                    }
                    else if (keDuoPham == "" && keDuoPhamKBHYT == "")
                    {
                        keToaThuocBHKBH = "";
                    }

                }

                if (tkThu.TaiKhoanBenhNhanChis.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(o => o.DonVTYTThanhToanChiTiet).Any())
                {
                    //BVHD-3943: gộp thuốc, VT
                    var vatTuThanhToanChiTiets = tkThu.TaiKhoanBenhNhanChis.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(o => o.DonVTYTThanhToanChiTiet).ToList();
                    var groupVatTuThanhToanChiTiets = vatTuThanhToanChiTiets.GroupBy(o => new { o.Ten, o.DonGiaBan }).ToList();
                    foreach (var vatTuThanhToanChiTiet in groupVatTuThanhToanChiTiets)
                    {
                        keVatTu += "<tr style='border: 1px solid #020000;'>"
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                idex++
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                vatTuThanhToanChiTiet.Key.Ten
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                vatTuThanhToanChiTiet.First().DonViTinh
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                vatTuThanhToanChiTiet.Sum(o=>o.SoLuong)
                                                + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                Convert.ToDouble(vatTuThanhToanChiTiet.Key.DonGiaBan).ApplyFormatMoneyToDouble()
                                                + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                Convert.ToDouble(vatTuThanhToanChiTiet.Sum(o=>o.GiaBan)).ApplyFormatMoneyToDouble()
                                                + "</tr>";
                        tongTienPhaiTra += vatTuThanhToanChiTiet.Sum(o => o.GiaBan);
                    }

                    //foreach (var vatTuThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(o => o.DonVTYTThanhToanChiTiet))
                    //{
                    //    if (vatTuThanhToanChiTiet != null)
                    //    {
                    //        keVatTu += "<tr style='border: 1px solid #020000;'>"
                    //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                            idex++
                    //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                            vatTuThanhToanChiTiet?.Ten
                    //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                            vatTuThanhToanChiTiet?.DonViTinh
                    //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                            vatTuThanhToanChiTiet?.SoLuong
                    //                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                            Convert.ToDouble(vatTuThanhToanChiTiet.DonGiaBan).ApplyFormatMoneyToDouble()
                    //                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                            Convert.ToDouble(vatTuThanhToanChiTiet.GiaBan).ApplyFormatMoneyToDouble()
                    //                            + "</tr>";
                    //        tongTienPhaiTra += vatTuThanhToanChiTiet.GiaBan;
                    //        //tongChiPhi += Convert.ToDecimal(vatTuThanhToanChiTiet.SoTienBenhNhanDaChi);
                    //    }
                    //}
                    keVatTuKBH = headerVTKhongBHYT + keVatTu;
                }

                var soChungTus = new List<string>();
                soChungTus.AddRange(tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null)
                                             .Select(o => o.DonThuocThanhToanChiTiet?.XuatKhoDuocPhamChiTietViTri?.XuatKhoDuocPhamChiTiet?.XuatKhoDuocPham?.SoPhieu)
                                             .Distinct());

                soChungTus.AddRange(tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null)
                                          .Select(o => o.DonVTYTThanhToanChiTiet?.XuatKhoVatTuChiTietViTri?.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.SoPhieu)
                                          .Distinct());

                keToaThuoc = keToaThuocBHKBH + keVatTuKBH;
                soTienBangChu = "<i>Bằng chữ:" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra)) + ".</i";
                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    NguoiNopTien = tkThu.YeuCauTiepNhan.HoTen,
                    //NamSinh = tkThu.YeuCauTiepNhan.NamSinh,
                    NamSinh = DateHelper.DOBFormat(tkThu.YeuCauTiepNhan?.NgaySinh, tkThu.YeuCauTiepNhan?.ThangSinh, tkThu.YeuCauTiepNhan?.NamSinh),
                    MaBn = tkThu.YeuCauTiepNhan.BenhNhan.MaBN,
                    NguoiThuTien = currentUserName,
                    GioiTinh = tkThu.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    DiaChi = tkThu.YeuCauTiepNhan.DiaChiDayDu,
                    DienDai = "Thu Tiền thuốc",
                    ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
                                       DateTime.Now.Year,
                    NguoiPhatThuoc = tkThu.YeuCauTiepNhan.NhanVienTiepNhan?.User?.HoTen,
                    TongTienPhaiTra = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble(),
                    keToaThuoc = keToaThuoc,
                    SoTienChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra)),
                    PhieuThu = "BangKeThuTienThuoc",
                    tongTienToaThuoc = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble(),
                    soTienBangChu = soTienBangChu,
                    SoChungTu = soChungTus.Any() ? "Số:" + String.Join(",", soChungTus.Where(c => c != null)) : string.Empty,
                    GoiHienTai = DateTime.Now.ApplyFormatTime(),
                };
                if (data.PhieuThu == "BangKeThuTienThuoc")
                {
                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data) + "<div class=\"pagebreak\"></div>";
                    //var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                }
                return content;
            }
            else if (thuTien && bangKe == false)
            {
                var result = _templateRepo.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("PhieuThu"));

                var tkThu = _taiKhoanBenhNhanThuRepository.GetById(id,
                    x => x.Include(o => o.CongTyBaoHiemTuNhanCongNos).Include(o => o.MienGiamChiPhis)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.TinhThanh)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.QuanHuyen)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.BenhNhan)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonViTinh)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonThuocThanhToan)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonVTYTThanhToanChiTiet).ThenInclude(o => o.DonVTYTThanhToan));

                decimal tongChiPhi = 0;
                foreach (var thuocThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Select(o => o.DonThuocThanhToanChiTiet))
                {
                    if (thuocThanhToanChiTiet != null)
                    {
                        tongChiPhi = Convert.ToDecimal(thuocThanhToanChiTiet.SoTienBenhNhanDaChi);
                    }
                    continue;
                }
                foreach (var vatTuThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Select(o => o.DonVTYTThanhToanChiTiet))
                {
                    if (vatTuThanhToanChiTiet != null)
                    {
                        tongChiPhi += Convert.ToDecimal(vatTuThanhToanChiTiet.SoTienBenhNhanDaChi);
                    }
                    continue;
                }

                var data = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    NguoiNopTien = tkThu.YeuCauTiepNhan.HoTen,
                    //NamSinh = tkThu.YeuCauTiepNhan.NamSinh,
                    NamSinh = DateHelper.DOBFormat(tkThu.YeuCauTiepNhan?.NgaySinh, tkThu.YeuCauTiepNhan?.ThangSinh, tkThu.YeuCauTiepNhan?.NamSinh),

                    GioiTinh = tkThu.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    DiaChi = tkThu.YeuCauTiepNhan.DiaChiDayDu,
                    NoiDung = tkThu.NoiDungThu,
                    ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
                                           DateTime.Now.Year,
                    NguoiLapPhieu = tkThu.YeuCauTiepNhan.NhanVienTiepNhan?.User?.HoTen,
                    //update 21/06/2022 làm tròn tiền trên phiếu thu ApplyFormatMoneyToDouble -> RoundAndApplyFormatMoney
                    ChiPhiKCB = Convert.ToDouble(thongTinPhieuThuQuayThuoc.TongChiPhi).RoundAndApplyFormatMoney(),
                    BHYTT = Convert.ToDouble(thongTinPhieuThuQuayThuoc.BHYTThanhToan).RoundAndApplyFormatMoney(),
                    MiemGiam = Convert.ToDouble(thongTinPhieuThuQuayThuoc.MienGiam).RoundAndApplyFormatMoney(),

                    TienMat = Convert.ToDouble(thongTinPhieuThuQuayThuoc.TienMat).RoundAndApplyFormatMoney(),
                    ChuyenKhoan = Convert.ToDouble(thongTinPhieuThuQuayThuoc.ChuyenKhoan).RoundAndApplyFormatMoney(),
                    TongCongNo = Convert.ToDouble(thongTinPhieuThuQuayThuoc.CongNo).RoundAndApplyFormatMoney(),
                    ThanhTien = Convert.ToDouble(thongTinPhieuThuQuayThuoc.BenhNhanThanhToan).RoundAndApplyFormatMoney(),
                    Pos = Convert.ToDouble(thongTinPhieuThuQuayThuoc.Pos).RoundAndApplyFormatMoney(),

                    TongChiPhi = Convert.ToDouble(thongTinPhieuThuQuayThuoc.TongChiPhi).RoundAndApplyFormatMoney(),
                    //SoQuyen = tkThu.SoQuyen,
                    SoPhieu = tkThu.SoPhieuHienThi,
                    SoTienBangChu = NumberHelper.ChuyenSoRaText(Math.Round(Convert.ToDouble(thongTinPhieuThuQuayThuoc.TongChiPhi), MidpointRounding.AwayFromZero)), // chưa có,

                    GoiHienTai = DateTime.Now.ApplyFormatTime(),
                    PhieuThu = "PhieuThu",
                };

                if (data.PhieuThu == "PhieuThu")
                {
                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data) + "<div class=\"pagebreak\"></div>";
                    //var test = content.IndexOf(tmp); // kiểm tra đoạn chuoi co ton tai
                }
                return content;
            }
            else if (thuTien && bangKe)
            {
                var tkThu = _taiKhoanBenhNhanThuRepository.GetById(id,
                    x => x.Include(o => o.CongTyBaoHiemTuNhanCongNos).Include(o => o.MienGiamChiPhis)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.TinhThanh)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.QuanHuyen)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.BenhNhan)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonViTinh)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonThuocThanhToan)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonVTYTThanhToanChiTiet).ThenInclude(o => o.DonVTYTThanhToan));


                var result1 = _templateRepo.TableNoTracking
                       .FirstOrDefault(x => x.Name.Equals("BangKeThuTienThuoc"));
                var tongTienToaThuoc = string.Empty;
                var soTienBangChu = string.Empty;
                var keToaThuoc = string.Empty;
                var keToaThuocBHKBH = string.Empty;
                var keDuoPham = string.Empty;
                var keDuoPhamKBHYT = string.Empty;
                var keVatTu = string.Empty;
                var keVatTuKBH = string.Empty;
                decimal tongTienPhaiTra = 0;
                int idex = 1;
                //decimal tongChiPhi = 0;

                if (tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTiet).Any())
                {
                    //BVHD-3943: gộp thuốc, VT
                    var thuocThanhToanChiTietBHYTs = tkThu.TaiKhoanBenhNhanChis
                        .Where(p => p.DonThuocThanhToanChiTietId != null && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT)
                        .Select(o => o.DonThuocThanhToanChiTiet).ToList();
                    var groupThuocThanhToanChiTietBHYTs = thuocThanhToanChiTietBHYTs.GroupBy(o => new { o.DuocPhamId, o.DonGiaBan }).ToList();
                    foreach (var thuocThanhToanChiTiet in groupThuocThanhToanChiTietBHYTs)
                    {
                        keDuoPham = keDuoPham + "<tr style='border: 1px solid #020000;'>"
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     idex++
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                      GetFormatDuocPham(thuocThanhToanChiTiet.Key.DuocPhamId)
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     thuocThanhToanChiTiet.First().DonViTinh.Ten
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     GetFormatSoLuong(thuocThanhToanChiTiet.Key.DuocPhamId, thuocThanhToanChiTiet.Sum(o=>o.SoLuong))
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Key.DonGiaBan).ApplyFormatMoneyToDouble()
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Sum(o=>o.GiaBan)).ApplyFormatMoneyToDouble()
                                                     + "</tr>";
                        tongTienPhaiTra += thuocThanhToanChiTiet.Sum(o => o.GiaBan);
                    }

                    //foreach (var thuocThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTiet))
                    //{
                    //    if (thuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT)
                    //    {
                    //        if (thuocThanhToanChiTiet != null)
                    //        {
                    //            keDuoPham = keDuoPham + "<tr style='border: 1px solid #020000;'>"
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 idex++
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                  GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 thuocThanhToanChiTiet?.DonViTinh.Ten
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuong)
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.DonGiaBan).ApplyFormatMoneyToDouble()
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.GiaBan).ApplyFormatMoneyToDouble()
                    //                                 + "</tr>";
                    //            tongTienPhaiTra += thuocThanhToanChiTiet.GiaBan;
                    //            //tongChiPhi = Convert.ToDecimal(thuocThanhToanChiTiet.SoTienBenhNhanDaChi);
                    //        }
                    //    }
                    //}

                    //BVHD-3943: gộp thuốc, VT
                    var thuocThanhToanChiTietKhongBHYTs = tkThu.TaiKhoanBenhNhanChis
                        .Where(p => p.DonThuocThanhToanChiTietId != null && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)
                        .Select(o => o.DonThuocThanhToanChiTiet).ToList();
                    var groupThuocThanhToanChiTietKhongBHYTs = thuocThanhToanChiTietKhongBHYTs.GroupBy(o => new { o.DuocPhamId, o.DonGiaBan }).ToList();
                    foreach (var thuocThanhToanChiTiet in groupThuocThanhToanChiTietKhongBHYTs)
                    {
                        keDuoPhamKBHYT = keDuoPhamKBHYT + "<tr style='border: 1px solid #020000;'>"
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     idex++
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                      GetFormatDuocPham(thuocThanhToanChiTiet.Key.DuocPhamId)
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     thuocThanhToanChiTiet.First().DonViTinh.Ten
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     GetFormatSoLuong(thuocThanhToanChiTiet.Key.DuocPhamId, thuocThanhToanChiTiet.Sum(o=>o.SoLuong))
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Key.DonGiaBan).ApplyFormatMoneyToDouble()
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Sum(o=>o.GiaBan)).ApplyFormatMoneyToDouble()
                                                     + "</tr>";
                        tongTienPhaiTra += thuocThanhToanChiTiet.Sum(o => o.GiaBan);
                    }

                    //foreach (var thuocThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTiet))
                    //{
                    //    if (thuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)
                    //    {
                    //        if (thuocThanhToanChiTiet != null)
                    //        {
                    //            keDuoPhamKBHYT = keDuoPhamKBHYT + "<tr style='border: 1px solid #020000;'>"
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 idex++
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                  GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 thuocThanhToanChiTiet?.DonViTinh.Ten
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuong)
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.DonGiaBan).ApplyFormatMoneyToDouble()
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.GiaBan).ApplyFormatMoneyToDouble()
                    //                                 + "</tr>";
                    //            tongTienPhaiTra += thuocThanhToanChiTiet.GiaBan;
                    //            //tongChiPhi = Convert.ToDecimal(thuocThanhToanChiTiet.SoTienBenhNhanDaChi);
                    //        }
                    //    }
                    //}
                    if (keDuoPham != "" && keDuoPhamKBHYT != "")
                    {
                        keToaThuocBHKBH = headerDPBHYT + keDuoPham + headerDPKhongBHYT + keDuoPhamKBHYT;
                    }
                    else if (keDuoPham != "" && keDuoPhamKBHYT == "")
                    {
                        keToaThuocBHKBH = headerDPBHYT + keDuoPham;
                    }
                    else if (keDuoPham == "" && keDuoPhamKBHYT != "")
                    {
                        keToaThuocBHKBH = headerDPKhongBHYT + keDuoPhamKBHYT;
                    }
                    else if (keDuoPham == "" && keDuoPhamKBHYT == "")
                    {
                        keToaThuocBHKBH = "";
                    }

                }
                if (tkThu.TaiKhoanBenhNhanChis.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(o => o.DonVTYTThanhToanChiTiet).Any())
                {
                    //BVHD-3943: gộp thuốc, VT
                    var vatTuThanhToanChiTiets = tkThu.TaiKhoanBenhNhanChis.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(o => o.DonVTYTThanhToanChiTiet).ToList();
                    var groupVatTuThanhToanChiTiets = vatTuThanhToanChiTiets.GroupBy(o => new { o.Ten, o.DonGiaBan }).ToList();
                    foreach (var vatTuThanhToanChiTiet in groupVatTuThanhToanChiTiets)
                    {
                        keVatTu += "<tr style='border: 1px solid #020000;'>"
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                idex++
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                vatTuThanhToanChiTiet.Key.Ten
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                vatTuThanhToanChiTiet.First().DonViTinh
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                vatTuThanhToanChiTiet.Sum(o=>o.SoLuong)
                                                + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                Convert.ToDouble(vatTuThanhToanChiTiet.Key.DonGiaBan).ApplyFormatMoneyToDouble()
                                                + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                Convert.ToDouble(vatTuThanhToanChiTiet.Sum(o=>o.GiaBan)).ApplyFormatMoneyToDouble()
                                                + "</tr>";
                        tongTienPhaiTra += vatTuThanhToanChiTiet.Sum(o => o.GiaBan);
                    }

                    //foreach (var vatTuThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Where(x => x.DonVTYTThanhToanChiTietId != null).Select(o => o.DonVTYTThanhToanChiTiet))
                    //{
                    //    if (vatTuThanhToanChiTiet != null)
                    //    {
                    //        keVatTu += "<tr style='border: 1px solid #020000;'>"
                    //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                            idex++
                    //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                            vatTuThanhToanChiTiet?.Ten
                    //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                            vatTuThanhToanChiTiet?.DonViTinh
                    //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                            vatTuThanhToanChiTiet?.SoLuong
                    //                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                            Convert.ToDouble(vatTuThanhToanChiTiet.DonGiaBan).ApplyFormatMoneyToDouble()
                    //                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                            Convert.ToDouble(vatTuThanhToanChiTiet.GiaBan).ApplyFormatMoneyToDouble()
                    //                            + "</tr>";
                    //        tongTienPhaiTra += vatTuThanhToanChiTiet.GiaBan;
                    //        //tongChiPhi += Convert.ToDecimal(vatTuThanhToanChiTiet.SoTienBenhNhanDaChi);
                    //    }
                    //}
                    keVatTuKBH = headerVTKhongBHYT + keVatTu;
                }


                var soChungTus = new List<string>();
                soChungTus.AddRange(tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null)
                                             .Select(o => o.DonThuocThanhToanChiTiet?.XuatKhoDuocPhamChiTietViTri?.XuatKhoDuocPhamChiTiet?.XuatKhoDuocPham?.SoPhieu)
                                             .Distinct());

                soChungTus.AddRange(tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null)
                                          .Select(o => o.DonVTYTThanhToanChiTiet?.XuatKhoVatTuChiTietViTri?.XuatKhoVatTuChiTiet?.XuatKhoVatTu?.SoPhieu)
                                          .Distinct());

                keToaThuoc = keToaThuocBHKBH + keVatTuKBH;
                tongTienToaThuoc = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble();
                soTienBangChu = "<i>Bằng chữ:" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra)) + ".</i";

                var result2 = _templateRepo.TableNoTracking
              .FirstOrDefault(x => x.Name.Equals("PhieuThu"));

                var data2 = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    NguoiNopTien = tkThu.YeuCauTiepNhan.HoTen,
                    //NamSinh = tkThu.YeuCauTiepNhan.NamSinh,

                    NamSinh = DateHelper.DOBFormat(tkThu.YeuCauTiepNhan?.NgaySinh, tkThu.YeuCauTiepNhan?.ThangSinh, tkThu.YeuCauTiepNhan?.NamSinh),
                    GioiTinh = tkThu.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    DiaChi = tkThu.YeuCauTiepNhan.DiaChiDayDu,
                    NoiDung = tkThu.NoiDungThu,
                    ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
                                          DateTime.Now.Year,
                    NguoiLapPhieu = tkThu.YeuCauTiepNhan.NhanVienTiepNhan?.User?.HoTen,
                    //update 21/06/2022 làm tròn tiền trên phiếu thu ApplyFormatMoneyToDouble -> RoundAndApplyFormatMoney
                    ChiPhiKCB = Convert.ToDouble(thongTinPhieuThuQuayThuoc.TongChiPhi).RoundAndApplyFormatMoney(),
                    BHYTT = Convert.ToDouble(thongTinPhieuThuQuayThuoc.BHYTThanhToan).RoundAndApplyFormatMoney(),
                    MiemGiam = Convert.ToDouble(thongTinPhieuThuQuayThuoc.MienGiam).RoundAndApplyFormatMoney(),

                    TienMat = Convert.ToDouble(thongTinPhieuThuQuayThuoc.TienMat).RoundAndApplyFormatMoney(),
                    ChuyenKhoan = Convert.ToDouble(thongTinPhieuThuQuayThuoc.ChuyenKhoan).RoundAndApplyFormatMoney(),
                    TongCongNo = Convert.ToDouble(thongTinPhieuThuQuayThuoc.CongNo).RoundAndApplyFormatMoney(),
                    ThanhTien = Convert.ToDouble(thongTinPhieuThuQuayThuoc.BenhNhanThanhToan).RoundAndApplyFormatMoney(),
                    Pos = Convert.ToDouble(thongTinPhieuThuQuayThuoc.Pos).RoundAndApplyFormatMoney(),

                    TongChiPhi = Convert.ToDouble(thongTinPhieuThuQuayThuoc.TongChiPhi).RoundAndApplyFormatMoney(),
                    //SoQuyen = tkThu.SoQuyen,
                    SoPhieu = tkThu.SoPhieuHienThi,
                    SoTienBangChu = NumberHelper.ChuyenSoRaText(Math.Round(Convert.ToDouble(thongTinPhieuThuQuayThuoc.TongChiPhi), MidpointRounding.AwayFromZero)), // chưa có,

                    GoiHienTai = DateTime.Now.ApplyFormatTime(),
                    PhieuThu = "PhieuThu",
                };



                var data1 = new
                {
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    NguoiNopTien = tkThu.YeuCauTiepNhan.HoTen,
                    //NamSinh = tkThu.YeuCauTiepNhan.NamSinh,
                    NamSinh = DateHelper.DOBFormat(tkThu.YeuCauTiepNhan?.NgaySinh, tkThu.YeuCauTiepNhan?.ThangSinh, tkThu.YeuCauTiepNhan?.NamSinh),
                    MaBn = tkThu.YeuCauTiepNhan?.BenhNhan?.MaBN,
                    NguoiThuTien = currentUserName,
                    GioiTinh = tkThu.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    DiaChi = tkThu.YeuCauTiepNhan.DiaChiDayDu,
                    DienDai = "Thu Tiền thuốc",
                    ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
                                           DateTime.Now.Year,
                    NguoiPhatThuoc = tkThu.YeuCauTiepNhan.NhanVienTiepNhan?.User?.HoTen,
                    TongTienPhaiTra = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble(),
                    keToaThuoc = keToaThuoc,
                    SoTienChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra)),
                    PhieuThu = "BangKeThuTienThuoc",
                    tongTienToaThuoc = tongTienToaThuoc,
                    soTienBangChu = soTienBangChu,
                    SoChungTu = soChungTus.Any() ? "Số:" + String.Join(",", soChungTus.Where(c => c != null)) : string.Empty,
                    GoiHienTai = DateTime.Now.ApplyFormatTime(),
                };
                if (data1.PhieuThu == "BangKeThuTienThuoc")
                {
                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result1.Body, data1) + "<div class=\"pagebreak\"></div>";
                }

                if (data2.PhieuThu == "PhieuThu")
                {
                    content += TemplateHelpper.FormatTemplateWithContentTemplate(result2.Body, data2);
                }
            }
            return content;
        }
        #endregion
        #region Phiếu lĩnh thuốc
        public async Task<string> XacNhanInThuocCoBhyt(XacNhanInThuocBhyt xacNhanIn)
        {
            var result = await _templateRepo.TableNoTracking
                .FirstOrDefaultAsync(x => x.Name.Equals("PhieuLinhThuoc"));

            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var currentUserName = await GetUserName(currentUserId);

            var keToaThuoc = string.Empty;

            var yeuCauTiepNhan = await _yeuCauTiepNhanRepo.GetByIdAsync(xacNhanIn.TiepNhanId,
                x => x
                    .Include(o => o.TinhThanh)
                    .Include(o => o.QuanHuyen)
                    .Include(o => o.PhuongXa)
                    .Include(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                    .Include(o => o.BenhNhan)
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.ChanDoanSoBoICD));

            var thuocs = await _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                          .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonViTinh)
                          .Where(o => o.YeuCauTiepNhanId == xacNhanIn.TiepNhanId && o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT &&
                                      o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan).ToListAsync();

            decimal tongTienPhaiTra = 0;
            int idex = 1;

            if (thuocs.Any())
            {
                var donThuocThanhToanChiTiets = thuocs.SelectMany(c => c.DonThuocThanhToanChiTiets);
                var groupDonThuocThanhToanChiTiets = donThuocThanhToanChiTiets.GroupBy(o => new { o.DuocPhamId, o.DonGiaBan }).ToList();
                //BVHD-3943: gộp thuốc, VT
                foreach (var thuocThanhToanChiTiet in groupDonThuocThanhToanChiTiets)
                {
                    keToaThuoc = keToaThuoc + "<tr style='border: 1px solid #020000;'>"
                                       + "<td style='border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                       idex++
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                       GetFormatDuocPham(thuocThanhToanChiTiet.Key.DuocPhamId)
                                       + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                       thuocThanhToanChiTiet.First().HamLuong
                                       + "<td style = 'border: 1px solid #020000;text-align: center;padding: 5px;'>" +
                                       thuocThanhToanChiTiet.First().DonViTinh.Ten
                                       + "<td style = 'border: 1px solid #020000;text-align: right;padding: 5px;'>" +
                                       GetFormatSoLuong(thuocThanhToanChiTiet.Key.DuocPhamId, thuocThanhToanChiTiet.Sum(o=>o.SoLuong));
                    tongTienPhaiTra += (decimal)thuocThanhToanChiTiet.Sum(o => o.SoLuong) * thuocThanhToanChiTiet.Key.DonGiaBan;
                }

                //foreach (var thuocThanhToanChiTiet in donThuocThanhToanChiTiets)
                //{
                //    keToaThuoc = keToaThuoc + "<tr style='border: 1px solid #020000;'>"
                //                       + "<td style='border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                //                       idex++
                //                       + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                //                       GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                //                       + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                //                       thuocThanhToanChiTiet.HamLuong
                //                       + "<td style = 'border: 1px solid #020000;text-align: center;padding: 5px;'>" +
                //                       thuocThanhToanChiTiet?.DonViTinh.Ten
                //                       + "<td style = 'border: 1px solid #020000;text-align: right;padding: 5px;'>" +
                //                       GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuong);
                //    tongTienPhaiTra += (decimal)thuocThanhToanChiTiet.SoLuong * thuocThanhToanChiTiet.DonGiaBan;
                //}
            }

            keToaThuoc += "<tr style='border: 1px solid #020000;'>"
                          + "<td colspan='5' style='padding: 5px;'><b>Cộng khoản: " + (idex - 1) + "</b>";

            var department = yeuCauTiepNhan.YeuCauKhamBenhs.LastOrDefault(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiThucHien != null);

            var departmentName = department?.NoiThucHien?.KhoaPhong?.Ten;

            var departmentCode = department?.NoiThucHien?.KhoaPhong?.Ma;

            var diagnose = yeuCauTiepNhan.YeuCauKhamBenhs.LastOrDefault(p =>
                p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && p.ChanDoanSoBoICD != null);

            var diagnoseText = diagnose?.ChanDoanSoBoGhiChu;

            var diagnoseCode = diagnose?.ChanDoanSoBoICD?.Ma;

            var data = new
            {
                LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                NguoiNopTien = yeuCauTiepNhan.HoTen.ToUpper(),
                NguoiThuTien = currentUserName,
                MaBn = yeuCauTiepNhan.BenhNhan?.MaBN,
                KhoaPhongKham = departmentName != null ? departmentName + " - " + departmentCode : null,
                GioLinh = DateTime.Now.Hour + "h" + DateTime.Now.Minute,
                NgayLinh = DateTime.Now.Date.ApplyFormatDate(),
                NamSinh = yeuCauTiepNhan.NamSinh,
                ChanDoan = diagnoseText,
                MaBenh = diagnoseCode,
                SoTheBhyt = yeuCauTiepNhan.BHYTMaSoThe,
                TuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc.GetValueOrDefault().ApplyFormatDate(),
                DenNgay = yeuCauTiepNhan.BHYTNgayHetHan.GetValueOrDefault().ApplyFormatDate(),
                GioiTinh = yeuCauTiepNhan.GioiTinh.GetDescription(),
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
                                       DateTime.Now.Year,
                NguoiPhatThuoc = yeuCauTiepNhan.NhanVienTiepNhan?.User?.HoTen,
                TongTienPhaiTra = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble(),
                keToaThuoc = keToaThuoc,
                SoTienChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra))
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }
        /*
        public async Task<string> XacNhanInThuocCoBhyt1(XacNhanInThuocBhyt xacNhanIn)
        {
            var result = await _templateRepo.TableNoTracking
                .FirstOrDefaultAsync(x => x.Name.Equals("PhieuLinhThuoc"));

            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var currentUserName = await GetUserName(currentUserId);

            var keToaThuoc = string.Empty;

            var yeuCauTiepNhan = await _yeuCauTiepNhanRepo.GetByIdAsync(xacNhanIn.TiepNhanId,
                x => x
                    .Include(o => o.TinhThanh)
                    .Include(o => o.QuanHuyen)
                    .Include(o => o.PhuongXa)
                    .Include(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                    .Include(o => o.BenhNhan)
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                    .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.ChanDoanSoBoICD));

            var thuocs = await _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                          .Include(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonViTinh)
                          .Where(o => o.YeuCauTiepNhanId == xacNhanIn.TiepNhanId && o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT &&
                                      o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan).ToListAsync();
            var tkThu = _taiKhoanBenhNhanThuRepository.GetById(xacNhanIn.TaiKhoanBenhNhanThuId,
                    x => x.Include(o => o.CongTyBaoHiemTuNhanCongNos).Include(o => o.MienGiamChiPhis)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.TinhThanh)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.QuanHuyen)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonViTinh)
                        .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonThuocThanhToan));


            decimal tongTienPhaiTra = 0;
            int idex = 1;

            foreach (var thuocThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Select(o => o.DonThuocThanhToanChiTiet))
            {
                if (thuocThanhToanChiTiet == null)
                {
                    continue;
                }
                keToaThuoc = keToaThuoc + "<tr style='border: 1px solid #020000;'>"
                                        + "<td style='border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                        idex++
                                        + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                        GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                                        + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                        thuocThanhToanChiTiet.HamLuong
                                        + "<td style = 'border: 1px solid #020000;text-align: center;padding: 5px;'>" +
                                        thuocThanhToanChiTiet?.DonViTinh.Ten
                                        + "<td style = 'border: 1px solid #020000;text-align: right;padding: 5px;'>" +
                                        GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuong);
                tongTienPhaiTra += (decimal)thuocThanhToanChiTiet.SoLuong * thuocThanhToanChiTiet.DonGiaBan;
            }

            keToaThuoc += "<tr style='border: 1px solid #020000;'>"
                          + "<td colspan='5' style='padding: 5px;'><b>Cộng khoản: " + (idex - 1) + "</b>";

            var department = yeuCauTiepNhan.YeuCauKhamBenhs.LastOrDefault(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiThucHien != null);

            var departmentName = department?.NoiThucHien?.KhoaPhong?.Ten;

            var departmentCode = department?.NoiThucHien?.KhoaPhong?.Ma;

            var diagnose = yeuCauTiepNhan.YeuCauKhamBenhs.LastOrDefault(p =>
                p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && p.ChanDoanSoBoICD != null);

            var diagnoseText = diagnose?.ChanDoanSoBoGhiChu;

            var diagnoseCode = diagnose?.ChanDoanSoBoICD?.Ma;

            var data = new
            {
                LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                NguoiNopTien = yeuCauTiepNhan.HoTen.ToUpper(),
                NguoiThuTien = currentUserName,
                MaBn = tkThu.YeuCauTiepNhan.BenhNhan.MaBN,
                KhoaPhongKham = departmentName != null ? departmentName + " - " + departmentCode : null,
                GioLinh = DateTime.Now.Hour + "h" + DateTime.Now.Minute,
                NgayLinh = DateTime.Now.Date.ApplyFormatDate(),
                NamSinh = yeuCauTiepNhan.NamSinh,
                ChanDoan = diagnoseText,
                MaBenh = diagnoseCode,
                SoTheBhyt = yeuCauTiepNhan.BHYTMaSoThe,
                TuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc.GetValueOrDefault().ApplyFormatDate(),
                DenNgay = yeuCauTiepNhan.BHYTNgayHetHan.GetValueOrDefault().ApplyFormatDate(),
                GioiTinh = yeuCauTiepNhan.GioiTinh.GetDescription(),
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
                                       DateTime.Now.Year,
                NguoiPhatThuoc = yeuCauTiepNhan.NhanVienTiepNhan?.User?.HoTen,
                TongTienPhaiTra = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble(),
                keToaThuoc = keToaThuoc,
                SoTienChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra))
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }
        */
        #endregion
        #region xác nhận xuất đơn thuốc
        public async Task<string> XacNhanXuatDonThuoc(DanhSachChoXuatThuocVO danhSachChoXuatThuoc)
        {
            var ycTiepNhan = await _yeuCauTiepNhanRepo.GetByIdAsync(danhSachChoXuatThuoc.Id, x => x.Include(o => o.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(dt => dt.NhapKhoDuocPhamChiTiet).ThenInclude(dt => dt.NhapKhoDuocPhams).ThenInclude(dt => dt.KhoDuocPhams)
                                                                                                                .Include(o => o.DonThuocThanhToans).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(dt => dt.XuatKhoDuocPhamChiTiet)
                                                                                                                .Include(o => o.DonVTYTThanhToans).ThenInclude(vt => vt.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet).ThenInclude(o => o.NhapKhoVatTu).ThenInclude(o => o.Kho)
                                                                                                                .Include(o => o.DonVTYTThanhToans).ThenInclude(vt => vt.DonVTYTThanhToanChiTiets).ThenInclude(o => o.XuatKhoVatTuChiTietViTri).ThenInclude(dt => dt.XuatKhoVatTuChiTiet));

            var xuatKhoDuocPham = new XuatKhoDuocPham()
            {
                KhoXuatId = (int)Enums.EnumKhoDuocPham.KhoNhaThuoc,
                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
                TenNguoiNhan = ycTiepNhan.HoTen,
                NgayXuat = DateTime.Now,
                NguoiXuatId = _userAgentHelper.GetCurrentUserId()
            };
            var xuatKhoDuocPhamBHYT = new XuatKhoDuocPham()
            {
                KhoXuatId = (int)Enums.EnumKhoDuocPham.KhoThuocBHYT,
                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
                TenNguoiNhan = ycTiepNhan.HoTen,
                NgayXuat = DateTime.Now,
                NguoiXuatId = _userAgentHelper.GetCurrentUserId()
            };

            foreach (var dtBHYT in ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT && o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc))
            {
                bool duocXuat = false;
                foreach (var dtBHYTDonThuocThanhToanChiTiet in dtBHYT.DonThuocThanhToanChiTiets)
                {
                    if (danhSachChoXuatThuoc.ThuocBHYT.Where(o => o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select(o => o.Id).ToList().Contains(dtBHYTDonThuocThanhToanChiTiet.Id))
                    {
                        duocXuat = true;
                    }
                    else
                    {
                        if (duocXuat)
                            return "Danh sách thuốc không hợp lệ";
                    }
                }
                if (duocXuat)
                {
                    dtBHYT.ThoiDiemCapThuoc = DateTime.Now;
                    foreach (var donThuocThanhToanChiTiet in dtBHYT.DonThuocThanhToanChiTiets)
                    {
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NgayXuat = DateTime.Now;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.NgayXuat = DateTime.Now;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = xuatKhoDuocPhamBHYT;
                    }
                    dtBHYT.TrangThai = TrangThaiDonThuocThanhToan.DaXuatThuoc;
                }
            }
            //thuoc k bao hiểm
            //var tmp = ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc);
            foreach (var dtKhongBHYT in ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonThuocThanhToan.ChuaXuatThuoc))
            {
                bool duocXuat = false;
                foreach (var dtKhongBHYTDonThuocThanhToanChiTiet in dtKhongBHYT.DonThuocThanhToanChiTiets)
                {
                    if (danhSachChoXuatThuoc.ThuocKhongBHYT.Where(o => o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien).Select(o => o.Id).ToList().Contains(dtKhongBHYTDonThuocThanhToanChiTiet.Id))
                    {
                        duocXuat = true;
                    }
                    else
                    {
                        if (duocXuat)
                            return "Danh sách thuốc không hợp lệ";
                    }
                }
                if (duocXuat)
                {
                    dtKhongBHYT.ThoiDiemCapThuoc = DateTime.Now;
                    foreach (var donThuocThanhToanChiTiet in dtKhongBHYT.DonThuocThanhToanChiTiets)
                    {
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NgayXuat = DateTime.Now;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.NgayXuat = DateTime.Now;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = xuatKhoDuocPham;
                    }
                    dtKhongBHYT.TrangThai = TrangThaiDonThuocThanhToan.DaXuatThuoc;
                }

            }

            var xuatKhoVatTu = new XuatKhoVatTu()
            {
                KhoXuatId = (int)Enums.EnumKhoDuocPham.KhoNhaThuoc,
                LoaiXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan,
                LyDoXuatKho = EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
                TenNguoiNhan = ycTiepNhan.HoTen,
                NgayXuat = DateTime.Now,
                NguoiXuatId = _userAgentHelper.GetCurrentUserId()
            };
            foreach (var vTKhongBHYT in ycTiepNhan.DonVTYTThanhToans.Where(o => o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonVTYTThanhToan.ChuaXuatVTYT))
            {
                bool duocXuat = false;
                foreach (var dVTKhongBHYTDonThuocThanhToanChiTiet in vTKhongBHYT.DonVTYTThanhToanChiTiets)
                {
                    if (danhSachChoXuatThuoc.ThuocKhongBHYT.Where(o => o.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien).Select(o => o.Id).ToList().Contains(dVTKhongBHYTDonThuocThanhToanChiTiet.Id))
                    {
                        duocXuat = true;
                    }
                    else
                    {
                        if (duocXuat)
                            return "Danh sách thuốc không hợp lệ";
                    }
                }
                if (duocXuat)
                {
                    vTKhongBHYT.ThoiDiemCapVTYT = DateTime.Now;
                    foreach (var donVTYTThanhToanChiTiet in vTKhongBHYT.DonVTYTThanhToanChiTiets)
                    {
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NgayXuat = DateTime.Now;
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.NgayXuat = DateTime.Now;
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu = xuatKhoVatTu;
                    }
                    vTKhongBHYT.TrangThai = TrangThaiDonVTYTThanhToan.DaXuatVTYT;
                }
            }
            if (ycTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.MuaThuoc)
            {
                ycTiepNhan.TrangThaiYeuCauTiepNhan = EnumTrangThaiYeuCauTiepNhan.DaHoanTat;
            }
            await _yeuCauTiepNhanRepo.UpdateAsync(ycTiepNhan);
            return string.Empty;
        }
        #endregion
        public ThongTinBenhNhanGridVo GetThongTinBenhNhan(long maBN)
        {
            //todo: need improve
            var query = _yeuCauTiepNhanRepo.TableNoTracking
                .Where(x => x.BenhNhan.MaBN == Convert.ToString(maBN))
                .Select(s => new ThongTinBenhNhanGridVo
                {
                    MaBN = s.BenhNhan != null ? s.BenhNhan.MaBN : "",
                    BenhNhanId = s.BenhNhanId != null ? Convert.ToInt64(s.BenhNhanId) : 0,
                    HoTen = s.HoTen,
                    NamSinh = s.NamSinh.ToString(),
                    GioiTinhHienThi = s.GioiTinh.GetDescription(),
                    DiaChi = s.DiaChi,
                    DiaChiDayDu = s.DiaChiDayDu,
                    SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                    Email = s.Email,
                    PhuongXaId = s.PhuongXaId,
                    TinhThanhId = s.TinhThanhId,
                    QuanHuyenId = s.QuanHuyenId
                });
            return query.FirstOrDefault();
        }
        public ThongTinBenhNhanGridVo GetThongTinBenhNhanTN(long maTN)
        {
            //todo: need improve
            var query = _yeuCauTiepNhanRepo.TableNoTracking
                .Where(x => x.Id == maTN)
                .Select(s => new ThongTinBenhNhanGridVo
                {
                    MaBN = s.BenhNhan != null ? s.BenhNhan.MaBN : "",
                    BenhNhanId = s.BenhNhanId != null ? Convert.ToInt64(s.BenhNhanId) : 0,
                    HoTen = s.HoTen,
                    NamSinh = s.NamSinh.ToString(),
                    GioiTinhHienThi = s.GioiTinh.GetDescription(),
                    DiaChi = s.DiaChiDayDu,
                    SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                    Email = s.Email
                });
            return query.FirstOrDefault();
        }
        public async Task<KhachVangLaiNavigateVo> GetBenhNhan(long taiKhoanThuId)
        {
            var benhNhan = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(p => p.Id == taiKhoanThuId)
                .Select(p => new KhachVangLaiNavigateVo
                {
                    TaiKhoanBenhNhanId = p.TaiKhoanBenhNhanId,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    BenhNhanId = p.YeuCauTiepNhan.BenhNhanId
                }).FirstOrDefault();
            return benhNhan;
        }
        public async Task<List<TinhThanhTemplateVo>> GetPhuongXa(DropDownListRequestModel model)
        {
            //var quan = JsonConvert.DeserializeObject<TinhQuan>(model.ParameterDependencies);
            //var lstPhuongXa = await _donViHanhChinhRepo.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.PhuongXa && p.TrucThuocDonViHanhChinhId == quan.QuanHuyen
            //                                                                             && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
            //    .Take(200)
            //    .Select(p => new TinhThanhTemplateVo
            //    {
            //        DisplayName = p.Ma + " - " + p.Ten,
            //        Ten = p.Ten,
            //        Ma = p.Ma,
            //        KeyId = p.Id,
            //    })
            //    .ToListAsync();
            //return lstPhuongXa;
            var subId = CommonHelper.GetIdFromRequestDropDownList(model);

            if (subId == 0)
            {
                var lst = await _donViHanhChinhRepo.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.PhuongXa
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

            var lstPhuongXa = await _donViHanhChinhRepo.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.PhuongXa
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
        public async Task<List<TinhThanhTemplateVo>> GetQuanHuyen(DropDownListRequestModel model)
        {
            if (model.ParameterDependencies != null)
            {
                var tinh = JsonConvert.DeserializeObject<TinhQuan>(model.ParameterDependencies);
                if (tinh != null)
                {
                    var lstQuanHuyen = await _donViHanhChinhRepo.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.QuanHuyen && p.TrucThuocDonViHanhChinhId == tinh.TinhThanh && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
                   .Take(200)
                   .Select(p => new TinhThanhTemplateVo
                   {
                       DisplayName = p.Ma + " - " + p.Ten,
                       Ten = p.Ten,
                       Ma = p.Ma,
                       KeyId = p.Id,
                   })
                   .ToListAsync();

                    return lstQuanHuyen;
                }
                else
                {
                    var lstQuanHuyen = await _donViHanhChinhRepo.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.QuanHuyen && p.TrucThuocDonViHanhChinhId == tinh.TinhThanh && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
               .Take(200)
               .Select(p => new TinhThanhTemplateVo
               {
                   DisplayName = p.Ma + " - " + p.Ten,
                   Ten = p.Ten,
                   Ma = p.Ma,
                   KeyId = p.Id,
               })
               .ToListAsync();

                    return lstQuanHuyen;
                }
            }
            else
            {
                List<TinhThanhTemplateVo> list = new List<TinhThanhTemplateVo>();

                return list.ToList();
            }

        }
        public async Task<List<TinhThanhTemplateVo>> GetTinhThanh(DropDownListRequestModel model)
        {

            var lstTinhThanh = await _donViHanhChinhRepo.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.TinhThanhPho
                                                                                          && (p.Ten.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "") || p.TenVietTat.Contains(model.Query ?? "")))
                .Take(100)
                .Select(p => new TinhThanhTemplateVo
                {
                    DisplayName = p.Ma + " - " + p.Ten,
                    Ten = p.Ten,
                    Ma = p.Ma,
                    KeyId = p.Id,
                })
                .ToListAsync();

            return lstTinhThanh;
        }
        public async Task<List<TinhHuyenTemplateVo>> GetTinhHuyen(long phuongXaId)
        {
            var donViHanhChinhTrucThuocHuyen =
                await _donViHanhChinhRepo.TableNoTracking.FirstOrDefaultAsync(p => p.Id == phuongXaId);
            var donViHanhChinhTrucThuocHuyenId = donViHanhChinhTrucThuocHuyen.TrucThuocDonViHanhChinhId;

            var lstQuanHuyen = _donViHanhChinhRepo.TableNoTracking.Where(p =>
                   p.CapHanhChinh == Enums.CapHanhChinh.QuanHuyen && p.Id == donViHanhChinhTrucThuocHuyenId
                    )
                .Take(200)
                .Select(p => new TinhHuyenTemplateVo
                {
                    HuyenId = p.Id,
                    TenHuyen = p.Ten
                }).FirstOrDefault();
            var donViHanhChinhTrucThuocTinh =
                    await _donViHanhChinhRepo.TableNoTracking.FirstOrDefaultAsync(p => p.Id == lstQuanHuyen.HuyenId);
            var donViHanhChinhTrucThuocTinhId = donViHanhChinhTrucThuocTinh.TrucThuocDonViHanhChinhId;
            var lstTinhThanhHuyen = await _donViHanhChinhRepo.TableNoTracking.Where(p => p.CapHanhChinh == Enums.CapHanhChinh.TinhThanhPho && p.Id == donViHanhChinhTrucThuocTinhId
                                                                                          )
                .Take(100)
                .Select(p => new TinhHuyenTemplateVo
                {
                    HuyenId = lstQuanHuyen.HuyenId,
                    TenHuyen = lstQuanHuyen.TenHuyen,
                    TinhId = p.Id,
                    TenTinh = p.Ten
                })
                .ToListAsync();

            return lstTinhThanhHuyen;
        }
        #region export exel don thuoc trong ngay
        public virtual byte[] ExportDanhSachDonThuocTrongNgay(ICollection<DonThuocThanhToanGridVo> datas)
        {
            var queryInfo = new DonThuocThanhToanGridVo();
            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<DonThuocThanhToanGridVo>("STT", p => ind++)
            };
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH ĐƠN THUỐC TRONG NGÀY");

                    // set row

                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 25;
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 30;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 30;

                    worksheet.DefaultColWidth = 7;

                    //set column 
                    string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G", "H", "A", "I", "J", };
                    var worksheetTitle = SetColumnItems[0] + 1 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 1;
                    var worksheetTitleStatus = SetColumnItems[0] + 2 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 2;
                    var worksheetTitleNgay = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 3;
                    var worksheetTitleHeader = SetColumnItems[0] + 3 + ":" + SetColumnItems[SetColumnItems.Length - 1] + 6;

                    using (var range = worksheet.Cells[worksheetTitle])
                    {
                        range.Worksheet.Cells[worksheetTitle].Merge = true;
                        range.Worksheet.Cells[worksheetTitle].Value = "DANH SÁCH ĐƠN THUỐC TRONG NGÀY".ToUpper();
                        range.Worksheet.Cells[worksheetTitle].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitle].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitle].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleStatus])
                    {
                        range.Worksheet.Cells[worksheetTitleStatus].Merge = true;
                        //range.Worksheet.Cells[worksheetTitleStatus].Value = "Trạng thái: ";/*+ string.Join(", ", arrTrangThai);*/
                        range.Worksheet.Cells[worksheetTitleStatus].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleStatus].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleNgay])
                    {
                        range.Worksheet.Cells[worksheetTitleNgay].Merge = true;
                        //range.Worksheet.Cells[worksheetTitleNgay].Value = "Từ ngày: " + tuNgay?.ApplyFormatDate() + " - đến ngày: " + denNgay?.ApplyFormatDate();
                        range.Worksheet.Cells[worksheetTitleNgay].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.SetFromFont(new Font("Times New Roman", 14));
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Bold = true;
                        range.Worksheet.Cells[worksheetTitleNgay].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells[worksheetTitleHeader])
                    {
                        range.Worksheet.Cells[worksheetTitleHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells[worksheetTitleHeader].Style.Font.Bold = true;

                        string[,] SetColumns ={ { "A" , "Mã TN" }, { "B", "Mã BN" }, { "C", "Họ Tên" } , { "D", "Năm Sinh" },
                                    { "E", "Giới Tính" }, { "F", "Địa Chỉ" },{ "G", "Điện Thoại" },{ "H", "Đối Tượng" },{ "I", "TỔng Giá Trị Đơn Thuốc" },{ "J", "Trạng Thái" }};

                        for (int i = 0; i < SetColumns.Length / 2; i++)
                        {
                            var setColumn = ((SetColumns[i, 0]).ToString() + 4 + ":" + (SetColumns[i, 0]).ToString() + 6).ToString();
                            range.Worksheet.Cells[setColumn].Merge = true;
                            range.Worksheet.Cells[setColumn].Value = SetColumns[i, 1];
                        }

                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    var manager = new PropertyManager<DonThuocThanhToanGridVo>(requestProperties);
                    int index = 7;
                    var worksheetFirstLast = SetColumnItems[0] + index + ":" + SetColumnItems[SetColumnItems.Length - 1] + index;

                    int sttYCTN = 1;
                    foreach (var donThuocTrongNgay in datas)
                    {
                        manager.CurrentObject = donThuocTrongNgay;
                        manager.WriteToXlsx(worksheet, index);

                        worksheet.Cells["A" + index].Value = donThuocTrongNgay.MaTN;
                        worksheet.Cells["B" + index].Value = donThuocTrongNgay.MaBN;
                        worksheet.Cells["C" + index].Value = donThuocTrongNgay.HoTen;
                        worksheet.Cells["D" + index].Value = donThuocTrongNgay.NamSinh + "";
                        worksheet.Cells["E" + index].Value = donThuocTrongNgay.GioiTinhHienThi;
                        worksheet.Cells["F" + index].Value = donThuocTrongNgay.DiaChi;
                        worksheet.Cells["G" + index].Value = donThuocTrongNgay.SoDienThoai;
                        worksheet.Cells["H" + index].Value = donThuocTrongNgay.DoiTuong;
                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["I" + index].Value = Convert.ToDouble(donThuocTrongNgay.TongGiaTriDonThuoc).ApplyFormatMoneyToDouble();
                        worksheet.Cells["J" + index].Value = donThuocTrongNgay.TrangThaiHienThi;

                        for (int ii = 0; ii < SetColumnItems.Length; ii++)
                        {
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                        }

                        worksheet.Row(index).Height = 20.5;

                        var indexMain = index;
                        index++;
                        int sttItems = 1;


                        using (var range = worksheet.Cells["B" + index + ":H" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Font.Bold = true;
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Worksheet.Cells["B" + index + ":H" + index].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                            string[,] SetColumnLinhDuTrus = { { "B", "#" },{ "C", "Mã Đơn" }, { "D", "Ngày Kê Đơn" },{ "E", "Dịch Vụ Khám" } , { "F", "Bác Sĩ Khám" } ,
                                { "G", "Số Tiền" },{ "H", "Tình Trạng" }};

                            for (int i = 0; i < SetColumnLinhDuTrus.Length / 2; i++)
                            {
                                var setColumn = ((SetColumnLinhDuTrus[i, 0]).ToString() + index + ":" + (SetColumnLinhDuTrus[i, 0]).ToString() + index).ToString();
                                range.Worksheet.Cells[setColumn].Merge = true;
                                range.Worksheet.Cells[setColumn].Value = SetColumnLinhDuTrus[i, 1];
                            }
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        index++;
                        var donThuocTrongNgayBHYT = donThuocTrongNgay.ListChilDonThuocTrongNgay.Where(s => s.LoaiDonThuoc == "Thuốc BHYT")
                           .Select(k => new DonThuocCuaBenhNhanGridVo()
                           {
                               MaDon = k.MaDon,
                               NgayKeDon = k.NgayKeDon,
                               DVKham = k.DVKham,
                               BSKham = k.BSKham,
                               SoTien = k.SoTien,
                               TinhTrang = k.TinhTrang
                           }).ToList();
                        var donThuocTrongNgayKhongBHYT = donThuocTrongNgay.ListChilDonThuocTrongNgay.Where(s => s.LoaiDonThuoc == "Thuốc Không BHYT")
                            .Select(k => new DonThuocCuaBenhNhanGridVo()
                            {
                                MaDon = k.MaDon,
                                NgayKeDon = k.NgayKeDon,
                                DVKham = k.DVKham,
                                BSKham = k.BSKham,
                                SoTien = k.SoTien,
                                TinhTrang = k.TinhTrang
                            }).ToList();
                        if (donThuocTrongNgayBHYT.Count() > 0)
                        {
                            using (var range = worksheet.Cells["B" + index + ":H" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                //Set column A to K
                                string[,] SetColumnLoaiDuocPham = { { "B", "BHYT" } };

                                for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;
                            foreach (var nhom in donThuocTrongNgayBHYT)
                            {
                                worksheet.Cells["B" + index].Value = sttItems++;
                                worksheet.Cells["C" + index].Value = nhom.MaDon;  // to do
                                worksheet.Cells["D" + index].Value = nhom.NgayKeDon;
                                worksheet.Cells["E" + index].Value = nhom.DVKham;
                                worksheet.Cells["F" + index].Value = nhom.BSKham;
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Value = nhom.SoTien.ApplyFormatMoneyToDouble();
                                worksheet.Cells["H" + index].Value = nhom.TinhTrang;

                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                            }
                        }
                        if (donThuocTrongNgayKhongBHYT.Count() > 0)
                        {
                            using (var range = worksheet.Cells["B" + index + ":H" + index])
                            {
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Font.Bold = true;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Worksheet.Cells["B" + index + ":B" + index].Style.Fill.BackgroundColor.SetColor(Color.RosyBrown);
                                //Set column A to K
                                string[,] SetColumnLoaiDuocPham = { { "B", "Không BHYT" } };

                                for (int i = 0; i < SetColumnLoaiDuocPham.Length / 2; i++)
                                {
                                    var setColumn = ((SetColumnLoaiDuocPham[i, 0]).ToString() + index + ":" + (SetColumnLoaiDuocPham[i, 0]).ToString() + index).ToString();
                                    range.Worksheet.Cells[setColumn].Merge = true;
                                    range.Worksheet.Cells[setColumn].Value = SetColumnLoaiDuocPham[i, 1];
                                }

                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            index++;
                            foreach (var nhom in donThuocTrongNgayKhongBHYT)
                            {
                                worksheet.Cells["B" + index].Value = sttItems++;
                                worksheet.Cells["C" + index].Value = nhom.MaDon;  // to do
                                worksheet.Cells["D" + index].Value = nhom.NgayKeDon;
                                worksheet.Cells["E" + index].Value = nhom.DVKham;
                                worksheet.Cells["F" + index].Value = nhom.BSKham;
                                worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["G" + index].Value = nhom.SoTien.ApplyFormatMoneyToDouble();
                                worksheet.Cells["H" + index].Value = nhom.TinhTrang;

                                for (int ii = 0; ii < SetColumnItems.Length; ii++)
                                {
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                index++;
                            }
                        }
                        for (int inde = indexMain + 1; inde <= index - 1; inde++)
                        {
                            worksheet.Row(inde).OutlineLevel = 1;
                        }

                        index++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        #endregion

        public bool LoaiYCTN(long tiepNhanId)
        {
            var queryLoaiYCTN = _yeuCauTiepNhanRepo.TableNoTracking.Where(s => s.Id == tiepNhanId).Select(d => d.LoaiYeuCauTiepNhan);
            if (queryLoaiYCTN.Any())
            {
                return (Enums.EnumLoaiYeuCauTiepNhan)(queryLoaiYCTN.First()) == Enums.EnumLoaiYeuCauTiepNhan.MuaThuoc ? true : false;
            }
            return false;
        }

        public string GetFormatDuocPham(long duocPhamId)
        {
            var tenFormat = string.Empty;
            //GetThongTinDuocPham
            if (duocPhamId != null)
            {
                var getDataDuocPham = _duocPhamnRepo.TableNoTracking.Where(d => d.Id == duocPhamId)
                                       .Select(d => new GetThongTinDuocPham
                                       {
                                           Ten = d.Ten,
                                           HoatChat = d.HoatChat,
                                           DuocPhamBenhVienPhanNhomId = d.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                           HamLuong = d.HamLuong,
                                           LoaiThuocTheoQuanLy = d.DuocPhamBenhVien.LoaiThuocTheoQuanLy
                                       });
                if (getDataDuocPham.Any())
                {
                    tenFormat = _yeuCauKhamBenhService.FormatTenDuocPham(getDataDuocPham.First().Ten, getDataDuocPham.First().HoatChat, getDataDuocPham.First().HamLuong, getDataDuocPham.First().DuocPhamBenhVienPhanNhomId);
                }
            }
            return tenFormat;
        }
        public string GetFormatSoLuong(long duocPhamId, double soLuong)
        {
            var tenFormat = string.Empty;
            //GetThongTinDuocPham
            if (duocPhamId != null)
            {
                var getDataDuocPham = _duocPhamnRepo.TableNoTracking.Where(d => d.Id == duocPhamId)
                                       .Select(d => new GetThongTinDuocPham
                                       {
                                           Ten = d.Ten,
                                           HoatChat = d.HoatChat,
                                           DuocPhamBenhVienPhanNhomId = d.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                                           HamLuong = d.HamLuong,
                                           LoaiThuocTheoQuanLy = d.DuocPhamBenhVien.LoaiThuocTheoQuanLy
                                       });
                if (getDataDuocPham.Any())
                {
                    tenFormat = _yeuCauKhamBenhService.FormatSoLuong(soLuong, getDataDuocPham.First().LoaiThuocTheoQuanLy);
                }
            }
            return tenFormat;
        }

        #region in phiếu xem trước bảng kê

        public async Task<string> XemTruocBangKeThuoc(XemTruocBangKeThuoc xemTruocBangKeThuoc)
        {
            var content = "";
            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var currentUserName = await GetUserName(currentUserId);

            var groupVTKhongBHYT = "Vật Tư Không BHYT";
            var headerVTKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupVTKhongBHYT.ToUpper()
                                        + "</b></tr>";

            var groupThuocKhongBHYT = "Dược Phẩm Không BHYT";
            var headerDPKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                        + "</b></tr>";

            var result = _templateRepo.TableNoTracking.FirstOrDefault(x => x.Name.Equals("BangKeThuTienThuoc"));


            var tongTienToaThuoc = string.Empty;
            var soTienBangChu = string.Empty;
            var keToaThuoc = string.Empty;
            var keToaThuocBHKBH = string.Empty;
            var keDuoPham = string.Empty;
            var keDuoPhamKBHYT = string.Empty;
            var keVatTu = string.Empty;
            var keVatTuKBH = string.Empty;
            decimal tongTienPhaiTra = 0;
            int idex = 1;

            var danhSachDuocPhamBenhViens = xemTruocBangKeThuoc.DanhSachDonThuocs.Where(c => c.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.DuocPhamBenhVien);
            var danhSachVatTuBenhViens = xemTruocBangKeThuoc.DanhSachDonThuocs.Where(c => c.LoaiDuocPhamHoacVatTu == LoaiDuocPhamHoacVatTu.VatTuBenhVien);

            if (danhSachDuocPhamBenhViens.Any())
            {
                //BVHD-3943: gộp thuốc, VT
                var groupDanhSachDuocPhamBenhViens = danhSachDuocPhamBenhViens.GroupBy(o => new { o.DuocPhamId, o.DonGia }).ToList();
                foreach (var thuocThanhToanChiTiet in groupDanhSachDuocPhamBenhViens)
                {                    
                    keDuoPhamKBHYT += keDuoPham + "<tr style='border: 1px solid #020000;'>"
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                            idex++
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                            GetFormatDuocPham(thuocThanhToanChiTiet.Key.DuocPhamId)
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                            thuocThanhToanChiTiet.First().DonViTinh
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                            GetFormatSoLuong(thuocThanhToanChiTiet.Key.DuocPhamId, thuocThanhToanChiTiet.Sum(o=>o.SoLuongMua))
                                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                            Convert.ToDouble(thuocThanhToanChiTiet.Key.DonGia).ApplyFormatMoneyToDouble()
                                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                            Convert.ToDouble((decimal)thuocThanhToanChiTiet.Sum(o => o.SoLuongMua) * thuocThanhToanChiTiet.Key.DonGia).ApplyFormatMoneyToDouble()
                                            + "</tr>";
                    tongTienPhaiTra += (decimal)thuocThanhToanChiTiet.Sum(o => o.SoLuongMua) * thuocThanhToanChiTiet.Key.DonGia;                    
                }
                //foreach (var thuocThanhToanChiTiet in danhSachDuocPhamBenhViens)
                //{
                //    if (thuocThanhToanChiTiet != null)
                //    {
                //        keDuoPhamKBHYT += keDuoPham + "<tr style='border: 1px solid #020000;'>"
                //                             + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                //                             idex++
                //                             + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                //                             GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                //                             + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                //                             thuocThanhToanChiTiet?.DonViTinh
                //                             + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                //                             GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuongMua)
                //                             + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                //                             Convert.ToDouble(thuocThanhToanChiTiet.DonGia).ApplyFormatMoneyToDouble()
                //                             + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                //                             Convert.ToDouble((decimal)thuocThanhToanChiTiet?.SoLuongMua * thuocThanhToanChiTiet.DonGia).ApplyFormatMoneyToDouble()
                //                             + "</tr>";
                //        tongTienPhaiTra += (decimal)thuocThanhToanChiTiet?.SoLuongMua * thuocThanhToanChiTiet.DonGia;
                //    }
                //}

                keToaThuocBHKBH = headerDPKhongBHYT + keDuoPhamKBHYT;

            }

            if (danhSachVatTuBenhViens.Any())
            {
                //BVHD-3943: gộp thuốc, VT
                var groupDanhSachDuocPhamBenhViens = danhSachVatTuBenhViens.GroupBy(o => new { o.TenDuocPham, o.DonGia }).ToList();
                foreach (var vatTuThanhToanChiTiet in groupDanhSachDuocPhamBenhViens)
                {                    
                    keVatTu += "<tr style='border: 1px solid #020000;'>"
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        idex++
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        vatTuThanhToanChiTiet.Key.TenDuocPham
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        vatTuThanhToanChiTiet.First().DonViTinh
                                        + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                        vatTuThanhToanChiTiet.Sum(o => o.SoLuongMua)
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        Convert.ToDouble(vatTuThanhToanChiTiet.Key.DonGia).ApplyFormatMoneyToDouble()
                                        + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                        Convert.ToDouble((decimal)vatTuThanhToanChiTiet.Sum(o => o.SoLuongMua) * vatTuThanhToanChiTiet.Key.DonGia).ApplyFormatMoneyToDouble()
                                        + "</tr>";

                    tongTienPhaiTra += (decimal)vatTuThanhToanChiTiet.Sum(o => o.SoLuongMua) * vatTuThanhToanChiTiet.Key.DonGia;
                }
                //foreach (var vatTuThanhToanChiTiet in danhSachVatTuBenhViens)
                //{
                //    if (vatTuThanhToanChiTiet != null)
                //    {
                //        keVatTu += "<tr style='border: 1px solid #020000;'>"
                //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                //                            idex++
                //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                //                            vatTuThanhToanChiTiet?.TenDuocPham
                //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                //                            vatTuThanhToanChiTiet?.DonViTinh
                //                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                //                            vatTuThanhToanChiTiet?.SoLuongMua
                //                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                //                            Convert.ToDouble(vatTuThanhToanChiTiet.DonGia).ApplyFormatMoneyToDouble()
                //                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                //                            Convert.ToDouble((decimal)vatTuThanhToanChiTiet?.SoLuongMua * vatTuThanhToanChiTiet.DonGia).ApplyFormatMoneyToDouble()
                //                            + "</tr>";

                //        tongTienPhaiTra += (decimal)vatTuThanhToanChiTiet?.SoLuongMua * vatTuThanhToanChiTiet.DonGia;

                //    }
                //}
                keVatTuKBH = headerVTKhongBHYT + keVatTu;
            }

            keToaThuoc = keToaThuocBHKBH + keVatTuKBH;
            soTienBangChu = "<i>Bằng chữ:" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra)) + ".</i";


            var thongTinBenhNhan = _yeuCauTiepNhanRepo.TableNoTracking.Where(c => c.Id == xemTruocBangKeThuoc.YeuCauTiepNhanId).Include(c => c.BenhNhan).FirstOrDefault();

            var data = new
            {
                LogoUrl = xemTruocBangKeThuoc.Hosting + "/assets/img/logo-bacha-full.png",

                NguoiNopTien = thongTinBenhNhan != null ? thongTinBenhNhan.HoTen : xemTruocBangKeThuoc.KhachVangLai,
                NamSinh = thongTinBenhNhan != null ? DateHelper.DOBFormat(thongTinBenhNhan?.NgaySinh, thongTinBenhNhan?.ThangSinh, thongTinBenhNhan?.NamSinh) : xemTruocBangKeThuoc.NamSinh.ToString(),
                MaBn = thongTinBenhNhan != null ? thongTinBenhNhan.BenhNhan.MaBN : string.Empty,
                GioiTinh = thongTinBenhNhan != null ? thongTinBenhNhan.GioiTinh.GetDescription() : xemTruocBangKeThuoc.GioiTinh.GetDescription(),
                DiaChi = thongTinBenhNhan != null ? thongTinBenhNhan?.DiaChiDayDu : xemTruocBangKeThuoc.DiaChi,

                NguoiThuTien = currentUserName,
                DienDai = "Thu Tiền thuốc",
                ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year,
                NguoiPhatThuoc = currentUserName,
                TongTienPhaiTra = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble(),
                keToaThuoc = keToaThuoc,
                SoTienChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra)),
                PhieuThu = "BangKeThuTienThuoc",
                tongTienToaThuoc = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble(),
                soTienBangChu = soTienBangChu,
                SoChungTu = string.Empty,
                GoiHienTai = DateTime.Now.ApplyFormatTime(),
            };

            if (data.PhieuThu == "BangKeThuTienThuoc")
            {
                content += TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data) + "<div class=\"pagebreak\"></div>";
            }

            return content;
        }

        #endregion
    }
}

