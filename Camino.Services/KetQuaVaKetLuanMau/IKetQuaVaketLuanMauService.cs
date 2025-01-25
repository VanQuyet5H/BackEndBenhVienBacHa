using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.KetQuaVaKetLuanMau
{
    public interface IKetQuaVaKetLuanMauService : IMasterFileService<Core.Domain.Entities.KetQuaVaKetLuanMaus.KetQuaVaKetLuanMau>
    {
        Task<bool> KiemTraTrungMa(long id, string ma);

        #region lookup
        Task<List<NoiDungMauLookupItemVo>> GetListNoiDungMauAsync(DropDownListRequestModel model);

        #endregion
    }
}
