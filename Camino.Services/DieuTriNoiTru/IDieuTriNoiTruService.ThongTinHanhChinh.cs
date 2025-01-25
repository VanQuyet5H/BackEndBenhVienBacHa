using Camino.Core.Domain.ValueObject;
using System.Collections.Generic;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        List<LookupItemVo> GetListNhomMau(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListYeuToRh(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListLoaiChuyenTuyen(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListHinhThucRaVien(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListBenhVien(DropDownListRequestModel queryInfo);
    }
}
