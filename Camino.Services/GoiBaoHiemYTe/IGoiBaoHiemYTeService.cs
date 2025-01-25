
using Camino.Core.Domain.Entities.HamGuiHoSoWatchings;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BHYT;
using Camino.Core.Domain.ValueObject.GoiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.HamGuiHoSoWatchings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.GoiBaoHiemYTe
{
    public partial interface IGoiBaoHiemYTeService : IMasterFileService<YeuCauTiepNhan>
    {
        #region Danh Sách Gởi Bảo Hiểm Y Tế

        Task<GridDataSource> GetDanhSachGoiBaoHiemYteForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetDanhSachGoiBaoHiemYteTotalPageForGridAsync(QueryInfo queryInfo);
        
        Task<GridDataSource> GetDanhSachLichSuBHYTForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetDanhSachLichSuBHYTTotalPageForGridAsync(QueryInfo queryInfo);

        #endregion

        ThongTinBenhNhan GetThongTinChiTietBaoHiemYTe(GoiDanhSachThongTinBenhNhanCoBHYT danhSach);
        List<LookupItemVo> GetThongTinGoiBHYTVersion(long yeuCauTiepNhan);
        
        #region CẬP NHẬT HÀM GỞI GIÁM ĐINH VÀ XUẤT XML VA LOAD THÔNG TIN BHYT

        //void CapNhatThongTinBHYT(long yeuCauTiepNhanId, HamGuiHoSoWatching hamGuiHoSoWatching);
        List<ThongTinBenhNhan> GetThongTinBenhNhanCoBHYT(DanhSachYeuCauTiepNhanIds danhSachYeuCauTiepNhanId);
        HamGuiHoSoWatchingVO addValueToXml(List<ThongTinBenhNhan> thongTinBenhNhans);
        Task<HamGuiHoSoWatchingVO> GoiHoSoGiamDinhs(List<ThongTinBenhNhan> thongTinBenhNhans);
        int LuuLichSuDuLieuGuiCongBHYT(List<ThongTinBenhNhan> thongTinBenhNhans, bool coGuiCong);
        //int LuuThongTinDaGoiVaoHeThong(List<ThongTinBenhNhan> thongTinBenhNhans);
        string KiemTraYeuCauTiepNhanGoiBHYT(long yeuCauTiepNhanId);

        #endregion
    }
}
