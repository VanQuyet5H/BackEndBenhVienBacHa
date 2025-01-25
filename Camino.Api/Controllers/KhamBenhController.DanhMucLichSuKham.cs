using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        #region Grid DanhMucLichSuKham
        [HttpPost("GetDataForGridAsyncDanhMucLichSuKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhMucLichSuKhamBenh([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncDanhMucLichSuKhamBenh(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncDanhMucLichSuKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhMucLichSuKhamBenh([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridAsyncDanhMucLichSuKhamBenh(queryInfo);
            return Ok(gridData);
        }
        #endregion

        [HttpGet("ThongTinYeuCauKham")]
        public async Task<ActionResult> ThongTinYeuCauKham(long yeuCauKhamBenhId)
        {
            var ttYeuCauKham = await _yeuCauKhamBenhService.GetThongTinYeuCauKham(yeuCauKhamBenhId);
            return Ok(ttYeuCauKham);
        }

        [HttpGet("ThongTinBenhNhan")]
        public async Task<ActionResult> ThongTinBenhNhan(long yeuCauKhamBenhId)
        {
            var entity = await _yeuCauKhamBenhService.GetThongTinBenhNhan(yeuCauKhamBenhId);
            
            #region //BVHD-3941
            if (entity != null && entity.CoBaoHiemTuNhan == true)
            {
                entity.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(entity.YeuCauTiepNhanId);
            }
            #endregion

            return Ok(entity);
        }
        //// get 
        //[HttpPost("GetDataGridChanDoanPhanBiet")]
        //public async Task<ActionResult<GridDataSource>> GetDataGridChanDoanPhanBiet(QueryInfo queryInfo)
        //{
        //    var gridData = await _yeuCauKhamBenhChanDoanPhanBietService.GetDataGridChanDoanPhanBiet(queryInfo);
        //    return Ok(gridData);
        //}

        #region Excel
        [HttpPost("ExportLichSuKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuKhamBenh)]
        public async Task<ActionResult> ExportLichSuKhamBenh(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncDanhMucLichSuKhamBenh(queryInfo, true);
            var lichSuTiepNhanData = gridData.Data.Select(p => (DanhSachChoKhamGridVo)p).ToList();
            var excelData = lichSuTiepNhanData.Map<List<DanhSachTiepNhanExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(DanhSachTiepNhanExportExcel.MaYeuCauTiepNhan), "Mã TN"),
                (nameof(DanhSachTiepNhanExportExcel.MaBenhNhan), "Mã BN"),
                (nameof(DanhSachTiepNhanExportExcel.HoTen), "Tên người bệnh"),
                (nameof(DanhSachTiepNhanExportExcel.NamSinh), "Năm sinh"),
                (nameof(DanhSachTiepNhanExportExcel.DiaChi), "Địa chỉ"),
                (nameof(DanhSachTiepNhanExportExcel.TenDichVu), "Tên dịch vụ"),
                (nameof(DanhSachTiepNhanExportExcel.ThoiDiemThucHienDisplay), "Thời điểm thực hiện"),
                (nameof(DanhSachTiepNhanExportExcel.TenNhanVienTiepNhan), "Người tiếp nhận"),
                (nameof(DanhSachTiepNhanExportExcel.ThoiDiemTiepNhanDisplay), "Tiếp nhận lúc"),
                (nameof(DanhSachTiepNhanExportExcel.TrieuChungTiepNhan), "Lý do khám bệnh"),
                (nameof(DanhSachTiepNhanExportExcel.ChuanDoan), "Chẩn đoán"),
                (nameof(DanhSachTiepNhanExportExcel.CachGiaiQuyet), "Cách giải quyết"),
                (nameof(DanhSachTiepNhanExportExcel.BSKham), "Bác sĩ khám"),
                (nameof(DanhSachTiepNhanExportExcel.DoiTuong), "Đối tượng"),
                (nameof(DanhSachTiepNhanExportExcel.TrangThaiYeuCauKhamBenhSearch), "Trạng thái")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Lịch Sử Khám Bệnh", 2, "Lịch Sử Khám Bệnh");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuKhamBenh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        [HttpPost("InToaThuocKhamBenhDanhSach")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuKhamBenh)]
        public ActionResult InToaThuocKhamBenhDanhSach(InToaThuocKhamBenhDanhSach inToaThuocDanhSach)//InToaThuocDanhSach
        {
            var result = _yeuCauKhamBenhService.InToaThuocKhamBenhDanhSach(inToaThuocDanhSach);
            return Ok(result);
        }
        [HttpPost("InToaThuocBHYTVaKhongBHYTDanhSachKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuKhamBenh)]
        public ActionResult InToaThuocBHYTVaKhongBHYTDanhSachKhamBenh(InToaThuocKhamBenhDanhSach inToaThuocDanhSach)//InToaThuocDanhSach
        {
            var result = _yeuCauKhamBenhService.InToaThuocBHYTVaKhongBHYTDanhSachKhamBenh(inToaThuocDanhSach);
            return Ok(result);
        }
    }
}
