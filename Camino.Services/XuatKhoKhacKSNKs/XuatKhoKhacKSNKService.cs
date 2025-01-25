using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
//using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.XuatKhoKSNK;

using Camino.Data;
using Camino.Services.CauHinh;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using System.Globalization;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.Localization;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.XuatKhos;

namespace Camino.Services.XuatKhoKhacKSNKs
{
    [ScopedDependency(ServiceType = typeof(IXuatKhoKhacKSNKService))]
    public class XuatKhoKhacKSNKService : MasterFileService<XuatKhoVatTu>, IXuatKhoKhacKSNKService
    {
        private readonly IRepository<Kho> _khoRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<XuatKhoVatTuChiTiet> _xuatKhoVatTuChiTietRepository;
        private readonly IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;
        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<NhapKhoVatTu> _nhapKhoVatTuRepository;
        private readonly IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<YeuCauXuatKhoVatTu> _yeuCauXuatKhoVatTuRepository;
        private readonly IRepository<YeuCauXuatKhoDuocPham> _yeuCauXuatKhoDuocPhamRepository;
        private readonly IRepository<YeuCauXuatKhoVatTuChiTiet> _yeuCauXuatKhoVatTuChiTietRepository;
        private readonly IRepository<YeuCauNhapKhoVatTuChiTiet> _yeuCauNhapKhoVatTuChiTietRepository;
        private readonly IRepository<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTietRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;
        private readonly IRepository<Core.Domain.Entities.NhaThaus.NhaThau> _nhaThauRepository;
        private readonly IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;
        private readonly IRepository<XuatKhoDuocPham> _xuatKhoDuocPhamRepository;
        private readonly IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        private readonly IRepository<YeuCauXuatKhoDuocPhamChiTiet> _yeuCauXuatKhoDuocPhamChiTietRepository;


        public XuatKhoKhacKSNKService(IRepository<XuatKhoVatTu> repository, IRepository<Kho> khoRepository
            , IRepository<XuatKhoVatTuChiTiet> xuatKhoVatTuChiTietRepository, IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository
            , IRepository<NhapKhoVatTu> nhapKhoVatTuRepository, IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository
            , IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository
            , IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository, IRepository<Template> templateRepository
            , ICauHinhService cauHinhService, IUserAgentHelper userAgentHelper
            , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
            , IRepository<YeuCauXuatKhoVatTu> yeuCauXuatKhoVatTuRepository
            , IRepository<YeuCauXuatKhoDuocPham> yeuCauXuatKhoDuocPhamRepository
            , IRepository<XuatKhoVatTu> xuatKhoVatTuRepository
            , IRepository<XuatKhoDuocPham> xuatKhoDuocPhamRepository
            , IRepository<YeuCauXuatKhoVatTuChiTiet> yeuCauXuatKhoVatTuChiTietRepository
            , ILocalizationService localizationService
            , IRepository<LocaleStringResource> localeStringResourceRepository
            , IRepository<YeuCauNhapKhoDuocPhamChiTiet> yeuCauNhapKhoDuocPhamChiTietRepository
            , IRepository<YeuCauNhapKhoVatTuChiTiet> yeuCauNhapKhoVatTuChiTietRepository
            , IRepository<Core.Domain.Entities.NhaThaus.NhaThau> nhaThauRepository
            , IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository
            , IRepository<YeuCauXuatKhoDuocPhamChiTiet> yeuCauXuatKhoDuocPhamChiTietRepository
            , IRepository<VatTuBenhVien> vatTuBenhVienRepository) : base(repository)
        {
            _khoRepository = khoRepository;
            _nhanVienRepository = nhanVienRepository;
            _xuatKhoVatTuChiTietRepository = xuatKhoVatTuChiTietRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _templateRepository = templateRepository;
            _cauHinhService = cauHinhService;
            _phongBenhVienRepository = phongBenhVienRepository;
            _userAgentHelper = userAgentHelper;
            _yeuCauXuatKhoVatTuRepository = yeuCauXuatKhoVatTuRepository;
            _yeuCauXuatKhoDuocPhamRepository = yeuCauXuatKhoDuocPhamRepository;
            _yeuCauXuatKhoVatTuChiTietRepository = yeuCauXuatKhoVatTuChiTietRepository;
            _localizationService = localizationService;
            _localeStringResourceRepository = localeStringResourceRepository;
            _yeuCauNhapKhoDuocPhamChiTietRepository = yeuCauNhapKhoDuocPhamChiTietRepository;
            _yeuCauNhapKhoVatTuChiTietRepository = yeuCauNhapKhoVatTuChiTietRepository;
            _nhaThauRepository = nhaThauRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
            _yeuCauXuatKhoDuocPhamChiTietRepository = yeuCauXuatKhoDuocPhamChiTietRepository;
        }
        public async Task<GridDataSource> GetDataForGridAsyncDpVtKsnkDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            BuildDefaultSortExpression(queryInfo);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauXuatKhoVatTuChiTietVoSearch>(queryInfo.AdditionalSearchString);

            List<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>();
            if (info.LoaiDuocPhamVatTu == null || info.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham)
            {
                var nhapKhoDuocPhamChiTietQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPham).ThenInclude(dpbv => dpbv.DonViTinh)
                    .Include(nkct => nkct.DuocPhamBenhViens).ThenInclude(dpbv => dpbv.DuocPhamBenhVienPhanNhom)
                    .Include(nkct => nkct.NhapKhoDuocPhams)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat && x.NhapKhoDuocPhams.KhoId == info.KhoXuatId);
                if (!string.IsNullOrEmpty(info.SearchString))
                {
                    var searchTerms = info.SearchString.Replace("\t", "").Trim();
                    nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTietQuery.ApplyLike(searchTerms,
                        g => g.DuocPhamBenhViens.DuocPham.Ten,
                        g => g.DuocPhamBenhViens.Ma,
                        g => g.Solo
                    ).ToList();
                }
                else
                {
                    nhapKhoDuocPhamChiTiets = nhapKhoDuocPhamChiTietQuery.ToList();
                }
            }

            List<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>();
            if (info.LoaiDuocPhamVatTu == null || info.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiVatTu)
            {
                var nhapKhoVatTuChiTietQuery = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(dpbv => dpbv.VatTus).ThenInclude(dpbv => dpbv.NhomVatTu)
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat && x.NhapKhoVatTu.KhoId == info.KhoXuatId);
                if (!string.IsNullOrEmpty(info.SearchString))
                {
                    var searchTerms = info.SearchString.Replace("\t", "").Trim();
                    nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietQuery.ApplyLike(searchTerms,
                        g => g.VatTuBenhVien.VatTus.Ten,
                        g => g.VatTuBenhVien.Ma,
                        g => g.Solo
                    ).ToList();
                }
                else
                {
                    nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietQuery.ToList();
                }
            }

            var yeuCauDieuChuyenVatTuChiTietTheoKhoXuats = new List<YeuCauXuatKhoKSNKGridVo>();

            if (info.NhaThauId != null && !string.IsNullOrEmpty(info.SoChungTu))
            {
                var yeuCauNhapKhoDuocPhamChiTiets = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(z => z.YeuCauNhapKhoDuocPham.SoChungTu.Equals(info.SoChungTu)).ToList();
                var nhapKhoDPCTs = new List<NhapKhoDuocPhamChiTiet>();
                foreach (var item in yeuCauNhapKhoDuocPhamChiTiets)
                {
                    foreach (var nhapKhoDuocPhamChiTiet in nhapKhoDuocPhamChiTiets)
                    {
                        if (item.DuocPhamBenhVienId == nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId
                         && item.LaDuocPhamBHYT == nhapKhoDuocPhamChiTiet.LaDuocPhamBHYT
                         && item.HopDongThauDuocPhamId == nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId
                         && item.Solo == nhapKhoDuocPhamChiTiet.Solo
                         && item.HanSuDung == nhapKhoDuocPhamChiTiet.HanSuDung
                         && item.TiLeTheoThapGia == nhapKhoDuocPhamChiTiet.TiLeTheoThapGia
                         && item.VAT == nhapKhoDuocPhamChiTiet.VAT
                         && item.MaVach == nhapKhoDuocPhamChiTiet.MaVach
                         && item.MaRef == nhapKhoDuocPhamChiTiet.MaRef)
                        {
                            nhapKhoDPCTs.Add(nhapKhoDuocPhamChiTiet);
                        }
                    }
                }

                var nhapKhoDuocPhamChiTietGroup = nhapKhoDPCTs.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Solo, x.HanSuDung });

                foreach (var item in nhapKhoDuocPhamChiTietGroup)
                {
                    var slTon = item.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    var yeuCauXuatKhoDuocPhamGridVo = new YeuCauXuatKhoKSNKGridVo
                    {
                        Id = item.First().Id,
                        LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                        VatTuBenhVienId = item.Key.DuocPhamBenhVienId,
                        Ten = item.First().DuocPhamBenhViens.DuocPham.Ten,
                        DVT = item.First().DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        LaVatTuBHYT = item.Key.LaDuocPhamBHYT,
                        NhomVatTuId = item.First().DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                        TenNhom = item.First().DuocPhamBenhViens.DuocPhamBenhVienPhanNhom?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.First().DuocPhamBenhViens.Ma,
                        SoLo = item.Key.Solo,
                        HanSuDung = item.Key.HanSuDung,
                        DonGiaNhap = item.First().DonGiaNhap,
                        KhoXuatId = item.First().NhapKhoDuocPhams.KhoId,
                        SoLuongTon = slTon,
                        SoLuongXuat = slTon
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoDuocPhamGridVo);
                }

                var yeuCauNhapKhoVatTuChiTiets = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking.Where(z => z.YeuCauNhapKhoVatTu.SoChungTu.Equals(info.SoChungTu)).ToList();
                var nhapKhoVTCTs = new List<NhapKhoVatTuChiTiet>();
                foreach (var item in yeuCauNhapKhoVatTuChiTiets)
                {
                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                    {
                        if (item.VatTuBenhVienId == nhapKhoVatTuChiTiet.VatTuBenhVienId
                         && item.LaVatTuBHYT == nhapKhoVatTuChiTiet.LaVatTuBHYT
                         && item.HopDongThauVatTuId == nhapKhoVatTuChiTiet.HopDongThauVatTuId
                         && item.Solo == nhapKhoVatTuChiTiet.Solo
                         && item.HanSuDung == nhapKhoVatTuChiTiet.HanSuDung
                         && item.TiLeTheoThapGia == nhapKhoVatTuChiTiet.TiLeTheoThapGia
                         && item.VAT == nhapKhoVatTuChiTiet.VAT
                         && item.MaVach == nhapKhoVatTuChiTiet.MaVach
                         && item.MaRef == nhapKhoVatTuChiTiet.MaRef)
                        {
                            nhapKhoVTCTs.Add(nhapKhoVatTuChiTiet);
                        }
                    }
                }

                var nhapKhoVatTuChiTietGroup = nhapKhoVTCTs.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Solo, x.HanSuDung });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var slTon = item.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoKSNKGridVo
                    {
                        Id = item.First().Id,
                        LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                        VatTuBenhVienId = item.Key.VatTuBenhVienId,
                        Ten = item.First().VatTuBenhVien.VatTus.Ten,
                        DVT = item.First().VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.Key.LaVatTuBHYT,
                        NhomVatTuId = item.First().VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.First().VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.First().VatTuBenhVien.Ma,
                        SoLo = item.Key.Solo,
                        HanSuDung = item.Key.HanSuDung,
                        DonGiaNhap = item.First().DonGiaNhap,
                        KhoXuatId = item.First().NhapKhoVatTu.KhoId,
                        SoLuongTon = slTon,
                        SoLuongXuat = slTon
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }
            else
            {
                var nhapKhoDuocPhamChiTietGroup = nhapKhoDuocPhamChiTiets.GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.Solo, x.HanSuDung });

                foreach (var item in nhapKhoDuocPhamChiTietGroup)
                {
                    var slTon = item.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    var yeuCauXuatKhoDuocPhamGridVo = new YeuCauXuatKhoKSNKGridVo
                    {
                        Id = item.First().Id,
                        LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                        VatTuBenhVienId = item.Key.DuocPhamBenhVienId,
                        Ten = item.First().DuocPhamBenhViens.DuocPham.Ten,
                        DVT = item.First().DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        LaVatTuBHYT = item.Key.LaDuocPhamBHYT,
                        NhomVatTuId = item.First().DuocPhamBenhViens.DuocPhamBenhVienPhanNhomId,
                        TenNhom = item.First().DuocPhamBenhViens.DuocPhamBenhVienPhanNhom?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.First().DuocPhamBenhViens.Ma,
                        SoLo = item.Key.Solo,
                        HanSuDung = item.Key.HanSuDung,
                        DonGiaNhap = item.First().DonGiaNhap,
                        KhoXuatId = item.First().NhapKhoDuocPhams.KhoId,
                        SoLuongTon = slTon,
                        SoLuongXuat = slTon
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoDuocPhamGridVo);
                }

                var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Solo, x.HanSuDung });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var slTon = item.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoKSNKGridVo
                    {
                        Id = item.First().Id,
                        LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                        VatTuBenhVienId = item.Key.VatTuBenhVienId,
                        Ten = item.First().VatTuBenhVien.VatTus.Ten,
                        DVT = item.First().VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.Key.LaVatTuBHYT,
                        NhomVatTuId = item.First().VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.First().VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.First().VatTuBenhVien.Ma,
                        SoLo = item.Key.Solo,
                        HanSuDung = item.Key.HanSuDung,
                        DonGiaNhap = item.First().DonGiaNhap,
                        KhoXuatId = item.First().NhapKhoVatTu.KhoId,
                        SoLuongTon = slTon,
                        SoLuongXuat = slTon
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }

            //var vatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == info.KhoXuatId
            //             && x.SoLuongDaXuat < x.SoLuongNhap
            //             //&& x.HanSuDung >= DateTime.Now
            //             ).ToList();

            //var result = yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Select(o =>
            //{
            //    o.SoLuongTon = vatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
            //    o.SoLuongXuat = vatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
            //    return o;
            //});
            var dataReturn = yeuCauDieuChuyenVatTuChiTietTheoKhoXuats;
            if (info.VatTuBenhViens.Any())
            {
                dataReturn = yeuCauDieuChuyenVatTuChiTietTheoKhoXuats
                    .Where(x => !info.VatTuBenhViens.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung))
                    .ToList();
            }
            //var query = result.AsQueryable();
            //var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = dataReturn.OrderBy(o => o.Ma).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = dataReturn.Count };
        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                 .Where(p => p.LoaiXuatKho == Enums.EnumLoaiXuatKho.XuatKhac && p.KhoVatTuXuat.KhoaPhongId == phongBenhVien.KhoaPhongId && p.KhoVatTuXuat.LaKhoKSNK == true)
                .Select(s => new XuatKhoVatTuKhacGridVo
                {
                    Id = s.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                    KhoVatTuXuat = s.KhoVatTuXuat.Ten,
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    TenNguoiNhan = s.NguoiNhan != null ? s.NguoiNhan.User.HoTen : "",
                    TenNguoiXuat = s.NguoiXuat.User.HoTen,
                    NgayXuat = s.NgayXuat,
                    DuocDuyet = true,
                    TraNCC = s.TraNCC
                }).Union(_xuatKhoDuocPhamRepository.TableNoTracking
                 .Where(p => p.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatKhac && p.KhoDuocPhamXuat.KhoaPhongId == phongBenhVien.KhoaPhongId && p.KhoDuocPhamXuat.LaKhoKSNK == true)
                .Select(s => new XuatKhoVatTuKhacGridVo
                {
                    Id = s.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    KhoVatTuXuat = s.KhoDuocPhamXuat.Ten,
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    TenNguoiNhan = s.NguoiNhan != null ? s.NguoiNhan.User.HoTen : "",
                    TenNguoiXuat = s.NguoiXuat.User.HoTen,
                    NgayXuat = s.NgayXuat,
                    DuocDuyet = true,
                    TraNCC = s.TraNCC
                }))
                .Union(
                _yeuCauXuatKhoVatTuRepository
                .TableNoTracking
                 .Where(p => p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId && p.KhoXuat.LaKhoKSNK == true)
                .Select(z => new XuatKhoVatTuKhacGridVo
                {
                    Id = z.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                    KhoVatTuXuat = z.KhoXuat.Ten,
                    LyDoXuatKho = z.LyDoXuatKho,
                    TenNguoiNhan = z.NguoiNhan != null ? z.NguoiNhan.User.HoTen : "",
                    TenNguoiXuat = z.NguoiXuat.User.HoTen,
                    NgayXuat = z.NgayXuat,
                    DuocDuyet = null,
                    TraNCC = z.TraNCC
                }))
                .Union(
                _yeuCauXuatKhoDuocPhamRepository
                .TableNoTracking
                 .Where(p => p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId && p.KhoXuat.LaKhoKSNK == true)
                .Select(z => new XuatKhoVatTuKhacGridVo
                {
                    Id = z.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    KhoVatTuXuat = z.KhoXuat.Ten,
                    LyDoXuatKho = z.LyDoXuatKho,
                    TenNguoiNhan = z.NguoiNhan != null ? z.NguoiNhan.User.HoTen : "",
                    TenNguoiXuat = z.NguoiXuat.User.HoTen,
                    NgayXuat = z.NgayXuat,
                    DuocDuyet = null,
                    TraNCC = z.TraNCC
                }));

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<XuatKhoVatTuKhacGridVo>(queryInfo.AdditionalSearchString);

                if (queryString.ChoDuyet == false && queryString.DaDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0);
                }

                if (queryString.RangeFromDate != null &&
                            (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayXuat <= denNgay));
                }

                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.KhoVatTuXuat,
                         g => g.LyDoXuatKho
                   );
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
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var query = BaseRepository.TableNoTracking
                 .Where(p => p.LoaiXuatKho == Enums.EnumLoaiXuatKho.XuatKhac && p.KhoVatTuXuat.KhoaPhongId == phongBenhVien.KhoaPhongId && p.KhoVatTuXuat.LaKhoKSNK == true)
                .Select(s => new XuatKhoVatTuKhacGridVo
                {
                    Id = s.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                    KhoVatTuXuat = s.KhoVatTuXuat.Ten,
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NgayXuat = s.NgayXuat,
                    DuocDuyet = true
                }).Union(_xuatKhoDuocPhamRepository.TableNoTracking
                 .Where(p => p.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatKhac && p.KhoDuocPhamXuat.KhoaPhongId == phongBenhVien.KhoaPhongId && p.KhoDuocPhamXuat.LaKhoKSNK == true)
                .Select(s => new XuatKhoVatTuKhacGridVo
                {
                    Id = s.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    KhoVatTuXuat = s.KhoDuocPhamXuat.Ten,
                    SoPhieu = s.SoPhieu,
                    LyDoXuatKho = s.LyDoXuatKho,
                    NgayXuat = s.NgayXuat,
                    DuocDuyet = true
                }))
                .Union(
                _yeuCauXuatKhoVatTuRepository
                .TableNoTracking
                 .Where(p => p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId && p.KhoXuat.LaKhoKSNK == true)
                .Select(z => new XuatKhoVatTuKhacGridVo
                {
                    Id = z.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,
                    KhoVatTuXuat = z.KhoXuat.Ten,
                    LyDoXuatKho = z.LyDoXuatKho,
                    NgayXuat = z.NgayXuat,
                    DuocDuyet = null
                }))
                .Union(
                _yeuCauXuatKhoDuocPhamRepository
                .TableNoTracking
                 .Where(p => p.KhoXuat.KhoaPhongId == phongBenhVien.KhoaPhongId && p.KhoXuat.LaKhoKSNK == true)
                .Select(z => new XuatKhoVatTuKhacGridVo
                {
                    Id = z.Id,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                    KhoVatTuXuat = z.KhoXuat.Ten,
                    LyDoXuatKho = z.LyDoXuatKho,
                    NgayXuat = z.NgayXuat,
                    DuocDuyet = null
                }));

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<XuatKhoVatTuKhacGridVo>(queryInfo.AdditionalSearchString);

                if (queryString.ChoDuyet == false && queryString.DaDuyet == true)
                {
                    query = query.Where(p => p.TinhTrang == 1);
                }
                else if (queryString.ChoDuyet == true && queryString.DaDuyet == false)
                {
                    query = query.Where(p => p.TinhTrang == 0);
                }

                if (queryString.RangeFromDate != null &&
                            (!string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || !string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay)))
                {
                    DateTime.TryParseExact(queryString.RangeFromDate.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                    DateTime.TryParseExact(queryString.RangeFromDate.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
                    query = query.Where(p => (string.IsNullOrEmpty(queryString.RangeFromDate.TuNgay) || p.NgayXuat >= tuNgay)
                                             && (string.IsNullOrEmpty(queryString.RangeFromDate.DenNgay) || p.NgayXuat <= denNgay));
                }

                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms,
                         g => g.SoPhieu,
                         g => g.KhoVatTuXuat,
                         g => g.LyDoXuatKho
                   );
                }
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridVatTuChildAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var xuatKhoVatTuId = long.Parse(queryObj[0]);
            var tinhTrang = long.Parse(queryObj[1]);
            IQueryable<YeuCauXuatKhoKSNKGridVo> query;
            if (tinhTrang == 0)
            {
                query = _yeuCauXuatKhoVatTuChiTietRepository.TableNoTracking
               .Where(x => x.YeuCauXuatKhoVatTuId == xuatKhoVatTuId)
               .Select(s => new YeuCauXuatKhoKSNKGridVo()
               {
                   Id = s.Id,
                   Ten = s.VatTuBenhVien.VatTus.Ten,
                   DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                   TenNhom = s.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.VatTuBenhVien.Ma,
                   SoLo = s.Solo,
                   SoLuongXuat = s.SoLuongXuat,
                   HanSuDung = s.HanSuDung,
               });
            }
            else
            {
                query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
               .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == xuatKhoVatTuId)
               .Select(s => new YeuCauXuatKhoKSNKGridVo()
               {
                   Id = s.Id,
                   Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                   DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                   TenNhom = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                   SoLo = s.NhapKhoVatTuChiTiet.Solo,
                   HanSuDung = s.NhapKhoVatTuChiTiet.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,
                   SoPhieu = s.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
               });
            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridVatTuChildAsync(QueryInfo queryInfo)
        {
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var xuatKhoVatTuId = long.Parse(queryObj[0]);
            var tinhTrang = long.Parse(queryObj[1]);
            IQueryable<YeuCauXuatKhoKSNKGridVo> query;
            if (tinhTrang == 0)
            {
                query = _yeuCauXuatKhoVatTuChiTietRepository.TableNoTracking
               .Where(x => x.YeuCauXuatKhoVatTuId == xuatKhoVatTuId)
               .Select(s => new YeuCauXuatKhoKSNKGridVo()
               {
                   Id = s.Id,
                   Ten = s.VatTuBenhVien.VatTus.Ten,
                   DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                   TenNhom = s.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.VatTuBenhVien.Ma,
                   SoLo = s.Solo,
                   SoLuongXuat = s.SoLuongXuat,
                   HanSuDung = s.HanSuDung,
               });
            }
            else
            {
                query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
               .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == xuatKhoVatTuId)
               .Select(s => new YeuCauXuatKhoKSNKGridVo()
               {
                   Id = s.Id,
                   Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                   DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                   TenNhom = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                   SoLo = s.NhapKhoVatTuChiTiet.Solo,
                   HanSuDung = s.NhapKhoVatTuChiTiet.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,
                   SoPhieu = s.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
               });
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridDuocPhamChildAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var xuatKhoDuocPhamId = long.Parse(queryObj[0]);
            var tinhTrang = long.Parse(queryObj[1]);
            IQueryable<YeuCauXuatKhoKSNKGridVo> query;
            if (tinhTrang == 0)
            {
                query = _yeuCauXuatKhoDuocPhamChiTietRepository.TableNoTracking
               .Where(x => x.YeuCauXuatKhoDuocPhamId == xuatKhoDuocPhamId)
               .Select(s => new YeuCauXuatKhoKSNKGridVo()
               {
                   Id = s.Id,
                   Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                   DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                   TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.DuocPhamBenhVien.Ma,
                   SoDangKy = s.DuocPhamBenhVien.DuocPham.SoDangKy,
                   HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                   SoLo = s.Solo,
                   SoLuongXuat = s.SoLuongXuat,
                   HanSuDung = s.HanSuDung,
               });
            }
            else
            {
                query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
               .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == xuatKhoDuocPhamId)
               .Select(s => new YeuCauXuatKhoKSNKGridVo()
               {
                   Id = s.Id,
                   Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                   DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                   TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                   SoDangKy = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.SoDangKy,
                   HamLuong = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                   SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
                   HanSuDung = s.NhapKhoDuocPhamChiTiet.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,
                   SoPhieu = s.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
               });
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridDuocPhamChildAsync(QueryInfo queryInfo)
        {
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var xuatKhoDuocPhamId = long.Parse(queryObj[0]);
            var tinhTrang = long.Parse(queryObj[1]);
            IQueryable<YeuCauXuatKhoKSNKGridVo> query;
            if (tinhTrang == 0)
            {
                query = _yeuCauXuatKhoDuocPhamChiTietRepository.TableNoTracking
               .Where(x => x.YeuCauXuatKhoDuocPhamId == xuatKhoDuocPhamId)
               .Select(s => new YeuCauXuatKhoKSNKGridVo()
               {
                   Id = s.Id,
                   Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                   DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                   TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.DuocPhamBenhVien.Ma,
                   SoDangKy = s.DuocPhamBenhVien.DuocPham.SoDangKy,
                   HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                   SoLo = s.Solo,
                   HanSuDung = s.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,

               });
            }
            else
            {
                query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
               .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == xuatKhoDuocPhamId)
               .Select(s => new YeuCauXuatKhoKSNKGridVo()
               {
                   Id = s.Id,
                   Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                   DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                   TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                   Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                   SoDangKy = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.SoDangKy,
                   HamLuong = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
                   SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
                   HanSuDung = s.NhapKhoDuocPhamChiTiet.HanSuDung,
                   SoLuongXuat = s.SoLuongXuat,
                   SoPhieu = s.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu
               });
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<GridDataSource> GetDataForGridChildAsyncDaDuyet(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<YeuCauXuatKhoKSNKGridVo>(queryInfo.AdditionalSearchString);
            var query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
           .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == queryString.XuatKhoVatTuId)
           .Select(s => new YeuCauXuatKhoKSNKGridVo()
           {
               Id = s.Id,
               Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
               DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
               TenNhom = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
               Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
               SoLo = s.NhapKhoVatTuChiTiet.Solo,
               HanSuDung = s.NhapKhoVatTuChiTiet.HanSuDung,
               SoLuongXuat = s.SoLuongXuat,
               SoPhieu = s.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
           });
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.DVT,
                     g => g.Ten,
                     g => g.Ma,
                     g => g.SoLo,
                     g => g.SoPhieu
               );
            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyet(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<YeuCauXuatKhoKSNKGridVo>(queryInfo.AdditionalSearchString);
            var query = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
           .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == queryString.XuatKhoVatTuId)
           .Select(s => new YeuCauXuatKhoKSNKGridVo()
           {
               Id = s.Id,
               Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
               DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
               TenNhom = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
               Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
               SoLo = s.NhapKhoVatTuChiTiet.Solo,
               HanSuDung = s.NhapKhoVatTuChiTiet.HanSuDung,
               SoLuongXuat = s.SoLuongXuat,
               SoPhieu = s.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
           });
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.DVT,
                     g => g.Ten,
                     g => g.Ma,
                     g => g.SoLo,
                     g => g.SoPhieu
               );
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridAsyncVatTuDaChon(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            BuildDefaultSortExpression(queryInfo);
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauXuatKhoVatTuChiTietVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(dpbv => dpbv.VatTus).ThenInclude(dpbv => dpbv.NhomVatTu)
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoVatTu.Kho.Id == info.KhoXuatId);

            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoVatTuChiTiets = nhapKhoVatTuChiTiets.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten,
                    g => g.VatTuBenhVien.Ma,
                    g => g.VatTuBenhVien.VatTus.DonViTinh,
                    g => g.Solo
               );
            }
            var yeuCauDieuChuyenVatTuChiTietTheoKhoXuats = new List<YeuCauXuatKhoKSNKGridVo>();

            if (info.NhaThauId != null && !string.IsNullOrEmpty(info.SoChungTu))
            {
                var yeuCauNhapKhoVatTuChiTiets = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking.Where(z => z.YeuCauNhapKhoVatTu.SoChungTu.Equals(info.SoChungTu)).ToList();
                var nhapKhoVTCTs = new List<NhapKhoVatTuChiTiet>();
                foreach (var item in yeuCauNhapKhoVatTuChiTiets)
                {
                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                    {
                        if (item.VatTuBenhVienId == nhapKhoVatTuChiTiet.VatTuBenhVienId
                         && item.LaVatTuBHYT == nhapKhoVatTuChiTiet.LaVatTuBHYT
                         && item.HopDongThauVatTuId == nhapKhoVatTuChiTiet.HopDongThauVatTuId
                         && item.Solo == nhapKhoVatTuChiTiet.Solo
                         && item.HanSuDung == nhapKhoVatTuChiTiet.HanSuDung
                         && item.TiLeTheoThapGia == nhapKhoVatTuChiTiet.TiLeTheoThapGia
                         && item.VAT == nhapKhoVatTuChiTiet.VAT
                         && item.MaVach == nhapKhoVatTuChiTiet.MaVach
                         && item.MaRef == nhapKhoVatTuChiTiet.MaRef)
                        {
                            nhapKhoVTCTs.Add(nhapKhoVatTuChiTiet);
                        }
                    }
                }

                var nhapKhoVatTuChiTietGroup = nhapKhoVTCTs.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh })
                                                .Select(g => new { nhapKhoVatTuChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoKSNKGridVo
                    {
                        Id = item.nhapKhoVatTuChiTiets.Id,
                        VatTuBenhVienId = item.nhapKhoVatTuChiTiets.VatTuBenhVienId,
                        Ten = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.Ten,
                        DVT = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.nhapKhoVatTuChiTiets.LaVatTuBHYT,
                        NhomVatTuId = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.nhapKhoVatTuChiTiets.VatTuBenhVien.Ma,
                        SoLo = item.nhapKhoVatTuChiTiets.Solo,
                        HanSuDung = item.nhapKhoVatTuChiTiets.HanSuDung,
                        DonGiaNhap = item.nhapKhoVatTuChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoVatTuChiTiets.NhapKhoVatTu.KhoId
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }
            else
            {
                var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh })
                                                .Select(g => new { nhapKhoVatTuChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoKSNKGridVo
                    {
                        Id = item.nhapKhoVatTuChiTiets.Id,
                        VatTuBenhVienId = item.nhapKhoVatTuChiTiets.VatTuBenhVienId,
                        Ten = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.Ten,
                        DVT = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.nhapKhoVatTuChiTiets.LaVatTuBHYT,
                        NhomVatTuId = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.nhapKhoVatTuChiTiets.VatTuBenhVien.Ma,
                        SoLo = item.nhapKhoVatTuChiTiets.Solo,
                        HanSuDung = item.nhapKhoVatTuChiTiets.HanSuDung,
                        DonGiaNhap = item.nhapKhoVatTuChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoVatTuChiTiets.NhapKhoVatTu.KhoId
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }

            var vatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => x.NhapKhoVatTu.KhoId == info.KhoXuatId
                         && x.SoLuongDaXuat < x.SoLuongNhap
                         //&& x.HanSuDung >= DateTime.Now
                         ).ToList();

            var result = yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Select(o =>
            {
                o.SoLuongTon = vatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                o.SoLuongXuat = vatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            });
            if (info.VatTuBenhViens.Any())
            {
                result = result.Where(x => !info.VatTuBenhViens.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung));
            }
            var query = result.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncVatTuDaChon(QueryInfo queryInfo)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var info = JsonConvert.DeserializeObject<YeuCauXuatKhoVatTuChiTietVoSearch>(queryInfo.AdditionalSearchString);
            var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                    .Include(nkct => nkct.VatTuBenhVien).ThenInclude(dpbv => dpbv.VatTus).ThenInclude(dpbv => dpbv.NhomVatTu)
                    .Include(nkct => nkct.NhapKhoVatTu)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                    .Where(x => x.SoLuongNhap > x.SoLuongDaXuat
                    //&& x.HanSuDung >= DateTime.Now 
                    && x.NhapKhoVatTu.Kho.Id == info.KhoXuatId);

            if (!string.IsNullOrEmpty(info.SearchString))
            {
                var searchTerms = info.SearchString.Replace("\t", "").Trim();
                nhapKhoVatTuChiTiets = nhapKhoVatTuChiTiets.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten,
                    g => g.VatTuBenhVien.Ma,
                    g => g.VatTuBenhVien.VatTus.DonViTinh,
                    g => g.Solo
               );
            }
            var yeuCauDieuChuyenVatTuChiTietTheoKhoXuats = new List<YeuCauXuatKhoKSNKGridVo>();

            if (info.NhaThauId != null && !string.IsNullOrEmpty(info.SoChungTu))
            {
                var yeuCauNhapKhoVatTuChiTiets = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking.Where(z => z.YeuCauNhapKhoVatTu.SoChungTu.Equals(info.SoChungTu)).ToList();
                var nhapKhoVTCTs = new List<NhapKhoVatTuChiTiet>();
                foreach (var item in yeuCauNhapKhoVatTuChiTiets)
                {
                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                    {
                        if (item.VatTuBenhVienId == nhapKhoVatTuChiTiet.VatTuBenhVienId
                         && item.LaVatTuBHYT == nhapKhoVatTuChiTiet.LaVatTuBHYT
                         && item.HopDongThauVatTuId == nhapKhoVatTuChiTiet.HopDongThauVatTuId
                         && item.Solo == nhapKhoVatTuChiTiet.Solo
                         && item.HanSuDung == nhapKhoVatTuChiTiet.HanSuDung
                         && item.TiLeTheoThapGia == nhapKhoVatTuChiTiet.TiLeTheoThapGia
                         && item.VAT == nhapKhoVatTuChiTiet.VAT
                         && item.MaVach == nhapKhoVatTuChiTiet.MaVach
                         && item.MaRef == nhapKhoVatTuChiTiet.MaRef)
                        {
                            nhapKhoVTCTs.Add(nhapKhoVatTuChiTiet);
                        }
                    }
                }

                var nhapKhoVatTuChiTietGroup = nhapKhoVTCTs.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh })
                                                .Select(g => new { nhapKhoVatTuChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoKSNKGridVo
                    {
                        Id = item.nhapKhoVatTuChiTiets.Id,
                        VatTuBenhVienId = item.nhapKhoVatTuChiTiets.VatTuBenhVienId,
                        Ten = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.Ten,
                        DVT = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.nhapKhoVatTuChiTiets.LaVatTuBHYT,
                        NhomVatTuId = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.nhapKhoVatTuChiTiets.VatTuBenhVien.Ma,
                        SoLo = item.nhapKhoVatTuChiTiets.Solo,
                        HanSuDung = item.nhapKhoVatTuChiTiets.HanSuDung,
                        DonGiaNhap = item.nhapKhoVatTuChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoVatTuChiTiets.NhapKhoVatTu.KhoId
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }
            else
            {
                var nhapKhoVatTuChiTietGroup = nhapKhoVatTuChiTiets.GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.VatTuBenhVien.Ma, x.VatTuBenhVien.VatTus.Ten, x.Solo, x.HanSuDung, x.VatTuBenhVien.VatTus.DonViTinh })
                                                .Select(g => new { nhapKhoVatTuChiTiets = g.FirstOrDefault() });

                foreach (var item in nhapKhoVatTuChiTietGroup)
                {
                    var yeuCauXuatKhoVatTuGridVo = new YeuCauXuatKhoKSNKGridVo
                    {
                        Id = item.nhapKhoVatTuChiTiets.Id,
                        VatTuBenhVienId = item.nhapKhoVatTuChiTiets.VatTuBenhVienId,
                        Ten = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.Ten,
                        DVT = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.DonViTinh,
                        LaVatTuBHYT = item.nhapKhoVatTuChiTiets.LaVatTuBHYT,
                        NhomVatTuId = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTuId,
                        TenNhom = item.nhapKhoVatTuChiTiets.VatTuBenhVien.VatTus.NhomVatTu?.Ten ?? "CHƯA PHÂN NHÓM",
                        Ma = item.nhapKhoVatTuChiTiets.VatTuBenhVien.Ma,
                        SoLo = item.nhapKhoVatTuChiTiets.Solo,
                        HanSuDung = item.nhapKhoVatTuChiTiets.HanSuDung,
                        DonGiaNhap = item.nhapKhoVatTuChiTiets.DonGiaNhap,
                        KhoXuatId = item.nhapKhoVatTuChiTiets.NhapKhoVatTu.KhoId
                    };
                    yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Add(yeuCauXuatKhoVatTuGridVo);
                }
            }

            if (info.VatTuBenhViens.Any())
            {
                yeuCauDieuChuyenVatTuChiTietTheoKhoXuats = yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.Where(x => !info.VatTuBenhViens.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT && z.Ten.Trim() == x.Ten.Trim() && z.Ma.Trim() == x.Ma.Trim() && z.SoLo.Trim() == x.SoLo.Trim() && z.HanSuDung == x.HanSuDung)).ToList();
            }
            var query = yeuCauDieuChuyenVatTuChiTietTheoKhoXuats.AsQueryable();
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<long?> GetNhapKhoVatTuIdBy(string soChungTu)
        {
            return await _nhapKhoVatTuRepository.TableNoTracking.Where(z => z.SoChungTu.Equals(soChungTu)).Select(z => z.Id).FirstOrDefaultAsync();
        }

        public async Task<List<LookupItemVo>> GetSoHoaDonTheoKhoKSNK(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.ParameterDependencies))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<SoCTTheoKhoDuocPhamJsonVo>(model.ParameterDependencies);

            var nhapDuocPhams = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(o => o.HopDongThauDuocPham.NhaThauId == info.NhaThauId && o.YeuCauNhapKhoDuocPham.DuocKeToanDuyet == true && !string.IsNullOrEmpty(o.YeuCauNhapKhoDuocPham.SoChungTu))
                .Select(o => new { o.YeuCauNhapKhoDuocPhamId, o.YeuCauNhapKhoDuocPham.SoChungTu }).Distinct().ToList();
            var nhapVatTus = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking
                .Where(o => o.HopDongThauVatTu.NhaThauId == info.NhaThauId && o.YeuCauNhapKhoVatTu.DuocKeToanDuyet == true && !string.IsNullOrEmpty(o.YeuCauNhapKhoVatTu.SoChungTu))
                .Select(o => new { o.YeuCauNhapKhoVatTuId, o.YeuCauNhapKhoVatTu.SoChungTu }).Distinct().ToList();

            var results = new List<LookupItemVo>();
            foreach (var nhapDuocPham in nhapDuocPhams)
            {
                if (results.All(o => o.DisplayName != nhapDuocPham.SoChungTu) && (string.IsNullOrEmpty(model.Query) || nhapDuocPham.SoChungTu.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())))
                {
                    results.Add(new LookupItemVo
                    {
                        KeyId = nhapDuocPham.YeuCauNhapKhoDuocPhamId,
                        DisplayName = nhapDuocPham.SoChungTu
                    });
                }
            }
            foreach (var nhapVatTu in nhapVatTus)
            {
                if (results.All(o => o.DisplayName != nhapVatTu.SoChungTu) && (string.IsNullOrEmpty(model.Query) || nhapVatTu.SoChungTu.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())))
                {
                    results.Add(new LookupItemVo
                    {
                        KeyId = nhapVatTu.YeuCauNhapKhoVatTuId,
                        DisplayName = nhapVatTu.SoChungTu
                    });
                }
            }

            return results.Take(model.Take).ToList();
        }

        public async Task<List<LookupItemVo>> GetSoHoaDonTheoKhoVatTus(DropDownListRequestModel model)
        {
            if (string.IsNullOrEmpty(model.ParameterDependencies))
            {
                return null;
            }
            var info = JsonConvert.DeserializeObject<SoCTTheoKhoDuocPhamJsonVo>(model.ParameterDependencies);
            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Include(cc => cc.NhapKhoVatTu)
                        .Where(z =>
                        //z.NhapKhoVatTu.Kho.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap1 && 
                        z.NhapKhoVatTu.Kho.LoaiVatTu == true &&
                        z.HopDongThauVatTu.NhaThauId == info.NhaThauId
                        && !string.IsNullOrEmpty(z.NhapKhoVatTu.SoChungTu))
                        .GroupBy(x => new { x.NhapKhoVatTu.SoChungTu }).Select(g => new { NhapKhoVTChiTiets = g.FirstOrDefault() });
            var results = new List<LookupItemVo>();
            foreach (var item in query)
            {
                var result = new LookupItemVo
                {
                    KeyId = item.NhapKhoVTChiTiets.NhapKhoVatTuId,
                    DisplayName = item.NhapKhoVTChiTiets.NhapKhoVatTu.SoChungTu
                };
                results.Add(result);
            }
            results = results.Take(model.Take).ToList();
            if (!string.IsNullOrEmpty(model.Query))
            {
                results = results.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return results;
        }

        public async Task<List<YeuCauXuatKhoKSNKGridVo>> YeuCauXuatVatTuChiTiets(long yeuCauXuatKhoVatTuId)
        {
            var query = _yeuCauXuatKhoVatTuChiTietRepository.TableNoTracking.Where(z => z.YeuCauXuatKhoVatTuId == yeuCauXuatKhoVatTuId)
                .Select(s => new YeuCauXuatKhoKSNKGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ten = s.VatTuBenhVien.VatTus.Ten,
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    NhomVatTuId = s.VatTuBenhVien.VatTus.NhomVatTuId,
                    TenNhom = s.VatTuBenhVien.VatTus.NhomVatTu.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = s.VatTuBenhVien.Ma,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    DonGiaNhap = s.DonGiaNhap,
                    KhoXuatId = s.YeuCauXuatKhoVatTu.KhoXuatId,
                    SoLuongXuat = s.SoLuongXuat,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiVatTu,

                })
                .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new YeuCauXuatKhoKSNKGridVo
                {
                    Id = g.First().Id,
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    NhomVatTuId = g.First().NhomVatTuId,
                    TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    KhoXuatId = g.First().KhoXuatId,
                    SoLuongXuat = g.Sum(z => z.SoLuongXuat),
                    LoaiDuocPhamVatTu = g.First().LoaiDuocPhamVatTu,
                });

            var yeuCauXuatPhamBuGridParentVos = query.ToList();

            var lstVatTuBenhVien = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(x => yeuCauXuatPhamBuGridParentVos.Any(z => z.VatTuBenhVienId == x.VatTuBenhVienId && z.LaVatTuBHYT == x.LaVatTuBHYT && x.NhapKhoVatTu.KhoId == z.KhoXuatId && x.HanSuDung == z.HanSuDung && x.Solo == z.SoLo) && x.SoLuongNhap > x.SoLuongDaXuat
            //&& x.HanSuDung >= DateTime.Now
            ).ToList();

            var result = yeuCauXuatPhamBuGridParentVos.Where(p => lstVatTuBenhVien.Any(o => o.VatTuBenhVienId == p.VatTuBenhVienId && o.LaVatTuBHYT == p.LaVatTuBHYT));
            result = result.Select(o =>
            {
                o.SoLuongTon = lstVatTuBenhVien.Where(t => t.VatTuBenhVienId == o.VatTuBenhVienId && t.LaVatTuBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            });
            return result.ToList();
        }

        public async Task<XuatKhoKhacKSNKResultVo> XuLyThemYeuCauXuatKhoKSNKAsync(XuatKhoKhacKSNKVo xuatKhoKhacKSNKVo, List<XuatKhoKhacKSNKChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var xuatKhoDuocPhamChiTietVos = yeuCauXuatVatTuChiTiets.Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham);
            var xuatKhoVatTuChiTietVos = yeuCauXuatVatTuChiTiets.Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiVatTu);
            if (!laLuuDuyet) // chỉ lưu
            {
                YeuCauXuatKhoDuocPham yeuCauXuatKhoDuocPham = null;
                if (xuatKhoDuocPhamChiTietVos.Any())
                {
                    var duocPhamBenhVienIds = xuatKhoDuocPhamChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                    var soLos = xuatKhoDuocPhamChiTietVos.Select(o => o.SoLo).ToList();

                    var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(o => o.NhapKhoDuocPhams.KhoId == xuatKhoKhacKSNKVo.KhoXuatId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();

                    yeuCauXuatKhoDuocPham = new YeuCauXuatKhoDuocPham
                    {
                        KhoXuatId = xuatKhoKhacKSNKVo.KhoXuatId.Value,
                        LyDoXuatKho = xuatKhoKhacKSNKVo.LyDoXuatKho,
                        NguoiXuatId = xuatKhoKhacKSNKVo.NguoiXuatId.Value,
                        NguoiNhanId = xuatKhoKhacKSNKVo.NguoiNhanId,
                        NgayXuat = xuatKhoKhacKSNKVo.NgayXuat ?? DateTime.Now,
                        TraNCC = xuatKhoKhacKSNKVo.TraNCC,
                        NhaThauId = xuatKhoKhacKSNKVo.NhaThauId,
                        SoChungTu = xuatKhoKhacKSNKVo.SoChungTu
                    };

                    foreach (var chiTietVo in xuatKhoDuocPhamChiTietVos)
                    {
                        var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                        .Where(o => o.DuocPhamBenhVienId == chiTietVo.VatTuBenhVienId && o.LaDuocPhamBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (nhapKhoDuocPhamChiTietXuats.Count() == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat))
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        var nhapKhoDuocPhamChiTietXuat = nhapKhoDuocPhamChiTietXuats.First();
                        var yeuCauXuatKhoDuocPhamChiTietNew = new YeuCauXuatKhoDuocPhamChiTiet
                        {
                            DuocPhamBenhVienId = nhapKhoDuocPhamChiTietXuat.DuocPhamBenhVienId,
                            HopDongThauDuocPhamId = nhapKhoDuocPhamChiTietXuat.HopDongThauDuocPhamId,
                            LaDuocPhamBHYT = nhapKhoDuocPhamChiTietXuat.LaDuocPhamBHYT,
                            Solo = nhapKhoDuocPhamChiTietXuat.Solo,
                            HanSuDung = nhapKhoDuocPhamChiTietXuat.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoDuocPhamChiTietXuat.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoDuocPhamChiTietXuat.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoDuocPhamChiTietXuat.TiLeTheoThapGia,
                            VAT = nhapKhoDuocPhamChiTietXuat.VAT,
                            MaVach = nhapKhoDuocPhamChiTietXuat.MaVach,
                            TiLeBHYTThanhToan = nhapKhoDuocPhamChiTietXuat.TiLeBHYTThanhToan,
                            MaRef = nhapKhoDuocPhamChiTietXuat.MaRef,
                            SoLuongXuat = chiTietVo.SoLuongXuat.Value
                        };
                        yeuCauXuatKhoDuocPham.YeuCauXuatKhoDuocPhamChiTiets.Add(yeuCauXuatKhoDuocPhamChiTietNew);
                    }
                    _yeuCauXuatKhoDuocPhamRepository.AutoCommitEnabled = false;
                    _yeuCauXuatKhoDuocPhamRepository.Add(yeuCauXuatKhoDuocPham);
                }
                YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu = null;
                if (xuatKhoVatTuChiTietVos.Any())
                {
                    var vatTuBenhVienIds = xuatKhoVatTuChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                    var soLos = xuatKhoVatTuChiTietVos.Select(o => o.SoLo).ToList();

                    var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(o => o.NhapKhoVatTu.KhoId == xuatKhoKhacKSNKVo.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();

                    yeuCauXuatKhoVatTu = new YeuCauXuatKhoVatTu
                    {
                        KhoXuatId = xuatKhoKhacKSNKVo.KhoXuatId.Value,
                        LyDoXuatKho = xuatKhoKhacKSNKVo.LyDoXuatKho,
                        NguoiXuatId = xuatKhoKhacKSNKVo.NguoiXuatId.Value,
                        NguoiNhanId = xuatKhoKhacKSNKVo.NguoiNhanId,
                        NgayXuat = xuatKhoKhacKSNKVo.NgayXuat ?? DateTime.Now,
                        TraNCC = xuatKhoKhacKSNKVo.TraNCC,
                        NhaThauId = xuatKhoKhacKSNKVo.NhaThauId,
                        SoChungTu = xuatKhoKhacKSNKVo.SoChungTu
                    };

                    foreach (var chiTietVo in xuatKhoVatTuChiTietVos)
                    {
                        var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                        .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (nhapKhoVatTuChiTietXuats.Count() == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat))
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        var nhapKhoVatTuChiTietXuat = nhapKhoVatTuChiTietXuats.First();
                        var yeuCauXuatKhoVatTuChiTietNew = new YeuCauXuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTietXuat.VatTuBenhVienId,
                            HopDongThauVatTuId = nhapKhoVatTuChiTietXuat.HopDongThauVatTuId,
                            LaVatTuBHYT = nhapKhoVatTuChiTietXuat.LaVatTuBHYT,
                            Solo = nhapKhoVatTuChiTietXuat.Solo,
                            HanSuDung = nhapKhoVatTuChiTietXuat.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoVatTuChiTietXuat.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoVatTuChiTietXuat.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTietXuat.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTietXuat.VAT,
                            MaVach = nhapKhoVatTuChiTietXuat.MaVach,
                            TiLeBHYTThanhToan = nhapKhoVatTuChiTietXuat.TiLeBHYTThanhToan,
                            MaRef = nhapKhoVatTuChiTietXuat.MaRef,
                            SoLuongXuat = chiTietVo.SoLuongXuat.Value
                        };
                        yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.Add(yeuCauXuatKhoVatTuChiTietNew);
                    }
                    _yeuCauXuatKhoVatTuRepository.AutoCommitEnabled = false;
                    _yeuCauXuatKhoVatTuRepository.Add(yeuCauXuatKhoVatTu);
                }
                BaseRepository.Context.SaveChanges();
                return new XuatKhoKhacKSNKResultVo { XuatDuocPhamId = yeuCauXuatKhoDuocPham?.Id, XuatVatTuId = yeuCauXuatKhoVatTu?.Id };
            }
            else
            {
                XuatKhoDuocPham xuatKhoDuocPham = null;
                XuatKhoVatTu xuatKhoVatTu = null;
                if (xuatKhoDuocPhamChiTietVos.Any())
                {
                    var duocPhamBenhVienIds = xuatKhoDuocPhamChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                    var soLos = xuatKhoDuocPhamChiTietVos.Select(o => o.SoLo).ToList();

                    var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.Table
                        .Where(o => o.NhapKhoDuocPhams.KhoId == xuatKhoKhacKSNKVo.KhoXuatId && duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();
                    //xuat kho
                    xuatKhoDuocPham = new XuatKhoDuocPham
                    {
                        LoaiXuatKho = Enums.XuatKhoDuocPham.XuatKhac,
                        KhoXuatId = xuatKhoKhacKSNKVo.KhoXuatId.Value,
                        LyDoXuatKho = xuatKhoKhacKSNKVo.LyDoXuatKho,
                        NguoiXuatId = xuatKhoKhacKSNKVo.NguoiXuatId.Value,
                        TenNguoiNhan = xuatKhoKhacKSNKVo.TenNguoiNhan,
                        NguoiNhanId = xuatKhoKhacKSNKVo.NguoiNhanId,
                        LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                        NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value,
                        TraNCC = xuatKhoKhacKSNKVo.TraNCC,
                        NhaThauId = xuatKhoKhacKSNKVo.NhaThauId,
                        SoChungTu = xuatKhoKhacKSNKVo.SoChungTu
                    };


                    foreach (var chiTietVo in xuatKhoDuocPhamChiTietVos)
                    {
                        var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                            .Where(o => o.DuocPhamBenhVienId == chiTietVo.VatTuBenhVienId && o.LaDuocPhamBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat)
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        double soLuongCanXuat = chiTietVo.SoLuongXuat.Value;
                        while (!soLuongCanXuat.AlmostEqual(0))
                        {
                            // tinh so luong xuat
                            var nhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTietXuats
                                .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                            var soLuongTon = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                            var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                            nhapKhoDuocPhamChiTiet.SoLuongDaXuat = (nhapKhoDuocPhamChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                            var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                            {
                                SoLuongXuat = soLuongXuat,
                                NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                                NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value
                            };
                            var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                            {
                                DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                                NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value
                            };
                            xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                            xuatKhoDuocPham.XuatKhoDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);

                            soLuongCanXuat = (soLuongCanXuat - soLuongXuat).MathRoundNumber(2);
                        }
                    }
                    _xuatKhoDuocPhamRepository.AutoCommitEnabled = false;
                    _xuatKhoDuocPhamRepository.Add(xuatKhoDuocPham);
                }
                if (xuatKhoVatTuChiTietVos.Any())
                {
                    var vatTuBenhVienIds = xuatKhoVatTuChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                    var soLos = xuatKhoVatTuChiTietVos.Select(o => o.SoLo).ToList();

                    var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.Table
                        .Where(o => o.NhapKhoVatTu.KhoId == xuatKhoKhacKSNKVo.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                        .ToList();
                    //xuat kho
                    xuatKhoVatTu = new XuatKhoVatTu
                    {
                        LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatKhac,
                        KhoXuatId = xuatKhoKhacKSNKVo.KhoXuatId.Value,
                        LyDoXuatKho = xuatKhoKhacKSNKVo.LyDoXuatKho,
                        NguoiXuatId = xuatKhoKhacKSNKVo.NguoiXuatId.Value,
                        TenNguoiNhan = xuatKhoKhacKSNKVo.TenNguoiNhan,
                        NguoiNhanId = xuatKhoKhacKSNKVo.NguoiNhanId,
                        LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                        NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value,
                        TraNCC = xuatKhoKhacKSNKVo.TraNCC,
                        NhaThauId = xuatKhoKhacKSNKVo.NhaThauId,
                        SoChungTu = xuatKhoKhacKSNKVo.SoChungTu
                    };


                    foreach (var chiTietVo in xuatKhoVatTuChiTietVos)
                    {
                        var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                            .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                        var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                        if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat)
                        {
                            throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                        }
                        double soLuongCanXuat = chiTietVo.SoLuongXuat.Value;
                        while (!soLuongCanXuat.AlmostEqual(0))
                        {
                            // tinh so luong xuat
                            var nhapKhoVatTuChiTiet = nhapKhoVatTuChiTietXuats
                                .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                            var soLuongTon = (nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                            var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                            nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                            var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                            {
                                SoLuongXuat = soLuongXuat,
                                NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                                NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value
                            };
                            var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                            {
                                VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                                NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value
                            };
                            xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                            xuatKhoVatTu.XuatKhoVatTuChiTiets.Add(xuatKhoVatTuChiTiet);

                            soLuongCanXuat = (soLuongCanXuat - soLuongXuat).MathRoundNumber(2);
                        }
                    }
                    _xuatKhoVatTuRepository.AutoCommitEnabled = false;
                    _xuatKhoVatTuRepository.Add(xuatKhoVatTu);
                }
                BaseRepository.Context.SaveChanges();
                return new XuatKhoKhacKSNKResultVo { XuatDuocPhamId = xuatKhoDuocPham?.Id, XuatVatTuId = xuatKhoVatTu?.Id };
            }
        }

        public async Task<CapNhatXuatKhoKhacKSNKResultVo> XuLyCapNhatYeuCauXuatKhoKSNKAsync(XuatKhoKhacKSNKVo xuatKhoKhacKSNKVo, List<XuatKhoKhacKSNKChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet)
        {
            if (xuatKhoKhacKSNKVo.LoaiDuocPhamVatTu != null &&
                xuatKhoKhacKSNKVo.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham)
            {
                var yeuCauXuatKhac = _yeuCauXuatKhoDuocPhamRepository.GetById(xuatKhoKhacKSNKVo.Id,
                    s => s.Include(r => r.YeuCauXuatKhoDuocPhamChiTiets));
                foreach (var chiTiet in yeuCauXuatKhac.YeuCauXuatKhoDuocPhamChiTiets)
                {
                    if (chiTiet.Id != 0)
                    {
                        chiTiet.WillDelete = true;
                    }
                }
                var xuatKhoDuocPhamChiTietVos = yeuCauXuatVatTuChiTiets
                    .Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiDuocPham).ToList();
                if (!laLuuDuyet) // chỉ lưu
                {
                    yeuCauXuatKhac.KhoXuatId = xuatKhoKhacKSNKVo.KhoXuatId.Value;
                    yeuCauXuatKhac.LyDoXuatKho = xuatKhoKhacKSNKVo.LyDoXuatKho;
                    yeuCauXuatKhac.NguoiXuatId = xuatKhoKhacKSNKVo.NguoiXuatId.Value;
                    yeuCauXuatKhac.NguoiNhanId = xuatKhoKhacKSNKVo.NguoiNhanId;
                    yeuCauXuatKhac.NgayXuat = xuatKhoKhacKSNKVo.NgayXuat ?? DateTime.Now;
                    yeuCauXuatKhac.TraNCC = xuatKhoKhacKSNKVo.TraNCC;
                    yeuCauXuatKhac.NhaThauId = xuatKhoKhacKSNKVo.NhaThauId;
                    yeuCauXuatKhac.SoChungTu = xuatKhoKhacKSNKVo.SoChungTu;

                    if (xuatKhoDuocPhamChiTietVos.Any())
                    {
                        var duocPhamBenhVienIds = xuatKhoDuocPhamChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                        var soLos = xuatKhoDuocPhamChiTietVos.Select(o => o.SoLo).ToList();

                        var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(o => o.NhapKhoDuocPhams.KhoId == xuatKhoKhacKSNKVo.KhoXuatId &&
                                        duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) &&
                                        o.SoLuongNhap > o.SoLuongDaXuat)
                            .ToList();

                        foreach (var chiTietVo in xuatKhoDuocPhamChiTietVos)
                        {
                            var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                                .Where(o => o.DuocPhamBenhVienId == chiTietVo.VatTuBenhVienId &&
                                            o.LaDuocPhamBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo &&
                                            o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                            var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                            if (nhapKhoDuocPhamChiTietXuats.Count() == 0 ||
                                (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat))
                            {
                                throw new Exception(
                                    _localizationService.GetResource(
                                        "XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                            }
                            var nhapKhoDuocPhamChiTietXuat = nhapKhoDuocPhamChiTietXuats.First();
                            var yeuCauXuatKhoDuocPhamChiTietNew = new YeuCauXuatKhoDuocPhamChiTiet
                            {
                                DuocPhamBenhVienId = nhapKhoDuocPhamChiTietXuat.DuocPhamBenhVienId,
                                HopDongThauDuocPhamId = nhapKhoDuocPhamChiTietXuat.HopDongThauDuocPhamId,
                                LaDuocPhamBHYT = nhapKhoDuocPhamChiTietXuat.LaDuocPhamBHYT,
                                Solo = nhapKhoDuocPhamChiTietXuat.Solo,
                                HanSuDung = nhapKhoDuocPhamChiTietXuat.HanSuDung,
                                NgayNhapVaoBenhVien = nhapKhoDuocPhamChiTietXuat.NgayNhapVaoBenhVien,
                                DonGiaNhap = nhapKhoDuocPhamChiTietXuat.DonGiaNhap,
                                TiLeTheoThapGia = nhapKhoDuocPhamChiTietXuat.TiLeTheoThapGia,
                                VAT = nhapKhoDuocPhamChiTietXuat.VAT,
                                MaVach = nhapKhoDuocPhamChiTietXuat.MaVach,
                                TiLeBHYTThanhToan = nhapKhoDuocPhamChiTietXuat.TiLeBHYTThanhToan,
                                MaRef = nhapKhoDuocPhamChiTietXuat.MaRef,
                                SoLuongXuat = chiTietVo.SoLuongXuat.Value
                            };
                            yeuCauXuatKhac.YeuCauXuatKhoDuocPhamChiTiets.Add(yeuCauXuatKhoDuocPhamChiTietNew);
                        }
                    }
                    _yeuCauXuatKhoDuocPhamRepository.Update(yeuCauXuatKhac);
                    return new CapNhatXuatKhoKhacKSNKResultVo{ Id = yeuCauXuatKhac.Id, LastModified = yeuCauXuatKhac.LastModified };
                }
                else
                {
                    if (xuatKhoDuocPhamChiTietVos.Any())
                    {
                        var duocPhamBenhVienIds = xuatKhoDuocPhamChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                        var soLos = xuatKhoDuocPhamChiTietVos.Select(o => o.SoLo).ToList();

                        var nhapKhoDuocPhamChiTiets = _nhapKhoDuocPhamChiTietRepository.Table
                            .Where(o => o.NhapKhoDuocPhams.KhoId == xuatKhoKhacKSNKVo.KhoXuatId &&
                                        duocPhamBenhVienIds.Contains(o.DuocPhamBenhVienId) && soLos.Contains(o.Solo) &&
                                        o.SoLuongNhap > o.SoLuongDaXuat)
                            .ToList();
                        //xuat kho
                        var xuatKhoDuocPham = new XuatKhoDuocPham
                        {
                            LoaiXuatKho = Enums.XuatKhoDuocPham.XuatKhac,
                            KhoXuatId = xuatKhoKhacKSNKVo.KhoXuatId.Value,
                            LyDoXuatKho = xuatKhoKhacKSNKVo.LyDoXuatKho,
                            NguoiXuatId = xuatKhoKhacKSNKVo.NguoiXuatId.Value,
                            TenNguoiNhan = xuatKhoKhacKSNKVo.TenNguoiNhan,
                            NguoiNhanId = xuatKhoKhacKSNKVo.NguoiNhanId,
                            LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                            NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value,
                            TraNCC = xuatKhoKhacKSNKVo.TraNCC,
                            NhaThauId = xuatKhoKhacKSNKVo.NhaThauId,
                            SoChungTu = xuatKhoKhacKSNKVo.SoChungTu
                        };


                        foreach (var chiTietVo in xuatKhoDuocPhamChiTietVos)
                        {
                            var nhapKhoDuocPhamChiTietXuats = nhapKhoDuocPhamChiTiets
                                .Where(o => o.DuocPhamBenhVienId == chiTietVo.VatTuBenhVienId &&
                                            o.LaDuocPhamBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo &&
                                            o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                            var slTon = nhapKhoDuocPhamChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                            if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat)
                            {
                                throw new Exception(
                                    _localizationService.GetResource(
                                        "XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                            }
                            double soLuongCanXuat = chiTietVo.SoLuongXuat.Value;
                            while (!soLuongCanXuat.AlmostEqual(0))
                            {
                                // tinh so luong xuat
                                var nhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTietXuats
                                    .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien)
                                    .First();
                                var soLuongTon = (nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                                var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                                nhapKhoDuocPhamChiTiet.SoLuongDaXuat = (nhapKhoDuocPhamChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                                var xuatKhoDuocPhamChiTietViTri = new XuatKhoDuocPhamChiTietViTri
                                {
                                    SoLuongXuat = soLuongXuat,
                                    NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
                                    NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value
                                };
                                var xuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
                                {
                                    DuocPhamBenhVienId = nhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                                    NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value
                                };
                                xuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoDuocPhamChiTietViTri);
                                xuatKhoDuocPham.XuatKhoDuocPhamChiTiets.Add(xuatKhoDuocPhamChiTiet);

                                soLuongCanXuat = (soLuongCanXuat - soLuongXuat).MathRoundNumber(2);
                            }
                        }
                        yeuCauXuatKhac.WillDelete = true;
                        _xuatKhoDuocPhamRepository.Add(xuatKhoDuocPham);
                        return new CapNhatXuatKhoKhacKSNKResultVo { Id = xuatKhoDuocPham.Id, LastModified = xuatKhoDuocPham.LastModified };
                    }
                    return null;
                }
            }
            else
            {
                var yeuCauXuatKhac = _yeuCauXuatKhoVatTuRepository.GetById(xuatKhoKhacKSNKVo.Id, s => s.Include(r => r.YeuCauXuatKhoVatTuChiTiets));
                foreach (var chiTiet in yeuCauXuatKhac.YeuCauXuatKhoVatTuChiTiets)
                {
                    if (chiTiet.Id != 0)
                    {
                        chiTiet.WillDelete = true;
                    }
                }
                var xuatKhoVatTuChiTietVos = yeuCauXuatVatTuChiTiets.Where(o => o.LoaiDuocPhamVatTu == Enums.LoaiDuocPhamVatTu.LoaiVatTu).ToList();
                if (!laLuuDuyet) // chỉ lưu
                {
                    yeuCauXuatKhac.KhoXuatId = xuatKhoKhacKSNKVo.KhoXuatId.Value;
                    yeuCauXuatKhac.LyDoXuatKho = xuatKhoKhacKSNKVo.LyDoXuatKho;
                    yeuCauXuatKhac.NguoiXuatId = xuatKhoKhacKSNKVo.NguoiXuatId.Value;
                    yeuCauXuatKhac.NguoiNhanId = xuatKhoKhacKSNKVo.NguoiNhanId;
                    yeuCauXuatKhac.NgayXuat = xuatKhoKhacKSNKVo.NgayXuat ?? DateTime.Now;
                    yeuCauXuatKhac.TraNCC = xuatKhoKhacKSNKVo.TraNCC;
                    yeuCauXuatKhac.NhaThauId = xuatKhoKhacKSNKVo.NhaThauId;
                    yeuCauXuatKhac.SoChungTu = xuatKhoKhacKSNKVo.SoChungTu;

                    if (xuatKhoVatTuChiTietVos.Any())
                    {
                        var vatTuBenhVienIds = xuatKhoVatTuChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                        var soLos = xuatKhoVatTuChiTietVos.Select(o => o.SoLo).ToList();

                        var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(o => o.NhapKhoVatTu.KhoId == xuatKhoKhacKSNKVo.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                            .ToList();

                        foreach (var chiTietVo in xuatKhoVatTuChiTietVos)
                        {
                            var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                            .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                            var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                            if (nhapKhoVatTuChiTietXuats.Count() == 0 || (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat))
                            {
                                throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                            }
                            var nhapKhoVatTuChiTietXuat = nhapKhoVatTuChiTietXuats.First();
                            var yeuCauXuatKhoVatTuChiTietNew = new YeuCauXuatKhoVatTuChiTiet
                            {
                                VatTuBenhVienId = nhapKhoVatTuChiTietXuat.VatTuBenhVienId,
                                HopDongThauVatTuId = nhapKhoVatTuChiTietXuat.HopDongThauVatTuId,
                                LaVatTuBHYT = nhapKhoVatTuChiTietXuat.LaVatTuBHYT,
                                Solo = nhapKhoVatTuChiTietXuat.Solo,
                                HanSuDung = nhapKhoVatTuChiTietXuat.HanSuDung,
                                NgayNhapVaoBenhVien = nhapKhoVatTuChiTietXuat.NgayNhapVaoBenhVien,
                                DonGiaNhap = nhapKhoVatTuChiTietXuat.DonGiaNhap,
                                TiLeTheoThapGia = nhapKhoVatTuChiTietXuat.TiLeTheoThapGia,
                                VAT = nhapKhoVatTuChiTietXuat.VAT,
                                MaVach = nhapKhoVatTuChiTietXuat.MaVach,
                                TiLeBHYTThanhToan = nhapKhoVatTuChiTietXuat.TiLeBHYTThanhToan,
                                MaRef = nhapKhoVatTuChiTietXuat.MaRef,
                                SoLuongXuat = chiTietVo.SoLuongXuat.Value
                            };
                            yeuCauXuatKhac.YeuCauXuatKhoVatTuChiTiets.Add(yeuCauXuatKhoVatTuChiTietNew);
                        }
                        _yeuCauXuatKhoVatTuRepository.Update(yeuCauXuatKhac);
                    }
                    return new CapNhatXuatKhoKhacKSNKResultVo { Id = yeuCauXuatKhac.Id, LastModified = yeuCauXuatKhac.LastModified };
                }
                else
                {
                    if (xuatKhoVatTuChiTietVos.Any())
                    {
                        var vatTuBenhVienIds = xuatKhoVatTuChiTietVos.Select(o => o.VatTuBenhVienId).ToList();
                        var soLos = xuatKhoVatTuChiTietVos.Select(o => o.SoLo).ToList();

                        var nhapKhoVatTuChiTiets = _nhapKhoVatTuChiTietRepository.Table
                            .Where(o => o.NhapKhoVatTu.KhoId == xuatKhoKhacKSNKVo.KhoXuatId && vatTuBenhVienIds.Contains(o.VatTuBenhVienId) && soLos.Contains(o.Solo) && o.SoLuongNhap > o.SoLuongDaXuat)
                            .ToList();
                        //xuat kho
                        var xuatKhoVatTu = new XuatKhoVatTu
                        {
                            LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatKhac,
                            KhoXuatId = xuatKhoKhacKSNKVo.KhoXuatId.Value,
                            LyDoXuatKho = xuatKhoKhacKSNKVo.LyDoXuatKho,
                            NguoiXuatId = xuatKhoKhacKSNKVo.NguoiXuatId.Value,
                            TenNguoiNhan = xuatKhoKhacKSNKVo.TenNguoiNhan,
                            NguoiNhanId = xuatKhoKhacKSNKVo.NguoiNhanId,
                            LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                            NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value,
                            TraNCC = xuatKhoKhacKSNKVo.TraNCC,
                            NhaThauId = xuatKhoKhacKSNKVo.NhaThauId,
                            SoChungTu = xuatKhoKhacKSNKVo.SoChungTu
                        };


                        foreach (var chiTietVo in xuatKhoVatTuChiTietVos)
                        {
                            var nhapKhoVatTuChiTietXuats = nhapKhoVatTuChiTiets
                                .Where(o => o.VatTuBenhVienId == chiTietVo.VatTuBenhVienId && o.LaVatTuBHYT == chiTietVo.LaVatTuBHYT && o.Solo == chiTietVo.SoLo && o.HanSuDung.Date == chiTietVo.HanSuDung.Value.Date);
                            var slTon = nhapKhoVatTuChiTietXuats.Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
                            if (!slTon.AlmostEqual(chiTietVo.SoLuongXuat.Value) && slTon < chiTietVo.SoLuongXuat)
                            {
                                throw new Exception(_localizationService.GetResource("XuatKho.KhoVatTuChiTiet.SoLuongTonMoreThanSoLuongXuat"));
                            }
                            double soLuongCanXuat = chiTietVo.SoLuongXuat.Value;
                            while (!soLuongCanXuat.AlmostEqual(0))
                            {
                                // tinh so luong xuat
                                var nhapKhoVatTuChiTiet = nhapKhoVatTuChiTietXuats
                                    .Where(o => o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(p => p.NgayNhapVaoBenhVien).First();
                                var soLuongTon = (nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat).MathRoundNumber(2);
                                var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
                                nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongXuat).MathRoundNumber(2);
                                var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                                {
                                    SoLuongXuat = soLuongXuat,
                                    NhapKhoVatTuChiTiet = nhapKhoVatTuChiTiet,
                                    NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value
                                };
                                var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                                {
                                    VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                                    NgayXuat = xuatKhoKhacKSNKVo.NgayXuat.Value
                                };
                                xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoVatTuChiTietViTri);
                                xuatKhoVatTu.XuatKhoVatTuChiTiets.Add(xuatKhoVatTuChiTiet);

                                soLuongCanXuat = (soLuongCanXuat - soLuongXuat).MathRoundNumber(2);
                            }
                        }
                        yeuCauXuatKhac.WillDelete = true;
                        _xuatKhoVatTuRepository.Add(xuatKhoVatTu);
                        return new CapNhatXuatKhoKhacKSNKResultVo { Id = xuatKhoVatTu.Id, LastModified = xuatKhoVatTu.LastModified };
                    }
                    return null;
                }
            }
        }

        public async Task XuLyThemHoacCapNhatVaDuyetYeuCauVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu, List<XuatKhoKhacKSNKChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet, bool isCreate)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            if (!laLuuDuyet) // chỉ lưu
            {
                if (!isCreate)
                {
                    foreach (var chiTiet in yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets)
                    {
                        if (chiTiet.Id != 0)
                        {
                            chiTiet.WillDelete = true;
                        }
                    }
                }
                var khoXuatIds = yeuCauXuatVatTuChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
                var nhapKhoVatTuChiTietAlls = _nhapKhoVatTuChiTietRepository.Table
                    .Include(vt => vt.NhapKhoVatTu)
                    .Where(o => khoXuatIds.Contains(o.NhapKhoVatTu.KhoId)
                    && o.SoLuongNhap > o.SoLuongDaXuat)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var yeuCauXuatVatTuChiTiet in yeuCauXuatVatTuChiTiets)
                {
                    var nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietAlls.Where(o => o.NhapKhoVatTu.KhoId == yeuCauXuatVatTuChiTiet.KhoXuatId
                      && o.LaVatTuBHYT == yeuCauXuatVatTuChiTiet.LaVatTuBHYT
                      && o.VatTuBenhVienId == yeuCauXuatVatTuChiTiet.VatTuBenhVienId
                      && o.HanSuDung == yeuCauXuatVatTuChiTiet.HanSuDung
                      && o.Solo == yeuCauXuatVatTuChiTiet.SoLo
                      && o.SoLuongNhap > o.SoLuongDaXuat)
                      .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                    var SLTon = nhapKhoVatTuChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                    if (SLTon < yeuCauXuatVatTuChiTiet.SoLuongXuat)
                    {
                        throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                    }

                    var soLuongXuat = yeuCauXuatVatTuChiTiet.SoLuongXuat.MathRoundNumber(2);// số lượng xuất

                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                    {
                        if (soLuongXuat == 0)
                        {
                            break;
                        }
                        var yeuCauXuatKhoVatTuChiTietNew = new YeuCauXuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                            HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                            LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT,
                            Solo = nhapKhoVatTuChiTiet.Solo,
                            HanSuDung = nhapKhoVatTuChiTiet.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTiet.VAT,
                            MaVach = nhapKhoVatTuChiTiet.MaVach,
                            TiLeBHYTThanhToan = nhapKhoVatTuChiTiet.TiLeBHYTThanhToan,
                            MaRef = nhapKhoVatTuChiTiet.MaRef,
                        };
                        var SLTonHienTai = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;
                        if (SLTonHienTai > soLuongXuat || SLTonHienTai.AlmostEqual(soLuongXuat.Value))
                        {
                            yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat = soLuongXuat.Value.MathRoundNumber(2);
                            soLuongXuat = 0;
                        }
                        else
                        {
                            yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat = SLTonHienTai.MathRoundNumber(2);
                            soLuongXuat = (soLuongXuat - SLTonHienTai).MathRoundNumber(2);
                        }
                        yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.Add(yeuCauXuatKhoVatTuChiTietNew);
                    }
                }
            }
            else
            {
                if (!isCreate)
                {
                    foreach (var chiTiet in yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets)
                    {
                        if (chiTiet.Id != 0)
                        {
                            chiTiet.WillDelete = true;
                        }
                    }
                }
                var tenNguoiNhan = _nhanVienRepository.TableNoTracking.Where(x => x.Id == yeuCauXuatKhoVatTu.NguoiNhanId).Select(z => z.User.HoTen).FirstOrDefault();
                var xuatKhoVatTu = new XuatKhoVatTu
                {
                    LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatKhac,
                    LyDoXuatKho = yeuCauXuatKhoVatTu.LyDoXuatKho,
                    NguoiNhanId = yeuCauXuatKhoVatTu.NguoiNhanId,
                    TenNguoiNhan = tenNguoiNhan,
                    NguoiXuatId = yeuCauXuatKhoVatTu.NguoiXuatId,
                    LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                    NgayXuat = DateTime.Now,
                    KhoXuatId = yeuCauXuatKhoVatTu.KhoXuatId,
                    TraNCC = yeuCauXuatKhoVatTu.TraNCC,
                    NhaThauId = yeuCauXuatKhoVatTu.NhaThauId,
                    SoChungTu = yeuCauXuatKhoVatTu.SoChungTu
                };
                var khoXuatIds = yeuCauXuatVatTuChiTiets.Select(c => c.KhoXuatId).Distinct().ToList();
                var nhapKhoVatTuChiTietAlls = _nhapKhoVatTuChiTietRepository.Table
                    .Include(vt => vt.NhapKhoVatTu)
                    .Where(o => khoXuatIds.Contains(o.NhapKhoVatTu.KhoId)
                    && o.SoLuongNhap > o.SoLuongDaXuat)
                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                foreach (var yeuCauXuatVatTuChiTiet in yeuCauXuatVatTuChiTiets)
                {
                    var nhapKhoVatTuChiTiets = nhapKhoVatTuChiTietAlls.Where(o => o.NhapKhoVatTu.KhoId == yeuCauXuatVatTuChiTiet.KhoXuatId
                     && o.LaVatTuBHYT == yeuCauXuatVatTuChiTiet.LaVatTuBHYT
                     && o.VatTuBenhVienId == yeuCauXuatVatTuChiTiet.VatTuBenhVienId
                     && o.HanSuDung == yeuCauXuatVatTuChiTiet.HanSuDung
                     && o.Solo == yeuCauXuatVatTuChiTiet.SoLo
                     && o.SoLuongNhap > o.SoLuongDaXuat)
                     .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung).ToList();
                    var SLTon = nhapKhoVatTuChiTiets.Sum(z => z.SoLuongNhap - z.SoLuongDaXuat).MathRoundNumber(2);

                    if (SLTon < yeuCauXuatVatTuChiTiet.SoLuongXuat)
                    {
                        throw new Exception(_localizationService.GetResource("DonVTYT.VTYTSoLuongTon"));
                    }

                    var xuatKhoVatTuChiTiet = new XuatKhoVatTuChiTiet
                    {
                        VatTuBenhVienId = yeuCauXuatVatTuChiTiet.VatTuBenhVienId,
                        XuatKhoVatTu = xuatKhoVatTu
                    };

                    var soLuongXuat = yeuCauXuatVatTuChiTiet.SoLuongXuat.MathRoundNumber(2);// số lượng xuất

                    foreach (var nhapKhoVatTuChiTiet in nhapKhoVatTuChiTiets)
                    {
                        if (soLuongXuat == 0)
                        {
                            break;
                        }
                        var yeuCauXuatKhoVatTuChiTietNew = new YeuCauXuatKhoVatTuChiTiet
                        {
                            VatTuBenhVienId = nhapKhoVatTuChiTiet.VatTuBenhVienId,
                            HopDongThauVatTuId = nhapKhoVatTuChiTiet.HopDongThauVatTuId,
                            LaVatTuBHYT = nhapKhoVatTuChiTiet.LaVatTuBHYT,
                            Solo = nhapKhoVatTuChiTiet.Solo,
                            HanSuDung = nhapKhoVatTuChiTiet.HanSuDung,
                            NgayNhapVaoBenhVien = nhapKhoVatTuChiTiet.NgayNhapVaoBenhVien,
                            DonGiaNhap = nhapKhoVatTuChiTiet.DonGiaNhap,
                            TiLeTheoThapGia = nhapKhoVatTuChiTiet.TiLeTheoThapGia,
                            VAT = nhapKhoVatTuChiTiet.VAT,
                            MaVach = nhapKhoVatTuChiTiet.MaVach,
                            TiLeBHYTThanhToan = nhapKhoVatTuChiTiet.TiLeBHYTThanhToan,
                            MaRef = nhapKhoVatTuChiTiet.MaRef,
                        };
                        var SLTonHienTai = nhapKhoVatTuChiTiet.SoLuongNhap - nhapKhoVatTuChiTiet.SoLuongDaXuat;
                        if (SLTonHienTai > soLuongXuat || SLTonHienTai.AlmostEqual(soLuongXuat.Value))
                        {
                            nhapKhoVatTuChiTiet.SoLuongDaXuat = (nhapKhoVatTuChiTiet.SoLuongDaXuat + soLuongXuat.Value).MathRoundNumber(2);
                            yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat = soLuongXuat.Value.MathRoundNumber(2);
                            soLuongXuat = 0;
                        }
                        else
                        {
                            nhapKhoVatTuChiTiet.SoLuongDaXuat = nhapKhoVatTuChiTiet.SoLuongNhap.MathRoundNumber(2);
                            yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat = SLTonHienTai.MathRoundNumber(2);
                            soLuongXuat = (soLuongXuat - SLTonHienTai).MathRoundNumber(2);
                        }
                        var xuatKhoVatTuChiTietViTri = new XuatKhoVatTuChiTietViTri
                        {
                            SoLuongXuat = yeuCauXuatKhoVatTuChiTietNew.SoLuongXuat,
                            //GhiChu = "Xuất khác.",
                            GhiChu = yeuCauXuatKhoVatTu.TraNCC == true ? "Trả nhà cung cấp" : "Xuất khác.",
                            XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet,
                            NgayXuat = DateTime.Now,
                            NhapKhoVatTuChiTietId = nhapKhoVatTuChiTiet.Id
                        };
                        yeuCauXuatKhoVatTuChiTietNew.XuatKhoVatTuChiTietViTri = xuatKhoVatTuChiTietViTri;
                        yeuCauXuatKhoVatTuChiTietNew.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet = xuatKhoVatTuChiTiet;
                        yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.Add(yeuCauXuatKhoVatTuChiTietNew);
                        await XuLySoLuongTonAsync(nhapKhoVatTuChiTiet);
                    }
                }
            }
            await _yeuCauXuatKhoVatTuRepository.UpdateAsync(yeuCauXuatKhoVatTu);
        }
        private async Task XuLySoLuongTonAsync(NhapKhoVatTuChiTiet nhapKhoVatTuChiTiet)
        {
            await _nhapKhoVatTuChiTietRepository.UpdateAsync(nhapKhoVatTuChiTiet);
        }
        public async Task<XuatKhoKhacVatTuRsVo> XuLyXoaYeuCauVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu)
        {
            foreach (var item in yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets)
            {
                item.WillDelete = true;
            }
            yeuCauXuatKhoVatTu.WillDelete = true;
            await _yeuCauXuatKhoVatTuRepository.UpdateAsync(yeuCauXuatKhoVatTu);
            var xuatKhoKhacDuocPhamRsVo = new XuatKhoKhacVatTuRsVo
            {
                Id = yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTuId.GetValueOrDefault(),
                LastModified = yeuCauXuatKhoVatTu.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.LastModified,
            };
            return xuatKhoKhacDuocPhamRsVo;
        }
        public async Task CheckPhieuYeuCauXuatVTKhacDaDuyetHoacDaHuy(long yeuCauXuatKhoVatTuId)
        {
            var result = await _yeuCauXuatKhoVatTuRepository.TableNoTracking.Where(p => p.Id == yeuCauXuatKhoVatTuId).Select(p => p).FirstOrDefaultAsync();
            var resourceName = string.Empty;
            if (result == null)
            {
                resourceName = "YeuCauDieuChuyenDuocPham.PhieuYeuCau.NotExists";
            }
            else
            {
                if (result.DuocDuyet == true)
                {
                    resourceName = "YeuCauDieuChuyenDuocPham.PhieuYeuCau.DaDuyet";
                }
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                var currentUserLanguge = _userAgentHelper.GetUserLanguage();
                var mess = await _localeStringResourceRepository.TableNoTracking
                    .Where(x => x.ResourceName == resourceName && x.Language == (int)currentUserLanguge)
                    .Select(x => x.ResourceValue).FirstOrDefaultAsync();
                throw new Exception(mess ?? resourceName);
            }
        }
        public async Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long yeuCauXuatKhoVatTuId)
        {
            var yeuCau = await _yeuCauXuatKhoVatTuRepository.TableNoTracking.Where(p => p.Id == yeuCauXuatKhoVatTuId).FirstOrDefaultAsync();
            var trangThaiVo = new TrangThaiDuyetVo();
            if (yeuCau != null)
            {
                trangThaiVo.TrangThai = null;
                trangThaiVo.Ten = "Đang chờ duyệt";
                return trangThaiVo;
            }
            else
            {
                trangThaiVo.TrangThai = true;
                trangThaiVo.Ten = "Đã duyệt xuất";
                return trangThaiVo;
            }
        }
        public async Task<List<XuatKhoKhacLookupItem>> GetKhoKsnk(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var khoId = CommonHelper.GetIdFromRequestDropDownList(model);
            var khos = await _khoRepository.TableNoTracking
                .Where(p => p.KhoaPhongId == phongBenhVien.KhoaPhongId
                        && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
                        && p.LaKhoKSNK == true)
                        .ApplyLike(model.Query, p => p.Ten)
                        .Select(item => new XuatKhoKhacLookupItem
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            LoaiKho = item.LoaiKho
                        })
                        .OrderByDescending(x => x.KeyId == khoId).ThenBy(x => x.DisplayName)
                        .Take(model.Take).ToListAsync();
            return khos;
        }
        public async Task<List<XuatKhoKhacLookupItem>> GetKhoTheoLoaiVatTu(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId);
            var khoId = CommonHelper.GetIdFromRequestDropDownList(model);
            var khos = await _khoRepository.TableNoTracking
                .Where(p =>
                        ((p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe
                         || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoVacXin
                        || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc)
                        && p.KhoaPhongId == phongBenhVien.KhoaPhongId
                        && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
                        || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                        && p.LoaiVatTu == true)
                        .ApplyLike(model.Query, p => p.Ten)
                        .Select(item => new XuatKhoKhacLookupItem
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            LoaiKho = item.LoaiKho
                        })
                        .OrderByDescending(x => x.KeyId == khoId).ThenBy(x => x.DisplayName)
                        .Take(model.Take).ToListAsync();
            return khos;
        }
        public async Task<bool> CheckSoLuongTonVatTu(long vatTuBenhVienId, bool? laVatTuBHYT, long? khoXuatId, double? soLuongXuat, string soLo, DateTime? hanSuDung)
        {
            if (soLuongXuat == null)
            {
                return true;
            }
            var soLuongTonVatTu = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.VatTuBenhVienId == vatTuBenhVienId && o.LaVatTuBHYT == laVatTuBHYT && o.NhapKhoVatTu.KhoId == khoXuatId && o.HanSuDung == hanSuDung && o.Solo == soLo && o.SoLuongNhap > o.SoLuongDaXuat
            //&& o.HanSuDung >= DateTime.Now
            ).Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);
            soLuongTonVatTu = soLuongTonVatTu.MathRoundNumber(2);
            if (soLuongXuat > soLuongTonVatTu)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<double> GetSoLuongTonThucTe(YeuCauXuatKhoKSNKGridVo yeuCauXuatKhoVatTuGridVo)
        {
            return await _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(z => z.VatTuBenhVienId == yeuCauXuatKhoVatTuGridVo.VatTuBenhVienId && z.LaVatTuBHYT == yeuCauXuatKhoVatTuGridVo.LaVatTuBHYT && z.Solo == yeuCauXuatKhoVatTuGridVo.SoLo && z.HanSuDung == yeuCauXuatKhoVatTuGridVo.HanSuDung && z.NhapKhoVatTu.KhoId == yeuCauXuatKhoVatTuGridVo.KhoXuatId
            //&& z.HanSuDung >= DateTime.Now 
            && z.SoLuongNhap > z.SoLuongDaXuat).SumAsync(x => x.SoLuongNhap - x.SoLuongDaXuat);
        }
        public async Task<List<LookupItemVo>> GetKhoVatTu(DropDownListRequestModel model)
        {
            var userCurrentId = _userAgentHelper.GetCurrentUserId();

            var khos = _khoRepository.TableNoTracking.Where(p =>
                    (p.LoaiKho == Enums.EnumLoaiKhoDuocPham.NhaThuoc || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe || p.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                    && p.KhoNhanVienQuanLys.Any(x => x.NhanVienId == userCurrentId)
                    && p.LoaiVatTu == true)
                    .ApplyLike(model.Query, p => p.Ten)
                    .Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                    }).Take(model.Take);
            return await khos.ToListAsync();
        }
        public string InPhieuXuatKhoKhacKSNK(PhieuXuatKhoKhacVo phieuXuatKhoKhac)
        {
            var content = string.Empty;
            var template = new Template();
            var noiLamViecCurrentId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = _phongBenhVienRepository.GetById(noiLamViecCurrentId, s => s.Include(c => c.KhoaPhong));
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var xuatKhoDuocPhamKhacInVo = new XuatKhoKhacKSNKInVo();
            var xuatKhoDuocPhamKhacChiTietInVos = new List<XuatKhoKhacKSNKKhacChiTietInVo>();
            var duocPhamHoacVatTus = string.Empty;


            var hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                           "<th>PHIẾU XUẤT</th>" +
                      "</p>";

            if (phieuXuatKhoKhac.LaDuocPham)
            {
                if (phieuXuatKhoKhac.DuocDuyet == true)
                {
                    xuatKhoDuocPhamKhacInVo = _xuatKhoDuocPhamRepository.TableNoTracking.Where(c => c.Id == phieuXuatKhoKhac.Id)
                                            .Select(s => new XuatKhoKhacKSNKInVo
                                            {
                                                Header = hearder,
                                                SoPhieu = s.SoPhieu,
                                                KhoaPhong = phongBenhVien.KhoaPhong.Ten,
                                                CreatedOn = s.CreatedOn,
                                                KhoTraLai = s.KhoDuocPhamXuat.Ten,
                                                NguoiXuat = s.NguoiXuat.User.HoTen,
                                                //SoPhieuNhap = s.NhapKhoDuocPhams.Any() ? s.NhapKhoDuocPhams.FirstOrDefault().SoPhieu : "",
                                                //NgayNhap = s.NhapKhoDuocPhams.Any() ? s.NhapKhoDuocPhams.FirstOrDefault().NgayNhap : (DateTime?)null,
                                                SoHoaDon = s.SoChungTu,
                                                NhaThauId = s.NhaThauId,
                                                //NgayHoaDon = s.NhapKhoDuocPhams.Any() ? s.NhapKhoDuocPhams.FirstOrDefault().NgayHoaDon : null,
                                                NCC = s.NhaThau != null ? s.NhaThau.Ten : "",
                                                DienGiai = s.LyDoXuatKho
                                            }).First();
                    xuatKhoDuocPhamKhacChiTietInVos = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                       .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == phieuXuatKhoKhac.Id)
                       .Select(s => new XuatKhoKhacKSNKKhacChiTietInVo()
                       {
                           Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
                           Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                           DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                           SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
                           HanSuDungDisplay = s.NhapKhoDuocPhamChiTiet.HanSuDung.ApplyFormatDate(),
                           SoLuong = s.SoLuongXuat,
                           DonGia = s.NhapKhoDuocPhamChiTiet.DonGiaNhap,
                           GhiChu = s.GhiChu
                       }).OrderBy(z => z.Ten).ToList();
                    decimal tongCong = 0;
                    tongCong = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.ThanhTien);
                    xuatKhoDuocPhamKhacInVo.TongCong = tongCong.ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.VAT = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.VAT).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.GiaTriThanhToan = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongTienBangChu = ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan));
                }
                else
                {
                    xuatKhoDuocPhamKhacInVo = _yeuCauXuatKhoDuocPhamRepository.TableNoTracking.Where(c => c.Id == phieuXuatKhoKhac.Id)
                                           .Select(s => new XuatKhoKhacKSNKInVo
                                           {
                                               Header = hearder,
                                               SoPhieu = "",
                                               KhoaPhong = phongBenhVien.KhoaPhong.Ten,
                                               CreatedOn = s.CreatedOn,
                                               KhoTraLai = s.KhoXuat.Ten,
                                               NguoiXuat = s.NguoiXuat.User.HoTen,
                                               //SoPhieuNhap = s.YeuCauXuatKhoDuocPhamChiTiets.Any() && s.YeuCauXuatKhoDuocPhamChiTiets.First().XuatKhoDuocPhamChiTietViTri != null ? s.YeuCauXuatKhoDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.SoPhieu : "",
                                               //NgayNhap = s.YeuCauXuatKhoDuocPhamChiTiets.Any() && s.YeuCauXuatKhoDuocPhamChiTiets.First().XuatKhoDuocPhamChiTietViTri != null ? s.YeuCauXuatKhoDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.NgayNhap : (DateTime?)null,
                                               SoHoaDon = s.SoChungTu,
                                               NhaThauId = s.NhaThauId,
                                               //NgayHoaDon = s.YeuCauXuatKhoDuocPhamChiTiets.Any() && s.YeuCauXuatKhoDuocPhamChiTiets.First().XuatKhoDuocPhamChiTietViTri != null ? s.YeuCauXuatKhoDuocPhamChiTiets.FirstOrDefault().XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.NgayHoaDon : null,
                                               NCC = s.NhaThau != null ? s.NhaThau.Ten : "",
                                               DienGiai = s.LyDoXuatKho,
                                               ChietKhau = 0
                                           }).First();
                    xuatKhoDuocPhamKhacChiTietInVos = _yeuCauXuatKhoDuocPhamChiTietRepository.TableNoTracking
                       .Where(x => x.YeuCauXuatKhoDuocPhamId == phieuXuatKhoKhac.Id)
                       .Select(s => new XuatKhoKhacKSNKKhacChiTietInVo()
                       {
                           Ma = s.DuocPhamBenhVien.Ma,
                           Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                           DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                           SoLo = s.Solo,
                           HanSuDungDisplay = s.HanSuDung.ApplyFormatDate(),
                           SoLuong = s.SoLuongXuat,
                           DonGia = s.DonGiaNhap,
                           VATCal = s.VAT,
                           GhiChu = ""
                       }).OrderBy(z => z.Ten).ToList();
                    decimal tongCong = 0;
                    tongCong = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.ThanhTien);
                    xuatKhoDuocPhamKhacInVo.VAT = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.VAT).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.GiaTriThanhToan = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongCong = tongCong.ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongTienBangChu = ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan));
                }
                if (phieuXuatKhoKhac.CoNCC != true)
                {
                    var STT = 1;
                    template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatThuocKhacKhoKSNK"));
                    foreach (var item in xuatKhoDuocPhamKhacChiTietInVos)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLo
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HanSuDungDisplay
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.SoLuong != (int)item.SoLuong ? item.SoLuong.ToString() : item.SoLuong.ApplyNumber())
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                   + "</tr>";
                        STT++;
                    }
                    xuatKhoDuocPhamKhacInVo.ThuocVatTu = duocPhamHoacVatTus;
                    xuatKhoDuocPhamKhacInVo.CongKhoan = STT - 1;
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, xuatKhoDuocPhamKhacInVo);
                }
                else
                {
                    //lay SoPhieuNhap,NgayNhap, NgayHoaDon
                    if (xuatKhoDuocPhamKhacInVo.NhaThauId != null && !string.IsNullOrEmpty(xuatKhoDuocPhamKhacInVo.SoHoaDon))
                    {
                        var thongTinNhap = _yeuCauNhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(o => o.YeuCauNhapKhoDuocPham.DuocKeToanDuyet == true && o.YeuCauNhapKhoDuocPham.SoChungTu == xuatKhoDuocPhamKhacInVo.SoHoaDon && o.HopDongThauDuocPham.NhaThauId == xuatKhoDuocPhamKhacInVo.NhaThauId)
                            .Select(o => new { o.Id, o.YeuCauNhapKhoDuocPham.SoPhieu, o.YeuCauNhapKhoDuocPham.NgayNhap, o.YeuCauNhapKhoDuocPham.NgayHoaDon })
                            .OrderBy(o => o.Id).LastOrDefault();
                        if (thongTinNhap != null)
                        {
                            xuatKhoDuocPhamKhacInVo.SoPhieuNhap = thongTinNhap.SoPhieu;
                            xuatKhoDuocPhamKhacInVo.DTNgayNhap = thongTinNhap.NgayNhap;
                            xuatKhoDuocPhamKhacInVo.DTNgayHoaDon = thongTinNhap.NgayHoaDon;
                        }
                    }

                    template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatThuocKhacNCCKhoKSNK"));
                    var STT = 1;
                    foreach (var item in xuatKhoDuocPhamKhacChiTietInVos)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLo
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.HanSuDungDisplay
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.SoLuong != (int)item.SoLuong ? item.SoLuong.ToString() : item.SoLuong.ApplyNumber())
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.ThanhTien.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "</tr>";
                        STT++;
                    }
                    xuatKhoDuocPhamKhacInVo.ThuocVatTu = duocPhamHoacVatTus;
                    xuatKhoDuocPhamKhacInVo.CongKhoan = STT - 1;
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, xuatKhoDuocPhamKhacInVo);
                }
            }
            else // VẬT TƯ
            {
                if (phieuXuatKhoKhac.DuocDuyet == true)
                {
                    xuatKhoDuocPhamKhacInVo = _xuatKhoVatTuRepository.TableNoTracking.Where(c => c.Id == phieuXuatKhoKhac.Id)
                                            .Select(s => new XuatKhoKhacKSNKInVo
                                            {
                                                Header = hearder,
                                                SoPhieu = s.SoPhieu,
                                                KhoaPhong = phongBenhVien.KhoaPhong.Ten,
                                                CreatedOn = s.CreatedOn,
                                                KhoTraLai = s.KhoVatTuXuat.Ten,
                                                NguoiXuat = s.NguoiXuat.User.HoTen,
                                                //SoPhieuNhap = s.NhapKhoVatTus.Any() ? s.NhapKhoVatTus.FirstOrDefault().SoPhieu : "",
                                                //NgayNhap = s.NhapKhoVatTus.Any() ? s.NhapKhoVatTus.FirstOrDefault().NgayNhap : (DateTime?)null,
                                                SoHoaDon = s.SoChungTu,
                                                NhaThauId = s.NhaThauId,
                                                //NgayHoaDon = s.NhapKhoVatTus.Any() ? s.NhapKhoVatTus.FirstOrDefault().NgayHoaDon : null,
                                                NCC = s.NhaThau != null ? s.NhaThau.Ten : "",
                                                DienGiai = s.LyDoXuatKho
                                            }).First();
                    xuatKhoDuocPhamKhacChiTietInVos = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                       .Where(x => x.XuatKhoVatTuChiTiet.XuatKhoVatTuId == phieuXuatKhoKhac.Id)
                       .Select(s => new XuatKhoKhacKSNKKhacChiTietInVo()
                       {
                           Ma = s.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                           Ten = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                           DVT = s.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                           SoLo = s.NhapKhoVatTuChiTiet.Solo,
                           HanSuDungDisplay = s.NhapKhoVatTuChiTiet.HanSuDung.ApplyFormatDate(),
                           SoLuong = s.SoLuongXuat,
                           DonGia = s.NhapKhoVatTuChiTiet.DonGiaNhap,
                           GhiChu = s.GhiChu
                       }).OrderBy(z => z.Ten).ToList();
                    decimal tongCong = 0;
                    tongCong = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.ThanhTien);
                    xuatKhoDuocPhamKhacInVo.TongCong = tongCong.ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.VAT = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.VAT).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.GiaTriThanhToan = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongTienBangChu = ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan));
                }
                else
                {
                    xuatKhoDuocPhamKhacInVo = _yeuCauXuatKhoVatTuRepository.TableNoTracking.Where(c => c.Id == phieuXuatKhoKhac.Id)
                                           .Select(s => new XuatKhoKhacKSNKInVo
                                           {
                                               Header = hearder,
                                               SoPhieu = "",
                                               KhoaPhong = phongBenhVien.KhoaPhong.Ten,
                                               CreatedOn = s.CreatedOn,
                                               KhoTraLai = s.KhoXuat.Ten,
                                               NguoiXuat = s.NguoiXuat.User.HoTen,
                                               //SoPhieuNhap = s.YeuCauXuatKhoVatTuChiTiets.Any() && s.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri != null ? s.YeuCauXuatKhoVatTuChiTiets.FirstOrDefault().XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NhapKhoVatTu.SoPhieu : "",
                                               //NgayNhap = s.YeuCauXuatKhoVatTuChiTiets.Any() && s.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri != null ? s.YeuCauXuatKhoVatTuChiTiets.FirstOrDefault().XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NhapKhoVatTu.NgayNhap : (DateTime?)null,
                                               SoHoaDon = s.SoChungTu,
                                               NhaThauId = s.NhaThauId,
                                               //NgayHoaDon = s.YeuCauXuatKhoVatTuChiTiets.Any() && s.YeuCauXuatKhoVatTuChiTiets.First().XuatKhoVatTuChiTietViTri != null ? s.YeuCauXuatKhoVatTuChiTiets.FirstOrDefault().XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.NhapKhoVatTu.NgayHoaDon : null,
                                               NCC = s.NhaThau != null ? s.NhaThau.Ten : "",
                                               DienGiai = s.LyDoXuatKho,
                                               ChietKhau = 0
                                           }).First();
                    xuatKhoDuocPhamKhacChiTietInVos = _yeuCauXuatKhoVatTuChiTietRepository.TableNoTracking
                       .Where(x => x.YeuCauXuatKhoVatTuId == phieuXuatKhoKhac.Id)
                       .Select(s => new XuatKhoKhacKSNKKhacChiTietInVo()
                       {
                           Ma = s.VatTuBenhVien.Ma,
                           Ten = s.VatTuBenhVien.VatTus.Ten,
                           DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                           SoLo = s.Solo,
                           HanSuDungDisplay = s.HanSuDung.ApplyFormatDate(),
                           SoLuong = s.SoLuongXuat,
                           DonGia = s.DonGiaNhap,
                           VATCal = s.VAT,
                           GhiChu = ""
                       }).OrderBy(z => z.Ten).ToList();
                    decimal tongCong = 0;
                    tongCong = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.ThanhTien);
                    xuatKhoDuocPhamKhacInVo.VAT = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.VAT).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.GiaTriThanhToan = xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan).ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongCong = tongCong.ApplyFormatMoneyVND().Replace(" ₫", "");
                    xuatKhoDuocPhamKhacInVo.TongTienBangChu = ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(xuatKhoDuocPhamKhacChiTietInVos.Sum(z => z.GiaTriThanhToan));
                }
                if (phieuXuatKhoKhac.CoNCC != true)
                {
                    var STT = 1;
                    template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatThuocKhacKhoKSNK"));
                    var gopThuocCungTenLoDvtHsdDonGias = xuatKhoDuocPhamKhacChiTietInVos.GroupBy(p => new { p.Ten, p.DVT, p.SoLo, p.HanSuDungDisplay, p.DonGia });
                    foreach (var item in gopThuocCungTenLoDvtHsdDonGias)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Key.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.SoLo
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.HanSuDungDisplay
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.Sum(c => c.SoLuong) != (int)item.Sum(c => c.SoLuong) ? item.Sum(c => c.SoLuong).ToString() : item.Sum(c => c.SoLuong).ApplyNumber())
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.Key.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + ((int)item.Sum(c => c.SoLuong) * item.Key.DonGia).ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + "&nbsp;"
                                                   + "</tr>";
                        STT++;
                    }
                    xuatKhoDuocPhamKhacInVo.ThuocVatTu = duocPhamHoacVatTus;
                    xuatKhoDuocPhamKhacInVo.CongKhoan = STT - 1;
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, xuatKhoDuocPhamKhacInVo);
                }
                else
                {
                    //lay SoPhieuNhap,NgayNhap, NgayHoaDon
                    if (xuatKhoDuocPhamKhacInVo.NhaThauId != null && !string.IsNullOrEmpty(xuatKhoDuocPhamKhacInVo.SoHoaDon))
                    {
                        var thongTinNhap = _yeuCauNhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(o => o.YeuCauNhapKhoVatTu.DuocKeToanDuyet == true && o.YeuCauNhapKhoVatTu.SoChungTu == xuatKhoDuocPhamKhacInVo.SoHoaDon && o.HopDongThauVatTu.NhaThauId == xuatKhoDuocPhamKhacInVo.NhaThauId)
                            .Select(o => new { o.Id, o.YeuCauNhapKhoVatTu.SoPhieu, o.YeuCauNhapKhoVatTu.NgayNhap, o.YeuCauNhapKhoVatTu.NgayHoaDon })
                            .OrderBy(o => o.Id).LastOrDefault();
                        if (thongTinNhap != null)
                        {
                            xuatKhoDuocPhamKhacInVo.SoPhieuNhap = thongTinNhap.SoPhieu;
                            xuatKhoDuocPhamKhacInVo.DTNgayNhap = thongTinNhap.NgayNhap;
                            xuatKhoDuocPhamKhacInVo.DTNgayHoaDon = thongTinNhap.NgayHoaDon;
                        }
                    }

                    template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatThuocKhacNCCKhoKSNK"));
                    var STT = 1;
                    var gopThuocCungTenLoDvtHsdDonGias = xuatKhoDuocPhamKhacChiTietInVos.GroupBy(p => new { p.Ma, p.Ten, p.DVT, p.SoLo, p.HanSuDungDisplay, p.DonGia });
                    foreach (var item in gopThuocCungTenLoDvtHsdDonGias)
                    {
                        duocPhamHoacVatTus += "<tr style = 'border: 1px solid #020000;'>"
                                                   + "<td style = 'border: 1px solid #020000;text-align: center'>" + STT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.Ma
                                                   + "<td style = 'border: 1px solid #020000;text-align: left; padding-left:3px'>" + item.Key.Ten
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.DVT
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.SoLo
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Key.HanSuDungDisplay
                                                   + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.Sum(c => c.SoLuong) != (int)item.Sum(c => c.SoLuong) ? item.Sum(c => c.SoLuong).ToString() : item.Sum(c => c.SoLuong).ApplyNumber())
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + item.Key.DonGia.ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "<td style = 'border: 1px solid #020000;text-align: right; padding-right:3px'>" + ((int)item.Sum(c => c.SoLuong) * item.Key.DonGia).ApplyFormatMoneyVND().Replace(" ₫", "")
                                                   + "</tr>";
                        STT++;
                    }
                    xuatKhoDuocPhamKhacInVo.ThuocVatTu = duocPhamHoacVatTus;
                    xuatKhoDuocPhamKhacInVo.CongKhoan = STT - 1;
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, xuatKhoDuocPhamKhacInVo);
                }
            }
            return content;
        }

        public async Task<List<YeuCauXuatKhoKSNKGridVo>> YeuCauXuatDuocPhamChiTiets(long yeuCauXuatKhoDuocPhamId)
        {
            var query = _yeuCauXuatKhoDuocPhamChiTietRepository.TableNoTracking.Where(z => z.YeuCauXuatKhoDuocPhamId == yeuCauXuatKhoDuocPhamId)
                .Select(s => new YeuCauXuatKhoKSNKGridVo
                {
                    Id = s.Id,
                    VatTuBenhVienId = s.DuocPhamBenhVienId,
                    Ten = s.DuocPhamBenhVien.DuocPham.Ten,
                    DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                    LaVatTuBHYT = s.LaDuocPhamBHYT,
                    NhomVatTuId = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId,
                    TenNhom = s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
                    Ma = s.DuocPhamBenhVien.Ma,
                    SoDangKy = s.DuocPhamBenhVien.DuocPham.SoDangKy,
                    HamLuong = s.DuocPhamBenhVien.DuocPham.HamLuong,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    DonGiaNhap = s.DonGiaNhap,
                    KhoXuatId = s.YeuCauXuatKhoDuocPham.KhoXuatId,
                    SoLuongXuat = s.SoLuongXuat,
                    LoaiDuocPhamVatTu = Enums.LoaiDuocPhamVatTu.LoaiDuocPham,
                })
                .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.Ten, x.SoLo, x.HanSuDung, x.DVT })
                .Select(g => new YeuCauXuatKhoKSNKGridVo
                {
                    Id = g.First().Id,
                    VatTuBenhVienId = g.First().VatTuBenhVienId,
                    Ten = g.First().Ten,
                    DVT = g.First().DVT,
                    LaVatTuBHYT = g.First().LaVatTuBHYT,
                    NhomVatTuId = g.First().NhomVatTuId,
                    TenNhom = g.First().TenNhom ?? "CHƯA PHÂN NHÓM",
                    Ma = g.First().Ma,
                    SoLo = g.First().SoLo,
                    LoaiDuocPhamVatTu = g.First().LoaiDuocPhamVatTu,
                    HanSuDung = g.First().HanSuDung,
                    DonGiaNhap = g.First().DonGiaNhap,
                    KhoXuatId = g.First().KhoXuatId,
                    SoLuongXuat = g.Sum(z => z.SoLuongXuat),
                });

            var yeuCauXuatPhamBuGridParentVos = query.ToList();

            var lstDuocPhamBenhVien = _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => yeuCauXuatPhamBuGridParentVos
            .Any(z => z.VatTuBenhVienId == x.DuocPhamBenhVienId && z.LaVatTuBHYT == x.LaDuocPhamBHYT &&
            x.NhapKhoDuocPhams.KhoId == z.KhoXuatId && x.HanSuDung == z.HanSuDung && x.Solo == z.SoLo) && x.SoLuongNhap > x.SoLuongDaXuat).ToList();

            var result = yeuCauXuatPhamBuGridParentVos.Where(p => lstDuocPhamBenhVien.Any(o => o.DuocPhamBenhVienId == p.VatTuBenhVienId && o.LaDuocPhamBHYT == p.LaVatTuBHYT));
            result = result.Select(o =>
            {
                o.SoLuongTon = lstDuocPhamBenhVien.Where(t => t.DuocPhamBenhVienId == o.VatTuBenhVienId && t.LaDuocPhamBHYT == o.LaVatTuBHYT && t.HanSuDung == o.HanSuDung && t.Solo == o.SoLo).Sum(t => t.SoLuongNhap - t.SoLuongDaXuat).MathRoundNumber(2);
                return o;
            });
            return result.ToList();
        }


        public async Task<GridDataSource> GetDataForGridChildAsyncDuocPhamDaDuyet(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<YeuCauXuatKhoKSNKGridVo>(queryInfo.AdditionalSearchString);
            var query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
           .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == queryString.XuatKhoVatTuId)
           .Select(s => new YeuCauXuatKhoKSNKGridVo()
           {
               Id = s.Id,
               Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
               DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
               TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
               Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
               SoDangKy = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.SoDangKy,
               HamLuong = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
               SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
               HanSuDung = s.NhapKhoDuocPhamChiTiet.HanSuDung,
               SoLuongXuat = s.SoLuongXuat,
               SoPhieu = s.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu

           });
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.DVT,
                     g => g.Ten,
                     g => g.Ma,
                     g => g.SoDangKy,
                     g => g.SoLo,
                     g => g.HamLuong,
                     g => g.SoPhieu
               );
            }
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridChildAsyncDuocPhamDaDuyet(QueryInfo queryInfo)
        {
            var queryString = JsonConvert.DeserializeObject<YeuCauXuatKhoKSNKGridVo>(queryInfo.AdditionalSearchString);
            var query = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
           .Where(x => x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId == queryString.XuatKhoVatTuId)
           .Select(s => new YeuCauXuatKhoKSNKGridVo()
           {
               Id = s.Id,
               Ten = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
               DVT = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
               TenNhom = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPhamBenhVienPhanNhom.Ten ?? "CHƯA PHÂN NHÓM",
               Ma = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.Ma,
               SoDangKy = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.SoDangKy,
               HamLuong = s.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.HamLuong,
               SoLo = s.NhapKhoDuocPhamChiTiet.Solo,
               HanSuDung = s.NhapKhoDuocPhamChiTiet.HanSuDung,
               SoLuongXuat = s.SoLuongXuat,
               SoPhieu = s.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu

           });
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.DVT,
                     g => g.Ten,
                     g => g.Ma,
                     g => g.SoDangKy,
                     g => g.SoLo,
                     g => g.HamLuong,
                     g => g.SoPhieu
               );
            }
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

    }
}
