using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.TiemChung
{
    public partial class TiemChungService
    {
        public async Task<List<LookupItemVo>> GetNhanViensAsync(DropDownListRequestModel queryInfo)
        {
            var lstNhanVien = await _nhanVienRepository.TableNoTracking.Select(item => new LookupItemVo
                                                                        {
                                                                            KeyId = item.Id,
                                                                            DisplayName = item.User.HoTen
                                                                        })
                                                                        .ApplyLike(queryInfo.Query, w => w.DisplayName)
                                                                        .OrderBy(p => p.DisplayName)
                                                                        .Take(queryInfo.Take)
                                                                        .ToListAsync();

            return lstNhanVien;
        }

        public List<LookupItemVo> GetPhanUngSauTiems(DropDownListRequestModel queryInfo)
        {
            var phanUngSauTiems = EnumHelper.GetListEnum<LoaiPhanUngSauTiem>();

            var lstPhanUngSauTiem = phanUngSauTiems.Select(item => new LookupItemVo
            {
                KeyId = (int)item,
                DisplayName = item.GetDescription()
            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstPhanUngSauTiem = lstPhanUngSauTiem.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                                     .ToList();
            }

            return lstPhanUngSauTiem;
        }

        public List<LookupItemVo> GetNoiXuTris(DropDownListRequestModel queryInfo)
        {
            var noiXuTris = EnumHelper.GetListEnum<NoiXuTriTheoDoiTiemVacxin>();

            var lstNoiXuTri = noiXuTris.Select(item => new LookupItemVo
            {
                KeyId = (int)item,
                DisplayName = item.GetDescription()
            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstNoiXuTri = lstNoiXuTri.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                         .ToList();
            }

            return lstNoiXuTri;
        }

        public List<LookupItemVo> GetTinhTrangHienTais(DropDownListRequestModel queryInfo)
        {
            var tinhTrangHienTais = EnumHelper.GetListEnum<TinhTrangHienTaiTheoDoiTiemVacxin>();

            var lstTinhTrangHienTai = tinhTrangHienTais.Select(item => new LookupItemVo
            {
                KeyId = (int)item,
                DisplayName = item.GetDescription()
            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstTinhTrangHienTai = lstTinhTrangHienTai.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                                         .ToList();
            }

            return lstTinhTrangHienTai;
        }

        public bool KiemTraThoiGianTheoDoiTiemChungVoiLanTiemKhac(DateTime? thoiGianTheoDoi, long yeuCauDichVuKyThuatKhamSangLocId)
        {
            if (thoiGianTheoDoi == null)
            {
                return true;
            }

            var yeuCauDichVuKyThuatKhamSangLoc = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(p => p.Id == yeuCauDichVuKyThuatKhamSangLocId)
                                                                                               .Include(p => p.KhamSangLocTiemChung).ThenInclude(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.TiemChung)
                                                                                               .First();
            
            if (!yeuCauDichVuKyThuatKhamSangLoc.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.TiemChung != null))
            {
                return true;
            }

            return thoiGianTheoDoi.Value > yeuCauDichVuKyThuatKhamSangLoc.KhamSangLocTiemChung.YeuCauDichVuKyThuats.OrderBy(p => p.TiemChung.ThoiDiemTiem).First().TiemChung.ThoiDiemTiem;
        }

        public bool KiemTraThoiGianTheoDoiTiemChungVoiKhamSangLoc(DateTime? thoiGianTheoDoi, long yeuCauDichVuKyThuatKhamSangLocId)
        {
            if (thoiGianTheoDoi == null)
            {
                return true;
            }

            var yeuCauDichVuKyThuatKhamSangLoc = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(p => p.Id == yeuCauDichVuKyThuatKhamSangLocId)
                                                                                               .Include(p => p.KhamSangLocTiemChung)
                                                                                               .First();

            return thoiGianTheoDoi.Value > yeuCauDichVuKyThuatKhamSangLoc.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc;
        }
    }
}