using Camino.Core.Domain.ValueObject.Home;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<ThongKeBenhVien> GetThongKeBenhVienAsync(ThongKeKhamBenhSearch queryInfo);
    }
}
