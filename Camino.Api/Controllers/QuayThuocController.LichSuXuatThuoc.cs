using Camino.Api.Auth;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class QuayThuocController
    {
        #region Hủy Bán Thuốc 

        [HttpPost("HuyBanThuocTrongNgay")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public ActionResult HuyBanThuocTrongNgay(ThongTinHuyPhieuXuatTrongNgayViewModel huyBanThuocTrongNgayViewModel)
        {
            var thongTinHuyPhieuVo = huyBanThuocTrongNgayViewModel.Map<ThongTinHuyPhieuXuatTrongNgayVo>();
            _quayThuocLichSuXuatThuocService.HuyBanThuocTrongNgay(thongTinHuyPhieuVo);
            return Ok();
        }

        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridLichSuBanThuocAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLichSuBanThuocAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuXuatThuocService.GetDataForGridLichSuXuatThuocAsync(queryInfo, false);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridLichSuBanThuocAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLichSuBanThuocAsync([FromBody]QueryInfo queryInfo)
        {
            //đã bỏ lazy load total row
            var gridData = await _quayThuocLichSuXuatThuocService.GetDataForGridLichSuXuatThuocAsync(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridLichSuXuatThuocAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLichSuXuatThuocAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuXuatThuocService.GetDataForGridLichSuXuatThuocAsync(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridLichSuXuatThuocAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLichSuXuatThuocAsync([FromBody]QueryInfo queryInfo)
        {
            //đã bỏ lazy load total row
            var gridData = await _quayThuocLichSuXuatThuocService.GetDataForGridLichSuXuatThuocAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetThongTinBenhNhanXuatThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult> GetThongTinBenhNhanXuatThuoc(long ycTiepNhanId, string idBenhNhan)
        {
            var result = _quayThuocLichSuXuatThuocService.GetThongTinBenhNhanDetail(ycTiepNhanId, idBenhNhan);

            #region BVHD-3941
            if (result.Any())
            {
                var tenCongTy = string.Empty;
                foreach (var item in result)
                {
                    if (item.CoBaoHiemTuNhan == true)
                    {
                        if (string.IsNullOrEmpty(tenCongTy))
                        {
                            tenCongTy = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(ycTiepNhanId);
                        }

                        item.TenCongTyBaoHiemTuNhan = tenCongTy;
                    }
                }
            }
            #endregion
            return Ok(result);
        }

        [HttpPost("GetThongTinBenhNhanTheoMaBN")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public ActionResult GetThongTinBenhNhanTheoMaBN(string maBN)
        {
            var result = _quayThuocLichSuXuatThuocService.GetThongTinBenhNhanTheoMaBN(maBN);
            return Ok(result);
        }

        [HttpPost("GetDanhSachXuatThuocBHYTLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult> GetDanhSachXuatThuocBHYTLS(long tiepNhanId, long idTaiKhoanThu)
        {
            var result = await _quayThuocLichSuXuatThuocService.GetDanhSachThuocDaXuatThuocBHYTLS(tiepNhanId, idTaiKhoanThu);
            return Ok(result);
        }

        [HttpPost("GetDanhSachXuatThuocKhongBHYTLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult> GetDanhSachXuatThuocKhongBHYTLS(long tiepNhanId, long idTaiKhoanThu)
        {
            var result = await _quayThuocLichSuXuatThuocService.GetDanhSachThuocDaXuatThuocKhongBHYTLS(tiepNhanId, idTaiKhoanThu);
            return Ok(result);
        }

        [HttpPost("ExportLichSuBanThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult> ExportLichSuBanThuoc(QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuXuatThuocService.GetDataForGridLichSuXuatThuocAsync(queryInfo, true);
            var lsBanThuoc = gridData.Data.Select(p => (LichSuXuatThuocGridVo)p).ToList();
            var dataExcel = lsBanThuoc.Map<List<LichSuBanThuocExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.SoDon), "SỐ ĐƠN"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.MaTN), "MÃ TIẾP NHẬN"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.MaBN), "MÃ NGƯỜI BỆNH"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.HoTen), "TÊN NGƯỜI BỆNH"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.NamSinh), "NĂM SINH"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.GioiTinhHienThi), "GIỚI TÍNH"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.DiaChi), "ĐỊA CHỈ"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.SoDienThoai), "SỐ ĐIỆN THOẠI"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.DoiTuong), "ĐỐI TƯỢNG"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.SoTienThuString), "TỔNG GIÁ TRỊ ĐƠN THUOC"));
            lstValueObject.Add((nameof(LichSuBanThuocExportExcel.NgayThuStr), "NGÀY THU"));
            //lstValueObject.Add((nameof(LichSuBanThuocExportExcel.ThoiDiemCapPhatThuoc), "THỜI ĐIỂM CẤP PHÁT THUỐC"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Lịch Sử Bán Thuốc", 2, "Lịch Sử Bán Thuốc");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuBanThuoc" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportLichSuXuatThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult> ExportLichSuXuatThuoc(QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuXuatThuocService.GetDataForGridLichSuXuatThuoc(queryInfo, true);
            var lsBanThuoc = gridData.Data.Select(p => (LichSuXuatThuocGridVo)p).ToList();
            var dataExcel = lsBanThuoc.Map<List<LichSuXuatThuocExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            //lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.SoDon), "Số Đơn"));
            //lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.MaTN), "Mã Tiếp Nhận"));
            //lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.MaBN), "Mã Người Bệnh"));
            //lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.HoTen), "Tên Người Bệnh"));
            //lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.SoTienThuString), "Tổng giá trị đơn thuốc"));
            //lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.DoiTuong), "Đối Tượng"));
            //lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.ThoiDiemCapPhatThuocString), "THỜI ĐIỂM CẤP PHÁT THUỐC"));

            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.SoDon), "SỐ ĐƠN"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.MaTN), "MÃ TIẾP NHẬN"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.MaBN), "MÃ NGƯỜI BỆNH"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.HoTen), "TÊN NGƯỜI BỆNH"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.NamSinh), "NĂM SINH"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.GioiTinhHienThi), "GIỚI TÍNH"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.DiaChi), "ĐỊA CHỈ"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.SoDienThoai), "SỐ ĐIỆN THOẠI"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.DoiTuong), "ĐỐI TƯỢNG"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.SoTienThuString), "TỔNG GIÁ TRỊ ĐƠN THUOC"));
            lstValueObject.Add((nameof(LichSuXuatThuocExportExcel.ThoiDiemCapPhatThuocString), "THỜI ĐIỂM CẤP PHÁT THUỐC"));
            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Lịch Sử Xuất Thuốc", 2, "Lịch Sử Xuất Thuốc");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuXuatThuoc" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #region BVHD-3941
        [HttpGet("GetThongTinCongTyBaoHiemTuNhanTheoMaTN")]
        public async Task<ActionResult<string>> GetThongTinCongTyBaoHiemTuNhanTheoMaTN(string maTN)
        {
            var tenCongTyBaoHiemTuNhan = string.Empty;
            var yctnId = await _quayThuocLichSuXuatThuocService.GetThongTinCongTyBaoHiemTuNhanTheoMaTN(maTN);
            if (yctnId != 0)
            {
                tenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(yctnId);
            }

            return tenCongTyBaoHiemTuNhan;
        }


        #endregion
    }
}
