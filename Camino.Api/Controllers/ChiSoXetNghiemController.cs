using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.ChiSoXetNghiems;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChiSoXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ChiSoXetNghiems;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ChiSoXetNghiems;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class ChiSoXetNghiemController : CaminoBaseController
    {
        private readonly IChiSoXetNghiemService _chiSoXetNghiemService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public ChiSoXetNghiemController(IChiSoXetNghiemService chiSoXetNghiemService,ILocalizationService localizationService, IExcelService excelService)
        {
            _chiSoXetNghiemService = chiSoXetNghiemService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        #region GetDataForGrid

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChiSoXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chiSoXetNghiemService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChiSoXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chiSoXetNghiemService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChiSoXetNghiem)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ChiSoXetNghiemViewModel>> Get(long id)
        {
            var result = await _chiSoXetNghiemService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var r = result.ToModel<ChiSoXetNghiemViewModel>();
            return Ok(r);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucChiSoXetNghiem)]
        public async Task<ActionResult<ChiSoXetNghiemViewModel>> Post([FromBody] ChiSoXetNghiemViewModel viewModel)
        {
            var entity = viewModel.ToEntity<ChiSoXetNghiem>();
            await _chiSoXetNghiemService.AddAsync(entity);
            return NoContent();
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucChiSoXetNghiem)]
        public async Task<ActionResult> Put([FromBody] ChiSoXetNghiemViewModel viewModel)
        {
            var entity = await _chiSoXetNghiemService.GetByIdAsync(viewModel.Id);
            if (entity == null)
            {
                return NotFound();
            }
            viewModel.ToEntity(entity);
            await _chiSoXetNghiemService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("KichHoatChiSo")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucChiSoXetNghiem)]
        public async Task<ActionResult> KichHoatChiSo(long id)
        {
            var entity = await _chiSoXetNghiemService.GetByIdAsync(id);
            entity.HieuLuc = entity?.HieuLuc == null ? true : !entity.HieuLuc;
            await _chiSoXetNghiemService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChiSoXetNghiem)]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _chiSoXetNghiemService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            await _chiSoXetNghiemService.DeleteAsync(entity);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChiSoXetNghiem)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var entitys = await _chiSoXetNghiemService.GetByIdsAsync(model.Ids);
            if (entitys == null)
            {
                return NotFound();
            }
            if (entitys.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService.GetResource("Common.WrongLengthMultiDelete"));
            }
            await _chiSoXetNghiemService.DeleteAsync(entitys);
            return NoContent();
        }

        #endregion

        #region GetListLookupItemVo
        [HttpPost("GetListXetNghiem")]
        public ActionResult<ICollection<LookupItemVo>> GetListXetNghiem(DropDownListRequestModel model)
        {
            var listEnum = EnumHelper.GetListEnum<EnumLoaiXetNghiem>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            });
            var searchString = model.Query != null ? model.Query : "";
            var resultList = result.Where(p => Regex.IsMatch(p.DisplayName.ToLower().ConvertToUnSign(), 
                searchString.Trim().ToLower().ConvertToUnSign() ?? "" + ".*[mn]"));
            return Ok(resultList.ToList());
        }
        #endregion

        [HttpPost("ExportChiSoXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucChiSoXetNghiem)]
        public async Task<ActionResult> ExportChiSoXetNghiem(QueryInfo queryInfo)
        {
            var gridData = await _chiSoXetNghiemService.GetDataForGridAsync(queryInfo, true);
            var chiSoXetNghiemData = gridData.Data.Select(p => (ChiSoXetNghiemGridVo)p).ToList();
            var excelData = chiSoXetNghiemData.Map<List<ChiSoXetNghiemExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ChiSoXetNghiemExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(ChiSoXetNghiemExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(ChiSoXetNghiemExportExcel.TenTiengAnh), "Tên tiếng Anh"));
            lstValueObject.Add((nameof(ChiSoXetNghiemExportExcel.ChiSoBinhThuongNam), "Chỉ số xét nghiệm nam"));
            lstValueObject.Add((nameof(ChiSoXetNghiemExportExcel.ChiSoBinhThuongNu), "Chỉ số xét nghiệm nữ"));
            lstValueObject.Add((nameof(ChiSoXetNghiemExportExcel.TenLoaiXetNghiem), "Loại xét nghiệm"));
            lstValueObject.Add((nameof(ChiSoXetNghiemExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(ChiSoXetNghiemExportExcel.HieuLuc), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Chỉ số xét nghiệm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ChiSoXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}