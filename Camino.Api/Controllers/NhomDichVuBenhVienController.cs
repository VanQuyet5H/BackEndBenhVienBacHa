using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Api.Models.NhomDichVuBenhVien;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhomDichVuBenhVien;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhomDichVuBenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NhomDichVuBenhVien;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class NhomDichVuBenhVienController : CaminoBaseController
    {
        private readonly INhomDichVuBenhVienService _nhomDichVuBenhVienService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public NhomDichVuBenhVienController(INhomDichVuBenhVienService nhomDichVuBenhVienService, ILocalizationService localizationService, IExcelService excelService)
        {
            _nhomDichVuBenhVienService = nhomDichVuBenhVienService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        #region GetDataForGrid
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpPost("GetDataForGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomDichVuBenhVien)]
        //public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
        //    ([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = await _nhomDichVuBenhVienService.GetDataForGridAsync(queryInfo);
        //    return Ok(gridData);
        //}

        //[HttpPost("GetTotalPageForGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomDichVuBenhVien)]
        //public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
        //    ([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = await _nhomDichVuBenhVienService.GetTotalPageForGridAsync(queryInfo);
        //    return Ok(gridData);
        //}


        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucNhomDichVuBenhVien)]
        public async Task<ActionResult<NhomDichVuBenhVienViewModel>> Post
            ([FromBody]NhomDichVuBenhVienViewModel nhomDichVuBenhVienViewModel)
        {
            var nhomDichVuBenhVien = nhomDichVuBenhVienViewModel.ToEntity<NhomDichVuBenhVien>();
            await _nhomDichVuBenhVienService.AddAsync(nhomDichVuBenhVien);
            // var nhomDichVuBenhVienAdded = await _nhomDichVuBenhVienService.GetByIdAsync(nhomDichVuBenhVien.Id);
            var actionName = nameof(Post);

            return CreatedAtAction(
                actionName,
                new { id = nhomDichVuBenhVien.Id });
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomDichVuBenhVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NhomDichVuBenhVienViewModel>> Get(long id)
        {
            var nhomDichVuBenhVien = await _nhomDichVuBenhVienService.GetByIdAsync(id, w => w.Include(q => q.NhomDichVuBenhVienCha));

            if (nhomDichVuBenhVien == null)
            {
                return NotFound();
            }

            return Ok(nhomDichVuBenhVien.ToModel<NhomDichVuBenhVienViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucNhomDichVuBenhVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatNhomDichVuBenhVien([FromBody]NhomDichVuBenhVienViewModel nhomDichVuBenhVienViewModel)
        {
            var nhomDichVuBenhVien = await _nhomDichVuBenhVienService.GetByIdAsync(nhomDichVuBenhVienViewModel.Id);

            nhomDichVuBenhVienViewModel.ToEntity(nhomDichVuBenhVien);
            await _nhomDichVuBenhVienService.UpdateAsync(nhomDichVuBenhVien);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhomDichVuBenhVien)]
        public async Task<ActionResult> Delete(long id)
        {
            var nhomDichVuBenhVien = await _nhomDichVuBenhVienService.GetByIdAsync(id, s => s.Include(x => x.NhomDichVuBenhViens));

            if (nhomDichVuBenhVien == null)
            {
                return NotFound();
            }

            if (nhomDichVuBenhVien.IsDefault)
            {
                throw new ApiException(_localizationService.GetResource("NhomDichVu.IsDefault.Delete"), (int)HttpStatusCode.BadRequest);
            }

            await _nhomDichVuBenhVienService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNhomDichVuBenhVien)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var nhomDichVuBenhViens = await _nhomDichVuBenhVienService.GetByIdsAsync(model.Ids);

            if (nhomDichVuBenhViens == null)
            {
                return NotFound();
            }

            var listNhomDichVuBenhViens = nhomDichVuBenhViens.ToList();

            foreach (var nhomDichVuBenhVien in listNhomDichVuBenhViens)
            {
                if (nhomDichVuBenhVien.IsDefault)
                {
                    throw new ApiException(_localizationService.GetResource("NhomDichVus.IsDefault.Delete"), (int)HttpStatusCode.BadRequest);
                }
            }

            if (listNhomDichVuBenhViens.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }

            await _nhomDichVuBenhVienService.DeleteAsync(listNhomDichVuBenhViens);
            return NoContent();
        }

        [HttpPost("GetNhomDichVuBenhVien")]
        public async Task<ActionResult> GetNhomDichVuBenhVien([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _nhomDichVuBenhVienService.GetDichVuKhamBenh(queryInfo);
            return Ok(lookup);
        }
     
        #endregion

        //[HttpPost("ExportNhomDichVuBenhVien")]
        //[ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucNhomDichVuBenhVien)]
        //public async Task<ActionResult> ExportNhomDichVuBenhVien(QueryInfo queryInfo)
        //{
        //    var gridData = await _nhomDichVuBenhVienService.GetDataForGridAsync(queryInfo, true);
        //    var nhomDichVuBenhVienData = gridData.Data.Select(p => (NhomDichVuBenhVienGridVo)p).ToList();
        //    var excelData = nhomDichVuBenhVienData.Map<List<NhomDichVuBenhVienExportExcel>>();

        //    var lstValueObject = new List<(string, string)>();
        //    lstValueObject.Add((nameof(NhomDichVuBenhVienExportExcel.Ma), "Mã"));
        //    lstValueObject.Add((nameof(NhomDichVuBenhVienExportExcel.NhomDichVuBenhVienCha), "Tên nhóm dịch vụ bệnh viện cha"));
        //    lstValueObject.Add((nameof(NhomDichVuBenhVienExportExcel.Ten), "Tên"));
        //    lstValueObject.Add((nameof(NhomDichVuBenhVienExportExcel.MoTa), "Mô tả"));

        //    var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nhóm dịch vụ bệnh viện");

        //    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhomDichVuBenhVien" + DateTime.Now.Year + ".xls");
        //    Response.ContentType = "application/vnd.ms-excel";

        //    return new FileContentResult(bytes, "application/vnd.ms-excel");
        //}

        #region Treeview Nhóm dịch vụ bệnh viện
        [HttpPost("NhomDichVuBenhVienTreeViews")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomDichVuBenhVien)]
        public List<NhomDichVuBenhVienTreeViewGridVo> NhomDichVuBenhVienTreeViewChas([FromBody]QueryInfo queryInfo)
        {
            var gridData = _nhomDichVuBenhVienService.NhomDichVuBenhVienTreeViewChas(queryInfo);
            return gridData.ToList();
        }

        [HttpPost("NhomDichVuBenhVienTreeViewCons")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ChiSoXetNghiem)]
        public List<NhomDichVuBenhVienTreeViewGridVo> NhomDichVuBenhVienTreeViewCons([FromBody]QueryInfo queryInfo)
        {
            var gridData = _nhomDichVuBenhVienService.NhomDichVuBenhVienTreeViewCons(queryInfo);
            return gridData.ToList();
        }
        #endregion
    }
}
