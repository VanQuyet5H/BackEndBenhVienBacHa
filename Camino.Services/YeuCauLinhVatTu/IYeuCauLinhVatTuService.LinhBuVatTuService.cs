using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Camino.Services.YeuCauLinhVatTu
{
    public partial interface IYeuCauLinhVatTuService
    {
        Task<List<LookupItemVo>> GetKhoCurrentUserLinhBu(DropDownListRequestModel queryInfo);
        Task<GridDataSource> GetYeuCauVatTuBenhVienDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetYeuCauVatTuBenhVienTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoVatTuDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoVatTuTotalPageForGridAsync(QueryInfo queryInfo);
        bool KiemTraSoLuongTon(long khoLinhTuId, long vatTuBenhVienId, bool laVatTuBHYT, double? soLuongBu);
        //Task XuLyThemYeuCauLinhVatTuBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<long> yeuCauVatTuIds);
        Task XuLyThemYeuCauLinhVatTuBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<ThongTinVatTuTietItem> thongTinVatTuChiTietItems);
        Task XuLyCapNhatYeuCauLinhVatTuBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<ThongTinVatTuTietItem> thongTinVatTuChiTietItems);

        List<long> GetIdsYeuCauVatTu(long KhoLinhTuId, long KhoLinhVeId);
        DateTime GetDateTime(long YeuCauVatTuBenhVienId);
        List<long> GetIdsYeuCauVT(long KhoLinhTuId, long KhoLinhVeId,long vatTuBenhVienId);
    }
}
