using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public List<BaoCaoTongHopDoanhThuTheoBacSiGridVo> GetDataForBaoCaoTongHopDoanhThuTheoBacSi(
            DateTimeFilterVo dateTimeFilter)
        {
            var baoCaoTongHopDoanhThuTheoBacSiGrid = new List<BaoCaoTongHopDoanhThuTheoBacSiGridVo>();

            var baoCaoTongHopDoanhThuTheoBacSi = new BaoCaoTongHopDoanhThuTheoBacSiGridVo
            {
                Id = 1,
                HoTenBacSi = "Ronaldo",
                DoanhThu = 806000,
                Bhyt = 179000,
                MienGiam = 247000
            };

            baoCaoTongHopDoanhThuTheoBacSiGrid.Add(baoCaoTongHopDoanhThuTheoBacSi);

            return baoCaoTongHopDoanhThuTheoBacSiGrid;
        }

        public async Task<GridDataSource> GetBaoCaoTongHopDoanhThuTheoBacSiForGridAsync(BaoCaoTongHopDoanhThuTheoBacSiQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay));

            var ycTiepNhanData = await ycTiepNhanQuery
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            
            var totalRowCount = await _nhanVienRepository.TableNoTracking
                .Where(o => o.ChucDanh != null && o.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSi).CountAsync();

            var dsBacSi = await _nhanVienRepository.TableNoTracking.Include(o => o.ChucDanh).ThenInclude(cd => cd.NhomChucDanh).Include(o => o.User)
                .Where(o=> o.ChucDanh != null && o.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSi)
                .OrderBy(o=>o.Id)
                .Skip(queryInfo.LayTatCa ? 0 : queryInfo.Skip).Take(queryInfo.LayTatCa ? int.MaxValue : queryInfo.Take)
                .ToListAsync();
            
            var gridData = dsBacSi.Select((o,i)=>new BaoCaoTongHopDoanhThuTheoBacSiGridVo
            {
                Id = o.Id,
                Stt = queryInfo.Skip + i + 1,
                HoTenBacSi = o.User.HoTen,
                DoanhThu = 0,
                MienGiam = 0,
                KhoanGiamTruKhac = 0,
                Bhyt = 0
            }).ToList();
            foreach (var yeuCauTiepNhan in ycTiepNhanData)
            {
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var gridItem = gridData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.BacSiThucHienId);
                    if (gridItem != null)
                    {
                        gridItem.DoanhThu = gridItem.DoanhThu.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                        gridItem.MienGiam = gridItem.MienGiam.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        gridItem.KhoanGiamTruKhac = gridItem.MienGiam.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        gridItem.Bhyt = gridItem.Bhyt.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                    }
                }
                foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                {
                    var gridItem = gridData.FirstOrDefault(o => (yeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && o.Id == yeuCauDichVuKyThuat.NhanVienThucHienId) ||
                                                                (yeuCauDichVuKyThuat.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && o.Id == yeuCauDichVuKyThuat.NhanVienChiDinhId));
                    if (gridItem != null)
                    {
                        gridItem.DoanhThu = gridItem.DoanhThu.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                        gridItem.MienGiam = gridItem.MienGiam.GetValueOrDefault() + yeuCauDichVuKyThuat
                                                .MienGiamChiPhis
                                                .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        gridItem.KhoanGiamTruKhac = gridItem.MienGiam.GetValueOrDefault() + yeuCauDichVuKyThuat
                                                        .MienGiamChiPhis.Where(mg =>
                                                            mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher)
                                                        .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        gridItem.Bhyt = gridItem.Bhyt.GetValueOrDefault() + yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                    }
                }
                //TODO: need update goi dv
                //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //{
                //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0)? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs)
                //    {
                //        var gridItem = gridData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.BacSiThucHienId);
                //        if (gridItem != null)
                //        {
                //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal) yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                //            gridItem.DoanhThu = gridItem.DoanhThu.GetValueOrDefault() + doanhThuDv;
                //            gridItem.MienGiam = gridItem.MienGiam.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            gridItem.KhoanGiamTruKhac = gridItem.KhoanGiamTruKhac.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats)
                //    {
                //        var gridItem = gridData.FirstOrDefault(o => yeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && o.Id == yeuCauDichVuKyThuat.NhanVienThucHienId);
                //        if (gridItem != null)
                //        {
                //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                //            gridItem.DoanhThu = gridItem.DoanhThu.GetValueOrDefault() + doanhThuDv;
                //            gridItem.MienGiam = gridItem.MienGiam.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            gridItem.KhoanGiamTruKhac = gridItem.KhoanGiamTruKhac.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //}
            }
            return new GridDataSource { Data = gridData.ToArray(), TotalRowCount = totalRowCount };
        }
        public async Task<GridItem> GetTotalBaoCaoTongHopDoanhThuTheoBacSiForGridAsync(BaoCaoTongHopDoanhThuTheoBacSiQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay));

            var ycTiepNhanData = await ycTiepNhanQuery
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            var dsBacSi = await _nhanVienRepository.TableNoTracking.Include(o => o.ChucDanh).ThenInclude(cd => cd.NhomChucDanh).Include(o => o.User)
                .Where(o => o.ChucDanh != null && o.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSi)
                .ToListAsync();

            var gridData = dsBacSi.Select((o, i) => new BaoCaoTongHopDoanhThuTheoBacSiGridVo
            {
                Id = o.Id,
                HoTenBacSi = o.User.HoTen,
                DoanhThu = 0,
                MienGiam = 0,
                KhoanGiamTruKhac = 0,
                Bhyt = 0
            }).ToList();
            foreach (var yeuCauTiepNhan in ycTiepNhanData)
            {
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true))
                {
                    var gridItem = gridData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.BacSiThucHienId);
                    if (gridItem != null)
                    {
                        gridItem.DoanhThu = gridItem.DoanhThu.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                        gridItem.MienGiam = gridItem.MienGiam.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        gridItem.KhoanGiamTruKhac = gridItem.MienGiam.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        gridItem.Bhyt = gridItem.Bhyt.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                    }
                }
                foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null))
                {
                    var gridItem = gridData.FirstOrDefault(o => (yeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && o.Id == yeuCauDichVuKyThuat.NhanVienThucHienId) ||
                                                                (yeuCauDichVuKyThuat.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && o.Id == yeuCauDichVuKyThuat.NhanVienChiDinhId));
                    if (gridItem != null)
                    {
                        gridItem.DoanhThu = gridItem.DoanhThu.GetValueOrDefault() + yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                        gridItem.MienGiam = gridItem.MienGiam.GetValueOrDefault() + yeuCauDichVuKyThuat
                                                .MienGiamChiPhis
                                                .Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher)
                                                .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        gridItem.KhoanGiamTruKhac = gridItem.MienGiam.GetValueOrDefault() + yeuCauDichVuKyThuat
                                                        .MienGiamChiPhis.Where(mg =>
                                                            mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher)
                                                        .Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                        gridItem.Bhyt = gridItem.Bhyt.GetValueOrDefault() + yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.SoLan;
                    }
                }
                //TODO: need update goi dv
                //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))
                //{
                //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs)
                //    {
                //        var gridItem = gridData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.BacSiThucHienId);
                //        if (gridItem != null)
                //        {
                //            var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                //            gridItem.DoanhThu = gridItem.DoanhThu.GetValueOrDefault() + doanhThuDv;
                //            gridItem.MienGiam = gridItem.MienGiam.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            gridItem.KhoanGiamTruKhac = gridItem.KhoanGiamTruKhac.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats)
                //    {
                //        var gridItem = gridData.FirstOrDefault(o => yeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && o.Id == yeuCauDichVuKyThuat.NhanVienThucHienId);
                //        if (gridItem != null)
                //        {
                //            var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                //            gridItem.DoanhThu = gridItem.DoanhThu.GetValueOrDefault() + doanhThuDv;
                //            gridItem.MienGiam = gridItem.MienGiam.GetValueOrDefault() + tiLeMienGiam * doanhThuDv;
                //            gridItem.KhoanGiamTruKhac = gridItem.KhoanGiamTruKhac.GetValueOrDefault() + tiLeGiamTruKhac * doanhThuDv;
                //        }
                //    }
                //}
            }
            return new BaoCaoTongHopDoanhThuTheoBacSiGridVo
            {
                DoanhThu = gridData.Sum(o => o.DoanhThu.GetValueOrDefault()),
                MienGiam = gridData.Sum(o => o.MienGiam.GetValueOrDefault()),
                KhoanGiamTruKhac = gridData.Sum(o => o.KhoanGiamTruKhac.GetValueOrDefault()),
                Bhyt = gridData.Sum(o => o.Bhyt.GetValueOrDefault())
            };
        }
    }
}
