using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamBenhs;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDanhSachDaKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhSachDaKham([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncDanhSachDaKham(queryInfo);
            return Ok(gridData);
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDanhSachDaKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhSachDaKham([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridAsyncDanhSachDaKham(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("CapNhatKhamLai")]
        public async Task<ActionResult<GridDataSource>> CapNhatKhamLai(long yckbId,long phongkham)
        {
            var gridData = await _yeuCauKhamBenhService.UpdateBenhNhanCanKhamLai(yckbId, phongkham);
            return Ok(gridData);
        }

        [HttpPost("CapNhatKhamLaiKhamSucKhoe")]
        public async Task<ActionResult> CapNhatKhamLaiKhamSucKhoeAsync(KhamLaiKhamDoanVo khamLaiVo)
        {
            await _yeuCauKhamBenhService.CapNhatKhamLaiKhamSucKhoeAsync(khamLaiVo);
            return Ok();
        }
        [HttpPost("GetYeuCauKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauKhamBenh(long yeuCauTiepNhanId,long yeuCauKhamBenhId)
        {
            var gridData = await _yeuCauKhamBenhService.GetYeuCauKhamBenh(yeuCauKhamBenhId, yeuCauTiepNhanId);
            return Ok(gridData);
        }
    }
}
