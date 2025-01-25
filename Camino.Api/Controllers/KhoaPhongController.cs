using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.KhoaPhong;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.KhoaPhong;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Models.Error;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhoaPhong;
using Camino.Services.BenhVien.Khoa;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public class KhoaPhongController : CaminoBaseController
    {
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly IExcelService _excelService;
        private readonly IKhoaService _khoaService;
        private readonly ILocalizationService _localizationService;

        public KhoaPhongController(IKhoaPhongService khoaPhongService, IKhoaService khoaService, IExcelService excelService, ILocalizationService localizationService)
        {
            _khoaPhongService = khoaPhongService;
            _khoaService = khoaService;
            _excelService = excelService;
            _localizationService = localizationService;
        }

        #region GetDataForGrid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoaPhong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoaPhongService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoaPhong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khoaPhongService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region GetListLookupItemVo
        [HttpPost("GetListKhoa")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoa([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khoaService.GetListKhoa(model);
            return Ok(lookup);
        }

        [HttpPost("GetListLoaiKhoaPhong")]
        public ActionResult<ICollection<Enums.EnumLoaiKhoaPhong>> GetListLoaiKhoaPhong([FromBody]LookupQueryInfo model)
        {
            var listEnum = _khoaPhongService.GetListLoaiKhoaPhong(model);
            return Ok(listEnum);
        }

        [HttpPost("GetListKieuKham")]
        public ActionResult<ICollection<Enums.EnumKieuKham>> GetListKieuKham([FromBody]LookupQueryInfo model)
        {
            var listEnum = _khoaPhongService.GetListKieuKham(model);
            return Ok(listEnum);
        }


        #endregion

        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucKhoaPhong)]
        public async Task<ActionResult<KhoaPhongViewModel>> Post
            ([FromBody]KhoaPhongViewModel khoaPhongViewModel)
        {
            var khoaPhong = khoaPhongViewModel.ToEntity<KhoaPhong>();
            await _khoaPhongService.AddAsync(khoaPhong);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucKhoaPhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KhoaPhongViewModel>> Get(long id)
        {
            var khoaPhong = await _khoaPhongService.GetByIdAsync(id, s => s.Include(r => r.KhoaPhongChuyenKhoas).ThenInclude(p => p.Khoa));

            if (khoaPhong == null)
            {
                return NotFound();
            }

            return Ok(khoaPhong.ToModel<KhoaPhongViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucKhoaPhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatKhoaPhong([FromBody]KhoaPhongViewModel khoaPhongViewModel)
        {
            var khoaPhong = await _khoaPhongService.GetByIdAsync(khoaPhongViewModel.Id, s => s.Include(r => r.KhoaPhongChuyenKhoas).ThenInclude(p => p.Khoa));

            khoaPhongViewModel.ToEntity(khoaPhong);
            await _khoaPhongService.UpdateAsync(khoaPhong);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucKhoaPhong)]
        public async Task<ActionResult> Delete(long id)
        {
            var khoaPhong = await _khoaPhongService.GetByIdAsync(id, s => s.Include(r => r.KhoaPhongChuyenKhoas).ThenInclude(p => p.Khoa));

            if (khoaPhong == null)
            {
                return NotFound();
            }

            if (khoaPhong.IsDefault)
            {
                throw new ApiException(_localizationService.GetResource("KhoaPhong.IsDefault.Delete"), (int)HttpStatusCode.BadRequest);
            }

            await _khoaPhongService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucKhoaPhong)]
        public async Task<ActionResult> Deletes([FromBody]DeletesViewModel model)
        {
            var khoaPhongs = await _khoaPhongService.GetByIdsAsync(model.Ids, s => s.Include(r => r.KhoaPhongChuyenKhoas).ThenInclude(p => p.Khoa));
            if (khoaPhongs == null)
            {
                return NotFound();
            }

            var listKhoaPhongs = khoaPhongs.ToList();

            foreach (var khoaPhong in listKhoaPhongs)
            {
                if (khoaPhong.IsDefault)
                {
                    throw new ApiException(_localizationService.GetResource("KhoaPhongs.IsDefault.Delete"), (int)HttpStatusCode.BadRequest);
                }
            }

            if (listKhoaPhongs.Count != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }

            await _khoaPhongService.DeleteAsync(listKhoaPhongs);
            return NoContent();
        }

        [HttpPost("KichHoatKhoaPhong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucKhoaPhong)]
        public async Task<ActionResult> KichHoatKhoaPhong(long id)
        {
            var entity = await _khoaPhongService.GetByIdAsync(id);
            entity.IsDisabled = entity.IsDisabled == null ? true : !entity.IsDisabled;
            await _khoaPhongService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ThayDoiKieuKham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucKhoaPhong)]
        public async Task<ActionResult> ThayDoiKieuKham(long id)
        {
            var entity = await _khoaPhongService.GetByIdAsync(id);
            entity.CoKhamNgoaiTru = entity.CoKhamNgoaiTru == null ? true : !entity.CoKhamNgoaiTru;
            await _khoaPhongService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportKhoaPhong")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucKhoaPhong)]
        public async Task<ActionResult> ExportKhoaPhong(QueryInfo queryInfo)
        {
            var gridData = await _khoaPhongService.GetDataForGridAsync(queryInfo, true);
            var khoaPhongData = gridData.Data.Select(p => (KhoaPhongGridVo)p).ToList();
            var dataExcel = khoaPhongData.Map<List<KhoaPhongExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(KhoaPhongExportExcel.Ma), "Mã"),
                (nameof(KhoaPhongExportExcel.Ten), "Tên Khoa Phòng"),
                (nameof(KhoaPhongExportExcel.TenKhoa), "Tên Chuyên Khoa"),
                (nameof(KhoaPhongExportExcel.SoTienThuTamUng), "Số Tiền Thu Tạm Ứng"),
                (nameof(KhoaPhongExportExcel.CoKhamNgoaiTru), "Kiểu Khám"),
                (nameof(KhoaPhongExportExcel.IsDisabled), "Trạng Thái"),
                (nameof(KhoaPhongExportExcel.TenLoaiKhoaPhongDisplayName), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Khoa Phòng");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KhoaPhong" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
