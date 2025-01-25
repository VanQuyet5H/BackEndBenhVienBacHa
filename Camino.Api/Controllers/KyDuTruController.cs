using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.KyDuTru;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KyDuTru;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KyDuTru;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class KyDuTruController : CaminoBaseController
    {
        private readonly IKyDuTruService _kyDuTruService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public KyDuTruController(IKyDuTruService kyDuTruService, ILocalizationService localizationService, IExcelService excelService)
        {
            _kyDuTruService = kyDuTruService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KyDuTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _kyDuTruService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KyDuTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _kyDuTruService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KyDuTru)]
        public async Task<ActionResult<KyDuTruViewModel>> Get(long id)
        {
            var kyDuTru = await _kyDuTruService.GetByIdAsync(id);

            if (kyDuTru == null)
            {
                return NotFound();
            }

            KyDuTruViewModel kyDuTruVM = kyDuTru.ToModel<KyDuTruViewModel>();

            return Ok(kyDuTruVM);
        }

        [HttpPost]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.KyDuTru)]
        public async Task<ActionResult<KyDuTruViewModel>> Post([FromBody] KyDuTruViewModel kyDuTruViewModel)
        {
            var kyDuTru = kyDuTruViewModel.ToEntity<KyDuTruMuaDuocPhamVatTu>();

            await _kyDuTruService.AddAsync(kyDuTru);

            var kyDuTruId = await _kyDuTruService.GetByIdAsync(kyDuTru.Id);

            var actionName = nameof(Get);

            return CreatedAtAction(
                actionName,
                new { id = kyDuTru.Id },
                kyDuTruId.ToModel<KyDuTruViewModel>()
            );
        }

        [HttpPut]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KyDuTru)]
        public async Task<ActionResult> Put([FromBody] KyDuTruViewModel kyDuTruViewModel)
        {
            var kyDuTru = await _kyDuTruService.GetByIdAsync(kyDuTruViewModel.Id);
            if (kyDuTru == null)
            {
                return NotFound();
            }

            kyDuTruViewModel.ToEntity(kyDuTru);

            await _kyDuTruService.UpdateAsync(kyDuTru);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.KyDuTru)]
        public async Task<ActionResult> Delete(long id)
        {
            var kyDuTru = await _kyDuTruService.GetByIdAsync(id);

            if (kyDuTru == null)
            {
                return NotFound();
            }

            await _kyDuTruService.DeleteByIdAsync(id);

            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.KyDuTru)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var kyDuTrus = await _kyDuTruService.GetByIdsAsync(model.Ids);

            if (kyDuTrus == null)
            {
                return NotFound();
            }

            if (kyDuTrus.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService.GetResource("Common.WrongLengthMultiDelete"));
            }

            IEnumerable<KyDuTruMuaDuocPhamVatTu> list = await _kyDuTruService.GetByIdsAsync(model.Ids);
            await _kyDuTruService.DeleteAsync(list);

            return NoContent();
        }
        #endregion

        [HttpPost("KichHoatTrangThai")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KyDuTru)]
        public async Task<ActionResult> KichHoatTrangThai(long id)
        {
            var entity = await _kyDuTruService.GetByIdAsync(id);
            entity.HieuLuc = !entity.HieuLuc;

            await _kyDuTruService.UpdateAsync(entity);

            return NoContent();
        }

        [HttpGet("IsDaDuocSuDung")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KyDuTru)]
        public async Task<ActionResult> IsDaDuocSuDung(long kyDuTruId)
        {
            return Ok(await _kyDuTruService.IsDaDuocSuDung(kyDuTruId));
        }

        [HttpGet("IsDaDuocSuDungChoDuTruMuaDuocPham")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KyDuTru)]
        public async Task<ActionResult> IsDaDuocSuDungChoDuTruMuaDuocPham(long kyDuTruId)
        {
            return Ok(await _kyDuTruService.IsDaDuocSuDungChoDuTruMuaDuocPham(kyDuTruId));
        }

        [HttpGet("IsDaDuocSuDungChoDuTruMuaVatTu")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KyDuTru)]
        public async Task<ActionResult> IsDaDuocSuDungChoDuTruMuaVatTu(long kyDuTruId)
        {
            return Ok(await _kyDuTruService.IsDaDuocSuDungChoDuTruMuaVatTu(kyDuTruId));
        }

        [HttpPost("ExportKyDuTru")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.KyDuTru)]
        public async Task<ActionResult> ExportKyDuTru(QueryInfo queryInfo)
        {
            var gridData = await _kyDuTruService.GetDataForGridAsync(queryInfo, true);
            var kyDuTruData = gridData.Data.Select(p => (KyDuTruGridVo)p).ToList();
            var excelData = kyDuTruData.Map<List<KyDuTruExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(KyDuTruExportExcel.TuNgayDisplay), "Từ ngày"));
            lstValueObject.Add((nameof(KyDuTruExportExcel.DenNgayDisplay), "Đến ngày"));
            lstValueObject.Add((nameof(KyDuTruExportExcel.NhanVienTaoDisplay), "Nhân viên tạo"));
            lstValueObject.Add((nameof(KyDuTruExportExcel.ApDung), "Áp dụng"));
            lstValueObject.Add((nameof(KyDuTruExportExcel.HieuLucDisplay), "Hiệu lực"));
            lstValueObject.Add((nameof(KyDuTruExportExcel.NgayTaoDisplay), "Ngày tạo"));
            lstValueObject.Add((nameof(KyDuTruExportExcel.NgayBatDauLapDisplay), "Ngày bắt đầu lập"));
            lstValueObject.Add((nameof(KyDuTruExportExcel.NgayKetThucLapDisplay), "Ngày kết thúc lập"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Kỳ dự trù");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KyDuTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
