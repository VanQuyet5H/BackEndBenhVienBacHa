using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Services.Localization;
using System.Linq;
using System;
using Camino.Services.HocViHocHam;
using Camino.Api.Models.HocViHocHam;
using Camino.Core.Domain.Entities.HocViHocHams;
using Camino.Services.ExportImport;
using Camino.Core.Domain.ValueObject.HocViHocHam;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public class HocViHocHamController : CaminoBaseController
    {
        private readonly IHocViHocHamService _hocViHocHamService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public HocViHocHamController(IHocViHocHamService hocViHocHamService,ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _hocViHocHamService = hocViHocHamService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHocViHocHam)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hocViHocHamService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHocViHocHam)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _hocViHocHamService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucHocViHocHam)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<HocViHocHamViewModel>> Get(long id)
        {
            var result = await _hocViHocHamService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var r = result.ToModel<HocViHocHamViewModel>();
            return Ok(r);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucHocViHocHam)]
        public async Task<ActionResult<HocViHocHamViewModel>> Post([FromBody] HocViHocHamViewModel viewModel)
        {
            var entity = viewModel.ToEntity<HocViHocHam>();
            await _hocViHocHamService.AddAsync(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToModel<HocViHocHamViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucHocViHocHam)]
        public async Task<ActionResult> Put([FromBody] HocViHocHamViewModel viewModel)
        {
            var entity = await _hocViHocHamService.GetByIdAsync(viewModel.Id);
            if (entity == null)
            {
                return NotFound();
            }
            viewModel.ToEntity(entity);
            await _hocViHocHamService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucHocViHocHam)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucvu = await _hocViHocHamService.GetByIdAsync(id);
            if (chucvu == null)
            {
                return NotFound();
            }
            await _hocViHocHamService.DeleteByIdAsync(id);
            return NoContent();
        }


        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucHocViHocHam)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var entitys = await _hocViHocHamService.GetByIdsAsync(model.Ids);
            if (entitys == null)
            {
                return NotFound();
            }
            if (entitys.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _hocViHocHamService.DeleteAsync(entitys);
            return NoContent();
        }

        #endregion

        #region GetListLookupItemVo
        //[HttpPost("GetListHocViHocHam")]
        //public async Task<ActionResult<ICollection<LookupItemVo>>> GetListHocViHocHam()
        //{
        //    var lookup = await _hocViHocHamService.GetListHocViHocHam();
        //    return Ok(lookup);
        //}
        [HttpPost("GetListHocViHocHam")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListHocViHocHam([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _hocViHocHamService.GetListHocViHocHam(model);
            return Ok(lookup);
        }
        #endregion

        [HttpPost("ExportHocViHocHam")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucHocViHocHam)]
        public async Task<ActionResult> ExportHocViHocHam(QueryInfo queryInfo)
        {
            var gridData = await _hocViHocHamService.GetDataForGridAsync(queryInfo, true);
            var hocViHocHamData = gridData.Data.Select(p => (HocViHocHamGripVo)p).ToList();
            var excelData = hocViHocHamData.Map<List<HocViHocHamExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(HocViHocHamExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(HocViHocHamExportExcel.Ten), "Tên học vị"));
            lstValueObject.Add((nameof(HocViHocHamExportExcel.MoTa), "Mô tả"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Học vị học hàm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=HocViHocHam" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}