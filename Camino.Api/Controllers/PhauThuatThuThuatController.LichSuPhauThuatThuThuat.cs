using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;


namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        [HttpPost("GetDataForGridAsyncLichSuPhauThuatThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncLichSuPhauThuatThuThuat([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncLichSuPhauThuatThuThuat(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncLichSuPhauThuatThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncLichSuPhauThuatThuThuat([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridAsyncLichSuPhauThuatThuThuat(queryInfo);
            return Ok(gridData);
        }

        #region CanLamSang

        [HttpPost("GetDataForGridAsyncLichSuCLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncLichSuCLS([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncLichSuCLS(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncLichSuCLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncLichSuCLS([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridAsyncLichSuCLS(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region PTTT

        [HttpPost("GetDichVuPTTTs")]
        public async Task<ActionResult<ICollection<DichVuPTTTsLookupItemVo>>> GetDichVuPTTTs([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _phauThuatThuThuatService.GetDichVuPTTTs(model);
            return Ok(lookup);
        }


        [HttpGet("ThongTinBenhNhanPTTT")]
        public async Task<ActionResult> ThongTinBenhNhan(long yeuCauDichVuKyThuatId)
        {
            var entity = await _phauThuatThuThuatService.GetThongTinBenhNhan(yeuCauDichVuKyThuatId);

            #region BVHD-3941
            if (entity != null && entity.CoBaoHiemTuNhan == true)
            {
                entity.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(entity.YeuCauTiepNhanId).Result;
            }
            #endregion

            return Ok(entity);
        }

        [HttpPost("GetDataForGridAsyncLichSuEkipBacSi")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncLichSuEkipBacSi([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncLichSuEkipBacSi(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncLichSuEkipBacSi")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncLichSuEkipBacSi([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridAsyncLichSuEkipBacSi(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("ThongTinLichSuEkipPTTT")]
        public async Task<ActionResult> ThongTinLichSuEkipPTTT(long yeuCauDichVuKyThuatId)
        {
            var entity = await _phauThuatThuThuatService.GetThongTinLichSuEkipPTTT(yeuCauDichVuKyThuatId);
            return Ok(entity);
        }

        [HttpGet("KiemTraCoDichVuHuy")]
        public async Task<ActionResult> KiemTraCoDichVuHuy(long yeuCauTiepNhanId)
        {
            var entity = await _phauThuatThuThuatService.KiemTraCoDichVuHuy(yeuCauTiepNhanId);
            return Ok(entity);
        }

        ///////////////////////////////Danh sách dịch vụ không thực hiện tường trình
        ///
        [HttpPost("GetDataForGridAsyncLichSuDVPTTTKhongThucHien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncLichSuDVPTTTKhongThucHien([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncLichSuDVPTTTKhongThucHien(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncLichSuDVPTTTKhongThucHien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncLichSuDVPTTTKhongThucHien([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridAsyncLichSuDVPTTTKhongThucHien(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region KetLuan
        [HttpGet("LichSuKetLuanPTTT")]
        public async Task<ActionResult> LichSuKetLuanPTTT(long yeuCauDichVuKyThuatId)
        {
            var entity = await _phauThuatThuThuatService.GetThongTinLichSuKetLuanPTTT(yeuCauDichVuKyThuatId);
            return Ok(entity);
        }

        [HttpPost("DichVuDaTuongTrinh")]
        public async Task<ActionResult> GetDichVuDaTuongTrinh(LichSuDichVuKyThuatDaTuongTrinhVo lichSuDichVuKyThuatDaTuongTrinhVo)
        {
            var entity = await _phauThuatThuThuatService.GetDichVuDaTuongTrinh(lichSuDichVuKyThuatDaTuongTrinhVo);
            return Ok(entity);
        }
        #endregion

        #region TheoDoi
        [HttpPost("GetDataForGridAsyncChiSoSinhHieuPTTT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncChiSoSinhHieuPTTT([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncChiSoSinhHieuPTTT(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncChiSoSinhHieuPTTT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncChiSoSinhHieuPTTT([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridAsyncChiSoSinhHieuPTTT(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridAsyncLichSuKhamCacCoQuan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncLichSuKhamCacCoQuan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncLichSuKhamCacCoQuan(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncLichSuKhamCacCoQuan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncLichSuKhamCacCoQuan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridAsyncLichSuKhamCacCoQuan(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Excel
        [HttpPost("ExportLichSuPhauThuatThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuPhauThuatThuThuat)]
        public async Task<ActionResult> ExportLichSuPhauThuatThuThuat(QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncLichSuPhauThuatThuThuat(queryInfo, true);
            var lichSuTiepNhanData = gridData.Data.Select(p => (LichSuPhauThuatThuThuatGridVo)p).ToList();
            var excelData = lichSuTiepNhanData.Map<List<LichSuPhauThuatThuThuatExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(LichSuPhauThuatThuThuatExportExcel.MaYeuCauTiepNhan), "Mã TN"),
                (nameof(LichSuPhauThuatThuThuatExportExcel.MaBN), "Mã BN"),
                (nameof(LichSuPhauThuatThuThuatExportExcel.HoTen), "Tên người bệnh"),
                (nameof(LichSuPhauThuatThuThuatExportExcel.NamSinh), "Năm sinh"),
                (nameof(LichSuPhauThuatThuThuatExportExcel.DoiTuong), "Đối tượng"),
                (nameof(LichSuPhauThuatThuThuatExportExcel.ThoiDiemHoanThanhDisplay), "Thời điểm hoàn thành"),
                (nameof(LichSuPhauThuatThuThuatExportExcel.ThoiDiemThucHienDisplay), "Thời điểm thực hiện"),
                (nameof(LichSuPhauThuatThuThuatExportExcel.ThoiDiemTiepNhanDisplay), "Tiếp nhận lúc"),
                (nameof(LichSuPhauThuatThuThuatExportExcel.TrangThaiPTTTSearch), "Trạng thái"),
                (nameof(LichSuPhauThuatThuThuatExportExcel.NoiChuyenGiao), "Nơi chuyển giao")
                //(nameof(LichSuPhauThuatThuThuatExportExcel.DiaChi), "Địa chỉ"),
                //(nameof(LichSuPhauThuatThuThuatExportExcel.TenDichVu), "Tên dịch vụ"),
                //(nameof(LichSuPhauThuatThuThuatExportExcel.TrieuChungTiepNhan), "Lý do khám bệnh"),
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lịch Sử Phẫu Thuật Thủ Thuật", 2, "Lịch Sử Phẫu Thuật Thủ Thuật");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuPhauThuatThuThuat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
