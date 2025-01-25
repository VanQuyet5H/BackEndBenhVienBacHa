using System.Collections.Generic;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.GachNos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Domain.ValueObject.YeuCauTraThuocTuBenhNhan;

namespace Camino.Services.ExportImport
{
    public interface IExcelService : IMasterFileService<ICD>
    {
        byte[] ExportDetailedRevenueReportByDepartment(IList<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo> baoCaoThuChi,
            DateTimeFilterVo dateTimeFilter);

        byte[] ExportAggregateRevenueReportByDepartment(IList<BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo> baoCaoThuPhiVienPhiGridVos, DateTimeFilterVo dateTimeFilter);

        byte[] ExportConsolidatedSalesReportToXlsx(IList<BaoCaoTongHopDoanhThuTheoBacSiGridVo> baoCaoThuChi, DateTimeFilterVo dateTimeFilter);

        byte[] DetailedSalesReportByDoctor(
            IList<BaoCaoChiTietDoanhThuTheoBacSiGridVo> baoCaoChiTietDoanhThuTheoBacSi,
            DateTimeFilterVo dateTimeFilter,string tenBacSi);


        #region Báo cáo thu tiền viện phí

        byte[] ExportBaoCaoThuTienVienPhi(IList<BaoCaoThuPhiVienPhiGridVo> baoCaoThuTienVienPhis , BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo, string tenNhanVien, string tenPhongBenhVien,string hosting, TotalBaoCaoThuPhiVienPhiGridVo datatotal);
        
        #endregion

        #region Báo cáo thu tiền người bệnh
        byte[] ExportBaoCaoThuTienBenhNhan(IList<BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo> baoCaoThuTienBenhNhans , BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo queryInfo, string tenNhanVien, string tenPhongBenhVien);
        #endregion

        #region export managerment view

        byte[] ExportManagermentView<T>(List<T> lstModel, List<(string, string)> valueObject, string titleName, int indexStartChildGrid = 2, string labelName = null, bool isAutomaticallyIncreasesSTT = false);

        #endregion export managerment view

        #region Báo cáo công nợ BHTN
        byte[] ExportBaoCaoCongNoCongTyBaoHiemTuNhan(ICollection<BaoCaoGachNoCongTyBhtnGridVo> baoCaoCongNoCongTyBhtns, BaoCaoGachNoCongTyBhtnGridVo congNoChung, string strQueryInfo);


        #endregion

        #region Xét nghiệm - Lấy mẫu
        byte[] ExportDanhSachLayMauXetNghiem(ICollection<LayMauXetNghiemYeuCauTiepNhanGridVo> dataLayMaus, string strQueryInfo);
        byte[] ExportDanhSachGoiMauXetNghiem(ICollection<GoiMauDanhSachXetNghiemGridVo> dataGoiMaus, string strQueryInfo);
        byte[] ExportDanhSachNhanMauXetNghiem(ICollection<NhanMauDanhSachXetNghiemGridVo> dataNhanMaus, string strQueryInfo);

        byte[] ExportPhieuTraThuocTuBenhNhanNoiTru(ICollection<YeuCauTraThuocTuBenhNhanGridVo> dataTraThuocs, string strQueryInfo);
        byte[] ExportPhieuTraVatTuTuBenhNhanNoiTru(ICollection<YeuCauTraVatTuTuBenhNhanGridVo> dataTraVatTus, string strQueryInfo);

        byte[] ExportDanhSachNguoiBenhDaCapCodeXetNghiemAsync(ICollection<BenhNhanXetNghiemGridVo> dataCapCodes, List<PhienXetNghiem> phienXetNghiemDaCaps, string strQueryInfo);
        #endregion

        #region Báo cáo danh thu theo nhóm dịch vụ

        byte[] ExportBaoCaoDoanhThuTheoNhomBenhVien(GridDataSource gridDataSource,
                                                    BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo queryInfo);
        #endregion

    }
}
