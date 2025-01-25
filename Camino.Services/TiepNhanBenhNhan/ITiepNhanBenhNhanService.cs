using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKhuyenMai;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.LyDoTiepNhan;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Services.TiepNhanBenhNhan
{
    public partial interface ITiepNhanBenhNhanService : IYeuCauTiepNhanBaseService
    {
        //Task<List<LookupItemVo>> GetLyDoKhamBenh(DropDownListRequestModel queryInfo);
        Task<List<KhoaKhamTemplateVo>> GetKhoaKham(DropDownListRequestModel model);
        Task<List<PhongKhamTemplateVo>> GetPhongKham(DropDownListRequestModel model, int tongSoNguoiKham);
        Task<List<PhongKhamDVKTTemplateVo>> GetPhongKhamKyThuat(DropDownListRequestModel model, int tongSoNguoiKham, int loai);
        Task<List<PhongKhamTemplateVo>> SettPhongKham(long khoaPhongKhamId, int tongSoNguoiKham);
        Task<List<LookupItemVo>> GetDoiTuongKhamChuaBenhUuTien(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetDanToc(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetQuocTich(DropDownListRequestModel model);
        Task<List<TinhThanhTemplateVo>> GetTinhThanh(DropDownListRequestModel model, long? quanHuyenId);
        Task<List<TinhThanhTemplateVo>> GetQuanHuyen(DropDownListRequestModel model, long? phuongXaId);
        Task<List<TinhThanhTemplateVo>> GetPhuongXa(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetNgheNghiep(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetQuanHe(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetDoiTuongUuDai(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetCongTyUuDai(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetLyDoVaoVien(DropDownListRequestModel model);

        Task<List<LookupTreeItemVo>> GetLyDoTiepNhanTreeView(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetHinhThucDen(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetGioiTinh(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetLoaiTaiLieuDinhKem(DropDownListRequestModel model);

        Task<string> GetNameLoaiTaiLieuDinhKem(long id);

        Task<List<MaDichVuTemplateVo>> NoiGioiThieu(DropDownListRequestModel model);

        Task<List<GoiDichVuTemplateVo>> GetGoiKhamTongHop(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetGoiKham(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetGoiKhamCoChietKhau(DropDownListRequestModel model);

        Task<List<MaDichVuTemplateVo>> GetDichVuKhamBenh(DropDownListRequestModel model);

        Task<List<MaDichVuTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model);

        Task<List<MaDichVuTemplateVo>> GetDichVuGiuongBenh(DropDownListRequestModel model);

        Task<List<MaDichVuTemplateVo>> GetDichVu(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetLoaiGiaKhamBenh(DropDownListRequestModel model);


        Task<BenhNhan> GetBenhNhan(string maSoTheBHYT);

        Task<bool> CheckBenhNhanTNBNExists(string maSoTheBHYT);

        //Task<bool> IsUnder6yearsold(DateTime dateTime);

        Task<BenhNhan> CreateNewBenhNhan(YeuCauTiepNhan model);
        Task UpdateBenhNhan(long id, YeuCauTiepNhan model);

        Task UpdateBenhNhanForUpdateView(long id, YeuCauTiepNhan model);

        Task<BenhNhan> CreateNewBenhNhanKhongBHYT(YeuCauTiepNhan model);

        Task<List<string>> GetListTenTrieuChungKhamAsync();
        
        Task<ChiDinhDichVuGridVo> GetDataForDichVuKhamBenhGridAsync(ThemDichVuKhamBenhVo model);
        Task<YeuCauTiepNhan> GetDataForDichVuKhamBenhForUpdateViewGridAsync(ThemDichVuKhamBenhVo model);

        Task<YeuCauTiepNhan> GetDataForDichVuKyThuatOrGiuongForUpdateViewGridAsync(ThemDichVuKhamBenhVo model, int loai);

        Task<List<ChiDinhDichVuGridVo>> GetDataForGoiCoHoacKhongChietKhauGridAsync(ThemDichVuKhamBenhVo model, bool isCoChietKhau, List<ListDichVuCheckTruocDo> ListDichVuCheckTruocDos);

        Task<List<ChiDinhDichVuGridVo>> GetDataForGoiCoHoacKhongChietKhauGridForUpdateAsync(ThemDichVuKhamBenhVo model, bool isCoChietKhau, List<ListDichVuCheckTruocDo> ListDichVuCheckTruocDos);

        Task<ChiDinhDichVuKyThuatGridVo> GetDataForDichVuKyThuatGridAsync(ThemDichVuKyThuatVo model, int loai);

        Task<double> GetDonGia(GetDonGiaVo model);

        Task<GetDonGiaVo> GetDonGiaKyThuat(GetDonGiaVo model);

        Task<double> GetDonGiaGiuongBenh(GetDonGiaVo model);

        Task<List<LookupItemVo>> GetTuyenChuyen(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetCongTyBaoHiemTuNhan(DropDownListRequestModel model);

        Task<ThemBaoHiemTuNhan> GetThongTinBHTN(long congTyBaoHiemTuNhanId);

        Task<DiaChiBHYT> GetDiaChiBHYT(DiaChiBHYT model);

        Task<ThemBaoHiemTuNhanGridVo> ThemThongTinBHTN(ThemBaoHiemTuNhan model);

        Task<List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>> SetListYeuCauKhamBenh(
            List<ChiDinhDichVuGridVo> model);

        Task<List<YeuCauDichVuKyThuat>> SetListYeuCauKyThuat(
            List<ChiDinhDichVuGridVo> model);

        Task<YeuCauTiepNhan> SetListYeuCauKhac(
            List<ChiDinhDichVuGridVo> model, YeuCauTiepNhan entity);
        //Move to ResourceHelper
        //string CreateMaYeuCauTiepNhan();
        string DisplayMaYeuCauTiepNhan(string maYeuCauTiepNhan);
        string ConvertFullMaYeuCauTiepNhan(string maYeuCauTiepNhan);

        Task<int?> GetTyLeMienGiamKhamBenh(long doiTuongId, long maDichVuId);
        Task<int?> GetTyLeMienGiamKyThuat(long doiTuongId, long maDichVuId);
        Task<string> GetTenCongTyBaoHiemTuNhan(long congTyBaoHiemTuNhanId);

        #region cap nhat TNBN
        Task<YeuCauTiepNhan> GetByIdHaveInclude(long id);
        Task<YeuCauTiepNhan> GetByIdHaveIncludeSimplify(long id);

        Task<YeuCauTiepNhan> GetByIdHaveIncludeForUpdate(long id);

        //Task<List<ChiDinhDichVuGridVo>> SetGiaForGrid(List<ChiDinhDichVuGridVo> lstGrid, int? bhytMucHuong);

        Task<List<ChiDinhDichVuGridVo>> ConvertDichVuToGridVo(YeuCauTiepNhan entity);

        Task<YeuCauTiepNhan> SetMucHuongChoDichVu(long yeuCauTiepNhanId, int mucHuong);

        Task<YeuCauTiepNhan> CheckOrUncheckBHYTForDichVu(bool isChecked, long id, string nhom, long yeuCauTiepNhanId, int mucHuong);
        Task<YeuCauTiepNhan> CapNhatDuocHuongBHYTDichVuAsync(bool isChecked, long id, string nhom, long yeuCauTiepNhanId);

        Task<YeuCauTiepNhan> AddListDichVuToServer(List<DichVuNeedUpdate> lstDichVu, long yeuCauTiepNhanId, bool coBHYT, DateTime ngayBatDau, DateTime ngayHetHan, Enums.EnumLyDoVaoVien lyDoVaoVien);

        Task<YeuCauTiepNhan> RemoveListDichVuToServer(List<DichVuNeedUpdate> lstDichVu, long yeuCauTiepNhanId, bool coBHYT, DateTime ngayBatDau, DateTime ngayHetHan, Enums.EnumLyDoVaoVien lyDoVaoVien);

        Task<YeuCauTiepNhan> RemoveDichVu(long id, string nhom, long yeuCauTiepNhanId, int? mucHuong, string lyDoHuy = null);

        Task<YeuCauTiepNhan> RemoveDichVuCoChietKhau(long goiCoChietKhauId, string nhom, long yeuCauTiepNhanId, int? mucHuong);

        Task<YeuCauTiepNhan> NoiThucHienChange(long id, string nhom, long yeuCauTiepNhanId, int? mucHuong, string noiThucHienId);

        Task<YeuCauTiepNhan> ThemGoiKhongChietKhauPopup(List<ChiDinhDichVuGridVo> lstGoi, long yeuCauTiepNhanId, int? mucHuong);

        Task<YeuCauTiepNhan> ThemGoiCoChietKhauPopup(List<ChiDinhDichVuGridVo> lstGoi, long yeuCauTiepNhanId, int? mucHuong);

        Task<YeuCauTiepNhan> HuyBHYTChiDinhDichVu(List<ListChiDinhNeedUpdate> lstChiDinhDichVu, long yeuCauTiepNhanId, int? mucHuong);
        Task<long?> GetYeuCauTiepNhanIdOfBenhNhan(string maThe, long? id = 0);

        Task<long?> GetYeuCauTiepNhanIdOfBenhNhanTrongNgay(string maThe, long? id = 0);

        Task<long?> GetYeuCauTiepNhanIdOfBenhNhanNgoaiNgay(string maThe, long? id = 0);

        Task<bool> IsHaveCongNo(long yeuCauTiepNhanId, long congTyId);

        Task<string> NoiThucHienModelText(long noiThucHienId, long? nguoiThucHienId = 0);

        Task<string> NoiThucHienKyThuatGiuongModelText(long noiThucHienId, long? nguoiThucHienId = 0);

        #endregion  cap nhat TNBN

        Task<string> GetLyDoTiepNhan(long lyDoTiepNhan);

        Task<LyDoTiepNhanDefaultDataGridVo> GetLyDoTiepNhanDefault();

        Task<GridDataSource> GetDataForGridAsyncTaiKham(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalForGridAsyncTaiKham(QueryInfo queryInfo);

        Task<QuanHuyenTinhThanhModel> GetTinhThanhQuanHuyen(long phuongXaId);

        Task<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> GetPhongBenhVien(long id);
        Task<bool> DuocHuongBHYT(long dichVuId, int loai);
        Task<decimal> GetGiaDichVu(long dichVuId, long loaiGiaId, string nhom);

        Task<GridDataSource> GetDataForGridAsyncPopupTimKiem(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncPopupTimKiem(QueryInfo queryInfo);

        Task<bool> IsUnder6YearOld(DateTime? dateTime, int? year);

        Task<DefaultValueTNBNModel> GetDefaultValueForTNBN();

        Task<GridDataSource> GetDataForGridAsyncDichVuKhuyenMais(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDichVuKhuyenMais(QueryInfo queryInfo);
        Task<GridDataSourceDichVuKhuyenMai> GetDanhSachDichVuKhuyenMaiForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncDichVuKhuyenMaiChild(QueryInfo queryInfo);

        Task<(bool, long)> KiemTraTatCaDichVuTrongGoi(long yeuCauTiepNhanId);

        #region update nhóm dịch vụ thường dùng
        Task<ChiDinhDichVuTrongNhomThuongDungVo> ThemYeuGoiDichVuThuongDungTaoMoiYCTNAsync(YeuCauThemGoiDichVuThuongDungVo yeuCauVo);

        #endregion

        #region [PHÁT SINH TRIỂN KHAI] [Tiếp đón] BN BHYT: sửa điều kiện được phép tạo yêu cầu tiếp nhận mới

        Task<KetQuaKiemTraTaoMoiYeuCauTiepNhanVo> KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(long benhNhanId, bool? laKiemTraCungNgay = false, long? yeuCauTiepNhanCapNhatId = null);

        #endregion

        #region [PHÁT SINH TRIỂN KHAI][THU NGÂN] YÊU CẦU HIỂN THỊ CẢNH BÁO KHI THÊM NB CÓ TÊN, NGÀY SINH VÀ SĐT GIỐNG NHAU

        Task<KiemTraNguoiBenhTrongHeThongVo> KiemTraBenhNhanTrongHeThongAsync(KiemTraNguoiBenhTrongHeThongVo kiemTraVo);

        #endregion

        #region BVHD-3761
        Task<DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham> GetSarsCoVs();
        Task<List<long>> GetKiemTraYeuCauDichVuKyThuatThuocNhomSarsCov2(List<long> yeuCauDichVuKyThuatIds);
        Task<InfoSarsCoVTheoYeuCauTiepNhan> GetInfoSarsCoVTheoYeuCauTiepNhan(long id);
        Task<InfoSarsCoVTheoYeuCauTiepNhan> GetInfoSarsCoVTheoYeuCauTiepNhanNoiTru(long id);
        #endregion


        #region BVHD-3825
        Task KiemTraSoLuongConLaiCuaDichVuKhuyenMaiTrongGoiAsync(ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo yeuCauVo);
        Task<List<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiVo>> KiemTraValidationChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(long yeuCauTiepNhanId, List<string> lstGoiDichVuId, long? noiTruPhieuDieuTriId = null);

        Task XuLyThemChiDinhGoiDichVuKhuyenMaiTheoBenhNhanAsync(YeuCauTiepNhan yeuCauTiepNhan, ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanVo yeuCauVo);
        Task GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(ThongTinDichVuTrongGoi thongTinChiDinhVo);
        #endregion

        #region BVHD-3575

        Task XuLyXoaYLenhVaCapNhatDienBienKhiXoaDichVuAsync(Enums.EnumNhomGoiDichVu nhomDichVu, long yeuCauDichVuId);


        #endregion

        #region BVHD-3920
        Task<bool> KiemTraBatBuocNhapNoiGioiThieuAsync(long? hinhThucDenId, long? noiGioiThieuId);


        #endregion

        #region BVHD-3941
        Task<string> GetThongTinBaoHiemTuNhanAsync(long yeuCauTiepNhanId);


        #endregion

        #region Cập nhật 16/12/2022

        List<LookupItemVo> GetListNoiThucHien(List<long> phongIds);
        (List<long>, List<long>) KiemTraDichVuCoGiaBaoHiem(List<long> dichVuKhamIds, List<long> dichVuKyThuatIds);
        (bool, bool) KiemTraDisableMienGiamGetYCTN(long yctnId);
        List<long> GetChuongTrinhGoiDichVuIdTheoBenhNhanId(long benhNhanId);
        (List<long>, List<long>) GetDichVuIdTrongGoi(List<long> chuongTrinhIds);
        (List<long>, List<long>) GetDichVuKhuyenMaiIdTrongGoi(List<long> chuongTrinhIds);
        List<LookupItemTemplate> GetYeuCauDichVuLaKhuyenMai(List<long> yeuCauDichVuIds, bool laYeuCauKham = true);
        Task<YeuCauTiepNhan> GetByIdHaveIncludeForAdddichVu(long id);
        Task<YeuCauTiepNhan> GetByIdHaveIncludeMienGiam(long id);
        Task<YeuCauTiepNhan> GetByIdHaveIncludeSimplifyChuyenDoiDichVuTrongVaNgoaiGoi(long id);
        Task<(List<long>, List<long>)> GetDichVuIdHuyThanhToan(List<long> yeuCauKhamIds, List<long> yeuCauKyThuatIds);
        #endregion

        string AddChiDinhKhamBenhTheoNguoiChiDinhVaNhom(long yeuCauTiepNhanId, List<ListDichVuChiDinhTheoNguoiChiDinh> listDichVuTheoNguoiChiDinh,
           List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK,
           List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> listDVKT, string content, string ghiChuCLS, string hostingName);
        List<long> GetListSarsCauHinh();
    }
}