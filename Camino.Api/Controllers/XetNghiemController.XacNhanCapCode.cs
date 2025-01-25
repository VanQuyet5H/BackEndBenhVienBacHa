using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.XetNghiem;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class XetNghiemController
    {
        #region grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChuaCapCode")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChuaCapCodeAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _xetNghiemService.GetDataForGridChuaCapCodeAsync(queryInfo);
                //gridData = await _xetNghiemService.GetDataForGridChuaCapCodeAsyncVer2(queryInfo);
            }
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChuaCapCode")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChuaCapCodeAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _xetNghiemService.GetTotalPageForGridChuaCapCodeAsync(queryInfo);
                //gridData = await _xetNghiemService.GetTotalPageForGridChuaCapCodeAsyncVer2(queryInfo);
            }

            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDaCapCode")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDaCapCodeAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _xetNghiemService.GetDataForGridDaCapCodeAsync(queryInfo);
            }

            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDaCapCode")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDaCapCodeAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = new GridDataSource();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                gridData = await _xetNghiemService.GetTotalPageForGridDaCapCodeAsync(queryInfo);
            }

            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _xetNghiemService.GetDataForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsync(queryInfo);
            var gridData = await _xetNghiemService.GetDataForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsyncVer2(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsync([FromBody] QueryInfo queryInfo)
        {
            //hiện tại ko dùng total
            var gridData = await _xetNghiemService.GetTotalPageForGridDichVuXetNghiemQuyTrinhCapCodeVaNhanMauAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region get data
        [HttpPost("GetListHopDongKhamSucKhoeHieuLuc")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListHopDongKhamSucKhoeHieuLucAsync(DropDownListRequestModel model)
        {
            var lookup = await _xetNghiemService.GetListHopDongKhamSucKhoeHieuLucAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetThongTinBenhNhanXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<ThongTinBenhNhanXetNghiemVo>> GetThongTinBenhNhanXetNghiemAsync(BenhNhanXetNghiemQueryVo query)
        {
            var chiTiet = await _xetNghiemService.GetThongTinBenhNhanXetNghiemAsync(query);

            #region BVHD-3941
            if (chiTiet != null && chiTiet.CoBaoHiemTuNhan == true)
            {
                chiTiet.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(chiTiet.YeuCauTiepNhanId);
            }
            #endregion

            return Ok(chiTiet);
        }
        #endregion

        #region Xử lý data
        [HttpPut("XuLyXacNhanCapBarcodeDichhVuXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> XuLyXacNhanCapBarcodeDichhVuXetNghiemAsync(CapBarcodeTheoDichVuViewModel capBarcodeTheoDichVuViewModel)
        {
            if (!capBarcodeTheoDichVuViewModel.YeuCauDichVuKyThuatIds.Any())
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.YeuCauDichVuKyThuatIds.Required"));
            }
            // xử lý tạo phiên xét nghiệm (dựa vào barcode để tạo hoặc cập nhật)
            var capBarcodeTheoDichVuVo = new CapBarcodeTheoDichVuVo()
            {
                YeuCauTiepNhanId = capBarcodeTheoDichVuViewModel.YeuCauTiepNhanId,
                BenhNhanId = capBarcodeTheoDichVuViewModel.BenhNhanId,
                BarcodeNumber = capBarcodeTheoDichVuViewModel.BarcodeNumber.Value,
                BarcodeId = capBarcodeTheoDichVuViewModel.BarcodeId,
                YeuCauDichVuKyThuatIds = capBarcodeTheoDichVuViewModel.YeuCauDichVuKyThuatIds,
                NhanVienLayMauId = capBarcodeTheoDichVuViewModel.NhanVienLayMauId,
                ThoiGianLayMau = capBarcodeTheoDichVuViewModel.ThoiGianLayMau
            };
            await _xetNghiemService.XuLyCapBarcodeChoDichhVuDangChonAsync(capBarcodeTheoDichVuVo);

            // xử lý lưu số lượng in thêm theo user
            await _xetNghiemService.XuLyCapNhatSoLuongInThemTheoUserAsync(capBarcodeTheoDichVuViewModel.SoLuongThem);
            return Ok();
        }

        [HttpPut("XuLyXacNhanHuyCapBarcodeDichVuChuaNhanMau")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> XuLyXacNhanHuyCapBarcodeDichVuChuaNhanMauAsync(XacNhanNhanMauChoDichVuViewModel xacNhanNhanMauViewModel)
        {
            if (!xacNhanNhanMauViewModel.YeuCauDichVuKyThuatIds.Any())
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.YeuCauDichVuKyThuatIds.Required"));
            }
            var xacNhanNhanMauVo = new XacNhanNhanMauChoDichVuVo()
            {
                YeuCauTiepNhanId = xacNhanNhanMauViewModel.YeuCauTiepNhanId,
                BenhNhanId = xacNhanNhanMauViewModel.BenhNhanId,
                YeuCauDichVuKyThuatIds = xacNhanNhanMauViewModel.YeuCauDichVuKyThuatIds
            };
            await _xetNghiemService.KiemTraDichVuCanHuyCapCodeAsync(xacNhanNhanMauVo.YeuCauDichVuKyThuatIds);
            await _xetNghiemService.XuLyXacNhanHuyCapBarcodeTheoDichVuAsync(xacNhanNhanMauVo);
            return Ok();
        }

        [HttpPut("XuLyXacNhanNhanMauDichVuChuaNhanMau")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<string>> XuLyXacNhanNhanMauDichVuChuaNhanMauAsync(XacNhanNhanMauChoDichVuViewModel xacNhanNhanMauViewModel) //long phienXetNghiemChiTietId
        {
            if (!xacNhanNhanMauViewModel.YeuCauDichVuKyThuatIds.Any())
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.YeuCauDichVuKyThuatIds.Required"));
            }
            var xacNhanNhanMauVo = new XacNhanNhanMauChoDichVuVo()
            {
                YeuCauTiepNhanId = xacNhanNhanMauViewModel.YeuCauTiepNhanId,
                BenhNhanId = xacNhanNhanMauViewModel.BenhNhanId,
                YeuCauDichVuKyThuatIds = xacNhanNhanMauViewModel.YeuCauDichVuKyThuatIds
            };
            await _xetNghiemService.XuLyXacNhanNhanMauXetNghiemAsync(xacNhanNhanMauVo);
            return Ok();
        }

        [HttpPut("XuLyXacNhanHuyNhanMauTheoDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> XuLyXacNhanHuyNhanMauTheoDichVuAsync(XacNhanNhanMauChoDichVuViewModel xacNhanNhanMauViewModel)
        {
            if (!xacNhanNhanMauViewModel.YeuCauDichVuKyThuatIds.Any())
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.YeuCauDichVuKyThuatIds.Required"));
            }
            var xacNhanNhanMauVo = new XacNhanNhanMauChoDichVuVo()
            {
                YeuCauTiepNhanId = xacNhanNhanMauViewModel.YeuCauTiepNhanId,
                BenhNhanId = xacNhanNhanMauViewModel.BenhNhanId,
                YeuCauDichVuKyThuatIds = xacNhanNhanMauViewModel.YeuCauDichVuKyThuatIds
            };
            await _xetNghiemService.KiemTraDichVuCanHuyNhanMauAsync(xacNhanNhanMauVo.YeuCauDichVuKyThuatIds);
            await _xetNghiemService.XuLyXacNhanHuyNhanMauTheoDichVuAsync(xacNhanNhanMauVo);
            return Ok();
        }
        #endregion

        #region Export excel
        [HttpPost("ExportDanhSachNguoiBenhDaCapCodeXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> ExportDanhSachNguoiBenhDaCapCodeXetNghiemAsync(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var benhNhanDaCapCodes = await _xetNghiemService.GetDataForGridDaCapCodeAsync(queryInfo);
            if (benhNhanDaCapCodes == null || benhNhanDaCapCodes.Data.Count == 0)
            {
                return NoContent();
            }

            var dataCapCodes = benhNhanDaCapCodes.Data.Cast<BenhNhanXetNghiemGridVo>().ToList();
            var lstPhienId = dataCapCodes.Where(x => x.PhienXetNghiemId != null).Select(x => x.PhienXetNghiemId.Value).ToList();
            var phienXetNghiemDaCaps = await _xetNghiemService.GetChiTietPhienXetNghiemsAsync(lstPhienId);
            var bytes = _excelService.ExportDanhSachNguoiBenhDaCapCodeXetNghiemAsync(dataCapCodes, phienXetNghiemDaCaps, queryInfo.AdditionalSearchString);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachNguoiBenhDaCapCode" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        [HttpPost("ExportDanhSachNguoiBenhChuaCapBarcodeXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> ExportDanhSachNguoiBenhChuaCapBarcodeXetNghiem(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var benhNhanChuaCapCodes = await _xetNghiemService.GetDataForGridChuaCapCodeAsync(queryInfo);
            var datas = benhNhanChuaCapCodes.Data.Cast<BenhNhanXetNghiemGridVo>().ToList();
            var excelData = datas.Map<List<BenhNhanXetNghiemChuaCapCodeExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(BenhNhanXetNghiemChuaCapCodeExcel.TenCongTy), "TÊN CÔNG TY"),
                (nameof(BenhNhanXetNghiemChuaCapCodeExcel.SoHopDong), "SỐ HỢP ĐỒNG"),
                (nameof(BenhNhanXetNghiemChuaCapCodeExcel.MaBenhNhan), "MÃ NGƯỜI BỆNH"),
                (nameof(BenhNhanXetNghiemChuaCapCodeExcel.MaTiepNhan), "MÃ TIẾP NHẬN"),
                (nameof(BenhNhanXetNghiemChuaCapCodeExcel.Barcode), "MÃ BARCODE"),
                (nameof(BenhNhanXetNghiemChuaCapCodeExcel.HoTen), "TÊN NGƯỜI BỆNH"),
                (nameof(BenhNhanXetNghiemChuaCapCodeExcel.GioiTinhDisplay), "GIỚI  TÍNH"),
                (nameof(BenhNhanXetNghiemChuaCapCodeExcel.NamSinhDisplay), "NGÀY THÁNG NĂM SINH"),
                 (nameof(BenhNhanXetNghiemChuaCapCodeExcel.ThoiGianLayMauDisplay), "THỜI GIAN LẤY MẪU"),
                (nameof(BenhNhanXetNghiemChuaCapCodeExcel.NhanVienLayMauIdDisplay), "NHÂN VIÊN LẤY MẪU")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Người Bệnh Chưa Cấp Barcode", 2, "DS Người Bệnh Chưa Cấp Barcode");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachNguoiBenhChuaCapCode" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ImportNguoiBenhChuaCapBarcodeXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> ImportNguoiBenhChuaCapBarcodeXetNghiem(BenhNhanXetNghiemChuaCapBarcodeImport model)
        {
            List<BenhNhanChuaCapBarcode> listError = new List<BenhNhanChuaCapBarcode>();
            var path = _taiLieuDinhKemService.GetObjectStream(model.DuongDan, model.TenGuid);
            listError = await _xetNghiemService.ImportNguoiBenhChuaCapBarcodeXetNghiem(path);
            return Ok(listError);
        }

        [HttpPost("InBarcodeCuaBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LayMauXetNghiem)]
        public List<string> InBarcodeCuaBenhNhan(InBarcodeDaCapCodeBenhNhan inBarcodeDaCapCodeBenhNhan)
        {
            return _xetNghiemService.InBarcodeCuaBenhNhan(inBarcodeDaCapCodeBenhNhan);
        }
        #endregion
    }
}
