using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial interface IYeuCauLinhDuocPhamService
    {
        Task<List<DanhSachLinhVeKhoGridVo>> GetListKhoLinhVe(DropDownListRequestModel model);
        List<ThongTinLinhTuKhoGridVo> GetData(long idKhoLinh, long phongDangNhapId, string dateSearchStart, string dateSearchEnd);
        List<ThongTinLinhTuKhoGridVo> GetDataDaTao(long idKhoLinh,long idYeuCauLinhDuocPham, long phongDangNhapId, long trangThai);
        List<ThongTinLinhTuKho> GetDataThongTin(long idKhoLinh);
        ThongTinLinhTuKho ThongTinDanhSachCanLinh(long idKhoLinh, long phongBenhVienId);
        List<ThongTinLinhTuKho> GetDataThongTinDaTao(long idYeuCauLinh);
        List<LinhTrucTiepDuocPhamChiTietGridVo> GetDataForGridChiTietChildCreateAsync(long yeuCauDuocPhamBenhVienId);
        List<YeuCauDuocPhamBenhVienTT> GetPhieuLinhTrucTiepTT(long yeuCauTiepNhanId, long? khoLinhId);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetAllYeuCauLinhThuocTuKhoDaTao(QueryInfo queryInfo);
        long GetIdKhoNhap(long? idPhongBenhVien);
        long GetAllIdKhoNhapNhanVien(long? idNhanVien);
        string TenKhoCho(long IdDuocPham);
        bool? GetTrangThaiDuyet(long id);
        DaDuyet GetDaDuyet(long IdYeuCauLinh);
        string InPhieuLinhTrucTiepDuocPham(XacNhanInLinhDuocPham xacNhanInLinhDuocPham);
        string InPhieuLinhTrucTiepDuocPhamXemTruoc(XacNhanInLinhDuocPhamXemTruoc xacNhanInLinhDuocPhamXemTruoc);
        Task XuLyThemYeuCauLinhDuocPhamTTAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham yeuCauLinhTtDuocPham, List<long> yeuCauDuocPhamIds);
        Task<List<ThongTinLanKhamKho>> GetDataThuocAsync(long yeuCauTiepNhanId, long phongBenhVienId, long nhanVienLogin, long khoLinhId);
        #region update ds cho goi 30072021
        List<ThongTinLinhTuKhoGridVo> GetGridChoGoi(long yeuCauLinhDuocPhamId, string dateSearchStart, string dateSearchEnd);
        List<long> GetYeuCauDuocPhamIdDaTao(long yeuCauDuocPhamBenhVienId);
        void XuLyHuyYeuCauDuocPhamTTAsync(List<long> ycdpbvs);
        string TenNoiNhanPhieuLinhTrucTiep(long noiYeuCauId);
        #endregion
    }
}
