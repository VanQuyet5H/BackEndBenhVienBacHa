using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Thuoc;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.Thuocs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class ThuocHoacHoatChatController : CaminoBaseController
    {
        private readonly IThuocHoacHoatChatService _thuocHoacHoatChatService;
        private readonly IExcelService _excelService;
        private readonly IDuongDungService _duongDungService;
        private readonly INhomThuocService _nhomThuocService;
        private readonly ILocalizationService _localizationService;

        public ThuocHoacHoatChatController(IThuocHoacHoatChatService thuocHoacHoatChatService
            , IDuongDungService duongDungService
            , IExcelService excelService
            , INhomThuocService nhomThuocService
            , ILocalizationService localizationService)
        {
            _thuocHoacHoatChatService = thuocHoacHoatChatService;
            _excelService = excelService;
            _nhomThuocService = nhomThuocService;
            _duongDungService = duongDungService;
            _localizationService = localizationService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucThuocHoacHoatChat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _thuocHoacHoatChatService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucThuocHoacHoatChat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _thuocHoacHoatChatService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucThuocHoacHoatChat)]
        public async Task<ActionResult<ThuocHoacHoatChatViewModel>> Post
            ([FromBody]ThuocHoacHoatChatViewModel thuocHoacHoatChatViewModel)
        {
            var thuocHoacHoatChat = thuocHoacHoatChatViewModel.ToEntity<ThuocHoacHoatChat>();
            await _thuocHoacHoatChatService.AddAsync(thuocHoacHoatChat);
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucThuocHoacHoatChat)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ThuocHoacHoatChatViewModel>> Get(long id)
        {
            var thuocHoacHoatChat = await _thuocHoacHoatChatService.GetByIdAsync(
                id,
                s => s.Include(k => k.DuongDung)
                            .Include(k => k.NhomThuoc)
                );

            if (thuocHoacHoatChat == null)
            {
                return NotFound();
            }

            return Ok(thuocHoacHoatChat.ToModel<ThuocHoacHoatChatViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucThuocHoacHoatChat)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatThuocHoacHoatChat([FromBody]ThuocHoacHoatChatViewModel thuocHoacHoatChatViewModel)
        {
            var thuocHoacHoatChat = await _thuocHoacHoatChatService.GetByIdAsync(thuocHoacHoatChatViewModel.Id);
            thuocHoacHoatChatViewModel.ToEntity(thuocHoacHoatChat);
            await _thuocHoacHoatChatService.UpdateAsync(thuocHoacHoatChat);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucThuocHoacHoatChat)]
        public async Task<ActionResult> Delete(long id)
        {
            var thuocHoacHoatChat = await _thuocHoacHoatChatService.GetByIdAsync(id);
            if (thuocHoacHoatChat == null)
            {
                return NotFound();
            }

            await _thuocHoacHoatChatService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucThuocHoacHoatChat)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var thuocHoacHoatChats = await _thuocHoacHoatChatService.GetByIdsAsync(model.Ids);
            if (thuocHoacHoatChats == null)
            {
                return NotFound();
            }

            var hoacHoatChats = thuocHoacHoatChats.ToList();
            if (hoacHoatChats.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _thuocHoacHoatChatService.DeleteAsync(hoacHoatChats);
            return NoContent();
        }
        #endregion

        #region GetListLookupItemVo
        [HttpPost("GetListDuongDung")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListDuongDung([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _duongDungService.GetListDuongDung(model);
            return Ok(lookup);
        }

        [HttpPost("GetListNhomThuoc")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhomThuoc([FromBody]LookupQueryInfo model)
        {
            var lookup = await _nhomThuocService.GetListNhomThuoc(model);
            return Ok(lookup);
        }

        [HttpPost("LookupThuocHoacHoatChat")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LookupThuocHoacHoatChat([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _thuocHoacHoatChatService.LookupThuocHoacHoatChat(model);
            return Ok(lookup);
        }
        [HttpPost("ExportThuocHoacHoatChat")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucThuocHoacHoatChat)]
        public async Task<ActionResult> ExportThuocHoacHoatChat(QueryInfo queryInfo)
        {
            var gridData = await _thuocHoacHoatChatService.GetDataForGridAsync(queryInfo, true);
            var thuocHoacHoatChatData = gridData.Data.Select(p => (ThuocHoacHoatChatGridVo)p).ToList();
            var dataExcel = thuocHoacHoatChatData.Map<List<ThuocHoacHoatChatExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(ThuocHoacHoatChatExportExcel.Ma), "Mã"),
                (nameof(ThuocHoacHoatChatExportExcel.Ten), "Tên"),
                (nameof(ThuocHoacHoatChatExportExcel.SttHoatChat), "Số Thứ Tự Hoạt Chất"),
                (nameof(ThuocHoacHoatChatExportExcel.SttThuoc), "Số Thứ Tự Thuốc"),
                (nameof(ThuocHoacHoatChatExportExcel.MaATC), "Mã ATC"),
                (nameof(ThuocHoacHoatChatExportExcel.DuongDung), "Loại Đường Dùng"),
                (nameof(ThuocHoacHoatChatExportExcel.HoiChan), "Hôi Chân"),
                (nameof(ThuocHoacHoatChatExportExcel.TyLeBaoHiemThanhToan), "Tỷ Lệ Bảo Hiểm Thanh Toán"),
                (nameof(ThuocHoacHoatChatExportExcel.CoDieuKienThanhToan), "Có Thể Thanh Toán"),
                (nameof(ThuocHoacHoatChatExportExcel.MoTa), "Mô Tả"),
                (nameof(ThuocHoacHoatChatExportExcel.NhomThuoc), "Nhóm Thuốc"),
                (nameof(ThuocHoacHoatChatExportExcel.BenhVienHang1), "Bệnh Viện Hạng I"),
                (nameof(ThuocHoacHoatChatExportExcel.BenhVienHang2), "Bệnh Viện Hạng II"),
                (nameof(ThuocHoacHoatChatExportExcel.BenhVienHang3), "Bệnh Viện Hạng III"),
                (nameof(ThuocHoacHoatChatExportExcel.BenhVienHang4), "Bệnh Viện Hạng IV")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Thuốc Hoặc Hoạt Chất");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ThuocHoacHoatChat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}