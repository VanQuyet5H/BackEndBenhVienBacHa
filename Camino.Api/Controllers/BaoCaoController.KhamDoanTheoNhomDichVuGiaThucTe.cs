using System;
using System.Linq;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDoanhThuKhamDoanTheoNhomDichVu;
using Camino.Services.BaoCaoKhamDoanHopDong;
using Microsoft.AspNetCore.Mvc;


namespace Camino.Api.Controllers
{
    public partial class BaoCaoController
    {
        [HttpPost("ExportKhamDoanTheoNhomDichVuGiaThucTe")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BCDTKhamDoanTheoNhomDVDGThucTe)]
        public ActionResult ExportKhamDoanTheoNhomDichVuGiaThucTe(BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeQueryInfo queryInfo)
        {
            var gridData = _baoCaoKhamDoanTheoGiaThucTeServices.BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTe(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoKhamDoanTheoGiaThucTeServices.ExportBaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTe(gridData.Data.Cast<BaoCaoDoanhThuKhamDoanTheoNhomDichVuThucTeGridVo>().ToList(), queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DoanhThuKhamDoanTheoNhomDichVuThucTe" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
