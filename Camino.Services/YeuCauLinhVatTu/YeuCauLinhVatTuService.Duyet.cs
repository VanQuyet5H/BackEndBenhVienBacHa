using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial class YeuCauLinhVatTuService
    {
        #region Get data 

        public async Task<List<LookupItemVo>> GetListNhanVienAsync(DropDownListRequestModel model)
        {
            var lstNhanVien = await
                _userRepository.TableNoTracking.Where(x => x.Id == model.Id)
                    .Select(item => new LookupItemVo()
                    {
                        DisplayName = item.HoTen,
                        KeyId = item.Id
                    }).Union(
                        _userRepository.TableNoTracking
                            .Where(x => x.IsActive && x.Id != model.Id)
                            .Select(item => new LookupItemVo()
                            {
                                DisplayName = item.HoTen,
                                KeyId = item.Id
                            }))
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName)
                    .Distinct()
                    .Take(model.Take)
                    .ToListAsync();
            return lstNhanVien;
        }

        public async Task<GridDataSource> GetVatTuYeuCauLinhBuDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            //var quyeryObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauLinhVatTuId = long.Parse(queryInfo.AdditionalSearchString);

            //var isTuChoiDuyet = false;
            //if (quyeryObj.Length > 1)
            //{
            //    isTuChoiDuyet = quyeryObj[1] == "FALSE";
            //}

            //var yeuCauLinhVatTuId = long.Parse(queryInfo.AdditionalSearchString);
            var yeuCauLinhVatTu =
                await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhVatTuId);

            var queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId)
                    .Select(item => new YeuCauLinhBuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.VatTuBenhVienId,
                        LaBHYT = item.LaVatTuBHYT,
                        Nhom = item.LaVatTuBHYT ? "Vật tư BHYT" : "Vật tư không BHYT",
                        TenVatTu = item.YeuCauVatTuBenhVien.Ten,
                        DonViTinh = (item.YeuCauVatTuBenhVien.DonViTinh ?? ""), //item.VatTuBenhVien.VatTus.DonViTinh,
                        HangSanXuat = (item.YeuCauVatTuBenhVien.NhaSanXuat ?? "").Trim(),
                        NuocSanXuat = (item.YeuCauVatTuBenhVien.NuocSanXuat ?? "").Trim(),
                        SoLuongCanBu = item.SoLuong
                    })
                    .GroupBy(x => new
                    {
                        x.YeuCauLinhVatTuId,
                        x.VatTuBenhVienId,
                        x.LaBHYT,
                        x.Nhom,
                        x.DuongDung,
                        x.DonViTinh,
                        x.HangSanXuat,
                        x.NuocSanXuat,
                        x.SoLuongTon
                    })
                    .Select(item => new YeuCauLinhBuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.First().VatTuBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenVatTu = item.First().TenVatTu,
                        Nhom = item.First().Nhom,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                        SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                        && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                        && x.NhapKhoVatTu.DaHet != true
                                        && x.LaVatTuBHYT == item.First().LaBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                        //BVHD-3806
                        KhongHighLight = yeuCauLinhVatTu.DuocDuyet != null
                    })
                    .OrderByDescending(x => x.HighLightClass).ThenBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();

            //IQueryable<YeuCauLinhBuVatTuGridParentVo> queryable = null;

            //if (!isTuChoiDuyet)
            //{
            //    queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
            //        .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId
            //                    && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
            //        .Select(item => new YeuCauLinhBuVatTuGridParentVo()
            //        {
            //            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
            //            VatTuBenhVienId = item.VatTuBenhVienId,
            //            LaBHYT = item.LaVatTuBHYT,
            //            TenVatTu = item.Ten,
            //            Nhom = item.LaVatTuBHYT ? "Vật tư BHYT" : "Vật tư không BHYT",
            //            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
            //            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
            //            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
            //            SoLuongCanBu = item.SoLuong
            //        })
            //        .GroupBy(x => new
            //        {
            //            x.YeuCauLinhVatTuId, x.VatTuBenhVienId, x.LaBHYT, x.Nhom, x.NongDoHamLuong, x.HoatChat,
            //            x.DuongDung, x.DonViTinh, x.HangSanXuat, x.NuocSanXuat, x.SoLuongTon
            //        })
            //        .Select(item => new YeuCauLinhBuVatTuGridParentVo()
            //        {
            //            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
            //            VatTuBenhVienId = item.First().VatTuBenhVienId,
            //            LaBHYT = item.First().LaBHYT,
            //            TenVatTu = item.First().TenVatTu,
            //            Nhom = item.First().Nhom,
            //            DuongDung = item.First().DuongDung,
            //            DonViTinh = item.First().DonViTinh,
            //            HangSanXuat = item.First().HangSanXuat,
            //            NuocSanXuat = item.First().NuocSanXuat,
            //            SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
            //            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
            //                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
            //                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
            //                            && x.NhapKhoVatTu.DaHet != true
            //                            && x.LaVatTuBHYT == item.First().LaBHYT
            //                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
            //        })
            //        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
            //}
            //else
            //{
            //    queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
            //        .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId)
            //        .Select(item => new YeuCauLinhBuVatTuGridParentVo()
            //        {
            //            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
            //            VatTuBenhVienId = item.VatTuBenhVienId,
            //            LaBHYT = item.LaVatTuBHYT,
            //            Nhom = item.LaVatTuBHYT ? "Vật tư BHYT" : "Vật tư không BHYT",
            //            TenVatTu = item.VatTuBenhVien.VatTus.Ten,
            //            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
            //            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
            //            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
            //            SoLuongCanBu = item.SoLuong
            //        })
            //        .GroupBy(x => new
            //        {
            //            x.YeuCauLinhVatTuId,
            //            x.VatTuBenhVienId,
            //            x.LaBHYT,
            //            x.Nhom,
            //            x.DuongDung,
            //            x.DonViTinh,
            //            x.HangSanXuat,
            //            x.NuocSanXuat,
            //            x.SoLuongTon
            //        })
            //        .Select(item => new YeuCauLinhBuVatTuGridParentVo()
            //        {
            //            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
            //            VatTuBenhVienId = item.First().VatTuBenhVienId,
            //            LaBHYT = item.First().LaBHYT,
            //            TenVatTu = item.First().TenVatTu,
            //            Nhom = item.First().Nhom,
            //            DuongDung = item.First().DuongDung,
            //            DonViTinh = item.First().DonViTinh,
            //            HangSanXuat = item.First().HangSanXuat,
            //            NuocSanXuat = item.First().NuocSanXuat,
            //            SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
            //            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
            //                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
            //                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
            //                            && x.NhapKhoVatTu.DaHet != true
            //                            && x.LaVatTuBHYT == item.First().LaBHYT
            //                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
            //        })
            //        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
            //}

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
            var queryTask = queryable.OrderBy(queryInfo.SortString)
                //.Skip(queryInfo.Skip)
                //.Take(queryInfo.Take)
                .ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetVatTuYeuCauLinhBuTotalPageForGridAsync(QueryInfo queryInfo)
        {
            //var quyeryObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauLinhVatTuId = long.Parse(queryInfo.AdditionalSearchString);

            //var isTuChoiDuyet = false;
            //if (quyeryObj.Length > 1)
            //{
            //    isTuChoiDuyet = quyeryObj[1] == "FALSE";
            //}

            //var yeuCauLinhVatTuId = long.Parse(queryInfo.AdditionalSearchString);
            var yeuCauLinhVatTu =
                await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhVatTuId);

            var queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId)
                    .Select(item => new YeuCauLinhBuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.VatTuBenhVienId,
                        LaBHYT = item.LaVatTuBHYT,
                        Nhom = item.LaVatTuBHYT ? "Vật tư BHYT" : "Vật tư không BHYT",
                        TenVatTu = item.YeuCauVatTuBenhVien.Ten,
                        DonViTinh = (item.YeuCauVatTuBenhVien.DonViTinh ?? ""), //item.VatTuBenhVien.VatTus.DonViTinh,
                        HangSanXuat = (item.YeuCauVatTuBenhVien.NhaSanXuat ?? "").Trim(),
                        NuocSanXuat = (item.YeuCauVatTuBenhVien.NuocSanXuat ?? "").Trim(),
                        SoLuongCanBu = item.SoLuong
                    })
                    .GroupBy(x => new
                    {
                        x.YeuCauLinhVatTuId,
                        x.VatTuBenhVienId,
                        x.LaBHYT,
                        x.Nhom,
                        x.DuongDung,
                        x.DonViTinh,
                        x.HangSanXuat,
                        x.NuocSanXuat,
                        x.SoLuongTon
                    })
                    .Select(item => new YeuCauLinhBuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.First().VatTuBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenVatTu = item.First().TenVatTu,
                        Nhom = item.First().Nhom,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                        SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                        && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                        && x.NhapKhoVatTu.DaHet != true
                                        && x.LaVatTuBHYT == item.First().LaBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();

            //IQueryable<YeuCauLinhBuVatTuGridParentVo> queryable = null;

            //if (!isTuChoiDuyet)
            //{
            //    queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
            //        .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId
            //                    && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
            //        .Select(item => new YeuCauLinhBuVatTuGridParentVo()
            //        {
            //            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
            //            VatTuBenhVienId = item.VatTuBenhVienId,
            //            LaBHYT = item.LaVatTuBHYT,
            //            TenVatTu = item.Ten,
            //            Nhom = item.LaVatTuBHYT ? "Vật tư BHYT" : "Vật tư không BHYT",
            //            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
            //            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
            //            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
            //            SoLuongCanBu = item.SoLuong
            //        })
            //        .GroupBy(x => new
            //        {
            //            x.YeuCauLinhVatTuId,
            //            x.VatTuBenhVienId,
            //            x.LaBHYT,
            //            x.Nhom,
            //            x.NongDoHamLuong,
            //            x.HoatChat,
            //            x.DuongDung,
            //            x.DonViTinh,
            //            x.HangSanXuat,
            //            x.NuocSanXuat,
            //            x.SoLuongTon
            //        })
            //        .Select(item => new YeuCauLinhBuVatTuGridParentVo()
            //        {
            //            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
            //            VatTuBenhVienId = item.First().VatTuBenhVienId,
            //            LaBHYT = item.First().LaBHYT,
            //            TenVatTu = item.First().TenVatTu,
            //            Nhom = item.First().Nhom,
            //            DuongDung = item.First().DuongDung,
            //            DonViTinh = item.First().DonViTinh,
            //            HangSanXuat = item.First().HangSanXuat,
            //            NuocSanXuat = item.First().NuocSanXuat,
            //            SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
            //            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
            //                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
            //                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
            //                            && x.NhapKhoVatTu.DaHet != true
            //                            && x.LaVatTuBHYT == item.First().LaBHYT
            //                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
            //        })
            //        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
            //}
            //else
            //{
            //    queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
            //        .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId)
            //        .Select(item => new YeuCauLinhBuVatTuGridParentVo()
            //        {
            //            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
            //            VatTuBenhVienId = item.VatTuBenhVienId,
            //            LaBHYT = item.LaVatTuBHYT,
            //            Nhom = item.LaVatTuBHYT ? "Vật tư BHYT" : "Vật tư không BHYT",
            //            TenVatTu = item.VatTuBenhVien.VatTus.Ten,
            //            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
            //            HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
            //            NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
            //            SoLuongCanBu = item.SoLuong
            //        })
            //        .GroupBy(x => new
            //        {
            //            x.YeuCauLinhVatTuId,
            //            x.VatTuBenhVienId,
            //            x.LaBHYT,
            //            x.Nhom,
            //            x.DuongDung,
            //            x.DonViTinh,
            //            x.HangSanXuat,
            //            x.NuocSanXuat,
            //            x.SoLuongTon
            //        })
            //        .Select(item => new YeuCauLinhBuVatTuGridParentVo()
            //        {
            //            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
            //            VatTuBenhVienId = item.First().VatTuBenhVienId,
            //            LaBHYT = item.First().LaBHYT,
            //            TenVatTu = item.First().TenVatTu,
            //            Nhom = item.First().Nhom,
            //            DuongDung = item.First().DuongDung,
            //            DonViTinh = item.First().DonViTinh,
            //            HangSanXuat = item.First().HangSanXuat,
            //            NuocSanXuat = item.First().NuocSanXuat,
            //            SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
            //            SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
            //                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
            //                            && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
            //                            && x.NhapKhoVatTu.DaHet != true
            //                            && x.LaVatTuBHYT == item.First().LaBHYT
            //                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
            //        })
            //        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
            //}

            var countTask = queryable.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetBenhNhanTheoVatTuCanLinhBuDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauLinhId = long.Parse(queryObj[0]);
            var VatTuBenhVienId = long.Parse(queryObj[1]);
            bool laBHYT = bool.Parse(queryObj[2]);

            //var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
            //    .Where(o => o.YeuCauLinhVatTuId != null
            //                && o.YeuCauLinhVatTuId == yeuCauLinhId
            //                && o.VatTuBenhVienId == VatTuBenhVienId
            //                && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
            //                && o.LaVatTuBHYT == laBHYT
            //                && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
            //    .OrderBy(x => x.ThoiDiemChiDinh)
            //    .Select(s => new YeuCauLinhVatTuBuGridChildVo
            //    {
            //        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //        MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
            //        HoTen = s.YeuCauTiepNhan.HoTen,
            //        SoLuong = s.SoLuong,
            //        DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
            //        BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
            //        NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH()
            //    });

            //BVHD-3806
            var yeuCauLinhVatTu =
                _yeuCauLinhVatTuRepository.TableNoTracking
                    .First(x => x.Id == yeuCauLinhId);
            var nhapKhoChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                            && x.NhapKhoVatTu.DaHet != true
                            && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                .Where(o => o.YeuCauLinhVatTuId == yeuCauLinhId
                            && o.VatTuBenhVienId == VatTuBenhVienId
                            && o.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                            && o.LaVatTuBHYT == laBHYT
                            && o.YeuCauVatTuBenhVien.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                .Select(s => new YeuCauLinhVatTuBuGridChildVo
                {
                    MaYeuCauTiepNhan = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                    SoLuong = s.SoLuong,
                    DichVuKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh != null ? 
                        s.YeuCauVatTuBenhVien.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu : (s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? 
                            s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                    BacSiKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh,

                    //BVHD-3806
                    SoLuongTonTheoDichVu = nhapKhoChiTiets.Where(x => x.VatTuBenhVienId == s.VatTuBenhVienId
                                                                      && x.LaVatTuBHYT == s.LaVatTuBHYT)
                        .Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    KhongHighLight = yeuCauLinhVatTu.DuocDuyet != null
                });
                //.GroupBy(x => new
                //{
                //    x.MaYeuCauTiepNhan,
                //    x.MaBenhNhan,
                //    x.HoTen,
                //    x.DichVuKham,
                //    x.BacSiKeToa,
                //    x.NgayKe
                //})
                //.Select(s => new YeuCauLinhVatTuBuGridChildVo
                //{
                //    MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                //    MaBenhNhan = s.First().MaBenhNhan,
                //    HoTen = s.First().HoTen,
                //    SoLuong = s.Sum(a => a.SoLuong),
                //    DichVuKham = s.First().DichVuKham,
                //    BacSiKeToa = s.First().BacSiKeToa,
                //    NgayKe = s.First().NgayKe
                //});

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetBenhNhanTheoVatTuCanLinhBuTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauLinhId = long.Parse(queryObj[0]);
            var VatTuBenhVienId = long.Parse(queryObj[1]);
            bool laBHYT = bool.Parse(queryObj[2]);

            //var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
            //    .Where(o => o.YeuCauLinhVatTuId != null
            //                && o.YeuCauLinhVatTuId == yeuCauLinhId
            //                && o.VatTuBenhVienId == VatTuBenhVienId
            //                && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
            //                && o.LaVatTuBHYT == laBHYT
            //                && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
            //    .OrderBy(x => x.ThoiDiemChiDinh)
            //    .Select(s => new YeuCauLinhVatTuBuGridChildVo
            //    {
            //        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //        MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
            //        HoTen = s.YeuCauTiepNhan.HoTen,
            //        SoLuong = s.SoLuong,
            //        DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
            //        BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
            //        NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH()
            //    });

            var query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                .Where(o => o.YeuCauLinhVatTuId == yeuCauLinhId
                            && o.VatTuBenhVienId == VatTuBenhVienId
                            && o.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                            && o.LaVatTuBHYT == laBHYT
                            && o.YeuCauVatTuBenhVien.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                .Select(s => new YeuCauLinhVatTuBuGridChildVo
                {
                    MaYeuCauTiepNhan = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                    SoLuong = s.SoLuong,
                    DichVuKham = s.YeuCauVatTuBenhVien.YeuCauKhamBenh != null ?
                        s.YeuCauVatTuBenhVien.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu : (s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ?
                            s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                    BacSiKeToa = s.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    NgayDieuTri = s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauVatTuBenhVien.ThoiDiemChiDinh
                });
                //.GroupBy(x => new
                //{
                //    x.MaYeuCauTiepNhan,
                //    x.MaBenhNhan,
                //    x.HoTen,
                //    x.DichVuKham,
                //    x.BacSiKeToa,
                //    x.NgayKe
                //})
                //.Select(s => new YeuCauLinhVatTuBuGridChildVo
                //{
                //    MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                //    MaBenhNhan = s.First().MaBenhNhan,
                //    HoTen = s.First().HoTen,
                //    SoLuong = s.Sum(a => a.SoLuong),
                //    DichVuKham = s.First().DichVuKham,
                //    BacSiKeToa = s.First().BacSiKeToa,
                //    NgayKe = s.First().NgayKe
                //});
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<GridDataSource> GetVatTuYeuCauLinhTrucTiepDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var quyeryObj = queryInfo.AdditionalSearchString.Split(';');
            var subObj = quyeryObj[0].Split('|');
            var yeuCauLinhVatTuId = long.Parse(subObj[0]);

            var isTuChoiDuyet = false;
            if (quyeryObj.Length > 1)
            {
                isTuChoiDuyet = quyeryObj[1] == "FALSE";
            }

            var yeuCauLinhVatTu =
                 _yeuCauLinhVatTuRepository.TableNoTracking
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .First(x => x.Id == yeuCauLinhVatTuId);
            var nhapKhoChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                            && x.NhapKhoVatTu.DaHet != true

                            //BVHD-3821
                            // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                            && x.HanSuDung.Date >= DateTime.Now.Date
                ).ToList();

            IQueryable<YeuCauLinhVatTuVatTuGridParentVo> queryable = null;

            // cập nhật 08/11/2021: trường hợp đã duyệt thì lấy thông tin từ YeCauLinhVatTuCHiTiet
            if (!isTuChoiDuyet)
            {
                long yeuCauTiepNhanId = 0;
                if (subObj.Length > 1)
                {
                    yeuCauTiepNhanId = long.Parse(subObj[1]);
                }

                if (yeuCauLinhVatTu.DuocDuyet != true)
                {
                    queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId
                                    && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                    && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                    && x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                        .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                        {
                            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                            VatTuBenhVienId = item.VatTuBenhVienId,
                            LaBHYT = item.LaVatTuBHYT,
                            TenVatTu = item.Ten,
                            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                            HangSanXuat = item.NhaSanXuat,
                            NuocSanXuat = item.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,

                            DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                            NgayKe = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                            NgayDieuTri = item.NoiTruPhieuDieuTri != null ? item.NoiTruPhieuDieuTri.NgayDieuTri : item.ThoiDiemChiDinh
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.TenVatTu,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.DichVuKham,
                            x.BacSiKeToa,
                            x.NgayKe
                        })
                        .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                        {
                            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenVatTu = item.First().TenVatTu,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                        //SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        //    .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                        //                && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                        //                && x.LaVatTuBHYT == item.First().LaBHYT
                        //                && x.NhapKhoVatTu.DaHet != true
                        //                && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                        SoLuongTon = nhapKhoChiTiets
                                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                            && x.LaVatTuBHYT == item.First().LaBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            SoLuongTonTheoDuocPham = nhapKhoChiTiets
                                                         .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                                                     && x.LaVatTuBHYT == item.First().LaBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                                                     - yeuCauLinhVatTu.YeuCauVatTuBenhViens.Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                                                                                             && x.LaVatTuBHYT == item.First().LaBHYT
                                                                                                             && x.YeuCauTiepNhanId != yeuCauTiepNhanId
                                                                                                             && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                                                                                             && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                                                         .Sum(x => x.SoLuong),

                            DichVuKham = item.First().DichVuKham,
                            BacSiKeToa = item.First().BacSiKeToa,
                            NgayKe = item.First().NgayKe,
                            NgayDieuTri = item.First().NgayDieuTri,

                            // trường hợp đã duyệt, từ chối thì ko cần highlight
                            KhongHighLight = yeuCauLinhVatTu.DuocDuyet != null
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                }
                else
                {
                    queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId
                                    && x.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                    && x.YeuCauVatTuBenhVien.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                    && x.YeuCauVatTuBenhVien.YeuCauTiepNhanId == yeuCauTiepNhanId)
                        .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                        {
                            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                            VatTuBenhVienId = item.VatTuBenhVienId,
                            LaBHYT = item.LaVatTuBHYT,
                            TenVatTu = item.YeuCauVatTuBenhVien.Ten,
                            DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                            HangSanXuat = item.YeuCauVatTuBenhVien.NhaSanXuat,
                            NuocSanXuat = item.YeuCauVatTuBenhVien.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,

                            DichVuKham = item.YeuCauVatTuBenhVien.YeuCauKhamBenh != null 
                                ? item.YeuCauVatTuBenhVien.YeuCauKhamBenh.TenDichVu 
                                : (item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauVatTuBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = item.YeuCauVatTuBenhVien.NhanVienChiDinh.User.HoTen,
                            NgayKe = item.YeuCauVatTuBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                            NgayDieuTri = item.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri != null ? item.YeuCauVatTuBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : item.YeuCauVatTuBenhVien.ThoiDiemChiDinh
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.TenVatTu,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.DichVuKham,
                            x.BacSiKeToa,
                            x.NgayKe
                        })
                        .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                        {
                            YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenVatTu = item.First().TenVatTu,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),

                            SoLuongTon = nhapKhoChiTiets
                                .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                            && x.LaVatTuBHYT == item.First().LaBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            SoLuongTonTheoDuocPham = nhapKhoChiTiets
                                                         .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                                                     && x.LaVatTuBHYT == item.First().LaBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                                                     - yeuCauLinhVatTu.YeuCauVatTuBenhViens.Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                                                                                             && x.LaVatTuBHYT == item.First().LaBHYT
                                                                                                             && x.YeuCauTiepNhanId != yeuCauTiepNhanId
                                                                                                             && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                                                                                             && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                                                         .Sum(x => x.SoLuong),

                            DichVuKham = item.First().DichVuKham,
                            BacSiKeToa = item.First().BacSiKeToa,
                            NgayKe = item.First().NgayKe,
                            NgayDieuTri = item.First().NgayDieuTri,

                            // trường hợp đã duyệt, từ chối thì ko cần highlight
                            KhongHighLight = yeuCauLinhVatTu.DuocDuyet != null
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
                }

                if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "desc";
                    queryInfo.Sort[0].Field = "HighLightClass";
                }
            }
            else
            {
                queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId)
                    .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.VatTuBenhVienId,
                        LaBHYT = item.LaVatTuBHYT,
                        TenVatTu = item.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                        HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                        SoLuongYeuCau = item.SoLuong
                    })
                    .GroupBy(x => new
                    {
                        x.YeuCauLinhVatTuId,
                        x.VatTuBenhVienId,
                        x.LaBHYT,
                        x.Nhom,
                        x.DonViTinh,
                        x.HangSanXuat,
                        x.NuocSanXuat,
                        x.SoLuongYeuCau
                    })
                    .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.First().VatTuBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenVatTu = item.First().TenVatTu,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongYeuCau = item.First().SoLuongYeuCau,
                        SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                        && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                                        && x.NhapKhoVatTu.DaHet != true
                                        && x.LaVatTuBHYT == item.First().LaBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                        // trường hợp đã duyệt, từ chối thì ko cần highlight
                        KhongHighLight = yeuCauLinhVatTu.DuocDuyet != null
                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
            var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetVatTuYeuCauLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var quyeryObj = queryInfo.AdditionalSearchString.Split(';');
            var subObj = quyeryObj[0].Split('|');
            var yeuCauLinhVatTuId = long.Parse(subObj[0]);

            var isTuChoiDuyet = false;
            if (quyeryObj.Length > 1)
            {
                isTuChoiDuyet = quyeryObj[1] == "FALSE";
            }

            IQueryable<YeuCauLinhVatTuVatTuGridParentVo> queryable = null;
            if (!isTuChoiDuyet)
            {
                long yeuCauTiepNhanId = 0;
                if (subObj.Length > 1)
                {
                    yeuCauTiepNhanId = long.Parse(subObj[1]);
                }
                queryable = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId
                                && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                && x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                    .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.VatTuBenhVienId,
                        LaBHYT = item.LaVatTuBHYT,
                        TenVatTu = item.Ten,
                        DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                        HangSanXuat = item.NhaSanXuat,
                        NuocSanXuat = item.NuocSanXuat,
                        SoLuongYeuCau = item.SoLuong,

                        DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                        BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                        NgayKe = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH()
                    })
                    .GroupBy(x => new
                    {
                        x.YeuCauLinhVatTuId,
                        x.VatTuBenhVienId,
                        x.LaBHYT,
                        x.Nhom,
                        x.TenVatTu,
                        x.DonViTinh,
                        x.HangSanXuat,
                        x.NuocSanXuat,
                        x.DichVuKham,
                        x.BacSiKeToa,
                        x.NgayKe
                    })
                    .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.First().VatTuBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenVatTu = item.First().TenVatTu,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                        DichVuKham = item.First().DichVuKham,
                        BacSiKeToa = item.First().BacSiKeToa,
                        NgayKe = item.First().NgayKe
                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
            }
            else
            {
                queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId)
                    .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.VatTuBenhVienId,
                        LaBHYT = item.LaVatTuBHYT,
                        TenVatTu = item.VatTuBenhVien.VatTus.Ten,
                        DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                        HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                        NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                        SoLuongYeuCau = item.SoLuong
                    })
                    .GroupBy(x => new
                    {
                        x.YeuCauLinhVatTuId,
                        x.VatTuBenhVienId,
                        x.LaBHYT,
                        x.Nhom,
                        x.DonViTinh,
                        x.HangSanXuat,
                        x.NuocSanXuat,
                        x.SoLuongYeuCau
                    })
                    .Select(item => new YeuCauLinhVatTuVatTuGridParentVo()
                    {
                        YeuCauLinhVatTuId = yeuCauLinhVatTuId,
                        VatTuBenhVienId = item.First().VatTuBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenVatTu = item.First().TenVatTu,
                        //Nhom = item.First().Nhom,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongYeuCau = item.First().SoLuongYeuCau,
                        //SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        //    .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                        //                && x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                        //                && x.NhapKhoVatTu.DaHet != true
                        //                && x.LaVatTuBHYT == item.First().LaBHYT
                        //                && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
            }
            var countTask = queryable.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetBenhNhanTheoVatTuCanLinhTrucTiepDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauLinhId = long.Parse(queryInfo.AdditionalSearchString);

            var yeuCauLinhVatTu =
                _yeuCauLinhVatTuRepository.TableNoTracking
                    .Include(x => x.YeuCauVatTuBenhViens)
                    .First(x => x.Id == yeuCauLinhId);
            var nhapKhoChiTiets = _nhapKhoVatTuChiTietRepository.TableNoTracking
                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                            && x.NhapKhoVatTu.DaHet != true

                            //BVHD-3821
                            // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                            && x.HanSuDung.Date >= DateTime.Now.Date
                ).ToList();

            IQueryable<YeuCauLinhVatTuTrucTiepGridChildVo> query = null;
            // cập nhật 08/11/2021: trường hợp chưa duyệt: thì lấy số lượng theo YCVT
            if (yeuCauLinhVatTu.DuocDuyet != true)
            {
                query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(o => o.YeuCauLinhVatTuId == yeuCauLinhId
                                && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                && o.YeuCauTiepNhan.BenhNhanId != null)
                    .OrderBy(x => x.ThoiDiemChiDinh)
                    .Select(s => new YeuCauLinhVatTuTrucTiepGridChildVo
                    {
                    //BenhNhanId = s.YeuCauTiepNhan.BenhNhanId.Value,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauTiepNhan.HoTen,
                        SoLuong = s.SoLuong,
                    //DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                    //BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                    //NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH()

                    SoLuongTonTheoDichVu = (nhapKhoChiTiets
                                                    .Where(x => x.VatTuBenhVienId == s.VatTuBenhVienId
                                                                && x.LaVatTuBHYT == s.LaVatTuBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                                                - yeuCauLinhVatTu.YeuCauVatTuBenhViens.Where(x => x.VatTuBenhVienId == s.VatTuBenhVienId
                                                                                                        && x.LaVatTuBHYT == s.LaVatTuBHYT
                                                                                                        && x.Id != s.Id
                                                                                                        && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                                                                                        && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                                                    .Sum(x => x.SoLuong)),
                    }).GroupBy(x => new
                    {
                        x.YeuCauTiepNhanId
                    //x.BenhNhanId,
                    //x.MaYeuCauTiepNhan,
                    //x.HoTen
                    }).Select(s => new YeuCauLinhVatTuTrucTiepGridChildVo
                {
                    //BenhNhanId = s.First().BenhNhanId,
                    YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                    MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                    MaBenhNhan = s.First().MaBenhNhan,
                    HoTen = s.First().HoTen,
                    SoLuong = s.Sum(a => a.SoLuong),

                    // trường hợp đã duyệt, từ chối thì ko cần highlight
                    HighLightClass = (s.Any(a => a.KhongDuTon) && yeuCauLinhVatTu.DuocDuyet == null) ? "bg-row-lightRed" : ""
                    //DichVuKham = s.First().DichVuKham,
                    //BacSiKeToa = s.First().BacSiKeToa,
                    //NgayKe = s.First().NgayKe
                });
            }
            // cập nhật 08/11/2021: trường hợp đã duyệt: thì lấy số lượng theo YeuCauLinhVatTuCHiTiet
            else
            {
                query = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                    .Where(o => o.YeuCauLinhVatTuId == yeuCauLinhId
                                && o.YeuCauVatTuBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                && o.YeuCauVatTuBenhVien.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                && o.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhanId != null)
                    .OrderBy(x => x.YeuCauVatTuBenhVien.ThoiDiemChiDinh)
                    .Select(s => new YeuCauLinhVatTuTrucTiepGridChildVo
                    {
                        YeuCauTiepNhanId = s.YeuCauVatTuBenhVien.YeuCauTiepNhanId,
                        MaYeuCauTiepNhan = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauVatTuBenhVien.YeuCauTiepNhan.HoTen,
                        SoLuong = s.SoLuong,

                        SoLuongTonTheoDichVu = (nhapKhoChiTiets
                                                    .Where(x => x.VatTuBenhVienId == s.VatTuBenhVienId
                                                                && x.LaVatTuBHYT == s.LaVatTuBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                                                - yeuCauLinhVatTu.YeuCauVatTuBenhViens.Where(x => x.VatTuBenhVienId == s.VatTuBenhVienId
                                                                                                        && x.LaVatTuBHYT == s.LaVatTuBHYT
                                                                                                        && x.Id != s.Id
                                                                                                        && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                                                                                        && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                                                    .Sum(x => x.SoLuong)),
                    }).GroupBy(x => new
                    {
                        x.YeuCauTiepNhanId
                    }).Select(s => new YeuCauLinhVatTuTrucTiepGridChildVo
                    {
                        YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                        MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                        MaBenhNhan = s.First().MaBenhNhan,
                        HoTen = s.First().HoTen,
                        SoLuong = s.Sum(a => a.SoLuong),

                        // trường hợp đã duyệt, từ chối thì ko cần highlight
                        HighLightClass = (s.Any(a => a.KhongDuTon) && yeuCauLinhVatTu.DuocDuyet == null) ? "bg-row-lightRed" : ""
                    });
            }

            if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "desc";
                queryInfo.Sort[0].Field = "HighLightClass";
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetBenhNhanTheoVatTuCanLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauLinhId = long.Parse(queryInfo.AdditionalSearchString);

            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.YeuCauLinhVatTuId == yeuCauLinhId
                            && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                            && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                            && o.YeuCauTiepNhan.BenhNhanId != null)
                .OrderBy(x => x.ThoiDiemChiDinh)
                .Select(s => new YeuCauLinhVatTuTrucTiepGridChildVo
                {
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    SoLuong = s.SoLuong
                }).GroupBy(x => new
                {
                    x.YeuCauTiepNhanId
                }).Select(s => new YeuCauLinhVatTuTrucTiepGridChildVo
                {
                    YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                    MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                    MaBenhNhan = s.First().MaBenhNhan,
                    HoTen = s.First().HoTen,
                    SoLuong = s.Sum(a => a.SoLuong)
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion
        #region Xử lý thêm/xóa/sửa lĩnh thường
        public async Task XuLyDuyetYeuCauLinhVatTuThuongAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhVatTu, long nguoiXuatId, long nguoiNhapId)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            // cập nhật thông tin duyệt
            var thoiDiemHienTai = DateTime.Now;


            // xử lý nhập xuất dược phẩm
            #region Xử lý xuất
            var newNhapKho = new NhapKhoVatTu()
            {
                NguoiNhapId = nguoiNhapId,
                LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayNhap = thoiDiemHienTai,
                KhoId = yeuCauLinhVatTu.KhoNhapId,
                YeuCauLinhVatTuId = yeuCauLinhVatTu.Id
            };

            var newPhieuXuat = new XuatKhoVatTu()
            {
                LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatQuaKhoKhac,
                LyDoXuatKho = Enums.EnumLoaiXuatKho.XuatQuaKhoKhac.GetDescription(),
                NguoiNhanId = nguoiNhapId,
                NguoiXuatId = nguoiXuatId,
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayXuat = thoiDiemHienTai,
                KhoXuatId = yeuCauLinhVatTu.KhoXuatId,
                KhoNhapId = yeuCauLinhVatTu.KhoNhapId
            };

            var lstNhapKhoTatCaVatTuYeuCauLinh = await _nhapKhoVatTuChiTietRepository.Table
                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauLinhVatTu.KhoXuatId
                            && yeuCauLinhVatTu.YeuCauLinhVatTuChiTiets.Any(y => y.VatTuBenhVienId == x.VatTuBenhVienId && (x.SoLuongDaXuat < x.SoLuongNhap)))
                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                .ToListAsync();
            foreach (var yeuCauLinhChiTiet in yeuCauLinhVatTu.YeuCauLinhVatTuChiTiets)
            {
                var lstNhapKhoChiTiet = lstNhapKhoTatCaVatTuYeuCauLinh.Where(x =>
                    x.VatTuBenhVienId == yeuCauLinhChiTiet.VatTuBenhVienId &&
                    x.LaVatTuBHYT == yeuCauLinhChiTiet.LaVatTuBHYT &&
                    x.SoLuongDaXuat < x.SoLuongNhap).ToList();


                if (!lstNhapKhoChiTiet.Any() || lstNhapKhoChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                    yeuCauLinhChiTiet.SoLuong)
                {
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhVatTuThuong.SoLuongTon.KhongDu"));
                }

                var newXuatKhoChiTiet = new XuatKhoVatTuChiTiet()
                {
                    VatTuBenhVienId = yeuCauLinhChiTiet.VatTuBenhVienId,
                    NgayXuat = thoiDiemHienTai
                };

                var slYeuCau = yeuCauLinhChiTiet.SoLuong;
                var isExists = false;
                foreach (var nhapChiTiet in lstNhapKhoChiTiet)
                {
                    if (slYeuCau > 0)
                    {
                        var newNhapKhoChiTiet = new NhapKhoVatTuChiTiet()
                        {
                            SoLuongNhap = 0,
                            SoLuongDaXuat = 0
                        };
                        var nhapKhoChiTiettemp = newNhapKho.NhapKhoVatTuChiTiets.FirstOrDefault(x => x.VatTuBenhVienId == nhapChiTiet.VatTuBenhVienId
                                                                                                        && x.LaVatTuBHYT == nhapChiTiet.LaVatTuBHYT
                                                                                                        && x.NgayNhapVaoBenhVien == nhapChiTiet.NgayNhapVaoBenhVien
                                                                                                        && x.DonGiaNhap == nhapChiTiet.DonGiaNhap
                                                                                                        && x.VAT == nhapChiTiet.VAT
                                                                                                        && x.TiLeTheoThapGia == nhapChiTiet.TiLeTheoThapGia
                                                                                                        && x.MaVach == nhapChiTiet.MaVach
                                                                                                     && x.MaRef == nhapChiTiet.MaRef
                                                                                                        && x.HanSuDung == nhapChiTiet.HanSuDung
                                                                                                        && x.Solo == nhapChiTiet.Solo
                                                                                                        && x.HopDongThauVatTuId == nhapChiTiet.HopDongThauVatTuId
                                                                                                        && x.NgayNhap == nhapChiTiet.NgayNhap
                                                                                                        && x.TiLeBHYTThanhToan == nhapChiTiet.TiLeBHYTThanhToan);
                        if (nhapKhoChiTiettemp != null)
                        {
                            newNhapKhoChiTiet = nhapKhoChiTiettemp;
                            isExists = true;
                        }
                        else
                        {
                            isExists = false;
                            newNhapKhoChiTiet.VatTuBenhVienId = nhapChiTiet.VatTuBenhVienId;
                            newNhapKhoChiTiet.HopDongThauVatTuId = nhapChiTiet.HopDongThauVatTuId;
                            newNhapKhoChiTiet.Solo = nhapChiTiet.Solo;
                            newNhapKhoChiTiet.HanSuDung = nhapChiTiet.HanSuDung;
                            newNhapKhoChiTiet.MaVach = nhapChiTiet.MaVach;
                            newNhapKhoChiTiet.MaRef = nhapChiTiet.MaRef;
                            newNhapKhoChiTiet.LaVatTuBHYT = nhapChiTiet.LaVatTuBHYT;
                            newNhapKhoChiTiet.NgayNhapVaoBenhVien = nhapChiTiet.NgayNhapVaoBenhVien;
                            newNhapKhoChiTiet.PhuongPhapTinhGiaTriTonKho = nhapChiTiet.PhuongPhapTinhGiaTriTonKho;
                            newNhapKhoChiTiet.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                            newNhapKhoChiTiet.VAT = nhapChiTiet.VAT;
                            newNhapKhoChiTiet.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                            newNhapKhoChiTiet.NgayNhap = thoiDiemHienTai;
                            newNhapKhoChiTiet.TiLeBHYTThanhToan = nhapChiTiet.TiLeBHYTThanhToan;
                        }

                        var newXuatKhoChiTietViTri = new XuatKhoVatTuChiTietViTri()
                        {
                            NhapKhoVatTuChiTietId = nhapChiTiet.Id,
                            NgayXuat = thoiDiemHienTai
                        };

                        //var slTonNhapChiTiet = nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat;
                        var slTonNhapChiTiet = Math.Round(nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat, 2);
                        if (slYeuCau < slTonNhapChiTiet || slYeuCau.AlmostEqual(slTonNhapChiTiet))
                        {
                            newXuatKhoChiTietViTri.SoLuongXuat = slYeuCau;
                            nhapChiTiet.SoLuongDaXuat = Math.Round(nhapChiTiet.SoLuongDaXuat + slYeuCau, 2);

                            newNhapKhoChiTiet.SoLuongNhap = Math.Round(newNhapKhoChiTiet.SoLuongNhap + slYeuCau, 2);

                            slYeuCau = 0;

                        }
                        else
                        {
                            newXuatKhoChiTietViTri.SoLuongXuat = slTonNhapChiTiet;

                            newNhapKhoChiTiet.SoLuongNhap = Math.Round(newNhapKhoChiTiet.SoLuongNhap + slTonNhapChiTiet, 2);

                            //slYeuCau = nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat;
                            slYeuCau = Math.Round(slYeuCau - slTonNhapChiTiet, 2);
                            nhapChiTiet.SoLuongDaXuat = nhapChiTiet.SoLuongNhap;
                        }
                        newXuatKhoChiTiet.XuatKhoVatTuChiTietViTris.Add(newXuatKhoChiTietViTri);
                        if (!isExists)
                        {
                            newNhapKho.NhapKhoVatTuChiTiets.Add(newNhapKhoChiTiet);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                newPhieuXuat.XuatKhoVatTuChiTiets.Add(newXuatKhoChiTiet);
            }
            #endregion
            
            newPhieuXuat.NhapKhoVatTus.Add(newNhapKho);
            yeuCauLinhVatTu.XuatKhoVatTus.Add(newPhieuXuat);


            // lưu thông tin yêu cầu lĩnh
            await _yeuCauLinhVatTuRepository.UpdateAsync(yeuCauLinhVatTu);
            // cập nhật thông tin tông
            //await _nhapKhoVatTuChiTietRepository.UpdateAsync(lstNhapKhoTatCaVatTuYeuCauLinh);
        }

        public async Task<string> InPhieuDuyetLinhThuongVatTuAsync(InPhieuDuyetLinhVatTu inPhieu)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var templateLinhThuongVatTu = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocThuong"));
            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == inPhieu.YeuCauLinhVatTuId)
                .Select(item => new ThongTinInPhieuLinhVatTuVo()
                {
                    TenNguoiNhanHang = item.XuatKhoVatTus.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault(),
                    BoPhan = item.NoiYeuCauId != null ? item.NoiYeuCau.KhoaPhongId != null ? item.NoiYeuCau.KhoaPhong.Ma + "-" + item.NoiYeuCau.KhoaPhong.Ten
                             : ""
                             : "",
                    LyDoXuatKho = item.XuatKhoVatTus.Select(x => x.LyDoXuatKho).FirstOrDefault(),
                    XuatTaiKho = item.KhoXuat.Ten,
                    DiaDiem = "",

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            if (inPhieu.HasHeader)
            {
                hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU XUẤT</th>" +
                         "</p>";
            }
            var query = await _yeuCauLinhVatTuChiTietRepository.TableNoTracking.Where(p => p.YeuCauLinhVatTuId == inPhieu.YeuCauLinhVatTuId)
                    .Select(s => new ThongTinInPhieuLinhVatTuChiTietVo
                    {
                        Ma = s.VatTuBenhVien.Ma,
                        TenThuoc = s.VatTuBenhVien.VatTus.Ten +
                                               (s.VatTuBenhVien.VatTus.NhaSanXuat != null && s.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                               (s.VatTuBenhVien.VatTus.NuocSanXuat != null && s.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                        DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                        SLYeuCau = s.SoLuong,
                        SLThucXuat = s.SoLuongCoTheXuat,
                        LaVatTuBHYT = s.LaVatTuBHYT
                    }).OrderByDescending(p => p.LaVatTuBHYT).ThenBy(p => !p.LaVatTuBHYT)
                    .ToListAsync();

            var infoLinhDuocChiTiet = string.Empty;
            var groupThuocBHYT = "Vật Tư BHYT";
            var groupThuocKhongBHYT = "Vật Tư Không BHYT";

            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                        + "</b></tr>";
            var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                        + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                        + "</b></tr>";
            var STT = 1;
            if (query.Any(p => p.LaVatTuBHYT))
            {
                infoLinhDuocChiTiet += headerBHYT;
                var queryBHYT = query.Where(x => x.LaVatTuBHYT).OrderBy(z => z.TenThuoc).ToList();
                foreach (var item in queryBHYT)
                {
                    if (item.LaVatTuBHYT)
                    {
                        infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                            + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                            + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                            + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucXuat
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"
                                            + "</tr>";
                        STT++;
                        groupThuocBHYT = string.Empty;
                    }

                }
            }
            if (query.Any(p => p.LaVatTuBHYT == false))
            {
                infoLinhDuocChiTiet += headerKhongBHYT;
                var queryKhongBHYT = query.Where(x => !x.LaVatTuBHYT).OrderBy(z => z.TenThuoc).ToList();
                foreach (var item in queryKhongBHYT)
                {
                    if (item.LaVatTuBHYT == false)
                    {
                        infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                           + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                           + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                           + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                           + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucXuat
                                           + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"
                                           + "</tr>";
                        STT++;
                        groupThuocKhongBHYT = string.Empty;
                    }
                }
            }

            data.HeaderPhieuLinhThuoc = hearder;
            data.DanhSachThuoc = infoLinhDuocChiTiet;

            content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongVatTu.Body, data);
            return content;
        }
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh bù

        public async Task XuLyDuyetYeuCauLinhVatTuBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, long nguoiXuatId, long nguoiNhapId)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            // cập nhật thông tin duyệt
            var thoiDiemHienTai = DateTime.Now;

            // xử lý nhập xuất dược phẩm
            #region Xử lý xuất
            var newNhapKho = new NhapKhoVatTu()
            {
                NguoiNhapId = nguoiNhapId,
                LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayNhap = thoiDiemHienTai,
                KhoId = yeuCauLinhBuVatTu.KhoNhapId,
                YeuCauLinhVatTuId = yeuCauLinhBuVatTu.Id
            };

            var newPhieuXuat = new XuatKhoVatTu()
            {
                LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatQuaKhoKhac,
                LyDoXuatKho = Enums.EnumLoaiXuatKho.XuatQuaKhoKhac.GetDescription(),
                NguoiNhanId = nguoiNhapId,
                NguoiXuatId = nguoiXuatId,
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayXuat = thoiDiemHienTai,
                KhoXuatId = yeuCauLinhBuVatTu.KhoXuatId,
                KhoNhapId = yeuCauLinhBuVatTu.KhoNhapId
            };


            var lstNhapKhoTatCaVatTuYeuCauLinh = await _nhapKhoVatTuChiTietRepository.Table
                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauLinhBuVatTu.KhoXuatId
                            && yeuCauLinhBuVatTu.YeuCauLinhVatTuChiTiets.Any(y => y.VatTuBenhVienId == x.VatTuBenhVienId)
                            && (x.SoLuongDaXuat < x.SoLuongNhap))
                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                .ToListAsync();

            foreach (var yeuCauLinhVatTuChiTiet in yeuCauLinhBuVatTu.YeuCauLinhVatTuChiTiets)//.YeuCauVatTuBenhViens.Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
            {
                var lstNhapKhoChiTiet = lstNhapKhoTatCaVatTuYeuCauLinh.Where(x =>
                    x.VatTuBenhVienId == yeuCauLinhVatTuChiTiet.VatTuBenhVienId &&
                    x.LaVatTuBHYT == yeuCauLinhVatTuChiTiet.LaVatTuBHYT &&
                    x.SoLuongDaXuat < x.SoLuongNhap).ToList();


                var slYeuCau = yeuCauLinhVatTuChiTiet.SoLuong;
                if (!lstNhapKhoChiTiet.Any() || lstNhapKhoChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauLinhVatTuChiTiet.SoLuong)
                {
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhVatTuBu.SoLuongTon.KhongDu"));
                }

                // update 07/06/2021: cập nhật lĩnh bù -> rào giá trị max đã bù chỉ cho bằng số lượng
                var soLuongDaLinhBuNew = (yeuCauLinhVatTuChiTiet.YeuCauVatTuBenhVien.SoLuongDaLinhBu ?? 0) + slYeuCau;
                yeuCauLinhVatTuChiTiet.YeuCauVatTuBenhVien.SoLuongDaLinhBu = soLuongDaLinhBuNew > yeuCauLinhVatTuChiTiet.YeuCauVatTuBenhVien.SoLuong ? yeuCauLinhVatTuChiTiet.YeuCauVatTuBenhVien.SoLuong : soLuongDaLinhBuNew;

                var newXuatKhoChiTiet = new XuatKhoVatTuChiTiet()
                {
                    VatTuBenhVienId = yeuCauLinhVatTuChiTiet.VatTuBenhVienId,
                    NgayXuat = thoiDiemHienTai
                };

                var isExists = false;
                foreach (var nhapChiTiet in lstNhapKhoChiTiet)
                {
                    if (slYeuCau > 0)
                    {
                        var newNhapKhoChiTiet = new NhapKhoVatTuChiTiet()
                        {
                            SoLuongNhap = 0,
                            SoLuongDaXuat = 0
                        };
                        var nhapKhoChiTiettemp = newNhapKho.NhapKhoVatTuChiTiets.FirstOrDefault(x => x.VatTuBenhVienId == nhapChiTiet.VatTuBenhVienId
                                                                                                        && x.LaVatTuBHYT == nhapChiTiet.LaVatTuBHYT
                                                                                                        && x.NgayNhapVaoBenhVien == nhapChiTiet.NgayNhapVaoBenhVien
                                                                                                        && x.DonGiaNhap == nhapChiTiet.DonGiaNhap
                                                                                                        && x.VAT == nhapChiTiet.VAT
                                                                                                        && x.TiLeTheoThapGia == nhapChiTiet.TiLeTheoThapGia
                                                                                                        && x.MaVach == nhapChiTiet.MaVach
                                                                                                     && x.MaRef == nhapChiTiet.MaRef
                                                                                                        && x.HanSuDung == nhapChiTiet.HanSuDung
                                                                                                        && x.Solo == nhapChiTiet.Solo
                                                                                                        && x.HopDongThauVatTuId == nhapChiTiet.HopDongThauVatTuId
                                                                                                        && x.NgayNhap == nhapChiTiet.NgayNhap
                                                                                                        && x.TiLeBHYTThanhToan == nhapChiTiet.TiLeBHYTThanhToan);
                        if (nhapKhoChiTiettemp != null)
                        {
                            newNhapKhoChiTiet = nhapKhoChiTiettemp;
                            isExists = true;
                        }
                        else
                        {
                            isExists = false;
                            newNhapKhoChiTiet.VatTuBenhVienId = nhapChiTiet.VatTuBenhVienId;
                            newNhapKhoChiTiet.HopDongThauVatTuId = nhapChiTiet.HopDongThauVatTuId;
                            newNhapKhoChiTiet.Solo = nhapChiTiet.Solo;
                            newNhapKhoChiTiet.HanSuDung = nhapChiTiet.HanSuDung;
                            newNhapKhoChiTiet.MaVach = nhapChiTiet.MaVach;
                            newNhapKhoChiTiet.MaRef = nhapChiTiet.MaRef;
                            newNhapKhoChiTiet.LaVatTuBHYT = nhapChiTiet.LaVatTuBHYT;
                            newNhapKhoChiTiet.NgayNhapVaoBenhVien = nhapChiTiet.NgayNhapVaoBenhVien;
                            newNhapKhoChiTiet.PhuongPhapTinhGiaTriTonKho = nhapChiTiet.PhuongPhapTinhGiaTriTonKho;
                            newNhapKhoChiTiet.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                            newNhapKhoChiTiet.VAT = nhapChiTiet.VAT;
                            newNhapKhoChiTiet.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                            newNhapKhoChiTiet.NgayNhap = thoiDiemHienTai;
                            newNhapKhoChiTiet.TiLeBHYTThanhToan = nhapChiTiet.TiLeBHYTThanhToan;
                        }

                        var newXuatKhoChiTietViTri = new XuatKhoVatTuChiTietViTri()
                        {
                            NhapKhoVatTuChiTietId = nhapChiTiet.Id,
                            NgayXuat = thoiDiemHienTai
                        };

                        //var slTonNhapChiTiet = nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat;
                        var slTonNhapChiTiet = Math.Round(nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat, 2);
                        if (slYeuCau < slTonNhapChiTiet || slYeuCau.AlmostEqual(slTonNhapChiTiet))
                        {
                            newXuatKhoChiTietViTri.SoLuongXuat = slYeuCau;
                            nhapChiTiet.SoLuongDaXuat = Math.Round(nhapChiTiet.SoLuongDaXuat + slYeuCau, 2);

                            newNhapKhoChiTiet.SoLuongNhap = Math.Round(newNhapKhoChiTiet.SoLuongNhap + slYeuCau, 2);

                            slYeuCau = 0;

                        }
                        else
                        {
                            newXuatKhoChiTietViTri.SoLuongXuat = slTonNhapChiTiet;

                            newNhapKhoChiTiet.SoLuongNhap = Math.Round(newNhapKhoChiTiet.SoLuongNhap + slTonNhapChiTiet, 2);

                            //slYeuCau = nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat;
                            slYeuCau = Math.Round(slYeuCau - slTonNhapChiTiet, 2);
                            nhapChiTiet.SoLuongDaXuat = nhapChiTiet.SoLuongNhap;
                        }
                        newXuatKhoChiTiet.XuatKhoVatTuChiTietViTris.Add(newXuatKhoChiTietViTri);
                        if (!isExists)
                        {
                            newNhapKho.NhapKhoVatTuChiTiets.Add(newNhapKhoChiTiet);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                newPhieuXuat.XuatKhoVatTuChiTiets.Add(newXuatKhoChiTiet);
            }
            #endregion

            newPhieuXuat.NhapKhoVatTus.Add(newNhapKho);
            yeuCauLinhBuVatTu.XuatKhoVatTus.Add(newPhieuXuat);


            // lưu thông tin yêu cầu lĩnh
            await _yeuCauLinhVatTuRepository.UpdateAsync(yeuCauLinhBuVatTu);
            // cập nhật thông tin tông
            //await _nhapKhoVatTuChiTietRepository.UpdateAsync(lstNhapKhoTatCaVatTuYeuCauLinh);
        }

        public async Task<string> InPhieuDuyetLinhBuVatTuAsync(InPhieuDuyetLinhVatTu inPhieu)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var templateLinhThuongVatTu = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocBu"));
            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == inPhieu.YeuCauLinhVatTuId)
                .Select(item => new ThongTinInPhieuLinhVatTuVo()
                {
                    TenNguoiNhanHang = item.XuatKhoVatTus.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault(),
                    //BoPhan = item.KhoNhap.PhongBenhVien != null ? item.KhoNhap.PhongBenhVien.Ma + " - " + item.KhoNhap.PhongBenhVien.Ten : (item.KhoNhap.KhoaPhong != null ? item.KhoNhap.KhoaPhong.Ma + " - " + item.KhoNhap.KhoaPhong.Ten : ""),
                    BoPhan = item.NoiYeuCauId != null ? item.NoiYeuCau.KhoaPhongId != null ? item.NoiYeuCau.KhoaPhong.Ma + "-" + item.NoiYeuCau.KhoaPhong.Ten
                             : ""
                             : "",
                    LyDoXuatKho = item.XuatKhoVatTus.Select(x => x.LyDoXuatKho).FirstOrDefault(),
                    XuatTaiKho = item.KhoXuat.Ten,
                    DiaDiem = "",

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            if (inPhieu.HasHeader)
            {
                hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU XUẤT</th>" +
                         "</p>";
            }
            var query = await _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                .Where(x => x.YeuCauLinhVatTuId == inPhieu.YeuCauLinhVatTuId && x.YeuCauVatTuBenhVien.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                .Select(s => new ThongTinInPhieuLinhVatTuChiTietVo
                {
                    VatTuBenhVienId = s.VatTuBenhVienId,
                    Ma = s.VatTuBenhVien.Ma,
                    TenThuoc = s.VatTuBenhVien.VatTus.Ten +
                                               (s.VatTuBenhVien.VatTus.NhaSanXuat != null && s.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                               (s.VatTuBenhVien.VatTus.NuocSanXuat != null && s.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                    DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                    SLYeuCau = s.SoLuong,
                    SLThucXuat = s.SoLuong,
                    LaVatTuBHYT = s.LaVatTuBHYT
                })
                .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT,x.Ma,x.DVT })
                .Select(item => new ThongTinInPhieuLinhVatTuChiTietVo()
                {
                    Ma = item.First().Ma,
                    TenThuoc = item.First().TenThuoc,
                    DVT = item.First().DVT,
                    SLYeuCau = item.Sum(x => x.SLYeuCau),
                    SLThucXuat = item.Sum(x => x.SLThucXuat),
                    LaVatTuBHYT = item.First().LaVatTuBHYT
                }).OrderByDescending(p => p.LaVatTuBHYT).ThenBy(p => !p.LaVatTuBHYT)
                .ToListAsync();

            var infoLinhDuocChiTiet = string.Empty;
            var STT = 1;
            if (query.Any())
            {
                foreach (var item in query)
                {
                    infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                            + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                            + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                            + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucXuat
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"
                                            + "</tr>";
                    STT++;

                }
            }

            data.HeaderPhieuLinhThuoc = hearder;
            data.DanhSachThuoc = infoLinhDuocChiTiet;

            content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongVatTu.Body, data);
            return content;
        }
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh trực tiếp

        public async Task XuLyDuyetYeuCauLinhVatTuTrucTiepAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhTrucTiepVatTu, long nguoiXuatId, long nguoiNhapId)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            // cập nhật thông tin duyệt
            var thoiDiemHienTai = DateTime.Now;

            // xử lý nhập xuất dược phẩm
            #region Xử lý xuất
            //var newNhapKho = new NhapKhoVatTu()
            //{
            //    NguoiNhapId = nguoiNhapId,
            //    LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
            //    NgayNhap = thoiDiemHienTai,
            //    KhoId = yeuCauLinhTrucTiepVatTu.KhoNhapId,
            //    YeuCauLinhVatTuId = yeuCauLinhTrucTiepVatTu.Id
            //};

            var newPhieuXuat = new XuatKhoVatTu()
            {
                LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatChoBenhNhan, //Enums.EnumLoaiXuatKho.XuatQuaKhoKhac,
                LyDoXuatKho = Enums.EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(), //Enums.EnumLoaiXuatKho.XuatQuaKhoKhac.GetDescription(),
                NguoiNhanId = nguoiNhapId,
                NguoiXuatId = nguoiXuatId,
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayXuat = thoiDiemHienTai,
                KhoXuatId = yeuCauLinhTrucTiepVatTu.KhoXuatId,
               // KhoNhapId = yeuCauLinhTrucTiepVatTu.KhoNhapId
            };


            var lstNhapKhoTatCaVatTuYeuCauLinh = await _nhapKhoVatTuChiTietRepository.Table
                .Where(x => x.NhapKhoVatTu.KhoId == yeuCauLinhTrucTiepVatTu.KhoXuatId
                            && yeuCauLinhTrucTiepVatTu.YeuCauVatTuBenhViens.Any(y => y.VatTuBenhVienId == x.VatTuBenhVienId 
                                                                                           && y.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy 
                                                                                           && (x.SoLuongDaXuat < x.SoLuongNhap))
                            //BVHD-3821
                            // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                            && x.HanSuDung.Date >= DateTime.Now.Date)
                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                .ToListAsync();
            if (!lstNhapKhoTatCaVatTuYeuCauLinh.Any())
            {
                throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhVatTuBu.SoLuongTon.KhongDu"));
            }

            var lstYeuCauDuocPhamThemMoi = new List<YeuCauVatTuBenhVien>();
            foreach (var yeuCauVatTuChiTiet in yeuCauLinhTrucTiepVatTu.YeuCauVatTuBenhViens.Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
            {
                var lstNhapKhoChiTiet = lstNhapKhoTatCaVatTuYeuCauLinh.Where(x =>
                    x.VatTuBenhVienId == yeuCauVatTuChiTiet.VatTuBenhVienId &&
                    x.LaVatTuBHYT == yeuCauVatTuChiTiet.LaVatTuBHYT &&
                    x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                var vatTuBenhVien = _vatTuBenhVienRepository.GetById(yeuCauVatTuChiTiet.VatTuBenhVienId,
                    o => o.Include(y => y.VatTus).ThenInclude(y => y.HopDongThauVatTuChiTiets));

                if (!lstNhapKhoChiTiet.Any() || lstNhapKhoChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauVatTuChiTiet.SoLuong)
                {
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhVatTuBu.SoLuongTon.KhongDu"));
                }

                var newXuatKhoChiTiet = new XuatKhoVatTuChiTiet()
                {
                    VatTuBenhVienId = yeuCauVatTuChiTiet.VatTuBenhVienId,
                    NgayXuat = thoiDiemHienTai
                };

                // tạo biến tạm yêu cầu dược phẩm bệnh viện
                yeuCauVatTuChiTiet.TrangThai = Enums.EnumYeuCauVatTuBenhVien.DaThucHien;
                var yeuCauVatTuTemp = new YeuCauVatTuBenhVien()
                {
                    YeuCauTiepNhanId = yeuCauVatTuChiTiet.YeuCauTiepNhanId,
                    KhoLinhId = yeuCauVatTuChiTiet.KhoLinhId,

                    YeuCauKhamBenhId = yeuCauVatTuChiTiet.YeuCauKhamBenhId,
                    YeuCauDichVuKyThuatId = yeuCauVatTuChiTiet.YeuCauDichVuKyThuatId,

                    YeuCauLinhVatTuId = yeuCauVatTuChiTiet.YeuCauLinhVatTuId,

                    LoaiPhieuLinh = yeuCauVatTuChiTiet.LoaiPhieuLinh,

                    VatTuBenhVienId = yeuCauVatTuChiTiet.VatTuBenhVienId,
                    Ten = yeuCauVatTuChiTiet.Ten,
                    Ma = yeuCauVatTuChiTiet.Ma,
                    DonViTinh = yeuCauVatTuChiTiet.DonViTinh,
                    NhomVatTuId = yeuCauVatTuChiTiet.NhomVatTuId,
                    NhaSanXuat = yeuCauVatTuChiTiet.NhaSanXuat,
                    NuocSanXuat = yeuCauVatTuChiTiet.NuocSanXuat,
                    QuyCach = yeuCauVatTuChiTiet.QuyCach,
                    TieuChuan = yeuCauVatTuChiTiet.TieuChuan,
                    MoTa = yeuCauVatTuChiTiet.MoTa,

                    KhongTinhPhi = yeuCauVatTuChiTiet.KhongTinhPhi,
                    LaVatTuBHYT = yeuCauVatTuChiTiet.LaVatTuBHYT,

                    NhanVienChiDinhId = yeuCauVatTuChiTiet.NhanVienChiDinhId,
                    NoiChiDinhId = yeuCauVatTuChiTiet.NoiChiDinhId,
                    ThoiDiemChiDinh = yeuCauVatTuChiTiet.ThoiDiemChiDinh,

                    DaCapVatTu = yeuCauVatTuChiTiet.DaCapVatTu,
                    TrangThai = yeuCauVatTuChiTiet.TrangThai,
                    TrangThaiThanhToan = yeuCauVatTuChiTiet.TrangThaiThanhToan,

                    DuocHuongBaoHiem = yeuCauVatTuChiTiet.DuocHuongBaoHiem,
                    LoaiNoiChiDinh = yeuCauVatTuChiTiet.LoaiNoiChiDinh,
                    NoiTruPhieuDieuTriId = yeuCauVatTuChiTiet.NoiTruPhieuDieuTriId
                };

                var yeuCauNew = yeuCauVatTuChiTiet;

                var slYeuCau = yeuCauVatTuChiTiet.SoLuong;
                //var isExists = false;
                var isFirstYeuCauVatTu = true;
                foreach (var nhapChiTiet in lstNhapKhoChiTiet)
                {
                    if (slYeuCau > 0)
                    {
                        var giaTheoHopDong = vatTuBenhVien.VatTus.HopDongThauVatTuChiTiets.First(o => o.HopDongThauVatTuId == nhapChiTiet.HopDongThauVatTuId).Gia;
                        var donGiaBaoHiem = nhapChiTiet.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : nhapChiTiet.DonGiaNhap;

                        var tiLeBHTT = nhapChiTiet.LaVatTuBHYT ? nhapChiTiet.TiLeBHYTThanhToan ?? 100 : 0;
                        if (yeuCauNew.DonGiaNhap != nhapChiTiet.DonGiaNhap || yeuCauNew.VAT != nhapChiTiet.VAT || yeuCauNew.TiLeTheoThapGia != nhapChiTiet.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != tiLeBHTT)
                        {
                            // trường hợp cập nhật lại thông tin cho yêu cầu duyệt cũ
                            if (isFirstYeuCauVatTu && yeuCauNew.Id != 0)
                            {
                                yeuCauNew.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                                yeuCauNew.VAT = nhapChiTiet.VAT;
                                yeuCauNew.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tiLeBHTT; //nhapChiTiet.TiLeBHYTThanhToan ?? 100;

                                //isFirstYeuCauVatTu = false;
                            }
                            // trường hợp tạo mới yêu cầu duyệt
                            else
                            {
                                if (newXuatKhoChiTiet.XuatKhoVatTuChiTietViTris.All(x => x.SoLuongXuat > 0))
                                {
                                    newPhieuXuat.XuatKhoVatTuChiTiets.Add(newXuatKhoChiTiet);
                                    yeuCauNew.XuatKhoVatTuChiTiet = newXuatKhoChiTiet;
                                    yeuCauNew.SoLuong = yeuCauNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat);

                                    if (yeuCauNew.Id == 0)
                                    {
                                        lstYeuCauDuocPhamThemMoi.Add(yeuCauNew);
                                    }

                                    yeuCauNew = yeuCauVatTuTemp.Clone();
                                    yeuCauNew.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                                    yeuCauNew.VAT = nhapChiTiet.VAT;
                                    yeuCauNew.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                                    yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                    yeuCauNew.TiLeBaoHiemThanhToan = tiLeBHTT; //nhapChiTiet.TiLeBHYTThanhToan ?? 100;

                                    newXuatKhoChiTiet = new XuatKhoVatTuChiTiet()
                                    {
                                        VatTuBenhVienId = yeuCauNew.VatTuBenhVienId,
                                        NgayXuat = thoiDiemHienTai
                                    };
                                }

                            }
                        }
                        else
                        {
                            yeuCauNew.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                            yeuCauNew.VAT = nhapChiTiet.VAT;
                            yeuCauNew.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                            yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                            yeuCauNew.TiLeBaoHiemThanhToan = tiLeBHTT; //nhapChiTiet.TiLeBHYTThanhToan ?? 100;
                        }


                        //var newNhapKhoChiTiet = new NhapKhoVatTuChiTiet()
                        //{
                        //    SoLuongNhap = 0,
                        //    SoLuongDaXuat = 0
                        //};
                        //var nhapKhoChiTiettemp = newNhapKho.NhapKhoVatTuChiTiets.FirstOrDefault(x => x.VatTuBenhVienId == nhapChiTiet.VatTuBenhVienId
                        //                                                                                && x.LaVatTuBHYT == nhapChiTiet.LaVatTuBHYT
                        //                                                                                && x.NgayNhapVaoBenhVien == nhapChiTiet.NgayNhapVaoBenhVien
                        //                                                                                && x.DonGiaNhap == nhapChiTiet.DonGiaNhap
                        //                                                                                && x.VAT == nhapChiTiet.VAT
                        //                                                                                && x.TiLeTheoThapGia == nhapChiTiet.TiLeTheoThapGia
                        //                                                                                && x.MaVach == nhapChiTiet.MaVach
                        //                                                                                && x.HanSuDung == nhapChiTiet.HanSuDung
                        //                                                                                && x.Solo == nhapChiTiet.Solo
                        //                                                                                && x.HopDongThauVatTuId == nhapChiTiet.HopDongThauVatTuId
                        //                                                                                && x.NgayNhap == nhapChiTiet.NgayNhap
                        //                                                                                && x.TiLeBHYTThanhToan == nhapChiTiet.TiLeBHYTThanhToan);
                        //if (nhapKhoChiTiettemp != null)
                        //{
                        //    newNhapKhoChiTiet = nhapKhoChiTiettemp;
                        //    isExists = true;
                        //}
                        //else
                        //{
                        //    isExists = false;
                        //    newNhapKhoChiTiet.VatTuBenhVienId = nhapChiTiet.VatTuBenhVienId;
                        //    newNhapKhoChiTiet.HopDongThauVatTuId = nhapChiTiet.HopDongThauVatTuId;
                        //    newNhapKhoChiTiet.Solo = nhapChiTiet.Solo;
                        //    newNhapKhoChiTiet.HanSuDung = nhapChiTiet.HanSuDung;
                        //    newNhapKhoChiTiet.MaVach = nhapChiTiet.MaVach;
                        //    newNhapKhoChiTiet.LaVatTuBHYT = nhapChiTiet.LaVatTuBHYT;
                        //    newNhapKhoChiTiet.NgayNhapVaoBenhVien = nhapChiTiet.NgayNhapVaoBenhVien;
                        //    newNhapKhoChiTiet.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                        //    newNhapKhoChiTiet.VAT = nhapChiTiet.VAT;
                        //    newNhapKhoChiTiet.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                        //    newNhapKhoChiTiet.NgayNhap = thoiDiemHienTai;
                        //    newNhapKhoChiTiet.TiLeBHYTThanhToan = nhapChiTiet.TiLeBHYTThanhToan;
                        //}

                        var newXuatKhoChiTietViTri = new XuatKhoVatTuChiTietViTri()
                        {
                            NhapKhoVatTuChiTietId = nhapChiTiet.Id,
                            NgayXuat = thoiDiemHienTai
                        };

                        //var slTonNhapChiTiet = nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat;
                        var slTonNhapChiTiet = Math.Round(nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat, 2);
                        if (slYeuCau < slTonNhapChiTiet || slYeuCau.AlmostEqual(slTonNhapChiTiet))
                        {
                            newXuatKhoChiTietViTri.SoLuongXuat = slYeuCau;
                            nhapChiTiet.SoLuongDaXuat = Math.Round(nhapChiTiet.SoLuongDaXuat + slYeuCau, 2);

                            //newNhapKhoChiTiet.SoLuongNhap += slYeuCau;

                            slYeuCau = 0;

                        }
                        else
                        {
                            newXuatKhoChiTietViTri.SoLuongXuat = slTonNhapChiTiet;

                            //newNhapKhoChiTiet.SoLuongNhap += slTonNhapChiTiet;

                            //slYeuCau = nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat;
                            slYeuCau = Math.Round(slYeuCau - slTonNhapChiTiet, 2);
                            nhapChiTiet.SoLuongDaXuat = nhapChiTiet.SoLuongNhap;
                        }
                        newXuatKhoChiTiet.XuatKhoVatTuChiTietViTris.Add(newXuatKhoChiTietViTri);
                        //if (!isExists)
                        //{
                        //    newNhapKho.NhapKhoVatTuChiTiets.Add(newNhapKhoChiTiet);
                        //}

                        if (isFirstYeuCauVatTu){
                            isFirstYeuCauVatTu = false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (newXuatKhoChiTiet.XuatKhoVatTuChiTietViTris.Any())
                {
                    newPhieuXuat.XuatKhoVatTuChiTiets.Add(newXuatKhoChiTiet);
                    yeuCauNew.XuatKhoVatTuChiTiet = newXuatKhoChiTiet;
                    yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(x => x.SoLuongXuat), 2);

                    if (yeuCauNew.Id == 0)
                    {
                        lstYeuCauDuocPhamThemMoi.Add(yeuCauNew);
                    }
                }
            }

            if (lstYeuCauDuocPhamThemMoi.Any())
            {
                foreach (var yeuCauMoi in lstYeuCauDuocPhamThemMoi)
                {
                    yeuCauLinhTrucTiepVatTu.YeuCauVatTuBenhViens.Add(yeuCauMoi);
                }
            }
            #endregion

            ////newPhieuXuat.NhapKhoVatTus.Add(newNhapKho);
            yeuCauLinhTrucTiepVatTu.XuatKhoVatTus.Add(newPhieuXuat);

            //// tạo phiếu xuất cho người bệnh
            //var newXuatChoBenhNhan = new XuatKhoVatTu()
            //{
            //    LoaiXuatKho = Enums.EnumLoaiXuatKho.XuatChoBenhNhan,
            //    LyDoXuatKho = Enums.EnumLoaiXuatKho.XuatChoBenhNhan.GetDescription(),
            //    NguoiNhanId = nguoiNhapId,
            //    NguoiXuatId = nguoiXuatId,
            //    LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
            //    NgayXuat = thoiDiemHienTai,
            //    KhoXuatId = yeuCauLinhTrucTiepVatTu.KhoNhapId
            //};


            //foreach (var nhapChiTiet in newNhapKho.NhapKhoVatTuChiTiets)
            //{
            //    var newXuatChoBenhNhanChiTiet = new XuatKhoVatTuChiTiet()
            //    {
            //        VatTuBenhVienId = nhapChiTiet.VatTuBenhVienId,
            //        NgayXuat = thoiDiemHienTai
            //    };
            //    newXuatChoBenhNhan.XuatKhoVatTuChiTiets.Add(newXuatChoBenhNhanChiTiet);

            //    var newXuatChoBenhNhanChiTietViTri = new XuatKhoVatTuChiTietViTri()
            //    {
            //        SoLuongXuat = nhapChiTiet.SoLuongNhap,
            //        NgayXuat = thoiDiemHienTai
            //    };
            //    newXuatChoBenhNhanChiTietViTri.NhapKhoVatTuChiTiet = nhapChiTiet;
            //    newXuatChoBenhNhanChiTiet.XuatKhoVatTuChiTietViTris.Add(newXuatChoBenhNhanChiTietViTri);

            //    nhapChiTiet.SoLuongDaXuat = nhapChiTiet.SoLuongNhap;
            //}
            //yeuCauLinhTrucTiepVatTu.XuatKhoVatTus.Add(newXuatChoBenhNhan);


            // lưu thông tin yêu cầu lĩnh
            await _yeuCauLinhVatTuRepository.UpdateAsync(yeuCauLinhTrucTiepVatTu);
            // cập nhật thông tin tông
            //await _nhapKhoVatTuChiTietRepository.UpdateAsync(lstNhapKhoTatCaVatTuYeuCauLinh);
        }

        public async Task<string> InPhieuDuyetLinhTrucTiepVatTuAsync(InPhieuDuyetLinhVatTu inPhieu)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var templateLinhThuongVatTu = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocTrucTiep"));
            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == inPhieu.YeuCauLinhVatTuId)
                .Select(item => new ThongTinInPhieuLinhVatTuVo()
                {
                    TenNguoiNhanHang = item.XuatKhoVatTus.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault(),
                    BoPhan = item.NoiYeuCauId != null ? item.NoiYeuCau.KhoaPhongId != null ? item.NoiYeuCau.KhoaPhong.Ma + "-" + item.NoiYeuCau.KhoaPhong.Ten
                             : ""
                             : "",
                    LyDoXuatKho = item.XuatKhoVatTus.Select(x => x.LyDoXuatKho).FirstOrDefault(),
                    XuatTaiKho = item.KhoXuat.Ten,
                    DiaDiem = "",

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            if (inPhieu.HasHeader)
            {
                hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU XUẤT</th>" +
                         "</p>";
            }

            // cập nhật 08/11/2021: trường hợp đã duyệt thì lấy thông tin từ YeCauLinhVatTuCHiTiet
            var yeuCauLinhVatTu = await _yeuCauLinhVatTuRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == inPhieu.YeuCauLinhVatTuId);
            var query = new List<ThongTinInPhieuLinhVatTuChiTietVo>();

            if (yeuCauLinhVatTu.DuocDuyet != true)
            {
                query = await _yeuCauVatTuBenhVienRepository.TableNoTracking.Where(x => x.YeuCauLinhVatTuId == inPhieu.YeuCauLinhVatTuId && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                   .Select(s => new ThongTinInPhieuLinhVatTuChiTietVo
                   {
                       VatTuBenhVienId = s.VatTuBenhVienId,
                       Ma = s.VatTuBenhVien.Ma,
                       TenThuoc = s.VatTuBenhVien.VatTus.Ten +
                                                  (s.VatTuBenhVien.VatTus.NhaSanXuat != null && s.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                                  (s.VatTuBenhVien.VatTus.NuocSanXuat != null && s.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                       DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                       SLYeuCau = s.SoLuong,
                       SLThucXuat = s.SoLuong,
                       LaVatTuBHYT = s.LaVatTuBHYT,
                   })
                   .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.DVT })
                   .Select(item => new ThongTinInPhieuLinhVatTuChiTietVo()
                   {
                       Ma = item.First().Ma,
                       TenThuoc = item.First().TenThuoc,
                       DVT = item.First().DVT,
                       SLYeuCau = item.Sum(x => x.SLYeuCau),
                       SLThucXuat = item.Sum(x => x.SLThucXuat),
                       LaVatTuBHYT = item.First().LaVatTuBHYT,
                   }).OrderBy(d => d.TenThuoc)
                   .ToListAsync();
            }
            else
            {
                query = await _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhVatTuId == inPhieu.YeuCauLinhVatTuId && x.YeuCauVatTuBenhVien.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                    .Select(s => new ThongTinInPhieuLinhVatTuChiTietVo
                    {
                        VatTuBenhVienId = s.VatTuBenhVienId,
                        Ma = s.VatTuBenhVien.Ma,
                        TenThuoc = s.VatTuBenhVien.VatTus.Ten +
                                   (s.VatTuBenhVien.VatTus.NhaSanXuat != null && s.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                   (s.VatTuBenhVien.VatTus.NuocSanXuat != null && s.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                        DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                        SLYeuCau = s.SoLuong,
                        SLThucXuat = s.SoLuong,
                        LaVatTuBHYT = s.LaVatTuBHYT,
                    })
                    .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.DVT })
                    .Select(item => new ThongTinInPhieuLinhVatTuChiTietVo()
                    {
                        Ma = item.First().Ma,
                        TenThuoc = item.First().TenThuoc,
                        DVT = item.First().DVT,
                        SLYeuCau = item.Sum(x => x.SLYeuCau),
                        SLThucXuat = item.Sum(x => x.SLThucXuat),
                        LaVatTuBHYT = item.First().LaVatTuBHYT,
                    }).OrderBy(d => d.TenThuoc)
                    .ToListAsync();
            }

            var infoLinhDuocChiTiet = string.Empty;
            
            var STT = 1;
            if (query.Any())
            {
                foreach (var item in query.ToList())
                {
                   
                        infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                            + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                            + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                            + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLYeuCau
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLThucXuat
                                            + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"
                                            + "</tr>";
                        STT++;
                }
            }
            data.HeaderPhieuLinhThuoc = hearder;
            data.DanhSachThuoc = infoLinhDuocChiTiet;

            content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongVatTu.Body, data);
            return content;
        }
        #endregion
    }
}
