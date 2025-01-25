using Camino.Core.Domain.ValueObject;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.Grid;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        List<LookupItemVo> GetListNhomChucDanh(DropDownListRequestModel queryInfo);
        
        Task<List<LookupItemVo>> GetListBacSiDieuDuong(DropDownListRequestModel queryInfo, EnumNhomChucDanh nhomChucDanh);
        
        Task<List<string>> GetListBacSiAutoComplete(DropDownListRequestModel queryInfo);

        Task<List<string>> GetListDieuDuongAutoComplete(DropDownListRequestModel queryInfo);

        Task<List<string>> GetListPhauThuatAutoComplete(DropDownListRequestModel queryInfo);

        List<LookupItemVo> GetListVaiTroBacSi(DropDownListRequestModel queryInfo);
        
        List<LookupItemVo> GetListVaiTroDieuDuong(LookupQueryInfo queryInfo);
        
        Task<GridDataSource> LoadEkip(long ycdvktId);
    }
}
