using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.CongTyBhtns;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.CongTyBhtns;
using Camino.Services.CongTyBhtns;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class CongTyBhtnController : CaminoBaseController
    {
        private readonly ICongTyBhtnService _congTyBhtn;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public CongTyBhtnController(ICongTyBhtnService congTyBhtn, ILocalizationService localizationService, IExcelService excelService)
        {
            _congTyBhtn = congTyBhtn;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCongTyBhtn)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _congTyBhtn.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCongTyBhtn)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _congTyBhtn.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucCongTyBhtn)]
        public async Task<ActionResult<CongTyBhtnViewModel>> Post
            ([FromBody]CongTyBhtnViewModel congTyBhtnViewModel)
        {
            var congTyBaoHiemTuNhan = congTyBhtnViewModel.ToEntity<CongTyBaoHiemTuNhan>();
            await _congTyBhtn.AddAsync(congTyBaoHiemTuNhan);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCongTyBhtn)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CongTyBhtnViewModel>> Get(long id)
        {
            var congTyBaoHiemTuNhan = await _congTyBhtn.GetByIdAsync(id);
            if (congTyBaoHiemTuNhan == null)
            {
                return NotFound();
            }

            return Ok(congTyBaoHiemTuNhan.ToModel<CongTyBhtnViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCongTyBhtn)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Put([FromBody]CongTyBhtnViewModel congTyBhtnViewModel)
        {
            var congTyBaoHiemTuNhan = await _congTyBhtn.GetByIdAsync(congTyBhtnViewModel.Id);
            congTyBhtnViewModel.ToEntity(congTyBaoHiemTuNhan);
            await _congTyBhtn.UpdateAsync(congTyBaoHiemTuNhan);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCongTyBhtn)]
        public async Task<ActionResult> Delete(long id)
        {
            var congTyBaoHiemTuNhan = await _congTyBhtn.GetByIdAsync(id);
            if (congTyBaoHiemTuNhan == null)
            {
                return NotFound();
            }

            await _congTyBhtn.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCongTyBhtn)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var congTyBaoHiemTuNhans = await _congTyBhtn.GetByIdsAsync(model.Ids);
            if (congTyBaoHiemTuNhans == null)
            {
                return NotFound();
            }

            var congTyBaoHiemTuNhansEnumerableList = congTyBaoHiemTuNhans.ToList();

            if (congTyBaoHiemTuNhansEnumerableList.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _congTyBhtn.DeleteAsync(congTyBaoHiemTuNhansEnumerableList);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportCongTyBhtn")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucCongTyBhtn)]
        public async Task<ActionResult> ExportCongTyBhtn(QueryInfo queryInfo)
        {
            var gridData = await _congTyBhtn.GetDataForGridAsync(queryInfo, true);
            var congTyBaoHiemTuNhanData = gridData.Data.Select(p => (CongTyBhtnGridVo)p).ToList();
            var excelData = congTyBaoHiemTuNhanData.Map<List<CongTyBaoHiemTuNhanExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(CongTyBaoHiemTuNhanExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(CongTyBaoHiemTuNhanExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(CongTyBaoHiemTuNhanExportExcel.Email), "Email"));
            lstValueObject.Add((nameof(CongTyBaoHiemTuNhanExportExcel.SoDienThoai), "SĐT"));
            lstValueObject.Add((nameof(CongTyBaoHiemTuNhanExportExcel.DiaChi), "Địa Chỉ"));
            lstValueObject.Add((nameof(CongTyBaoHiemTuNhanExportExcel.HinhThucBaoHiem), "Hình Thức Bảo Hiểm"));
            lstValueObject.Add((nameof(CongTyBaoHiemTuNhanExportExcel.PhamViBaoHiem), "Phạm Vi Bảo Hiểm"));
            lstValueObject.Add((nameof(CongTyBaoHiemTuNhanExportExcel.GhiChu), "Ghi Chú"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Công Ty Bảo Hiểm Tư Nhân");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=CongTyBaoHiemTuNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetHinhThucBaoHiem")]
        public ActionResult<ICollection<LookupItemVo>> GetHinhThucBaoHiem()
        {
            var lookup = _congTyBhtn.GetHinhThucBaoHiem();
            return Ok(lookup);
        }

        [HttpPost("GetPhamViBaoHiem")]
        public ActionResult<ICollection<LookupItemVo>> GetPhamViBaoHiem()
        {
            var lookup = _congTyBhtn.GetPhamViBaoHiem();
            return Ok(lookup);
        }
    }
}
