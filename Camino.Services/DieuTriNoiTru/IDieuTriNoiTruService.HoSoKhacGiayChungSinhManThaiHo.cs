using Camino.Core.Domain.ValueObject.GiayChungSinhMangThaiHo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial  interface IDieuTriNoiTruService
    {
        GiayChungNhanSinhMangThaiHoGrid GetThongTinGiayChungSinhMangThaiHo(long yeuCauTiepNhan);
        ThongTinGiayChungNhanSinhMangThaiHo GetDataGiayChungSinhMangThaiHoCreate(long yeuCauTiepNhanId);
        Task<string> InGiayChungSinhMangThaiHo(XacNhanInPhieuGiaySinhMangThaiHo xacNhanIn);
        List<ThongTinDacDiemTreSoSinhGridVo> GetDacDiemTreSoSinh(long yeuCauTiepNhanId);
    }
}
