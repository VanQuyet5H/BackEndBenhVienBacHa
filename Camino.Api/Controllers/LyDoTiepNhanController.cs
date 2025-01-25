using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain;
using Camino.Services.LyDoTiepNhan;
using Camino.Core.Domain.ValueObject.LyDoTiepNhan;
using System.Collections.Generic;
using Camino.Api.Models.LyDoTiepNhan;
using Camino.Core.Domain.Entities.LyDoTiepNhans;
using Camino.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Camino.Services.ExportImport;

namespace Camino.Api.Controllers
{

    public class LyDoTiepNhanController : CaminoBaseController
    {
        private readonly ILyDoTiepNhanService _lyDoTiepNhanService;
        private readonly IExcelService _excelService;

        public LyDoTiepNhanController(ILyDoTiepNhanService lyDoTiepNhanService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _lyDoTiepNhanService = lyDoTiepNhanService;
            _excelService = excelService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpPost("GetDataTreeView")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLyDoTiepNhan)]
        //public async Task<List<LyDoTiepNhanGridVo>> GetDataTreeView([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = await _lyDoTiepNhanService.GetDataTreeView(queryInfo);
        //    return gridData.ToList();
        //    //return Ok(gridData);
        //}

        /// Test tìm kiếm con có theo kèm theo cha. => OK.
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTreeView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLyDoTiepNhan)]
        public async Task<List<LyDoTiepNhanGridVo>> GetDataTreeView([FromBody]QueryInfo queryInfo)
        {
            var gridData = _lyDoTiepNhanService.GetDataTreeView(queryInfo);
            return gridData.ToList();
            //return Ok(gridData);
        }

        [HttpPost("GetListLyDoTiepNhanCha")]
        public async Task<ActionResult<ICollection<LookupItemTemplate>>> GetListTrieuChungCha([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _lyDoTiepNhanService.GetListLyDoTiepNhanCha(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetListLyDoTiepNhanChaChinhSua")]
        public async Task<ActionResult<ICollection<LookupItemTemplate>>> GetListLyDoTiepNhanChaChinhSua([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _lyDoTiepNhanService.GetListLyDoTiepNhanChaChinhSua(queryInfo);
            return Ok(lookup);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucLyDoTiepNhan)]
        public async Task<ActionResult<LyDoTiepNhanViewModel>> Post([FromBody] LyDoTiepNhanViewModel lyDoViewModel)
        {
            if (lyDoViewModel.LyDoTiepNhanChaId == null)
            {
                lyDoViewModel.LyDoTiepNhanChaId = null;
                lyDoViewModel.CapNhom = 0;
            }
            if (lyDoViewModel.LyDoTiepNhanChaId != null)
            {
                var capNhomCha = await _lyDoTiepNhanService.GetCapNhom(lyDoViewModel.LyDoTiepNhanChaId);
                foreach (var item in capNhomCha)
                {
                    lyDoViewModel.CapNhom = item.CapNhom;
                }
            }
            lyDoViewModel.CapNhom = lyDoViewModel.CapNhom + 1;
            var lyDo = lyDoViewModel.ToEntity<LyDoTiepNhan>();
            await _lyDoTiepNhanService.AddAsync(lyDo);
            return CreatedAtAction(nameof(Get), new { id = lyDo.Id }, lyDo.ToModel<LyDoTiepNhanViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLyDoTiepNhan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LyDoTiepNhanViewModel>> Get(long id)
        {
            var result = await _lyDoTiepNhanService.GetByIdAsync(id, s => s.Include(g => g.LyDoTiepNhans));
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<LyDoTiepNhanViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucLyDoTiepNhan)]
        public async Task<ActionResult> Put([FromBody] LyDoTiepNhanViewModel lyDoViewModel)
        {
            var getLydoCha = new LyDoTiepNhan();
            var lyDo = await _lyDoTiepNhanService.GetByIdAsync(lyDoViewModel.Id, s => s.Include(q => q.LyDoTiepNhans));
            if (lyDoViewModel.LyDoTiepNhanChaId == null)
            {
                lyDoViewModel.CapNhom = 1;
            }
            else
            {
                getLydoCha = await _lyDoTiepNhanService.GetByIdAsync(lyDoViewModel.LyDoTiepNhanChaId.Value, s => s.Include(q => q.LyDoTiepNhans));
                lyDoViewModel.CapNhom = getLydoCha.CapNhom + 1;
            }
            if (lyDoViewModel.LyDoTiepNhanChaId == lyDoViewModel.Id)
            {
                lyDoViewModel.LyDoTiepNhanChaId = null;
            }
            lyDoViewModel.ToEntity(lyDo);
            foreach (var item in lyDo.LyDoTiepNhans)
            {
                item.CapNhom = lyDo.CapNhom + 1;
            }
            await _lyDoTiepNhanService.UpdateAsync(lyDo);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucLyDoTiepNhan)]
        public async Task<ActionResult> Delete(long id)
        {
            var lyDo = await _lyDoTiepNhanService.XoaLyDoTiepNhan(id);
            return NoContent();
        }
    }
}