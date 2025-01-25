using Camino.Core.Domain.ValueObject.BaoCaos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public List<BaoCaoChiTietDoanhThuTheoBacSiGridVo> GetDataForBaoCaoChiTietDoanhThuTheoBacSi(DateTimeFilterVo dateTimeFilter)
        {
            var baoCaoChiTietDoanhThuTheoBacSiGrid = new List<BaoCaoChiTietDoanhThuTheoBacSiGridVo>();

            var baoCaoChiTietDoanhThuTheoBacSi = new BaoCaoChiTietDoanhThuTheoBacSiGridVo
            {
                Id = 1,
                MaTn = "0096",
                NgayThu = DateTime.Now,
                MaBn = "2390",
                HoTenBn = "Trần Văn A",
                DichVuChiDinh = "Chụp MRI",
                DoanhThuTheoThang = 706000,
                MienGiamTheoThang = 79000,
                BhytTheoThang = 147000
            };

            baoCaoChiTietDoanhThuTheoBacSiGrid.Add(baoCaoChiTietDoanhThuTheoBacSi);

            var baoCaoChiTietDoanhThuTheoBacSi1 = new BaoCaoChiTietDoanhThuTheoBacSiGridVo
            {
                Id = 2,
                MaTn = "0096",
                NgayThu = DateTime.Now,
                MaBn = "2390",
                HoTenBn = "Trần Văn A",
                DichVuChiDinh = "Chụp XQuang",
                DoanhThuTheoThang = 806000,
                MienGiamTheoThang = 179000,
                BhytTheoThang = 247000
            };

            baoCaoChiTietDoanhThuTheoBacSiGrid.Add(baoCaoChiTietDoanhThuTheoBacSi1);

            var baoCaoChiTietDoanhThuTheoBacSi2 = new BaoCaoChiTietDoanhThuTheoBacSiGridVo
            {
                Id = 3,
                MaTn = "0120",
                NgayThu = DateTime.Now,
                MaBn = "8110",
                HoTenBn = "Trần Văn C",
                DichVuChiDinh = "Chụp Từ Quang",
                DoanhThuTheoThang = 906000,
                MienGiamTheoThang = 189000,
                BhytTheoThang = 147000
            };

            baoCaoChiTietDoanhThuTheoBacSiGrid.Add(baoCaoChiTietDoanhThuTheoBacSi2);

            var baoCaoChiTietDoanhThuTheoBacSi3 = new BaoCaoChiTietDoanhThuTheoBacSiGridVo
            {
                Id = 3,
                MaTn = "0120",
                NgayThu = DateTime.Now,
                MaBn = "8110",
                HoTenBn = "Trần Văn C",
                DichVuChiDinh = "Chụp Phim",
                DoanhThuTheoThang = 1006000,
                MienGiamTheoThang = 129000,
                BhytTheoThang = 127000
            };

            baoCaoChiTietDoanhThuTheoBacSiGrid.Add(baoCaoChiTietDoanhThuTheoBacSi3);

            return baoCaoChiTietDoanhThuTheoBacSiGrid;
        }

        public async Task<GridDataSource> GetBaoCaoChiTietDoanhThuTheoBacSiForGridAsync(BaoCaoDoanhThuTheoBacSiQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay));

            ycTiepNhanQuery = ycTiepNhanQuery.Where(o =>
                o.YeuCauKhamBenhs.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.BacSiThucHienId == queryInfo.BacSiId) ||
                o.YeuCauDichVuKyThuats.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && 
                                                    ((yc.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && yc.NhanVienThucHienId == queryInfo.BacSiId) ||
                                                     (yc.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && yc.NhanVienChiDinhId == queryInfo.BacSiId)))
                //TODO: need update goi dv
                //|| o.YeuCauGoiDichVus.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && (yc.YeuCauKhamBenhs.Any(k=>k.BacSiThucHienId == queryInfo.BacSiId) || yc.YeuCauDichVuKyThuats.Any(k => k.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && k.NhanVienThucHienId == queryInfo.BacSiId)))
                );

            var totalRowCount = await ycTiepNhanQuery.CountAsync();
            if (totalRowCount == 0)
            {
                return new GridDataSource { Data = new List<GridItem>(), TotalRowCount = totalRowCount };
            }

            var ycTiepNhanDataQuery = queryInfo.LayTatCa ? ycTiepNhanQuery.OrderBy(o => o.Id) : ycTiepNhanQuery.OrderBy(o => o.Id).Skip(queryInfo.Skip).Take(queryInfo.Take);

            var ycTiepNhanData = await ycTiepNhanDataQuery
                .Include(o => o.BenhNhan)
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            var gridData = new List<BaoCaoChiTietDoanhThuTheoBacSiGridVo>();
            for (int i = 0; i < ycTiepNhanData.Count; i++)
            {
                gridData.AddRange(ycTiepNhanData[i].YeuCauKhamBenhs.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.BacSiThucHienId == queryInfo.BacSiId)
                    .Select(o => new BaoCaoChiTietDoanhThuTheoBacSiGridVo
                    {
                        Stt = i + 1,
                        MaTn = ycTiepNhanData[i].MaYeuCauTiepNhan,
                        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                        MaBn = ycTiepNhanData[i].BenhNhan.MaBN,
                        HoTenBn = ycTiepNhanData[i].BenhNhan.HoTen,
                        DichVuChiDinh = o.TenDichVu,
                        DoanhThuTheoThang = o.Gia,
                        BhytTheoThang = o.DonGiaBaoHiem.GetValueOrDefault()*o.TiLeBaoHiemThanhToan.GetValueOrDefault()/100*o.MucHuongBaoHiem.GetValueOrDefault()/100,
                        MienGiamTheoThang = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        CacKhoanGiamKhacTheoThang = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum()
                    }));
                gridData.AddRange(ycTiepNhanData[i].YeuCauDichVuKyThuats.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null &&
                                                                                     ((yc.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && yc.NhanVienThucHienId == queryInfo.BacSiId) ||
                                                                                      (yc.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && yc.NhanVienChiDinhId == queryInfo.BacSiId)))
                    .Select(o => new BaoCaoChiTietDoanhThuTheoBacSiGridVo
                    {
                        Stt = i + 1,
                        MaTn = ycTiepNhanData[i].MaYeuCauTiepNhan,
                        NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                        MaBn = ycTiepNhanData[i].BenhNhan.MaBN,
                        HoTenBn = ycTiepNhanData[i].BenhNhan.HoTen,
                        DichVuChiDinh = o.TenDichVu,
                        DoanhThuTheoThang = o.Gia * o.SoLan,
                        BhytTheoThang = o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100 * o.SoLan,
                        MienGiamTheoThang = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum(),
                        CacKhoanGiamKhacTheoThang = o.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum()
                    }));
                //TODO: need update goi dv
                //foreach (var yeuCauGoiDichVu in ycTiepNhanData[i].YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && (yc.YeuCauKhamBenhs.Any(k => k.BacSiThucHienId == queryInfo.BacSiId) || yc.YeuCauDichVuKyThuats.Any(k => k.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && k.NhanVienThucHienId == queryInfo.BacSiId))))
                //{
                //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                //    gridData.AddRange(yeuCauGoiDichVu.YeuCauKhamBenhs.Where(k => k.BacSiThucHienId == queryInfo.BacSiId)
                //        .Select(o => new BaoCaoChiTietDoanhThuTheoBacSiGridVo
                //        {
                //            Stt = i + 1,
                //            MaTn = ycTiepNhanData[i].MaYeuCauTiepNhan,
                //            NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                //            MaBn = ycTiepNhanData[i].BenhNhan.MaBN,
                //            HoTenBn = ycTiepNhanData[i].BenhNhan.HoTen,
                //            DichVuChiDinh = o.TenDichVu,
                //            DoanhThuTheoThang = o.Gia - (o.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100),
                //            MienGiamTheoThang = (o.Gia - (o.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * tiLeMienGiam,
                //            CacKhoanGiamKhacTheoThang = (o.Gia - (o.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * tiLeGiamTruKhac
                //        }));
                //    gridData.AddRange(yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(k => k.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && k.NhanVienThucHienId == queryInfo.BacSiId)
                //        .Select(o => new BaoCaoChiTietDoanhThuTheoBacSiGridVo
                //        {
                //            Stt = i + 1,
                //            MaTn = ycTiepNhanData[i].MaYeuCauTiepNhan,
                //            NgayThu = ycTiepNhanData[i].ThoiDiemTiepNhan,
                //            MaBn = ycTiepNhanData[i].BenhNhan.MaBN,
                //            HoTenBn = ycTiepNhanData[i].BenhNhan.HoTen,
                //            DichVuChiDinh = o.TenDichVu,
                //            DoanhThuTheoThang = (o.Gia - (o.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * o.SoLan,
                //            MienGiamTheoThang = (o.Gia - (o.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * tiLeMienGiam,
                //            CacKhoanGiamKhacTheoThang = (o.Gia - (o.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * tiLeGiamTruKhac
                //        }));
                //}
            }
            return new GridDataSource { Data = gridData.ToArray(), TotalRowCount = totalRowCount };
        }

        public async Task<GridItem> GetTotalBaoCaoChiTietDoanhThuTheoBacSiForGridAsync(BaoCaoDoanhThuTheoBacSiQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ycTiepNhanQuery = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                (queryInfo.TuNgay == null || tuNgay <= o.ThoiDiemTiepNhan) &&
                (queryInfo.DenNgay == null || o.ThoiDiemTiepNhan < denNgay));

            ycTiepNhanQuery = ycTiepNhanQuery.Where(o =>
                o.YeuCauKhamBenhs.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.BacSiThucHienId == queryInfo.BacSiId) ||
                o.YeuCauDichVuKyThuats.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null &&
                                                    ((yc.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && yc.NhanVienThucHienId == queryInfo.BacSiId) ||
                                                     (yc.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && yc.NhanVienChiDinhId == queryInfo.BacSiId)))
                //TODO: need update goi dv
                //|| o.YeuCauGoiDichVus.Any(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && (yc.YeuCauKhamBenhs.Any(k => k.BacSiThucHienId == queryInfo.BacSiId) || yc.YeuCauDichVuKyThuats.Any(k => k.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && k.NhanVienThucHienId == queryInfo.BacSiId)))
                );
            
            var ycTiepNhanData = await ycTiepNhanQuery
                .Include(o => o.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                //TODO: need update goi dv
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauKhamBenhs).ThenInclude(yc => yc.MienGiamChiPhis)
                //.Include(o => o.YeuCauGoiDichVus).ThenInclude(yc => yc.YeuCauDichVuKyThuats).ThenInclude(yc => yc.MienGiamChiPhis)
                .ToListAsync();

            var gridItem = new BaoCaoChiTietDoanhThuTheoBacSiGridVo()
            {
                DoanhThuTheoThang = 0,
                BhytTheoThang = 0,
                MienGiamTheoThang = 0,
                CacKhoanGiamKhacTheoThang = 0
            };
            foreach (var yeuCauTiepNhan in ycTiepNhanData)
            {
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.BacSiThucHienId == queryInfo.BacSiId))
                {
                    gridItem.DoanhThuTheoThang = gridItem.DoanhThuTheoThang.GetValueOrDefault() + yeuCauKhamBenh.Gia;
                    gridItem.BhytTheoThang = gridItem.BhytTheoThang.GetValueOrDefault() + yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100;
                    gridItem.MienGiamTheoThang = gridItem.MienGiamTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    gridItem.CacKhoanGiamKhacTheoThang = gridItem.CacKhoanGiamKhacTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                }
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yc.YeuCauGoiDichVuId == null &&
                                                                                               ((yc.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && yc.NhanVienThucHienId == queryInfo.BacSiId) ||
                                                                                                (yc.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && yc.NhanVienChiDinhId == queryInfo.BacSiId))))
                {
                    gridItem.DoanhThuTheoThang = gridItem.DoanhThuTheoThang.GetValueOrDefault() + yeuCauKhamBenh.Gia * yeuCauKhamBenh.SoLan;
                    gridItem.BhytTheoThang = gridItem.BhytTheoThang.GetValueOrDefault() + (yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100) * yeuCauKhamBenh.SoLan;
                    gridItem.MienGiamTheoThang = gridItem.MienGiamTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                    gridItem.CacKhoanGiamKhacTheoThang = gridItem.CacKhoanGiamKhacTheoThang.GetValueOrDefault() + yeuCauKhamBenh.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                }
                //TODO: need update goi dv
                //foreach (var yeuCauGoiDichVu in yeuCauTiepNhan.YeuCauGoiDichVus.Where(yc => yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && (yc.YeuCauKhamBenhs.Any(k => k.BacSiThucHienId == queryInfo.BacSiId) || yc.YeuCauDichVuKyThuats.Any(k => k.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && k.NhanVienThucHienId == queryInfo.BacSiId))))
                //{
                //    var sotienMienGiam = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam != Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var sotienGiamTruKhac = yeuCauGoiDichVu.MienGiamChiPhis.Where(mg => mg.LoaiMienGiam == Enums.LoaiMienGiam.Voucher).Select(mg => mg.SoTien).DefaultIfEmpty(0).Sum();
                //    var tiLeMienGiam = sotienMienGiam.AlmostEqual(0) ? 0 : (sotienMienGiam / yeuCauGoiDichVu.ChiPhiGoiDichVu);
                //    var tiLeGiamTruKhac = sotienGiamTruKhac.AlmostEqual(0) ? 0 : (sotienGiamTruKhac / yeuCauGoiDichVu.ChiPhiGoiDichVu);

                //    foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs.Where(yc => yc.BacSiThucHienId == queryInfo.BacSiId))
                //    {
                //        var doanhThuDv = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100);
                //        gridItem.DoanhThuTheoThang = gridItem.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //        gridItem.MienGiamTheoThang = gridItem.MienGiamTheoThang.GetValueOrDefault() + doanhThuDv * tiLeMienGiam;
                //        gridItem.CacKhoanGiamKhacTheoThang = gridItem.CacKhoanGiamKhacTheoThang.GetValueOrDefault() + doanhThuDv * tiLeGiamTruKhac;
                //    }
                //    foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(k => k.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && k.NhanVienThucHienId == queryInfo.BacSiId))
                //    {
                //        var doanhThuDv = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.Gia * (decimal)yeuCauGoiDichVu.TiLeChietKhau.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                //        gridItem.DoanhThuTheoThang = gridItem.DoanhThuTheoThang.GetValueOrDefault() + doanhThuDv;
                //        gridItem.MienGiamTheoThang = gridItem.MienGiamTheoThang.GetValueOrDefault() + doanhThuDv * tiLeMienGiam;
                //        gridItem.CacKhoanGiamKhacTheoThang = gridItem.CacKhoanGiamKhacTheoThang.GetValueOrDefault() + doanhThuDv * tiLeGiamTruKhac;
                //    }
                //}
            }
            return gridItem;
        }
        public async Task<List<LookupItemVo>> GetListBacSy(DropDownListRequestModel queryInfo)
        {
            var dsBacSi = await _nhanVienRepository.TableNoTracking.Include(o => o.ChucDanh).ThenInclude(cd => cd.NhomChucDanh).Include(o => o.User)
               .Where(o => o.ChucDanh != null && o.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSi && o.User.HoTen.Contains(queryInfo.Query ?? ""))
               .OrderBy(o => o.Id)
               //.Skip(queryInfo.LayTatCa ? 0 : queryInfo.Skip).Take(queryInfo.LayTatCa ? int.MaxValue : queryInfo.Take)
               .ToListAsync();
            var query = dsBacSi.Select(item => new LookupItemVo
            {
                DisplayName = item.User.HoTen,
                KeyId = item.Id,
            }).ToList();
            return query;
        }
        public async Task<string> GetNameBacSy(long bacSiId)
        {
            var dsBacSi = await _nhanVienRepository.TableNoTracking.Include(o => o.ChucDanh).ThenInclude(cd => cd.NhomChucDanh).Include(o => o.User)
               .Where(o => o.ChucDanh != null && o.ChucDanh.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSi)
               .ToListAsync();
            var query = dsBacSi.Where(x => x.Id == bacSiId).FirstOrDefault()?.User?.HoTen != null ? dsBacSi.Where(x => x.Id == bacSiId).FirstOrDefault()?.User?.HoTen : "";
          
            return query;
        }
    }
}
