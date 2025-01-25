using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        List<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo> GetDataForBaoCaoTongHopDoanhThuTheoKhoaPhong(DateTimeFilterVo dateTimeFilter);

        Task<GridDataSource> GetBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync(BaoCaoTongHopDoanhThuTheoKhoaPhongQueryInfo queryInfo);

        Task<GridItem> GetTotalBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync(BaoCaoTongHopDoanhThuTheoKhoaPhongQueryInfo queryInfo);

        #region Báo cáo xuất nhập tồn

        Task<GridDataSource> GetDataBaoCaoXuatNhapTonForGridAsync(BaoCaoXuatNhapTonQueryInfo queryInfo);
        Task<GridDataSource> GetTotalBaoCaoXuatNhapTonForGridAsync(BaoCaoXuatNhapTonQueryInfo queryInfo);
        Task<GridDataSource> GetDataBaoCaoXuatNhapTonForGridAsyncChild(BaoCaoXuatNhapTonQueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalBaoCaoXuatNhapTonForGridAsyncChild(BaoCaoXuatNhapTonQueryInfo queryInfo);
        string InBaoCaoXuatNhapTon(InBaoCaoXuatNhapTonVo inBaoCaoXuatNhapTon);
        byte[] ExportBaoCaoXuatNhapTon(GridDataSource datas, BaoCaoXuatNhapTonQueryInfo query);

        Task<GridDataSource> GetDataBaoCaoTonKhoForGridAsync(BaoCaoTonKhoQueryInfo queryInfo);
        byte[] ExportBaoCaoTonKho(GridDataSource datas, BaoCaoTonKhoQueryInfo query);
        #endregion
     

        #region Báo cáo tiếp nhận người bệnh khám

        Task<GridDataSource> GetDataBaoCaoTiepNhanBenhNhanKhamForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalBaoCaoTiepNhanBenhNhanKhamForGridAsync(QueryInfo queryInfo);
        string InBaoCaoTiepNhanBenhNhanKham(InBaoCaoTNBenhNhanKhamVo inBaoCaoTiepNhanBenhNhanKham);
        byte[] ExportBaoCaoTiepNhanBenhNhanKham(ICollection<BaoCaoTNBenhNhanKhamGridVo> datas, QueryInfo query);

        #endregion

    }
}
