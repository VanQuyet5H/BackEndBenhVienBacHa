using Camino.Api.Extensions;
using Camino.Api.Models;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.HDPP;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.HDPP;
using Camino.Api.Auth;

namespace Camino.Api.Controllers
{
    public class HDPPController : CaminoBaseController
    {
        private readonly IHDPPService _hdppService;
        private readonly ILocalizationService _localizationService;

        public HDPPController(IHDPPService hdppService,
               ILocalizationService localizationService)
        {
            _hdppService = hdppService;
            _localizationService = localizationService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Core.Domain.Enums.SecurityOperation.Add, Core.Domain.Enums.DocumentType.DanhMucQuanLyHDPP)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _hdppService.GetDataHDPPForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Core.Domain.Enums.SecurityOperation.Add, Core.Domain.Enums.DocumentType.DanhMucQuanLyHDPP)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _hdppService.GetHDPPTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
       
        [HttpGet("{id}")]
        [ClaimRequirement(Core.Domain.Enums.SecurityOperation.View, Core.Domain.Enums.DocumentType.DanhMucQuanLyHDPP)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<HDPPViewModel>> Get(long id)
        {
            var result = await _hdppService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<HDPPViewModel>();
            return Ok(resultData);
        }

        [HttpPost("GetListHDPPAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListHDPPAsync(DropDownListRequestModel model)
        {
            var lookup = await _hdppService.GetListHDPPAsync(model);
            return Ok(lookup);
        }

        [HttpPost]
        [ClaimRequirement(Core.Domain.Enums.SecurityOperation.Add, Core.Domain.Enums.DocumentType.DanhMucQuanLyHDPP)]
        public async Task<ActionResult<HDPPViewModel>> Post([FromBody] HDPPViewModel hdppViewModel)
        {
            var hdpp = hdppViewModel.ToEntity<HDPP>();
            _hdppService.Add(hdpp);
            return CreatedAtAction(nameof(Get), new { id = hdpp.Id }, hdpp.ToModel<HDPPViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Core.Domain.Enums.SecurityOperation.Update, Core.Domain.Enums.DocumentType.DanhMucQuanLyHDPP)]
        public async Task<ActionResult> Put([FromBody] HDPPViewModel hdppViewModel)
        {
            var hdpp = await _hdppService.GetByIdAsync(hdppViewModel.Id);
            if (hdpp == null)
            {
                return NotFound();
            }
            hdppViewModel.ToEntity(hdpp);
            await _hdppService.UpdateAsync(hdpp);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Core.Domain.Enums.SecurityOperation.Delete, Core.Domain.Enums.DocumentType.DanhMucQuanLyHDPP)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucdanh = await _hdppService.GetByIdAsync(id);
            if (chucdanh == null)
            {
                return NotFound();
            }
            await _hdppService.DeleteByIdAsync(id);
            return NoContent();
        }

    }
}
