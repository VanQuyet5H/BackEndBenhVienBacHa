using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService
    {
        Task<GridDataSource> GetDataForGridAsyncDanhSachDaKham(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachDaKham(QueryInfo queryInfo);

        //Task<List<LookupItemVo>> GetDoiTuongs(DropDownListRequestModel queryInfo);
        //List<LookupItemVo> GetTinhTrangKhamBenhs();
        string SetSoPhanTramHuongBHYT(string maThe);
        Task<string> UpdateBenhNhanCanKhamLai(long yckbId,long phongbenhvienId);

        Task CapNhatKhamLaiKhamSucKhoeAsync(KhamLaiKhamDoanVo khamLaiVo);
        Task<DanhSachChoKhamGridVo> GetYeuCauKhamBenh(long yckbId, long yctnId);
        #region BVHD-3698
        Task<List<long>> GetYeuCauKhamBenhKhamThaiIds(long yctnId);
        Task<List<InfoYeuCauKhamBenhVo>> GetYeuCauKhamBenhNghiHuongBHXH(long yctnId);
        #endregion
    }
}
