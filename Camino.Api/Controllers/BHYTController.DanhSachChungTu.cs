using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.BHYT;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Camino.Services.BHYT;
using Camino.Core.Domain.ValueObject.BHYT;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Text;
using Camino.Api.Models.HamGuiHoSoWatchings;
using Camino.Services.HamGuiHoSoWatchings;
using Camino.Core.Domain.Entities.HamGuiHoSoWatchings;
using System.Xml.Linq;
using Camino.Services.GoiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.GoiBaoHiemYTe;
using System.Globalization;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using Camino.Api.Models.GiayNghiDuongThaiNoiTru;

namespace Camino.Api.Controllers
{
    public partial class BHYTController : CaminoBaseController
    {
        [HttpPost("GetLoaiChungTuXuatExcels")]
        public List<LookupItemVo> GetLoaiChungTuXuatExcels()
        {
            var loaiChungTuXuatExcels = EnumHelper.GetListEnum<LoaiChungTuXuatExcel>();
            var ketQuas = loaiChungTuXuatExcels.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return ketQuas;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDanhSachXuatChungTuExcelForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachXuatChungTuExcel)]
        public ActionResult<GridDataSource> GetDataDanhSachXuatChungTuExcelForGrid([FromBody]QueryInfo queryInfo)
        {
            var gridData = _goiBaoHiemYTeService.GetDataDanhSachXuatChungTuExcelForGrid(queryInfo);
            return Ok(gridData);
        }
       
     
        #region get Id nội trú hồ sơ khac theo phiếu

        [HttpPost("GetIdPhieuNoiTruHoSoKhac")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachXuatChungTuExcel)]
        public ActionResult<GridDataSource> GetIdPhieuNoiTruHoSoKhac(long id, Enums.LoaiHoSoDieuTriNoiTru loai)
        {
            var gridData = _goiBaoHiemYTeService.GetIdPhieuNoiTruHoSoKhac(id, loai);
            return Ok(gridData);
        }
        #endregion get Id nội trú hồ sơ khac theo phiếu
    }
}
