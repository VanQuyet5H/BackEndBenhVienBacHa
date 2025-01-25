using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial interface IYeuCauTiepNhanService
    {
        GridDataSource TimKiemTiepNhan(TimKiemQueryInfo queryInfo);
    }
}