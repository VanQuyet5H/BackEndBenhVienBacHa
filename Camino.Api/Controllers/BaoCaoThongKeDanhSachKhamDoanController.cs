using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.ThongKeDanhSachKhamDoan;
using Camino.Services.BaoCaoVatTus;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoThongKeDanhSachKhamDoanController : ControllerBase
    {
        private readonly IThongKeDanhSachKhamDoan _thongKeDanhSachKhamDoanService;
        public BaoCaoThongKeDanhSachKhamDoanController(IThongKeDanhSachKhamDoan thongKeDanhSachKhamDoanService)
        {
            _thongKeDanhSachKhamDoanService = thongKeDanhSachKhamDoanService;
        }

        [HttpPost("GetCongTyKhamSucKhoe")]
        public ActionResult<ICollection<LookupItemVo>> GetCongTyKhamSucKhoe([FromBody] LookupQueryInfo queryInfo)
        {
            var result = _thongKeDanhSachKhamDoanService.GetCongTyKhamSucKhoe(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetSoHopDongTheoCongTy")]
        public ActionResult<ICollection<LookupItemVo>> GetSoHopDongTheoCongTy([FromBody] LookupQueryInfo queryInfo, long? congTyKhamSucKhoeId)
        {
            var result = _thongKeDanhSachKhamDoanService.GetSoHopDongTheoCongTy(queryInfo ,  congTyKhamSucKhoeId);
            return Ok(result);
        }



        [HttpPost("GetDataThongKeDichVuKhamSucKhoeForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThongKeDichVuKhamSucKhoe)]
        public async Task<ActionResult> GetDataThongKeDichVuKhamSucKhoeForGrid(QueryInfo queryInfo)
        {
            var gridData = await _thongKeDanhSachKhamDoanService.GetDataThongKeDichVuKhamSucKhoe(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalThongKeDichVuKhamSucKhoePageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThongKeDichVuKhamSucKhoe)]
        public async Task<ActionResult<GridDataSource>> GetTotalThongKeDichVuKhamSucKhoePageForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _thongKeDanhSachKhamDoanService.GetDataTotalThongKeDichVuKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportThongKeDichVuKhamSucKhoe")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.ThongKeDichVuKhamSucKhoe)]
        public async Task<ActionResult> ExportThongKeDichVuKhamSucKhoe(QueryInfo queryInfo)
        {
            var gridData = await _thongKeDanhSachKhamDoanService.GetDataThongKeDichVuKhamSucKhoe(queryInfo, true);
            var data = gridData.Data.Select(p => (ThongKeDanhSachKhamDoanVo)p).ToList();

            var bytes = _thongKeDanhSachKhamDoanService.ExportBaoCaoThongKeDichVuKhamSucKhoe(queryInfo, data);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ThongKeDichVuKhamSucKhoe" + DateTime.Now.Year + ".xls");

            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
