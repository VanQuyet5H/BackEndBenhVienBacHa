using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhaThau;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoVatTu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.NhapKhoVatTus
{
    public partial interface INhapKhoVatTuService : IMasterFileService<NhapKhoVatTu>
    {
        GridDataSource GetDataForGridAsync(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<YeuCauNhapKhoVatTuChiTietGridVo> GetVatTuGrid(YeuCauNhapKhoVatTuChiTietGridVo model);

        Task<List<LookupItemVo>> GetListLoaiSuDung(LookupQueryInfo model);

        Task<Enums.LoaiSuDung?> SuggestLoaiSuDung(long vatTuId);

        Task<List<NhaThauHopDongTemplateVo>> GetListNhaThauNhapKho(LookupQueryInfo model);

        List<VatTuNhapKhoTemplateVo> GetDropDownListVatTu(NhapKhoDuocPhamVatTuTheoHopDongThau nhapKhoInput, DropDownListRequestModel model);

        List<VatTuNhapKhoTemplateVo> GetDropDownListVatTuFromNhaThau(DropDownListRequestModel model);

        Task<long> GetKhoTongVatTu2();

        Task<decimal> GetPriceOnContract(long hopDongThauId, long vatTuId);

        Task UpdateNhapKhoVatTuChiTiet(long id, double soLuongDaXuat);

        Task<NhapKhoVatTuChiTiet> GetNhapKhoVatTuChiTietById(long id);


        string InPhieuYeuCauNhapKhoVatTu(InPhieuNhapKhoVatTu inPhieuNhapKhoVatTu);
        Task<List<YeuCauNhapKhoVatTuChiTietGridVo>> YeuCauNhapKhoVatTuChiTiets(long Id, string kyHieuHD, string soChungTu, List<YeuCauNhapKhoVatTuChiTietGridVo> models);
        LookupItemVo KhoVatTuYTe();

        NhapKhoDuocPhamTemplateVo SoLuongHopDongThauVatTu(long? hopDongThauVatTuId, long? vatTuBenhVienId, long khoId, bool? laVatTuBHYT);
        bool KiemTraNgayHetHanHopDong(long? hopDongThauVatTuId);

    }
}
