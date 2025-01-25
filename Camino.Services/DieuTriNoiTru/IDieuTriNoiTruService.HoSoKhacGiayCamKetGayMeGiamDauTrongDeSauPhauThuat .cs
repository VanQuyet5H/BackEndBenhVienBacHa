using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        string GetTenBS(long bsId);
        Task<string> PhieuInGiayCamKetGayMeGiamDauTrongDeSauPhauThuat(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);
    }
}
