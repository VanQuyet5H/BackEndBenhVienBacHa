using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DinhMucVatTuTonKhos;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DinhMucVatTuTonKhos;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DinhMucVatTuTonKho;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.DinhMucVatTuTonKhos;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class DinhMucVatTuTonKhoController : CaminoBaseController
    {
        private readonly IDinhMucVatTuTonKhoService _dinhMucVatTuTonKhoService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public DinhMucVatTuTonKhoController(IDinhMucVatTuTonKhoService dinhMucVatTuTonKhoService, ILocalizationService localizationService, IExcelService excelService)
        {
            _dinhMucVatTuTonKhoService = dinhMucVatTuTonKhoService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDinhMucVatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dinhMucVatTuTonKhoService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDinhMucVatTuTonKho)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dinhMucVatTuTonKhoService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetListVatTu")]
        public async Task<ActionResult> GetListVatTu([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dinhMucVatTuTonKhoService.GetListVatTu(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetListKhoAsync")]
        public async Task<ActionResult> GetListKhoAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dinhMucVatTuTonKhoService.GetListKhoAsync(queryInfo);
            return Ok(lookup);
        }

        #region Get/Add/Delete/Update
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDinhMucVatTuTonKho)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Post([FromBody] DinhMucVatTuTonKhoViewModel dinhMucVatTuTonKhoVm)
        {
            var vatTuTk = dinhMucVatTuTonKhoVm.ToEntity<DinhMucVatTuTonKho>();
            var check = await _dinhMucVatTuTonKhoService.CheckVatTuExist(dinhMucVatTuTonKhoVm.VatTuBenhVienId.GetValueOrDefault());
            if (check == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucVatTuTonKho.HoatChatDeleted"));
            }
            var checkHieuLuc = await _dinhMucVatTuTonKhoService.CheckHieuLuc(dinhMucVatTuTonKhoVm.VatTuBenhVienId.GetValueOrDefault());
            if (checkHieuLuc == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucVatTuTonKho.HoatChatExpired"));
            }

            await _dinhMucVatTuTonKhoService.AddAsync(vatTuTk);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDinhMucVatTuTonKho)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DinhMucVatTuTonKhoViewModel>> Get(long id)
        {

            var vatTuTk = await _dinhMucVatTuTonKhoService.GetByIdAsync(id, s => s.Include(u => u.VatTuBenhVien)
                                                                                        .ThenInclude(u => u.VatTus).Include(u => u.Kho));
            if (vatTuTk == null)
            {
                return NotFound();
            }

            var dmVtTkVm = vatTuTk.ToModel<DinhMucVatTuTonKhoViewModel>();
            return Ok(dmVtTkVm);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDinhMucVatTuTonKho)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Put([FromBody] DinhMucVatTuTonKhoViewModel dinhMucVatTuTonKhoVm)
        {
            var vatTuTk = await _dinhMucVatTuTonKhoService.GetByIdAsync(dinhMucVatTuTonKhoVm.Id);
            if (vatTuTk == null)
            {
                return NotFound();
            }
            dinhMucVatTuTonKhoVm.ToEntity(vatTuTk);
            var check = await _dinhMucVatTuTonKhoService.CheckVatTuExist(dinhMucVatTuTonKhoVm.VatTuBenhVienId.GetValueOrDefault());
            if (check == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucVatTuTonKho.HoatChatDeleted"));
            }
            var checkHieuLuc = await _dinhMucVatTuTonKhoService.CheckHieuLuc(dinhMucVatTuTonKhoVm.VatTuBenhVienId.GetValueOrDefault());
            if (checkHieuLuc == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucVatTuTonKho.HoatChatExpired"));
            }
            await _dinhMucVatTuTonKhoService.UpdateAsync(vatTuTk);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDinhMucVatTuTonKho)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Delete(long id)
        {
            var vatTuTk = await _dinhMucVatTuTonKhoService.GetByIdAsync(id);
            if (vatTuTk == null)
            {
                return NotFound();
            }
            await _dinhMucVatTuTonKhoService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDinhMucVatTuTonKho)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var vatTuTKs = await _dinhMucVatTuTonKhoService.GetByIdsAsync(model.Ids);
            if (vatTuTKs == null)
            {
                return NotFound();
            }

            var dinhMucVatTuTonKhos = vatTuTKs as DinhMucVatTuTonKho[] ?? vatTuTKs.ToArray();

            if (dinhMucVatTuTonKhos.Length != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }

            await _dinhMucVatTuTonKhoService.DeleteAsync(dinhMucVatTuTonKhos);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportDinhMucVatTuTonKho")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDinhMucVatTuTonKho)]
        public async Task<ActionResult> ExportDinhMucVatTuTonKho(QueryInfo queryInfo)
        {
            var gridData = await _dinhMucVatTuTonKhoService.GetDataForGridAsync(queryInfo, true);
            var dinhMucVatTuTonKhoData = gridData.Data.Select(p => (DinhMucVatTuTonKhoGridVo)p).ToList();
            var excelData = dinhMucVatTuTonKhoData.Map<List<DinhMucVatTuTonKhoExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DinhMucVatTuTonKhoExportExcel.TenVt), "Tên vật tư"));
            lstValueObject.Add((nameof(DinhMucVatTuTonKhoExportExcel.Kho), "Tên kho"));
            lstValueObject.Add((nameof(DinhMucVatTuTonKhoExportExcel.TonToiThieu), "Tồn tối thiểu"));
            lstValueObject.Add((nameof(DinhMucVatTuTonKhoExportExcel.TonToiDa), "Tồn tối đa"));
            lstValueObject.Add((nameof(DinhMucVatTuTonKhoExportExcel.SoNgayTruocKhiHetHan), "Số ngày trước khi hết hạn"));
            lstValueObject.Add((nameof(DinhMucVatTuTonKhoExportExcel.MoTa), "Mô tả"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Định mức vật tư tồn kho");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DinhMucVatTuTonKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
