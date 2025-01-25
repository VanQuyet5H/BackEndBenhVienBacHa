using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.NhapKhoMarketing;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.ChucDanhs;
using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhapKhoMarketting;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class MarketingController
    {
        #region Ds nhập kho marketing
        [HttpPost("GetDSNhapKhoMarketingDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDSNhapKhoMarketingDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoQuaTangService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDSNhapKhoMarketingTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoMarketing)]
        public async Task<ActionResult<GridDataSource>> GetDSNhapKhoMarketingTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoQuaTangService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region CRUD
        [HttpGet("GetThongTinNhapKho")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.NhapKhoMarketing)]
        public async Task<ActionResult<ThongTinNhapKhoQuaTangGridVo>> GetThongTinNhapKho(long id)
        {
            var gridData = _nhapKhoQuaTangService.GetThongTinNhapKhoQuaTang(id);
            return Ok(gridData);
        }
    
        [HttpPost("GetNhanVienTaiBenhVien")]
        public async Task<ActionResult<ICollection<ChucDanhItemVo>>> GetNhanVienTaiBenhVien([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _nhapKhoQuaTangService.GetListNhanVienAsync(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetListQuaTang")]
        public async Task<ActionResult<ICollection<ChucDanhItemVo>>> GetListQuaTang([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _nhapKhoQuaTangService.GetListDanhSachQuaTangAsync(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetDonViTinhQuaTang")]
        public string GetDonViTinhQuaTang(long idQuaTang)
        {
            var lookup =  _nhapKhoQuaTangService.GetDonViTinhQuaTang(idQuaTang);
            return lookup;
        }
        [HttpPost("GetTenQuaTang")]
        public string GetTenQuaTang(long idQuaTang)
        {
            var lookup = _nhapKhoQuaTangService.GetTenQuaTang(idQuaTang);
            return lookup;
        }
        [HttpGet("GetThongTinNhanVien")]
        public LookupItemVo GetThongTinNhanVien(long nhanVienId)
        {
            var lookup = _nhapKhoQuaTangService.GetThongTinNhanVienLoginAsync(nhanVienId);
            return lookup;
        }
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.NhapKhoMarketing)]
        public async Task<ActionResult> Put([FromBody] NhapKhoMarketingViewModel nhapKhoMarketingViewModel)
        {
            var nhapKho = await _nhapKhoQuaTangService.GetByIdAsync(nhapKhoMarketingViewModel.Id,s=>s.Include(p=>p.NhapKhoQuaTangChiTiets));
            if (!nhapKhoMarketingViewModel.DanhSachQuaTangDuocThemList.Any())
            {
                throw new Models.Error.ApiException(_localizationService.GetResource("Marketing.DanhSachQuaTangDuocThemList.Required"));
            }
            if (nhapKho == null)
            {
                return NotFound();
            }
            nhapKhoMarketingViewModel.ToEntity(nhapKho);
            await _nhapKhoQuaTangService.UpdateAsync(nhapKho);
            return NoContent();
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.NhapKhoMarketing)]
        public async Task<ActionResult<NhapKhoMarketingViewModel>> Post([FromBody] NhapKhoMarketingViewModel nhapKhoMarketingViewModel)
        {
            if (!nhapKhoMarketingViewModel.DanhSachQuaTangDuocThemList.Any())
            {
                throw new Models.Error.ApiException(_localizationService.GetResource("Marketing.DanhSachQuaTangDuocThem.Required"));
            }
            var dataItem = nhapKhoMarketingViewModel.ToEntity<NhapKhoQuaTang>();
            _nhapKhoQuaTangService.Add(dataItem);
            return CreatedAtAction(nameof(Get), new { id = dataItem.Id }, dataItem.ToModel<NhapKhoMarketingViewModel>());
        }
        [HttpPost("XoaQuaTang")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.NhapKhoMarketing)]
        public async Task<ActionResult> Delete(long id)
        {
            var listHuy = await _nhapKhoQuaTangService.GetByIdAsync(id, s => s.Include(x => x.NhapKhoQuaTangChiTiets));
            if (listHuy == null)
            {
                return NotFound();
            }
            await _nhapKhoQuaTangService.DeleteByIdAsync(id);
            return NoContent();
        }
        [HttpPost("ValidateNhaCungCap")]
        public async Task<bool> ValidateNhaCungCap(string nhaCungCap,long quaTangId)
        {
            var result =await _nhapKhoQuaTangService.IsTenExists(nhaCungCap, quaTangId);
            return result;
        }
        [HttpPost("ValidateThongTinQuaTang")]
        public async Task<bool> ValidateThongTinQuaTang([FromBody] DanhSachQuaTangDuocThem nhapKhoMarketingViewModel)
        {
            return true;
        }
        #endregion
        #region export excel
        [HttpPost("ExportNhapKhoMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.NhapKhoMarketing)]
        public async Task<ActionResult> ExportNhapKhoMarketing(QueryInfo queryInfo)
        {
            var gridData = await _nhapKhoQuaTangService.GetDataForGridAsync(queryInfo, true);
            var result = gridData.Data.Select(p => (NhapKhoQuaTangMarketingGridVo)p).ToList();
            var excelData = result.Map<List<NhapKhoQuaTangMarketingExcel>>();

            var lstValueObject = new List<(string, string)>();
                lstValueObject.Add((nameof(NhapKhoQuaTangMarketingExcel.SoPhieu), "Số PN"));
                lstValueObject.Add((nameof(NhapKhoQuaTangMarketingExcel.SoChungTu), "Số Chứng Từ"));
                lstValueObject.Add((nameof(NhapKhoQuaTangMarketingExcel.LoaiNguoiGiaoString), "Loại Người Giao"));
                lstValueObject.Add((nameof(NhapKhoQuaTangMarketingExcel.TenNguoiGiao), "Tên Người Giao"));
                lstValueObject.Add((nameof(NhapKhoQuaTangMarketingExcel.NhanVienNhap), "Người Nhập"));
                lstValueObject.Add((nameof(NhapKhoQuaTangMarketingExcel.NgayNhapDisplay), "Ngày Nhập"));

                var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nhập Kho Marketing", 2, "Nhập Kho Marketing");

                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NhapKhoMarketing" + DateTime.Now.Year + ".xls");
                Response.ContentType = "application/vnd.ms-excel";
                return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
