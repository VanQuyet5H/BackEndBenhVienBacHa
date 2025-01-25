using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDanhSachKetLuanKhamSucKhoe")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhSachKetLuanKhamSucKhoe([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncDanhSachKetLuanKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDanhSachKetLuanKhamSucKhoe")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhSachKetLuanKhamSucKhoe([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalPageForGridAsyncDanhSachKetLuanKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetGoiKhamSucKhoeDoanYeuCauTNYeuCauTN")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<KetLuanKhamSucKhoeDoanViewModel> GetGoiKhamSucKhoeDoanYeuCauTN(KetLuanKhamSucKhoeDoanChiTietVo khoeDoanChiTietVo)
        {
            var entity = _khamDoanService.GetYeuCauTiepNhan(khoeDoanChiTietVo.Id, khoeDoanChiTietVo.HopDongKhamSucKhoeNhanVienId);
            var viewModel = entity.ToModel<KetLuanKhamSucKhoeDoanViewModel>();
            if (entity.YeuCauKhamBenhs.Any(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                            && p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa)
             && entity.YeuCauKhamBenhs.Any(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                            && p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa)
             && entity.YeuCauKhamBenhs.Any(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                             && p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat)
             && entity.YeuCauKhamBenhs.Any(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                             && p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong)
             && entity.YeuCauKhamBenhs.Any(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                             && p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat)
             && entity.YeuCauKhamBenhs.Any(p => p.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && p.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                             && p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.DaLieu)
                                             )
            {
                viewModel.CoHienThiPhanLoai = true;
            }
            //if (
            //      !entity.YeuCauKhamBenhs.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NoiKhoa)
            //   || !entity.YeuCauKhamBenhs.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.NgoaiKhoa)
            //   || !entity.YeuCauKhamBenhs.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.Mat)
            //   || !entity.YeuCauKhamBenhs.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.TaiMuiHong)
            //   || !entity.YeuCauKhamBenhs.Any(p => p.ChuyenKhoaKhamSucKhoe == ChuyenKhoaKhamSucKhoe.RangHamMat))
            //{
            //    viewModel.IsHiddenKetLuan = true;
            //}

            if (!string.IsNullOrEmpty(entity.KSKKetLuanPhanLoaiSucKhoe))
            {
                viewModel.PhanLoaiSucKhoeId = EnumHelper.GetValueFromDescription<PhanLoaiSucKhoe>(entity.KSKKetLuanPhanLoaiSucKhoe);
            }
            viewModel.DichVuKhamChuaKhams.AddRange(entity.YeuCauKhamBenhs.Where(p => p.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham).Select(p => p.TenDichVu));
            viewModel.DichVuKyThuatChuaThucHiens.AddRange(entity.YeuCauDichVuKyThuats.Where(p => p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien).Select(p => p.TenDichVu));
            if (viewModel.DichVuKhamChuaKhams.Any() || viewModel.DichVuKyThuatChuaThucHiens.Any())
            {
                viewModel.CoDichVuChuaKham = true;
            }
            viewModel.KetLuanKhamSucKhoeDoanDichVuKhamTemplates = _khamDoanService.KetLuanKhamSucKhoeDoanDichVuKhamVos(entity);
            viewModel.CongTyKhamSucKhoeId = entity.HopDongKhamSucKhoeNhanVien != null && entity.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe != null ? entity.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId : 0;
            viewModel.HopDongKhamSucKhoeId = entity.HopDongKhamSucKhoeNhanVien != null ? entity.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId : 0;
            return Ok(viewModel);
        }

        [HttpPost("LuuHoacLuuVaHoanThanhKhamKetLuanKSKDoan")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan)]
        public async Task<ActionResult> LuuHoacLuuVaHoanThanhKhamKetLuanKSKDoan(KetLuanKhamSucKhoeDoanViewModel viewModel)
        {
            var entity = _khamDoanService.GetYeuCauTiepNhan(viewModel.Id, viewModel.HopDongKhamSucKhoeId);
            if (entity.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
            {
                throw new Exception(_localizationService.GetResource("YeuCauTiepNhan.Done"));
            }
            if (!viewModel.IsOnlySave)
            {
                foreach (var item in entity.YeuCauKhamBenhs.Where(z => z.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham && z.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham))
                {
                    if (item.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham)
                    {
                        item.TrangThai = EnumTrangThaiYeuCauKhamBenh.DaKham;
                        item.ThoiDiemHoanThanh = DateTime.Now;
                    }
                    else if(item.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                    {
                        item.WillDelete = true;
                    }
                    var lichSuNew = new YeuCauKhamBenhLichSuTrangThai
                    {
                        TrangThaiYeuCauKhamBenh = item.TrangThai,
                        MoTa = item.TrangThai.GetDescription(),
                    };
                    item.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
                }

                foreach (var item in entity.YeuCauDichVuKyThuats.Where(z => z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                {
                    if (item.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                    {
                        item.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                    }
                    else
                    {
                        item.WillDelete = true;
                    }
                }
                var phongBenhVienHangDois = entity.YeuCauKhamBenhs.SelectMany(p => p.PhongBenhVienHangDois).ToList();
                foreach (var item in phongBenhVienHangDois)
                {
                    item.WillDelete = true;
                }
                viewModel.TrangThaiYeuCauTiepNhan = EnumTrangThaiYeuCauTiepNhan.DaHoanTat;
                viewModel.KSKNhanVienKetLuanId = _userAgentHelper.GetCurrentUserId();
                viewModel.KSKThoiDiemKetLuan = DateTime.Now;
            }
            viewModel.KSKKetLuanPhanLoaiSucKhoe = viewModel.PhanLoaiSucKhoeId.GetDescription();
            viewModel.ToEntity(entity);
            if (!viewModel.IsOnlySave)
            {
                await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(entity);
            }
            else
            {
                await _yeuCauTiepNhanService.UpdateAsync(entity);
            }
            return Ok(entity.LastModified);
        }

        [HttpPost("GetKetQuaMaus")]
        public async Task<ActionResult> GetKetQuaMau(KetLuanKhamSucKhoeDoanViewModel viewModel)//GetKetQuaMaus
        {
            var entity = _khamDoanService.GetYeuCauTiepNhan(viewModel.Id, viewModel.HopDongKhamSucKhoeId);
            var result = _khamDoanService.GetKetQuaMau(entity);
            return Ok(result.Result);
        }

        [HttpPost("InSoKSKVaKetQua")]
        public ActionResult InSoKSKVaKetQua(InSoKSKVaKetQua inSoKSKDinhKy)//InSoKSKDinhKy
        {
            var result = _khamDoanService.InSoKSKDinhKy(inSoKSKDinhKy);
            return Ok(result);
        }

        #region  GetDataKetQuaKSKDoanEdit
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataKetQuaKSKDoanEdit")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<DanhSachDichVuKhamGrid>> GetDataKetQuaKSKDoanEdit(long hopDongKhamSucKhoeId)
        {
            var gridData = await _khamDoanService.GetDataKetQuaKSKDoanEdit(hopDongKhamSucKhoeId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SaveBanInKhamDoanTiepNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<KetQuaKhamSucKhoe>> SaveBanInKhamDoanTiepNhan(KetQuaKhamSucKhoe ketQuaKhamSucKhoe)
        {
            //var entity = await _khamDoanService.GetYeuCauTiepNhan(ketQuaKhamSucKhoe.YeuCauTiepNhanId, ketQuaKhamSucKhoe.HopDongKhamSucKhoeId);
            //entity.KetQuaKhamSucKhoeData = (string)ketQuaKhamSucKhoe.JsonKetQuaKSK;
            //entity.KSKKetLuanData = (string)ketQuaKhamSucKhoe.JsonKetLuan;
            //await _yeuCauTiepNhanService.UpdateAsync(entity);
            #region cập nhật xem người khám   kết luận, Thời điểm kết luận
            var objJsonKetLuan = new KetQuaKhamSucKhoeDaTa
            {
                NhanVienKetLuanId = _userAgentHelper.GetCurrentUserId(),
                ThoiDiemKetLuan = DateTime.Now,
                KetQuaKhamSucKhoe = ketQuaKhamSucKhoe.JsonKetQuaKSK
            };
            var json = JsonConvert.SerializeObject(objJsonKetLuan);
            #endregion end cập nhật xem người khám   kết luận, Thời điểm kết luận

            _khamDoanService.SaveBanInKhamDoanTiepNhan(ketQuaKhamSucKhoe.YeuCauTiepNhanId, json, ketQuaKhamSucKhoe.JsonKetLuan);
            return Ok(ketQuaKhamSucKhoe.YeuCauTiepNhanId);
        }
        [HttpPost("XuLyInKetQuaKhamSucKhoeKetLuanAsync")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan)]
        public async Task<ActionResult<string>> XuLyInKetQuaKhamSucKhoeKetLuanAsync(PhieuInNhanVienKhamSucKhoeInfoVo phieuInNhanVienKhamSucKhoeInfoVo)
        {
            var phieuIn = await _khamDoanService.XuLyInKetQuaKhamSucKhoeKetLuanAsync(phieuInNhanVienKhamSucKhoeInfoVo);
            //var dsk ="hello";
            var list = new List<HtmlToPdfVo>();
            PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();
            var footerHtml = @"<!DOCTYPE html>
              <html><head><script>
                function subst() {
                   var vars={};
                    var x=window.location.search.substring(1).split('&');
                    for (var i in x) {var z=x[i].split('=',2);vars[z[0]] = unescape(z[1]);}
                    var x=['frompage','topage','page','webpage','section','subsection','subsubsection'];
                    for (var i in x) {
                        var y = document.getElementsByClassName(x[i]);
                        for (var j=0; j<y.length; ++j) y[j].textContent = vars[x[i]];
                        ";
            footerHtml += @"}}
              </script></head><body style='border:0; margin: 0;' onload='subst()'>
                <table style='width: 100%'>   
                   <tr>";
      
            footerHtml += @"<td><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Ghi ch&#250;: Ph&#226;n lo&#7841;i kh&#225;m s&#7913;c kh&#7887;e: Lo&#7841;i 1: R&#7845;t kh&#7887;e; Lo&#7841;i 2: Kh&#7887;e; Lo&#7841;i 3: Trung b&#236;nh; Lo&#7841;i 4: Y&#7871;u; Lo&#7841;i 5: R&#7845;t y&#7871;u.</b></td>";
            footerHtml += @"<td style='text-align:right'>
                    Trang <span class='page'></span>/<span class='topage'></span>
                  </td>
                </tr>
              </table>
              </body></html>";
            htmlContent.TenFile = "KetQuaKSK";
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = phieuIn,
                FooterHtml = htmlContent.NoFooter != true ? footerHtml : "",
                Bottom = 15
            };
            list.Add(htmlToPdfVo);
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }
        #endregion
        #region 772021
        [HttpPost("GetGridPhanLoaiVaCacBenhtatDenghi")]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan)]
        public async Task<List<DanhSachPhanLoaiCacBenhTatGrid>> GetGridPhanLoaiVaCacBenhtatDenghi(long hopDongKhamSucKhoeId)
        {
            var phieuIn = await _khamDoanService.GetGridPhanLoaiVaCacBenhtatDenghi(hopDongKhamSucKhoeId);
            return phieuIn;
        }
        [HttpPost("GetPhanLoaiSucKhoeKetLuan")]
        public ActionResult<ICollection<LookupItemVo>> GetPhanLoaiSucKhoeKetLuan( long id)
        {
            //var lookup = _khamDoanService.GetPhanLoaiSucKhoeKetLuan( id);
            return Ok();
        }
        #endregion
        #region chức năng copy dịch vụ xét nghiệm có data của tất cả yêu cầu tiếp nhận thuộc hợp đồng khám 
        [HttpGet("UpdateAllKetQuaKSKDoanCuaHopDongNhanVienBatDauKham")]
        public async Task<ActionResult<KetQuaKhamSucKhoe>> UpdateAllKetQuaKSKDoanCuaHopDongNhanVienBatDauKham(long id)
        {
            _khamDoanService.UpdateAllKetQuaKSKDoanCuaHopDongNhanVienBatDauKham(id);
            return Ok(id);
        }
        #endregion
        #region kiểm tra đúng dịch vụ của bệnh nhân
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("CheckDungDichVuCuaBenhNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public  bool CheckDungDichVuCuaBenhNhan(DichVuGridVos vo)
        {
            var result =  _khamDoanService.CheckDungDichVuCuaBenhNhan(vo);
            return result;
        }
        #endregion
    }
}
