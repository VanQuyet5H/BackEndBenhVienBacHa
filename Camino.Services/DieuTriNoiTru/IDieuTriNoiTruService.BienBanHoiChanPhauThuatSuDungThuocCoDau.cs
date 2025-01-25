using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<List<ThuocPhieuDieuTriBenhNhanItemVo>> GetDuocPhamBenhNhanNoiTru(DropDownListRequestModel model, long yeuCauTiepNhanId);
        Task<List<ThuocPhieuDieuTriBenhNhanItemVo>> GetDuocPhamCoDau(DropDownListRequestModel queryInfo);
        Task<ThuocPhieuDieuTriBenhNhanItemVo> GetDuocPhamCoDauVo(long duocPhamId);
        Task<string> PhieuInBienBanHoiChanPhauThuatCoDau(BangTheoDoiHoiTinhHttpParamsRequest phieuInBienBanHoiChanParams);
    }
}
