using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaCLS;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        #region danh sách kết luận cls ksk đoàn
        [HttpPost("GetDataForGridAsyncDanhSachKetLuanCLSKhamSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhSachKetLuanCLSKhamSucKhoe([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncDanhSachKetLuanCLSKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDanhSachKetLuanCLSKhamSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhSachKetLuanCLSKhamSucKhoe([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForGridAsyncDanhSachKetLuanCLSKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetDataDanhSachChuaKetLuanCLSKhamSucKhoeForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataDanhSachChuaKetLuanCLSKhamSucKhoeForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncDanhSachChuaKetLuanCLSKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageDanhSachChuaKetLuanCLSKhamSucKhoeForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachChuaKetLuanCLSKhamSucKhoeForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForGridAsyncDanhSachChuaKetLuanCLSKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region thông tin cls
        [HttpPost("GetDataThongTinCLSForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataThongTinCLSForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncKetQuaCLS(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetTotalPageForGridAsyncKetQuaCLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncKetQuaCLS([FromBody]QueryInfo queryInfo)
        {
            //bo lazy load
            var gridData = await _khamDoanService.GetDataForGridAsyncKetQuaCLS(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetListKetQuaTheoYeuCauTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        public async Task<List<ListKetQuaCLS>> GetListKetQuaTheoYeuCauTiepNhan(long yeuCauTiepNhanId)
        {
            var gridData = await _khamDoanService.GetListKetQuaCLSNew(yeuCauTiepNhanId);
            return  gridData.ToList();
        }
        [HttpPost("LuuKetLuanCLS")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        public async Task<ActionResult> Put([FromBody] KhamDoanKetLuanCLSViewModel khamDoanKetLuanCLSViewModel)
        {
            // thông tin cần hỏi  lưu cls cần update nhưng thông tin nào
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(khamDoanKetLuanCLSViewModel.Id);
        
            if (yeuCauTiepNhan == null)
            {
                return NotFound();
            }
            khamDoanKetLuanCLSViewModel.ToEntity(yeuCauTiepNhan);
            yeuCauTiepNhan.KSKNhanVienDanhGiaCanLamSangId = _userAgentHelper.GetCurrentUserId();
            await _yeuCauTiepNhanService.UpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }
        [HttpGet("GetDataCLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KhamDoanKetLuanCLSViewModel>> GetDataCLS(long id)
        {
            var result = await _yeuCauTiepNhanService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<KhamDoanKetLuanCLSViewModel>();
            return Ok(resultData);
        }
        #endregion
        #region check hợp hồng đã kết thúc chưa
        [HttpPost("CheckHopDongKetThuc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKetLuanCanLamSangKhamSucKhoeDoan)]
        public bool CheckHopDongKetThuc(ThongTinCheckHopDong chechHopDong)
        {
            var kq =  _khamDoanService.CheckHopDongKetThuc(chechHopDong);
            return kq;
        }
        #endregion
    }
}
