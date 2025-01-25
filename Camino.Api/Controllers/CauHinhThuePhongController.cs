using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.CauHinhThuePhong;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinhThuePhong;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.CauHinhThuePhong;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class CauHinhThuePhongController : CaminoBaseController
    {
        private ICauHinhThuePhongService _cauHinhThuePhongService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public CauHinhThuePhongController(ICauHinhThuePhongService cauHinhThuePhongService
            , ILocalizationService localizationService
            , IExcelService excelService
            )
        {
            _cauHinhThuePhongService = cauHinhThuePhongService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridCauHinhThuePhong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridCauHinhThuePhongAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _cauHinhThuePhongService.GetDataForGridCauHinhThuePhongAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridCauHinhThuePhong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridCauHinhThuePhongAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _cauHinhThuePhongService.GetTotalPageForGridCauHinhThuePhongAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Get data
        [HttpPost("GetListCauHinhThuePhong")]
        public async Task<ActionResult> GetListCauHinhThuePhong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhThuePhongService.GetListCauHinhThuePhong(queryInfo);
            return Ok(lookup);
        }

        #endregion

        #region Xử lý data
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult> Post([FromBody] CauHinhThuePhongViewModel viewModel)
        {
            var cauHinhThuePhong = viewModel.ToEntity<CauHinhThuePhong>();
            await _cauHinhThuePhongService.AddAsync(cauHinhThuePhong);
            return CreatedAtAction(nameof(Get), new { id = cauHinhThuePhong.Id }, cauHinhThuePhong.ToModel<CauHinhThuePhongViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CauHinhThuePhongViewModel>> Get(long id)
        {
            var cauHinhThuePhong = await _cauHinhThuePhongService.GetByIdAsync(id);
            if (cauHinhThuePhong == null)
            {
                return NotFound();
            }
            return Ok(cauHinhThuePhong.ToModel<CauHinhThuePhongViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult> Put([FromBody] CauHinhThuePhongViewModel chucVuViewModel)
        {
            var cauHinhThuePhong = await _cauHinhThuePhongService.GetByIdAsync(chucVuViewModel.Id);
            if (cauHinhThuePhong == null)
            {
                return NotFound();
            }
            chucVuViewModel.ToEntity(cauHinhThuePhong);

            await _cauHinhThuePhongService.UpdateAsync(cauHinhThuePhong);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucvu = await _cauHinhThuePhongService.GetByIdAsync(id);
            if (chucvu == null)
            {
                return NotFound();
            }
            await _cauHinhThuePhongService.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion

        #region export excel
        [HttpPost("ExportDanhSachCauHinhThuePhong")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult> ExportDanhSachCauHinhThuePhong(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;

            var gridData = await _cauHinhThuePhongService.GetDataForGridCauHinhThuePhongAsync(queryInfo);
            var cauHinhThuePhongData = gridData.Data.Select(p => (CauHinhThuePhongGridVo)p).ToList();
            var excelData = cauHinhThuePhongData.Map<List<CauHinhThuePhongExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.LoaiThuePhongPhauThuat), "Loại Phẫu Thuật"));
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.LoaiThuePhongNoiThucHien), "Nơi Thực Hiện"));
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.ThoiGianThueDisplay), "Thời Gian Thuê"));
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.GiaThueDisplay), "Giá Thuê"));
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.PhanTramNgoaiGio), "%Ngoài Giờ HC"));
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.PhanTramLeTet), "%Lễ, Tết"));
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.GiaThuePhatSinhDisplay), "Giá Phát Sinh"));
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.PhanTramPhatSinhNgoaiGio), "%PS Ngoài HC"));
            lstValueObject.Add((nameof(CauHinhThuePhongExportExcel.PhanTramPhatSinhLeTet), "%PS Lễ, Tết"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Cấu hình thuê phòng");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=CauHinhThuePhong" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion
    }
}
