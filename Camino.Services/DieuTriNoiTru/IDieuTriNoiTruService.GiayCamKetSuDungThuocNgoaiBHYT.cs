using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        string GetChanDoanNhapVienGiayCamKetSuDungNgoaiBHYT(long yctnId);
        Task<string> PhieuInGiayCamKetSuDungNgoaiBHYT(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);
    }
}
