using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.TiemChung
{
    public partial interface ITiemChungService
    {
        Task<List<LookupItemVo>> GetNhanViensAsync(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetPhanUngSauTiems(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetNoiXuTris(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetTinhTrangHienTais(DropDownListRequestModel queryInfo);
        bool KiemTraThoiGianTheoDoiTiemChungVoiLanTiemKhac(DateTime? thoiGianTheoDoi, long yeuCauDichVuKyThuatKhamSangLocId);
        bool KiemTraThoiGianTheoDoiTiemChungVoiKhamSangLoc(DateTime? thoiGianTheoDoi, long yeuCauDichVuKyThuatKhamSangLocId);
    }
}