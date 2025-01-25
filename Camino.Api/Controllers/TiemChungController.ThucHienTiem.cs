using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.TiemChung;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TiemChungs;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class TiemChungController
    {
        #region grid data
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridHoanThanhThucHienTiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridHoanThanhThucHienTiemAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _tiemChungService.GetDataForGridHoanThanhTiemChungAsync(queryInfo);
            var gridData = await _tiemChungService.GetDataForGridHoanThanhTiemChungAsyncVer2(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridHoanThanhThucHienTiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridHoanThanhThucHienTiemAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _tiemChungService.GetTotalPageForGridHoanThanhTiemChungAsync(queryInfo);
            var gridData = await _tiemChungService.GetTotalPageForGridHoanThanhTiemChungAsyncVer2(queryInfo);
            return Ok(gridData);
        }


        #endregion

        #region xử lý cập nhật thông tin tiêm chủng

        [HttpPut("XuLyLuuThongTinThucHienTiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult> XuLyLuuThongTinThucHienTiem(ThucHienTiemKhamSangLocViewModel thongTinTiem)
        {
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thongTinKhamSangLoc = await _tiemChungService.GetThongTinTiemChungTheoPhongThucHienAsync(thongTinTiem.Id);

            // kiểm tra vacxin đã tiêm mà lại cập nhật trạng thái thành chưa tiêm
            var lstVacxinChuaTiemId = thongTinTiem.YeuCauDichVuKyThuats
                .Where(x => x.TiemChung.TrangThaiTiemChung == Enums.TrangThaiTiemChung.ChuaTiemChung).Select(x => x.Id)
                .ToList();
            if (thongTinKhamSangLoc.YeuCauDichVuKyThuats
                .Where(x => lstVacxinChuaTiemId.Contains(x.Id))
                .Any(x => x.TiemChung.TrangThaiTiemChung == Enums.TrangThaiTiemChung.DaTiemChung))
            {
                throw new ApiException(_localizationService.GetResource("ThucHienTiem.VacXin.DaTiem"));
            }

            // kiểm tra vacxin mới thêm chưa hiển thị trên UI
            var lstVacxinPhongHienTaiId = thongTinTiem.YeuCauDichVuKyThuats
                .Select(x => x.Id)
                .ToList();
            if (thongTinKhamSangLoc.YeuCauDichVuKyThuats
                .Any(x => !lstVacxinPhongHienTaiId.Contains(x.Id)
                          && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                          && x.NoiThucHienId == phongHienTaiId
                          && (x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var model = thongTinTiem.ToEntity(thongTinKhamSangLoc);
            await _tiemChungService.XuLyLuuThongTinThucHienTiemAsync(model, thongTinTiem.IsHoanThanhTiem);
            return Ok();
        }

        [HttpPut("CapNhatKhamLaiKhamSangLocTiemChung")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult> CapNhatKhamLaiKhamSangLocTiemChungAsync(KhamTiemChungMoLaiVo thongTinKham)
        {
            await _tiemChungService.CapNhatKhamLaiKhamSangLocTiemChungAsync(thongTinKham);
            return Ok();
        }

        [HttpPut("CapNhatKhamLaiThucHienTiemTheoPhong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult> CapNhatKhamLaiThucHienTiemTheoPhongAsync(KhamTiemChungMoLaiVo thongTinKham)
        {
            await _tiemChungService.CapNhatKhamLaiThucHienTiemTheoPhongAsync(thongTinKham);
            return Ok();
        }

        [HttpPut("XuLyCapNhatThucHienYeuCauDichVuKyThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiemChungKhamSangLoc, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult> XuLyCapNhatThucHienYeuCauDichVuKyThuatAsync([FromBody] TrangThaiThucHienYeuCauDichVuKyThuatViewModel viewModel)
        {
            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(viewModel.Id);
            if (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            {
                throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiYeuCauDichVuKyThuat.DaHuy"));
            }
            viewModel.ToEntity(yeuCauDichVuKyThuat);
            if (viewModel.LaThucHienDichVu)
            {
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan != Enums.TrangThaiThanhToan.BaoLanhThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != Enums.TrangThaiThanhToan.CapNhatThanhToan)
                {
                    throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiThanhToanYeuCauDichVuKyThuat.ChuaThanhToan"));
                }

                //yeuCauDichVuKyThuat.LyDoHuyTrangThaiDaThucHien = null;
                //yeuCauDichVuKyThuat.NhanVienHuyTrangThaiDaThucHienId = null;
                yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            }
            else
            {
                yeuCauDichVuKyThuat.NhanVienHuyTrangThaiDaThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauDichVuKyThuat.NhanVienThucHienId = yeuCauDichVuKyThuat.NhanVienKetLuanId = null;
                yeuCauDichVuKyThuat.ThoiDiemThucHien = yeuCauDichVuKyThuat.ThoiDiemHoanThanh = null;
                yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            }

            _yeuCauDichVuKyThuatService.Update(yeuCauDichVuKyThuat);
            return Ok();
        }

        [HttpPut("XuLyHuyTiemVacxin")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult> XuLyHuyTiemVacxinAsync(long yeuCauVacxinId)
        {
            await _tiemChungService.XuLyHuyTiemVacxinAsync(yeuCauVacxinId);
            return Ok();
        }
        #endregion
    }
}
