using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GridDataSource GetDanhSachLichSuVaoVienForGrid(QueryInfo queryInfo, bool isAllData = false);
        GridDataSource GetTotalPagesDanhSachLichSuVaoVienForGrid(QueryInfo queryInfo);
        Task<List<LookupItemTemplateVo>> GetTenDichVuKhamBenh(DropDownListRequestModel model, long yeuCauTiepNhanId);
        ThongTinTheoKhamBenh GetThongTinTheoKhamBenh(long khamBenhId, long yeuCauTiepNhanId);
        ThongTinLichSuKhamBenhNoiTru ThongTinLichSuKhamBenhNoiTru(long khamBenhId, long yeuCauTiepNhanId);
    }
}
