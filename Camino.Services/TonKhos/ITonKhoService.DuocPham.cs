using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ToaThuocMau;
using Camino.Core.Domain.ValueObject.TonKhos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.TonKhos
{
    public partial interface ITonKhoService
    {
        Task<List<Kho>> GetDataTonKho();
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetKho(LookupQueryInfo queryInfo);
        Task<List<LookupItemVo>> GetKhoDuocPhamAsync(LookupQueryInfo queryInfo);
        Task<List<LookupItemVo>> GetKhoVatTuChoKT(LookupQueryInfo queryInfo);
        GridDataSource GetTotalPageForGridTatCaAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridTatCaAsync(QueryInfo queryInfo, bool exportExcel = false);
        GridDataSource GetTotalPageDuocPhamSapHetHanForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForDuocPhamSapHetHanGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        List<DuocPhamSapHetHanGridVo> GetDuocPhamSapHetHan(string search);
        string GetCanhBaoDuocPhamHTML(string search);
        string GetTonKhoDuocPhamHTML(string search);
        List<TonKhoGridVo> GetTonKhoCanhBao(string search);
        List<TonKhoTatCaGridVo> GetTongHopTonKho(string search);
        string GeHTML(string search);

        #region Nhap xuat ton

        Task<GridDataSource> GetDataForGridNhapXuatTonAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridNhapXuatTonAsync(QueryInfo queryInfo);

        string GetXuatNhapTonKhoHTML(string search);

        Task<GridDataSource> GetDataForGridNhapXuatTonChiTietAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNhapXuatTonChiTietAsync(QueryInfo queryInfo);

        Task<ChiTietItem> GetChiTiet(ChiTietItem model);

        #endregion Nhap xuat ton

        Task<GridDataSource> GetChiTietTonKhoCuaDuocPham(QueryInfo queryInfo);
        double GetTongTonKhoCuaDuocPham(QueryInfo queryInfo);
        void UpdateChiTietTonKhoCuaDuocPham(CapNhatTonKhoItem capNhatTonKhoItem);

        Task<List<DuocPhamTemplateGridVo>> GetDuocPhamBenhVien(DropDownListRequestModel queryInfo);

    }
}
