using Camino.Core.Domain.ValueObject.PhieuCongKhaiVatTu;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        List<string> GetDataPhieuCongKhaiVatTu(PhieuCongKhaiVatTu phieuCongKhaiVatTu);
        PhieuCongKhaiVatTuObject GetThongTinPhieuCongKhaiVatTu(long yeuCauTiepNhanId);
        Task<string> InPhieuCongKhaiVatTu(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan);
    }
}
