using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaoThucHienCls;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.BaoCaoThucHienCls
{
    public interface IBaoCaoThucHienClsService : IMasterFileService<YeuCauDichVuKyThuat>
    {
        GridDataSource GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDataForGridAsyncChild(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetDataForGridAsyncHoatDongCLS(BaoCaoHoatDongCLSVo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncHoatDongCLS(BaoCaoHoatDongCLSVo queryInfo);
        Task<GridDataSource> GetDataSoThongKeCLSForGridAsync(BaoCaoSoThongKeCLSChiTietVo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageSoThongKeCLSForGridAsyn(BaoCaoSoThongKeCLSChiTietVo queryInfo);

        byte[] ExportBaoCaoBangKeBacSiCLS(BaoCaoThucHienCLSVo queryInfo);
        byte[] ExportBaoCaoHoatDongClsTheoKhoa(BaoCaoHoatDongCLSVo queryInfo);
        byte[] ExportBaoCaoSoThongKeCls(BaoCaoSoThongKeCLSChiTietVo queryInfo);
        Task<List<LookupItemVo>> KhoaPhongs(DropDownListRequestModel queryInfo);

    }
}
