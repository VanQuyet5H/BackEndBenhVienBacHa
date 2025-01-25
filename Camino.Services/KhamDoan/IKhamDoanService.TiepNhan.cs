using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        #region Grid
        Task<GridDataSource> GetDataNhanVienTheoHopDongForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalNhanVienTheoHopDongForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataNhanVienTheoHopDongForGridAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalNhanVienTheoHopDongForGridAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetDataNhanVienTheoHopDongForGridAsyncVer3(QueryInfo queryInfo);

        Task<ICollection<TiepNhanDichVuChiDinhVo>> GetDanhSachDichVuTheoGoiKhamCuaBenhNhanAsync(TiepNhanDichVuChiDinhQueryVo hopDongQueryInfo);

        Task<GridDataSource> GetDataDichVuChiDinhKhamSucKhoeNhanVienForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalDichVuChiDinhKhamSucKhoeNhanVienForGridAsync(QueryInfo queryInfo);
        #endregion

        #region Get data
        Task<int> GetSoLuongConLaiDichVuTrongGoiKhamSucKhoeAsync(long goiKhamSucKhoeId, long dichVuBenhVienId, Enums.EnumNhomGoiDichVu nhomDichVu, long? yeuCauTiepNhanId = null, bool laGoiChung = true);
        Task<List<LookupItemDichVuMultiselectTemplateVo>> GetDonGiaTheoDichVuKhamSucKhoeAsync(List<LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo> lstDichVu, string goiChungStringQuery);

        Task KiemTraSoLuongConLaiNhieuDichVuTrongGoiKhamSucKhoeAsync(long goiKhamSucKhoeId, List<string> dichVuThems, List<TiepNhanDichVuChiDinhVo> dichVuGois, long? yeuCauTiepNhanId = null);
        Task KiemTraTrungDichVuKhamTrongGoiKhamSucKhoeAsync(List<string> dichVuThems, List<TiepNhanDichVuChiDinhVo> dichVuGois, List<TiepNhanDichVuChiDinhVo> dichVuNgoaiGois, long? yeuCauTiepNhanId = null);

        Task<YeuCauTiepNhan> GetYeucauTiepNhanDungDeLuuDichVuAsync(long? yeuCauTiepNhanId, long? yeuCauTiepNhanKSKId, long? benhNhanId = null);
        #endregion

        #region Xử lý data
        List<DichVuKhamBenhBenhVien> GetDichVuKhamBenhBenhViens();
        List<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> GetDichVuKyThuatBenhViens();
        List<TemplateDichVuKhamSucKhoe> GetTemplateDichVuKhamSucKhoes();
        Core.Domain.Entities.ICDs.ICD GetIcdKhamSucKhoe();
        Task XuLyLuuThongTinHopDongKhamNhanVienAsync(YeuCauTiepNhan yeuCauTiepNhanKhamSucKhoe, List<TiepNhanDichVuChiDinhVo> dichVu, bool coQuyenCapNhatHanhChinh = true
            , List<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhViens = null, List<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhViens = null, List<TemplateDichVuKhamSucKhoe> templateDichVuKhamSucKhoes = null, Core.Domain.Entities.ICDs.ICD icdKhamSucKhoe = null);
        Task XuLyXoaDichVuKhamSucKhoeChiDinhAsync(YeuCauTiepNhan yeuCauTiepNhan, long yeuCauDichVuId, bool laDichVuKham);
        Task XuLyThemDichVuKhamSucKhoeChiDinhAsync(YeuCauTiepNhan yeuCauTiepNhan, TiepNhanDichVuChiDinhVo dichVu);
        Task<List<TiepNhanDichVuChiDinhVo>> XuLyThemDichVuKhamSucKhoeChiDinhAsyncMultiselect(List<string> dichVuThems, Enums.HinhThucKhamBenh hinhThucKham, long hopDongKhamSucKhoeId, YeuCauTiepNhan yeuCauTiepNhan = null, long? HopDongKhamSucKhoeNhanVienId = null);
        Task<string> GetTenNhomGiaTheoLoaiDichVuAsync(Enums.NhomDichVuChiDinhKhamSucKhoe nhomDichVu, long NhomGiaId);
        string GetDataDefaultDichVuKhamSucKhoe(Enums.ChuyenKhoaKhamSucKhoe? chuyenKhoaKhamSucKhoe);

        Task XuLyXoaDichVuGoiChungChuaBatDauKhamAsync(DichVuGoiChungXoaChuaBatDauKhamVo xoaDichVuVo);

        Task XuLyQuayLaiChuaKhamNhieuNhanVienAsync(List<long> hopDongKSKNhanVienIds);
        #endregion

        #region Kiểm tra validator
        Task<bool> KiemTraTrungMaNhanVienTheoHopDongAsync(long hopDongId, long hopDongNhanVienId, string maNhanVien);

        Task KiemTraNguoiBenhCanQuayLaiChuaKhamKSK(List<long> hopDongKSKNhanVienIds);
        #endregion

        #region Xử lý html, pdf
        Task<string> XuLyInBangHuongDanKhamSucKhoeAsync(InHoSoKhamSucKhoeVo hoSoIn);


        #endregion
        #region in chỉ định dịch vụ ngoài gói
        string InDichVuChiDinhPhatSinh(string hosting, long yeuCauTiepNhanId, List<DichVuChiDinhInGridVo> vos);
        #endregion
    }
}
