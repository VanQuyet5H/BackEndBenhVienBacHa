using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaos;
using System;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoKetQuaKhamChuaBenh;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoVienPhiThuTien;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoThongKeDonThuoc;
using Camino.Core.Domain.ValueObject.BaoCaoLuuketQuaXeNghiemTrongNgay;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        #region Báo cáo thu tiền

        #region Báo cáo thu tiền viện phí
        Task<GridDataSource> GetBaoCaoChiTietThuTienVienPhiForGridAsync(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo);
        Task<GridItem> GetTotalBaoCaoChiTietThuTienVienPhiForGridAsync(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo);
    
        Task<GridDataSource> GetBaoCaoChiTietThuTienVienPhiForMasterGridAsync(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo);
        Task<string> GetNamePhongBenhVien(long? phongBenhVienId);
        Task<string> GetNameNhanVien(long? nhanVienId);
        Task<GridDataSource> GetBaoCaoChiTietThuTienVienPhiForDetailGridAsync(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo);
        #endregion

        #region Báo cáo thu tiền người bệnh
        Task<GridDataSource> GetBaoCaoChiTietThuTienBenhNhanForGridAsync(BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo queryInfo);
        Task<GridItem> GetTotalBaoCaoChiTietThuTienBenhNhanForGridAsync(BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo queryInfo);
        #endregion

        #endregion
        #region Báo cáo lưu trữ hồ sơ bệnh án 19/2/2021
        Task<GridDataSource> GetBaoCaoLuuTruHoSoBenhAnForGridAsync(BaoCaoLuuHoSoBenhAnVo queryInfo);
        Task<string> InBaoCaoLuuTruHoSoBenhAn(BaoCaoLuuHoSoBenhAnVo baoCaoLuuHoSoBenhAnVo);
        byte[] ExportBaoCaoLuuTruHoSoBenhAn(ICollection<BaoCaoLuuHoSoBenhAnGridVo> baoCaoLuuHoSoBenhAnGridVo, DateTime? tuNgay, DateTime? denNgay, long khoaId, string hosting);
        #endregion
        #region Báo cáo lưu trữ hồ sơ bệnh án 22/2/2021
        Task<List<ChucDanhItemVo>> GetTaCaPhongTheoKhoa(DropDownListRequestModel queryInfo);
        //Task<GridDataSource> GetBaoCaoBenhNhanKhamNgoaiTruForGridAsync(BaoCaoBenhNhanKhamNgoaiTruVo queryInfo);
        Task<GridDataSource> GetDataBaoCaoBenhNhanKhamNgoaiTruForGridAsync(BaoCaoBenhNhanKhamNgoaiTruQueryInfo queryInfo);

        Task<string> InBaoCaoBenhNhanKhamNgoaiTru(BaoCaoBenhNhanKhamNgoaiTruVo baoCaoBenhNhanKhamNgoaiTruVo);
        //byte[] ExportBaoCaoBenhNhanKhamNgoaiTru(ICollection<BaoCaoBenhNhanKhamNgoaiTruGridVo> baoCaoBenhNhanKhamNgoaiTruGridVo, DateTime? tuNgay, DateTime? denNgay,long PhongId, string hosting);
        byte[] ExportBaoCaoBenhNhanKhamNgoaiTru(GridDataSource datas, BaoCaoBenhNhanKhamNgoaiTruQueryInfo query);
        #endregion
        #region báo cáo khám chữa bệnh 24/2/2021
        string GetDataTemplateBaoCaoKetQuaKhamChuaBenhAsync(BaoCaoKetQuaKhamChuaBenhVo baoCaoKetQuaKhamChuaBenhVo);
        byte[] ExportBaoCaoKetQuaKhamChuaBenhAsync(DataTongHopBaoCaoKetQuaKhamChuaBenhVo datas, BaoCaoKetQuaKhamChuaBenhVo query);
        Task<string> InBaoCaoKetQuaKhamChuaBenh(BaoCaoKetQuaKhamChuaBenhVo baoCaoKetQuaKhamChuaBenhVo);
        #endregion
        #region báo cáo viện phí thu tiền 2/3/2021
        Task<GridDataSource> GetBaoCaoThuTienVienPhiForGridAsync(BaoCaoVienPhiThuTienVo queryInfo);
        Task<GridDataSource> GetBaoCaoThuTienVienPhiChildForGridAsync(BaoCaoVienPhiThuTienVo queryInfo);
        Task<string> InBaoCaoVienPhiThuTien(BaoCaoVienPhiThuTienVo baoCaoVienPhiThuTienVo);
        #endregion
        #region báo cáo thông kê đơn thuốc 4/3/2021
        Task<GridDataSource> GetBaoCaoThongKeDonThuocForGridAsync(BaoCaoThongKeDonThuocVo queryInfo);
        byte[] ExportBaoCaoThongKeDonThuoc(ICollection<BaoCaoThongKeDonThuocGridVo> data, DateTime? tuNgay, DateTime? denNga,string hosting);
        #endregion
        Task<TotalBaoCaoThuPhiVienPhiGridVo> GetTotalBaoCaoChiTietThuTienVienPhi(BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo);
        #region báo cáo,ecel lưu kết qua xe nghiem trong ngay
        //Task<GridDataSource> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayForGridAsync(QueryInfo queryInfo, bool exportExcel);
        //Task<GridDataSource> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayTotalPageForGridAsync(QueryInfo queryInfo);
        byte[] ExportBaoCaoLuuKetQuaXetNghiemHangNgay(ICollection<BaoCaoLuuketQuaXeNghiemTrongNgayGridVo> data, BaoCaoKetQuaXetNghiemQueryInfo baoCaoKetQuaXetNghiemQueryInfo);
        #endregion

    }
}
