using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<List<string>> GetListTenNhanVienmAsync();
        Task<List<LookupItemVo>> GetChuToa(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetThuKy(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetThanhVienThamGia(DropDownListRequestModel model);
        TrichBienBanHoiChanGridVo GetThongTinTrichBienBanHoiChan(long yeuCauTiepNhan);
        NhanVienNgayThucHien GetTenNguoiThucHien(long idNguoiLogin,long yeuCauTiepNhanId);
        long KiemTraTonTai(long yeuCauTiepNhan, Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru);
        Task<string> BienBanHoiChan(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan);
        Task<GridDataSource> GetDanhSachBienBanHoiChan(QueryInfo queryInfo);
    }
}
