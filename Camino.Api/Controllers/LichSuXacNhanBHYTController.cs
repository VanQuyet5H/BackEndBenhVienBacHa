using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauTiepNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XacNhanBHYTs;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.XacNhanBHYTs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class LichSuXacNhanBHYTController : CaminoBaseController
    {
        private readonly ILichSuXacNhanBhytService _lichSuXacNhanBhytService;
        private readonly IExcelService _excelService;
        private readonly IXacNhanBHYTService _xacNhanBhytService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;

        public LichSuXacNhanBHYTController
        (
            IExcelService excelService,
            ILichSuXacNhanBhytService lichSuXacNhanBhytService,
            IXacNhanBHYTService xacNhanBhytService,
            ITiepNhanBenhNhanService tiepNhanBenhNhanService
        )
        {
            _lichSuXacNhanBhytService = lichSuXacNhanBhytService;
            _excelService = excelService;
            _xacNhanBhytService = xacNhanBhytService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
        }

        #region LoadGridLichSuCacBaoHiemYTe
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridLichSuBhytAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuXacNhanBHYT)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLichSuBhytAsync
            ([FromBody]QueryInfo queryInfo)
        {
            if (queryInfo.Sort.FirstOrDefault()?.Field == "ThoiDiemDuyetBaoHiem")
            {
                Sort first = null;
                foreach (var sort in queryInfo.Sort)
                {
                    first = sort;
                    break;
                }

                if (first != null) first.Field = "ThoiDiemDuyet";
            }

            var gridData = await _lichSuXacNhanBhytService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridLichSuBhytAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuXacNhanBHYT)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLichSuBhytAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _lichSuXacNhanBhytService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region LoadGridChiTietBhytTheoYeuCauTiepNhan
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridXacNhanAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuXacNhanBHYT)]
        public ActionResult<GridDataSource> GetDataForGridXacNhanAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = _lichSuXacNhanBhytService.GetDataForGridXacNhanAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportLichSuXacNhanBhyt")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuXacNhanBHYT)]
        public async Task<ActionResult<GridDataSource>> ExportLichSuXacNhanBhyt
            ([FromBody]QueryInfo queryInfo)
        {
            if (queryInfo.Sort.FirstOrDefault()?.Field == "ThoiDiemDuyetBaoHiem")
            {
                Sort first = null;
                foreach (var sort in queryInfo.Sort)
                {
                    first = sort;
                    break;
                }

                if (first != null) first.Field = "ThoiDiemDuyet";
            }

            var gridData = await _lichSuXacNhanBhytService.GetDataForGridAsync(queryInfo, true);
            var xacNhanBhytData = gridData.Data.Select(p => (LichSuXacNhanBHYTGridVo)p).ToList();
            var dataExcel = xacNhanBhytData.Map<List<LichSuXacNhanBhytExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.IdLichSuXacNhan), "Mã XN"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.MaTN), "Mã TN"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.MaBN), "Mã BN"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.HoTen), "Họ Tên"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.NamSinh), "Năm Sinh"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.TenGioiTinh), "Giới Tính"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.DiaChi), "Địa Chỉ"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.SoDienThoai), "Số Điện Thoại"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.SoTienDaXacNhan), "Số Tiền Đã XN"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.ThoiDiemDuyetBaoHiem), "Thời Điểm Duyệt"));
            lstValueObject.Add((nameof(LichSuXacNhanBhytExportExcel.NhanVienDuyetBaoHiem), "Người Duyệt"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Lịch Sử Xác Nhận BHYT");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuXacNhanBHYT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region LoadThongTinHanhChinhVaThongTinBaoHiemYTeVaThongTinNguoiDuyet
        [HttpPost("GetById")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuXacNhanBHYT)]
        public async Task<ActionResult<YeuCauTiepNhanViewModel>> GetById(long id)
        {
            var duyetBaoHiem = await _lichSuXacNhanBhytService.GetByIdAsync(id,
                s => s.Include(k => k.NhanVienDuyetBaoHiem).ThenInclude(k => k.User));


            var yeuCauTiepNhan = await _xacNhanBhytService.GetByIdAsync(duyetBaoHiem.YeuCauTiepNhanId,
                s => s.Include(k => k.DoiTuongUuDai)
                    .Include(k => k.CongTyUuDai)
                    .Include(k => k.PhuongXa)
                    .Include(k => k.QuanHuyen)
                    .Include(k => k.TinhThanh)
                    .Include(k => k.BenhNhan));

            var resultData = yeuCauTiepNhan.ToModel<YeuCauTiepNhanViewModel>();
            resultData.LyDoVaoVienDisplay = yeuCauTiepNhan.LyDoVaoVien.GetDescription();

            if (resultData.BHYTMaDKBD != null)
            {
                var noiDKBD = _xacNhanBhytService.GetNoiDKBD(resultData.BHYTMaDKBD);
                resultData.DKBD = noiDKBD;
            }

            if (resultData.GiayChuyenVienId != null)
            {
                var giayChuyenVien = _xacNhanBhytService.GetGiayChuyenVien(resultData.GiayChuyenVienId);
                resultData.GiayChuyenVienDisplay = giayChuyenVien;
            }

            if (resultData.BHYTGiayMienCungChiTraId != null)
            {
                var giayMienCungChiTra = _xacNhanBhytService.GetGiayMienCungChiTra(resultData.BHYTGiayMienCungChiTraId);
                resultData.GiayMienCungChiTraDisplay = giayMienCungChiTra;
            }

            if (resultData.BHYTNgayHieuLuc != null)
            {
                var ngayHieuLucFormat = resultData.BHYTNgayHieuLuc.Value.ApplyFormatDate();
                resultData.BHYTngayHieuLucStr = ngayHieuLucFormat;
            }

            if (resultData.BHYTNgayHetHan != null)
            {
                var ngayHetHanFormat = resultData.BHYTNgayHetHan.Value.ApplyFormatDate();
                resultData.BHYTngayHetHanStr = ngayHetHanFormat;
            }

            resultData.DiaChi = yeuCauTiepNhan.DiaChiDayDu;

            resultData.NhanVienDuyet = duyetBaoHiem.NhanVienDuyetBaoHiem.User.HoTen;
            resultData.ThoiDiemDuyet = duyetBaoHiem.ThoiDiemDuyetBaoHiem.ApplyFormatDateTimeSACH();

            #region BVHD-3941
            if (yeuCauTiepNhan.CoBHTN == true)
            {
                resultData.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(yeuCauTiepNhan.Id);
            }
            #endregion

            return Ok(resultData);
        }
        #endregion
    }
}