using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.YeuCauTiepNhans
{
    public interface IDanhSachChoKhamService : IMasterFileService<YeuCauTiepNhan>
    {
        Task<GridDataSource> GetDataForGridAsyncDanhSachChoKham(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhSachChoKham(QueryInfo queryInfo);
        string GetBodyByName(string ten);
        List<string> InPhieuCacDichVuKhamBenh(long yeuCauTiepNhanId, string hostingName, bool header, bool laPhieuKhamBenh);
        string InTheBenhNhan(long yeuCauTiepNhanId, string hostingName);
        string InVongDeoTay(long yeuCauTiepNhanId, string hostingName);
        string XemGiayNghiHuongBHYTLien1(ThongTinNgayNghiHuongBHYTTiepNhan thongTinNgayNghi);
        string XemGiayNghiHuongBHYTLien2(ThongTinNgayNghiHuongBHYTTiepNhan thongTinNgayNghi);
        Task<List<LookupItemVo>> GetBacSiKhamBenhs(DropDownListRequestModel queryInfo);
        Task<bool> KiemTraNgayTiepNhan(DateTime? ngayTiepNhan, DateTime? ngayNghiHuong, long yeuCauTiepNhanId);
        Task<bool> KiemTraDenNgay(DateTime? ngayTiepNhan, DateTime? ngayNghiHuong, long yeuCauTiepNhanId);
        long GetThongTinYeuCauTiepNhan(TimKiemThongTin timKiemThongTin);
        ThongTinChungCuaBenhNhan ThongTinBenhNhanHienTai(long yeuCauTiepNhanId);
    }
}
