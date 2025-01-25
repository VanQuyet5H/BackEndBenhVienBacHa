using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamBenhs;

namespace Camino.Services.TiepNhanBenhNhan
{
    public partial interface ITiepNhanBenhNhanService
    {
        Task<List<DanhSachGoiChon>> GetMarketingForBenhNhan(long benhNhanId);
        Task<GridDataSource> GetDataThongTinGoiForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataThongTinGoiForGridAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalThongTinGoiPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataDichVuGoiForGridAsync(QueryInfo queryInfo, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null);
        Task<GridDataSource> GetDataDichVuGoiForGridAsyncVer2(QueryInfo queryInfo, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null);
        Task<GridDataSource> GetTotalDichVuGoiPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataDichVuGoiForGridAsyncUpdateView(QueryInfo queryInfo, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null);
        Task<GridDataSource> GetDataDichVuGoiForGridAsyncUpdateViewVer2(QueryInfo queryInfo, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null);
        Task<GridDataSource> GetTotalDichVuGoiPageForGridAsyncUpdateView(QueryInfo queryInfo);

        Task<List<ChiDinhDichVuGridVo>> GetDataForGoiGridAsync(List<ThemDichVuKhamBenhVo> model);

        Task<DichVuTrongGoiKhiThem> CheckSoLuongTonTrongGoi(CheckDuSoLuongTonTrongGoi model);

        Task<DichVuTrongGoiKhiThem> CheckSoLuongTonTrongGoiForCreate(CheckDuSoLuongTonTrongGoi model);

        Task<bool> CheckSoLuongTonTrongGoiLstDichVu(CheckDuSoLuongTonTrongGoiListDichVu model);

        Task<bool> isValidateSoLuongTonTrongGoi(List<ChiDinhDichVuGridVo> lstDichVuThem, long benhNhanId);

        //
        Task<YeuCauTiepNhan> ThemDichVuFromGoiUpdateView(List<ThemDichVuKhamBenhVo> model, long yeuCauTiepNhanId, long benhNhanid, int? mucHuongBHYT);

        Task<string> GetTenChuongTrinhGoiDichVu(long yeuCauGoiDichVuId, bool? laDichVuKhuyenMai = false);

        Task<decimal?> GetDonGiaDichVuGoi(long yeuCauGoiDichVuId, long dichVuId, bool khamBenh = false, bool kyThuat = false, bool giuongBenh = false);

        Task<YeuCauTiepNhan> XoaGoiGiuLaiDichVu(DanhSachGoiChon model);

        Task<YeuCauTiepNhan> XoaGoiKhongGiuLaiDichVu(DanhSachGoiChon model);
    }
}
