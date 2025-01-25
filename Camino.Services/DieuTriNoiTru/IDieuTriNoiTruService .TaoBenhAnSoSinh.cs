using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        List<ThongTinBenhAnMe> ChonBenhAnMe(DropDownListRequestModel model);
        List<LookupItemVo> KhoaChuyenBenhAnSoSinhVe(DropDownListRequestModel model);
    }
}
