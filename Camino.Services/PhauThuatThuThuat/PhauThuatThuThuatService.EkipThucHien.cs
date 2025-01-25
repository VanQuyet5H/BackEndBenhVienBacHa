using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        public List<LookupItemVo> GetListNhomChucDanh(DropDownListRequestModel queryInfo)
        {
            var lstNhomChucDanh = _nhomChucDanhRepository.TableNoTracking.Where(p => p.Id == (long)EnumNhomChucDanh.BacSi || p.Id == (long)EnumNhomChucDanh.DieuDuong)
                                                                         .Select(item => new LookupItemVo
                                                                         {
                                                                             KeyId = item.Id,
                                                                             DisplayName = item.Ten
                                                                         })
                                                                         .ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstNhomChucDanh = lstNhomChucDanh.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                                 .ToList();
            }

            return lstNhomChucDanh;
        }

        public async Task<List<string>> GetListBacSiAutoComplete(DropDownListRequestModel queryInfo)
        {
            var lstBacSiDieuDuong = _nhanVienRepository.TableNoTracking
                .Where(e => e.ChucDanh.NhomChucDanhId == (long)EnumNhomChucDanh.BacSi)
                .Select(item => item.User.HoTen)
                .ApplyLike(queryInfo.Query, o => o)
                .Take(queryInfo.Take);
            return await lstBacSiDieuDuong.ToListAsync();
        }

        public async Task<List<string>> GetListDieuDuongAutoComplete(DropDownListRequestModel queryInfo)
        {
            var lstBacSiDieuDuong = _nhanVienRepository.TableNoTracking
                .Where(e => e.ChucDanh.NhomChucDanhId == (long)EnumNhomChucDanh.DieuDuong)
                .Select(item => item.User.HoTen)
                .ApplyLike(queryInfo.Query, o => o)
                .Take(queryInfo.Take);
            return await lstBacSiDieuDuong.ToListAsync();
        }

        public async Task<List<string>> GetListPhauThuatAutoComplete(DropDownListRequestModel queryInfo)
        {
            var lstBacSiDieuDuong = _nhanVienRepository.TableNoTracking
                .Where(e => e.ChucDanh.NhomChucDanhId == (long)EnumNhomChucDanh.DieuDuong || e.ChucDanh.NhomChucDanhId == (long)EnumNhomChucDanh.BacSi)
                .Select(item => item.User.HoTen)
                .ApplyLike(queryInfo.Query, o => o)
                .Take(queryInfo.Take);
            return await lstBacSiDieuDuong.ToListAsync();
        }

        public async Task<List<LookupItemVo>> GetListBacSiDieuDuong(DropDownListRequestModel queryInfo, EnumNhomChucDanh nhomChucDanh)
        {
            if (nhomChucDanh != EnumNhomChucDanh.BacSi && nhomChucDanh != EnumNhomChucDanh.DieuDuong)
            {
                return new List<LookupItemVo>();
            }

            var lstBacSiDieuDuong = await _nhanVienRepository.TableNoTracking
                .Where(e => e.ChucDanh.NhomChucDanhId == (long)nhomChucDanh)
                .Select(item => new LookupItemVo
                {
                    KeyId = item.Id,
                    DisplayName = item.User.HoTen
                })
                .ApplyLike(queryInfo.Query, w => w.DisplayName)
                .OrderByDescending(x => x.KeyId == queryInfo.Id).ThenBy(w => w.KeyId)
                .Take(queryInfo.Take).ToListAsync();
            return lstBacSiDieuDuong;
        }

        public List<LookupItemVo> GetListVaiTroBacSi(DropDownListRequestModel queryInfo)
        {
            var lstEnum = EnumHelper.GetListEnum<EnumVaiTroBacSi>();

            var lstVaiTroBacSi = lstEnum.Select(item => new LookupItemVo
            {
                KeyId = (int)item,
                DisplayName = item.GetDescription()
            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstVaiTroBacSi = lstVaiTroBacSi.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                               .ToList();
            }

            return lstVaiTroBacSi;
        }

        public List<LookupItemVo> GetListVaiTroDieuDuong(LookupQueryInfo queryInfo)
        {
            var lstEnum = EnumHelper.GetListEnum<EnumVaiTroDieuDuong>();

            var lstVaiTroDieuDuong = lstEnum.Select(item => new LookupItemVo
            {
                KeyId = (int)item,
                DisplayName = item.GetDescription()
            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstVaiTroDieuDuong = lstVaiTroDieuDuong.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                                       .ToList();
            }

            return lstVaiTroDieuDuong;
        }

        public async Task<GridDataSource> LoadEkip(long ycdvktId)
        {
            var ekipsQuery = _phauThuatThuThuatEkipBacSiRepository.TableNoTracking
                .Where(w => w.YeuCauDichVuKyThuatTuongTrinhPTTTId == ycdvktId)
                .Select(q => new EkipGridVo
                {
                    Id = q.Id,
                    NhomChucDanh = EnumNhomChucDanh.BacSi,
                    VaiTroBacSi = q.VaiTroBacSi,
                    VaiTroDieuDuong = null,
                    BacSiId = q.NhanVienId,
                    BacSi = q.NhanVien.User.HoTen
                }).Union(
                    _phauThuatThuThuatEkipDieuDuongRepository.TableNoTracking
                        .Where(w => w.YeuCauDichVuKyThuatTuongTrinhPTTTId == ycdvktId)
                        .Select(q => new EkipGridVo
                        {
                            Id = q.Id,
                            NhomChucDanh = EnumNhomChucDanh.DieuDuong,
                            VaiTroDieuDuong = q.VaiTroDieuDuong,
                            VaiTroBacSi = null,
                            BacSiId = q.NhanVienId,
                            BacSi = q.NhanVien.User.HoTen
                        })
                );

            var ekips = ekipsQuery.ToArrayAsync();
            await Task.WhenAll(ekips);
            return new GridDataSource
            {
                Data = ekips.Result,
                TotalRowCount = ekips.Result.Length
            };
        }
    }
}