using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Microsoft.EntityFrameworkCore.Query;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task<KhamDoanHopDongVaDiaDiemResultVo> GetHdKhamVaDiaDiemAsync(long id);

        Task<KhamDoanThongTinNhanVienLienQuanResultVo> GetNhanVienRelatedInfoAsync(long id);

        Task<string> GetNhanVienAsync(long nhanVienId);

        Task<long> GetLoaiNhanVienAsync(long nhanVienId);

        Task ThemYeuCauNhanSuAsync(YeuCauNhanSuKhamSucKhoe yeuCauNhanSuKhamSucKhoeEntity);

        Task UpdateYeuCauNhanSuAsync(YeuCauNhanSuKhamSucKhoe yeuCauNhanSuKhamSucKhoeEntity);

        Task<YeuCauNhanSuKhamSucKhoe> GetByYeuCauNhanSuKhamSucKhoeIdAsync
            (long id, Func<IQueryable<YeuCauNhanSuKhamSucKhoe>, IIncludableQueryable<YeuCauNhanSuKhamSucKhoe, object>> includes = null);

        TrangThaiKhamDoanAndSoLuongResultVo GetTrangThaiAndSoLuong(
            YeuCauNhanSuKhamSucKhoe ycNhanSuKhamSucKhoeEntity);

        Task DeleteByYcNhanSuKhamSucKhoeIdAsync(long id);
    }
}
