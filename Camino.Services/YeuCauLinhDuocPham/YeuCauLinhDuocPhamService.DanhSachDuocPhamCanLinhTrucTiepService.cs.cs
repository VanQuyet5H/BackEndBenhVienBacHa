using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Newtonsoft.Json;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.Grid;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DanhSachDuocPhamCanBu;
using Camino.Core.Domain.ValueObject.DanhSachDuocPhamCanLinhTrucTiep;
using System.Diagnostics;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial class YeuCauLinhDuocPhamService
    {

        public GridDataSource GetDanhSachDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo)
        {
            var list = GetDanhSachDuocPhamCanLinhTrucTiep(queryInfo);
            return new GridDataSource { Data = list.ToArray(), TotalRowCount = list.Count() };
        }

        private List<YeuCauLinhDuocPhamTrucTiepGridChildVo> GetDanhSachDuocPhamCanLinhTrucTiep(QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanLinhTrucTiepQueryInfo = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ?
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanLinhTrucTiepQueryInfo>(queryInfo.AdditionalSearchString) : new DanhSachDuocPhamCanLinhTrucTiepQueryInfo();

            var list = new List<YeuCauLinhDuocPhamTrucTiepGridChildVo>();
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }


            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled!=true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();
            
            //Lấy tất cả YCTN cần lĩnh trực tiếp về khoa
            var queryYeuCauDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking.Include(o=>o.DuocPhamBenhVien)
                .ThenInclude(o=>o.NhapKhoDuocPhamChiTiets).ThenInclude(p => p.NhapKhoDuocPhams).Include(o=>o.KhoLinh)
                .Include(o=>o.KhoLinh)
            .Where(x => (danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId==null || danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId==0 || x.KhoLinhId== danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId)
                        &&phongLinhVes.Any(o=>o.KeyId== x.NoiChiDinhId)
                        && x.YeuCauLinhDuocPhamId == null
                       
                        && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                        && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy).Select(o=>o);

            var yeuCauTiepNhanCoLinhTrucTieps= queryYeuCauDuocPhamBenhViens.Where(d=>d.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2).Select(s => new YeuCauLinhDuocPhamTrucTiepGridChildVo
            {
                Id = s.Id,
                MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                HoTen = s.YeuCauTiepNhan.HoTen,
                SoLuong = s.SoLuong,
                DichVuKham = s.YeuCauKhamBenh.TenDichVu,
                BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                KhoLinhId = s.KhoLinhId,
                KhoLinh = s.KhoLinh.Ten
            })
            .GroupBy(x => new
            {
                x.MaYeuCauTiepNhan,
                x.HoTen,
                x.KhoLinhId
            })
            .Select(item => new YeuCauLinhDuocPhamTrucTiepGridChildVo()
            {

                Id = item.First().Id,
                MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                MaBenhNhan = item.First().MaBenhNhan,
                HoTen = item.First().HoTen,
                SoLuong = item.Sum(a => a.SoLuong),
                YeuCauTiepNhanId = item.First().YeuCauTiepNhanId,
                KhoLinhId = item.First().KhoLinhId,
                KhoLinh = item.First().KhoLinh
            })
            .OrderBy(d => d.MaBenhNhan).Distinct().ToList();

            


            //Kiễm tra xem mỗi YCTN có thuốc tồn ở khoa nào
            var dsMaYeuCauTiepNhan = yeuCauTiepNhanCoLinhTrucTieps.Select(o => o.MaYeuCauTiepNhan).ToList();
            if (dsMaYeuCauTiepNhan.Any())
            {
                var listAllYCDPBV = queryYeuCauDuocPhamBenhViens.Where(o => dsMaYeuCauTiepNhan.Contains(o.YeuCauTiepNhan.MaYeuCauTiepNhan))
                               
                       .Select(item => new
                       {
                           Id = item.Id,
                           MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                           DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                           LaDuocPhamBHYT = item.LaDuocPhamBHYT,
                           SoLuong = item.SoLuong,
                           KhoLinhId = item.KhoLinhId,
                           KhoLinh = item.KhoLinh.Ten,
                       }).ToList();
                if (listAllYCDPBV != null)
                {


                    var dsDuocPhamBenhVienId = listAllYCDPBV.Select(o => o.DuocPhamBenhVienId).Distinct().ToList();
                    var dsKhoId = listAllYCDPBV.Select(o => o.KhoLinhId).Distinct().ToList();

                    var dsDuocPhamTrongKhoCanKiemTra = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                        .Where(nkct => dsDuocPhamBenhVienId.Contains(nkct.DuocPhamBenhVienId) &&
                                dsKhoId.Contains(nkct.NhapKhoDuocPhams.KhoId) && nkct.SoLuongNhap > nkct.SoLuongDaXuat && nkct.HanSuDung >= DateTime.Now)
                        .Select(nkct => new { nkct.DuocPhamBenhVienId, nkct.NhapKhoDuocPhams.KhoId, nkct.LaDuocPhamBHYT, nkct.SoLuongNhap, nkct.SoLuongDaXuat }).ToList();


                    foreach (var yeuCauTiepNhanCoLinhTrucTiep in yeuCauTiepNhanCoLinhTrucTieps)
                    {
                        var listYCDPBV = listAllYCDPBV.Where(o => o.MaYeuCauTiepNhan == yeuCauTiepNhanCoLinhTrucTiep.MaYeuCauTiepNhan && o.KhoLinhId == yeuCauTiepNhanCoLinhTrucTiep.KhoLinhId);
                        var tonKhongDu = false;

                        foreach (var yCDPBV in listYCDPBV)
                        {

                            var tonkho = dsDuocPhamTrongKhoCanKiemTra
                                .Where(nkct => nkct.DuocPhamBenhVienId == yCDPBV.DuocPhamBenhVienId &&
                                    nkct.KhoId == yCDPBV.KhoLinhId && nkct.LaDuocPhamBHYT == yCDPBV.LaDuocPhamBHYT)
                                .Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat);

                            if (tonkho <= 0 || yCDPBV.SoLuong > tonkho)
                            {
                                tonKhongDu = true;
                            }

                        }
                        if (tonKhongDu == true)
                        {
                            yeuCauTiepNhanCoLinhTrucTiep.TinhTrangTon = false;
                        }
                        else
                        {
                            yeuCauTiepNhanCoLinhTrucTiep.TinhTrangTon = true;
                        }
                    }
                }
            }

            //if (yeuCauTiepNhanCoLinhTrucTieps.Any())
            //{
            //    var stopwatch = new Stopwatch();
            //    stopwatch.Start();
                
            //    foreach (var yeuCauTiepNhanCoLinhTrucTiep in yeuCauTiepNhanCoLinhTrucTieps)
            //    {
            //        //Lấy ra tất cả thuốc của YCTN
            //       var listYCDPBV = queryYeuCauDuocPhamBenhViens.Where(o => o.YeuCauTiepNhan.MaYeuCauTiepNhan == yeuCauTiepNhanCoLinhTrucTiep.MaYeuCauTiepNhan)
            //                    .GroupBy(x => new
            //                    {
            //                        x.DuocPhamBenhVienId
            //                    })
            //           .Select(item => new
            //           {
            //               Id= item.First().Id,
            //               DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
            //               LaDuocPhamBHYT = item.First().LaDuocPhamBHYT,
            //               SoLuong = item.Sum(a => a.SoLuong),
            //               KhoLinhId = item.First().KhoLinhId,
            //               KhoLinh = item.First().KhoLinh.Ten,
            //           }).ToList();
                
            //        if (listYCDPBV != null)
            //        {
            //            var tonKhongDu = false;
            //            foreach (var yCDPBV in listYCDPBV)
            //            {
            //                var tonkho = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
            //                    .Where(nkct => nkct.DuocPhamBenhVienId == yCDPBV.DuocPhamBenhVienId &&
            //                        nkct.NhapKhoDuocPhams.KhoId == yCDPBV.KhoLinhId && nkct.LaDuocPhamBHYT == yCDPBV.LaDuocPhamBHYT &&
            //                        nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat);
            //                if (tonkho <= 0 || yCDPBV.SoLuong>tonkho)
            //                {
            //                    tonKhongDu = true;
            //                }

            //            }
            //            if (tonKhongDu == true)
            //            {
            //                yeuCauTiepNhanCoLinhTrucTiep.TinhTrangTon = false;
            //            }
            //            else
            //            {
            //                yeuCauTiepNhanCoLinhTrucTiep.TinhTrangTon = true;
            //            }
            //        }

            //    }
            //    stopwatch.Stop();
            //    var stringst = stopwatch.ElapsedTicks.ToString();
            //}
            var result = yeuCauTiepNhanCoLinhTrucTieps.Union(list).Where(x=> (danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId == null || x.KhoLinhId == danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId));
            return result.ToList();
        }
        private List<DanhSachDuocPhamCanLinhTrucTiepGridVo> GetDanhSachDuocPhamCanLinhTrucTiep1(QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanLinhTrucTiepQueryInfo = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ?
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanLinhTrucTiepQueryInfo>(queryInfo.AdditionalSearchString) : new DanhSachDuocPhamCanLinhTrucTiepQueryInfo();

            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _khoaPhongNhanVienRepository.TableNoTracking
                .Where(p => p.NhanVienId == userCurrentId && p.PhongBenhVienId != null &&
                            (danhSachDuocPhamCanLinhTrucTiepQueryInfo.PhongLinhVeId == null || p.PhongBenhVienId == danhSachDuocPhamCanLinhTrucTiepQueryInfo.PhongLinhVeId))
                .Select(s => new LookupItemVo
                {
                    KeyId = (long)s.PhongBenhVienId,
                    DisplayName = s.PhongBenhVien.Ten
                })
                .Union(_khoaPhongNhanVienRepository.TableNoTracking
                    .Where(p => p.NhanVienId == userCurrentId && p.PhongBenhVienId == null)
                    .SelectMany(s => s.KhoaPhong.PhongBenhViens
                        .Where(p => danhSachDuocPhamCanLinhTrucTiepQueryInfo.PhongLinhVeId == null || p.Id == danhSachDuocPhamCanLinhTrucTiepQueryInfo.PhongLinhVeId).Select(o => new LookupItemVo
                        {
                            KeyId = (long)o.Id,
                            DisplayName = o.Ten
                        })))
                .OrderBy(o => o.DisplayName).ToList();
            var list = new List<DanhSachDuocPhamCanLinhTrucTiepGridVo>();
            foreach (var phongLinhVe in phongLinhVes)
            {
                //Kiểm tra xem có dược phẩm nào cần bù trong kho lĩnh về này hay ko?
                var yeuCauDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan &&
                                o.YeuCauLinhDuocPhamId == null && o.NoiChiDinhId == phongLinhVe.KeyId
                                && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                    .Select(o => o);
                if (yeuCauDuocPhamBenhViens.Any())
                {
                    //Kiểm tra xem có tồn tại dược phẩm nào trong kho lĩnh về này còn tồn kho trong kho lĩnh từ hay ko?
                    var khoLinhTus = _khoRepository.TableNoTracking
                        .Where(p => (danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId == null || (danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId != null && p.Id == danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId))
                        && p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2 && p.NhapKhoDuocPhams
                                        .Where(o => o.DaHet != true)
                                        .SelectMany(o => o.NhapKhoDuocPhamChiTiets).Any(ct =>
                                            ct.SoLuongDaXuat < ct.SoLuongNhap && ct.HanSuDung >= DateTime.Now &&
                                            yeuCauDuocPhamBenhViens.Any(o =>
                                                o.DuocPhamBenhVienId == ct.DuocPhamBenhVienId &&
                                                o.LaDuocPhamBHYT == ct.LaDuocPhamBHYT && o.KhoLinhId == p.Id)));
                    if (khoLinhTus.Any())
                    {
                        foreach (var khoLinhTu in khoLinhTus)
                        {
                            list.Add(new DanhSachDuocPhamCanLinhTrucTiepGridVo
                            {
                                KhoLinhId = khoLinhTu.Id,
                                KhoLinh = khoLinhTu.Ten,
                                PhongLinhVeId = phongLinhVe.KeyId,
                                PhongLinhVe = phongLinhVe.DisplayName
                            });
                        }
                    }
                    else
                    {
                        if (danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId == null)
                        {
                            list.Add(new DanhSachDuocPhamCanLinhTrucTiepGridVo
                            {
                                KhoLinhId = 0,
                                KhoLinh = "---Không có kho tồn---",
                                PhongLinhVeId = phongLinhVe.KeyId,
                                PhongLinhVe = phongLinhVe.DisplayName
                            });
                        }
                    }
                }
            }
            return list;
        }

        public async Task<GridDataSource> GetDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanLinhTrucTiepQueryInfo =
                 JsonConvert.DeserializeObject<DanhSachDuocPhamCanLinhTrucTiepQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.NoiChiDinhId == danhSachDuocPhamCanLinhTrucTiepQueryInfo.PhongLinhVeId
                            && x.YeuCauLinhDuocPhamId == null 
                            && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                            && x.KhoLinhId == danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId
                            && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                 .Select(s => new YeuCauLinhDuocPhamTrucTiepGridChildVo
                 {
                     Id = s.Id,
                     MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                     MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                     HoTen = s.YeuCauTiepNhan.HoTen,
                     SoLuong = s.SoLuong,
                     DichVuKham = s.YeuCauKhamBenh.TenDichVu,
                     BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                     NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                     YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                     KhoLinhId = danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId
                 })
                .GroupBy(x => new
                {
                    x.MaYeuCauTiepNhan,
                    x.HoTen
                })
                .Select(item => new YeuCauLinhDuocPhamTrucTiepGridChildVo()
                {

                    Id = item.First().Id,
                    MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                    MaBenhNhan = item.First().MaBenhNhan,
                    HoTen = item.First().HoTen,
                    SoLuong = item.Sum(a => a.SoLuong),
                    KhoLinhId = danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId,
                    PhongLinhVeId = danhSachDuocPhamCanLinhTrucTiepQueryInfo.PhongLinhVeId,
                    YeuCauTiepNhanId = item.First().YeuCauTiepNhanId
                })
                .Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageDanhSachChiTietDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo)
        {
            var danhSachDuocPhamCanLinhTrucTiepQueryInfo =
                 JsonConvert.DeserializeObject<DanhSachDuocPhamCanLinhTrucTiepQueryInfo>(queryInfo.AdditionalSearchString);
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
               .Where(x => x.NoiChiDinhId == danhSachDuocPhamCanLinhTrucTiepQueryInfo.PhongLinhVeId
                           && x.YeuCauLinhDuocPhamId == null 
                           && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                           && x.KhoLinhId == danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId
                           && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                .Select(s => new YeuCauLinhDuocPhamTrucTiepGridChildVo
                {
                    Id = s.Id,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    SoLuong = s.SoLuong,
                    DichVuKham = s.YeuCauKhamBenh.TenDichVu,
                    BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId
                })
               .GroupBy(x => new
               {
                   x.MaYeuCauTiepNhan,
                   x.HoTen
               })
               .Select(item => new YeuCauLinhDuocPhamTrucTiepGridChildVo()
               {

                   Id = item.First().Id,
                   MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                   MaBenhNhan = item.First().MaBenhNhan,
                   HoTen = item.First().HoTen,
                   SoLuong = item.Sum(a => a.SoLuong),
                   KhoLinhId = danhSachDuocPhamCanLinhTrucTiepQueryInfo.KhoLinhId,
                   PhongLinhVeId = danhSachDuocPhamCanLinhTrucTiepQueryInfo.PhongLinhVeId,
                   YeuCauTiepNhanId = item.First().YeuCauTiepNhanId
               })
               .Distinct();

            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }
        public async Task<GridDataSource> GetDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo)
        {

            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }


            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();
            var danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo =
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                 .Where(x => phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) && 
                 x.YeuCauLinhDuocPhamId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                             && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                             && (danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo.KhoLinhId==null || danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo.KhoLinhId == 0|| x.KhoLinhId == danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo.KhoLinhId)
                             && x.YeuCauTiepNhanId == danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo.YeuCauTiepNhanId
                             && x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2)
                 .Select(item => new DSCanLinhDuocPhamTrucTiepGridParentVo()
                 {
                     Id = item.Id,
                     DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                     LaBHYT = item.LaDuocPhamBHYT,
                     TenDuocPham = item.Ten,
                     //Nhom = item.LaDuocPhamBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                     NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                     HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                     DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                     DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                     HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                     NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                     SoLuongYeuCau = item.SoLuong,
                     DichVuKham = item.YeuCauKhamBenh.TenDichVu,
                     BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                     NgayKe = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                     KhoLinhId = item.KhoLinhId
                 }).GroupBy(x => new
                 {
                     x.DuocPhamBenhVienId,
                     x.TenDuocPham,
                     x.Id
                 }).Select(item => new DSCanLinhDuocPhamTrucTiepGridParentVo() {
                        DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                        LaBHYT = item.First().LaBHYT,
                        TenDuocPham = item.First().TenDuocPham,
                        NongDoHamLuong = item.First().NongDoHamLuong,
                        HoatChat = item.First().HoatChat,
                        DuongDung = item.First().DuongDung,
                        DonViTinh = item.First().DonViTinh,
                        DichVuKham = item.First().DichVuKham,
                        BacSiKeToa = item.First().BacSiKeToa,
                        NgayKe = item.First().NgayKe,
                        HangSanXuat = item.First().HangSanXuat,
                        NuocSanXuat = item.First().NuocSanXuat,
                        SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                        SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                        && x.NhapKhoDuocPhams.KhoId == item.First().KhoLinhId
                                        && x.NhapKhoDuocPhams.DaHet != true
                                        && x.LaDuocPhamBHYT == item.First().LaBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                    
                 })
                 .OrderBy(x => x.TenDuocPham).ThenBy(x=>x.HighLightClass).Distinct();
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
        public async Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoDuocPhamCanLinhTrucTiepForGrid(QueryInfo queryInfo)
        {
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }


            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _phongBenhVienRepository.Table.Where(o => o.KhoaPhongId == khoaId && o.IsDisabled != true).Select(o => new LookupItemVo
            {
                KeyId = (long)o.Id,
                DisplayName = o.Ten
            }).OrderBy(o => o.DisplayName).ToList();
            var danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo =
                JsonConvert.DeserializeObject<DanhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                 .Where(x => phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId) &&
                 x.YeuCauLinhDuocPhamId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                             && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                             && x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2
                             && (danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo.KhoLinhId == null || danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo.KhoLinhId == 0 || x.KhoLinhId == danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo.KhoLinhId)
                             && x.YeuCauTiepNhanId == danhSachDuocPhamCanLinhTrucTiepChiTietQueryInfo.YeuCauTiepNhanId)
                 .Select(item => new DSCanLinhDuocPhamTrucTiepGridParentVo()
                 {
                     Id = item.Id,
                     DuocPhamBenhVienId = item.DuocPhamBenhVienId,
                     LaBHYT = item.LaDuocPhamBHYT,
                     TenDuocPham = item.Ten,
                     //Nhom = item.LaDuocPhamBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT",
                     NongDoHamLuong = item.DuocPhamBenhVien.DuocPham.HamLuong,
                     HoatChat = item.DuocPhamBenhVien.DuocPham.HoatChat,
                     DuongDung = item.DuocPhamBenhVien.DuocPham.DuongDung.Ten,
                     DonViTinh = item.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                     HangSanXuat = item.DuocPhamBenhVien.DuocPham.NhaSanXuat,
                     NuocSanXuat = item.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                     SoLuongYeuCau = item.SoLuong,
                     DichVuKham = item.YeuCauKhamBenh.TenDichVu,
                     BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                     NgayKe = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                     KhoLinhId = item.KhoLinhId
                 }).GroupBy(x => new
                 {
                     x.DuocPhamBenhVienId,
                     x.TenDuocPham,
                     x.Id
                 }).Select(item => new DSCanLinhDuocPhamTrucTiepGridParentVo()
                 {
                     DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                     LaBHYT = item.First().LaBHYT,
                     TenDuocPham = item.First().TenDuocPham,
                     NongDoHamLuong = item.First().NongDoHamLuong,
                     HoatChat = item.First().HoatChat,
                     DuongDung = item.First().DuongDung,
                     DonViTinh = item.First().DonViTinh,
                     DichVuKham = item.First().DichVuKham,
                     BacSiKeToa = item.First().BacSiKeToa,
                     NgayKe = item.First().NgayKe,
                     HangSanXuat = item.First().HangSanXuat,
                     NuocSanXuat = item.First().NuocSanXuat,
                     SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                     SoLuongTon = _nhapKhoDuocPhamChiTietRepository.TableNoTracking
                            .Where(x => x.DuocPhamBenhVienId == item.First().DuocPhamBenhVienId
                                        && x.NhapKhoDuocPhams.KhoId == item.First().KhoLinhId
                                        && x.NhapKhoDuocPhams.DaHet != true
                                        && x.LaDuocPhamBHYT == item.First().LaBHYT
                                        && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat)
                 })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenDuocPham).Distinct();
            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }
        public List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model)
        {
            var danhSachDuocPhamCanLinhTrucTiep = GetDanhSachDuocPhamCanLinhTrucTiep(new QueryInfo());
            return danhSachDuocPhamCanLinhTrucTiep.Any() ? danhSachDuocPhamCanLinhTrucTiep.GroupBy(o => new { o.KhoLinhId, o.KhoLinh }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.KhoLinhId ?? 0,
                DisplayName = s.Key.KhoLinh
            }).ToList() : new List<LookupItemVo>();
        }
        public List<LookupItemVo> GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model)
        {
            var danhSachDuocPhamCanLinhTrucTiep = GetDanhSachDuocPhamCanLinhTrucTiep(new QueryInfo());
            return danhSachDuocPhamCanLinhTrucTiep.Any() ? danhSachDuocPhamCanLinhTrucTiep.GroupBy(o => new { o.PhongLinhVeId, o.PhongLinhVe }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.PhongLinhVeId??0,
                DisplayName = s.Key.PhongLinhVe
            }).ToList() : new List<LookupItemVo>();
        }
    }
}
