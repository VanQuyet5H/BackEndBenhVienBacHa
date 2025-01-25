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
using Camino.Services.DinhMucDuocPhamTonKho;
using Camino.Api.Models.DinhMucDuocPhamTonKho;
using Camino.Core.Domain.Entities.DinhMucDuocPhamTonKhos;
using Microsoft.EntityFrameworkCore;
using Camino.Api.Models.Error;
using Camino.Services.ExportImport;
using Camino.Core.Domain.ValueObject.DinhMucDuocPhamTonKho;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public class DinhMucDuocPhamTonKhoController : CaminoBaseController
    {
        private readonly IDinhMucDuocPhamTonKhoService _dinhMucDuocPhamTonKhoService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public DinhMucDuocPhamTonKhoController(IDinhMucDuocPhamTonKhoService dinhMucDuocPhamTonKhoService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _dinhMucDuocPhamTonKhoService = dinhMucDuocPhamTonKhoService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDinhMucDuocPhamTonKho)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dinhMucDuocPhamTonKhoService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDinhMucDuocPhamTonKho)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dinhMucDuocPhamTonKhoService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTenDuocPham")]
        public async Task<ActionResult> GetTenDuocPham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dinhMucDuocPhamTonKhoService.GetListDuocPham(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetTenKhoDuocPham")]
        public async Task<ActionResult> GetTenKhoDuocPham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dinhMucDuocPhamTonKhoService.GetTenKhoDuocPhamAsync(queryInfo);
            return Ok(lookup);
        }
        #region Get/Add/Delete/Update
        //Add
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDinhMucDuocPhamTonKho)]
        public async Task<ActionResult> Post([FromBody] DinhMucDuocPhamTonKhoViewModel dinhMucDuocPhamTonKhoVm)
        {
            var duocPhamTk = dinhMucDuocPhamTonKhoVm.ToEntity<DinhMucDuocPhamTonKho>();
            var check = await _dinhMucDuocPhamTonKhoService.CheckHoatChatExist(dinhMucDuocPhamTonKhoVm.DuocPhamBenhVienId.GetValueOrDefault());
            if (check == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucDuocPhamTonKho.HoatChatDeleted"));
            }
            var checkHieuLuc = await _dinhMucDuocPhamTonKhoService.CheckHieuLuc(dinhMucDuocPhamTonKhoVm.DuocPhamBenhVienId.GetValueOrDefault());
            if (checkHieuLuc == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucDuocPhamTonKho.HoatChatExpired"));
            }

            duocPhamTk.KhoId = dinhMucDuocPhamTonKhoVm.KhoDuocPhamId.GetValueOrDefault();
            await _dinhMucDuocPhamTonKhoService.AddAsync(duocPhamTk);
            return CreatedAtAction(nameof(Get), new { id = duocPhamTk.Id }, duocPhamTk.ToModel<DinhMucDuocPhamTonKhoViewModel>());
        }
        //Get
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDinhMucDuocPhamTonKho)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DinhMucDuocPhamTonKhoViewModel>> Get(long id)
        {

            var duocPhamTK = await _dinhMucDuocPhamTonKhoService.GetByIdAsync(id, s => s.Include(u => u.KhoDuocPham).Include(u => u.DuocPhamBenhVien)
                                                                                                                   .ThenInclude(u => u.DuocPham));
            if (duocPhamTK == null)
            {
                return NotFound();
            }

            var dmDpTkVm = duocPhamTK.ToModel<DinhMucDuocPhamTonKhoViewModel>();
            dmDpTkVm.KhoDuocPhamId = duocPhamTK.KhoId;
            return Ok(dmDpTkVm);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDinhMucDuocPhamTonKho)]
        public async Task<ActionResult> Put([FromBody] DinhMucDuocPhamTonKhoViewModel dinhMucDuocPhamTonKhoVM)
        {
            var duocPhamTK = await _dinhMucDuocPhamTonKhoService.GetByIdAsync(dinhMucDuocPhamTonKhoVM.Id);
            if (duocPhamTK == null)
            {
                return NotFound();
            }
            dinhMucDuocPhamTonKhoVM.ToEntity(duocPhamTK);
            var check = await _dinhMucDuocPhamTonKhoService.CheckHoatChatExist(dinhMucDuocPhamTonKhoVM.DuocPhamBenhVienId.Value);
            if (check == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucDuocPhamTonKho.HoatChatDeleted"));
            }
            var checkHieuLuc = await _dinhMucDuocPhamTonKhoService.CheckHieuLuc(dinhMucDuocPhamTonKhoVM.DuocPhamBenhVienId.Value);
            if (checkHieuLuc == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucDuocPhamTonKho.HoatChatExpired"));
            }
            duocPhamTK.KhoId = dinhMucDuocPhamTonKhoVM.KhoDuocPhamId.GetValueOrDefault();
            await _dinhMucDuocPhamTonKhoService.UpdateAsync(duocPhamTK);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDinhMucDuocPhamTonKho)]
        public async Task<ActionResult> Delete(long id)
        {
            var duocPhamTK = await _dinhMucDuocPhamTonKhoService.GetByIdAsync(id);
            if (duocPhamTK == null)
            {
                return NotFound();
            }
            await _dinhMucDuocPhamTonKhoService.DeleteByIdAsync(id);
            return NoContent();
        }
        //Delete all selected items
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDinhMucDuocPhamTonKho)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var duocPhamTKs = await _dinhMucDuocPhamTonKhoService.GetByIdsAsync(model.Ids);
            if (duocPhamTKs == null)
            {
                return NotFound();
            }
            if (duocPhamTKs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _dinhMucDuocPhamTonKhoService.DeleteAsync(duocPhamTKs);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportDinhMucDuocPhamTonKho")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDinhMucDuocPhamTonKho)]
        public async Task<ActionResult> ExportDinhMucDuocPhamTonKho(QueryInfo queryInfo)
        {
            var gridData = await _dinhMucDuocPhamTonKhoService.GetDataForGridAsync(queryInfo, true);
            var dinhMucDuocPhamTonKhoData = gridData.Data.Select(p => (DinhMucDuocPhamTonKhoGridVo)p).ToList();
            var excelData = dinhMucDuocPhamTonKhoData.Map<List<DinhMucDuocPhamTonKhoExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DinhMucDuocPhamTonKhoExportExcel.TenDuocPham), "Tên dược phẩm"));
            lstValueObject.Add((nameof(DinhMucDuocPhamTonKhoExportExcel.TenKhoDuocPham), "Tên kho dược phẩm"));
            lstValueObject.Add((nameof(DinhMucDuocPhamTonKhoExportExcel.TonToiThieu), "Tồn tối thiểu"));
            lstValueObject.Add((nameof(DinhMucDuocPhamTonKhoExportExcel.TonToiDa), "Tồn tối đa"));
            lstValueObject.Add((nameof(DinhMucDuocPhamTonKhoExportExcel.SoNgayTruocKhiHetHan), "Số ngày trước khi hết hạn"));
            lstValueObject.Add((nameof(DinhMucDuocPhamTonKhoExportExcel.MoTa), "Mô tả"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Định mức dược phẩm tồn kho");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DinhMucDuocPhamTonKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}