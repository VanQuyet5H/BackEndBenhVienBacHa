using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.ThietBiXetNghiems;
using Camino.Core.Caching;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ThietBiXetNghiems;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.ThietBiXetNghiems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class ThietBiXetNghiemController : CaminoBaseController
    {
        private readonly IThietBiXetNghiemService _tbXetNghiem;
        private readonly IExcelService _excelService;
        private readonly ILocalizationService _localizationService;
        private readonly IStaticCacheManager _cacheManager;

        public ThietBiXetNghiemController(IThietBiXetNghiemService tbXetNghiem, IExcelService excelService, ILocalizationService localizationService, IStaticCacheManager cacheManager)
        {
            _tbXetNghiem = tbXetNghiem;
            _excelService = excelService;
            _localizationService = localizationService;
            _cacheManager = cacheManager;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThietBiXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tbXetNghiem.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThietBiXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _tbXetNghiem.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetNhomXetNghiem")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNhomXetNghiem([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _tbXetNghiem.GetNhomXetNghiem(model);
            return Ok(lookup);
        }

        [HttpPost("GetNhomThietBi")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNhomThietBi([FromBody]DropDownListRequestModel model)
        {
            var nhomXnId = CommonHelper.GetIdFromRequestDropDownList(model);

            if (nhomXnId == 0)
            {
                return Ok(new List<NhomThietBiTemplateVo>());
            }

            var lstPhongBenhVienWithKhoaPhong = await _tbXetNghiem.GetListNhomThietBi(nhomXnId, model);
            var lookup = lstPhongBenhVienWithKhoaPhong.Select(item => new NhomThietBiTemplateVo
            {
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma,
                NhaSanXuat = item.NhaSanXuat
            }).ToList();

            return Ok(lookup);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThietBiXetNghiem)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ThietBiXetNghiemViewModel>> Get(long id)
        {
            var tbXn = await _tbXetNghiem.GetByIdAsync(id, w => w.Include(q => q.MauMayXetNghiem).ThenInclude(q => q.NhomDichVuBenhVien));
            if (tbXn == null)
            {
                return NotFound();
            }

            return Ok(new ThietBiXetNghiemViewModel
            {
                Id = tbXn.Id,
                Ten = tbXn.Ten,
                Ma = tbXn.Ma,
                Ncc = tbXn.NhaCungCap,
                HieuLuc = tbXn.HieuLuc,
                NhomThietBiId = tbXn.MauMayXetNghiemID,
                NhomThietBiDisplay = tbXn.MauMayXetNghiem.Ten,
                NhomXetNghiemId = tbXn.MauMayXetNghiem.NhomDichVuBenhVienId.GetValueOrDefault(),
                NhomXetNghiemDisplay = tbXn.MauMayXetNghiem?.NhomDichVuBenhVien?.Ten
            });
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.ThietBiXetNghiem)]
        public async Task<ActionResult<ThietBiXetNghiemViewModel>> Post
            ([FromBody]ThietBiXetNghiemViewModel tbXn)
        {
            var isValidateMa = await _tbXetNghiem.IsMaExists(
                tbXn.NhomThietBiId, tbXn.Ma, tbXn.NhomXetNghiemId,
                tbXn.Id, tbXn.IsCopy);
            var isValidateTen = await _tbXetNghiem.IsTenExists(
                tbXn.NhomThietBiId, tbXn.Ten, tbXn.NhomXetNghiemId,
                tbXn.Id, tbXn.IsCopy);

            if (isValidateMa)
            {
                var errors = new List<ValidationError>();
                errors.Add(new ValidationError("NhomXetNghiemId", _localizationService.GetResource("ThietBi.Ma.Exists")));
                errors.Add(new ValidationError("NhomThietBiId", _localizationService.GetResource("ThietBi.Ma.Exists")));
                errors.Add(new ValidationError("Ma", _localizationService.GetResource("ThietBi.Ma.Exists")));
                throw new ApiException("Validation Failed", 500, errors);
            }

            if (isValidateTen)
            {
                var errors = new List<ValidationError>();
                errors.Add(new ValidationError("NhomXetNghiemId", _localizationService.GetResource("ThietBi.Ten.Exists")));
                errors.Add(new ValidationError("NhomThietBiId", _localizationService.GetResource("ThietBi.Ten.Exists")));
                errors.Add(new ValidationError("Ten", _localizationService.GetResource("ThietBi.Ten.Exists")));
                throw new ApiException("Validation Failed", 500, errors);
            }

            var tbXnEntity = new MayXetNghiem
            {
                Id = 0,
                Ten = tbXn.Ten,
                Ma = tbXn.Ma,
                MauMayXetNghiemID = tbXn.NhomThietBiId,
                NhaCungCap = tbXn.Ncc,
                HieuLuc = tbXn.HieuLuc,
                HostName = null,
                PortName = null,
                BaudRate = null,
                DataBits = null,
                StopBits = null,
                Parity = null,
                Handshake = null,
                Encoding = null,
                ReadBufferSize = null,
                RtsEnable = false,
                DtrEnable = false,
                DiscardNull = false,
                ConnectionMode = null,
                ConnectionProtocol = null,
                AutoOpenPort = false,
                AutoOpenForm = false,
                ConnectionStatus = Enums.EnumConnectionStatus.Close,
                OpenById = null,
                OpenDateTime = null,
                CloseDateTime = null,
                LogDataEnabled = false,
            };
            await _tbXetNghiem.AddAsync(tbXnEntity);
            _cacheManager.RemoveByPattern(ThietBiXetNghiemService.MAYXETNGHIEMS_PATTERN_KEY);
            return Ok();
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThietBiXetNghiem)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> UpdateThietBiXetNghiem([FromBody]ThietBiXetNghiemViewModel tbXn)
        {
            var isValidateMa = await _tbXetNghiem.IsMaExists(
                tbXn.NhomThietBiId, tbXn.Ma, tbXn.NhomXetNghiemId,
                tbXn.Id, tbXn.IsCopy);
            var isValidateTen = await _tbXetNghiem.IsTenExists(
                tbXn.NhomThietBiId, tbXn.Ten, tbXn.NhomXetNghiemId,
                tbXn.Id, tbXn.IsCopy);

            if (isValidateMa)
            {
                var errors = new List<ValidationError>();
                errors.Add(new ValidationError("NhomXetNghiemId", _localizationService.GetResource("ThietBi.Ma.Exists")));
                errors.Add(new ValidationError("NhomThietBiId", _localizationService.GetResource("ThietBi.Ma.Exists")));
                errors.Add(new ValidationError("Ma", _localizationService.GetResource("ThietBi.Ma.Exists")));
                throw new ApiException("Validation Failed", 500, errors);
            }

            if (isValidateTen)
            {
                var errors = new List<ValidationError>();
                errors.Add(new ValidationError("NhomXetNghiemId", _localizationService.GetResource("ThietBi.Ten.Exists")));
                errors.Add(new ValidationError("NhomThietBiId", _localizationService.GetResource("ThietBi.Ten.Exists")));
                errors.Add(new ValidationError("Ten", _localizationService.GetResource("ThietBi.Ten.Exists")));
                throw new ApiException("Validation Failed", 500, errors);
            }

            var tbXnEntity = await _tbXetNghiem.GetByIdAsync(tbXn.Id);
            tbXnEntity.Ten = tbXn.Ten;
            tbXnEntity.Ma = tbXn.Ma;
            tbXnEntity.MauMayXetNghiemID = tbXn.NhomThietBiId;
            tbXnEntity.NhaCungCap = tbXn.Ncc;
            tbXnEntity.HieuLuc = tbXn.HieuLuc;

            await _tbXetNghiem.UpdateAsync(tbXnEntity);
            _cacheManager.RemoveByPattern(ThietBiXetNghiemService.MAYXETNGHIEMS_PATTERN_KEY);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.ThietBiXetNghiem)]
        public async Task<ActionResult> Delete(long id)
        {
            var thietBiXnEntity = await _tbXetNghiem.GetByIdAsync(id, w => w.Include(q => q.KetQuaXetNghiemChiTiets));
            if (thietBiXnEntity == null)
            {
                return NotFound();
            }

            if (thietBiXnEntity.KetQuaXetNghiemChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("ApiError.DeleteConflictedError"));
            }

            await _tbXetNghiem.DeleteByIdAsync(id);
            _cacheManager.RemoveByPattern(ThietBiXetNghiemService.MAYXETNGHIEMS_PATTERN_KEY);
            return NoContent();
        }

        [HttpPost("KichHoatThietBiXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThietBiXetNghiem)]
        public async Task<ActionResult> KichHoatThietBiXetNghiem(long id)
        {
            var entity = await _tbXetNghiem.GetByIdAsync(id);
            entity.HieuLuc = !entity.HieuLuc;
            await _tbXetNghiem.UpdateAsync(entity);
            _cacheManager.RemoveByPattern(ThietBiXetNghiemService.MAYXETNGHIEMS_PATTERN_KEY);
            return NoContent();
        }

        [HttpPost("ExportThietBiXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.ThietBiXetNghiem)]
        public async Task<ActionResult> ExportThietBiXetNghiem(QueryInfo queryInfo)
        {
            var gridData = await _tbXetNghiem.GetDataForGridAsync(queryInfo, true);
            var ngheNghiepData = gridData.Data.Select(p => (ThietBiXetNghiemGridVo)p).ToList();
            var excelData = ngheNghiepData.Map<List<ThietBiXetNghiemExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(ThietBiXetNghiemExportExcel.NhomThietBiDisplay), "Nhóm"),
                (nameof(ThietBiXetNghiemExportExcel.Ma), "Mã"),
                (nameof(ThietBiXetNghiemExportExcel.Ten), "Tên"),
                (nameof(ThietBiXetNghiemExportExcel.Ncc), "Nhà Cung Cấp"),
                (nameof(ThietBiXetNghiemExportExcel.TinhTrang), "Tình Trạng"),
                (nameof(ThietBiXetNghiemExportExcel.HieuLuc), "Sử Dụng"),
                (nameof(ThietBiXetNghiemExportExcel.NhomXetNghiemDisplay), "")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Thiết Bị Xét Nghiệm", labelName: "Thiết Bị Xét Nghiệm", isAutomaticallyIncreasesSTT: true);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ThietBiXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        #region lookup
        [HttpPost("GetListMayXetNghiem")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListMayXetNghiemAsync(DropDownListRequestModel model)
        {
            var lookup = await _tbXetNghiem.GetListMayXetNghiemAsync(model);
            return Ok(lookup);
        }


        #endregion
    }
}