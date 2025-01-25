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
        Task<InfoGiayChungNhanPhauThuatVo> GetThongTinGiayChungNhanPhauThuat(long yeuCauTiepNhan);
        Task<List<LookupItemVo>> GetListDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan(DropDownListRequestModel model, long yctnId);
        Task<InfoYeuCauDichVuKyThuatTheoBenNhanVo> GetInfoDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan(long ycdvktId);
        Task<List<LookupItemVo>> GetListTinhTrangRaVienHoSoKhac(DropDownListRequestModel model);
        bool KiemTraTinhTrangExit(string tinhTrang);
        Task<string> InGiayChungNhanPhauThuat(long yeuCauTiepNhanId);
    }
}
