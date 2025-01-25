using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.General;
using Camino.Api.Models.KhoDuocPhamViTri;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhoDuocPhamViTris;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhamViTris;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoDuocPhamViTri;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{

    public class KhoDuocPhamViTriController : CaminoBaseController
    {
        private readonly IKhoduocPhamViTriService _khoduocPhamViTriService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public KhoDuocPhamViTriController(IKhoduocPhamViTriService khoduocPhamViTriService, ILocalizationService localizationService, IExcelService excelService)
        {
            _khoduocPhamViTriService = khoduocPhamViTriService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoDuocPhamViTri)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoduocPhamViTriService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoDuocPhamViTri)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoduocPhamViTriService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("KichHoatKhoDuocPhamViTri")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucKhoDuocPhamViTri)]
        public async Task<ActionResult> KichHoatKhoDuocPhamViTri(long id)
        {
            var entity = await _khoduocPhamViTriService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _khoduocPhamViTriService.UpdateAsync(entity);
            return NoContent();
        }


        [HttpPost("GetListTenKhoDuocPham")]
        public async Task<ActionResult<ICollection<ChucDanhItemVo>>> GetListTenKhoDuocPham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khoduocPhamViTriService.GetListTenKhoDuocPham(queryInfo);
            return Ok(lookup);
        }

        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhMucKhoDuocPhamViTri)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KhoDuocPhamViTriViewModel>> Get(long id)
        {
            var result = await _khoduocPhamViTriService.GetByIdAsync(id, s => s.Include(h => h.KhoDuocPham));
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<KhoDuocPhamViTriViewModel>();
            return Ok(resultData);
        }

        [HttpPost]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.DanhMucKhoDuocPhamViTri)]
        public async Task<ActionResult<KhoDuocPhamViTriViewModel>> Post([FromBody] KhoDuocPhamViTriViewModel viewModel)
        {
            var entity = viewModel.ToEntity<KhoViTri>();
            entity.KhoId = viewModel.KhoDuocPhamId;
            await _khoduocPhamViTriService.AddAsync(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToModel<KhoDuocPhamViTriViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhMucKhoDuocPhamViTri)]
        public async Task<ActionResult> Put([FromBody] KhoDuocPhamViTriViewModel viewModel)
        {
            var entity = await _khoduocPhamViTriService.GetByIdAsync(viewModel.Id, s => s.Include(h => h.KhoDuocPham));
            if (entity == null)
                return NotFound();
            viewModel.ToEntity(entity);
            entity.KhoId = viewModel.KhoDuocPhamId;
            await _khoduocPhamViTriService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhMucKhoDuocPhamViTri)]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _khoduocPhamViTriService.GetByIdAsync(id, s => s.Include(h => h.KhoDuocPham));
            if (entity == null)
                return NotFound();
            await _khoduocPhamViTriService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhMucKhoDuocPhamViTri)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var entitys = await _khoduocPhamViTriService.GetByIdsAsync(model.Ids, s => s.Include(h => h.KhoDuocPham));
            if (entitys == null)
            {
                return NotFound();
            }
            if (entitys.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService.GetResource("Common.WrongLengthMultiDelete"));
            }
            await _khoduocPhamViTriService.DeleteAsync(entitys);
            return NoContent();
        }

        #endregion



        [HttpPost("GetListViTriKhoDuocPham")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListViTriKhoDuocPham(DropDownListRequestModel model)
        {
            var lookup = await _khoduocPhamViTriService.GetListViTriKhoDuocPham(model);
            return Ok(lookup);
        }

        [HttpGet("GetListDataViTri")]
        public async Task<ActionResult<LookupItemViTriVo>> GetListDataViTri()
        {
            var lookup = await _khoduocPhamViTriService.GetListDataViTri();
            lookup.Insert(0, new LookupItemViTriVo { DisplayName = "Chưa xếp vị trí", KeyId = 0 });
            return Ok(lookup);

        }

        [HttpPost("ExportViTriKhoDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucKhoDuocPhamViTri)]
        public async Task<ActionResult> ExportViTriKhoDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _khoduocPhamViTriService.GetDataForGridAsync(queryInfo, true);
            var viTriKhoDuocPhamData = gridData.Data.Select(p => (KhoDuocPhamViTriGridVo)p).ToList();
            var excelData = viTriKhoDuocPhamData.Map<List<KhoDuocPhamViTriExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(KhoDuocPhamViTriExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(KhoDuocPhamViTriExportExcel.MoTa), "Mô tả"));
            lstValueObject.Add((nameof(KhoDuocPhamViTriExportExcel.IsDisabled), "Trạng thái"));
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Vị trí kho dược phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ViTriKhoDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}