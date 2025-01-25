using Camino.Core.Domain.ValueObject.Home;
using Camino.Services.BaoCao;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public class HomeController : CaminoBaseController
    {
        private readonly IBaoCaoService _baoCaoService;

        public HomeController(IBaoCaoService baoCaoService)
        {
            _baoCaoService = baoCaoService;
        }

        [HttpPost("ThongKeBenhVien")]
        public async Task<ActionResult<ThongKeBenhVien>> ThongKeBenhVien([FromBody] ThongKeKhamBenhSearch queryInfo)
        {
            var thongKeBenhVien = await _baoCaoService.GetThongKeBenhVienAsync(queryInfo);
            return thongKeBenhVien;           
        }
    }
}

