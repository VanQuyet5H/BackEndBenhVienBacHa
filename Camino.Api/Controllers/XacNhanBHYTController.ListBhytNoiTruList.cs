using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNoiTruAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNoiTruAsync
               ([FromBody]QueryInfo queryInfo)
        {
            var filter = new FilterDanhSachBHYTGridVo();

            if (queryInfo.AdditionalSearchString != null)
            {
                filter = JsonConvert.DeserializeObject<FilterDanhSachBHYTGridVo>(queryInfo.AdditionalSearchString);
            }

            if (filter.DaXacNhan == null)
            {
                filter = new FilterDanhSachBHYTGridVo
                {
                    DaXacNhan = true,
                    ChuaXacNhan = true
                };
            }

            if (filter.DaXacNhan == true && filter.ChuaXacNhan == false)
            {
                var gridData = await _xnBhytNoiTruListService.GetDataForDaXacNhanAsync(queryInfo);
                return Ok(gridData);
            }

            if (filter.DaXacNhan == false && filter.ChuaXacNhan == true)
            {
                var gridData = await _xnBhytNoiTruListService.GetDataForGridAsync(queryInfo);
                return Ok(gridData);
            }

            var gridDataAll = await _xnBhytNoiTruListService.GetDataForBothBhyt(queryInfo);
            return Ok(gridDataAll);
        }

        [HttpPost("GetTotalPageForGridNoiTruAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNoiTruAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var filter = new FilterDanhSachBHYTGridVo();

            if (queryInfo.AdditionalSearchString != null)
            {
                filter = JsonConvert.DeserializeObject<FilterDanhSachBHYTGridVo>(queryInfo.AdditionalSearchString);
            }

            if (filter.DaXacNhan == null)
            {
                filter = new FilterDanhSachBHYTGridVo
                {
                    DaXacNhan = true,
                    ChuaXacNhan = true
                };
            }

            if (filter.DaXacNhan == true && filter.ChuaXacNhan == false)
            {
                var gridData = await _xnBhytNoiTruListService.GetTotalPageForDaXacNhanAsync(queryInfo);
                return Ok(gridData);
            }

            if (filter.DaXacNhan == false && filter.ChuaXacNhan == true)
            {
                var gridData = await _xnBhytNoiTruListService.GetTotalPageForGridAsync(queryInfo);
                return Ok(gridData);
            }

            var gridDataAll = await _xnBhytNoiTruListService.GetTotalPageForBothBhyt(queryInfo);
            return Ok(gridDataAll);
        }
    }
}
