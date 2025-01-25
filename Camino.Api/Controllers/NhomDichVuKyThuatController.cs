using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.NhomDichVuKyThuat;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.NhomDichVuKyThuat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class NhomDichVuKyThuatController : CaminoBaseController
    {
        private readonly INhomDichVuKyThuatService _nhomDichVuKyThuatService;
        public NhomDichVuKyThuatController(INhomDichVuKyThuatService nhomDichVuKyThuatService)
        {
            _nhomDichVuKyThuatService = nhomDichVuKyThuatService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTreeView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucNhomDichVuKyThuat)]
        public async Task<List<NhomDichVuKyThuatGridVo>> GetDataTreeView([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhomDichVuKyThuatService.GetDataTreeView(queryInfo);
            return gridData.ToList();
        }
    }
}