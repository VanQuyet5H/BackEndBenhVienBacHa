using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;

namespace Camino.Services.YeuCauLinhDuocPham
{
    public partial interface IYeuCauLinhDuocPhamService : IMasterFileService<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham>
    {
        Task AddAsync(Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham linhThuongDuocPham);
    }
}
