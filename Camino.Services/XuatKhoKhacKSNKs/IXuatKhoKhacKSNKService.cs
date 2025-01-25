using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
//using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.XuatKhoKSNK;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.XuatKhoKhacKSNKs
{
    public interface IXuatKhoKhacKSNKService : IMasterFileService<XuatKhoVatTu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridVatTuChildAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridVatTuChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDuocPhamChildAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridDuocPhamChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsyncDuocPhamDaDuyet(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsyncDuocPhamDaDuyet(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsyncDaDuyet(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyet(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncDpVtKsnkDaChon(QueryInfo queryInfo);
        //Task<GridDataSource> GetDataForGridAsyncVatTuDaChon(QueryInfo queryInfo);
        //Task<GridDataSource> GetTotalPageForGridAsyncVatTuDaChon(QueryInfo queryInfo);
        Task<List<YeuCauXuatKhoKSNKGridVo>> YeuCauXuatVatTuChiTiets(long yeuCauXuatKhoVatTuId);
        Task<long?> GetNhapKhoVatTuIdBy(string soChungTu);
        Task<List<LookupItemVo>> GetSoHoaDonTheoKhoKSNK(DropDownListRequestModel model);
        //Task<List<LookupItemVo>> GetSoHoaDonTheoKhoVatTus(DropDownListRequestModel model);
        Task<XuatKhoKhacKSNKResultVo> XuLyThemYeuCauXuatKhoKSNKAsync(XuatKhoKhacKSNKVo xuatKhoKhacKSNKVo, List<XuatKhoKhacKSNKChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet);
        Task<CapNhatXuatKhoKhacKSNKResultVo> XuLyCapNhatYeuCauXuatKhoKSNKAsync(XuatKhoKhacKSNKVo xuatKhoKhacKSNKVo, List<XuatKhoKhacKSNKChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet);
        //Task XuLyThemHoacCapNhatVaDuyetYeuCauVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu, List<XuatKhoKhacKSNKChiTietVo> yeuCauXuatVatTuChiTiets, bool laLuuDuyet, bool isCreate);
        Task<XuatKhoKhacVatTuRsVo> XuLyXoaYeuCauVatTuAsync(YeuCauXuatKhoVatTu yeuCauXuatKhoVatTu);
        Task CheckPhieuYeuCauXuatVTKhacDaDuyetHoacDaHuy(long yeuCauXuatKhoVatTuId);
        Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long yeuCauXuatKhoVatTuId);
        Task<List<XuatKhoKhacLookupItem>> GetKhoKsnk(DropDownListRequestModel model);
        //Task<List<XuatKhoKhacLookupItem>> GetKhoTheoLoaiVatTu(DropDownListRequestModel model);
        Task<bool> CheckSoLuongTonVatTu(long vatTuBenhVienId, bool? laVatTuBHYT, long? khoXuatId, double? soLuongXuat, string soLo, DateTime? hanSuDung);
        Task<double> GetSoLuongTonThucTe(YeuCauXuatKhoKSNKGridVo yeuCauXuatKhoVatTuGridVo);
        Task<List<LookupItemVo>> GetKhoVatTu(DropDownListRequestModel model);
        string InPhieuXuatKhoKhacKSNK(PhieuXuatKhoKhacVo phieuXuatKhoKhac);
        Task<List<YeuCauXuatKhoKSNKGridVo>> YeuCauXuatDuocPhamChiTiets(long yeuCauXuatKhoDuocPhamId);

    }
}
