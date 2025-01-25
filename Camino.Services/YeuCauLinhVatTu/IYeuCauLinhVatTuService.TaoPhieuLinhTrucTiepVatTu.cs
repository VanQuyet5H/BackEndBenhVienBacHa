using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhVatTu
{
    public partial interface IYeuCauLinhVatTuService
    {
        List<ThongTinLinhVatTuTuKhoGridVo> GetData(long idKhoLinh, long phongDangNhapId, string dateSearchStart, string dateSearchEnd);
        List<ThongTinLinhVatTuTuKhoGridVo> GetDataDaTao(long idKhoLinh, long idYeuCauLinhVatTu, long phongDangNhapId, long trangThai);
        List<ThongTinLinhTuKho> GetDataThongTin(long idKhoLinh);
        ThongTinLinhTuKho ThongTinDanhSachCanLinh(long idKhoLinh, long phongBenhVienId);
        List<ThongTinLinhTuKho> GetDataThongTinDaTao(long idYeuCauLinh);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        List<LinhTrucTiepVatTuChiTietGridVo> GetDataForGridChiTietChildCreateAsync(long idYCVatTuBenhVien);
        List<YeuCauVatTuBenhVienTT> GetPhieuLinhTrucTiepTT(long yeuCauTiepNhanId);
        Task<GridDataSource> GetAllYeuCauLinhVatTuTuKhoDaTao(QueryInfo queryInfo);
        long GetIdKhoNhap(long? idPhongBenhVien);
        long GetAllIdKhoNhapNhanVien(long? idNhanVien);
        string TenKhoCho(long IdDuocPham);
        bool? GetTrangThaiDuyet(long id);
        DaDuyetVatTu GetDaDuyet(long IdYeuCauLinh);
        Task<string> InPhieuLinhTrucTiepVatTu(XacNhanInLinhVatTu xacNhanInLinhVatTu);
        Task<string> InXemtruocPhieuLinhTrucTiepVatTu(XacNhanInLinhVatTuXemTruoc xacNhanInLinhVatTuXemTruoc);
        Task XuLyThemYeuCauLinhVatTuTTAsync(Core.Domain.Entities.YeuCauLinhVatTus.YeuCauLinhVatTu yeuCauLinhVT, List<long> yeuCauVatTuIds);
        #region update ds cho goi 30072021
        List<ThongTinLinhVatTuTuKhoGridVo> GetGridChoGoi(long yeuCauLinhVatTuId, string dateSearchStart, string dateSearchEnd);
        List<long> GetYeuCauVatTuIdDaTao(long yeuCauVatTuBenhVienId);
        void XuLyHuyYeuCauVatTuTTAsync(List<long> ycvtbvs);
        #endregion
        string TenNoiNhanPhieuLinhTrucTiep(long noiYeuCauId);
    }
}
