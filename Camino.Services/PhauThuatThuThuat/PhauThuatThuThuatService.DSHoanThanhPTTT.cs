using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        public async Task TuongTrinhLai(TuongTrinhLai tuongTrinhLai)
        {
            if (tuongTrinhLai.TuongTrinhLaiTheoYeuCauDichVuKyThuatId.Any())
            {
                var maxSoThuTuHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking.Select(x => x.SoThuTu).Max();
                List<PhongBenhVienHangDoi> lstPhongBenhVienHangDoi = new List<PhongBenhVienHangDoi>();

                List<string> lstPhieuLinhBu = new List<string>();
                List<string> lstPhieuLinhTrucTiep = new List<string>();

                foreach (var tuongTrinhLaiTheoYeuCauDichVuKyThuatId in tuongTrinhLai.TuongTrinhLaiTheoYeuCauDichVuKyThuatId)
                {
                    var yeuCauTuongTrinhLai = await BaseRepository.TableNoTracking.Where(c => c.Id == tuongTrinhLaiTheoYeuCauDichVuKyThuatId &&
                                                                                              c.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                                                                  .Include(p => p.PhongBenhVienHangDois)
                                                                                  .Include(p => p.YeuCauDuocPhamBenhViens).ThenInclude(p => p.YeuCauLinhDuocPham)
                                                                                  .Include(p => p.YeuCauDuocPhamBenhViens).ThenInclude(p => p.YeuCauLinhDuocPhamChiTiets).ThenInclude(p => p.YeuCauLinhDuocPham)
                                                                                  .Include(p => p.YeuCauDuocPhamBenhViens).ThenInclude(p => p.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                                                                                  .Include(p => p.YeuCauVatTuBenhViens).ThenInclude(p => p.YeuCauLinhVatTu)
                                                                                  .Include(p => p.YeuCauVatTuBenhViens).ThenInclude(p => p.YeuCauLinhVatTuChiTiets).ThenInclude(p => p.YeuCauLinhVatTu)
                                                                                  .Include(p => p.YeuCauVatTuBenhViens).ThenInclude(p => p.YeuCauTraVatTuTuBenhNhanChiTiets)
                                                                                  .FirstOrDefaultAsync();
                    if (yeuCauTuongTrinhLai != null)
                    {
                        /* Kiểm tra lĩnh bù */
                        var soPhieuYeuCauDuocPhamLinhBu = yeuCauTuongTrinhLai.YeuCauDuocPhamBenhViens.Where(p => p.YeuCauLinhDuocPhamId == null)
                                                                                                     .SelectMany(p => p.YeuCauLinhDuocPhamChiTiets)
                                                                                                     .Where(p => p.YeuCauLinhDuocPham.DuocDuyet != false)
                                                                                                     .Select(p => p.YeuCauLinhDuocPham.SoPhieu)
                                                                                                     .ToList();

                        var soPhieuYeuCauVatTuLinhBu = yeuCauTuongTrinhLai.YeuCauVatTuBenhViens.Where(p => p.YeuCauLinhVatTuId == null)
                                                                                               .SelectMany(p => p.YeuCauLinhVatTuChiTiets)
                                                                                               .Where(p => p.YeuCauLinhVatTu.DuocDuyet != false)
                                                                                               .Select(p => p.YeuCauLinhVatTu.SoPhieu)
                                                                                               .ToList();

                        lstPhieuLinhBu.AddRange(soPhieuYeuCauDuocPhamLinhBu);
                        lstPhieuLinhBu.AddRange(soPhieuYeuCauVatTuLinhBu);
                        /* */

                        /* Kiểm tra lĩnh trực tiếp */
                        var soPhieuYeuCauDuocPhamLinhTrucTiep = yeuCauTuongTrinhLai.YeuCauDuocPhamBenhViens.Where(p => p.YeuCauLinhDuocPhamId != null &&
                                                                                                                       p.YeuCauLinhDuocPham.DuocDuyet != false &&
                                                                                                                       p.SoLuong > 0)
                                                                                                           .Select(p => p.YeuCauLinhDuocPham.SoPhieu)
                                                                                                           .ToList();

                        var soPhieuYeuCauVatTuLinhTrucTiep = yeuCauTuongTrinhLai.YeuCauVatTuBenhViens.Where(p => p.YeuCauLinhVatTuId != null &&
                                                                                                                 p.YeuCauLinhVatTu.DuocDuyet != false &&
                                                                                                                 p.SoLuong > 0)
                                                                                                     .Select(p => p.YeuCauLinhVatTu.SoPhieu)
                                                                                                     .ToList();

                        lstPhieuLinhTrucTiep.AddRange(soPhieuYeuCauDuocPhamLinhTrucTiep);
                        lstPhieuLinhTrucTiep.AddRange(soPhieuYeuCauVatTuLinhTrucTiep);
                        /* */

                        /* Thêm hàng đợi */
                        var phongBenhVienHangDoiEntity = new PhongBenhVienHangDoi();

                        phongBenhVienHangDoiEntity.PhongBenhVienId = (long)yeuCauTuongTrinhLai.NoiThucHienId;
                        phongBenhVienHangDoiEntity.YeuCauTiepNhanId = yeuCauTuongTrinhLai.YeuCauTiepNhanId;
                        phongBenhVienHangDoiEntity.YeuCauDichVuKyThuatId = tuongTrinhLaiTheoYeuCauDichVuKyThuatId;
                        phongBenhVienHangDoiEntity.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                        phongBenhVienHangDoiEntity.SoThuTu = maxSoThuTuHangDoi + 1;

                        lstPhongBenhVienHangDoi.Add(phongBenhVienHangDoiEntity);

                        maxSoThuTuHangDoi += 1;
                        /* */

                        //yeuCauTuongTrinhLai.PhongBenhVienHangDois.Add(phongBenhVienHangDoiEntity);
                        //await BaseRepository.UpdateAsync(yeuCauTuongTrinhLai);
                    }
                }

                if (lstPhieuLinhBu.Any())
                {
                    lstPhieuLinhBu = lstPhieuLinhBu.GroupBy(p => p).Select(p => p.Key).ToList();

                    throw new Exception(string.Format(_localizationService.GetResource("PTTT.TuongTrinhLai.TonTaiPhieuLinh"), string.Join(", ", lstPhieuLinhBu.OrderBy(p => p))));
                }
                else
                {
                    lstPhieuLinhTrucTiep = lstPhieuLinhTrucTiep.GroupBy(p => p).Select(p => p.Key).ToList();

                    if (lstPhieuLinhTrucTiep.Any())
                    {
                        throw new Exception(string.Format(_localizationService.GetResource("PTTT.TuongTrinhLai.TonTaiPhieuLinhTrucTiep"), string.Join(", ", lstPhieuLinhTrucTiep.OrderBy(p => p))));
                    }
                }

                await _phongBenhVienHangDoiRepository.AddRangeAsync(lstPhongBenhVienHangDoi);
            }

        }

        public async Task<GridDataSource> GetDataForGridAsyncDSHTPhauThuatThuThuat(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var query = BuildQueryHoanThanhPTTT(queryInfo);
            //var dataSreach = BaseRepository.TableNoTracking.Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.BenhNhan)
            //                                               .Include(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
            //                                               .Include(c => c.TheoDoiSauPhauThuatThuThuat)
            //                                               .Include(c => c.YeuCauTiepNhan).ThenInclude(c => c.NoiTruBenhAn)
            //                                               .Include(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
            //        .Where(yckt =>yckt.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan==EnumTrangThaiYeuCauTiepNhan.DangThucHien &&  yckt.ThoiDiemHoanThanh != null && yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && (yckt.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien));

            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{
            //    var queryString = JsonConvert.DeserializeObject<DanhSachHoanThanhPTTTVo>(queryInfo.AdditionalSearchString);
            //    if (!string.IsNullOrEmpty(queryString.SearchString))
            //    {
            //        var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
            //        dataSreach = dataSreach.ApplyLike(searchTerms,
            //            g => g.YeuCauTiepNhan.HoTen,
            //            g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //            g => g.YeuCauTiepNhan.NamSinh.ToString(),
            //            g => g.YeuCauTiepNhan.BenhNhan.MaBN,
            //            g => g.YeuCauTiepNhan.DiaChiDayDu
            //       );
            //    }
            //}

            //var dataGroupBySoLans = dataSreach.GroupBy(x => new { x.LanThucHien, x.YeuCauTiepNhanId }).OrderByDescending(g => g.Key.YeuCauTiepNhanId).ThenBy(g => g.Key.LanThucHien)
            //                                     .Select(g => new { YeuCauKyThuats = g.FirstOrDefault(), ListYeuCauKTIds = g.Select(c => c.Id) });

            //var DanhSachHoanThanhPTTTVos = new List<DanhSachHoanThanhPTTTVo>();
            //foreach (var group in dataGroupBySoLans)
            //{
            //    var phongBenhVienHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
            //                                                             .Where(x => x.YeuCauTiepNhanId == group.YeuCauKyThuats.YeuCauTiepNhanId &&
            //                                                                    x.YeuCauDichVuKyThuatId == group.YeuCauKyThuats.Id);
            //    if (!phongBenhVienHangDoi.Any())
            //    {
            //        var DanhSachHoanThanhPTTTVo = new DanhSachHoanThanhPTTTVo();
            //        DanhSachHoanThanhPTTTVo.Id = group.YeuCauKyThuats.Id;
            //        DanhSachHoanThanhPTTTVo.YeuCauTiepNhanId = group.YeuCauKyThuats.YeuCauTiepNhanId;
            //        DanhSachHoanThanhPTTTVo.PhongBenhVienId = group.YeuCauKyThuats.NoiThucHienId;
            //        DanhSachHoanThanhPTTTVo.NoiTiepNhanId = group.YeuCauKyThuats.YeuCauTiepNhan.NoiTiepNhanId;
            //        DanhSachHoanThanhPTTTVo.MaYeuCauTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.MaYeuCauTiepNhan;
            //        DanhSachHoanThanhPTTTVo.BenhNhanId = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhanId;
            //        DanhSachHoanThanhPTTTVo.MaBN = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhan.MaBN;
            //        DanhSachHoanThanhPTTTVo.HoTen = group.YeuCauKyThuats.YeuCauTiepNhan.HoTen;
            //        DanhSachHoanThanhPTTTVo.NamSinh = group.YeuCauKyThuats.YeuCauTiepNhan.NamSinh;
            //        DanhSachHoanThanhPTTTVo.DiaChi = group.YeuCauKyThuats.YeuCauTiepNhan.DiaChiDayDu;
            //        DanhSachHoanThanhPTTTVo.ThoiDiemHoanThanh = group.YeuCauKyThuats.ThoiDiemHoanThanh.GetValueOrDefault();
            //        DanhSachHoanThanhPTTTVo.DoiTuong = group.YeuCauKyThuats.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + group.YeuCauKyThuats.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)";
            //        DanhSachHoanThanhPTTTVo.TrangThaiPTTTSearch = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuats?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "Đã tử vong" : "Đã chuyển giao";
            //        DanhSachHoanThanhPTTTVo.TuVongTrongPTTT = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuats?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? true : false;
            //        DanhSachHoanThanhPTTTVo.SoLan = group.YeuCauKyThuats.LanThucHien;
            //        DanhSachHoanThanhPTTTVo.ThoiDiemThucHien = group.YeuCauKyThuats.ThoiDiemThucHien;

            //        DanhSachHoanThanhPTTTVo.NoiChuyenGiao = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT != null) || group.YeuCauKyThuats?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "" : group.YeuCauKyThuats.NoiChiDinh?.KhoaPhong?.Ten;

            //        DanhSachHoanThanhPTTTVo.TuongTrinhLaiYeuCauKyThuatIds = group.ListYeuCauKTIds.Any() ? group.ListYeuCauKTIds.ToList() : null;
            //        DanhSachHoanThanhPTTTVo.DuocTuongTrinhLai = group.YeuCauKyThuats.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
            //                                                    (group.YeuCauKyThuats.YeuCauTiepNhan.NoiTruBenhAn == null || group.YeuCauKyThuats.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null);
            //        DanhSachHoanThanhPTTTVos.Add(DanhSachHoanThanhPTTTVo);
            //    }

            //}

            //var query = DanhSachHoanThanhPTTTVos.AsQueryable();
            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{

            //    var queryString = JsonConvert.DeserializeObject<DanhSachHoanThanhPTTTVo>(queryInfo.AdditionalSearchString);
            //    query = query.Where(p => p.PhongBenhVienId == queryString.PhongBenhVienId);

            //    if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            //    {
            //        DateTime denNgay;
            //        queryString.FromDate.TryParseExactCustom(out var tuNgay);
            //        //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
            //        if (string.IsNullOrEmpty(queryString.ToDate))
            //        {
            //            denNgay = DateTime.Now;
            //        }
            //        else
            //        {
            //            queryString.ToDate.TryParseExactCustom(out denNgay);
            //            //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            //        }
            //        //denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            //        query = query.Where(p => p.ThoiDiemHoanThanh >= tuNgay && p.ThoiDiemHoanThanh <= denNgay);
            //    }
            //}

            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncDSHTPhauThuatThuThuat(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            //var dataSreach = BaseRepository.TableNoTracking.Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.BenhNhan).Include(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
            //          .Include(c => c.TheoDoiSauPhauThuatThuThuat)
            //         .Where(yckt => yckt.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien && yckt.ThoiDiemHoanThanh != null && yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat && (yckt.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien));
                                                           //.Include(c => c.TheoDoiSauPhauThuatThuThuat)
                                                           //.Include(c => c.YeuCauTiepNhan).ThenInclude(c => c.NoiTruBenhAn)

            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{
            //    var queryString = JsonConvert.DeserializeObject<DanhSachHoanThanhPTTTVo>(queryInfo.AdditionalSearchString);
            //    if (!string.IsNullOrEmpty(queryString.SearchString))
            //    {
            //        var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
            //        dataSreach = dataSreach.ApplyLike(searchTerms,
            //            g => g.YeuCauTiepNhan.HoTen,
            //            g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //            g => g.YeuCauTiepNhan.NamSinh.ToString(),
            //            g => g.YeuCauTiepNhan.BenhNhan.MaBN,
            //            g => g.YeuCauTiepNhan.DiaChiDayDu
            //       );
            //    }
            //}

            //var dataGroupBySoLans = dataSreach.GroupBy(x => new { x.LanThucHien, x.YeuCauTiepNhanId }).OrderByDescending(g => g.Key.YeuCauTiepNhanId).ThenBy(g => g.Key.LanThucHien)
            //                                     .Select(g => new { YeuCauKyThuats = g.FirstOrDefault() });

            //var DanhSachHoanThanhPTTTVos = new List<DanhSachHoanThanhPTTTVo>();
            //foreach (var group in dataGroupBySoLans)
            //{
            //    var phongBenhVienHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
            //                                                            .Where(x => x.YeuCauTiepNhanId == group.YeuCauKyThuats.YeuCauTiepNhanId &&
            //                                                                   x.YeuCauDichVuKyThuatId == group.YeuCauKyThuats.Id);
            //    if (!phongBenhVienHangDoi.Any())
            //    {
            //        var DanhSachHoanThanhPTTTVo = new DanhSachHoanThanhPTTTVo();
            //        DanhSachHoanThanhPTTTVo.Id = group.YeuCauKyThuats.Id;
            //        DanhSachHoanThanhPTTTVo.YeuCauTiepNhanId = group.YeuCauKyThuats.YeuCauTiepNhanId;
            //        DanhSachHoanThanhPTTTVo.PhongBenhVienId = group.YeuCauKyThuats.NoiThucHienId;
            //        DanhSachHoanThanhPTTTVo.NoiTiepNhanId = group.YeuCauKyThuats.YeuCauTiepNhan.NoiTiepNhanId;
            //        DanhSachHoanThanhPTTTVo.MaYeuCauTiepNhan = group.YeuCauKyThuats.YeuCauTiepNhan.MaYeuCauTiepNhan;
            //        DanhSachHoanThanhPTTTVo.BenhNhanId = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhanId;
            //        DanhSachHoanThanhPTTTVo.MaBN = group.YeuCauKyThuats.YeuCauTiepNhan.BenhNhan.MaBN;
            //        DanhSachHoanThanhPTTTVo.HoTen = group.YeuCauKyThuats.YeuCauTiepNhan.HoTen;
            //        DanhSachHoanThanhPTTTVo.NamSinh = group.YeuCauKyThuats.YeuCauTiepNhan.NamSinh;
            //        DanhSachHoanThanhPTTTVo.DiaChi = group.YeuCauKyThuats.YeuCauTiepNhan.DiaChiDayDu;
            //        DanhSachHoanThanhPTTTVo.ThoiDiemHoanThanh = group.YeuCauKyThuats.ThoiDiemHoanThanh.GetValueOrDefault();
            //        DanhSachHoanThanhPTTTVo.TrangThaiPTTTSearch = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuats?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "Đã tử vong" : "Đã chuyển giao";
            //        DanhSachHoanThanhPTTTVo.TuVongTrongPTTT = ((group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuats.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuats?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? true : false;
            //        DanhSachHoanThanhPTTTVo.SoLan = group.YeuCauKyThuats.LanThucHien;
            //        DanhSachHoanThanhPTTTVo.ThoiDiemThucHien = group.YeuCauKyThuats.ThoiDiemThucHien;
            //        DanhSachHoanThanhPTTTVos.Add(DanhSachHoanThanhPTTTVo);
            //    }
            //}

            //var query = DanhSachHoanThanhPTTTVos.AsQueryable();
            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{

            //    var queryString = JsonConvert.DeserializeObject<DanhSachHoanThanhPTTTVo>(queryInfo.AdditionalSearchString);
            //    query = query.Where(p => p.PhongBenhVienId == queryString.PhongBenhVienId);

            //    if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
            //    {
            //        DateTime denNgay;
            //        queryString.FromDate.TryParseExactCustom(out var tuNgay);
            //        //                    DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None,
            //        //                        out var tuNgay);
            //        if (string.IsNullOrEmpty(queryString.ToDate))
            //        {
            //            denNgay = DateTime.Now;
            //        }
            //        else
            //        {
            //            queryString.ToDate.TryParseExactCustom(out denNgay);
            //            //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
            //        }
            //        //denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);
            //        query = query.Where(p => p.ThoiDiemHoanThanh >= tuNgay && p.ThoiDiemHoanThanh <= denNgay);
            //    }

            //}

            var query = BuildQueryHoanThanhPTTT(queryInfo);
            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }
        private IQueryable<DanhSachHoanThanhPTTTVo> BuildQueryHoanThanhPTTT(QueryInfo queryInfo)
        {
            string searchString = null;
            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            long? phongBenhVienId = null;
            DanhSachKhamPTHoanThanhThucHienVo thongTinThucHien = null;

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                searchString = queryInfo.SearchTerms.Replace("\t", "").Trim();
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {

                var queryString = JsonConvert.DeserializeObject<LichSuPhauThuatThuThuatGridVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    if (queryString.FromDate != null && !string.IsNullOrEmpty(queryString.FromDate))
                    {
                        DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                        tuNgay = tuNgayTemp;
                    }

                    if (queryString.ToDate != null && !string.IsNullOrEmpty(queryString.ToDate))
                    {
                        DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                        denNgay = denNgayTemp.AddSeconds(59).AddMilliseconds(999);
                    }
                }
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    searchString = queryString.SearchString.Replace("\t", "").Trim();
                }
                if (queryString.PhongBenhVienId != null)
                {
                    phongBenhVienId = queryString.PhongBenhVienId;
                }

                //BVHD-3860
                thongTinThucHien = queryString.ThongTinThucHien;
            }

            //var dataSreach = BaseRepository.TableNoTracking.Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.BenhNhan)
            //                                               .Include(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
            //                                               .Include(c => c.TheoDoiSauPhauThuatThuThuat)
            //                                               .Include(c => c.YeuCauTiepNhan).ThenInclude(c => c.NoiTruBenhAn)
            //                                               .Include(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
            //        .Where(yckt => yckt.ThoiDiemHoanThanh != null && yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
            //        //yckt.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien &&
            //        (yckt.NoiThucHienId == phongBenhVienId) &&
            //        (tuNgay == null || yckt.ThoiDiemHoanThanh >= tuNgay) && (denNgay == null || yckt.ThoiDiemHoanThanh <= denNgay)).ApplyLike(searchString,
            //            g => g.YeuCauTiepNhan.HoTen,
            //            g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //            g => g.YeuCauTiepNhan.NamSinh.ToString(),
            //            g => g.YeuCauTiepNhan.BenhNhan.MaBN,
            //            g => g.YeuCauTiepNhan.DiaChiDayDu
            //       );

            #region BVHD-3860: 
            var dataSreach = BaseRepository.TableNoTracking.Include(cc => cc.YeuCauTiepNhan).ThenInclude(cc => cc.BenhNhan)
                .Include(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .Include(c => c.TheoDoiSauPhauThuatThuThuat)
                .Include(c => c.YeuCauTiepNhan).ThenInclude(c => c.NoiTruBenhAn)
                .Include(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                .Where(yckt => yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                               && (yckt.NoiThucHienId == phongBenhVienId) 
                               
                               && (
                                   // trường hợp dv không thực hiện
                                   (yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != null 
                                       && yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true
                                       && (tuNgay == null || yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh >= tuNgay)
                                       && (denNgay == null || yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh <= denNgay))

                                   // trường hợp dịch vụ đã tường trình
                                   || (yckt.ThoiDiemHoanThanh != null 
                                       && (tuNgay == null || yckt.ThoiDiemHoanThanh >= tuNgay)
                                       && (denNgay == null || yckt.ThoiDiemHoanThanh <= denNgay))
                                   )
                               && (
                                   thongTinThucHien == null
                                   || (thongTinThucHien.ThucHien == thongTinThucHien.KhongThucHien)
                                   || (thongTinThucHien.ThucHien == true && yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
                                   || (thongTinThucHien.KhongThucHien == true && yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != null && yckt.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true)
                                    )
                        )
                .ApplyLike(searchString,
                    g => g.YeuCauTiepNhan.HoTen,
                    g => g.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    g => g.YeuCauTiepNhan.NamSinh.ToString(),
                    g => g.YeuCauTiepNhan.BenhNhan.MaBN,
                    g => g.YeuCauTiepNhan.DiaChiDayDu
                ).ToList();


            #endregion


            var dataGroupBySoLans = dataSreach.GroupBy(x => new { x.LanThucHien, x.YeuCauTiepNhanId })
                                                .OrderByDescending(g => g.Key.YeuCauTiepNhanId)
                                                .ThenBy(g => g.Key.LanThucHien)
                                                .Select(g => new
                                                {
                                                    //BVHD-3860
                                                    YeuCauKyThuat = g.OrderByDescending(a => a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != true).ThenBy(a => a.Id).FirstOrDefault(),
                                                    ListYeuCauKTIds = g.Where(a => a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != true).Select(c => c.Id),
                                                    CountYeuCauKyThuatTuongTrinh = g.Count(a => a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != null && a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien == true)
                                                });

            var yeuCauDichVuKyThuatIds = dataSreach
                    //BVHD-3860
                    .Where(a => a.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != true)
                    .Select(o => o.Id).ToList();
            var phongBenhVienHangDois = _phongBenhVienHangDoiRepository.TableNoTracking
                                                                         .Where(x => x.YeuCauDichVuKyThuatId != null && yeuCauDichVuKyThuatIds.Any(o => o == (long)x.YeuCauDichVuKyThuatId))
                                                                         .Select(o => new { YeuCauTiepNhanId = o.YeuCauTiepNhanId, YeuCauDichVuKyThuatId = o.YeuCauDichVuKyThuatId }).ToList();
            var danhSachHoanThanhPTTTVos = new List<DanhSachHoanThanhPTTTVo>();
            foreach (var group in dataGroupBySoLans)
            {
                var phongBenhVienHangDoi = phongBenhVienHangDois.Where(x => x.YeuCauTiepNhanId == group.YeuCauKyThuat.YeuCauTiepNhanId &&
                                                                                x.YeuCauDichVuKyThuatId == group.YeuCauKyThuat.Id);
                if (!phongBenhVienHangDoi.Any())
                {
                    var danhSachHoanThanhPTTTVo = new DanhSachHoanThanhPTTTVo();
                    danhSachHoanThanhPTTTVo.Id = group.YeuCauKyThuat.Id;
                    danhSachHoanThanhPTTTVo.YeuCauTiepNhanId = group.YeuCauKyThuat.YeuCauTiepNhanId;
                    danhSachHoanThanhPTTTVo.PhongBenhVienId = group.YeuCauKyThuat.NoiThucHienId;
                    danhSachHoanThanhPTTTVo.NoiTiepNhanId = group.YeuCauKyThuat.YeuCauTiepNhan.NoiTiepNhanId;
                    danhSachHoanThanhPTTTVo.MaYeuCauTiepNhan = group.YeuCauKyThuat.YeuCauTiepNhan.MaYeuCauTiepNhan;
                    danhSachHoanThanhPTTTVo.BenhNhanId = group.YeuCauKyThuat.YeuCauTiepNhan.BenhNhanId;
                    danhSachHoanThanhPTTTVo.MaBN = group.YeuCauKyThuat.YeuCauTiepNhan.BenhNhan.MaBN;
                    danhSachHoanThanhPTTTVo.HoTen = group.YeuCauKyThuat.YeuCauTiepNhan.HoTen;
                    danhSachHoanThanhPTTTVo.NamSinh = group.YeuCauKyThuat.YeuCauTiepNhan.NamSinh;
                    danhSachHoanThanhPTTTVo.DiaChi = group.YeuCauKyThuat.YeuCauTiepNhan.DiaChiDayDu;
                    danhSachHoanThanhPTTTVo.ThoiDiemHoanThanh = group.YeuCauKyThuat.ThoiDiemHoanThanh ?? group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT?.ThoiDiemKetThucTuongTrinh;
                    danhSachHoanThanhPTTTVo.DoiTuong = group.YeuCauKyThuat.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + group.YeuCauKyThuat.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)";
                    danhSachHoanThanhPTTTVo.TrangThaiPTTTSearch = ((group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuat?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "Đã tử vong" : "Đã chuyển giao";
                    danhSachHoanThanhPTTTVo.TuVongTrongPTTT = ((group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT.HasValue) || group.YeuCauKyThuat?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? true : false;
                    danhSachHoanThanhPTTTVo.SoLan = group.YeuCauKyThuat.LanThucHien;
                    danhSachHoanThanhPTTTVo.ThoiDiemThucHien = group.YeuCauKyThuat.ThoiDiemThucHien;

                    danhSachHoanThanhPTTTVo.NoiChuyenGiao = ((group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null && group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.TuVongTrongPTTT != null) || group.YeuCauKyThuat?.TheoDoiSauPhauThuatThuThuat?.ThoiDiemTuVong != null) ? "" : group.YeuCauKyThuat.NoiChiDinh?.KhoaPhong?.Ten;

                    danhSachHoanThanhPTTTVo.TuongTrinhLaiYeuCauKyThuatIds = group.ListYeuCauKTIds.Any() ? group.ListYeuCauKTIds.ToList() : null;
                    danhSachHoanThanhPTTTVo.DuocTuongTrinhLai = group.YeuCauKyThuat.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien 
                                                                && (group.YeuCauKyThuat.YeuCauTiepNhan.NoiTruBenhAn == null || group.YeuCauKyThuat.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null)

                                                                //BVHD-3860: nếu có 1 dv mà dv đó không thực hiện PT thì ko cho tường trình lại
                                                                && group.ListYeuCauKTIds.Any();

                    //BVHD-3860
                    danhSachHoanThanhPTTTVo.SoDichVuKhongTuongTrinh = group.CountYeuCauKyThuatTuongTrinh;
                    danhSachHoanThanhPTTTVo.LaKhongThucHien = group.YeuCauKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien;

                    danhSachHoanThanhPTTTVos.Add(danhSachHoanThanhPTTTVo);
                }

            }
            return danhSachHoanThanhPTTTVos.AsQueryable();
        }

        #region CanLamSang

        public async Task<GridDataSource> GetDataCanLamSangForGridAsync(QueryInfo queryInfo)
        {
            var yeuCauTiepNhanSolanPTT = queryInfo.AdditionalSearchString.Split(',');

            // 1 là yêu cầu tiếp nhận 
            var yeuCauTiepNhanId = long.Parse(yeuCauTiepNhanSolanPTT[0]);

            var yeuCauTiepNhan = await _yeuCauTiepNhanRepository.TableNoTracking.Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.FileKetQuaCanLamSangs)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                                                                              .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                                                                              .Include(p => p.KetQuaNhomXetNghiems).ThenInclude(p => p.FileKetQuaCanLamSangs)
                                                                              .Where(p => p.Id == yeuCauTiepNhanId)
                                                                              .FirstOrDefaultAsync();

            var phauThuatThuThuatCanLamSanGridVo = new List<PhauThuatThuThuatCanLamSanGridVo>();
            if (yeuCauTiepNhan != null)
            {
                phauThuatThuThuatCanLamSanGridVo = GetDataCanLamSang(yeuCauTiepNhan);
            }

            var result = phauThuatThuThuatCanLamSanGridVo.AsQueryable();
            var queryTask = result.ToArray();
            return new GridDataSource { Data = queryTask.ToArray() };
        }

        public GridDataSource GetTotalCanLamSangPageForGridAsync(QueryInfo queryInfo)
        {
            var index = 1;
            var yeuCauTiepNhanSolanPTT = queryInfo.AdditionalSearchString.Split(',');
            var yeuCauTiepNhanId = long.Parse(yeuCauTiepNhanSolanPTT[0]);
            var query = BaseRepository.TableNoTracking
               .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId).Include(cc => cc.FileKetQuaCanLamSangs)
               .Select(s => new LichSuCLSPTTTVo
               {
                   STT = index + 1,
                   Id = s.Id,
                   TenDichVu = s.TenDichVu,
                   MaDichVu = s.MaDichVu,
                   SoLan = s.SoLan,
                   DonGia = s.Gia,
                   NhanVienThucHienId = s.NhanVienThucHienId,
                   NhanVienThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                   NoiThucHienId = s.NoiThucHienId,
                   NoiThucHien = s.NoiThucHien != null ? s.NoiThucHien.Ma + " - " + s.NoiThucHien.Ten : null,
                   TenLoaiGia = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                   DonGiaBaoHiem = s.DonGiaBaoHiem,
                   DuocHuongBaoHiem = s.DuocHuongBaoHiem != false ? "Có" : "Không",
                   TrangThai = s.TrangThai.GetDescription(),
                   ThanhTien = s.Gia * s.SoLan,
                   Nhom = s.LoaiDichVuKyThuat.GetDescription(),
                   NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                   TrangThaiDichVuId = (int)s.TrangThai
               });

            var result = query.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        private List<PhauThuatThuThuatCanLamSanGridVo> GetDataCanLamSang(YeuCauTiepNhan yeuCauTiepNhan)
        {
            long userId = _userAgentHelper.GetCurrentUserId();

            var goiDichVuKhamBenh = new List<PhauThuatThuThuatCanLamSanGridVo>();
            var stt = 1;

            goiDichVuKhamBenh.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats?.Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                           && (p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                                                                           .OrderBy(x => x.CreatedOn)
                                                                           .Select(p => new PhauThuatThuThuatCanLamSanGridVo
                                                                           {
                                                                               //STT = stt++,
                                                                               Id = p.Id,
                                                                               Nhom = GetTenNhomCha(p.NhomDichVuBenhVienId),
                                                                               NhomDichVuBenhVienId = p.NhomDichVuBenhVienId,
                                                                               NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                                                                               LoaiDichVuKyThuat = (int)p.LoaiDichVuKyThuat,
                                                                               LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien?.Id,
                                                                               NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                                                                               Ma = p.DichVuKyThuatBenhVien?.Ma,
                                                                               MaGiaDichVu = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaGia,
                                                                               MaTT37 = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.Ma4350,
                                                                               TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                                                                               TenLoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                                                                               LoaiGia = p.NhomGiaDichVuKyThuatBenhVienId,
                                                                               DonGia = p.Gia,
                                                                               //GiaBaoHiemThanhToan = p.GiaBaoHiemThanhToan ?? 0,
                                                                               ThanhTien = 0,
                                                                               BHYTThanhToan = 0,
                                                                               BNThanhToan = 0,
                                                                               NoiThucHien = String.Format("{0} - {1}", p.NoiThucHien?.Ma, p.NoiThucHien?.Ten),
                                                                               NoiThucHienId = p.NoiThucHienId ?? 0,
                                                                               TenNguoiThucHien = p.NhanVienThucHien?.User.HoTen,
                                                                               ThoiDiemThucHienDisplay = p.ThoiDiemThucHien?.ApplyFormatDateTime(),
                                                                               NguoiThucHienId = p.NhanVienThucHienId,
                                                                               SoLuong = Convert.ToDouble(p.SoLan),
                                                                               TrangThaiDichVu = p.TrangThai.GetDescription(),
                                                                               TrangThaiDichVuId = (int)p.TrangThai,
                                                                               NhomChiPhiDichVuKyThuatId = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.NhomChiPhi,
                                                                               KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                                                                               isCheckRowItem = false,
                                                                               DaThanhToan = p.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan,
                                                                               DonGiaBaoHiem = p.DonGiaBaoHiem,
                                                                               DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                                                                               NhanVienTaoYeuCauDichVuKyThuatId = p.CreatedById,
                                                                               ThoiDiemChiDinh = p.ThoiDiemChiDinh,
                                                                               NguoiChiDinhDisplay = p.NhanVienChiDinh?.User.HoTen,
                                                                               //todo need update FileKetQuaCanLamSangs
                                                                               //DuongDan = p.FileKetQuaCanLamSangs.Any() ? p.FileKetQuaCanLamSangs.First().DuongDan : p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? p.YeuCauTiepNhan.KetQuaNhomXetNghiems.Where(cc => cc.NhomDichVuBenhVienId == p.NhomDichVuBenhVienId).SelectMany(c => c.FileKetQuaCanLamSangs).Select(c => c.DuongDan).FirstOrDefault() : "",
                                                                               //TenGuid = p.FileKetQuaCanLamSangs.Any() ? p.FileKetQuaCanLamSangs.First().TenGuid : p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? p.YeuCauTiepNhan.KetQuaNhomXetNghiems.Where(cc => cc.NhomDichVuBenhVienId == p.NhomDichVuBenhVienId).SelectMany(c => c.FileKetQuaCanLamSangs).Select(c => c.TenGuid).FirstOrDefault() : "",
                                                                               //LoaiTapTin = p.FileKetQuaCanLamSangs.Any() ? (int)p.FileKetQuaCanLamSangs.First().LoaiTapTin : p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? (int)p.YeuCauTiepNhan.KetQuaNhomXetNghiems.Where(cc => cc.NhomDichVuBenhVienId == p.NhomDichVuBenhVienId).SelectMany(c => c.FileKetQuaCanLamSangs).Select(c => c.LoaiTapTin).FirstOrDefault() : 0,
                                                                               KetQuaCanLamSanPTTT = p.FileKetQuaCanLamSangs.Select(p2 => new KetQuaCanLamSanPTTT
                                                                               {
                                                                                   TenFile = p2.Ten,
                                                                                   Url = _taiLieuDinhKemService.GetTaiLieuUrl(p2.DuongDan, p2.TenGuid),
                                                                                   //LoaiFile = p2.Ten.Substring(p2.Ten.LastIndexOf('.') + 1).ToLower(),
                                                                                   LoaiTapTin = p2.LoaiTapTin,
                                                                                   MoTa = p2.MoTa,
                                                                                   TenGuid = p2.TenGuid,
                                                                                   DuongDan = p2.DuongDan
                                                                               }).ToList()
                                                                               //DuongDan = p.FileKetQuaCanLamSangId != null ? p.FileKetQuaCanLamSang.DuongDan : p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? p.YeuCauTiepNhan.KetQuaNhomXetNghiems.Where(cc => cc.NhomDichVuBenhVienId == p.NhomDichVuBenhVienId).Select(c => c.FileKetQuaCanLamSang.DuongDan).FirstOrDefault() : "",
                                                                               //TenGuid = p.FileKetQuaCanLamSangId != null ? p.FileKetQuaCanLamSang.TenGuid : p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? p.YeuCauTiepNhan.KetQuaNhomXetNghiems.Where(cc => cc.NhomDichVuBenhVienId == p.NhomDichVuBenhVienId).Select(c => c.FileKetQuaCanLamSang.TenGuid).FirstOrDefault() : "",
                                                                               //LoaiTapTin = p.FileKetQuaCanLamSangId != null ? (int)p.FileKetQuaCanLamSang.LoaiTapTin : p.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? (int)p.YeuCauTiepNhan.KetQuaNhomXetNghiems.Where(cc => cc.NhomDichVuBenhVienId == p.NhomDichVuBenhVienId).Select(c => c.FileKetQuaCanLamSang.LoaiTapTin).FirstOrDefault() : 0,
                                                                           }));

            //Sort
            goiDichVuKhamBenh = goiDichVuKhamBenh.OrderBy(p => p.Nhom.ToUpper()).ToList();
            foreach (var item in goiDichVuKhamBenh)
            {
                item.STT = stt++;
            }

            var ketQuaNhomXetNghiem = _ketQuaNhomXetNghiemsRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhan.Id)
                                                                                     .Select(p => new
                                                                                     {
                                                                                         Id = p.Id,
                                                                                         NhomDichVuBenhVienId = p.NhomDichVuBenhVienId
                                                                                     })
                                                                                     .Distinct()
                                                                                     .ToList();

            //apply lại hình ảnh
            foreach (var item in goiDichVuKhamBenh)
            {
                if (ketQuaNhomXetNghiem.Any(p => p.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId))
                {
                    //item.TenGuid = queryXetNghiem.Where(p => p.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId).Select(p => p.TenGuid).FirstOrDefault();
                    //item.DuongDan = queryXetNghiem.Where(p => p.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId).Select(p => p.DuongDan).FirstOrDefault();
                    var kqNhomXetNghiemId = ketQuaNhomXetNghiem.Where(p => p.NhomDichVuBenhVienId == item.NhomDichVuBenhVienId).Select(p => p.Id).FirstOrDefault();

                    item.KetQuaCanLamSanPTTT = _fileKetQuaCanLamSanRepository.TableNoTracking.Where(p => p.KetQuaNhomXetNghiemId == kqNhomXetNghiemId)
                                                                                             .Select(p => new KetQuaCanLamSanPTTT
                                                                                             {
                                                                                                 TenFile = p.Ten,
                                                                                                 Url = _taiLieuDinhKemService.GetTaiLieuUrl(p.DuongDan, p.TenGuid),
                                                                                                 //LoaiFile = p.Ten.Substring(p.Ten.LastIndexOf('.') + 1).ToLower(),
                                                                                                 LoaiTapTin = p.LoaiTapTin,
                                                                                                 MoTa = p.MoTa,
                                                                                                 TenGuid = p.TenGuid,
                                                                                                 DuongDan = p.DuongDan
                                                                                             }).ToList();
                }
            }

            //tính toán tiền cho các dịch vụ
            foreach (var itemx in goiDichVuKhamBenh)
            {
                decimal? thanhtien = itemx.DonGia * (decimal)itemx.SoLuong ?? 0;
                decimal? thanhTienBHTT = itemx.GiaBaoHiemThanhToan * (decimal)itemx.SoLuong ?? 0;

                itemx.ThanhTien = thanhtien;
                itemx.BHYTThanhToan = thanhTienBHTT;
                itemx.BNThanhToan = itemx.KhongTinhPhi != true ? (thanhtien - thanhTienBHTT) : 0;
            }

            return goiDichVuKhamBenh;
        }

        #endregion
        public async Task<List<LookupTrangThaiPtttVo>> GetDichVuHoanThanh(LookupQueryInfo queryInfo, long noiThucHienId, long yctnId, long soLan)
        {
            var users = _userRepository.TableNoTracking.ToDictionary(cc => cc.Id, cc => cc.HoTen);
            var bnPtttsQuery = BaseRepository.TableNoTracking
             .Where(e => e.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && e.LanThucHien == soLan &&
                            e.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            e.YeuCauTiepNhanId == yctnId)
                .Select(w => new LookupTrangThaiPtttVo
                {
                    KeyId = w.Id,
                    BacSi = w.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis.FirstOrDefault(s => s.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh).NhanVien != null ?
                       users[w.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis.FirstOrDefault(s => s.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh).NhanVien.Id] : string.Empty,
                    TenDv = w.TenDichVu,
                    IsDaTuongTrinh = w.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null,
                    DichVuKyThuatBenhVienId = w.DichVuKyThuatBenhVienId,
                    IsKhongTuongTrinh = w.YeuCauDichVuKyThuatTuongTrinhPTTT == null
                }).ApplyLike(queryInfo.Query,
                    e => e.TenDv);

            var ptttBnResult = await bnPtttsQuery.Where(cc => cc.IsDaTuongTrinh || cc.IsKhongTuongTrinh).Take(queryInfo.Take)
                .OrderBy(w => w.IsDaTuongTrinh).ThenBy(e => e.KeyId)
                .ToListAsync();

            foreach (var item in ptttBnResult)
            {
                item.LoaiPTTT = IsPhauThuat(item.DichVuKyThuatBenhVienId) ? "Phẫu thuật" : "Thủ thuật";
            }

            return ptttBnResult;
        }

        public async Task<LichSuKetLuanPTTTVo> GetThongKetLuanDaHoanThanh(long yeuCauDichVuKyThuatId)
        {
            var TheoDoiSauPhauThuatThuThuatId = await BaseRepository.TableNoTracking
                           .Where(p => p.Id == yeuCauDichVuKyThuatId && p.TheoDoiSauPhauThuatThuThuatId != null).Select(cc => cc.TheoDoiSauPhauThuatThuThuatId).FirstOrDefaultAsync();

            var result = await _theoDoiSauPhauThuatThuThuatRepository.TableNoTracking.Where(cc => cc.Id == TheoDoiSauPhauThuatThuThuatId)
                           .Select(s => new LichSuKetLuanPTTTVo
                           {
                               YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                               TinhTrang = s.TrangThai,
                               TinhTrangDisplay = s.TrangThai.GetDescription(),
                               TheoDoiSauPhauThuatThuThuatId = s.Id,
                               DieuDuongPhuTrachTheoDoi = s.DieuDuongPhuTrachTheoDoi != null ? s.DieuDuongPhuTrachTheoDoi.User.HoTen : null,
                               BacSiPhuTrachTheoDoi = s.BacSiPhuTrachTheoDoi != null ? s.BacSiPhuTrachTheoDoi.User.HoTen : null,
                               GhiChuTheoDoi = s.GhiChuTheoDoi,
                               ThoiDiemTheoDoi = s.ThoiDiemBatDauTheoDoi != null ? s.ThoiDiemBatDauTheoDoi.Value.ApplyFormatDateTimeSACH() : null
                           }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<ThongTinBenhNhanPTTTVo> GetThongTinBenhNhanPTTTHoanThanh(long yeuCauDichVuKyThuatId)
        {
            var result = await BaseRepository.TableNoTracking
                          .Where(p => p.Id == yeuCauDichVuKyThuatId)
                          .Select(s => new ThongTinBenhNhanPTTTVo
                          {
                              YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                              MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                              BenhNhanId = s.YeuCauTiepNhan.BenhNhanId,
                              MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                              HoTen = s.YeuCauTiepNhan.HoTen,
                              TenGioiTinh = s.YeuCauTiepNhan.GioiTinh == null ? null : s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                              Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
                              SoDienThoai = s.YeuCauTiepNhan.SoDienThoai.ApplyFormatPhone(),
                              DanToc = s.YeuCauTiepNhan.DanToc != null ? s.YeuCauTiepNhan.DanToc.Ten : null,
                              BHYTMucHuong = s.YeuCauTiepNhan.BHYTMucHuong == null ? null : s.YeuCauTiepNhan.BHYTMucHuong,
                              NgheNghiep = s.YeuCauTiepNhan.NgheNghiep != null ? s.YeuCauTiepNhan.NgheNghiep.Ten : null,
                              DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                              TuyenKham = s.YeuCauTiepNhan.LyDoVaoVien == null ? null : s.YeuCauTiepNhan.LyDoVaoVien.GetDescription(),
                              TenLyDoTiepNhan = s.YeuCauTiepNhan.LyDoTiepNhan.Ten,
                              TenLyDoKhamBenh = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                              ThoiDiemTiepNhanDisplay = s.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                              NamSinh = s.YeuCauTiepNhan.NamSinh,
                              ThangSinh = s.YeuCauTiepNhan.ThangSinh,
                              NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                              TuoiThoiDiemHienTai = "",
                              TrangThai = s.YeuCauTiepNhan.TuVongTrongPTTT,
                              LanThucThien = s.LanThucHien,
                              TheoDoiSauPhauThuatThuThuatId = s.TheoDoiSauPhauThuatThuThuatId,
                              //CoPhauThuat = s.YeuCauTiepNhan.YeuCauDichVuKyThuats
                              //.Any(p => !string.IsNullOrEmpty(p.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat) && p.DichVuKyThuatBenhVien.LoaiPhauThuatThuThuat.Substring(0, 1).ToLower().Contains("p")),
                              DichVuKyThuatBenhVienId = s.DichVuKyThuatBenhVienId,
                              ThongTinKhamKhacChiTiet = s.TheoDoiSauPhauThuatThuThuat.KhamTheoDois.Select(o => new ThongTinKhamKhacChiTietVo
                              {
                                  Id = o.Id,
                                  ThongTinKhamTheoDoiData = o.ThongTinKhamTheoDoiData,
                                  ThongTinKhamTheoDoiTemplate = o.ThongTinKhamTheoDoiTemplate,
                                  KhamToanThan = o.KhamToanThan,
                              }).ToList()
                          }).FirstOrDefaultAsync();
            if (result != null)
            {
                var tuoiThoiDiemHienTai = 0;
                if (result.NamSinh != null)
                {
                    tuoiThoiDiemHienTai = DateTime.Now.Year - result.NamSinh.Value;
                }
                var dobConvert = DateHelper.ConvertDOBToTimeJson(result.NgaySinh, result.ThangSinh, result.NamSinh);
                var jsonConvertString = new NgayThangNamSinhVo();

                if (!string.IsNullOrEmpty(dobConvert) && tuoiThoiDiemHienTai < 6)
                {
                    jsonConvertString = JsonConvert.DeserializeObject<NgayThangNamSinhVo>(dobConvert);
                }

                var tuoiBenhNhan = result.NamSinh != null ?
                                (tuoiThoiDiemHienTai < 6 ? jsonConvertString.Years + " Tuổi " + jsonConvertString.Months + " Tháng " + jsonConvertString.Days + " Ngày" : tuoiThoiDiemHienTai.ToString()) : tuoiThoiDiemHienTai.ToString();
                result.TuoiThoiDiemHienTai = tuoiBenhNhan;
                result.CoPhauThuat = IsPhauThuat(result.DichVuKyThuatBenhVienId);
            }
            return result;
        }

        public async Task<LichSuDichVuKyThuatDaTuongTrinhPTTT> GetDichVuDaTuongTrinhPTTT(LichSuDichVuKyThuatDaTuongTrinhVo lichSuDichVuKyThuatDaTuongTrinhVo)
        {
            var dichVuDaThucHien = await BaseRepository.TableNoTracking
                            .Where(o => o.YeuCauTiepNhanId == lichSuDichVuKyThuatDaTuongTrinhVo.YeuCauTiepNhanId
                                            && o.LanThucHien == lichSuDichVuKyThuatDaTuongTrinhVo.LanThucThien && (o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaHuy || o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                            && o.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null && o.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true).CountAsync();

            var tongDichVu = await BaseRepository.TableNoTracking
                            .Where(o => o.YeuCauTiepNhanId == lichSuDichVuKyThuatDaTuongTrinhVo.YeuCauTiepNhanId
                                            && o.LanThucHien == lichSuDichVuKyThuatDaTuongTrinhVo.LanThucThien).CountAsync();

            var lichSuDichVuKyThuatDaTuongTrinh = new LichSuDichVuKyThuatDaTuongTrinhPTTT
            {
                DichVuKyThuatDaTuongTrinh = dichVuDaThucHien,
                TongDichVuKyThuat = tongDichVu
            };
            return lichSuDichVuKyThuatDaTuongTrinh;
        }

    }
}
