using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhieuSoKet15NgayDieuTri;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        PhieuSoKet15NgayDieuTriVo GetThongTinPhieuSoKet15NgayDieuTri(long idNguoiLogin, long yeuCauTiepNhanId);
        TrichBienBanHoiChanGridVo GetThongTinPhieuSoKet15NgayDieuTriSave(long yeuCauTiepNhanId);
        TrichBienBanHoiChanGridVo GetThongTinPhieuSoKet15NgayDieuTriViewDS(long noiTruHoSoKhacId);
        Task<string> PhieuSoKet15NgayDieuTri(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan);
        Task<string> PhieuSoKet15NgayDieuTriUpdate(PhieuDieuTriVaServicesHttpParams15Ngay xacNhanInTrichBienBanHoiChan);
        Task<GridDataSource> GetDanhSachPhieuSoKet15NgayDieuTri(QueryInfo queryInfo);
        string GetTenDangNhap();
        TrichBienBanHoiChanGridVo GetThongTinTrichBienBanHoiChanViewDS(long noiTruHoSoKhacId);
        Task<bool> KiemTraNgay(DateTime? tuNgay, DateTime? denNgay);
        Task<bool> KiemTraNgayNhan(DateTime? ngayNhan,long yeuCauTiepNhanId);
        Task<ThongTinHoSoGetInfo> GetNoiTruHoSoKhac15Ngay(long yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo);
        Task<List<ThongTinHoSoGetInfo>> GetNoiTruHoSoKhac15Ngays(long yctn, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo);
        string GetChanDoanVaoVien15Ngay(long yeuCauTiepNhanId);
    }
}
