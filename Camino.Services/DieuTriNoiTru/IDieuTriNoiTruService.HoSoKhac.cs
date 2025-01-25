using Camino.Core.Domain.ValueObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        Task<List<string>> GetListNhanVienAutoComplete(DropDownListRequestModel queryInfo);
        Task<List<string>> GetListChucDanhAutoComplete(DropDownListRequestModel queryInfo);
        Task<List<string>> GetListVanBangChuyenMonAutoComplete(DropDownListRequestModel queryInfo);
    }
}
