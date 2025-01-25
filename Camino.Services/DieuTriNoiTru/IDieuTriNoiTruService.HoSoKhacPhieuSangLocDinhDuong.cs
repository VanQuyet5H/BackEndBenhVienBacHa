using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        NoiTruHoSoKhac GetThongTinHoSoKhacPhieuSangLocDinhDuong(long yeuCauTiepNhanId);
        GridDataSource GetDanhSachNhuCauDinhDuong();
        List<LookupItemVo> GetListGiamCan(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListSoKgGiam(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListAnUongKem(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListTinhTrangBenhLyNang(DropDownListRequestModel queryInfo);
        LookupItemVo GetDefaultTinhTrangBenhLyNang();
        List<LookupItemVo> GetListKeHoachDinhDuong(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListTocDoTangCan(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListBenhKemTheo(DropDownListRequestModel queryInfo);
        Task<string> InPhieuSangLocDinhDuongPhuSan(long yeuCauTiepNhanId, string hosting);
        Task<string> InPhieuSangLocDinhDuong(long yeuCauTiepNhanId, string hosting, long noiTruHoSoKhacId);
        string GetChanDoanVaoVien(long yeuCauTiepNhanId);
        List<LookupItemVo> GetListSutCanMotThangQua(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListAnKemLonHon5Ngay(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListDuongNuoiDuong(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListHoiChanDinhDuong(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListTaiDanhGia(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetListTCheDoAnUongs(DropDownListRequestModel model);
        List<NoiTruHoSoKhac> GetThongTinHoSoKhacPhieuSangLocDinhDuongs(long yeuCauTiepNhanId);
        NoiTruHoSoKhac ViewThongTinHoSoKhacPhieuSangLocDinhDuong(long noiTruHoSoKhacId);
    }
}