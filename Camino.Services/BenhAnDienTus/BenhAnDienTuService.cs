using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhAnDienTus;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhAnDienTus;
using Camino.Core.Domain.ValueObject.DichVuChiDinhNoiTruNgoaiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Camino.Services.NoiTruBenhAn;
using Camino.Services.TaiLieuDinhKem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Services.BenhAnDienTus
{
    [ScopedDependency(ServiceType = typeof(IBenhAnDienTuService))]
    public partial class BenhAnDienTuService : MasterFileService<GayBenhAn>, IBenhAnDienTuService
    {
        private readonly IRepository<GayBenhAn> _gayBenhAnRepository;
        private readonly IRepository<GayBenhAnPhieuHoSo> _gayBenhAnPhieuHoSoRepository;

        private readonly IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> _noiTruBenhAnRepository;
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepository;
        private readonly IRepository<NoiTruKhoaPhongDieuTri> _noiTruKhoaPhongDieuTriRepository;

        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;

        private readonly IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> _yCTNRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yCKBRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> _yCDVKTRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _pHongBVRepository;
        private IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> _hocViHocHamRepository;
        private IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private IRepository<Core.Domain.Entities.ICDs.ICD> _icdRepository;
        private readonly IUserAgentHelper _userAgentHelper;

        private readonly IRepository<NoiTruHoSoKhac> _noiTruHoSoKhacRepository;
        public BenhAnDienTuService(IRepository<GayBenhAn> repository
        , IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> noiTruBenhAnRepository
        , IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository
        , IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository
        , IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository
        , IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository
        , IRepository<Template> templateRepository
        , IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepository
        , IRepository<NoiTruKhoaPhongDieuTri> noiTruKhoaPhongDieuTriRepository,
            IRepository<GayBenhAnPhieuHoSo> gayBenhAnPhieuHoSoRepository,

        IRepository<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan> yCTNRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> yCDVKTRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> pHongBVRepository,
            IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> hocViHocHamRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yCKBRepository,
            IRepository<Core.Domain.Entities.ICDs.ICD> icdRepository,
            IUserAgentHelper userAgentHelper

        , ITaiLieuDinhKemService taiLieuDinhKemService,
             IRepository<NoiTruHoSoKhac> noiTruHoSoKhacRepository
            ) : base(repository)
        {
            _noiTruBenhAnRepository = noiTruBenhAnRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _yCTNRepository = yCTNRepository;
            _yCDVKTRepository = yCDVKTRepository;
            _pHongBVRepository = pHongBVRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _templateRepository = templateRepository;
            _noiTruPhieuDieuTriRepository = noiTruPhieuDieuTriRepository;
            _noiTruKhoaPhongDieuTriRepository = noiTruKhoaPhongDieuTriRepository;
            _hocViHocHamRepository = hocViHocHamRepository;
            _nhanVienRepository = nhanVienRepository;
            _yCKBRepository = yCKBRepository;
            _icdRepository = icdRepository;
            _userAgentHelper = userAgentHelper;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _gayBenhAnPhieuHoSoRepository = gayBenhAnPhieuHoSoRepository;
            _noiTruHoSoKhacRepository = noiTruHoSoKhacRepository;
        }

        #region grid
        public async Task<GridDataSource> GetDataForGridTimKiemNoiTruBenhAnAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new BenhAnDienTuTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BenhAnDienTuTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgayNhapVien = null;
            DateTime? denNgayNhapVien = null;
            DateTime? tuNgayXuatVien = null;
            DateTime? denNgayXuatVien = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgayNhapVien != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayNhapVien.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayNhapVien.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgayNhapVien = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgayNhapVien != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayNhapVien.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayNhapVien.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgayNhapVien = denNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgayXuatVien != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayXuatVien.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayXuatVien.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgayXuatVien = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgayXuatVien != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayXuatVien.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayXuatVien.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgayXuatVien = denNgayTemp;
            }

            var query =
                _noiTruBenhAnRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.SoBenhAn, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.YeuCauTiepNhan.HoTen)
                    .Where(x =>
                        // thời điểm nhập viện
                        (tuNgayNhapVien == null || x.ThoiDiemNhapVien.Date >= tuNgayNhapVien.Value.Date)
                        && (denNgayNhapVien == null || x.ThoiDiemNhapVien.Date <= denNgayNhapVien.Value.Date)

                        // thời điểm xuất viện
                        && (tuNgayXuatVien == null || (x.ThoiDiemRaVien != null && x.ThoiDiemRaVien.Value.Date >= tuNgayXuatVien.Value.Date))
                        && (denNgayXuatVien == null || (x.ThoiDiemRaVien != null && x.ThoiDiemRaVien.Value.Date <= denNgayXuatVien.Value.Date))
                    )
                    .Select(item => new BenhAnDienTuNoiTruBenhAnGridVo()
                    {
                        Id = item.Id,
                        SoBenhAn = item.SoBenhAn,
                        MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBN = item.YeuCauTiepNhan.BenhNhan.MaBN,
                        BHYTMaSoThe = item.YeuCauTiepNhan.BHYTMaSoThe,
                        HoTen = item.YeuCauTiepNhan.HoTen,
                        NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                        ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,
                        GioiTinh = item.YeuCauTiepNhan.GioiTinh,
                        SoChungMinhThu = item.YeuCauTiepNhan.SoChungMinhThu,
                        DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                        SoDienThoai = item.YeuCauTiepNhan.SoDienThoai,
                        SoDienThoaiDisplay = item.YeuCauTiepNhan.SoDienThoaiDisplay
                    })
                .OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            var result = query.ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridTimKiemNoiTruBenhAnAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BenhAnDienTuTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BenhAnDienTuTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgayNhapVien = null;
            DateTime? denNgayNhapVien = null;
            DateTime? tuNgayXuatVien = null;
            DateTime? denNgayXuatVien = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgayNhapVien != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayNhapVien.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayNhapVien.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgayNhapVien = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgayNhapVien != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayNhapVien.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayNhapVien.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgayNhapVien = denNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgayXuatVien != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayXuatVien.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayXuatVien.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgayXuatVien = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgayXuatVien != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayXuatVien.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayXuatVien.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgayXuatVien = denNgayTemp;
            }

            var query =
                _noiTruBenhAnRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.SoBenhAn, x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.YeuCauTiepNhan.HoTen)
                    .Where(x =>
                        // thời điểm nhập viện
                        (tuNgayNhapVien == null || x.ThoiDiemNhapVien.Date >= tuNgayNhapVien.Value.Date)
                        && (denNgayNhapVien == null || x.ThoiDiemNhapVien.Date <= denNgayNhapVien.Value.Date)

                        // thời điểm xuất viện
                        && (tuNgayXuatVien == null || (x.ThoiDiemRaVien != null && x.ThoiDiemRaVien.Value.Date >= tuNgayXuatVien.Value.Date))
                        && (denNgayXuatVien == null || (x.ThoiDiemRaVien != null && x.ThoiDiemRaVien.Value.Date <= denNgayXuatVien.Value.Date))
                    )
                    .Select(item => new BenhAnDienTuNoiTruBenhAnGridVo()
                    {
                        Id = item.Id
                    });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<GridDataSource> GetDataForGridAsyncDanhSachGayBenhAn(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = BaseRepository.TableNoTracking
                .Select(s => new GayBenhAnVo
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    ViTri = s.ViTriGay,
                    IsDisabled = s.IsDisabled,
                    //TenPhieuHoSo = string.Join(";", s.GayBenhAnPhieuHoSos.Select(g => g.LoaiPhieuHoSoBenhAnDienTu.GetDescription()))
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.Ma,
                    g => g.Ten
                    );

            }
            var queryResult = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            if (queryResult.Any())
            {
                var gayBenhAnIds = queryResult.Select(c => c.Id).ToList();
                var benhAnChiTiets = _gayBenhAnPhieuHoSoRepository.TableNoTracking.Where(x => gayBenhAnIds.Contains(x.GayBenhAnId))
                                    .Select(s => new GayBenhAnChiTietVo()
                                    {
                                        Id = s.Id,
                                        GayBenhAnId = s.GayBenhAnId,
                                        LoaiPhieuHoSoBenhAnDienTu = s.LoaiPhieuHoSoBenhAnDienTu,
                                        Value = s.Value,
                                        TenValue = s.Value == null ? s.LoaiPhieuHoSoBenhAnDienTu.GetDescription() : ""
                                    }).ToList();
                var result = new List<GayBenhAnChiTietVo>();
                var nhomDichVuCLSIds = new List<long?>();
                if (benhAnChiTiets.Any(c => c.LoaiPhieuHoSoBenhAnDienTu == LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS))
                {
                    nhomDichVuCLSIds = benhAnChiTiets.Where(c => c.LoaiPhieuHoSoBenhAnDienTu == LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS).Select(ba => ba.Value).ToList();
                    var nhomDichVuCLSs = _nhomDichVuBenhVienRepository.TableNoTracking.Where(n => nhomDichVuCLSIds.Contains(n.Id)).Select(s => new { s.Id, s.Ten }).ToList();
                    var hosoKhams = benhAnChiTiets.Where(c => c.LoaiPhieuHoSoBenhAnDienTu == LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS).Select(s => new GayBenhAnChiTietVo
                    {
                        Id = s.Id,
                        GayBenhAnId = s.GayBenhAnId,
                        TenValue = nhomDichVuCLSs.Where(n => n.Id == s.Value).Select(c => c.Ten).FirstOrDefault()
                    }).ToList();
                    result.AddRange(hosoKhams);
                }
                if (benhAnChiTiets.Any(c => c.LoaiPhieuHoSoBenhAnDienTu == LoaiPhieuHoSoBenhAnDienTu.HoSoKhac))
                {
                    var hoSoKhacs = Enum.GetValues(typeof(LoaiHoSoDieuTriNoiTru)).Cast<Enum>().ToList();
                    var hosoKhams = benhAnChiTiets.Where(c => c.LoaiPhieuHoSoBenhAnDienTu == LoaiPhieuHoSoBenhAnDienTu.HoSoKhac).Select(s => new GayBenhAnChiTietVo
                    {
                        Id = s.Id,
                        GayBenhAnId = s.GayBenhAnId,
                        TenValue = hoSoKhacs.FirstOrDefault(c => c.Equals((LoaiHoSoDieuTriNoiTru)s.Value)).GetDescription()
                    }).ToList();
                    result.AddRange(hosoKhams);
                }
                result.AddRange(benhAnChiTiets.Where(c => c.LoaiPhieuHoSoBenhAnDienTu != LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS && c.LoaiPhieuHoSoBenhAnDienTu != LoaiPhieuHoSoBenhAnDienTu.HoSoKhac));
                foreach (var item in queryResult)
                {
                    item.TenPhieuHoSo = string.Join("; ", result.Where(c => c.GayBenhAnId == item.Id).Select(ct => ct.TenValue));
                }
            }

            return new GridDataSource { Data = queryResult, TotalRowCount = queryResult.Count() };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachGayBenhAn(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new GayBenhAnVo
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    ViTri = s.ViTriGay,
                    IsDisabled = s.IsDisabled,
                    //TenPhieuHoSo = string.Join(";", s.GayBenhAnPhieuHoSos.Select(g => g.LoaiPhieuHoSoBenhAnDienTu.GetDescription()))
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                    g => g.Ma,
                    g => g.Ten
                    );

            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region get data
        public async Task<BenhAnDienTuDetailVo> GetGayBenhAnDienTuTheoBenhAnAsnc(long noiTruBenhAnId)
        {

            var lstGayBenhAn = BaseRepository.TableNoTracking
                .Include(x => x.GayBenhAnPhieuHoSos)
                .Where(x => x.IsDisabled != true)
                .OrderBy(x => x.ViTriGay)
                .ToList();

            BenhAnDienTuDetailVo benhAnDienTu = null;
            if (lstGayBenhAn.Any())
            {
                var lstHoSo = new List<BenhAnDienTuMenuInfoVo>();
                var thongTinLanTiepNhan = _yeuCauTiepNhanRepository.GetById(noiTruBenhAnId,
                    x => x.Include(a => a.NoiTruHoSoKhacs)
                        .Include(a => a.NoiTruBenhAn).ThenInclude(a => a.NoiTruPhieuDieuTriChiTietYLenhs)
                        .Include(a => a.NoiTruBenhAn).ThenInclude(a => a.NoiTruPhieuDieuTris));

                var kiemTraTaiLieuDieuTriLanTruoc = _yeuCauTiepNhanRepository.TableNoTracking
                    .Any(x => x.MaYeuCauTiepNhan.Equals(thongTinLanTiepNhan.MaYeuCauTiepNhan)
                              && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                              && x.HoSoYeuCauTiepNhans.Any());

                var phieuKhamChuyenKhoas = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhan.MaYeuCauTiepNhan.Equals(thongTinLanTiepNhan.MaYeuCauTiepNhan)
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    .Select(item => new BenhAnDienTuThongTinDichVuVo()
                    {
                        Id = item.Id,
                        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                        NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                        CoNhapVien = item.YeuCauNhapViens.Any()
                    })
                    .ToList();

                var dichVuCLSs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhan.MaYeuCauTiepNhan.Equals(thongTinLanTiepNhan.MaYeuCauTiepNhan)
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .Select(item => new BenhAnDienTuThongTinDichVuVo()
                    {
                        Id = item.Id,
                        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                        NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                        LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                        DaThucHien = item.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,
                        NhomBenhVienId = item.NhomDichVuBenhVienId
                    })
                    .ToList();
                var nhomDichVuBenhViens = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();

                foreach (var gayBenhAn in lstGayBenhAn)
                {
                    var gayHienTai = new BenhAnDienTuMenuInfoVo();
                    var gayCoData = false;

                    foreach (var chiTietHoSo in gayBenhAn.GayBenhAnPhieuHoSos)
                    {
                        var hoSoCoData = false;
                        switch (chiTietHoSo.LoaiPhieuHoSoBenhAnDienTu)
                        {
                            case Enums.LoaiPhieuHoSoBenhAnDienTu.BiaBenhAn:
                                hoSoCoData = true;
                                break;
                            case Enums.LoaiPhieuHoSoBenhAnDienTu.HanhChinhBenhAn:
                                hoSoCoData = true;
                                break;
                            case Enums.LoaiPhieuHoSoBenhAnDienTu.TaiLieuDieuTriLanTruoc:
                                hoSoCoData = kiemTraTaiLieuDieuTriLanTruoc;
                                break;
                            case Enums.LoaiPhieuHoSoBenhAnDienTu.PhieuKhamBenhVaoVien:
                                hoSoCoData = phieuKhamChuyenKhoas.Any(a => a.CoNhapVien);
                                break;
                            case Enums.LoaiPhieuHoSoBenhAnDienTu.PhieuChiDinh:
                                hoSoCoData = dichVuCLSs.Any();
                                break;
                            case Enums.LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS:
                                var loaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(chiTietHoSo.Value.GetValueOrDefault(), nhomDichVuBenhViens);
                                hoSoCoData = dichVuCLSs.Any(x => (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                                    || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                                    || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                                                 && x.LoaiDichVuKyThuat == loaiDichVuKyThuat
                                                                 && x.DaThucHien
                                                                 && x.NhomBenhVienId == chiTietHoSo.Value.GetValueOrDefault());
                                break;
                            case Enums.LoaiPhieuHoSoBenhAnDienTu.PhieuKhamTheoChuyenKhoa:
                                hoSoCoData = phieuKhamChuyenKhoas.Any();
                                break;
                            case Enums.LoaiPhieuHoSoBenhAnDienTu.HoSoKhac:
                                hoSoCoData = chiTietHoSo.Value != null
                                             && ((chiTietHoSo.Value.Value == (long)Enums.LoaiHoSoDieuTriNoiTru.PhieuChamSoc
                                                  && thongTinLanTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs
                                                      .Any(d => d.YeuCauDichVuKyThuatId == null
                                                                  && d.YeuCauTruyenMauId == null
                                                                  && d.YeuCauVatTuBenhVienId == null
                                                                  && d.NoiTruChiDinhDuocPhamId == null))
                                                 || (chiTietHoSo.Value.Value != (long)Enums.LoaiHoSoDieuTriNoiTru.PhieuChamSoc 
                                                     && thongTinLanTiepNhan.NoiTruHoSoKhacs.Any(x => x.LoaiHoSoDieuTriNoiTru == (Enums.LoaiHoSoDieuTriNoiTru)chiTietHoSo.Value.Value)));
                                break;

                            case Enums.LoaiPhieuHoSoBenhAnDienTu.ToDieuTri:
                                hoSoCoData = thongTinLanTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Any();
                                break;
                            default: hoSoCoData = false; break;
                        }

                        if (hoSoCoData)
                        {
                            gayCoData = true;
                            gayHienTai.ViTri = gayBenhAn.ViTriGay;
                            gayHienTai.TenGayHoSo = gayBenhAn.Ten;
                            gayHienTai.ChiTietHoSos.Add(new BenhAnDienTuChiTietMenuInfo()
                            {
                                LoaiHoSo = chiTietHoSo.LoaiPhieuHoSoBenhAnDienTu,
                                Value = chiTietHoSo.Value
                            });
                        }
                    }

                    if (gayCoData)
                    {
                        lstHoSo.Add(gayHienTai);
                    }
                }

                if (lstHoSo.Any())
                {
                    benhAnDienTu = new BenhAnDienTuDetailVo()
                    {
                        NoiTruBenhAnId = noiTruBenhAnId,
                        MaYeuCauTiepNhan = thongTinLanTiepNhan.MaYeuCauTiepNhan,
                        BenhNhanId = thongTinLanTiepNhan.BenhNhanId.GetValueOrDefault(),
                        GayBenhAns = lstHoSo
                    };
                }
            }
            return benhAnDienTu;
        }

        public async Task<YeuCauTiepNhan> GetLanTiepNhanTheoBenhAnAsnc(long noiTruBenhAnId)
        {
            var yeuCauTiepNhan = new YeuCauTiepNhan();
            yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(noiTruBenhAnId,
                a => a.Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTriChiTietYLenhs)
                    .Include(x => x.NoiTruHoSoKhacs)
                    .Include(x => x.NoiTruDonThuocs)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhienXetNghiemChiTiets)
                    .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhienXetNghiemChiTiets)
                    .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs)
                    //.Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs)
                    //.Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonVTYTs)
                    .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan).ThenInclude(x => x.HoSoYeuCauTiepNhans)
                    .Include(u => u.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)
                    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris).ThenInclude(p => p.BacSi).ThenInclude(p => p.User)
                    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruEkipDieuTris).ThenInclude(p => p.BacSi).ThenInclude(p => p.HocHamHocVi)
                    .Include(u => u.NoiTruBenhAn).ThenInclude(p => p.NoiTruKhoaPhongDieuTris).ThenInclude(p => p.KhoaPhongChuyenDen));
            return yeuCauTiepNhan;
        }
        #endregion

        #region In HTML
        public async Task<string> InBiaBenhAnDienTuAsync(BiaBenhAnDienTuInVo detail)
        {
            var content = string.Empty;
            var header = string.Empty;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("BiaBenhAnDienTu"));

            var thongTinBenhAn = _noiTruBenhAnRepository.TableNoTracking
                .Where(x => x.Id == detail.NoiTruBenhAnId)
                .Select(item => new ThongTinBiaBenhAnVo
                {
                    Header = header,
                    LogoUrl = detail.Hosting + "/assets/img/logo-bacha-full.png",
                    SoLuuTru = item.SoLuuTru,
                    MaVaoVien = item.SoBenhAn,
                    HoVaTen = item.YeuCauTiepNhan.HoTen,
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    GioiTinh = item.YeuCauTiepNhan.GioiTinh,
                    DiaChi = item.YeuCauTiepNhan.DiaChiDayDu,
                    NgheNghiep = item.YeuCauTiepNhan.NgheNghiep.Ten,

                    // cần check lại
                    //ChanDoan = item.ChanDoanChinhRaVienICD.TenTiengViet,
                    //MaChanDoan = item.ChanDoanChinhRaVienICD.Ma, 
                    LaBenhAnSanKhoa = item.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo || item.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong,

                    CoBHYT = 
                        //BVHD-3754
                        //item.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any(), //(item.YeuCauTiepNhan.CoBHYT != null && item.YeuCauTiepNhan.CoBHYT == true) || 
                        item.YeuCauTiepNhan.CoBHYT != null && item.YeuCauTiepNhan.CoBHYT == true,

                    KhoaNhapVien = item.YeuCauTiepNhan.YeuCauNhapVien.KhoaPhongNhapVien.Ten,
                    KhoaNhapVienId = item.YeuCauTiepNhan.YeuCauNhapVien.KhoaPhongNhapVienId,
                    NgayNhapVienDayDu = item.ThoiDiemNhapVien,
                    ThongTinKhoaChuyen = "",
                    KhoaXuatVien = "",
                    NgayXuatVienDayDu = item.ThoiDiemRaVien,
                    //-Năm BA: theo năm NB ra viện(hiện tại Đang mặc định năm 2021)
                    NamNBRaVien = item.ThoiDiemRaVien != null ? item.ThoiDiemRaVien.GetValueOrDefault().Year +"": "",
                    HinhThucRaVien = item.HinhThucRaVien
                }).First();

            //cập nhật bìa bệnh án điện tử ngày 05/12/2022
            //Bìa BA
            // -Năm BA: theo năm NB ra viện(hiện tại Đang mặc định năm 2021)
            //- Chẩn đoán: link theo chẩn đoán của giấy ra viện
            //-ICD(theo ICD và ICD phụ) MaChanDoan
            // - Đối với NB chuyển tuyến: thời gian ra viện phải hiển thị tại cột Chuyển viện - Khoa(hệ thống đang hiển thị nhầm sang cột Xuất viện)
            // Chưa thống nhất hiển thị Chẩn đoán, ICD và cột Chuyển viện - Khoa trên bìa BA


            // get chẩn đoán của giấy ra viện
            // giấy ra viện chỉ có 1 phiếu duy nhất => first
            var getInfoGiayRaVien = _noiTruHoSoKhacRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == detail.NoiTruBenhAnId && x.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.GiayRaVien)
                .Select(d => d.ThongTinHoSo).FirstOrDefault();
            // tạm thời cập nhật chẩn đoán theo giấy ra viện =>> chẩn đoán dài quá hơn 95 ký tự để 3 chấm
            if (!string.IsNullOrEmpty(getInfoGiayRaVien))
            {
                var data = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(getInfoGiayRaVien);
                if (data != null && !string.IsNullOrEmpty(data.ChanDoan))
                {
                    List<string> list = new List<string>();
                    int index = 0;
                    var defaultLength = 92;
                    while (index < data.ChanDoan.Length)
                    {
                        if (index + defaultLength < data.ChanDoan.Length)
                        {
                            var text92kiTu = data.ChanDoan.Substring(index, 92);

                            var resultkiTuCuoi = text92kiTu[91].ToString();

                            var lengthHopLe = LengthTest(resultkiTuCuoi, text92kiTu);

                            defaultLength = lengthHopLe;

                            list.Add(data.ChanDoan.Substring(index, lengthHopLe));
                        }
                        else
                        {
                            list.Add(data.ChanDoan.Substring(index));
                        }


                        index += defaultLength;
                    }
                    if (list.Any())
                    {
                        thongTinBenhAn.ChanDoan = list[0].Length > 92 ? list[0] + "..." : list[0];
                    }

                    // hiện tại format mã - tên

                    if (!string.IsNullOrEmpty(data.ChanDoan))
                    {
                       
                        var arrs = data.ChanDoan.Split("-");

                        if(arrs != null && arrs.ToList().Count() > 0)
                        {
                            var listMas = GetMaChanDoan(arrs.ToList());
                            if(listMas.Any())
                            {
                                thongTinBenhAn.MaChanDoan = listMas.Select(d=>d.DisplayName).Join(", ").Length > 6 ? listMas.Select(d => d.DisplayName).Join(", ")[6] + "..." : listMas.Select(d => d.DisplayName).Join(", ");
                            }
                            
                        }
                    }
                }
            }




            //if (thongTinBenhAn.LaBenhAnSanKhoa)
            //{
            //    var phieuDieuTriCuoiCung = _noiTruPhieuDieuTriRepository.TableNoTracking
            //        .Include(x => x.ChanDoanChinhICD)
            //        .Where(x => x.NoiTruBenhAnId == detail.NoiTruBenhAnId
            //                    && x.ChanDoanChinhICDId != null)
            //        .OrderByDescending(x => x.NgayDieuTri)
            //        .ThenByDescending(x => x.Id)
            //        .FirstOrDefault();

            //    if (phieuDieuTriCuoiCung != null)
            //    {
            //        thongTinBenhAn.ChanDoan = phieuDieuTriCuoiCung.ChanDoanChinhICD.TenTiengViet;
            //        thongTinBenhAn.MaChanDoan = phieuDieuTriCuoiCung.ChanDoanChinhICD.Ma;
            //    }
            //}

            #region Xử lý khoa phòng điều trị
            // nếu hình thức ra viện ==  chuyển viện col khoa ngày xuất viện null
            if(thongTinBenhAn.HinhThucRaVien != null && thongTinBenhAn.HinhThucRaVien == EnumHinhThucRaVien.ChuyenVien)
            {
                var lstKhoaPhongDieuTri = _noiTruKhoaPhongDieuTriRepository.TableNoTracking
                                        .Include(x => x.KhoaPhongChuyenDen)
                                        .Where(x => x.NoiTruBenhAnId == detail.NoiTruBenhAnId)
                                        .OrderBy(x => x.Id)
                                        .ToList();

                var lstKhoaPhongChuyenDenId = new List<long>();
                var htmlKhoaChuyen = string.Empty;
                if (lstKhoaPhongDieuTri.Any())
                {
                    htmlKhoaChuyen += GanThongTinKhoaPhongChuyenDen(new Core.Domain.Entities.KhoaPhongs.KhoaPhong(), thongTinBenhAn.NgayXuatVienDayDu, lstKhoaPhongChuyenDenId);
                }
                thongTinBenhAn.NgayXuatVienDayDu = null;

                thongTinBenhAn.ThongTinKhoaChuyen = htmlKhoaChuyen;
            }
            else
            {
                var lstKhoaPhongDieuTri = _noiTruKhoaPhongDieuTriRepository.TableNoTracking
                                         .Include(x => x.KhoaPhongChuyenDen)
                                         .Where(x => x.NoiTruBenhAnId == detail.NoiTruBenhAnId)
                                         .OrderBy(x => x.Id)
                                         .ToList();

                var lstKhoaPhongChuyenDenId = new List<long>();
                var htmlKhoaChuyen = string.Empty;
                if (lstKhoaPhongDieuTri.Any() && lstKhoaPhongDieuTri.Count > 1)
                {
                    var lstKhoaDieuTriChuyen = lstKhoaPhongDieuTri
                        .Where(x => x.KhoaPhongChuyenDenId != thongTinBenhAn.KhoaNhapVienId).ToList();
                    foreach (var khoaDieuTri in lstKhoaDieuTriChuyen)
                    {
                        htmlKhoaChuyen += GanThongTinKhoaPhongChuyenDen(khoaDieuTri.KhoaPhongChuyenDen, khoaDieuTri.ThoiDiemVaoKhoa, lstKhoaPhongChuyenDenId);
                    }
                }
                else
                {
                    htmlKhoaChuyen += GanThongTinKhoaPhongChuyenDen(new Core.Domain.Entities.KhoaPhongs.KhoaPhong(), null, lstKhoaPhongChuyenDenId);
                }
                thongTinBenhAn.ThongTinKhoaChuyen = htmlKhoaChuyen;
            }
         

            


            #endregion

            if (!thongTinBenhAn.CoBHYT)
            {
                template.Body = template.Body.Replace("<b class=\"tagBHYT\"", "<b class=\"hidden\"");
            }

            content += TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, thongTinBenhAn) + "<div class='pagebreak'></div>";
            return content;
        }

        private string GanThongTinKhoaPhongChuyenDen(Core.Domain.Entities.KhoaPhongs.KhoaPhong khoaPhong, DateTime? ngayChuyen, List<long> khoaPhongDaChuyenIds)
        {
            var html = string.Empty;

            if ((khoaPhong.Id != 0 && khoaPhongDaChuyenIds.All(x => x != khoaPhong.Id)) || khoaPhong.Id == 0)
            {
                if (khoaPhong.Id != 0)
                {
                    khoaPhongDaChuyenIds.Add(khoaPhong.Id);
                }

                var thongTinKhoaChuyen = new ThongTinChuyenKhoaVo()
                {
                    KhoaChuyen = khoaPhong.Ten ?? string.Empty,
                    NgayChuyenKhoaDayDu = ngayChuyen
                };

                html = "<div class=\"container\" style=\"width: 100%; margin-top: 5px;\">"
                         + "<div class=\"label\">"
                         + "<b>Khoa</b>(Dept):"
                         + "</div>"
                         + "<div class=\"value\">" + thongTinKhoaChuyen.KhoaChuyen
                         + "</div>"
                         + "</div>"
                         + "<div class=\"container\" style=\"width:100%;\">"
                         + "<div class=\"label\">"
                         + "<b>Ngày</b>(Date):"
                         + "</div>"
                         + "<div class=\"value\" style=\"width:30%;\">" + (thongTinKhoaChuyen.NgayChuyenKhoa ?? string.Empty) + "</div>"
                         + "<div class=\"value\" style=\"width:30%;\">/" + (thongTinKhoaChuyen.ThangChuyenKhoa ?? string.Empty) + "</div>"
                         + "<div class=\"value\">/20" + (thongTinKhoaChuyen.NamChuyenKhoa ?? string.Empty) + "</div>"
                         + "</div>";
            }

            return html;
        }

        public async Task<string> InTaiLieuDieuTriTruocBenhAnDienTuAsync(YeuCauTiepNhan yeuCauTiepNhan)
        {
            var html = "<table width=\"100%\">"
                + "<tr>"
                + "<td align = \"center\">"
                + "<h2>Tài liệu đợt điều trị trước/ truyến trước</h2>"
                + "</td>"
                + "</tr>";
            foreach (var hoSo in yeuCauTiepNhan.HoSoYeuCauTiepNhans)
            {
                var src = _taiLieuDinhKemService.GetTaiLieuUrl(hoSo.DuongDan, hoSo.TenGuid);
                html += "<tr>"
                        + "<td><a target=\"_blank\" href=\"" + src + "\">" + hoSo.Ten + "</a></td>"
                        + "</tr>";
            }
            html += "</table>"
                + "<div class='pagebreak'></div>";
            return html;
        }
        #endregion

        #region In dịch vụ kỹ thuật theo mã tiếp nhận
        public string InDichVuChiDinh(string hosting, long yeuCauTiepNhanId,bool header)
        {
            var content = "";
            // 1 yêu cầu tiếp nhận chỉ có 1 mã tn (nội trú và ngoại trú giống MaTN)
            var maYeuCauTiepNhan = _yCTNRepository.TableNoTracking
                .Where(d => d.Id == yeuCauTiepNhanId)
                .Select(d => d.MaYeuCauTiepNhan).First();

            var infoYeuCauDichVuKyThuats = _yCTNRepository.TableNoTracking
                .Where(d => d.MaYeuCauTiepNhan == maYeuCauTiepNhan)
                .SelectMany(d => d.YeuCauDichVuKyThuats).ToList();

            var listTheoNguoiChiDinh = new List<DichVuChiDinhTheoNguoiChiDinhGridVo>();

            var infoYeuCauDichVuKyThuatsKhacHuy = infoYeuCauDichVuKyThuats.Where(d => d.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();
            var noiTruPhieuDieuTris = _noiTruPhieuDieuTriRepository.TableNoTracking.Where(d => d.NoiTruBenhAnId == yeuCauTiepNhanId)
                .Select(d => new { NgayDieuTri = d.NgayDieuTri, Id = d.Id }).ToList();
            foreach (var ycdvkt in infoYeuCauDichVuKyThuatsKhacHuy)
            {
                var objNguoiChidinh = new DichVuChiDinhTheoNguoiChiDinhGridVo();
                objNguoiChidinh.DichVuChiDinhId = ycdvkt.Id; // 
                objNguoiChidinh.NhomChiDinhId = (int)EnumNhomGoiDichVu.DichVuKyThuat;
                objNguoiChidinh.NhanVienChiDinhId = ycdvkt.NhanVienChiDinhId;
                //objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ycdvkt.ThoiDiemChiDinh.Year, ycdvkt.ThoiDiemChiDinh.Month, ycdvkt.ThoiDiemChiDinh.Day, 0, 0, 0);
                if (ycdvkt.NoiTruPhieuDieuTriId != null)
                {
                    var ngayDieuTri = noiTruPhieuDieuTris.Where(d => d.Id == ycdvkt.NoiTruPhieuDieuTriId).Select(d => d.NgayDieuTri).FirstOrDefault();
                    if (ngayDieuTri != null)
                    {
                        objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ngayDieuTri.Year, ngayDieuTri.Month, ngayDieuTri.Day, 0, 0, 0);
                    }
                }
                else
                {
                    objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ycdvkt.ThoiDiemDangKy.Year, ycdvkt.ThoiDiemDangKy.Month, ycdvkt.ThoiDiemDangKy.Day, 0, 0, 0);
                }

                listTheoNguoiChiDinh.Add(objNguoiChidinh);
            }

            /// in theo nhóm dịch vụ và Người chỉ định
            var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d => d.Key.ThoiDiemChiDinh).ToList();
            foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
            {
                var listCanIn = new List<DichVuChiDinhTheoNguoiChiDinhGridVo>();
                listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                content = AddChiDinhKhamBenhTheoNguoiChiDinhVaNhoms(hosting, yeuCauTiepNhanId, content, listCanIn,header);
            }

            return content;
        }
        #region in dịch vụ  chỉ định ngoài gói khám đoàn 
        private string AddChiDinhKhamBenhTheoNguoiChiDinhVaNhoms(string hosting, long yeuCauTiepNhanId, string content, List<DichVuChiDinhTheoNguoiChiDinhGridVo> listCanIn, bool header)
        {
            var maTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.Id == yeuCauTiepNhanId).Select(d => d.MaYeuCauTiepNhan).First();

            var dsYeuCauDichVuKyThuatIds = listCanIn.Select(d => d.DichVuChiDinhId).ToList();
            var dichVuKyThuatInfos = _yCDVKTRepository.TableNoTracking
                .Where(d => dsYeuCauDichVuKyThuatIds.Contains(d.Id) && d.DichVuKyThuatBenhVien != null).ToList();

            decimal tongtien = 0;
            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var infoNoiYeuCau = _pHongBVRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId)
                         .Select(m => new
                         {
                             Ma = m.Ma,
                             Ten = m.Ten
                         }).First();

            string tenNguoiChiDinh = "";
            if(header == true)
            {
                content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            }
            

            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            string ngay = "";
            string thang = "";
            string nam = "";

            var isHave = false;

            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "</tr>";
            var i = 1;
            int indexDVKT = 1;
            var nhomDichVus = _nhomDichVuBenhVienRepository.TableNoTracking.Select(d => new { Ten = d.Ten, Id = d.Id }).ToList();

            #region dịch vụ kỹ thuật
            List<ListDichVuChiDinhCLSPTTT> lstDichVuChidinh = new List<ListDichVuChiDinhCLSPTTT>();
            foreach (var ycdvkt in dichVuKyThuatInfos)
            {
                var lstDichVuKT = new ListDichVuChiDinhCLSPTTT();
                lstDichVuKT.TenNhom = nhomDichVus.Where(x => x.Id == ycdvkt.NhomDichVuBenhVienId).Select(d => d.Ten).FirstOrDefault();
                lstDichVuKT.NhomChiDinhId = ycdvkt.NhomDichVuBenhVienId;
                lstDichVuKT.DichVuChiDinhId = ycdvkt.Id;
                lstDichVuChidinh.Add(lstDichVuKT);
            }

            var lstdvkt = dichVuKyThuatInfos;
            var maHocHamViInfos = _nhanVienRepository.TableNoTracking
                .Select(d => new
                {
                    NhanVienId = d.Id,
                    MaHocHamHocVi = d.HocHamHocVi.Ma,
                    HoTen = d.User.HoTen
                })
                .ToList();
            var chanDoanSoBoGhiChuYCKBs = _yCKBRepository.TableNoTracking.Where(d => d.YeuCauTiepNhan.MaYeuCauTiepNhan == maTiepNhan)
                .Select(d => new
                {
                    YeuCauKhamBenhId = d.Id,
                    ChanDoanSoBoGhiChu = d.ChanDoanSoBoGhiChu
                }).ToList();

            var chanDoanSoBoGhiChuNoiTruPhieuDieuTris = _noiTruPhieuDieuTriRepository.TableNoTracking.Where(d => d.NoiTruBenhAn.YeuCauTiepNhan.MaYeuCauTiepNhan == maTiepNhan)
                .Select(d => new
                {
                    NoiTruPhieuDieuTriId = d.Id,
                    ChanDoanSoBoGhiChu = d.ChanDoanChinhGhiChu
                }).ToList();
            var chanDoanICDs = _icdRepository.TableNoTracking
                .Select(d => new
                {
                    Id = d.Id,
                    Ten = d.TenTiengViet,
                    Ma = d.Ma
                }).ToList();


            var noiTruPhieuDieuTris = _noiTruPhieuDieuTriRepository.TableNoTracking.Where(d => d.NoiTruBenhAn.YeuCauTiepNhan.MaYeuCauTiepNhan == maTiepNhan)
                .Select(d => new { ChanDoanICDId = d.ChanDoanChinhICDId, Id = d.Id }).ToList();
            var yeuCauKhamBenhs = _yCKBRepository.TableNoTracking.Where(d => d.YeuCauTiepNhan.MaYeuCauTiepNhan == maTiepNhan)
                .Select(d => new { ChanDoanICDId = d.ChanDoanSoBoICDId, Id = d.Id }).ToList();

            var dichVuInfos = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(d => dsYeuCauDichVuKyThuatIds.Contains(d.Id))
                .Select(d => new
                {
                    Id = d.DichVuKyThuatBenhVienId,
                    Ten = d.DichVuKyThuatBenhVien.Ten,
                    NoiThucHienId = d.NoiThucHienId,
                    TenNoiThucHien = d.NoiThucHien.Ten
                }).ToList();

            var chanDoanSoBos = new List<string>();
            var dienGiaiChanDoanSoBo = new List<string>();
            foreach (var dv in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
            {

                if (dv.Count() > 1)
                {
                    foreach (var ycdvktIn in dv)
                    {
                        if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Any())
                        {
                            #region diễn giải ngoại trú
                            var yckbId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.YeuCauKhamBenhId).FirstOrDefault();
                            var cdsbNgoaiTru = chanDoanSoBoGhiChuYCKBs.Where(d => d.YeuCauKhamBenhId == yckbId).Select(d => d.ChanDoanSoBoGhiChu).FirstOrDefault();
                            dienGiaiChanDoanSoBo.Add(cdsbNgoaiTru);
                            #endregion

                            #region diễn giải nội trú
                            var notruPhieuDieuTriId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTriId).FirstOrDefault();
                            var cdsbNoiTru = chanDoanSoBoGhiChuNoiTruPhieuDieuTris.Where(d => d.NoiTruPhieuDieuTriId == notruPhieuDieuTriId).Select(d => d.ChanDoanSoBoGhiChu).FirstOrDefault();
                            dienGiaiChanDoanSoBo.Add(cdsbNoiTru);
                            #endregion

                            #region chẩn đoán sơ bộ ghi chú nội trú
                            var noiTruPhieuDieuTriId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId && p.NoiTruPhieuDieuTriId != null)
                                                    .Select(d => d.NoiTruPhieuDieuTriId).FirstOrDefault();

                            var noiTruChanDoanChinhICDId = noiTruPhieuDieuTris.Where(d => d.Id == noiTruPhieuDieuTriId).Select(d => d.ChanDoanICDId).FirstOrDefault(); ;
                            if (noiTruChanDoanChinhICDId != null)
                            {
                                chanDoanSoBos.Add(chanDoanICDs.Where(d => d.Id == noiTruChanDoanChinhICDId).First().Ma + "-" + chanDoanICDs.Where(d => d.Id == noiTruChanDoanChinhICDId).First().Ten);
                            }
                            #endregion
                            #region chẩn đoán sơ bộ ghi chú ngoại trú
                            var yeuCauKhamBenhId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId && p.YeuCauKhamBenhId != null)
                                                   .Select(d => d.YeuCauKhamBenhId).FirstOrDefault();
                            var ngoaiTruChanDoanChinhICDId = yeuCauKhamBenhs.Where(d => d.Id == yeuCauKhamBenhId).Select(d => d.ChanDoanICDId).FirstOrDefault();
                            if (ngoaiTruChanDoanChinhICDId != null)
                            {
                                chanDoanSoBos.Add(chanDoanICDs.Where(d => d.Id == ngoaiTruChanDoanChinhICDId).First().Ma + "-" + chanDoanICDs.Where(d => d.Id == ngoaiTruChanDoanChinhICDId).First().Ten);
                            }
                            #endregion


                            ngay = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                            thang = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                            nam = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                            if (indexDVKT == 1)
                            {
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                            }

                            var maHocHamVi = string.Empty;
                            var nhanVienChiDinhId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinhId).FirstOrDefault();
                            #endregion

                            if (nhanVienChiDinhId != null)
                            {
                                maHocHamVi = maHocHamViInfos.Where(d => d.NhanVienId == nhanVienChiDinhId).Select(d => d.MaHocHamHocVi).FirstOrDefault();
                                tenNguoiChiDinh = returnStringTen(maHocHamVi, "", maHocHamViInfos.Where(d => d.NhanVienId == nhanVienChiDinhId).Select(d => d.HoTen).FirstOrDefault());
                            }

                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + dichVuInfos.Where(d => d.Id == lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().DichVuKyThuatBenhVienId).Select(d => d.Ten).FirstOrDefault() + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHienId != null ? dichVuInfos.Where(d => d.NoiThucHienId == lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHienId).First().TenNoiThucHien : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan + "</td>";
                            htmlDanhSachDichVu += " </tr>";

                            i++;
                            indexDVKT++;
                            ycdvktIn.TenNhom = "";
                        }
                    }
                    indexDVKT = 1;
                }
                if (dv.Count() == 1)
                {
                    foreach (var ycdvktIn in dv)
                    {
                        if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First() != null)
                        {
                            #region diễn giải ngoại trú
                            var yckbId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.YeuCauKhamBenhId).FirstOrDefault();
                            var cdsbNgoaiTru = chanDoanSoBoGhiChuYCKBs.Where(d => d.YeuCauKhamBenhId == yckbId).Select(d => d.ChanDoanSoBoGhiChu).FirstOrDefault();
                            dienGiaiChanDoanSoBo.Add(cdsbNgoaiTru);
                            #endregion

                            #region diễn giải nội trú
                            var notruPhieuDieuTriId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.NoiTruPhieuDieuTriId).FirstOrDefault();
                            var cdsbNoiTru = chanDoanSoBoGhiChuNoiTruPhieuDieuTris.Where(d => d.NoiTruPhieuDieuTriId == notruPhieuDieuTriId).Select(d => d.ChanDoanSoBoGhiChu).FirstOrDefault();
                            dienGiaiChanDoanSoBo.Add(cdsbNoiTru);
                            #endregion

                            #region chẩn đoán sơ bộ ghi chú nội trú
                            var noiTruPhieuDieuTriId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId && p.NoiTruPhieuDieuTriId != null)
                                                    .Select(d => d.NoiTruPhieuDieuTriId).FirstOrDefault();

                            var noiTruChanDoanChinhICDId = noiTruPhieuDieuTris.Where(d => d.Id == noiTruPhieuDieuTriId).Select(d => d.ChanDoanICDId).FirstOrDefault(); ;
                            if (noiTruChanDoanChinhICDId != null)
                            {
                                chanDoanSoBos.Add(chanDoanICDs.Where(d => d.Id == noiTruChanDoanChinhICDId).First().Ma + "-" + chanDoanICDs.Where(d => d.Id == noiTruChanDoanChinhICDId).First().Ten);
                            }
                            #endregion
                            #region chẩn đoán sơ bộ ghi chú ngoại trú
                            var yeuCauKhamBenhId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId && p.YeuCauKhamBenhId != null)
                                                   .Select(d => d.YeuCauKhamBenhId).FirstOrDefault();
                            var ngoaiTruChanDoanChinhICDId = yeuCauKhamBenhs.Where(d => d.Id == yeuCauKhamBenhId).Select(d => d.ChanDoanICDId).FirstOrDefault();
                            if (ngoaiTruChanDoanChinhICDId != null)
                            {
                                chanDoanSoBos.Add(chanDoanICDs.Where(d => d.Id == ngoaiTruChanDoanChinhICDId).First().Ma + "-" + chanDoanICDs.Where(d => d.Id == ngoaiTruChanDoanChinhICDId).First().Ten);
                            }
                            #endregion


                            var maHocHamVi = string.Empty;
                            var nhanVienChiDinhId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinhId).FirstOrDefault();

                            if (nhanVienChiDinhId != null)
                            {
                                maHocHamVi = maHocHamViInfos.Where(d => d.NhanVienId == nhanVienChiDinhId).Select(d => d.MaHocHamHocVi).FirstOrDefault();
                                tenNguoiChiDinh = returnStringTen(maHocHamVi, "", maHocHamViInfos.Where(d => d.NhanVienId == nhanVienChiDinhId).Select(d => d.HoTen).FirstOrDefault());
                            }

                            ngay = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                            thang = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                            nam = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                            htmlDanhSachDichVu += " </tr>";
                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + dichVuInfos.Where(d => d.Id == lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().DichVuKyThuatBenhVienId).First().Ten + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHienId != null ? dichVuInfos.Where(d => d.NoiThucHienId == lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHienId).First().TenNoiThucHien : "") + "</td>";
                            htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan + "</td>";
                            htmlDanhSachDichVu += " </tr>";
                            i++;
                            indexDVKT++;
                            ycdvktIn.TenNhom = "";
                        }
                    }
                    indexDVKT = 1;
                }
            }
            #endregion
            // bệnh án điện tử trong nội trú mặc định lấy thông tin nội trú
            var inFoYeuCauTiepNhan = _yCTNRepository.TableNoTracking.Where(d => d.MaYeuCauTiepNhan == maTiepNhan && d.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                .Select(d => new
                {
                    MaYeuCauTiepNhan = d.MaYeuCauTiepNhan,
                    MaBN = d.BenhNhan.MaBN,
                    GioiTinh = d.GioiTinh,
                    HoTen = d.HoTen,
                    NamSinh = d.NamSinh,
                    DiaChiDayDu = d.DiaChi,
                    SoDienThoai = d.SoDienThoai,
                    DoiTuong = d.CoBHYT != true ? "Viện phí" : "BHYT " + (d.BHYTMucHuong != null ? "(" + d.BHYTMucHuong + "%)" :""),
                    SoTheBHYT = d.BHYTMaSoThe,
                    HanThe = d.BHYTNgayHieuLuc != null && d.BHYTNgayHetHan != null ? ("từ ngày: " + d.BHYTNgayHieuLuc.GetValueOrDefault().ToString("dd/MM/yyyy") + 
                                                                                     " đến ngày: " + d.BHYTNgayHetHan.GetValueOrDefault().ToString("dd/MM/yyyy")) : "",
                    NguoiLienHeHoTen = d.NguoiLienHeHoTen,
                    NguoiLienHeQuanHeNhanThan = d.NguoiLienHeQuanHeNhanThan.Ten
                }).First();



            var data = new
            {
                LogoUrl = hosting + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(inFoYeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(inFoYeuCauTiepNhan?.MaYeuCauTiepNhan) : "",
                MaTN = inFoYeuCauTiepNhan?.MaYeuCauTiepNhan,
                MaBN = inFoYeuCauTiepNhan?.MaBN,
                HoTen = inFoYeuCauTiepNhan?.HoTen,
                GioiTinhString = inFoYeuCauTiepNhan?.GioiTinh.GetDescription(),
                NamSinh = inFoYeuCauTiepNhan?.NamSinh ?? null,
                DiaChi = inFoYeuCauTiepNhan?.DiaChiDayDu,
                Ngay = ngay,
                Thang = thang,
                Nam = nam,
                DienThoai = inFoYeuCauTiepNhan?.SoDienThoai,
                DoiTuong = inFoYeuCauTiepNhan?.DoiTuong,

                SoTheBHYT = inFoYeuCauTiepNhan?.SoTheBHYT,
                HanThe = inFoYeuCauTiepNhan?.HanThe,

                NoiYeuCau = infoNoiYeuCau?.Ten,
                ChuanDoanSoBo = chanDoanSoBos.Where(d => d != null).Distinct().ToList().Join("; "),
                DienGiai = dienGiaiChanDoanSoBo.Where(d => d != null).Distinct().ToList().Join("; "),
                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = tenNguoiChiDinh,
                NguoiGiamHo = inFoYeuCauTiepNhan?.NguoiLienHeHoTen,
                TenQuanHeThanNhan = inFoYeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan,
                PhieuThu = "DichVuKyThuat",
                TongCong = tongtien.ApplyFormatMoneyVND()
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
            htmlDanhSachDichVu += "</tr>";
            i = 1;
            return content;

        }
        #endregion
        #region format tên người chỉ định 
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
        private string MaHocHamHocVi(long id)
        {
            var maHocHamVi = string.Empty;
            maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == id).Select(d => d.Ma).FirstOrDefault();
            return maHocHamVi;
        }
        #endregion

        public async Task<List<PhieuHoSoGayBenhAnLookupVo>> GetPhieuHoSoGayBenhAnLookupVos(DropDownListRequestModel queryInfo)
        {
            var phieuHoSoGayBenhAnLookupVos = new List<PhieuHoSoGayBenhAnLookupVo>();
            var loaiPhieuHoSoBenhAnDienTus = Enum.GetValues(typeof(LoaiPhieuHoSoBenhAnDienTu)).Cast<Enum>()
                                           .Where(s => !s.Equals(LoaiPhieuHoSoBenhAnDienTu.NhomDichVuCLS) && !s.Equals(LoaiPhieuHoSoBenhAnDienTu.HoSoKhac))
                                          .Select(item => new PhieuHoSoGayBenhAnLookupVo
                                          {
                                              KeyId = JsonConvert.SerializeObject(new KeyIdStringPhieuHoSoGayBenhAnLookupVo()
                                              {
                                                  PhieuHoSoId = Convert.ToInt32(item),
                                                  LoaiPhieuHoSoBenhAn = PhieuHoSoBenhAn.PhieuHoSoBenhAnDienTu,
                                              }),
                                              DisplayName = item.GetDescription()
                                          });
            var nhomDichVuCLSs = _nhomDichVuBenhVienRepository.TableNoTracking
                                  .Where(c => c.NhomDichVuBenhVienChaId == (long)LoaiDichVuKyThuat.XetNghiem
                                  || c.NhomDichVuBenhVienChaId == (long)LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                  || c.NhomDichVuBenhVienChaId == (long)LoaiDichVuKyThuat.ThamDoChucNang)
                                  .Select(s => new PhieuHoSoGayBenhAnLookupVo
                                  {
                                      KeyId = JsonConvert.SerializeObject(new KeyIdStringPhieuHoSoGayBenhAnLookupVo()
                                      {
                                          PhieuHoSoId = s.Id,
                                          LoaiPhieuHoSoBenhAn = PhieuHoSoBenhAn.NhomCLS,
                                      }),
                                      DisplayName = s.Ten
                                  });
            var hoSoKhacs = Enum.GetValues(typeof(LoaiHoSoDieuTriNoiTru)).Cast<Enum>()
                             .Select(item => new PhieuHoSoGayBenhAnLookupVo
                             {
                                 KeyId = JsonConvert.SerializeObject(new KeyIdStringPhieuHoSoGayBenhAnLookupVo()
                                 {
                                     PhieuHoSoId = Convert.ToInt32(item),
                                     LoaiPhieuHoSoBenhAn = PhieuHoSoBenhAn.HoSoKhac,
                                 }),
                                 DisplayName = item.GetDescription()
                             });
            phieuHoSoGayBenhAnLookupVos.AddRange(loaiPhieuHoSoBenhAnDienTus.Concat(nhomDichVuCLSs).Concat(hoSoKhacs));
            var result = phieuHoSoGayBenhAnLookupVos.ToList();
            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                result = result.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return result;
        }

        public async Task<bool> IsMaTenExists(string maHoacTen = null, long Id = 0, bool flag = false)
        {
            var result = false;
            if (Id == 0)
            {
                result = !flag ? await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maHoacTen))
                    : await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(maHoacTen));
            }
            else
            {
                result = !flag ? await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma.Equals(maHoacTen) && p.Id != Id) :
                     await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(maHoacTen) && p.Id != Id);
            }
            if (result)
                return false;
            return true;
        }
        private int LengthTest(string kiTuCuoiCung, string text92kiTu)
        {
            int count = 0;
            int defaultKiTu = 92;
            // cắt từ đầu đến kí tự khoảng cách  ,không phải khoảng cách lùi lại 1 chữ 
            if (kiTuCuoiCung == "")
            {
                count = 92;
            }
            else
            {
                for (int i = text92kiTu.Length - 1; i < defaultKiTu; i--)
                {
                    if (text92kiTu[i].ToString() == " ")
                    {
                        count = i;
                        break;
                    }
                }
            }
            return count;
        }
        public List<LookupItemVo> GetMaChanDoan(List<string> listMas)
        {
            var listICDs = _icdRepository.TableNoTracking
                .Where(d=> listMas.Contains(d.Ma)).Select(d => new LookupItemVo { 
                KeyId = d.Id,
                DisplayName = d.Ma
            }).ToList();

            return listICDs;
        }
    }
}
