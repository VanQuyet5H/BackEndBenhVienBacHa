using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<GridDataSource> GetDataForGridAsyncSuatAn(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncSuatAn(QueryInfo queryInfo);

        Task<List<LookupItemVo>> GetSuatAn(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetDoiTuongSuatAn(DropDownListRequestModel model);

        Task<Camino.Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> GetDichVuKyThuatBenhVien(long id);

        Task<YeuCauTiepNhan> ThemSuatAn(YeuCauTiepNhan yctn, long noiTruPhieuDieuTriId, Enums.DoiTuongSuDung DoiTuongId, int SoLuong, long SuatAnId, BuaAn? BuaAn);

        Task<string> InPhieuSuatAn(XacNhanInPhieuSuatAn xacNhanIn);
    }
}
