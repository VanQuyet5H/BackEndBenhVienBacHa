using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NguoiGioiThieu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.NguoiGioiThieu
{
    public interface INguoiGioiThieuService : IMasterFileService<Core.Domain.Entities.NguoiGioiThieus.NguoiGioiThieu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<NguoiQuanLyTemplateVo>> GetNguoiQuanLyListAsync(DropDownListRequestModel queryInfo);
        Task<bool> IsTenExists(string hoTen = null, long? nhanVienQuanLyId = null, long Id = 0);
        Task<bool> IsPhoneNumberExists(string hoTen = null, string soDienThoai = null, long Id = 0);


    }
}
