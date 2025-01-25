using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService
    {
        Task<GridDataSource> GetDataForGridAsyncDanhSachChoKhams(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachChoKhams(QueryInfo queryInfo);
        Task<List<DoiTuongPhongChoTemplateVo>> GetDoiTuongPhongCho(DropDownListRequestModel queryInfo);
        Task<Core.Domain.Entities.BenhVien.BenhVien> BenhVienHienTai();
        Core.Domain.Entities.BenhVien.BenhVien GetBenhViens(long? benhVienId);

    }
}
