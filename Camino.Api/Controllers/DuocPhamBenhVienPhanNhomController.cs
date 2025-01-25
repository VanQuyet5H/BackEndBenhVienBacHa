using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuocPhamBenhVienPhanNhoms;
using Camino.Services.DuocPhamBenhVienPhanNhoms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class DuocPhamBenhVienPhanNhomController : CaminoBaseController
    {
        private readonly IDuocPhamBenhVienPhanNhomService _dpbvPhanNhomService;

        public DuocPhamBenhVienPhanNhomController(IDuocPhamBenhVienPhanNhomService dpbvPhanNhomService)
        {
            _dpbvPhanNhomService = dpbvPhanNhomService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTreeView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuocPhamBenhVienPhanNhom)]
        public async Task<List<DuocPhamBenhVienPhanNhomGridVo>> GetDataTreeView([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dpbvPhanNhomService.GetDataTreeView(queryInfo);
            return gridData;
        }
        [HttpPost("GetListDuocPhamBenhVienPhanNhomCha")]
        public ActionResult<ICollection<DuocPhamBenhVienPhanNhomTemplateVo>> GetListDuocPhamBenhVienPhanNhomCha([FromBody]DropDownListRequestModel queryInfo, long id)
        {
            var lookup = _dpbvPhanNhomService.GetListDuocPhamBenhVienPhanNhomCha(queryInfo, id);
            return Ok(lookup);
        }

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDuocPhamBenhVienPhanNhom)]
        public async Task<ActionResult<DuocPhamBenhVienPhanNhomViewModel>> Post([FromBody] DuocPhamBenhVienPhanNhomViewModel duocPhamBenhVienPhanNhomViewModel)
        {
            var capNhom = 0;

            if (duocPhamBenhVienPhanNhomViewModel.NhomChaId != null)
            {
                capNhom = await _dpbvPhanNhomService.GetCapNhom(duocPhamBenhVienPhanNhomViewModel.NhomChaId) + 1;
            }

            duocPhamBenhVienPhanNhomViewModel.CapNhom = capNhom;
            var dpvbPhanNhomEntity = duocPhamBenhVienPhanNhomViewModel.ToEntity<DuocPhamBenhVienPhanNhom>();
            await _dpbvPhanNhomService.AddAsync(dpvbPhanNhomEntity);
            return NoContent();
        }

        [HttpGet("Get")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDuocPhamBenhVienPhanNhom)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuocPhamBenhVienPhanNhomViewModel>> Get(long id, bool createChild)
        {
            var result = await _dpbvPhanNhomService.GetByIdAsync(id, w => w.Include(e => e.NhomCha));
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<DuocPhamBenhVienPhanNhomViewModel>();
            resultData.TenCha = createChild ? result.Ten : result.NhomCha?.Ten;
            return Ok(resultData);
        }

        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDuocPhamBenhVienPhanNhom)]
        public async Task<ActionResult> Put([FromBody] DuocPhamBenhVienPhanNhomViewModel duocPhamBenhVienPhanNhomViewModel)
        {
            DuocPhamBenhVienPhanNhom duocPhamBenhVienPhanNhom;
            var capNhom = 0;

            if (duocPhamBenhVienPhanNhomViewModel.NhomChaId != null)
            {
                capNhom = await _dpbvPhanNhomService.GetCapNhom(duocPhamBenhVienPhanNhomViewModel.NhomChaId) + 1;
            }

            if (duocPhamBenhVienPhanNhomViewModel.Id == 0)
            {
                duocPhamBenhVienPhanNhom = new DuocPhamBenhVienPhanNhom
                {
                    Id = 0,
                    Ten = duocPhamBenhVienPhanNhomViewModel.Ten,
                    NhomChaId = duocPhamBenhVienPhanNhomViewModel.NhomChaId.GetValueOrDefault(),
                    CapNhom = capNhom
                };
            }
            else
            {
                duocPhamBenhVienPhanNhom = await _dpbvPhanNhomService.GetByIdAsync(duocPhamBenhVienPhanNhomViewModel.Id);
                if (duocPhamBenhVienPhanNhomViewModel.NhomChaId == duocPhamBenhVienPhanNhomViewModel.Id)
                {
                    duocPhamBenhVienPhanNhomViewModel.NhomChaId = null;
                }

                duocPhamBenhVienPhanNhomViewModel.ToEntity(duocPhamBenhVienPhanNhom);
                duocPhamBenhVienPhanNhom.CapNhom = capNhom;
            }

            await _dpbvPhanNhomService.UpdateAsync(duocPhamBenhVienPhanNhom);
            return NoContent();
        }

        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDuocPhamBenhVienPhanNhom)]
        public async Task<ActionResult> Delete(long id)
        {
            var duocPhamBenhVienPhanNhom = await _dpbvPhanNhomService.GetByIdAsync(id, w => w.Include(e => e.DuocPhamBenhVienPhanNhoms));
            if (duocPhamBenhVienPhanNhom != null)
            {
                var children = await _dpbvPhanNhomService.GetDataTreeViewChildren(id);
                if (children == null)
                {
                    await _dpbvPhanNhomService.DeleteByIdAsync(id);
                    return NoContent();
                }
                foreach (var child in children)
                {
                    child.WillDelete = true;
                }
                await _dpbvPhanNhomService.DeleteByIdAsync(id);
            }
            return NoContent();
        }
        #endregion
    }
}
