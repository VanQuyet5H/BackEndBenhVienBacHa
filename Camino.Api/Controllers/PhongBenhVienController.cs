using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.PhongBenhVien;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.KhoaPhong;
using Camino.Services.Localization;
using Camino.Services.PhongBenhVien;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.PhongBenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class PhongBenhVienController : CaminoBaseController
    {
        private readonly IPhongBenhVienService _phongBenhVienService;
        private readonly IExcelService _excelService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly ILocalizationService _localizationService;

        public PhongBenhVienController(IPhongBenhVienService phongBenhVienService, IKhoaPhongService khoaPhongService, IExcelService excelService, ILocalizationService localizationService)
        {
            _phongBenhVienService = phongBenhVienService;
            _khoaPhongService = khoaPhongService;
            _excelService = excelService;
            _localizationService = localizationService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoaPhongPhongKham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phongBenhVienService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoaPhongPhongKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phongBenhVienService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region GetListLookupItemVo       

        [HttpPost("GetListKhoaPhong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoaPhong([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khoaPhongService.GetListKhoaPhong(model);
            return Ok(lookup);
        }

        [HttpPost("GetListKhoa")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoa([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khoaPhongService.GetListKhoa(model);
            return Ok(lookup);
        }

        [HttpPost("GetListPhong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListPhong([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _phongBenhVienService.GetListPhongBenhVienByKhoaSreach(model, model.Id);
            return Ok(lookup);
        }

        [HttpPost("GetListPhongBenhVienByCurrentUser")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListPhongBenhVienByCurrentUser()
        {
            var lookup = await _phongBenhVienService.GetListPhongBenhVienByCurrentUser();
            return Ok(lookup);
        }

        [HttpPost("GetListPhongTatCa")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListPhongTatCa([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _phongBenhVienService.GetListPhongTatCa(model);
            return Ok(lookup);
        }
        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucKhoaPhongPhongKham)]
        public async Task<ActionResult<PhongBenhVienViewModel>> Post
            ([FromBody]PhongBenhVienViewModel phongBenhVienViewModel)
        {
            var phongBenhVien = phongBenhVienViewModel.ToEntity<PhongBenhVien>();
            await _phongBenhVienService.AddAsync(phongBenhVien);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoaPhongPhongKham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PhongBenhVienViewModel>> Get(long id)
        {
            var phongBenhVien = await _phongBenhVienService.GetByIdAsync(
                id,
                s => s.Include(k => k.KhoaPhong)
                );

            if (phongBenhVien == null)
            {
                return NotFound();
            }

            return Ok(phongBenhVien.ToModel<PhongBenhVienViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucKhoaPhongPhongKham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatKhoaPhong([FromBody]PhongBenhVienViewModel phongBenhVienViewModel)
        {
            var phongBenhVien = await _phongBenhVienService.GetByIdAsync(phongBenhVienViewModel.Id);
            phongBenhVienViewModel.ToEntity(phongBenhVien);
            await _phongBenhVienService.UpdateAsync(phongBenhVien);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucKhoaPhongPhongKham)]
        public async Task<ActionResult> Delete(long id)
        {
            var phongBenhVien = await _phongBenhVienService.GetByIdAsync(id);
            if (phongBenhVien == null)
            {
                return NotFound();
            }

            await _phongBenhVienService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucKhoaPhongPhongKham)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var phongBenhViens = await _phongBenhVienService.GetByIdsAsync(model.Ids);

            if (phongBenhViens == null)
            {
                return NotFound();
            }

            var phongBenhVienList = phongBenhViens.ToList();

            if (phongBenhVienList.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }

            await _phongBenhVienService.DeleteAsync(phongBenhVienList);
            return NoContent();
        }

        #endregion

        [HttpPost("KichHoatPhongBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucKhoaPhongPhongKham)]
        public async Task<ActionResult> KichHoatPhongBenhVien(long id)
        {
            var entity = await _phongBenhVienService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _phongBenhVienService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportKhoaPhongPhongKham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucKhoaPhongPhongKham)]
        public async Task<ActionResult> ExportKhoaPhongPhongKham(QueryInfo queryInfo)
        {
            var gridData = await _phongBenhVienService.GetDataForGridAsync(queryInfo, true);
            var khoaPhongPhongKhamData = gridData.Data.Select(p => (PhongBenhVienGridVo)p).ToList();
            var dataExcel = khoaPhongPhongKhamData.Map<List<KhoaPhongPhongKhamExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(KhoaPhongPhongKhamExportExcel.Ma), "Mã"),
                (nameof(KhoaPhongPhongKhamExportExcel.Ten), "Tên Phòng Khám"),
                (nameof(KhoaPhongPhongKhamExportExcel.IsDisabled), "Trạng Thái"),
                (nameof(KhoaPhongPhongKhamExportExcel.TenKhoaPhong), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Khoa Phòng Phòng Khám");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KhoaPhongPhongKham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetListPhongBenhVienByKhoa")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListPhongBenhVienByKhoa([FromBody] DropDownListRequestModel model)
        {
            var lookup = await _phongBenhVienService.GetListPhongBenhVienByKhoa(model);
            return Ok(lookup);
        }

        #region Chuyển phòng nội bộ

        [HttpPost("GetKhoaNoiBos")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoaNoiBos([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khoaPhongService.GetListKhoa(model);
            return Ok(lookup);
        }

        [HttpPost("GetPhongNoiBos")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetPhongNoiBos([FromBody]DropDownListRequestModel model, long khoaPhongId)
        {
            var lookup = await _phongBenhVienService.GetListPhongBenhVienByKhoaSreach(model, khoaPhongId);
            return Ok(lookup);
        }

        [HttpPost("GetPhongNgoaiViens")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetPhongNgoaiViens([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _phongBenhVienService.GetListPhongNgoaiVienByHopDongKhamSreach(model);
            return Ok(lookup);
        }

        [HttpGet("GetKhoaPhongNgoaiVien")]
        public ActionResult<LookupItemVo> GetKhoaPhongNgoaiVien()
        {
            var lookup = _phongBenhVienService.GetKhoaPhongNgoaiVien();
            return Ok(lookup);
        }

        [HttpPost("GetPhongBenhViensByKhoaPhongId")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetPhongBenhViensByKhoaPhongId([FromBody] DropDownListRequestModel model, long khoaPhongId)
        {
            var lookup = await _phongBenhVienService.GetPhongBenhViensByKhoaPhongId(model, khoaPhongId);
            return Ok(lookup);
        }

        [HttpGet("GetKhoaByPhong")]
        public async Task<ActionResult<LookupItemVo>> GetKhoaByPhong(long phongId)
        {
            var khoaPhongs = await _phongBenhVienService.GetKhoaByPhong(phongId);
            return Ok(khoaPhongs);
        }

        [HttpGet("GetTenKhoaByPhong")]
        public async Task<ActionResult<LookupItemVo>> GetTenKhoaByPhong(long phongId)
        {
            var khoaPhongs = await _phongBenhVienService.GetTenKhoaByPhong(phongId);
            return Ok(khoaPhongs);
        }
        #endregion

    }
}
