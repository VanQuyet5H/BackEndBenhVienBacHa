using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial class YeuCauLinhKSNKService
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

        public async Task<GridDataSource> GetKSNKYeuCauLinhBuDataForGridAsync(QueryInfo queryInfo)
        {
            

            var OBJVO = queryInfo.AdditionalSearchString.Split("-");

            var loaiDuocPhamHayVatTu = bool.Parse(OBJVO[1]);
            

            if(loaiDuocPhamHayVatTu == true)
            {
                BuildDefaultSortExpression(queryInfo);
                var yeuCauLinhDuocPhamId = long.Parse(OBJVO[0]);

                var yeuCauLinhDuocPham =
                    await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhDuocPhamId);

                var queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId)
                        .Select(item => new YeuCauLinhBuKSNKGridParentVo()
                        {
                            YeuCauLinhVatTuId = yeuCauLinhDuocPhamId,
                            VatTuBenhVienId = item.DuocPhamBenhVienId,
                            LaBHYT = item.LaDuocPhamBHYT,
                            TenVatTu = item.YeuCauDuocPhamBenhVien.Ten,
                            NongDoHamLuong = (item.YeuCauDuocPhamBenhVien.HamLuong ?? "").Trim(),
                            HoatChat = (item.YeuCauDuocPhamBenhVien.HoatChat ?? "").Trim(),
                            DuongDung = (item.YeuCauDuocPhamBenhVien.DuongDung.Ten ?? "").Trim(), // item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        DonViTinh = (item.YeuCauDuocPhamBenhVien.DonViTinh.Ten ?? "").Trim(), //item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        HangSanXuat = (item.YeuCauDuocPhamBenhVien.NhaSanXuat ?? "").Trim(),
                            NuocSanXuat = (item.YeuCauDuocPhamBenhVien.NuocSanXuat ?? "").Trim(),
                            SoLuongCanBu = item.SoLuong
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.NongDoHamLuong,
                            x.HoatChat,
                            x.DuongDung,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongTon
                        })
                        .Select(item => new YeuCauLinhBuKSNKGridParentVo()
                        {
                            YeuCauLinhVatTuId = yeuCauLinhDuocPhamId, // d
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            TenVatTu = item.First().TenVatTu,
                            NongDoHamLuong = item.First().NongDoHamLuong,
                            HoatChat = item.First().HoatChat,
                            DuongDung = item.First().DuongDung,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                            SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.DuocPhamBenhVienId == item.First().VatTuBenhVienId
                                            && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                        //BVHD-3806
                        KhongHighLight = yeuCauLinhDuocPham.DuocDuyet != null
                        })
                        .OrderByDescending(x => x.HighLightClass).ThenBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();

                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
                var queryTask = queryable.OrderBy(queryInfo.SortString)
                    //.Skip(queryInfo.Skip)
                    //.Take(queryInfo.Take)
                    .ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            else
            {
                BuildDefaultSortExpression(queryInfo);
                var yeuCauLinhVatTuId = long.Parse(OBJVO[0]);
                var yeuCauLinhVatTu =
                await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhVatTuId);

                var queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId)
                        .Select(item => new YeuCauLinhBuKSNKGridParentVo()
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
                        .Select(item => new YeuCauLinhBuKSNKGridParentVo()
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



                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
                var queryTask = queryable.OrderBy(queryInfo.SortString)
                    //.Skip(queryInfo.Skip)
                    //.Take(queryInfo.Take)
                    .ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
        }
        public async Task<GridDataSource> GetKSNKYeuCauLinhBuTotalPageForGridAsync(QueryInfo queryInfo)
        {

            var OBJVO = queryInfo.AdditionalSearchString.Split("-");

            var loaiDuocPhamHayVatTu = bool.Parse(OBJVO[1]);


            if (loaiDuocPhamHayVatTu == true)
            {
                BuildDefaultSortExpression(queryInfo);
                var yeuCauLinhDuocPhamId = long.Parse(OBJVO[0]);

                var yeuCauLinhDuocPham =
                    await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhDuocPhamId);

                var queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId)
                        .Select(item => new YeuCauLinhBuKSNKGridParentVo()
                        {
                            YeuCauLinhVatTuId = yeuCauLinhDuocPhamId,
                            VatTuBenhVienId = item.DuocPhamBenhVienId,
                            LaBHYT = item.LaDuocPhamBHYT,
                            TenVatTu = item.YeuCauDuocPhamBenhVien.Ten,
                            NongDoHamLuong = (item.YeuCauDuocPhamBenhVien.HamLuong ?? "").Trim(),
                            HoatChat = (item.YeuCauDuocPhamBenhVien.HoatChat ?? "").Trim(),
                            DuongDung = (item.YeuCauDuocPhamBenhVien.DuongDung.Ten ?? "").Trim(), // item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                            DonViTinh = (item.YeuCauDuocPhamBenhVien.DonViTinh.Ten ?? "").Trim(), //item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            HangSanXuat = (item.YeuCauDuocPhamBenhVien.NhaSanXuat ?? "").Trim(),
                            NuocSanXuat = (item.YeuCauDuocPhamBenhVien.NuocSanXuat ?? "").Trim(),
                            SoLuongCanBu = item.SoLuong
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhVatTuId,
                            x.VatTuBenhVienId,
                            x.LaBHYT,
                            x.Nhom,
                            x.NongDoHamLuong,
                            x.HoatChat,
                            x.DuongDung,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.SoLuongTon
                        })
                        .Select(item => new YeuCauLinhBuKSNKGridParentVo()
                        {
                            YeuCauLinhVatTuId = yeuCauLinhDuocPhamId, // d
                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                            TenVatTu = item.First().TenVatTu,
                            NongDoHamLuong = item.First().NongDoHamLuong,
                            HoatChat = item.First().HoatChat,
                            DuongDung = item.First().DuongDung,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                            SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                                .Where(x => x.DuocPhamBenhVienId == item.First().VatTuBenhVienId
                                            && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                            && x.NhapKhoDuocPhams.DaHet != true
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT
                                            && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                            //BVHD-3806
                            KhongHighLight = yeuCauLinhDuocPham.DuocDuyet != null
                        })
                        .OrderByDescending(x => x.HighLightClass).ThenBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();

                var countTask = queryable.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            else
            {
                BuildDefaultSortExpression(queryInfo);
                var yeuCauLinhVatTuId = long.Parse(OBJVO[0]);
                var yeuCauLinhVatTu =
                await _yeuCauLinhVatTuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhVatTuId);

                var queryable = _yeuCauLinhVatTuChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhVatTuId == yeuCauLinhVatTuId)
                        .Select(item => new YeuCauLinhBuKSNKGridParentVo()
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
                        .Select(item => new YeuCauLinhBuKSNKGridParentVo()
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



                var countTask = queryable.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }
        }
        public async Task<GridDataSource> GetBenhNhanTheoKSNKCanLinhBuDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObj = queryInfo.AdditionalSearchString.Split(";");

            var loaiDuocPhamHayVatTu = bool.Parse(queryObj[3]);

            if(loaiDuocPhamHayVatTu == true)
            {
                var yeuCauLinhId = long.Parse(queryObj[0]);
                var duocPhamBenhVienId = long.Parse(queryObj[1]);
                bool laBHYT = bool.Parse(queryObj[2]);

                //BVHD-3806
                var yeuCauLinhDuocPham =
                    _yeuCauLinhDuocPhamRepository.TableNoTracking
                        .Include(x => x.YeuCauDuocPhamBenhViens)
                        .First(x => x.Id == yeuCauLinhId);
                var nhapKhoChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                && x.NhapKhoDuocPhams.DaHet != true
                                && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                && o.DuocPhamBenhVienId == duocPhamBenhVienId
                                && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                                && o.LaDuocPhamBHYT == laBHYT
                                && o.YeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien)
                    .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                    .Select(s => new YeuCauLinhKSNKBuGridChildVo
                    {
                        MaYeuCauTiepNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                        SoLuong = s.SoLuong,
                        DichVuKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ?
                            s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu : (s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ?
                                s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                        BacSiKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                        NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh,

                    //BVHD-3806
                    SoLuongTonTheoDichVu = nhapKhoChiTiets.Where(x => x.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                                        && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT)
                                                             .Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                        KhongHighLight = yeuCauLinhDuocPham.DuocDuyet != null
                    });

                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }
            else
            {
                var yeuCauLinhId = long.Parse(queryObj[0]);
                var VatTuBenhVienId = long.Parse(queryObj[1]);
                bool laBHYT = bool.Parse(queryObj[2]);

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
                    .Select(s => new YeuCauLinhKSNKBuGridChildVo
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


                var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
                var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                    .Take(queryInfo.Take).ToArrayAsync();
                await Task.WhenAll(countTask, queryTask);
                return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            }

           
        }
        public async Task<GridDataSource> GetBenhNhanTheoKSNKCanLinhBuTotalPageForGridAsync(QueryInfo queryInfo)
        {

            var queryObj = queryInfo.AdditionalSearchString.Split(";");

            var loaiDuocPhamHayVatTu = bool.Parse(queryObj[3]);

            if (loaiDuocPhamHayVatTu == true)
            {
                var yeuCauLinhId = long.Parse(queryObj[0]);
                var duocPhamBenhVienId = long.Parse(queryObj[1]);
                bool laBHYT = bool.Parse(queryObj[2]);

                //BVHD-3806
                var yeuCauLinhDuocPham =
                    _yeuCauLinhDuocPhamRepository.TableNoTracking
                        .Include(x => x.YeuCauDuocPhamBenhViens)
                        .First(x => x.Id == yeuCauLinhId);
                var nhapKhoChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                && x.NhapKhoDuocPhams.DaHet != true
                                && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                && o.DuocPhamBenhVienId == duocPhamBenhVienId
                                && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                                && o.LaDuocPhamBHYT == laBHYT
                                && o.YeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien)
                    .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                    .Select(s => new YeuCauLinhKSNKBuGridChildVo
                    {
                        MaYeuCauTiepNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                        SoLuong = s.SoLuong,
                        DichVuKham = s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null ?
                            s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu : (s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ?
                                s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.MaDichVu + " - " + s.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                        BacSiKeToa = s.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                        NgayKe = s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                        NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh,

                        //BVHD-3806
                        SoLuongTonTheoDichVu = nhapKhoChiTiets.Where(x => x.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                                            && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT)
                                                             .Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                        KhongHighLight = yeuCauLinhDuocPham.DuocDuyet != null
                    });

                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }
            else
            {
                var yeuCauLinhId = long.Parse(queryObj[0]);
                var VatTuBenhVienId = long.Parse(queryObj[1]);
                bool laBHYT = bool.Parse(queryObj[2]);

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
                    .Select(s => new YeuCauLinhKSNKBuGridChildVo
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


                var countTask = query.CountAsync();
                await Task.WhenAll(countTask);
                return new GridDataSource { TotalRowCount = countTask.Result };
            }




           
           
        }


        public async Task<GridDataSource> GetKSNKYeuCauLinhTrucTiepDataForGridAsync(QueryInfo queryInfo)
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

            IQueryable<YeuCauLinhKSNKKSNKGridParentVo> queryable = null;

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
                        .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
                        .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
                        .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
                        .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
                    .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
                    .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
        public async Task<GridDataSource> GetKSNKYeuCauLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var quyeryObj = queryInfo.AdditionalSearchString.Split(';');
            var subObj = quyeryObj[0].Split('|');
            var yeuCauLinhVatTuId = long.Parse(subObj[0]);

            var isTuChoiDuyet = false;
            if (quyeryObj.Length > 1)
            {
                isTuChoiDuyet = quyeryObj[1] == "FALSE";
            }

            IQueryable<YeuCauLinhKSNKKSNKGridParentVo> queryable = null;
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
                    .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
                    .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
                    .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
                    .Select(item => new YeuCauLinhKSNKKSNKGridParentVo()
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
        public async Task<GridDataSource> GetBenhNhanTheoKSNKCanLinhTrucTiepDataForGridAsync(QueryInfo queryInfo)
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

            IQueryable<YeuCauLinhKSNKTrucTiepGridChildVo> query = null;
            // cập nhật 08/11/2021: trường hợp chưa duyệt: thì lấy số lượng theo YCVT
            if (yeuCauLinhVatTu.DuocDuyet != true)
            {
                query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(o => o.YeuCauLinhVatTuId == yeuCauLinhId
                                && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                && o.YeuCauTiepNhan.BenhNhanId != null)
                    .OrderBy(x => x.ThoiDiemChiDinh)
                    .Select(s => new YeuCauLinhKSNKTrucTiepGridChildVo
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
                    }).Select(s => new YeuCauLinhKSNKTrucTiepGridChildVo
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
                    .Select(s => new YeuCauLinhKSNKTrucTiepGridChildVo
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
                    }).Select(s => new YeuCauLinhKSNKTrucTiepGridChildVo
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
        public async Task<GridDataSource> GetBenhNhanTheoKSNKCanLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauLinhId = long.Parse(queryInfo.AdditionalSearchString);

            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => o.YeuCauLinhVatTuId == yeuCauLinhId
                            && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                            && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                            && o.YeuCauTiepNhan.BenhNhanId != null)
                .OrderBy(x => x.ThoiDiemChiDinh)
                .Select(s => new YeuCauLinhKSNKTrucTiepGridChildVo
                {
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    SoLuong = s.SoLuong
                }).GroupBy(x => new
                {
                    x.YeuCauTiepNhanId
                }).Select(s => new YeuCauLinhKSNKTrucTiepGridChildVo
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
        public async Task XuLyDuyetYeuCauLinhKSNKThuongAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhVatTu, long nguoiXuatId, long nguoiNhapId)
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
                    yeuCauLinhChiTiet.SoLuongCoTheXuat.GetValueOrDefault())
                {
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhVatTuThuong.SoLuongTon.KhongDu"));
                }

                var newXuatKhoChiTiet = new XuatKhoVatTuChiTiet()
                {
                    VatTuBenhVienId = yeuCauLinhChiTiet.VatTuBenhVienId,
                    NgayXuat = thoiDiemHienTai
                };

                var slYeuCau = yeuCauLinhChiTiet.SoLuongCoTheXuat.GetValueOrDefault();
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

        public async Task<string> InPhieuDuyetLinhThuongKSNKAsync(InPhieuDuyetLinhKSNK inPhieu)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var templateLinhThuongVatTu = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocThuong"));
            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == inPhieu.YeuCauLinhVatTuId)
                .Select(item => new ThongTinInPhieuLinhKSNKVo()
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
                    .Select(s => new ThongTinInPhieuLinhKSNKChiTietVo
                    {
                        Ma = s.VatTuBenhVien.Ma,
                        TenThuoc = s.VatTuBenhVien.VatTus.Ten +
                                               (s.VatTuBenhVien.VatTus.NhaSanXuat != null && s.VatTuBenhVien.VatTus.NhaSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NhaSanXuat : "") +
                                               (s.VatTuBenhVien.VatTus.NuocSanXuat != null && s.VatTuBenhVien.VatTus.NuocSanXuat != "" ? "; " + s.VatTuBenhVien.VatTus.NuocSanXuat : ""),
                        DVT = s.VatTuBenhVien.VatTus.DonViTinh,
                        SLYeuCau = s.SoLuong,
                        SLThucXuat = s.SoLuongCoTheXuat.GetValueOrDefault(),
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

        public async Task XuLyDuyetYeuCauLinhKSNKBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, long nguoiXuatId, long nguoiNhapId)
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
            await _nhapKhoVatTuChiTietRepository.UpdateAsync(lstNhapKhoTatCaVatTuYeuCauLinh);
        }

        public async Task<string> InPhieuDuyetLinhBuKSNKAsync(InPhieuDuyetLinhKSNK inPhieu)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var templateLinhThuongVatTu = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocBu"));
            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == inPhieu.YeuCauLinhVatTuId)
                .Select(item => new ThongTinInPhieuLinhKSNKVo()
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
                .Select(s => new ThongTinInPhieuLinhKSNKChiTietVo
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
                }).OrderByDescending(p => p.LaVatTuBHYT).ThenBy(p => !p.LaVatTuBHYT)
                .GroupBy(x => new { x.VatTuBenhVienId, x.LaVatTuBHYT, x.Ma, x.DVT })
                .Select(item => new ThongTinInPhieuLinhKSNKChiTietVo()
                {
                    Ma = item.First().Ma,
                    TenThuoc = item.First().TenThuoc,
                    DVT = item.First().DVT,
                    SLYeuCau = item.Sum(x => x.SLYeuCau),
                    SLThucXuat = item.Sum(x => x.SLThucXuat),
                    LaVatTuBHYT = item.First().LaVatTuBHYT
                }).OrderByDescending(z => z.LaVatTuBHYT).ThenBy(z => z.TenThuoc)
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
    }
}
