using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.LoaiThuePhongNoiThucHien;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.LoaiThuePhongNoiThucHiens;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class LoaiThuePhongNoiThucHienController: CaminoBaseController
    {
        private ILoaiThuePhongNoiThucHienService _loaiThuePhongNoiThucHienService;
        private ILocalizationService _localizationService;
        public LoaiThuePhongNoiThucHienController(ILoaiThuePhongNoiThucHienService loaiThuePhongNoiThucHienService
        , ILocalizationService localizationService)
        {
            _loaiThuePhongNoiThucHienService = loaiThuePhongNoiThucHienService;
            _localizationService = localizationService;
        }

        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNoiThucHien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNoiThucHienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiThuePhongNoiThucHienService.GetDataForGridNoiThucHienAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridNoiThucHien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNoiThucHienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _loaiThuePhongNoiThucHienService.GetTotalPageForGridNoiThucHienAsync(queryInfo);
            return Ok(gridData);
        }


        #endregion

        #region Get data
        [HttpGet("GetThongTinNoiThucHien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LoaiThuePhongNoiThucHienViewModel>> GetThongTinNoiThucHienAsync(long id)
        {
            var model = await _loaiThuePhongNoiThucHienService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToModel<LoaiThuePhongNoiThucHienViewModel>());
        }

        [HttpPost("GetListLoaiThuePhongNoiThucHien")]
        public async Task<ActionResult> GetListLoaiThuePhongNoiThucHienAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _loaiThuePhongNoiThucHienService.GetListLoaiThuePhongNoiThucHienAsync(queryInfo);
            return Ok(lookup);
        }
        #endregion

        #region Thêm/xóa/sửa
        [HttpPut("LuuThongTinNoiThucHien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> LuuThongTinNoiThucHienAsync(LoaiThuePhongNoiThucHienViewModel viewModel)
        {
            if (viewModel.Id == 0)
            {
                var model = viewModel.ToEntity(new LoaiThuePhongNoiThucHien());
                await _loaiThuePhongNoiThucHienService.AddAsync(model);
            }
            else
            {
                var model = await _loaiThuePhongNoiThucHienService.GetByIdAsync(viewModel.Id);
                if (model == null)
                {
                    return NotFound();
                }

                viewModel.ToEntity(model);

                await _loaiThuePhongNoiThucHienService.UpdateAsync(model);
            }
            return NoContent();
        }

        [HttpDelete("XoaThongTinNoiThucHien")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCauHinhThuePhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> XoaThongTinNoiThucHienAsync(long id)
        {
            var model = await _loaiThuePhongNoiThucHienService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            await _loaiThuePhongNoiThucHienService.DeleteByIdAsync(id);
            return NoContent();
        }


        #endregion
    }
}
