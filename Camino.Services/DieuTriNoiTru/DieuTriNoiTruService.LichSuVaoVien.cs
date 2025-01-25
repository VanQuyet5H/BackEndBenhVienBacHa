using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public GridDataSource GetDanhSachLichSuVaoVienForGrid(QueryInfo queryInfo, bool isAllData = false)
        {
            long yeuCauTiepNhanId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var benhNhan = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId).Select(c => c.BenhNhan)
                .Include(o => o.YeuCauTiepNhans).ThenInclude(o => o.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhChuanDoans).ThenInclude(o => o.ChuanDoan)
                .Include(o => o.YeuCauTiepNhans).ThenInclude(o => o.YeuCauKhamBenhs).ThenInclude(o => o.BacSiKetLuan).ThenInclude(o => o.User)
                .Include(o => o.YeuCauTiepNhans).ThenInclude(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien)
                .Include(o => o.YeuCauTiepNhans).ThenInclude(o => o.NoiTruBenhAn)
                .FirstOrDefault();
            var yeuCauTiepNhans = benhNhan?.YeuCauTiepNhans;
            if (yeuCauTiepNhans != null)
            {
                var query = yeuCauTiepNhans.Where(cc => cc.LoaiYeuCauTiepNhan == Core.Domain.Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
                                              cc.YeuCauKhamBenhs.Any(x => x.TrangThai == Core.Domain.Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                                            || cc.LoaiYeuCauTiepNhan == Core.Domain.Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && cc.NoiTruBenhAn?.DaQuyetToan == true)
                                            .Select(s => new LichSuVaoVienGridVo
                                            {
                                                Id = s.Id,
                                                MaTiepNhan = s.MaYeuCauTiepNhan,
                                                SoBenhAnh = s.NoiTruBenhAn != null ? s.NoiTruBenhAn.SoBenhAn : "",
                                                MaBenhNhan = s.BenhNhan.MaBN,
                                                ThoiGianTiepNhan = s.ThoiDiemTiepNhan,
                                                BenhNhanId = benhNhan.Id,
                                                ThoiGianTiepNhanDisplay = s.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                                                KiemTraNoiTru = s.NoiTruBenhAn != null,
                                                ChuanDoan = string.Join(";", s.YeuCauKhamBenhs.Where(cc => cc.GhiChuICDChinh != null).Select(cc => cc.GhiChuICDChinh).Distinct()),
                                                BacSi = string.Join(";", s.YeuCauKhamBenhs.Where(cc => cc.BacSiKetLuan != null && cc.BacSiKetLuan.User.HoTen != null).Select(cc => cc.BacSiKetLuan.User.HoTen).Distinct()),
                                                PhongKham = string.Join(";", s.YeuCauKhamBenhs.Where(cc => cc.NoiThucHien != null && cc.NoiThucHien.Ten != null).Select(cc => cc.NoiThucHien.Ten).Distinct())
                                            });

                var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
                var queryTask = isAllData == true ? query.ToArray() : query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            }
            return new GridDataSource { Data = null, TotalRowCount = 0 };
        }
        public GridDataSource GetTotalPagesDanhSachLichSuVaoVienForGrid(QueryInfo queryInfo)
        {

            BuildDefaultSortExpression(queryInfo);
            long yeuCauTiepNhanId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;
            var benhNhan = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId).Select(c => c.BenhNhan).FirstOrDefault();
            var yeuCauTiepNhans = benhNhan?.YeuCauTiepNhans;

            var query = yeuCauTiepNhans.Where(cc => cc.LoaiYeuCauTiepNhan == Core.Domain.Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && cc.TrangThaiYeuCauTiepNhan == Core.Domain.Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat
                                            || cc.LoaiYeuCauTiepNhan == Core.Domain.Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && cc.NoiTruBenhAn?.DaQuyetToan == true)
                                            .Select(s => new LichSuVaoVienGridVo
                                            {
                                                Id = s.Id,
                                                MaTiepNhan = s.MaYeuCauTiepNhan,
                                                SoBenhAnh = s.NoiTruBenhAn?.SoBenhAn,
                                                MaBenhNhan = s.BenhNhan.MaBN,
                                                ThoiGianTiepNhan = s.ThoiDiemTiepNhan,
                                                ThoiGianTiepNhanDisplay = s.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                                                KiemTraNoiTru = s.NoiTruBenhAn != null
                                            });

            if (query != null)
            {
                var countTask = query.Count();
                return new GridDataSource { TotalRowCount = countTask };
            }
            return new GridDataSource { TotalRowCount = 0 };
        }
        public async Task<List<LookupItemTemplateVo>> GetTenDichVuKhamBenh(DropDownListRequestModel model, long yeuCauTiepNhanId)
        {
            if (model.Id == 0)
            {
                var firstValue = await BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                      .SelectMany(cc => cc.YeuCauKhamBenhs).Where(cc => cc.ThoiDiemHoanThanh != null)
                                      .Select(item => new LookupItemTemplateVo()
                                      {
                                          KeyId = item.Id,
                                          DisplayName = item.TenDichVu,
                                          Ma = item.MaDichVu,
                                          Ten = item.TenDichVu
                                      }).ToListAsync();
                return firstValue;
            }
            var yeuCauKhamBenhs = await BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                        .SelectMany(cc => cc.YeuCauKhamBenhs).Where(cc => cc.ThoiDiemHoanThanh != null)
                                        .OrderByDescending(x => x.Id == model.Id)
                                        .Select(item => new LookupItemTemplateVo()
                                        {
                                            KeyId = item.Id,
                                            DisplayName = item.TenDichVu,
                                            Ma = item.MaDichVu,
                                            Ten = item.TenDichVu
                                        })
                                        .ApplyLike(model.Query, x => x.Ten, x => x.Ma)
                                        .Take(model.Take).ToListAsync();
            return yeuCauKhamBenhs;
        }
        public ThongTinTheoKhamBenh GetThongTinTheoKhamBenh(long khamBenhId, long yeuCauTiepNhanId)
        {
            var yeuCauKhamBenhs = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .SelectMany(cc => cc.YeuCauKhamBenhs).Where(cc => cc.ThoiDiemHoanThanh != null)
                                                .Include(o => o.YeuCauKhamBenhChuanDoans).ThenInclude(o => o.ChuanDoan)
                                                .Include(o => o.BacSiKetLuan).ThenInclude(o => o.User)
                                                .Include(o => o.NoiThucHien);
            var thongTinKhamBenh = yeuCauKhamBenhs.Where(cc => cc.Id == khamBenhId).FirstOrDefault();
            return new ThongTinTheoKhamBenh
            {
                BacSi = thongTinKhamBenh.BacSiKetLuan.User.HoTen,
                PhongKham = thongTinKhamBenh.NoiThucHien.Ten,
                NgayKham = thongTinKhamBenh.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH()
            };
        }

        #region Tab Thong tin theo khám bệnh
        public ThongTinLichSuKhamBenhNoiTru ThongTinLichSuKhamBenhNoiTru(long khamBenhId, long yeuCauTiepNhanId)
        {
            var yeuCauKhamBenhs = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .SelectMany(cc => cc.YeuCauKhamBenhs).Where(cc => cc.ThoiDiemHoanThanh != null)
                                                .Include(o => o.YeuCauKhamBenhChuanDoans).ThenInclude(o => o.ChuanDoan)
                                                .Include(o => o.BacSiKetLuan).ThenInclude(o => o.User)
                                                .Include(o => o.NoiChiDinh).Include(o => o.ChanDoanSoBoICD);
            var thongTinKhamBenh = yeuCauKhamBenhs.Where(cc => cc.Id == khamBenhId).FirstOrDefault();
            return new ThongTinLichSuKhamBenhNoiTru
            {
                LyDoVaoVien = thongTinKhamBenh.LyDoChuyenVien,
                BenhSu = thongTinKhamBenh.BenhSu,
                KhamToanThan = thongTinKhamBenh.KhamToanThan,
                ICD = thongTinKhamBenh.ChanDoanSoBoICD.TenTiengViet,
                ChuanDoan = thongTinKhamBenh.ChanDoanSoBoGhiChu,
            };
        }
        #endregion

    }
}
