using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.LoaiThuePhongPhauThuat;
using Camino.Api.Models.NoiGioiThieu;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.LoaiThuePhongPhauThuats;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class LoaiThuePhongPhauThuatController : CaminoBaseController
    {
        private ILoaiThuePhongPhauThuatService _loaiThuePhongPhauThuatService;
        private ILocalizationService _localizationService;
        public LoaiThuePhongPhauThuatController(ILoaiThuePhongPhauThuatService loaiThuePhongPhauThuatService
        , ILocalizationService localizationService
        )
        {
            _loaiThuePhongPhauThuatService = loaiThuePhongPhauThuatService;
            _localizationService = localizationService;
        }

        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridLoaiPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLoaiPhauThuatAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiThuePhongPhauThuatService.GetDataForGridCauHinhThuePhongAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridLoaiPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLoaiPhauThuatAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiThuePhongPhauThuatService.GetTotalPageForGridCauHinhThuePhongAsync(queryInfo);
            return Ok(gridData);
        }


        #endregion

        #region Get data
        [HttpGet("GetThongTinLoaiPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LoaiThuePhongPhauThuatViewModel>> GetThongTinLoaiPhauThuatAsync(long id)
        {
            var model = await _loaiThuePhongPhauThuatService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToModel<LoaiThuePhongPhauThuatViewModel>());
        }

        [HttpPost("GetListLoaiThuePhongPhauThuat")]
        public async Task<ActionResult> GetListLoaiThuePhongPhauThuatAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _loaiThuePhongPhauThuatService.GetListLoaiThuePhongPhauThuatAsync(queryInfo);
            return Ok(lookup);
        }
        #endregion

        #region Thêm/xóa/sửa
        [HttpPut("LuuThongTinLoaiPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> LuuThongTinLoaiPhauThuatAsync(LoaiThuePhongPhauThuatViewModel viewModel)
        {
            if (viewModel.Id == 0)
            {
                var model = viewModel.ToEntity(new LoaiThuePhongPhauThuat());
                await _loaiThuePhongPhauThuatService.AddAsync(model);
            }
            else
            {
                var model = await _loaiThuePhongPhauThuatService.GetByIdAsync(viewModel.Id);
                if (model == null)
                {
                    return NotFound();
                }

                viewModel.ToEntity(model);

                await _loaiThuePhongPhauThuatService.UpdateAsync(model);
            }
            return NoContent();
        }

        [HttpDelete("XoaThongTinLoaiPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XoaThongTinLoaiPhauThuatAsync(long id)
        {
            var model = await _loaiThuePhongPhauThuatService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            await _loaiThuePhongPhauThuatService.DeleteByIdAsync(id);
            return NoContent();
        }


        #endregion
    }
}
