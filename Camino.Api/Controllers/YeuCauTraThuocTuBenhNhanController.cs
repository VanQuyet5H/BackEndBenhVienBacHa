using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauTraDuocPhamTuBenhNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauTraThuocTuBenhNhan;
using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Camino.Services.YeuCauTraThuocTuBenhNhan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class YeuCauTraThuocTuBenhNhanController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IYeuCauTraThuocTuBenhNhanService _yeuCauTraThuocTuBenhNhanService;

        public YeuCauTraThuocTuBenhNhanController(IYeuCauTraThuocTuBenhNhanService yeuCauTraThuocTuBenhNhanService, ILocalizationService localizationService, IJwtFactory iJwtFactory, IExcelService excelService)
        {
            _localizationService = localizationService;
            _excelService = excelService;
            _yeuCauTraThuocTuBenhNhanService = yeuCauTraThuocTuBenhNhanService;
        }

        #region DS trả dược phẩm
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetDataForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS group dược phẩm
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDuocPhamChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDuocPhamChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetDataForGridAsyncDuocPhamChild(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncDuocPhamChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDuocPhamChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetTotalPageForGridAsyncDuocPhamChild(queryInfo);
            return Ok(gridData);
        }

        #endregion


        #region DS group dược phẩm theo khoa và kho
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDuocPhamTheoKho")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDuocPhamTheoKho([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetDataForGridAsyncDuocPhamTheoKho(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncDuocPhamTheoKho")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDuocPhamTheoKho([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetTotalPageForGridAsyncDuocPhamTheoKho(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS  dược phẩm chi tiết
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncBenhNhanChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncBenhNhanChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetDataForGridAsyncBenhNhanChild(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncBenhNhanChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncBenhNhanChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetTotalPageForGridAsyncBenhNhanChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncBenhNhanTheoKhoChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncBenhNhanTheoKhoChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetDataForGridAsyncBenhNhanTheoKhoChild(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncBenhNhanTheoKhoChild")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncBenhNhanTheoKhoChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTraThuocTuBenhNhanService.GetTotalPageForGridAsyncBenhNhanTheoKhoChild(queryInfo);
            return Ok(gridData);
        }

        #endregion

        [HttpPost("GetKhoDuocPhamCap2")]
        public async Task<ActionResult> GetKhoDuocPhamCap2([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauTraThuocTuBenhNhanService.GetKhoDuocPhamCap2(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetKhoaPhong")]
        public async Task<ActionResult> GetKhoaPhong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauTraThuocTuBenhNhanService.GetKhoaPhong(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("ThongTinKhoaHoanTra")]
        public async Task<ActionResult> ThongTinKhoaHoanTra()
        {
            var lookup = await _yeuCauTraThuocTuBenhNhanService.ThongTinKhoaHoanTra();
            return Ok(lookup);
        }

        #region CRUD

        [HttpGet("GetPhieuTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TraThuocTuBenhNhan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<YeuCauTraDuocPhamTuBenhNhanViewModel>> GetPhieuTraDuocPham(long id)
        {
            var traDuocPham = await _yeuCauTraThuocTuBenhNhanService.GetByIdAsync(id,
                x => x.Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                      .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                      .Include(a => a.KhoTra)
                      .Include(a => a.KhoaHoanTra));

            var viewModel = traDuocPham.ToModel<YeuCauTraDuocPhamTuBenhNhanViewModel>();
            return Ok(viewModel);
        }

        [HttpPost("GuiPhieuTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult> GuiPhieuTraDuocPham(YeuCauTraDuocPhamTuBenhNhanViewModel viewModel)
        {
            if (!viewModel.YeuCauTraThuocTuBenhNhanChiTiets.Any())
            {
                throw new ApiException(_localizationService.GetResource("TraThuocTuBenhNhan.YeuCauTraThuocTuBenhNhanChiTiets.Required"));
            }
            var traDuocPham = viewModel.ToEntity<YeuCauTraDuocPhamTuBenhNhan>();
            foreach (var item in viewModel.YeuCauTraThuocTuBenhNhanChiTiets)
            {
                var yeuCauTraDuocPhamTuBenhNhanChiTiets = await _yeuCauTraThuocTuBenhNhanService.YeuCauTraDuocPhamTuBenhNhanChiTiet(item.YeuCauDuocPhamBenhVienId, item.DuocPhamBenhVienId);
                foreach (var chiTiet in yeuCauTraDuocPhamTuBenhNhanChiTiets)
                {
                    traDuocPham.YeuCauTraDuocPhamTuBenhNhanChiTiets.Add(chiTiet);
                }
            }
            traDuocPham.ThoiDiemHoanTraTongHopTuNgay = !string.IsNullOrEmpty(viewModel.TuNgay) ?
                                                       DateTime.ParseExact(viewModel.TuNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : (DateTime?)null;
            traDuocPham.ThoiDiemHoanTraTongHopDenNgay = !string.IsNullOrEmpty(viewModel.DenNgay) ?
                                                    DateTime.ParseExact(viewModel.DenNgay, "dd/MM/yyyy hh:mm tt", null, DateTimeStyles.None) : DateTime.Now;
            await _yeuCauTraThuocTuBenhNhanService.AddAsync(traDuocPham);

            return Ok(traDuocPham.Id);
        }

        [HttpPost("GuiLaiPhieuTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult> GuiLaiPhieuTraDuocPham(YeuCauTraDuocPhamTuBenhNhanViewModel viewModel)
        {
            var traDuocPham = await _yeuCauTraThuocTuBenhNhanService.GetByIdAsync(viewModel.Id);
            viewModel.ToEntity(traDuocPham);
            await _yeuCauTraThuocTuBenhNhanService.UpdateAsync(traDuocPham);
            return Ok(traDuocPham.Id);
        }

        [HttpPost("XoaPhieuTraThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _yeuCauTraThuocTuBenhNhanService.GetByIdAsync(id, x => x.Include(a => a.YeuCauTraDuocPhamTuBenhNhanChiTiets));
            if (entity == null)
            {
                return NotFound();
            }

            await _yeuCauTraThuocTuBenhNhanService.XoaPhieuTraThuoc(entity);
            return NoContent();
        }

        [HttpGet("GetTrangThaiPhieuTraDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult<TrangThaiDuyetVo>> GetTrangThaiPhieuTraDuocPham(long phieuTraId)
        {
            var result = await _yeuCauTraThuocTuBenhNhanService.GetTrangThaiPhieuTraDuocPham(phieuTraId);
            return Ok(result);
        }
        #endregion


        #region Export excel
        [HttpPost("ExportPhieuTraThuocTuBenhNhanNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.TraThuocTuBenhNhan)]
        public async Task<ActionResult> ExportPhieuTraThuocTuBenhNhanNoiTru(QueryInfo queryInfo)
        {

            var yeuCauTraThuoc = await _yeuCauTraThuocTuBenhNhanService.GetDataForGridAsync(queryInfo, true);
            if (yeuCauTraThuoc == null || yeuCauTraThuoc.Data.Count == 0)
            {
                return NoContent();
            }
            var yeuCauTraThuocTuBenhNhanGridVos = yeuCauTraThuoc.Data.Cast<YeuCauTraThuocTuBenhNhanGridVo>().ToList();
            foreach (var yeuCau in yeuCauTraThuocTuBenhNhanGridVos)
            {
                var queryThuoc = new QueryInfo()
                {
                    Skip = queryInfo.Skip,
                    Take = queryInfo.Take,
                    AdditionalSearchString = yeuCau.Id.ToString() // yeuCauTraDuocPhamTuBenhNhanId
                };
                var dataThuocs = await _yeuCauTraThuocTuBenhNhanService.GetDataForGridAsyncDuocPhamChild(queryThuoc);
                yeuCau.TraDuocPhamChiTietVos = dataThuocs.Data.Cast<TraDuocPhamChiTietGridVo>().ToList();
                foreach (var thuoc in yeuCau.TraDuocPhamChiTietVos)
                {
                    var queryBenhNhan = new QueryInfo()
                    {
                        Skip = queryInfo.Skip,
                        Take = queryInfo.Take
                    };
                    queryBenhNhan.AdditionalSearchString = thuoc.YeuCauTraDuocPhamTuBenhNhanId + ";" + thuoc.DuocPhamBenhVienId;
                    var dataBenhNhans = await _yeuCauTraThuocTuBenhNhanService.GetDataForGridAsyncBenhNhanChild(queryBenhNhan);
                    thuoc.TraDuocPhamBenhNhanChiTietVos = dataBenhNhans.Data.Cast<TraDuocPhamBenhNhanChiTietGridVo>().ToList();
                }
            }
            var bytes = _excelService.ExportPhieuTraThuocTuBenhNhanNoiTru(yeuCauTraThuocTuBenhNhanGridVos, queryInfo.AdditionalSearchString);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSPhieuTraThuocTuBenhNhanNoiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");

        }
        #endregion

        [HttpPost("InPhieuYeuCauTraThuocTuBenhNhan")]
        public string InPhieuYeuCauTraThuocTuBenhNhan(PhieuTraThuoc phieuTraThuoc)
        {
            var result = _yeuCauTraThuocTuBenhNhanService.InPhieuYeuCauTraThuocTuBenhNhan(phieuTraThuoc);
            return result;
        }
    }
}