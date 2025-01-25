using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.BenhAnDienTus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhAnDienTus;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BenhAnDienTus
{
    public partial interface IBenhAnDienTuService : IMasterFileService<GayBenhAn>
    {
        #region grid
        Task<GridDataSource> GetDataForGridTimKiemNoiTruBenhAnAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridTimKiemNoiTruBenhAnAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncDanhSachGayBenhAn(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachGayBenhAn(QueryInfo queryInfo);

        #endregion

        #region get data
        Task<BenhAnDienTuDetailVo> GetGayBenhAnDienTuTheoBenhAnAsnc(long noiTruBenhAnId);
        Task<YeuCauTiepNhan> GetLanTiepNhanTheoBenhAnAsnc(long noiTruBenhAn);

        #endregion
        Task<List<PhieuHoSoGayBenhAnLookupVo>> GetPhieuHoSoGayBenhAnLookupVos(DropDownListRequestModel queryInfo);
        Task<bool> IsMaTenExists(string maHoacTen = null, long Id = 0, bool flag = false);

        #region In HTML
        Task<string> InBiaBenhAnDienTuAsync(BiaBenhAnDienTuInVo detail);
        Task<string> InTaiLieuDieuTriTruocBenhAnDienTuAsync(YeuCauTiepNhan yeuCauTiepNhan);

        #endregion

        #region in dịch vụ chỉ định theo mã tiếp nhận bệnh nhân
        string InDichVuChiDinh(string hosting, long yeuCauTiepNhanId, bool header);
        #endregion
    }
}
