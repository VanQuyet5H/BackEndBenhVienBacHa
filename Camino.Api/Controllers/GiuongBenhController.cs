using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.GiuongBenhs;
using Camino.Api.Models.NhapKhoDuocPhams;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GiuongBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Services.BenhVien.Khoa;
using Camino.Services.ExportImport;
using Camino.Services.GiuongBenhs;
using Camino.Services.Helpers;
using Camino.Services.KhoaPhong;
using Camino.Services.Localization;
using Camino.Services.PhongBenhVien;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class GiuongBenhController : CaminoBaseController
    {
        private readonly IGiuongBenhService _giuongBenhService;
        private readonly ILocalizationService _localizationService;
        private readonly IKhoaService _khoaService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly IPhongBenhVienService _phongBenhVienService;
        private readonly IExcelService _excelService;

        public GiuongBenhController(IGiuongBenhService giuongBenhService, ILocalizationService localizationService
            , IKhoaService khoaService, IKhoaPhongService khoaPhongService, IPhongBenhVienService phongBenhVienService, IExcelService excelService)
        {
            _giuongBenhService = giuongBenhService;
            _localizationService = localizationService;
            _khoaService = khoaService;
            _khoaPhongService = khoaPhongService;
            _phongBenhVienService = phongBenhVienService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucGiuongBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _giuongBenhService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucGiuongBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _giuongBenhService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetListKhoaPhong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoaPhong([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khoaPhongService.GetListKhoaPhongAll(model);
            return Ok(lookup);
        }

        [HttpPost("GetListPhong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListPhong([FromBody]DropDownListRequestModel model)
        {
            var subId = CommonHelper.GetIdFromRequestDropDownList(model);

            var lstPhongBenhVienWithKhoaPhong = await _phongBenhVienService.GetListPhongBenhVienByKhoaPhongId(subId, model);
            var lookup = lstPhongBenhVienWithKhoaPhong.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return Ok(lookup);
        }

        [HttpPost("GetListPhongSharedView")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListPhongSharedView([FromBody]DropDownListRequestModel model)
        {
            var subId = CommonHelper.GetIdFromRequestDropDownList(model);

            if (subId == 0)
            {
                return Ok(new List<KhoaKhamTemplateVo>());
            }

            var lstPhongBenhVienWithKhoaPhong = await _phongBenhVienService.GetListPhongBenhVienByKhoaPhongId(subId, model);
            var lookup = lstPhongBenhVienWithKhoaPhong.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();

            return Ok(lookup);
        }

        #region CRUD

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucGiuongBenh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<GiuongBenhViewModel>> Get(long id)
        {
            var result = await _giuongBenhService.GetByIdAsync(id, u => 
                u.Include(x => x.PhongBenhVien).ThenInclude(x => x.KhoaPhong)
            );
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<GiuongBenhViewModel>();
            return Ok(resultData);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucGiuongBenh)]
        public async Task<ActionResult<GiuongBenhViewModel>> Post([FromBody] GiuongBenhViewModel viewModel)
        {
            //if (viewModel.NhapKhoDuocPhamChiTiets.Count <= 0)
            //    throw new ApiException(_localizationService.GetResource("NhapKhoDuocPhamChiTiet.Required"), (int)HttpStatusCode.BadRequest);

            var entity = viewModel.ToEntity<GiuongBenh>();
            await _giuongBenhService.AddAsync(entity);

            return NoContent();
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucGiuongBenh)]
        public async Task<ActionResult<NhapKhoDuocPhamViewModel>> Put([FromBody] GiuongBenhViewModel viewModel)
        {
            var entity = await _giuongBenhService.GetByIdAsync(viewModel.Id);
            if (entity == null)
            {
                return NotFound();
            }
            if (viewModel.CoHieuLuc!=true && _giuongBenhService.GiuongDangCoBenhNhan(entity.Id)==true)
            {
                 throw new ApiException("Giường này đang có bệnh nhân nằm, không được bỏ hiệu lực.", (int)HttpStatusCode.BadRequest);
            }
            viewModel.ToEntity(entity);
            await _giuongBenhService.UpdateAsync(entity);

            return Ok(viewModel);
        }

        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucGiuongBenh)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucvu = await _giuongBenhService.GetByIdAsync(id, s => s.Include(p => p.HoatDongGiuongBenhs));
            if (chucvu == null)
            {
                return NotFound();
            }
            await _giuongBenhService.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion CRUD

        [HttpPost("ExportGiuongBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucGiuongBenh)]
        public async Task<ActionResult> ExportGiuongBenh(QueryInfo queryInfo)
        {
            var gridData = await _giuongBenhService.GetDataForGridAsync(queryInfo, true);
            var giuongBenhData = gridData.Data.Select(p => (GiuongBenhGridVo)p).ToList();
            var excelData = giuongBenhData.Map<List<GiuongBenhExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(GiuongBenhExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(GiuongBenhExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(GiuongBenhExportExcel.Khoa), "Khoa"));
            lstValueObject.Add((nameof(GiuongBenhExportExcel.Phong), "Phòng"));
            lstValueObject.Add((nameof(GiuongBenhExportExcel.MoTa), "Mô tả"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Giường bệnh");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GiuongBenh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}