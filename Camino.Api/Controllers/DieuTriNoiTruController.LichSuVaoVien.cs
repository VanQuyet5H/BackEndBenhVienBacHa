using Camino.Api.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachLichSuVaoVienForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<GridDataSource> GetDanhSachLichSuVaoVienForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetDanhSachLichSuVaoVienForGrid(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachLichSuVaoVienForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<GridDataSource> GetTotalPagesDanhSachLichSuVaoVienForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetTotalPagesDanhSachLichSuVaoVienForGrid(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTenDichVuKhamBenh")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetTenDichVuKhamBenh(DropDownListRequestModel model, long yeuCauTiepNhanId)
        {
            var lookup = await _dieuTriNoiTruService.GetTenDichVuKhamBenh(model, yeuCauTiepNhanId);
            return Ok(lookup);
        }

        [HttpGet("GetThongTinTheoKhamBenh")]
        public ActionResult<ThongTinTheoKhamBenh> GetThongTinTheoKhamBenh(long khamBenhId, long yeuCauTiepNhanId)
        {
            var getThongTinTheoKhamBenh = _dieuTriNoiTruService.GetThongTinTheoKhamBenh(khamBenhId, yeuCauTiepNhanId);
            return Ok(getThongTinTheoKhamBenh);
        }

        [HttpGet("ThongTinLichSuKhamBenhNoiTru")]
        public ActionResult<ThongTinTheoKhamBenh> ThongTinLichSuKhamBenhNoiTru(long khamBenhId, long yeuCauTiepNhanId)
        {
            var getThongTinTheoKhamBenh = _dieuTriNoiTruService.ThongTinLichSuKhamBenhNoiTru(khamBenhId, yeuCauTiepNhanId);
            return Ok(getThongTinTheoKhamBenh);
        }
    }
}
