using System.Collections.Generic;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TonKhos;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;

namespace Camino.Services.TonKhos
{
    public partial interface ITonKhoService
    {
        Task<GridDataSource> GetDanhSachVatTuTonKhoCanhBaoForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalVatTuTonKhoCanhBaoPagesForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        string GetVatTuTonKhoCanhBaoHTML(string search);
        Task<GridDataSource> GetDanhSachVatTuTonKhoTongHopForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalVatTuTonKhoTongHopPagesForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        string GetVatTuTonKhoTongHopHTML(string search);
        Task<GridDataSource> GetDanhSachVatTuTonKhoNhapXuatForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalVatTuTonKhoNhapXuatPagesForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        string GetVatTuTonKhoNhapXuatHTML(string search);
        Task<GridDataSource> GetDanhSachVatTuTonKhoNhapXuatChiTietForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetDanhSachVatTuTonKhoNhapXuatChiTietPagesForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<ChiTietVatTuTonKhoNhapXuat> GetVatTuAndKhoName(ChiTietVatTuTonKhoNhapXuat model);
        Task<GridDataSource> GetChiTietTonKhoCuaVatTu(QueryInfo queryInfo);
        double GetTongTonKhoCuaVatTu(QueryInfo queryInfo);
        void UpdateChiTietTonKhoCuaVatTu(CapNhatTonKhoVatTuItem capNhatTonKhoItem, out List<dynamic> errors);

        Task CapNhatChiTietVatTu(CapNhatTonKhoVatTuVo capNhatTonKhoVatTuVo);

        Task<bool> KiemTraSoLuongHopLe(double? soLuong, double? soLuongXuat);

        Task<List<VatTuLookupVo>> GetVatTuBenhVien(DropDownListRequestModel queryInfo);

    }
}