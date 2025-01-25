using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        int GetThongTinLoaiBenhAn(long yeuCauTiepNhanId);
        Task<GridDataSource> GetDataForGridNoiDungMauLoiDanBacSiAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridNoiDungMauLoiDanBacSiAsync(QueryInfo queryInfo);

        ThongTinBenhAn GetThongTinBenhAnTreSoSinh(long yeuCauTiepNhanId);
        void LuuHoacCapNhatThongTinBenhAnTreSoSinh(ThongTinBenhAn thongTinBenhAnNoiKhoaNhi);
        ThongTinBenhAn GetThongTinBenhAnSK(long yeuCauTiepNhanId);
        void LuuHoacCapNhatThongTinBenhAnSK(ThongTinBenhAn thongtin);

        #region Lấy thông tin bệnh án nội khoa 

        ThongTinBenhAn GetThongTinBenhAnNoiKhoaNhi(long yeuCauTiepNhanId);
        void LuuHoacCapNhatThongTinBenhAnNoiKhoaNhi(ThongTinBenhAn thongTinBenhAnNoiKhoaNhi);

        #endregion

        #region Lấy thông tin bệnh án phụ khoa

        ThongTinBenhAn GetThongTinBenhAnPhuKhoa(long yeuCauTiepNhanId);
        void LuuHoacCapNhatThongTinBenhAnPhuKhoa(ThongTinBenhAn thongTinBenhAnPhuKhoa);

        #endregion

        #region Lấy thông tin bệnh án ngoại khoa

        ThongTinBenhAn GetThongTinBenhAnNgoaiKhoa(long yeuCauTiepNhanId);
        void LuuHoacCapNhatThongTinBenhAnNgoaiKhoa(ThongTinBenhAn thongTinBenhAnNgoaiKhoa);

        #endregion

        #region Lấy thông tin bệnh án sản khoa mổ thường

        ThongTinBenhAn GetThongTinBenhAnSanKhoaMoThuong(long yeuCauTiepNhanId);
        void LuuHoacCapNhatThongTinBenhAnSanKhoaMoThuong(ThongTinBenhAn thongtin);

        #endregion

        #region Lấy thông tin bệnh án khoa nhi

        ThongTinBenhAn GetThongTinBenhAnNhiKhoa(long yeuCauTiepNhanId);
        void LuuHoacCapNhatThongTinBenhAnNhiKhoa(ThongTinBenhAn thongTinBenhAnNoiKhoaNhi);

        #endregion

        string InPhieuKhamBenhNoiTru(long noiTruBenhAnId);
    }
}
