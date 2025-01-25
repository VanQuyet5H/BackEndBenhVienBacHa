using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.ThongTinCongTyBaoHiemTuNhan;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.QuayThuoc
{
    public partial interface IQuayThuocService : IMasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>
    {
        Task NguoiBenhKhongMuaDonThuoc(long yeuCauTiepNhanId);
        #region Câp nhật 12/04/2021

        Task<List<ThongTinPhieuThuQuayThuocVo>> GetSoPhieu(DropDownListRequestModel model, long yeuCauTiepNhanId);
        ThongTinPhieuThuQuayThuoc GetThongTinPhieuThuQuayThuoc(long soPhieuId);
        void HuyPhieuThu(ThongTinHuyPhieuVo thongTinHuyPhieuVo);
        void CapnhatNguoiThuHoiPhieuThuThuoc(ThongTinHuyPhieuVo thongTinHuyPhieuVo);
        #endregion

        Task<string> XemTruocBangKeThuoc(XemTruocBangKeThuoc xemTruocBangKeThuoc);
        Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocChoXuatThuocBHYT(long tiepNhanId);
        Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocChoXuatThuocKhongBHYT(long tiepNhanId);
        List<ThongTinBenhNhanGridVo> FindQuayThuoc(string search);

        YeuCauTiepNhan GetYeuCauTiepNhan(long tiepNhanId);
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, long yCTiepNhanId);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo, long benhNhanID);
        List<CongTyBaoHiemTuNhanGridVo> CheckBenhNhanExistBaoHiemTuNhan(long id);
        #region don thuoc trong ngay list
        GridDataSource GetTotalPageForGridTimBenhNhanAsync(QueryInfo queryInfo);
        GridDataSource GetDataForGridTimBenhNhanAsync(QueryInfo queryInfo,bool isPrint);
        Task<GridDataSource> GetDanhSachThuocBenhNhanChild(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachThuocBenhNhan(QueryInfo queryInfo);
        Task<string> InPhieuThuTienThuoc(XacNhanInThuocBenhNhan xacNhanIn);
        #endregion
        Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuAsync(DropDownListRequestModel queryInfo);
        Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuDinhMucAsync(DropDownListRequestModel queryInfo);
        Task<List<ThongTinDuocPhamQuayThuocVo>> GetThongTinDuocPham(long duocPhamId,long loaiDuocPhamHoacVatTu);
        Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocChoThanhToan(long tiepNhanId);

        Task<List<CongTyBaoHiemTuNhanGridVo>> GetListCongTyBaoHiemTuNhans(long tiepNhanId);

        Task<string> GetTenBenhNhan(long benhNhanId);
        Task<string> GetTenBacSI(long userId);
        Task<GridDataSource> GetDataForGridToaThuocCuAsync(QueryInfo queryInfo,  long tiepNhanId);
        Task<GridDataSource> GetTotalPageForGridToaThuocCuAsync(QueryInfo queryInfo,  long tiepNhanId);
        Task<KetQuaThemThanhToanDonThuocVo> ThanhToanDonThuoc(ThongTinDonThuocVO thongTinDonThuoc, bool xuatThuoc = false);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        Task<string> InBaoCaoToaThuocAsync(long id, bool bangKe, bool thuTien, string hostingName);

        Task<string> XacNhanInThuocCoBhyt(XacNhanInThuocBhyt xacNhanIn);
        Task<string> XacNhanXuatDonThuoc(DanhSachChoXuatThuocVO danhSachChoXuatThuoc);

        Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachChoXuatThuocKhachVangLai(long benhNhanId);

        Task<KetQuaThemThanhToanDonThuocVo> ThanhToanDonThuocKhachVanLai(KhachVangLaiThanhToanDonThuocVo thongTinDonThuoc, bool xuatThuoc = false);

        Task<KhachVangLaiNavigateVo> GetBenhNhan(long taiKhoanThuId);
        Task<List<ThongTinDuocPhamQuayThuocCuVo>> GetDonThuocChiTietCu(long[] yeuCauKhamBenhDonThuocId);
        ThongTinBenhNhanGridVo GetThongTinBenhNhan(long maBN);
        ThongTinBenhNhanGridVo GetThongTinBenhNhanTN(long maTN);

        string InPhieuVatTu(XacNhanInThuocBenhNhan xacNhanInThuocBenhNhan);

        // update ngày 22/7/2020
        Task<List<TinhThanhTemplateVo>> GetTinhThanh(DropDownListRequestModel model);
        Task<List<TinhThanhTemplateVo>> GetQuanHuyen(DropDownListRequestModel model);
        Task<List<TinhThanhTemplateVo>> GetPhuongXa(DropDownListRequestModel model);
        Task<List<TinhHuyenTemplateVo>> GetTinhHuyen(long phuongXaId);
        byte[] ExportDanhSachDonThuocTrongNgay(ICollection<DonThuocThanhToanGridVo> datas);
        bool LoaiYCTN(long tiepNhanId);

    
    }
}
