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
using Camino.Services.Thuocs;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain.Entities.Thuocs;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Api.Models.Error;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public class ADRController : CaminoBaseController
    {
        private readonly IADRService _aDRService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public ADRController(IADRService aDRService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _aDRService = aDRService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucAdrPhanUngCoHaiCuaThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _aDRService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucAdrPhanUngCoHaiCuaThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _aDRService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetMaThuocHoaChat")]
        public async Task<ActionResult> GetMaThuocHoaChat([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _aDRService.GetListHoatChatAsync(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetChuYKhiChiDinh")]
        public ActionResult<ICollection<LookupItemVo>> GetChuYKhiChiDinh()
        {
            var lookup = _aDRService.GetChuYKhiChiDinhDescription();
            return Ok(lookup);
        }

        [HttpPost("GetTuongTac")]
        public ActionResult<ICollection<LookupItemVo>> GetTuongTac()
        {
            var lookup = _aDRService.GetChuYKhiChiDinhDescription();
            return Ok(lookup);
        }
        #region Get/Add/Delete/Update
        //Add
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucAdrPhanUngCoHaiCuaThuoc)]
        public async Task<ActionResult> Post([FromBody] ADRViewModel aDRViewModel)
        {
            var adr = aDRViewModel.ToEntity<ADR>();
            var check1 = await _aDRService.CheckHoatChatDeleted(aDRViewModel.ThuocHoacHoatChat1Id);
            if (check1 == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucDuocPhamTonKho.HoatChatDeleted"));
            }

            var check2 = await _aDRService.CheckHoatChatDeleted(aDRViewModel.ThuocHoacHoatChat2Id);
            if (check2 == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucDuocPhamTonKho.HoatChatDeleted"));
            }
            await _aDRService.AddAsync(adr);
            return CreatedAtAction(nameof(Get), new { id = adr.Id }, adr.ToModel<ADRViewModel>());
        }
        //Get
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucAdrPhanUngCoHaiCuaThuoc)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ADRViewModel>> Get(long id)
        {
            var adr = await _aDRService.GetByIdAsync(id, s => s.Include(u => u.ThuocHoacHoatChat1).Include(u => u.ThuocHoacHoatChat2));
            if (adr == null)
            {
                return NotFound();
            }
            return Ok(adr.ToModel<ADRViewModel>());
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucAdrPhanUngCoHaiCuaThuoc)]
        public async Task<ActionResult> Put([FromBody] ADRViewModel aDRViewModel)
        {
            var adr = await _aDRService.GetByIdAsync(aDRViewModel.Id);
            if (adr == null)
            {
                return NotFound();
            }
            aDRViewModel.ToEntity(adr);

            var check1 = await _aDRService.CheckHoatChatDeleted(aDRViewModel.ThuocHoacHoatChat1Id);
            if (check1 == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucDuocPhamTonKho.HoatChatDeleted"));
            }

            var check2 = await _aDRService.CheckHoatChatDeleted(aDRViewModel.ThuocHoacHoatChat2Id);
            if (check2 == false)
            {
                throw new ApiException(_localizationService.GetResource("DinhMucDuocPhamTonKho.HoatChatDeleted"));
            }
            await _aDRService.UpdateAsync(adr);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucAdrPhanUngCoHaiCuaThuoc)]
        public async Task<ActionResult> Delete(long id)
        {
            var adr = await _aDRService.GetByIdAsync(id);
            if (adr == null)
            {
                return NotFound();
            }
            await _aDRService.DeleteByIdAsync(id);
            return NoContent();
        }
        //Delete all selected items
        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucAdrPhanUngCoHaiCuaThuoc)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var adrs = await _aDRService.GetByIdsAsync(model.Ids);
            if (adrs == null)
            {
                return NotFound();
            }
            if (adrs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _aDRService.DeleteAsync(adrs);
            return NoContent();
        }
        #endregion

        [HttpPost("ExportADR")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucAdrPhanUngCoHaiCuaThuoc)]
        public async Task<ActionResult> ExportADR(QueryInfo queryInfo)
        {
            var gridData = await _aDRService.GetDataForGridAsync(queryInfo, true);
            var adrData = gridData.Data.Select(p => (ADRGridVo)p).ToList();
            var excelData = adrData.Map<List<ADRExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ADRExportExcel.TenThuocHoacHoatChat1), "Tên hoạt chất 1"));
            lstValueObject.Add((nameof(ADRExportExcel.TenThuocHoacHoatChat2), "Tên hoạt chất 2"));
            lstValueObject.Add((nameof(ADRExportExcel.MucDoChuYKhiChiDinhDisplay), "Mức độ chú ý khi chỉ định"));
            lstValueObject.Add((nameof(ADRExportExcel.MucDoTuongTacDisplay), "Mức độ tương tác"));
            lstValueObject.Add((nameof(ADRExportExcel.DuocDongHocDisplay), "Dược động học"));
            lstValueObject.Add((nameof(ADRExportExcel.DuocLucHocDisplay), "Dược lực học"));
            lstValueObject.Add((nameof(ADRExportExcel.ThuocThucAnDisplay), "Thuốc thức ăn"));
            lstValueObject.Add((nameof(ADRExportExcel.QuyTacDisplay), "Quy tắc"));
            lstValueObject.Add((nameof(ADRExportExcel.TuongTacHauQua), "Tương tác hậu quả"));
            lstValueObject.Add((nameof(ADRExportExcel.CachXuLy), "Cách xử lý"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "ADR");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ADR" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}