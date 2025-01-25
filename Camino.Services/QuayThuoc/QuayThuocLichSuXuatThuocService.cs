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
using Camino.Core.Domain.Entities.XuatKhos;

namespace Camino.Services.QuayThuoc
{
    [ScopedDependency(ServiceType = typeof(IQuayThuocLichSuXuatThuocService))]
    public class QuayThuocLichSuXuatThuocService : MasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>, IQuayThuocLichSuXuatThuocService
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


        IRepository<Camino.Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepo;
        IRepository<DonThuocThanhToan> _donThuocThanhToanRepo;
        IRepository<DonVTYTThanhToan> _donVTYTThanhToanRepo;

        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        IRepository<DuocPham> _duocPhamnRepo;
        public QuayThuocLichSuXuatThuocService(IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> repository,
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


        #region  DANH SÁCH LỊCH SỬ XUẤT THUỐC 07/09/2020

        public async Task<GridDataSource> GetDanhSachLichSuXuatThuoc(QueryInfo queryInfo, bool isPrint)
        {
            var queryDonThuoc = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => o.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)
                .GroupBy(
                    o => new
                    {
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.Id,
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        o.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        o.DonThuocThanhToan.YeuCauTiepNhan.HoTen,
                        o.DonThuocThanhToan.YeuCauTiepNhan.NamSinh,
                        o.DonThuocThanhToan.YeuCauTiepNhan.GioiTinh,
                        o.DonThuocThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                        o.DonThuocThanhToan.YeuCauTiepNhan.SoDienThoai,
                        o.DonThuocThanhToan.YeuCauTiepNhan.CoBHYT,
                        o.DonThuocThanhToan.YeuCauTiepNhan.BHYTMucHuong,
                        o.DonThuocThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                    }, o => o,
                    (k, v) => new DanhSachLichSuXuatThuocGridVo
                    {
                        Id = k.Id,
                        LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiDuocPham,
                        SoChungTu = k.SoPhieu,
                        NgayXuatThuoc = k.NgayXuat,
                        MaTN = k.MaYeuCauTiepNhan,
                        MaBN = k.MaBN,
                        HoTen = k.HoTen,
                        NamSinh = k.NamSinh,
                        DiaChi = k.DiaChiDayDu,
                        SoDienThoai = k.SoDienThoai,
                        DoiTuong = k.CoBHYT == true ? "BHYT (" + k.BHYTMucHuong + "%)" : "Viện phí",
                        LoaiGioiTinh = k.GioiTinh
                    })
                .Union(
                    _donVTYTThanhToanChiTietRepository.TableNoTracking
                        .Where(o => o.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)
                        .GroupBy(
                            o => new
                            {
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.Id,
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.HoTen,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.NamSinh,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.GioiTinh,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.SoDienThoai,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.CoBHYT,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.BHYTMucHuong,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                            }, o => o,
                            (k, v) => new DanhSachLichSuXuatThuocGridVo
                            {
                                Id = k.Id,
                                LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiVatTu,
                                SoChungTu = k.SoPhieu,
                                NgayXuatThuoc = k.NgayXuat,
                                MaTN = k.MaYeuCauTiepNhan,
                                MaBN = k.MaBN,
                                HoTen = k.HoTen,
                                NamSinh = k.NamSinh,
                                DiaChi = k.DiaChiDayDu,
                                SoDienThoai = k.SoDienThoai,
                                DoiTuong = k.CoBHYT == true ? "BHYT (" + k.BHYTMucHuong + "%)" : "Viện phí",
                                LoaiGioiTinh = k.GioiTinh
                            })
                    );

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                queryDonThuoc = queryDonThuoc.Where(o =>
                        EF.Functions.Like(o.MaBN, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.HoTen, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.MaTN, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.SoDienThoai, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.SoChungTu, $"%{queryInfo.SearchTerms}%")
                );
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    queryDonThuoc = queryDonThuoc.Where(p => p.NgayXuatThuoc >= tuNgay && p.NgayXuatThuoc <= denNgay.AddSeconds(59).AddMilliseconds(999));
                    //phieu = phieu.Where(p => p.NgayThu >= tuNgay && p.NgayThu <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }

            var dataOrderBy = queryDonThuoc.OrderBy(cc => cc.NgayXuatThuoc);

            var donthuoc = isPrint == true ? dataOrderBy.ToArray() : dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : queryDonThuoc.Count();
            return new GridDataSource { Data = donthuoc, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalDanhSachLichSuXuatThuoc(QueryInfo queryInfo)
        {
            var queryDonThuoc = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => o.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)
                .GroupBy(
                    o => new
                    {
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
                        o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        o.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        o.DonThuocThanhToan.YeuCauTiepNhan.HoTen,
                        o.DonThuocThanhToan.YeuCauTiepNhan.NamSinh,
                        o.DonThuocThanhToan.YeuCauTiepNhan.GioiTinh,
                        o.DonThuocThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                        o.DonThuocThanhToan.YeuCauTiepNhan.SoDienThoai,
                        o.DonThuocThanhToan.YeuCauTiepNhan.CoBHYT,
                        o.DonThuocThanhToan.YeuCauTiepNhan.BHYTMucHuong,
                        o.DonThuocThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                    }, o => o,
                    (k, v) => new DanhSachLichSuXuatThuocGridVo
                    {
                        LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiDuocPham,
                        SoChungTu = k.SoPhieu,
                        NgayXuatThuoc = k.NgayXuat,
                        MaTN = k.MaYeuCauTiepNhan,
                        MaBN = k.MaBN,
                        HoTen = k.HoTen,
                        NamSinh = k.NamSinh,
                        SoDienThoai = k.SoDienThoai,
                    })
                .Union(
                    _donVTYTThanhToanChiTietRepository.TableNoTracking
                        .Where(o => o.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)
                        .GroupBy(
                            o => new
                            {
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
                                o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.HoTen,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.NamSinh,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.GioiTinh,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.DiaChiDayDu,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.SoDienThoai,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.CoBHYT,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.BHYTMucHuong,
                                o.DonVTYTThanhToan.YeuCauTiepNhan.BenhNhan.MaBN,
                            }, o => o,
                            (k, v) => new DanhSachLichSuXuatThuocGridVo
                            {
                                LoaiDuocPhamVatTu = LoaiDuocPhamVatTu.LoaiDuocPham,
                                SoChungTu = k.SoPhieu,
                                NgayXuatThuoc = k.NgayXuat,
                                MaTN = k.MaYeuCauTiepNhan,
                                MaBN = k.MaBN,
                                HoTen = k.HoTen,
                                NamSinh = k.NamSinh,
                                SoDienThoai = k.SoDienThoai,
                            })
                    );

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                queryDonThuoc = queryDonThuoc.Where(o =>
                        EF.Functions.Like(o.MaBN, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.HoTen, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.MaTN, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.SoDienThoai, $"%{queryInfo.SearchTerms}%") ||
                        EF.Functions.Like(o.SoChungTu, $"%{queryInfo.SearchTerms}%")
                );
            }

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
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
                    queryDonThuoc = queryDonThuoc.Where(p => p.NgayXuatThuoc >= tuNgay && p.NgayXuatThuoc <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }
            return new GridDataSource { TotalRowCount = queryDonThuoc.Count() };
        }

        #endregion

        #region Lịch sử Xuất thuốc

        public async Task<GridDataSource> GetDataForGridLichSuXuatThuoc(QueryInfo queryInfo, bool isPrint)
        {
            // DS YCTN Đã hoàn thành hoặc hủy // // so tien thu  tinh cong no va mien giam
            BuildDefaultSortExpression(queryInfo);
            var querydataThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
                                                                           .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                                                                          .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                                                                         .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                                                                         .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans)
                                                                         .Include(x => x.TaiKhoanBenhNhanChis)
                                                                         .Where(o => (o.LoaiNoiThu == LoaiNoiThu.ThuNgan && o.TaiKhoanBenhNhanChis.Any(p => p.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)) ||
                                                                         (o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && ((o.TaiKhoanBenhNhanChis.Any(p => p.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)) || (o.TaiKhoanBenhNhanChis.Any(p => p.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)))));

            var phieuThu = querydataThu.Select(o => new LichSuXuatThuocGridVo
            {
                TaiKhoanBenhNhanThuId = o.Id,
                Id = o.YeuCauTiepNhanId,
                MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                NgayThu = o.NgayThu,
                //NgayThuStr = o.NgayThu != null ? o.NgayThu.ApplyFormatDateTimeSACH() : "",
                HoTen = o.YeuCauTiepNhan.BenhNhan.HoTen,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                GioiTinhHienThi = o.YeuCauTiepNhan.BenhNhan.GioiTinh.GetDescription(),
                DiaChi = o.YeuCauTiepNhan.DiaChiDayDu,
                SoDienThoaiDisplay = o.YeuCauTiepNhan.SoDienThoaiDisplay,
                SoDienThoai = o.YeuCauTiepNhan.SoDienThoai,
                DoiTuong = o.YeuCauTiepNhan.CoBHYT == true ? "BHYT (" + o.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                SoDon = o.SoPhieuHienThi,
                ThoiDiemCapThuocs = o.TaiKhoanBenhNhanChis.Select(x => x.DonThuocThanhToanChiTiet.DonThuocThanhToan.ThoiDiemCapThuoc).ToList(),
                ThoiDiemCapVatTus = o.TaiKhoanBenhNhanChis.Select(x => x.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.ThoiDiemCapVTYT).ToList(),
                //ThoiDiemCapPhatThuoc = o.TaiKhoanBenhNhanChis.Select(x => x.DonThuocThanhToanChiTiet.DonThuocThanhToan.ThoiDiemCapThuoc).FirstOrDefault().Value,
                Pos = o.POS.GetValueOrDefault(),
                TienMat = o.TienMat.GetValueOrDefault(),
                ChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                donDuocPham = o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null).Select(vc => vc.DonThuocThanhToanChiTiet).ToList(),
                donVatTu = o.TaiKhoanBenhNhanChis.Where(i => i.DonVTYTThanhToanChiTietId != null).Select(vc => vc.DonVTYTThanhToanChiTiet).ToList(),
                donVatTuTT = o.TaiKhoanBenhNhanChis.Where(i => i.DonVTYTThanhToanChiTietId != null).Select(vc => vc.DonVTYTThanhToanChiTiet.DonVTYTThanhToan).ToList(),
                donThuocTT = o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null).Select(vc => vc.DonThuocThanhToanChiTiet.DonThuocThanhToan).ToList(),
                SoChungTuDuocPhams = o.TaiKhoanBenhNhanChis.Select(vc => vc.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu).ToList(),
                SoChungTuVatTus = o.TaiKhoanBenhNhanChis.Select(vc => vc.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu).ToList()
                //SoChungTuDuocPham = String.Join(", ", o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null)
                //                                   .Select(vc => vc.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu)
                //                                   .Distinct()),

                //SoChungTuVatTu = String.Join(", ", o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null)
                //                                   .Select(vc => vc.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu)
                //                                   .Distinct())

            });
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                phieuThu = phieuThu.Where(o =>
                    EF.Functions.Like(o.MaBN, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.HoTen, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.MaTN, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.SoDienThoai, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.SoDienThoaiDisplay, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.SoDon, $"%{queryInfo.SearchTerms}%") //||
                                                                             //o.SoChungTuDuocPhams.Contains(queryInfo.SearchTerms) ||
                                                                             //o.SoChungTuVatTus.Contains(queryInfo.SearchTerms)
                    );
            }

            //var querydataThuNgan = _taiKhoanBenhNhanThuRepository.TableNoTracking.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
            //                                                               .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
            //                                                              .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
            //                                                             .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
            //                                                             .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans)
            //                                                             .Include(x => x.TaiKhoanBenhNhanChis)
            //                                                              .Where(o => o.LoaiNoiThu == LoaiNoiThu.ThuNgan && o.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc).Any());

            //var phieu = querydataThuNgan.Select(o => new LichSuXuatThuocGridVo
            //{
            //    TaiKhoanBenhNhanThuId = o.Id,
            //    Id = o.YeuCauTiepNhan.Id,
            //    MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
            //    MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //    NgayThu = o.NgayThu,
            //    NgayThuStr = o.NgayThu != null ? o.NgayThu.ApplyFormatDateTimeSACH() : "",
            //    DoiTuong = o.YeuCauTiepNhan.CoBHYT == true ? "BHYT (" + o.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)" : "Viện phí",
            //    HoTen = o.YeuCauTiepNhan.BenhNhan.HoTen,
            //    NamSinh = o.YeuCauTiepNhan.NamSinh,
            //    GioiTinhHienThi = o.YeuCauTiepNhan.BenhNhan.GioiTinh.GetDescription(),
            //    DiaChi = o.YeuCauTiepNhan.DiaChiDayDu,
            //    SoDienThoaiDisplay = o.YeuCauTiepNhan.SoDienThoai.ApplyFormatPhone(),
            //    SoDienThoai = o.YeuCauTiepNhan.SoDienThoai,
            //    SoDon = o.SoPhieu.ToString() ?? o.Id.ToString(),
            //    donDuocPham = o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null).Select(vc => vc.DonThuocThanhToanChiTiet).ToList(),
            //    donThuocTT = o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null).Select(vc => vc.DonThuocThanhToanChiTiet.DonThuocThanhToan).ToList(),
            //    Pos = o.POS.GetValueOrDefault(),
            //    TienMat = o.TienMat.GetValueOrDefault(),
            //    ChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),

            //    SoChungTuDuocPham = String.Join(", ", o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null)
            //                                       .Select(vc => vc.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu)
            //                                       .Distinct()),

            //    SoChungTuVatTu = String.Join(", ", o.TaiKhoanBenhNhanChis.Where(i => i.DonThuocThanhToanChiTietId != null)
            //                                       .Select(vc => vc.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu)
            //                                       .Distinct())

            //}).ApplyLike(queryInfo.SearchTerms.RemoveUniKeyAndToLower(),
            //                                                               g => g.TaiKhoanBenhNhanThuId.ToString(),
            //                                                               g => g.NgayThu.ToString().RemoveUniKeyAndToLower(),
            //                                                               g => g.MaBN,
            //                                                               g => g.HoTen.RemoveUniKeyAndToLower(),
            //                                                               g => g.MaTN.RemoveUniKeyAndToLower(),
            //                                                               g => g.SoDienThoaiDisplay.ApplyFormatPhone(),
            //                                                               g => g.SoDienThoai,
            //                                                               g => g.NamSinh.ToString(),
            //                                                               g => g.GioiTinhHienThi,
            //                                                               g => g.DiaChi.RemoveUniKeyAndToLower(),
            //                                                               g => g.SoDon,
            //                                                               g => g.SoChungTuDuocPham,
            //                                                               g => g.SoChungTuVatTu);


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    phieuThu = phieuThu.Where(p => p.NgayThu >= tuNgay && p.NgayThu <= denNgay.AddSeconds(59).AddMilliseconds(999));
                    //phieu = phieu.Where(p => p.NgayThu >= tuNgay && p.NgayThu <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }

            var dataOrderBy = phieuThu.OrderBy(cc => cc.NgayThu);
            var quaythuoc = isPrint == true ? dataOrderBy.ToArray() : dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : phieuThu.Count();

            foreach (var itemx in quaythuoc)
            {
                foreach (var itemxx in itemx.donDuocPham)
                {
                    itemx.SoTienDuocPham += itemxx.GiaBan;
                }
                foreach (var itemxx in itemx.donVatTu)
                {
                    itemx.SoTienVatTu += itemxx.GiaBan;
                }
                itemx.NgayThuStr = itemx.NgayThu.ApplyFormatDateTimeSACH();

                itemx.ThoiDiemCapPhatThuoc = itemx.ThoiDiemCapThuocs.FirstOrDefault(o => o != null) ?? itemx.ThoiDiemCapVatTus.FirstOrDefault(o => o != null);
                itemx.SoChungTuDuocPham = string.Join(", ", itemx.SoChungTuDuocPhams.Distinct());
                itemx.SoChungTuVatTu = string.Join(", ", itemx.SoChungTuVatTus.Distinct());
            }

            //var allData = new List<LichSuXuatThuocGridVo>();
            //foreach (var itemx in phieuThu)
            //{
            //    foreach (var itemxx in itemx.donDuocPham)
            //    {
            //        itemx.SoTienDuocPham += itemxx.GiaBan;
            //    }
            //    foreach (var itemxx in itemx.donVatTu)
            //    {
            //        itemx.SoTienVatTu += itemxx.GiaBan;
            //    }
            //    foreach (var itemxx in itemx.donVatTuTT)
            //    {
            //        itemx.ThoiDiemCapPhatThuoc = itemxx.ThoiDiemCapVTYT;
            //    }
            //    foreach (var itemxx in itemx.donThuocTT)
            //    {
            //        itemx.ThoiDiemCapPhatThuoc = itemxx.ThoiDiemCapThuoc;
            //    }
            //    allData.Add(itemx);
            //};
            //foreach (var itemx in phieu)
            //{
            //    foreach (var itemxx in itemx.donDuocPham)
            //    {
            //        itemx.SoTienDuocPham += itemxx.GiaBan;
            //    }
            //    foreach (var itemxx in itemx.donThuocTT)
            //    {
            //        itemx.ThoiDiemCapPhatThuoc = itemxx.ThoiDiemCapThuoc;
            //    }
            //    allData.Add(itemx);
            //};


            return new GridDataSource { Data = quaythuoc, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalPageForGridLichSuXuatThuoc(QueryInfo queryInfo)
        {
            var querydataThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
                                                                           .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                                                                          .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                                                                         .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                                                                         .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans)
                                                                         .Include(x => x.TaiKhoanBenhNhanChis)
                                                                         .Where(o => (o.LoaiNoiThu == LoaiNoiThu.ThuNgan && o.TaiKhoanBenhNhanChis.Any(p => p.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)) ||
                                                                         (o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && ((o.TaiKhoanBenhNhanChis.Any(p => p.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)) || (o.TaiKhoanBenhNhanChis.Any(p => p.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)))));

            var phieuThu = querydataThu.Select(o => new LichSuXuatThuocGridVo
            {
                TaiKhoanBenhNhanThuId = o.Id,
                Id = o.YeuCauTiepNhanId,
                MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
                MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
                NgayThu = o.NgayThu,
                HoTen = o.YeuCauTiepNhan.BenhNhan.HoTen,
                NamSinh = o.YeuCauTiepNhan.NamSinh,
                DiaChi = o.YeuCauTiepNhan.DiaChiDayDu,
                SoDienThoaiDisplay = o.YeuCauTiepNhan.SoDienThoaiDisplay,
                SoDienThoai = o.YeuCauTiepNhan.SoDienThoai,
                DoiTuong = o.YeuCauTiepNhan.CoBHYT == true ? "BHYT (" + o.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)" : "Viện phí",
                SoDon = o.SoPhieuHienThi,
                SoChungTuDuocPhams = o.TaiKhoanBenhNhanChis.Select(vc => vc.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu).ToList(),
                SoChungTuVatTus = o.TaiKhoanBenhNhanChis.Select(vc => vc.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu).ToList()

            });
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                phieuThu = phieuThu.Where(o =>
                    EF.Functions.Like(o.MaBN, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.HoTen, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.MaTN, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.SoDienThoai, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.SoDienThoaiDisplay, $"%{queryInfo.SearchTerms}%") ||
                    EF.Functions.Like(o.SoDon, $"%{queryInfo.SearchTerms}%") //||
                                                                             //o.SoChungTuDuocPhams.Contains(queryInfo.SearchTerms) ||
                                                                             //o.SoChungTuVatTus.Contains(queryInfo.SearchTerms)
                    );
            }


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    phieuThu = phieuThu.Where(p => p.NgayThu >= tuNgay && p.NgayThu <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }
            var countTask = phieuThu.Count();


            return new GridDataSource { TotalRowCount = countTask };
        }

        public List<ThongTinBenhNhanGridVo> GetThongTinBenhNhanTheoMaBN(string maBN)
        {
            return _benhNhanRepository.TableNoTracking.Where(x => x.MaBN == maBN)
                                             .Select(x => new ThongTinBenhNhanGridVo()
                                             {
                                                 MaBN = x.MaBN,
                                                 BenhNhanId = x.Id,
                                                 HoTen = x.HoTen,
                                                 NamSinh = x.NamSinh.ToString(),
                                                 GioiTinhHienThi = x.GioiTinh.GetDescription(),
                                                 DiaChi = x.DiaChi,
                                                 DiaChiDayDu = x.DiaChiDayDu,
                                                 SoDienThoai = x.SoDienThoai,
                                                 Email = x.Email
                                             }).ToList();
        }

        public List<ThongTinDuocPhamQuayThuocVo> GetDanhSachLichSuXuatThuocVatTuBHYT(long phieuXuatId, LoaiDuocPhamVatTu loaiDuocPhamVatTu)
        {
            if (loaiDuocPhamVatTu == LoaiDuocPhamVatTu.LoaiDuocPham)
            {
                var dataXuatKhoDuocPhams = _xuatKhoDuocPhamRepository.TableNoTracking.Where(o => o.Id == phieuXuatId).SelectMany(c => c.XuatKhoDuocPhamChiTiets);
                var vitriXuatKhoDuocPhamIds = dataXuatKhoDuocPhams.SelectMany(c => c.XuatKhoDuocPhamChiTietViTris).Select(c => c.Id);

                var dataDonThuocThanhToans = _donThuocThanhToanChiTietRepository.TableNoTracking.Where(o => vitriXuatKhoDuocPhamIds.Contains(o.XuatKhoDuocPhamChiTietViTriId))
                                                  .Where(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT == true)
                                                  .Select(x => new ThongTinDuocPhamQuayThuocVo()
                                                  {
                                                      Id = x.Id,
                                                      DuocPhamId = x.DuocPham.Id,
                                                      MaHoatChat = x.DuocPham.HoatChat,
                                                      NongDoVaHamLuong = x.DuocPham.HamLuong,
                                                      TenDuocPham = x.DuocPham.Ten,
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
                                                      HanSuDung = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung
                                                  }).ToList();
                return dataDonThuocThanhToans;
            }
            else
            {
                var dataXuatKhoVatTus = _xuatKhoVatTuRepository.TableNoTracking.Where(o => o.Id == phieuXuatId).SelectMany(c => c.XuatKhoVatTuChiTiets);
                var vitriXuatKhoVatTuIds = dataXuatKhoVatTus.SelectMany(c => c.XuatKhoVatTuChiTietViTris).Select(c => c.Id);

                var dataVatTuThanhToans = _donVTYTThanhToanChiTietRepository.TableNoTracking.Where(o => vitriXuatKhoVatTuIds.Contains(o.XuatKhoVatTuChiTietViTriId))
                                 .Where(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT == true)
                                 .Select(x => new ThongTinDuocPhamQuayThuocVo()
                                 {
                                     Id = x.Id,
                                     DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                                     TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                                     NhapKhoDuocPhamChiTietId = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Id,
                                     DonViTinh = x.VatTuBenhVien.VatTus.DonViTinh,
                                     SoLuongMua = x.SoLuong,
                                     Solo = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Solo,
                                     ViTri = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTri.Ten,
                                     HanSuDung = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HanSuDung,
                                     DonGiaNhap = x.DonGiaNhap,
                                     DonGia = x.DonGiaBan,
                                     TiLeTheoThapGia = x.TiLeTheoThapGia,
                                     VAT = x.VAT
                                 }).ToList();

                return dataVatTuThanhToans;
            }
        }

        public List<ThongTinDuocPhamQuayThuocVo> GetDanhSachLichSuXuatThuocVatTukhongBHYT(long phieuXuatId, LoaiDuocPhamVatTu loaiDuocPhamVatTu)
        {
            if (loaiDuocPhamVatTu == LoaiDuocPhamVatTu.LoaiDuocPham)
            {
                var dataXuatKhoDuocPhams = _xuatKhoDuocPhamRepository.TableNoTracking.Where(o => o.Id == phieuXuatId).SelectMany(c => c.XuatKhoDuocPhamChiTiets)
                                                                     .Include(c => c.XuatKhoDuocPhamChiTietViTris);
                var vitriXuatKhoDuocPhamIds = dataXuatKhoDuocPhams.SelectMany(c => c.XuatKhoDuocPhamChiTietViTris).Select(c => c.Id);

                var dataDonThuocThanhToans = _donThuocThanhToanChiTietRepository.TableNoTracking.Where(o => vitriXuatKhoDuocPhamIds.Contains(o.XuatKhoDuocPhamChiTietViTriId))
                                                  .Where(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT != true)
                                                  .Select(x => new ThongTinDuocPhamQuayThuocVo()
                                                  {
                                                      Id = x.Id,
                                                      DuocPhamId = x.DuocPham.Id,
                                                      MaHoatChat = x.DuocPham.HoatChat,
                                                      NongDoVaHamLuong = x.DuocPham.HamLuong,
                                                      TenDuocPham = x.DuocPham.Ten,
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
                                                      HanSuDung = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.HanSuDung
                                                  }).ToList();
                return dataDonThuocThanhToans;
            }
            else
            {
                var dataXuatKhoVatTus = _xuatKhoVatTuRepository.TableNoTracking.Where(o => o.Id == phieuXuatId).SelectMany(c => c.XuatKhoVatTuChiTiets);
                var vitriXuatKhoVatTuIds = dataXuatKhoVatTus.SelectMany(c => c.XuatKhoVatTuChiTietViTris).Select(c => c.Id);

                var dataVatTuThanhToans = _donVTYTThanhToanChiTietRepository.TableNoTracking.Where(o => vitriXuatKhoVatTuIds.Contains(o.XuatKhoVatTuChiTietViTriId))
                                 .Where(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT != true)
                                 .Select(x => new ThongTinDuocPhamQuayThuocVo()
                                 {
                                     Id = x.Id,
                                     DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                                     TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                                     NhapKhoDuocPhamChiTietId = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Id,
                                     DonViTinh = x.VatTuBenhVien.VatTus.DonViTinh,
                                     SoLuongMua = x.SoLuong,
                                     Solo = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Solo,
                                     ViTri = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTri.Ten,
                                     HanSuDung = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HanSuDung,
                                     DonGiaNhap = x.DonGiaNhap,
                                     DonGia = x.DonGiaBan,
                                     TiLeTheoThapGia = x.TiLeTheoThapGia,
                                     VAT = x.VAT
                                 }).ToList();

                return dataVatTuThanhToans;
            }
        }

        public async Task<string> XacNhanInThuocVatTuCoBhytXuatThuoc(XacNhanInThuocVatTu xacNhanIn, bool cobhyt)
        {

            var content = "";

            var groupVTBHYT = "Vật Tư BHYT";
            var groupVTKhongBHYT = "Vật Tư Không BHYT";

            var headerVTBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupVTBHYT.ToUpper()
                                        + "</b></tr>";
            var headerVTKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupVTKhongBHYT.ToUpper()
                                        + "</b></tr>";

            var groupThuocBHYT = "Thuốc BHYT";
            var groupThuocKhongBHYT = "Thuốc Không BHYT";

            var headerDPBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                        + "</b></tr>";
            var headerDPKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                        + "</b></tr>";



            var tongTienToaThuoc = string.Empty;
            var soTienBangChu = string.Empty;
            var keToaThuocBHKBH = string.Empty;
            var keDuoPham = string.Empty;
            var keDuoPhamKBHYT = string.Empty;
            var keVatTu = string.Empty;
            var keVatTuKBH = string.Empty;
            decimal tongTienPhaiTra = 0;
            int idex = 1;
            //decimal tongChiPhi = 0;

            var result = await _templateRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Name.Equals("BangKeThuTienThuoc"));
            var currentUserId = _userAgentHelper.GetCurrentUserId();


            var currentUserName = _userRepository.TableNoTracking.Where(c => c.Id == currentUserId).Select(c => c.HoTen).FirstOrDefault();
            var allData = new List<ThongTinDuocPhamQuayThuocVo>();
            var soChungTu = string.Empty;
            var keToaThuoc = string.Empty;

            if (xacNhanIn.LoaiDuocPhamVatTu == LoaiDuocPhamVatTu.LoaiDuocPham)
            {
                var xuatKhoDuocPham = _xuatKhoDuocPhamRepository.TableNoTracking.Where(o => o.Id == xacNhanIn.PhieuXuatId).Include(c => c.XuatKhoDuocPhamChiTiets);
                var dataXuatKhoDuocPhams = xuatKhoDuocPham.SelectMany(c => c.XuatKhoDuocPhamChiTiets);
                var vitriXuatKhoDuocPhamIds = dataXuatKhoDuocPhams.SelectMany(c => c.XuatKhoDuocPhamChiTietViTris).Select(c => c.Id);
                soChungTu = xuatKhoDuocPham.Select(c => c.SoPhieu).FirstOrDefault();

                allData.AddRange(_donThuocThanhToanChiTietRepository.TableNoTracking.Where(o => vitriXuatKhoDuocPhamIds.Contains(o.XuatKhoDuocPhamChiTietViTriId))
                                                  .Where(c => c.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT == cobhyt)
                                                  .Select(x => new ThongTinDuocPhamQuayThuocVo()
                                                  {
                                                      Id = x.Id,
                                                      DuocPhamId = x.DuocPham.Id,
                                                      MaHoatChat = x.DuocPham.HoatChat,
                                                      NongDoVaHamLuong = x.DuocPham.HamLuong,
                                                      TenDuocPham = x.DuocPham.Ten,
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
                                                      YeuCauTiepNhanId = x.DonThuocThanhToan.YeuCauTiepNhanId,
                                                      LoaiDonThuoc = x.DonThuocThanhToan.LoaiDonThuoc,
                                                  }).ToList());


                if (allData.Any())
                {
                    //BVHD-3943: gộp thuốc, VT
                    var groupThuocThanhToanChiTietBHYTs = allData.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT).GroupBy(o => new { o.DuocPhamId, o.DonGia }).ToList();
                    foreach (var thuocThanhToanChiTiet in groupThuocThanhToanChiTietBHYTs)
                    {
                        keDuoPham = keDuoPham + "<tr style='border: 1px solid #020000;'>"
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     idex++
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     //thuocThanhToanChiTiet?.TenDuocPham
                                                     GetFormatDuocPham(thuocThanhToanChiTiet.Key.DuocPhamId)
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                     thuocThanhToanChiTiet.First().DonViTinh
                                                     + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                      GetFormatSoLuong(thuocThanhToanChiTiet.Key.DuocPhamId, thuocThanhToanChiTiet.Sum(o=>o.SoLuongMua))
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Key.DonGia).ApplyFormatMoneyToDouble()
                                                     + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Sum(o=>o.ThanhTien)).ApplyFormatMoneyToDouble()
                                                     + "</tr>";
                        tongTienPhaiTra += thuocThanhToanChiTiet.Sum(o => o.ThanhTien);
                    }
                    //foreach (var thuocThanhToanChiTiet in allData)
                    //{
                    //    if (thuocThanhToanChiTiet.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT)
                    //    {
                    //        if (thuocThanhToanChiTiet != null)
                    //        {
                    //            keDuoPham = keDuoPham + "<tr style='border: 1px solid #020000;'>"
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 idex++
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 //thuocThanhToanChiTiet?.TenDuocPham
                    //                                 GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 thuocThanhToanChiTiet?.DonViTinh
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                  GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuongMua)
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.DonGia).ApplyFormatMoneyToDouble()
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.ThanhTien).ApplyFormatMoneyToDouble()
                    //                                 + "</tr>";
                    //            tongTienPhaiTra += thuocThanhToanChiTiet.ThanhTien;
                    //            tongChiPhi = Convert.ToDecimal(thuocThanhToanChiTiet.ThanhTien);
                    //        }
                    //    }
                    //}

                    //BVHD-3943: gộp thuốc, VT
                    var groupThuocThanhToanChiTietKhongBHYTs = allData.Where(o => o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT).GroupBy(o => new { o.DuocPhamId, o.DonGia }).ToList();
                    foreach (var thuocThanhToanChiTiet in groupThuocThanhToanChiTietKhongBHYTs)
                    {
                        keDuoPhamKBHYT = keDuoPhamKBHYT + "<tr style='border: 1px solid #020000;'>"
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
                                                     Convert.ToDouble(thuocThanhToanChiTiet.Sum(o=>o.ThanhTien)).ApplyFormatMoneyToDouble()
                                                     + "</tr>";
                        tongTienPhaiTra += thuocThanhToanChiTiet.Sum(o => o.ThanhTien);
                    }
                    //foreach (var thuocThanhToanChiTiet in allData)
                    //{                        
                    //    if (thuocThanhToanChiTiet.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT)
                    //    {
                    //        if (thuocThanhToanChiTiet != null)
                    //        {
                    //            keDuoPhamKBHYT = keDuoPhamKBHYT + "<tr style='border: 1px solid #020000;'>"
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 idex++
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 GetFormatDuocPham(thuocThanhToanChiTiet.DuocPhamId)
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 thuocThanhToanChiTiet?.DonViTinh
                    //                                 + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                    //                                 GetFormatSoLuong(thuocThanhToanChiTiet.DuocPhamId, thuocThanhToanChiTiet.SoLuongMua)
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.DonGia).ApplyFormatMoneyToDouble()
                    //                                 + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                    //                                 Convert.ToDouble(thuocThanhToanChiTiet.ThanhTien).ApplyFormatMoneyToDouble()
                    //                                 + "</tr>";
                    //            tongTienPhaiTra += thuocThanhToanChiTiet.ThanhTien;
                    //            //tongChiPhi = Convert.ToDecimal(thuocThanhToanChiTiet.ThanhTien);
                    //        }
                    //    }
                    //}
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
            else
            {
                var xuatKhoVatTu = _xuatKhoVatTuRepository.TableNoTracking.Where(o => o.Id == xacNhanIn.PhieuXuatId).Include(c => c.XuatKhoVatTuChiTiets);
                var dataXuatKhoVatTus = xuatKhoVatTu.SelectMany(c => c.XuatKhoVatTuChiTiets);
                var vitriXuatKhoVatTuIds = dataXuatKhoVatTus.SelectMany(c => c.XuatKhoVatTuChiTietViTris).Select(c => c.Id);
                soChungTu = xuatKhoVatTu.Select(c => c.SoPhieu).FirstOrDefault();

                allData.AddRange(_donVTYTThanhToanChiTietRepository.TableNoTracking.Where(o => vitriXuatKhoVatTuIds.Contains(o.XuatKhoVatTuChiTietViTriId))
                                 .Where(c => c.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.LaVatTuBHYT == cobhyt)
                                 .Select(x => new ThongTinDuocPhamQuayThuocVo()
                                 {
                                     Id = x.Id,
                                     DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                                     TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                                     NhapKhoDuocPhamChiTietId = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Id,
                                     DonViTinh = x.VatTuBenhVien.VatTus.DonViTinh,
                                     SoLuongMua = x.SoLuong,
                                     Solo = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.Solo,
                                     ViTri = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.KhoViTri.Ten,
                                     HanSuDung = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.HanSuDung,
                                     DonGiaNhap = x.DonGiaNhap,
                                     DonGia = x.DonGiaBan,
                                     TiLeTheoThapGia = x.TiLeTheoThapGia,
                                     VAT = x.VAT,
                                     YeuCauTiepNhanId = x.DonVTYTThanhToan.YeuCauTiepNhanId
                                 }).ToList());

                if (allData.Any())
                {
                    //BVHD-3943: gộp thuốc, VT
                    var groupVatTuThanhToanChiTiets = allData.GroupBy(o => new { o.TenDuocPham, o.DonGia }).ToList();
                    foreach (var vatTuThanhToanChiTiet in groupVatTuThanhToanChiTiets)
                    {
                        keVatTu += "<tr style='border: 1px solid #020000;'>"
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                idex++
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                vatTuThanhToanChiTiet.Key.TenDuocPham
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                vatTuThanhToanChiTiet.First().DonViTinh
                                                + "<td style='border: 1px solid #020000;text-align: left;padding:5px;'>" +
                                                vatTuThanhToanChiTiet.Sum(o=>o.SoLuongMua)
                                                + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                Convert.ToDouble(vatTuThanhToanChiTiet.Key.DonGia).ApplyFormatMoneyToDouble()
                                                + "<td style='border: 1px solid #020000;text-align: right;padding:5px;'>" +
                                                Convert.ToDouble(vatTuThanhToanChiTiet.Sum(o=>o.ThanhTien)).ApplyFormatMoneyToDouble()
                                                + "</tr>";
                        tongTienPhaiTra += vatTuThanhToanChiTiet.Sum(o => o.ThanhTien);
                    }
                    //foreach (var vatTuThanhToanChiTiet in allData)
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
                    //                            Convert.ToDouble(vatTuThanhToanChiTiet.ThanhTien).ApplyFormatMoneyToDouble()
                    //                            + "</tr>";
                    //        tongTienPhaiTra += vatTuThanhToanChiTiet.ThanhTien;
                    //        //tongChiPhi += Convert.ToDecimal(vatTuThanhToanChiTiet.SoTienBenhNhanDaChi);
                    //    }
                    //}
                    keVatTuKBH = headerVTKhongBHYT + keVatTu;
                }


            }

            var yeuCauTiepNhanId = allData.Where(c => c.YeuCauTiepNhanId != null).Select(c => c.YeuCauTiepNhanId).FirstOrDefault();
            var yeuCauTiepNhan = await _yeuCauTiepNhanRepo.GetByIdAsync((long)yeuCauTiepNhanId,
                                              x => x
                                                  .Include(o => o.TinhThanh)
                                                  .Include(o => o.QuanHuyen)
                                                  .Include(o => o.PhuongXa)
                                                  .Include(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                                                  .Include(o => o.BenhNhan)
                                                  .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                                                  .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.ChanDoanSoBoICD));


            keToaThuoc = keToaThuocBHKBH + keVatTuKBH;
            soTienBangChu = "<i>Bằng chữ:" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra)) + ".</i";
            var data = new
            {
                LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                NguoiNopTien = yeuCauTiepNhan.HoTen,
                NamSinh = yeuCauTiepNhan.NamSinh,
                MaBn = yeuCauTiepNhan.BenhNhan.MaBN,
                NguoiThuTien = currentUserName,
                GioiTinh = yeuCauTiepNhan.GioiTinh.GetDescription(),
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                DienDai = "Thu Tiền thuốc",
                ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
                                   DateTime.Now.Year,
                NguoiPhatThuoc = yeuCauTiepNhan.NhanVienTiepNhan?.User?.HoTen,
                TongTienPhaiTra = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble(),
                keToaThuoc = keToaThuoc,
                SoTienChu = NumberHelper.ChuyenSoRaText(Convert.ToDouble(tongTienPhaiTra)),
                PhieuThu = "BangKeThuTienThuoc",
                tongTienToaThuoc = Convert.ToDouble(tongTienPhaiTra).ApplyFormatMoneyToDouble(),
                soTienBangChu = soTienBangChu,
                SoChungTu = "Số:" + soChungTu
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }

        #endregion

        #region Lịch sử bán thuốc
        
        public async Task<GridDataSource> GetDataForGridLichSuXuatThuocAsync(QueryInfo queryInfo, bool isPrint)
        {
            // DS YCTN Đã hoàn thành hoặc hủy // // so tien thu  tinh cong no va mien giam
            BuildDefaultSortExpression(queryInfo);

            DateTime denNgay = DateTime.Now;
            DateTime tuNgay = DateTime.Now.AddYears(-1);
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
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

            var dataThus = _taiKhoanBenhNhanThuRepository.TableNoTracking
            .Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && (o.DaHuy == null || o.DaHuy == false) && o.NgayThu >= tuNgay && o.NgayThu <= denNgay)            
            .Select(o => new
            {
                TaiKhoanBenhNhanThuId = o.Id,
                Id = o.YeuCauTiepNhanId,
                NgayThu = o.NgayThu,
                SoDon = o.SoPhieuHienThi,
                SoTienVatTu = 0,
                SoTienDuocPham = 0,
                Pos = o.POS.GetValueOrDefault(),
                TienMat = o.TienMat.GetValueOrDefault(),
                ChuyenKhoan = o.ChuyenKhoan.GetValueOrDefault(),
                CongNo = o.CongNo.GetValueOrDefault(),
                PhieuChis = o.TaiKhoanBenhNhanChis.Select(chi => new { chi.DonThuocThanhToanChiTietId, chi.DonVTYTThanhToanChiTietId, LoaiChiTienBenhNhan = chi.LoaiChiTienBenhNhan, chi.SoLuong, Gia = chi.Gia }).ToList()
            }).ToList();
            var donThuocThanhToanChiTietIds = dataThus.SelectMany(o=>o.PhieuChis).Select(o=>o.DonThuocThanhToanChiTietId.GetValueOrDefault()).Distinct().ToList();
            var donVTYTThanhToanChiTietIds = dataThus.SelectMany(o => o.PhieuChis).Select(o => o.DonVTYTThanhToanChiTietId.GetValueOrDefault()).Distinct().ToList();
            var yeuCauTiepNhanIds = dataThus.Select(o => o.Id).Distinct().ToList();

            var donThuocThanhToanChiTietDaXuats = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => donThuocThanhToanChiTietIds.Contains(o.Id) && o.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)
                .Select(o => o.Id)
                .ToList();

            var donVTYTThanhToanChiTietDaXuats = _donVTYTThanhToanChiTietRepository.TableNoTracking
                .Where(o => donVTYTThanhToanChiTietIds.Contains(o.Id) && o.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)
                .Select(o => o.Id)
                .ToList();

            var yeuCauTiepNhans = _yeuCauTiepNhanRepo.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    MaBN = o.BenhNhan.MaBN,
                    MaTN = o.MaYeuCauTiepNhan,                
                    HoTen = o.HoTen,
                    NamSinh = o.NamSinh,
                    GioiTinh = o.GioiTinh,
                    DiaChi = o.DiaChiDayDu,
                    SoDienThoai = o.SoDienThoai,
                    SoDienThoaiDisplay = o.SoDienThoaiDisplay,
                    DoiTuong = o.CoBHYT,
                    BHYTMucHuong = o.BHYTMucHuong
                })
                .ToList();

            var returnData = new List<LichSuXuatThuocGridVo>();
            foreach(var dataThu in dataThus) 
            {                
                if(dataThu.PhieuChis.Any(o=>(o.DonThuocThanhToanChiTietId != null && donThuocThanhToanChiTietDaXuats.Contains(o.DonThuocThanhToanChiTietId.Value))
                                            || (o.DonVTYTThanhToanChiTietId != null && donVTYTThanhToanChiTietDaXuats.Contains(o.DonVTYTThanhToanChiTietId.Value))))
                {
                    var yctn = yeuCauTiepNhans.First(o => o.Id == dataThu.Id);
                    if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
                    {
                        var search = queryInfo.SearchTerms.ToLower().RemoveVietnameseDiacritics();
                        if(!(yctn.MaTN.ToLower().Contains(search) || yctn.MaBN.ToLower().Contains(search) 
                            || dataThu.SoDon.ToLower().Contains(search) || yctn.HoTen.ToLower().RemoveVietnameseDiacritics().Contains(search) 
                            || (yctn.SoDienThoai != null && yctn.SoDienThoai.ToLower().Contains(search)) || (yctn.SoDienThoaiDisplay != null && yctn.SoDienThoaiDisplay.ToLower().Contains(search))))
                        {
                            continue;
                        }
                    }
                    returnData.Add(new LichSuXuatThuocGridVo
                    {
                        TaiKhoanBenhNhanThuId = dataThu.TaiKhoanBenhNhanThuId,
                        Id = dataThu.Id,
                        MaBN = yctn.MaBN,
                        MaTN = yctn.MaTN,
                        NgayThu = dataThu.NgayThu,
                        NgayThuStr = dataThu.NgayThu.ApplyFormatDateTimeSACH(),
                        HoTen = yctn.HoTen,
                        NamSinh = yctn.NamSinh,
                        GioiTinhHienThi = yctn.GioiTinh != null ? yctn.GioiTinh.GetDescription() : "",
                        DiaChi = yctn.DiaChi,
                        SoDienThoaiDisplay = yctn.SoDienThoaiDisplay,
                        SoDienThoai = yctn.SoDienThoai,
                        DoiTuong = yctn.DoiTuong == true ? ("BHYT (" + (yctn.BHYTMucHuong != null ? yctn.BHYTMucHuong.ToString() : "") + "%)") : "Viện phí",
                        SoDon = dataThu.SoDon,
                        SoTienVatTu = 0,
                        Pos = dataThu.Pos,
                        TienMat = dataThu.TienMat,
                        ChuyenKhoan = dataThu.ChuyenKhoan,
                        CongNo = dataThu.CongNo,
                        SoTienDuocPham = dataThu.PhieuChis.Where(chi => chi.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi).Select(chi => Math.Round(chi.Gia.GetValueOrDefault() * (decimal)chi.SoLuong.GetValueOrDefault(), 2)).DefaultIfEmpty().Sum()
                    });
                }
            }
            if (isPrint)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            return new GridDataSource { Data = returnData.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray() , TotalRowCount = returnData.Count() };
        }

        //public async Task<GridDataSource> GetTotalPageForGridLichSuXuatThuocAsync(QueryInfo queryInfo)
        //{
        //    DateTime denNgay = DateTime.Now;
        //    DateTime tuNgay = DateTime.Now.AddYears(-1);
        //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {
        //        var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
        //        if (!string.IsNullOrEmpty(queryString.FromDate))
        //        {
        //            queryString.FromDate.TryParseExactCustom(out var tuNgayValue);
        //            tuNgay = tuNgayValue;
        //        }
        //        if (!string.IsNullOrEmpty(queryString.ToDate))
        //        {
        //            queryString.ToDate.TryParseExactCustom(out var denNgayValue);
        //            denNgay = denNgayValue.AddSeconds(59).AddMilliseconds(999);
        //        }
        //    }

        //    var querydataThu = _taiKhoanBenhNhanThuRepository.TableNoTracking
        //    .Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && (o.DaHuy == null || o.DaHuy == false) && o.NgayThu >= tuNgay && o.NgayThu <= denNgay &&
        //    o.TaiKhoanBenhNhanChis.Any(chi => (chi.DonThuocThanhToanChiTietId != null && chi.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)
        //                                                                                            || (chi.DonVTYTThanhToanChiTietId != null && chi.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)))
        //    .Select(o => new
        //    {
        //        TaiKhoanBenhNhanThuId = o.Id,
        //        Id = o.YeuCauTiepNhanId,
        //        MaBN = o.YeuCauTiepNhan.BenhNhan.MaBN,
        //        MaTN = o.YeuCauTiepNhan.MaYeuCauTiepNhan,
        //        NgayThu = o.NgayThu,
        //        SoDienThoai = o.YeuCauTiepNhan.SoDienThoai,
        //        SoDienThoaiDisplay = o.YeuCauTiepNhan.SoDienThoaiDisplay,
        //        HoTen = o.YeuCauTiepNhan.HoTen,
        //        SoDon = o.SoPhieuHienThi,
        //    }).ApplyLike(queryInfo.SearchTerms, g => g.MaBN, g => g.HoTen, g => g.MaTN, g => g.SoDon, g => g.SoDienThoai, g => g.SoDienThoaiDisplay);
        //    //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    //{
        //    //    var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
        //    //    if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
        //    //    {
        //    //        DateTime denNgay;
        //    //        queryString.FromDate.TryParseExactCustom(out var tuNgay);
        //    //        //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
        //    //        //    out var tuNgay);
        //    //        if (string.IsNullOrEmpty(queryString.ToDate))
        //    //        {
        //    //            denNgay = DateTime.Now;
        //    //        }
        //    //        else
        //    //        {
        //    //            queryString.ToDate.TryParseExactCustom(out denNgay);
        //    //            //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
        //    //        }
        //    //        phieuThu = phieuThu.Where(p => p.NgayThu >= tuNgay && p.NgayThu <= denNgay.AddSeconds(59).AddMilliseconds(999));
        //    //    }
        //    //}
        //    var totalRowCount = querydataThu.Count();
            
        //    return new GridDataSource { TotalRowCount = totalRowCount };
        //}

        public async Task<GridDataSource> GetDataForGridLichSuXuatThuocAsyncOld(QueryInfo queryInfo, bool isPrint)
        {
            // DS YCTN Đã hoàn thành hoặc hủy // // so tien thu  tinh cong no va mien giam
            BuildDefaultSortExpression(queryInfo);
            var querydataThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans)
                .Include(x => x.TaiKhoanBenhNhanChis)
            .Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && ((o.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc
                                                            && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).Any())
                                                            || (o.TaiKhoanBenhNhanChis.Where(p => p.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT).Any())));

            var phieuThu = querydataThu.Select(o => new LichSuXuatThuocGridVo
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
                                                                           g => g.SoDon
                                                                           );
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    phieuThu = phieuThu.Where(p => p.NgayThu >= tuNgay && p.NgayThu <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }
            var allData = new List<LichSuXuatThuocGridVo>();
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
            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : phieuThu.CountAsync();
            //var queryTask = isPrint == true ? phieuThu.OrderBy(queryInfo.SortString).ToArrayAsync() : phieuThu.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);

            //return new GridDataSource
            //{
            //    Data = queryTask.Result,
            //    TotalRowCount = countTask.Result
            //};
        }
        public async Task<GridDataSource> GetTotalPageForGridLichSuXuatThuocAsyncOld(QueryInfo queryInfo)
        {
            // DS YCTN Đã hoàn thành hoặc hủy // so tien thu chua tinh cong no va mien giam
            var querydataThu = _taiKhoanBenhNhanThuRepository.TableNoTracking.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
                                                                          .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                                                                         .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.QuanHuyen)
                                                                        .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.PhuongXa)
                                                                        .Include(x => x.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans)
                                                                        .Include(x => x.TaiKhoanBenhNhanChis)
                                                                        .Where(o => o.LoaiNoiThu == LoaiNoiThu.NhaThuoc && ((o.TaiKhoanBenhNhanChis.Where(p => p.DonThuocThanhToanChiTietId != null && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc && p.DonThuocThanhToanChiTiet.DonThuocThanhToan.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocKhongBHYT).Any()) || (o.TaiKhoanBenhNhanChis.Where(p => p.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT).Any())));

            var phieuThu = querydataThu.Select(o => new LichSuXuatThuocGridVo
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
                                                                           g => g.SoDon
                                                                           );
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<LichSuXuatThuocGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    DateTime denNgay;
                    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
                    //    out var tuNgay);
                    if (string.IsNullOrEmpty(queryString.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        queryString.ToDate.TryParseExactCustom(out denNgay);
                        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    }
                    phieuThu = phieuThu.Where(p => p.NgayThu >= tuNgay && p.NgayThu <= denNgay.AddSeconds(59).AddMilliseconds(999));
                }
            }
            var allData = new List<LichSuXuatThuocGridVo>();
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
        #endregion


        #region Hủy bán thuốc

        public void HuyBanThuocTrongNgay(ThongTinHuyPhieuXuatTrongNgayVo huyBanThuocTrongNgayViewModel)
        {
            var phieuThu = _taiKhoanBenhNhanThuRepository.GetById(huyBanThuocTrongNgayViewModel.TaiKhoanBenhNhanThuId,
                o => o.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.DonVTYTThanhToans).ThenInclude(x => x.DonVTYTThanhToanChiTiets)
                    .Include(x => x.TaiKhoanBenhNhan)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.TaiKhoanBenhNhanThus)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.DonThuocThanhToanChiTiet).ThenInclude(ct => ct.DonThuocThanhToan)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.DonThuocThanhToanChiTiet).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(ct => ct.NhapKhoDuocPhamChiTiet)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.DonThuocThanhToanChiTiet).ThenInclude(ct => ct.XuatKhoDuocPhamChiTietViTri).ThenInclude(ct => ct.XuatKhoDuocPhamChiTiet).ThenInclude(ct => ct.XuatKhoDuocPham)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.DonVTYTThanhToanChiTiet).ThenInclude(ct => ct.DonVTYTThanhToan)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.DonVTYTThanhToanChiTiet).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri).ThenInclude(ct => ct.NhapKhoVatTuChiTiet)
                    .Include(x => x.TaiKhoanBenhNhanChis).ThenInclude(c => c.DonVTYTThanhToanChiTiet).ThenInclude(ct => ct.XuatKhoVatTuChiTietViTri).ThenInclude(ct => ct.XuatKhoVatTuChiTiet).ThenInclude(ct => ct.XuatKhoVatTu)
                    .Include(x => x.CongTyBaoHiemTuNhanCongNos).ThenInclude(ct => ct.CongTyBaoHiemTuNhan)
                    .Include(x => x.MienGiamChiPhis));
            if (phieuThu.LoaiNoiThu != LoaiNoiThu.NhaThuoc)
            {
                throw new Exception("Phiếu thu không hợp lệ");
            }
            if (phieuThu.DaHuy == true)
            {
                throw new Exception("Phiếu thu đã được hủy");
            }

            //foreach (var phieuThuCongTyBaoHiemTuNhanCongNo in phieuThu.CongTyBaoHiemTuNhanCongNos)
            //{
            //    phieuThuCongTyBaoHiemTuNhanCongNo.CongTyBaoHiemTuNhan.CongTyBaoHiemTuNhanCongNos.Add(new CongTyBaoHiemTuNhanCongNo
            //    {
            //        TaiKhoanBenhNhanThuId = null,
            //        SoTien = phieuThuCongTyBaoHiemTuNhanCongNo.SoTien,
            //        YeuCauKhamBenhId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauKhamBenhId,
            //        YeuCauDichVuKyThuatId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauDichVuKyThuatId,
            //        YeuCauDuocPhamBenhVienId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauDuocPhamBenhVienId,
            //        YeuCauVatTuBenhVienId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauVatTuBenhVienId,
            //        YeuCauDichVuGiuongBenhVienId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauDichVuGiuongBenhVienId,
            //        DonThuocThanhToanChiTietId = phieuThuCongTyBaoHiemTuNhanCongNo.DonThuocThanhToanChiTietId,
            //        YeuCauGoiDichVuId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauGoiDichVuId,
            //        DonVTYTThanhToanChiTietId = phieuThuCongTyBaoHiemTuNhanCongNo.DonVTYTThanhToanChiTietId,
            //        YeuCauTruyenMauId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauTruyenMauId,
            //        YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = phieuThuCongTyBaoHiemTuNhanCongNo.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
            //    });
            //}
            //foreach (var mienGiamChiPhi in phieuThu.MienGiamChiPhis)
            //{
            //    phieuThu.YeuCauTiepNhan.MienGiamChiPhis.Add(new MienGiamChiPhi
            //    {
            //        TaiKhoanBenhNhanThuId = null,
            //        LoaiMienGiam = mienGiamChiPhi.LoaiMienGiam,
            //        SoTien = mienGiamChiPhi.SoTien,
            //        TheVoucherId = mienGiamChiPhi.TheVoucherId,
            //        YeuCauKhamBenhId = mienGiamChiPhi.YeuCauKhamBenhId,
            //        YeuCauDichVuKyThuatId = mienGiamChiPhi.YeuCauDichVuKyThuatId,
            //        YeuCauDuocPhamBenhVienId = mienGiamChiPhi.YeuCauDuocPhamBenhVienId,
            //        YeuCauVatTuBenhVienId = mienGiamChiPhi.YeuCauVatTuBenhVienId,
            //        YeuCauDichVuGiuongBenhVienId = mienGiamChiPhi.YeuCauDichVuGiuongBenhVienId,
            //        YeuCauGoiDichVuId = mienGiamChiPhi.YeuCauGoiDichVuId,
            //        DonThuocThanhToanChiTietId = mienGiamChiPhi.DonThuocThanhToanChiTietId,
            //        DonVTYTThanhToanChiTietId = mienGiamChiPhi.DonVTYTThanhToanChiTietId,
            //        LoaiChietKhau = mienGiamChiPhi.LoaiChietKhau,
            //        TiLe = mienGiamChiPhi.TiLe,
            //        MaTheVoucher = mienGiamChiPhi.MaTheVoucher,
            //        DoiTuongUuDaiId = mienGiamChiPhi.DoiTuongUuDaiId,
            //        YeuCauTruyenMauId = mienGiamChiPhi.YeuCauTruyenMauId,
            //        YeuCauDichVuGiuongBenhVienChiPhiBenhVienId = mienGiamChiPhi.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId
            //    });
            //}
            foreach (var phieuThuTaiKhoanBenhNhanChi in phieuThu.TaiKhoanBenhNhanChis.Where(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi))
            {
                phieuThuTaiKhoanBenhNhanChi.DaHuy = true;
                phieuThuTaiKhoanBenhNhanChi.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
                phieuThuTaiKhoanBenhNhanChi.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
                phieuThuTaiKhoanBenhNhanChi.LyDoHuy = huyBanThuocTrongNgayViewModel.LyDo;

                if (phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet != null)
                {
                    phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.SoTienBenhNhanDaChi = 0;
                    //phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;
                    //phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai = TrangThaiDonThuocThanhToan.ChuaXuatThuoc;
                    //phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DonThuocThanhToan.ThoiDiemCapThuoc = null;
                    //phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DonThuocThanhToan.NhanVienCapThuocId = null;
                    //phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DonThuocThanhToan.NoiCapThuocId = null;

                    phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                    phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai = TrangThaiDonThuocThanhToan.DaHuy;

                    var xuatKhoDuocPhamChiTietViTriTra = new XuatKhoDuocPhamChiTietViTri
                    {
                        XuatKhoDuocPhamChiTietId = phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTietId,
                        NhapKhoDuocPhamChiTietId = phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTietId,
                        NgayXuat = DateTime.Now,
                        SoLuongXuat = phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat * (-1)
                    };
                    phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTriTra);
                    phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;

                    //phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NgayXuat = null;
                    //phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.NgayXuat = null;
                    //phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId = null;
                    //if(phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
                    //    phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;
                    //phieuThuTaiKhoanBenhNhanChi.DonThuocThanhToanChiTietId = null;
                }
                if (phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet != null)
                {
                    phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.SoTienBenhNhanDaChi = 0;
                    //phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;
                    //phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThai = TrangThaiDonVTYTThanhToan.ChuaXuatVTYT;
                    //phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.ThoiDiemCapVTYT = null;
                    //phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.NhanVienCapVTYTId = null;
                    //phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.NoiCapVTYTId = null;
                    phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                    phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.DonVTYTThanhToan.TrangThai = TrangThaiDonVTYTThanhToan.DaHuy;

                    var xuatKhoVatTuChiTietViTriTra = new XuatKhoVatTuChiTietViTri
                    {
                        XuatKhoVatTuChiTietId = phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTietId,
                        NhapKhoVatTuChiTietId = phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTietId,
                        NgayXuat = DateTime.Now,
                        SoLuongXuat = phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat * (-1)
                    };
                    phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTriTra);
                    phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;

                    //phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NgayXuat = null;
                    //phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.NgayXuat = null;
                    //phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTuId = null;
                    //if(phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu != null)
                    //    phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.WillDelete = true;
                    //phieuThuTaiKhoanBenhNhanChi.DonVTYTThanhToanChiTietId = null;
                }
            }

            phieuThu.DaHuy = true;
            phieuThu.NhanVienHuyId = _userAgentHelper.GetCurrentUserId();
            phieuThu.NoiHuyId = _userAgentHelper.GetCurrentNoiLLamViecId();
            phieuThu.NgayHuy = DateTime.Now;
            phieuThu.LyDoHuy = huyBanThuocTrongNgayViewModel.LyDo;
            phieuThu.DaThuHoi = huyBanThuocTrongNgayViewModel.ThoiGianThuHoi != null;
            phieuThu.NhanVienThuHoiId = huyBanThuocTrongNgayViewModel.NguoiThuHoiId;
            phieuThu.NgayThuHoi = huyBanThuocTrongNgayViewModel.ThoiGianThuHoi;

            _taiKhoanBenhNhanThuRepository.Update(phieuThu);
        }

        #endregion

        public List<ThongTinBenhNhanGridVo> GetThongTinBenhNhanDetail(long yeuCauTiepNhanId, string idBenhNhan)
        {
            //todo: need improve
            var querys = _yeuCauTiepNhanRepo.TableNoTracking.Include(x => x.BenhNhan)
                .Where(x => x.Id == yeuCauTiepNhanId && x.BenhNhan.MaBN == idBenhNhan)
                .Select(s => new
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

                    //BVHD-3941
                    CoBaoHiemTuNhan = s.CoBHTN
                }).Distinct();
            //ThongTinBenhNhanGridVo
            var query = querys.Select(x => new ThongTinBenhNhanGridVo()
            {
                MaBN = x.MaBN,
                BenhNhanId = x.BenhNhanId,
                HoTen = x.HoTen,
                NamSinh = x.NamSinh.ToString(),
                GioiTinhHienThi = x.GioiTinhHienThi,
                DiaChi = x.DiaChi,
                DiaChiDayDu = x.DiaChiDayDu,
                SoDienThoai = x.SoDienThoai,
                Email = x.Email,

                //BVHD-3941
                CoBaoHiemTuNhan = x.CoBaoHiemTuNhan
            });

            return query.ToList();
        }
        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocDaXuatThuocBHYTLS(long tiepNhanId, long? idTaiKhoanBenhNhanThu)
        {
            var result = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                 .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                             o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocBHYT &&
                             o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)
                 .SelectMany(o => o.DonThuocThanhToanChiTiets)
                 .Select(x => new ThongTinDuocPhamQuayThuocVo()
                 {
                     Id = x.Id,
                     DuocPhamId = x.DuocPham.Id,
                     MaHoatChat = x.DuocPham.HoatChat,
                     NongDoVaHamLuong = x.DuocPham.HamLuong,
                     TenDuocPham = x.DuocPham.Ten,
                     //SoLuongTon = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongNhap - x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat,
                     //NhapKhoDuocPhamChiTietId = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.Id,
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
                     TaiKhoanbenhNhanThuId = x.TaiKhoanBenhNhanChis.Select(p => p.TaiKhoanBenhNhanThuId).ToList(),
                     TaiKhoanbenhNhanChiId = x.TaiKhoanBenhNhanChis.Select(c => c.Id).ToList()
                 });

            var resultVT = _donVTYTThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
               .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                           o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)
               .SelectMany(o => o.DonVTYTThanhToanChiTiets)
                  .Select(x => new ThongTinDuocPhamQuayThuocVo()
                  {
                      Id = x.Id,
                      DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                      TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                      //SoLuongTon = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongNhap - x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat,
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
                      TaiKhoanbenhNhanThuId = x.TaiKhoanBenhNhanChis.Select(p => p.TaiKhoanBenhNhanThuId).ToList(),
                  }).Where(x => x.VatTuBHYT == true);  // true lấy vật tư k bảo hiểm
            var allData = new List<ThongTinDuocPhamQuayThuocVo>();
            foreach (var item in result)
            {
                foreach (var itemx in item.TaiKhoanbenhNhanThuId)
                {
                    if (itemx == idTaiKhoanBenhNhanThu)
                    {
                        allData.Add(item);
                    }
                }
            }
            foreach (var item in resultVT)
            {
                foreach (var itemx in item.TaiKhoanbenhNhanThuId)
                {
                    if (itemx == idTaiKhoanBenhNhanThu)
                    {
                        allData.Add(item);
                    }
                }
            }
            return allData.ToList();
        }
        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocDaXuatThuocKhongBHYTLS(long tiepNhanId, long? idTaiKhoanBenhNhanThu)
        {
            var result = _donThuocThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
              .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                          o.LoaiDonThuoc == EnumLoaiDonThuoc.ThuocKhongBHYT && o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonThuocThanhToan.DaXuatThuoc)
              .SelectMany(o => o.DonThuocThanhToanChiTiets)
              .Select(x => new ThongTinDuocPhamQuayThuocVo()
              {

                  Id = x.Id,
                  DuocPhamId = x.DuocPham.Id,
                  MaHoatChat = x.DuocPham.HoatChat,
                  NongDoVaHamLuong = x.DuocPham.HamLuong,
                  TenDuocPham = x.DuocPham.Ten,
                  //SoLuongTon = x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongNhap - x.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat,
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
                  TaiKhoanbenhNhanThuId = x.TaiKhoanBenhNhanChis.Select(p => p.TaiKhoanBenhNhanThuId).ToList(),
              });
            var resultVT = _donVTYTThanhToanRepo.TableNoTracking.Include(x => x.YeuCauTiepNhan)
              .Where(o => o.YeuCauTiepNhanId == tiepNhanId &&
                          o.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan && o.TrangThai == TrangThaiDonVTYTThanhToan.DaXuatVTYT)
              .SelectMany(o => o.DonVTYTThanhToanChiTiets)
                 .Select(x => new ThongTinDuocPhamQuayThuocVo()
                 {
                     Id = x.Id,
                     DuocPhamId = x.VatTuBenhVien.VatTus.Id,
                     TenDuocPham = x.VatTuBenhVien.VatTus.Ten,
                     //SoLuongTon = x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongNhap - x.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat,
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
                     TaiKhoanbenhNhanThuId = x.TaiKhoanBenhNhanChis.Select(p => p.TaiKhoanBenhNhanThuId).ToList(),
                 }).Where(x => x.VatTuBHYT != true);  // true lấy vật tư k bảo hiểm
            var allData = new List<ThongTinDuocPhamQuayThuocVo>();
            foreach (var item in result)
            {
                foreach (var itemx in item.TaiKhoanbenhNhanThuId)
                {
                    if (itemx == idTaiKhoanBenhNhanThu)
                    {
                        allData.Add(item);
                    }
                }
            }
            foreach (var item in resultVT)
            {
                foreach (var itemx in item.TaiKhoanbenhNhanThuId)
                {
                    if (itemx == idTaiKhoanBenhNhanThu)
                    {
                        allData.Add(item);
                    }
                }
            }


            return allData.ToList();

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

        #region BVHD-3941

        public async Task<long> GetThongTinCongTyBaoHiemTuNhanTheoMaTN(string maTN)
        {
            var yctnId = _yeuCauTiepNhanRepo.TableNoTracking
                .Where(x => x.MaYeuCauTiepNhan.Equals(maTN)
                            && x.CoBHTN != null
                            && x.CoBHTN != false)
                .Select(x => x.Id).FirstOrDefault();
            return yctnId;
        }


        #endregion
    }
}
