using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.NoiDungMauLoiDanBacSi
{
    public interface INoiDungMauLoiDanBacSiService : IMasterFileService<Core.Domain.Entities.NoiDungMauLoiDanBacSi.NoiDungMauLoiDanBacSi>
    {
        Task<bool> KiemTraTrungMa(long id, string ma, long loaiBenhAn);
        Task<List<NoiDungLoiDanBacSiLookupItemVo>> GetListNoiDungMauAsync(DropDownListRequestModel model, long loaiBenhAn);
    }
}
