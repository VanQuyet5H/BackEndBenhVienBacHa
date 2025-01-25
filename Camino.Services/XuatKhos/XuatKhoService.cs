using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauKhamBenh;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.XuatKhos
{
    [ScopedDependency(ServiceType = typeof(IXuatKhoService))]
    public class XuatKhoService : MasterFileService<XuatKhoDuocPham>, IXuatKhoService
    {
        private readonly IRepository<XuatKhoDuocPhamChiTiet> _xuatKhoDuocPhamChiTietRepository;

        private readonly IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;

        private readonly IRepository<Kho> _khoDuocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<DuocPham> _duocPhamRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhom;

        private readonly IRepository<KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;

        private readonly IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Template> _templateRepository;
        IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhomRepository;
        private new readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly ILocalizationService _localizationService;


        public XuatKhoService(IRepository<XuatKhoDuocPham> repository, IRepository<XuatKhoDuocPhamChiTiet> xuatKhoDuocPhamChiTietRepository
            , IRepository<Kho> khoDuocPhamRepository
            , IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository
            , IRepository<DuocPham> duocPhamRepository
            , IRepository<Template> templateRepository
            , IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository
            , IRepository<NhapKhoDuocPham> nhapKhoDuocPhamRepository
            , IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhom
            , IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository
            , ICauHinhService cauHinhService, IUserAgentHelper userAgentHelper
            , ILocalizationService localizationService
            , IRepository<KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
                IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhomRepository
            , IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IYeuCauKhamBenhService yeuCauKhamBenhService) : base(repository)
        {
            _xuatKhoDuocPhamChiTietRepository = xuatKhoDuocPhamChiTietRepository;
            _khoDuocPhamRepository = khoDuocPhamRepository;
            _nhanVienRepository = nhanVienRepository;
            _duocPhamRepository = duocPhamRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _duocPhamBenhVienPhanNhom = duocPhamBenhVienPhanNhom;
            _duocPhamBenhVienPhanNhomRepository = duocPhamBenhVienPhanNhomRepository;

            _templateRepository = templateRepository;
            _cauHinhService = cauHinhService;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
            _localizationService = localizationService;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _userAgentHelper = userAgentHelper;
            _phongBenhVienRepository = phongBenhVienRepository;
        }

        public async Task<List<DuocPhamXuatGridVo>> GetDuocPhamOnGroup(long groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon)
        {
            var allDataNhapDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT
                }).GroupBy(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT,
                }, o => o,
                (k, v) => new DuocPhamXuatGridVo
                {
                    DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                    SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    DonGia = k.DonGiaTonKho,
                    SoLo = k.Solo,
                    HanSuDung = k.HanSuDung,
                    LaDuocPhamBHYT = k.LaDuocPhamBHYT
                }).Where(o => o.SoLuongTon >= 0.01).ToList();

            var duocPhamBenhVienIds = allDataNhapDuocPham.Select(o => o.DuocPhamBenhVienId).Distinct().ToList();
            var thongTinDuocPhams = _duocPhamBenhVienRepository.TableNoTracking
                .Where(o => duocPhamBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.DuocPham.Ten,
                    DuocPhamBenhVienPhanNhomId = (o.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhVienPhanNhomId : 0),
                    TenNhom = (o.DuocPhamBenhVienPhanNhom != null ? o.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM"),
                    DVT = o.DuocPham.DonViTinh.Ten,
                    SoDangKy = o.DuocPham.SoDangKy,
                    HamLuong = o.DuocPham.HamLuong
                }).ToList();

            var dataReturn = new List<DuocPhamXuatGridVo>();
            foreach (var duocPhamXuatGridVo in allDataNhapDuocPham)
            {
                var thongTinDuocPham = thongTinDuocPhams.First(o => o.Id == duocPhamXuatGridVo.DuocPhamBenhVienId);
                duocPhamXuatGridVo.MaDuocPham = thongTinDuocPham.Ma;
                duocPhamXuatGridVo.TenDuocPham = thongTinDuocPham.Ten;
                duocPhamXuatGridVo.TenNhom = thongTinDuocPham.TenNhom;
                duocPhamXuatGridVo.DVT = thongTinDuocPham.DVT;
                duocPhamXuatGridVo.DuocPhamBenhVienPhanNhomId = thongTinDuocPham.DuocPhamBenhVienPhanNhomId;
                duocPhamXuatGridVo.HamLuong = thongTinDuocPham.HamLuong;
                duocPhamXuatGridVo.SoDangKy = thongTinDuocPham.SoDangKy;


                if (string.IsNullOrEmpty(searchString) ||
                    (duocPhamXuatGridVo.TenDuocPham != null && duocPhamXuatGridVo.TenDuocPham.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())) ||
                    (duocPhamXuatGridVo.MaDuocPham != null && duocPhamXuatGridVo.MaDuocPham.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())) ||
                    (duocPhamXuatGridVo.SoLo != null && duocPhamXuatGridVo.SoLo.ToLower().RemoveDiacritics().Contains(searchString.ToLower().RemoveDiacritics())))
                {
                    if (duocPhamXuatGridVo.DuocPhamBenhVienPhanNhomId == groupId)
                        dataReturn.Add(duocPhamXuatGridVo);
                }
            }

            foreach (var item in dataReturn)
            {
                item.SoLuongXuat = item.SoLuongTon;
                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
            }
            return dataReturn;
        }

        //public async Task<GridDataSource> GetAllDuocPhamDataOld(QueryInfo queryInfo)
        //{
        //    BuildDefaultSortExpression(queryInfo);

        //    var lstIdString = string.Empty;
        //    long khoXuatId = 0;
        //    var lstDaChon = new List<DaSuaSoLuongXuat>();

        //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {
        //        lstIdString = queryInfo.AdditionalSearchString.Split("|").Length > 1 ? queryInfo.AdditionalSearchString.Split("|")[1] : "[]";
        //        long.TryParse(queryInfo.AdditionalSearchString.Split("|").Length > 0 ? queryInfo.AdditionalSearchString.Split("|")[0] : "0", out khoXuatId);
        //        lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|").Length > 2 ? queryInfo.AdditionalSearchString.Split("|")[2] : "[]");
        //    }
        //    var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
        //    var query = _duocPhamBenhVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuocPhamXuatGridVo { }).AsQueryable();
        //    var queryCoBHYT = _duocPhamBenhVienRepository.TableNoTracking
        //            .Where(p => p.NhapKhoDuocPhamChiTiets.Any(x => x.LaDuocPhamBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
        //                            && x.HanSuDung >= DateTime.Now
        //                            && x.NhapKhoDuocPhams.KhoDuocPhams.Id == khoXuatId))
        //            .Select(s => new DuocPhamXuatGridVo
        //            {
        //                Id = s.Id + "," + (s.DuocPhamBenhVienPhanNhomId != null ? s.DuocPhamBenhVienPhanNhomId : 0) + "," + "true",
        //                TenDuocPham = s.DuocPham.Ten,
        //                DVT = s.DuocPham.DonViTinh.Ten,
        //                LaDuocPhamBHYT = true,
        //                DuocPhamBenhVienPhanNhomId = (s.DuocPhamBenhVienPhanNhomId != null ? s.DuocPhamBenhVienPhanNhomId : 0),
        //                TenNhom = s.DuocPhamBenhVienPhanNhom != null ? s.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM",
        //                MaDuocPham = s.Ma,
        //                SoDangKy = s.DuocPham.SoDangKy,
        //                HamLuong = s.DuocPham.HamLuong,

        //                SoLo = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
        //                                                                 && nkct.LaDuocPhamBHYT == true
        //                                                                 && nkct.DuocPhamBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.Solo).FirstOrDefault(),

        //                HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
        //                                                                 && nkct.LaDuocPhamBHYT == true
        //                                                                 && nkct.DuocPhamBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.HanSuDung).FirstOrDefault(),

        //                DonGia = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
        //                                                                 && nkct.LaDuocPhamBHYT == true
        //                                                                 && nkct.DuocPhamBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.DonGiaNhap).FirstOrDefault(),
        //            });
        //    queryCoBHYT = queryCoBHYT.ApplyLike(queryInfo.SearchTerms, g => g.SoDangKy, g => g.MaDuocPham, g => g.TenDuocPham, g => g.DVT, g => g.SoLo, g => g.HamLuong);
        //    var queryKhongBHYT = _duocPhamBenhVienRepository.TableNoTracking
        //            .Where(p => p.NhapKhoDuocPhamChiTiets.Any(x => !x.LaDuocPhamBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
        //                            && x.HanSuDung >= DateTime.Now
        //                            && x.NhapKhoDuocPhams.KhoDuocPhams.Id == khoXuatId))
        //            .Select(s => new DuocPhamXuatGridVo
        //            {
        //                Id = s.Id + "," + (s.DuocPhamBenhVienPhanNhomId != null ? s.DuocPhamBenhVienPhanNhomId : 0) + "," + "false",
        //                TenDuocPham = s.DuocPham.Ten,
        //                DVT = s.DuocPham.DonViTinh.Ten,
        //                LaDuocPhamBHYT = false,
        //                MaDuocPham = s.Ma,
        //                SoDangKy = s.DuocPham.SoDangKy,
        //                DuocPhamBenhVienPhanNhomId = (s.DuocPhamBenhVienPhanNhomId != null ? s.DuocPhamBenhVienPhanNhomId : 0),
        //                TenNhom = s.DuocPhamBenhVienPhanNhom != null ? s.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM",
        //                HamLuong = s.DuocPham.HamLuong,

        //                SoLo = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
        //                                                                && nkct.LaDuocPhamBHYT == false
        //                                                                && nkct.DuocPhamBenhVienId == s.Id
        //                                                                && nkct.HanSuDung >= DateTime.Now
        //                                                                && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.Solo).FirstOrDefault(),

        //                HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
        //                                                                 && nkct.LaDuocPhamBHYT == false
        //                                                                 && nkct.DuocPhamBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.HanSuDung).FirstOrDefault(),

        //                DonGia = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
        //                                                                 && nkct.LaDuocPhamBHYT == false
        //                                                                 && nkct.DuocPhamBenhVienId == s.Id
        //                                                                 && nkct.HanSuDung >= DateTime.Now
        //                                                                 && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
        //                                                                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
        //                                                                .Select(z => z.DonGiaNhap).FirstOrDefault(),
        //            });
        //    queryKhongBHYT = queryKhongBHYT.ApplyLike(queryInfo.SearchTerms, g => g.SoDangKy, g => g.MaDuocPham, g => g.TenDuocPham, g => g.DVT, g => g.SoLo, g => g.HamLuong);
        //    query = query.Concat(queryCoBHYT).Concat(queryKhongBHYT);

        //    if (!string.IsNullOrEmpty(lstIdString))
        //    {
        //        var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
        //        query = query.Where(p => !lstId.Contains(p.Id));
        //    }

        //    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
        //    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
        //        .Take(queryInfo.Take).ToArrayAsync();
        //    await Task.WhenAll(countTask, queryTask);

        //    var stt = 1;
        //    foreach (var item in queryTask.Result)
        //    {
        //        //
        //        var id = long.Parse(item.Id.Split(",")[0]);
        //        var nhapKho = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.LaDuocPhamBHYT == item.LaDuocPhamBHYT && x.DuocPhamBenhVienId == id
        //        //&& x.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 
        //        && x.NhapKhoDuocPhams.KhoDuocPhams.Id == khoXuatId);
        //        //&& x.DuocPhamBenhVienPhanNhomId == item.DuocPhamBenhVienPhanNhomId);

        //        item.SoLuongTon = nhapKho.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
        //        if (lstDaChon.Any(p => p.Id == item.Id))
        //        {
        //            item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
        //        }
        //        else
        //        {
        //            item.SoLuongXuat = item.SoLuongTon;
        //        }
        //        //
        //        item.STT = stt;
        //        stt++;
        //    }

        //    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        //}

        public async Task<GridDataSource> GetAllDuocPhamData(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;
            var lstDaChon = new List<DaSuaSoLuongXuat>();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|").Length > 1 ? queryInfo.AdditionalSearchString.Split("|")[1] : "[]";
                long.TryParse(queryInfo.AdditionalSearchString.Split("|").Length > 0 ? queryInfo.AdditionalSearchString.Split("|")[0] : "0", out khoXuatId);
                lstDaChon = JsonConvert.DeserializeObject<List<DaSuaSoLuongXuat>>(queryInfo.AdditionalSearchString.Split("|").Length > 2 ? queryInfo.AdditionalSearchString.Split("|")[2] : "[]");
            }

            var allDataNhapDuocPham = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(o => o.NhapKhoDuocPhams.KhoId == khoXuatId)
                .Select(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.SoLuongNhap,
                    o.SoLuongDaXuat,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT
                }).GroupBy(o => new
                {
                    o.DuocPhamBenhVienId,
                    o.DonGiaTonKho,
                    o.Solo,
                    o.HanSuDung,
                    o.LaDuocPhamBHYT,
                }, o => o,
                (k, v) => new DuocPhamXuatGridVo
                {
                    DuocPhamBenhVienId = k.DuocPhamBenhVienId,
                    SoLuongTon = v.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    DonGia = k.DonGiaTonKho,
                    SoLo = k.Solo,
                    HanSuDung = k.HanSuDung,
                    LaDuocPhamBHYT = k.LaDuocPhamBHYT
                }).Where(o => o.SoLuongTon >= 0.01).ToList();

            var duocPhamBenhVienIds = allDataNhapDuocPham.Select(o => o.DuocPhamBenhVienId).Distinct().ToList();

            var thongTinDuocPhams = _duocPhamBenhVienRepository.TableNoTracking
                .Where(o => duocPhamBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ma,
                    Ten = o.DuocPham.Ten,
                    DuocPhamBenhVienPhanNhomId = (o.DuocPhamBenhVienPhanNhomId != null ? o.DuocPhamBenhVienPhanNhomId : 0),
                    TenNhom = (o.DuocPhamBenhVienPhanNhom != null ? o.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM"),
                    DVT = o.DuocPham.DonViTinh.Ten,
                    SoDangKy = o.DuocPham.SoDangKy,
                    HamLuong = o.DuocPham.HamLuong
                }).ToList();

            var allData = new List<DuocPhamXuatGridVo>();
            foreach (var duocPhamXuatGridVo in allDataNhapDuocPham)
            {
                var thongTinDuocPham = thongTinDuocPhams.First(o => o.Id == duocPhamXuatGridVo.DuocPhamBenhVienId);
                duocPhamXuatGridVo.MaDuocPham = thongTinDuocPham.Ma;
                duocPhamXuatGridVo.TenDuocPham = thongTinDuocPham.Ten;
                duocPhamXuatGridVo.TenNhom = thongTinDuocPham.TenNhom;
                duocPhamXuatGridVo.DVT = thongTinDuocPham.DVT;
                duocPhamXuatGridVo.DuocPhamBenhVienPhanNhomId = thongTinDuocPham.DuocPhamBenhVienPhanNhomId;
                duocPhamXuatGridVo.HamLuong = thongTinDuocPham.HamLuong;
                duocPhamXuatGridVo.SoDangKy = thongTinDuocPham.SoDangKy;

                if (string.IsNullOrEmpty(queryInfo.SearchTerms) ||
                    (duocPhamXuatGridVo.TenDuocPham != null && duocPhamXuatGridVo.TenDuocPham.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (duocPhamXuatGridVo.MaDuocPham != null && duocPhamXuatGridVo.MaDuocPham.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())) ||
                    (duocPhamXuatGridVo.SoLo != null && duocPhamXuatGridVo.SoLo.ToLower().RemoveDiacritics().Contains(queryInfo.SearchTerms.ToLower().RemoveDiacritics())))
                {
                    if (!lstIdString.Contains(duocPhamXuatGridVo.Id))
                        allData.Add(duocPhamXuatGridVo);
                }
            }
            var dataReturn = allData.OrderBy(o => o.TenNhom).ThenBy(o => o.MaDuocPham).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            var stt = 1;
            foreach (var item in dataReturn)
            {
                item.SoLuongXuat = item.SoLuongTon;
                if (lstDaChon.Any(p => p.Id == item.Id))
                {
                    item.SoLuongXuat = lstDaChon.First(p => p.Id == item.Id).SoLuongXuat;
                }
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = dataReturn, TotalRowCount = allData.Count };
        }

        public async Task<GridDataSource> GetAllDuocPhamTotal(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var lstIdString = string.Empty;
            long khoXuatId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                lstIdString = queryInfo.AdditionalSearchString.Split("|")[1];
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out khoXuatId);
            }
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var query = _duocPhamBenhVienRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DuocPhamXuatGridVo { }).AsQueryable();
            var queryCoBHYT = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(p => p.NhapKhoDuocPhamChiTiets.Any(x => x.LaDuocPhamBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
                                            && x.HanSuDung >= DateTime.Now
                                            && x.NhapKhoDuocPhams.KhoDuocPhams.Id == khoXuatId))
                    .Select(s => new DuocPhamXuatGridVo
                    {
                        //Id = s.Id + "," + (s.DuocPhamBenhVienPhanNhomId != null ? s.DuocPhamBenhVienPhanNhomId : 0) + "," + "true",
                        TenDuocPham = s.DuocPham.Ten,
                        DVT = s.DuocPham.DonViTinh.Ten,
                        LaDuocPhamBHYT = true,

                        DuocPhamBenhVienPhanNhomId = (s.DuocPhamBenhVienPhanNhomId != null ? s.DuocPhamBenhVienPhanNhomId : 0),
                        TenNhom = s.DuocPhamBenhVienPhanNhom != null ? s.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM",

                        MaDuocPham = s.Ma,
                        SoDangKy = s.DuocPham.SoDangKy,

                        HamLuong = s.DuocPham.HamLuong,

                        SoLo = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
                                                                        && nkct.LaDuocPhamBHYT == true
                                                                        && nkct.DuocPhamBenhVienId == s.Id
                                                                        && nkct.HanSuDung >= DateTime.Now
                                                                        && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.Solo).FirstOrDefault(),

                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
                                                                         && nkct.LaDuocPhamBHYT == true
                                                                         && nkct.DuocPhamBenhVienId == s.Id
                                                                         && nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.HanSuDung).FirstOrDefault(),

                        DonGia = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
                                                                         && nkct.LaDuocPhamBHYT == true
                                                                         && nkct.DuocPhamBenhVienId == s.Id
                                                                         && nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.DonGiaNhap).FirstOrDefault(),
                    });
            queryCoBHYT = queryCoBHYT.ApplyLike(queryInfo.SearchTerms, g => g.SoDangKy, g => g.MaDuocPham, g => g.TenDuocPham, g => g.DVT, g => g.SoLo, g => g.HamLuong);
            var queryKhongBHYT = _duocPhamBenhVienRepository.TableNoTracking
                    .Where(p => p.NhapKhoDuocPhamChiTiets.Any(x => !x.LaDuocPhamBHYT && x.SoLuongNhap - x.SoLuongDaXuat > 0
                                    && x.HanSuDung >= DateTime.Now
                                    && x.NhapKhoDuocPhams.KhoDuocPhams.Id == khoXuatId))
                    .Select(s => new DuocPhamXuatGridVo
                    {
                        //Id = s.Id + "," + (s.DuocPhamBenhVienPhanNhomId != null ? s.DuocPhamBenhVienPhanNhomId : 0) + "," + "false",
                        TenDuocPham = s.DuocPham.Ten,
                        DVT = s.DuocPham.DonViTinh.Ten,
                        LaDuocPhamBHYT = false,
                        MaDuocPham = s.Ma,
                        SoDangKy = s.DuocPham.SoDangKy,

                        DuocPhamBenhVienPhanNhomId = (s.DuocPhamBenhVienPhanNhomId != null ? s.DuocPhamBenhVienPhanNhomId : 0),
                        TenNhom = s.DuocPhamBenhVienPhanNhom != null ? s.DuocPhamBenhVienPhanNhom.Ten : "CHƯA PHÂN NHÓM",
                        HamLuong = s.DuocPham.HamLuong,

                        SoLo = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
                                                                        && nkct.LaDuocPhamBHYT == false
                                                                        && nkct.DuocPhamBenhVienId == s.Id
                                                                        && nkct.HanSuDung >= DateTime.Now
                                                                        && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.Solo).FirstOrDefault(),

                        HanSuDung = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
                                                                         && nkct.LaDuocPhamBHYT == false
                                                                         && nkct.DuocPhamBenhVienId == s.Id
                                                                         && nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.HanSuDung).FirstOrDefault(),

                        DonGia = s.NhapKhoDuocPhamChiTiets.Where(nkct => nkct.NhapKhoDuocPhams.KhoId == khoXuatId
                                                                         && nkct.LaDuocPhamBHYT == false
                                                                         && nkct.DuocPhamBenhVienId == s.Id
                                                                         && nkct.HanSuDung >= DateTime.Now
                                                                         && nkct.SoLuongDaXuat < nkct.SoLuongNhap)
                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                        .Select(z => z.DonGiaNhap).FirstOrDefault(),
                    });
            queryKhongBHYT = queryKhongBHYT.ApplyLike(queryInfo.SearchTerms, g => g.SoDangKy, g => g.MaDuocPham, g => g.TenDuocPham, g => g.DVT, g => g.SoLo, g => g.HamLuong);
            query = query.Concat(queryCoBHYT).Concat(queryKhongBHYT);

            if (!string.IsNullOrEmpty(lstIdString))
            {
                var lstId = JsonConvert.DeserializeObject<List<string>>(lstIdString);
                query = query.Where(p => !lstId.Contains(p.Id));
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<long> XuatKhoDuocPham(ThongTinXuatKhoDuocPhamVo thongTinXuatKhoDuocPhamVo)
        {
            foreach (var xuatKhoDuocPhamChiTiet in thongTinXuatKhoDuocPhamVo.ThongTinXuatKhoDuocPhamChiTietVos)
            {
                var thongTinChiTiet = xuatKhoDuocPhamChiTiet.Id.Split(',');
                xuatKhoDuocPhamChiTiet.DuocPhamId = long.Parse(thongTinChiTiet[0]);
                xuatKhoDuocPhamChiTiet.LaDpBHYT = thongTinChiTiet[1] == "1";
                xuatKhoDuocPhamChiTiet.HanSuDung = DateTime.ParseExact(thongTinChiTiet[2], "yyyyMMdd", null);
                xuatKhoDuocPhamChiTiet.SoLo = thongTinChiTiet[3];
            }
            var xuatKhoDuocPhamChiTietVos = thongTinXuatKhoDuocPhamVo.ThongTinXuatKhoDuocPhamChiTietVos;
            
                var duocPhamBenhVienIds = xuatKhoDuocPhamChiTietVos.Select(o => o.DuocPhamId).ToList();
                var soLos = xuatKhoDuocPhamChiTietVos.Select(o => o.SoLo).ToList();

                var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.Table
                    .Where(o => o.NhapKhoDuocPhams.KhoId == thongTinXuatKhoDuocPhamVo.KhoXuatId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                    .ToList();
                //xuat kho
                var xuatKhoDuocPham = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham
                {
                    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                    KhoXuatId = thongTinXuatKhoDuocPhamVo.KhoXuatId,
                    KhoNhapId = thongTinXuatKhoDuocPhamVo.KhoNhapId,
                    LyDoXuatKho = thongTinXuatKhoDuocPhamVo.LyDoXuatKho,
                    NguoiXuatId = thongTinXuatKhoDuocPhamVo.NguoiXuatId,
                    LoaiNguoiNhan = thongTinXuatKhoDuocPhamVo.LoaiNguoiNhan,
                    TenNguoiNhan = thongTinXuatKhoDuocPhamVo.TenNguoiNhan,
                    NguoiNhanId = thongTinXuatKhoDuocPhamVo.NguoiNhanId,
                    NgayXuat = thongTinXuatKhoDuocPhamVo.NgayXuat
                };
                foreach (var chiTietVo in xuatKhoDuocPhamChiTietVos)
                {
                    var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                        .Where(o => o.DuocPhamBenhVienId == chiTietVo.DuocPhamId && o.LaDuocPhamBHYT == chiTietVo.LaDpBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Date);
                    var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat) && slTon < chiTietVo.SoLuongXuat)
                    {
                        throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                    }
                    double soLuongCanXuat = chiTietVo.SoLuongXuat;
                    while (!soLuongCanXuat.Equals(0))
                    {
                        // tinh so luong xuat
                        var nhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTietXuats
                            .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                        var soLuongTon = nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat;
                        var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                        nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongXuat;
                        var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                        {
                            SoLuongXuat = soLuongXuat,
                            NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                            NgayXuat = thongTinXuatKhoDuocPhamVo.NgayXuat
                        };
                        var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                            NgayXuat = thongTinXuatKhoDuocPhamVo.NgayXuat
                        };
                        xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                        xuatKhoDuocPham.XuatKhoDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);

                        soLuongCanXuat = soLuongCanXuat - soLuongXuat;
                    }
                }
                //nhap kho
                var nhapKho = new NhapKhoDuocPham();
                nhapKho.XuatKhoDuocPham = xuatKhoDuocPham;
                nhapKho.KhoId = thongTinXuatKhoDuocPhamVo.KhoNhapId;
                nhapKho.NguoiGiaoId = thongTinXuatKhoDuocPhamVo.NguoiXuatId;
                nhapKho.NguoiNhapId = thongTinXuatKhoDuocPhamVo.NguoiNhanId ?? _userAgentHelper.GetCurrentUserId();
                nhapKho.NgayNhap = thongTinXuatKhoDuocPhamVo.NgayXuat;
                nhapKho.LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong;
                foreach (var item in xuatKhoDuocPham.XuatKhoDuocPhamChiTiets)
                {
                    foreach (var viTri in item.XuatKhoDuocPhamChiTietViTris)
                    {
                        var nhapKhoChiTiet = new NhapKhoDuocPhamChiTiet();
                        nhapKhoChiTiet.HopDongThauDuocPhamId = viTri.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId;
                        nhapKhoChiTiet.Solo = viTri.NhapKhoDuocPhamChiTiet.Solo;
                        nhapKhoChiTiet.LaDuocPhamBHYT = viTri.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT;
                        nhapKhoChiTiet.HanSuDung = viTri.NhapKhoDuocPhamChiTiet.HanSuDung;
                        nhapKhoChiTiet.SoLuongNhap = viTri.SoLuongXuat;
                        nhapKhoChiTiet.DonGiaNhap = viTri.NhapKhoDuocPhamChiTiet.DonGiaNhap;
                        nhapKhoChiTiet.VAT = viTri.NhapKhoDuocPhamChiTiet.VAT;
                        nhapKhoChiTiet.TiLeBHYTThanhToan = viTri.NhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan;
                        nhapKhoChiTiet.MaVach = viTri.NhapKhoDuocPhamChiTiet.MaVach;
                        nhapKhoChiTiet.MaRef = viTri.NhapKhoDuocPhamChiTiet.MaRef;
                        nhapKhoChiTiet.DuocPhamBenhVienId = viTri.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId;
                        nhapKhoChiTiet.SoLuongDaXuat = 0;
                        nhapKhoChiTiet.NgayNhap = thongTinXuatKhoDuocPhamVo.NgayXuat;
                        nhapKhoChiTiet.NgayNhapVaoBenhVien = viTri.NhapKhoDuocPhamChiTiet.NgayNhapVaoBenhVien;
                        nhapKhoChiTiet.TiLeTheoThapGia = viTri.NhapKhoDuocPhamChiTiet.TiLeTheoThapGia;
                        nhapKhoChiTiet.PhuongPhapTinhGiaTriTonKho = viTri.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho;
                        nhapKho.NhapKhoDuocPhamChiTiets.Add(nhapKhoChiTiet);
                    }
                }
                _nhapKhoDuocPhamRepository.Add(nhapKho);

            return xuatKhoDuocPham.Id;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var queryObject = new XuatKhoDuocPhamSearch();
            string searchString = null;
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                searchString = queryInfo.SearchTerms.Replace("\t", "").Trim();
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<XuatKhoDuocPhamSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    searchString = queryObject.SearchString.Replace("\t", "").Trim();
                }
            }
            //update 01/06/2021
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                .Where(p => p.LoaiXuatKho != Enums.XuatKhoDuocPham.XuatHuy &&
                        p.KhoDuocPhamXuat != null && (phongBenhVien != null && p.KhoDuocPhamXuat.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoDuocPhamXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2))
                .Select(s => new XuatKhoDuocPhamGridVo
                {
                    Id = s.Id,
                    KhoDuocPhamXuat = s.KhoDuocPhamXuat != null ? s.KhoDuocPhamXuat.Ten : "",
                    KhoDuocPhamNhap = s.KhoDuocPhamNhap != null ? s.KhoDuocPhamNhap.Ten : "",
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NguoiNhan = s.LoaiNguoiNhan == Enums.LoaiNguoiGiaoNhan.TrongHeThong ? (s.NguoiNhan != null ? (s.NguoiNhan.User != null ? s.NguoiNhan.User.HoTen : s.TenNguoiNhan) : s.TenNguoiNhan) : s.TenNguoiNhan,
                    NguoiXuat = s.NguoiXuat != null ? (s.NguoiXuat.User != null ? s.NguoiXuat.User.HoTen : "") : "",

                    NgayXuat = s.NgayXuat,
                });
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.ApplyLike(searchString, g => g.KhoDuocPhamXuat, g => g.KhoDuocPhamNhap, g => g.SoPhieu, g => g.NguoiNhan, g => g.NguoiXuat, g => g.LyDoXuatKho);
            }

            if (queryObject != null)
            {
                if (queryObject.RangeXuat != null &&
                               (!string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || !string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay)))
                {
                    //DateTime.TryParseExact(queryObject.RangeXuat.TuNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    //DateTime.TryParseExact(queryObject.RangeXuat.DenNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    queryObject.RangeXuat.TuNgay.TryParseExactCustom(out var tuNgay);
                    queryObject.RangeXuat.DenNgay.TryParseExactCustom(out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay) || p.NgayXuat <= denNgay));
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);


            var queryObject = new XuatKhoDuocPhamSearch();
            string searchString = null;
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                searchString = queryInfo.SearchTerms.Replace("\t", "").Trim();
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<XuatKhoDuocPhamSearch>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    searchString = queryObject.SearchString.Replace("\t", "").Trim();
                }
            }
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                .Where(p => p.LoaiXuatKho != Enums.XuatKhoDuocPham.XuatHuy &&
                //p.KhoDuocPhamXuat != null && phongBenhVien != null && p.KhoDuocPhamXuat.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId)
               p.KhoDuocPhamXuat != null && (phongBenhVien != null && p.KhoDuocPhamXuat.PhongBenhVien.KhoaPhongId == phongBenhVien.KhoaPhongId || p.KhoDuocPhamXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2))


                .Select(s => new XuatKhoDuocPhamGridVo
                {
                    Id = s.Id,
                    KhoDuocPhamXuat = s.KhoDuocPhamXuat != null ? s.KhoDuocPhamXuat.Ten : "",
                    KhoDuocPhamNhap = s.KhoDuocPhamNhap != null ? s.KhoDuocPhamNhap.Ten : "",
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NguoiNhan = s.LoaiNguoiNhan == Enums.LoaiNguoiGiaoNhan.TrongHeThong ? (s.NguoiNhan != null ? (s.NguoiNhan.User != null ? s.NguoiNhan.User.HoTen : s.TenNguoiNhan) : s.TenNguoiNhan) : s.TenNguoiNhan,
                    NguoiXuat = s.NguoiXuat != null ? (s.NguoiXuat.User != null ? s.NguoiXuat.User.HoTen : "") : "",

                    NgayXuat = s.NgayXuat,
                });
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.ApplyLike(searchString, g => g.KhoDuocPhamXuat, g => g.KhoDuocPhamNhap, g => g.SoPhieu, g => g.NguoiNhan, g => g.NguoiXuat, g => g.LyDoXuatKho);
            }

            if (queryObject != null)
            {
                if (queryObject.RangeXuat != null &&
                              (!string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || !string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay)))
                {
                    //DateTime.TryParseExact(queryObject.RangeXuat.TuNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    //DateTime.TryParseExact(queryObject.RangeXuat.DenNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    queryObject.RangeXuat.TuNgay.TryParseExactCustom(out var tuNgay);
                    queryObject.RangeXuat.DenNgay.TryParseExactCustom(out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryObject.RangeXuat.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryObject.RangeXuat.DenNgay) || p.NgayXuat <= denNgay));
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? XuatKhoDuocPhamId, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "DuocPham", Dir = "asc" } };
            }

            long par = 0;
            if (XuatKhoDuocPhamId != null && XuatKhoDuocPhamId != 0)
            {
                par = XuatKhoDuocPhamId ?? 0;
            }
            else
            {
                par = long.Parse(queryInfo.SearchTerms);
            }

            var query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == par && x.SoLuongXuat > 0)
                .Select(s => new XuatKhoDuocPhamChildrenGridVo()
                {
                    Id = s.Id,
                    DuocPham = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.Ten,
                    Nhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    DVT = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    Loai = s.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT ? "BHYT" : "Không BHYT",
                    SoLuongXuat = s.SoLuongXuat.ApplyNumber(),
                    SoPhieu = s.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.SoPhieu,
                });



            var queryString = queryInfo.AdditionalSearchString;

            if (!string.IsNullOrEmpty(queryString))
            {
                query = query.ApplyLike(queryString, g => g.DuocPham, g => g.DVT, g => g.Loai);
            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == long.Parse(queryInfo.SearchTerms) && x.SoLuongXuat > 0)
                .Select(s => new XuatKhoDuocPhamChildrenGridVo()
                {
                    Id = s.Id,
                    DuocPham = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.Ten,
                    Nhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    DVT = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    Loai = s.NhapKhoDuocPhamChiTiet.LaDuocPhamBHYT ? "BHYT" : "Không BHYT",
                    SoLuongXuat = s.SoLuongXuat.ApplyFormatMoneyToDouble(false),
                });
            var queryString = queryInfo.AdditionalSearchString;

            if (!string.IsNullOrEmpty(queryString))
            {
                query = query.ApplyLike(queryString, g => g.DuocPham, g => g.DVT, g => g.Loai);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupItemVo>> GetKhoDuocPham(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var lstEntity = await _khoDuocPhamRepository.TableNoTracking.Where(p =>
            (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1 || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
            && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
            && EF.Functions.Like(p.Ten, $"%{model.Query}%"))
                .Take(1000)
                .ToListAsync();
            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetKhoTheoLoaiDuocPham(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var khos = await _khoDuocPhamRepository.TableNoTracking.Where(p =>
                                ((p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe
                         || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoVacXin)
                              && p.KhoaPhongId == phongBenhVien.KhoaPhongId
                              && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId) || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
                              && p.LoaiDuocPham == true

                              )
                                .ApplyLike(model.Query, p => p.Ten)
                                .Select(item => new LookupItemVo
                                {
                                    DisplayName = item.Ten,
                                    KeyId = item.Id,
                                }).Take(model.Take).ToListAsync();
            return khos;
        }

        public async Task<List<LookupItemVo>> GetKhoDuocPhamNhap(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var khoXuatId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (khoXuatId == 0) return new List<LookupItemVo>();
            var khoXuat = _khoDuocPhamRepository.TableNoTracking.First(p => p.Id == khoXuatId);

            if (khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1)
            {
                var lst = await _khoDuocPhamRepository.TableNoTracking.Where(p => p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId))
                    .Where(p => EF.Functions.Like(p.Ten, $"%{model.Query}%"))
                    .Take(1000).Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id
                    }).ToListAsync();

                return lst;
            }
            else
            {
                var lst = await _khoDuocPhamRepository.TableNoTracking.Where(p => (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc)
                    && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId))
                    .Where(p => EF.Functions.Like(p.Ten, $"%{model.Query}%"))
                    .Take(1000).Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id
                    }).ToListAsync();

                return lst;
            }
        }

        public async Task<List<LookupItemVo>> GetKhoLoaiDuocPhamNhap(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var khoXuatId = CommonHelper.GetIdFromRequestDropDownList(model);
            if (khoXuatId == 0) return new List<LookupItemVo>();
            var khoXuat = _khoDuocPhamRepository.TableNoTracking.First(p => p.Id == khoXuatId);

            var khos = _khoDuocPhamRepository.TableNoTracking
                       .Where(s => (khoXuat.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1) ? s.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && s.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId) && s.LoaiDuocPham == true : (s.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe || s.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc || (khoXuatId == (long)Enums.EnumKhoDuocPham.KhoHoaChat && s.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoKSNK)) && s.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId) && s.LoaiDuocPham == true)

                       .ApplyLike(model.Query, p => p.Ten)
                       .Select(item => new LookupItemVo
                       {
                           DisplayName = item.Ten,
                           KeyId = item.Id
                       }).Take(model.Take);
            return await khos.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetLoaiXuatKho(DropDownListRequestModel model)
        {
            var lst = await _khoDuocPhamRepository.TableNoTracking.Where(p => p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1)
                .Where(p => EF.Functions.Like(p.Ten, $"%{model.Query}%"))
                .Take(model.Take).Select(item => new LookupItemVo
                {
                    DisplayName = item.Ten,
                    KeyId = item.Id
                }).ToListAsync();

            return lst;
        }

        public async Task<List<LookupItemVo>> GetNguoiXuat(DropDownListRequestModel model)
        {
            var lstEntity = await _nhanVienRepository.TableNoTracking
                .Include(p => p.User)
                .Where(p => EF.Functions.Like(p.User.HoTen, $"%{model.Query}%"))
                //.Take(model.Take)
                .ToListAsync();
            var query = lstEntity.Select(item => new LookupItemVo
            {
                DisplayName = item.User.HoTen,
                KeyId = item.Id,
            }).ToList();

            return query;
        }

        public async Task<List<KhoTemplateVo>> GetListDuocPham(DropDownListRequestModel model)
        {
            var id = CommonHelper.GetIdFromRequestDropDownList(model);

            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                var lstEntity2 = _duocPhamRepository.TableNoTracking
                    .Where(p => p.DuocPhamBenhVien != null && p.DuocPhamBenhVien.HieuLuc && p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(x => x.NhapKhoDuocPhams.KhoId == id));

                var query2 = await lstEntity2.Select(item => new KhoTemplateVo
                {
                    DisplayName = item.Ten + " - " + item.HoatChat,
                    KeyId = item.Id,
                    Ten = item.Ten,
                    HoatChat = item.HoatChat,
                })
                .Where(p => p.Ten.Contains(model.Query ?? "") || p.HoatChat.Contains(model.Query ?? ""))
                .ToListAsync();

                return query2;
            }

            var lstColumnNameSearch = new List<string>();
            lstColumnNameSearch.Add("Ten");
            lstColumnNameSearch.Add("HoatChat");

            var lstEntity = await _duocPhamRepository.ApplyFulltext(model.Query, "DuocPham", lstColumnNameSearch)
                //.Include(p => p.DuocPhamBenhVien).ThenInclude(p => p.NhapKhoDuocPhamChiTiets).ThenInclude(p => p.NhapKhoDuocPhams)
                //.Where(p => p.DuocPhamBenhVien != null && p.DuocPhamBenhVien.HieuLuc && p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(x => x.NhapKhoDuocPhamId == id))
                .Where(p => p.DuocPhamBenhVien != null && p.DuocPhamBenhVien.HieuLuc && p.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Any(x => x.NhapKhoDuocPhams.KhoId == id))

                .ToListAsync();
            //var query = lstEntity.Select(item => new LookupItemVo
            //{
            //    DisplayName = item.Ten,
            //    KeyId = item.Id,
            //}).ToList();
            var query = lstEntity.Select(item => new KhoTemplateVo
            {
                DisplayName = item.Ten + " - " + item.HoatChat,
                KeyId = item.Id,
                Ten = item.Ten,
                HoatChat = item.HoatChat,
            }).ToList();

            return query;
        }

        public async Task<List<LookupItemVo>> GetNguoiNhan(DropDownListRequestModel model)
        {
            var lstEntity = await _nhanVienRepository.TableNoTracking
                .Where(p => EF.Functions.Like(p.User.HoTen, $"%{model.Query}%"))
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.User.HoTen,
                    KeyId = item.Id,
                })
                .Take(model.Take)
                .ToListAsync();
            //var query = lstEntity.Select(item => new LookupItemVo
            //{
            //    DisplayName = item.User.HoTen,
            //    KeyId = item.Id,
            //}).ToList();

            return lstEntity;
        }

        public async Task<XuatKhoDuocPhamChiTiet> GetDuocPham(ThemDuocPham model)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var result = new XuatKhoDuocPhamChiTiet();
            result.DuocPhamBenhVienId = model.DuocPhamBenhVienId ?? 0;
            //result.DuocPhamBenhVien = new Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien();
            //result.DuocPhamBenhVien.DuocPham = new DuocPham();
            //result.DuocPhamBenhVien.DuocPham.Id = model.DuocPhamBenhVienId ?? 0;
            //var duocPham = await _duocPhamRepository.TableNoTracking
            //    .Include(p => p.DuocPhamBenhVien)
            //    .ThenInclude(p => p.NhapKhoDuocPhamChiTiets)
            //    .ThenInclude(p => p.NhapKhoDuocPhams)
            //    .FirstOrDefaultAsync(p => p.Id == model.DuocPhamBenhVienId);
            var duocPhamBenhVien = await _duocPhamBenhVienRepository.TableNoTracking
                    .Include(p => p.NhapKhoDuocPhamChiTiets)
                    .ThenInclude(p => p.NhapKhoDuocPhams)
                .FirstOrDefaultAsync(p => p.Id == model.DuocPhamBenhVienId);
            var soLuongXuat = model.SoLuongXuat;
            long tempId = 0;
            var nhomDuocPhamId = model.NhomDuocPhamId ?? 0;
            foreach (var item in duocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(p => p.LaDuocPhamBHYT == model.LaDuocPhamBHYT
            //&& p.DuocPhamBenhVienPhanNhomId == nhomDuocPhamId
            && p.NhapKhoDuocPhams.KhoId == (model.KhoId ?? tempId))
            .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung))
            {
                if (soLuongXuat == null || soLuongXuat == 0) break;
                var soLuongCon = item.SoLuongNhap - item.SoLuongDaXuat;
                if (soLuongXuat < soLuongCon)
                {
                    var chiTietViTri = new XuatKhoDuocPhamChiTietViTri
                    {
                        SoLuongXuat = soLuongXuat ?? 0,
                        NhapKhoDuocPhamChiTietId = item.Id,
                    };
                    result.XuatKhoDuocPhamChiTietViTris.Add(chiTietViTri);
                    soLuongXuat = 0;
                    break;
                }
                else if (soLuongCon == 0)
                {
                    continue;
                }
                else if (soLuongCon < 0)
                {
                    continue;
                }
                else
                {
                    soLuongXuat = soLuongXuat - soLuongCon;
                    var chiTietViTri = new XuatKhoDuocPhamChiTietViTri
                    {
                        SoLuongXuat = item.SoLuongNhap - item.SoLuongDaXuat,
                        NhapKhoDuocPhamChiTietId = item.Id,
                    };
                    result.XuatKhoDuocPhamChiTietViTris.Add(chiTietViTri);
                }
            }

            if (soLuongXuat != 0)
            {
                return null;
            }

            return result;
        }

        public async Task<double> GetSoLuongTon(long duocPhamId, bool isDatChatLuong, long khoNhapId)
        {
            var duocPham = await _duocPhamRepository.TableNoTracking
                .Include(p => p.DuocPhamBenhVien)
                .ThenInclude(p => p.NhapKhoDuocPhamChiTiets)
                .ThenInclude(p => p.NhapKhoDuocPhams)
                .FirstOrDefaultAsync(p => p.Id == duocPhamId);
            double total = 0;
            foreach (var item in duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(p => p.NhapKhoDuocPhams.KhoId == khoNhapId))
            {
                total = total + item.SoLuongNhap - item.SoLuongDaXuat;
            }
            return total;
        }

        public async Task<decimal> GetDonGiaBan(long duocPhamId)
        {
            var duocPham = await _duocPhamRepository.TableNoTracking
                .Include(p => p.DuocPhamBenhVien).ThenInclude(p => p.NhapKhoDuocPhamChiTiets).ThenInclude(p => p.NhapKhoDuocPhams)
                .LastOrDefaultAsync(p => p.Id == duocPhamId);
            if (duocPham == null) return 0;

            //var bhytDuocPham = duocPham.DuocPhamBenhVien.DuocPhamBenhVienGiaBaoHiems.LastOrDefault(p =>
            //    p.TuNgay.Date <= DateTime.Now.Date
            //    && (p.DenNgay == null || p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date));

            var nhapKhoChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.LastOrDefault();

            if (nhapKhoChiTiet == null) return 0;
            //Co BH
            if (nhapKhoChiTiet.LaDuocPhamBHYT == true)
            {
                return nhapKhoChiTiet.DonGiaNhap;
            }
            else
            {
                var donGia = nhapKhoChiTiet.DonGiaNhap;
                if (donGia <= 1000)
                {
                    return donGia + donGia * 15 / 100;
                }
                else if (1000 < donGia && donGia <= 5000)
                {
                    return donGia + donGia * 10 / 100;
                }
                else if (5000 < donGia && donGia <= 100000)
                {
                    return donGia + donGia * 7 / 100;
                }
                else if (100000 < donGia && donGia <= 1000000)
                {
                    return donGia + donGia * 5 / 100;
                }
                else
                {
                    return donGia + donGia * 2 / 100;
                }
            }

            return 0;
        }

        public async Task<XuatKhoDuocPham> UpdateXuatKho(XuatKhoDuocPham entity)
        {
            _nhapKhoDuocPhamRepository.AutoCommitEnabled = false;
            BaseRepository.AutoCommitEnabled = false;
            _nhapKhoDuocPhamChiTietRepository.AutoCommitEnabled = false;
            var currentEntity = await BaseRepository.Table
                .Include(p => p.XuatKhoDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTris)
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            var loaiXuatKhoOld = currentEntity.LoaiXuatKho;

            if (currentEntity != null)
            {
                //update XuatKhoDuocPham
                currentEntity.KhoXuatId = entity.KhoXuatId;
                currentEntity.KhoNhapId = entity.KhoNhapId;
                currentEntity.SoPhieu = entity.SoPhieu;
                currentEntity.LoaiXuatKho = entity.LoaiXuatKho;
                currentEntity.NguoiXuatId = entity.NguoiXuatId;
                currentEntity.LoaiNguoiNhan = entity.LoaiNguoiNhan;
                currentEntity.NguoiNhanId = entity.NguoiNhanId;
                currentEntity.TenNguoiNhan = entity.TenNguoiNhan;
                currentEntity.LyDoXuatKho = entity.LyDoXuatKho;
                currentEntity.NgayXuat = entity.NgayXuat;

                var nhapKhoEntity =
                    await _nhapKhoDuocPhamRepository.Table
                    .Include(p => p.NhapKhoDuocPhamChiTiets)
                    .FirstOrDefaultAsync(p => p.XuatKhoDuocPhamId == entity.Id);

                //update XuatKhoDuocPhamChiTiet
                var chiTietTemp = entity.XuatKhoDuocPhamChiTiets;
                foreach (var item in currentEntity.XuatKhoDuocPhamChiTiets)
                {
                    if (!chiTietTemp.Any(p => p.Id == item.Id))
                    {
                        item.WillDelete = true;

                        //remove NhapKhoDuocPham added
                        if (nhapKhoEntity != null)
                        {
                            var nhapKhoChiTiet = nhapKhoEntity.NhapKhoDuocPhamChiTiets.FirstOrDefault(p =>
                                p.DuocPhamBenhVienId == item.DuocPhamBenhVienId);
                            if (nhapKhoChiTiet != null)
                            {
                                nhapKhoChiTiet.WillDelete = true;
                            }
                        }

                        //return value SoLuongDaXuat;
                        foreach (var viTri in item.XuatKhoDuocPhamChiTietViTris)
                        {
                            var nhapChiTietRevert =
                                await _nhapKhoDuocPhamChiTietRepository.Table.FirstOrDefaultAsync(p =>
                                    p.Id == viTri.NhapKhoDuocPhamChiTietId);
                            nhapChiTietRevert.SoLuongDaXuat = nhapChiTietRevert.SoLuongDaXuat - viTri.SoLuongXuat;
                            await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(nhapChiTietRevert);
                        }
                    }
                    else
                    {
                        item.NgayXuat = entity.NgayXuat;
                        foreach (var viTri in item.XuatKhoDuocPhamChiTietViTris)
                        {
                            viTri.NgayXuat = entity.NgayXuat;
                        }
                    }
                }

                //add new
                var lstXuatKhoDuocPhamChiTietNeedAdd = new List<XuatKhoDuocPhamChiTiet>();
                foreach (var item in entity.XuatKhoDuocPhamChiTiets)
                {
                    if (item.Id == 0)
                    {
                        var modelThemDuocPham = new ThemDuocPham
                        {
                            //TODO update entity kho on 9/9/2020
                            //ChatLuong = item.DatChatLuong == true ? 1 : 0,
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                            SoLuongXuat = item.XuatKhoDuocPhamChiTietViTris.Select(p => p.SoLuongXuat).Sum(),
                            KhoId = entity.KhoXuatId,
                        };
                        var xuatKhoDuocPhamChiTiet = await GetDuocPham(modelThemDuocPham);
                        //var m = new System.Collections.Generic.List<XuatKhoDuocPhamChiTietViTri>();
                        //clear data old

                        //
                        //foreach (var viTri in xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
                        //{
                        //    var viTriAdd = new XuatKhoDuocPhamChiTietViTri
                        //    {
                        //        SoLuongXuat = viTri.SoLuongXuat,
                        //        NhapKhoDuocPhamChiTietId = viTri.NhapKhoDuocPhamChiTietId,
                        //    };
                        //    item.XuatKhoDuocPhamChiTietViTris.Add(viTriAdd);
                        //}
                        //TODO update entity kho on 9/9/2020
                        //xuatKhoDuocPhamChiTiet.DatChatLuong = item.DatChatLuong;
                        xuatKhoDuocPhamChiTiet.DuocPhamBenhVienId = item.DuocPhamBenhVienId;
                        xuatKhoDuocPhamChiTiet.DuocPhamBenhVien = null;
                        xuatKhoDuocPhamChiTiet.NgayXuat = entity.NgayXuat;
                        //update 24/03/2020
                        foreach (var viTriItem in xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
                        {
                            viTriItem.NgayXuat = entity.NgayXuat;
                        }
                        lstXuatKhoDuocPhamChiTietNeedAdd.Add(xuatKhoDuocPhamChiTiet);
                    }
                }

                foreach (var chiTiet in lstXuatKhoDuocPhamChiTietNeedAdd)
                {
                    currentEntity.XuatKhoDuocPhamChiTiets.Add(chiTiet);
                }
                //add new

                //update NhapKhoDuocPhamChiTiet
                foreach (var item in currentEntity.XuatKhoDuocPhamChiTiets.Where(p => p.WillDelete == false))
                {
                    foreach (var viTri in item.XuatKhoDuocPhamChiTietViTris)
                    {
                        await UpdateNhapKhoDuocPhamChiTiet(viTri.NhapKhoDuocPhamChiTietId,
                            viTri.SoLuongXuat);
                    }
                }

                //update NhapKhoDuocPham added
                if (nhapKhoEntity != null)
                {
                    nhapKhoEntity.LoaiNguoiGiao = entity.LoaiNguoiNhan;
                    nhapKhoEntity.TenNguoiGiao = entity.NguoiXuat?.User?.HoTen;
                    nhapKhoEntity.NguoiGiaoId = entity.NguoiXuatId;
                    nhapKhoEntity.NguoiNhapId = entity.NguoiNhanId ?? 0;
                    nhapKhoEntity.NgayNhap = entity.NgayXuat;
                    nhapKhoEntity.SoChungTu = entity.SoPhieu;
                }

                //remove NhapKhoDuocPham added
                if (nhapKhoEntity != null && !nhapKhoEntity.NhapKhoDuocPhamChiTiets.Any(p => p.WillDelete == false))
                {
                    nhapKhoEntity.WillDelete = true;
                }
                else if (nhapKhoEntity != null && currentEntity.LoaiXuatKho != Enums.XuatKhoDuocPham.XuatQuaKhoKhac && loaiXuatKhoOld == Enums.XuatKhoDuocPham.XuatQuaKhoKhac)
                {
                    nhapKhoEntity.WillDelete = true;
                }

                //add NhapKhoDuocPham added
                if (currentEntity.KhoNhapId != null && currentEntity.KhoNhapId != 0
                    && currentEntity.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatQuaKhoKhac && loaiXuatKhoOld != Enums.XuatKhoDuocPham.XuatQuaKhoKhac)
                {
                    var nhapKho = new NhapKhoDuocPham();
                    nhapKho.XuatKhoDuocPhamId = currentEntity.Id;
                    nhapKho.KhoId = currentEntity.KhoNhapId ?? 0;
                    nhapKho.SoChungTu = currentEntity.SoPhieu;
                    //TODO update entity kho on 9/9/2020
                    //nhapKho.LoaiNhapKho = Enums.EnumLoaiNhapKho.NhapTuKhoKhac;
                    nhapKho.TenNguoiGiao = currentEntity.NguoiXuat?.User?.HoTen;
                    nhapKho.NguoiGiaoId = currentEntity.NguoiXuatId;
                    nhapKho.NguoiNhapId = currentEntity.NguoiNhanId ?? 0;
                    nhapKho.DaHet = false;
                    nhapKho.NgayNhap = entity.NgayXuat;
                    nhapKho.LoaiNguoiGiao = currentEntity.LoaiNguoiNhan;
                    foreach (var item in currentEntity.XuatKhoDuocPhamChiTiets.Where(p => p.WillDelete == false))
                    {
                        foreach (var viTri in item.XuatKhoDuocPhamChiTietViTris)
                        {
                            var duocPhamNhapChiTietCu = await GetNhapKhoDuocPhamChiTietById(viTri.NhapKhoDuocPhamChiTietId);
                            var nhapKhoDuocPhamChiTiet = new NhapKhoDuocPhamChiTiet();
                            nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId = duocPhamNhapChiTietCu.HopDongThauDuocPhamId;
                            nhapKhoDuocPhamChiTiet.Solo = duocPhamNhapChiTietCu.Solo;
                            nhapKhoDuocPhamChiTiet.HanSuDung = duocPhamNhapChiTietCu.HanSuDung;
                            nhapKhoDuocPhamChiTiet.SoLuongNhap = viTri.SoLuongXuat;
                            nhapKhoDuocPhamChiTiet.DonGiaNhap = duocPhamNhapChiTietCu.DonGiaNhap;
                            nhapKhoDuocPhamChiTiet.TiLeBHYTThanhToan = duocPhamNhapChiTietCu.TiLeBHYTThanhToan;
                            nhapKhoDuocPhamChiTiet.VAT = duocPhamNhapChiTietCu.VAT;
                            nhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho = duocPhamNhapChiTietCu.PhuongPhapTinhGiaTriTonKho;
                            nhapKhoDuocPhamChiTiet.MaVach = duocPhamNhapChiTietCu.MaVach;
                            nhapKhoDuocPhamChiTiet.MaRef = duocPhamNhapChiTietCu.MaRef;
                            nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId = item.DuocPhamBenhVienId;
                            //need update
                            //nhapKhoDuocPhamChiTiet.KhoDuocPhamViTriId = null;
                            nhapKhoDuocPhamChiTiet.SoLuongDaXuat = 0;
                            nhapKhoDuocPhamChiTiet.NgayNhap = entity.NgayXuat;
                            nhapKho.NhapKhoDuocPhamChiTiets.Add(nhapKhoDuocPhamChiTiet);
                            //nhapKho.NgayNhap = entity.NgayXuat;
                        }
                    }
                    await _nhapKhoDuocPhamRepository.AddAsync(nhapKho);
                }




                await BaseRepository.UpdateAsync(currentEntity);

                await _nhapKhoDuocPhamRepository.Context.SaveChangesAsync();
            }

            return currentEntity;
        }

        public async Task DeleteXuatKho(XuatKhoDuocPham entity)
        {
            _nhapKhoDuocPhamChiTietRepository.AutoCommitEnabled = false;
            BaseRepository.AutoCommitEnabled = false;

            foreach (var item in entity.XuatKhoDuocPhamChiTiets)
            {
                foreach (var viTri in item.XuatKhoDuocPhamChiTietViTris)
                {
                    var nhapKhoChiTietRevert = await _nhapKhoDuocPhamChiTietRepository.Table.FirstOrDefaultAsync(p =>
                            p.Id == viTri.NhapKhoDuocPhamChiTietId);
                    if (nhapKhoChiTietRevert != null)
                    {
                        nhapKhoChiTietRevert.SoLuongDaXuat = nhapKhoChiTietRevert.SoLuongDaXuat - viTri.SoLuongXuat;
                        await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(nhapKhoChiTietRevert);
                    }
                }
            }

            await BaseRepository.DeleteAsync(entity);
            await BaseRepository.Context.SaveChangesAsync();
        }

        public async Task<bool> IsValidateUpdateOrRemove(long id)
        {
            var result = true;
            //var entity = BaseRepository.TableNoTracking
            //    .Include(p => p.XuatKhoDuocPhamChiTiets).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTris)
            //    .FirstOrDefaultAsync(p => p.Id == id);
            var nhapKhoEntity = await _nhapKhoDuocPhamRepository.TableNoTracking
                                                .Include(p => p.NhapKhoDuocPhamChiTiets)
                                                .FirstOrDefaultAsync(p => p.XuatKhoDuocPhamId == id);
            if (nhapKhoEntity != null && nhapKhoEntity.NhapKhoDuocPhamChiTiets != null)
            {
                if (nhapKhoEntity.NhapKhoDuocPhamChiTiets.Any(p => p.SoLuongDaXuat != 0))
                {
                    result = false;
                }
            }
            return result;
        }

        public async Task DisabledAutoCommit()
        {
            BaseRepository.AutoCommitEnabled = false;
            _nhapKhoDuocPhamRepository.AutoCommitEnabled = false;
            _nhapKhoDuocPhamChiTietRepository.AutoCommitEnabled = false;
            _xuatKhoDuocPhamChiTietRepository.AutoCommitEnabled = false;
        }

        public async Task SaveChange()
        {
            await BaseRepository.Context.SaveChangesAsync();
        }

        public async Task<bool> IsKhoManagerment(long id)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var kho = await _khoNhanVienQuanLyRepository.TableNoTracking.FirstOrDefaultAsync(p => p.KhoId == id && p.NhanVienId == userCurrentId);
            return kho != null;
        }

        public async Task<bool> IsKhoExists(long id)
        {
            var kho = await _khoDuocPhamRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return kho != null;
        }

        public async Task<bool> IsKhoLeOrNhaThuoc(long id)
        {
            var kho = await _khoDuocPhamRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id);
            return (kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2);
        }

        public async Task<NhapKhoDuocPhamChiTiet> GetNhapKhoDuocPhamChiTietById(long id)
        {
            var result = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                .Include(p => p.KhoDuocPhamViTri)
                .FirstOrDefaultAsync(p => p.Id == id);
            return result;
        }


        public async Task<string> InPhieuXuat(long id, string hostingName)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatKhoDuocPham"));

            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == id)
                .Select(item => new ThongTinInXuatKhoDuocPhamVo()
                {
                    TenNguoiNhanHang = item.NguoiNhan.User.HoTen,
                    BoPhan = item.KhoDuocPhamNhap.PhongBenhVien != null
                            ? item.KhoDuocPhamNhap.PhongBenhVien.Ma + " - " + item.KhoDuocPhamNhap.PhongBenhVien.Ten
                            : (item.KhoDuocPhamNhap.KhoaPhong != null ? item.KhoDuocPhamNhap.KhoaPhong.Ma + " - " + item.KhoDuocPhamNhap.KhoaPhong.Ten : ""),
                    LyDoXuatKho = item.LyDoXuatKho,
                    XuatTaiKho = item.KhoDuocPhamXuat.Ten,
                    DiaDiem = "",
                    LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU XUẤT</th>" +
                         "</p>";

            var query = await _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking.Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == id && x.SoLuongXuat > 0)
                .Select(s => new ThongTinInXuatKhoDuocPhamChiTietVo
                {
                    DuocPhamBenhVienId = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ma = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.Ma,
                    TenThuoc = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.Ten,
                    //DVT = s.DuocPhamBenhVien.DuocPham.DonViTinhThamKhao ?? (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                    DVT = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    SLYeuCau = s.SoLuongXuat.ApplyNumber(),
                    SLThucXuat = s.SoLuongXuat.ApplyNumber(),
                    SLYC = s.SoLuongXuat,
                    SLTX = s.SoLuongXuat,
                    //LaDuocPhamBHYT = s.LaDuocPhamBHYT
                    TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    DuocPhamBenhVienPhanNhomId = s.XuatKhoDuocPhamChiTiet.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId
                }).OrderBy(z => z.TenThuoc)
                .ToListAsync();
            var duocPhamBenhVienPhanNhoms = await _duocPhamBenhVienPhanNhomRepository.TableNoTracking.ToListAsync();

            foreach (var item in query)
            {
                item.DuocPhamBenhVienPhanNhomChaId = CalculateHelper.GetDuocPhamBenhVienPhanNhomCha(item.DuocPhamBenhVienPhanNhomId.Value, duocPhamBenhVienPhanNhoms);
                item.TenNhom = _duocPhamBenhVienPhanNhomRepository.TableNoTracking.Where(p => p.Id == item.DuocPhamBenhVienPhanNhomChaId).Select(z => z.Ten).FirstOrDefault() ?? "CHƯA PHÂN NHÓM";
            }

            data.SoLuongThucXuatTong = query.Sum(p => p.SLTX).ApplyNumber();
            data.SoLuongYeuCauTong = query.Sum(p => p.SLYC).ApplyNumber();

            var totalTenNhom = query.Select(p => p.TenNhom).Distinct().ToList();

            var info = string.Empty;

            var STT = 1;
            foreach (var tenNhom in totalTenNhom)
            {
                var headerNhom = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + tenNhom.ToUpper()
                                        + "</b></tr>";
                info += headerNhom;
                var queryNhom = query.Where(p => p.TenNhom == tenNhom).ToList();
                foreach (var item in queryNhom)
                {
                    info = info
                                           + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                           + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                           + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucXuat
                                           + "</tr>";
                    STT++;
                }
            }

            data.Header = hearder;
            data.DanhSachThuoc = info;
            ;

            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        #region private class
        //Clone NhapKhoDuocPhamService
        private async Task UpdateNhapKhoDuocPhamChiTiet(long id, double soLuongDaXuat)
        {
            var nhapKhoChiTiet = await _nhapKhoDuocPhamChiTietRepository.Table.FirstOrDefaultAsync(p => p.Id == id);
            if (nhapKhoChiTiet != null)
            {
                nhapKhoChiTiet.SoLuongDaXuat = nhapKhoChiTiet.SoLuongDaXuat + soLuongDaXuat;
                await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(nhapKhoChiTiet);
            }
        }

        #endregion private class
    }
}