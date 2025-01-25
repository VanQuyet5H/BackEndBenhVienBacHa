using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.BaoCao;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoKetQuaKhamChuaBenh;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoThongKeDonThuoc;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoVienPhiThuTien;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Services.BaoCao;
using Camino.Services.BaoCaoTheKho;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.PhongBenhVien;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace Camino.Api.Controllers
{
    public class BaoCaoTheKhoController : CaminoBaseController
    {
        private readonly IBaoCaoTheKhoService _baoCaoTheKhoService;
        private readonly IExcelService _excelService;
        private readonly IPhongBenhVienService _phongBenhVienService;

        public BaoCaoTheKhoController(
            IExcelService excelService,
            IPhongBenhVienService phongBenhVienService,
            IBaoCaoTheKhoService baoCaoTheKhoService
        )
        {
            _excelService = excelService;
            _phongBenhVienService = phongBenhVienService;
            _baoCaoTheKhoService = baoCaoTheKhoService;
        }

        [HttpPost("GetTatCaKho")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTatCaKho(DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoTheKhoService.GetTatCaKho(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDuocPhamTheoKhoBaoCao")]
        public async Task<ActionResult<ICollection<DuocPhamTheoKhoBaoCaoLookup>>> GetDuocPhamTheoKhoBaoCao(DropDownListRequestModel model)
        {
            var lookup = await _baoCaoTheKhoService.GetDuocPhamTheoKho(model);
            return Ok(lookup);
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTheKho)]
        public async Task<ActionResult> GetDataForGridAsync(BaoCaoTheKhoQueryInfo queryInfo)
        {
            var grid = await _baoCaoTheKhoService.GetDataForGridAsync(queryInfo);
            return Ok(grid);
        }
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTheKho)]
        public async Task<ActionResult> GetDataForGridChildAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoTheKhoService.GetDataForGridChildAsync(queryInfo);
            return Ok(grid);
        }

        [HttpPost("ExportTheKho")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTheKho)]
        public async Task<ActionResult> ExportTheKho(BaoCaoTheKhoQueryInfo queryInfo)
        {
            var bytes = _baoCaoTheKhoService.ExportTheKho(queryInfo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoXuatNhapTon" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
