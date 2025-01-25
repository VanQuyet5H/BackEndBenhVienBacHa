using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhapKhoMarketting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.NhapKhoQuaTangMarketing
{
    public partial interface INhapKhoQuaTangMarketingService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        ThongTinNhapKhoQuaTangGridVo GetThongTinNhapKhoQuaTang(long nhapKhoQuaTangId);
        Task<List<LookupItemVo>> GetListNhanVienAsync(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListDanhSachQuaTangAsync(DropDownListRequestModel model);
        string GetDonViTinhQuaTang(long IdQuaTang);
        string GetTenQuaTang(long IdQuaTang);
        LookupItemVo GetThongTinNhanVienLoginAsync(long nhanVienId);
        Task<bool> IsTenExists(string ten, long quaTangId);
    }
}
