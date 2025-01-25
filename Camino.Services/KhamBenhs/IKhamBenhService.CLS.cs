using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.DichVuKhamBenh;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.VatTuBenhViens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Microsoft.AspNetCore.Mvc;
using PhongKhamTemplateVo = Camino.Core.Domain.ValueObject.KhoaPhongNhanVien.PhongKhamTemplateVo;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.KhamBenhs
{
    public partial interface IKhamBenhService
    {
        List<LookupItemVo> GetDataLoaiGiaDichVu();
        List<LookupItemVo> GetDataNoiThucHienDichVu();
        Task<List<LookupItemVo>> GetListNhomDichVuKyThuat(DropDownListRequestModel model);
        Task<List<NhomDichVuBenhVienTreeViewVo>> GetListNhomDichVuBenhVienAsync(DropDownListRequestModel model, bool isLoadNhomTiemChung = false);
        Task<List<DichVuKyThuatBenhVienTemplateVo>> GetListDichVuKyThuat(long id, DropDownListRequestModel model);
        Task<List<DichVuKyThuatBenhVienMultiSelectTemplateVo>> GetListDichVuKyThuatMultiSelectAsync(MultiselectQueryInfo model, bool isPTTT = false, bool isPhieuDieuTri = false);
        Task<List<LookupItemVo>> GetGoiChietKhau(DropDownListRequestModel model, bool coChietKhau);
        Task<List<LookupItemVo>> GetListBacSiThucHienAsync(DropDownListRequestModel model);
        Task<List<DichVuTheoGoiVo>> GetListDichVuTheoGoiAsync(DropDownListRequestModel model);
        Task<List<LookupCheckItemVo>> GetListIdDichVuTheoGoiAsync(long goiDichVuId);
        Task<List<LookupItemVo>> GetNhomGiaTheoLoaiDichVuKyThuatAsync(DropDownListRequestModel model);

        Task<List<PhongKhamTemplateVo>> GetPhongThucHienChiDinhGiuong(DropDownListRequestModel model);
        Task<List<PhongKhamTemplateVo>> GetAllPhongBenhVienDangHoatDongAsync(DropDownListRequestModel model);
        Task<List<LookupItemGiuongBenhVo>> GetListGiuongBenhTheoPhongAsync(DropDownListRequestModel model);
        Task<List<PhongKhamTemplateVo>> GetPhongThucHienChiDinhDuocOrVatTu(DropDownListRequestModel model);
        Task<List<PhongKhamTemplateVo>> GetPhongThucHienChiDinhKhamOrDichVuKyThuat(DropDownListRequestModel model, string selectedItems = null);


        GridDataSource GetDataForGridChiTietAsync(QueryInfo queryInfo, long yeuCauTiepNhanId, long yeuCauKhamBenhId);
        //List<KhamBenhGoiDichVuGridVo> GetDataTableGoiDichVu(long goiDichVuId, long yeuCauTiepNhanId);
        List<GoiDichVuChiDinhGridVo> GetGoiDichVuKhamBenhKhac(long goiDichVuId, bool coChietkhau);
        Task<List<KhamBenhGoiDichVuGridVo>> GetDichVuKhacByTiepNhanBenhNhan(GridChiDinhDichVuQueryInfoVo queryInfo);
        Task<List<KhamBenhGoiDichVuGridVo>> GetDichVuKhacByTiepNhanBenhNhanVer2(GridChiDinhDichVuQueryInfoVo queryInfo);
        List<GoiDichVuChiDinhGridVo> GetGoiDichChietKhau(long yeuCauTiepNhanId, bool coChietkhau);



        Task<List<DichVuKhamBenhBenhVienTemplateVo>> GetListDichVuKhamBenh(DropDownListRequestModel model);
        Task<List<DichVuGiuongBenhVienTemplateVo>> GetListDichVuGiuongBenhVien(DropDownListRequestModel model);
        Task<List<VatTuBenhVienTemplateVo>> GetListVatTuYTeBenhVien(DropDownListRequestModel model);
        Task<List<YeuCauDuocPhamBenhVienTemplateVo>> GetListDuocPhamBenhVien(DropDownListRequestModel model);

        string InBaoCaoChiDinh(long yeuCauTiepNhanId, long yeuCauKhamBenhId, string hostingName, List<ListDichVuChiDinh> lst,long inChungChiDinh, bool KieuInChung, string ghiChuCLS, bool isKhamDoan,bool? inDichVuBacSiChiDinh);

        Task<ActionResult<DichVuKyThuatBenhVienTemplateVo>> GetChiDinhThongTinDichVuKyThuatAsync(long dichVuKyThuatBenhVienId);
        Task<ActionResult<List<SoDoGiuongBenhTheoPhongKhamVo>>> GetSoDoGiuongBenhTheoPhongKhamAsync(long phongBenhVienId = 0, bool giuongTrong = false, bool giuongDangSuDung = false, long dichVuGiuongBenhVienId = 0, long noiThucHienId = 0, long giuongBenhId = 0);
        #region cập nhật tính giá bhtt

        //Task<ChiTietBaoHiemThanhToanVo> XuLyChiTietBaoHiemThanhToanAsync(ChiTietBaoHiemThanhToanVo chiTietThanhToan);
        Task<decimal> GetDonGiaBenhVienDichVuKyThuatAsync(long dichVuKyThuatBenhVienId, long nhomGia);
        Task<decimal> GetDonGiaBenhVienDichVuKhamBenhAsync(long dichVuKhamBenhBenhVienId, long nhomGia);

        #endregion

        Task CapNhatHangChoKhiChiDinhDichVuKyThuatAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId, long phongBenhVienId);


        Task<GridDataSource> GetChiDinhCuaBacSiKhacDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetChiDinhBacSiKhacTotalPageForGridAsync(QueryInfo queryInfo);
        Task<NoiThucHienDichVuAutoVo> AutoGetThongTinNoiThucHienDichVuGiuong(long dichVuGiuongBenhVienId);
        Task XuLyThemYeuCauDichVuKyThuatMultiselectAsync(ChiDinhDichVuKyThuatMultiselectVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhan);

        Task<List<KhoSapXepUuTienLookupItemVo>> GetListKhoSapXepUutienAsync(DropDownListRequestModel model);
        Task<EnumLoaiKhoDuocPham> GetLoaiKhoAsync(long khoId);
        Task<List<DichVuCanGhiNhanVTTHThuocVo>> GetListDichVuCanGhiNhanVTTHThuocAsync(DropDownListRequestModel model);
        Task<List<VatTuThuocTieuHaoVo>> GetListVatTuTieuHaoThuocAsync(DropDownListRequestModel model);
        Task XuLyThemGhiNhanVatTuBenhVienAsync(ChiDinhGhiNhanVatTuThuocTieuHaoVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet);
		Task<List<GhiNhanVatTuTieuHaoThuocGridVo>> GetGridDataGhiNhanVTTHThuocAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId);
        Task<List<GhiNhanVatTuTieuHaoThuocGroupParentGridVo>> GetGridDataGhiNhanVTTHThuocAsyncVer2(long yeuCauTiepNhanId, long yeuCauKhamBenhId);
        Task<List<GhiNhanVatTuTieuHaoThuocGridVo>> GetGridDataGhiNhanVTTHcAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId);
        Task XuLyXoaYeuCauGhiNhanVTTHThuocAsync(YeuCauTiepNhan yeuCauTiepNhanChiTiet, string yeuCauGhiNhanId);
        Task CapNhatGridItemGhiNhanVTTHThuocAsync(YeuCauTiepNhan yeuCauTiepNhanChiTiet, ChiDinhGhiNhanVatTuThuocTieuHaoVo ghiNhanVo);
        Task CapNhatSoLuongTonKhiGhiNhanVTTHThuocAsync(List<NhapKhoDuocPhamChiTiet> lstNhapKhoDuocPhamChiTiet, List<NhapKhoVatTuChiTiet> lstNhapKhoVatTuChiTiet);
        Task XuLyXuatYeuCauGhiNhanVTTHThuocAsync(ChiDinhGhiNhanVatTuThuocTieuHaoVo yeuCauVo);

        #region Nhóm dịch vụ thường dùng

        #region Grid
        Task<GridDataSource> GetNhomDichVuThuongDungDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetNhomDichVuThuongDungTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetChiTietDichVuThuongDungTrongGoiDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetChiTietDichVuThuongDungTrongGoiTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetGoiDichVuCuaBenhNhanDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetGoiDichVuCuaBenhNhanTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSourceChiTietGoiDichVuTheoBenhNhan> GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync(QueryInfo queryInfo, bool isDieuTriNoiTru = false, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null);
        Task<GridDataSourceChiTietGoiDichVuTheoBenhNhan> GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync(QueryInfo queryInfo, bool isDieuTriNoiTru = false);

        #endregion

        Task<List<ChiDinhGoiDichVuThuongDungDichVuLoiVo>> KiemTraDichVuTrongGoiDaCoTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, List<long> lstGoiDichVuId, List<DichVuDaChonYCTNVo> danhSachDichVuChons = null, bool laPTTT = false, long? phieuDieuTriId = null);
        Task XuLyThemGoiDichVuThuongDungAsync(YeuCauTiepNhan yeuCauTiepNhan, YeuCauThemGoiDichVuThuongDungVo yeuCauVo);
        Task<bool> KiemTraDangKyGoiDichVuTheoBenhNhanAsync(long benhNhanId);

        Task<List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>> KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync(long yeuCauTiepNhanId, List<string> lstGoiDichVuId, long? noiTruPhieuDieuTriId = null);
        Task XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(YeuCauTiepNhan yeuCauTiepNhan, ChiDinhGoiDichVuTheoBenhNhanVo yeuCauVo);
        Task KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(ChiDinhGoiDichVuTheoBenhNhanVo yeuCauVo);
        Task<int> GetSoLuongConLaiDichVuKyThuatTrongGoiMarketingBenhNhanAsync(long yeuCauGoiDichVuId, long dichVuKyThuatBenhVienId);
        Task<int> GetSoLuongConLaiDichVuKyThuatKhuyenMaiTrongGoiMarketingBenhNhanAsync(long yeuCauGoiDichVuId, long dichVuKyThuatBenhVienId);

        Task KiemTraDichVuChiDinhCoTrongGoiCuaBenhNhanAsync(DichVuChiDinhCoTrongGoiCuaBenhNhanVo dichVuChiDinhCoTrongGoiCuaBenhNhanVo);

        #endregion

        Task CapNhatGhiChuCanLamSangAsync(UpdateGhiChuCanLamSangVo updateVo);
        Task<List<string>> GetGhiChuDichVuCanLamSangsAsync(DropDownListRequestModel model);
        Task GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(ThongTinDichVuTrongGoi thongTinChiDinhVo);

        #region Cập nhật ghi nhận VTTH/Thuốc 24/05/2021
        Task<bool> KiemTraSoLuongTonCuaThuocVTTHHienTaiAsync(UpdateSoLuongItemGhiNhanVTTHThuocVo updateVo);
        //bool KiemTraTrungGhiNhanVTTHThuoc(VTTHThuocCanKiemTraTrungKhiThemVo info, YeuCauTiepNhan yeuCauTiepNhan);
        bool KiemTraTrungGhiNhanVTTHThuoc(VTTHThuocCanKiemTraTrungKhiThemVo info, long yeuCauTiepNhanId);
        #endregion
        #region search popin 
        Task<List<TimKiemPopupInKhamBenhKhamBenhGoiDichVuGridVo>> GetDanhSachSearchPopupInChiDinhKhamBenhForGrid(TimKiemPopupInKhamBenhGoiDichVuVo model);
        #endregion
        #region gét danh sách tất cả in chỉ định của bệnh nhân khác hủy
        GridDataSource GetDanhSachDichVuChiDinhCuaBenhNhan(long yeuCauTiepNhanId, long yeuCauKhamBenhId);
        #endregion

        #region BVHD-3298: [PHÁT SINH TRIỂN KHAI] [Chuyển DV khám] Chỉ cho phép chuyển phòng cho DV khám trong khám bệnh
        Task<List<LookupItemTemplateVo>> GetListPhongThucHienDichVuTrongKhoaHienTaiAsync(DropDownListRequestModel model);
        Task XuLyChuyenPhongThucHienDichVuKhamAsync(PhongKhamChuyenDenInfoVo phongKhamChuyenDenInfoVo);

        #endregion
        #region BVHD-3761
        void UpdateDichVuKyThuatSarsCoVTheoYeuCauTiepNhan(long yeuCauTiepNhanId, string bieuHienLamSang, string dichTe);
        #endregion
    }

}
