using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial class YeuCauLinhDuocPhamService
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
                .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName).Distinct()
                .Take(model.Take)
                .ToListAsync();
            return lstNhanVien;
        }

        public async Task<GridDataSource> GetDuocPhamYeuCauLinhBuDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauLinhDuocPhamId = long.Parse(queryInfo.AdditionalSearchString);

            var yeuCauLinhDuocPham =
                await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhDuocPhamId);

            var queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId)
                    .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                        LaBHYT = item.LaDuocPhamBHYT,
                        TenDuocPham = item.YeuCauDuocPhamBenhVien.Ten,
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
                        x.YeuCauLinhDuocPhamId,
                        x.DuocPhamBenhVienId,
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
                    .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenDuocPham = item.First().TenDuocPham,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                        SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                        && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                        && x.NhapKhoDuocPhams.DaHet != true
                                        && x.LaDuocPhamBHYT == item.First().LaBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                        //BVHD-3806
                        KhongHighLight = yeuCauLinhDuocPham.DuocDuyet != null
                    })
                    .OrderByDescending(x => x.HighLightClass).ThenBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
            var queryTask = queryable.OrderBy(queryInfo.SortString)
                //.Skip(queryInfo.Skip)
                //.Take(queryInfo.Take)
                .ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDuocPhamYeuCauLinhBuTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauLinhDuocPhamId = long.Parse(queryInfo.AdditionalSearchString);

            var yeuCauLinhDuocPham =
                await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauLinhDuocPhamId);
            var queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId)
                    .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                        LaBHYT = item.LaDuocPhamBHYT,
                        TenDuocPham = item.YeuCauDuocPhamBenhVien.Ten,
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
                        x.YeuCauLinhDuocPhamId,
                        x.DuocPhamBenhVienId,
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
                    .Select(item => new YeuCauLinhDuocPhamBuGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenDuocPham = item.First().TenDuocPham,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongCanBu = item.Sum(x => x.SoLuongCanBu),
                        SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                        && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                        && x.NhapKhoDuocPhams.DaHet != true
                                        && x.LaDuocPhamBHYT == item.First().LaBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();

            var countTask = queryable.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetBenhNhanTheoDuocPhamCanLinhBuDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObj = queryInfo.AdditionalSearchString.Split(";");
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
                .Select(s => new YeuCauLinhDuocPhamBuGridChildVo
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
            //.GroupBy(x => new
            //{
            //    x.MaYeuCauTiepNhan,
            //    x.MaBenhNhan,
            //    x.HoTen,
            //    x.DichVuKham,
            //    x.BacSiKeToa,
            //    x.NgayKe
            //})
            //.Select(s => new YeuCauLinhDuocPhamBuGridChildVo
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
        public async Task<GridDataSource> GetBenhNhanTheoDuocPhamCanLinhBuTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauLinhId = long.Parse(queryObj[0]);
            var duocPhamBenhVienId = long.Parse(queryObj[1]);
            bool laBHYT = bool.Parse(queryObj[2]);

            var query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                            && o.DuocPhamBenhVienId == duocPhamBenhVienId
                            && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu
                            && o.LaDuocPhamBHYT == laBHYT
                            && o.YeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien)
                .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                .Select(s => new YeuCauLinhDuocPhamBuGridChildVo
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
                    NgayDieuTri = s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? s.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : s.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
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
            //.Select(s => new YeuCauLinhDuocPhamBuGridChildVo
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


        public async Task<GridDataSource> GetDuocPhamYeuCauLinhTrucTiepDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var quyeryObj = queryInfo.AdditionalSearchString.Split(';');
            var subObj = quyeryObj[0].Split('|');
            var yeuCauLinhDuocPhamId = long.Parse(subObj[0]);

            var isTuChoiDuyet = false;
            if (quyeryObj.Length > 1)
            {
                isTuChoiDuyet = quyeryObj[1] == "FALSE";
            }

            var yeuCauLinhDuocPham =
                 _yeuCauLinhDuocPhamRepository.TableNoTracking
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .First(x => x.Id == yeuCauLinhDuocPhamId);
            var nhapKhoChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                            && x.NhapKhoDuocPhams.DaHet != true
                            && x.SoLuongDaXuat < x.SoLuongNhap
                            
                            //BVHD-3821
                            // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                            && x.HanSuDung.Date >= DateTime.Now.Date
                            ).ToList();

            IQueryable<YeuCauLinhDuocPhamTrucTiepGridParentVo> queryable = null;

            // cập nhật 29/10/2021: trường hợp đã duyệt thì lấy thông tin từ YeCauLinhDuocPhamCHiTiet
            if (!isTuChoiDuyet)
            {
                long yeuCauTiepNhanId = 0;
                if (subObj.Length > 1)
                {
                    yeuCauTiepNhanId = long.Parse(subObj[1]);
                }

                if (yeuCauLinhDuocPham.DuocDuyet != true)
                {
                    queryable = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                                    && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                    && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                    && x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                        .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                        {
                            YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                            LaBHYT = item.LaDuocPhamBHYT,
                            TenDuocPham = item.Ten,
                            NongDoHamLuong = item.HamLuong,
                            HoatChat = item.HoatChat,
                            DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                            DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
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
                            x.YeuCauLinhDuocPhamId,
                            x.DuocPhamBenhVienId,
                            x.LaBHYT,
                            x.TenDuocPham,
                            x.Nhom,
                            x.NongDoHamLuong,
                            x.HoatChat,
                            x.DuongDung,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.DichVuKham,
                            x.BacSiKeToa,
                            x.NgayKe
                        })
                        .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                        {
                            YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenDuocPham = item.First().TenDuocPham,
                            NongDoHamLuong = item.First().NongDoHamLuong,
                            HoatChat = item.First().HoatChat,
                            DuongDung = item.First().DuongDung,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                        //SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        //    .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                        //                && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                        //                && x.LaDuocPhamBHYT == item.First().LaBHYT
                        //                && x.NhapKhoDuocPhams.DaHet != true
                        //                && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                        SoLuongTon = nhapKhoChiTiets
                                .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            SoLuongTonTheoDuocPham = nhapKhoChiTiets
                                .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                                         - yeuCauLinhDuocPham.YeuCauDuocPhamBenhViens.Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                                                                                 && x.LaDuocPhamBHYT == item.First().LaBHYT
                                                                                                 && x.YeuCauTiepNhanId != yeuCauTiepNhanId
                                                                                                 && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                 && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                                             .Sum(x => x.SoLuong),
                        //HighLightClass = (nhapKhoChiTiets
                        //    .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                        //                && x.LaDuocPhamBHYT == item.First().LaBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                        //             - yeuCauLinhDuocPham.YeuCauDuocPhamBenhViens.Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                        //                                                                     && x.LaDuocPhamBHYT == item.First().LaBHYT
                        //                                                                     && x.YeuCauTiepNhanId != yeuCauTiepNhanId
                        //                                                                     && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                        //                                                                     && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                        //                 .Sum(x => x.SoLuong)) > item.Sum(x => x.SoLuongYeuCau) ? "" : "bg-row-lightRed",
                        DichVuKham = item.First().DichVuKham,
                            BacSiKeToa = item.First().BacSiKeToa,
                            NgayKe = item.First().NgayKe,
                            NgayDieuTri = item.First().NgayDieuTri,
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
                }
                else
                {
                    queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                        .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                                    && x.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                    && x.YeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                    && x.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId == yeuCauTiepNhanId)
                        .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                        {
                            YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                            DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                            LaBHYT = item.LaDuocPhamBHYT,
                            TenDuocPham = item.YeuCauDuocPhamBenhVien.Ten,
                            NongDoHamLuong = item.YeuCauDuocPhamBenhVien.HamLuong,
                            HoatChat = item.YeuCauDuocPhamBenhVien.HoatChat,
                            DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                            DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                            HangSanXuat = item.YeuCauDuocPhamBenhVien.NhaSanXuat,
                            NuocSanXuat = item.YeuCauDuocPhamBenhVien.NuocSanXuat,
                            SoLuongYeuCau = item.SoLuong,

                            DichVuKham = item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh != null
                                ? item.YeuCauDuocPhamBenhVien.YeuCauKhamBenh.TenDichVu
                                : (item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDuocPhamBenhVien.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : "Ghi nhận trong PTTT"),
                            BacSiKeToa = item.YeuCauDuocPhamBenhVien.NhanVienChiDinh.User.HoTen,
                            NgayKe = item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                            NgayDieuTri = item.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri != null ? item.YeuCauDuocPhamBenhVien.NoiTruPhieuDieuTri.NgayDieuTri : item.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh
                        })
                        .GroupBy(x => new
                        {
                            x.YeuCauLinhDuocPhamId,
                            x.DuocPhamBenhVienId,
                            x.LaBHYT,
                            x.TenDuocPham,
                            x.Nhom,
                            x.NongDoHamLuong,
                            x.HoatChat,
                            x.DuongDung,
                            x.DonViTinh,
                            x.HangSanXuat,
                            x.NuocSanXuat,
                            x.DichVuKham,
                            x.BacSiKeToa,
                            x.NgayKe
                        })
                        .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                        {
                            YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                            DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                            LaBHYT = item.First().LaBHYT,
                            TenDuocPham = item.First().TenDuocPham,
                            NongDoHamLuong = item.First().NongDoHamLuong,
                            HoatChat = item.First().HoatChat,
                            DuongDung = item.First().DuongDung,
                            DonViTinh = item.First().DonViTinh,
                            HangSanXuat = item.First().HangSanXuat,
                            NuocSanXuat = item.First().NuocSanXuat,
                            SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                            SoLuongTon = nhapKhoChiTiets
                                .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                            SoLuongTonTheoDuocPham = nhapKhoChiTiets
                                .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                            && x.LaDuocPhamBHYT == item.First().LaBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                                         - yeuCauLinhDuocPham.YeuCauDuocPhamBenhViens.Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                                                                                 && x.LaDuocPhamBHYT == item.First().LaBHYT
                                                                                                 && x.YeuCauTiepNhanId != yeuCauTiepNhanId
                                                                                                 && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                 && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                                             .Sum(x => x.SoLuong),
                            DichVuKham = item.First().DichVuKham,
                            BacSiKeToa = item.First().BacSiKeToa,
                            NgayKe = item.First().NgayKe,
                            NgayDieuTri = item.First().NgayDieuTri,

                            // trường hợp đã duyệt, từ chối thì ko cần highlight
                            KhongHighLight = yeuCauLinhDuocPham.DuocDuyet != null
                        })
                        .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
                }

                if (queryInfo.Sort.Count == 1 && !string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
                {
                    queryInfo.Sort[0].Dir = "desc";
                    queryInfo.Sort[0].Field = "HighLightClass";
                }
            }
            else
            {
                queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId)
                    .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                        LaBHYT = item.LaDuocPhamBHYT,
                        TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                        NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                        HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                        DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        SoLuongYeuCau = item.SoLuong,
                    })
                    .GroupBy(x => new
                    {
                        x.YeuCauLinhDuocPhamId,
                        x.DuocPhamBenhVienId,
                        x.LaBHYT,
                        x.Nhom,
                        x.NongDoHamLuong,
                        x.HoatChat,
                        x.DuongDung,
                        x.DonViTinh,
                        x.HangSanXuat,
                        x.NuocSanXuat,
                        x.SoLuongYeuCau
                    })
                    .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenDuocPham = item.First().TenDuocPham,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                        SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                        && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                                        && x.NhapKhoDuocPhams.DaHet != true
                                        && x.LaDuocPhamBHYT == item.First().LaBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),

                        // trường hợp đã duyệt, từ chối thì ko cần highlight
                        KhongHighLight = yeuCauLinhDuocPham.DuocDuyet != null
                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : queryable.CountAsync();
            var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDuocPhamYeuCauLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var quyeryObj = queryInfo.AdditionalSearchString.Split(';');
            var subObj = quyeryObj[0].Split('|');
            var yeuCauLinhDuocPhamId = long.Parse(subObj[0]);

            var isTuChoiDuyet = false;
            if (quyeryObj.Length > 1)
            {
                isTuChoiDuyet = quyeryObj[1] == "FALSE";
            }

            IQueryable<YeuCauLinhDuocPhamTrucTiepGridParentVo> queryable = null;
            if (!isTuChoiDuyet)
            {
                long yeuCauTiepNhanId = 0;
                if (subObj.Length > 1)
                {
                    yeuCauTiepNhanId = long.Parse(subObj[1]);
                }
                queryable = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId
                                && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                && x.YeuCauTiepNhanId == yeuCauTiepNhanId)
                    .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                        LaBHYT = item.LaDuocPhamBHYT,
                        TenDuocPham = item.Ten,
                        //Nhom = item.LaDuocPhamBHYT ? "Thuốc BHYT" : "Thuốc không BHYT",
                        NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                        HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                        DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        SoLuongYeuCau = item.SoLuong,
                        DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                        BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                        NgayKe = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                        NgayDieuTri = item.NoiTruPhieuDieuTri != null ? item.NoiTruPhieuDieuTri.NgayDieuTri :item.ThoiDiemChiDinh
                    })
                    .GroupBy(x => new
                    {
                        x.YeuCauLinhDuocPhamId,
                        x.DuocPhamBenhVienId,
                        x.LaBHYT,
                        x.TenDuocPham,
                        x.Nhom,
                        x.NongDoHamLuong,
                        x.HoatChat,
                        x.DuongDung,
                        x.DonViTinh,
                        x.HangSanXuat,
                        x.NuocSanXuat,
                        x.DichVuKham,
                        x.BacSiKeToa,
                        x.NgayKe
                    })
                    .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenDuocPham = item.First().TenDuocPham,
                        //Nhom = item.First().Nhom,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                        //SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        //    .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                        //                && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                        //                && x.LaDuocPhamBHYT == item.First().LaBHYT
                        //                && x.NhapKhoDuocPhams.DaHet != true
                        //                && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                        NgayDieuTri =item.First().NgayDieuTri

                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
            }
            else
            {
                queryable = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhDuocPhamId == yeuCauLinhDuocPhamId)
                    .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                        LaBHYT = item.LaDuocPhamBHYT,
                        TenDuocPham = item.DuocPhamBenhVien.DuocPham.Ten,
                        //Nhom = item.LaDuocPhamBHYT ? "Thuốc BHYT" : "Thuốc không BHYT",
                        NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                        HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                        DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                        DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                        HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                        NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                        SoLuongYeuCau = item.SoLuong,
                    })
                    .GroupBy(x => new
                    {
                        x.YeuCauLinhDuocPhamId,
                        x.DuocPhamBenhVienId,
                        x.LaBHYT,
                        x.Nhom,
                        x.NongDoHamLuong,
                        x.HoatChat,
                        x.DuongDung,
                        x.DonViTinh,
                        x.HangSanXuat,
                        x.NuocSanXuat,
                        x.SoLuongYeuCau
                    })
                    .Select(item => new YeuCauLinhDuocPhamTrucTiepGridParentVo()
                    {
                        YeuCauLinhDuocPhamId = yeuCauLinhDuocPhamId,
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenDuocPham = item.First().TenDuocPham,
                        //Nhom = item.First().Nhom,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                        //SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        //    .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                        //                && x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                        //                && x.NhapKhoDuocPhams.DaHet != true
                        //                && x.LaDuocPhamBHYT == item.First().LaBHYT
                        //                && x.SoLuongDaXuat < x.SoLuongNhap).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                    })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
            }

            var countTask = queryable.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetBenhNhanTheoDuocPhamCanLinhTrucTiepDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var yeuCauLinhId = long.Parse(queryInfo.AdditionalSearchString);

            var yeuCauLinhDuocPham =
                 _yeuCauLinhDuocPhamRepository.TableNoTracking
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .First(x => x.Id == yeuCauLinhId);
            var nhapKhoChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                            && x.NhapKhoDuocPhams.DaHet != true
                            && x.SoLuongDaXuat < x.SoLuongNhap

                            //BVHD-3821
                            // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                            && x.HanSuDung.Date >= DateTime.Now.Date
                ).ToList();

            IQueryable<YeuCauLinhDuocPhamTrucTiepGridChildVo> query = null;
            // cập nhật 29/10/2021: trường hợp chưa duyệt: thì lấy số lượng theo YCDP
            if (yeuCauLinhDuocPham.DuocDuyet != true)
            {
                query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                   .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                               && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                               && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                               && o.YeuCauTiepNhan.BenhNhanId != null)
                   .OrderBy(x => x.ThoiDiemChiDinh)
                   .Select(s => new YeuCauLinhDuocPhamTrucTiepGridChildVo
                   {
                       YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    //BenhNhanId = s.YeuCauTiepNhan.BenhNhanId.Value, 
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                       MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                       HoTen = s.YeuCauTiepNhan.HoTen,
                       SoLuong = s.SoLuong,
                    //DichVuKham = s.YeuCauKhamBenh != null ? s.YeuCauKhamBenh.TenDichVu : (s.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? s.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                    //BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                    //NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH()
                    SoLuongTonTheoDichVu = (nhapKhoChiTiets
                                             .Where(x => x.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                         && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                                         - yeuCauLinhDuocPham.YeuCauDuocPhamBenhViens.Where(x => x.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                                                                 && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                                                                                 && x.Id != s.Id
                                                                                                 && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                 && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                                             .Sum(x => x.SoLuong)),
                   })
                   .GroupBy(x => new
                   {
                       x.YeuCauTiepNhanId,
                    //x.BenhNhanId,
                    //x.MaYeuCauTiepNhan,
                    //x.MaBenhNhan,
                    //x.HoTen,
                    //x.DichVuKham,
                    //x.BacSiKeToa,
                    //x.NgayKe
                })
                   .Select(s => new YeuCauLinhDuocPhamTrucTiepGridChildVo
                   {
                    //BenhNhanId = s.First().BenhNhanId,
                    YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                       MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                       MaBenhNhan = s.First().MaBenhNhan,
                       HoTen = s.First().HoTen,
                       SoLuong = s.Sum(a => a.SoLuong),

                       // trường hợp đã duyệt, từ chối thì ko cần highlight
                       HighLightClass = (s.Any(a => a.KhongDuTon) && yeuCauLinhDuocPham.DuocDuyet == null) ? "bg-row-lightRed" : ""
                    //DichVuKham = s.First().DichVuKham,
                    //BacSiKeToa = s.First().BacSiKeToa,
                    //NgayKe = s.First().NgayKe
                });
            }

            // cập nhật 29/10/2021: trường hợp đã duyệt: thì lấy số lượng theo YeuCauLinhDuocPhamCHiTiet
            else
            {
                query = _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                                && o.YeuCauDuocPhamBenhVien.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                                && o.YeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                && o.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhanId != null)
                    .OrderBy(x => x.YeuCauDuocPhamBenhVien.ThoiDiemChiDinh)
                    .Select(s => new YeuCauLinhDuocPhamTrucTiepGridChildVo
                    {
                        YeuCauTiepNhanId = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                        MaYeuCauTiepNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBenhNhan = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = s.YeuCauDuocPhamBenhVien.YeuCauTiepNhan.HoTen,
                        SoLuong = s.SoLuong,
                        SoLuongTonTheoDichVu = (nhapKhoChiTiets
                                             .Where(x => x.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                         && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                                         - yeuCauLinhDuocPham.YeuCauDuocPhamBenhViens.Where(x => x.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                                                                 && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                                                                                 && x.Id != s.Id
                                                                                                 && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                 && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                                             .Sum(x => x.SoLuong)),
                    })
                   .GroupBy(x => new
                   {
                       x.YeuCauTiepNhanId
                   })
                   .Select(s => new YeuCauLinhDuocPhamTrucTiepGridChildVo
                   {
                       YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                       MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                       MaBenhNhan = s.First().MaBenhNhan,
                       HoTen = s.First().HoTen,
                       SoLuong = s.Sum(a => a.SoLuong),

                       // trường hợp đã duyệt, từ chối thì ko cần highlight
                       HighLightClass = (s.Any(a => a.KhongDuTon) && yeuCauLinhDuocPham.DuocDuyet == null) ? "bg-row-lightRed" : ""
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
        public async Task<GridDataSource> GetBenhNhanTheoDuocPhamCanLinhTrucTiepTotalPageForGridAsync(QueryInfo queryInfo)
        {
            //var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauLinhId = long.Parse(queryInfo.AdditionalSearchString);
            //var duocPhamBenhVienId = long.Parse(queryObj[1]);
            //bool laBHYT = bool.Parse(queryObj[2]);

            var yeuCauLinhDuocPham =
                _yeuCauLinhDuocPhamRepository.TableNoTracking
                    .Include(x => x.YeuCauDuocPhamBenhViens)
                    .First(x => x.Id == yeuCauLinhId);
            var nhapKhoChiTiets = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                            && x.NhapKhoDuocPhams.DaHet != true
                            && x.SoLuongDaXuat < x.SoLuongNhap).ToList();

            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => o.YeuCauLinhDuocPhamId == yeuCauLinhId
                            && o.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan
                            && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                            && o.YeuCauTiepNhan.BenhNhanId != null)
                .OrderBy(x => x.ThoiDiemChiDinh)
                .Select(s => new YeuCauLinhDuocPhamTrucTiepGridChildVo
                {
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    //BenhNhanId = s.YeuCauTiepNhan.BenhNhanId.Value,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    SoLuong = s.SoLuong,
                    SoLuongTonTheoDichVu = (nhapKhoChiTiets
                                                .Where(x => x.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                            && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                                            - yeuCauLinhDuocPham.YeuCauDuocPhamBenhViens.Where(x => x.DuocPhamBenhVienId == s.DuocPhamBenhVienId
                                                                                                    && x.LaDuocPhamBHYT == s.LaDuocPhamBHYT
                                                                                                    && x.Id != s.Id
                                                                                                    && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                                    && x.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhChoBenhNhan)
                                                .Sum(x => x.SoLuong)),
                })
                .GroupBy(x => new
                {
                    x.YeuCauTiepNhanId
                })
                .Select(s => new YeuCauLinhDuocPhamTrucTiepGridChildVo
                {
                    //BenhNhanId = s.First().BenhNhanId,
                    YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                    MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                    MaBenhNhan = s.First().MaBenhNhan,
                    HoTen = s.First().HoTen,
                    SoLuong = s.Sum(a => a.SoLuong),
                    HighLightClass = s.Any(a => a.KhongDuTon) ? "bg-row-lightRed" : ""
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh thường
        public async Task XuLyDuyetYeuCauLinhDuocPhamThuongAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhDuocPham, long nguoiXuatId, long nguoiNhapId)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            // cập nhật thông tin duyệt
            var thoiDiemHienTai = DateTime.Now;


            // xử lý nhập xuất dược phẩm
            #region Xử lý xuất
            var newNhapKho = new NhapKhoDuocPham()
            {
                NguoiNhapId = nguoiNhapId,
                LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayNhap = thoiDiemHienTai,
                KhoId = yeuCauLinhDuocPham.KhoNhapId,
                YeuCauLinhDuocPhamId = yeuCauLinhDuocPham.Id
            };

            var newPhieuXuat = new XuatKhoDuocPham()
            {
                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac.GetDescription(),
                NguoiNhanId = nguoiNhapId,
                NguoiXuatId = nguoiXuatId,
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayXuat = thoiDiemHienTai,
                KhoXuatId = yeuCauLinhDuocPham.KhoXuatId,
                KhoNhapId = yeuCauLinhDuocPham.KhoNhapId
            };

            var lstNhapKhoTatCaDuocPhamYeuCauLinh = await _nhapKhoDuocPhamChiTietRepository.Table
                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                            && yeuCauLinhDuocPham.YeuCauLinhDuocPhamChiTiets.Any(y => y.DuocPhamBenhVienId == x.DuocPhamBenhVienId && (x.SoLuongDaXuat < x.SoLuongNhap)))
                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                .ToListAsync();
            foreach (var yeuCauLinhChiTiet in yeuCauLinhDuocPham.YeuCauLinhDuocPhamChiTiets)
            {
                var lstNhapKhoChiTiet = lstNhapKhoTatCaDuocPhamYeuCauLinh.Where(x =>
                    x.DuocPhamBenhVienId == yeuCauLinhChiTiet.DuocPhamBenhVienId &&
                    x.LaDuocPhamBHYT == yeuCauLinhChiTiet.LaDuocPhamBHYT &&
                    x.SoLuongDaXuat < x.SoLuongNhap).ToList();


                if (!lstNhapKhoChiTiet.Any() || lstNhapKhoChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) <
                    yeuCauLinhChiTiet.SoLuong)
                {
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.SoLuongTon.KhongDu"));
                }

                var newXuatKhoChiTiet = new XuatKhoDuocPhamChiTiet()
                {
                    DuocPhamBenhVienId = yeuCauLinhChiTiet.DuocPhamBenhVienId,
                    NgayXuat = thoiDiemHienTai
                };

                var slYeuCau = yeuCauLinhChiTiet.SoLuong;
                var isExists = false;
                foreach (var nhapChiTiet in lstNhapKhoChiTiet)
                {
                    if (slYeuCau > 0)
                    {
                        var newNhapKhoChiTiet = new NhapKhoDuocPhamChiTiet()
                        {
                            SoLuongNhap = 0,
                            SoLuongDaXuat = 0
                        };
                        var nhapKhoChiTiettemp = newNhapKho.NhapKhoDuocPhamChiTiets.FirstOrDefault(x => x.DuocPhamBenhVienId == nhapChiTiet.DuocPhamBenhVienId
                                                                                                        && x.LaDuocPhamBHYT == nhapChiTiet.LaDuocPhamBHYT
                                                                                                        && x.NgayNhapVaoBenhVien == nhapChiTiet.NgayNhapVaoBenhVien
                                                                                                        && x.DonGiaNhap == nhapChiTiet.DonGiaNhap
                                                                                                        && x.VAT == nhapChiTiet.VAT
                                                                                                        && x.TiLeTheoThapGia == nhapChiTiet.TiLeTheoThapGia
                                                                                                        && x.MaVach == nhapChiTiet.MaVach
                                                                                                        && x.MaRef == nhapChiTiet.MaRef
                                                                                                        && x.HanSuDung == nhapChiTiet.HanSuDung
                                                                                                        && x.Solo == nhapChiTiet.Solo
                                                                                                        && x.HopDongThauDuocPhamId == nhapChiTiet.HopDongThauDuocPhamId
                                                                                                        && x.NgayNhap == nhapChiTiet.NgayNhap
                                                                                                        && x.DuocPhamBenhVienPhanNhomId == nhapChiTiet.DuocPhamBenhVienPhanNhomId
                                                                                                        && x.TiLeBHYTThanhToan == nhapChiTiet.TiLeBHYTThanhToan);
                        if (nhapKhoChiTiettemp != null)
                        {
                            newNhapKhoChiTiet = nhapKhoChiTiettemp;
                            isExists = true;
                        }
                        else
                        {
                            isExists = false;
                            newNhapKhoChiTiet.DuocPhamBenhVienId = nhapChiTiet.DuocPhamBenhVienId;
                            newNhapKhoChiTiet.HopDongThauDuocPhamId = nhapChiTiet.HopDongThauDuocPhamId;
                            newNhapKhoChiTiet.Solo = nhapChiTiet.Solo;
                            newNhapKhoChiTiet.HanSuDung = nhapChiTiet.HanSuDung;
                            newNhapKhoChiTiet.MaVach = nhapChiTiet.MaVach;
                            newNhapKhoChiTiet.MaRef = nhapChiTiet.MaRef;
                            newNhapKhoChiTiet.LaDuocPhamBHYT = nhapChiTiet.LaDuocPhamBHYT;
                            newNhapKhoChiTiet.DuocPhamBenhVienPhanNhomId = nhapChiTiet.DuocPhamBenhVienPhanNhomId;
                            newNhapKhoChiTiet.NgayNhapVaoBenhVien = nhapChiTiet.NgayNhapVaoBenhVien;
                            newNhapKhoChiTiet.PhuongPhapTinhGiaTriTonKho = nhapChiTiet.PhuongPhapTinhGiaTriTonKho;
                            newNhapKhoChiTiet.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                            newNhapKhoChiTiet.VAT = nhapChiTiet.VAT;
                            newNhapKhoChiTiet.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                            newNhapKhoChiTiet.NgayNhap = thoiDiemHienTai;
                            newNhapKhoChiTiet.TiLeBHYTThanhToan = nhapChiTiet.TiLeBHYTThanhToan;
                            newNhapKhoChiTiet.DanhSachMayXetNghiemId = yeuCauLinhChiTiet.DanhSachMayXetNghiemId;
                        }

                        var newXuatKhoChiTietViTri = new XuatKhoDuocPhamChiTietViTri()
                        {
                            NhapKhoDuocPhamChiTietId = nhapChiTiet.Id,
                            NgayXuat = thoiDiemHienTai
                        };

                        var slTonNhapChiTiet = nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat;
                        if (slYeuCau <= slTonNhapChiTiet)
                        {
                            newXuatKhoChiTietViTri.SoLuongXuat = slYeuCau;
                            nhapChiTiet.SoLuongDaXuat += slYeuCau;

                            newNhapKhoChiTiet.SoLuongNhap += slYeuCau;

                            slYeuCau = 0;

                        }
                        else
                        {
                            newXuatKhoChiTietViTri.SoLuongXuat = slTonNhapChiTiet;

                            newNhapKhoChiTiet.SoLuongNhap += slTonNhapChiTiet;

                            //slYeuCau = nhapChiTiet.SoLuongNhap - nhapChiTiet.SoLuongDaXuat;
                            slYeuCau -= slTonNhapChiTiet;
                            nhapChiTiet.SoLuongDaXuat = nhapChiTiet.SoLuongNhap;
                        }
                        newXuatKhoChiTiet.XuatKhoDuocPhamChiTietViTris.Add(newXuatKhoChiTietViTri);
                        if (!isExists)
                        {
                            newNhapKho.NhapKhoDuocPhamChiTiets.Add(newNhapKhoChiTiet);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                newPhieuXuat.XuatKhoDuocPhamChiTiets.Add(newXuatKhoChiTiet);
            }
            #endregion

            newPhieuXuat.NhapKhoDuocPhams.Add(newNhapKho);
            yeuCauLinhDuocPham.XuatKhoDuocPhams.Add(newPhieuXuat);


            // lưu thông tin yêu cầu lĩnh
            await _yeuCauLinhDuocPhamRepository.UpdateAsync(yeuCauLinhDuocPham);
            // cập nhật thông tin tông
            //await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(lstNhapKhoTatCaDuocPhamYeuCauLinh);
        }

        public async Task XuLyDuyetYeuCauLinhDuocPhamThuongAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhDuocPham, List<DuocPhamCanXuatVo> duocPhamCanXuatVos, long nguoiXuatId, long nguoiNhapId)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            // cập nhật thông tin duyệt
            var thoiDiemHienTai = DateTime.Now;


            // xử lý nhập xuất dược phẩm
            #region Xử lý xuất
            var newNhapKho = new NhapKhoDuocPham()
            {
                NguoiNhapId = nguoiNhapId,
                LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayNhap = thoiDiemHienTai,
                KhoId = yeuCauLinhDuocPham.KhoNhapId,
                YeuCauLinhDuocPhamId = yeuCauLinhDuocPham.Id
            };

            var newPhieuXuat = new XuatKhoDuocPham()
            {
                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac.GetDescription(),
                NguoiNhanId = nguoiNhapId,
                NguoiXuatId = nguoiXuatId,
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayXuat = thoiDiemHienTai,
                KhoXuatId = yeuCauLinhDuocPham.KhoXuatId,
                KhoNhapId = yeuCauLinhDuocPham.KhoNhapId
            };

            var lstNhapKhoTatCaDuocPhamYeuCauLinh = await _nhapKhoDuocPhamChiTietRepository.Table
                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauLinhDuocPham.KhoXuatId
                            && yeuCauLinhDuocPham.YeuCauLinhDuocPhamChiTiets.Any(y => y.DuocPhamBenhVienId == x.DuocPhamBenhVienId && y.LaDuocPhamBHYT == x.LaDuocPhamBHYT && (x.SoLuongDaXuat < x.SoLuongNhap)))
                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                .ToListAsync();
            foreach (var yeuCauLinhChiTiet in yeuCauLinhDuocPham.YeuCauLinhDuocPhamChiTiets)
            {
                var lstNhapKhoChiTiet = lstNhapKhoTatCaDuocPhamYeuCauLinh.Where(x =>
                    x.DuocPhamBenhVienId == yeuCauLinhChiTiet.DuocPhamBenhVienId &&
                    x.LaDuocPhamBHYT == yeuCauLinhChiTiet.LaDuocPhamBHYT &&
                    x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                var slYeuCau = duocPhamCanXuatVos.FirstOrDefault(o => o.LaDuocPhamBHYT == yeuCauLinhChiTiet.LaDuocPhamBHYT && o.DuocPhamBenhVienId == yeuCauLinhChiTiet.DuocPhamBenhVienId)?.SoLuongXuat ?? 0;
                if (slYeuCau.AlmostEqual(0)) continue;

                if (!lstNhapKhoChiTiet.Any() || lstNhapKhoChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < slYeuCau)
                {
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.SoLuongTon.KhongDu"));
                }

                var newXuatKhoChiTiet = new XuatKhoDuocPhamChiTiet()
                {
                    DuocPhamBenhVienId = yeuCauLinhChiTiet.DuocPhamBenhVienId,
                    NgayXuat = thoiDiemHienTai
                };


                var isExists = false;
                foreach (var nhapChiTiet in lstNhapKhoChiTiet)
                {
                    if (slYeuCau > 0)
                    {
                        var newNhapKhoChiTiet = new NhapKhoDuocPhamChiTiet()
                        {
                            SoLuongNhap = 0,
                            SoLuongDaXuat = 0
                        };
                        var nhapKhoChiTiettemp = newNhapKho.NhapKhoDuocPhamChiTiets.FirstOrDefault(x => x.DuocPhamBenhVienId == nhapChiTiet.DuocPhamBenhVienId
                                                                                                        && x.LaDuocPhamBHYT == nhapChiTiet.LaDuocPhamBHYT
                                                                                                        && x.NgayNhapVaoBenhVien == nhapChiTiet.NgayNhapVaoBenhVien
                                                                                                        && x.DonGiaNhap == nhapChiTiet.DonGiaNhap
                                                                                                        && x.VAT == nhapChiTiet.VAT
                                                                                                        && x.TiLeTheoThapGia == nhapChiTiet.TiLeTheoThapGia
                                                                                                        && x.MaVach == nhapChiTiet.MaVach
                                                                                                        && x.MaRef == nhapChiTiet.MaRef
                                                                                                        && x.HanSuDung == nhapChiTiet.HanSuDung
                                                                                                        && x.Solo == nhapChiTiet.Solo
                                                                                                        && x.HopDongThauDuocPhamId == nhapChiTiet.HopDongThauDuocPhamId
                                                                                                        && x.NgayNhap == nhapChiTiet.NgayNhap
                                                                                                        && x.DuocPhamBenhVienPhanNhomId == nhapChiTiet.DuocPhamBenhVienPhanNhomId
                                                                                                        && x.TiLeBHYTThanhToan == nhapChiTiet.TiLeBHYTThanhToan);
                        if (nhapKhoChiTiettemp != null)
                        {
                            newNhapKhoChiTiet = nhapKhoChiTiettemp;
                            isExists = true;
                        }
                        else
                        {
                            isExists = false;
                            newNhapKhoChiTiet.DuocPhamBenhVienId = nhapChiTiet.DuocPhamBenhVienId;
                            newNhapKhoChiTiet.HopDongThauDuocPhamId = nhapChiTiet.HopDongThauDuocPhamId;
                            newNhapKhoChiTiet.Solo = nhapChiTiet.Solo;
                            newNhapKhoChiTiet.HanSuDung = nhapChiTiet.HanSuDung;
                            newNhapKhoChiTiet.MaVach = nhapChiTiet.MaVach;
                            newNhapKhoChiTiet.MaRef = nhapChiTiet.MaRef;
                            newNhapKhoChiTiet.LaDuocPhamBHYT = nhapChiTiet.LaDuocPhamBHYT;
                            newNhapKhoChiTiet.DuocPhamBenhVienPhanNhomId = nhapChiTiet.DuocPhamBenhVienPhanNhomId;
                            newNhapKhoChiTiet.NgayNhapVaoBenhVien = nhapChiTiet.NgayNhapVaoBenhVien;
                            newNhapKhoChiTiet.PhuongPhapTinhGiaTriTonKho = nhapChiTiet.PhuongPhapTinhGiaTriTonKho;
                            newNhapKhoChiTiet.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                            newNhapKhoChiTiet.VAT = nhapChiTiet.VAT;
                            newNhapKhoChiTiet.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                            newNhapKhoChiTiet.NgayNhap = thoiDiemHienTai;
                            newNhapKhoChiTiet.TiLeBHYTThanhToan = nhapChiTiet.TiLeBHYTThanhToan;
                            newNhapKhoChiTiet.DanhSachMayXetNghiemId = yeuCauLinhChiTiet.DanhSachMayXetNghiemId;
                        }

                        var newXuatKhoChiTietViTri = new XuatKhoDuocPhamChiTietViTri()
                        {
                            NhapKhoDuocPhamChiTietId = nhapChiTiet.Id,
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
                        newXuatKhoChiTiet.XuatKhoDuocPhamChiTietViTris.Add(newXuatKhoChiTietViTri);
                        if (!isExists)
                        {
                            newNhapKho.NhapKhoDuocPhamChiTiets.Add(newNhapKhoChiTiet);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                newPhieuXuat.XuatKhoDuocPhamChiTiets.Add(newXuatKhoChiTiet);
            }
            #endregion

            newPhieuXuat.NhapKhoDuocPhams.Add(newNhapKho);
            yeuCauLinhDuocPham.XuatKhoDuocPhams.Add(newPhieuXuat);


            // lưu thông tin yêu cầu lĩnh
            await _yeuCauLinhDuocPhamRepository.UpdateAsync(yeuCauLinhDuocPham);
            // cập nhật thông tin tông
            //await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(lstNhapKhoTatCaDuocPhamYeuCauLinh);
        }

        public async Task<string> InPhieuDuyetLinhThuongDuocPhamAsync(InPhieuDuyetLinhDuocPham inPhieu)
        {
            var content = string.Empty;
            var contentGNHT = string.Empty;
            var hearder = string.Empty;

            string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
            int? nhomVatTu = 0;
            if (string.IsNullOrEmpty(nhomVatTuString))
            {
                nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
            }

            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocThuong"));

            var templateLinhThuongDuocPhamGNHT = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetThuongGayNghienHuongThan"));

            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == inPhieu.YeuCauLinhDuocPhamId)
                .Select(item => new ThongTinInPhieuLinhDuocPhamVo()
                {
                    TenNguoiNhanHang = item.XuatKhoDuocPhams.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault(),
                    BoPhan = item.NoiYeuCauId != null ? item.NoiYeuCau.KhoaPhongId != null ? item.NoiYeuCau.KhoaPhong.Ma + "-"+ item.NoiYeuCau.KhoaPhong.Ten 
                             :""
                             :"",
                    LyDoXuatKho = item.XuatKhoDuocPhams.Select(x => x.LyDoXuatKho).FirstOrDefault(),
                    XuatTaiKho = item.KhoXuat.Ten,
                    DiaDiem = "",

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            //if (inPhieu.HasHeader)
            //{
            //    hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                  "<th>PHIẾU XUẤT</th>" +
            //             "</p>";
            //}
            var query = await _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(p => p.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId &&
                                                                                              (p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien &&
                                                                                               p.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan))
                    .Select(s => new ThongTinInPhieuLinhDuocPhamChiTietVo
                    {
                        Ma = s.DuocPhamBenhVien.Ma,
                        TenThuoc = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && s.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                       (s.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && s.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.HamLuong != null && s.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                        //DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null,
                        DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                        SLYeuCau = s.SoLuong,
                        SLThucXuat = s.SoLuongCoTheXuat.GetValueOrDefault(),
                        LaDuocPhamBHYT = s.LaDuocPhamBHYT
                    }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
                    .ToListAsync();
            var queryGNHT = await _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking.Where(p => p.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId &&
                                                                                           (p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien ||
                                                                                            p.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                 .Select(s => new ThongTinInPhieuLinhDuocPhamChiTietVo
                 {
                     Ma = s.DuocPhamBenhVien.Ma,
                     TenThuoc = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                  s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && s.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                    (s.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && s.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                  s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.HamLuong != null && s.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                     //DVT = s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null,
                     DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                     SLYeuCau = s.SoLuong,
                     SLThucXuat = s.SoLuongCoTheXuat.GetValueOrDefault(),
                     LaDuocPhamBHYT = s.LaDuocPhamBHYT
                 }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
                 .ToListAsync();
            #region thuốc bình thường
            if (query.Any())
            {
                var infoLinhDuocChiTiet = string.Empty;
                var groupThuocBHYT = "Thuốc BHYT";
                var groupThuocKhongBHYT = "Thuốc Không BHYT";

                var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                            + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                            + "</b></tr>";
                var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                            + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                            + "</b></tr>";
                var STT = 1;
                if (query.Any(p => p.LaDuocPhamBHYT))
                {
                    infoLinhDuocChiTiet += headerBHYT;
                    var queryBHYT = query.Where(x => x.LaDuocPhamBHYT).OrderBy(z => z.TenThuoc).ToList();
                    foreach (var item in queryBHYT)
                    {
                        if (item.LaDuocPhamBHYT)
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
                if (query.Any(p => p.LaDuocPhamBHYT == false))
                {
                    infoLinhDuocChiTiet += headerKhongBHYT;
                    var queryKhongBHYT = query.Where(x => !x.LaDuocPhamBHYT).OrderBy(z => z.TenThuoc).ToList();
                    foreach (var item in queryKhongBHYT)
                    {
                        if (item.LaDuocPhamBHYT == false)
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

                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
            }
            #endregion
            #region thuốc gây nghiện hướng thần
            if (queryGNHT.Any())
            {
                var infoLinhDuocChiTiet = string.Empty;
                var groupThuocBHYT = "Thuốc BHYT";
                var groupThuocKhongBHYT = "Thuốc Không BHYT";

                var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                            + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocBHYT.ToUpper()
                                            + "</b></tr>";
                var headerKhongBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                            + "<td style='border: 1px solid #020000;text-align: left;' colspan='7'><b>" + groupThuocKhongBHYT.ToUpper()
                                            + "</b></tr>";
                var STT = 1;
                if (queryGNHT.Any(p => p.LaDuocPhamBHYT))
                {
                    infoLinhDuocChiTiet += headerBHYT;
                    var queryBHYT = queryGNHT.Where(x => x.LaDuocPhamBHYT).OrderBy(z => z.TenThuoc).ToList();
                    foreach (var item in queryBHYT)
                    {
                        if (item.LaDuocPhamBHYT)
                        {
                            infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                                + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                                + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                                + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                                + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLYeuCau), false)
                                                + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLThucXuat), false)
                                                + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"
                                                + "</tr>";
                            STT++;
                            groupThuocBHYT = string.Empty;
                        }

                    }
                }
                if (queryGNHT.Any(p => p.LaDuocPhamBHYT == false))
                {
                    infoLinhDuocChiTiet += headerKhongBHYT;
                    var queryKhongBHYT = queryGNHT.Where(x => !x.LaDuocPhamBHYT).OrderBy(z => z.TenThuoc).ToList();
                    foreach (var item in queryKhongBHYT)
                    {
                        if (item.LaDuocPhamBHYT == false)
                        {
                            infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                               + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                               + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                               + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                               + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                               + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLYeuCau), false)
                                               + "<td style = 'border: 1px solid #020000;text-align: right;'>" + NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLThucXuat), false)
                                               + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"
                                               + "</tr>";
                            STT++;
                            groupThuocKhongBHYT = string.Empty;
                        }
                    }
                }

                data.HeaderPhieuLinhThuoc = hearder;
                data.DanhSachThuoc = infoLinhDuocChiTiet;

                contentGNHT = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPhamGNHT.Body, data);
            }
            #endregion
            var headerTitile = "<div class=\'wrap\'><div class=\'content\'>PHIẾU XUẤT THUỐC</div></div>";

            if (!string.IsNullOrEmpty(content))
            {
                content = headerTitile + content;
            }
            var congPage = string.Empty;
            congPage = !string.IsNullOrEmpty(content) ? "<div style='break-after:page'></div>" : "";
            if (!string.IsNullOrEmpty(contentGNHT))
            {
                content = content + headerTitile + congPage + contentGNHT;
            }
            return content;
        }
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh bù

        public async Task XuLyDuyetYeuCauLinhDuocPhamBuAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhBuDuocPham, long nguoiXuatId, long nguoiNhapId)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            // cập nhật thông tin duyệt
            var thoiDiemHienTai = DateTime.Now;

            // xử lý nhập xuất dược phẩm
            #region Xử lý xuất
            var newNhapKho = new NhapKhoDuocPham()
            {
                NguoiNhapId = nguoiNhapId,
                LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayNhap = thoiDiemHienTai,
                KhoId = yeuCauLinhBuDuocPham.KhoNhapId,
                YeuCauLinhDuocPhamId = yeuCauLinhBuDuocPham.Id
            };

            var newPhieuXuat = new XuatKhoDuocPham()
            {
                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatQuaKhoKhac.GetDescription(),
                NguoiNhanId = nguoiNhapId,
                NguoiXuatId = nguoiXuatId,
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayXuat = thoiDiemHienTai,
                KhoXuatId = yeuCauLinhBuDuocPham.KhoXuatId,
                KhoNhapId = yeuCauLinhBuDuocPham.KhoNhapId
            };

            var duocPhamBenhVienIds = yeuCauLinhBuDuocPham.YeuCauLinhDuocPhamChiTiets.Select(o => o.DuocPhamBenhVienId).Distinct().ToList();

            var lstNhapKhoTatCaDuocPhamYeuCauLinh = _nhapKhoDuocPhamChiTietRepository.Table
                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauLinhBuDuocPham.KhoXuatId
                            && duocPhamBenhVienIds.Contains(x.DuocPhamBenhVienId)
                            && (x.SoLuongDaXuat < x.SoLuongNhap))
                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                .ToList();

            foreach (var yeuCauLinhDuocPhamChiTiet in yeuCauLinhBuDuocPham.YeuCauLinhDuocPhamChiTiets)//.YeuCauDuocPhamBenhViens.Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
            {
                var lstNhapKhoChiTiet = lstNhapKhoTatCaDuocPhamYeuCauLinh.Where(x =>
                    x.DuocPhamBenhVienId == yeuCauLinhDuocPhamChiTiet.DuocPhamBenhVienId &&
                    x.LaDuocPhamBHYT == yeuCauLinhDuocPhamChiTiet.YeuCauDuocPhamBenhVien.LaDuocPhamBHYT &&
                    x.SoLuongDaXuat < x.SoLuongNhap).ToList();


                var slYeuCau = yeuCauLinhDuocPhamChiTiet.SoLuong;
                if (!lstNhapKhoChiTiet.Any() || Math.Round(lstNhapKhoChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat), 2) < slYeuCau)
                {
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamBu.SoLuongTon.KhongDu"));
                }

                // update 21/05/2021: cập nhật lĩnh bù -> rào giá trị max đã bù chỉ cho bằng số lượng
                var soLuongDaLinhBuNew = Math.Round((yeuCauLinhDuocPhamChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu ?? 0) + slYeuCau, 2);
                yeuCauLinhDuocPhamChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaLinhBu = soLuongDaLinhBuNew > yeuCauLinhDuocPhamChiTiet.YeuCauDuocPhamBenhVien.SoLuong ? yeuCauLinhDuocPhamChiTiet.YeuCauDuocPhamBenhVien.SoLuong : soLuongDaLinhBuNew;

                var newXuatKhoChiTiet = new XuatKhoDuocPhamChiTiet()
                {
                    DuocPhamBenhVienId = yeuCauLinhDuocPhamChiTiet.DuocPhamBenhVienId,
                    NgayXuat = thoiDiemHienTai
                };

                var isExists = false;
                foreach (var nhapChiTiet in lstNhapKhoChiTiet)
                {
                    if (slYeuCau > 0)
                    {
                        var newNhapKhoChiTiet = new NhapKhoDuocPhamChiTiet()
                        {
                            SoLuongNhap = 0,
                            SoLuongDaXuat = 0
                        };
                        var nhapKhoChiTiettemp = newNhapKho.NhapKhoDuocPhamChiTiets.FirstOrDefault(x => x.DuocPhamBenhVienId == nhapChiTiet.DuocPhamBenhVienId
                                                                                                        && x.LaDuocPhamBHYT == nhapChiTiet.LaDuocPhamBHYT
                                                                                                        && x.NgayNhapVaoBenhVien == nhapChiTiet.NgayNhapVaoBenhVien
                                                                                                        && x.DonGiaNhap == nhapChiTiet.DonGiaNhap
                                                                                                        && x.VAT == nhapChiTiet.VAT
                                                                                                        && x.TiLeTheoThapGia == nhapChiTiet.TiLeTheoThapGia
                                                                                                        && x.MaVach == nhapChiTiet.MaVach
                                                                                                        && x.MaRef == nhapChiTiet.MaRef
                                                                                                        && x.HanSuDung == nhapChiTiet.HanSuDung
                                                                                                        && x.Solo == nhapChiTiet.Solo
                                                                                                        && x.HopDongThauDuocPhamId == nhapChiTiet.HopDongThauDuocPhamId
                                                                                                        && x.NgayNhap == nhapChiTiet.NgayNhap
                                                                                                        && x.DuocPhamBenhVienPhanNhomId == nhapChiTiet.DuocPhamBenhVienPhanNhomId
                                                                                                        && x.TiLeBHYTThanhToan == nhapChiTiet.TiLeBHYTThanhToan);
                        if (nhapKhoChiTiettemp != null)
                        {
                            newNhapKhoChiTiet = nhapKhoChiTiettemp;
                            isExists = true;
                        }
                        else
                        {
                            isExists = false;
                            newNhapKhoChiTiet.DuocPhamBenhVienId = nhapChiTiet.DuocPhamBenhVienId;
                            newNhapKhoChiTiet.HopDongThauDuocPhamId = nhapChiTiet.HopDongThauDuocPhamId;
                            newNhapKhoChiTiet.Solo = nhapChiTiet.Solo;
                            newNhapKhoChiTiet.HanSuDung = nhapChiTiet.HanSuDung;
                            newNhapKhoChiTiet.MaVach = nhapChiTiet.MaVach;
                            newNhapKhoChiTiet.MaRef = nhapChiTiet.MaRef;
                            newNhapKhoChiTiet.LaDuocPhamBHYT = nhapChiTiet.LaDuocPhamBHYT;
                            newNhapKhoChiTiet.DuocPhamBenhVienPhanNhomId = nhapChiTiet.DuocPhamBenhVienPhanNhomId;
                            newNhapKhoChiTiet.NgayNhapVaoBenhVien = nhapChiTiet.NgayNhapVaoBenhVien;
                            newNhapKhoChiTiet.PhuongPhapTinhGiaTriTonKho = nhapChiTiet.PhuongPhapTinhGiaTriTonKho;
                            newNhapKhoChiTiet.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                            newNhapKhoChiTiet.VAT = nhapChiTiet.VAT;
                            newNhapKhoChiTiet.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                            newNhapKhoChiTiet.NgayNhap = thoiDiemHienTai;
                            newNhapKhoChiTiet.TiLeBHYTThanhToan = nhapChiTiet.TiLeBHYTThanhToan;
                        }

                        var newXuatKhoChiTietViTri = new XuatKhoDuocPhamChiTietViTri()
                        {
                            NhapKhoDuocPhamChiTietId = nhapChiTiet.Id,
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
                        newXuatKhoChiTiet.XuatKhoDuocPhamChiTietViTris.Add(newXuatKhoChiTietViTri);
                        if (!isExists)
                        {
                            newNhapKho.NhapKhoDuocPhamChiTiets.Add(newNhapKhoChiTiet);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                newPhieuXuat.XuatKhoDuocPhamChiTiets.Add(newXuatKhoChiTiet);
            }
            #endregion

            newPhieuXuat.NhapKhoDuocPhams.Add(newNhapKho);
            yeuCauLinhBuDuocPham.XuatKhoDuocPhams.Add(newPhieuXuat);


            // lưu thông tin yêu cầu lĩnh
            await _yeuCauLinhDuocPhamRepository.UpdateAsync(yeuCauLinhBuDuocPham);
            // cập nhật thông tin tông
            //await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(lstNhapKhoTatCaDuocPhamYeuCauLinh);
        }

        public async Task<string> InPhieuDuyetLinhBuDuocPhamAsync(InPhieuDuyetLinhDuocPham inPhieu)
        {
            var content = string.Empty;
            var contentGNHT = string.Empty;
            var hearder = string.Empty;

            int? nhomVatTu = 0;
            string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(nhomVatTuString))
            {
                nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
            }

            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocBu"));

            var templateLinhThuongDuocPhamGNHT = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocBuGayNghienHuongThan"));

            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == inPhieu.YeuCauLinhDuocPhamId)
                .Select(item => new ThongTinInPhieuLinhDuocPhamVo()
                {
                    TenNguoiNhanHang = item.XuatKhoDuocPhams.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault(),
                    BoPhan = item.NoiYeuCauId != null ? item.NoiYeuCau.KhoaPhongId != null ? item.NoiYeuCau.KhoaPhong.Ma + "-" + item.NoiYeuCau.KhoaPhong.Ten
                             : ""
                             : "",
                    LyDoXuatKho = item.XuatKhoDuocPhams.Select(x => x.LyDoXuatKho).FirstOrDefault(),
                    XuatTaiKho = item.KhoXuat.Ten,
                    DiaDiem = "",

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();


            var query = await _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                .Where(x => x.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId &&
                            x.YeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                            && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien && x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan))
                .Select(s => new ThongTinInPhieuLinhDuocPhamChiTietVo
                {
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    Ma = s.DuocPhamBenhVien.Ma,
                    TenThuoc = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && s.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                       (s.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && s.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.HamLuong != null && s.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                    DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                    SLYeuCau = s.SoLuong,
                    SLThucXuat = s.SoLuong,
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT
                })
                .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT })
                .Select(item => new ThongTinInPhieuLinhDuocPhamChiTietVo()
                {
                    Ma = item.First().Ma,
                    TenThuoc = item.First().TenThuoc,
                    DVT = item.First().DVT,
                    SLYeuCau = item.Sum(x => x.SLYeuCau),
                    SLThucXuat = item.Sum(x => x.SLThucXuat),
                    LaDuocPhamBHYT = item.First().LaDuocPhamBHYT
                }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
                .ToListAsync();

            var queryGNHT = await _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
              .Where(x => x.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId &&
                          x.YeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                          && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
              .Select(s => new ThongTinInPhieuLinhDuocPhamChiTietVo
              {
                  DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                  Ma = s.DuocPhamBenhVien.Ma,
                  TenThuoc = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                   s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && s.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                     (s.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && s.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                   s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.HamLuong != null && s.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                  DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                  SLYeuCau = s.SoLuong,
                  SLThucXuat = s.SoLuong,
                  LaDuocPhamBHYT = s.LaDuocPhamBHYT
              })
              .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT })
              .Select(item => new ThongTinInPhieuLinhDuocPhamChiTietVo()
              {
                  Ma = item.First().Ma,
                  TenThuoc = item.First().TenThuoc,
                  DVT = item.First().DVT,
                  SLYeuCau = item.Sum(x => x.SLYeuCau),
                  SLThucXuat = item.Sum(x => x.SLThucXuat),
                  LaDuocPhamBHYT = item.First().LaDuocPhamBHYT
              }).OrderByDescending(p => p.LaDuocPhamBHYT).ThenBy(p => !p.LaDuocPhamBHYT)
              .ToListAsync();
            #region bình thường
            if (query.Any())
            {
                var objData = GetHTMLLinhBuDaDuyet(query,false);
                data.HeaderPhieuLinhThuoc = hearder;
                data.DanhSachThuoc = objData.html;

                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
            }
            #endregion
            #region gây nghiện hướng thần
            if (queryGNHT.Any())
            {
                var objData = GetHTMLLinhBuDaDuyet(queryGNHT,true);
                data.HeaderPhieuLinhThuoc = hearder;
                data.DanhSachThuoc = objData.html;
                contentGNHT = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPhamGNHT.Body, data);
            }
            #endregion

            var headerTitile = "<div class=\'wrap\'><div class=\'content\'>PHIẾU XUẤT THUỐC</div></div>";

            if (!string.IsNullOrEmpty(content))
            {
                content = headerTitile + content;
            }
            var congPage = string.Empty;
            congPage = !string.IsNullOrEmpty(content) ? "<div style='break-after:page'></div>" : "";
            if (!string.IsNullOrEmpty(contentGNHT))
            {
                content = content + headerTitile + congPage + contentGNHT;
            }

            return content;
        }
        #endregion

        #region Xử lý thêm/xóa/sửa lĩnh trực tiếp

        public async Task XuLyDuyetYeuCauLinhDuocPhamTrucTiepAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhTrucTiepDuocPham, long nguoiXuatId, long nguoiNhapId)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            // cập nhật thông tin duyệt
            var thoiDiemHienTai = DateTime.Now;

            // xử lý nhập xuất dược phẩm
            #region Xử lý xuất
            //var newNhapKho = new NhapKhoDuocPham()
            //{
            //    NguoiNhapId = nguoiNhapId,
            //    LoaiNguoiGiao = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
            //    NgayNhap = thoiDiemHienTai,
            //    KhoId = yeuCauLinhTrucTiepDuocPham.KhoNhapId,
            //    YeuCauLinhDuocPhamId = yeuCauLinhTrucTiepDuocPham.Id
            //};

            var newPhieuXuat = new XuatKhoDuocPham()
            {
                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan, //Enums.XuatKhoDuocPham.XuatQuaKhoKhac,
                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(), //Enums.XuatKhoDuocPham.XuatQuaKhoKhac.GetDescription(),
                NguoiNhanId = nguoiNhapId,
                NguoiXuatId = nguoiXuatId,
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
                NgayXuat = thoiDiemHienTai,
                KhoXuatId = yeuCauLinhTrucTiepDuocPham.KhoXuatId,
                //KhoNhapId = yeuCauLinhTrucTiepDuocPham.KhoNhapId
            };

            var lstNhapKhoTatCaDuocPhamYeuCauLinh = await _nhapKhoDuocPhamChiTietRepository.Table
                .Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauLinhTrucTiepDuocPham.KhoXuatId
                            && yeuCauLinhTrucTiepDuocPham.YeuCauDuocPhamBenhViens.Any(y => y.DuocPhamBenhVienId == x.DuocPhamBenhVienId
                                                                                           && y.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                           && (x.SoLuongDaXuat < x.SoLuongNhap))
                            //BVHD-3821
                            // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                            && x.HanSuDung.Date >= DateTime.Now.Date)
                .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                .ToListAsync();
            if (!lstNhapKhoTatCaDuocPhamYeuCauLinh.Any())
            {
                throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamBu.SoLuongTon.KhongDu"));
            }


            var lstYeuCauDuocPhamThemMoi = new List<YeuCauDuocPhamBenhVien>();
            foreach (var yeuCauDuocPhamChiTiet in yeuCauLinhTrucTiepDuocPham.YeuCauDuocPhamBenhViens.Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
            {
                var lstNhapKhoChiTiet = lstNhapKhoTatCaDuocPhamYeuCauLinh.Where(x =>
                    x.DuocPhamBenhVienId == yeuCauDuocPhamChiTiet.DuocPhamBenhVienId &&
                    x.LaDuocPhamBHYT == yeuCauDuocPhamChiTiet.LaDuocPhamBHYT &&
                    x.SoLuongDaXuat < x.SoLuongNhap).ToList();

                var duocPhamBenhVien = _duocPhamBenhVienRepository.GetById(yeuCauDuocPhamChiTiet.DuocPhamBenhVienId,
                    o => o.Include(y => y.DuocPham).ThenInclude(y => y.HopDongThauDuocPhamChiTiets));

                if (!lstNhapKhoChiTiet.Any() || lstNhapKhoChiTiet.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) < yeuCauDuocPhamChiTiet.SoLuong)
                {
                    throw new Exception(_localizationService.GetResource("DuyetYeuCauLinhDuocPhamBu.SoLuongTon.KhongDu"));
                }

                var newXuatKhoChiTiet = new XuatKhoDuocPhamChiTiet()
                {
                    DuocPhamBenhVienId = yeuCauDuocPhamChiTiet.DuocPhamBenhVienId,
                    NgayXuat = thoiDiemHienTai
                };

                if (yeuCauDuocPhamChiTiet.NoiTruChiDinhDuocPhamId != null)
                {

                }
                // tạo biến tạm yêu cầu dược phẩm bệnh viện
                yeuCauDuocPhamChiTiet.TrangThai = Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien;
                var yeuCauDuocPhamTemp = new YeuCauDuocPhamBenhVien()
                {
                    YeuCauTiepNhanId = yeuCauDuocPhamChiTiet.YeuCauTiepNhanId,
                    KhoLinhId = yeuCauDuocPhamChiTiet.KhoLinhId,

                    YeuCauKhamBenhId = yeuCauDuocPhamChiTiet.YeuCauKhamBenhId,
                    YeuCauDichVuKyThuatId = yeuCauDuocPhamChiTiet.YeuCauDichVuKyThuatId,

                    YeuCauLinhDuocPhamId = yeuCauDuocPhamChiTiet.YeuCauLinhDuocPhamId,

                    LoaiPhieuLinh = yeuCauDuocPhamChiTiet.LoaiPhieuLinh,

                    DuocPhamBenhVienId = yeuCauDuocPhamChiTiet.DuocPhamBenhVienId,
                    Ten = yeuCauDuocPhamChiTiet.Ten,
                    TenTiengAnh = yeuCauDuocPhamChiTiet.TenTiengAnh,
                    SoDangKy = yeuCauDuocPhamChiTiet.SoDangKy,
                    STTHoatChat = yeuCauDuocPhamChiTiet.STTHoatChat,
                    MaHoatChat = yeuCauDuocPhamChiTiet.MaHoatChat,
                    HoatChat = yeuCauDuocPhamChiTiet.HoatChat,
                    LoaiThuocHoacHoatChat = yeuCauDuocPhamChiTiet.LoaiThuocHoacHoatChat,
                    NhaSanXuat = yeuCauDuocPhamChiTiet.NhaSanXuat,
                    NuocSanXuat = yeuCauDuocPhamChiTiet.NuocSanXuat,
                    DuongDungId = yeuCauDuocPhamChiTiet.DuongDungId,
                    HamLuong = yeuCauDuocPhamChiTiet.HamLuong,
                    QuyCach = yeuCauDuocPhamChiTiet.QuyCach,
                    TieuChuan = yeuCauDuocPhamChiTiet.TieuChuan,
                    DangBaoChe = yeuCauDuocPhamChiTiet.DangBaoChe,
                    DonViTinhId = yeuCauDuocPhamChiTiet.DonViTinhId,
                    HuongDan = yeuCauDuocPhamChiTiet.HuongDan,
                    MoTa = yeuCauDuocPhamChiTiet.MoTa,
                    ChiDinh = yeuCauDuocPhamChiTiet.ChiDinh,
                    ChongChiDinh = yeuCauDuocPhamChiTiet.ChongChiDinh,
                    LieuLuongCachDung = yeuCauDuocPhamChiTiet.LieuLuongCachDung,
                    TacDungPhu = yeuCauDuocPhamChiTiet.TacDungPhu,
                    ChuYdePhong = yeuCauDuocPhamChiTiet.ChuYdePhong,

                    KhongTinhPhi = yeuCauDuocPhamChiTiet.KhongTinhPhi,
                    LaDuocPhamBHYT = yeuCauDuocPhamChiTiet.LaDuocPhamBHYT,

                    NhanVienChiDinhId = yeuCauDuocPhamChiTiet.NhanVienChiDinhId,
                    NoiChiDinhId = yeuCauDuocPhamChiTiet.NoiChiDinhId,
                    ThoiDiemChiDinh = yeuCauDuocPhamChiTiet.ThoiDiemChiDinh,

                    DaCapThuoc = yeuCauDuocPhamChiTiet.DaCapThuoc,
                    TrangThai = yeuCauDuocPhamChiTiet.TrangThai,
                    TrangThaiThanhToan = yeuCauDuocPhamChiTiet.TrangThaiThanhToan,

                    DuocHuongBaoHiem = yeuCauDuocPhamChiTiet.DuocHuongBaoHiem,

                    NoiTruChiDinhDuocPhamId = yeuCauDuocPhamChiTiet.NoiTruChiDinhDuocPhamId,
                    TheTich = yeuCauDuocPhamChiTiet.TheTich,
                    LaDichTruyen = yeuCauDuocPhamChiTiet.LaDichTruyen,
                    NoiTruPhieuDieuTriId = yeuCauDuocPhamChiTiet.NoiTruPhieuDieuTriId
                };

                var yeuCauNew = yeuCauDuocPhamChiTiet;

                var slYeuCau = yeuCauDuocPhamChiTiet.SoLuong;
                //var isExists = false;
                var isFirstYeuCauDuocPham = true;
                foreach (var nhapChiTiet in lstNhapKhoChiTiet)
                {
                    if (slYeuCau > 0)
                    {
                        var giaTheoHopDong = duocPhamBenhVien.DuocPham.HopDongThauDuocPhamChiTiets.First(o => o.HopDongThauDuocPhamId == nhapChiTiet.HopDongThauDuocPhamId).Gia;
                        var donGiaBaoHiem = nhapChiTiet.DonGiaNhap >= giaTheoHopDong ? giaTheoHopDong : nhapChiTiet.DonGiaNhap;
                        var tiLeBHTT = nhapChiTiet.LaDuocPhamBHYT ? nhapChiTiet.TiLeBHYTThanhToan ?? 100 : 0;
                        if (yeuCauNew.DonGiaNhap != nhapChiTiet.DonGiaNhap || yeuCauNew.VAT != nhapChiTiet.VAT || yeuCauNew.TiLeTheoThapGia != nhapChiTiet.TiLeTheoThapGia || yeuCauNew.TiLeBaoHiemThanhToan != tiLeBHTT)
                        {
                            // trường hợp cập nhật lại thông tin cho yêu cầu duyệt cũ
                            if (isFirstYeuCauDuocPham && yeuCauNew.Id != 0)
                            {
                                yeuCauNew.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                                yeuCauNew.VAT = nhapChiTiet.VAT;
                                yeuCauNew.PhuongPhapTinhGiaTriTonKho = nhapChiTiet.PhuongPhapTinhGiaTriTonKho;
                                yeuCauNew.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                                yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                yeuCauNew.TiLeBaoHiemThanhToan = tiLeBHTT; //nhapChiTiet.TiLeBHYTThanhToan ?? 100;

                                //isFirstYeuCauDuocPham = false;
                            }
                            // trường hợp tạo mới yêu cầu duyệt
                            else
                            {
                                if (newXuatKhoChiTiet.XuatKhoDuocPhamChiTietViTris.All(x => x.SoLuongXuat > 0))
                                {
                                    newPhieuXuat.XuatKhoDuocPhamChiTiets.Add(newXuatKhoChiTiet);
                                    yeuCauNew.XuatKhoDuocPhamChiTiet = newXuatKhoChiTiet;
                                    yeuCauNew.SoLuong = yeuCauNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat);

                                    if (yeuCauNew.Id == 0)
                                    {
                                        lstYeuCauDuocPhamThemMoi.Add(yeuCauNew);
                                    }

                                    yeuCauNew = yeuCauDuocPhamTemp.Clone();
                                    yeuCauNew.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                                    yeuCauNew.VAT = nhapChiTiet.VAT;
                                    yeuCauNew.PhuongPhapTinhGiaTriTonKho = nhapChiTiet.PhuongPhapTinhGiaTriTonKho;
                                    yeuCauNew.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                                    yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                                    yeuCauNew.TiLeBaoHiemThanhToan = tiLeBHTT; //nhapChiTiet.TiLeBHYTThanhToan ?? 100;

                                    newXuatKhoChiTiet = new XuatKhoDuocPhamChiTiet()
                                    {
                                        DuocPhamBenhVienId = yeuCauNew.DuocPhamBenhVienId,
                                        NgayXuat = thoiDiemHienTai
                                    };
                                }

                            }
                        }
                        else
                        {
                            yeuCauNew.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                            yeuCauNew.VAT = nhapChiTiet.VAT;
                            yeuCauNew.PhuongPhapTinhGiaTriTonKho = nhapChiTiet.PhuongPhapTinhGiaTriTonKho;
                            yeuCauNew.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                            yeuCauNew.DonGiaBaoHiem = donGiaBaoHiem;
                            yeuCauNew.TiLeBaoHiemThanhToan = tiLeBHTT; //nhapChiTiet.TiLeBHYTThanhToan ?? 100;
                        }


                        //var newNhapKhoChiTiet = new NhapKhoDuocPhamChiTiet()
                        //{
                        //    SoLuongNhap = 0,
                        //    SoLuongDaXuat = 0
                        //};
                        //var nhapKhoChiTiettemp = newNhapKho.NhapKhoDuocPhamChiTiets.FirstOrDefault(x => x.DuocPhamBenhVienId == nhapChiTiet.DuocPhamBenhVienId
                        //                                                                                && x.LaDuocPhamBHYT == nhapChiTiet.LaDuocPhamBHYT
                        //                                                                                && x.NgayNhapVaoBenhVien == nhapChiTiet.NgayNhapVaoBenhVien
                        //                                                                                && x.DonGiaNhap == nhapChiTiet.DonGiaNhap
                        //                                                                                && x.VAT == nhapChiTiet.VAT
                        //                                                                                && x.TiLeTheoThapGia == nhapChiTiet.TiLeTheoThapGia
                        //                                                                                && x.MaVach == nhapChiTiet.MaVach
                        //                                                                                && x.HanSuDung == nhapChiTiet.HanSuDung
                        //                                                                                && x.Solo == nhapChiTiet.Solo
                        //                                                                                && x.HopDongThauDuocPhamId == nhapChiTiet.HopDongThauDuocPhamId
                        //                                                                                && x.NgayNhap == nhapChiTiet.NgayNhap
                        //                                                                                && x.DuocPhamBenhVienPhanNhomId == nhapChiTiet.DuocPhamBenhVienPhanNhomId
                        //                                                                                && x.TiLeBHYTThanhToan == nhapChiTiet.TiLeBHYTThanhToan);
                        //if (nhapKhoChiTiettemp != null)
                        //{
                        //    newNhapKhoChiTiet = nhapKhoChiTiettemp;
                        //    isExists = true;
                        //}
                        //else
                        //{
                        //    isExists = false;
                        //    newNhapKhoChiTiet.DuocPhamBenhVienId = nhapChiTiet.DuocPhamBenhVienId;
                        //    newNhapKhoChiTiet.HopDongThauDuocPhamId = nhapChiTiet.HopDongThauDuocPhamId;
                        //    newNhapKhoChiTiet.Solo = nhapChiTiet.Solo;
                        //    newNhapKhoChiTiet.HanSuDung = nhapChiTiet.HanSuDung;
                        //    newNhapKhoChiTiet.MaVach = nhapChiTiet.MaVach;
                        //    newNhapKhoChiTiet.LaDuocPhamBHYT = nhapChiTiet.LaDuocPhamBHYT;
                        //    newNhapKhoChiTiet.DuocPhamBenhVienPhanNhomId = nhapChiTiet.DuocPhamBenhVienPhanNhomId;
                        //    newNhapKhoChiTiet.NgayNhapVaoBenhVien = nhapChiTiet.NgayNhapVaoBenhVien;
                        //    newNhapKhoChiTiet.TiLeTheoThapGia = nhapChiTiet.TiLeTheoThapGia;
                        //    newNhapKhoChiTiet.VAT = nhapChiTiet.VAT;
                        //    newNhapKhoChiTiet.DonGiaNhap = nhapChiTiet.DonGiaNhap;
                        //    newNhapKhoChiTiet.NgayNhap = thoiDiemHienTai;
                        //    newNhapKhoChiTiet.TiLeBHYTThanhToan = nhapChiTiet.TiLeBHYTThanhToan;
                        //}

                        var newXuatKhoChiTietViTri = new XuatKhoDuocPhamChiTietViTri()
                        {
                            NhapKhoDuocPhamChiTietId = nhapChiTiet.Id,
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

                            slYeuCau = Math.Round(slYeuCau - slTonNhapChiTiet, 2);
                            nhapChiTiet.SoLuongDaXuat = nhapChiTiet.SoLuongNhap;
                        }
                        newXuatKhoChiTiet.XuatKhoDuocPhamChiTietViTris.Add(newXuatKhoChiTietViTri);
                        //if (!isExists)
                        //{
                        //    newNhapKho.NhapKhoDuocPhamChiTiets.Add(newNhapKhoChiTiet);
                        //}
                        if (isFirstYeuCauDuocPham)
                        {
                            isFirstYeuCauDuocPham = false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (newXuatKhoChiTiet.XuatKhoDuocPhamChiTietViTris.Any())
                {
                    newPhieuXuat.XuatKhoDuocPhamChiTiets.Add(newXuatKhoChiTiet);
                    yeuCauNew.XuatKhoDuocPhamChiTiet = newXuatKhoChiTiet;
                    yeuCauNew.SoLuong = Math.Round(yeuCauNew.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat), 2);

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
                    yeuCauLinhTrucTiepDuocPham.YeuCauDuocPhamBenhViens.Add(yeuCauMoi);
                }
            }
            #endregion

            //newPhieuXuat.NhapKhoDuocPhams.Add(newNhapKho);
            yeuCauLinhTrucTiepDuocPham.XuatKhoDuocPhams.Add(newPhieuXuat);

            // tạo phiếu xuất cho người bệnh
            //var newXuatChoBenhNhan = new XuatKhoDuocPham()
            //{
            //    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
            //    LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
            //    NguoiNhanId = nguoiNhapId,
            //    NguoiXuatId = nguoiXuatId,
            //    LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.TrongHeThong,
            //    NgayXuat = thoiDiemHienTai,
            //    KhoXuatId = yeuCauLinhTrucTiepDuocPham.KhoNhapId
            //};


            //foreach (var nhapChiTiet in newNhapKho.NhapKhoDuocPhamChiTiets)
            //{
            //    var newXuatChoBenhNhanChiTiet = new XuatKhoDuocPhamChiTiet()
            //    {
            //        DuocPhamBenhVienId = nhapChiTiet.DuocPhamBenhVienId,
            //        NgayXuat = thoiDiemHienTai
            //    };
            //    newXuatChoBenhNhan.XuatKhoDuocPhamChiTiets.Add(newXuatChoBenhNhanChiTiet);

            //    var newXuatChoBenhNhanChiTietViTri = new XuatKhoDuocPhamChiTietViTri()
            //    {
            //        SoLuongXuat = nhapChiTiet.SoLuongNhap,
            //        NgayXuat = thoiDiemHienTai
            //    };
            //    newXuatChoBenhNhanChiTietViTri.NhapKhoDuocPhamChiTiet = nhapChiTiet;
            //    newXuatChoBenhNhanChiTiet.XuatKhoDuocPhamChiTietViTris.Add(newXuatChoBenhNhanChiTietViTri);

            //    nhapChiTiet.SoLuongDaXuat = nhapChiTiet.SoLuongNhap;
            //}
            //yeuCauLinhTrucTiepDuocPham.XuatKhoDuocPhams.Add(newXuatChoBenhNhan);


            // lưu thông tin yêu cầu lĩnh
            await _yeuCauLinhDuocPhamRepository.UpdateAsync(yeuCauLinhTrucTiepDuocPham);
            // cập nhật thông tin tông
            //await _nhapKhoDuocPhamChiTietRepository.UpdateAsync(lstNhapKhoTatCaDuocPhamYeuCauLinh);
        }

        public async Task<string> InPhieuDuyetLinhTrucTiepDuocPhamAsync(InPhieuDuyetLinhDuocPham inPhieu)
        {
            var content = string.Empty;
            var contentGNHT = string.Empty;
            var hearder = string.Empty;

            int? nhomVatTu = 0;
            string nhomVatTuString = _cauHinhRepository.TableNoTracking.Where(x => x.Name == "CauHinhNoiTru.NhomVatTuYTeBenhVien").Select(s => s.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(nhomVatTuString))
            {
                nhomVatTu = (int?)Convert.ToInt32(nhomVatTuString);
            }

            var templateLinhThuongDuocPham = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocTrucTiep"));
            var templateLinhThuongDuocPhamGNHT = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatDuyetYeuCauLinhThuocTrucTiepGayNghienHuongThan"));

            var data = await BaseRepository.TableNoTracking
                .Where(x => x.Id == inPhieu.YeuCauLinhDuocPhamId)
                .Select(item => new ThongTinInPhieuLinhDuocPhamVo()
                {
                    TenNguoiNhanHang = item.XuatKhoDuocPhams.Where(x => x.NguoiNhan != null).Select(x => x.NguoiNhan.User.HoTen).FirstOrDefault(),
                    //BoPhan = item.KhoNhap.PhongBenhVien != null ? item.KhoNhap.PhongBenhVien.Ma + " - " + item.KhoNhap.PhongBenhVien.Ten : (item.KhoNhap.KhoaPhong != null ? item.KhoNhap.KhoaPhong.Ma + " - " + item.KhoNhap.KhoaPhong.Ten : ""),
                    BoPhan = item.NoiYeuCauId != null ? item.NoiYeuCau.KhoaPhongId != null ? item.NoiYeuCau.KhoaPhong.Ma + "-" +item.NoiYeuCau.KhoaPhong.Ten
                             : ""
                             : "",
                    LyDoXuatKho = item.XuatKhoDuocPhams.Select(x => x.LyDoXuatKho).FirstOrDefault(),
                    XuatTaiKho = item.KhoXuat.Ten,
                    DiaDiem = "",

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            // cập nhật 29/10/2021: trường hợp đã duyệt thì lấy thông tin từ YeCauLinhDuocPhamCHiTiet
            var yeuCauLinhDuocPham = await _yeuCauLinhDuocPhamRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == inPhieu.YeuCauLinhDuocPhamId);
            var query = new List<ThongTinInPhieuLinhDuocPhamChiTietVo>();
            var queryGNHT = new List<ThongTinInPhieuLinhDuocPhamChiTietVo>();

            if (yeuCauLinhDuocPham.DuocDuyet != true)
            {
                query = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(x => x.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId &&
                                                                                           x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                           && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien &&
                                                                                               x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan))
                .Select(s => new ThongTinInPhieuLinhDuocPhamChiTietVo
                {
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    Ma = s.DuocPhamBenhVien.Ma,
                    TenThuoc = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && s.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                       (s.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && s.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.HamLuong != null && s.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                    DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                    SLYeuCau = s.SoLuong,
                    SLThucXuat = s.SoLuong,
                })
                .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DVT, x.Ma })
                .Select(item => new ThongTinInPhieuLinhDuocPhamChiTietVo()
                {
                    Ma = item.First().Ma,
                    TenThuoc = item.First().TenThuoc,
                    DVT = item.First().DVT,
                    SLYeuCau = item.Sum(x => x.SLYeuCau),
                    SLThucXuat = item.Sum(x => x.SLThucXuat),
                    LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                }).OrderBy(d =>d.TenThuoc)
                .ToListAsync();

                queryGNHT = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(x => x.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId &&
                                                                                          x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                          && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien ||
                                                                                              x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
               .Select(s => new ThongTinInPhieuLinhDuocPhamChiTietVo
               {
                   DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                   Ma = s.DuocPhamBenhVien.Ma,
                   TenThuoc = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                    s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && s.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                      (s.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && s.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                    s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.HamLuong != null && s.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                   //DVT = s.DuocPhamBenhVien.DuocPham.DonViTinhThamKhao ?? (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                   DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                   SLYeuCau = s.SoLuong,
                   SLThucXuat = s.SoLuong,
                   LaDuocPhamBHYT = s.LaDuocPhamBHYT,
               })
               .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DVT, x.Ma })
               .Select(item => new ThongTinInPhieuLinhDuocPhamChiTietVo()
               {
                   Ma = item.First().Ma,
                   TenThuoc = item.First().TenThuoc,
                   DVT = item.First().DVT,
                   SLYeuCau = item.Sum(x => x.SLYeuCau),
                   SLThucXuat = item.Sum(x => x.SLThucXuat),
                   LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
               }).OrderBy(d=>d.TenThuoc)
               .ToListAsync();
            }
            else
            {
                //query = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(x => x.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId &&
                //                                                                           x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                //                                                                           && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien &&
                //                                                                               x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan))
                query = await _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId 
                                && x.YeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.GayNghien 
                                    && x.DuocPhamBenhVien.LoaiThuocTheoQuanLy != Enums.LoaiThuocTheoQuanLy.HuongThan))
                .Select(s => new ThongTinInPhieuLinhDuocPhamChiTietVo
                {
                    DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                    Ma = s.DuocPhamBenhVien.Ma,
                    TenThuoc = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && s.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                       (s.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && s.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                     s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.HamLuong != null && s.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                    DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                    SLYeuCau = s.SoLuong,
                    SLThucXuat = s.SoLuong,
                    LaDuocPhamBHYT = s.LaDuocPhamBHYT,
                })
                .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DVT, x.Ma })
                .Select(item => new ThongTinInPhieuLinhDuocPhamChiTietVo()
                {
                    Ma = item.First().Ma,
                    TenThuoc = item.First().TenThuoc,
                    DVT = item.First().DVT,
                    SLYeuCau = item.Sum(x => x.SLYeuCau),
                    SLThucXuat = item.Sum(x => x.SLThucXuat),
                    LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
                }).OrderBy(d => d.TenThuoc)
                .ToListAsync();

                //queryGNHT = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Where(x => x.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId &&
                //                                                                          x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                //                                                                          && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien ||
                //                                                                              x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
                queryGNHT = await _yeuCauLinhDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.YeuCauLinhDuocPhamId == inPhieu.YeuCauLinhDuocPhamId
                                && x.YeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                && (x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien 
                                    || x.DuocPhamBenhVien.LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan))
               .Select(s => new ThongTinInPhieuLinhDuocPhamChiTietVo
               {
                   DuocPhamBenhVienId = s.DuocPhamBenhVienId,
                   Ma = s.DuocPhamBenhVien.Ma,
                   TenThuoc = (int?)(s.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId) == (int?)nhomVatTu ?
                                                                    s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.NhaSanXuat != null && s.DuocPhamBenhVien.DuocPham.NhaSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NhaSanXuat : "") +
                                                                                                      (s.DuocPhamBenhVien.DuocPham.NuocSanXuat != null && s.DuocPhamBenhVien.DuocPham.NuocSanXuat != "" ? "; " + s.DuocPhamBenhVien.DuocPham.NuocSanXuat : "") :
                                                                    s.DuocPhamBenhVien.DuocPham.Ten + (s.DuocPhamBenhVien.DuocPham.HamLuong != null && s.DuocPhamBenhVien.DuocPham.HamLuong != "" ? "; " + s.DuocPhamBenhVien.DuocPham.HamLuong : ""),
                   //DVT = s.DuocPhamBenhVien.DuocPham.DonViTinhThamKhao ?? (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                   DVT = (s.DuocPhamBenhVien.DuocPham.DonViTinh != null ? s.DuocPhamBenhVien.DuocPham.DonViTinh.Ten : null),
                   SLYeuCau = s.SoLuong,
                   SLThucXuat = s.SoLuong,
                   LaDuocPhamBHYT = s.LaDuocPhamBHYT,
               })
               .GroupBy(x => new { x.DuocPhamBenhVienId, x.LaDuocPhamBHYT, x.DVT, x.Ma })
               .Select(item => new ThongTinInPhieuLinhDuocPhamChiTietVo()
               {
                   Ma = item.First().Ma,
                   TenThuoc = item.First().TenThuoc,
                   DVT = item.First().DVT,
                   SLYeuCau = item.Sum(x => x.SLYeuCau),
                   SLThucXuat = item.Sum(x => x.SLThucXuat),
                   LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
               }).OrderBy(d => d.TenThuoc)
               .ToListAsync();
            }


            #region thuốc thường
            if (query.Any())
            {
                var objData = GetHTMLLinhBenhNhanDaDuyet(query, false);
                data.DanhSachThuoc = objData.html;

                content = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPham.Body, data);
            }
            #endregion
            #region thuốc gây nghiện hướng thần
            if (queryGNHT.Any())
            {
                var objData = GetHTMLLinhBenhNhanDaDuyet(queryGNHT,true);
                data.HeaderPhieuLinhThuoc = hearder;
                data.DanhSachThuoc = objData.html;
                contentGNHT = TemplateHelpper.FormatTemplateWithContentTemplate(templateLinhThuongDuocPhamGNHT.Body, data);
            }
            #endregion
            var headerTitile = "<div class=\'wrap\'><div class=\'content\'>PHIẾU XUẤT THUỐC</div></div>";

            if (!string.IsNullOrEmpty(content))
            {
                content = headerTitile + content;
            }
            var congPage = string.Empty;
            congPage = !string.IsNullOrEmpty(content) ? "<div style='break-after:page'></div>" : "";
            if (!string.IsNullOrEmpty(contentGNHT))
            {
                content = content + headerTitile + congPage + contentGNHT;
            }
            return content;
        }
        #endregion
        #region get html lĩnh bệnh nhân đã duyệt
        private OBJList GetHTMLLinhBenhNhanDaDuyet(List<ThongTinInPhieuLinhDuocPhamChiTietVo> gridVos,bool loaiThuoc)
        {
            int STT = 1;
            string infoLinhDuocChiTiet = string.Empty;
            foreach (var item in gridVos)
            {
                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLYeuCau), false) : item.SLYeuCau.MathRoundNumber(2) + "")
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLThucXuat), false) : item.SLThucXuat.MathRoundNumber(2) + "")
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"
                                       + "</tr>";
                STT++;
            }
            var data = new OBJList { 
                html = infoLinhDuocChiTiet,
                Index = STT
            };
            return data;
        }
        #endregion
        #region get html lĩnh bù đã duyệt
        private OBJList GetHTMLLinhBuDaDuyet(List<ThongTinInPhieuLinhDuocPhamChiTietVo> gridVos, bool loaiThuoc)
        {
            int STT = 1;
            string infoLinhDuocChiTiet = string.Empty;
            foreach (var item in gridVos)
            {
                infoLinhDuocChiTiet = infoLinhDuocChiTiet
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.TenThuoc
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                         + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLYeuCau), false) : item.SLYeuCau.MathRoundNumber(2) + "")
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + (loaiThuoc == true ? NumberHelper.ChuyenSoRaText(Convert.ToDouble(item.SLThucXuat), false) : item.SLThucXuat.MathRoundNumber(2) + "")
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + "&nbsp;"
                                       + "</tr>";
                STT++;
            }
            var data = new OBJList
            {
                html = infoLinhDuocChiTiet,
                Index = STT
            };
            return data;
        }
        #endregion
    }
}
