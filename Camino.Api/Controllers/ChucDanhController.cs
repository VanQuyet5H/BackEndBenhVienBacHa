using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Services.ChucDanh;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Api.Models.ChucDanh;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Core.Domain.Entities.ChucDanhs;
using Camino.Services.Localization;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{

    public class ChucDanhController : CaminoBaseController
    {
        private readonly IChucDanhService _chucDanhService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public ChucDanhController(IChucDanhService chucDanhService, ILocalizationService localizationService, IExcelService excelService)
        {
            _chucDanhService = chucDanhService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chucDanhService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chucDanhService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("KichHoatChucDanh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult> KichHoatChucDanh(long id)
        {
            var entity = await _chucDanhService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _chucDanhService.UpdateAsync(entity);
            return NoContent();
        }


        [HttpPost("GetListNhomChucDanh")]
        public async Task<ActionResult<ICollection<ChucDanhItemVo>>> GetListNhomChucDanh([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _chucDanhService.GetListNhomChucDanh(queryInfo);
            return Ok(lookup);
        }
        #region CRUD
        /// <summary>
        ///     Add can ho
        /// </summary>
        /// <param name="chucDanhViewModel"></param>
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
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult<ChucDanhViewModel>> Post([FromBody] ChucDanhViewModel chucDanhViewModel)
        {
            var user = chucDanhViewModel.ToEntity<ChucDanh>();
            _chucDanhService.Add(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<ChucDanhViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChucDanh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ChucDanhViewModel>> Get(long id)
        {
            var result = await _chucDanhService.GetByIdAsync(id, s => s.Include(k => k.NhomChucDanh));
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<ChucDanhViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult> Put([FromBody] ChucDanhViewModel chucDanhViewModel)
        {
            var chucdanh = await _chucDanhService.GetByIdAsync(chucDanhViewModel.Id);
            if (chucdanh == null)
            {
                return NotFound();
            }
            chucDanhViewModel.ToEntity(chucdanh);
            await _chucDanhService.UpdateAsync(chucdanh);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucdanh = await _chucDanhService.GetByIdAsync(id);
            if (chucdanh == null)
            {
                return NotFound();
            }
            await _chucDanhService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var chucDanhs = await _chucDanhService.GetByIdsAsync(model.Ids);
            if (chucDanhs == null)
            {
                return NotFound();
            }
            if (chucDanhs.Count()!= model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _chucDanhService.DeleteAsync(chucDanhs);
            return NoContent();
        }
        #endregion
        #region GetListLookupItemVo
        //[HttpPost("GetListChucDanh")]
        //public async Task<ActionResult<ICollection<LookupItemVo>>> GetListChucDanh()
        //{
        //    var lookup = await _chucDanhService.GetListChucDanh();
        //    return Ok(lookup);
        //}
        [HttpPost("GetListChucDanh")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListChucDanh([FromBody]DropDownListRequestModel model)
       {
            var lookup = await _chucDanhService.GetListChucDanh(model);
            return Ok(lookup);
        }
        #endregion

        [HttpPost("ExportChucDanh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult> ExportChucDanh(QueryInfo queryInfo)
        {
            var gridData = await _chucDanhService.GetDataForGridAsync(queryInfo, true);
            var chucDanhData = gridData.Data.Select(p => (ChucDanhGridVo)p).ToList();
            var excelData = chucDanhData.Map<List<ChucDanhExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ChucDanhExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(ChucDanhExportExcel.Ten), "Tên đầy đủ"));
            lstValueObject.Add((nameof(ChucDanhExportExcel.TenNhomChucDanh), "Tên nhóm chức danh"));
            lstValueObject.Add((nameof(ChucDanhExportExcel.IsDisabled), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Chức danh");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ChucDanh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
