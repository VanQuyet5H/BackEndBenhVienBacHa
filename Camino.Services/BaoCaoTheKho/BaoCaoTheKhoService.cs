using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject.BaoCaoTheKhos;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Services.ExportImport.Help;
using System.IO;
using OfficeOpenXml;
using System.Net;
using System.Drawing;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using System;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.Thuocs;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Services.BaoCaoTheKho
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoTheKhoService))]

    public class BaoCaoTheKhoService : MasterFileService<NhapKhoDuocPham>, IBaoCaoTheKhoService
    {
        IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        IRepository<DuocPham> _duocPhamRepository;
        IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;

        IRepository<Kho> _khoRepository;
        IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> _vatTuBenhVienRepository;
        IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        IRepository<DonThuocThanhToan> _donThuocThanhToanRepository;
        IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        IRepository<Core.Domain.Entities.XuatKhos.XuatKhoDuocPham> _xuatKhoDuocPhamRepository;

        public BaoCaoTheKhoService(
            IRepository<NhapKhoDuocPham> repository,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
            IRepository<Kho> khoRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
            IRepository<DuocPham> duocPhamRepository,
            IRepository<Core.Domain.Entities.VatTus.VatTu> vatTuRepository,
            IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<DonThuocThanhToan> donThuocThanhToanRepository,
            IRepository<Core.Domain.Entities.XuatKhos.XuatKhoDuocPham> xuatKhoDuocPhamRepository,
            IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository,
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
            IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository
            ) : base(repository)
        {
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
            _khoRepository = khoRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _duocPhamRepository = duocPhamRepository;
            _vatTuRepository = vatTuRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _donThuocThanhToanRepository = donThuocThanhToanRepository;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(BaoCaoTheKhoQueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var baoCaoTheKhoGridVos = GetDataBaoCaoTheKho(queryInfo);
            if (baoCaoTheKhoGridVos != null)
                return new GridDataSource {Data = baoCaoTheKhoGridVos.ToArray(), TotalRowCount = baoCaoTheKhoGridVos.Count()};
            else
                return null;
            /*
            var fromDate = queryInfo.startDate.Date;
            var toDate = queryInfo.endDate.Date.AddDays(1).AddMilliseconds(-1);
            if (queryInfo.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien)
            {
                var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Where(o =>
                        o.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                        o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= toDate)
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        TenKho = o.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                        Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                        DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        NhapTuKho = o.NhapKhoDuocPhams.XuatKhoDuocPhamId != null ? o.NhapKhoDuocPhams.XuatKhoDuocPham.KhoDuocPhamXuat.Ten : "",
                        TenNhaThau = o.HopDongThauDuocPhams.NhaThau.Ten,
                        SoHopDongThau = o.HopDongThauDuocPhams.SoHopDong,
                        NhapTuNhaCungCap = o.HopDongThauDuocPhams.HeThongTuPhatSinh != null && o.HopDongThauDuocPhams.HeThongTuPhatSinh == true,
                        NgayThang = o.NgayNhap,
                        SCTNhap = o.NhapKhoDuocPhams.SoPhieu,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

                var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId
                                && ((o.NgayXuat != null && o.NgayXuat <= toDate) ||
                                    (o.NgayXuat == null &&
                                     o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= toDate)))
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                        DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        TenKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.Ten,
                        LoaiXuatKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LoaiXuatKho,
                        XuatQuaKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoNhapId != null
                            ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamNhap.Ten
                            : "",
                        NgayThang = o.NgayXuat != null
                            ? o.NgayXuat.Value
                            : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        //SCTXuat = o.YeuCauTraDuocPhamTuBenhNhanChiTietId == null ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : "",
                        SCTXuat = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
                        SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                        SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                        BenhNhanTraLai = o.SoLuongXuat < 0
                    }).ToList();
                var allDataNhapXuat = allDataNhap.Concat(allDataXuat).OrderBy(o => o.NgayThang).ToList();
                var tonDauKy = allDataNhapXuat.Where(o => o.NgayThang < fromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum();
                var allDataNhapXuatTuNgay = allDataNhapXuat.Where(o => o.NgayThang >= fromDate).ToList();
                for (int i = 0; i < allDataNhapXuatTuNgay.Count; i++)
                {
                    if (i == 0)
                    {
                        allDataNhapXuatTuNgay[i].SLTon = tonDauKy + allDataNhapXuatTuNgay[i].SLNhap.GetValueOrDefault() - allDataNhapXuatTuNgay[i].SLXuat.GetValueOrDefault();
                    }
                    else
                    {
                        allDataNhapXuatTuNgay[i].SLTon = allDataNhapXuatTuNgay[i - 1].SLTon + allDataNhapXuatTuNgay[i].SLNhap.GetValueOrDefault() - allDataNhapXuatTuNgay[i].SLXuat.GetValueOrDefault();
                    }
                }
                var baoCaoTheKhoGridVos = allDataNhapXuatTuNgay.GroupBy(o => o.NgayThangDisplay).Select(o => new BaoCaoTheKhoGridVo
                {
                    TongSLNhap = o.Sum(i => i.SLNhap.GetValueOrDefault()),
                    TongSLXuat = o.Sum(i => i.SLXuat.GetValueOrDefault()),
                    NgayThang = o.First().NgayThang.Date,
                    BaoCaoTheKhoChiTiets = o.ToList()
                }).ToArray();
                for (int i = 0; i < baoCaoTheKhoGridVos.Length; i++)
                {
                    if (i == 0)
                    {
                        baoCaoTheKhoGridVos[i].TongSLTonDauKy = tonDauKy;
                        baoCaoTheKhoGridVos[i].TongSLTon = baoCaoTheKhoGridVos[i].TongSLTonDauKy + baoCaoTheKhoGridVos[i].TongSLNhap.GetValueOrDefault() - baoCaoTheKhoGridVos[i].TongSLXuat.GetValueOrDefault();
                    }
                    else
                    {
                        baoCaoTheKhoGridVos[i].TongSLTonDauKy = baoCaoTheKhoGridVos[i - 1].TongSLTon;
                        baoCaoTheKhoGridVos[i].TongSLTon = baoCaoTheKhoGridVos[i].TongSLTonDauKy + baoCaoTheKhoGridVos[i].TongSLNhap.GetValueOrDefault() - baoCaoTheKhoGridVos[i].TongSLXuat.GetValueOrDefault();
                    }
                }

                //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                //    .Take(queryInfo.Take).ToArrayAsync();

                //await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = baoCaoTheKhoGridVos.ToArray(), TotalRowCount = baoCaoTheKhoGridVos.Count() };
            }
            return null;
            */
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            //var query = null;
            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);
            //return new GridDataSource { TotalRowCount = countTask.Result };
            return null;
        }

        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }

            var timKiemNangCaoObj = new BaoCaoTheKhoAdditionalSearchString();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoTheKhoAdditionalSearchString>(queryInfo.AdditionalSearchString);
            }
            var baoCaoTheKhoQueryInfo = new BaoCaoTheKhoQueryInfo
            {
                KhoId = timKiemNangCaoObj.KhoId,
                DuocPhamHoacVatTuBenhVienId = timKiemNangCaoObj.DuocPhamHoacVatTuBenhVienId,
                LoaiDuocPhamHoacVatTu = timKiemNangCaoObj.LoaiDuocPhamHoacVatTu,
                startDate = timKiemNangCaoObj.NgayThang,
                endDate = timKiemNangCaoObj.NgayThang
            };
            var baoCaoTheKhoGridVos = GetDataBaoCaoTheKho(baoCaoTheKhoQueryInfo);
            if (baoCaoTheKhoGridVos != null && baoCaoTheKhoGridVos.Any())
                return new GridDataSource { Data = baoCaoTheKhoGridVos.First().BaoCaoTheKhoChiTiets.ToArray(), TotalRowCount = baoCaoTheKhoGridVos.First().BaoCaoTheKhoChiTiets.Count() };
            else
                return null;
            /*
            if ((Enums.LoaiDuocPhamHoacVatTu)loai == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien)
            {
                var allDataNhap = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Where(o =>
                        o.DuocPhamBenhVienId == duocPhamBenhVienId &&
                        o.NhapKhoDuocPhams.KhoId == khoId && o.NgayNhap <= toDate)
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        TenKho = o.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                        Ten = o.DuocPhamBenhViens.DuocPham.Ten,
                        DVT = o.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        NhapTuKho = o.NhapKhoDuocPhams.XuatKhoDuocPhamId != null ? o.NhapKhoDuocPhams.XuatKhoDuocPham.KhoDuocPhamXuat.Ten : "",
                        TenNhaThau = o.HopDongThauDuocPhams.NhaThau.Ten,
                        SoHopDongThau = o.HopDongThauDuocPhams.SoHopDong,
                        NhapTuNhaCungCap = o.HopDongThauDuocPhams.HeThongTuPhatSinh != null && o.HopDongThauDuocPhams.HeThongTuPhatSinh == true,
                        NgayThang = o.NgayNhap,
                        SCTNhap = o.NhapKhoDuocPhams.SoPhieu,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

                var allDataXuat = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == duocPhamBenhVienId &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == khoId
                                && ((o.NgayXuat != null && o.NgayXuat <= toDate) ||
                                    (o.NgayXuat == null &&
                                     o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= toDate)))
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        Ten = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.Ten,
                        DVT = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                        TenKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.Ten,
                        LoaiXuatKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LoaiXuatKho,
                        XuatQuaKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoNhapId != null
                            ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamNhap.Ten
                            : "",
                        NgayThang = o.NgayXuat != null
                            ? o.NgayXuat.Value
                            : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        //SCTXuat = o.YeuCauTraDuocPhamTuBenhNhanChiTietId == null ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : "",
                        SCTXuat = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu,
                        SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                        SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                        BenhNhanTraLai = o.SoLuongXuat < 0
                    }).ToList();
                var allDataNhapXuat = allDataNhap.Concat(allDataXuat).OrderBy(o => o.NgayThang).ToList();
                var tonDauKy = allDataNhapXuat.Where(o => o.NgayThang < fromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum();
                var allDataNhapXuatTuNgay = allDataNhapXuat.Where(o => o.NgayThang >= fromDate).ToList();
                for (int i = 0; i < allDataNhapXuatTuNgay.Count; i++)
                {
                    if (i == 0)
                    {
                        allDataNhapXuatTuNgay[i].SLTon = tonDauKy + allDataNhapXuatTuNgay[i].SLNhap.GetValueOrDefault() - allDataNhapXuatTuNgay[i].SLXuat.GetValueOrDefault();
                    }
                    else
                    {
                        allDataNhapXuatTuNgay[i].SLTon = allDataNhapXuatTuNgay[i - 1].SLTon + allDataNhapXuatTuNgay[i].SLNhap.GetValueOrDefault() - allDataNhapXuatTuNgay[i].SLXuat.GetValueOrDefault();
                    }
                }
                var grid = allDataNhapXuatTuNgay.Where(s => s.NgayThang.Date == ngayThang.Date).ToList();
                var countTask = grid.Count();
                var queryTask = grid.Skip(queryInfo.Skip)
                    .Take(queryInfo.Take);

                return new GridDataSource { Data = queryTask.ToArray(), TotalRowCount = countTask };
            }
            return null;
            */
        }

        private ICollection<BaoCaoTheKhoGridVo> GetDataBaoCaoTheKho(BaoCaoTheKhoQueryInfo queryInfo)
        {
            var fromDate = queryInfo.startDate.Date;
            var toDate = queryInfo.endDate.Date.AddDays(1).AddMilliseconds(-1);
            if (queryInfo.LoaiDuocPhamHoacVatTu == Enums.LoaiDuocPhamHoacVatTu.DuocPhamBenhVien)
            {
                var thongTinKhoaPhong = _khoaPhongRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
                IQueryable<NhapKhoDuocPhamChiTiet> allDataNhapQuery = null;
                if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoTongDuocPham)
                {
                    allDataNhapQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                        .Where(o =>
                                            o.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                            o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= toDate);
                }
                else
                {
                    allDataNhapQuery = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                        .Where(o =>
                                            o.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId && o.KhoNhapSauKhiDuyetId == null &&
                                            o.NhapKhoDuocPhams.KhoId == queryInfo.KhoId && o.NgayNhap <= toDate);                    
                }

                var allDataNhap = allDataNhapQuery
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        TenKho = o.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                        NhapTuKho = o.NhapKhoDuocPhams.XuatKhoDuocPhamId != null ? o.NhapKhoDuocPhams.XuatKhoDuocPham.KhoDuocPhamXuat.Ten : "",
                        SoHoaDon = o.NhapKhoDuocPhams.XuatKhoDuocPhamId == null ? o.NhapKhoDuocPhams.SoChungTu : "",
                        TenNhaThau = o.HopDongThauDuocPhams.NhaThau.Ten,
                        HopDongThauId = o.HopDongThauDuocPhamId,
                        SoHopDongThau = o.HopDongThauDuocPhams.SoHopDong,
                        //NhapTuNhaCungCap = o.HopDongThauDuocPhams.HeThongTuPhatSinh != null && o.HopDongThauDuocPhams.HeThongTuPhatSinh == true,
                        NgayThang = o.NgayNhap,
                        SCTNhap = o.NhapKhoDuocPhams.SoPhieu,
                        NhapTuXuatKhoId = o.NhapKhoDuocPhams.XuatKhoDuocPhamId,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ToList();

                var nhapTuXuatKhoIds = allDataNhap.Where(o => o.NhapTuXuatKhoId != null && o.NgayThang >= fromDate).Select(o => o.NhapTuXuatKhoId.Value).Distinct().ToList();
                var xuatKhoSauKhiDuyetIds = _xuatKhoDuocPhamRepository.TableNoTracking
                .Where(o => nhapTuXuatKhoIds.Contains(o.Id) && o.KhoXuatId == (long)EnumKhoDuocPham.KhoTongDuocPham && o.LyDoXuatKho == Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet)
                .Select(o => o.Id).ToList();

                foreach (var baoCaoTheKhoChiTietData in allDataNhap)
                {
                    if (baoCaoTheKhoChiTietData.NhapTuXuatKhoId == null || xuatKhoSauKhiDuyetIds.Contains(baoCaoTheKhoChiTietData.NhapTuXuatKhoId.Value))
                    {
                        baoCaoTheKhoChiTietData.NhapTuNhaCungCap = true;
                    }
                }

                var groupDataNhap = allDataNhap.GroupBy(o => new
                    {
                        SCTNhap = o.SCTNhap,
                        TenKho = o.TenKho,
                        NhapTuKho = o.NhapTuKho,
                        TenNhaThau = o.TenNhaThau,
                        HopDongThauId = o.HopDongThauId,
                        SoHopDongThau = o.SoHopDongThau,
                        NhapTuNhaCungCap = o.NhapTuNhaCungCap,
                        NgayThang = o.NgayThang,
                    }, o => o,
                    (k, v) => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = v.First().Id,
                        SCTNhap = k.SCTNhap,
                        NhapTuXuatKhoId = v.First().NhapTuXuatKhoId,
                        SoHoaDon = v.First().SoHoaDon,
                        TenKho = k.TenKho,
                        NhapTuKho = k.NhapTuKho,
                        TenNhaThau = k.TenNhaThau,
                        HopDongThauId = k.HopDongThauId,
                        SoHopDongThau = k.SoHopDongThau,
                        NhapTuNhaCungCap = k.NhapTuNhaCungCap,
                        NgayThang = k.NgayThang,
                        SLNhap = v.Sum(x=>x.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                        SLXuat = 0
                    }).ToList();
                //var nhapTuXuatKhoIds = groupDataNhap.Where(o=>o.NhapTuXuatKhoId != null && o.NgayThang >= fromDate).Select(o => o.NhapTuXuatKhoId.Value).ToList();
                var thongTinXuatKhos = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                nhapTuXuatKhoIds.Contains(o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId.Value))
                    .Select(o => new BaoCaoTheKhoThongTinXuatKhoDeNhapVe
                    {
                        XuatKhoId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId.Value,
                        KhoXuatId = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId,
                        TenKhoXuat = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.Ten,
                        DuocPhamBenhVienId = o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId,
                        HopDongThauId = o.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                        SoHoaDon = o.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.SoChungTu,
                        KhoKhacHoanTra = o.YeuCauTraDuocPhamChiTiets.Any()
                    }).ToList();

                IQueryable<XuatKhoDuocPhamChiTietViTri> allDataXuatQuery = null;
                if (queryInfo.KhoId != (long)EnumKhoDuocPham.KhoTongDuocPham)
                {
                    allDataXuatQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId
                                && ((o.NgayXuat != null && o.NgayXuat <= toDate) ||
                                    (o.NgayXuat == null &&
                                     o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= toDate)));
                }
                else
                {
                    allDataXuatQuery = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                    .Where(o => o.NhapKhoDuocPhamChiTiet.DuocPhamBenhVienId == queryInfo.DuocPhamHoacVatTuBenhVienId &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null &&
                                o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoXuatId == queryInfo.KhoId && o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LyDoXuatKho != Constants.StringXuatNhapKho.LyDoXuatVeKhoSauKhiDuyet
                                && ((o.NgayXuat != null && o.NgayXuat <= toDate) ||
                                    (o.NgayXuat == null &&
                                     o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat <= toDate)));
                }

                var allDataXuat = allDataXuatQuery
                    .Select(o => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = o.Id,
                        XuatKhoDuocPhamChiTietId = o.XuatKhoDuocPhamChiTietId,
                        TenKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamXuat.Ten,
                        LoaiXuatKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.LoaiXuatKho,
                        XuatQuaKho = o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoNhapId != null
                            ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.KhoDuocPhamNhap.Ten
                            : "",
                        NgayThang = o.NgayXuat != null
                            ? o.NgayXuat.Value
                            : o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.NgayXuat,
                        SCTNhap = o.SoLuongXuat < 0 ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : string.Empty,
                        SCTXuat = o.SoLuongXuat > 0 ? o.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : string.Empty,
                        SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                        SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                        //HoTenBenhNhan = o.DonThuocThanhToanChiTiets.Select(c=> c.DonThuocThanhToan.YeuCauTiepNhan.HoTen).FirstOrDefault(),
                        //MaTiepNhan = o.DonThuocThanhToanChiTiets.Select(c => c.DonThuocThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan).FirstOrDefault(),
                        DonThuocThanhToanIds = o.DonThuocThanhToanChiTiets.Select(c => c.DonThuocThanhToanId).ToList(),
                        BenhNhanTraLai = o.SoLuongXuat < 0
                    }).ToList();

                var donThuocThanhToanIds = allDataXuat.SelectMany(o => o.DonThuocThanhToanIds).Select(o => o).Distinct().ToList();
                var thongTinDonThuocs = _donThuocThanhToanRepository.TableNoTracking
                    .Where(o => donThuocThanhToanIds.Contains(o.Id))
                    .Select(o => new
                    {
                        o.Id, 
                        MaYeuCauTiepNhan = o.YeuCauTiepNhan != null ? o.YeuCauTiepNhan.MaYeuCauTiepNhan : "",
                        HoTen = o.YeuCauTiepNhan != null ? o.YeuCauTiepNhan.HoTen : ""
                    }).ToList();

                var xuatKhoChiTietIds = allDataXuat.Select(o => o.XuatKhoDuocPhamChiTietId).Distinct().ToList();
                var yeuCauDuocPhamXuatKhoChiTietData = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.XuatKhoDuocPhamChiTietId != null && xuatKhoChiTietIds.Contains(o.XuatKhoDuocPhamChiTietId.Value))
                    .Select(o => new { o.NoiChiDinh.KhoaPhongId, o.XuatKhoDuocPhamChiTietId, o.LoaiPhieuLinh, o.YeuCauTiepNhan.MaYeuCauTiepNhan, o.YeuCauTiepNhan.HoTen }).ToList();

                foreach (var baoCaoTheKhoChiTietGridVo in allDataXuat)
                {
                    if (baoCaoTheKhoChiTietGridVo.DonThuocThanhToanIds.Any())
                    {
                        var thongTinDonThuoc = thongTinDonThuocs.FirstOrDefault(o => baoCaoTheKhoChiTietGridVo.DonThuocThanhToanIds.Contains(o.Id));
                        if (thongTinDonThuoc != null)
                        {
                            baoCaoTheKhoChiTietGridVo.HoTenBenhNhan = thongTinDonThuoc.HoTen;
                            baoCaoTheKhoChiTietGridVo.MaTiepNhan = thongTinDonThuoc.MaYeuCauTiepNhan;
                        }
                    }
                    var yeuCauDuocPhamXuatKhoChiTiet = yeuCauDuocPhamXuatKhoChiTietData.FirstOrDefault(o => o.XuatKhoDuocPhamChiTietId == baoCaoTheKhoChiTietGridVo.XuatKhoDuocPhamChiTietId);
                    if (yeuCauDuocPhamXuatKhoChiTiet != null)
                    {
                        baoCaoTheKhoChiTietGridVo.HoTenBenhNhan = yeuCauDuocPhamXuatKhoChiTiet.HoTen;
                        baoCaoTheKhoChiTietGridVo.MaTiepNhan = yeuCauDuocPhamXuatKhoChiTiet.MaYeuCauTiepNhan;
                        if (yeuCauDuocPhamXuatKhoChiTiet.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan)
                        {
                            baoCaoTheKhoChiTietGridVo.KhoaLinh = thongTinKhoaPhong.FirstOrDefault(o => o.Id == yeuCauDuocPhamXuatKhoChiTiet.KhoaPhongId)?.Ten;
                        }
                    }
                }

                var groupDataXuat = allDataXuat.GroupBy(o => new
                    {
                        SCTNhap = o.SCTNhap,
                        SCTXuat = o.SCTXuat,
                        TenKho = o.TenKho,
                        LoaiXuatKho = o.LoaiXuatKho,
                        XuatQuaKho = o.XuatQuaKho,
                        NgayThang = o.NgayThang,
                        MaTiepNhan = o.MaTiepNhan,
                        BenhNhanTraLai = o.BenhNhanTraLai,
                    }, o => o,
                    (k, v) => new BaoCaoTheKhoChiTietGridVo
                    {
                        Id = v.First().Id,
                        SCTNhap = k.SCTNhap,
                        SCTXuat = k.SCTXuat,
                        TenKho = k.TenKho,
                        LoaiXuatKho = k.LoaiXuatKho,
                        XuatQuaKho = k.XuatQuaKho,
                        NgayThang = k.NgayThang,
                        SLNhap = v.Sum(x => x.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                        SLXuat = v.Sum(x => x.SLXuat.GetValueOrDefault()).MathRoundNumber(2),
                        HoTenBenhNhan = v.First().HoTenBenhNhan,
                        MaTiepNhan = v.First().MaTiepNhan,
                        KhoaLinh = v.First().KhoaLinh,
                        BenhNhanTraLai = k.BenhNhanTraLai
                    }).ToList();

                var allDataNhapXuat = groupDataNhap.Concat(groupDataXuat).OrderBy(o => o.NgayThang).ToList();
                var tonDauKy = allDataNhapXuat.Where(o => o.NgayThang < fromDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum().MathRoundNumber(2);
                var allDataNhapXuatTuNgay = allDataNhapXuat.Where(o => o.NgayThang >= fromDate).ToList();
                for (int i = 0; i < allDataNhapXuatTuNgay.Count; i++)
                {
                    if (i == 0)
                    {
                        allDataNhapXuatTuNgay[i].SLTon = (tonDauKy + allDataNhapXuatTuNgay[i].SLNhap.GetValueOrDefault() - allDataNhapXuatTuNgay[i].SLXuat.GetValueOrDefault()).MathRoundNumber(2);
                    }
                    else
                    {
                        allDataNhapXuatTuNgay[i].SLTon = (allDataNhapXuatTuNgay[i - 1].SLTon + allDataNhapXuatTuNgay[i].SLNhap.GetValueOrDefault() - allDataNhapXuatTuNgay[i].SLXuat.GetValueOrDefault()).MathRoundNumber(2);
                    }

                    if (allDataNhapXuatTuNgay[i].NhapTuXuatKhoId != null)
                    {
                        var thongTinXuatKho = thongTinXuatKhos.FirstOrDefault(o =>
                            o.XuatKhoId == allDataNhapXuatTuNgay[i].NhapTuXuatKhoId &&
                            o.HopDongThauId == allDataNhapXuatTuNgay[i].HopDongThauId);
                        if (thongTinXuatKho != null)
                        {
                            allDataNhapXuatTuNgay[i].KhoKhacHoanTra = thongTinXuatKho.KhoKhacHoanTra;
                            allDataNhapXuatTuNgay[i].SoHoaDon = thongTinXuatKho.SoHoaDon;
                        }
                    }
                }
                var baoCaoTheKhoGridVos = allDataNhapXuatTuNgay.GroupBy(o => o.NgayThangDisplay).Select(o => new BaoCaoTheKhoGridVo
                {
                    TongSLNhap = o.Sum(i => i.SLNhap.GetValueOrDefault()).MathRoundNumber(2),
                    TongSLXuat = o.Sum(i => i.SLXuat.GetValueOrDefault()).MathRoundNumber(2),
                    NgayThang = o.First().NgayThang.Date,
                    BaoCaoTheKhoChiTiets = o.ToList()
                }).ToArray();
                for (int i = 0; i < baoCaoTheKhoGridVos.Length; i++)
                {
                    if (i == 0)
                    {
                        baoCaoTheKhoGridVos[i].TongSLTonDauKy = tonDauKy;
                        baoCaoTheKhoGridVos[i].TongSLTon = (baoCaoTheKhoGridVos[i].TongSLTonDauKy + baoCaoTheKhoGridVos[i].TongSLNhap.GetValueOrDefault() - baoCaoTheKhoGridVos[i].TongSLXuat.GetValueOrDefault()).MathRoundNumber(2);
                    }
                    else
                    {
                        baoCaoTheKhoGridVos[i].TongSLTonDauKy = baoCaoTheKhoGridVos[i - 1].TongSLTon;
                        baoCaoTheKhoGridVos[i].TongSLTon = (baoCaoTheKhoGridVos[i].TongSLTonDauKy + baoCaoTheKhoGridVos[i].TongSLNhap.GetValueOrDefault() - baoCaoTheKhoGridVos[i].TongSLXuat.GetValueOrDefault()).MathRoundNumber(2);
                    }
                }
                return baoCaoTheKhoGridVos.ToArray();
            }
            return null;
        }
        public virtual byte[] ExportTheKho(BaoCaoTheKhoQueryInfo queryInfo)
        {
            ICollection<BaoCaoTheKhoGridVo> dataBaoCaoTheKho = GetDataBaoCaoTheKho(queryInfo);
            var duocPham = _duocPhamBenhVienRepository.TableNoTracking.Where(p => p.Id == queryInfo.DuocPhamHoacVatTuBenhVienId).Select(p => new
            {
                TenDuocPham = p.DuocPham.Ten,
                p.Ma,
                p.DuocPham.HamLuong,
                DVT = p.DuocPham.DonViTinh.Ten
            }).First();
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("[Dược]Thẻ kho");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 60;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 20;
                    worksheet.Column(8).Width = 20;
                    worksheet.Column(9).Width = 20;

                    worksheet.DefaultColWidth = 7;

                    //SET img 
                    //using (var range = worksheet.Cells["A1:C1"])
                    //{
                    //    var url = hostingName + "/assets/img/logo-bacha-full.png";
                    //    WebClient wc = new WebClient();
                    //    byte[] bytes = wc.DownloadData(url); // download file từ server
                    //    MemoryStream ms = new MemoryStream(bytes); //
                    //    Image img = Image.FromStream(ms); // chuyển đổi thành img
                    //    ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                    //    pic.SetPosition(0, 0, 0, 0);
                    //    var height = 80; // chiều cao từ A1 đến A4
                    //    var width = 510; // chiều rộng từ A1 đến D1
                    //    pic.SetSize(width, height);
                    //    range.Worksheet.Protection.IsProtected = false;
                    //    range.Worksheet.Protection.AllowSelectLockedCells = false;
                    //}

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["A3:H3"])
                    {
                        range.Worksheet.Cells["A3:H3"].Merge = true;
                        range.Worksheet.Cells["A3:H3"].Value = "BÁO CÁO THẺ KHO";
                        range.Worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:H3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:H4"])
                    {
                        range.Worksheet.Cells["A4:H4"].Merge = true;
                        range.Worksheet.Cells["A4:H4"].Value = "Thời gian từ: " + queryInfo.startDate.ApplyFormatDate()
                                                          + " -  " + queryInfo.endDate.ApplyFormatDate();
                        range.Worksheet.Cells["A4:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:H4"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                    }

                    var tenKho = string.Empty;
                    if (queryInfo.KhoId == 0)
                    {
                        tenKho = "Tất cả";
                    }
                    else
                    {
                        tenKho = _khoRepository.TableNoTracking.Where(p => p.Id == queryInfo.KhoId).Select(p => p.Ten).FirstOrDefault();
                    }
                    using (var range = worksheet.Cells["A5:H5"])
                    {
                        range.Worksheet.Cells["A5:H5"].Merge = true;
                        range.Worksheet.Cells["A5:H5"].Value = "Kho: " + tenKho;
                        range.Worksheet.Cells["A5:H5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:H5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:H5"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A5:H5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:H5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A7:H7"])
                    {
                        range.Worksheet.Cells["A7:H7"].Merge = true;
                        range.Worksheet.Cells["A7:H7"].Value = "Tên thuốc, hóa chất, vật tư y tế tiêu hao: " + duocPham.Ma + " - " + duocPham.TenDuocPham + (!string.IsNullOrEmpty(duocPham.HamLuong) ? " - " + duocPham.HamLuong : "");
                        range.Worksheet.Cells["A7:H7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:H7"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A7:H7"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A7:H7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A8:H8"])
                    {
                        range.Worksheet.Cells["A8:H8"].Merge = true;
                        range.Worksheet.Cells["A8:H8"].Value = "Đơn vị: " + duocPham.DVT;
                        range.Worksheet.Cells["A8:H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:H8"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A8:H8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:H8"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A10:I10"])
                    {
                        range.Worksheet.Cells["A10:I10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A10:I10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A10:I10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:I10"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A10:I10"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A10:I10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A10:A11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A10:A11"].Merge = true;
                        range.Worksheet.Cells["A10:A11"].Value = "STT";
                        range.Worksheet.Cells["A10:A11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A10:A11"].Style.Font.Bold = true;


                        range.Worksheet.Cells["B10:B11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B10:B11"].Merge = true;
                        range.Worksheet.Cells["B10:B11"].Value = "Ngày tháng";
                        range.Worksheet.Cells["B10:B11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B10:B11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["C10:D10"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C10:D10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C10:D10"].Merge = true;
                        range.Worksheet.Cells["C10:D10"].Value = "Số chứng từ";
                        range.Worksheet.Cells["C10:D10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C10:D10"].Style.Font.Bold = true;

                        range.Worksheet.Cells["C11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C11"].Value = "Nhập";
                        range.Worksheet.Cells["C11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["D11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D11"].Value = "Xuất";
                        range.Worksheet.Cells["D11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["E10:E11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E10:E11"].Merge = true;
                        range.Worksheet.Cells["E10:E11"].Value = "Diễn giải";
                        range.Worksheet.Cells["E10:E11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E10:E11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["F10:F11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F10:F11"].Merge = true;
                        range.Worksheet.Cells["F10:F11"].Value = "SL Tồn Đầu Kỳ";
                        range.Worksheet.Cells["F10:F11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["F10:F11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["G10:I10"].Merge = true;
                        range.Worksheet.Cells["G10:I10"].Value = "Số lượng";
                        range.Worksheet.Cells["G10:I10"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G10:I10"].Style.Font.Bold = true;

                        range.Worksheet.Cells["G11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G11"].Value = "Nhập";
                        range.Worksheet.Cells["G11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["H11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H11"].Value = "Xuất";
                        range.Worksheet.Cells["H11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["H11"].Style.Font.Bold = true;

                        range.Worksheet.Cells["I11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I11"].Value = "Tồn";
                        range.Worksheet.Cells["I11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["I11"].Style.Font.Bold = true;

                    }
                    int ind = 1;
                    var requestProperties = new[]
                    {
                        new PropertyByName<BaoCaoTheKhoGridVo>("STT", p => ind++)
                    };
                    var manager = new PropertyManager<BaoCaoTheKhoGridVo>(requestProperties);
                    var index = 12; // bắt đầu đổ data từ dòng 12

                    foreach (var data in dataBaoCaoTheKho)
                    {
                        manager.CurrentObject = data;
                        using (var range = worksheet.Cells["A" + index + ":I" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Bold = true;
                        }
                        worksheet.Row(index).Height = 20.5;
                        manager.WriteToXlsx(worksheet, index);
                        using (var range = worksheet.Cells["B" + index + ":E" + index])
                        {
                            range.Worksheet.Cells["B" + index + ":E" + index].Merge = true;
                            range.Worksheet.Cells["B" + index + ":E" + index].Value = data.NgayThangDisplay;
                            range.Worksheet.Cells["B" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        using (var range = worksheet.Cells["F" + index])
                        {
                            range.Worksheet.Cells["F" + index].Value = data.TongSLTonDauKy;
                            range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }

                        using (var range = worksheet.Cells["G" + index])
                        {
                            range.Worksheet.Cells["G" + index].Value = data.TongSLNhap;
                            range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        }

                        using (var range = worksheet.Cells["H" + index])
                        {
                            range.Worksheet.Cells["H" + index].Value = data.TongSLXuat;
                            range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        }

                        using (var range = worksheet.Cells["I" + index])
                        {
                            range.Worksheet.Cells["I" + index].Value = data.TongSLTon;
                            range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }
                        index++;
                        foreach (var chitiet in data.BaoCaoTheKhoChiTiets)
                        {
                            using (var range = worksheet.Cells["A" + index + ":I" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["B" + index])
                            {
                                range.Worksheet.Cells["B" + index].Value = chitiet.NgayThangDisplay;
                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["C" + index])
                            {
                                range.Worksheet.Cells["C" + index].Value = chitiet.SCTNhap;
                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["D" + index])
                            {
                                range.Worksheet.Cells["D" + index].Value = chitiet.SCTXuat;
                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["E" + index])
                            {
                                range.Worksheet.Cells["E" + index].Value = chitiet.DienGiai;
                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["E" + index].Style.WrapText = true;
                            }

                            using (var range = worksheet.Cells["F" + index])
                            {
                                range.Worksheet.Cells["F" + index].Value = chitiet.SLTonDauKy;
                                range.Worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["G" + index])
                            {
                                range.Worksheet.Cells["G" + index].Value = chitiet.SLNhap;
                                range.Worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["H" + index])
                            {
                                range.Worksheet.Cells["H" + index].Value = chitiet.SLXuat;
                                range.Worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["I" + index])
                            {
                                range.Worksheet.Cells["I" + index].Value = chitiet.SLTon;
                                range.Worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }
                            index++;
                        }
                    }
                    index++;
                    using (var range = worksheet.Cells["G" + index + ":I" + index])
                    {
                        range.Worksheet.Cells["G" + index + ":I" + index].Merge = true;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G" + index + ":I" + index].Value = "Người Lập";
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                        index++;
                        range.Worksheet.Cells["G" + index + ":I" + index].Merge = true;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G" + index + ":I" + index].Value = "(ký, ghi rõ họ tên)";
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Bold = true;
                        index += 4;
                        range.Worksheet.Cells["G" + index + ":I" + index].Merge = true;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["G" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G" + index + ":I" + index].Value = "Khoa Dược";
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();

            }
        }

        public async Task<List<LookupItemVo>> GetTatCaKho(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
               nameof(Kho.Ten),
            };

            //if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            //{
            var result = _khoRepository.TableNoTracking
               .Select(s => new LookupItemVo
               {
                   KeyId = s.Id,
                   DisplayName = s.Ten,
               })
               .ApplyLike(queryInfo.Query, o => o.DisplayName)
               .Take(queryInfo.Take);
            return await result.ToListAsync();
            //}
            //else
            //{
            //    var lstKhoId = await _khoRepository
            //      .ApplyFulltext(queryInfo.Query, nameof(Kho), lstColumnNameSearch)
            //      .Select(s => s.Id).ToListAsync();

            //    var dct = lstKhoId.Select((p, i) => new
            //    {
            //        key = p,
            //        rank = i
            //    }).ToDictionary(o => o.key, o => o.rank);

            //    var khos = _khoRepository
            //                        .ApplyFulltext(queryInfo.Query, nameof(Kho), lstColumnNameSearch)
            //     .Select(s => new LookupItemVo
            //     {
            //         KeyId = s.Id,
            //         DisplayName = s.Ten,
            //     })
            //      .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
            //      .Take(queryInfo.Take);
            //    return await khos.ToListAsync();
            //}
        }

        public async Task<List<DuocPhamTheoKhoBaoCaoLookup>> GetDuocPhamTheoKho(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearchDuocPham = new List<string>
            {
               nameof(DuocPham.Ten),
               nameof(DuocPham.HoatChat),
            };
            var lstColumnNameSearchVatTu = new List<string>
            {
               nameof(Core.Domain.Entities.VatTus.VatTu.Ten),
               nameof(Core.Domain.Entities.VatTus.VatTu.Ma),
            };
            if (string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
                {
                    var query = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                       .Select(s => new DuocPhamTheoKhoBaoCaoLookup
                       {
                           KeyId = s.Id,
                           DuocPhamHoacVatTuBenhVienId = s.DuocPhamBenhVienId,
                           DisplayName = s.DuocPhamBenhViens.DuocPham.Ten,
                           Ten = s.DuocPhamBenhViens.DuocPham.Ten,
                           Ma = s.DuocPhamBenhViens.Ma,
                           DVT = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                           LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                       }).GroupBy(x => new { x.DuocPhamHoacVatTuBenhVienId, x.Ma, x.Ten })
                       .Select(item => new DuocPhamTheoKhoBaoCaoLookup()
                       {
                           KeyId = item.First().KeyId,
                           DuocPhamHoacVatTuBenhVienId = item.First().DuocPhamHoacVatTuBenhVienId,
                           DisplayName = item.First().DisplayName,
                           Ten = item.First().Ten,
                           Ma = item.First().Ma,
                           DVT = item.First().DVT,
                           LoaiDuocPhamHoacVatTu = item.First().LoaiDuocPhamHoacVatTu,
                       }).OrderBy(z => z.DuocPhamHoacVatTuBenhVienId).ThenBy(z => z.Ten)
                       .ApplyLike(queryInfo.Query, o => o.Ten, o => o.Ma, o => o.DisplayName)
                       .Take(queryInfo.Take);
                    return await query.ToListAsync();
                }
                else
                {
                    var lstDuocPhamId = await _duocPhamRepository
                     .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearchDuocPham)
                     .Select(s => s.Id).ToListAsync();
                    var dct = lstDuocPhamId.Select((p, i) => new
                    {
                        key = p,
                        rank = i
                    }).ToDictionary(o => o.key, o => o.rank);

                    var lstDuocPhamBV = _duocPhamBenhVienRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearchDuocPham)
                     .Select(s => new DuocPhamTheoKhoBaoCaoLookup
                     {
                         KeyId = s.Id,
                         DuocPhamHoacVatTuBenhVienId = s.Id,
                         DisplayName = s.DuocPham.Ten,
                         Ten = s.DuocPham.Ten,
                         Ma = s.Ma,
                         DVT = s.DuocPham.DonViTinh.Ten,
                         LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                     }).GroupBy(x => new { x.DuocPhamHoacVatTuBenhVienId, x.Ma, x.Ten })
                       .Select(item => new DuocPhamTheoKhoBaoCaoLookup()
                       {
                           KeyId = item.First().KeyId,
                           DuocPhamHoacVatTuBenhVienId = item.First().DuocPhamHoacVatTuBenhVienId,
                           DisplayName = item.First().DisplayName,
                           Ten = item.First().Ten,
                           Ma = item.First().Ma,
                           DVT = item.First().DVT,
                           LoaiDuocPhamHoacVatTu = item.First().LoaiDuocPhamHoacVatTu,
                       }).OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                               .Take(queryInfo.Take);
                    return await lstDuocPhamBV.ToListAsync();
                }
            }
            else
            {
                var info = JsonConvert.DeserializeObject<DuocPhamBaoCaoJsonVo>(queryInfo.ParameterDependencies);
                //if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
                //{
                var query = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Where(p => info.KhoId == 0 || p.NhapKhoDuocPhams.KhoId == info.KhoId)
                   .Select(s => new DuocPhamTheoKhoBaoCaoLookup
                   {
                       KeyId = s.Id,
                       DuocPhamHoacVatTuBenhVienId = s.DuocPhamBenhVienId,
                       DisplayName = s.DuocPhamBenhViens.DuocPham.Ten,
                       Ten = s.DuocPhamBenhViens.DuocPham.Ten,
                       Ma = s.DuocPhamBenhViens.Ma,
                       DVT = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                       LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                   }).GroupBy(x => new { x.DuocPhamHoacVatTuBenhVienId, x.Ma, x.Ten })
                   .Select(item => new DuocPhamTheoKhoBaoCaoLookup()
                   {
                       KeyId = item.First().KeyId,
                       DuocPhamHoacVatTuBenhVienId = item.First().DuocPhamHoacVatTuBenhVienId,
                       DisplayName = item.First().DisplayName,
                       Ten = item.First().Ten,
                       Ma = item.First().Ma,
                       DVT = item.First().DVT,
                       LoaiDuocPhamHoacVatTu = item.First().LoaiDuocPhamHoacVatTu,
                   })
                    .OrderBy(z => z.DuocPhamHoacVatTuBenhVienId).ThenBy(z => z.Ten)
                   .ApplyLike(queryInfo.Query, o => o.Ten, o => o.Ma, o => o.DisplayName)
                   .Take(queryInfo.Take);
                return await query.ToListAsync();
                //}
                //else
                //{
                //    var lstDuocPhamBVTrongNhapKhoChiTiet = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(p => info.KhoId == 0 || p.NhapKhoDuocPhams.KhoId == info.KhoId).ApplyLike(queryInfo.Query, p => p.DuocPhamBenhViens.DuocPham.Ten, p => p.DuocPhamBenhViens.DuocPham.HoatChat).Select(z => z.DuocPhamBenhVienId).Distinct().ToListAsync();

                //    var lstDuocPhamId = await _duocPhamRepository
                //      .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearchDuocPham)
                //      .Where(p => lstDuocPhamBVTrongNhapKhoChiTiet.Contains(p.Id))
                //      .Select(s => s.Id).ToListAsync();

                //    var lstVatTuBVTrongNhapKhoChiTiet = await _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => info.KhoId == 0 || p.NhapKhoVatTu.KhoId == info.KhoId).ApplyLike(queryInfo.Query, p => p.VatTuBenhVien.VatTus.Ten, p => p.VatTuBenhVien.VatTus.Ma).Select(z => z.VatTuBenhVienId).Distinct().ToListAsync();

                //    var lstVatTuId = await _vatTuRepository
                //      .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearchVatTu)
                //      .Where(p => lstVatTuBVTrongNhapKhoChiTiet.Contains(p.Id))
                //      .Select(s => s.Id).ToListAsync();

                //    var lstDuocPhamVaVatTuId = lstDuocPhamId.Concat(lstVatTuId);
                //    var dct = lstDuocPhamVaVatTuId.Select((p, i) => new
                //    {
                //        key = p,
                //        rank = i
                //    }).ToDictionary(o => o.key, o => o.rank);

                //    var lstDuocPhamBV = _nhapKhoDuocPhamChiTietRepository
                //                        .ApplyFulltext(queryInfo.Query, nameof(DuocPham), lstColumnNameSearchDuocPham)
                //                        .Where(p => info.KhoId == 0 || p.NhapKhoDuocPhams.KhoId == info.KhoId)
                //     .Select(s => new DuocPhamTheoKhoBaoCaoLookup
                //     {
                //         KeyId = s.Id,
                //         DuocPhamHoacVatTuBenhVienId = s.DuocPhamBenhVienId,
                //         DisplayName = s.DuocPhamBenhViens.DuocPham.Ten,
                //         Ten = s.DuocPhamBenhViens.DuocPham.Ten,
                //         Ma = s.DuocPhamBenhViens.DuocPham.MaHoatChat,
                //         DVT = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                //         LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                //     }).GroupBy(x => new { x.DuocPhamHoacVatTuBenhVienId, x.Ma, x.Ten })
                //       .Select(item => new DuocPhamTheoKhoBaoCaoLookup()
                //       {
                //           KeyId = item.First().KeyId,
                //           DuocPhamHoacVatTuBenhVienId = item.Select(z => z.DuocPhamHoacVatTuBenhVienId).FirstOrDefault(),
                //           DisplayName = item.First().DisplayName,
                //           Ten = item.First().Ten,
                //           Ma = item.First().Ma,
                //           DVT = item.First().DVT,
                //           LoaiDuocPhamHoacVatTu = item.First().LoaiDuocPhamHoacVatTu,
                //       }).Union(
                //        _nhapKhoVatTuChiTietRepository
                //                        .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearchVatTu)
                //                        .Where(p => info.KhoId == 0 || p.NhapKhoVatTu.KhoId == info.KhoId)
                //             .Select(s => new DuocPhamTheoKhoBaoCaoLookup
                //             {
                //                 KeyId = s.Id,
                //                 DuocPhamHoacVatTuBenhVienId = s.VatTuBenhVienId,
                //                 DisplayName = s.VatTuBenhVien.VatTus.Ten,
                //                 Ten = s.VatTuBenhVien.VatTus.Ten,
                //                 Ma = s.VatTuBenhVien.VatTus.Ma,
                //                 DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                //                 LoaiDuocPhamHoacVatTu = LoaiDuocPhamHoacVatTu.DuocPhamBenhVien
                //             }).GroupBy(x => new { x.DuocPhamHoacVatTuBenhVienId, x.Ma, x.Ten })
                //               .Select(item => new DuocPhamTheoKhoBaoCaoLookup()
                //               {
                //                   KeyId = item.First().KeyId,
                //                   DuocPhamHoacVatTuBenhVienId = item.Select(z => z.DuocPhamHoacVatTuBenhVienId).FirstOrDefault(),
                //                   DisplayName = item.First().DisplayName,
                //                   Ten = item.First().Ten,
                //                   Ma = item.First().Ma,
                //                   DVT = item.First().DVT,
                //                   LoaiDuocPhamHoacVatTu = item.First().LoaiDuocPhamHoacVatTu,
                //               })
                //               ).OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                //               .Take(queryInfo.Take);
                //return await lstDuocPhamBV.ToListAsync();
                //}
            }
        }
    }
}
