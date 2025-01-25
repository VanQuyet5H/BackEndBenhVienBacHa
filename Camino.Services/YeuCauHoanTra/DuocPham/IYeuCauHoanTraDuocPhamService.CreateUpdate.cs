using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauHoanTra.DuocPham
{
    public partial interface IYeuCauHoanTraDuocPhamService
    {
        Task<GridDataSource> GetAllDuocPhamData(QueryInfo queryInfo);
        Task<GridDataSource> GetAllDuocPhamTotal(QueryInfo queryInfo);

        Task<List<LookupItemVo>> GetKhoDuocPham(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetKhoDuocHoanTra(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetKhoLoaiDuocPham(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetKhoLoaiDuocHoanTra(DropDownListRequestModel model);

        Task<List<DuocPhamHoanTraGridVo>> GetDuocPhamOnGroup(long groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon);

        Task<YeuCauTraDuocPhamChiTiet> GetDuocPham(ThemDuocPhamHoanTra model);

        Task UpdateGiaChoNhapKhoChiTiet(double SoLuongXuat, long id);

        Task UpdateSlXuatNhapKhoChiTiet(double soLuongXuat, long id);

        Task<bool> CheckValidSlTon(List<ThemDuocPhamHoanTra> lstModelThemDuocPhamHoanTra, long id);
    }
}
