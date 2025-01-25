using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public List<LookupItemVo> GetListNhomMau(DropDownListRequestModel queryInfo)
        {
            var listEnumNhomMau = EnumHelper.GetListEnum<EnumNhomMau>()
                                            .Select(item => new LookupItemVo
                                            {
                                                KeyId = Convert.ToInt32(item),
                                                DisplayName = item.ToString()
                                                //DisplayName = item.GetDescription()
                                            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumNhomMau = listEnumNhomMau.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumNhomMau;
        }

        public List<LookupItemVo> GetListYeuToRh(DropDownListRequestModel queryInfo)
        {
            var listEnumYeuToRh = EnumHelper.GetListEnum<EnumYeuToRh>()
                                            .Select(item => new LookupItemVo
                                            {
                                                KeyId = Convert.ToInt32(item),
                                                DisplayName = item.GetDescription()
                                            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumYeuToRh = listEnumYeuToRh.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumYeuToRh;
        }

        public List<LookupItemVo> GetListLoaiChuyenTuyen(DropDownListRequestModel queryInfo)
        {
            var listEnumLoaiChuyenTuyen = EnumHelper.GetListEnum<LoaiChuyenTuyen>()
                                                    .Select(item => new LookupItemVo
                                                    {
                                                        KeyId = Convert.ToInt32(item),
                                                        DisplayName = item.GetDescription()
                                                    }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumLoaiChuyenTuyen = listEnumLoaiChuyenTuyen.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumLoaiChuyenTuyen;
        }

        public List<LookupItemVo> GetListHinhThucRaVien(DropDownListRequestModel queryInfo)
        {
            var listEnumHinhThucRaVien = EnumHelper.GetListEnum<EnumHinhThucRaVien>()
                                                   .Select(item => new LookupItemVo
                                                   {
                                                       KeyId = Convert.ToInt32(item),
                                                       DisplayName = item.GetDescription()
                                                   }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listEnumHinhThucRaVien = listEnumHinhThucRaVien.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return listEnumHinhThucRaVien;
        }

        public List<LookupItemVo> GetListBenhVien(DropDownListRequestModel queryInfo)
        {
            var query = _benhVienRepository.TableNoTracking.Select(p => new LookupItemVo
            {
                KeyId = p.Id,
                DisplayName = p.Ten
            })
            .ApplyLike(queryInfo.Query, p => p.DisplayName);

            return query.ToList();
        }

        //public async Task<List<LookupItemVo>> GetListNhomMau(DropDownListRequestModel queryInfo, EnumNhomChucDanh nhomChucDanh)
        //{
        //    if (nhomChucDanh != EnumNhomChucDanh.BacSi && nhomChucDanh != EnumNhomChucDanh.DieuDuong)
        //    {
        //        return new List<LookupItemVo>();
        //    }

        //    var lstBacSiDieuDuong = await _nhanVienRepository.TableNoTracking
        //        .Where(e => e.ChucDanh.NhomChucDanhId == (long)nhomChucDanh)
        //        .Select(item => new LookupItemVo
        //        {
        //            KeyId = item.Id,
        //            DisplayName = item.User.HoTen
        //        })
        //        .ApplyLike(queryInfo.Query, w => w.DisplayName)
        //        .Take(queryInfo.Take).OrderBy(w => w.KeyId).ToListAsync();
        //    return lstBacSiDieuDuong;
        //}
    }
}
