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
using System.Drawing;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Globalization;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.Helpers;
using DotLiquid.Tags;
using static Camino.Core.Domain.Enums;
using Camino.Services.YeuCauKhamBenh;

namespace Camino.Services.YeuCauTiepNhans
{
    [ScopedDependency(ServiceType = typeof(IDanhSachChoKhamService))]

    public class DanhSachChoKhamService : MasterFileService<YeuCauTiepNhan>, IDanhSachChoKhamService
    {

        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.Users.User> _userRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        public DanhSachChoKhamService(IRepository<YeuCauTiepNhan> repository,
          IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> yeuCauTiepNhanRepository,
          IUserAgentHelper userAgentHelper,
          IRepository<Core.Domain.Entities.Users.User> userRepository,
          IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
          IYeuCauKhamBenhService yeuCauKhamBenhService,
          IRepository<Template> templateRepository) : base(repository)
        {
            _templateRepository = templateRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _userAgentHelper = userAgentHelper;
            _userRepository = userRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
        }
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachChoKham(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = BaseRepository.TableNoTracking
                .Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                .Select(s => new DanhSachChoKhamGridVo
                {
                    Id = s.Id,
                    MaYeuCauTiepNhan = s.MaYeuCauTiepNhan,
                    BenhNhanId = s.BenhNhanId,
                    MaBenhNhan = s.BenhNhan.MaBN,
                    HoTen = s.HoTen,
                    NamSinh = s.NamSinh,
                    NgayThangNam = s.NgaySinh != null && s.NgaySinh != 0 && s.ThangSinh != null && s.ThangSinh != 0 && s.NamSinh != null ? new DateTime(s.NamSinh ?? 1500, s.ThangSinh ?? 1, s.NgaySinh ?? 1).ToString("dd/MM/yyyy") : s.NamSinh.ToString(),
                    DiaChi = s.DiaChiDayDu,
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                    ThoiDiemTiepNhan = s.ThoiDiemTiepNhan,
                    TrieuChungTiepNhan = s.TrieuChungTiepNhan,
                    BHYTMaSoThe = s.BHYTMaSoThe,
                    DoiTuong = (s.CoBHYT != true) ? "Viện phí" : "BHYT (" + s.BHYTMucHuong.ToString() + "%)",
                    CoBHYT = s.CoBHYT,
                    TenNhanVienTiepNhan = s.NhanVienTiepNhan.User.HoTen,
                    //CoYeuCauKhamBenhNhapVien = s.YeuCauKhamBenhs.Any(z => z.CoNhapVien == true)
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);
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
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                        g => g.HoTen,
                        g => g.MaYeuCauTiepNhan,
                        g => g.NamSinh.ToString(),
                        g => g.DiaChi,
                        g => g.MaBenhNhan,
                        g => g.TrieuChungTiepNhan,
                        g => g.TenNhanVienTiepNhan

                   );

                }

            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.HoTen,
                    g => g.MaYeuCauTiepNhan,
                    g => g.NamSinh.ToString(),
                    g => g.DiaChi,
                    g => g.MaBenhNhan,
                    g => g.TrieuChungTiepNhan,
                    g => g.TenNhanVienTiepNhan

                    );

            }
            //query = query.Where(o =>o.ThoiDiemTiepNhan.Value.Date == DateTime.Now.Date);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var queryResult = queryTask.Result;

            if (queryResult.Length > 0)
            {
                var yctnIds = queryResult.Select(o => o.Id).ToList();

                var yeuCauTiepNhanCoNhapVienIds = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(o => o.CoNhapVien == true && yctnIds.Contains(o.YeuCauTiepNhanId))
                    .Select(o => o.YeuCauTiepNhanId)
                    .ToList();

                foreach (var danhSachChoKham in queryResult)
                {
                    if (yeuCauTiepNhanCoNhapVienIds.Contains(danhSachChoKham.Id))
                    {
                        danhSachChoKham.CoYeuCauKhamBenhNhapVien = true;
                    }
                }
            }

            return new GridDataSource { Data = queryResult, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachChoKham(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                .Select(s => new DanhSachChoKhamGridVo
                {
                    Id = s.Id,
                    MaYeuCauTiepNhan = s.MaYeuCauTiepNhan,
                    BenhNhanId = s.BenhNhanId,
                    MaBenhNhan = s.BenhNhan.MaBN,
                    HoTen = s.HoTen,
                    NamSinh = s.NamSinh,
                    DiaChi = s.DiaChiDayDu,
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                    ThoiDiemTiepNhan = s.ThoiDiemTiepNhan,
                    TrieuChungTiepNhan = s.TrieuChungTiepNhan,
                    BHYTMaSoThe = s.BHYTMaSoThe,
                    DoiTuong = (s.CoBHYT != true) ? "Viện phí" : "BHYT (" + s.BHYTMucHuong.ToString() + "%)",
                    CoBHYT = s.CoBHYT,
                    TenNhanVienTiepNhan = s.NhanVienTiepNhan.User.HoTen                    
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DanhSachChoKhamGridVo>(queryInfo.AdditionalSearchString);
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
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => p.ThoiDiemTiepNhan >= tuNgay && p.ThoiDiemTiepNhan <= denNgay);
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                        g => g.HoTen,
                        g => g.MaYeuCauTiepNhan,
                        g => g.NamSinh.ToString(),
                        g => g.DiaChi,
                        g => g.MaBenhNhan,
                        g => g.TrieuChungTiepNhan,
                        g => g.TenNhanVienTiepNhan

                   );
                }

            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                      g => g.HoTen,
                      g => g.MaYeuCauTiepNhan,
                      g => g.NamSinh.ToString(),
                      g => g.DiaChi,
                      g => g.MaBenhNhan,
                      g => g.TrieuChungTiepNhan,
                      g => g.TenNhanVienTiepNhan
                  );
            }
            //query = query.Where(o =>o.ThoiDiemTiepNhan.Value.Date == DateTime.Now.Date);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };

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

        public ThongTinChungCuaBenhNhan ThongTinBenhNhanHienTai(long yeuCauTiepNhanId)
        {
            var ycHienTai = BaseRepository.GetById(yeuCauTiepNhanId,
                                       x => x.Include(yckb => yckb.NguoiLienHeQuanHeNhanThan)
                                             .Include(yctn => yctn.NgheNghiep)
                                             .Include(yctn => yctn.PhuongXa)
                                             .Include(yctn => yctn.QuanHuyen)
                                             .Include(yctn => yctn.QuocTich)
                                             .Include(yctn => yctn.TinhThanh)
                                             .Include(yctn => yctn.BenhNhan)
                                             .Include(yctn => yctn.DanToc));

            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var tenuserCurrent = _userRepository.TableNoTracking
                                 .Where(u => u.Id == userCurrentId).Select(u =>
                                 (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                               //+ (u.NhanVien.ChucDanh != null ? u.NhanVien.ChucDanh.NhomChucDanh.Ma + "." : "")
                               + u.HoTen).FirstOrDefault();
            var tuoiThoiDiemHienTai = 0;
            if (ycHienTai.NamSinh != null)
            {
                tuoiThoiDiemHienTai = DateTime.Now.Year - ycHienTai.NamSinh.Value;
            }
            var dobConvert = DateHelper.ConvertDOBToTimeJson(ycHienTai.NgaySinh, ycHienTai.ThangSinh, ycHienTai.NamSinh);
            var jsonConvertString = new NgayThangNamSinhVo();

            if (!string.IsNullOrEmpty(dobConvert) && tuoiThoiDiemHienTai < 6)
            {
                jsonConvertString = JsonConvert.DeserializeObject<NgayThangNamSinhVo>(dobConvert);
            }

            var tuoiBenhNhan = ycHienTai.NamSinh != null ?
                            (tuoiThoiDiemHienTai < 6 ? jsonConvertString.Years + " Tuổi " + jsonConvertString.Months + " Tháng " + jsonConvertString.Days + " Ngày" : tuoiThoiDiemHienTai.ToString()) : tuoiThoiDiemHienTai.ToString();

            var result = new ThongTinChungCuaBenhNhan
            {
                MaTN = ycHienTai.MaYeuCauTiepNhan,
                MaBN = ycHienTai.BenhNhan.MaBN,
                HoTenBenhNhan = ycHienTai.HoTen.ToUpper(),
                SinhNgay = DateHelper.DOBFormat(ycHienTai.NgaySinh, ycHienTai.ThangSinh, ycHienTai.NamSinh),
                Tuoi = tuoiBenhNhan ?? "0",
                GioiTinh = ycHienTai.GioiTinh.GetDescription(),
                NgheNghiep = ycHienTai.NgheNghiep?.Ten,
                DanToc = ycHienTai.DanToc?.Ten,
                QuocTich = ycHienTai.QuocTich?.Ten,
                SoNha = ycHienTai.DiaChi,
                DiaChiDayDu = ycHienTai.DiaChiDayDu,
                XaPhuong = ycHienTai.PhuongXa?.Ten,
                Huyen = ycHienTai.QuanHuyen?.Ten,
                TinhThanhPho = ycHienTai.TinhThanh?.Ten,
                NoiLamViec = ycHienTai.NoiLamViec,
                DoiTuong = ycHienTai.CoBHYT != true ? "Viện phí" : "BHYT (" + ycHienTai.BHYTMucHuong.ToString() + "%)",
                BHYTNgayHetHan = (ycHienTai.BHYTNgayHieuLuc != null || ycHienTai.BHYTNgayHetHan != null) ? "từ " + (ycHienTai.BHYTNgayHieuLuc?.ApplyFormatDate() ?? "") + " đến " + (ycHienTai.BHYTNgayHetHan?.ApplyFormatDate() ?? "") : "",
                BHYTMaSoThe = ycHienTai.BHYTMaSoThe + (ycHienTai.BHYTMaDKBD == null ? null : " - " + ycHienTai.BHYTMaDKBD),
                BHYTMaSoThe2 = ycHienTai.BHYTMaSoThe,
                NguoiLienHeQuanHeThanNhan = ycHienTai.NguoiLienHeQuanHeNhanThan?.Ten + (ycHienTai.NguoiLienHeHoTen != null ? " " + ycHienTai.NguoiLienHeHoTen : ""),
                //NguoiLienHeDiaChiDayDu = ycHienTai.NguoiLienHeDiaChiDayDu,
                NguoiLienHeQuanSoDienThoai = ycHienTai.NguoiLienHeSoDienThoai.ApplyFormatPhone(),
                SoDienThoai = ycHienTai.SoDienThoai.ApplyFormatPhone(),
                ThoiDiemTiepNhan = ycHienTai.ThoiDiemTiepNhan.ConvertDatetimeToString(),
                ThoiDiemTiepNhanFormat = ycHienTai.ThoiDiemTiepNhan.ApplyFormatDate(),
                Ngay = DateTime.Today.Day.ConvertDateToString(),
                Thang = DateTime.Today.Month.ConvertMonthToString(),
                Nam = DateTime.Today.Year.ToString(),
                HoTenBacSi = tenuserCurrent,
            };
            return result;
        }

        public List<string> InPhieuCacDichVuKhamBenh(long yeuCauTiepNhanId, string hostingName, bool header, bool laPhieuKhamBenh)
        {
            var thongTinChungBN = ThongTinBenhNhanHienTai(yeuCauTiepNhanId);
            var content = string.Empty;
            var contents = new List<string>();


            var ycTiepNhanHienTai = BaseRepository.GetById(yeuCauTiepNhanId,
                                     x => x.Include(yctn => yctn.KetQuaSinhHieus)
                                           .Include(yctn => yctn.BenhNhan).ThenInclude(bn => bn.BenhNhanTienSuBenhs)
                                           .Include(yctn => yctn.BenhNhan).ThenInclude(bn => bn.BenhNhanDiUngThuocs)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.DuocPham).ThenInclude(z => z.DonViTinh)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.DuocPham).ThenInclude(z => z.DuongDung)
                                            .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuongDung)
                                            .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.YeuCauVatTuBenhViens)
                                           .Include(yctn => yctn.YeuCauDuocPhamBenhViens)
                                           .Include(yctn => yctn.YeuCauVatTuBenhViens)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.YeuCauDichVuKyThuats).ThenInclude(yckb => yckb.DichVuKyThuatBenhVien)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.YeuCauKhamBenhKhamBoPhanKhacs)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.YeuCauKhamBenhBoPhanTonThuongs)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.KhoaPhongNhapVien)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.NoiThucHien).ThenInclude(nth => nth.KhoaPhong)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.BacSiThucHien).ThenInclude(bs => bs.User)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.BacSiKetLuan).ThenInclude(z => z.HocHamHocVi)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.BacSiKetLuan).ThenInclude(bs => bs.User)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.Icdchinh)
                                           .Include(yctn => yctn.YeuCauKhamBenhs).ThenInclude(yckb => yckb.YeuCauKhamBenhICDKhacs).ThenInclude(yckb => yckb.ICD)

                                           );

            var yeuCauDVKTs = string.Empty;

            var lstyeuCauKhamBenhs = ycTiepNhanHienTai.YeuCauKhamBenhs.Where(yc => yc.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham).ToList();

            var tienSuDiUngThuoc = string.Empty;
            var tienSuDiUngThucAn = string.Empty;
            var tienSuDiUngKhac = string.Empty;
            if (laPhieuKhamBenh)
            {
                var index = 0;
                foreach (var yeuCauKhamBenh in lstyeuCauKhamBenhs)
                {
                    var tienSuBenhBanThan = string.Empty;
                    var tienSuBenhGiaDinh = string.Empty;
                    var contentYeuCauKhamBenh = string.Empty;
                    //var yeuCauDuocPhamBenhViens = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Select(z => z.Ten).Distinct().ToList();
                    //var coNoiDungKhac = _yeuCauKhamBenhService.KiemTraDichVuHienTaiCoNhieuNguoiBenhAsync(yeuCauKhamBenh.DichVuKhamBenhBenhVienId).Result;

                    var yeuCauDuocPhamBenhViensHuongThanGayNghien = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z =>
                                                                                                   z.DuongDung.Ma.Trim() != "2.10".Trim()
                                                                                                && z.DuongDung.Ma.Trim() != "1.01".Trim()
                                                                                                && z.DuongDung.Ma.Trim() != "4.04".Trim()
                                                                                                && z.DuongDung.Ma.Trim() != "3.05".Trim()
                                                                                                && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                                   || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViensTiem = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "2.10".Trim()
                                                                                             && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                             && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViensUong = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "1.01".Trim()
                                                                                             && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                             && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViensDat = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "4.04".Trim()
                                                                                             && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                             && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViensDungNgoai = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "3.05".Trim()
                                                                                             && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                             && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();

                    var yeuCauDuocPhamBenhViensKhac = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() != "2.10".Trim()
                                                                                                     && z.DuongDung.Ma.Trim() != "1.01".Trim()
                                                                                                     && z.DuongDung.Ma.Trim() != "4.04".Trim()
                                                                                                     && z.DuongDung.Ma.Trim() != "3.05".Trim()
                                                                                                     && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                     && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)
                                                                                                     ).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViens = (yeuCauDuocPhamBenhViensHuongThanGayNghien
                                                        .Concat(yeuCauDuocPhamBenhViensTiem)
                                                        .Concat(yeuCauDuocPhamBenhViensUong)
                                                        .Concat(yeuCauDuocPhamBenhViensDat)
                                                        .Concat(yeuCauDuocPhamBenhViensDungNgoai)
                                                        .Concat(yeuCauDuocPhamBenhViensKhac)).Distinct();
                    var yeuCauVatTuBenhViens = yeuCauKhamBenh.YeuCauVatTuBenhViens.Where(z => z.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDichVuKyThuats = yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(z => z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                          && !string.IsNullOrEmpty(z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat)
                                                                          && z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat.Substring(0, 1).ToLower() != "p"
                                                                            #region Cập nhật 26/12/2022: bỏ trạng thái đã huỷ
                                                                            && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                            #endregion
                                                                          ).Select(z => z.TenDichVu).Distinct().ToList();

                    var icdPhus = new List<string>();
                    foreach (var item in yeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
                    {
                        icdPhus.Add(item.ICD.Ma + " - " + item.ICD.TenTiengViet + (!string.IsNullOrEmpty(item.GhiChu) ? " (" + item.GhiChu?.Replace("\n", "<br>") + ")" : ""));
                    }
                    var ketQuaSinhHieu = yeuCauKhamBenh.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(kq => kq.Id).FirstOrDefault();
                    foreach (var item in yeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanTienSuBenhs)
                    {
                        if (item.LoaiTienSuBenh == Enums.EnumLoaiTienSuBenh.BanThan)
                        {
                            if (!string.IsNullOrEmpty(item.TenBenh))
                            {
                                tienSuBenhBanThan += item.TenBenh + "; ";
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.TenBenh))
                            {
                                tienSuBenhGiaDinh += item.TenBenh + "; ";
                            }
                        }
                    }

                    foreach (var item in yeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs)
                    {
                        if (item.LoaiDiUng == Enums.LoaiDiUng.Thuoc)
                        {
                            if (!string.IsNullOrEmpty(item.TenDiUng))
                            {
                                tienSuDiUngThuoc += item.TenDiUng + "; ";
                            }
                        }
                        else if (item.LoaiDiUng == Enums.LoaiDiUng.ThucAn)
                        {
                            if (!string.IsNullOrEmpty(item.TenDiUng))
                            {
                                tienSuDiUngThucAn += item.TenDiUng + "; ";
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.TenDiUng))
                            {
                                tienSuDiUngKhac += item.TenDiUng + "; ";
                            }
                        }
                    }

                    var phongKham = string.Empty;
                    if (yeuCauKhamBenh.TenDichVu.RemoveUniKeyAndToLower().Contains("Khám Mắt".RemoveUniKeyAndToLower()))
                    {
                        var templatePhieuKhamBenhKM = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenhVaoVienChuyenKhoaMat")).FirstOrDefault();
                        phongKham = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == yeuCauKhamBenh.NoiThucHienId).Select(v => v.Ten).FirstOrDefault();
                        var templateDichVuMat = yeuCauKhamBenh.ThongTinKhamTheoDichVuData;
                        var thongTinBenhNhan = new ThongTinBenhNhanTaiMuiHongList();
                        var thongTinBenhMatChiTiet = new ThongTinBenhMatChiTiet();
                        if (!string.IsNullOrEmpty(templateDichVuMat))
                        {
                            thongTinBenhNhan = JsonConvert.DeserializeObject<ThongTinBenhNhanTaiMuiHongList>(templateDichVuMat);
                            foreach (var item in thongTinBenhNhan.DataKhamTheoTemplate)
                            {
                                if (item.Id == "ThiLucKhongKinh")
                                {
                                    thongTinBenhMatChiTiet.ThiLucKhongKinh = item.Value;
                                }
                                if (item.Id == "ThiLucKhongKinhMatPhai")
                                {
                                    thongTinBenhMatChiTiet.MPKK = item.Value;
                                }
                                if (item.Id == "ThiLucKhongKinhMatTrai")
                                {
                                    thongTinBenhMatChiTiet.MTKK = item.Value;
                                }
                                if (item.Id == "NhanAp")
                                {
                                    thongTinBenhMatChiTiet.NhanAp = item.Value;
                                }
                                if (item.Id == "NhanApMatPhai")
                                {
                                    thongTinBenhMatChiTiet.MPNA = item.Value;
                                }
                                if (item.Id == "NhanApMatTrai")
                                {
                                    thongTinBenhMatChiTiet.MTNA = item.Value;
                                }
                                if (item.Id == "ThiLucCoKinh")
                                {
                                    thongTinBenhMatChiTiet.ThiLucCoKinh = item.Value;
                                }
                                if (item.Id == "ThiLucCoKinhPhai")
                                {
                                    thongTinBenhMatChiTiet.MPCK = item.Value;
                                }
                                if (item.Id == "ThiLucCoKinhTrai")
                                {
                                    thongTinBenhMatChiTiet.MTCK = item.Value;
                                }
                                if (item.Id == "KhamMatNoiDung")
                                {
                                    thongTinBenhMatChiTiet.KhamMatNoiDung = item.Value;
                                }
                            }
                        }

                        var data = new ThongTinBenhMat
                        {
                            MaBN = thongTinChungBN.MaBN,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(thongTinChungBN?.MaBN, 210, 56),
                            HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                            SinhNgay = thongTinChungBN?.SinhNgay,
                            GioiTinh = thongTinChungBN?.GioiTinh,
                            NgheNghiep = thongTinChungBN?.NgheNghiep,
                            DanToc = thongTinChungBN?.DanToc,
                            QuocTich = thongTinChungBN?.QuocTich,
                            SoNha = thongTinChungBN?.SoNha,
                            XaPhuong = thongTinChungBN?.XaPhuong,
                            Huyen = thongTinChungBN?.Huyen,
                            TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                            NoiLamViec = thongTinChungBN?.NoiLamViec,
                            DoiTuong = thongTinChungBN?.DoiTuong,
                            BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                            BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,
                            NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),
                            SoDienThoaiQHTN = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                            ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu?.Replace("\n", "<br>"),
                            ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                            LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                            LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                            TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Tiền sử bệnh bản thân: " + tienSuBenhBanThan : "")
                                    + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Tiền sử bệnh gia đình: " + tienSuBenhGiaDinh : ""),
                            TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                                ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                                + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                                + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                            //ToanThan = yeuCauKhamBenh?.KhamToanThan,
                            //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") : yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>"),
                            NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                            KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                            TomTatCLS = yeuCauKhamBenh?.TomTatKetQuaCLS?.Replace("\n", "<br>"),
                            //DaXuLi = string.Join("; ", daXuLis),
                            DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                            HuongXuLi = yeuCauKhamBenh.CachGiaiQuyet?.Replace("\n", "<br>"),
                            PhongKham = phongKham,
                            KhamMat = thongTinBenhMatChiTiet.KhamMatNoiDung?.Replace("\n", "<br>"),
                            MPCK = thongTinBenhMatChiTiet.MPCK,
                            MPKK = thongTinBenhMatChiTiet.MPKK,
                            MTCK = thongTinBenhMatChiTiet.MTCK,
                            MTKK = thongTinBenhMatChiTiet.MTKK,
                            MTNA = thongTinBenhMatChiTiet.MTNA,
                            MPNA = thongTinBenhMatChiTiet.MPNA,
                            //HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan?.User.HoTen,
                            HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                            QuaTrinhBenhLy = thongTinChungBN?.BenhSu?.Replace("\n", "<br>"),
                            ChanDoanVaoVien = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),

                            //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                            //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                            NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),

                            NgayGioIn = DateTime.Now.ApplyFormatFullDateTime()
                        };
                        contentYeuCauKhamBenh = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenhKM.Body, data);
                        if (contentYeuCauKhamBenh != "")
                        {
                            contentYeuCauKhamBenh = contentYeuCauKhamBenh + "<div class=\"pagebreak\"> </div>";
                        };
                        contents.Add(contentYeuCauKhamBenh);

                    }
                    else if (yeuCauKhamBenh.TenDichVu.RemoveUniKeyAndToLower().Contains("Khám Tai mũi họng".RemoveUniKeyAndToLower()))
                    {
                        var templatePhieuKhamBenhTMH = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenhVaoVienTaiMuiHong")).FirstOrDefault();
                        phongKham = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == yeuCauKhamBenh.NoiThucHienId).Select(v => v.Ten).FirstOrDefault();
                        var templateDichVuTMH = yeuCauKhamBenh.ThongTinKhamTheoDichVuData;
                        var thongTinBenhNhan = new ThongTinBenhNhanTaiMuiHongList();
                        var thongTinBenhTMHChiTiet = new ThongTinBenhNhanTaiMuiHongChiTiet();
                        if (!string.IsNullOrEmpty(templateDichVuTMH))
                        {
                            thongTinBenhNhan = JsonConvert.DeserializeObject<ThongTinBenhNhanTaiMuiHongList>(templateDichVuTMH);
                            foreach (var item in thongTinBenhNhan.DataKhamTheoTemplate)
                            {
                                if (item.Id == "Tai")
                                {
                                    thongTinBenhTMHChiTiet.Tai = item.Value;
                                }
                                if (item.Id == "Mui")
                                {
                                    thongTinBenhTMHChiTiet.Mui = item.Value;
                                }
                                if (item.Id == "Hong")
                                {
                                    thongTinBenhTMHChiTiet.Hong = item.Value;
                                }
                            }
                        }

                        var data = new ThongTinBenhTaiMuiHong
                        {
                            MaBN = thongTinChungBN.MaBN,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(thongTinChungBN?.MaBN, 210, 56),
                            HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                            SinhNgay = thongTinChungBN?.SinhNgay,
                            GioiTinh = thongTinChungBN?.GioiTinh,
                            NgheNghiep = thongTinChungBN?.NgheNghiep,
                            DanToc = thongTinChungBN?.DanToc,
                            QuocTich = thongTinChungBN?.QuocTich,
                            SoNha = thongTinChungBN?.SoNha,
                            XaPhuong = thongTinChungBN?.XaPhuong,
                            Huyen = thongTinChungBN?.Huyen,
                            TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                            NoiLamViec = thongTinChungBN?.NoiLamViec,
                            DoiTuong = thongTinChungBN?.DoiTuong,
                            BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                            BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,
                            NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),
                            SoDienThoaiQHTN = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                            ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu?.Replace("\n", "<br>"),
                            //ThoiDiemTiepNhan = thongTinChungBN?.ThoiDiemTiepNhan,
                            ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                            LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                            LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                            TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Tiền sử bệnh bản thân: " + tienSuBenhBanThan : "")
                                    + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Tiền sử bệnh gia đình: " + tienSuBenhGiaDinh : ""),
                            TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                                ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                                + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                                + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                            //ToanThan = yeuCauKhamBenh?.KhamToanThan,
                            //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") : yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>"),
                            NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                            KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                            TomTatCLS = yeuCauKhamBenh?.TomTatKetQuaCLS?.Replace("\n", "<br>"),
                            //DaXuLi = string.Join("; ", daXuLis),
                            DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                            HuongXuLi = yeuCauKhamBenh.CachGiaiQuyet?.Replace("\n", "<br>"),
                            PhongKham = phongKham,
                            Tai = thongTinBenhTMHChiTiet.Tai,
                            Mui = thongTinBenhTMHChiTiet.Mui,
                            Hong = thongTinBenhTMHChiTiet.Hong,
                            HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                            QuaTrinhBenhLy = thongTinChungBN?.BenhSu?.Replace("\n", "<br>"),
                            ChanDoanVaoVien = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),

                            //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                            //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                            NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),

                            NgayGioIn = DateTime.Now.ApplyFormatFullDateTime()
                        };
                        contentYeuCauKhamBenh = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenhTMH.Body, data);
                        if (contentYeuCauKhamBenh != "")
                        {
                            contentYeuCauKhamBenh = contentYeuCauKhamBenh + "<div class=\"pagebreak\"> </div>";
                        };
                        contents.Add(contentYeuCauKhamBenh);
                    }
                    else if (yeuCauKhamBenh.TenDichVu.RemoveUniKeyAndToLower().Contains("Khám Răng hàm mặt".RemoveUniKeyAndToLower()))
                    {
                        var templatePhieuKhamBenhRHM = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenhVaoVienRangHamMat")).FirstOrDefault();
                        phongKham = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == yeuCauKhamBenh.NoiThucHienId).Select(v => v.Ten).FirstOrDefault();
                        var templateDichVuTMH = yeuCauKhamBenh.ThongTinKhamTheoDichVuData;
                        var thongTinBenhNhan = new ThongTinBenhNhanTaiMuiHongList();
                        var thongTinBenhTMHChiTiet = new ThongTinBenhRHMChiTiet();
                        if (!string.IsNullOrEmpty(templateDichVuTMH))
                        {
                            thongTinBenhNhan = JsonConvert.DeserializeObject<ThongTinBenhNhanTaiMuiHongList>(templateDichVuTMH);
                            foreach (var item in thongTinBenhNhan.DataKhamTheoTemplate)
                            {
                                if (item.Id == "Rang")
                                {
                                    thongTinBenhTMHChiTiet.Rang = item.Value;
                                }
                                if (item.Id == "Ham")
                                {
                                    thongTinBenhTMHChiTiet.Ham = item.Value;
                                }
                                if (item.Id == "Mat")
                                {
                                    thongTinBenhTMHChiTiet.Mat = item.Value;
                                }
                            }
                        }

                        var data = new ThongTinBenhRangHamMat
                        {
                            MaBN = thongTinChungBN.MaBN,
                            BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(thongTinChungBN?.MaBN, 210, 56),
                            HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                            SinhNgay = thongTinChungBN?.SinhNgay,
                            GioiTinh = thongTinChungBN?.GioiTinh,
                            NgheNghiep = thongTinChungBN?.NgheNghiep,
                            DanToc = thongTinChungBN?.DanToc,
                            QuocTich = thongTinChungBN?.QuocTich,
                            SoNha = thongTinChungBN?.SoNha,
                            XaPhuong = thongTinChungBN?.XaPhuong,
                            Huyen = thongTinChungBN?.Huyen,
                            TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                            NoiLamViec = thongTinChungBN?.NoiLamViec,
                            DoiTuong = thongTinChungBN?.DoiTuong,
                            BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                            BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,
                            NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),
                            SoDienThoaiQHTN = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                            ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu?.Replace("\n", "<br>"),
                            //ThoiDiemTiepNhan = thongTinChungBN?.ThoiDiemTiepNhan,
                            ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                            LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                            LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                            TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Tiền sử bệnh bản thân: " + tienSuBenhBanThan : "")
                                    + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Tiền sử bệnh gia đình: " + tienSuBenhGiaDinh : ""),
                            TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                                ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                                + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                                + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                            //ToanThan = yeuCauKhamBenh?.KhamToanThan,
                            //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") : yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>"),
                            NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                            KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                            TomTatCLS = yeuCauKhamBenh?.TomTatKetQuaCLS?.Replace("\n", "<br>"),
                            //DaXuLi = string.Join("; ", daXuLis),
                            DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                            HuongXuLi = yeuCauKhamBenh.CachGiaiQuyet?.Replace("\n", "<br>"),
                            PhongKham = phongKham,
                            Rang = thongTinBenhTMHChiTiet.Rang,
                            Ham = thongTinBenhTMHChiTiet.Ham,
                            Mat = thongTinBenhTMHChiTiet.Mat,
                            HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                            QuaTrinhBenhLy = thongTinChungBN?.BenhSu?.Replace("\n", "<br>"),
                            ChanDoanVaoVien = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),

                            //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                            //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                            NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),

                            NgayGioIn = DateTime.Now.ApplyFormatFullDateTime()
                        };
                        contentYeuCauKhamBenh = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenhRHM.Body, data);
                        if (contentYeuCauKhamBenh != "")
                        {
                            contentYeuCauKhamBenh = contentYeuCauKhamBenh + "<div class=\"pagebreak\"> </div>";
                        };
                        contents.Add(contentYeuCauKhamBenh);
                    }
                    else
                    {
                        var templatePhieuKhamBenh = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenh2")).FirstOrDefault();
                        phongKham = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == yeuCauKhamBenh.NoiThucHienId).Select(v => v.Ten).FirstOrDefault();
                        //foreach (var item in yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(p => p.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien))
                        //{
                        //    yeuCauDVKTs += item.TenDichVu + "; ";
                        //}
                        var cacBoPhanKhac = string.Empty;
                        var lstCacBoPhanKhacs = yeuCauKhamBenh.YeuCauKhamBenhKhamBoPhanKhacs;
                        var khamBenh = string.Empty;
                        foreach (var item in lstCacBoPhanKhacs)
                        {
                            cacBoPhanKhac += "-Bộ phận: " + item.Ten + ", Mô tả: " + item.NoiDUng + "; <br>";
                        }

                        var lstHinhVeTonThuong = yeuCauKhamBenh.YeuCauKhamBenhBoPhanTonThuongs.ToList();
                        var imageStr = string.Empty;
                        var lstHinhVeTonThuongCount = lstHinhVeTonThuong.Count();
                        for (var i = 0; i < lstHinhVeTonThuongCount; i++)
                        {
                            if (i > 0)
                            {
                                imageStr = imageStr
                               + "<tr>"
                                       + "<td colspan='2' style='border-top: 1px dashed gray;'>"
                                           + "<img style='height: 300;width: 350px;align: top' src='" + lstHinhVeTonThuong[i].HinhAnh + "'/>"
                                       + "</td>"

                                + "</tr>"
                                 + "<tr>"
                                       + "<td colspan='2' style='font-size:15px; vertical-align: top;border-top: 1px dashed gray; text-align:justify;'><b>"
                                           + lstHinhVeTonThuong[i].MoTa
                                         + "</b></td>"
                                + "</tr>"
                                ;
                            }
                            else
                            {
                                imageStr = imageStr
                               + "<tr>"
                                       + "<td colspan='2'>"
                                           + "<img style='height: 300px;width: 350px;vertical-align: top' src='" + lstHinhVeTonThuong[i].HinhAnh + "'/>"
                                       + "</td>"
                                + "</tr>"

                                + "<tr>"
                                       + "<td colspan='2' style='font-size:15px; vertical-align: top; text-align:justify;'><b>"
                                           + lstHinhVeTonThuong[i].MoTa
                                         + "</b></td>"
                                + "</tr>"
                                ;
                            }
                        }
                        //var dataKhamBenh = !string.IsNullOrEmpty(yeuCauKhamBenh.KhamToanThan) ? "Khám toàn thân: " + yeuCauKhamBenh.KhamToanThan + "<br>" : "";
                        //var dataKhamBenh = coNoiDungKhac ? (!string.IsNullOrEmpty(yeuCauKhamBenh.NoiDungKhamBenh) ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") + "<br>" : "")
                        //                                   : (!string.IsNullOrEmpty(yeuCauKhamBenh.KhamToanThan) ? "Khám toàn thân: " + yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>") + "<br>" : "");
                        var dataKhamBenh = string.Empty;
                        if (!string.IsNullOrEmpty(yeuCauKhamBenh.NoiDungKhamBenh))
                        {
                            dataKhamBenh += yeuCauKhamBenh.NoiDungKhamBenh + "<br>";
                        }
                        if (!string.IsNullOrEmpty(yeuCauKhamBenh.KhamToanThan))
                        {
                            dataKhamBenh += "Khám toàn thân: " + yeuCauKhamBenh.KhamToanThan + "<br>";
                        }
                        if (!string.IsNullOrEmpty(yeuCauKhamBenh.ThongTinKhamTheoDichVuData) && !string.IsNullOrEmpty(yeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate))
                        {
                            var thongTinKhamTheoDichVuData =
                                 JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(yeuCauKhamBenh
                                     .ThongTinKhamTheoDichVuData);
                            var thongTinBenhNhanKhamKhacTemplate =
                                JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(yeuCauKhamBenh
                                    .ThongTinKhamTheoDichVuTemplate);
                            foreach (var item in thongTinBenhNhanKhamKhacTemplate.ComponentDynamics)
                            {
                                if (item.Type == 4 && item.groupItems != null && item.groupItems.Count > 0)
                                {
                                    var itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == item.Id);
                                    if (itemData != null)
                                    {
                                        dataKhamBenh += item.Label + ": " + itemData?.Value + "<br>";
                                    }
                                    else
                                    {
                                        itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o =>
                                            item.groupItems.Any(g => g.Id == o.Id));
                                        if (itemData != null)
                                        {
                                            dataKhamBenh += item.Label + ": ";
                                            var i = 0;
                                            foreach (var itemGroup in item.groupItems)
                                            {
                                                if (itemGroup.Type == 1)
                                                {
                                                    dataKhamBenh += "<br>" + itemGroup.Label + ": ";
                                                }
                                                else
                                                {
                                                    var itemDataGroup = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == itemGroup.Id);
                                                    if (itemDataGroup != null)
                                                    {
                                                        dataKhamBenh += " +" + itemGroup.Label + ": " + itemDataGroup.Value;
                                                    }
                                                }
                                                i++;
                                                if (i == item.groupItems.Count)
                                                {
                                                    dataKhamBenh += "<br>";
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    var itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == item.Id);
                                    if (itemData != null)
                                    {
                                        dataKhamBenh += item.Label + ": " + itemData?.Value + "<br>";
                                    }
                                }
                            }
                        }
                        var cacBoPhanKhacs = string.Empty;
                        if (!string.IsNullOrEmpty(cacBoPhanKhac))
                        {
                            cacBoPhanKhacs = "Các bộ phận khác: <br>" + cacBoPhanKhac;
                        }
                        khamBenh += "<br>" + dataKhamBenh + cacBoPhanKhacs + imageStr;
                        var data = new ThongTinBenhKhac
                        {
                            MaTN = thongTinChungBN.MaTN,
                            So = thongTinChungBN.MaTN,
                            HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                            SinhNgay = thongTinChungBN?.SinhNgay,
                            GioiTinh = thongTinChungBN?.GioiTinh,
                            NgheNghiep = thongTinChungBN?.NgheNghiep,
                            DanToc = thongTinChungBN?.DanToc,
                            QuocTich = thongTinChungBN?.QuocTich,
                            SoNha = thongTinChungBN?.SoNha,
                            XaPhuong = thongTinChungBN?.XaPhuong,
                            Huyen = thongTinChungBN?.Huyen,
                            TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                            NoiLamViec = thongTinChungBN?.NoiLamViec,
                            DoiTuong = thongTinChungBN?.DoiTuong,
                            BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                            BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,
                            NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),
                            SoDienThoai = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                            ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu?.Replace("\n", "<br>"),
                            //ThoiDiemTiepNhan = thongTinChungBN?.ThoiDiemTiepNhan,
                            ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                            LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                            LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                            //ToanThan = yeuCauKhamBenh.KhamToanThan,
                            //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh : yeuCauKhamBenh.KhamToanThan,
                            NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                            KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                            TomTatCLS = yeuCauKhamBenh.TomTatKetQuaCLS,
                            TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                                ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                                + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                                + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                            BenhSu = yeuCauKhamBenh.BenhSu,
                            //DaXuLi = tenYCDuocPhamBV,
                            DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                            HuongXuLy = yeuCauKhamBenh.CachGiaiQuyet,
                            PhongKham = phongKham,
                            HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                            Mach = ketQuaSinhHieu?.NhipTim == null ? null : ketQuaSinhHieu?.NhipTim.ToString(),
                            NhietDo = ketQuaSinhHieu?.ThanNhiet == null ? null : ketQuaSinhHieu?.ThanNhiet.ToString(),
                            HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                            NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                            CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                            ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                            BMI = ketQuaSinhHieu?.Bmi == null ? null : ((double?)Math.Round((ketQuaSinhHieu.Bmi.Value), 2)).ToString(),
                            SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                            TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Tiền sử bệnh bản thân: " + tienSuBenhBanThan : "")
                                    + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Tiền sử bệnh gia đình: " + tienSuBenhGiaDinh : ""),
                            XetNghiemDaLam = string.Join("; ", yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(p => p.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien).Select(z => z.TenDichVu).Distinct()),
                            ChanDoanVaoVien = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),
                            Ngay = thongTinChungBN.Ngay,
                            Thang = thongTinChungBN.Thang,
                            Nam = thongTinChungBN.Nam,

                            //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                            //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                            NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),

                            KhamBenh = khamBenh,

                        };
                        contentYeuCauKhamBenh = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenh.Body, data);
                        if (contentYeuCauKhamBenh != "")
                        {
                            contentYeuCauKhamBenh = contentYeuCauKhamBenh + "<div class=\"pagebreak\"> </div>";
                        };
                        contents.Add(contentYeuCauKhamBenh);
                    }
                    index++;

                }
            }
            else
            {
                foreach (var yeuCauKhamBenh in lstyeuCauKhamBenhs.Where(z => z.CoNhapVien == true))
                {
                    var ketQuaSinhHieu = yeuCauKhamBenh.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(kq => kq.Id).FirstOrDefault();
                    var templatePhieuKhamBenhVaoVien = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenhVaoVien") && x.Version == 2).FirstOrDefault();
                    var thuocDaXuLy = string.Empty;
                    var tienSuBenhBanThan = string.Empty;
                    var tienSuBenhGiaDinh = string.Empty;
                    var phongKham = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == yeuCauKhamBenh.NoiThucHienId).Select(p => p.Ten).FirstOrDefault();
                    var coNoiDungKhac = _yeuCauKhamBenhService.KiemTraDichVuHienTaiCoNhieuNguoiBenhAsync(yeuCauKhamBenh.DichVuKhamBenhBenhVienId).Result;
                    foreach (var item in yeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanTienSuBenhs)
                    {
                        if (item.LoaiTienSuBenh == Enums.EnumLoaiTienSuBenh.BanThan)
                        {
                            if (!string.IsNullOrEmpty(item.TenBenh))
                            {
                                tienSuBenhBanThan += item.TenBenh + "; ";
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.TenBenh))
                            {
                                tienSuBenhGiaDinh += item.TenBenh + "; ";
                            }
                        }
                    }
                    //foreach (var item in yeuCauDuocPhamBenhViens)
                    //{
                    //    thuocDaXuLy += item.DuocPhamBenhVien.DuocPham.Ten + " " + item.DuocPhamBenhVien.DuocPham.HamLuong + " x" + item.SoLuong.ToString() + " " + item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten + " " + item.DuocPhamBenhVien.DuocPham.DuongDung.Ten + "; ";
                    //}
                    var cacBoPhanKhac = string.Empty;
                    var lstCacBoPhanKhacs = yeuCauKhamBenh.YeuCauKhamBenhKhamBoPhanKhacs;
                    foreach (var item in lstCacBoPhanKhacs)
                    {
                        cacBoPhanKhac += "-Bộ phận: " + item.Ten + ", Mô tả: " + item.NoiDUng + "; <br>";
                    }
                    var cacBoPhan = string.Empty;
                    if (!string.IsNullOrEmpty(yeuCauKhamBenh.ThongTinKhamTheoDichVuData) && !string.IsNullOrEmpty(yeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate))
                    {
                        var thongTinKhamTheoDichVuData =
                             JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(yeuCauKhamBenh
                                 .ThongTinKhamTheoDichVuData);
                        var thongTinBenhNhanKhamKhacTemplate =
                            JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(yeuCauKhamBenh
                                .ThongTinKhamTheoDichVuTemplate);
                        foreach (var item in thongTinBenhNhanKhamKhacTemplate.ComponentDynamics)
                        {
                            if (item.Type == 4 && item.groupItems != null && item.groupItems.Count > 0)
                            {
                                var itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == item.Id);
                                if (itemData != null)
                                {
                                    cacBoPhan += item.Label + ": " + itemData?.Value + "<br>";
                                }
                                else
                                {
                                    itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o =>
                                        item.groupItems.Any(g => g.Id == o.Id));
                                    if (itemData != null)
                                    {
                                        cacBoPhan += item.Label + ": ";
                                        var i = 0;
                                        foreach (var itemGroup in item.groupItems)
                                        {
                                            if (itemGroup.Type == 1)
                                            {
                                                cacBoPhan += "<br>" + itemGroup.Label + ": ";
                                            }
                                            else
                                            {
                                                var itemDataGroup = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == itemGroup.Id);
                                                if (itemDataGroup != null)
                                                {
                                                    cacBoPhan += " +" + itemGroup.Label + ": " + itemDataGroup.Value;
                                                }
                                            }
                                            i++;
                                            if (i == item.groupItems.Count)
                                            {
                                                cacBoPhan += "<br>";
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                var itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == item.Id);
                                if (itemData != null)
                                {
                                    cacBoPhan += item.Label + ": " + itemData?.Value + "<br>";
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(cacBoPhanKhac))
                    {
                        cacBoPhan += "<br>" + cacBoPhanKhac;
                    }
                    //var yeuCauDuocPhamBenhViens = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViensHuongThanGayNghien = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z =>
                                                                                                   z.DuongDung.Ma.Trim() != "2.10".Trim()
                                                                                                && z.DuongDung.Ma.Trim() != "1.01".Trim()
                                                                                                && z.DuongDung.Ma.Trim() != "4.04".Trim()
                                                                                                && z.DuongDung.Ma.Trim() != "3.05".Trim()
                                                                                                && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                                   || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViensTiem = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "2.10".Trim()
                                                                                             && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                             && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViensUong = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "1.01".Trim()
                                                                                             && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                             && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViensDat = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "4.04".Trim()
                                                                                             && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                             && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViensDungNgoai = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "3.05".Trim()
                                                                                             && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                             && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();

                    var yeuCauDuocPhamBenhViensKhac = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() != "2.10".Trim()
                                                                                                     && z.DuongDung.Ma.Trim() != "1.01".Trim()
                                                                                                     && z.DuongDung.Ma.Trim() != "4.04".Trim()
                                                                                                     && z.DuongDung.Ma.Trim() != "3.05".Trim()
                                                                                                     && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                     && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                        && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)
                                                                                                     ).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDuocPhamBenhViens = (yeuCauDuocPhamBenhViensHuongThanGayNghien
                                                        .Concat(yeuCauDuocPhamBenhViensTiem)
                                                        .Concat(yeuCauDuocPhamBenhViensUong)
                                                        .Concat(yeuCauDuocPhamBenhViensDat)
                                                        .Concat(yeuCauDuocPhamBenhViensDungNgoai)
                                                        .Concat(yeuCauDuocPhamBenhViensKhac)).Distinct();
                    var yeuCauVatTuBenhViens = yeuCauKhamBenh.YeuCauVatTuBenhViens.Where(z => z.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Select(z => z.Ten).Distinct().ToList();
                    var yeuCauDichVuKyThuats = yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(z => z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                          && !string.IsNullOrEmpty(z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat)
                                                                          && z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat.Substring(0, 1).ToLower() != "p").Select(z => z.TenDichVu).Distinct().ToList();

                    var icdPhus = new List<string>();
                    foreach (var item in yeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
                    {
                        icdPhus.Add(item.ICD.Ma + " - " + item.ICD.TenTiengViet + (!string.IsNullOrEmpty(item.GhiChu) ? " (" + item.GhiChu?.Replace("\n", "<br>") + ")" : ""));
                    }
                    var data = new ThongTinBenhKhac
                    {
                        MaTN = thongTinChungBN.MaTN,
                        MaBN = thongTinChungBN.MaBN,
                        HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                        SinhNgay = thongTinChungBN?.SinhNgay,
                        GioiTinh = thongTinChungBN?.GioiTinh,
                        NgheNghiep = thongTinChungBN?.NgheNghiep,
                        DanToc = thongTinChungBN?.DanToc,
                        QuocTich = thongTinChungBN?.QuocTich,
                        SoNha = thongTinChungBN?.SoNha,
                        XaPhuong = thongTinChungBN?.XaPhuong,
                        Huyen = thongTinChungBN?.Huyen,
                        TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                        NoiLamViec = thongTinChungBN?.NoiLamViec,
                        DoiTuong = thongTinChungBN?.DoiTuong,
                        BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                        BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,
                        NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),
                        SoDienThoai = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                        ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu?.Replace("\n", "<br>"),
                        //ThoiDiemTiepNhan = thongTinChungBN?.ThoiDiemTiepNhan,
                        ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                        LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        //ToanThan = yeuCauKhamBenh?.KhamToanThan,
                        //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") : yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>"),
                        NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                        KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                        TomTatCLS = yeuCauKhamBenh?.TomTatKetQuaCLS?.Replace("\n", "<br>"),
                        QuaTrinhBenhLy = thongTinChungBN?.BenhSu?.Replace("\n", "<br>"),
                        //DaXuLi = thuocDaXuLy,
                        DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                        Mach = ketQuaSinhHieu?.NhipTim == null ? null : ketQuaSinhHieu?.NhipTim.ToString(),
                        NhietDo = ketQuaSinhHieu?.ThanNhiet == null ? null : ketQuaSinhHieu?.ThanNhiet.ToString(),
                        HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                        NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                        CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                        ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                        BMI = ketQuaSinhHieu?.Bmi == null ? null : ((double?)Math.Round((ketQuaSinhHieu.Bmi.Value), 2)).ToString(),
                        SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                        TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Tiền sử bệnh bản thân: " + tienSuBenhBanThan : "")
                                + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Tiền sử bệnh gia đình: " + tienSuBenhGiaDinh : ""),

                        ChanDoan = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),

                        //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                        //Ngay = thongTinChungBN.Ngay,
                        //Thang = thongTinChungBN.Thang,
                        //Nam = thongTinChungBN.Nam,
                        //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                        NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),
                        Ngay = yeuCauKhamBenh.ThoiDiemChiDinh.Day.ToString(),
                        Thang = yeuCauKhamBenh.ThoiDiemChiDinh.Month.ToString(),
                        Nam = yeuCauKhamBenh.ThoiDiemChiDinh.Year.ToString(),
                        //BVHD-3845 The End 

                        HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                        CacBoPhan = cacBoPhan,
                        ChanDoanVaoVien = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),
                        HuongDieuTri = "Nhập viện - " + yeuCauKhamBenh.KhoaPhongNhapVien?.Ten,
                        Khoa = phongKham
                    };
                    var contentYeuCauKhamBenhVaoVien = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenhVaoVien.Body, data);
                    if (contentYeuCauKhamBenhVaoVien != "")
                    {
                        contentYeuCauKhamBenhVaoVien = contentYeuCauKhamBenhVaoVien + "<div class=\"pagebreak\"> </div>";
                    };
                    contents.Add(contentYeuCauKhamBenhVaoVien);
                }
            }

            if (!contents.Any())
            {
                contents.Add("Yêu cầu tiếp nhận này không có DV khám bệnh nào đã hoàn thành");
            }
            return contents;
        }

        private ThongTinTheBenhNhan ThongTinhTheBenhNhan(long yeuCauTiepNhanId)
        {
            var thongTinBN = BaseRepository.TableNoTracking
               .Where(yctn => yctn.Id == yeuCauTiepNhanId)
               .Select(s => new ThongTinTheBenhNhan
               {
                   Id = s.Id,
                   MaBN = s.BenhNhan.MaBN,
                   HoTenBenhNhan = s.HoTen,
                   NgaySinh = s.NgaySinh,
                   ThangSinh = s.ThangSinh,
                   NamSinh = s.NamSinh,
                   GioiTinh = s.GioiTinh,
                   GioiTinhDisplay = s.GioiTinh != null ? s.GioiTinh.GetDescription() : null,
                   SoDienThoai = s.SoDienThoai.ApplyFormatPhone(),
                   NgheNghiep = s.NgheNghiep != null ? s.NgheNghiep.Ten : null,
                   QuocTich = s.QuocTich != null ? s.QuocTich.Ten : null,
                   Tinh = s.TinhThanh != null ? s.TinhThanh.Ten : null,
                   Phuong = s.PhuongXa != null ? s.PhuongXa.Ten : null,
                   Huyen = s.QuanHuyen != null ? s.QuanHuyen.Ten : null,
                   SoNha = s.DiaChi,
                   DiaChiDayDu = s.DiaChiDayDu,
                   SoCMND = s.SoChungMinhThu,
                   Email = s.Email,
                   DanToc = s.DanToc != null ? s.DanToc.Ten : null
               })
               .FirstOrDefault();
            return thongTinBN;
        }
        public string InTheBenhNhan(long yeuCauTiepNhanId, string hostingName)
        {
            var content = string.Empty;
            var thongTinBN = ThongTinhTheBenhNhan(yeuCauTiepNhanId);
            //var barCode = string.Empty;

            var barCode = BarcodeHelper.EncodeStringsToContentBarcode(true, thongTinBN?.HoTenBenhNhan,
                DateHelper.DOBFormat(thongTinBN?.NgaySinh, thongTinBN?.ThangSinh, thongTinBN?.NamSinh),
                thongTinBN?.NamSinh.ToString(),
                thongTinBN?.GioiTinh.ToString(),
                thongTinBN?.SoDienThoai,
                thongTinBN?.NgheNghiep,
                thongTinBN?.QuocTich,
                thongTinBN?.Tinh,
                thongTinBN?.Phuong,
                thongTinBN?.SoNha,
                thongTinBN?.SoCMND,
                thongTinBN?.Email,
                thongTinBN?.DanToc);

            if (thongTinBN != null)
            {
                barCode = thongTinBN.MaBN + "|" + barCode;
            }
            var templateTheBenhNhan = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("TheBenhNhan")).FirstOrDefault();
            if (templateTheBenhNhan != null)
            {
                var data = new TheBenhNhan
                {
                    HostingName = hostingName,
                    BarCodeImgBase64 = BarcodeHelper.GenerateQrCode(barCode, Color.FromArgb(38, 42, 54), Color.White),
                    MaBN = thongTinBN?.MaBN,
                    HoTenBenhNhan = thongTinBN?.HoTenBenhNhan.ToUpper()
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateTheBenhNhan.Body, data);
            }
            return content;
        }
        public string InVongDeoTay(long yeuCauTiepNhanId, string hostingName)
        {
            var content = string.Empty;
            var thongTinBN = ThongTinhTheBenhNhan(yeuCauTiepNhanId);
            //var user = _userRepository.GetById(_userAgentHelper.GetCurrentUserId(),x=>x.Include(o=>o.NhanVien).ThenInclude(o=>o.HoatDongNhanViens).ThenInclude(o=>o.PhongBenhVien).ThenInclude(o=>o.KhoaPhong));
            var kho = "CHƯA CÓ";//user?.NhanVien?.HoatDongNhanViens?.OrderByDescending(o => o.Id).FirstOrDefault()?.PhongBenhVien?.KhoaPhong?.Ten;
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId, x => x.Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong));
            if (!string.IsNullOrEmpty(yeuCauTiepNhan?.YeuCauKhamBenhs?.OrderByDescending(o => o.ThoiDiemChiDinh)
                .FirstOrDefault()?.NoiChiDinh?.KhoaPhong?.Ten?.ToUpper()))
            {
                kho = yeuCauTiepNhan?.YeuCauKhamBenhs?.OrderByDescending(o => o.ThoiDiemChiDinh)
                    .FirstOrDefault()?.NoiChiDinh?.KhoaPhong?.Ten?.ToUpper();
            }
            var templateTheBenhNhan = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("VongDeoTay")).FirstOrDefault();
            if (templateTheBenhNhan != null)
            {
                var data = new VongDeoTay
                {
                    HostingName = hostingName,
                    BarCodeByMaBN = BarcodeHelper.GenerateBarCode(thongTinBN?.MaBN, 210, 56),
                    MaBN = thongTinBN?.MaBN,
                    HoTenBenhNhan = thongTinBN?.HoTenBenhNhan.ToUpper(),
                    NgaySinh = thongTinBN?.NgaySinh,
                    ThangSinh = thongTinBN?.ThangSinh,
                    NamSinh = thongTinBN?.NamSinh,
                    GioiTinh = thongTinBN?.GioiTinhDisplay,
                    SoDienThoai = thongTinBN?.SoDienThoai,
                    Tuoi = (thongTinBN?.NamSinh == null ? 0 : (DateTime.Now.Year - thongTinBN?.NamSinh.Value)),
                    Khoa = !string.IsNullOrEmpty(kho) ? kho.ToUpper() : "CHƯA CÓ"
                };
                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateTheBenhNhan.Body, data);
            }
            return content;
        }


        public long GetThongTinYeuCauTiepNhan(TimKiemThongTin timKiemThongTin)
        {
            var query = BaseRepository.TableNoTracking.Where(cc => cc.MaYeuCauTiepNhan.Contains(timKiemThongTin.TimKiemMaBNVaMaTN) || cc.BenhNhan.MaBN.ToString().Contains(timKiemThongTin.TimKiemMaBNVaMaTN)).FirstOrDefault();
            if (query != null)
            {
                return query.Id;
            }
            return 0;
        }

        public string XemGiayNghiHuongBHYTLien1(ThongTinNgayNghiHuongBHYTTiepNhan thongTinNgayNghi)
        {
            var thongTinChungBN = ThongTinBenhNhanHienTai(thongTinNgayNghi.YeuCauTiepNhanId);
            var contentLienSo1 = string.Empty;
            var templateGiayNghiHuongBHYTLienSo1 = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("GiayNghiHuongBHYT")).First();

            var ycHienTai = BaseRepository.GetById(thongTinNgayNghi.YeuCauTiepNhanId,
                                   x => x.Include(yctn => yctn.NgheNghiep)
                                         .Include(yctn => yctn.NguoiLienHeQuanHeNhanThan)
                                         .Include(yctn => yctn.YeuCauKhamBenhs));
            var hoTenBacSi = string.Empty;
            if (thongTinNgayNghi.BacSiKetLuanId != null)
            {
                hoTenBacSi = _userRepository.TableNoTracking.Where(u => u.Id == thongTinNgayNghi.BacSiKetLuanId).First().HoTen;
            }

            var lstYeuCauKhamBenh = ycHienTai.YeuCauKhamBenhs.Where(yckb => yckb.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham).ToList();
            var chanDoan = string.Empty;

            var hoTenCha = string.Empty;
            var hoTenMe = string.Empty;
            var quanHeThanNhanCha = ycHienTai.NguoiLienHeQuanHeNhanThan?.TenVietTat;
            if (!string.IsNullOrEmpty(quanHeThanNhanCha) && quanHeThanNhanCha.Contains("ChaDe"))
            {
                hoTenCha = ycHienTai.NguoiLienHeHoTen;
            }

            if (!string.IsNullOrEmpty(quanHeThanNhanCha) && quanHeThanNhanCha.Contains("MeDe"))
            {
                hoTenMe = ycHienTai.NguoiLienHeHoTen;
            }

            foreach (var item in lstYeuCauKhamBenh)
            {
                //todo: Cần bổ sung phương pháp điều trị (Thạch)
                if (!string.IsNullOrEmpty(item.GhiChuICDChinh))
                {
                    chanDoan += "<p style='margin: 0;'> CĐ: " + item.GhiChuICDChinh + "; PPĐT: " + "" + "</p>";
                }
            }
            var thoiDiemTiepNhanFormat = string.Empty;
            var denNgayFormat = string.Empty;
            var soNgayNghi = string.Empty;

            if (thongTinNgayNghi.ThoiDiemTiepNhan != null && thongTinNgayNghi.DenNgay != null)
            {
                thoiDiemTiepNhanFormat = thongTinNgayNghi.ThoiDiemTiepNhan?.ApplyFormatDate();
                denNgayFormat = thongTinNgayNghi.DenNgay.Value.ApplyFormatDate();
                //int result = DateTime.Compare(Convert.ToDateTime(thoiDiemTiepNhanFormat), Convert.ToDateTime(denNgayFormat));
                int result = DateTime.Compare(DateTime.ParseExact(thoiDiemTiepNhanFormat, "dd/MM/yyyy", null), DateTime.ParseExact(denNgayFormat, "dd/MM/yyyy", null));
                if (result == 0)
                {
                    soNgayNghi = "1";
                }
                else if (result == -1)
                {
                    TimeSpan value = DateTime.ParseExact(denNgayFormat, "dd/MM/yyyy", null).Subtract(DateTime.ParseExact(thoiDiemTiepNhanFormat, "dd/MM/yyyy", null));
                    var getday = value.Days + 1;
                    soNgayNghi = getday.ToString();
                }
                else
                {
                    denNgayFormat = thoiDiemTiepNhanFormat;
                    soNgayNghi = "1";
                }
            }
            //var header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                   "<th>Liên số 1</th>" +
            //              "</p>";
            var dataLienSo1 = new ThongTinNgayNghiHuongBHYTDetail
            {
                //Header = header,
                //MaTN = thongTinChungBN.MaTN,
                //LienSo = "1",
                HoTenBenhNhan = thongTinChungBN.HoTenBenhNhan,
                SinhNgay = thongTinChungBN.SinhNgay,
                BHYTMaSoThe = thongTinChungBN.BHYTMaSoThe2,
                GioiTinh = thongTinChungBN.GioiTinh,
                NoiLamViec = thongTinChungBN.NoiLamViec,
                ChanDoanPhuongPhap = chanDoan,
                SoNgayNghi = soNgayNghi + " NGÀY",
                ThoiDiemTiepNhan = thoiDiemTiepNhanFormat,
                DenNgay = denNgayFormat,
                HoTenCha = hoTenCha,
                //todo: confirm HoTenMe
                HoTenMe = hoTenMe,
                //HoTenBacSi = "Bs." + thongTinChungBN.HoTenBacSi,
                //Ngay = thongTinChungBN.Ngay,
                //Thang = thongTinChungBN.Thang,
                //Nam = thongTinChungBN.Nam,
                NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
            };
            contentLienSo1 = TemplateHelpper.FormatTemplateWithContentTemplate(templateGiayNghiHuongBHYTLienSo1.Body, dataLienSo1);
            //if (contentLienSo1 != "")
            //{
            //    contentLienSo1 = contentLienSo1 + "<div class=\"pagebreak\"> </div>";
            //}
            return contentLienSo1;
        }

        public string XemGiayNghiHuongBHYTLien2(ThongTinNgayNghiHuongBHYTTiepNhan thongTinNgayNghi)
        {
            var thongTinChungBN = ThongTinBenhNhanHienTai(thongTinNgayNghi.YeuCauTiepNhanId);
            var contentLienSo2 = string.Empty;
            var templateGiayNghiHuongBHYTLienSo2 = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("GiayNghiHuongBHYT")).First();

            var ycHienTai = BaseRepository.GetById(thongTinNgayNghi.YeuCauTiepNhanId,
                                   x => x.Include(yctn => yctn.NgheNghiep)
                                         .Include(yctn => yctn.NguoiLienHeQuanHeNhanThan)
                                         .Include(yctn => yctn.YeuCauKhamBenhs));
            var hoTenBacSi = string.Empty;
            if (thongTinNgayNghi.BacSiKetLuanId != null)
            {
                hoTenBacSi = _userRepository.TableNoTracking.Where(u => u.Id == thongTinNgayNghi.BacSiKetLuanId).First().HoTen;
            }


            var hoTenCha = string.Empty;
            var hoTenMe = string.Empty;
            var quanHeThanNhanCha = ycHienTai.NguoiLienHeQuanHeNhanThan?.TenVietTat;
            if (!string.IsNullOrEmpty(quanHeThanNhanCha) && quanHeThanNhanCha.Contains("ChaDe"))
            {
                hoTenCha = ycHienTai.NguoiLienHeHoTen;
            }

            if (!string.IsNullOrEmpty(quanHeThanNhanCha) && quanHeThanNhanCha.Contains("MeDe"))
            {
                hoTenMe = ycHienTai.NguoiLienHeHoTen;
            }
            var lstYeuCauKhamBenh = ycHienTai.YeuCauKhamBenhs.Where(yckb => yckb.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham).ToList();
            var chanDoan = string.Empty;
            foreach (var item in lstYeuCauKhamBenh)
            {
                //todo: Cần bổ sung phương pháp điều trị (Thạch)
                if (!string.IsNullOrEmpty(item.GhiChuICDChinh))
                {
                    chanDoan += "<p style='margin: 0;'> CĐ: " + item.GhiChuICDChinh + "; PPĐT: " + "" + "</p>";
                }
            }
            var thoiDiemTiepNhanFormat = string.Empty;
            var denNgayFormat = string.Empty;
            var soNgayNghi = string.Empty;

            if (thongTinNgayNghi.ThoiDiemTiepNhan != null && thongTinNgayNghi.DenNgay != null)
            {
                thoiDiemTiepNhanFormat = thongTinNgayNghi.ThoiDiemTiepNhan?.ApplyFormatDate();
                denNgayFormat = thongTinNgayNghi.DenNgay.Value.ApplyFormatDate();
                //int result = DateTime.Compare(Convert.ToDateTime(thoiDiemTiepNhanFormat), Convert.ToDateTime(denNgayFormat));
                int result = DateTime.Compare(DateTime.ParseExact(thoiDiemTiepNhanFormat, "dd/MM/yyyy", null), DateTime.ParseExact(denNgayFormat, "dd/MM/yyyy", null));

                if (result == 0)
                {
                    soNgayNghi = "1";
                }
                else if (result == -1)
                {
                    TimeSpan value = DateTime.ParseExact(denNgayFormat, "dd/MM/yyyy", null).Subtract(DateTime.ParseExact(thoiDiemTiepNhanFormat, "dd/MM/yyyy", null));
                    var getday = value.Days + 1;
                    soNgayNghi = getday.ToString();
                }
                else
                {
                    denNgayFormat = thoiDiemTiepNhanFormat;
                    soNgayNghi = "1";
                }
            }

            var header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                             "<th>Liên số 2</th>" +
                        "</p>";
            var dataLienSo2 = new ThongTinNgayNghiHuongBHYTDetail
            {
                Header = header,
                MaTN = thongTinChungBN.MaTN,
                LienSo = "2",
                HoTenBenhNhan = thongTinChungBN.HoTenBenhNhan,
                SinhNgay = thongTinChungBN.SinhNgay,
                BHYTMaSoThe = thongTinChungBN.BHYTMaSoThe,
                GioiTinh = thongTinChungBN.GioiTinh,
                NoiLamViec = thongTinChungBN.NoiLamViec,
                ChanDoanPhuongPhap = chanDoan,
                SoNgayNghi = soNgayNghi,
                ThoiDiemTiepNhan = thoiDiemTiepNhanFormat,
                DenNgay = denNgayFormat,
                HoTenCha = hoTenCha,
                //todo: confirm HoTenMe
                HoTenMe = hoTenMe,
                HoTenBacSi = !string.IsNullOrEmpty(hoTenBacSi) ? "Bs." + hoTenBacSi : null,
                Ngay = thongTinChungBN.Ngay,
                Thang = thongTinChungBN.Thang,
                Nam = thongTinChungBN.Nam,
            };
            contentLienSo2 = TemplateHelpper.FormatTemplateWithContentTemplate(templateGiayNghiHuongBHYTLienSo2.Body, dataLienSo2);
            return contentLienSo2;
        }

        public async Task<List<LookupItemVo>> GetBacSiKhamBenhs(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                "HoTen",
                "SoDienThoai"
            };
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                var result = _yeuCauKhamBenhRepository.TableNoTracking
               .Where(yckb => yckb.YeuCauTiepNhanId == long.Parse(queryInfo.ParameterDependencies) && yckb.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && yckb.BacSiKetLuanId != null)
               .OrderByDescending(yckb => yckb.ThoiDiemHoanThanh)
               .Select(item => new LookupItemVo
               {
                   DisplayName = item.BacSiKetLuan.User.HoTen + " - (" + item.BacSiKetLuan.User.SoDienThoai.ApplyFormatPhone() + ")",
                   KeyId = (long)item.BacSiKetLuanId,
               }).Distinct().ApplyLike(queryInfo.Query, o => o.DisplayName).Take(queryInfo.Take).ToListAsync();
                await Task.WhenAll(result);
                return result.Result;
            }
            else
            {
                var userLst = _userRepository
                                .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                                .Select(p => p.Id).ToList();

                var dictionary = userLst.Select((id, index) => new
                {
                    key = id,
                    rank = index,
                }).ToDictionary(o => o.key, o => o.rank);

                var result = await _yeuCauKhamBenhRepository
                                    .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                                    .Where(yckb => yckb.YeuCauTiepNhanId == long.Parse(queryInfo.ParameterDependencies)
                                                    && yckb.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && yckb.BacSiKetLuanId != null)
                                    .OrderByDescending(yckb => yckb.ThoiDiemHoanThanh)
                                    .Take(queryInfo.Take)
                                    .Select(item => new LookupItemVo
                                    {
                                        DisplayName = item.BacSiKetLuan.User.HoTen + " - (" + item.BacSiKetLuan.User.SoDienThoai.ApplyFormatPhone() + ")",
                                        KeyId = (long)item.BacSiKetLuanId,
                                    }).ToListAsync();
                return result;
            }
        }

        #region KiemTraNgayNghiHuongBHXH
        public async Task<bool> KiemTraNgayTiepNhan(DateTime? tuNgay, DateTime? denNgay, long yeuCauTiepNhanId)
        {
            var thoiDiemTiepNhan = await BaseRepository.TableNoTracking
                                    .Where(p => p.Id == yeuCauTiepNhanId)
                                    .Select(p => p.ThoiDiemTiepNhan)
                                    .FirstOrDefaultAsync();
            var myTime = DateTime.Today;
            if (tuNgay != null && denNgay != null)
            {
                if (thoiDiemTiepNhan.Date > tuNgay.Value.Date && tuNgay.Value.Date <= myTime)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> KiemTraDenNgay(DateTime? tuNgay, DateTime? denNgay, long yeuCauTiepNhanId)
        {
            var thoiDiemTiepNhan = await BaseRepository.TableNoTracking
                                    .Where(p => p.Id == yeuCauTiepNhanId)
                                    .Select(p => p.ThoiDiemTiepNhan)
                                    .FirstOrDefaultAsync();
            var myTime = DateTime.Today;
            if (tuNgay != null && denNgay != null)
            {
                if (tuNgay.Value.Date > denNgay.Value.Date)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
