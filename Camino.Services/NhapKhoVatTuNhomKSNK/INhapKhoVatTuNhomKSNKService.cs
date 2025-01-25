using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhaThau;
using Camino.Core.Domain.ValueObject.Thuoc;
//using Camino.Core.Domain.ValueObject.YeuCauNhapKhoVatTu;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoKSNK;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.NhapKhoVatTuNhomKSNK
{
    public partial interface INhapKhoVatTuNhomKSNKService : IMasterFileService<NhapKhoVatTu>
    {
        GridDataSource GetDataForGridAsync(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<YeuCauNhapKhoKSNKChiTietGridVo> GetVatTuGrid(YeuCauNhapKhoKSNKChiTietGridVo model);
        Task<List<LookupItemVo>> GetListLoaiSuDung(LookupQueryInfo model);
        Task<Enums.LoaiSuDung?> SuggestLoaiSuDung(long vatTuId);
        Task<List<NhaThauHopDongTemplateVo>> GetListNhaThauNhapKho(LookupQueryInfo model);
        List<VatTuNhapKhoTemplateVo> GetDropDownListVatTu(NhapKhoDuocPhamVatTuTheoHopDongThau nhapKhoInput, DropDownListRequestModel model);
        List<VatTuNhapKhoTemplateVo> GetDropDownListVatTuFromNhaThau(DropDownListRequestModel model);
        Task<long> GetKhoHanhChinh();
        Task<decimal> GetPriceOnContract(long hopDongThauId, long vatTuId);
        Task UpdateNhapKhoVatTuChiTiet(long id, double soLuongDaXuat);
        Task<NhapKhoVatTuChiTiet> GetNhapKhoVatTuChiTietById(long id);
        string InPhieuYeuCauNhapKhoVatTu(InPhieuNhapKhoKSNK inPhieuNhapKhoVatTu);
        Task<List<YeuCauNhapKhoKSNKChiTietGridVo>> YeuCauNhapKhoVatTuChiTiets(long Id, string kyHieuHD, string soChungTu, List<YeuCauNhapKhoKSNKChiTietGridVo> models);
        LookupItemVo KhoKSNK();
        NhapKhoDuocPhamTemplateVo SoLuongHopDongThauVatTu(long? hopDongThauVatTuId, long? vatTuBenhVienId, long khoId, bool? laVatTuBHYT);
        bool KiemTraNgayHetHanHopDong(long? hopDongThauVatTuId);
    }
}
