using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.QuocGia;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Camino.Services.NhomICDTheoBenhViens;
using Camino.Services.QuocGias;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhomICDTheoBenhVienController : CaminoBaseController
    {
        private readonly INhomICDTheoBenhVienService _nhomICDTheoBenhVienService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public NhomICDTheoBenhVienController(INhomICDTheoBenhVienService nhomICDTheoBenhVienService,
            ILocalizationService localizationService, IExcelService excelService)
        {
            _nhomICDTheoBenhVienService = nhomICDTheoBenhVienService;
            _localizationService = localizationService;
            _excelService = excelService;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucBenhVaNhomBenh)]
        [HttpPost("GetDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhomICDTheoBenhVienService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucBenhVaNhomBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _nhomICDTheoBenhVienService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("KichHoatTrangThaiNhoBenhVaTenBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucBenhVaNhomBenh)]
        public async Task<ActionResult> KichHoatTrangThaiNhoBenhVaTenBenh(long id)
        {
            var entity = await _nhomICDTheoBenhVienService.GetByIdAsync(id);
            entity.HieuLuc = !entity.HieuLuc;
            await _nhomICDTheoBenhVienService.UpdateAsync(entity);

            return NoContent();
        }

        [HttpPost("GetMaICD")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetMaICD([FromBody] DropDownListRequestModel model, bool coHienThiMa = false)
        {
            var lookup = await _nhomICDTheoBenhVienService.GetMaICD(model, coHienThiMa);
            return Ok(lookup);
        }

        [HttpPost("GetChuongICD")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetChuongICD([FromBody] DropDownListRequestModel model)
        {
            var lookup = await _nhomICDTheoBenhVienService.GetChuong(model);
            return Ok(lookup);
        }

        [HttpPost("GetMaTuTaoICD")]
        public ActionResult<ICollection<LookupItemVo>> GetMaTuTaoICD([FromBody] DropDownListRequestModel model)
        {
            var maICDs = JsonConvert.DeserializeObject<JsonMaICD>(model.ParameterDependencies.Replace("undefined", "null"));
            var lookup = _nhomICDTheoBenhVienService.GetMaTuTaoICD(model, maICDs);
            return Ok(lookup);
        }

        #region CRUD

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucBenhVaNhomBenh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NhomICDTheoBenhVienViewModel>> Get(long id)
        {
            var nhomICDTheoBenhVien = await _nhomICDTheoBenhVienService.GetByIdAsync(id, s => s.Include(c => c.ChuongICD));
            if (nhomICDTheoBenhVien == null)
            {
                return NotFound();
            }
            NhomICDTheoBenhVienViewModel nhomICDTheoBenhVienVM = nhomICDTheoBenhVien.ToModel<NhomICDTheoBenhVienViewModel>();
            return Ok(nhomICDTheoBenhVienVM);
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucBenhVaNhomBenh)]
        public async Task<ActionResult> Post([FromBody] NhomICDTheoBenhVienViewModel nhomICDTheoBenhVienViewModel)
        {
            var nhomICDTheoBenhVien = nhomICDTheoBenhVienViewModel.ToEntity<NhomICDTheoBenhVien>();
            await _nhomICDTheoBenhVienService.AddAsync(nhomICDTheoBenhVien);

            return NoContent();
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucBenhVaNhomBenh)]
        public async Task<ActionResult> Put([FromBody] NhomICDTheoBenhVienViewModel nhomICDTheoBenhVienViewModel)
        {
            var nhomICDTheoBenhVien = await _nhomICDTheoBenhVienService.GetByIdAsync(nhomICDTheoBenhVienViewModel.Id, s => s.Include(c => c.ChuongICD));
            if (nhomICDTheoBenhVien == null)
            {
                return NotFound();
            }

            nhomICDTheoBenhVienViewModel.ToEntity(nhomICDTheoBenhVien);
            await _nhomICDTheoBenhVienService.UpdateAsync(nhomICDTheoBenhVien);

            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucBenhVaNhomBenh)]
        public async Task<ActionResult> Deletes([FromBody]  DeletesViewModel DeletesViewModel)
        {
            var nhomICDTheoBenhVien = await _nhomICDTheoBenhVienService.GetByIdsAsync(DeletesViewModel.Ids);
            if (nhomICDTheoBenhVien == null)
            {
                return NotFound();
            }
            await _nhomICDTheoBenhVienService.DeleteAsync(nhomICDTheoBenhVien);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucBenhVaNhomBenh)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucvu = await _nhomICDTheoBenhVienService.GetByIdAsync(id);
            if (chucvu == null)
            {
                return NotFound();
            }
            await _nhomICDTheoBenhVienService.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion
    }
}
