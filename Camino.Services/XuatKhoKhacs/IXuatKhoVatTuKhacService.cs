using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.XuatKhoKhacs
{
    public interface IXuatKhoVatTuKhacService : IMasterFileService<XuatKhoVatTu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsyncDaDuyet(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyet(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncVatTuDaChon(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncVatTuDaChon(QueryInfo queryInfo);
        Task<List<YeuCauXuatKhoVatTuGridVo>> YeuCauXuatVatTuChiTiets(long yeuCauXuatKhoVatTuId);
        Task<long?> GetNhapKhoVatTuIdBy(string soChungTu);
        Task<List<LookupItemVo>> GetSoHoaDonTheoKhoVatTus(DropDownListRequestModel model);

        Task XuLyThemYeuCauXuatKhoVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu, List<XuatKhoKhacVatTuChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet);
        Task<CapNhatXuatKhoKhacResultVo> XuLyCapNhatYeuCauXuatKhoVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu, List<XuatKhoKhacVatTuChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet);
        //Task XuLyThemHoacCapNhatVaDuyetYeuCauVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu, List<XuatKhoKhacVatTuChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet, bool isCreate);
        Task<XuatKhoKhacVatTuRsVo> XuLyXoaYeuCauVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu);
        Task CheckPhieuYeuCauXuatVTKhacDaDuyetHoacDaHuy(long yeuCauXuatKhoVatTuId);
        Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long yeuCauXuatKhoVatTuId);
        Task<List<XuatKhoKhacLookupItem>> GetKhoTheoLoaiVatTu(DropDownListRequestModel model);
        Task<bool> CheckSoLuongTonVatTu(long vatTuBenhVienId, bool? laVatTuBHYT, long? khoXuatId, double? soLuongXuat, string soLo, DateTime? hanSuDung);
        Task<double> GetSoLuongTonThucTe(YeuCauXuatKhoVatTuGridVo yeuCauXuatKhoVatTuGridVo);
        Task<List<LookupItemVo>> GetKhoVatTu(DropDownListRequestModel model);
    }
}
