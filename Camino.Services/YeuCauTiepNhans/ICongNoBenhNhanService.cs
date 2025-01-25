using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CongNoBenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauTiepNhans
{
    public interface ICongNoBenhNhanService : IYeuCauTiepNhanBaseService
    {
        //BVHD-3956
        byte[] ExportDanhSachConNoBenhNhan(QueryInfo queryInfo);


        Task<GridDataSource> GetDanhSachBenhNhanConNoAsync(QueryInfo queryInfo, bool exportExcel = false);
        //Task<GridDataSource> GetTotalPagesBenhNhanConNoAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachBenhNhanHetNoAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPagesBenhNhanHetNoAsync(QueryInfo queryInfo);
        Task<CongNoBenhNhanTTHCVo> GetCongNoBenhNhanTTHCAsync(long benhNhanId);
        Task<GridDataSource> GetDanhSachThongTinThuNoAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPagesThongTinThuNoAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachThongTinChuaThuNoAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPagesThongTinChuaThuNoAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachThongTinDaThuNoAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPagesThongTinDaThuNoAsync(QueryInfo queryInfo);
        decimal GetSoTienChuaThu(long taiKhoanBenhNhanThuId);
        ThongTinTraNoVo GetThongTinTraNo(long taiKhoanBenhNhanThuId);
        long ThuTienTraNo(ThuTienTraNoVo thuTienTraNoVo);

        List<HtmlToPdfVo> GetHTMLTatCaPhieuThuNoBenhNhan(long taiKhoanBenhNhanThuChinhId, string hostingName);
        List<HtmlToPdfVo> GetHTMLPhieuThuNoBenhNhan(long taiKhoanBenhNhanThuId, string hostingName);
    }
}