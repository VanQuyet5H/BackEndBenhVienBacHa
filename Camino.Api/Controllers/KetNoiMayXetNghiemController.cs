using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Configuration;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.ValueObject.XetNghiems;
using Camino.Services.Helpers;
using Camino.Services.ThietBiXetNghiems;
using Camino.Services.XetNghiems;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class KetNoiMayXetNghiemController : CaminoBaseController
    {
        private readonly LISConfig _lisConfig;
        private readonly IKetNoiMayXetNghiemService _ketNoiMayXetNghiemService;
        private readonly IThietBiXetNghiemService _thietBiXetNghiemService;
        public KetNoiMayXetNghiemController(LISConfig lisConfig, IKetNoiMayXetNghiemService ketNoiMayXetNghiemService, IThietBiXetNghiemService thietBiXetNghiemService)
        {
            _lisConfig = lisConfig;
            _ketNoiMayXetNghiemService = ketNoiMayXetNghiemService;
            _thietBiXetNghiemService = thietBiXetNghiemService;
        }
        [HttpPost("GoiKetQuaXetNghiem")]
        public async Task<IActionResult> GoiKetQuaXetNghiem()
        {
            if (!Directory.Exists(_lisConfig.ResultFolder))
            {
                Directory.CreateDirectory(_lisConfig.ResultFolder);
            }
            var files = HttpContext.Request.Form.Files;
            long size = files.Sum(f => f.Length);
            if (files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var filePath = Path.Combine(_lisConfig.ResultFolder, file.FileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                }
            }
            return Ok(new { count = files.Count, size });
        }
        [HttpGet("GetDanhSachMayXetNghiem")]
        public async Task<IActionResult> GetDanhSachMayXetNghiem()
        {
            var dsMayXetNghiem = await _ketNoiMayXetNghiemService.GetDanhSachMayXetNghiem();
            return Ok(dsMayXetNghiem.Select(o => o.Map<MayXetNghiemVo>()).ToList());
        }

        [HttpPost("CapNhatMayXetNghiem")]
        public async Task<ActionResult> CapNhatMayXetNghiem([FromBody]MayXetNghiemVo mayXetNghiemVo)
        {
            var mayXetNghiem = await _thietBiXetNghiemService.GetByIdAsync(mayXetNghiemVo.Id);
            mayXetNghiemVo.MapTo(mayXetNghiem);

            await _thietBiXetNghiemService.UpdateAsync(mayXetNghiem);
            return Ok();
        }

        [HttpGet("GetDanhSachChiSoXetNghiem")]
        public async Task<IActionResult> GetDanhSachChiSoXetNghiem(string barCodeNumber, long mauMayXetNghiemId)
        {
            var dsChiSo = await _ketNoiMayXetNghiemService.GetDanhSachChiSoXetNghiem(barCodeNumber, mauMayXetNghiemId);
            return Ok(dsChiSo);
        }
    }
}
