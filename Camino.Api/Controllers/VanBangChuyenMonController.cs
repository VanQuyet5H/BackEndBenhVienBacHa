using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject;
using Camino.Services.VanBangChuyenMon;
using Camino.Api.Models.VanBangChuyenMon;
using Camino.Api.Extensions;
using System;
using Camino.Services.Localization;
using Camino.Api.Models.General;
using System.Linq;
using Camino.Core.Domain.Entities.VanBangChuyenMons;
using System.Collections.Generic;
using Camino.Services.ExportImport;
using Camino.Core.Domain.ValueObject.VanBangChuyenMon;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public class VanBangChuyenMonController : CaminoBaseController
    {
        private readonly IVanBangChuyenMonService _vanBangChuyenMonService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        public VanBangChuyenMonController(IVanBangChuyenMonService vanBangChuyenMonService, 
            ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _vanBangChuyenMonService = vanBangChuyenMonService;
            _localizationService =  localizationService;
            _excelService = excelService;
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVanBangChuyenMon)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _vanBangChuyenMonService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVanBangChuyenMon)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _vanBangChuyenMonService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucVanBangChuyenMon)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<VanBangChuyenMonViewModel>> Get(long id)
        {
            var result = await _vanBangChuyenMonService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var r = result.ToModel<VanBangChuyenMonViewModel>();
            return Ok(r);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucVanBangChuyenMon)]
        public ActionResult<VanBangChuyenMonViewModel> Post([FromBody] VanBangChuyenMonViewModel viewModel)
        {
            var entity = viewModel.ToEntity<VanBangChuyenMon>();
            _vanBangChuyenMonService.Add(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToModel<VanBangChuyenMonViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucVanBangChuyenMon)]
        public async Task<ActionResult> Put([FromBody] VanBangChuyenMonViewModel viewModel)
        {
            var entity = await _vanBangChuyenMonService.GetByIdAsync(viewModel.Id);
            if (entity == null)
            {
                return NotFound();
            }
            viewModel.ToEntity(entity);
            await _vanBangChuyenMonService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucVanBangChuyenMon)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucvu = await _vanBangChuyenMonService.GetByIdAsync(id);
            if (chucvu == null)
            {
                return NotFound();
            }
            await _vanBangChuyenMonService.DeleteByIdAsync(id);
            return NoContent();
        }


        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucVanBangChuyenMon)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var entitys = await _vanBangChuyenMonService.GetByIdsAsync(model.Ids);
            if (entitys == null)
            {
                return NotFound();
            }
            if (entitys.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _vanBangChuyenMonService.DeleteAsync(entitys);
            return NoContent();
        }

        [HttpPost("KichHoatTrangThai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucVanBangChuyenMon)]
        public async Task<ActionResult> KichHoatTrangThai(long id)
        {
            var entity = await _vanBangChuyenMonService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _vanBangChuyenMonService.UpdateAsync(entity);
            return NoContent();
        }
        #endregion

        #region GetListLookupItemVo
        //[HttpPost("GetListVanBangChuyenMon")]
        //public async Task<ActionResult<ICollection<LookupItemVo>>> GetListVanBangChuyenMon()
        //{
        //    var lookup = await _vanBangChuyenMonService.GetListVanBangChuyenMon();
        //    return Ok(lookup);
        //}
        [HttpPost("GetListVanBangChuyenMon")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListVanBangChuyenMon([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _vanBangChuyenMonService.GetListVanBangChuyenMon(model);
            return Ok(lookup);
        }
        [HttpPost("GetListVanBangChuyenMonAutoComplete")]
        public ActionResult<ICollection<string>> GetListVanBangChuyenMonAutoComplete([FromBody]DropDownListRequestModel model)
        {
            var lookup = _vanBangChuyenMonService.GetListVanBangChuyenMon(model).Result.Select(cc => cc.DisplayName).ToList();
            return Ok(lookup);
        }
        #endregion

        [HttpPost("ExportVanBangChuyenMon")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucVanBangChuyenMon)]    
        public async Task<ActionResult> ExportVanBangChuyenMon(QueryInfo queryInfo)
        {
            var gridData = await _vanBangChuyenMonService.GetDataForGridAsync(queryInfo, true);
            var vanBangChuyenMonData = gridData.Data.Select(p => (VanBangChuyenMonGridVo)p).ToList();
            var excelData = vanBangChuyenMonData.Map<List<VanBangChuyenMonExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(VanBangChuyenMonExportExcel.Ma), "Mã số"));
            lstValueObject.Add((nameof(VanBangChuyenMonExportExcel.TenVietTat), "Tên viết tắt"));
            lstValueObject.Add((nameof(VanBangChuyenMonExportExcel.Ten), "Tên đầy đủ"));
            lstValueObject.Add((nameof(VanBangChuyenMonExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(VanBangChuyenMonExportExcel.IsDisabled), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Văn bằng chuyên môn");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=VanBangChuyenMon" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}