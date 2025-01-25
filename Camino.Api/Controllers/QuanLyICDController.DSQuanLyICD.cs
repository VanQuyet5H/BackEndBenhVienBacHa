using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using System.Linq;
using System;
using Camino.Services.Helpers;
using Camino.Api.Models.ICDs;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject.ICDs;
using Camino.Api.Models.Error;

namespace Camino.Api.Controllers
{
    public partial class QuanLyICDController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyICD)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _icdService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyICD)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _icdService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTenLoaiICD")]
        public async Task<ActionResult<ICollection<QuanLyICDTemplateVo>>> GetTenLoaiICDLookup(DropDownListRequestModel model)
        {
            var lookup = await _icdService.GetTenLoaiICD(model);
            return Ok(lookup);
        }

        [HttpPost("GetTenKhoa")]
        public async Task<ActionResult<ICollection<KhoaQuanLyICDTemplateVo>>> GetTenKhoaLookup(DropDownListRequestModel model)
        {
            var lookup = await _icdService.GetTenKhoa(model);
            return Ok(lookup);
        }


        #region CRUD
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuanLyICD)]
        public async Task<ActionResult> Post([FromBody] QuanLyICDViewModel quanLyICDViewModel)
        {
            var checkICDExist = await _icdService.CheckLoaiICDExist(quanLyICDViewModel.LoaiICDId.GetValueOrDefault());
            if (checkICDExist == false)
            {
                throw new ApiException(_localizationService.GetResource("ICD.TenLoaiICDNotExist"));
            }
            var checkKhoaExist = await _icdService.CheckKhoaExist(quanLyICDViewModel.KhoaId.GetValueOrDefault());
            if (checkKhoaExist == false)
            {
                throw new ApiException(_localizationService.GetResource("ICD.KhoaNotExist"));
            }
            var iCD = quanLyICDViewModel.ToEntity<ICD>();
            await _icdService.AddAsync(iCD);
            return CreatedAtAction(nameof(Get), new { id = iCD.Id }, iCD.ToModel<QuanLyICDViewModel>());
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuanLyICD)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<QuanLyICDViewModel>> Get(long id)
        {
            var iCD = await _icdService.GetByIdAsync(id, s => s.Include(o => o.Khoa)
                                                                  .Include(o => o.LoaiICD));
            if (iCD == null)
            {
                return NotFound();
            }
            return Ok(iCD.ToModel<QuanLyICDViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.QuanLyICD)]
        public async Task<ActionResult> Put([FromBody] QuanLyICDViewModel quanLyICDViewModel)
        {
            var iCD = await _icdService.GetByIdAsync(quanLyICDViewModel.Id, s => s.Include(o => o.Khoa)
                                                                  .Include(o => o.LoaiICD));

            var checkICDExist = await _icdService.CheckLoaiICDExist(quanLyICDViewModel.LoaiICDId.GetValueOrDefault());
            if (checkICDExist == false)
            {
                throw new ApiException(_localizationService.GetResource("ICD.TenLoaiICDNotExist"));
            }
            var checkKhoaExist = await _icdService.CheckKhoaExist(quanLyICDViewModel.KhoaId.GetValueOrDefault());
            if (checkKhoaExist == false)
            {
                throw new ApiException(_localizationService.GetResource("ICD.KhoaNotExist"));
            }
            if (iCD == null)
            {
                return NotFound();
            }
            quanLyICDViewModel.ToEntity(iCD);

            await _icdService.UpdateAsync(iCD);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.QuanLyICD)]
        public async Task<ActionResult> Delete(long id)
        {
            var iCD = await _icdService.GetByIdAsync(id, s => s.Include(o => o.Khoa)
                                                                  .Include(o => o.LoaiICD));
            if (iCD == null)
            {
                return NotFound();
            }
            await _icdService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("Deletes")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.QuanLyICD)]
        public async Task<ActionResult> Deletes([FromBody] DeletesViewModel model)
        {
            var iCDs = await _icdService.GetByIdsAsync(model.Ids);
            if (iCDs == null)
            {
                return NotFound();
            }
            if (iCDs.Count() != model.Ids.Length)
            {
                throw new ArgumentException(_localizationService
                    .GetResource("Common.WrongLengthMultiDelete"));
            }
            await _icdService.DeleteAsync(iCDs);
            return NoContent();
        }
        #endregion

        [HttpPost("KichHoatHieuLuc")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.QuanLyICD)]
        public async Task<ActionResult> KichHoatHieuLuc(long id)
        {
            var entity = await _icdService.GetByIdAsync(id);
            entity.HieuLuc = entity.HieuLuc == null ? true : !entity.HieuLuc;
            await _icdService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("ExportICD")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.QuanLyICD)]
        public async Task<ActionResult> ExportICD(QueryInfo queryInfo)
        {
            var gridData = await _icdService.GetDataForGridAsync(queryInfo, true);
            var chucVuData = gridData.Data.Select(p => (QuanLyICDGridVo)p).ToList();
            var excelData = chucVuData.Map<List<QuanLyICDExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(QuanLyICDExportExcel.Ma), "Mã"),
                (nameof(QuanLyICDExportExcel.MaChiTiet), "Mã Chi Tiết"),
                (nameof(QuanLyICDExportExcel.TenTiengViet), "Tên Tiếng Việt"),
                (nameof(QuanLyICDExportExcel.TenTiengAnh), "Tên Tiếng Anh"),
                (nameof(QuanLyICDExportExcel.TenLoaiICD), "Tên Loại ICD"),
                (nameof(QuanLyICDExportExcel.TenKhoa), "Tên Khoa"),
                (nameof(QuanLyICDExportExcel.GioiTinhDisplay), "Giới Tính"),
                (nameof(QuanLyICDExportExcel.ManTinh), "Mãn Tính"),
                (nameof(QuanLyICDExportExcel.BenhThuongGap), "Bệnh Thường Gặp"),
                (nameof(QuanLyICDExportExcel.BenhNam), "Bệnh Năm"),
                (nameof(QuanLyICDExportExcel.KhongBaoHiem), "Không Bảo Hiểm"),
                (nameof(QuanLyICDExportExcel.NgoaiDinhSuat), "Ngoài Định Suất"),
                (nameof(QuanLyICDExportExcel.ChuanDoanLamSan), "Chẩn Đoán Lâm Sàn"),
                (nameof(QuanLyICDExportExcel.ThongTinThamKhaoChoBenhNhan), "Thông Tin Tham Khảo Cho Người Bệnh"),
                (nameof(QuanLyICDExportExcel.TenGoiKhac), "Tên Gọi Khác"),
                (nameof(QuanLyICDExportExcel.LoiDanCuaBacSi), "Lời Dặn Bác Sĩ"),
                (nameof(QuanLyICDExportExcel.MoTa), "Mô Tả"),
                (nameof(QuanLyICDExportExcel.HieuLuc), "Hiệu Lực"),
            };

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Quản Lý ICD");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=QuanLyICD" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
