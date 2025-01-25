using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class TiepNhanBenhNhanController : CaminoBaseController
    {

        #region feedback 29/10/2020

        [HttpPost("GetMarketingForBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> GetMarketingForBenhNhan(long benhNhanId)
        {
            var result = await _tiepNhanBenhNhanService.GetMarketingForBenhNhan(benhNhanId);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataThongTinGoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataThongTinGoiForGridAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _tiepNhanBenhNhanService.GetDataThongTinGoiForGridAsync(queryInfo);
            var gridData = await _tiepNhanBenhNhanService.GetDataThongTinGoiForGridAsyncVer2(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalThongTinGoiPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalThongTinGoiPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            // hàm này đang ko sử dụng vì ko phân trang => ko cần sửa
            var gridData = await _tiepNhanBenhNhanService.GetTotalThongTinGoiPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDichVuGoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataDichVuGoiForGridAsync([FromBody] QueryInfo queryInfo)
        {
            long benhNhanId = 0;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
            }
            var dichVuGiuongDaChiDinhs = await _dieuTriNoiTruService.GetThongTinSuDungDichVuGiuongTrongGoiAsync(benhNhanId);
            //var gridData = await _tiepNhanBenhNhanService.GetDataDichVuGoiForGridAsync(queryInfo, dichVuGiuongDaChiDinhs);
            var gridData = await _tiepNhanBenhNhanService.GetDataDichVuGoiForGridAsyncVer2(queryInfo, dichVuGiuongDaChiDinhs);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDichVuGoiPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalDichVuGoiPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetTotalDichVuGoiPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDichVuGoiForGridAsyncUpdateView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataDichVuGoiForGridAsyncUpdateView([FromBody] QueryInfo queryInfo)
        {
            long benhNhanId = 0;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
            }
            var dichVuGiuongDaChiDinhs = await _dieuTriNoiTruService.GetThongTinSuDungDichVuGiuongTrongGoiAsync(benhNhanId);
            //var gridData = await _tiepNhanBenhNhanService.GetDataDichVuGoiForGridAsyncUpdateView(queryInfo, dichVuGiuongDaChiDinhs);
            var gridData = await _tiepNhanBenhNhanService.GetDataDichVuGoiForGridAsyncUpdateViewVer2(queryInfo, dichVuGiuongDaChiDinhs);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDichVuGoiPageForGridAsyncUpdateView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalDichVuGoiPageForGridAsyncUpdateView([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tiepNhanBenhNhanService.GetTotalDichVuGoiPageForGridAsyncUpdateView(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ThemDichVuTuGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemDichVuTuGoi([FromBody] List<ThemDichVuKhamBenhVo> model)
        {
            var result = await _tiepNhanBenhNhanService.GetDataForGoiGridAsync(model);
            return Ok(result);
        }

        [HttpPost("CheckDuSoLuongTonTrongGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> CheckDuSoLuongTonTrongGoi(CheckDuSoLuongTonTrongGoi model)
        {
            var result = await _tiepNhanBenhNhanService.CheckSoLuongTonTrongGoi(model);

            return Ok(result);
        }

        [HttpPost("CheckDuSoLuongTonTrongGoiForCreate")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> CheckDuSoLuongTonTrongGoiForCreate(CheckDuSoLuongTonTrongGoi model)
        {
            var result = await _tiepNhanBenhNhanService.CheckSoLuongTonTrongGoiForCreate(model);

            return Ok(result);
        }

        [HttpPost("CheckDuSoLuongTonTrongGoiLstDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> CheckDuSoLuongTonTrongGoiLstDichVu(CheckDuSoLuongTonTrongGoiListDichVu model)
        {
            var result = await _tiepNhanBenhNhanService.CheckSoLuongTonTrongGoiLstDichVu(model);

            return Ok(result);
        }

        [HttpPost("ThemDichVuFromGoiUpdateView")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> ThemDichVuFromGoiUpdateView(modelUpdateView model)
        {
            var entity = await _tiepNhanBenhNhanService.ThemDichVuFromGoiUpdateView(model.Data, model.YeuCauTiepNhanId, model.BenhNhanid, model.MucHuongBHYT);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(entity);

            // get lại YCTN
            //var yeuCauTiepNhan = await _tiepNhanBenhNhanService.GetByIdHaveInclude(model.YeuCauTiepNhanId);
            var yeuCauTiepNhan = await _tiepNhanBenhNhanService.GetByIdHaveIncludeSimplify(model.YeuCauTiepNhanId);
            var result = yeuCauTiepNhan.ToModel<TiepNhanBenhNhanViewModel>();
            //var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            // DichVuKhamBenhBenhVienId => MaDichVuId
            //ChuongTrinhGoiDichVuId => id
            result = SetValueForGrid(result, entity, true, model.ListDichVuCheckTruocDos, null, model.Data,null);

            //result.YeuCauKhacGrid = await _tiepNhanBenhNhanService.SetGiaForGrid(result.YeuCauKhacGrid, entity.BHYTMucHuong);

            return Ok(result.YeuCauKhacGrid);

        }

        [HttpPost("XoaGoiGiuLaiDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> XoaGoiGiuLaiDichVu(DanhSachGoiChon model)
        {
            var entity = await _tiepNhanBenhNhanService.XoaGoiGiuLaiDichVu(model);

            //update basic
            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(entity);

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, false,null,null,null,null);

            return Ok(result.YeuCauKhacGrid);
        }

        [HttpPost("XoaGoiKhongGiuLaiDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult> XoaGoiKhongGiuLaiDichVu(DanhSachGoiChon model)
        {
            var entity = await _tiepNhanBenhNhanService.XoaGoiKhongGiuLaiDichVu(model);

            //update basic
            // 31/05/2021: cập nhật xóa dịch vụ và gói dịch vụ -> gọi hàm delete chung
            await _tiepNhanBenhNhanService.PrepareForDeleteDichVuAndUpdateAsync(entity); //PrepareForEditDichVuAndUpdateAsync

            var result = entity.ToModel<TiepNhanBenhNhanViewModel>();

            result = SetValueForGrid(result, entity, false,null,null,null,null);

            return Ok(result.YeuCauKhacGrid);
        }

        #endregion feedback 29/10/2020
    }
}
