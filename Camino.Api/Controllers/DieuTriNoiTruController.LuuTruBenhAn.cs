using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetDataForGridAsyncLuuTruHoSo")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LuuTruHoSo)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncLuuTruHoSo([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridAsyncLuuTruHoSo(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetTotalPageForGridAsyncLuuTruHoSo")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LuuTruHoSo)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncLuuTruHoSo([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridAsyncLuuTruHoSo(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetThongTiLuuTruBenhAnNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LuuTruHoSo)]
        public async Task<ActionResult<ThongTiLuuTruBenhAnNoiTru>> ThongTiLuuTruBenhAnNoiTru(long id)
        {
            var model = _dieuTriNoiTruService.ThongTiLuuTruBenhAnNoiTru(id);

            #region BVHD-3941

            if (model != null && model.CoBaoHiemTuNhan == true)
            {
                model.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(model.YeuCauTiepNhanId ?? 0);
            }
            #endregion

            return Ok(model);
        }

        [HttpPost("CapNhatLuuTruBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LuuTruHoSo)]
        public async Task<ActionResult> CapNhatLuuTruBenhAn(LuuTruBenhAnViewModel model)
        {
            var benhAn = _noiTruBenhAnService.GetById(model.NoiTruBenhAnId);
            benhAn.ThuTuSapXepLuuTru = model.ThuTuSapXepLuuTru;
            if (!string.IsNullOrEmpty(model.ThuTuSapXepLuuTru))
            {
                benhAn.NgayLuuTru = DateTime.Now;
                benhAn.NhanVienLuuTruId = _userAgentHelper.GetCurrentUserId();
            }
            else
            {
                benhAn.NgayLuuTru = null;
                benhAn.NhanVienLuuTruId = null;
            }
            await _noiTruBenhAnService.UpdateAsync(benhAn);
            return NoContent();
        }

        [HttpPost("ExportLuuTruBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LuuTruHoSo)]
        public async Task<ActionResult> ExportDanhSachTiepNhan(QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridAsyncLuuTruHoSo(queryInfo, true);
            var benhAnData = gridData.Data.Select(p => (LuuTruHoSoGridVo)p).ToList();
            //var excelData = benhAnData.Map<List<LuuTruHoSoExportExcel>>();
            //var lstValueObject = new List<(string, string)>
            //{
            //    (nameof(LuuTruHoSoExportExcel.MaTN), "Mã TN"),
            //    (nameof(LuuTruHoSoExportExcel.SoBA), "Số BA"),
            //    (nameof(LuuTruHoSoExportExcel.MaBN), "Mã BN"),
            //    (nameof(LuuTruHoSoExportExcel.HoTen), "Họ Tên"),
            //    (nameof(LuuTruHoSoExportExcel.GioiTinhDisplay), "Giới Tính"),
            //    (nameof(LuuTruHoSoExportExcel.DoiTuong), "Đối tượng"),
            //    (nameof(LuuTruHoSoExportExcel.KhoaNhapVien), "Khoa Nhập Viện"),
            //    (nameof(LuuTruHoSoExportExcel.TinhTrangDisplay), "Tình Trạng")
            //};
            ////var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lưu Trữ Bệnh Án", 2, "Lưu Trữ Bệnh Án");
            //_dieuTriNoiTruService.ExportBaoCaoSoLuuTruBenhAn
            //HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LuuTruBenhAn" + DateTime.Now.Year + ".xls");
            //Response.ContentType = "application/vnd.ms-excel";
            //return new FileContentResult(bytes, "application/vnd.ms-excel");
           
            var bytes = _dieuTriNoiTruService.ExportBaoCaoSoLuuTruBenhAn(benhAnData.ToList(), queryInfo);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoLuuTruBA" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetListKhoaPhong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoaPhong([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khoaPhongService.GetListKhoaPhongAll(model);
            lookup.Insert(0, new KhoaKhamTemplateVo { DisplayName = "Toàn viện", KeyId = 0 });
            return Ok(lookup);
        }

        [HttpPost("GetListKhoaPhongThuocNoiTru")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoaPhongThuocNoiTru([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khoaPhongService.GetListKhoaPhongThuocNoiTruAll(model);
            lookup.Insert(0, new KhoaKhamTemplateVo { DisplayName = "Toàn viện", KeyId = 0 });
            return Ok(lookup);
        }
    }

}
