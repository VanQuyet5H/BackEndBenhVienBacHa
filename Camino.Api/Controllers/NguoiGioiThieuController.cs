using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Services.Localization;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Camino.Services.NguoiGioiThieu;
using Camino.Core.Domain.ValueObject.NguoiGioiThieu;
using Camino.Api.Models.NguoiGioiThieu;
using Camino.Core.Domain.Entities.NguoiGioiThieus;

namespace Camino.Api.Controllers
{
    public class NguoiGioiThieuController : CaminoBaseController
    {
        private readonly INguoiGioiThieuService _nguoiGioiThieuService;
        private readonly ILocalizationService _localizationService;

        public NguoiGioiThieuController(INguoiGioiThieuService nguoiGioiThieuService, ILocalizationService localizationService, IJwtFactory iJwtFactory)
        {
            _nguoiGioiThieuService = nguoiGioiThieuService;
            _localizationService = localizationService;
        }
        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNguoiGioiThieu)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nguoiGioiThieuService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNguoiGioiThieu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nguoiGioiThieuService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        [HttpPost("GetNguoiQuanLy")]
        public async Task<ActionResult<ICollection<NguoiQuanLyTemplateVo>>> GetPhongKham(DropDownListRequestModel model)
        {
            var lookup = await _nguoiGioiThieuService.GetNguoiQuanLyListAsync(model);
            return Ok(lookup);
        }


        #region Get/Add/Delete/Update
        //Add
        [HttpPost]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNguoiGioiThieu)]
        public async Task<ActionResult> Post([FromBody] NguoiGioiThieuViewModel nguoiGioiThieuViewModel)
        {
            var model = nguoiGioiThieuViewModel.ToEntity<NguoiGioiThieu>();
            await _nguoiGioiThieuService.AddAsync(model);
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model.ToModel<NguoiGioiThieuViewModel>());
        }
        //Get
        [HttpGet("{id}")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNguoiGioiThieu)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NguoiGioiThieuViewModel>> Get(long id)
        {
            var model = await _nguoiGioiThieuService.GetByIdAsync(id, s => s.Include(u => u.NhanVien).ThenInclude(u => u.User));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToModel<NguoiGioiThieuViewModel>());
        }
        //Update
        [HttpPut]
        //[ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNguoiGioiThieu)]
        public async Task<ActionResult> Put([FromBody] NguoiGioiThieuViewModel nguoiGioiThieuViewModel)
        {
            var model = await _nguoiGioiThieuService.GetByIdAsync(nguoiGioiThieuViewModel.Id);
            if (model == null)
            {
                return NotFound();
            }
            nguoiGioiThieuViewModel.ToEntity(model);
            await _nguoiGioiThieuService.UpdateAsync(model);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        //[ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNguoiGioiThieu)]
        public async Task<ActionResult> Delete(long id)
        {
            var adr = await _nguoiGioiThieuService.GetByIdAsync(id);
            if (adr == null)
            {
                return NotFound();
            }
            await _nguoiGioiThieuService.DeleteByIdAsync(id);
            return NoContent();
        }

        //Delete all selected items
        [HttpPost("Deletes")]
        //[ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNguoiGioiThieu)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var adrs = await _nguoiGioiThieuService.GetByIdsAsync(model.Ids);
            if (adrs == null)
            {
                return NotFound();
            }
            if (adrs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _nguoiGioiThieuService.DeleteAsync(adrs);
            return NoContent();
        }
        #endregion
    }
}