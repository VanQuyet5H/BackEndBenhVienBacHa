using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Services.Thuocs
{
    public interface IDuocPhamService: IMasterFileService<DuocPham>
    {
        #region danh sách thuốc bênh viện
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        #endregion

        #region get thông tin chung
        List<LookupItemVo> GetDanhSachLoaiThuocHoacHoatChat(DropDownListRequestModel queryInfo);
        Task<ICollection<DuongDungTemplateVo>> GetDanhSachDuongDungAsync(DropDownListRequestModel model);
        Task<ICollection<string>> GetListTenNhaSanXuatAsync();
        Task<ICollection<string>> GetListTenNuocSanXuatAsync();
        Task<ICollection<MaHoatChatHoatChatDuongDungTemplateVo>> GetListTenMaHoatChatAsync(DropDownListRequestModel queryInfo);
        Task<ICollection<string>> GetListTenHoatChatVaDuongDungAsync();
        Task<string> GetTenHoatChatAsync(string tenHoatChatDuongDung);
        #endregion

        #region xử lý thuốc bệnh viện

        Task<bool> KiemTraSoDangKyTonTaiAsync(string soDangKy, long? id);

        #endregion
        Task<List<LookupItemVo>> GetListLookupDuocPham(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetListNhaSanXuat(DropDownListRequestModel model);
        bool CheckTenExits(string ten, long id);
        Task<bool> CheckDuongDungAsync(long Idduongdung);
        Task<bool> CheckDVTAsync(long Id);
        Task<bool> ChecMaHoatChatAsync(string ma, long id);
        Task<bool> IsTenExists(string ten = null, long id = 0);
    }
}
