using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDataBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGrid3961Async(QueryInfo queryInfo);
        //Task<GridDataSource> GetDataBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataTotalPageBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo(GridDataSource gridDataSource, QueryInfo query);
        Task<List<LookupItemTemplateVo>> GetNoiGioiThieuDaCoNguoiBenhAsync(DropDownListRequestModel queryInfo, bool theoHinhThucDen = false);
        Task<List<LookupItemTemplateVo>> GetHinhThucDenCoTatCaAsync(DropDownListRequestModel queryInfo);
        Task<List<MaTiepNhanTheoHinhThucDenLookupItemVo>> GetMaYeuCauTiepNhanTheoHinhThucDenAsync(DropDownListRequestModel queryInfo);
    }
}
