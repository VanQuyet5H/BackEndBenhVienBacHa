using Camino.Core.Domain.ValueObject.PhieuCongKhaiThuoc;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        List<string> GetDataPhieuCongKhaiThuoc(PhieuCongKhaiThuoc phieuCongKhaiThuoc);
        PhieuCongKhaiThuocObject GetThongTinPhieuCongKhaiThuoc(long yeuCauTiepNhanId);
        Task<string> InPhieuCongKhaiThuoc(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan);
    }
}
