using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.QuocGia;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.QuocGias;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuocGia;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.QuocGias;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuocGiaController : CaminoBaseController
    {
        private readonly IQuocGiaService _quocGiaService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public QuocGiaController(IQuocGiaService quocGiaService, ILocalizationService localizationService, IExcelService excelService)
        {
            _quocGiaService = quocGiaService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucQuocGia)]
        [HttpPost("GetDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _quocGiaService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucQuocGia)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _quocGiaService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetListChauLuc")]
        public Task<List<ChauLucVo>> GetListChauLuc([FromBody] DropDownListRequestModel queryInfo)
        {
            var lookup = _quocGiaService.GetListChauLuc(queryInfo);
            return Task.FromResult(lookup);
        }

        [HttpPost("KichHoatTrangThai")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucQuocGia)]
        public async Task<ActionResult> KichHoatQuocGia(long id)
        {
            var entity = await _quocGiaService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;

            await _quocGiaService.UpdateAsync(entity);

            return NoContent();
        }

        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucQuocGia)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<QuocGiaViewModel>> Get(long id)
        {
            var quocGia = await _quocGiaService.GetByIdAsync(id);

            if(quocGia == null)
            {
                return NotFound();
            }

            QuocGiaViewModel quocGiaVM = quocGia.ToModel<QuocGiaViewModel>();
            quocGiaVM.ChauLucText = _quocGiaService.GetTenChauLuc(quocGia.ChauLuc);

            return Ok(quocGiaVM);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucQuocGia)]
        public async Task<ActionResult<QuocGiaViewModel>> Post ([FromBody] QuocGiaViewModel quocGiaViewModel)
        {
            var quocGia = quocGiaViewModel.ToEntity<QuocGia>();

            await _quocGiaService.AddAsync(quocGia);

            var quocGiaId = await _quocGiaService.GetByIdAsync(quocGia.Id);

            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = quocGia.Id },
                quocGiaId.ToModel<QuocGiaViewModel>()
            );
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucQuocGia)]
        public async Task<ActionResult> Put([FromBody] QuocGiaViewModel quocGiaViewModel)
        {
            var quocGia = await _quocGiaService.GetByIdAsync(quocGiaViewModel.Id);
            if (quocGia == null)
            {
                return NotFound();
            }

            quocGiaViewModel.ToEntity(quocGia);

            await _quocGiaService.UpdateAsync(quocGia);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucQuocGia)]
        public async Task<ActionResult> Delete(long id)
        {
            var quocGia = await _quocGiaService.GetQuocGiaRelationshipsById(id);

            if (quocGia == null)
            {
                return NotFound();
            }

            if (quocGia.NhaSanXuatTheoQuocGias || quocGia.QuocTichBenhNhans || quocGia.YeuCauTiepNhans || quocGia.DanTocs)
            {
                throw new ArgumentException(_localizationService.GetResource("QuocGia.IsAvailable.Error"));
            }
            
            await _quocGiaService.DeleteByIdAsync(id);
            
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucQuocGia)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var quocGias = await _quocGiaService.GetQuocGiaRelationshipsByIds(model.Ids);

            if (quocGias == null)
            {
                return NotFound();
            }

            if(quocGias.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService.GetResource("Common.WrongLengthMultiDelete"));
            }

            foreach (var item in quocGias)
            {
                if (item.NhaSanXuatTheoQuocGias || item.QuocTichBenhNhans || item.YeuCauTiepNhans || item.DanTocs)
                {
                    throw new ArgumentException(_localizationService.GetResource("QuocGia.IsAvailable.Error") + " (" + item.Ten + ")");
                }
            }

            IEnumerable<QuocGia> list = await _quocGiaService.GetByIdsAsync(model.Ids);
            await _quocGiaService.DeleteAsync(list);
            
            return NoContent();
        }
        #endregion

        [HttpPost("ExportQuocGia")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucQuocGia)]
        public async Task<ActionResult> ExportQuocGia(QueryInfo queryInfo)
        {
            var gridData = await _quocGiaService.GetDataForGridAsync(queryInfo, true);
            var quocGiaData = gridData.Data.Select(p => (QuocGiaGridVo)p).ToList();
            var excelData = quocGiaData.Map<List<QuocGiaExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(QuocGiaExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(QuocGiaExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(QuocGiaExportExcel.TenVietTat), "Tên viết tắt"));
            lstValueObject.Add((nameof(QuocGiaExportExcel.QuocTich), "Quốc tịch"));
            lstValueObject.Add((nameof(QuocGiaExportExcel.MaDienThoaiQuocTe), "Mã điện thoại quốc tế"));
            lstValueObject.Add((nameof(QuocGiaExportExcel.ThuDo), "Thủ đô"));
            lstValueObject.Add((nameof(QuocGiaExportExcel.ChauLuc), "Châu lục"));
            lstValueObject.Add((nameof(QuocGiaExportExcel.IsDisabled), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Quốc gia");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=QuocGia" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
