using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Services.BenhNhans
{
    public interface ITaiKhoanBenhNhanService : IMasterFileService<TaiKhoanBenhNhan>
    {
        //Task<decimal> SoDuTaiKhoan(long benhNhanId);
        Task<decimal> GetSoTienUocLuongConLai(long yeuCauTiepNhanId);
        decimal GetSoTienCanThanhToanNgoaiTru(YeuCauTiepNhan yeuCauTiepNhan);
        Task<decimal> GetSoTienDaTamUngAsync(long yeuCauTiepNhanId);
        Task<bool> KiemTraConPhieuThuCongNo(long yeuCauTiepNhanId);
    }
}
