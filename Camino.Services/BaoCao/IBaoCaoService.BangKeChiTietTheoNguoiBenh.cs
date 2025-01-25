using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<List<LookupItemTemplateVo>> GetNguoiBenhTheoNoiGioiThieuAsync(DropDownListRequestModel queryInfo);
        Task<GridDataSource> GetDataBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageBaoCaoBangKeChiTietTheoNguoiBenhForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoBangKeChiTietTheoNguoiBenh(GridDataSource gridDataSource, QueryInfo query);
    }
}
