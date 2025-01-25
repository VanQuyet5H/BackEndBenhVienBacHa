using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetDataForGridKetQuaCLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetDataForGridKetQuaCLS([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetDataNoiTruKetQuaCDHATDCN(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPageForGridKetQuaCLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetTotalPageForGridKetQuaCLS([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetTotalNoiTruKetQuaCDHATDCN(queryInfo);
            return gridData;
        }

        [HttpGet("GetDataForGridXetNghiemKetQuaCLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetDataForGridXetNghiemKetQuaCLS(long yeuCauTiepNhanId , long phieuDieuTriHienTaiId)
        {
            var gridData = _dieuTriNoiTruService.GetDataNoiTruKetQuaXetNghiem(yeuCauTiepNhanId , phieuDieuTriHienTaiId);            
            return gridData;
        }

        #region BVHD-3575
        [HttpPost("GetDataForGridLichSuKham")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLichSuKhamAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridLichSuKhamAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridLichSuKham")]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLichSuKhamAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridLichSuKhamAsync(queryInfo);
            return Ok(gridData);
        }


        #endregion
    }
}
