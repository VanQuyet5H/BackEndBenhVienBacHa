using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.NhapKhoDuocPhams
{
    public partial interface INhapKhoDuocPhamService
    {
        Task<ThongTinDuyetKhoDuocPham> GetThongTinDuyetKhoDuocPham(long yeuCauNhapKhoDuocPhamId);
        Task<bool> TuChoiDuyetDuocPhamNhapKho(ThongTinLyDoHuyNhapKhoDuocPham thongTinLyDoHuyNhapKhoDuocPham);
        Task<YeuCauNhapKhoDuocPham> GetYeuCauNhapKhoDuocPham(long id);
        Task<string> DuyetDuocPhamNhapKho(long id);
        Task<GridDataSource> GetDanhSachDuyetKhoDuocPhamForGridAsync(QueryInfo queryInfo , bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachDuyetKhoDuocPhamForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachDuyetKhoDuocPhamChiTietForGridAsync(QueryInfo queryInfo ,bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachDuyetKhoDuocPhamChiTietForGridAsync(QueryInfo queryInfo);
        Task<string> HuyDuyetNhapKhoDuocPham(long yeuCauNhapKhoDuocPhamId);
        Task<string> HuyDuyetNhapKhoVatTu(long yeuCauNhapKhoVatTuId);


    }
}