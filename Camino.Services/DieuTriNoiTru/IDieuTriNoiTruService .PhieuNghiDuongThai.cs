using Camino.Core.Domain.ValueObject.GiayChungNhanNghiDuongThai;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GiayChungNhanNghiDuongThaiGrid GetThongTinGiayChungNhanNghiDuongThai(long yeuCauTiepNhan);
        ThongTinChungNhanNghiDuongThai GetDataChungNhanNghiDuongThai(long yeuCauTiepNhanId);
        Task<string> InGiayChungNhanNghiDuongThai(XacNhanInPhieuGiayChungNhanNghiDuongThai xacNhanIn);
    }
}
