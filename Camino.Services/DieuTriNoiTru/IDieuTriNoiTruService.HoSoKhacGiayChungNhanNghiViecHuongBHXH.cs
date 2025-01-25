using Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GiayChungNhanNghiViecHuongBHXHGrid GetThongTinGiayChungNhanNghiViecHuongBHXH(long yeuCauTiepNhan);
        ThongTinChungNhanNghiViecHuongBHXH GetDataChungNhanNghiViecHuongBHXH(long yeuCauTiepNhanId);
        Task<string> InGiayChungNhanNghiViecHuongBHXH(XacNhanInPhieuGiayChungNhanNghiViecHuongBHXH xacNhanIn);
        Task<string> GetMaBS(string searching);
        
    }
}
