using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Services.Thuocs;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Services.Localization;
using System.Linq;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Api.Controllers
{
    public class NhomThuocController : CaminoBaseController
    {
        private readonly INhomThuocService _nhomThuocService;
        private readonly ILocalizationService _localizationService;
        public NhomThuocController(INhomThuocService nhomThuocService, ILocalizationService localizationService)
        {
            _nhomThuocService = nhomThuocService;
            _localizationService = localizationService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTreeView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomThuoc)]
        public async Task<List<NhomThuocGridVo>> GetDataTreeView([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhomThuocService.GetDataTreeView(queryInfo);
            return gridData.ToList();
        }
    }
}