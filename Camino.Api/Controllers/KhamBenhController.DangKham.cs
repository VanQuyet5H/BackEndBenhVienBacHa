using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhVien;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridKhamBenhDangKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridKhamBenhDangKhamAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridKhamBenhDangKhamAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridKhamBenhDangKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridKhamBenhDangKhamAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridKhamBenhDangKhamAsync(queryInfo);
            return Ok(gridData);
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridKhamBenhDangKhamTheoPhongKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridKhamBenhDangKhamTheoPhongKhamAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridKhamBenhDangKhamTheoPhongKhamAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridKhamBenhDangKhamTheoPhongKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridKhamBenhDangKhamTheoPhongKhamAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridKhamBenhDangKhamTheoPhongKhamAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region get list
        [HttpPost("GetListPhongBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListPhongBenhVienAsync(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauKhamBenhService.GetListPhongBenhVienAsync(model);
            return Ok(lookup);
        }
        [HttpPost("GetListKhoaBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListKhoaBenhVienAsync(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauKhamBenhService.GetListKhoaBenhVienAsync(model);
            return Ok(lookup);
        }


        #endregion

        #region get data
        [HttpGet("GetYeuCauKhamBenhDangKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<KhamBenhPhongBenhVienHangDoiViewModel> GetYeuCauKhamBenhDangKhamAsync(long hangDoiId)
        {
            var result = await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamAsync(hangDoiId);
            if (result == null)
            {
                return null;
            }

            var resultViewModel = result.ToModel<KhamBenhPhongBenhVienHangDoiViewModel>();
            var yeuCauKTNew = new YeuCauDichVuKyThuatViewModel();
            foreach (var item in resultViewModel.YeuCauKhamBenh.YeuCauDichVuKyThuats.Where(p => p.DieuTriNgoaiTru == true && p.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan))
            {
                yeuCauKTNew = new YeuCauDichVuKyThuatViewModel
                {
                    Id = item.Id,
                    TenDichVu = item.TenDichVu,
                    MaDichVu = item.MaDichVu,
                    MaGiaDichVu = item.MaGiaDichVu,
                    NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                    Gia = item.Gia,
                    NhomChiPhi = item.NhomChiPhi,
                    LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                    NhomDichVuBenhVienId = item.NhomDichVuBenhVienId,
                    DieuTriNgoaiTru = item.DieuTriNgoaiTru,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId,
                    SoLan = item.SoLan,
                    ThoiDiemBatDauDieuTri = item.ThoiDiemBatDauDieuTri,
                    TenDichVuHienThi = item.MaDichVu + " - " + item.TenDichVu,
                    YeuCauKhamBenhId = item.YeuCauKhamBenhId,
                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                    TenNhomDichVu = item.TenNhomDichVu,
                    TrangThai = item.TrangThai,
                    TrangThaiThanhToan = item.TrangThaiThanhToan,
                    ThoiDiemChiDinh = item.ThoiDiemChiDinh,
                    ThoiDiemDangKy = item.ThoiDiemDangKy,
                    NhanVienChiDinhId = item.NhanVienChiDinhId
                };
            }
            resultViewModel.YeuCauKhamBenh.YeuCauDichVuKyThuat = yeuCauKTNew;
            if (resultViewModel.YeuCauKhamBenh.YeuCauDichVuKyThuat.Id == 0)
            {
                resultViewModel.YeuCauKhamBenh.CoDieuTriNgoaiTru = null;
            }
            var benhVienHienTai = await _yeuCauKhamBenhService.BenhVienHienTai();
            if (benhVienHienTai != null)
            {
                resultViewModel.YeuCauTiepNhan.BenhVienHienTai = benhVienHienTai.ToModel<BienVienViewModel>();
            }
            // get template khám theo dịch vụ khám
            if (string.IsNullOrEmpty(resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate))
            {
                resultViewModel.YeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate = await _khamBenhService.GetTemplateKhamBenhTheoDichVuKham(result.YeuCauKhamBenh.DichVuKhamBenhBenhVienId);
            }

            //BVHD-3825
            // dich vu khuyen mai
            if ((result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any() && result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
                                                                                                                                   z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                                                                                                                                   z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
                || (result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any() && result.YeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
                                                                                                                                                  z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                                                                                                                                                  z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any())))
            {
                resultViewModel.CoDichVuKhuyenMai = true;
            }

            // get số dư toàn khoản
            resultViewModel.YeuCauKhamBenh.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(resultViewModel.YeuCauTiepNhanId);
            resultViewModel.YeuCauKhamBenh.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(resultViewModel.YeuCauTiepNhanId);

            resultViewModel.LaChuyenKhoaKhamNhieuKhamBenhDangKham = //await _yeuCauKhamBenhService.KiemTraKhoaHienTaiCoNhieuNguoiBenhAsync(resultViewModel.PhongBenhVienId);
                await _yeuCauKhamBenhService.KiemTraDichVuHienTaiCoNhieuNguoiBenhAsync(resultViewModel.YeuCauKhamBenh.DichVuKhamBenhBenhVienId.Value);

            #region BVHD-3895
            //if (result.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
            //{
            //    resultViewModel.LaDichVuKhamVietTat = await _yeuCauKhamBenhService.KiemTraDichVuKhamHienThiTenVietTatAsync(resultViewModel.YeuCauKhamBenh.DichVuKhamBenhBenhVienId ?? 0);
            //}

            #endregion

            #region BVHD-3941
            if (resultViewModel.YeuCauTiepNhan.CoBaoHiemTuNhan == true)
            {
                resultViewModel.YeuCauTiepNhan.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(result.YeuCauTiepNhanId);
            }
            #endregion

            return resultViewModel;
        }
        #endregion

        #region tab khám bệnh
        // hàm này hiện ko dùng, đang dùng chung hàm lưu tab khám bệnh trong màn hình khám bệnh
        [HttpPost("LuuThongTinKhamBenhDangKham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<KhamBenhPhongBenhVienHangDoiViewModel>> LuuThongTinKhamBenhDangKhamAsync([FromBody]PhongBenhVienHangDoiKhamBenhViewModel hangDoiViewModel)
        {
            // kiểm tra yêu cầu khám bệnh trước khi lưu
            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(hangDoiViewModel.YeuCauKhamBenhId ?? 0);

            var benhNhanHienTai =
                await _yeuCauKhamBenhService.GetYeuCauKhamBenhDangKhamAsync(hangDoiViewModel.Id);
            if (benhNhanHienTai == null)
            {
                //throw new ArgumentNullException(nameof(benhNhanHienTai));
                throw new ApiException(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.NotExists"));
                //return NotFound();
            }

            // kiểm tra kết quả sinh hiệu
            var lstKetQuaSinhHieu = hangDoiViewModel.YeuCauTiepNhan.KetQuaSinhHieus;
            if (lstKetQuaSinhHieu.Any())
            {
                lstKetQuaSinhHieu = lstKetQuaSinhHieu.Where(x => x.Id == 0).ToList();
                foreach (var item in lstKetQuaSinhHieu)
                {
                    if (item.BMI == null && item.CanNang == null && item.ChieuCao == null &&
                        item.HuyetApTamThu == null && item.HuyetApTamTruong == null && item.NhipTho == null && item.NhipTim == null &&
                        item.ThanNhiet == null && item.Glassgow == null && item.SpO2 == null)
                    {
                        throw new ApiException(_localizationService.GetResource("KhamBenh.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
                    }

                    if (item.HuyetApTamThu != null && item.HuyetApTamTruong == null ||
                        item.HuyetApTamThu == null && item.HuyetApTamTruong != null)
                    {
                        throw new ApiException(_localizationService.GetResource("KhamBenh.TamThuTamTruong.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
                    }
                }

            }

            // kiểm tra tự động lưu chẩn đoán icd chính
            var isAutoFillICDChinh = false;
            if (hangDoiViewModel.YeuCauKhamBenh.IsHoanThanhKham != true && benhNhanHienTai.YeuCauKhamBenh.IcdchinhId == null && hangDoiViewModel.YeuCauKhamBenh.ChanDoanSoBoICDId != null)
            {
                isAutoFillICDChinh = true;
                benhNhanHienTai.YeuCauKhamBenh.IcdchinhId = hangDoiViewModel.YeuCauKhamBenh.ChanDoanSoBoICDId;
            }

            if (isAutoFillICDChinh
                && string.IsNullOrEmpty(benhNhanHienTai.YeuCauKhamBenh.GhiChuICDChinh)
                && !string.IsNullOrEmpty(hangDoiViewModel.YeuCauKhamBenh.ChanDoanSoBoGhiChu))
            {
                benhNhanHienTai.YeuCauKhamBenh.GhiChuICDChinh = hangDoiViewModel.YeuCauKhamBenh.ChanDoanSoBoGhiChu;
            }

            //kiểm tra chẩn đoán phân biệt
            var lstChanDoanPhanBietId =
                hangDoiViewModel.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.Where(x => x.Id == 0)
                    .Select(x => x.ICDId).ToList();
            if (lstChanDoanPhanBietId.Count() != lstChanDoanPhanBietId.Distinct().Count())
            {
                throw new ApiException(_localizationService.GetResource("KhamBenh.ChanDoanPhanBietICDId.IsExists"));
            }

            hangDoiViewModel.ToEntity(benhNhanHienTai);

            // kiểm tra trạng thái yêu cầu khám bệnh dang khám
            if (benhNhanHienTai.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                benhNhanHienTai.YeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangKham;
                benhNhanHienTai.YeuCauKhamBenh.NoiThucHienId = benhNhanHienTai.YeuCauKhamBenh.NoiDangKyId; //_userAgentHelper.GetCurrentNoiLLamViecId();
                benhNhanHienTai.YeuCauKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                benhNhanHienTai.YeuCauKhamBenh.ThoiDiemThucHien = DateTime.Now;

                YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = benhNhanHienTai.YeuCauKhamBenh.TrangThai,
                    MoTa = benhNhanHienTai.YeuCauKhamBenh.TrangThai.GetDescription()

                };
                benhNhanHienTai.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
            }

            await _phongBenhVienHangDoiService.UpdateAsync(benhNhanHienTai);

            var yeuCauKhamBenh = benhNhanHienTai.ToModel<KhamBenhPhongBenhVienHangDoiViewModel>();

            foreach (var item in yeuCauKhamBenh.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets)
            {
                if (string.IsNullOrEmpty(item.TenICD))
                {
                    var chanDoanPhanBiet = hangDoiViewModel.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.FirstOrDefault(x => x.ICDId == item.ICDId);
                    item.TenICD = chanDoanPhanBiet?.TenICD;
                }
            }
            //Kiem tra neu icd khac null va phan biet item > 0
            if (!yeuCauKhamBenh.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Any() && yeuCauKhamBenh.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.Any())
            {
                foreach (var icdKhac in yeuCauKhamBenh.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
                {
                    if (string.IsNullOrEmpty(icdKhac.TenICD))
                    {
                        var chanDoanICDKhac = hangDoiViewModel.YeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.FirstOrDefault(x => x.ICDId == icdKhac.ICDId);
                        icdKhac.TenICD = chanDoanICDKhac?.TenICD;
                    }
                }
            }

            // kiểm tra tự động lưu chẩn đoán icd chính
            if (yeuCauKhamBenh.YeuCauKhamBenh.IcdchinhId != null
                && yeuCauKhamBenh.YeuCauKhamBenh.IcdchinhId != 0
                && yeuCauKhamBenh.YeuCauKhamBenh.ChanDoanSoBoICDId == yeuCauKhamBenh.YeuCauKhamBenh.IcdchinhId
                && string.IsNullOrEmpty(yeuCauKhamBenh.YeuCauKhamBenh.TenICDChinh))
            {
                yeuCauKhamBenh.YeuCauKhamBenh.TenICDChinh = hangDoiViewModel.YeuCauKhamBenh.TenChanDoanSoBoICD;
            }

            return yeuCauKhamBenh;
        }

        //hàm này hiện ko dùng, đang dùng chung hàm chuyển dịch vụ khám trong màn hình khám bệnh
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenhDangKham)]
        [HttpPost("XuLyChuyenDichVuKhamDangKham")]
        public async Task<ActionResult> XuLyChuyenDichVuKhamDangKhamAsync([FromBody] ChuyenKhamYeuCauKhamBenhViewModel viewModel)
        {
            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(viewModel.YeuCauKhamBenhTruocId);

            // xử lý hủy dịch vụ khám hiện tại
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);
            var yeuCauKhamCanHuy = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == viewModel.YeuCauKhamBenhTruocId);
            if (yeuCauKhamCanHuy == null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }
            if (yeuCauKhamCanHuy.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                throw new ApiException(_localizationService.GetResource("ChuyenKham.DichVuKham.DaThucHien"));
            }

            yeuCauKhamCanHuy.WillDelete = true;
            await _tiepNhanBenhNhanServiceService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);


            // xử lý tạo mới dịch vụ khám chuyển khám
            var yeuCauKhamEntity = viewModel.ToEntity<YeuCauKhamBenh>();
            yeuCauKhamEntity.YeuCauKhamBenhTruocId = null;
            await _yeuCauKhamBenhService.XuLyDataYeuCauKhamBenhChuyenKhamAsync(yeuCauKhamEntity, yeuCauTiepNhanChiTiet.CoBHYT ?? false);

            yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.Add(yeuCauKhamEntity);
            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return Ok();
        }
        #endregion

        #region export excel
        [HttpPost("ExportKhamBenhDangKham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> ExportKhamBenhDangKhamAsync(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _yeuCauKhamBenhService.GetDataForGridKhamBenhDangKhamAsync(queryInfo);
            var khamBenhDangKhamData = gridData.Data.Select(p => (KhamBenhDangKhamGridVo)p).ToList();
            var dataExcel = khamBenhDangKhamData.Map<List<KhamBenhDangKhamExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(KhamBenhDangKhamExportExcel.Phong), "Phòng"),
                (nameof(KhamBenhDangKhamExportExcel.BacSiDangKham), "Bác sĩ đang khám"),
                (nameof(KhamBenhDangKhamExportExcel.BenhNhanDangKham), "Người bệnh đang khám"),
                (nameof(KhamBenhDangKhamExportExcel.SoLuongBenhNhan), "SL Người bệnh trong phòng"),
                (nameof(KhamBenhDangKhamExportExcel.Khoa), "Khoa")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Khám bệnh đang khám");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BenhNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportKhamBenhDangKhamTheoPhongKham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> ExportKhamBenhDangKhamTheoPhongKhamAsync(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _yeuCauKhamBenhService.GetDataForGridKhamBenhDangKhamTheoPhongKhamAsync(queryInfo);
            var khamBenhTheoPhongData = gridData.Data.Select(p => (KhamBenhDangKhamTheoPhongKhamGridVo)p).ToList();
            var dataExcel = khamBenhTheoPhongData.Map<List<KhamBenhDangKhamTheoPhongKhamExportExcel>>();

            var lstValueObject = new List<(string, string)>
            {
                (nameof(KhamBenhDangKhamTheoPhongKhamExportExcel.MaTiepNhan), "Mã TN"),
                (nameof(KhamBenhDangKhamTheoPhongKhamExportExcel.MaBenhNhan), "Mã BN"),
                (nameof(KhamBenhDangKhamTheoPhongKhamExportExcel.HoTen), "Họ Tên"),
                (nameof(KhamBenhDangKhamTheoPhongKhamExportExcel.NamSinh), "Năm Sinh"),
                (nameof(KhamBenhDangKhamTheoPhongKhamExportExcel.SoDienThoai), "Điện Thoại"),
                (nameof(KhamBenhDangKhamTheoPhongKhamExportExcel.ThoiDiemTiepNhanDisplay), "Thời Điểm Tiếp Nhận"),
                (nameof(KhamBenhDangKhamTheoPhongKhamExportExcel.TenTrangThai), "Trạng Thái")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Khám bệnh đang khám theo phòng khám");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KhamBenhDangKhamTheoPhongKham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
