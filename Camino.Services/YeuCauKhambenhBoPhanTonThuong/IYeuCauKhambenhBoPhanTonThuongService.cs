using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauKhambenhBoPhanTonThuong
{
    public interface IYeuCauKhambenhBoPhanTonThuongService : IMasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhBoPhanTonThuong>
    {
        Task<ActionResult<GridDataSource>> GetDataGridBoPhanTonThuong(long idYCKB);
    }
}
