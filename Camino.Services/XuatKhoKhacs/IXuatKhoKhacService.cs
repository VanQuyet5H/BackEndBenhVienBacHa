using Camino.Core.Domain.Entities.XuatKhos;
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
    public interface IXuatKhoKhacService : IMasterFileService<XuatKhoDuocPham>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsyncDaDuyet(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsyncDaDuyet(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncDuocPhamDaChon(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDuocPhamDaChon(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetKhoDuocPham(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetNhaCungCapTheoKhoDuocPhams(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetSoHoaDonTheoKhoDuocPhams(DropDownListRequestModel model);
        Task<List<XuatKhoKhacLookupItem>> GetKhoTheoLoaiDuocPham(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetNguoiNhanXuatKhac(DropDownListRequestModel model);
        Task<long?> GetNhapKhoDuocPhamIdBy(string soChungTu);

        Task XuLyThemYeuCauXuatKhoKSNKAsync(YeuCauXuatKhoDuocPham yeuCauXuatKhoDuocPham, List<XuatKhoKhacDuocPhamChiTietVo> yeuCauXuatDuocPhamChiTiets, bool laLuuDuyet);
        Task<CapNhatXuatKhoKhacResultVo> XuLyCapNhatYeuCauXuatKhoKSNKAsync(YeuCauXuatKhoDuocPham yeuCauXuatKhoDuocPham, List<XuatKhoKhacDuocPhamChiTietVo> yeuCauXuatDuocPhamChiTiets, bool laLuuDuyet);
        //Task XuLyThemHoacCapNhatVaDuyetYeuCauThuocAsync(YeuCauXuatKhoDuocPham yeuCauXuatKhoDuocPham, List<XuatKhoKhacDuocPhamChiTietVo> yeuCauXuatDuocPhamChiTiets, bool laLuuDuyet, bool isCreate);
        List<string> KiemTraXuatHoaChatTheoMayXetNghiem(List<XuatKhoKhacDuocPhamChiTietVo> yeuCauXuatDuocPhamChiTiets);
        Task<XuatKhoKhacDuocPhamRsVo> XuLyXoaYeuCauThuocAsync(YeuCauXuatKhoDuocPham yeuCauXuatKhoDuocPham);
        Task<double> GetSoLuongTonThucTe(YeuCauXuatKhoDuocPhamGridVo yeuCauXuatKhoDuocPhamGridVo);
        Task<List<YeuCauXuatKhoDuocPhamGridVo>> YeuCauXuatDuocPhamChiTiets(long yeuCauXuatKhoDuocPhamId);
        Task CheckPhieuYeuCauXuatThuocKhacDaDuyetHoacDaHuy(long yeuCauXuatKhoDuocPhamId);
        Task<TrangThaiDuyetVo> GetTrangThaiPhieuLinh(long yeuCauXuatKhoDuocPhamId);
        Task<bool> CheckSoLuongTonDuocPham(long duocPhamBenhVienId, bool? laDuocPhamBHYT, long? khoXuatId, double? soLuongXuat, string soLo, DateTime? hanSuDung);
        Task<Core.Domain.Enums.EnumLoaiKhoDuocPham> GetLoaiKho(long khoId);
        string InPhieuXuatKhoKhac(PhieuXuatKhoKhacVo phieuXuatKhoKhac);
        List<LookupItemVo> GetAllMayXetNghiemKhoKhac(DropDownListRequestModel queryInfo, DuocPhamBenhVienMayXetNghiemJson duocPhamBenhVienMayXetNghiemJson);
    }
}
