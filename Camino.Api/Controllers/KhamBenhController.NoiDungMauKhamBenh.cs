using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNoiDungMauKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNoiDungMauKhamBenhAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridNoiDungMauKhamBenhAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridNoiDungMauKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNoiDungMauKhamBenhAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridNoiDungMauKhamBenhAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Get data
        [HttpGet("GetThongTinNoiDungMauKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<NoiDungMauKhamBenhViewModel>> GetThongTinNoiDungMauKhamBenhAsync(long id)
        {
            var result = await _yeuCauKhamBenhService.GetThongTinNoiDungMauKhamBenhAsync(id);
            return result.ToModel<NoiDungMauKhamBenhViewModel>();
        }

        #endregion

        #region Xử lý data
        [HttpDelete("XoaThongTinNoiDungMauKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<NoiDungMauKhamBenhViewModel>> XoaThongTinNoiDungMauKhamBenhAsync(long id)
        {
            await _yeuCauKhamBenhService.XoaThongTinNoiDungMauKhamBenhAsync(id);
            return Ok();
        }

        [HttpPost("LuuThongTinNoiDungMauKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> LuuThongTinNoiDungMauKhamBenhAsync([FromBody] NoiDungMauKhamBenhViewModel viewModel)
        {
            if (viewModel.Id != 0)
            {
                var noiDungMau = await _yeuCauKhamBenhService.GetThongTinNoiDungMauKhamBenhAsync(viewModel.Id, true);
                viewModel.ToEntity(noiDungMau);
                await _yeuCauKhamBenhService.LuuThongTinNoiDungMauKhamBenhAsync(noiDungMau);
            }
            else
            {
                var noiDungMau = viewModel.ToEntity<NoiDungMauKhamBenh>();
                await _yeuCauKhamBenhService.LuuThongTinNoiDungMauKhamBenhAsync(noiDungMau);
            }

            return Ok();
        }
        #endregion
    }
}
