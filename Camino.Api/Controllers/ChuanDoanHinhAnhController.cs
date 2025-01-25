using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.ChuanDoanHinhAnhViewModelCategory;
using Camino.Api.Models.General;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChuanDoanHinhAnhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.ChuanDoanHinhAnhs;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.ChuanDoanHinhAnh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class ChuanDoanHinhAnhController : CaminoBaseController
    {
        private readonly IChuanDoanHinhAnhService _chuanDoanHinhAnhService;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;

        public ChuanDoanHinhAnhController(
            IChuanDoanHinhAnhService chuanDoanHinhAnhService,
            IExcelService excelService,
            ILocalizationService localizationService)
        {
            _excelService = excelService;
            _chuanDoanHinhAnhService = chuanDoanHinhAnhService;
            _localizationService = localizationService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuanDoanHinhAnh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chuanDoanHinhAnhService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuanDoanHinhAnh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _chuanDoanHinhAnhService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucChuanDoanHinhAnh)]
        public async Task<ActionResult<ChuanDoanHinhAnhViewModel>> Post
            ([FromBody]ChuanDoanHinhAnhViewModel chuanDoanHinhAnhViewModel)
        {
            var chuanDoanHinhAnh = chuanDoanHinhAnhViewModel.ToEntity<ChuanDoanHinhAnh>();
            await _chuanDoanHinhAnhService.AddAsync(chuanDoanHinhAnh);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChuanDoanHinhAnh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ChuanDoanHinhAnhViewModel>> Get(long id)
        {
            var chuanDoanHinhAnh = await _chuanDoanHinhAnhService.GetByIdAsync(id);

            if (chuanDoanHinhAnh == null)
            {
                return NotFound();
            }

            return Ok(chuanDoanHinhAnh.ToModel<ChuanDoanHinhAnhViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucChuanDoanHinhAnh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatChuanDoanHinhAnh([FromBody]ChuanDoanHinhAnhViewModel chuanDoanHinhAnhViewModel)
        {
            var chuanDoanHinhAnh = await _chuanDoanHinhAnhService.GetByIdAsync(chuanDoanHinhAnhViewModel.Id);
            chuanDoanHinhAnhViewModel.ToEntity(chuanDoanHinhAnh);
            await _chuanDoanHinhAnhService.UpdateAsync(chuanDoanHinhAnh);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChuanDoanHinhAnh)]
        public async Task<ActionResult> Delete(long id)
        {
            var chuanDoanHinhAnh = await _chuanDoanHinhAnhService.GetByIdAsync(id);
            if (chuanDoanHinhAnh == null)
            {
                return NotFound();
            }

            await _chuanDoanHinhAnhService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChuanDoanHinhAnh)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var chuanDoanHinhAnhs = await _chuanDoanHinhAnhService.GetByIdsAsync(model.Ids);
            if (chuanDoanHinhAnhs == null)
            {
                return NotFound();
            }

            var listChuanDoanHinhAnhs = chuanDoanHinhAnhs.ToList();
            if (listChuanDoanHinhAnhs.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _chuanDoanHinhAnhService.DeleteAsync(listChuanDoanHinhAnhs);
            return NoContent();
        }
        #endregion

        #region GetListVo
        [HttpPost("GetListLoaiChuanDoanHinhAnh")]
        public ActionResult<ICollection<Enums.EnumLoaiChuanDoanHinhAnh>> GetListLoaiChuanDoanHinhAnh([FromBody]LookupQueryInfo model)
        {
            var listEnum = _chuanDoanHinhAnhService.GetListLoaiChuanDoanHinhAnh(model);
            return Ok(listEnum);
        }

        [HttpPost("ExportChuanDoanHinhAnh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucChuanDoanHinhAnh)]
        public async Task<ActionResult> ExportChuanDoanHinhAnh(QueryInfo queryInfo)
        {
            var gridData = await _chuanDoanHinhAnhService.GetDataForGridAsync(queryInfo, true);
            var chanDoanHinhAnhData = gridData.Data.Select(p => (ChuanDoanHinhAnhGridVo)p).ToList();
            var dataExcel = chanDoanHinhAnhData.Map<List<ChanDoanHinhAnhExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ChanDoanHinhAnhExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(ChanDoanHinhAnhExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(ChanDoanHinhAnhExportExcel.TenTiengAnh), "Tên Tiếng Anh"));
            lstValueObject.Add((nameof(ChanDoanHinhAnhExportExcel.LoaiChuanDoanHinhAnhDisplay), "Loại Chẩn đoán Hình Ảnh"));
            lstValueObject.Add((nameof(ChanDoanHinhAnhExportExcel.MoTa), "Mô Tả"));
            lstValueObject.Add((nameof(ChanDoanHinhAnhExportExcel.HieuLuc), "Hiệu Lực"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Chẩn Đoán Hình Ảnh");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=QuanHeThanNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
