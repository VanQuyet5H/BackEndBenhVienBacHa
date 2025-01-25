using Camino.Core.Domain.ValueObject.PhieuCongKhaiThuoc;
using Camino.Core.Domain.ValueObject.PhieuCongKhaiDichVuKyThuat;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        List<string> GetDataPhieuCongKhaiDichVuKyThuat(PhieuCongKhaiDichVuKyThuat phieuCongKhaiDichVuKyThuat);
        PhieuCongKhaiDichVuKyThuatObject GetThongTinPhieuCongKhaiDichVuKyThuat(long yeuCauTiepNhanId);
        Task<string> InPhieuCongKhaiDichVuKyThuat(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan);
    }
}
