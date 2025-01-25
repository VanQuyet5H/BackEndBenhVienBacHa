using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task<GridDataSource> GetDataForGridAsyncDanhSachInKetQuaKhamSucKhoe(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachInKetQuaKhamSucKhoe(QueryInfo queryInfo);
        Task<List<LookupItemTemplateVo>> GetCongTyInKetQuaKSKs(DropDownListRequestModel queryInfo);
        Task<List<LookupItemHopDingKhamSucKhoeTemplateVo>> GetHopDongKhamSucKhoeInKetQuaKSKs(DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false);
        Task<KhamDoanThongTinHanhChinhInKetQuaKhamSucKhoeVo> GetThongTinHanhChinhInKetQuaKSKAsync(long yeuCauTiepNhanId);
        List<InFoBarCodeKSK> ComPareBarCode(InFoBarCodeKSKVo searching);
    
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanDoanCantAsync(long yctnId);
        Task<long> GetHopDongId(long hpNhanVienId);
    }
}
