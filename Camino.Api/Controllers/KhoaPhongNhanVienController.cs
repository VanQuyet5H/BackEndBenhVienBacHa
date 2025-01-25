using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.KhoaPhongNhanVien;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhoaPhongNhanViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoaPhongNhanVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoaPhong;
using Camino.Services.KhoaPhongNhanVien;
using Camino.Services.Localization;
using Camino.Services.NhanVien;
using Camino.Services.PhongBenhVien;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class KhoaPhongNhanVienController : CaminoBaseController
    {
        private readonly IKhoaPhongNhanVienService _khoaPhongNhanVienService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly IExcelService _excelService;
        private readonly INhanVienService _nhanVienService;
        private readonly ILocalizationService _localizationService;
        private readonly IPhongBenhVienService _phongBenhVienService;

        public KhoaPhongNhanVienController(
            IKhoaPhongNhanVienService khoaPhongNhanVienService,
            IKhoaPhongService khoaPhongService,
            IExcelService excelService,
            INhanVienService nhanVienService,
            ILocalizationService localizationService,
            IPhongBenhVienService phongBenhVienService
            )
        {
            _khoaPhongNhanVienService = khoaPhongNhanVienService;
            _excelService = excelService;
            _khoaPhongService = khoaPhongService;
            _nhanVienService = nhanVienService;
            _localizationService = localizationService;
            _phongBenhVienService = phongBenhVienService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoaPhongNhanVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoaPhongNhanVienService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoaPhongNhanVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoaPhongNhanVienService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region GetListLookupItemVo
        [HttpPost("GetListKhoaPhong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoaPhong([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khoaPhongService.GetListKhoaPhong(model);
            return Ok(lookup);
        }

        [HttpPost("GetListNhanVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhanVien([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _nhanVienService.GetListNhanVien(model);
            return Ok(lookup);
        }

        /// <summary>
        /// get danh sách phòng khoa phòng trong tài chính kế toán
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetListKhoaPhongThuNgan")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoaPhongThuNgan([FromBody]DropDownListRequestModel model)
        {
            LookupItemVo lookupItemVo = new LookupItemVo { KeyId = 0, DisplayName = "Tất cả các quầy" };
            var lookup = await _khoaPhongService.GetListKhoaPhongThuNgan(model);
            lookup.Add(lookupItemVo);
            return Ok(lookup.OrderBy(cc=>cc.KeyId));
        }


        [HttpPost("GetListNhanVienThuNgan")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhanVienThuNgan([FromBody]DropDownListRequestModel model)
        {
            LookupItemVo lookupItemVo = new LookupItemVo { KeyId = 0, DisplayName = "Tất cả nhân viên" };
            var lookup = await _nhanVienService.GetListNhanVien(model);
            lookup.Add(lookupItemVo);
            return Ok(lookup.OrderBy(cc => cc.KeyId));
        }

        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucKhoaPhongNhanVien)]
        public async Task<ActionResult<KhoaPhongNhanVienMultiViewModel>> Post
            ([FromBody]KhoaPhongNhanVienMultiViewModel khoaPhongNhanVienMultiViewModel)
        {
            foreach (var nhanVienId in khoaPhongNhanVienMultiViewModel.NhanVienIds)
            {
                var khoaPhongNhanVien = new KhoaPhongNhanVienViewModel
                {
                    KhoaPhongId = khoaPhongNhanVienMultiViewModel.KhoaPhongId,
                    NhanVienId = nhanVienId
                };

                var khoaPhongNhanVienModel = new KhoaPhongNhanVien
                {
                    KhoaPhongId = khoaPhongNhanVien.KhoaPhongId,
                    NhanVienId = khoaPhongNhanVien.NhanVienId
                };

                await _khoaPhongNhanVienService.AddAsync(khoaPhongNhanVienModel);
            }
            return Ok();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoaPhongNhanVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KhoaPhongNhanVienViewModel>> Get(long id)
        {
            var khoaPhongNhanVien = await _khoaPhongNhanVienService.GetByIdAsync(
                id,
                s => s.Include(k => k.KhoaPhong)
                    .Include(k => k.NhanVien).ThenInclude(k => k.User)
                );

            if (khoaPhongNhanVien == null)
            {
                return NotFound();
            }

            return Ok(khoaPhongNhanVien.ToModel<KhoaPhongNhanVienViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucKhoaPhongNhanVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatKhoaPhongNhanVien([FromBody]KhoaPhongNhanVienViewModel khoaPhongNhanVienViewModel)
        {
            var khoaPhongNhanVien = await _khoaPhongNhanVienService.GetByIdAsync(khoaPhongNhanVienViewModel.Id);
            khoaPhongNhanVienViewModel.ToEntity(khoaPhongNhanVien);
            await _khoaPhongNhanVienService.UpdateAsync(khoaPhongNhanVien);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucKhoaPhongNhanVien)]
        public async Task<ActionResult> Delete(long id)
        {
            var khoaPhongNhanVien = await _khoaPhongNhanVienService.GetByIdAsync(id);
            if (khoaPhongNhanVien == null)
            {
                return NotFound();
            }

            await _khoaPhongNhanVienService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucKhoaPhongNhanVien)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var khoaPhongNhanViens = await _khoaPhongNhanVienService.GetByIdsAsync(model.Ids);
            if (khoaPhongNhanViens == null)
            {
                return NotFound();
            }
            var listKhoaPhongNhanViens = khoaPhongNhanViens.ToList();
            if (listKhoaPhongNhanViens.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _khoaPhongNhanVienService.DeleteAsync(listKhoaPhongNhanViens);
            return NoContent();
        }

        [HttpPost("ExportKhoaPhongNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucKhoaPhongNhanVien)]
        public async Task<ActionResult> ExportKhoaPhongNhanVien(QueryInfo queryInfo)
        {
            var gridData = await _khoaPhongNhanVienService.GetDataForGridAsync(queryInfo, true);
            var khoaPhongNhanVienData = gridData.Data.Select(p => (KhoaPhongNhanVienGridVo)p).ToList();
            var dataExcel = khoaPhongNhanVienData.Map<List<KhoaPhongNhanVienExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(KhoaPhongNhanVienExportExcel.TenNhanVien), "Tên Nhân Viên"),
                (nameof(KhoaPhongNhanVienExportExcel.TenKhoaPhong), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Khoa Phòng Nhân Viên");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KhoaPhongNhanVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}