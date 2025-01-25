using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.General;
using Camino.Api.Models.NhaSanXuat;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhaSanXuats;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhaSanXuat;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NhaSanXuat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class NhaSanXuatController : CaminoBaseController
    {
        private readonly INhaSanXuatService _nhaSanXuatService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public NhaSanXuatController(INhaSanXuatService nhaSanXuatService, IExcelService excelService)
        {
            _nhaSanXuatService = nhaSanXuatService;
            _excelService = excelService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhaSanXuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhaSanXuatService.GetDataForGridAsync(queryInfo,false);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhaSanXuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhaSanXuatService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region CRUD
        /// <summary>
        ///     Add can ho
        /// </summary>
        /// <param name="NhaSanXuatViewModel"></param>
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
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNhaSanXuat)]
        public async Task<ActionResult<NhaSanXuatViewModel>> Post([FromBody] NhaSanXuatViewModel nhaSanXuatViewModel)
        {
            var nhasanxuat = nhaSanXuatViewModel.ToEntity<NhaSanXuat>();
                  await _nhaSanXuatService.AddAsync(nhasanxuat);
            return CreatedAtAction(nameof(Get), new { id = nhasanxuat.Id }, nhasanxuat.ToModel<NhaSanXuatViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhaSanXuat)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NhaSanXuatViewModel>> Get(long id)
        {
            var result = await _nhaSanXuatService.GetByIdAsync(id, s => s.Include(k => k.NhaSanXuatTheoQuocGias).ThenInclude(h=>h.QuocGia));
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<NhaSanXuatViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNhaSanXuat)]
        public async Task<ActionResult> Put([FromBody] NhaSanXuatViewModel nhaSanXuatViewModel)
        {
                var nhasanxuat = await _nhaSanXuatService.GetByIdAsync(nhaSanXuatViewModel.Id, s => s.Include(k => k.NhaSanXuatTheoQuocGias));
                if (nhasanxuat == null)
                {
                    return NotFound();
                }
                nhaSanXuatViewModel.ToEntity(nhasanxuat);
                await _nhaSanXuatService.UpdateAsync(nhasanxuat);
                return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhaSanXuat)]
        public async Task<ActionResult> Delete(long id)
        {
            var nhasanxuat = await _nhaSanXuatService.GetByIdAsync(id, s => s.Include(k => k.NhaSanXuatTheoQuocGias));
            if (nhasanxuat == null)
            {
                return NotFound();
            }

            await _nhaSanXuatService.DeleteByIdAsync(id);
            return NoContent();
        }
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhaSanXuat)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var nhasanxuats = await _nhaSanXuatService.GetByIdsAsync(model.Ids, s => s.Include(k => k.NhaSanXuatTheoQuocGias));
            if (nhasanxuats == null)
            {
                return NotFound();
            }
            if (nhasanxuats.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _nhaSanXuatService.DeleteAsync(nhasanxuats);
            return NoContent();
        }
        #endregion
        #region getListTenQuocGia
        [HttpPost("GetListTenQuocGia")]
        public async Task<ActionResult<ICollection<ChucDanhItemVo>>> GetListTenQuocGia(DropDownListRequestModel model)
        {
            var lookup = await _nhaSanXuatService.GetListTenQuocGia(model);
            return Ok(lookup);
        }
        #endregion
        #region   // export excel

        [HttpPost("ExportNhaSanXuat")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucNhaSanXuat)]
        public async Task<ActionResult> ExportNhaSanXuat(QueryInfo queryInfo)
        {
            var gridData = await _nhaSanXuatService.GetDataForGridAsync(queryInfo, true);
            var lsNhaSanXuat = gridData.Data.Select(p => (NhaSanXuatGridVo)p).ToList();
            var dataExcel = lsNhaSanXuat.Map<List<ExportNhaSanXuatExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ExportNhaSanXuatExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(ExportNhaSanXuatExcel.Ten), "Tên"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Nhà Sản Xuất");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhaSanXuat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}