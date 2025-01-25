using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.MauMayXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuXetNghiem;
using Camino.Core.Domain.ValueObject.Grid;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DichVuXetNghiem
{
    public interface IDichVuXetNghiemService : IMasterFileService<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>
    {
        List<DichVuXetNghiemGridVo> GetDataTreeView(QueryInfo queryInfo);
        List<DichVuXetNghiemGridVo> SearchDichVuXetNghiem(QueryInfo queryInfo);
        List<LookupItemVo> GetLoaiMau(DropDownListRequestModel queryInfo);
        Task XoaDichVuXetNghiems(long dichVuXetNghiemId);
        Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien DichVuKyThuatBenhVienEntity(long dichVuXetNghiemId,long nhomDichVuBenhVienId);
        List<MauMayXetNghiemGridVo> MauMayXetNghiemGridVo(long dichVuXetNghiemId);
        Task<List<MauMayXetNghiemLookup>> MauMayXetNghiemLookUp(DropDownListRequestModel queryInfo);
        Task<List<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>> DichVuXetNghiems(long dichVuXNChaId, long dichVuXNConId, long? nhomDichVuBenhVienId);

        ///Export
        ///
        Task<List<DichVuXetNghiemGridVo>> NhomDichVuXetNghiems();

        byte[] ExportExportDataTreeViews(List<DichVuXetNghiemGridVo> nhomDichVuXetNghiems);

    }
}
