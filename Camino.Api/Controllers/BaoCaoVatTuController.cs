using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Services.BaoCaoVatTus;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoVatTuController : ControllerBase
    {
        private readonly IBaoCaoVatTuService _baoCaoVatTuService;
        public BaoCaoVatTuController(IBaoCaoVatTuService baoCaoVatTuService)
        {
            _baoCaoVatTuService = baoCaoVatTuService;
        }

        #region Báo cáo tồn kho vật tư

        [HttpPost("GetKhoChoBaoCaoVatTu")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoChoBaoCaoVatTu([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoVatTuService.GetKhoChoBaoCaoVatTu(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoTonKhoVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTonKhoVatTuYTe)]
        public async Task<ActionResult> GetDataBaoCaoTonKhoVatTuForGridAsync(BaoCaoTonKhoVatTuQueryInfo queryInfo)
        {
            var gridData = await _baoCaoVatTuService.GetDataBaoCaoTonKhoVatTuForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoTonKhoVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Core.Domain.Enums.DocumentType.BaoCaoTonKhoVatTuYTe)]
        public async Task<ActionResult> ExportBaoCaoTonKhoVatTu(BaoCaoTonKhoVatTuQueryInfo queryInfo)
        {
            var gridData = await _baoCaoVatTuService.GetDataBaoCaoTonKhoVatTuForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoVatTuService.ExportBaoCaoTonKhoVatTu(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTonKhoVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #region Báo cáo thẻ kho vật tư

        [HttpPost("GetKhoChoBaoCaoTheKhoVatTu")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoChoBaoCaoTheKhoVatTu([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoVatTuService.GetKhoChoBaoCaoVatTu(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetVatTuTheoKhoBaoCao")]
        public async Task<ActionResult<ICollection<DuocPhamTheoKhoBaoCaoLookup>>> GetVatTuTheoKhoBaoCao(DropDownListRequestModel model)
        {
            var lstKhoIdDaChon = JsonConvert.DeserializeObject<KhoDaChonVo>(model.ParameterDependencies);
            var result = await _baoCaoVatTuService.GetKhoDuocPhamVatTuTheoKhoHangHoa(model, lstKhoIdDaChon.KhoId);

            return Ok(result);
        }

        [HttpPost("GetDataTheKhoVatTuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTheKhoVatTuYTe)]
        public async Task<ActionResult> GetDataTheKhoVatTuForGridAsync(BaoCaoTheKhoQueryInfo queryInfo)
        {
             var grid = await _baoCaoVatTuService.GetDataTheKhoVatTuForGridAsync(queryInfo);
            return Ok(grid);
        }

        [HttpPost("GetDataTheKhoVatTuForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTheKhoVatTuYTe)]
        public async Task<ActionResult> GetDataTheKhoVatTuForGridChildAsync(QueryInfo queryInfo)
        {
            var gridChild = await _baoCaoVatTuService.GetDataTheKhoVatTuForGridChildAsync(queryInfo);
            return Ok(gridChild);
        }

        [HttpPost("ExportTheKhoVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTheKhoVatTuYTe)]
        public async Task<ActionResult> ExportTheKhoVatTu(BaoCaoTheKhoQueryInfo queryInfo)
        {
            var bytes = _baoCaoVatTuService.ExportTheKhoVatTu(queryInfo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoXuatNhapTonVatTu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion
    }
}
