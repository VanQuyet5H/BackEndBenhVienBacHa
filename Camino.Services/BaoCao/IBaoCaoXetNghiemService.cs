using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaoLuuketQuaXeNghiemTrongNgay;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoXetNghiemService
    {
        Task<GridDataSource> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayForGridAsync(BaoCaoKetQuaXetNghiemQueryInfo queryInfo, bool exportExcel);
        Task<GridDataSource> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayTotalPageForGridAsync(BaoCaoKetQuaXetNghiemQueryInfo queryInfo);
        Task<List<LookupItemVo>> GetListBHYT(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListKSK(DropDownListRequestModel model);
        Task<string> XuLyInBaoCaoLuuKetQuaXetNghiemHangNgayAsync(BaoCaoKetQuaXetNghiemQueryInfo phieuInNhanVienKhamSucKhoeInfoVo, ICollection<BaoCaoLuuketQuaXeNghiemTrongNgayGridVo> data);
    }
}
