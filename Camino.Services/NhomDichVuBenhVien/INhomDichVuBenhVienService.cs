using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhomDichVuBenhVien;

namespace Camino.Services.NhomDichVuBenhVien
{
    public interface INhomDichVuBenhVienService : IMasterFileService<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien>
    {
        //Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        //Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<List<NhomDichVuBenhVienGridCombobox>> GetDichVuKhamBenh(DropDownListRequestModel model);

        Task<bool> CheckChiDinhVong(long id, long? nhomDichVuChaId);


        List<NhomDichVuBenhVienTreeViewGridVo> NhomDichVuBenhVienTreeViewChas(QueryInfo queryInfo);
        List<NhomDichVuBenhVienTreeViewGridVo> NhomDichVuBenhVienTreeViewCons(QueryInfo queryInfo);

    }
}
