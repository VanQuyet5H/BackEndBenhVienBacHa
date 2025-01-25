using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TiemChungs;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Services.TiemChung
{
    public partial interface ITiemChungService : IYeuCauTiepNhanBaseService
    {
        #region Get data
        Task<ICollection<HangDoiTiemChungGridVo>> GetDanhSachChoKhamHienTaiAsync(HangDoiTiemChungQuyeryInfo quyeryInfo);
        Task<ICollection<HangDoiTiemChungGridVo>> GetDanhSachChoKhamHienTaiAsyncVer2(HangDoiTiemChungQuyeryInfo quyeryInfo);
        Task<ICollection<HangDoiTiemChungGridVo>> GetDanhSachChoKhamHienTaiAsyncVer3(HangDoiTiemChungQuyeryInfo quyeryInfo);
        Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungDangKhamTheoPhongKhamAsync(HangDoiTiemChungDangKhamQuyeryInfo queryInfo);
        Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(HangDoiTiemChungDangKhamQuyeryInfo queryInfo);
        Task<YeuCauDichVuKyThuat> GetThongTinYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(HangDoiTiemChungDangKhamQuyeryInfo queryInfo);
        Task<YeuCauDichVuKyThuat> GetThongTinLuuYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(HangDoiTiemChungDangKhamQuyeryInfo queryInfo);
        Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungTiepTheoAsync(HangDoiTiemChungQuyeryInfo quyeryInfo);
        Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungTiepTheoAsyncVer2(HangDoiTiemChungQuyeryInfo quyeryInfo);
        Task<YeuCauDichVuKyThuat> GetYeuCauKhamTiemChungTiepTheoAsyncVer3(HangDoiTiemChungQuyeryInfo quyeryInfo);
        #endregion

        #region Get list lookup
        List<LookupItemVo> GetListTrangThaiTiemVacxin();
        Task<List<LookupItemTemplateVo>> GetListPhongBenhVienAsync(DropDownListRequestModel model);
        Task<List<LookupItemTemplateVo>> GetListVacXinAsync(DropDownListRequestModel model);

        #endregion

        #region Cập nhật bỏ bớt include
        Task<bool?> KiemTraLanTiepNhanLaCapCuuAsync(long yeuCauTiepNhanId);
        Task<bool> KiemTraCoDichVuKhuyenMaiAsync(long benhNhanId);
        Task<List<ThongTinLoTheoXuatChiTietVo>> GetThongTinSoLoAsync(List<long> xuatChiTietIds);

        #endregion
    }
}
