using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TuDienKyThuat;
using Camino.Services.DichVuKyThuatBenhVien;
using Camino.Services.DichVuXetNghiem;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Camino.Api.Controllers
{

    public class TuDienDichVuKyThuatController : CaminoBaseController
    {
        private readonly ITuDienDichVuKyThuatService _dichVuKyThuatService;


        public TuDienDichVuKyThuatController(ITuDienDichVuKyThuatService dichVuKyThuatService,
                                   IDichVuKyThuatBenhVienService dichVuKyThuatBenhVienService)
        {
            _dichVuKyThuatService = dichVuKyThuatService;
        }

        #region Ds TreeView And Search

        [HttpPost("GetDataTreeView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TuDienDichVuKyThuat)]
        public List<TuDienKyThuatGridVo> GetDataTreeView([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dichVuKyThuatService.GetDataTreeView(queryInfo);
            return gridData.ToList();
        }

        [HttpPost("SearchDichVuKyThuatBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TuDienDichVuKyThuat)]
        public List<TuDienKyThuatGridVo> SearchDichVuKyThuatBenhVien([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dichVuKyThuatService.SearchDichVuKyThuatBenhVien(queryInfo);
            return gridData.ToList();
        }
        

        [HttpPost("GetDichVuKyThuats")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TuDienDichVuKyThuat)]
        public TuDienKyThuatGridVo GetDichVuKyThuats(long dichVuKyThuatBenhVienId)
        {
            return _dichVuKyThuatService.GetDichVuKyThuats(dichVuKyThuatBenhVienId);
        }

        [HttpPost("LuuDichVukyThuatBenhVienMauKetQua")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TuDienDichVuKyThuat)]
        public ActionResult LuuDichVukyThuatBenhVienMauKetQua([FromBody]TuDienKyThuatGridVo TuDienKyThuatGridVo)
        {
            _dichVuKyThuatService.LuuDichVukyThuatBenhVienMauKetQua(TuDienKyThuatGridVo);
            return Ok();
        }

        #endregion

    }
}