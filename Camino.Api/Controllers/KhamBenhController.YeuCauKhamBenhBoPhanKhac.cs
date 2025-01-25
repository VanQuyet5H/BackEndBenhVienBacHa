using Camino.Api.Models.YeuCauKhamBenhKhamBoPhanKhac;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        [HttpPost("LuuKhamKhamKhac")]
        public async Task<ActionResult> LuuKhamKhamKhac([FromBody]YeuCauKhamBenhKhamBoPhanKhacViewModel boPhanKhac)
        {

            var yeuCauKhamBenhId = boPhanKhac.YeuCauKhamBenhId;

            var boPhanKhacEntitiy = new YeuCauKhamBenhKhamBoPhanKhac
            {
                Id = boPhanKhac.Id,
                Ten = boPhanKhac.Ten,
                NoiDUng = boPhanKhac.NoiDUng,
                YeuCauKhamBenhId = boPhanKhac.YeuCauKhamBenhId,
            };
            var ycKhamBenh = await _yeuCauKhamBenhService.GetByIdAsync(yeuCauKhamBenhId, o => o.Include(a => a.YeuCauKhamBenhKhamBoPhanKhacs));
            ycKhamBenh.YeuCauKhamBenhKhamBoPhanKhacs.Add(boPhanKhacEntitiy);
            await _yeuCauKhamBenhService.UpdateAsync(ycKhamBenh);
            ////await _yeuCauKhamBenhChanDoanPhanBietService.AddAsync(chanDoanPhanBietcEntitiy);

            var ChanDoanPhanBietUi = new YeuCauKhamBenhKhamBoPhanKhacReturnViewModel
            {
                Ten = boPhanKhacEntitiy.Ten,
                NoiDUng = boPhanKhacEntitiy.NoiDUng,
                YeuCauKhamBenhId = yeuCauKhamBenhId,
            };

            return Ok(ChanDoanPhanBietUi);
        }
        // get 
        [HttpPost("GetDataGridBoPhanKhac")]
        public async Task<ActionResult<GridDataSource>> GetDataGridBoPhanKhac(long idYCKB)
        {
            var gridData = await _yeuCauKhamBenhKhamBoPhanKhacService.GetDataGridBoPhanKhac(idYCKB);
            return Ok(gridData);
        }
        // xoa
        [HttpPost("XoaChanBoPhanKhac")]
        public async Task<ActionResult> XoaChanBoPhanKhac(long id)
        {
            var ycKhamBenh = await _yeuCauKhamBenhKhamBoPhanKhacService.GetByIdAsync(id, o => o.Include(a => a.YeuCauKhamBenh));

            if (ycKhamBenh == null)
            {
                return NotFound();
            }
            await _yeuCauKhamBenhKhamBoPhanKhacService.DeleteByIdAsync(id);
            return Ok();
        }
    }
}
