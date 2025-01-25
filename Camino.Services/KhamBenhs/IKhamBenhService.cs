using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.TemplateKhamBenhTheoDichVus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ChuanDoan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ICDs;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Services.KhamBenhs
{
    public partial interface IKhamBenhService : IMasterFileService<ICD>
    {
        Task<List<ICDTemplateVo>> GetListBenh(DropDownListRequestModel queryInfo);

        string GetTenBenh(int id);

        Task<List<ThuocHoacHoatChatTemplateVo>> GetListThuoc(DropDownListRequestModel model);

        Task<string> GetTenThuoc(long id);

        List<LookupItemVo> GetListNhomMau(LookupQueryInfo queryInfo);

        string GetTenNhomMau(int id);

        Task<List<LookupItemVo>> GetListTrieuChung(DropDownListRequestModel model);

        Task<List<ChuanDoanTemplateVo>> GetListChuanDoan(DropDownListRequestModel queryInfo);

        long GetIdYeuCauTiepNhan(long idTrieuChung, long idYeuCauTiepNhan);

        long GetIdYeuCauChuanDoan(long idChuanDoan, long idYeuCauTiepNhan);

        List<long> DeleteAllTrieuChungByYctn(long idYeuCauTiepNhan);

        List<long> DeleteAllChuanDoanByYeuCauTiepNhan(long idYeuCauTiepNhan);

        string GetTenNhanVien(long id);
        List<LookupItemVo> getListLoaiDiUng();

        Task<TemplateKhamBenhTheoDichVu> GetKhoaPhong(long idDichVuKhamBenh);

        Task<string> GetLoiDan(long id);

        Task<YeuCauTiepNhan> GetYeuCauTiepNhanByIdAsync(long yeuCauTiepNhanId);
        List<LookupItemVo> getListLoaiTienSuBenh();

        Task<string> GetTemplateKhamBenhTheoDichVuKham(long dichVuKhamBenhid);
        decimal GetMucTranChiPhi();


        //Cập nhật 17/03/2022: get thông tin YCTN khi thêm dịch vụ
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaDichVuNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaNhieuDichVuNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaVTTHThuocNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiSuaXoaDVKyThuatNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDvThuongDungNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDVTrongGoiNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanValidationChuyenDichVuKhamNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiChuyenDichVuKhamNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDVKhamSucKhoeByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiSuaXoaDVKhamSucKhoeByIdAsync(long yeuCauTiepNhanId);

        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaRiengDichVuKhamNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaRiengDichVuKhamTabChiDinhByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaRiengDichVuKyThuatNgoaiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiCapNhatRiengDichVuKyThuatTiemChungByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuKyThuatKhuyenMaiNgoaiTruByIdAsync(long yeuCauTiepNhanId);

        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuKhamNoiTruByIdAsync(long yeuCauTiepNhanId);
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiThemDichVuKyThuatNoiTruByIdAsync(long yeuCauTiepNhanId);
        
        Task<YeuCauTiepNhan> GetYeuCauTiepNhanKhiXoaRiengDichVuKyThuatNoiTruByIdAsync(long yeuCauTiepNhanId);

        #region cập nhật 5/12/2022
        Task<MienGiamChiPhi> GetMienGiamChiPhiTrongGoiTheoDichVu(long? yeuCauKhamBenhId = null, long? yeuCauKyThuatId = null);
        Task<bool> KiemTraTaiKhoanBenhNhanChiTheoDichVu(long? yeuCauKhamBenhId = null, long? yeuCauKyThuatId = null);
        Task<List<long>> KiemTraTaiKhoanBenhNhanChiTheoDichVus(List<long> yeuCauKhamBenhIds = null, List<long> yeuCauKyThuatIds = null);
        #endregion
    }
}