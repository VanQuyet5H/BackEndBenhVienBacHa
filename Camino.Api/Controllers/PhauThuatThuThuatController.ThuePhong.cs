using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        [HttpPost("XuLyLuuThongTinThuePhong")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> XuLyLuuThongTinThuePhongAsync([FromBody] PhauThuatThuThuatThuePhongViewModel thongTinThuePhong)
        {
            var thongTinThuePhongVo = new ThongTinThuePhongVo()
            {
                YeuCauTiepNhanId = thongTinThuePhong.YeuCauTiepNhanId,
                YeuCauDichVuKyThuatId = thongTinThuePhong.YeuCauDichVuKyThuatId,
                ThuePhongId = thongTinThuePhong.ThuePhongId,
                CauHinhThuePhongId = thongTinThuePhong.CauHinhThuePhongId,
                CoThuePhong = thongTinThuePhong.CoThuePhong == true,
                ThoiDiemBatDau = thongTinThuePhong.ThoiDiemBatDau.Value,
                ThoiDiemKetThuc = thongTinThuePhong.ThoiDiemKetThuc.Value
            };
            await _phauThuatThuThuatService.XuLyLuuThongTinThuePhongAsync(thongTinThuePhongVo);
            return Ok(thongTinThuePhongVo);
        }

        [HttpPost("XuLyCapNhatThongTinThuePhong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucLichSuThuePhong)]
        public async Task<ActionResult> XuLyCapNhatThongTinThuePhongAsync([FromBody] PhauThuatThuThuatLichSuThuePhongViewModel thongTinThuePhong)
        {
            var thongTinThuePhongVo = new ThongTinThuePhongVo()
            {
                YeuCauTiepNhanId = thongTinThuePhong.YeuCauTiepNhanId,
                YeuCauDichVuKyThuatId = thongTinThuePhong.YeuCauDichVuKyThuatId.Value,
                ThuePhongId = thongTinThuePhong.ThuePhongId,
                CauHinhThuePhongId = thongTinThuePhong.CauHinhThuePhongId,
                CoThuePhong = thongTinThuePhong.CoThuePhong == true,
                ThoiDiemBatDau = thongTinThuePhong.ThoiDiemBatDau.Value,
                ThoiDiemKetThuc = thongTinThuePhong.ThoiDiemKetThuc.Value
            };
            await _phauThuatThuThuatService.XuLyLuuThongTinThuePhongAsync(thongTinThuePhongVo);
            return Ok(thongTinThuePhongVo);
        }

        #region Grid lịch sử thuê phòng
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridLichSuThuePhong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLichSuThuePhong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLichSuThuePhongAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridLichSuThuePhongAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridLichSuThuePhong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLichSuThuePhong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLichSuThuePhongAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridLichSuThuePhongAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region export excel
        [HttpPost("ExportDanhSachLichSuThuePhong")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucLichSuThuePhong)]
        public async Task<ActionResult> ExportDanhSachLichSuThuePhong(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;

            var gridData = await _phauThuatThuThuatService.GetDataForGridLichSuThuePhongAsync(queryInfo);
            var lichSuThuePhongData = gridData.Data.Select(p => (LichSuThuePhongGridVo)p).ToList();
            var excelData = lichSuThuePhongData.Map<List<LichSuThuePhongExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.MaYeuCauTiepNhan), "Mã TN"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.MaNB), "Mã NB"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.TenNB), "Tên NB"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.NgaySinhDisplay), "NS"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.DiaChi), "Địa Chỉ"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.DoiTuong), "Đối Tượng"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.DichVuThue), "DV Thuê"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.LoaiPhongThue), "Loại Phòng Thuê"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.BatDauThueDisplay), "Bắt Đầu"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.KetThucThueDisplay), "Kết Thúc"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.PhongThucHien), "Phòng TH"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.BacSiGayMe), "BS Gây Mê"));
            lstValueObject.Add((nameof(LichSuThuePhongExportExcel.PhauThuatVien), "PTV"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lịch Sử Thuê Phòng");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuThuePhong" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #region Chi tiết lịch sử thuê phòng
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHanhChinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucLichSuThuePhong)]
        public async Task<ActionResult<LichSuThuePhongThongTinHanhChinhVo>> GetThongTinHanhChinh(long yeuCauTiepNhanId)
        {
            var result = await _phauThuatThuThuatService.GetThongTinHanhChinh(yeuCauTiepNhanId);

            #region BVHD-3941
            if (result?.CoBaoHiemTuNhan == true)
            {
                result.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(result.YeuCauTiepNhanId ?? 0).Result;
            }
            #endregion

            return result;
        }

        [HttpPost("GetListDichVuCoThuePhongTheoTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay, Enums.DocumentType.DanhMucLichSuThuePhong)]
        public async Task<ActionResult> GetListDichVuCoThuePhongTheoTiepNhan([FromBody]DropDownListRequestModel model)
        {
            var lstTuVong = await _phauThuatThuThuatService.GetListDichVuCoThuePhongTheoTiepNhan(model);
            return Ok(lstTuVong);
        }
        #endregion
    }
}
