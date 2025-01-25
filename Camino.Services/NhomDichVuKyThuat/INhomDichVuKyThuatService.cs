using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;

namespace Camino.Services.NhomDichVuKyThuat
{
    public interface INhomDichVuKyThuatService : IMasterFileService<Core.Domain.Entities.DichVuKyThuats.NhomDichVuKyThuat>
    {
        Task<List<NhomDichVuKyThuatGridVo>> GetDataTreeView(QueryInfo queryInfo) ;
    }
   
}
