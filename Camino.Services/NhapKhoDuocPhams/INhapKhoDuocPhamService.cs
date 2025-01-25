using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Domain.ValueObject.NhaThau;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoDuocPham;
using Camino.Core.Domain.Entities.XuatKhos;

namespace Camino.Services.NhapKhoDuocPhams
{
    public partial interface INhapKhoDuocPhamService : IMasterFileService<NhapKhoDuocPham>
    {
        GridDataSource GetDataForGridAsync(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task UpdateNhapKhoDuocPhamChiTiet(long id, double soLuongDaXuat);
        Task<NhapKhoDuocPhamChiTiet> GetNhapKhoDuocPhamChiTietById(long id);
        Task NhapKhoDuocPhamChiTietUpdateViTri(long id, long? vitriId);
        List<NhapKhoDuocPhamTemplateVo> GetDropDownListDuocPham(NhapKhoDuocPhamVatTuTheoHopDongThau nhapKhoInput, DropDownListRequestModel model);
        List<NhapKhoDuocPhamTemplateVo> GetDropDownListDuocPhamFromNhaThau(DropDownListRequestModel model);
        Task<List<NhaThauHopDongTemplateVo>> GetListNhaThauNhapKho(LookupQueryInfo model);
        Task<bool> CheckViTriKhoDuocPhamAsync(long idVitri);
        Task<bool> CheckKhoDuocPhamAsync(long idKhoDuoc);
        Task<bool> CheckMaVachAsync(long nhapKhoId, string sochungtu, long? duocPhamBenhVienId);
        Task<bool> CheckSoChungTuAsync(string mavach, long idNhapKho);

        Task<List<LookupItemVo>> GetListViTriKhoDuocPhamTheoKho(long id, LookupQueryInfo model);     
        Task<NhapKhoDuocPham> UpdateNhapKho(NhapKhoDuocPham nhapKhoDuocPham);
        Task CapNhatSoLuongThauSauKhiXoaNhapKhoAsync(List<NhapKhoDuocPhamChiTiet> nhapKhiDuocPhamChiTiets);

        // kiểm tra data
        Task<bool> KiemTraNhapKhoDaCoChiTietXuatKhoAsync(long nhapKhoId);
        Task<bool> KiemTraSoLuongNhapDuocPhamTheoHopDongThau(long? nhapKhoDuocPhamChiTiet, long? nhapKhoDuocPhamId, long? hopDongThauId, long? duocPhamId, double? soLuongNhap, double soLuongNhapTrongGrid, double soLuongHienTaiDuocPhamTheoHopDongThauDaLuu);

        Task<decimal> GetPriceOnContract(long hopDongThauId, long duocPhamId);
        //vu le
        Task<List<NhomThuocTreeViewVo>> GetListNhomThuocAsync(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetListViTriKhoTong1(LookupQueryInfo model);

        Task<YeuCauNhapKhoDuocPhamChiTietGridVo> GetDuocPhamGrid(YeuCauNhapKhoDuocPhamChiTietGridVo model);
        Task<List<LookupItemVo>> GetKhoLoaiVatTus(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetKhoTheoLoaiDuocPham(DropDownListRequestModel model);

        string InPhieuYeuCauNhapKhoDuocPham(long yeuCauNhapKhoDuocPhamId);

        Task<List<YeuCauNhapKhoDuocPhamChiTietGridVo>> YeuCauNhapKhoDuocPhamChiTiets(long Id, string kyHieuHD, string soChungTu, List<YeuCauNhapKhoDuocPhamChiTietGridVo> models);
        NhapKhoDuocPhamTemplateVo SoLuongHopDongThauDuocPham(long? hopDongThauDuocPhamId, long? duocPhamBenhVienId, long khoId, bool? laDuocPhamBHYT);
        bool KiemTraNgayHetHanHopDong(long? hopDongThauDuocPhamId);
    }
}
