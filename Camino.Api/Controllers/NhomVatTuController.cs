using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain;
using Camino.Services.Localization;
using System.Linq;
using Camino.Services.NhomVatTu;
using Camino.Core.Domain.Entities.NhomVatTus;
using Camino.Core.Domain.ValueObject.NhomVatTu;

namespace Camino.Api.Controllers
{
    public class NhomVatTuController : CaminoBaseController
    {
        private readonly INhomVatTuService _nhomVatTuService;
        private readonly ILocalizationService _localizationService;
        public NhomVatTuController(INhomVatTuService nhomVatTuService, ILocalizationService localizationService, IJwtFactory iJwtFactory)
        {
            _nhomVatTuService = nhomVatTuService;
            _localizationService = localizationService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTreeView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomVatTuYTe)]
        public async Task<List<NhomVatTuGridVo>> GetDataTreeView([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhomVatTuService.GetDataTreeView(queryInfo);
            return gridData.ToList();
        }

        [HttpPost("GetTreeTemp")] //todo: cần xóa bỏ
        public async Task<ActionResult<ICollection<LookupTreeItemVo>>> GetTreeTemp(DropDownListRequestModel model)
        {
            var lookup = await _nhomVatTuService.GetTreeTemp(model);
            return Ok(lookup);
        }
    }
}