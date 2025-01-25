using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDoanhThuKhamDoanTheoNhomDichVu;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDuTruSoLuongNguoiThucHienDvLSCLS;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCaoKhamDoanHopDong;
using Camino.Core.Domain.ValueObject.DSDichVuNgoaiGoiKeToan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.BaoCaoKhamDoanHopDong
{
    public interface IBaoCaoKhamDoanHopDongServices : IMasterFileService<HopDongKhamSucKhoeNhanVien>
    {
        //GridDataSource GetDataForGridAsyncTheoHopDong(QueryInfo queryInfo);
        GridDataSource GetDataForGridAsyncTheoHopDongVer2(QueryInfo queryInfo);

        GridDataSource GetTotalPageForGridAsyncTheoHopDong(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncTheoNhanVien(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncTheoNhanVien(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncDichVuNgoai(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncDichVuNgoai(QueryInfo queryInfo);

        byte[] ExportBaoCaoHoatDongDichVuNgoai(QueryInfo queryInfo, List<BaoCaoKhamDoanHopDongDichVuNgoaiVo> data);

        Task<List<LookupItemVo>> GetLoaiNhanVienHoacHopDong(DropDownListRequestModel queryInfo);
        string InDanhSachNhanVien(InDanhSachNhanVien inDanhSachNhanVien);

        #region BÁO CÁO TỔNG HỢP KẾT QUẢ KHÁM SỨC KHỎE
        byte[] ExportBaoCaoTongHopKetQuaKSK(List<BaoCaoTongHopKetQuaKhamDoanGridVo> list, List<BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo> listNhanVien, string tenCongTy, string soHopdong);
        Task<List<BaoCaoTongHopKetQuaKhamDoanGridVo>> ListDichVu(ModelVoNhanVien model);
        Task<List<BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo>> ListDichVuNhanVien(ModelVoNhanVien model);
        Task<List<LookupItemHopDingKhamSucKhoeTemplateVo>> GetHopDongKhamSucKhoe(DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false);
        Task<List<LookupItemTemplateVo>> GetCongTy(DropDownListRequestModel queryInfo);
        string GetNameCongTy(long congTyId);
        string GetNameHopDongKhamSucKhoe(long hopDongId);
        #endregion BÁO CÁO TỔNG HỢP KẾT QUẢ KHÁM SỨC KHỎE

        Task<GridDataSource> GetDataForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncTheoNhanVienKhamDichVuTheoPhongKham(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo);
        byte[] ExportBaoCaoHoatDongKhamDoan(BaoCaoNguoiBenhKhamDichVuTheoPhongQueryInfo queryInfo, List<NguoiBenhKhamDichVuTheoPhong> nguoiBenhKhamDichVuTheoPhongs);

        Task<string> GetHTMLBaoCaoTongHopKetQuaKSK(List<BaoCaoTongHopKetQuaKhamDoanGridVo> tatCaDichVu, List<BaoCaoTongHopKetQuaKhamDoanTheoNhanVienGridVo> listDichVuTheoNB);
        #region báo cáo dự trù số lượng người thực hiện dv ls -cls 22/10/2021
        Task<GridDataSource> ListDichVuBenhNhanDangKy(BaoCaoTongHopKetQuaKhamDoanQueryInfoQueryInfo queryInfo);
        byte[] ExportBaoCaoDuTruSLNguoiThucHienDichVu(IList<BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo> list);
        #endregion báo cáo dự trù số lượng người thực hiện dv ls -cls 22/10/2021
        #region  Báo cáo dịch vu ngoài gói 2/11/2021 
        //Task<GridDataSource> ListDichVuBenhNhanDangKy(BaoCaoTongHopKetQuaKhamDoanQueryInfoQueryInfo queryInfo);
        //byte[] ExportBaoCaoDuTruSLNguoiThucHienDichVu(IList<BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo> list);
        Task<List<BaoCaoDVNgoaiGoiKeToanGridVo>> GetAllForBaoCaoBenhNhanKhamDicVuNgoaiGoi(BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo queryInfo);
        byte[] ExportBaoCaoChiTietThuTienDichVuNgoaiGoi(IList<BaoCaoDVNgoaiGoiKeToanGridVo> baoCaoThuTienVienPhis, BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo queryInfo, string tenCongTy,TotalBaoCaoThuPhiVienPhiGridVo datatotal);
        string GetTenCongTy(long id);
        Task<TotalBaoCaoThuPhiVienPhiGridVo> GetTotalBaoCaoChiTietThuTienDichVuNgoaiGoiForGridAsync(BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo queryInfo);
        Task<List<LookupItemHopDingKhamSucKhoeTemplateVo>> GetHopDongKhamDoan(DropDownListRequestModel queryInfo);
        #endregion  Báo cáo dịch vu ngoài gói 2/11/2021 
        #region BÁO CÁO DOANH THU KHÁM ĐOÀN THEO NHÓM DỊCH VỤ
        Task<GridDataSource> BaoCaoDoanhThuKhamDoanTheoNhomDichVu(BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo queryInfo);
        byte[] ExportBaoCaoDoanhThuKhamDoanTheoNhomDichVu(IList<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo> bc, BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo queryInfo);
        #endregion
    }
}
