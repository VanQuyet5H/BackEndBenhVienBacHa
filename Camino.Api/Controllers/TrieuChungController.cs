using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.ChucDanh;
using Camino.Api.Models.TrieuChung;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.TrieuChungs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TrieuChungs;
using Camino.Services.TrieuChung;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
   
    public class TrieuChungController : CaminoBaseController
    {
        private readonly ITrieuChungService _trieuChungService;

        public TrieuChungController(ITrieuChungService trieuChungService)
        {
            _trieuChungService = trieuChungService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTreeView")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucTrieuChung)]
        public async Task<List<TrieuChungGridVo>> GetDataTreeView([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _trieuChungService.GetDataTreeView(queryInfo);
            return gridData.ToList();
        }
        [HttpPost("GetListTrieuChungCha")]
        public async Task<ActionResult<ICollection<LookupItemTemplate>>> GetListTrieuChungCha([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _trieuChungService.GetListTrieuChungCha(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetListTrieuChungCha1")]
        public async Task<ActionResult<ICollection<LookupItemTemplate>>> GetListTrieuChungCha1([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _trieuChungService.GetListTrieuChungCha1(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetListDanhMucChuanDoan")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListDanhMucChuanDoan([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _trieuChungService.GetListDanhMucChuanDoan(queryInfo);
            return Ok(lookup);
        }
        #region CRUD
        /// <summary>
        ///     Add can ho
        /// </summary>
        /// <param name="trieuChungViewModel"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucTrieuChung)]
        public async Task<ActionResult<TrieuChungViewModel>> Post([FromBody] TrieuChungViewModel trieuChungViewModel)
        {
            if (trieuChungViewModel.TrieuChungChaId == null)
            {
                trieuChungViewModel.TrieuChungChaId = 0;
                trieuChungViewModel.CapNhom = 0; 
            }
            if(trieuChungViewModel.TrieuChungChaId != null)
            {
                var capNhomCha = await _trieuChungService.GetCapNhom(trieuChungViewModel.TrieuChungChaId);
                foreach(var item in capNhomCha)
                {
                    trieuChungViewModel.CapNhom = item.CapNhom;
                }
            }
            trieuChungViewModel.CapNhom = (int)trieuChungViewModel.CapNhom + 1 ;
                var user = trieuChungViewModel.ToEntity<TrieuChung>();
            await _trieuChungService.AddAsync(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<TrieuChungViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucTrieuChung)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<TrieuChungViewModel>> Get(long id)
        {
            var result = await _trieuChungService.GetByIdAsync(id, s => s.Include(g => g.TrieuChungDanhMucChuanDoans).ThenInclude(k => k.DanhMucChuanDoan).Include(g => g.TrieuChungCha));
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<TrieuChungViewModel>();
            return Ok(resultData);
        }
        //Update
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucTrieuChung)]
        public async Task<ActionResult> Put([FromBody] TrieuChungViewModel trieuChungViewModel)
        {
            var getTrieuChung = new TrieuChung();
            var trieuChung = await _trieuChungService.GetByIdAsync(trieuChungViewModel.Id, s => s.Include(q => q.TrieuChungDanhMucChuanDoans).ThenInclude(k => k.DanhMucChuanDoan));
            if (trieuChungViewModel.TrieuChungChaId == null)
            {
                trieuChungViewModel.CapNhom = 1;
            }
            else
            {
                getTrieuChung = await _trieuChungService.GetByIdAsync(trieuChungViewModel.TrieuChungChaId.Value, s => s.Include(q => q.TrieuChungDanhMucChuanDoans).ThenInclude(k => k.DanhMucChuanDoan));
                trieuChungViewModel.CapNhom = getTrieuChung.CapNhom + 1;
            }
            if (trieuChungViewModel.TrieuChungChaId == trieuChungViewModel.Id)
            {
                trieuChungViewModel.TrieuChungChaId = null;
            }
            trieuChungViewModel.ToEntity(trieuChung);
            foreach (var item in trieuChung.TrieuChungs)
            {
                item.CapNhom = trieuChung.CapNhom + 1;
            }
            await _trieuChungService.UpdateAsync(trieuChung);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucTrieuChung)]
        public async Task<ActionResult> Delete(long id)
        {
            var trieuChung = await _trieuChungService.GetByIdAsync(id, s => s.Include(q => q.TrieuChungDanhMucChuanDoans).ThenInclude(k => k.DanhMucChuanDoan));
            if (trieuChung != null)
            {
                var gridData = await _trieuChungService.GetDataTreeViewChildren(id);
                foreach (var model in gridData)
                {
                    var data = await _trieuChungService.FindChildren(model.KeyId);
                    foreach (var item in data)
                    {
                        item.WillDelete = true;
                    }
                    //_trieuChungService.Delete(data);
                }
                await _trieuChungService.DeleteByIdAsync(id);
            }
            return NoContent();
        }
    }
        #endregion
}