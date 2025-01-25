using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKSNK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhKSNK
{
    public partial interface IYeuCauLinhKSNKService
    {
        Task<List<LookupItemVo>> GetKhoCurrentUserLinhBu(DropDownListRequestModel queryInfo);
        Task<GridDataSource> GetYeuCauKSNKBenhVienDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetYeuCauKSNKBenhVienTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoKSNKDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoKSNKTotalPageForGridAsync(QueryInfo queryInfo);
        bool KiemTraSoLuongTon(long khoLinhTuId, long vatTuBenhVienId, bool laVatTuBHYT, double? soLuongBu);
        //Task XuLyThemYeuCauLinhVatTuBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<long> yeuCauVatTuIds);
        Task XuLyThemYeuCauLinhKSNKBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<ThongTinKSNKTietItem> thongTinVatTuChiTietItems);
        Task XuLyCapNhatYeuCauLinhKSNKBuAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhBuVatTu, List<ThongTinKSNKTietItem> thongTinVatTuChiTietItems);

        List<long> GetIdsYeuCauKSNK(long KhoLinhTuId, long KhoLinhVeId);
        DateTime GetDateTime(long YeuCauVatTuBenhVienId);
        //List<long> GetIdsYeuCauVT(long KhoLinhTuId, long KhoLinhVeId, long vatTuBenhVienId);
        Task<GridDataSource> GetDataDSYeuCauLinhDuocPhamKSNKChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSYeuCauLinhDuocPhamKSNKChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDSYeuCauLinhDuocPhamKSNKChildChildForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageFDSYeuCauLinhDuocPhamKSNKChildChildForGridAsync(QueryInfo queryInfo);
        string InPhieuLinhBuKSNK(PhieuLinhThuongDPVTModel phieuLinhThuongKSNK);
    }
}
