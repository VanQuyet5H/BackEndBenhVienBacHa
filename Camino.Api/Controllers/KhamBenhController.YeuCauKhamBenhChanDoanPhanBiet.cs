using Camino.Api.Models.BenhNhanDiUngThuocs;
using Camino.Api.Models.YeuCauKhamBenhChanDoanPhanBiet;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
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
        [HttpPost("LuuKhamChanDoanPhanBiet")]
        public async Task<ActionResult> LuuKhamChanDoanPhanBiet([FromBody]YeuCauKhamBenhChanDoanPhanBietViewModel chanDoanPhanBiet)
        {
            if (chanDoanPhanBiet.GhiChu == "null")
            {
                chanDoanPhanBiet.GhiChu = await _khamBenhService.GetTenThuoc(chanDoanPhanBiet.ICDId.Value);
            }

            var yeuCauKhamBenhId = chanDoanPhanBiet.YeuCauKhamBenhId;

            var chanDoanPhanBietcEntitiy = new YeuCauKhamBenhChanDoanPhanBiet
            {
                Id = chanDoanPhanBiet.Id,
                ICDId = chanDoanPhanBiet.ICDId.Value,
                GhiChu = chanDoanPhanBiet.GhiChu,
                YeuCauKhamBenhId = chanDoanPhanBiet.YeuCauKhamBenhId,
            };
            var ycKhamBenh = await _yeuCauKhamBenhService.GetByIdAsync(yeuCauKhamBenhId, o => o.Include(a => a.YeuCauKhamBenhChanDoanPhanBiets));
            ycKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.Add(chanDoanPhanBietcEntitiy);
            await _yeuCauKhamBenhService.UpdateAsync(ycKhamBenh);
            ////await _yeuCauKhamBenhChanDoanPhanBietService.AddAsync(chanDoanPhanBietcEntitiy);

            var ChanDoanPhanBietUi = new YeuCauKhamBenhChanDoanPhanBietReturnViewModel
            {
                ICDId = chanDoanPhanBietcEntitiy.Id,
                GhiChu = chanDoanPhanBiet.GhiChu,
                YeuCauKhamBenhId = yeuCauKhamBenhId,
            };

            return Ok(ChanDoanPhanBietUi);
        }
        // xoa
        [HttpPost("XoaChanDoanPhanBiet")]
        public async Task<ActionResult> XoaChanDoanPhanBiet(long id)
        {
            var ycKhamBenh = await _yeuCauKhamBenhChanDoanPhanBietService.GetByIdAsync(id, o => o.Include(a => a.YeuCauKhamBenh));

            if (ycKhamBenh == null)
            {
                return NotFound();
            }
            //var getyeuCauKhamBenhChanDoanPhanBiet = await _yeuCauKhamBenhChanDoanPhanBietService.GetChanDoanPhanBiet(id);
            //if(getyeuCauKhamBenhChanDoanPhanBiet != null)
            //{
            //   foreach(var item in getyeuCauKhamBenhChanDoanPhanBiet)
            //    {
            //        item.WillDelete = true;
            //    }
            //}
            await _yeuCauKhamBenhChanDoanPhanBietService.DeleteByIdAsync(id);
            return Ok();
        }
        // get 
        [HttpPost("GetDataGridChanDoanPhanBiet")]
        public async Task<ActionResult<GridDataSource>> GetDataGridChanDoanPhanBiet(long idYCKB)
        {
            var gridData = await _benhNhanDiUngThuocService.GetDataGridChanDoanPhanBiet(idYCKB);
            return Ok(gridData);
        }

        [HttpPost("GetDataGridChanDoanKemTheo")]
        public async Task<ActionResult<GridDataSource>> GetDataGridChanDoanKemTheo(long idYCKB)
        {
            var gridData = await _benhNhanDiUngThuocService.GetDataGridChanDoanKemTheo(idYCKB);
            return Ok(gridData);
        }
    }
}
