using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        int GetTotalDanhSachNhanVienCongTyTheoHopDongKham(long hopDongKhamSucKhoeId);

        Task<bool> KiemTraTrungSTTTrongHopDongKham(long id, long hopDongKhamSucKhoeId, int? sttNhanVien);

        void MoLaiHopDongKhamSucKhoe(MoHopDongKhamViewModel moHopDongKhamViewModel);

        Task<GridDataSource> GetDSHopDongKhamForGrid(QueryInfo queryInfo, bool isAllData = false);

        Task<GridDataSource> GetTotalPagesDSHopDongKhamForGrid(QueryInfo queryInfo);

        Task<HopDongKhamSucKhoe> ThemHoacCapNhatHopDongKham(HopDongKhamSucKhoe hopDongKhamSucKhoe);

        HopDongKhamSucKhoe GetThongTinHopDongKham(long hopDongKhamSucKhoeId);
        decimal GetGiaTriHopDong(long hopDongKhamSucKhoeId);

        Task<HopDongKhamSucKhoeNhanVien> ThemHoacCapNhatHopDongKhamSucKhoeNhanVien(HopDongKhamSucKhoeNhanVien hopDongKhamSucKhoe);

        List<HopDongKhamSucKhoeNhanVien> NhanVienKhamSucKhoeTheoHDs(long hopDongKhamSucKhoeId);

        Task<GridDataSource> GetDSHopDongKhamSucKhoeNhanVienForGrid(QueryInfo queryInfo, long hopDongKhamSucKhoeId, bool isAllData = false);

        Task<GridDataSource> GetTotalPagesDSHopDongKhamSucKhoeNhanVienForGrid(QueryInfo queryInfo, long hopDongKhamSucKhoeId);

        bool XoaHopDongKhamSucKhoeNhanVien(long hopDongKhamSucKhoeId);

        HopDongKhamSucKhoeNhanVien GetThongTinHopDongKhamSucKhoeNhanVien(long hopDongKhamSucKhoeNhanVienId);

        Task<GridDataSource> GetDanhSachPhongBenhVienGrid(QueryInfo queryInfo, long hopDongKhamSucKhoeId, bool isAllData = false);

        Task<GridDataSource> GetTotalDanhSachPhongBenhVienGrid(QueryInfo queryInfo, long hopDongKhamSucKhoeId);

        (bool, string) XoaHopDongKham(long hopDongKhamSucKhoeId);

        bool XoaPhongKhamTaiCongTy(long id);

        List<KiemTraHopDongNhanVienChuaKham> KiemTraHopDongNhanVienChuaKham(long hopDongKhamSucKhoeId);

        void KetThucHopDongKham(long hopDongKhamSucKhoeId);

        List<GoiKhamSucKhoe> GetGoiKhamTheoMaHDKvaMa(long hopDongKhamSucKhoeId, string maGoiKham);

        Core.Domain.Entities.PhongBenhViens.PhongBenhVien GetPhongBenhVien(long phongBenhVienId);

        Task<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> ThemHoacCapNhatDanhSachPhongKhamCongTy
                                                                        (Core.Domain.Entities.PhongBenhViens.PhongBenhVien phongBenhVien, List<long> DanhSachNhanSu);

        Task<GridDataSource> GetDataForGridAsyncBangKeDichVuKhamDoan(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncBangKeDichVuKhamDoan(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncBangKeDichVuKhamDoanChiTiet(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncBangKeDichVuKhamDoanChiTiet(QueryInfo queryInfo);

        List<ExportExcelNhanVienDichVuTrongGoi> ExportExcelNhanVienDichVuTrongGois(long hopDongKhamSucKhoeId);
        List<ExportExcelNhanVienDichVuTrongGoi> ExportExcelNhanVienDichVuNgoaiGois(long hopDongKhamSucKhoeId);

        Task<string> GetTenCongTyTheoHopDong(long hopDongKhamSucKhoeId);

        Task<bool> KiemTraTrungMaNhanVien(long id, long hopDongKhamSucKhoeId, string maNhanVien);
        Task<bool> KiemTraTrungSoChungMinhThu(long id, long hopDongKhamSucKhoeId, string soChungMinhThu);
        Task<bool> XoaTatCaNhanVienChuaKham(long hopDongKhamSucKhoeId);
        byte[] ExportDichVuKhamDoanChiTiets(ICollection<ExportExcelNhanVienDichVuTrongGoi> dichVuKhamDoanChiTiets, string tenCongTy);

    }
}
