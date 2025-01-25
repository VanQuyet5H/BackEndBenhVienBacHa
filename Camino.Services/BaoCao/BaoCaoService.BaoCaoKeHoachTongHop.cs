using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoKeHoachTongHopForGridAsync(BaoCaoKeHoachTongHopQueryInfo queryInfo)
        {
            var query = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhienXetNghiemChiTiets)
                .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                            && x.ThoiDiemTiepNhan >= queryInfo.TuNgay
                            && x.ThoiDiemTiepNhan <= queryInfo.DenNgay)
                .OrderBy(x => x.ThoiDiemTiepNhan).ToList();

            var result = query
            .Select(item => new BaoCaoKeHoachTongHopGridVo()
            {
                Id = item.Id,
                MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                TenNguoiBenh = item.HoTen,
                ThoiDiemTN = item.ThoiDiemTiepNhan,
                ThoiDiemBSKham = item.YeuCauKhamBenhs
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.ThoiDiemThucHien != null)
                    .OrderBy(x => x.ThoiDiemThucHien).Select(x => x.ThoiDiemThucHien)
                    .FirstOrDefault(),
                ThoiDiemRaChiDinh = item.YeuCauDichVuKyThuats
                    .Any(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                    || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                    || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))
                        ? item.YeuCauDichVuKyThuats
                            .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))
                            .OrderBy(x => x.ThoiDiemChiDinh)
                            .Select(x => x.ThoiDiemChiDinh)
                            .FirstOrDefault()
                        : (DateTime?)null,
                ThoiDiemLayMauXN = item.YeuCauDichVuKyThuats
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.PhienXetNghiemChiTiets.Any(a => a.ThoiDiemNhanMau != null))
                    .SelectMany(x => x.PhienXetNghiemChiTiets)
                    .OrderBy(x => x.ThoiDiemNhanMau)
                    .Select(x => x.ThoiDiemNhanMau)
                    .FirstOrDefault(),
                ThoiDiemTraKetQuaXN = item.YeuCauDichVuKyThuats
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                && x.PhienXetNghiemChiTiets.Any(a => a.ThoiDiemKetLuan != null))
                    .SelectMany(x => x.PhienXetNghiemChiTiets)
                    .OrderBy(x => x.ThoiDiemKetLuan)
                    .Select(x => x.ThoiDiemKetLuan)
                    .FirstOrDefault(),
                ThoiDiemThucHienCLS = item.YeuCauDichVuKyThuats
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                    || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                    .OrderBy(x => x.ThoiDiemHoanThanh)
                    .Select(x => x.ThoiDiemHoanThanh)
                    .FirstOrDefault(),
                ThoiDiemBacSiKetLuan = item.YeuCauKhamBenhs
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.ThoiDiemHoanThanh != null)
                    .OrderBy(x => x.ThoiDiemHoanThanh).Select(x => x.ThoiDiemHoanThanh)
                    .FirstOrDefault(),
                ThoiDiemBacSiKeDonThuoc = item.YeuCauKhamBenhs
                    .Any(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.YeuCauKhamBenhDonThuocs.Any())
                        ? item.YeuCauKhamBenhs
                            .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                        && x.YeuCauKhamBenhDonThuocs.Any())
                            .SelectMany(x => x.YeuCauKhamBenhDonThuocs)
                            .OrderBy(x => x.ThoiDiemKeDon).Select(x => x.ThoiDiemKeDon)
                            .FirstOrDefault()
                        : (DateTime?)null
            }).ToList();

            return new GridDataSource { Data = result.ToArray(), TotalRowCount = result.Count() };
        }
        public async Task<GridDataSource> GetDataBaoCaoKeHoachTongHopTheoNgayForGridAsync(BaoCaoKeHoachTongHopQueryInfo queryInfo)
        {
            var result = _yeuCauTiepNhanRepository.TableNoTracking
                  .Include(x => x.BenhNhan)
                  .Include(x => x.YeuCauKhamBenhs)
                  .Where(x => x.ThoiDiemTiepNhan >= queryInfo.TuNgay
                              && x.ThoiDiemTiepNhan <= queryInfo.DenNgay
                              && x.ThoiDiemTiepNhan != null
                              && x.YeuCauKhamBenhs.Select(a => a.ThoiDiemThucHien).FirstOrDefault() != null)
                  .Select(item => new BaoCaoKeHoachTongHopTheoNgayGridVo()
                  {
                      SoPhutChoKhamBenh = (float)Math.Round(((TimeSpan)(item.YeuCauKhamBenhs.Select(x => x.ThoiDiemThucHien).FirstOrDefault() - item.ThoiDiemTiepNhan)).TotalMinutes, 2),
                      SoPhutChoXetNghiem = 0,
                      SoPhutChoChuanDoanHA = 0,
                      SoPhutChoThamDoChucNang = 0
                  }).ToList();
            var result1 =_yeuCauDichVuKyThuatRepository.Table
                        .Include(x=>x.YeuCauTiepNhan).ThenInclude(x=>x.BenhNhan)
                        .Include(x=>x.NhomDichVuBenhVien)
                        .Include(x=>x.KetQuaXetNghiemChiTiets)
                        .Where(x=>x.ThoiDiemChiDinh != null
                                  && x.KetQuaXetNghiemChiTiets.Select(a=>a.ThoiDiemNhanKetQua).FirstOrDefault()!=null
                                  && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                  && (x.NhomDichVuBenhVien.Ma.Contains("XNHH")
                                  || x.NhomDichVuBenhVien.Ma.Contains("XNNT")
                                  || x.NhomDichVuBenhVien.Ma.Contains("XNHS")))
                  .Select(item => new BaoCaoKeHoachTongHopTheoNgayGridVo()
                  {
                      SoPhutChoKhamBenh = 0,
                      SoPhutChoXetNghiem = (float)Math.Round(((TimeSpan)(item.KetQuaXetNghiemChiTiets.Select(a => a.ThoiDiemNhanKetQua).FirstOrDefault() - item.ThoiDiemChiDinh)).TotalMinutes, 2),
                      SoPhutChoChuanDoanHA = 0,
                      SoPhutChoThamDoChucNang = 0
                  }).ToList();
            var result2 = _yeuCauDichVuKyThuatRepository.Table
                        .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan)
                        .Include(x => x.NhomDichVuBenhVien)
                        .Where(x => x.ThoiDiemChiDinh != null
                                  &&x.ThoiDiemThucHien != null
                                  && x.LoaiDichVuKyThuat==Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                  && (x.NhomDichVuBenhVien.Ma!="CDHACT"
                                  ||x.NhomDichVuBenhVien.Ma!="CDHAMRI")
                                  )
                  .Select(item => new BaoCaoKeHoachTongHopTheoNgayGridVo()
                  {
                      SoPhutChoKhamBenh = 0,
                      SoPhutChoXetNghiem = 0,
                      SoPhutChoChuanDoanHA = (float)Math.Round(((TimeSpan)(item.ThoiDiemThucHien-item.ThoiDiemThucHien)).TotalMinutes,2),
                      SoPhutChoThamDoChucNang = 0
                  }).ToList();
            var result3 = _yeuCauDichVuKyThuatRepository.Table
                        .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan)
                        .Include(x => x.NhomDichVuBenhVien)
                        .Where(x => x.ThoiDiemChiDinh != null
                                  && x.ThoiDiemThucHien != null
                                  && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                  &&(x.DichVuKyThuatBenhVien.Ma != "NS01"
                                    || x.DichVuKyThuatBenhVien.Ma != "20.0081.0137"
                                    || x.DichVuKyThuatBenhVien.Ma != "20.0071.0184"
                                    || x.DichVuKyThuatBenhVien.Ma != "20.0070.0500"))
                  .Select(item => new BaoCaoKeHoachTongHopTheoNgayGridVo()
                  {
                      SoPhutChoKhamBenh = 0,
                      SoPhutChoXetNghiem = 0,
                      SoPhutChoChuanDoanHA = 0,
                      SoPhutChoThamDoChucNang = (float)Math.Round(((TimeSpan)(item.ThoiDiemThucHien - item.ThoiDiemThucHien)).TotalMinutes, 2)
                  }).ToList();
            var kq = result.Concat(result1).ToList();
            var kq1 = kq.Concat(result2).ToList();
            var kq2 = kq1.Concat(result3).ToList();
            
            return new GridDataSource { Data = result.ToArray(), TotalRowCount = result.Count() };
        }
    }
}
