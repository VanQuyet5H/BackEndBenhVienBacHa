using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System.Globalization;
using Camino.Core.Domain.ValueObject.DanhSachVatTuCanLinhTrucTiep;
using Camino.Core.Domain.ValueObject.DanhSachVatTuCanLinhTrucTiep;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial class YeuCauLinhVatTuService
    {
        #region  Ds vat tư cần lĩnh trực tiếp

        public GridDataSource GetDanhSachVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo)
        {
            var list = GetDanhSachVatTuCanLinhTrucTiep(queryInfo);
            return new GridDataSource { Data = list.ToArray(), TotalRowCount = list.Count() };

        }

        private List<YeuCauLinhVatTuTrucTiepGridChildVo> GetDanhSachVatTuCanLinhTrucTiep(QueryInfo queryInfo)
        {
            var danhSachVatTuCanLinhTrucTiepQueryInfo = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ?
                JsonConvert.DeserializeObject<DanhSachVatTuCanLinhTrucTiepQueryInfo>(queryInfo.AdditionalSearchString) : new DanhSachVatTuCanLinhTrucTiepQueryInfo();

            var list = new List<YeuCauLinhVatTuTrucTiepGridChildVo>();
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

            //Lấy tất cả YCTN cần lĩnh trực tiếp về khoa
            var queryYeuCauVatTuBenhViens = _yeuCauVatTuBenhVienRepository.TableNoTracking.Include(o => o.VatTuBenhVien)
                .ThenInclude(o => o.NhapKhoVatTuChiTiets).ThenInclude(p => p.NhapKhoVatTu)
                .Include(o => o.KhoLinh)
            .Where(x => (danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId == null || danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId == 0 || x.KhoLinhId == danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId)
                        && phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId)
                        && x.YeuCauLinhVatTuId == null
                        && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                        && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Select(o => o);

            var yeuCauTiepNhanCoLinhTrucTieps = queryYeuCauVatTuBenhViens.Select(s => new YeuCauLinhVatTuTrucTiepGridChildVo
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
                x.HoTen
            })
            .Select(item => new YeuCauLinhVatTuTrucTiepGridChildVo()
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

            //Kiễm tra xem mỗi YCTN có vật tư tồn ở khoa nào
            var dsMaYeuCauTiepNhan = yeuCauTiepNhanCoLinhTrucTieps.Select(o => o.MaYeuCauTiepNhan).ToList();
            if (dsMaYeuCauTiepNhan.Any())
            {
                var listAllYCDPBV = queryYeuCauVatTuBenhViens.Where(o => dsMaYeuCauTiepNhan.Contains(o.YeuCauTiepNhan.MaYeuCauTiepNhan))
                                //.GroupBy(x => new
                                //{
                                //    x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                //    //x.VatTuBenhVienId
                                //})
                       .Select(item => new
                       {
                           Id = item.Id,
                           MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                           DuocPhamBenhVienId = item.VatTuBenhVienId,
                           LaDuocPhamBHYT = item.LaVatTuBHYT,
                           SoLuong = item.SoLuong,
                           KhoLinhId = item.KhoLinhId,
                           KhoLinh = item.KhoLinh.Ten,
                       }).ToList();
                if (listAllYCDPBV != null)
                {


                    var dsDuocPhamBenhVienId = listAllYCDPBV.Select(o => o.DuocPhamBenhVienId).Distinct().ToList();
                    var dsKhoId = listAllYCDPBV.Select(o => o.KhoLinhId).Distinct().ToList();

                    var dsDuocPhamTrongKhoCanKiemTra = _nhapKhoVatTuChiTietRepository.TableNoTracking
                        .Where(nkct => dsDuocPhamBenhVienId.Contains(nkct.VatTuBenhVienId) &&
                                dsKhoId.Contains(nkct.NhapKhoVatTu.KhoId) && nkct.SoLuongNhap > nkct.SoLuongDaXuat && nkct.HanSuDung >= DateTime.Now)
                        .Select(nkct => new { nkct.VatTuBenhVienId, nkct.NhapKhoVatTu.KhoId, nkct.LaVatTuBHYT, nkct.SoLuongNhap, nkct.SoLuongDaXuat }).ToList();


                    foreach (var yeuCauTiepNhanCoLinhTrucTiep in yeuCauTiepNhanCoLinhTrucTieps)
                    {
                        var listYCDPBV = listAllYCDPBV.Where(o => o.MaYeuCauTiepNhan == yeuCauTiepNhanCoLinhTrucTiep.MaYeuCauTiepNhan && o.KhoLinhId == yeuCauTiepNhanCoLinhTrucTiep.KhoLinhId);
                        var tonKhongDu = false;

                        foreach (var yCDPBV in listYCDPBV)
                        {

                            var tonkho = dsDuocPhamTrongKhoCanKiemTra
                                .Where(nkct => nkct.VatTuBenhVienId == yCDPBV.DuocPhamBenhVienId &&
                                    nkct.KhoId == yCDPBV.KhoLinhId && nkct.LaVatTuBHYT == yCDPBV.LaDuocPhamBHYT)
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
            //    foreach (var yeuCauTiepNhanCoLinhTrucTiep in yeuCauTiepNhanCoLinhTrucTieps)
            //    {
            //        //Lấy ra tất cả vật tư của YCTN
            //        var listYCDPBV = queryYeuCauVatTuBenhViens.Where(o => o.YeuCauTiepNhan.MaYeuCauTiepNhan == yeuCauTiepNhanCoLinhTrucTiep.MaYeuCauTiepNhan)
            //                     .GroupBy(x => new
            //                     {
            //                         x.VatTuBenhVienId
            //                     })
            //            .Select(item => new
            //            {
            //                Id = item.First().Id,
            //                VatTuBenhVienId = item.First().VatTuBenhVienId,
            //                LaVatTuBHYT = item.First().LaVatTuBHYT,
            //                SoLuong = item.Sum(a => a.SoLuong),
            //                KhoLinhId = item.First().KhoLinhId,
            //                KhoLinh = item.First().KhoLinh.Ten,
            //                NhapKhoVatTuChiTiets = item.First().VatTuBenhVien.NhapKhoVatTuChiTiets
            //            });
            //        if (listYCDPBV != null)
            //        {
            //            var tonKhongDu = false;
            //            foreach (var yCDPBV in listYCDPBV)
            //            {
            //                var tonkho = yCDPBV.NhapKhoVatTuChiTiets
            //                    .Where(nkct =>
            //                        nkct.NhapKhoVatTu.KhoId == yCDPBV.KhoLinhId && nkct.LaVatTuBHYT == yCDPBV.LaVatTuBHYT &&
            //                        nkct.HanSuDung >= DateTime.Now).Sum(nkct => nkct.SoLuongNhap - nkct.SoLuongDaXuat);
            //                if (tonkho <= 0 || yCDPBV.SoLuong > tonkho)
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
            //}
            var result = yeuCauTiepNhanCoLinhTrucTieps.Union(list).Where(x => (danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId == null || x.KhoLinhId == danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId));
            return result.ToList();
        }
        private List<DanhSachVatTuCanLinhTrucTiepGridVo> GetDanhSachVatTuCanLinhTrucTiep1(QueryInfo queryInfo)
        {
            var danhSachVatTuCanLinhTrucTiepQueryInfo = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ?
                JsonConvert.DeserializeObject<DanhSachVatTuCanLinhTrucTiepQueryInfo>(queryInfo.AdditionalSearchString) : new DanhSachVatTuCanLinhTrucTiepQueryInfo();
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            //Lấy ds phòng mà nhân viên login thuộc (bao gồm các phòng mà nhân viên chỉ thuộc khoa)
            var phongLinhVes = _khoaPhongNhanVienRepository.TableNoTracking
                .Where(p => p.NhanVienId == userCurrentId && p.PhongBenhVienId != null &&
                            (danhSachVatTuCanLinhTrucTiepQueryInfo.PhongLinhVeId == null || p.PhongBenhVienId == danhSachVatTuCanLinhTrucTiepQueryInfo.PhongLinhVeId))
                .Select(s => new LookupItemVo
                {
                    KeyId = (long)s.PhongBenhVienId,
                    DisplayName = s.PhongBenhVien.Ten
                })
                .Union(_khoaPhongNhanVienRepository.TableNoTracking
                    .Where(p => p.NhanVienId == userCurrentId && p.PhongBenhVienId == null)
                    .SelectMany(s => s.KhoaPhong.PhongBenhViens
                        .Where(p => danhSachVatTuCanLinhTrucTiepQueryInfo.PhongLinhVeId == null || p.Id == danhSachVatTuCanLinhTrucTiepQueryInfo.PhongLinhVeId).Select(o => new LookupItemVo
                        {
                            KeyId = (long)o.Id,
                            DisplayName = o.Ten
                        })))
                .OrderBy(o => o.DisplayName);

            var list = new List<DanhSachVatTuCanLinhTrucTiepGridVo>();
            foreach (var phongLinhVe in phongLinhVes)
            {
                //Kiểm tra xem có dược phẩm nào cần bù trong kho lĩnh về này hay ko?
                var yeuCauVatTuBenhViens = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(o => o.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan && o.YeuCauLinhVatTuId == null &&
                                o.NoiChiDinhId == phongLinhVe.KeyId && o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                    .Select(o => o);
                if (yeuCauVatTuBenhViens.Any())
                {
                    //Kiểm tra xem có tồn tại dược phẩm nào trong kho lĩnh về này còn tồn kho trong kho lĩnh từ hay ko?
                    var khoLinhTus = _khoRepository.TableNoTracking
                        .Where(p => (danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId == null || (danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId != null &&p.Id == danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId))
                        && p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 && p.NhapKhoVatTus
                                        .Where(o => o.DaHet != true)
                                        .SelectMany(o => o.NhapKhoVatTuChiTiets).Any(ct =>
                                            ct.SoLuongDaXuat < ct.SoLuongNhap && ct.HanSuDung >= DateTime.Now &&
                                            yeuCauVatTuBenhViens.Any(o =>
                                                o.VatTuBenhVienId == ct.VatTuBenhVienId &&
                                                o.LaVatTuBHYT == ct.LaVatTuBHYT && o.KhoLinhId == p.Id)));
                    if (khoLinhTus.Any())
                    {
                        foreach (var khoLinhTu in khoLinhTus)
                        {
                            list.Add(new DanhSachVatTuCanLinhTrucTiepGridVo
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
                        if (danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId == null)
                        {
                            list.Add(new DanhSachVatTuCanLinhTrucTiepGridVo
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
        public async Task<GridDataSource> GetDanhSachChiTietVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo)
        {
            var danhSachVatTuCanLinhTrucTiepQueryInfo =
                 JsonConvert.DeserializeObject<DanhSachVatTuCanLinhTrucTiepQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                   .Where(x => x.NoiChiDinhId == danhSachVatTuCanLinhTrucTiepQueryInfo.PhongLinhVeId
                               && x.YeuCauLinhVatTuId == null 
                               && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                               && x.KhoLinhId == danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId
                               && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                   .Select(s => new YeuCauLinhTrucTiepVatTuGridChildVo
                   {
                       Id = s.Id,
                       MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                       MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                       HoTen = s.YeuCauTiepNhan.HoTen,
                       SoLuong = s.SoLuong,
                       DichVuKham = s.YeuCauKhamBenh.TenDichVu,
                       BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                       NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                       KhoLinhId = danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId,
                       PhongLinhVeId = danhSachVatTuCanLinhTrucTiepQueryInfo.PhongLinhVeId,
                       YeuCauTiepNhanId = s.YeuCauTiepNhanId
                   })
                   .GroupBy(x => new
                   {
                       x.MaYeuCauTiepNhan,
                       x.HoTen,
                   })
                   .Select(item => new YeuCauLinhTrucTiepVatTuGridChildVo()
                   {
                       Id = item.First().Id,
                       MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                       MaBenhNhan = item.First().MaBenhNhan,
                       HoTen = item.First().HoTen,
                       SoLuong = item.Sum(a => a.SoLuong),
                       DichVuKham = item.First().DichVuKham,
                       BacSiKeToa = item.First().HoTen,
                       NgayKe = item.First().NgayKe,
                       KhoLinhId = item.First().KhoLinhId,
                       PhongLinhVeId = item.First().PhongLinhVeId,
                       YeuCauTiepNhanId = item.First().YeuCauTiepNhanId
                   })
                   .Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageDanhSachChiTietVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo)
        {
            var danhSachVatTuCanLinhTrucTiepQueryInfo =
                 JsonConvert.DeserializeObject<DanhSachVatTuCanLinhTrucTiepQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.NoiChiDinhId == danhSachVatTuCanLinhTrucTiepQueryInfo.PhongLinhVeId
                            && x.YeuCauLinhVatTuId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                            && x.KhoLinhId == danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId
                            && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                .Select(s => new YeuCauLinhTrucTiepVatTuGridChildVo
                {
                    Id = s.Id,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBenhNhan = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    SoLuong = s.SoLuong,
                    DichVuKham = s.YeuCauKhamBenh.TenDichVu,
                    BacSiKeToa = s.NhanVienChiDinh.User.HoTen,
                    NgayKe = s.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                    KhoLinhId = danhSachVatTuCanLinhTrucTiepQueryInfo.KhoLinhId,
                    PhongLinhVeId = danhSachVatTuCanLinhTrucTiepQueryInfo.PhongLinhVeId,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId
                })
                .GroupBy(x => new
                {
                    x.MaYeuCauTiepNhan,
                    x.HoTen,
                })
                .Select(item => new YeuCauLinhTrucTiepVatTuGridChildVo()
                {
                    Id = item.First().Id,
                    MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                    MaBenhNhan = item.First().MaBenhNhan,
                    HoTen = item.First().HoTen,
                    SoLuong = item.Sum(a => a.SoLuong),
                    DichVuKham = item.First().DichVuKham,
                    BacSiKeToa = item.First().HoTen,
                    NgayKe = item.First().NgayKe,
                    KhoLinhId = item.First().KhoLinhId,
                    PhongLinhVeId = item.First().PhongLinhVeId,
                    YeuCauTiepNhanId = item.First().YeuCauTiepNhanId
                })
                .Distinct();

            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }
        public async Task<GridDataSource> GetDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo)
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

            var danhSachVatTuCanLinhTrucTiepChiTietQueryInfo =
                JsonConvert.DeserializeObject<DanhSachVatTuCanLinhTrucTiepChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
            .Where(x => phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId)
                        && x.YeuCauLinhVatTuId == null
                        && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                        && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                        //&& (danhSachVatTuCanLinhTrucTiepChiTietQueryInfo.KhoLinhId == null || danhSachVatTuCanLinhTrucTiepChiTietQueryInfo.KhoLinhId == 0 || x.KhoLinhId == danhSachVatTuCanLinhTrucTiepChiTietQueryInfo.KhoLinhId)
                        && x.YeuCauTiepNhanId == danhSachVatTuCanLinhTrucTiepChiTietQueryInfo.YeuCauTiepNhanId
                        && x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
            .Select(item => new YeuCauLinhTrucTiepVatTuVo()
            {
                Id = item.Id,
                VatTuBenhVienId = item.VatTuBenhVienId,
                LaBHYT = item.LaVatTuBHYT,
                TenVatTu = item.Ten,
                Nhom = item.LaVatTuBHYT ? "Vật Tư BHYT" : "Vặt Tư Không BHYT",
                DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                SoLuongYeuCau = item.SoLuong,
                DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                NgayKe = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                KhoLinhId = item.KhoLinhId
            })
              .GroupBy(x => new
              {
                  x.TenVatTu,
                  x.VatTuBenhVienId,
                  x.Id
              })
             .Select(item => new YeuCauLinhTrucTiepVatTuVo()
             {
                 Nhom = item.First().Nhom,
                 VatTuBenhVienId = item.First().VatTuBenhVienId,
                 LaBHYT = item.First().LaBHYT,
                 TenVatTu = item.First().TenVatTu,
                 DonViTinh = item.First().DonViTinh,
                 HangSanXuat = item.First().HangSanXuat,
                 NuocSanXuat = item.First().NuocSanXuat,
                 SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                 SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                        && x.NhapKhoVatTu.KhoId == item.First().KhoLinhId
                                        && x.LaVatTuBHYT == item.First().LaBHYT
                                        && x.NhapKhoVatTu.DaHet != true
                                        && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                 DichVuKham = item.First().DichVuKham,
                 BacSiKeToa = item.First().BacSiKeToa,
                 NgayKe = item.First().NgayKe,
                 HighLightClass = item.Sum(x => x.SoLuongYeuCau) > _nhapKhoVatTuChiTietRepository.TableNoTracking
                                                            .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                                                        && x.NhapKhoVatTu.KhoId == item.First().KhoLinhId
                                                                        && x.LaVatTuBHYT == item.First().LaBHYT
                                                                        && x.NhapKhoVatTu.DaHet != true
                                                                        && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat) ? "bg-row-lightRed" : ""

             })
                    .OrderBy(x => x.TenVatTu).Distinct();
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
        public async Task<GridDataSource> GetTotalPageDanhSachChiTietYeuCauTheoVatTuCanLinhTrucTiepForGrid(QueryInfo queryInfo)
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
            var danhSachVatTuCanLinhTrucTiepChiTietQueryInfo =
                   JsonConvert.DeserializeObject<DanhSachVatTuCanLinhTrucTiepChiTietQueryInfo>(queryInfo.AdditionalSearchString);
            BuildDefaultSortExpression(queryInfo);
            var query = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => phongLinhVes.Any(o => o.KeyId == x.NoiChiDinhId)
                            && x.YeuCauLinhVatTuId == null && x.LoaiPhieuLinh == EnumLoaiPhieuLinh.LinhChoBenhNhan
                            && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy
                            //&& (danhSachVatTuCanLinhTrucTiepChiTietQueryInfo.KhoLinhId == null || danhSachVatTuCanLinhTrucTiepChiTietQueryInfo.KhoLinhId == 0 || x.KhoLinhId == danhSachVatTuCanLinhTrucTiepChiTietQueryInfo.KhoLinhId)
                            && x.YeuCauTiepNhanId == danhSachVatTuCanLinhTrucTiepChiTietQueryInfo.YeuCauTiepNhanId
                            && x.KhoLinh.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2)
                .Select(item => new YeuCauLinhTrucTiepVatTuVo()
                {
                    Id = item.Id,
                    VatTuBenhVienId = item.VatTuBenhVienId,
                    LaBHYT = item.LaVatTuBHYT,
                    TenVatTu = item.Ten,
                    Nhom = item.LaVatTuBHYT ? "Vật Tư BHYT" : "Vật Tư Không BHYT",
                    DonViTinh = item.VatTuBenhVien.VatTus.DonViTinh,
                    HangSanXuat = item.VatTuBenhVien.VatTus.NhaSanXuat,
                    NuocSanXuat = item.VatTuBenhVien.VatTus.NuocSanXuat,
                    SoLuongYeuCau = item.SoLuong,
                    DichVuKham = item.YeuCauKhamBenh != null ? item.YeuCauKhamBenh.TenDichVu : (item.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? item.YeuCauDichVuKyThuat.YeuCauKhamBenh.TenDichVu : ""),
                    BacSiKeToa = item.NhanVienChiDinh.User.HoTen,
                    NgayKe = item.ThoiDiemChiDinh.ApplyFormatDateTimeSACH(),
                })
                  .GroupBy(x => new
                  {
                      x.TenVatTu,
                      x.VatTuBenhVienId,
                      x.Id
                  })
             .Select(item => new YeuCauLinhTrucTiepVatTuVo()
             {

                 VatTuBenhVienId = item.First().VatTuBenhVienId,
                 LaBHYT = item.First().LaBHYT,
                 TenVatTu = item.First().TenVatTu,
                 DonViTinh = item.First().DonViTinh,
                 HangSanXuat = item.First().HangSanXuat,
                 NuocSanXuat = item.First().NuocSanXuat,
                 SoLuongYeuCau = item.Sum(x => x.SoLuongYeuCau),
                 SoLuongTon = _nhapKhoVatTuChiTietRepository.TableNoTracking
                            .Where(x => x.VatTuBenhVienId == item.First().VatTuBenhVienId
                                        && x.NhapKhoVatTu.KhoId == item.First().KhoLinhId
                                        && x.LaVatTuBHYT == item.First().LaBHYT
                                        && x.NhapKhoVatTu.DaHet != true
                                        && x.SoLuongDaXuat < x.SoLuongNhap && x.HanSuDung >= DateTime.Now).Sum(x => x.SoLuongNhap - x.SoLuongDaXuat),
                 DichVuKham = item.First().DichVuKham,
                 BacSiKeToa = item.First().BacSiKeToa,
                 NgayKe = item.First().NgayKe
             })
                    .OrderBy(x => x.LaBHYT).ThenBy(x => x.TenVatTu).Distinct();
            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }
        public List<LookupItemVo> GetTatCakhoLinhTuCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model)
        {
            var danhSachVatTuCanLinhTrucTiep = GetDanhSachVatTuCanLinhTrucTiep(new QueryInfo());
            return danhSachVatTuCanLinhTrucTiep.Any() ? danhSachVatTuCanLinhTrucTiep.GroupBy(o => new { o.KhoLinhId, o.KhoLinh }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.KhoLinhId??0,
                DisplayName = s.Key.KhoLinh
            }).ToList() : new List<LookupItemVo>();
        }
        public List<LookupItemVo> GetTatCaPhongLinhVeCuaNhanVienLoginLinhTrucTiep(LookupQueryInfo model)
        {
            var danhSachVatTuCanLinhTrucTiep = GetDanhSachVatTuCanLinhTrucTiep(new QueryInfo());
            return danhSachVatTuCanLinhTrucTiep.Any() ? danhSachVatTuCanLinhTrucTiep.GroupBy(o => new { o.PhongLinhVeId, o.PhongLinhVe }).Select(s => new LookupItemVo
            {
                KeyId = s.Key.PhongLinhVeId ?? 0,
                DisplayName = s.Key.PhongLinhVe
            }).ToList() : new List<LookupItemVo>();
        }
        #endregion
    }
}
