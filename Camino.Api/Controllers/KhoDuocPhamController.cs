using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.General;
using Camino.Api.Models.KhoDuocPhams;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoDuocPhams;
using Camino.Services.KhoNhanVienQuanLys;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class KhoDuocPhamController : CaminoBaseController
    {
        private readonly IKhoDuocPhamService _khoDuocPhamService;
        private readonly IKhoNhanVienQuanLyService _khoNvQuanLyService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public KhoDuocPhamController(IKhoDuocPhamService khoDuocPhamService, ILocalizationService localizationService, IExcelService excelService, IKhoNhanVienQuanLyService khoNvQuanLyService)
        {
            _khoDuocPhamService = khoDuocPhamService;
            _localizationService = localizationService;
            _excelService = excelService;
            _khoNvQuanLyService = khoNvQuanLyService;
        }
        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhMucKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoDuocPhamService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhMucKhoDuocPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoDuocPhamService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region CRUD
        [HttpGet("{id}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhMucKhoDuocPham)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KhoDuocPhamViewModel>> Get(long id)
        {
            //var result = await _khoNvQuanLyService.GetByIdAsync(id, e => e.Include(r => r.Kho).ThenInclude(r => r.PhongBenhVien)
            //    .Include(r => r.Kho).ThenInclude(r => r.KhoaPhong).Include(r => r.NhanVien.User));
            var result = await _khoDuocPhamService.GetByIdAsync(id, o => o.Include(p => p.KhoNhanVienQuanLys).Include(p => p.KhoaPhong).Include(p => p.PhongBenhVien));

            if (result == null)
            {
                return NotFound();
            }

            var resultData = result.ToModel<KhoDuocPhamViewModel>();
           
            return Ok(resultData);
        }

        [HttpPost]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.DanhMucKhoDuocPham)]
        public async Task<ActionResult<KhoDuocPhamViewModel>> Post([FromBody] KhoDuocPhamViewModel viewModel)
        {
            //TODO update entity kho on 9/9/2020
            //if(viewModel.LoaiKho == EnumLoaiKhoDuocPham.KhoTong) 
            //{
            //    if (await _khoDuocPhamService.CheckExistKhoTongAsync(viewModel.LoaiKho))
            //        throw new ApiException(_localizationService.GetResource("KhoDuocPham.Exists"), (int)HttpStatusCode.BadRequest);
            //}

            var entity = viewModel.ToEntity<Kho>();
            if (viewModel.NhanVienIds != null && viewModel.NhanVienIds.Any())
            {
                foreach (var nv in viewModel.NhanVienIds)
                {
                    var khoNvQuanLyEntity = new KhoNhanVienQuanLy
                    {
                        Id = 0,
                        KhoId = entity.Id,
                        NhanVienId = nv
                    };
                    entity.KhoNhanVienQuanLys.Add(khoNvQuanLyEntity);
                }
            }
            await _khoDuocPhamService.AddAsync(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToModel<KhoDuocPhamViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhMucKhoDuocPham)]
        public async Task<ActionResult> Put([FromBody] KhoDuocPhamViewModel viewModel)
        {
            var entity = await _khoDuocPhamService.GetByIdAsync(viewModel.Id, o => o.Include(p => p.KhoNhanVienQuanLys));

            if(entity == null)
            {
                return NotFound();
            }

            if(entity.IsDefault && (entity.LoaiKho != viewModel.LoaiKho || entity.KhoaPhongId != viewModel.KhoaPhongId || entity.PhongBenhVienId != viewModel.PhongBenhVienId))
            {
                throw new ApiException(_localizationService.GetResource("Kho.KhoDuocPhamVTYT.Update.NhanVien.Only"), (int)HttpStatusCode.BadRequest);
            }

            viewModel.ToEntity(entity);

            //Tạo nv mới
            var lstNhanVienQuanLy = new List<KhoNhanVienQuanLy>();
            foreach (var nv in viewModel.NhanVienIds)
            {
                if(!entity.KhoNhanVienQuanLys.Any(p => p.NhanVienId == nv))
                {
                    var khoNvQuanLyEntity = new KhoNhanVienQuanLy
                    {
                        Id = 0,
                        KhoId = entity.Id,
                        NhanVienId = nv
                    };
                    //entity.KhoNhanVienQuanLys.Add(khoNvQuanLyEntity);
                    lstNhanVienQuanLy.Add(khoNvQuanLyEntity);
                }
            }

            //Xoá nv
            foreach (var item in entity.KhoNhanVienQuanLys.ToList())
            {
                if(!viewModel.NhanVienIds.Any(p => p == item.NhanVienId))
                {
                    entity.KhoNhanVienQuanLys.Remove(item);
                }
            }

            //Thêm nv mới vào lsit
            foreach(var item in lstNhanVienQuanLy)
            {
                entity.KhoNhanVienQuanLys.Add(item);
            }
            
            await _khoDuocPhamService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhMucKhoDuocPham)]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _khoDuocPhamService.GetByIdAsync(id, o => o.Include(p => p.KhoNhanVienQuanLys));

            if(entity == null)
            {
                return NotFound();
            }

            if(entity.IsDefault)
            {
                throw new ApiException(_localizationService.GetResource("Kho.KhoDuocPhamVTYT.Delete.Forbidden"), (int)HttpStatusCode.BadRequest);
            }

            await _khoDuocPhamService.DeleteByIdAsync(id);

            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhMucKhoDuocPham)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var entities = await _khoDuocPhamService.GetByIdsAsync(model.Ids, o => o.Include(p => p.KhoNhanVienQuanLys));
            
            if(entities == null)
            {
                return NotFound();
            }

            if(entities.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService.GetResource("Common.WrongLengthMultiDelete"));
            }

            foreach(var entity in entities)
            {
                if (entity.IsDefault)
                {
                    throw new ApiException($"{_localizationService.GetResource("Kho.KhoDuocPhamVTYT.Delete.Forbidden")} ({entity.Ten})", (int)HttpStatusCode.BadRequest);
                }
            }

            await _khoDuocPhamService.DeleteAsync(entities);

            return NoContent();
        }


        #endregion
        #region GetListLookupItemVo
        [HttpPost("GetLoaiKhos")]

        public async Task<ActionResult<ICollection<LookupItemVo>>> GetLoaiKhos(DropDownListRequestModel model)
        {
            var lookup = await _khoDuocPhamService.GetLoaiKhos(model);
            return Ok(lookup);
        }

        [HttpPost("GetListKhoDuocPham")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoDuocPhamAsync(DropDownListRequestModel model)
        {
            var lookup = await _khoDuocPhamService.GetListKhoDuocPham(model);
            lookup.Add(new LookupItemVo { DisplayName = "Tất cả", KeyId = 0 });
            lookup.Reverse();
            return Ok(lookup);
        }
        [HttpPost("GetListKhoDuocPhamShare")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoDuocPhamShare(DropDownListRequestModel model)
        {
            var lookup = await _khoDuocPhamService.GetListKhoDuocPham(model);
            return Ok(lookup);
        }
        [HttpPost("GetListLoaiNhapKho")]
        public ActionResult<ICollection<LookupItemVo>> GetListLoaiNhapKho(DropDownListRequestModel model)
        {
            var listEnum = EnumHelper.GetListEnum<EnumLoaiNhapKho>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();
            // var resultList = result.Where(p => Regex.IsMatch(p.DisplayName.ToLower().ConvertToUnSign(), model.Query.Trim().ToLower().ConvertToUnSign() ?? "" + ".*[mn]")).ToList();
            result.Add(new LookupItemVo { DisplayName = "Tất cả", KeyId = 0 });
            result.Reverse();
            return Ok(result);
        }
        [HttpPost("GetListLoaiNhapKhoShare")]
        public ActionResult<ICollection<LookupItemVo>> GetListLoaiNhapKhoShare(DropDownListRequestModel model)
        {
            var listEnum = EnumHelper.GetListEnum<EnumLoaiNhapKho>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();
            // var resultList = result.Where(p => Regex.IsMatch(p.DisplayName.ToLower().ConvertToUnSign(), model.Query.Trim().ToLower().ConvertToUnSign() ?? "" + ".*[mn]")).ToList();
            return Ok(result);
        }

        #endregion

        [HttpPost("ExportKhoDuocPham")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhMucKhoDuocPham)]
        public async Task<ActionResult> ExportKhoDuocPham(QueryInfo queryInfo)
        {
            var gridData = await _khoDuocPhamService.GetDataForGridAsync(queryInfo, true);
            var khoDuocPhamData = gridData.Data.Select(p => (KhoDuocPhamGridVo)p).ToList();
            var excelData = khoDuocPhamData.Map<List<KhoDuocPhamExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(KhoDuocPhamExportExcel.Ten), "Tên kho"),
                (nameof(KhoDuocPhamExportExcel.TextLoaiKho), "Loại kho")
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Kho dược phẩm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KhoDuocPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}