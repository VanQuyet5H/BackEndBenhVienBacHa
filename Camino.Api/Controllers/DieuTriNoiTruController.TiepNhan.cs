using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BHYT;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauTiepNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridTiepNhanNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridTiepNhanNoiTruAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridTiepNhanNoiTruAsyncVer4(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridTiepNhanNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridTiepNhanNoiTruAsync([FromBody] QueryInfo queryInfo)
        {
            // lazyLoadPage = "false"
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridTiepNhanNoiTruAsyncVer3(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridSoDoGiuongTiepNhanNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiepNhanNoiTru, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridSoDoGiuongTiepNhanNoiTruAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridSoDoGiuongTiepNhanNoiTruAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridSoDoGiuongTiepNhanNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiepNhanNoiTru, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridSoDoGiuongTiepNhanNoiTruAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridSoDoGiuongTiepNhanNoiTruAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataLichSuChuyenDoiTuongForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataLichSuChuyenDoiTuongForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataLichSuChuyenDoiTuongForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageLichSuChuyenDoiTuongForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageLichSuChuyenDoiTuongForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageLichSuChuyenDoiTuongForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region export pdf
        [HttpPost("ExportDanhSachTiepNhanNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult> ExportDanhSachTiepNhanNoiTruAsync(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _dieuTriNoiTruService.GetDataForGridTiepNhanNoiTruAsyncVer4(queryInfo);
            var lsNhaSanXuat = gridData.Data.Select(p => (TiepNhanNoiTruGridVo)p).ToList();
            var dataExcel = lsNhaSanXuat.Map<List<DanhSachTiepNhanNoiTruExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.MaTiepNhan), "Mã TN"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.MaBenhNhan), "Mã BN"));

            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.HoTen), "Họ tên"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.GioiTinh), "Giới tính"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.KhoaNhapVien), "Khoa nhập viện"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.ThoiGianTiepNhanDisplay), "Thời gian tiếp nhận"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.SoBenhAn), "SỐ BA"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.NoiChiDinh), "Nơi chỉ định"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.ChanDoan), "Chẩn đoán"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.DoiTuong), "Đối tượng"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.LaCapCuu), "Cấp cứu"));
            lstValueObject.Add((nameof(DanhSachTiepNhanNoiTruExportExcel.TenTrangThai), "Trạng thái"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Tiếp Nhận Nội Trú");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TiepNhanNoiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region get data

        [HttpGet("GetThongTinBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult<NoiTruBenhAnThongTinHanhChinhViewModel>> GetThongTinBenhAnAsync(int yeuCauTiepNhanId)
        {
            var benhAn = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId,
                x => x.Include(y => y.DanToc)
                    .Include(y => y.BenhNhan)
                    .Include(y => y.NgheNghiep)
                    .Include(y => y.NoiTruBenhAn)
                    .Include(y => y.NhanVienTiepNhan).ThenInclude(z => z.User)
                    .Include(y => y.YeuCauNhapVien).ThenInclude(z => z.YeuCauNhapVienChanDoanKemTheos).ThenInclude(a => a.ICD)
                    .Include(y => y.YeuCauNhapVien).ThenInclude(z => z.KhoaPhongNhapVien)
                    .Include(y => y.YeuCauNhapVien).ThenInclude(z => z.BacSiChiDinh).ThenInclude(a => a.User)
                    .Include(y => y.YeuCauNhapVien).ThenInclude(z => z.ChanDoanNhapVienICD)
                    .Include(y => y.YeuCauNhapVien).ThenInclude(z => z.NoiChiDinh)
                    .Include(y => y.YeuCauNhapVien).ThenInclude(z => z.YeuCauKhamBenh).ThenInclude(a => a.YeuCauTiepNhan)
                    .Include(y => y.NoiTruBenhAn)
                    .Include(y => y.YeuCauTiepNhanTheBHYTs));

            //BVHD-3800
            var laCapCuu = benhAn.LaCapCuu ?? benhAn.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

            var viewModel = benhAn.ToModel<NoiTruBenhAnThongTinHanhChinhViewModel>();
            if (viewModel.NoiTruBenhAn == null)
            {
                viewModel.NoiTruBenhAn = new NoiTruBenhAnViewModel();
                viewModel.NoiTruBenhAn.ThoiDiemTaoBenhAn = viewModel.NoiTruBenhAn.ThoiDiemNhapVien = DateTime.Now;
            }

            viewModel.NoiTruBenhAn.ThoiDiemTiepNhanNgoaiTru = benhAn.YeuCauNhapVien.YeuCauKhamBenh?.YeuCauTiepNhan?.ThoiDiemTiepNhan;

            //viewModel.SoDuTaiKhoan = await _taiKhoanBenhNhanService.SoDuTaiKhoan(viewModel.BenhNhanId);
            //viewModel.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(viewModel.Id);

            //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanId).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
            var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhanId).Result;
            var soTienDaTamUng = _thuNganNoiTruService.GetSoTienDaTamUngAsync(yeuCauTiepNhanId).Result;
            viewModel.SoDuTaiKhoan = soTienDaTamUng;
            viewModel.SoDuTaiKhoanConLai = soTienDaTamUng - chiPhiKhamChuaBenh;
            viewModel.SoDuTaiKhoanConLai = viewModel.SoDuTaiKhoanConLai < 0 ? 0 : viewModel.SoDuTaiKhoanConLai;

            //BVHD-3800
            viewModel.LaCapCuu = laCapCuu;

            #region BVHD-3941
            if (benhAn.CoBHTN == true)
            {
                viewModel.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(benhAn.Id);
            }
            #endregion

            return viewModel;
        }


        [HttpPost("GetListLoaiBenhAn")]
        public List<LookupItemVo> GetListLoaiBenhAnAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var lookups = _dieuTriNoiTruService.GetListLoaiBenhAn(queryInfo);
            return lookups;
        }

        [HttpPost("GetListBacSiDieuTri")]
        public async Task<ActionResult> GetListBacSiDieuTriAsync([FromBody] DropDownListRequestModel model)
        {
            var lstNhomChucDanh = await _phauThuatThuThuatService.GetListBacSiDieuDuong(model, Enums.EnumNhomChucDanh.BacSi);
            return Ok(lstNhomChucDanh);
        }

        [HttpPost("GetListDieuDuong")]
        public async Task<ActionResult> GetListDieuDuongAsync([FromBody] DropDownListRequestModel model)
        {
            var lstNhomChucDanh = await _phauThuatThuThuatService.GetListBacSiDieuDuong(model, Enums.EnumNhomChucDanh.DieuDuong);
            return Ok(lstNhomChucDanh);
        }

        [HttpGet("GetThongTinDoiTuongTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult<ThongTinDoiTuongTiepNhanViewModel>> GetThongTinDoiTuongTiepNhanAsync(int yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId,
                x => x.Include(y => y.YeuCauTiepNhanTheBHYTs).ThenInclude(z => z.GiayMienCungChiTra)
                    .Include(y => y.YeuCauTiepNhanLichSuChuyenDoiTuongs)
                    .Include(y => y.YeuCauTiepNhanCongTyBaoHiemTuNhans).ThenInclude(z => z.CongTyBaoHiemTuNhan)
                    .Include(y => y.YeuCauNhapVien)
                    .Include(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs));

            var result = yeuCauTiepNhan.ToModel<ThongTinDoiTuongTiepNhanViewModel>();
            foreach (var theBHYT in result.YeuCauTiepNhanTheBHYTs)
            {
                if (!string.IsNullOrEmpty(theBHYT.MaDKBD))
                {
                    var benhVien = await _benhVienService.GetBenhVienWithMaBenhVien(theBHYT.MaDKBD);
                    if (benhVien != null)
                    {
                        theBHYT.NoiDangKyBHYT = benhVien.Ten;
                    }
                }
            }
            return result;
        }

        [HttpPost("GetCongTyBaoHiemTuNhan")]
        public async Task<ActionResult<List<LookupItemVo>>> GetCongTyBaoHiemTuNhanAsync(DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetCongTyBaoHiemTuNhanAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetDanToc")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanToc(DropDownListRequestModel model)
        {
            var lookup = await _tiepNhanBenhNhanService.GetDanToc(model);
            return Ok(lookup);
        }
        #endregion

        #region Thêm/xóa/sửa
        [HttpPost("XuLyTaoBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult> XuLyTaoBenhAnAsync([FromBody] NoiTruBenhAnThongTinHanhChinhViewModel yeuCauViewModel)
        {
            var thongTinBenhAn = yeuCauViewModel.NoiTruBenhAn.ToEntity<NoiTruBenhAn>();
            thongTinBenhAn.BenhNhanId = yeuCauViewModel.BenhNhanId;
            thongTinBenhAn.NhanVienTaoBenhAnId = _userAgentHelper.GetCurrentUserId();
            thongTinBenhAn.Id = yeuCauViewModel.Id;
            await _dieuTriNoiTruService.XuLyTaoBenhAnAsync(thongTinBenhAn);
            return NoContent();
        }

        [HttpPut("XuLyCapNhatBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult> XuLyCapNhatBenhAnAsync([FromBody] NoiTruBenhAnThongTinHanhChinhViewModel yeuCauViewModel)
        {
            var benhAn =
                await _dieuTriNoiTruService.GetByIdAsync(yeuCauViewModel.Id, x => x.Include(y => y.NoiTruBenhAn).ThenInclude(z => z.NoiTruKhoaPhongDieuTris)
                                                                                        .Include(y => y.NoiTruBenhAn).ThenInclude(z => z.NoiTruKhoaPhongDieuTris)
                                                                                        .Include(y => y.NoiTruBenhAn).ThenInclude(z => z.NoiTruEkipDieuTris)
                                                                                        .Include(y => y.YeuCauDichVuGiuongBenhViens).ThenInclude(z => z.HoatDongGiuongBenhs));
            var loaiBenhAnTruocCapNhat = benhAn.NoiTruBenhAn.LoaiBenhAn;

            yeuCauViewModel.NoiTruBenhAn.ToEntity(benhAn.NoiTruBenhAn);
            await _dieuTriNoiTruService.XuLyCapNhatBenhAnAsync(benhAn, loaiBenhAnTruocCapNhat);
            return NoContent();
        }

        [HttpPost("XuLyChiDinhEkipVaDichVuGiuongNoiTru")]
        public async Task<ActionResult> XuLyChiDinhEkipVaDichVuGiuongNoiTruAsync([FromBody] ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanViewModel yeuCau)
        {
            if (yeuCau.DichVuGiuongId != null)
            {
                // xử lý kiểm tra giường: còn trống? có bao phòng?
                var giuongBenhTrong = new GiuongBenhTrongVo()
                {
                    GiuongBenhId = yeuCau.GiuongId.Value,
                    BaoPhong = yeuCau.BaoPhong,
                    ThoiGianNhan = yeuCau.ThoiGianNhan.Value
                };

                await _dieuTriNoiTruService.KiemTraPhongChiDinhTiepNhanNoiTru(giuongBenhTrong);
            }

            // xử lý lưu
            var yeuCauVo = new ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanVo()
            {
                YeuCauTiepNhanId = yeuCau.YeuCauTiepNhanId,
                BacSiDieuTriId = yeuCau.BacSiDieuTriId.Value,
                DieuDuongId = yeuCau.DieuDuongId.Value,
                TuNgay = yeuCau.TuNgay.Value,
                DichVuGiuongId = yeuCau.DichVuGiuongId ?? 0,
                GiuongId = yeuCau.GiuongId ?? 0,
                TenGiuong = yeuCau.TenGiuong,
                LoaiGiuong = yeuCau.LoaiGiuong ?? 0,
                BaoPhong = yeuCau.BaoPhong ?? false,
                ThoiGianNhan = yeuCau.ThoiGianNhan ?? new DateTime(),
                YeuCauGoiDichVuId = yeuCau.YeuCauGoiDichVuId
            };
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauVo.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = _yeuCauTiepNhanService.GetById(yeuCauVo.YeuCauTiepNhanId,
                x => x.Include(a => a.YeuCauDichVuGiuongBenhViens)
                    .Include(a => a.NoiTruBenhAn).ThenInclude(a => a.NoiTruEkipDieuTris)
                    .Include(a => a.BenhNhan));
            await _dieuTriNoiTruService.XuLyChiDinhEkipVaDichVuGiuongNoiTruAsync(yeuCauVo, yeuCauTiepNhanChiTiet);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return NoContent();
        }

        [HttpPost("KiemTraPhongChiDinhTiepNhanNoiTru")]
        public async Task<ActionResult> KiemTraPhongChiDinhTiepNhanNoiTruAsync(DieuTriNoiTruChonGiuongBenhViewModel chonGiuongViewModel)
        {
            if (chonGiuongViewModel.GiuongId == null)
            {
                throw new ApiException(_localizationService.GetResource("NoiTruBenhAn.ChonGiuongBenh.Required"));
            }

            var giuongBenhTrong = new GiuongBenhTrongVo()
            {
                GiuongBenhId = chonGiuongViewModel.GiuongId.Value,
                BaoPhong = chonGiuongViewModel.BaoPhong,
                ThoiGianNhan = chonGiuongViewModel.ThoiGianNhan,
                ThoiGianTra = chonGiuongViewModel.ThoiGianTra,
                YeuCauDichVuGiuongBenhVienId = chonGiuongViewModel.YeuCauDichVuGiuongBenhVienId,
                YeuCauTiepNhanNoiTruId = chonGiuongViewModel.YeuCauTiepNhanNoiTruId
            };

            await _dieuTriNoiTruService.KiemTraPhongChiDinhTiepNhanNoiTru(giuongBenhTrong);

            return NoContent();
        }

        [HttpPut("XuLyCapNhatThongTinDoiTuong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiepNhanNoiTru, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> XuLyCapNhatThongTinDoiTuongAsync([FromBody] ThongTinDoiTuongTiepNhanViewModel yeuCauViewModel)
        {
            //BVHD-3754
            var warningMessage = string.Empty;

            if (yeuCauViewModel.CoBHYT == true)
            {
                var kiemTra = await _tiepNhanBenhNhanService.KiemTraDieuKienTaoMoiYeuCauTiepNhanAsync(yeuCauViewModel.BenhNhanId, false, yeuCauViewModel.Id);
                if (!string.IsNullOrEmpty(kiemTra.ErrorMessage))
                {
                    throw new ApiException(kiemTra.ErrorMessage);
                }

                foreach (var theBHYT in yeuCauViewModel.YeuCauTiepNhanTheBHYTs)
                {
                    if (theBHYT.NgayHieuLuc.Value.Date > DateTime.Now.Date
                        || (theBHYT.DuocGiaHanThe != true && theBHYT.NgayHetHan.Value.Date < DateTime.Now.Date)
                        || (theBHYT.DuocGiaHanThe == true && (DateTime.Now.Date - theBHYT.NgayHetHan.Value.Date).Days > 15))
                    {
                        //throw new ApiException(_localizationService.GetResource("ThongTinDoiTuongTiepNhan.TheKhongCoHieuLuc"));

                        //BVHD-3754
                        warningMessage = _localizationService.GetResource("ThongTinDoiTuongTiepNhan.TheKhongCoHieuLuc");
                        break;
                    }
                }
            }

            if (yeuCauViewModel.CoBHTN == true && !yeuCauViewModel.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any())
            {
                throw new ApiException(_localizationService.GetResource("ThongTinDoiTuongTiepNhan.CongTyBaoHiemTuNhan.Required"));
            }

            //if (yeuCauViewModel.CoBHYT == true && (yeuCauViewModel.BHYTNgayHieuLuc.Value.Date > DateTime.Now.Date || yeuCauViewModel.BHYTNgayHetHan.Value.Date < DateTime.Now.Date))
            //{
            //    throw new ApiException(_localizationService.GetResource("ThongTinDoiTuongTiepNhan.TheKhongCoHieuLuc"));
            //}
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(yeuCauViewModel.Id,
                x => x.Include(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs)
                            .Include(y => y.NoiTruBenhAn)
                            .Include(y => y.YeuCauTiepNhanTheBHYTs).ThenInclude(z => z.GiayMienCungChiTra)
                            .Include(y => y.YeuCauTiepNhanLichSuChuyenDoiTuongs)
                    .Include(y => y.YeuCauNhapVien).ThenInclude(z => z.YeuCauKhamBenh).ThenInclude(z => z.YeuCauTiepNhan)
                            .Include(y => y.YeuCauTiepNhanCongTyBaoHiemTuNhans).ThenInclude(z => z.CongTyBaoHiemTuNhan));

            if (yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.TinhTrangRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ThongTinDoiTuongTiepNhan.BenhAn.DaRaVien"));
            }

            // chỉ user có quyền mới đc cập nhật thông tin hành chính người bệnh
            var coQuyenCapNhatHanhChinh = _roleService.IsHavePermissionForUpdateInformationTNBN();
            if (!coQuyenCapNhatHanhChinh)
            {
                yeuCauViewModel.HoTen = yeuCauTiepNhan.HoTen.ToUpper();
                yeuCauViewModel.NgaySinh = yeuCauTiepNhan.NgaySinh;
                yeuCauViewModel.ThangSinh = yeuCauTiepNhan.ThangSinh;
                yeuCauViewModel.NamSinh = yeuCauTiepNhan.NamSinh;
                yeuCauViewModel.PhuongXaId = yeuCauTiepNhan.PhuongXaId;
                yeuCauViewModel.TinhThanhId = yeuCauTiepNhan.TinhThanhId;
                yeuCauViewModel.QuanHuyenId = yeuCauTiepNhan.QuanHuyenId;
                yeuCauViewModel.DiaChi = yeuCauTiepNhan.DiaChi;
                yeuCauViewModel.QuocTichId = yeuCauTiepNhan.QuocTichId;
                yeuCauViewModel.SoDienThoai = yeuCauTiepNhan.SoDienThoai;
                yeuCauViewModel.SoChungMinhThu = yeuCauTiepNhan.SoChungMinhThu;
                yeuCauViewModel.Email = yeuCauTiepNhan.Email;
                yeuCauViewModel.NgheNghiepId = yeuCauTiepNhan.NgheNghiepId;
                yeuCauViewModel.GioiTinh = yeuCauTiepNhan.GioiTinh;
                yeuCauViewModel.NoiLamViec = yeuCauTiepNhan.NoiLamViec;
                yeuCauViewModel.DanTocId = yeuCauTiepNhan.DanTocId;

                yeuCauViewModel.NguoiLienHeHoTen = yeuCauTiepNhan.NguoiLienHeHoTen;
                yeuCauViewModel.NguoiLienHeQuanHeNhanThanId = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId;
                yeuCauViewModel.NguoiLienHeSoDienThoai = yeuCauTiepNhan.NguoiLienHeSoDienThoai;
                yeuCauViewModel.NguoiLienHeEmail = yeuCauTiepNhan.NguoiLienHeEmail;
                yeuCauViewModel.NguoiLienHeTinhThanhId = yeuCauTiepNhan.NguoiLienHeTinhThanhId;
                yeuCauViewModel.NguoiLienHeQuanHuyenId = yeuCauTiepNhan.NguoiLienHeQuanHuyenId;
                yeuCauViewModel.NguoiLienHePhuongXaId = yeuCauTiepNhan.NguoiLienHePhuongXaId;
                yeuCauViewModel.NguoiLienHeDiaChi = yeuCauTiepNhan.NguoiLienHeDiaChi;
            }

            var thongTinDoiTuong = yeuCauViewModel.ToEntity(yeuCauTiepNhan);
            if (yeuCauTiepNhan.BenhNhan != null)
            {
                yeuCauTiepNhan.BenhNhan.HoTen = yeuCauTiepNhan.HoTen.ToUpper();
                yeuCauTiepNhan.BenhNhan.NgaySinh = yeuCauTiepNhan.NgaySinh;
                yeuCauTiepNhan.BenhNhan.ThangSinh = yeuCauTiepNhan.ThangSinh;
                yeuCauTiepNhan.BenhNhan.NamSinh = yeuCauTiepNhan.NamSinh;
                yeuCauTiepNhan.BenhNhan.PhuongXaId = yeuCauTiepNhan.PhuongXaId;
                yeuCauTiepNhan.BenhNhan.TinhThanhId = yeuCauTiepNhan.TinhThanhId;
                yeuCauTiepNhan.BenhNhan.QuanHuyenId = yeuCauTiepNhan.QuanHuyenId;
                yeuCauTiepNhan.BenhNhan.DiaChi = yeuCauTiepNhan.DiaChi;
                yeuCauTiepNhan.BenhNhan.QuocTichId = yeuCauTiepNhan.QuocTichId;
                yeuCauTiepNhan.BenhNhan.SoDienThoai = yeuCauTiepNhan.SoDienThoai;
                yeuCauTiepNhan.BenhNhan.SoChungMinhThu = yeuCauTiepNhan.SoChungMinhThu;
                yeuCauTiepNhan.BenhNhan.Email = yeuCauTiepNhan.Email;
                yeuCauTiepNhan.BenhNhan.NgheNghiepId = yeuCauTiepNhan.NgheNghiepId;
                yeuCauTiepNhan.BenhNhan.GioiTinh = yeuCauTiepNhan.GioiTinh;
                yeuCauTiepNhan.BenhNhan.NoiLamViec = yeuCauTiepNhan.NoiLamViec;
                yeuCauTiepNhan.BenhNhan.DanTocId = yeuCauTiepNhan.DanTocId;

                if (!string.IsNullOrEmpty(yeuCauTiepNhan.NguoiLienHeHoTen)
                    || (yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId != 0 && yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId != null)
                    || !string.IsNullOrEmpty(yeuCauTiepNhan.NguoiLienHeSoDienThoai)
                    || !string.IsNullOrEmpty(yeuCauTiepNhan.NguoiLienHeEmail)
                    || (yeuCauTiepNhan.NguoiLienHeTinhThanhId != 0 && yeuCauTiepNhan.NguoiLienHeTinhThanhId != null)
                    || (yeuCauTiepNhan.NguoiLienHeQuanHuyenId != 0 && yeuCauTiepNhan.NguoiLienHeQuanHuyenId != null)
                    || (yeuCauTiepNhan.NguoiLienHePhuongXaId != 0 && yeuCauTiepNhan.NguoiLienHePhuongXaId != null)
                    || !string.IsNullOrEmpty(yeuCauTiepNhan.NguoiLienHeDiaChi)
                )
                {
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeHoTen = yeuCauTiepNhan.NguoiLienHeHoTen;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeQuanHeNhanThanId = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeSoDienThoai = yeuCauTiepNhan.NguoiLienHeSoDienThoai;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeEmail = yeuCauTiepNhan.NguoiLienHeEmail;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeTinhThanhId = yeuCauTiepNhan.NguoiLienHeTinhThanhId;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeQuanHuyenId = yeuCauTiepNhan.NguoiLienHeQuanHuyenId;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHePhuongXaId = yeuCauTiepNhan.NguoiLienHePhuongXaId;
                    yeuCauTiepNhan.BenhNhan.NguoiLienHeDiaChi = yeuCauTiepNhan.NguoiLienHeDiaChi;
                }

                if (yeuCauViewModel.YeuCauGoiDichVuId != null)
                {
                    if (yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.All(x => x.Id != yeuCauViewModel.YeuCauGoiDichVuId))
                    {
                        foreach (var yeuCauGoi in yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs)
                        {
                            var kiemTra = await _dieuTriNoiTruService.KiemTraYeuCauGoiDichVuDaSuDungAsync(yeuCauGoi.Id, yeuCauTiepNhan.BenhNhanId, true, yeuCauViewModel.YeuCauTiepNhanMeId);
                            if (!kiemTra)
                            {
                                yeuCauGoi.BenhNhanSoSinhId = null;
                            }
                            else
                            {
                                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.YeuCauGoiDichVuSoSinh.DaThucHien"));
                            }
                        }
                        var yeuCauGoiDichVu = _yeuCauGoiDichVuService.GetById(yeuCauViewModel.YeuCauGoiDichVuId.Value);
                        yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Add(yeuCauGoiDichVu);
                    }
                }
                else
                {
                    if (yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.Any())
                    {
                        foreach (var yeuCauGoi in yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs)
                        {
                            var kiemTra = await _dieuTriNoiTruService.KiemTraYeuCauGoiDichVuDaSuDungAsync(yeuCauGoi.Id, yeuCauTiepNhan.BenhNhanId, true, yeuCauViewModel.YeuCauTiepNhanMeId);
                            if (!kiemTra)
                            {
                                yeuCauGoi.BenhNhanSoSinhId = null;
                            }
                            else
                            {
                                throw new ApiException(_localizationService.GetResource("TiepNhanBenhNhan.YeuCauGoiDichVuSoSinh.DaThucHien"));
                            }
                        }
                    }
                }

            }

            //todo: cần kiểm tra lại
            // tham khảo từ function RemoveBHYT bên tiếp nhận người bệnh
            if (yeuCauTiepNhan.CoBHYT != true)
            {
                thongTinDoiTuong.BHYTNgayDuocMienCungChiTra = null;

                thongTinDoiTuong.BHYTMaDKBD = null;
                thongTinDoiTuong.BHYTNgayDu5Nam = null;
                thongTinDoiTuong.BHYTMucHuong = null;
                thongTinDoiTuong.BHYTDiaChi = null;
                thongTinDoiTuong.BHYTMaSoThe = null;
                thongTinDoiTuong.BHYTCoQuanBHXH = null;
                thongTinDoiTuong.BHYTMaKhuVuc = null;
                thongTinDoiTuong.BHYTNgayHieuLuc = null;
                thongTinDoiTuong.BHYTNgayHetHan = null;

                thongTinDoiTuong.LyDoVaoVien = null;

                foreach (var theBHYT in yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs)
                {
                    if (theBHYT.GiayMienCungChiTra != null
                        && !string.IsNullOrEmpty(theBHYT.GiayMienCungChiTra.DuongDan)
                        && !string.IsNullOrEmpty(theBHYT.GiayMienCungChiTra.TenGuid))
                    {
                        await _taiLieuDinhKemService.XoaTaiLieuAsync(theBHYT.GiayMienCungChiTra.DuongDan, theBHYT.GiayMienCungChiTra.TenGuid);
                    }
                }
            }
            else
            {
                foreach (var theBHYT in yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs)
                {
                    if (theBHYT.GiayMienCungChiTra != null 
                        && !string.IsNullOrEmpty(theBHYT.GiayMienCungChiTra.DuongDan)
                        && !string.IsNullOrEmpty(theBHYT.GiayMienCungChiTra.TenGuid))
                    {
                        if (theBHYT.GiayMienCungChiTra.Id != 0)
                        {
                            await _taiLieuDinhKemService.XoaTaiLieuAsync(theBHYT.GiayMienCungChiTra.DuongDan, theBHYT.GiayMienCungChiTra.TenGuid);
                        }
                        await _taiLieuDinhKemService.LuuTaiLieuAsync(theBHYT.GiayMienCungChiTra.DuongDan, theBHYT.GiayMienCungChiTra.TenGuid);
                    }
                }
            }

            //await _yeuCauTiepNhanService.UpdateAsync(thongTinDoiTuong);
            await _yeuCauTiepNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(thongTinDoiTuong);

            //Cập nhật ngược lại yêu cầu nhập viện
            _yeuCauTiepNhanService.CapNhatThongTinHanhChinhVaoNgoaiTru(yeuCauTiepNhan.Id);

            //BVHD-3754
            //return NoContent();
            return warningMessage;
        }

        [HttpPost("KiemTraThemCongTyBHTN")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> KiemTraThemCongTyBHTN([FromBody] NoiTruYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel model)
        {
            return NoContent();
        }

        [HttpPost("KiemTraValidationListTheBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> KiemTraValidationListTheBHYT([FromBody] KiemTraValidationListTheBHYTViewModel model)
        {
            return NoContent();
        }
        #endregion

        #region bệnh án sơ sinh
        [HttpGet("KiemTraTaoBenhAnSoSinh")]
        public async Task<ActionResult<bool>> KiemTraTaoBenhAnSoSinhAsync(long yeuCauTiepNhanId)
        {
            //var kiemTra = await _dieuTriNoiTruService.KiemTraTaoBenhAnSoSinhAsync(yeuCauTiepNhanId);
            //if (string.IsNullOrEmpty(kiemTra) || !JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(kiemTra).DacĐiemTreSoSinhs.Any())
            //{
            //    throw  new ApiException(_localizationService.GetResource("BenhAnSoSinh.SoLuongBenhAnCon.Enough"));
            //}

            await XuLyKiemTraTaoBenhAnSoSinhAsync(yeuCauTiepNhanId);
            return true;
        }

        private async Task XuLyKiemTraTaoBenhAnSoSinhAsync(long yeuCauTIepNhanId)
        {
            var yeuCauTiepNhan = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTIepNhanId, x => x.Include(y => y.NoiTruBenhAn).Include(y => y.YeuCauNhapVienCons));

            if (yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("BenhAnSoSinh.BenhAn.DaKetThuc"));
            }

            if (string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn.ThongTinTongKetBenhAn))
            {
                throw new ApiException(_localizationService.GetResource("BenhAnSoSinh.SoLuongBenhAnCon.Enough"));
            }
            else
            {
                var thongTinTongKet = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(yeuCauTiepNhan.NoiTruBenhAn.ThongTinTongKetBenhAn);
                //đặt cái tên biến kiểu này thì cũng nhức nách -> DacĐiemTreSoSinhs
                if (!thongTinTongKet.DacDiemTreSoSinhs.Any()
                    || yeuCauTiepNhan.YeuCauNhapVienCons.Count >= thongTinTongKet.DacDiemTreSoSinhs.Count)

                    throw new ApiException(_localizationService.GetResource("BenhAnSoSinh.SoLuongBenhAnCon.Enough"));
            }
        }

        [HttpGet("GetThongTinTiepNhanBenhAnMe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru, Enums.DocumentType.TaoBenhAnSoSinh)]
        public async Task<ActionResult<BenhAnSoSinhChiTietViewModel>> GetThongTinTiepNhanBenhAnMeAsync(int yeuCauTiepNhanId)
        {
            var yeuCauTiepNhanMe = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId,
                x => x.Include(y => y.NoiTruBenhAn));

            var yeuCauTiepNhanCon = new YeuCauTiepNhan()
            {
                SoDienThoai = yeuCauTiepNhanMe.SoDienThoai,
                QuocTichId = yeuCauTiepNhanMe.QuocTichId,
                TinhThanhId = yeuCauTiepNhanMe.TinhThanhId,
                QuanHuyenId = yeuCauTiepNhanMe.QuanHuyenId,
                PhuongXaId = yeuCauTiepNhanMe.PhuongXaId,
                DiaChi = yeuCauTiepNhanMe.DiaChi,
                DanTocId = yeuCauTiepNhanMe.DanTocId,
                Email = yeuCauTiepNhanMe.Email,
                NoiLamViec = yeuCauTiepNhanMe.NoiLamViec,

                NguoiLienHeHoTen = yeuCauTiepNhanMe.HoTen,
                NguoiLienHeSoDienThoai = yeuCauTiepNhanMe.SoDienThoai,
                NguoiLienHeEmail = yeuCauTiepNhanMe.Email,
                NguoiLienHeTinhThanhId = yeuCauTiepNhanMe.TinhThanhId,
                NguoiLienHeQuanHuyenId = yeuCauTiepNhanMe.QuanHuyenId,
                NguoiLienHePhuongXaId = yeuCauTiepNhanMe.PhuongXaId,
                NguoiLienHeDiaChi = yeuCauTiepNhanMe.DiaChi
            };
            await _dieuTriNoiTruService.GetThongTinTiepNhanBenhAnMeAsync(yeuCauTiepNhanCon);
            var viewModel = yeuCauTiepNhanCon.ToModel<BenhAnSoSinhChiTietViewModel>();
            viewModel.MaBenhAnMe = yeuCauTiepNhanMe.NoiTruBenhAn.SoBenhAn;
            viewModel.YeuCauTiepNhanId = yeuCauTiepNhanMe.Id;
            return viewModel;
        }

        [HttpPost("XuLyTaoBenhAnSoSinh")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult<BenhAnSoSinhChiTietViewModel>> XuLyTaoBenhAnSoSinhAsync([FromBody] BenhAnSoSinhChiTietViewModel yeuCauViewModel)
        {
            // gọi function KiemTraTaoBenhAnSoSinh kiểm tra số lượng bệnh án sơ sinh đã tạo
            //var kiemTra = await _dieuTriNoiTruService.KiemTraTaoBenhAnSoSinhAsync(yeuCauViewModel.YeuCauTiepNhanId);
            //if (!kiemTra)
            //{
            //    throw new ApiException(_localizationService.GetResource("BenhAnSoSinh.SoLuongBenhAnCon.Enough"));
            //}

            await XuLyKiemTraTaoBenhAnSoSinhAsync(yeuCauViewModel.YeuCauTiepNhanId);

            var thongTinBenhAn = yeuCauViewModel.ToEntity<YeuCauTiepNhan>();
            var phongId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienService.GetByIdAsync(phongId, x => x.Include(y => y.KhoaPhong));

            await _dieuTriNoiTruService.XuLyTaoBenhAnSoSinhAsync(thongTinBenhAn, yeuCauViewModel.YeuCauTiepNhanId, (long)yeuCauViewModel.KhoaChuyenBenhAnSoSinhVeId, (DateTime)yeuCauViewModel.LucDeSoSinh, yeuCauViewModel.YeuCauGoiDichVuId);

            yeuCauViewModel.ResultKhoaNhapVienId = phongBenhVien.KhoaPhongId;
            yeuCauViewModel.ResultTenKhoaNhapVien = phongBenhVien.KhoaPhong.Ten;
            yeuCauViewModel.ResultYeuCauTiepNhanId = thongTinBenhAn.Id;
            yeuCauViewModel.ResultBenhNhanId = thongTinBenhAn.BenhNhanId;

            var khoaSan = _cauHinhService.GetSetting("CauHinhNoiTru.KhoaPhuSan");
            long.TryParse(khoaSan?.Value, out long khoaSanId);
            yeuCauViewModel.ResultKhongCanChiDinhGiuong = yeuCauViewModel.ResultKhoaNhapVienId == khoaSanId;
            return yeuCauViewModel;
        }


        [HttpPost("XuLyTaoBenhAnSoSinhKhacKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoBenhAnSoSinh)]
        public async Task<ActionResult<TaoBenhAnSoSinhKhacKhoaViewModel>> XuLyTaoBenhAnSoSinhKhacKhoa([FromBody] TaoBenhAnSoSinhKhacKhoaViewModel yeuCauViewModel)
        {
            var entity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauViewModel.BenhAnSoSinhChiTietViewModel.YeuCauTiepNhanId, s => s.Include(x => x.NoiTruBenhAn));
            var noiTruBenhAn = entity.NoiTruBenhAn;
            var result = new DieuTriNoiTruTongKetBenhAnViewModel();

            var thongTinBenhAn = yeuCauViewModel.BenhAnSoSinhChiTietViewModel.ToEntity<YeuCauTiepNhan>();
            var phongId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var phongBenhVien = await _phongBenhVienService.GetByIdAsync(phongId, x => x.Include(y => y.KhoaPhong));

            await _dieuTriNoiTruService.XuLyTaoBenhAnSoSinhAsync(thongTinBenhAn, yeuCauViewModel.BenhAnSoSinhChiTietViewModel.YeuCauTiepNhanId, (long)yeuCauViewModel.BenhAnSoSinhChiTietViewModel.KhoaChuyenBenhAnSoSinhVeId, (DateTime)yeuCauViewModel.DacDiemTreSoSinh.DeLuc, yeuCauViewModel.BenhAnSoSinhChiTietViewModel.YeuCauGoiDichVuId);


            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinTongKetBenhAn))
            {
                result = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(noiTruBenhAn.ThongTinTongKetBenhAn);

                yeuCauViewModel.DacDiemTreSoSinh.YeuCauTiepNhanConId = thongTinBenhAn.Id;
                yeuCauViewModel.DacDiemTreSoSinh.GioiTinh = yeuCauViewModel.DacDiemTreSoSinh.GioiTinhId.Value.GetDescription();
                yeuCauViewModel.DacDiemTreSoSinh.TenTinhTrang = yeuCauViewModel.DacDiemTreSoSinh.TinhTrangId.GetDescription();

                result.DacDiemTreSoSinhs.Add(yeuCauViewModel.DacDiemTreSoSinh);
                noiTruBenhAn.ThongTinTongKetBenhAn = JsonConvert.SerializeObject(result);
                await _dieuTriNoiTruService.UpdateAsync(entity);
            }
            else
            {
                result.DacDiemTreSoSinhs = new List<DacDiemTreSoSinh>();

                yeuCauViewModel.DacDiemTreSoSinh.YeuCauTiepNhanConId = thongTinBenhAn.Id;
                yeuCauViewModel.DacDiemTreSoSinh.GioiTinh = yeuCauViewModel.DacDiemTreSoSinh.GioiTinhId.Value.GetDescription();
                yeuCauViewModel.DacDiemTreSoSinh.TenTinhTrang = yeuCauViewModel.DacDiemTreSoSinh.TinhTrangId.GetDescription();

                result.DacDiemTreSoSinhs.Add(yeuCauViewModel.DacDiemTreSoSinh);
                noiTruBenhAn.ThongTinTongKetBenhAn = JsonConvert.SerializeObject(result);
                await _dieuTriNoiTruService.UpdateAsync(entity);
            }

            //await XuLyKiemTraTaoBenhAnSoSinhAsync(yeuCauViewModel.BenhAnSoSinhChiTietViewModel.YeuCauTiepNhanId);

            yeuCauViewModel.BenhAnSoSinhChiTietViewModel.ResultKhoaNhapVienId = phongBenhVien.KhoaPhongId;
            yeuCauViewModel.BenhAnSoSinhChiTietViewModel.ResultTenKhoaNhapVien = phongBenhVien.KhoaPhong.Ten;
            yeuCauViewModel.BenhAnSoSinhChiTietViewModel.ResultYeuCauTiepNhanId = thongTinBenhAn.Id;
            yeuCauViewModel.BenhAnSoSinhChiTietViewModel.ResultBenhNhanId = thongTinBenhAn.BenhNhanId;

            var khoaSan = _cauHinhService.GetSetting("CauHinhNoiTru.KhoaPhuSan");
            long.TryParse(khoaSan?.Value, out long khoaSanId);
            yeuCauViewModel.BenhAnSoSinhChiTietViewModel.ResultKhongCanChiDinhGiuong = yeuCauViewModel.BenhAnSoSinhChiTietViewModel.ResultKhoaNhapVienId == khoaSanId;
            return yeuCauViewModel;
        }



        [HttpGet("BenhAnMeCoConTrungTen")]
        public async Task<ActionResult<bool>> BenhAnMeCoConTrungTen(long yeuCauTiepNhanBenhAnMeId, string hoTen)
        {
            var kiemTraBenhAnMeCoConTrungTen = await _dieuTriNoiTruService.KiemTraBenhAnMeCoConTrungTen(yeuCauTiepNhanBenhAnMeId, hoTen);
            return Ok(kiemTraBenhAnMeCoConTrungTen);
        }

        [HttpPost("GetYeuCauGoiDichVuSoSinhCuaMe")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetYeuCauGoiDichVuSoSinhCuaMeAsync(DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetYeuCauGoiDichVuSoSinhCuaMeAsync(model);
            return Ok(lookup);
        }
        #endregion

        #region hủy nhập viện
        [HttpPut("XuLyHuyNhapVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiepNhanNoiTru)]
        public async Task<ActionResult> XuLyHuyNhapVienAsync([FromBody] HuyNhapVienViewModel yeuCauViewModel)
        {
            var yeuCauTiepNhan = await _dieuTriNoiTruService.GetByIdAsync(yeuCauViewModel.Id,
                x => x.Include(y => y.YeuCauTiepNhanNgoaiTruCanQuyetToan)
                             .Include(y => y.NoiTruBenhAn)
                             .Include(y => y.TaiKhoanBenhNhanThus));
            if (yeuCauTiepNhan.TaiKhoanBenhNhanThus.Any(x => x.DaHuy != true))
            {
                throw new ApiException(_localizationService.GetResource("HuyNhapVien.BenhNhan.PhatSinhChiPhi"));
            }

            if (yeuCauTiepNhan.NoiTruBenhAn != null)
            {
                throw new ApiException(_localizationService.GetResource("HuyNhapVien.YeuCauTiepNhan.DaTaoBenhAn"));
            }

            if (yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToan != null)
            {
                yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToan.QuyetToanTheoNoiTru = false;
            }

            yeuCauTiepNhan.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy;
            yeuCauTiepNhan.ThoiDiemCapNhatTrangThai = DateTime.Now;
            yeuCauTiepNhan.LyDoHuyNhapVien = yeuCauViewModel.LyDo;
            await _dieuTriNoiTruService.UpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }

        #endregion
    }
}
