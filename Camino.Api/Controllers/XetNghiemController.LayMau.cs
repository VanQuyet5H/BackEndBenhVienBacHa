using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.XetNghiem;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class XetNghiemController
    {
        #region grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridYeuCauTiepNhanCanLayMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _xetNghiemService.GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsync(queryInfo);
            //var gridData = await _xetNghiemService.GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer2(queryInfo);
            //var gridData = await _xetNghiemService.GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer3(queryInfo);
            var gridData = await _xetNghiemService.GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer4(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _xetNghiemService.GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsync(queryInfo);
            //var gridData = await _xetNghiemService.GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer2(queryInfo);
            //var gridData = await _xetNghiemService.GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer3(queryInfo);
            var gridData = await _xetNghiemService.GetTotalPageForGridYeuCauTiepNhanCanLayMauXetNghiemAsyncVer4(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNhomCanLayMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNhomCanLayMauXetNghiemAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetDataForGridNhomCanLayMauXetNghiemAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridNhomCanLayMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNhomCanLayMauXetNghiemAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _xetNghiemService.GetTotalPageForGridNhomCanLayMauXetNghiemAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridDichVuCanLayMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDichVuCanLayMauXetNghiemAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _xetNghiemService.GetDataForGridDichVuCanLayMauXetNghiemAsync(queryInfo);
            var gridData = await _xetNghiemService.GetDataForGridDichVuCanLayMauXetNghiemAsyncVer2(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDichVuCanLayMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDichVuCanLayMauXetNghiemAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _xetNghiemService.GetTotalPageForGridDichVuCanLayMauXetNghiemAsync(queryInfo);
            var gridData = await _xetNghiemService.GetTotalPageForGridDichVuCanLayMauXetNghiemAsyncVer2(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Export excel
        [HttpPost("ExportDanhSachLayMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> ExportDanhSachLayMauXetNghiemAsync(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var yeuCauTiepNhanCanXetNghiem = await _xetNghiemService.GetDataForGridYeuCauTiepNhanCanLayMauXetNghiemAsync(queryInfo);
            if (yeuCauTiepNhanCanXetNghiem == null || yeuCauTiepNhanCanXetNghiem.Data.Count == 0)
            {
                return NoContent();
            }

            //var datas = (List<LayMauXetNghiemYeuCauTiepNhanGridVo>)yeuCauTiepNhanCanXetNghiem.Data;
            var datas = yeuCauTiepNhanCanXetNghiem.Data.Cast<LayMauXetNghiemYeuCauTiepNhanGridVo>().ToList();
            foreach (var yeuCauTiepNhan in datas)
            {
                // add nhóm dịch vụ xét nghiệm
                var queryNhom = new QueryInfo()
                {
                    Skip = queryInfo.Skip,
                    Take = queryInfo.Take,
                    AdditionalSearchString = queryInfo.AdditionalSearchString
                };
                queryNhom.SearchString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("<AdvancedQueryParameters><SearchTerms>" + yeuCauTiepNhan.Id + "</SearchTerms></AdvancedQueryParameters>"));
                var dataNhoms = await _xetNghiemService.GetDataForGridNhomCanLayMauXetNghiemAsync(queryNhom);
                yeuCauTiepNhan.NhomCanLayMauXetNghiems = dataNhoms.Data.Cast<NhomCanLayMauXetNghiemGridVo>().ToList();

                foreach (var nhomXetNghiem in yeuCauTiepNhan.NhomCanLayMauXetNghiems)
                {
                    // add dịch vụ xét nghiệm
                    var queryDichVu = new QueryInfo()
                    {
                        Skip = queryInfo.Skip,
                        Take = queryInfo.Take
                    };
                    queryDichVu.AdditionalSearchString = nhomXetNghiem.YeuCauTiepNhanId + ";" + nhomXetNghiem.PhienXetNghiemId + ";" + nhomXetNghiem.NhomDichVuBenhVienId + ";" + (int)nhomXetNghiem.TrangThai;
                    var dataDichVus = await _xetNghiemService.GetDataForGridDichVuCanLayMauXetNghiemAsync(queryDichVu);
                    nhomXetNghiem.DichVuCanLayMauXetNghiems = dataDichVus.Data.Cast<DichVuCanLayMauXetNghiemGridVo>().ToList();
                }
            }

            var bytes = _excelService.ExportDanhSachLayMauXetNghiem(datas, queryInfo.AdditionalSearchString);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachLayMauXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        #endregion

        #region Get data
        [HttpGet("GetDanhThongTinYeuCauTiepNhanLayMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<ThongTinYeuCauTiepNhanLayMauVo>> GetDanhThongTinYeuCauTiepNhanLayMauAsync(long yeuCauTiepNhanId)
        {
            var chiTiet = await _xetNghiemService.GetDanhThongTinYeuCauTiepNhanLayMauAsync(yeuCauTiepNhanId);
            return Ok(chiTiet);
        }

        [HttpPost("GetListBarcodeTheoYeuCauTiepNhan")]
        public async Task<ActionResult<List<LookupItemTemplateVo>>> GetListBarcodeTheoYeuCauTiepNhanAsync([FromBody]DropDownListRequestModel model)
        {
            var list = await _xetNghiemService.GetListBarcodeTheoYeuCauTiepNhanAsync(model);
            return Ok(list);
        }

        [HttpPost("KiemTraBarcodeDangChon")]
        public async Task<ActionResult<LookupItemVo>> KiemTraBarcodeDangChonAsync(KiemTraBarcodeLayMauXetNghiemViewModel kiemTra)
        {
            var strQuery = kiemTra.YeuCauTiepNhanId + ";" + ((kiemTra.IsCapMoi && kiemTra.IsCapBarcodeChoDichVu != true) ? "" : kiemTra.BarcodeNumber) + ";" + kiemTra.BarcodeString;
            var barcode = await _xetNghiemService.KiemTraBarcodeDangChonAsync(strQuery);
            return Ok(barcode);
        }

        [HttpGet("GetLichSuTuChoiMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<List<LichSuTuChoiMauVo>> GetLichSuTuChoiMauAsync(long yeuCauTiepNhanId)
        {
            var lichSu = await _xetNghiemService.GetLichSuTuChoiMauAsync(yeuCauTiepNhanId);
            return lichSu;
        }

        [HttpPost("GetListPhongBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListPhongBenhVienAsync(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauKhamBenhService.GetListPhongBenhVienAsync(model);
            return Ok(lookup);
        }


        [HttpPost("InBarcodeLayMauXetNghiem")]
        public async Task<ActionResult<string>> InBarcodeLayMauXetNghiemAsync(LayMauXetNghiemInBarcodeVo model)
        {
            var content = await _xetNghiemService.InBarcodeLayMauXetNghiemAsync(model);
            return Ok(content);
        }
        #endregion

        #region Xử lý tương tác data
        [HttpPut("XuLyLayMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> XuLyLayMauXetNghiemAsync(LayMauXetNghiemViewModel layMauXetNghiemViewModel)
        {
            if (layMauXetNghiemViewModel.BarcodeNumber == null)
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Required"));
            }

            if (layMauXetNghiemViewModel.BarcodeNumber.ToString().Length > 4)
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Length"));
            }

            // xử lý tạo phiên xét nghiệm (dựa vào barcode để tạo hoặc cập nhật)
            var layMauXetNghiemVo = new LayMauXetNghiemXacNhanLayMauVo()
            {
                YeuCauTiepNhanId = layMauXetNghiemViewModel.YeuCauTiepNhanId,
                BenhNhanId = layMauXetNghiemViewModel.BenhNhanId,
                PhienXetNghiemId = layMauXetNghiemViewModel.PhienXetNghiemId,
                NhomDichVuBenhVienId = layMauXetNghiemViewModel.NhomDichVuBenhVienId,
                BarcodeNumber = layMauXetNghiemViewModel.BarcodeNumber.Value,
                BarcodeId = layMauXetNghiemViewModel.BarcodeId
            };
            await _xetNghiemService.XuLyLayMauXetNghiemAsync(layMauXetNghiemVo);
            return Ok();
        }

        [HttpPut("XuLyLayLaiMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> XuLyLayLaiMauXetNghiemAsync(LayMauXetNghiemViewModel layMauXetNghiemViewModel)
        {
            // xử lý tạo phiên xét nghiệm (dựa vào barcode để tạo hoặc cập nhật)
            var layMauXetNghiemVo = new LayMauXetNghiemXacNhanLayMauVo()
            {
                YeuCauTiepNhanId = layMauXetNghiemViewModel.YeuCauTiepNhanId,
                BenhNhanId = layMauXetNghiemViewModel.BenhNhanId,
                PhienXetNghiemId = layMauXetNghiemViewModel.PhienXetNghiemId,
                NhomDichVuBenhVienId = layMauXetNghiemViewModel.NhomDichVuBenhVienId,
                //BarcodeNumber = layMauXetNghiemViewModel.BarcodeNumber.Value,
                //BarcodeId = layMauXetNghiemViewModel.BarcodeId
            };
            await _xetNghiemService.XuLyLayLaiMauXetNghiemAsync(layMauXetNghiemVo);
            return Ok();
        }

        [HttpPut("XuLyBenhNhanNhanKetQua")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> XuLyBenhNhanNhanKetQuaAsync(long yeuCauTiepNhanId)
        {
            await _xetNghiemService.XuLyBenhNhanNhanKetQuaAsync(yeuCauTiepNhanId);
            return Ok();
        }

        [HttpPost("XuLyGuiMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.GoiMauXetNghiem)]
        public async Task<ActionResult<string>> XuLyGuiMauXetNghiemAsync([FromBody]PhieuGuiMauXetNghiemViewModel phieuGuiMauXetNghiem)
        {
            if (!phieuGuiMauXetNghiem.NhomMauGuis.Any())
            {
                throw new ApiException(_localizationService.GetResource("GuiMau.NhomMauGuis.Required"));
            }
            var phieuGuiMauVo = new GuiMauXetNghiemVo()
            {
                NhanVienGuiMauId = phieuGuiMauXetNghiem.NhanVienGuiMauId,
                ThoiDiemGuiMau = phieuGuiMauXetNghiem.ThoiDiemGuiMau,
                PhongNhanMauId = phieuGuiMauXetNghiem.PhongNhanMauId,
                GhiChu = phieuGuiMauXetNghiem.GhiChu
            };
            foreach (var nhomMau in phieuGuiMauXetNghiem.NhomMauGuis)
            {
                var newNhomMau = new LayMauXetNghiemVo()
                {
                    YeuCauTiepNhanId = nhomMau.YeuCauTiepNhanId,
                    BenhNhanId = nhomMau.BenhNhanId,
                    PhienXetNghiemId = nhomMau.PhienXetNghiemId,
                    NhomDichVuBenhVienId = nhomMau.NhomDichVuBenhVienId,
                    BarcodeNumber = nhomMau.BarcodeNumber,
                    BarcodeId = nhomMau.BarcodeId
                };
                phieuGuiMauVo.NhomMauGuis.Add(newNhomMau);
            }

            var phieuGuiMauId = await _xetNghiemService.XuLyGuiMauXetNghiemAsync(phieuGuiMauVo);
            var inPhieuVo = new InPhieuDGuimauXetNghiemVo()
            {
                PhieuGuiMauId = phieuGuiMauId,
                HasHeader = true
            };
            var phieuGuiMau = await _xetNghiemService.InPhieuGuiMauXetNghiemAsync(inPhieuVo);
            return phieuGuiMau;
        }
        #endregion

        #region Xử lý lấy, gửi và nhận mẫu 1 lần
        [HttpPut("XuLyLayGuiVaNhanMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<string>> XuLyLayGuiVaNhanMauXetNghiemAsync(LayMauXetNghiemViewModel layMauXetNghiemViewModel)
        {
            #region Xử lý lấy mẫu
            if (layMauXetNghiemViewModel.BarcodeNumber == null)
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Required"));
            }

            if (layMauXetNghiemViewModel.BarcodeNumber.ToString().Length > 4)
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Length"));
            }

            // xử lý tạo phiên xét nghiệm (dựa vào barcode để tạo hoặc cập nhật)
            var layMauXetNghiemVo = new LayMauXetNghiemXacNhanLayMauVo()
            {
                YeuCauTiepNhanId = layMauXetNghiemViewModel.YeuCauTiepNhanId,
                BenhNhanId = layMauXetNghiemViewModel.BenhNhanId,
                PhienXetNghiemId = layMauXetNghiemViewModel.PhienXetNghiemId,
                NhomDichVuBenhVienId = layMauXetNghiemViewModel.NhomDichVuBenhVienId,
                BarcodeNumber = layMauXetNghiemViewModel.BarcodeNumber.Value,
                BarcodeId = layMauXetNghiemViewModel.BarcodeId
            };

            await _xetNghiemService.XuLyLayMauXetNghiemAsync(layMauXetNghiemVo);
            var phieuGuiMauId = await _xetNghiemService.XuLyGuiVaNhanMauXetNghiemAsync(layMauXetNghiemVo);
            #endregion

            var inPhieuVo = new InPhieuDGuimauXetNghiemVo()
            {
                PhieuGuiMauId = phieuGuiMauId,
                HasHeader = true
            };
            var phieuGuiMau = await _xetNghiemService.InPhieuGuiMauXetNghiemAsync(inPhieuVo);
            return phieuGuiMau;
        }

        [HttpPut("XuLyLayLaiGuiVaNhanMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<string>> XuLyLayLaiGuiVaNhanMauXetNghiemAsync(LayMauXetNghiemViewModel layMauXetNghiemViewModel)
        {
            // xử lý tạo phiên xét nghiệm (dựa vào barcode để tạo hoặc cập nhật)
            var layMauXetNghiemVo = new LayMauXetNghiemXacNhanLayMauVo()
            {
                YeuCauTiepNhanId = layMauXetNghiemViewModel.YeuCauTiepNhanId,
                BenhNhanId = layMauXetNghiemViewModel.BenhNhanId,
                PhienXetNghiemId = layMauXetNghiemViewModel.PhienXetNghiemId,
                NhomDichVuBenhVienId = layMauXetNghiemViewModel.NhomDichVuBenhVienId,
                //BarcodeNumber = layMauXetNghiemViewModel.BarcodeNumber.Value,
                BarcodeId = layMauXetNghiemViewModel.BarcodeId
            };
            await _xetNghiemService.XuLyLayLaiMauXetNghiemAsync(layMauXetNghiemVo);
            var phieuGuiMauId = await _xetNghiemService.XuLyGuiVaNhanMauXetNghiemAsync(layMauXetNghiemVo);
            var inPhieuVo = new InPhieuDGuimauXetNghiemVo()
            {
                PhieuGuiMauId = phieuGuiMauId,
                HasHeader = true
            };
            var phieuGuiMau = await _xetNghiemService.InPhieuGuiMauXetNghiemAsync(inPhieuVo);
            return phieuGuiMau;
        }

        [HttpPut("XuLyHuyMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> XuLyHuyMauXetNghiemAsync(LayMauXetNghiemViewModel layMauXetNghiemViewModel)
        {
            // xử lý tạo phiên xét nghiệm (dựa vào barcode để tạo hoặc cập nhật)
            var layMauXetNghiemVo = new LayMauXetNghiemXacNhanLayMauVo()
            {
                YeuCauTiepNhanId = layMauXetNghiemViewModel.YeuCauTiepNhanId,
                BenhNhanId = layMauXetNghiemViewModel.BenhNhanId,
                PhienXetNghiemId = layMauXetNghiemViewModel.PhienXetNghiemId,
                NhomDichVuBenhVienId = layMauXetNghiemViewModel.NhomDichVuBenhVienId,
                //BarcodeNumber = layMauXetNghiemViewModel.BarcodeNumber.Value,
                BarcodeId = layMauXetNghiemViewModel.BarcodeId
            };
            await _xetNghiemService.XuLyHuyMauXetNghiemAsync(layMauXetNghiemVo);
            return Ok();
        }
        #endregion

        #region Kết quả xét nghiệm

        [HttpPost("LayPhieuXNTheoYeuCauKyThuatVaNhomAsync")]
        public ActionResult<List<HtmlToPdfVo>> LayPhieuXNTheoYeuCauKyThuatVaNhomAsync(LayPhieuXetNghiemTheoYCKTVaNhomDVBVVo phieuInVo)
        {
            var phieuXns = new List<HtmlToPdfVo>();

            var typeSize = "A4";
            var typeLayout = "portrait";

            var lstHtml = new List<string>();
            var result = _duyetKetQuaXetNghiemService.YeuCauDichVuKyThuatIdTheoPhienXetNghiemSars(phieuInVo);

            var phieuInItems = new List<PhieuInXetNghiemModel>();

            if (result.XetNghiemThuocNhomSarsCov.Count() > 0)
            {
                var groupTheoPhien = result.XetNghiemThuocNhomSarsCov
                                    .GroupBy(d => d.PhienXetNghiemId)
                                    .Select(d => new
                                    {
                                        PhienXetNghiemId = d.First().PhienXetNghiemId,
                                        YeuCauDichVuKyThuatIds = d.Select(f => f.YeuCauDichVuKyThuatId).ToList(),
                                    }).ToList();

                foreach (var itemPhien in groupTheoPhien)
                {
                    var phieuIn = _duyetKetQuaXetNghiemService.InDiChXetNghiemTestNhanhKhangNguyenSarsCoV2(itemPhien.YeuCauDichVuKyThuatIds, itemPhien.PhienXetNghiemId, phieuInVo.HostingName);
                    phieuInItems.AddRange(phieuIn);
                }
            }
            if(result.XetNghiemKhongThuocNhomSarsCov.Count() > 0)
            {
                var phieuInItem = _duyetKetQuaXetNghiemService.InPhieuXetNghiemTheoYeuCauKyThuatVaNhomNew(phieuInVo);
                phieuInItems.AddRange(phieuInItem);
            }
            
            
            var i = 0;
            foreach (var phieuIn in phieuInItems)
            {
                var htmlContent = "";
                htmlContent +=
                    "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" + typeSize + " " + typeLayout + ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
                htmlContent += "</head><body>";
                htmlContent += phieuIn.Html;
                i++;

                htmlContent += "</body></html>";
                lstHtml.Add(htmlContent);
            }

            var list = new List<HtmlToPdfVo>();
            var footerHtml = string.Empty;
            string ngayGioHienTai = DateTime.Now.ApplyFormatDateTimeSACH();
            string classing = "luv";

            foreach (var itemHTML in lstHtml)
            {
                footerHtml = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <script charset='utf-8'>

                    function replaceParams() {
                      var url = window.location.href
                        .replace(/#$/, '');
                      var params = (url.split('?')[1] || '').split('&');
                      for (var i = 0; i < params.length; i++) {
                          var param = params[i].split('=');
                          var key = param[0];
                          var value = param[1] || '';
                          var regex = new RegExp('{' + key + '}', 'g');
                          document.body.innerText = document.body.innerText.replace(regex, value);
                      }
                    }
                    </script>
                </head>
                <body onload='replaceParams()'> " + classing +
                ngayGioHienTai + classing
                      + "Trang {page}/{topage}</body></html>";
                var htmlToPdfVo = new HtmlToPdfVo
                {
                    Html = itemHTML,
                    FooterHtml = footerHtml,
                    Bottom = 7
                };
                list.Add(htmlToPdfVo);
            }
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.FooterHtml))
                {
                    var kyTu = item.FooterHtml.Split(classing);
                    if (kyTu.Length > 2)
                    {
                        item.FooterHtml = kyTu[0] + footerPhieuIn(kyTu[1], kyTu[2]);
                    }
                }

            }

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TongHopPhieu" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf"); ;
        }

        [HttpPost("InKetQuaXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult<List<PhieuInXetNghiemModel>>> InKetQuaXetNghiemAsync(LayMauXetNghiemInKetQuaVo phieuInVo)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(phieuInVo.YeuCauTiepNhanId, x => x.Include(y => y.PhienXetNghiems));
            if (!yeuCauTiepNhan.PhienXetNghiems.Any() || yeuCauTiepNhan.PhienXetNghiems.Any(x => x.NhanVienKetLuanId == null))
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.KetQua.NotEnough"));
            }

            var lstPhieuIn = new List<PhieuInXetNghiemModel>();
            foreach (var phienXetNghiem in yeuCauTiepNhan.PhienXetNghiems)
            {
                DuyetKetQuaXetNghiemPhieuInVo ketQuaVo = new DuyetKetQuaXetNghiemPhieuInVo()
                {
                    Id = phienXetNghiem.Id,
                    HostingName = phieuInVo.HostingName,
                    Header = phieuInVo.Header
                };
                var phieuInItems = await _duyetKetQuaXetNghiemService.InDuyetKetQuaXetNghiem(ketQuaVo);
                lstPhieuIn.AddRange(phieuInItems);
            }

            return lstPhieuIn;
        }

        [HttpPost("GetAllHtmlPdfKetQuaXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<List<string>>> ExportPdfKetQuaXetNghiemAsync(LayMauXetNghiemInKetQuaVo phieuInVo)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(phieuInVo.YeuCauTiepNhanId, x => x.Include(y => y.PhienXetNghiems).ThenInclude(z => z.PhienXetNghiemChiTiets).Include(y => y.PhienXetNghiems).ThenInclude(z => z.NhanVienKetLuan));
            //if (!yeuCauTiepNhan.PhienXetNghiems.Any() || yeuCauTiepNhan.PhienXetNghiems.Any(x => x.NhanVienKetLuanId == null))
            //{
            //    throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.KetQua.NotEnough"));
            //}

            var lstPhieuIn = new List<PhieuInXetNghiemModel>();
            foreach (var phienXetNghiem in yeuCauTiepNhan.PhienXetNghiems.Where(o => (phieuInVo.PhienXetNghiemId == null || o.Id == phieuInVo.PhienXetNghiemId) && o.PhienXetNghiemChiTiets.Any(x => x.NhanVienKetLuanId != null)))
            {
                DuyetKetQuaXetNghiemPhieuInVo ketQuaVo = new DuyetKetQuaXetNghiemPhieuInVo()
                {
                    Id = phienXetNghiem.Id,
                    HostingName = phieuInVo.HostingName,
                    Header = phieuInVo.Header,
                    NhomDichVuBenhVienIds = phieuInVo.NhomDichVuBenhVienIds,
                    ListIn = phieuInVo.ListIn,
                    LoaiIn = phieuInVo.LoaiIn
                };
                var phieuInItems = await _duyetKetQuaXetNghiemService.InDuyetKetQuaXetNghiem(ketQuaVo);
                lstPhieuIn.AddRange(phieuInItems);
            }

            var lstHtml = new List<string>();

            var typeSize = "A4";
            var typeLayout = "portrait";

            var i = 0;
            foreach (var phieuIn in lstPhieuIn)
            {
                var htmlContent = "";
                htmlContent +=
                    "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" + typeSize + " " + typeLayout + ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
                htmlContent += "</head><body>";
                htmlContent += phieuIn.Html;
                i++;
                //                if (i < lstPhieuIn.Count)
                //                {
                //                    htmlContent += "<div class='pagebreak'></div>";
                //                }
                htmlContent += "</body></html>";
                lstHtml.Add(htmlContent);
            }
            return lstHtml;
        }

        [HttpPost("GetAllHtmlPdfKetQuaXetNghiemKetQua")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KetQuaXetNghiem)]
        public async Task<ActionResult<List<string>>> ExportPdfKetQuaXetNghiemKetQuaAsync(LayMauXetNghiemInKetQuaVo phieuInVo)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(phieuInVo.YeuCauTiepNhanId, x => x.Include(y => y.PhienXetNghiems));
            if (!yeuCauTiepNhan.PhienXetNghiems.Any() || yeuCauTiepNhan.PhienXetNghiems.Any(x => x.NhanVienKetLuanId == null))
            {
                throw new ApiException(_localizationService.GetResource("LayMauXetNghiem.KetQua.NotEnough"));
            }

            var lstPhieuIn = new List<PhieuInXetNghiemModel>();
            foreach (var phienXetNghiem in yeuCauTiepNhan.PhienXetNghiems.Where(o => phieuInVo.PhienXetNghiemId == null || o.Id == phieuInVo.PhienXetNghiemId))
            {
                DuyetKetQuaXetNghiemPhieuInVo ketQuaVo = new DuyetKetQuaXetNghiemPhieuInVo()
                {
                    Id = phienXetNghiem.Id,
                    HostingName = phieuInVo.HostingName,
                    Header = phieuInVo.Header
                };
                var phieuInItems = await _duyetKetQuaXetNghiemService.InKetQuaXetNghiem(ketQuaVo);
                lstPhieuIn.AddRange(phieuInItems);
            }

            var lstHtml = new List<string>();

            var typeSize = "A4";
            var typeLayout = "portrait";

            var i = 0;
            foreach (var phieuIn in lstPhieuIn)
            {
                var htmlContent = "";
                htmlContent +=
                    "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" + typeSize + " " + typeLayout + ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
                htmlContent += "</head><body>";
                htmlContent += phieuIn.Html;
                i++;
                //                if (i < lstPhieuIn.Count)
                //                {
                //                    htmlContent += "<div class='pagebreak'></div>";
                //                }
                htmlContent += "</body></html>";
                lstHtml.Add(htmlContent);
            }

            return lstHtml;

        }

        [HttpPost("GetFilePDFFromHtml")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public ActionResult GetFilePDFFromHtml(LayMauXetNghiemFileKetQuaViewModel htmlContent)
        {
            var footerHtml = string.Empty;
            if (htmlContent.LaPhieuKhamBenh == true)
            {
                footerHtml = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <script charset='utf-8'>
                    function hostfunction() {
                        replaceParams();
                        timeNow();
                    }
                    function replaceParams() {
                      var url = window.location.href
                        .replace(/#$/, '');
                      var params = (url.split('?')[1] || '').split('&');
                      for (var i = 0; i < params.length; i++) {
                          var param = params[i].split('=');
                          var key = param[0];
                          var value = param[1] || '';
                          var regex = new RegExp('{' + key + '}', 'g');
                          document.getElementById('total').innerHTML = document.body.innerText.replace(regex, value);
                      }
                    }
                        function timeNow() {
                                                  var today = new Date();
                                                  var day = today.getDate();
                                                  var month = today.getMonth() + 1;
                                                  var hour = today.getHours();
                                                  var minutes = today.getMinutes();
                                                   if(day < 10){
                                                        day = '0' + day;
                                                    };
                                                  if(month < 10){
                                                        month = '0' + month;
                                                    };
                                                  if(hour < 10){
                                                        hour = '0' + hour;
                                                    };
                                                  if(minutes < 10){
                                                        minutes = '0' + minutes;
                                                    };
                                                  var date = day+'/'+(month)+'/'+today.getFullYear();
                                                  var time = hour + ': ' + minutes;
                                                  document.getElementById('hvn').innerHTML = date + ' ' + time;
                                            }
                    
                    </script>
                </head>
                <body  onload='hostfunction()' >
                        <div id='hvn' style='float: left;  display: inline; width: 50%; '>
                        </div>
                        <div  id='total' style='float: left;display: inline; width: 50%; text-align: right'>
                        Trang {page}/{topage}
                        </div>  
                        <div style='clear: both; '></div>
                 </body>
                </html>";
            }
            else
            {
                footerHtml = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <script charset='utf-8'>

                    function replaceParams() {
                      var url = window.location.href
                        .replace(/#$/, '');
                      var params = (url.split('?')[1] || '').split('&');
                      for (var i = 0; i < params.length; i++) {
                          var param = params[i].split('=');
                          var key = param[0];
                          var value = param[1] || '';
                          var regex = new RegExp('{' + key + '}', 'g');
                          document.body.innerText = document.body.innerText.replace(regex, value);
                      }
                    }
                    </script>
                </head>
                <body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}
                </body>
                </html>";
            }


            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = htmlContent.Html,
                FooterHtml = footerHtml
            };
            var bytes = _pdfService.ExportFilePdfFromHtml(htmlToPdfVo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        #endregion

        [HttpPost("XemPhieuXetNghiem")]
        public async Task<ActionResult> XemPhieuXetNghiem(DuyetKetQuaXetNghiemPhieuInVo ketQuaVo)//XemPhieuXetNghiem
        {
            var result = await _duyetKetQuaXetNghiemService.InKetQuaXetNghiem(ketQuaVo);
            return Ok(result);
        }

        #region Cập nhật lấy mẫu
        [HttpPut("XuLyCapBarcodeChoDichhVuDangChon")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> XuLyCapBarcodeChoDichhVuDangChonAsync(CapBarcodeTheoDichVuViewModel capBarcodeTheoDichVuViewModel)
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
                YeuCauDichVuKyThuatIds = capBarcodeTheoDichVuViewModel.YeuCauDichVuKyThuatIds
            };
            await _xetNghiemService.XuLyCapBarcodeChoDichhVuDangChonAsync(capBarcodeTheoDichVuVo);
            return Ok();
        }

        [HttpPut("XuLyXacNhanNhanMauXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult<string>> XuLyXacNhanNhanMauXetNghiemAsync(XacNhanNhanMauChoDichVuViewModel xacNhanNhanMauViewModel) //long phienXetNghiemChiTietId
        {
            var xacNhanNhanMauVo = new XacNhanNhanMauChoDichVuVo()
            {
                YeuCauTiepNhanId = xacNhanNhanMauViewModel.YeuCauTiepNhanId,
                BenhNhanId = xacNhanNhanMauViewModel.BenhNhanId,
                YeuCauDichVuKyThuatIds = xacNhanNhanMauViewModel.YeuCauDichVuKyThuatIds
            };
            await _xetNghiemService.XuLyXacNhanNhanMauXetNghiemAsync(xacNhanNhanMauVo);
            return Ok();
        }

        [HttpPut("XuLyXacNhanHuyCapBarcodeTheoDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> XuLyXacNhanHuyCapBarcodeTheoDichVuAsync(XacNhanNhanMauChoDichVuViewModel xacNhanNhanMauViewModel)
        {
            var xacNhanNhanMauVo = new XacNhanNhanMauChoDichVuVo()
            {
                YeuCauTiepNhanId = xacNhanNhanMauViewModel.YeuCauTiepNhanId,
                BenhNhanId = xacNhanNhanMauViewModel.BenhNhanId,
                YeuCauDichVuKyThuatIds = xacNhanNhanMauViewModel.YeuCauDichVuKyThuatIds
            };
            await _xetNghiemService.XuLyXacNhanHuyCapBarcodeTheoDichVuAsync(xacNhanNhanMauVo);
            return Ok();
        }

        [HttpPut("CapNhatGridItemThoiGianNhanMau")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.LayMauXetNghiem)]
        public async Task<ActionResult> CapNhatGridItemThoiGianNhanMauAsync(CapNhatGridItemChoDichVuDaCapCodeViewModel capNhatNgayNhanMauViewModel)
        {
            var capNhatVo = new CapNhatGridItemChoDichVuDaCapCodeVo()
            {
                YeuCauTiepNhanId = capNhatNgayNhanMauViewModel.YeuCauTiepNhanId,
                YeuCauDichVuKyThuatId = capNhatNgayNhanMauViewModel.YeuCauDichVuKyThuatId,
                NgayNhanMau = capNhatNgayNhanMauViewModel.NgayNhanMau.Value
            };
            await _xetNghiemService.CapNhatGridItemThoiGianNhanMauAsync(capNhatVo);
            return Ok();
        }
        #endregion
        #region update phiếu in (15072021): chỉ in những dịch vụ được check trên grid
        [HttpPost("GetAllHtmlPdfDuyetKetQuaXetNghiemDuocCheckGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult<List<string>>> GetAllHtmlPdfDuyetKetQuaXetNghiemDuocCheckGrid(LayMauXetNghiemInKetQuaVo phieuInVo)
        {

            var phienXetNghiemIds = _duyetKetQuaXetNghiemService.GetListPhienXetNghiemIdChoIn(phieuInVo.YeuCauTiepNhanId, phieuInVo.PhienXetNghiemId);
            //var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(phieuInVo.YeuCauTiepNhanId, x => x.Include(y => y.PhienXetNghiems).ThenInclude(z => z.PhienXetNghiemChiTiets)
            //                                                                                                    .Include(y => y.PhienXetNghiems)
            //                                                                                                    .ThenInclude(z => z.NhanVienKetLuan).Include(d=>d.HopDongKhamSucKhoeNhanVien));
            
            var listYeuCauDichVuKyThuatThuocNhomSarsCoV2 = await _duyetKetQuaXetNghiemService.GetListYeuCauTrongNhomSars(phieuInVo.ListIn);
            
            var lstPhieuIn = new List<PhieuInXetNghiemModel>();
            foreach (var phienXetNghiemId in phienXetNghiemIds)
            {
                if(phieuInVo.ListIn.Where(d => !listYeuCauDichVuKyThuatThuocNhomSarsCoV2.Contains(d.YeuCauDichVuKyThuatId)).Count() > 0)
                {
                    DuyetKetQuaXetNghiemPhieuInVo ketQuaVo = new DuyetKetQuaXetNghiemPhieuInVo()
                    {
                        Id = phienXetNghiemId,
                        HostingName = phieuInVo.HostingName,
                        Header = phieuInVo.Header,
                        NhomDichVuBenhVienIds = phieuInVo.NhomDichVuBenhVienIds,
                        ListIn = phieuInVo.ListIn.Where(d => !listYeuCauDichVuKyThuatThuocNhomSarsCoV2.Contains(d.YeuCauDichVuKyThuatId)).ToList(),
                        LoaiIn = phieuInVo.LoaiIn
                    };
                    var phieuInItems = await _duyetKetQuaXetNghiemService.InDuyetKetQuaXetNghiemManHinhDuyet(ketQuaVo);
                    lstPhieuIn.AddRange(phieuInItems);
                }
                #region BVHD-3761
                if (listYeuCauDichVuKyThuatThuocNhomSarsCoV2.ToList().Count() != 0)
                {
                    var phieuInItems = _duyetKetQuaXetNghiemService.InDiChXetNghiemTestNhanhKhangNguyenSarsCoV2(listYeuCauDichVuKyThuatThuocNhomSarsCoV2, phienXetNghiemId, phieuInVo.HostingName);
                    lstPhieuIn.AddRange(phieuInItems);
                }
                #endregion
            }






            //var stt = yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
            var lstHtml = new List<string>();

            var typeSize = "A4";
            var typeLayout = "portrait";

            var i = 0;
            foreach (var phieuIn in lstPhieuIn)
            {
                var htmlContent = "";
                htmlContent +=
                    "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" + typeSize + " " + typeLayout + ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
                htmlContent += "</head><body>";
                htmlContent += phieuIn.Html;
                i++;
                //                if (i < lstPhieuIn.Count)
                //                {
                //                    htmlContent += "<div class='pagebreak'></div>";
                //                }
                htmlContent += "</body></html>";
                lstHtml.Add(htmlContent);
            }
            var list = new List<HtmlToPdfVo>();
            var footerHtml = string.Empty;
            string ngayGioHienTai = DateTime.Now.ApplyFormatDateTimeSACH();
            string classing = "<div style='width: 34px; line-height:34px;border-radius:50 %; text-align:center;font-size:24px;border:2px solid #666;float:left;display:inline;width:5%'>30</div>";
            foreach (var itemHTML in lstHtml)
            {
                footerHtml = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <script charset='utf-8'>
                    function hostfunction() {
                        replaceParams();
                        timeNow();
                    }
                    function replaceParams() {
                      var url = window.location.href
                        .replace(/#$/, '');
                      var params = (url.split('?')[1] || '').split('&');
                      for (var i = 0; i < params.length; i++) {
                          var param = params[i].split('=');
                          var key = param[0];
                          var value = param[1] || '';
                          var regex = new RegExp('{' + key + '}', 'g');
                          document.getElementById('total').innerHTML = document.body.innerText.replace(regex, value);
                      }
                    }
                        function timeNow() {
                                                  var today = new Date();
                                                  var day = today.getDate();
                                                  var month = today.getMonth() + 1;
                                                  var hour = today.getHours();
                                                  var minutes = today.getMinutes();
                                                   if(day < 10){
                                                        day = '0' + day;
                                                    };
                                                  if(month < 10){
                                                        month = '0' + month;
                                                    };
                                                  if(hour < 10){
                                                        hour = '0' + hour;
                                                    };
                                                  if(minutes < 10){
                                                        minutes = '0' + minutes;
                                                    };
                                                  var date = day+'/'+(month)+'/'+today.getFullYear();
                                                  var time = hour + ': ' + minutes;
                                                  document.getElementById('hvn').innerHTML =date + ' ' + time;
                                            }
                    </script>
                </head>
                <body  onload='hostfunction()' >
                       <div id='hvn' style='float: left; display: inline; width: 50%'>
                        </div>
                        <div id='total' style='float: left;display: inline; width: 50%; text-align: right'>
                        Trang {page}/{topage}
                        </div>
                        <div style='clear: both; '></div>
                 </body>
                </html>";


                var htmlToPdfVo = new HtmlToPdfVo
                {
                    Html = itemHTML,
                    FooterHtml = footerHtml,
                    Bottom = 7
                };
                list.Add(htmlToPdfVo);
            }
            

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + "KetQuaXetNghiem" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }
        [HttpPost("GetAllHtmlPdfDuyetKetQuaXetNghiemDuocCheckGridOld")]
        public async Task<ActionResult<List<string>>> GetAllHtmlPdfDuyetKetQuaXetNghiemDuocCheckGridOld(LayMauXetNghiemInKetQuaVo phieuInVo)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(phieuInVo.YeuCauTiepNhanId, x => x.Include(y => y.PhienXetNghiems).ThenInclude(z => z.PhienXetNghiemChiTiets)
                                                                                                                .Include(y => y.PhienXetNghiems)
                                                                                                                .ThenInclude(z => z.NhanVienKetLuan).Include(d => d.HopDongKhamSucKhoeNhanVien));

            var listYeuCauDichVuKyThuatThuocNhomSarsCoV2 = await _duyetKetQuaXetNghiemService.GetListYeuCauTrongNhomSars(phieuInVo.ListIn);

            var lstPhieuIn = new List<PhieuInXetNghiemModel>();
            foreach (var phienXetNghiem in yeuCauTiepNhan.PhienXetNghiems.Where(o => (phieuInVo.PhienXetNghiemId == null || o.Id == phieuInVo.PhienXetNghiemId) && o.PhienXetNghiemChiTiets.Any(x => x.NhanVienKetLuanId != null)))
            {
                if (phieuInVo.ListIn.Where(d => !listYeuCauDichVuKyThuatThuocNhomSarsCoV2.Contains(d.YeuCauDichVuKyThuatId)) != null)
                {
                    DuyetKetQuaXetNghiemPhieuInVo ketQuaVo = new DuyetKetQuaXetNghiemPhieuInVo()
                    {
                        Id = phienXetNghiem.Id,
                        HostingName = phieuInVo.HostingName,
                        Header = phieuInVo.Header,
                        NhomDichVuBenhVienIds = phieuInVo.NhomDichVuBenhVienIds,
                        ListIn = phieuInVo.ListIn.Where(d => !listYeuCauDichVuKyThuatThuocNhomSarsCoV2.Contains(d.YeuCauDichVuKyThuatId)).ToList(),
                        LoaiIn = phieuInVo.LoaiIn
                    };
                    var phieuInItems = await _duyetKetQuaXetNghiemService.InDuyetKetQuaXetNghiemManHinhDuyet(ketQuaVo);
                    lstPhieuIn.AddRange(phieuInItems);
                }
                #region BVHD-3761
                if (listYeuCauDichVuKyThuatThuocNhomSarsCoV2.ToList().Count() != 0)
                {
                    var phieuInItems = _duyetKetQuaXetNghiemService.InDiChXetNghiemTestNhanhKhangNguyenSarsCoV2(listYeuCauDichVuKyThuatThuocNhomSarsCoV2, phienXetNghiem.Id, phieuInVo.HostingName);
                    lstPhieuIn.AddRange(phieuInItems);
                }
                #endregion
            }






            var stt = yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null && yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien != null ? $"<p class='round-sttNhanVien' style='margin:0;padding:0'>{yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.STTNhanVien.ToString()}</p>" : string.Empty;
            var lstHtml = new List<string>();

            var typeSize = "A4";
            var typeLayout = "portrait";

            var i = 0;
            foreach (var phieuIn in lstPhieuIn)
            {
                var htmlContent = "";
                htmlContent +=
                    "<html><head><title>Kết quả</title><style>*{ box-sizing: border-box;} @media print {@page{size:" + typeSize + " " + typeLayout + ";} .pagebreak {clear: both;page-break-after: always;}}</style><link href='https:///fonts.googleapis.com//css?family=Libre Barcode 39' rel='stylesheet'>";
                htmlContent += "</head><body>";
                htmlContent += phieuIn.Html;
                i++;
                //                if (i < lstPhieuIn.Count)
                //                {
                //                    htmlContent += "<div class='pagebreak'></div>";
                //                }
                htmlContent += "</body></html>";
                lstHtml.Add(htmlContent);
            }
            var list = new List<HtmlToPdfVo>();
            var footerHtml = string.Empty;
            string ngayGioHienTai = DateTime.Now.ApplyFormatDateTimeSACH();
            string classing = "<div style='width: 34px; line-height:34px;border-radius:50 %; text-align:center;font-size:24px;border:2px solid #666;float:left;display:inline;width:5%'>30</div>";
            foreach (var itemHTML in lstHtml)
            {
                footerHtml = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <script charset='utf-8'>
                    function hostfunction() {
                        replaceParams();
                        timeNow();
                    }
                    function replaceParams() {
                      var url = window.location.href
                        .replace(/#$/, '');
                      var params = (url.split('?')[1] || '').split('&');
                      for (var i = 0; i < params.length; i++) {
                          var param = params[i].split('=');
                          var key = param[0];
                          var value = param[1] || '';
                          var regex = new RegExp('{' + key + '}', 'g');
                          document.getElementById('total').innerHTML = document.body.innerText.replace(regex, value);
                      }
                    }
                        function timeNow() {
                                                  var today = new Date();
                                                  var day = today.getDate();
                                                  var month = today.getMonth() + 1;
                                                  var hour = today.getHours();
                                                  var minutes = today.getMinutes();
                                                   if(day < 10){
                                                        day = '0' + day;
                                                    };
                                                  if(month < 10){
                                                        month = '0' + month;
                                                    };
                                                  if(hour < 10){
                                                        hour = '0' + hour;
                                                    };
                                                  if(minutes < 10){
                                                        minutes = '0' + minutes;
                                                    };
                                                  var date = day+'/'+(month)+'/'+today.getFullYear();
                                                  var time = hour + ': ' + minutes;
                                                  document.getElementById('hvn').innerHTML =date + ' ' + time;
                                            }
                    </script>
                </head>
                <body  onload='hostfunction()' >
                       <div id='hvn' style='float: left; display: inline; width: 50%'>
                        </div>
                        <div id='total' style='float: left;display: inline; width: 50%; text-align: right'>
                        Trang {page}/{topage}
                        </div>
                        <div style='clear: both; '></div>
                 </body>
                </html>";


                var htmlToPdfVo = new HtmlToPdfVo
                {
                    Html = itemHTML,
                    FooterHtml = footerHtml,
                    Bottom = 7
                };
                list.Add(htmlToPdfVo);
            }


            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + "KetQuaXetNghiem" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        private string footerPhieuIn(string ngayGioHienTai, string page)
        {
            string htmlFooter = string.Empty;
            htmlFooter += "<table width = '100%'><tr><td style ='text-align: left;width:50%;'>"  + ngayGioHienTai + "</td>";
            for (var item = 0; item < 9; item++)
            {
                htmlFooter += "<td>" + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;" + "</td>";
            }
            htmlFooter += "<td style ='text-align: right;width: 50 %;'>" + page + "</td></tr></table>";
            return htmlFooter;

        }
        #endregion
    }
}
