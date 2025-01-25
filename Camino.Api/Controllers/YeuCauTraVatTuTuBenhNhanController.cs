using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauTraVatTuTuBenhNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Camino.Core.Domain.ValueObject.YeuCauTraThuocTuBenhNhan;
using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Camino.Services.YeuCauTraVatTuTuBenhNhan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
  
    public class YeuCauTraVatTuTuBenhNhanController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IYeuCauTraVatTuTuBenhNhanService _yeuCauTraVatTuTuBenhNhanService;

        public YeuCauTraVatTuTuBenhNhanController(IYeuCauTraVatTuTuBenhNhanService yeuCauTraVatTuTuBenhNhanService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _localizationService = localizationService;
            _excelService = excelService;
            _yeuCauTraVatTuTuBenhNhanService = yeuCauTraVatTuTuBenhNhanService;
        }

        #region DS trả vật tư
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS group vật tư
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncVatTuChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncVatTuChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetDataForGridAsyncVatTuChild(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncVatTuChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncVatTuChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetTotalPageForGridAsyncVatTuChild(queryInfo);
            return Ok(gridData);
        }

        #endregion


        #region DS group vật tư theo khoa và kho
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncVatTuTheoKho")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncVatTuTheoKho([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetDataForGridAsyncVatTuTheoKho(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncVatTuTheoKho")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncVatTuTheoKho([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetTotalPageForGridAsyncVatTuTheoKho(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS  vật tư chi tiết
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncBenhNhanChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncBenhNhanChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetDataForGridAsyncBenhNhanChild(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncBenhNhanChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncBenhNhanChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetTotalPageForGridAsyncBenhNhanChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncBenhNhanTheoKhoChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncBenhNhanTheoKhoChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetDataForGridAsyncBenhNhanTheoKhoChild(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncBenhNhanTheoKhoChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncBenhNhanTheoKhoChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraVatTuTuBenhNhanService.GetTotalPageForGridAsyncBenhNhanTheoKhoChild(queryInfo);
            return Ok(gridData);
        }

        #endregion

        [HttpPost("GetKhoVatTuCap2")]
        public async Task<ActionResult> GetKhoVatTuCap2([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauTraVatTuTuBenhNhanService.GetKhoVatTuCap2(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetTrangThaiPhieuTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiPhieuTraVatTu(long phieuTraId)
        {
            var result = await _yeuCauTraVatTuTuBenhNhanService.GetTrangThaiPhieuTraVatTu(phieuTraId);
            return Ok(result);
        }

        #region CRUD

        [HttpGet("GetPhieuTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TraVatTuTuBenhNhan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<YeuCauTraVatTuTuBenhNhanViewModel>> GetPhieuTraVatTu(long id)
        {
            var traVatTu = await _yeuCauTraVatTuTuBenhNhanService.GetByIdAsync(id,
                x => x.Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                      .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                      .Include(a => a.KhoTra)
                      .Include(a => a.KhoaHoanTra));

            var viewModel = traVatTu.ToModel<YeuCauTraVatTuTuBenhNhanViewModel>();
            return Ok(viewModel);
        }

        [HttpPost("GuiPhieuTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult> GuiPhieuTraVatTu(YeuCauTraVatTuTuBenhNhanViewModel viewModel)
        {
            if (!viewModel.YeuCauTraVatTuTuBenhNhanChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("TraVatTuTuBenhNhan.YeuCauTraVatTuTuBenhNhanChiTiets.Required"));
            }
            var traVatTu = viewModel.ToEntity<YeuCauTraVatTuTuBenhNhan>();
            foreach (var item in viewModel.YeuCauTraVatTuTuBenhNhanChiTiets)
            {
                var yeuCauTraVatTuTuBenhNhanChiTiets = await _yeuCauTraVatTuTuBenhNhanService.YeuCauTraVatTuTuBenhNhanChiTiets(item.YeuCauVatTuBenhVienId, item.VatTuBenhVienId);
                foreach (var chiTiet in yeuCauTraVatTuTuBenhNhanChiTiets)
                {
                    traVatTu.YeuCauTraVatTuTuBenhNhanChiTiets.Add(chiTiet);
                }
            }
            traVatTu.ThoiDiemHoanTraTongHopTuNgay = !string.IsNullOrEmpty(viewModel.TuNgay) ?
                                                      DateTime.ParseExact(viewModel.TuNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : (DateTime?)null;
            traVatTu.ThoiDiemHoanTraTongHopDenNgay = !string.IsNullOrEmpty(viewModel.DenNgay) ?
                                                    DateTime.ParseExact(viewModel.DenNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : DateTime.Now;
            await _yeuCauTraVatTuTuBenhNhanService.AddAsync(traVatTu);

            return Ok(traVatTu.Id);
        }

        [HttpPost("GuiLaiPhieuTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult> GuiLaiPhieuTraVatTu(YeuCauTraVatTuTuBenhNhanViewModel viewModel)
        {
            var traVatTu = await _yeuCauTraVatTuTuBenhNhanService.GetByIdAsync(viewModel.Id);
            viewModel.ToEntity(traVatTu);
            await _yeuCauTraVatTuTuBenhNhanService.UpdateAsync(traVatTu);
            return Ok(traVatTu.Id);
        }

        [HttpPost("XoaPhieuTraVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _yeuCauTraVatTuTuBenhNhanService.GetByIdAsync(id, x => x.Include(a => a.YeuCauTraVatTuTuBenhNhanChiTiets));
            if (entity == null)
            {
                return NotFound();
            }

            await _yeuCauTraVatTuTuBenhNhanService.XoaPhieuTraVatTu(entity);
            return NoContent();
        }

        #endregion

        #region Export excel
        [HttpPost("ExportPhieuTraVatTuTuBenhNhanNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.TraVatTuTuBenhNhan)]
        public async Task<ActionResult> ExportPhieuTraVatTuTuBenhNhanNoiTru(QueryInfo queryInfo)
        {

            var yeuCauTraVatTu = await _yeuCauTraVatTuTuBenhNhanService.GetDataForGridAsync(queryInfo, true);
            if (yeuCauTraVatTu == null || yeuCauTraVatTu.Data.Count == 0)
            {
                return NoContent();
            }
            var yeuCauTraVatTuTuBenhNhanGridVos = yeuCauTraVatTu.Data.Cast<YeuCauTraVatTuTuBenhNhanGridVo>().ToList();
            foreach (var yeuCau in yeuCauTraVatTuTuBenhNhanGridVos)
            {
                var queryVatTu = new QueryInfo()
                {
                    Skip = queryInfo.Skip,
                    Take = queryInfo.Take,
                    AdditionalSearchString = yeuCau.Id.ToString() // yeuCauTraVatTuTuBenhNhanId
                };
                var dataVatTus = await _yeuCauTraVatTuTuBenhNhanService.GetDataForGridAsyncVatTuChild(queryVatTu);
                yeuCau.TraVatTuChiTietVos = dataVatTus.Data.Cast<TraVatTuChiTietGridVo>().ToList();
                foreach (var vattu in yeuCau.TraVatTuChiTietVos)
                {
                    var queryBenhNhan = new QueryInfo()
                    {
                        Skip = queryInfo.Skip,
                        Take = queryInfo.Take
                    };
                    queryBenhNhan.AdditionalSearchString = vattu.YeuCauTraVatTuTuBenhNhanId + ";" + vattu.VatTuBenhVienId;
                    var dataBenhNhans = await _yeuCauTraVatTuTuBenhNhanService.GetDataForGridAsyncBenhNhanChild(queryBenhNhan);
                    vattu.TraVatTuBenhNhanChiTietVos = dataBenhNhans.Data.Cast<TraVatTuBenhNhanChiTietGridVo>().ToList();
                }
            }
            var bytes = _excelService.ExportPhieuTraVatTuTuBenhNhanNoiTru(yeuCauTraVatTuTuBenhNhanGridVos, queryInfo.AdditionalSearchString);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSPhieuTraVatTuTuBenhNhanNoiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");

        }
        #endregion

        [HttpPost("InPhieuYeuCauTraVatTuTuBenhNhan")]
        public string InPhieuYeuCauTraVatTuTuBenhNhan(PhieuTraVatTu phieuTraVatTu)
        {
            var result = _yeuCauTraVatTuTuBenhNhanService.InPhieuYeuCauTraVatTuTuBenhNhan(phieuTraVatTu);
            return result;
        }
    }
}