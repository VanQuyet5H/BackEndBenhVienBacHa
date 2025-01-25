using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial interface IYeuCauLinhDuocPhamService
    {
        Task<GridDataSource> GetYeuCauDuocPhamBenhVienDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetYeuCauDuocPhamBenhVienTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoDuocPhamDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetBenhNhanTheoDuocPhamTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetKhoCurrentUserLinhBu(DropDownListRequestModel queryInfo);
        bool KiemTraSoLuongTon(long khoLinhTuId, long duocPhamBenhVienId, bool laDuocPhamBHYT, double? soLuongBu);
        //Task XuLyThemYeuCauLinhDuocPhamBuAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhBuDuocPham, List<long> yeuCauDuocPhamIds);
        Task XuLyThemYeuCauLinhDuocPhamBuAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhBuDuocPham, List<ThongTinDuocPhamChiTietItem> thongTinDuocPhamChiTietItems);
        Task XuLyCapNhatYeuCauLinhDuocPhamBuAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhBuDuocPham, List<ThongTinDuocPhamChiTietItem> thongTinDuocPhamChiTietItems);

        List<long> GetIdsYeuCauKhamBenh(long KhoLinhTuId, long KhoLinhVeId);
        DateTime GetDateTime(long YeuCauDuocPhamBenhVienId);
        List<long> GetIdsYeuCauDuocPhamBenhVien(long KhoLinhTuId, long KhoLinhVeId, long duocPhamId);
        List<YeuCauLinhDuocPhamBuGridParentVo> GetChiTietYeuCauDuocPhamBenhVienCanBu(long KhoLinhTuId, long KhoLinhVeId, List<long> duocPhamIds);
    }
}
