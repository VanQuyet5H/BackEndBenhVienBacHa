using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.General;
using Camino.Api.Models.MauVaChePham;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.MauVaChePham;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.MauVaChePham;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MauVaChePhamController : CaminoBaseController
    {
        private readonly IMauVaChePhamService _mauVaChePhamService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        public MauVaChePhamController(IMauVaChePhamService mauVaChePhamService, IExcelService excelService)
        {
            _mauVaChePhamService = mauVaChePhamService;
            _excelService = excelService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucMauVaChePham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _mauVaChePhamService.GetDataForGridAsync(queryInfo,false);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucMauVaChePham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _mauVaChePhamService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region CRUD
        /// <summary>
        ///     Add can ho
        /// </summary>
        /// <param name="lydokhambenhViewModel"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucMauVaChePham)]
        public async Task<ActionResult<MauVaChePhamViewModel>> Post([FromBody] MauVaChePhamViewModel mauVaChePhamViewModel)
        {
            var mauvachepham = mauVaChePhamViewModel.ToEntity<MauVaChePham>();
            _mauVaChePhamService.Add(mauvachepham);
            return CreatedAtAction(nameof(Get), new { id = mauvachepham.Id }, mauvachepham.ToModel<MauVaChePhamViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucMauVaChePham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<MauVaChePhamViewModel>> Get(long id)
        {
            var result = await _mauVaChePhamService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<MauVaChePhamViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucMauVaChePham)]
        public async Task<ActionResult> Put([FromBody] MauVaChePhamViewModel mauVaChePhamViewModel)
        {
            var mauvachepham = await _mauVaChePhamService.GetByIdAsync(mauVaChePhamViewModel.Id);
            if (mauvachepham == null)
            {
                return NotFound();
            }
            mauVaChePhamViewModel.ToEntity(mauvachepham);
            await _mauVaChePhamService.UpdateAsync(mauvachepham);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucMauVaChePham)]
        public async Task<ActionResult> Delete(long id)
        {
            var mauvachepham = await _mauVaChePhamService.GetByIdAsync(id);
            if (mauvachepham == null)
            {
                return NotFound();
            }

            await _mauVaChePhamService.DeleteByIdAsync(id);
            return NoContent();
        }
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucMauVaChePham)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var mauvachephams = await _mauVaChePhamService.GetByIdsAsync(model.Ids);
            if (mauvachephams == null)
            {
                return NotFound();
            }
            if (mauvachephams.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _mauVaChePhamService.DeleteAsync(mauvachephams);
            return NoContent();
        }
        #endregion

        #region getListPhanLoaiMau Enums

        [HttpPost("GetListPhanLoaiMau")]
        public ActionResult<ICollection<Enums.PhanLoaiMau>> GetListPhanLoaiMau([FromBody]LookupQueryInfo model)
        {
            var listEnum = _mauVaChePhamService.GetListPhanLoaiMau(model);
            return Ok(listEnum);
        }

        #endregion
        #region   // export excel

        [HttpPost("ExportMauVaChePham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucMauVaChePham)]
        public async Task<ActionResult> ExportExportExportMauVaChePham(QueryInfo queryInfo)
        {
            var gridData = await _mauVaChePhamService.GetDataForGridAsync(queryInfo, true);
            var lsMauVaChePham = gridData.Data.Select(p => (MauVaChePhamGridVo)p).ToList();
            var dataExcel = lsMauVaChePham.Map<List<ExportMauVaChePhamExportExcel >> ();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ExportMauVaChePhamExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(ExportMauVaChePhamExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(ExportMauVaChePhamExportExcel.TheTichs), "Thể Tích"));
            lstValueObject.Add((nameof(ExportMauVaChePhamExportExcel.GiaTriToiDas), "Giá Trị Tối Đa"));
        

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Máu Và Chế Phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=MauVaChePham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}