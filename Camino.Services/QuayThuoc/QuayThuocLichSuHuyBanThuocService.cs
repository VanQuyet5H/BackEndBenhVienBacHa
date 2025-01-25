using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.BenhNhans;
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
using static Camino.Core.Domain.Enums;
using XuatKhoDuocPham = Camino.Core.Domain.Entities.XuatKhos.XuatKhoDuocPham;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Services.Helpers;
using Camino.Services.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;

namespace Camino.Services.QuayThuoc
{
    [ScopedDependency(ServiceType = typeof(IQuayThuocLichSuHuyBanThuocService))]
    public class QuayThuocLichSuHuyBanThuocService : MasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>, IQuayThuocLichSuHuyBanThuocService
    {
        private readonly IRepository<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuRepository;
        private readonly IRepository<XuatKhoDuocPham> _xuatKhoDuocPhamRepository;
        private readonly IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepo;
        private readonly IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiRepository;
        private readonly IRepository<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTietRepository;
        private readonly IRepository<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTietRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<BenhNhan> _benhNhanRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Camino.Core.Domain.Entities.Users.User> _userRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepo;
        private readonly IRepository<DonThuocThanhToan> _donThuocThanhToanRepo;
        private readonly IRepository<DonVTYTThanhToan> _donVTYTThanhToanRepo;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IRepository<DuocPham> _duocPhamnRepo;

        public QuayThuocLichSuHuyBanThuocService(IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> repository,
                                                IRepository<TaiKhoanBenhNhanThu> taiKhoanBenhNhanThuRepository,
                                                IRepository<XuatKhoDuocPham> xuatKhoDuocPhamRepository,
                                                IRepository<XuatKhoVatTu> xuatKhoVatTuRepository,
                                                IRepository<DonThuocThanhToanChiTiet> donThuocThanhToanChiTietRepository,
                                                IRepository<DonVTYTThanhToanChiTiet> donVTYTThanhToanChiTietRepository,
                                                IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepo,
                                                IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChiRepository, IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepo,
                                                IRepository<DonThuocThanhToan> donThuocThanhToanRepo,
                                                IRepository<BenhNhan> benhNhanRepository,
                                                IRepository<Template> templateRepository,
                                                IUserAgentHelper userAgentHelper,
                                                IRepository<Camino.Core.Domain.Entities.Users.User> userRepository,
                                                IRepository<DonVTYTThanhToan> donVTYTThanhToanRepo,
                                                IYeuCauKhamBenhService yeuCauKhamBenhService,
                                                IRepository<DuocPham> duocPhamnRepo) : base(repository)
        {
            _taiKhoanBenhNhanThuRepository = taiKhoanBenhNhanThuRepository;
            _donThuocThanhToanChiTietRepository = donThuocThanhToanChiTietRepository;
            _donVTYTThanhToanChiTietRepository = donVTYTThanhToanChiTietRepository;
            _yeuCauKhamBenhRepo = yeuCauKhamBenhRepo;
            _taiKhoanBenhNhanChiRepository = taiKhoanBenhNhanChiRepository;
            _yeuCauTiepNhanRepo = yeuCauTiepNhanRepo;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _donThuocThanhToanRepo = donThuocThanhToanRepo;
            _donVTYTThanhToanRepo = donVTYTThanhToanRepo;
            _benhNhanRepository = benhNhanRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _donVTYTThanhToanChiTietRepository = donVTYTThanhToanChiTietRepository;
            _templateRepository = templateRepository;
            _userAgentHelper = userAgentHelper;
            _userRepository = userRepository;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _duocPhamnRepo = duocPhamnRepo;
        }


        #region Lịch sử bán thuốc
        private void BuildSortGetDataForGridLichSuHuyBanThuoc(IQueryInfo queryInfo, params string[] sortFields)
        {
            queryInfo.Sort = queryInfo.Sort.Where(o => sortFields.Contains(o.Field)).ToList();

            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }
        }
        public async Task<GridDataSource> GetDataForGridLichSuHuyBanThuocAsync(QueryInfo queryInfo, bool isPrint)
        {
            // DS YCTN Đã hoàn thành hoặc hủy // // so tien thu  tinh cong no va mien giam
            BuildSortGetDataForGridLichSuHuyBanThuoc(queryInfo, "SoDon", "MaBN", "MaTN", "NgayThu", "HoTen", "NamSinh", "SoDienThoaiDisplay", "DoiTuong");

            DateTime denNgay = DateTime.Now;
            DateTime tuNgay = DateTime.Now.AddYears(-1);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuHuyBanThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate))
                {
                    queryString.FromDate.TryParseExactCustom(out var tuNgayValue);
                    tuNgay = tuNgayValue;
                }
                if (!string.IsNullOrEmpty(queryString.ToDate))
                {
                    queryString.ToDate.TryParseExactCustom(out var denNgayValue);
                    denNgay = denNgayValue.AddSeconds(59).AddMilliseconds(999);
                }
            }

            var querydataThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
            .Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && o.NgayThu >= tuNgay && o.NgayThu <= denNgay && o.DaHuy == true)
            .Select(o => new
            {
                TaiKhoanBenhNhanThuId = o.Id,
                Id = o.YeuCauTiepNhanId,
                MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                NgayThu = o.NgayThu,
                HoTen = o.YeuCauTiepNhan.HoTen,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinh = o.YeuCauTiepNhan.GioiTinh,
                DiaChi = o.YeuCauTiepNhan.DiaChiDayDu,
                SoDienThoai = o.YeuCauTiepNhan.SoDienThoai,
                SoDienThoaiDisplay = o.YeuCauTiepNhan.SoDienThoaiDisplay,
                DoiTuong = o.YeuCauTiepNhan.CoBHYT,
                BHYTMucHuong = o.YeuCauTiepNhan.BHYTMucHuong,
                SoDon = o.SoPhieuHienThi,
                SoTienVatTu = 0,
                SoTienDuocPham = 0,
                Pos = o.POS.GetValueOrDefault(),
                TienMat = o.TienMat.GetValueOrDefault(),
                ChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                CongNo = o.CongNo.GetValueOrDefault(),
                NgayHuy = o.NgayHuy,
                NguoiHuy = o.NhanVienHuy.User.HoTen,
                LyDoHuyThu = o.LyDoHuy,
                PhieuChis = o.TaiKhoanBenhNhanChis.Select(chi => new { LoaiChiTienBenhNhan = chi.LoaiChiTienBenhNhan, chi.SoLuong, Gia = chi.Gia }).ToList()
            }).ApplyLike(queryInfo.SearchTerms, g => g.MaBN, g => g.HoTen, g => g.MaTN, g => g.SoDon, g => g.SoDienThoai, g => g.SoDienThoaiDisplay);

            var totalRowCount = queryInfo.LazyLoadPage == true ? 0 : querydataThu.Count();
            if (isPrint)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var queryData = querydataThu.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
            var returnData = queryData.Select(o => new LichSuHuyBanThuocGridVo
            {
                TaiKhoanBenhNhanThuId = o.TaiKhoanBenhNhanThuId,
                Id = o.Id,
                MaBN = o.MaBN,
                MaTN = o.MaTN,
                NgayThu = o.NgayThu,
                NgayThuStr = o.NgayThu.ApplyFormatDateTimeSACH(),
                HoTen = o.HoTen,
                NamSinh = o.NamSinh,
                GioiTinhHienThi = o.GioiTinh != null ? o.GioiTinh.GetDescription() : "",
                DiaChi = o.DiaChi,
                SoDienThoaiDisplay = o.SoDienThoaiDisplay,
                SoDienThoai = o.SoDienThoai,
                DoiTuong = o.DoiTuong == true ? ("BHYT (" + (o.BHYTMucHuong != null ? o.BHYTMucHuong.ToString() : "") + "%)") : "Viện phí",
                SoDon = o.SoDon,
                SoTienVatTu = 0,
                Pos = o.Pos,
                TienMat = o.TienMat,
                ChuyenKhoan = o.ChuyenKhoan,
                CongNo = o.CongNo,
                NgayHuy = o.NgayHuy,
                NguoiHuy = o.NguoiHuy,
                LyDoHuyThu = o.LyDoHuyThu,
                SoTienDuocPham = o.PhieuChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => Math.Round(chi.Gia.GetValueOrDefault() * (decimal)chi.SoLuong.GetValueOrDefault(), 2)).DefaultIfEmpty().Sum()
            }).ToArray();

            return new GridDataSource { Data = returnData, TotalRowCount = totalRowCount };
        }

        public async Task<GridDataSource> GetTotalPageForGridLichSuHuyBanThuocAsync(QueryInfo queryInfo)
        {
            DateTime denNgay = DateTime.Now;
            DateTime tuNgay = DateTime.Now.AddYears(-1);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuHuyBanThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate))
                {
                    queryString.FromDate.TryParseExactCustom(out var tuNgayValue);
                    tuNgay = tuNgayValue;
                }
                if (!string.IsNullOrEmpty(queryString.ToDate))
                {
                    queryString.ToDate.TryParseExactCustom(out var denNgayValue);
                    denNgay = denNgayValue.AddSeconds(59).AddMilliseconds(999);
                }
            }

            var querydataThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
            .Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && o.NgayThu >= tuNgay && o.NgayThu <= denNgay && o.DaHuy == true)
            .Select(o => new
            {
                TaiKhoanBenhNhanThuId = o.Id,
                Id = o.YeuCauTiepNhanId,
                MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                NgayThu = o.NgayThu,
                SoDienThoai = o.YeuCauTiepNhan.SoDienThoai,
                SoDienThoaiDisplay = o.YeuCauTiepNhan.SoDienThoaiDisplay,
                HoTen = o.YeuCauTiepNhan.HoTen,
                SoDon = o.SoPhieuHienThi,
            }).ApplyLike(queryInfo.SearchTerms, g => g.MaBN, g => g.HoTen, g => g.MaTN, g => g.SoDon, g => g.SoDienThoai, g => g.SoDienThoaiDisplay);
            
            var totalRowCount = querydataThu.Count();

            return new GridDataSource { TotalRowCount = totalRowCount };
        }

        public async Task<GridDataSource> GetDataForGridLichSuHuyBanThuocAsyncOld(QueryInfo queryInfo, bool isPrint)
        {
            BuildDefaultSortExpression(queryInfo);

            var querydataThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans)
                .Include(x => x.TaiKhoanBenhNhanChis)
            .Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && o.DaHuy == true
                            && ((o.TaiKhoanBenhNhanChis.Where(p => p.DaHuy == true
                            && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).Any())
                            || (o.TaiKhoanBenhNhanChis.Where(p => p.DaHuy == true).Any())));

            var phieuThu = querydataThu.Select(o => new LichSuHuyBanThuocGridVo
            {
                TaiKhoanBenhNhanThuId = o.Id,
                Id = o.YeuCauTiepNhanId,
                MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                NgayThu = o.NgayThu,
                NgayThuStr = o.NgayThu != null ? o.NgayThu.ApplyFormatDateTimeSACH() : "",

                HoTen = o.YeuCauTiepNhan.BenhNhan.HoTen,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinhHienThi = o.YeuCauTiepNhan.BenhNhan.GioiTinh.GetDescription(),
                DiaChi = o.YeuCauTiepNhan.DiaChiDayDu,
                SoDienThoaiDisplay = o.YeuCauTiepNhan.SoDienThoai.ApplyFormatPhone(),
                SoDienThoai = o.YeuCauTiepNhan.SoDienThoai,

                DoiTuong = o.YeuCauTiepNhan.CoBHYT == true ? "BHYT (" + o.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                SoDon = o.SoPhieuHienThi,
                SoTienVatTu = 0,
                SoTienDuocPham = 0,
                Pos = o.POS.GetValueOrDefault(),
                TienMat = o.TienMat.GetValueOrDefault(),
                ChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),

                donDuocPham = o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null).Select(vc => vc.DonThuocThanhToanChiTiet).ToList(),
                donVatTu = o.TaiKhoanBenhNhanChis.Where(i => i.DonVTYTThanhToanChiTietId != null).Select(vc => vc.DonVTYTThanhToanChiTiet).ToList(),


                NgayHuy = o.NgayHuy,
                NguoiHuy = o.NhanVienHuy.User.HoTen,
                LyDoHuyThu = o.LyDoHuy

            }).ApplyLike(queryInfo.SearchTerms.RemoveUniKeyAndToLower(),
                        g => g.TaiKhoanBenhNhanThuId.ToString(),
                        g => g.NgayThu.ToString().RemoveUniKeyAndToLower(),
                        g => g.MaBN,
                        g => g.HoTen.RemoveUniKeyAndToLower(),
                        g => g.MaTN.RemoveUniKeyAndToLower(),
                        g => g.SoDienThoaiDisplay.ApplyFormatPhone(),
                        g => g.SoDienThoai,
                        g => g.NamSinh.ToString(),
                        g => g.GioiTinhHienThi,
                        g => g.DiaChi.RemoveUniKeyAndToLower(),
                        g => g.SoDon);

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuHuyBanThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }

                    phieuThu = phieuThu.Where(p => p.NgayThu >= tuNgay && p.NgayThu <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }

            var allData = new List<LichSuHuyBanThuocGridVo>();

            foreach (var itemx in phieuThu)
            {
                foreach (var itemxx in itemx.donDuocPham)
                {
                    itemx.SoTienDuocPham += itemxx.GiaBan;
                }
                foreach (var itemxx in itemx.donVatTu)
                {
                    itemx.SoTienVatTu += itemxx.GiaBan;
                }
                allData.Add(itemx);
            }

            var dataOrderBy = allData.AsQueryable().OrderBy(cc => cc.NgayThu).OrderBy(queryInfo.SortString);
            var quaythuoc = isPrint == true ? dataOrderBy.ToArray() : dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();

            return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };

        }

        public async Task<GridDataSource> GetTotalPageForGridLichSuHuyBanThuocAsyncOld(QueryInfo queryInfo)
        {

            var querydataThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
                 .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                 .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                 .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                 .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans)
                 .Include(x => x.TaiKhoanBenhNhanChis)
             .Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && o.DaHuy == true
                             && ((o.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null
                             && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc
                             && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).Any())
                             || (o.TaiKhoanBenhNhanChis.Where(p => p.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT).Any())));

            var phieuThu = querydataThu.Select(o => new LichSuHuyBanThuocGridVo
            {
                TaiKhoanBenhNhanThuId = o.Id,
                Id = o.YeuCauTiepNhanId,
                MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                NgayThu = o.NgayThu,
                NgayThuStr = o.NgayThu != null ? o.NgayThu.ApplyFormatDateTimeSACH() : "",

                HoTen = o.YeuCauTiepNhan.BenhNhan.HoTen,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinhHienThi = o.YeuCauTiepNhan.BenhNhan.GioiTinh.GetDescription(),
                DiaChi = o.YeuCauTiepNhan.DiaChiDayDu,
                SoDienThoaiDisplay = o.YeuCauTiepNhan.SoDienThoai.ApplyFormatPhone(),
                SoDienThoai = o.YeuCauTiepNhan.SoDienThoai,
                DoiTuong = o.YeuCauTiepNhan.CoBHYT == true ? "BHYT (" + o.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                SoDon = o.SoPhieu.ToString() ?? o.Id.ToString(),
                SoTienVatTu = 0,
                SoTienDuocPham = 0,
                Pos = o.POS.GetValueOrDefault(),
                TienMat = o.TienMat.GetValueOrDefault(),
                ChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                donDuocPham = o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null).Select(vc => vc.DonThuocThanhToanChiTiet).ToList(),
                donVatTu = o.TaiKhoanBenhNhanChis.Where(i => i.DonVTYTThanhToanChiTietId != null).Select(vc => vc.DonVTYTThanhToanChiTiet).ToList()
            }).ApplyLike(queryInfo.SearchTerms.RemoveUniKeyAndToLower(),
                    g => g.TaiKhoanBenhNhanThuId.ToString(),
                    g => g.NgayThu.ToString().RemoveUniKeyAndToLower(),
                    g => g.MaBN,
                    g => g.HoTen.RemoveUniKeyAndToLower(),
                    g => g.MaTN.RemoveUniKeyAndToLower(),
                    g => g.SoDienThoaiDisplay.ApplyFormatPhone(),
                    g => g.SoDienThoai,
                    g => g.NamSinh.ToString(),
                    g => g.GioiTinhHienThi,
                    g => g.DiaChi.RemoveUniKeyAndToLower(),
                    g => g.SoDon);

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuHuyBanThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                    }
                    phieuThu = phieuThu.Where(p => p.NgayThu >= tuNgay && p.NgayThu <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }
            var allData = new List<LichSuHuyBanThuocGridVo>();
            foreach (var itemx in phieuThu)
            {
                foreach (var itemxx in itemx.donDuocPham)
                {
                    itemx.SoTienDuocPham += itemxx.GiaBan;
                }
                foreach (var itemxx in itemx.donVatTu)
                {
                    itemx.SoTienVatTu += itemxx.GiaBan;
                }
                allData.Add(itemx);
            }
            var dataOrderBy = allData.AsQueryable().OrderBy(queryInfo.SortString);
            var countTask = dataOrderBy.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }

        public List<ThongTinDuocPhamQuayThuocVo> GetDanhSachThuocDaHuyThuocKhongBHYTLS(long tiepNhanId, long? idTaiKhoanBenhNhanThu)
        {

            var donThuocThanhToanChiTietTheoPhieuThus = _taiKhoanBenhNhanChiRepository.TableNoTracking
                                               .Where(c => c.TaiKhoanBenhNhanThuId == idTaiKhoanBenhNhanThu
                                                 && c.DonThuocThanhToanChiTietTheoPhieuThuId != null)
                                               .Select(x => x.DonThuocThanhToanChiTietTheoPhieuThu).Include(c => c.DonViTinh);

            var donThuocThanhToans = donThuocThanhToanChiTietTheoPhieuThus.Select(x => new ThongTinDuocPhamQuayThuocVo()
            {

                Id = x.Id,
                DuocPhamId = x.Id,
                MaHoatChat = x.HoatChat,
                NongDoVaHamLuong = x.HamLuong,
                TenDuocPham = x.Ten,
                //NhapKhoDuocPhamChiTietId = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Id,
                TenHoatChat = x.HoatChat,
                DonViTinh = x.DonViTinh.Ten,
                SoLuongToa = x.SoLuongToa ?? 0L,
                SoLuongMua = x.SoLuong,
                DonGiaNhap = x.DonGiaNhap,
                DonGia = x.DonGiaBan,
                TiLeTheoThapGia = x.TiLeTheoThapGia,
                VAT = x.VAT,
                Solo = x.Solo,
                //ViTri = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.KhoDuocPhamViTri.Ten,
                HanSuDung = x.HanSuDung,
                isNew = false,
                //TaiKhoanbenhNhanThuId = x.TaiKhoanBenhNhanChis.Select(p => p.TaiKhoanBenhNhanThuId).ToList(),
            });

            var donVTYTThanhToanChiTietTheoPhieuThus = _taiKhoanBenhNhanChiRepository.TableNoTracking
                .Where(c => c.TaiKhoanBenhNhanThuId == idTaiKhoanBenhNhanThu && c.DonVTYTThanhToanChiTietTheoPhieuThuId != null)
                .Select(x => x.DonVTYTThanhToanChiTietTheoPhieuThu);
            var donVTYTThanhToans = donVTYTThanhToanChiTietTheoPhieuThus
                 .Select(x => new ThongTinDuocPhamQuayThuocVo()
                 {
                     Id = x.Id,
                     DuocPhamId = x.Id,
                     TenDuocPham = x.Ten,
                     //NhapKhoDuocPhamChiTietId = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Id,
                     DonViTinh = x.DonViTinh,
                     SoLuongMua = x.SoLuong,
                     Solo = x.Solo,
                     //ViTri = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTri.Ten,
                     HanSuDung = x.HanSuDung,
                     isNew = false,
                     VatTuBHYT = x.SoTienBaoHiemTuNhanChiTra != null,
                     DonGiaNhap = x.DonGiaNhap,
                     DonGia = x.DonGiaBan,
                     TiLeTheoThapGia = x.TiLeTheoThapGia,
                     VAT = x.VAT,
                     //TaiKhoanbenhNhanThuId = x.TaiKhoanBenhNhanChis.Select(p => p.TaiKhoanBenhNhanThuId).ToList(),
                 }).Where(x => x.VatTuBHYT != true);

            var allData = new List<ThongTinDuocPhamQuayThuocVo>();

            allData.AddRange(donThuocThanhToans);
            allData.AddRange(donVTYTThanhToans);

            return allData.ToList();

        }


        #region in bảng kê hủy thuốc

        private async Task<string> GetUserName(long id)
        {
            var name = await _userRepository.TableNoTracking.Where(p => p.Id == id)
                .Select(p => p.HoTen).FirstOrDefaultAsync();
            return name;
        }

        public async Task<string> InBaoCaoToaThuocHuyBanAsync(long id, bool bangKe, bool thuTien, string hostingName)
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



            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("BangKeThuTienThuoc"));

            var tkThu = _taiKhoanBenhNhanThuRepository.GetById(id,
                x => x.Include(o => o.CongTyBaoHiemTuNhanCongNos).Include(o => o.MienGiamChiPhis)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.TinhThanh)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.QuanHuyen)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                    .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.BenhNhan)
                    .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonViTinh)
                    .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(o => o.DonThuocThanhToan)

                    .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonThuocThanhToanChiTietTheoPhieuThu).ThenInclude(o => o.DonViTinh)
                    .Include(o => o.TaiKhoanBenhNhanChis).ThenInclude(o => o.DonVTYTThanhToanChiTietTheoPhieuThu));


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
            decimal tongChiPhi = 0;


            if (tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietTheoPhieuThuId != null).Select(o => o.DonThuocThanhToanChiTietTheoPhieuThu).Any())
            {
                foreach (var thuocThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietTheoPhieuThuId != null)
                                                           .Select(o => o.DonThuocThanhToanChiTietTheoPhieuThu))
                {
                    keDuoPhamKBHYT = keDuoPhamKBHYT + "<tr style='border: 1px solid #020000;'>"
                                                  + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                  idex++
                                                  + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                   GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                                                  + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                  thuocThanhToanChiTiet?.DonViTinh.Ten
                                                  + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                  GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuong)
                                                  + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                  Convert.ToDouble(thuocThanhToanChiTiet.DonGiaBan).ApplyFormatMoneyToDouble()
                                                  + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                  Convert.ToDouble(thuocThanhToanChiTiet.GiaBan).ApplyFormatMoneyToDouble()
                                                  + "</tr>";
                    tongTienPhaiTra += thuocThanhToanChiTiet.GiaBan;
                    tongChiPhi = Convert.ToDecimal(thuocThanhToanChiTiet.SoTienBenhNhanDaChi);
                }


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

            if (tkThu.TaiKhoanBenhNhanChis.Where(x => x.DonVTYTThanhToanChiTietTheoPhieuThuId != null).Select(o => o.DonVTYTThanhToanChiTietTheoPhieuThu).Any())
            {
                foreach (var vatTuThanhToanChiTiet in tkThu.TaiKhoanBenhNhanChis.Where(x => x.DonVTYTThanhToanChiTietTheoPhieuThuId != null)
                                                           .Select(o => o.DonVTYTThanhToanChiTietTheoPhieuThu))
                {
                    if (vatTuThanhToanChiTiet != null)
                    {
                        keVatTu += "<tr style='border: 1px solid #020000;'>"
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                            idex++
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                            vatTuThanhToanChiTiet?.Ten
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                            vatTuThanhToanChiTiet?.DonViTinh
                                            + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                            vatTuThanhToanChiTiet?.SoLuong
                                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                            Convert.ToDouble(vatTuThanhToanChiTiet.DonGiaBan).ApplyFormatMoneyToDouble()
                                            + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                            Convert.ToDouble(vatTuThanhToanChiTiet.GiaBan).ApplyFormatMoneyToDouble()
                                            + "</tr>";
                        tongTienPhaiTra += vatTuThanhToanChiTiet.GiaBan;
                        tongChiPhi += Convert.ToDecimal(vatTuThanhToanChiTiet.SoTienBenhNhanDaChi);
                    }
                }
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
                SoChungTu = soChungTus.Any() ? "Số:" + String.Join(",", soChungTus.Where(c => c != null)) : string.Empty
            };
            if (data.PhieuThu == "BangKeThuTienThuoc")
            {
                content += TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data) + "<div class=\"pagebreak\"></div>";
            }

            return content;
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

        #endregion

        #endregion
    }
}
