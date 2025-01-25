using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KetQuaCLS;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using System.Text.RegularExpressions;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        //public async Task<GridDataSource> GetDataDanhSachKetLuanCLSKhamSucKhoeDoanForGridAsync(QueryInfo queryInfo)
        //{
        //    if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {
        //        return null;
        //    }
        //    BuildDefaultSortExpression(queryInfo);
        //    var queryString = JsonConvert.DeserializeObject<DanhSachKetLuanCLSKhamSucKhoeDoanGridVo>(queryInfo.AdditionalSearchString);

        //    var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
        //        .Where(p => p.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
        //                 && p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId
        //                 && p.BenhNhanId != null)
        //        .Select(s => new DanhSachKetLuanCLSKhamSucKhoeDoanGridVo
        //        {
        //            Id = s.Id,
        //            YeuCauTiepNhanId = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault(),
        //            CongTyKhamSucKhoeId = s.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
        //            HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId,
        //            MaTN = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.MaYeuCauTiepNhan).FirstOrDefault(),
        //            MaBN = s.BenhNhan.MaBN,
        //            MaNhanVien = s.MaNhanVien,
        //            HoTen = s.HoTen,
        //            TenNgheNghiep = s.NgheNghiep.Ten,
        //            GioiTinh = s.GioiTinh,
        //            NamSinh = s.NamSinh,
        //            SoDienThoai = s.SoDienThoai,
        //            Email = s.Email,
        //            SoChungMinhThu = s.SoChungMinhThu,
        //            TenDanToc = s.DanToc.Ten,
        //            TenTinhThanh = s.TinhThanh.Ten,
        //            NhomDoiTuongKhamSucKhoe = s.NhomDoiTuongKhamSucKhoe,
        //            KSKKetLuanPhanLoaiSucKhoe = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KSKKetLuanPhanLoaiSucKhoe).FirstOrDefault(),
        //            DichVuDaThucHien = s.YeuCauTiepNhans.SelectMany(p => p.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.GoiKhamSucKhoeId && yc.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)).Count()
        //                               + s.YeuCauTiepNhans.SelectMany(p => p.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.GoiKhamSucKhoeId && yc.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)).Count(),
        //            TongDichVu = s.YeuCauTiepNhans.SelectMany(p => p.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)).Count()
        //                       + s.YeuCauTiepNhans.SelectMany(p => p.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)).Count(),
        //            NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc,
        //            LaHopDongDaKetLuan = s.HopDongKhamSucKhoe.DaKetThuc
        //        });
        //    if (queryString.LaHopDongDaKetLuan == true)
        //    {
        //        query = query.Where(p => p.LaHopDongDaKetLuan == true);
        //    }
        //    // 0: ChuaKetLuan, 1 : DaKetLuan
        //    if (queryString.TrangThai != null && (queryString.TrangThai.ChuaKetLuan != null && queryString.TrangThai.ChuaKetLuan != null))
        //    {
        //        if (queryString.TrangThai.ChuaKetLuan == false && queryString.TrangThai.DaKetLuan == true)
        //        {
        //            query = query.Where(p => p.TinhTrang == 1);
        //        }
        //        else if (queryString.TrangThai.ChuaKetLuan == true && queryString.TrangThai.DaKetLuan == false)
        //        {
        //            query = query.Where(p => p.TinhTrang == 0);
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(queryString.SearchString))
        //    {
        //        var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
        //        query = query.ApplyLike(searchTerms,
        //           g => g.MaTN,
        //           g => g.MaBN,
        //           g => g.MaNhanVien,
        //           g => g.HoTen,
        //           g => g.TenNgheNghiep,
        //           g => g.NamSinh.ToString(),
        //           g => g.SoDienThoai,
        //           g => g.Email,
        //           g => g.SoChungMinhThu,
        //           g => g.TenTinhThanh,
        //           g => g.NhomDoiTuongKhamSucKhoe
        //       );
        //    }
        //    var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
        //    var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
        //        .Take(queryInfo.Take).ToArrayAsync();
        //    await Task.WhenAll(countTask, queryTask);
        //    return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        //}
        //public async Task<GridDataSource> GetTotalPageDanhSachKetLuanCLSKhamSucKhoeDoanForGridAsync(QueryInfo queryInfo)
        //{
        //    if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
        //    {
        //        return null;

        //    }
        //    var queryString = JsonConvert.DeserializeObject<DanhSachKetLuanCLSKhamSucKhoeDoanGridVo>(queryInfo.AdditionalSearchString);

        //    var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
        //           .Where(p => p.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId && p.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId && p.BenhNhanId != null)
        //        .Select(s => new DanhSachKetLuanCLSKhamSucKhoeDoanGridVo
        //        {
        //            Id = s.Id,
        //            YeuCauTiepNhanId = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault(),
        //            CongTyKhamSucKhoeId = s.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
        //            HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeId,
        //            MaTN = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.MaYeuCauTiepNhan).FirstOrDefault(),
        //            MaBN = s.BenhNhan.MaBN,
        //            MaNhanVien = s.MaNhanVien,
        //            HoTen = s.HoTen,
        //            TenNgheNghiep = s.NgheNghiep.Ten,
        //            GioiTinh = s.GioiTinh,
        //            NamSinh = s.NamSinh,
        //            SoDienThoai = s.SoDienThoai,
        //            Email = s.Email,
        //            SoChungMinhThu = s.SoChungMinhThu,
        //            TenDanToc = s.DanToc.Ten,
        //            TenTinhThanh = s.TinhThanh.Ten,
        //            NhomDoiTuongKhamSucKhoe = s.NhomDoiTuongKhamSucKhoe,
        //            KSKKetLuanPhanLoaiSucKhoe = s.YeuCauTiepNhans.OrderByDescending(p => p.Id).Select(p => p.KSKKetLuanPhanLoaiSucKhoe).FirstOrDefault(),
        //            DichVuDaThucHien = s.YeuCauTiepNhans.SelectMany(p => p.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.GoiKhamSucKhoeId && yc.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)).Count()
        //                               + s.YeuCauTiepNhans.SelectMany(p => p.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.GoiKhamSucKhoeId && yc.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)).Count(),
        //            TongDichVu = s.YeuCauTiepNhans.SelectMany(p => p.YeuCauKhamBenhs.Where(yc => yc.GoiKhamSucKhoeId == s.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)).Count()
        //                       + s.YeuCauTiepNhans.SelectMany(p => p.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)).Count(),
        //            NgayKetThuc = s.HopDongKhamSucKhoe.NgayKetThuc,
        //            LaHopDongDaKetLuan = s.HopDongKhamSucKhoe.DaKetThuc
        //        });
        //    if (queryString.LaHopDongDaKetLuan == true)
        //    {
        //        query = query.Where(p => p.LaHopDongDaKetLuan == true);
        //    }
        //    // 0: ChuaKetLuan, 1 : DaKetLuan
        //    if (queryString.TrangThai != null && (queryString.TrangThai.ChuaKetLuan != null && queryString.TrangThai.ChuaKetLuan != null))
        //    {
        //        if (queryString.TrangThai.ChuaKetLuan == false && queryString.TrangThai.DaKetLuan == true)
        //        {
        //            query = query.Where(p => p.TinhTrang == 1);
        //        }
        //        else if (queryString.TrangThai.ChuaKetLuan == true && queryString.TrangThai.DaKetLuan == false)
        //        {
        //            query = query.Where(p => p.TinhTrang == 0);
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(queryString.SearchString))
        //    {
        //        var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
        //        query = query.ApplyLike(searchTerms,
        //           g => g.MaTN,
        //           g => g.MaBN,
        //           g => g.MaNhanVien,
        //           g => g.HoTen,
        //           g => g.TenNgheNghiep,
        //           g => g.NamSinh.ToString(),
        //           g => g.SoDienThoai,
        //           g => g.Email,
        //           g => g.SoChungMinhThu,
        //           g => g.TenTinhThanh,
        //           g => g.NhomDoiTuongKhamSucKhoe
        //       );
        //    }
        //    var countTask = query.CountAsync();
        //    await Task.WhenAll(countTask);
        //    return new GridDataSource { TotalRowCount = countTask.Result };
        //}
        public async Task<GridDataSource> GetDataForGridAsyncKetQuaCLS(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long yeuCauTiepNhan = 0;
            long hopDongId = 0;
            long congTyId = 0;
            if (queryInfo.AdditionalSearchString != null)
            {
                var querystring = queryInfo.AdditionalSearchString.Split('-');
                yeuCauTiepNhan = long.Parse(querystring[0]);
                hopDongId = long.Parse(querystring[1]);
                congTyId = long.Parse(querystring[2]);
            }
            var yeuCauKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                 .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhan)
                .Include(cc => cc.PhienXetNghiemChiTiets)
                .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhomDichVuBenhVien)
                .Include(cc => cc.GoiKhamSucKhoe).ToList();

            if (!yeuCauKyThuat.Any())
            {
                return null;
            }
            var ketQuaCLSs = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                  && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh) && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
               .Select(s => new KetQuaCLSGridVo()
               {
                   Id = s.Id,
                   NoiDung = s.TenDichVu,
                   NguoiThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                   NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                   BacSiKetLuan = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : null,
                   NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                   LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                   LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                   YeuCauTiepNhanId = yeuCauTiepNhan,
                   //ChuanDoan = s.YeuCauKhamBenh.Icdchinh.TenTiengViet,
                   IsDisable = true,
                   IsCheck = true
               });

            //group by theo phiên và nhóm dịch vu bệnh viện ID
            var yeuCauKyThuatOfXNs = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                   && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem) && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);

            var yeuCauKyThuatIdsOfXN = yeuCauKyThuatOfXNs.Select(cc => cc.Id).ToList();
            if (yeuCauKyThuatIdsOfXN.Any())
            {
                //var phienXetNghiemChiIds = _phienXetNghiemRepository.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhan)
                //.Select(cc => cc.Id).ToList();

                List<KetQuaCLSGridVo> listKQXN = new List<KetQuaCLSGridVo>();

                var phienXetNghiemChiTiets = _phienXetNghiemChiTietRepository.TableNoTracking
                    .Where(o => yeuCauKyThuatIdsOfXN.Contains(o.YeuCauDichVuKyThuatId) && o.ThoiDiemKetLuan != null && o.PhienXetNghiem.ThoiDiemKetLuan != null)
                                                                        .Include(x => x.KetQuaXetNghiemChiTiets)
                                                                        .Include(x => x.PhienXetNghiem)
                                                                        .ToList();

                var nhanViens = _nhanVienRepository.TableNoTracking
                    .Select(o => new LookupItemVo
                    {
                        KeyId = o.Id,
                        DisplayName = o.User.HoTen
                    })
                    .ToList();
                var dichVuXetNghiems = _dichVuXetNghiemRepository.TableNoTracking
                    .Select(o => new LookupItemVo
                    {
                        KeyId = o.Id,
                        DisplayName = o.Ten
                    })
                    .ToList();
                var mayXetNghiems = _mayXetNghiemRepository.TableNoTracking
                    .Select(o => new LookupItemVo
                    {
                        KeyId = o.Id,
                        DisplayName = o.Ten
                    })
                    .ToList();

                if (phienXetNghiemChiTiets.Any())
                {
                    foreach (var groupPhienXetNghiemChiTiet in phienXetNghiemChiTiets.GroupBy(o => new {o.PhienXetNghiemId, o.NhomDichVuBenhVienId}))
                    {
                        var data = new KetQuaCLSGridVo();
                        var phienXetNghiemChiTietLast = groupPhienXetNghiemChiTiet.OrderBy(o=>o.ThoiDiemKetLuan).Last();
                        
                        data.Id = phienXetNghiemChiTietLast.YeuCauDichVuKyThuatId;
                        data.NoiDung = yeuCauKyThuatOfXNs.First(o=>o.Id == phienXetNghiemChiTietLast.YeuCauDichVuKyThuatId).NhomDichVuBenhVien.Ten;
                        data.LoaiKetQuaId = phienXetNghiemChiTietLast.NhomDichVuBenhVienId;
                        data.LoaiKetQuaCLS = Enums.LoaiDichVuKyThuat.XetNghiem.GetDescription();
                        data.NguoiThucHien = nhanViens.FirstOrDefault(o=>o.KeyId == phienXetNghiemChiTietLast.PhienXetNghiem.NhanVienThucHienId)?.DisplayName;
                        data.NgayThucHien = phienXetNghiemChiTietLast.PhienXetNghiem.ThoiDiemBatDau.ApplyFormatDateTimeSACH();
                        data.BacSiKetLuan = nhanViens.FirstOrDefault(o => o.KeyId == phienXetNghiemChiTietLast.PhienXetNghiem.NhanVienKetLuanId)?.DisplayName;
                        data.NgayKetLuan = phienXetNghiemChiTietLast.ThoiDiemKetLuan?.ApplyFormatDateTimeSACH();
                        data.ChuanDoan = phienXetNghiemChiTietLast.PhienXetNghiem.KetLuan;
                        data.PhienXetNghiemId = phienXetNghiemChiTietLast.PhienXetNghiem.Id;
                        data.YeuCauTiepNhanId = yeuCauTiepNhan;
                        data.NhomDichVuBenhVienId = phienXetNghiemChiTietLast.NhomDichVuBenhVienId;

                        foreach (var phienXetNghiemChiTiet in groupPhienXetNghiemChiTiet)
                        {
                            data.LanThucHien = 1;
                            data.KetQuaCLSGridChiTietVos.AddRange(addDetailDataChild(phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.ToList(), new List<KetQuaXetNghiemChiTiet>(), nhanViens, dichVuXetNghiems, mayXetNghiems, true));
                        }
                        listKQXN.Add(data);
                    }
                    
                }

                ketQuaCLSs = ketQuaCLSs.Union(listKQXN); //listTDCNCDHA.AsQueryable().Union(listKQXN);
            }

            //query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);
            //var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);

            var dataCLS = ketQuaCLSs.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = dataCLS, TotalRowCount = ketQuaCLSs.Count() };
        }
        public async Task<GridDataSource> GetDataForGridAsyncKetQuaCLSOld(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long yeuCauTiepNhan = 0;
            long hopDongId = 0;
            long congTyId = 0;
            if (queryInfo.AdditionalSearchString != null)
            {
                var querystring = queryInfo.AdditionalSearchString.Split('-');
                yeuCauTiepNhan = long.Parse(querystring[0]);
                hopDongId = long.Parse(querystring[1]);
                congTyId = long.Parse(querystring[2]);
            }
            var yeuCauKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                 .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhan &&
                                                                 (x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                                                 && (x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == congTyId
                                                                                                                        && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongId) 
                                                                 ))
                .Include(cc => cc.PhienXetNghiemChiTiets)
                .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhomDichVuBenhVien)
                .Include(cc => cc.GoiKhamSucKhoe);

            if (!yeuCauKyThuat.Any())
            {
                return null;
            }
            var query = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                  && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh) && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
               .Select(s => new KetQuaCLSGridVo()
               {
                   Id = s.Id,
                   NoiDung = s.TenDichVu,
                   NguoiThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                   NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                   BacSiKetLuan = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : null,
                   NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                   LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                   LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                   YeuCauTiepNhanId =yeuCauTiepNhan,
                   //ChuanDoan = s.YeuCauKhamBenh.Icdchinh.TenTiengViet,
                   IsDisable = true,
                   IsCheck = true
               });

            //group by theo phiên và nhóm dịch vu bệnh viện ID
            var yeuCauKyThuatOfXNs = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien )
                   && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem) && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);

            var yeuCauKyThuatIdsOfXN = yeuCauKyThuatOfXNs.Select(cc => cc.Id);
            if (yeuCauKyThuatIdsOfXN.Any())
            {
                var phienXetNghiemChiIds = _phienXetNghiemRepository.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhan)
                .Select(cc => cc.Id).ToList();

                List<KetQuaCLSGridVo> listKQXN = new List<KetQuaCLSGridVo>();

                var phienXetNghiemChiTiets = _phienXetNghiemChiTietRepository.TableNoTracking.Where(o => yeuCauKyThuatIdsOfXN.Contains(o.YeuCauDichVuKyThuatId)
                                                                                                && o.ThoiDiemKetLuan != null
                                                                                                && o.PhienXetNghiem.ThoiDiemKetLuan != null
                                                                                                && phienXetNghiemChiIds.Contains(o.PhienXetNghiemId))
                                                                        .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhomDichVuBenhVien)
                                                                        .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                                                                        .Include(x => x.PhienXetNghiem).ThenInclude(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                                                                        .Include(x => x.PhienXetNghiem).ThenInclude(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                                                                        .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                        .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                                        .GroupBy(cc => new { cc.NhomDichVuBenhVienId, cc.PhienXetNghiemId });
                if (phienXetNghiemChiTiets.Any())
                {

                    foreach (var items in phienXetNghiemChiTiets)
                    {
                        var data = new KetQuaCLSGridVo();
                        var phienXetNghiemChiTiet = items.Select(cc => cc).LastOrDefault();
                        if (phienXetNghiemChiTiet != null)
                        {
                            data.Id = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.Id;
                            data.NoiDung = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.NhomDichVuBenhVien.Ten;
                            data.LoaiKetQuaId = phienXetNghiemChiTiet.NhomDichVuBenhVienId;
                            data.LoaiKetQuaCLS = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.LoaiDichVuKyThuat.GetDescription();
                            data.NguoiThucHien = phienXetNghiemChiTiet.PhienXetNghiem.NhanVienThucHien.User.HoTen;
                            data.NgayThucHien = phienXetNghiemChiTiet.PhienXetNghiem.ThoiDiemBatDau.ApplyFormatDateTimeSACH();
                            data.BacSiKetLuan = phienXetNghiemChiTiet.PhienXetNghiem.NhanVienKetLuan.User.HoTen;
                            data.NgayKetLuan = phienXetNghiemChiTiet.ThoiDiemKetLuan?.ApplyFormatDateTimeSACH();
                            data.ChuanDoan = phienXetNghiemChiTiet.PhienXetNghiem.KetLuan;
                            data.PhienXetNghiemId = phienXetNghiemChiTiet.PhienXetNghiem.Id;
                            data.YeuCauTiepNhanId = yeuCauTiepNhan;
                            data.NhomDichVuBenhVienId = phienXetNghiemChiTiet.NhomDichVuBenhVienId;
                            //data.GoiPhienXetNghiemLai = GoiPhienXetNghiemLai(phienXetNghiemChiTiet.PhienXetNghiem.Id);
                            //data.TrangThaiXetNghiemLai = TrangThaiChayLaiXetNghiem(phienXetNghiemChiTiet.PhienXetNghiem.Id, phienXetNghiemChiTiet.NhomDichVuBenhVienId);
                        }

                        var phienXetNghiemChiTietCuoiCungIds = items.GroupBy(cc => cc.YeuCauDichVuKyThuatId).Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).ToList();
                        if (phienXetNghiemChiTietCuoiCungIds.Any())
                        {
                            foreach (var yeuCaukyThuatChaySauCung in phienXetNghiemChiTietCuoiCungIds)
                            {
                                data.LanThucHien = yeuCaukyThuatChaySauCung.LanThucHien;
                                var ketqua = _ketQuaXetNghiemChiTietRepository.TableNoTracking.Where(c => c.PhienXetNghiemChiTietId == yeuCaukyThuatChaySauCung.Id)
                                     .Include(x => x.DichVuXetNghiem)
                                    .Include(x => x.MayXetNghiem).Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                                    .Include(x => x.NhanVienDuyet).ThenInclude(cc => cc.User).ToList();
                                //data.KetQuaCLSGridChiTietVos.AddRange(addDetailDataChild(ketqua, new List<KetQuaXetNghiemChiTiet>(), true));
                                listKQXN.Add(data);
                            }
                        }
                    }
                }

                query = query.Union(listKQXN); //listTDCNCDHA.AsQueryable().Union(listKQXN);
            }

            query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);
            var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);

            var dataCLS = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = dataCLS };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncKetQuaCLS(QueryInfo queryInfo)
        {
            long yeuCauTiepNhan = 0;
            long hopDongId = 0;
            long congTyId = 0;
            if (queryInfo.AdditionalSearchString != null)
            {
                var querystring = queryInfo.AdditionalSearchString.Split('-');
                yeuCauTiepNhan = long.Parse(querystring[0]);
                hopDongId = long.Parse(querystring[1]);
                congTyId = long.Parse(querystring[2]);
            }
            var yeuCauKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                 .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhan &&
                                                                 (x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                                                 && (x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == congTyId
                                                                                                                        && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == hopDongId)))
                .Include(cc => cc.PhienXetNghiemChiTiets)
                .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhomDichVuBenhVien);
            if (!yeuCauKyThuat.Any())
            {
                return null;
            }
            var query = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                  && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh) && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
               .Select(s => new KetQuaCLSGridVo()
               {
                   Id = s.Id,
                   NoiDung = s.TenDichVu,
                   NguoiThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                   NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                   BacSiKetLuan = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : null,
                   NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                   LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                   LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                   YeuCauTiepNhanId = yeuCauTiepNhan,
                   //ChuanDoan = s.YeuCauKhamBenh.Icdchinh.TenTiengViet,
                   IsDisable = true,
                   IsCheck = true
               });

            //group by theo phiên và nhóm dịch vu bệnh viện ID
            var yeuCauKyThuatOfXNs = yeuCauKyThuat.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                   && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem) && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);

            var yeuCauKyThuatIdsOfXN = yeuCauKyThuatOfXNs.Select(cc => cc.Id);
            if (yeuCauKyThuatIdsOfXN.Any())
            {
                var phienXetNghiemChiIds = _phienXetNghiemRepository.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhan)
                .Select(cc => cc.Id).ToList();

                List<KetQuaCLSGridVo> listKQXN = new List<KetQuaCLSGridVo>();

                var phienXetNghiemChiTiets = _phienXetNghiemChiTietRepository.TableNoTracking.Where(o => yeuCauKyThuatIdsOfXN.Contains(o.YeuCauDichVuKyThuatId)
                                                                                                && o.ThoiDiemKetLuan != null
                                                                                                && o.PhienXetNghiem.ThoiDiemKetLuan != null
                                                                                                && phienXetNghiemChiIds.Contains(o.PhienXetNghiemId))
                                                                        .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhomDichVuBenhVien)
                                                                        .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                                                                        .Include(x => x.PhienXetNghiem).ThenInclude(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                                                                        .Include(x => x.PhienXetNghiem).ThenInclude(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                                                                        .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                                                        .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                                                        .GroupBy(cc => new { cc.NhomDichVuBenhVienId, cc.PhienXetNghiemId });
                if (phienXetNghiemChiTiets.Any())
                {

                    foreach (var items in phienXetNghiemChiTiets)
                    {
                        var data = new KetQuaCLSGridVo();
                        var phienXetNghiemChiTiet = items.Select(cc => cc).LastOrDefault();
                        if (phienXetNghiemChiTiet != null)
                        {
                            data.Id = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.Id;
                            data.NoiDung = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.NhomDichVuBenhVien.Ten;
                            data.LoaiKetQuaId = phienXetNghiemChiTiet.NhomDichVuBenhVienId;
                            data.LoaiKetQuaCLS = phienXetNghiemChiTiet.YeuCauDichVuKyThuat.LoaiDichVuKyThuat.GetDescription();
                            data.NguoiThucHien = phienXetNghiemChiTiet.PhienXetNghiem.NhanVienThucHien.User.HoTen;
                            data.NgayThucHien = phienXetNghiemChiTiet.PhienXetNghiem.ThoiDiemBatDau.ApplyFormatDateTimeSACH();
                            data.BacSiKetLuan = phienXetNghiemChiTiet.PhienXetNghiem.NhanVienKetLuan.User.HoTen;
                            data.NgayKetLuan = phienXetNghiemChiTiet.ThoiDiemKetLuan?.ApplyFormatDateTimeSACH();
                            data.ChuanDoan = phienXetNghiemChiTiet.PhienXetNghiem.KetLuan;
                            data.PhienXetNghiemId = phienXetNghiemChiTiet.PhienXetNghiem.Id;
                            data.YeuCauTiepNhanId = yeuCauTiepNhan;
                            data.NhomDichVuBenhVienId = phienXetNghiemChiTiet.NhomDichVuBenhVienId;
                            //data.GoiPhienXetNghiemLai = GoiPhienXetNghiemLai(phienXetNghiemChiTiet.PhienXetNghiem.Id);
                            //data.TrangThaiXetNghiemLai = TrangThaiChayLaiXetNghiem(phienXetNghiemChiTiet.PhienXetNghiem.Id, phienXetNghiemChiTiet.NhomDichVuBenhVienId);
                        }

                        var phienXetNghiemChiTietCuoiCungIds = items.GroupBy(cc => cc.YeuCauDichVuKyThuatId).Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).ToList();
                        if (phienXetNghiemChiTietCuoiCungIds.Any())
                        {
                            foreach (var yeuCaukyThuatChaySauCung in phienXetNghiemChiTietCuoiCungIds)
                            {
                                data.LanThucHien = yeuCaukyThuatChaySauCung.LanThucHien;
                                var ketqua = _ketQuaXetNghiemChiTietRepository.TableNoTracking.Where(c => c.PhienXetNghiemChiTietId == yeuCaukyThuatChaySauCung.Id)
                                     .Include(x => x.DichVuXetNghiem)
                                    .Include(x => x.MayXetNghiem).Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
                                    .Include(x => x.NhanVienDuyet).ThenInclude(cc => cc.User).ToList();
                                //data.KetQuaCLSGridChiTietVos.AddRange(addDetailDataChild(ketqua, new List<KetQuaXetNghiemChiTiet>(), true));
                                listKQXN.Add(data);
                            }
                        }
                    }
                }

                query = query.Union(listKQXN); //listTDCNCDHA.AsQueryable().Union(listKQXN);
            }

            query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);
            var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
            var countTask = dataOrderBy.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }
        public async Task<List<ListKetQuaCLS>> GetListKetQuaCLS(long yeuCauTiepNhanId)
        {
            //var yeuCauKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null)
            //    .Include(cc => cc.PhienXetNghiemChiTiets).ThenInclude(cx=>cx.KetQuaXetNghiemChiTiets)
            //    .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
            //    .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
            //    .Include(cc => cc.NhomDichVuBenhVien);
            var thongTinNhanVienKham =
              await BaseRepository.TableNoTracking
                  .Include(x => x.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                  .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z => z.KetQuaXetNghiemChiTiets)
                  .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.PhienXetNghiemChiTiets).ThenInclude(z=>z.PhienXetNghiem)
                  .Include(x => x.KetQuaSinhHieus)
                  .Include(x => x.BenhNhan).ThenInclude(y => y.BenhNhanTienSuBenhs)
                  .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.BenhNhan)
                  .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.HopDongKhamSucKhoe).ThenInclude(z => z.CongTyKhamSucKhoe)
                  .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                  .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(y => y.GoiKhamSucKhoe).ThenInclude(z => z.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens)
                  .OrderByDescending(x => x.Id)
                   .FirstOrDefaultAsync(x => x.Id == yeuCauTiepNhanId);
            List<ListKetQuaCLS> listKetQua = new List<ListKetQuaCLS>();
            if (thongTinNhanVienKham.YeuCauDichVuKyThuats.Any())
            {
                var queryTDCNCDHA = thongTinNhanVienKham.YeuCauDichVuKyThuats.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                      && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh) )
                   .Select(s => new
                   {
                       DataKetQuaCLS = s.DataKetQuaCanLamSang
                   }).ToList();
                // ket qua CDHA TDCN
                foreach (var item in queryTDCNCDHA)
                {
                    ListKetQuaCLS listKetQuaOBJ = new ListKetQuaCLS();
                    if (item.DataKetQuaCLS != null)
                    {
                        var jsonOjbect = JsonConvert.DeserializeObject<DataCLS>(item.DataKetQuaCLS);
                        listKetQuaOBJ.KetQuaCLS = jsonOjbect.KetLuan;
                    }
                    listKetQuaOBJ.IsCheck = true;
                    listKetQua.Add(listKetQuaOBJ);
                }
              
                if (thongTinNhanVienKham.YeuCauDichVuKyThuats.Where(s => s.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien    && s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem).Any())
                {
                    var phienXetNghiemChiChiTiets = thongTinNhanVienKham.YeuCauDichVuKyThuats
                        .Where(s => s.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && 
                                    s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem )
                        .SelectMany(z => z.PhienXetNghiemChiTiets);
                    var dichVuXetNghiemKetNoiChiSos = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Select(s => new { TenKetNoi = s.TenKetNoi, Id = s.Id }).ToList();
                    if (phienXetNghiemChiChiTiets.Any(d=>d.ThoiDiemKetLuan != null && d.PhienXetNghiem.ThoiDiemKetLuan != null))
                    {
                        foreach (var itemDv in thongTinNhanVienKham.YeuCauDichVuKyThuats
                      .Where(s => s.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                      .Select(z => new { DataKetQuaCanLamSang = z.PhienXetNghiemChiTiets.Select(v => new { KetQuaXetNghiemChiTiet = v.KetQuaXetNghiemChiTiets.ToList(), LanThucHien = v.LanThucHien, KetLuan = v.KetLuan }).OrderBy(s => s.LanThucHien).LastOrDefault() }).ToList())
                        {
                            ListKetQuaCLS listKetQuaOBJ = new ListKetQuaCLS();
                            if (itemDv.DataKetQuaCanLamSang != null)
                            {
                                // phiên xét nghiệm chi tiết orderby cuoi cung
                                if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet != null)
                                {
                                    if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Any())
                                    {
                                        if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Count == 1)
                                        {
                                            var itemGiaTriMin = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriMin).First();
                                            var itemGiaTriMax = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriMax).First();
                                            var itemGTDuyet = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriDuyet).First();
                                            var itemGiaTriNhapTay = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriNhapTay).First();
                                            var itemGiaTriTuMay = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Select(s => s.GiaTriTuMay).First();
                                            var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; //? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? string.Empty));
                                            bool KieuSo = false;
                                            if (value != null)
                                            {
                                                KieuSo = IsInt(value) ? true : false;
                                            }
                                            else
                                            {
                                                KieuSo = false;
                                            }
                                            if (KieuSo == true)
                                            {
                                                double ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                double cSBTMin = 0;
                                                double cSBTMax = 0;

                                                if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                {
                                                    listKetQuaOBJ.KetQuaCLS = ketQua.ToString() + " ";
                                                }
                                                // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                {
                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                    if (!string.IsNullOrEmpty(min))
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Giảm)";
                                                    }
                                                    else
                                                    {
                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                        if (!string.IsNullOrEmpty(max))
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Tăng)";
                                                        }
                                                        else
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "";
                                                        }
                                                    }


                                                }
                                                if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                {
                                                    if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                    {
                                                        var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                        if (!string.IsNullOrEmpty(min))
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "( Giảm)";
                                                        }
                                                        else
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                    }
                                                }
                                                if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                {
                                                    if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                    {
                                                        var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                        if (!string.IsNullOrEmpty(max))
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ( Tăng)";
                                                        }
                                                        else
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                    }
                                                }
                                                // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                            }
                                            if (KieuSo == false)
                                            {
                                                listKetQuaOBJ.KetQuaCLS += value + " ";
                                            }
                                        }
                                        else
                                        {
                                            int itemCongKyTu = 0;
                                            foreach (var itemKetQuaListCon in itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.OrderByDescending(d => d.CapDichVu == 1 ? 1 : 0).ThenBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList())
                                            {
                                                var dichVuXetNghiemId = itemKetQuaListCon.DichVuXetNghiemId;
                                                var mauMayXetNghiemId = itemKetQuaListCon.MauMayXetNghiemId;
                                                var tenketQua = "";
                                                // nếu  mẫu máy xét nghiệm khác null => lấy ten dich vụ xét nghiệm trong db.DichVuXetNghiemKetNoiChiSo => field : TenKetNoi
                                                //itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId != null 
                                                if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId != null)
                                                {
                                                    tenketQua = dichVuXetNghiemKetNoiChiSos.Where(s => s.Id == itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId.GetValueOrDefault()).Select(s => s.TenKetNoi).FirstOrDefault();
                                                    
                                                    listKetQuaOBJ.KetQuaCLS += tenketQua != null ? tenketQua + ": " :"";

                                                    var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                    var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                    var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                    var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                    var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                    var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; //? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? string.Empty));
                                                    double ketQua;
                                                    bool KieuSo = false;
                                                    if (value != null)
                                                    {
                                                        KieuSo = IsInt(value) ? true : false;
                                                    }
                                                    else
                                                    {
                                                        KieuSo = false;
                                                    }
                                                    if (KieuSo == true)
                                                    {
                                                        double cSBTMin = 0;
                                                        double cSBTMax = 0;
                                                        ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                        if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                        }
                                                        // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                        if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Giảm)";
                                                            }
                                                            else
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "";
                                                                }
                                                            }


                                                        }
                                                        if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                        {
                                                            if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                            {
                                                                var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                if (!string.IsNullOrEmpty(min))
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "( Giảm)";
                                                                }
                                                                else
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ( Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE

                                                    }
                                                    if (KieuSo == false)
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += value + " ";

                                                    }

                                                    if (itemCongKyTu < itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Count())
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += "; ";

                                                    }

                                                }
                                                // nếu mẫu máy xét nghiệm == null => tên dịch vụ xét nghiệm trong db.KetQuaXetNghiemChiTiet => field :DichVuXetNghiemTen 
                                                //itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId == null
                                                if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId  == null)
                                                {
                                                    listKetQuaOBJ.KetQuaCLS += tenketQua != null ? itemKetQuaListCon.DichVuXetNghiemTen + ": " : "";

                                                    var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                    var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                    var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                    var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                    var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                    var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; //? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? string.Empty));
                                                    double ketQua;
                                                    bool KieuSo = false;
                                                    if (value != null)
                                                    {
                                                        KieuSo = IsInt(value) ? true : false;
                                                    }
                                                    else
                                                    {
                                                        KieuSo = false;
                                                    }
                                                    double cSBTMin = 0;
                                                    int cSBTMax = 0;
                                                    if (KieuSo == true)
                                                    {
                                                        ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                        if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";

                                                        }
                                                        // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                        if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Giảm)";
                                                            }
                                                            else
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "";
                                                                }
                                                            }


                                                        }
                                                        if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                        {
                                                            if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                            {
                                                                var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                                if (!string.IsNullOrEmpty(min))
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "( Giảm)";
                                                                }
                                                                else
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                            {
                                                                var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                                if (!string.IsNullOrEmpty(max))
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ( Tăng)";
                                                                }
                                                                else
                                                                {
                                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE 
                                                    }
                                                    if (KieuSo == false)
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += value + " ";

                                                    }

                                                    if (itemCongKyTu < itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiet.Count())
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += "; ";

                                                    }
                                                }
                                                itemCongKyTu++;
                                            }
                                        }

                                    }
                                }
                            }
                            listKetQuaOBJ.IsCheck = true;
                            if(!string.IsNullOrEmpty(listKetQuaOBJ.KetQuaCLS))
                            {
                                var plits = listKetQuaOBJ.KetQuaCLS.Split(";");
                                if(plits.Any())
                                {
                                    listKetQuaOBJ.KetQuaCLS = "";
                                    listKetQuaOBJ.KetQuaCLS = plits.Where(d => d != null && d != "" && d !=" ").ToList().Distinct().Join(";");
                                }
                            }
                            listKetQua.Add(listKetQuaOBJ);
                        }
                    }
                }
            }
            return  listKetQua;
        }

        public async Task<List<ListKetQuaCLS>> GetListKetQuaCLSNew(long yeuCauTiepNhanId)
        {
            List<ListKetQuaCLS> listKetQua = new List<ListKetQuaCLS>();

            var thongTinKham = BaseRepository.TableNoTracking.Where(x => x.Id == yeuCauTiepNhanId  && x.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
                              .Select(d => new
                              {
                                  YeuCauDichVuKyThuatIds = d.YeuCauDichVuKyThuats.Select(g => g.Id).ToList(),
                                  KetQuaKhamSucKhoeData = d.KetQuaKhamSucKhoeData,
                                  LoaiLuuInKetQuaKSK = d.LoaiLuuInKetQuaKSK
                              }).FirstOrDefault();



            #region // YeuCauDichVuKyThuat 
            // +CDHA TDCN
            var infoDichVuKyThuatCDHATDCNs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                 .Where(s => thongTinKham.YeuCauDichVuKyThuatIds.Contains(s.Id) && s.GoiKhamSucKhoeId != null &&
                 s.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien &&
                 (s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                 .Select(z => new
                 {
                     DataKetQuaCanLamSang = z.DataKetQuaCanLamSang,
                     TenDichVuKyThuat = z.TenDichVu,
                     Id = z.Id,
                     GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                     TrangThaiDVKham = (int)z.TrangThai
                 }).ToList();
            #endregion
            #region // + XetNghiem

           

            var infoDichVuKyThuatXNs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(s => thongTinKham.YeuCauDichVuKyThuatIds.Contains(s.Id) && s.GoiKhamSucKhoeId != null &&
                s.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien &&
                s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
               .Select(z => new DVKTXetNghiem
               {
                   TenDichVuKyThuat = z.TenDichVu,
                   Id = z.Id,
                   GoiKhamSucKhoeId = z.GoiKhamSucKhoeId,
                   TrangThai = (int)z.TrangThai
               }).ToList();

            var ids = infoDichVuKyThuatXNs.Select(d => d.Id).ToList();

            var listPhienXetNghiemChiTietss = _phienXetNghiemChiTietRepository.TableNoTracking
                .Include(d => d.KetQuaXetNghiemChiTiets)
                .Where(s => ids.Contains(s.YeuCauDichVuKyThuatId)).ToList();


            // lấy info phiên kêt qua xet nghien
            if (infoDichVuKyThuatXNs != null)
            {
                foreach (var idDichVuKyThuat in infoDichVuKyThuatXNs)
                {
                    idDichVuKyThuat.DataKetQuaCanLamSang = listPhienXetNghiemChiTietss.Where(d => d.YeuCauDichVuKyThuatId == idDichVuKyThuat.Id)
                                                                                       .Select(v => new DataKetQuaCanLamSangVo
                                                                                       {
                                                                                           KetQuaXetNghiemChiTiets = v.KetQuaXetNghiemChiTiets.ToList(),
                                                                                           LanThucHien = v.LanThucHien,
                                                                                           KetLuan = v.KetLuan
                                                                                       }).OrderBy(s => s.LanThucHien)
                                                                                        .LastOrDefault();

                }
            }
            #endregion

            var dichVuXetNghiemKetNoiChiSos = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Select(s => new
            {
                Id = s.Id,
                DichVuXetNghiemId = s.DichVuXetNghiemId,
                MauMayXetNghiemId = s.MauMayXetNghiemId,
                TenKetNoi = s.TenKetNoi
            }).ToList();

            var data = new KetQuaKhamSucKhoeVo();
            var tableKham = "";
            var tableKyThuat = "";
            List<DanhSachDichVuKhamGrid> listDichVu = new List<DanhSachDichVuKhamGrid>();
            List<DanhSachDichVuKhamGrid> listDichVuCu = new List<DanhSachDichVuKhamGrid>();
            List<long> listDichVuKyThuatTheoGoi = new List<long>();
            if (thongTinKham != null)
            {

                // list theo yêu cầu tiếp nhận
                // CDHA TDCN
                if (infoDichVuKyThuatCDHATDCNs.Any())
                {

                    foreach (var itemDv in infoDichVuKyThuatCDHATDCNs)
                    {
                        DanhSachDichVuKhamGrid dvObject = new DanhSachDichVuKhamGrid();
                        dvObject.Id = itemDv.Id;
                        dvObject.NhomId = EnumNhomGoiDichVu.DichVuKyThuat;
                        dvObject.TenNhom = EnumNhomGoiDichVu.DichVuKyThuat.GetDescription();
                        dvObject.TenDichVu = itemDv.TenDichVuKyThuat;
                        dvObject.Type = EnumTypeLoaiChuyenKhoaEdit.Dvkt;
                        dvObject.NhomDichVuKyThuat = EnumTypeLoaiDichVuKyThuat.NhomDichVuKyThuatTDCNCDHA;
                        dvObject.GoiKhamSucKhoeId = itemDv.GoiKhamSucKhoeId;
                        dvObject.TrangThaiDVKham = itemDv.TrangThaiDVKham;

                        if (itemDv.DataKetQuaCanLamSang != null && itemDv.TrangThaiDVKham == 3) // != 1 => dịch vụ chưa thực hiện
                        {
                            var jsonOjbect = JsonConvert.DeserializeObject<DataCLS>(itemDv.DataKetQuaCanLamSang);

                            //dvObject.KetQuaDichVuDefault = jsonOjbect.KetQua;
                            //dvObject.KetQuaDichVu = jsonOjbect.KetQua;
                            var ketLuan = jsonOjbect.KetLuan;
                            if (!string.IsNullOrEmpty(ketLuan))
                            {
                                ketLuan = CommonHelper.StripHTML(Regex.Replace(ketLuan, "</p>(?![\n\r]+)", Environment.NewLine));
                                if (ketLuan.Length > 2 && ketLuan.Substring(ketLuan.Length - 2) == "\r\n")
                                {
                                    ketLuan = ketLuan.Remove(ketLuan.Length - 2);
                                }
                            }
                            dvObject.KetQuaDichVuDefault = ketLuan;
                            dvObject.KetQuaDichVu = ketLuan;
                        }

                        listDichVu.Add(dvObject);
                    }
                }

                // xét nghiệm
                if (infoDichVuKyThuatXNs.Any())
                {

                    foreach (var itemDv in infoDichVuKyThuatXNs)
                    {
                        ListKetQuaCLS listKetQuaOBJ = new ListKetQuaCLS();
                        if (itemDv.DataKetQuaCanLamSang != null)
                        {
                            // phiên xét nghiệm chi tiết orderby cuoi cung
                            if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets != null)
                            {
                                if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Any())
                                {
                                    if (itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Count == 1)
                                    {
                                        var itemGiaTriMin = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMin).First();
                                        var itemGiaTriMax = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriMax).First();
                                        var itemGTDuyet = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriDuyet).First();
                                        var itemGiaTriNhapTay = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriNhapTay).First();
                                        var itemGiaTriTuMay = itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Select(s => s.GiaTriTuMay).First();
                                        var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; //? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? string.Empty));
                                        bool KieuSo = false;
                                        if (value != null)
                                        {
                                            KieuSo = IsInt(value) ? true : false;
                                        }
                                        else
                                        {
                                            KieuSo = false;
                                        }
                                        if (KieuSo == true)
                                        {
                                            double ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                            double cSBTMin = 0;
                                            double cSBTMax = 0;

                                            if (itemGiaTriMin == null && itemGiaTriMax == null)
                                            {
                                                listKetQuaOBJ.KetQuaCLS = ketQua.ToString() + " ";
                                            }
                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                            if (itemGiaTriMin != null && itemGiaTriMax != null)
                                            {
                                                var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                if (!string.IsNullOrEmpty(min))
                                                {
                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Giảm)";
                                                }
                                                else
                                                {
                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                    if (!string.IsNullOrEmpty(max))
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Tăng)";
                                                    }
                                                    else
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "";
                                                    }
                                                }


                                            }
                                            if (itemGiaTriMin != null && itemGiaTriMax == null)
                                            {
                                                if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                {
                                                    var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                    if (!string.IsNullOrEmpty(min))
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "( Giảm)";
                                                    }
                                                    else
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                    }
                                                }
                                                else
                                                {
                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                }
                                            }
                                            if (itemGiaTriMin == null && itemGiaTriMax != null)
                                            {
                                                if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                {
                                                    var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                    if (!string.IsNullOrEmpty(max))
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ( Tăng)";
                                                    }
                                                    else
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                    }
                                                }
                                                else
                                                {
                                                    listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                }
                                            }
                                            // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                        }
                                        if (KieuSo == false)
                                        {
                                            listKetQuaOBJ.KetQuaCLS += value + " ";
                                        }
                                    }
                                    else
                                    {
                                        int itemCongKyTu = 0;
                                        foreach (var itemKetQuaListCon in itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.OrderByDescending(d => d.CapDichVu == 1 ? 1 : 0).ThenBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList())
                                        {
                                            var dichVuXetNghiemId = itemKetQuaListCon.DichVuXetNghiemId;
                                            var mauMayXetNghiemId = itemKetQuaListCon.MauMayXetNghiemId;
                                            var tenketQua = "";
                                            // nếu  mẫu máy xét nghiệm khác null => lấy ten dich vụ xét nghiệm trong db.DichVuXetNghiemKetNoiChiSo => field : TenKetNoi
                                            //itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId != null 
                                            if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId != null)
                                            {
                                                tenketQua = dichVuXetNghiemKetNoiChiSos.Where(s => s.Id == itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId.GetValueOrDefault()).Select(s => s.TenKetNoi).FirstOrDefault();

                                                listKetQuaOBJ.KetQuaCLS += tenketQua != null ? tenketQua + ": " : "";

                                                var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; //? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? string.Empty));
                                                double ketQua;
                                                bool KieuSo = false;
                                                if (value != null)
                                                {
                                                    KieuSo = IsInt(value) ? true : false;
                                                }
                                                else
                                                {
                                                    KieuSo = false;
                                                }
                                                if (KieuSo == true)
                                                {
                                                    double cSBTMin = 0;
                                                    double cSBTMax = 0;
                                                    ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                    if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                    if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                    {
                                                        var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                        if (!string.IsNullOrEmpty(min))
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Giảm)";
                                                        }
                                                        else
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Tăng)";
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "";
                                                            }
                                                        }


                                                    }
                                                    if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "( Giảm)";
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ( Tăng)";
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE

                                                }
                                                if (KieuSo == false)
                                                {
                                                    listKetQuaOBJ.KetQuaCLS += value + " ";

                                                }

                                                if (itemCongKyTu < itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Count())
                                                {
                                                    listKetQuaOBJ.KetQuaCLS += "; ";

                                                }

                                            }
                                            // nếu mẫu máy xét nghiệm == null => tên dịch vụ xét nghiệm trong db.KetQuaXetNghiemChiTiet => field :DichVuXetNghiemTen 
                                            //itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId == null
                                            if (itemKetQuaListCon.DichVuXetNghiemKetNoiChiSoId == null)
                                            {
                                                listKetQuaOBJ.KetQuaCLS += tenketQua != null ? itemKetQuaListCon.DichVuXetNghiemTen + ": " : "";

                                                var itemGiaTriMin = itemKetQuaListCon.GiaTriMin;
                                                var itemGiaTriMax = itemKetQuaListCon.GiaTriMax;
                                                var itemGTDuyet = itemKetQuaListCon.GiaTriDuyet;
                                                var itemGiaTriNhapTay = itemKetQuaListCon.GiaTriNhapTay;
                                                var itemGiaTriTuMay = itemKetQuaListCon.GiaTriTuMay;
                                                var value = !string.IsNullOrEmpty(itemGTDuyet) ? itemGTDuyet : !string.IsNullOrEmpty(itemGiaTriNhapTay) ? itemGiaTriNhapTay : !string.IsNullOrEmpty(itemGiaTriTuMay) ? itemGiaTriTuMay : string.Empty; //? (itemGiaTriNhapTay ?? (itemGiaTriTuMay ?? string.Empty));
                                                double ketQua;
                                                bool KieuSo = false;
                                                if (value != null)
                                                {
                                                    KieuSo = IsInt(value) ? true : false;
                                                }
                                                else
                                                {
                                                    KieuSo = false;
                                                }
                                                double cSBTMin = 0;
                                                int cSBTMax = 0;
                                                if (KieuSo == true)
                                                {
                                                    ketQua = value != null ? IsInt(value) ? Convert.ToDouble(value) : 0 : 0;
                                                    if (itemGiaTriMin == null && itemGiaTriMax == null)
                                                    {
                                                        listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";

                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE
                                                    if (itemGiaTriMin != null && itemGiaTriMax != null)
                                                    {
                                                        var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                        if (!string.IsNullOrEmpty(min))
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Giảm)";
                                                        }
                                                        else
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " (Tăng)";
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "";
                                                            }
                                                        }


                                                    }
                                                    if (itemGiaTriMin != null && itemGiaTriMax == null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMin))
                                                        {
                                                            var min = GetStatusForXetNghiemGiaTriMin(itemGiaTriMin, value);
                                                            if (!string.IsNullOrEmpty(min))
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + "( Giảm)";
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    if (itemGiaTriMin == null && itemGiaTriMax != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(itemGiaTriMax))
                                                        {
                                                            var max = GetStatusForXetNghiemGiaTriMax(itemGiaTriMax, value);
                                                            if (!string.IsNullOrEmpty(max))
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ( Tăng)";
                                                            }
                                                            else
                                                            {
                                                                listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            listKetQuaOBJ.KetQuaCLS += ketQua.ToString() + " ";
                                                        }
                                                    }
                                                    // BVHD-3922 [PHÁT SINH TRIỂN KHAI][XN] MÀN HÌNH KẾT QUẢ KHÁM SỨC KHỎE 
                                                }
                                                if (KieuSo == false)
                                                {
                                                    listKetQuaOBJ.KetQuaCLS += value + " ";

                                                }

                                                if (itemCongKyTu < itemDv.DataKetQuaCanLamSang.KetQuaXetNghiemChiTiets.Count())
                                                {
                                                    listKetQuaOBJ.KetQuaCLS += "; ";

                                                }
                                            }
                                            itemCongKyTu++;
                                        }
                                    }

                                }
                            }
                        }
                        listKetQuaOBJ.IsCheck = true;
                        if (!string.IsNullOrEmpty(listKetQuaOBJ.KetQuaCLS))
                        {
                            var plits = listKetQuaOBJ.KetQuaCLS.Split(";");
                            if (plits.Any())
                            {
                                listKetQuaOBJ.KetQuaCLS = "";
                                listKetQuaOBJ.KetQuaCLS = plits.Where(d => d != null && d != "" && d != " ").ToList().Distinct().Join(";");
                            }
                        }
                        listKetQua.Add(listKetQuaOBJ);
                    }
                }
            }

            foreach (var item in listDichVu)
            {
                ListKetQuaCLS listKetQuaOBJ = new ListKetQuaCLS();
                if (!string.IsNullOrEmpty(item.KetQuaDichVu))
                {
                    listKetQuaOBJ.KetQuaCLS = item.KetQuaDichVu;
                }
                listKetQuaOBJ.IsCheck = true;
                listKetQua.Add(listKetQuaOBJ);
            }


            return listKetQua; // trả về 1 list dịch vụ (dịch vụ khám , cls (dịch vụ kỹ thuật))
        }
        private List<KetQuaCLSGridChiTietVo> addDetailDataChild(List<KetQuaXetNghiemChiTiet> lstChiTietNhomConLai
        , List<KetQuaXetNghiemChiTiet> lstChiTietNhomChild, List<LookupItemVo> nhanViens, List<LookupItemVo> dichVuXetNghiems, List<LookupItemVo> mayXetNghiems
        , bool theFirst = false)
        {
            var result = new List<KetQuaCLSGridChiTietVo>();
            if (!lstChiTietNhomChild.Any() && theFirst != true) return null;

            //add root
            if (theFirst)
            {
                var lstParent = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == null).ToList();
                foreach (var parent in lstParent)
                {
                    var ketQua = new KetQuaCLSGridChiTietVo
                    {
                        Id = parent.Id,
                        TenDichVu = dichVuXetNghiems.FirstOrDefault(o=>o.KeyId == parent.DichVuXetNghiemId)?.DisplayName,
                        KetQuaCu = parent.GiaTriCu,
                        KetQuaMoi = !string.IsNullOrEmpty(parent.GiaTriDuyet) ? parent.GiaTriDuyet : !string.IsNullOrEmpty(parent.GiaTriNhapTay) ? parent.GiaTriNhapTay : !string.IsNullOrEmpty(parent.GiaTriTuMay) ? parent.GiaTriTuMay : string.Empty,
                        CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),//(parent.GiaTriMin != null || parent.GiaTriMax != null) ? parent.GiaTriMin + " - " + parent.GiaTriMax : "",
                        DonVi = parent.DonVi,
                        MayXN = mayXetNghiems.FirstOrDefault(o => o.KeyId == parent.MayXetNghiemId)?.DisplayName,
                        NguoiDuyet = nhanViens.FirstOrDefault(o => o.KeyId == parent.NhanVienDuyetId)?.DisplayName,
                        NgayDuyet = parent.ThoiDiemDuyetKetQua != null ? parent.ThoiDiemDuyetKetQua.Value.ApplyFormatDateTimeSACH() : null,
                        //structure tree                       
                        IsRoot = lstChiTietNhomConLai.Any(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId),
                        IsParent = lstChiTietNhomConLai.Any(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId),
                        IsBold = parent.ToDamGiaTri,
                        IsCheck = true
                    };
                    var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                    ketQua.Items = addDetailDataChild(lstChiTietNhomConLai, lstChiTietChild, nhanViens, dichVuXetNghiems, mayXetNghiems);
                    //
                    result.Add(ketQua);
                }
            }
            else
            {
                if (lstChiTietNhomChild != null)
                {
                    foreach (var parent in lstChiTietNhomChild)
                    {
                        var ketQua = new KetQuaCLSGridChiTietVo
                        {
                            Id = parent.Id,
                            TenDichVu = dichVuXetNghiems.FirstOrDefault(o => o.KeyId == parent.DichVuXetNghiemId)?.DisplayName,
                            KetQuaCu = parent.GiaTriCu,
                            KetQuaMoi = !string.IsNullOrEmpty(parent.GiaTriDuyet) ? parent.GiaTriDuyet : !string.IsNullOrEmpty(parent.GiaTriNhapTay) ? parent.GiaTriNhapTay : !string.IsNullOrEmpty(parent.GiaTriTuMay) ? parent.GiaTriTuMay : string.Empty,
                            CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),//(parent.GiaTriMin != null || parent.GiaTriMax != null) ? parent.GiaTriMin + " - " + parent.GiaTriMax : "",
                            DonVi = parent.DonVi,
                            MayXN = mayXetNghiems.FirstOrDefault(o => o.KeyId == parent.MayXetNghiemId)?.DisplayName,
                            NguoiDuyet = nhanViens.FirstOrDefault(o => o.KeyId == parent.NhanVienDuyetId)?.DisplayName,
                            NgayDuyet = parent.ThoiDiemDuyetKetQua != null ? parent.ThoiDiemDuyetKetQua.Value.ApplyFormatDateTimeSACH() : null,
                            //structure tree
                            IsRoot = false,
                            IsParent = lstChiTietNhomConLai.Any(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId),
                            IsBold = parent.ToDamGiaTri,
                            IsCheck = true
                        };
                        var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                        ketQua.Items = addDetailDataChild(lstChiTietNhomConLai, lstChiTietChild, nhanViens, dichVuXetNghiems, mayXetNghiems);
                        //
                        result.Add(ketQua);
                    }
                }
            }

            return result;
        }
        #region ds đã kết luận
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachKetLuanCLSKhamSucKhoe(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<KetLuanKhamSucKhoeDoanVo>().ToArray(), TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var queryString = JsonConvert.DeserializeObject<KetLuanKhamSucKhoeDoanVo>(queryInfo.AdditionalSearchString);
            var query = BaseRepository.TableNoTracking
                                   .Where(p => ( p.HopDongKhamSucKhoeNhanVien != null && (p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                                                                                          && p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId && p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.DaKetThuc == true
                                                                                          && p.BenhNhanId != null ))
                         ).Select(s => new KetLuanKhamSucKhoeDoanVo
                         {
                             Id = s.Id,
                             YeuCauTiepNhanId = s.Id,
                             CongTyKhamSucKhoeId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                             HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                             MaTN = s.MaYeuCauTiepNhan,
                             MaBN = s.BenhNhan.MaBN,
                             MaNhanVien = s.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                             HoTen = s.HoTen,
                             TenNgheNghiep = s.NgheNghiep.Ten,
                             GioiTinh = s.GioiTinh,
                             NamSinh = s.NamSinh,
                             SoDienThoai = s.SoDienThoai,
                             Email = s.Email,
                             SoChungMinhThu = s.SoChungMinhThu,
                             TenDanToc = s.DanToc.Ten,
                             TenTinhThanh = s.TinhThanh.Ten,
                             NhomDoiTuongKhamSucKhoe = s.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe,
                             KSKKetLuanPhanLoaiSucKhoe = s.KSKKetLuanPhanLoaiSucKhoe,
                             //DichVuDaThucHien = s.YeuCauDichVuKyThuats.Where(ac => ac.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId
                             //                                                                   && ac.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                             //                                                                   && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy ).Count(),
                             //TongDichVu = s.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Count(),
                             NgayKetThuc = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc,
                             LaHopDongDaKetLuan = s.KSKKetQuaCanLamSang != null && s.KSKDanhGiaCanLamSang != null ? true : false,
                             TrangThaiYeuCauTiepNhan = s.TrangThaiYeuCauTiepNhan,
                             GoiDichVuId = s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId,
                         });
           
            if (queryString.ChuaKetLuan != true && queryString.DaKetLuan == true)
            {
                query = query.Where(p => p.TinhTrang == 1);
            }
            else if (queryString.ChuaKetLuan == true && queryString.DaKetLuan != true)
            {
                query = query.Where(p => p.TinhTrang == 0);
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                   g => g.MaTN,
                   g => g.MaBN,
                   g => g.MaNhanVien,
                   g => g.HoTen,
                   g => g.TenNgheNghiep,
                   g => g.NamSinh.ToString(),
                   g => g.SoDienThoai,
                   g => g.Email,
                   g => g.SoChungMinhThu,
                   g => g.TenTinhThanh,
                   g => g.NhomDoiTuongKhamSucKhoe
               );
            }
            var dsKetLuans = query.ToList();
            var ds = new List<KetLuanKhamSucKhoeDoanVo>();
            if (dsKetLuans.Any())
            {
                var entityDVKT = _goiKhamSucKhoeRepository.TableNoTracking
                    .Where(d=> d.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                               && d.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId &&
                               d.HopDongKhamSucKhoe.DaKetThuc == true
                               )
                   .SelectMany(s => s.YeuCauDichVuKyThuats)
                   .Select(s => new {
                       GoiKhamSucKhoeId = s.GoiKhamSucKhoeId,
                       YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                       TrangThai = s.TrangThai,
                       LoaiDichVuKyThuat = s.LoaiDichVuKyThuat
                   }).Where(d=>
                            d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ||
                            d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang ||
                            d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                   .ToList();

                foreach (var item in dsKetLuans)
                {
                    // dịch vu đã thực hiện 
                    var dichVuDaThucHien = entityDVKT.Where(ac => ac.GoiKhamSucKhoeId == item.GoiDichVuId
                                                                  && ac.YeuCauTiepNhanId == item.YeuCauTiepNhanId
                                                                  && ac.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                  && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();

                    var dichVuDaThucHiencGoiChung = entityDVKT.Where(ac => ac.GoiKhamSucKhoeId != item.GoiDichVuId
                                                                  && ac.YeuCauTiepNhanId == item.YeuCauTiepNhanId
                                                                  && ac.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                  && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();

                    item.DichVuDaThucHien = dichVuDaThucHien.Count() + dichVuDaThucHiencGoiChung.Count();

                    // tổng dịch vụ
                    var tongDichVu = entityDVKT.Where(ac => ac.GoiKhamSucKhoeId == item.GoiDichVuId
                                                            && ac.YeuCauTiepNhanId == item.YeuCauTiepNhanId
                                                             && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();

                    var tongDichVuKhacGoiRieng = entityDVKT.Where(ac => ac.GoiKhamSucKhoeId != item.GoiDichVuId
                                                           && ac.YeuCauTiepNhanId == item.YeuCauTiepNhanId
                                                            && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();

                    item.TongDichVu = tongDichVu.Count() + tongDichVuKhacGoiRieng.Count();
                    ds.Add(item);
                }
            }
            var dataOrderBy = ds.AsQueryable().OrderBy(queryInfo.SortString);
            var dsChuaDaThucHien = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();

            return new GridDataSource { Data = dsChuaDaThucHien, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachKetLuanCLSKhamSucKhoe(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;

            }
            var queryString = JsonConvert.DeserializeObject<KetLuanKhamSucKhoeDoanVo>(queryInfo.AdditionalSearchString);

            var query = BaseRepository.TableNoTracking
                                    .Where(p => (p.HopDongKhamSucKhoeNhanVien != null && (p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                                                                                           && p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId && p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.DaKetThuc == true
                                                                                           && p.BenhNhanId != null ))
                          ).Select(s => new KetLuanKhamSucKhoeDoanVo
                          {
                              Id = s.Id,
                              YeuCauTiepNhanId = s.Id,
                              CongTyKhamSucKhoeId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                              HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                              MaTN = s.MaYeuCauTiepNhan,
                              MaBN = s.BenhNhan.MaBN,
                              MaNhanVien = s.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                              HoTen = s.HoTen,
                              TenNgheNghiep = s.NgheNghiep.Ten,
                              GioiTinh = s.GioiTinh,
                              NamSinh = s.NamSinh,
                              SoDienThoai = s.SoDienThoai,
                              Email = s.Email,
                              SoChungMinhThu = s.SoChungMinhThu,
                              TenDanToc = s.DanToc.Ten,
                              TenTinhThanh = s.TinhThanh.Ten,
                              NhomDoiTuongKhamSucKhoe = s.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe,
                              KSKKetLuanPhanLoaiSucKhoe = s.KSKKetLuanPhanLoaiSucKhoe,
                              //DichVuDaThucHien = s.YeuCauDichVuKyThuats.Where(ac => ac.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId
                              //                                                                   && ac.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                              //                                                                   && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy ).Count(),
                              //TongDichVu= s.YeuCauDichVuKyThuats.Where(yc => yc.GoiKhamSucKhoeId == s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId && yc.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Count(),
                              NgayKetThuc = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc,
                              LaHopDongDaKetLuan = s.KSKKetQuaCanLamSang != null && s.KSKDanhGiaCanLamSang != null ? true : false,
                              TrangThaiYeuCauTiepNhan = s.TrangThaiYeuCauTiepNhan,
                              GoiDichVuId = s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId,
                          });

           
            if (queryString.ChuaKetLuan != true && queryString.DaKetLuan == true)
            {
                query = query.Where(p => p.TinhTrang == 1);
            }
            else if (queryString.ChuaKetLuan == true && queryString.DaKetLuan != true)
            {
                query = query.Where(p => p.TinhTrang == 0);
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                   g => g.MaTN,
                   g => g.MaBN,
                   g => g.MaNhanVien,
                   g => g.HoTen,
                   g => g.TenNgheNghiep,
                   g => g.NamSinh.ToString(),
                   g => g.SoDienThoai,
                   g => g.Email,
                   g => g.SoChungMinhThu,
                   g => g.TenTinhThanh,
                   g => g.NhomDoiTuongKhamSucKhoe
               );
            }
           
            var dataOrderBy = query.AsQueryable();
            var countTask = dataOrderBy.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }
        #endregion
        #region ds chưa kết luận
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachChuaKetLuanCLSKhamSucKhoe(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new GridDataSource { Data = new List<KetLuanKhamSucKhoeDoanVo>().ToArray(), TotalRowCount = 0 };
            }
            BuildDefaultSortExpression(queryInfo);
            var queryString = JsonConvert.DeserializeObject<KetLuanKhamSucKhoeDoanVo>(queryInfo.AdditionalSearchString);
            var query = BaseRepository.TableNoTracking
                                   .Where(p => (p.HopDongKhamSucKhoeNhanVien != null && (p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                                                                                          && p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId
                                                                                          && p.BenhNhanId != null ))
                         ).Select(s => new KetLuanKhamSucKhoeDoanVo
                         {
                             Id = s.Id,
                             YeuCauTiepNhanId = s.Id,
                             CongTyKhamSucKhoeId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                             HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                             MaTN = s.MaYeuCauTiepNhan,
                             MaBN = s.BenhNhan.MaBN,
                             MaNhanVien = s.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                             HoTen = s.HoTen,
                             TenNgheNghiep = s.NgheNghiep.Ten,
                             GioiTinh = s.GioiTinh,
                             NamSinh = s.NamSinh,
                             SoDienThoai = s.SoDienThoai,
                             Email = s.Email,
                             SoChungMinhThu = s.SoChungMinhThu,
                             TenDanToc = s.DanToc.Ten,
                             TenTinhThanh = s.TinhThanh.Ten,
                             NhomDoiTuongKhamSucKhoe = s.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe,
                             KSKKetLuanPhanLoaiSucKhoe = s.KSKKetLuanPhanLoaiSucKhoe,
                             NgayKetThuc = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc,
                             LaHopDongDaKetLuan = s.KSKKetQuaCanLamSang != null && s.KSKDanhGiaCanLamSang != null ? true : false,
                             TrangThaiYeuCauTiepNhan = s.TrangThaiYeuCauTiepNhan,
                             GoiDichVuId = s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId,
                         });



            if (queryString.ChuaKetLuan != true && queryString.DaKetLuan == true)
            {
                query = query.Where(p => p.TinhTrang == 1);
            }
            else if (queryString.ChuaKetLuan == true && queryString.DaKetLuan != true)
            {
                query = query.Where(p => p.TinhTrang == 0);
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                   g => g.MaTN,
                   g => g.MaBN,
                   g => g.MaNhanVien,
                   g => g.HoTen,
                   g => g.TenNgheNghiep,
                   g => g.NamSinh.ToString(),
                   g => g.SoDienThoai,
                   g => g.Email,
                   g => g.SoChungMinhThu,
                   g => g.TenTinhThanh,
                   g => g.NhomDoiTuongKhamSucKhoe
               );
            }
            var dsChuaKetLuans = query.ToList();
            var ds = new List<KetLuanKhamSucKhoeDoanVo>();
            if(dsChuaKetLuans.Any())
            {
                var entityDVKT = _goiKhamSucKhoeRepository.TableNoTracking
                   .Where(d => d.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                              && d.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId 
                              )
                  .SelectMany(s => s.YeuCauDichVuKyThuats)
                  .Select(s => new {
                      GoiKhamSucKhoeId = s.GoiKhamSucKhoeId,
                      YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                      TrangThai = s.TrangThai,
                      LoaiDichVuKyThuat = s.LoaiDichVuKyThuat
                  }).Where(d =>
                           d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ||
                           d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang ||
                           d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                  .ToList();
                foreach (var item in dsChuaKetLuans)
                {
                    
                    // dịch vu đã thực hiện 
                    var dichVuDaThucHien = entityDVKT.Where(ac => ac.GoiKhamSucKhoeId == item.GoiDichVuId
                                                                  && ac.YeuCauTiepNhanId == item.YeuCauTiepNhanId
                                                                  && ac.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                  && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();

                    var dichVuDaThucHiencGoiChung = entityDVKT.Where(ac => ac.GoiKhamSucKhoeId != item.GoiDichVuId
                                                                  && ac.YeuCauTiepNhanId == item.YeuCauTiepNhanId
                                                                  && ac.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                  && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();

                    item.DichVuDaThucHien = dichVuDaThucHien.Count() + dichVuDaThucHiencGoiChung.Count();

                    // tổng dịch vụ
                    var tongDichVu = entityDVKT.Where(ac => ac.GoiKhamSucKhoeId == item.GoiDichVuId
                                                            && ac.YeuCauTiepNhanId == item.YeuCauTiepNhanId
                                                             && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();

                    var tongDichVuKhacGoiRieng = entityDVKT.Where(ac => ac.GoiKhamSucKhoeId != item.GoiDichVuId
                                                           && ac.YeuCauTiepNhanId == item.YeuCauTiepNhanId
                                                            && ac.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList();

                    item.TongDichVu = tongDichVu.Count() + tongDichVuKhacGoiRieng.Count();
                    ds.Add(item);
                }
            }

            var dataOrderBy = ds.AsQueryable().OrderBy(queryInfo.SortString);
            var dsChuaThucHien = dataOrderBy.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = dataOrderBy.Count();

            return new GridDataSource { Data = dsChuaThucHien, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachChuaKetLuanCLSKhamSucKhoe(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;

            }
            var queryString = JsonConvert.DeserializeObject<KetLuanKhamSucKhoeDoanVo>(queryInfo.AdditionalSearchString);

            var query = BaseRepository.TableNoTracking
                                    .Where(p => (p.HopDongKhamSucKhoeNhanVien != null && (p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == queryString.CongTyKhamSucKhoeId
                                                                                           && p.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == queryString.HopDongKhamSucKhoeId 
                                                                                           && p.BenhNhanId != null))
                          ).Select(s => new KetLuanKhamSucKhoeDoanVo
                          {
                              Id = s.Id,
                              YeuCauTiepNhanId = s.Id,
                              CongTyKhamSucKhoeId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId,
                              HopDongKhamSucKhoeId = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId,
                              MaTN = s.MaYeuCauTiepNhan,
                              MaBN = s.BenhNhan.MaBN,
                              MaNhanVien = s.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                              HoTen = s.HoTen,
                              TenNgheNghiep = s.NgheNghiep.Ten,
                              GioiTinh = s.GioiTinh,
                              NamSinh = s.NamSinh,
                              SoDienThoai = s.SoDienThoai,
                              Email = s.Email,
                              SoChungMinhThu = s.SoChungMinhThu,
                              TenDanToc = s.DanToc.Ten,
                              TenTinhThanh = s.TinhThanh.Ten,
                              NhomDoiTuongKhamSucKhoe = s.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe,
                              KSKKetLuanPhanLoaiSucKhoe = s.KSKKetLuanPhanLoaiSucKhoe,
                              NgayKetThuc = s.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.NgayKetThuc,
                              LaHopDongDaKetLuan = s.KSKKetQuaCanLamSang != null && s.KSKDanhGiaCanLamSang != null ? true : false,
                              TrangThaiYeuCauTiepNhan = s.TrangThaiYeuCauTiepNhan,
                              GoiDichVuId = s.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId,
                          });

           
            if (queryString.ChuaKetLuan != true && queryString.DaKetLuan == true)
            {
                query = query.Where(p => p.TinhTrang == 1);
            }
            else if (queryString.ChuaKetLuan == true && queryString.DaKetLuan != true)
            {
                query = query.Where(p => p.TinhTrang == 0);
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                   g => g.MaTN,
                   g => g.MaBN,
                   g => g.MaNhanVien,
                   g => g.HoTen,
                   g => g.TenNgheNghiep,
                   g => g.NamSinh.ToString(),
                   g => g.SoDienThoai,
                   g => g.Email,
                   g => g.SoChungMinhThu,
                   g => g.TenTinhThanh,
                   g => g.NhomDoiTuongKhamSucKhoe
               );
            }
            var dataOrderBy = query.AsQueryable();
            var countTask = dataOrderBy.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }
        #endregion
        #region check họp đồng kết thúc
        public bool CheckHopDongKetThuc(ThongTinCheckHopDong checkHopDong)
        {
            var query = _hopDongKhamSucKhoeRepository.TableNoTracking.Where(s => s.Id == checkHopDong.HopDongId && s.CongTyKhamSucKhoeId == checkHopDong.CongTyId);
            if(query.Any())
            {
                return query.Any(s => s.DaKetThuc == true) ? true : false;
            }
            return false;
        }
        #endregion


    }
}
