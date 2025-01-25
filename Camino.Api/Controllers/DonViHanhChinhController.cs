using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.DonViHanhChinh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonViHanhChinhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DonViHanhChinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.DonViHanhChinh;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class DonViHanhChinhController : CaminoBaseController
    {
        private readonly IDonViHanhChinhService _donViHanhChinhService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        public DonViHanhChinhController(IDonViHanhChinhService donViHanhChinhService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _donViHanhChinhService = donViHanhChinhService;
            _localizationService = localizationService;
            _excelService = excelService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDonViHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]DonViHanhChinhQueryInfo queryInfo)
        {
            var gridData = await _donViHanhChinhService.GetDataForGridAsync(queryInfo,false);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDonViHanhChinh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]DonViHanhChinhQueryInfo queryInfo)
        {
            var gridData = await _donViHanhChinhService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region Get/Add/Delete/Update
        //Add
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDonViHanhChinh)]
        public async Task<ActionResult> Post([FromBody] DonViHanhChinhViewModel model)
        {
            if (model.CapHanhChinh == CapHanhChinh.QuanHuyen)
            {
                model.TrucThuocDonViHanhChinhId = model.TrucThuocThanhPhoId.HasValue ? model.TrucThuocThanhPhoId : null;
            }
            if (model.CapHanhChinh == CapHanhChinh.PhuongXa)
            {
                model.TrucThuocDonViHanhChinhId = model.TrucThuocQuanHuyenId.HasValue ? model.TrucThuocQuanHuyenId : null;
            }

            var obj = model.ToEntity<DonViHanhChinh>();
            obj.TenDonViHanhChinh = model.Ten;

            await _donViHanhChinhService.AddAsync(obj);
            return Ok(obj);
        }
        //Get
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDonViHanhChinh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DonViHanhChinhViewModel>> Get(long id)
        {
            var data = await _donViHanhChinhService.GetByIdAsync(id, a => a.Include(b => b.TrucThuocDonViHanhChinh)
                                                                            .ThenInclude(b => b.TrucThuocDonViHanhChinh)
                                                                            .ThenInclude(b => b.TrucThuocDonViHanhChinh));
            var result = data.Map<DonViHanhChinhViewModel>();
            if (data.CapHanhChinh == CapHanhChinh.QuanHuyen)
                result.TrucThuocThanhPhoId = data.TrucThuocDonViHanhChinhId;
            if (data.CapHanhChinh == CapHanhChinh.PhuongXa)
            {
                result.TrucThuocQuanHuyenId = data.TrucThuocDonViHanhChinhId;
                result.TrucThuocThanhPhoId = data.TrucThuocDonViHanhChinh?.TrucThuocDonViHanhChinhId;
            }
            return Ok(result);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDonViHanhChinh)]
        public async Task<ActionResult> Put([FromBody] DonViHanhChinhViewModel model)
        {
            var obj = await _donViHanhChinhService.GetByIdAsync(model.Id);

            //model.TrucThuocDonViHanhChinhId = model.TrucThuocPhuongXaId.HasValue ? model.TrucThuocPhuongXaId :
            //    model.TrucThuocQuanHuyenId.HasValue ? model.TrucThuocQuanHuyenId :
            //    model.TrucThuocThanhPhoId.HasValue ? model.TrucThuocThanhPhoId : null;

            model.ToEntity(obj);
            obj.TenDonViHanhChinh = model.Ten;
            await _donViHanhChinhService.UpdateAsync(obj);
            return Ok(obj);
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDonViHanhChinh)]
        public async Task<ActionResult> Delete(long id)
        {
            var chucvu = await _donViHanhChinhService.GetByIdAsync(id);
            if (chucvu == null)
            {
                return NotFound();
            }
            await _donViHanhChinhService.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion

        #region get lookup
        [HttpPost("GetTinhThanhLookup")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTinhThanhLookup([FromBody] DonViHanhChinhLookupQueryInfo model)
        {
            var lookup = await _donViHanhChinhService.GetTinhThanhLookup(model);
            return Ok(lookup);
        }

        [HttpPost("GetQuanHuyenLookup")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetQuanHuyenLookup([FromBody] DonViHanhChinhLookupQueryInfo model)
        {
            var lookup = await _donViHanhChinhService.GetQuanHuyenLookup(model, model != null && model.TinhThanhId != null ? (long)model.TinhThanhId : 0);
            return Ok(lookup);
        }

        [HttpPost("GetPhuongXaLookup")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetPhuongXaLookup([FromBody] DonViHanhChinhLookupQueryInfo model)
        {
            var lookup = await _donViHanhChinhService.GetPhuongXaLookup(model, model != null && model.QuanHuyenId != null ? (long)model.QuanHuyenId : 0, model != null && model.TinhThanhId != null ? (long)model.TinhThanhId : 0);
            return Ok(lookup);
        }

        [HttpPost("GetKhomApLookup")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhomApLookup([FromBody] DonViHanhChinhLookupQueryInfo model)
        {
            var lookup = await _donViHanhChinhService.GetKhomApLookup(model, model != null && model.PhuongXaId != null ? (long)model.PhuongXaId : 0, model != null && model.QuanHuyenId != null ? (long)model.QuanHuyenId : 0, model != null && model.TinhThanhId != null ? (long)model.TinhThanhId : 0);
            return Ok(lookup);
        }
        #endregion

        [HttpPost("ExportDonViHanhChinh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDonViHanhChinh)]
        public async Task<ActionResult> ExportDonViHanhChinh([FromBody]DonViHanhChinhQueryInfo queryInfo)
        {
            //var gridData =  _donViHanhChinhService.GetDataExportExecl(queryInfo);

           


            byte[] bytes = null;
            //if (gridData != null)
            //{
            //    //bytes = _baoCaoKhamDoanHopDongService.ExportBaoCaoTongHopKetQuaKSK(gridData, gridDataTheoNhanVien, getTenCongTy, getHopDong);
            //}
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTongHopKetQuaKSK" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");

        }
    }
}
