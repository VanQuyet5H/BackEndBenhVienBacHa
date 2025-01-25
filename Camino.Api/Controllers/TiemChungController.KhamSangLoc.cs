using System;
using System.Collections.Generic;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.TiemChung;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TiemChungs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Api.Controllers
{
    public partial class TiemChungController
    {
        #region get grid
        [HttpPost("GetGridDichVuKyThuatTiemChung")]
        public async Task<List<KhamBenhGoiDichVuGridVo>> GetGridDichVuKyThuatTiemChung(GridChiDinhVuKyThuatTiemChungQueryInfoVo queryInfo)
        {
            var gridData = await _tiemChungService.GetGridDichVuKyThuatTiemChung(queryInfo);
            return gridData;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridHoanThanhKhamSangLoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridHoanThanhKhamSangLocAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _tiemChungService.GetDataForGridHoanThanhTiemChungAsync(queryInfo);
            var gridData = await _tiemChungService.GetDataForGridHoanThanhTiemChungAsyncVer2(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridHoanThanhKhamSangLoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridHoanThanhKhamSangLocAsync([FromBody] QueryInfo queryInfo)
        {
            //var gridData = await _tiemChungService.GetTotalPageForGridHoanThanhTiemChungAsync(queryInfo);
            var gridData = await _tiemChungService.GetTotalPageForGridHoanThanhTiemChungAsyncVer2(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Get list
        [HttpPost("GetKetLuans")]
        public ActionResult GetKetLuans([FromBody]DropDownListRequestModel model)
        {
            var lstKetLuan = _tiemChungService.GetKetLuans(model);

            return Ok(lstKetLuan);
        }

        [HttpPost("GetViTriTiems")]
        public ActionResult GetViTriTiems([FromBody]DropDownListRequestModel model)
        {
            var lstViTriTiem = _tiemChungService.GetViTriTiems(model);

            return Ok(lstViTriTiem);
        }

        [HttpPost("GetVaccines")]
        public async Task<ActionResult> GetVaccines([FromBody]DropDownListRequestModel model)
        {
            var lstVaccine = await _tiemChungService.GetVaccinesAsync(model);

            return Ok(lstVaccine);
        }
        #endregion

        #region Kiểm tra data
        [HttpGet("KiemTraDangKyGoiDichVuTheoBenhNhan")]
        public async Task<ActionResult<bool>> KiemTraDangKyGoiDichVuTheoBenhNhanAsync(long benhNhanId)
        {
            var kiemTra = await _khamBenhService.KiemTraDangKyGoiDichVuTheoBenhNhanAsync(benhNhanId);
            return kiemTra;
        }

        [HttpGet("KiemTraDangKyGoiDichVuVacxinTheoBenhNhan")]
        public async Task<ActionResult<bool>> KiemTraDangKyGoiDichVuVacxinTheoBenhNhan(long benhNhanId)
        {
            var kiemTra = await _tiemChungService.KiemTraDangKyGoiDichVuTheoBenhNhanAsync(benhNhanId);
            return kiemTra;
        }

        //[HttpPost("KiemTraValidationChiDinhDichVuTrongGoiMarketing")]
        //public async Task<ActionResult> KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync([FromBody] TiemChungChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        //{
        //    if (!chiDinhViewModel.DichVus.Any())
        //    {
        //        throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.Required"));
        //    }

        //    if (chiDinhViewModel.DichVus.Any(x => x.SoLuongSuDung == 0))
        //    {
        //        throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongChiDinh.Required"));
        //    }

        //    if (chiDinhViewModel.DichVus.Any(x => x.ViTriTiem == null))
        //    {
        //        throw new ApiException(_localizationService.GetResource("TiemChung.ViTriTiem.Required"));
        //    }

        //    if (chiDinhViewModel.DichVus.Any(x => x.MuiSo == null || x.MuiSo <= 0))
        //    {
        //        throw new ApiException(_localizationService.GetResource("TiemChung.MuiSo.Required"));
        //    }

        //    if (chiDinhViewModel.DichVus.Any(x => x.NoiThucHienId == null || x.NoiThucHienId <= 0))
        //    {
        //        throw new ApiException(_localizationService.GetResource("TiemChung.NoiThucHienId.Required"));
        //    }

        //    var yeuCauVo = chiDinhViewModel.Map<TiemChungChiDinhGoiDichVuTheoBenhNhanVo>();
        //    await _tiemChungService.KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(yeuCauVo);
        //    //await _khamBenhService.KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(yeuCauVo);
        //    return Ok();
        //}

        [HttpPost("KiemTraValidationChiDinhDichVuVacxinTrongGoiMarketing")]
        public async Task<ActionResult> KiemTraValidationChiDinhDichVuVacxinTrongGoiMarketing([FromBody] TiemChungChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        {
            if (!chiDinhViewModel.DichVus.Any())
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.Required"));
            }

            if (chiDinhViewModel.DichVus.Any(x => x.SoLuongSuDung == 0))
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongChiDinh.Required"));
            }

            if (chiDinhViewModel.DichVus.Any(x => x.ViTriTiem == null))
            {
                throw new ApiException(_localizationService.GetResource("TiemChung.ViTriTiem.Required"));
            }

            if (chiDinhViewModel.DichVus.Any(x => x.MuiSo == null || x.MuiSo <= 0))
            {
                throw new ApiException(_localizationService.GetResource("TiemChung.MuiSo.Required"));
            }

            if (chiDinhViewModel.DichVus.Any(x => x.NoiThucHienId == null || x.NoiThucHienId <= 0))
            {
                throw new ApiException(_localizationService.GetResource("TiemChung.NoiThucHienId.Required"));
            }

            var yeuCauVo = chiDinhViewModel.Map<TiemChungChiDinhGoiDichVuTheoBenhNhanVo>();
            await _tiemChungService.KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(yeuCauVo);

            return Ok();
        }

        [HttpPost("KiemTraDichVuTrongGoiMarketingDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>>> KiemTraDichVuTrongGoiMarketingDaCoTheoYeuCauTiepNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        {
            var dichVuTrung = await _khamBenhService.KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVus.Select(a => a.Id).ToList(), chiDinhViewModel.NoiTruPhieuDieuTriId);
            return dichVuTrung;
        }

        [HttpPost("KiemTraDichVuVacxinTrongGoiMarketingDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>>> KiemTraDichVuVacxinTrongGoiMarketingDaCoTheoYeuCauTiepNhanAsync([FromBody] TiemChungChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        {
            var dichVuTrung = await _khamBenhService.KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVus.Select(a => a.Id).ToList(), chiDinhViewModel.NoiTruPhieuDieuTriId);
            return dichVuTrung;
        }

        [HttpPost("KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<bool>> KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhanAsync([FromBody] KhamBenhChiDinhDichVuKyThuatMultiselectViewModel chiDinhViewModel)
        {
            var kiemtra = await _yeuCauKhamBenhService.KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVuKyThuatBenhVienChiDinhs);
            return kiemtra;
        }

        [HttpGet("KiemTraTatCaVacxinDaThucHien")]
        public async Task<ActionResult<bool>> KiemTraTatCaVacxinDaThucHienAsync(long yeuCauKhamSangLocId)
        {
            var kiemTra = await _tiemChungService.KiemTraTatCaVacxinDaThucHienAsync(yeuCauKhamSangLocId);
            return kiemTra;
        }
        #endregion

        #region Thêm/Xóa/Sửa
        [HttpPost("GetThongTinDichVuKyThuatChoVacxin")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<YeuCauKhamTiemChungViewModel>> GetThongTinDichVuKyThuatChoVacxin(YeuCauKhamTiemChungViewModel yeuCauKhamTiemChungViewModel)
        {
            var tenGoi = yeuCauKhamTiemChungViewModel.TenGoiDichVu;

            var yeuCauTiepNhan = await _tiemChungService.GetByIdAsync(yeuCauKhamTiemChungViewModel.YeuCauTiepNhanId, o => o.Include(p => p.DoiTuongUuDai).ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                                                                                                                           .Include(p => p.YeuCauDichVuKyThuats));

            var yeuCauKhamTiemChungVo = yeuCauKhamTiemChungViewModel.Map<YeuCauKhamTiemChungVo>();

            var yeuCauDichVuKyThuat = await _tiemChungService.ThemChiDinhVacxinAsync(yeuCauKhamTiemChungVo, yeuCauTiepNhan);

            var currentYeuCauDichVuKyThuatSangLoc = yeuCauTiepNhan.YeuCauDichVuKyThuats.First(p => p.Id == yeuCauKhamTiemChungViewModel.Id);
            yeuCauDichVuKyThuat.NoiTruPhieuDieuTriId = currentYeuCauDichVuKyThuatSangLoc.NoiTruPhieuDieuTriId;

            //BVHD-3825
            var result = yeuCauDichVuKyThuat.ToModel(yeuCauKhamTiemChungViewModel);
            result.TenGoiDichVu = tenGoi;
            result.YeuCauGoiDichVuKhuyenMaiId = yeuCauKhamTiemChungViewModel.YeuCauGoiDichVuKhuyenMaiId;
            return result;
        }

        [HttpGet("KiemTraXoaDichVuKyThuatChoVacxin")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult> KiemTraXoaDichVuKyThuatChoVacxin(long yeuCauDichVuKyThuatTiemChungId, bool IsSuaDichVuVacxin = false)
        {
            if(yeuCauDichVuKyThuatTiemChungId != 0)
            {
                var yeuCauDichVuKyThuat = await _yeuCauDichVuKyThuatService.GetByIdAsync(yeuCauDichVuKyThuatTiemChungId, 
                    o => o.Include(p => p.TaiKhoanBenhNhanChis).Include(p => p.MienGiamChiPhis));

                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                {
                    if(IsSuaDichVuVacxin)
                    {
                        throw new Exception(_localizationService.GetResource("TiemChung.ChiDinhVacxin.SuaDichVu.DaThanhToan"));
                    }
                    else
                    {
                        throw new Exception(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                    }
                }

                if (yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && yeuCauDichVuKyThuat.TaiKhoanBenhNhanChis.Any())
                {
                    if (IsSuaDichVuVacxin)
                    {
                        throw new Exception(_localizationService.GetResource("TiemChung.ChiDinhVacxin.SuaDichVu.DaHuyThanhToan"));
                    }
                    else
                    {
                        throw new Exception(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                    }
                }
            }

            return Ok();
        }

        [HttpPost("LuuKhamSangLocChung")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<YeuCauKhamTiemChungViewModel>> LuuKhamSangLocChung(YeuCauKhamTiemChungViewModel yeuCauKhamTiemChungViewModel)
        {
            //if (yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.NgayHenTiemMuiTiepTheo != null && yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.NgayHenTiemMuiTiepTheo.Value.Date <= DateTime.Now.Date)
            if (yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.SoNgayHenTiemMuiTiepTheo != null && yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.SoNgayHenTiemMuiTiepTheo.Value <= 0)
            {
                throw new ApiException(_localizationService.GetResource("TiemChung.NgayHenTiemMuiTiepTheo.GreaterThanToday"), (int)HttpStatusCode.BadRequest);
            }

            /* Kiểm tra kết quả sinh hiệu */
            var lstKetQuaSinhHieu = yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.KetQuaSinhHieus;

            if (lstKetQuaSinhHieu.Any())
            {
                lstKetQuaSinhHieu = lstKetQuaSinhHieu.Where(x => x.Id == 0).ToList();

                foreach (var item in lstKetQuaSinhHieu)
                {
                    if(item.NhipTim == null && (item.HuyetApTamThu == null && item.HuyetApTamTruong == null) && item.ThanNhiet == null && item.NhipTho == null && item.CanNang == null && item.ChieuCao == null && item.BMI == null && item.Glassgow == null && item.SpO2 == null)
                    {
                        throw new ApiException(_localizationService.GetResource("KhamBenh.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
                    }

                    if((item.HuyetApTamThu != null && item.HuyetApTamTruong == null) ||
                        (item.HuyetApTamThu == null && item.HuyetApTamTruong != null))
                    {
                        throw new ApiException(_localizationService.GetResource("KhamBenh.TamThuTamTruong.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
                    }
                }
            }
            /* */

            var currentPhongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var queryInfo = new HangDoiTiemChungDangKhamQuyeryInfo() {
                PhongKhamHienTaiId = currentPhongBenhVienId,
                YeuCauKhamTiemChungId = yeuCauKhamTiemChungViewModel.Id,
                TrangThaiHangDoi = EnumTrangThaiHangDoi.DangKham,
                LoaiHangDoi = LoaiHangDoiTiemVacxin.KhamSangLoc
            };

            //var yeuCauKhamTiemChung = await _tiemChungService.GetYeuCauKhamTiemChungDangKhamTheoPhongKhamAsync(queryInfo);
            //var yeuCauKhamTiemChung = await _tiemChungService.GetYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(queryInfo);
            var yeuCauKhamTiemChung = await _tiemChungService.GetThongTinLuuYeuCauKhamTiemChungDangKhamTheoPhongKhamAsyncVer2(queryInfo);

            if (yeuCauKhamTiemChung == null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            if (yeuCauKhamTiemChung.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
            {
                throw new ApiException(_localizationService.GetResource("TiemChung.DaHoanThanh.Error"));
            }

            yeuCauKhamTiemChungViewModel.ToEntity(yeuCauKhamTiemChung);

            _tiemChungService.KiemTraKetLuanPhuHopVoiHuongDan(yeuCauKhamTiemChung);

            /* Map tay YCDVKT Tiêm chủng */
            foreach (var item in yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats)
            {
                if (item.Id == 0)
                {
                    var yeuCauDichVuKyThuatEntity = new YeuCauDichVuKyThuat();
                    item.ToEntity(yeuCauDichVuKyThuatEntity);

                    yeuCauKhamTiemChung.YeuCauTiepNhan.YeuCauDichVuKyThuats.Add(yeuCauDichVuKyThuatEntity);
                    yeuCauKhamTiemChung.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Add(yeuCauDichVuKyThuatEntity);
                }
            }

            foreach (var item in yeuCauKhamTiemChung.YeuCauTiepNhan.YeuCauDichVuKyThuats)
            {
                if (item.Id != 0 && item.TiemChung != null)
                {
                    var ycdvkt = yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats.FirstOrDefault(x => x.Id == item.Id);
                    //var isExisted = yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(x => x.Id == item.Id);

                    //if (!isExisted)
                    if (ycdvkt == null || (ycdvkt.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && !string.IsNullOrEmpty(ycdvkt.LyDoHuyDichVu)))
                    {
                        if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                        }

                        if (string.IsNullOrEmpty(ycdvkt?.LyDoHuyDichVu ?? string.Empty) && item.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && item.TaiKhoanBenhNhanChis.Any())
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                        }

                        if(!string.IsNullOrEmpty(ycdvkt?.LyDoHuyDichVu ?? string.Empty))
                        {
                            item.NhanVienHuyDichVuId = currentUserId;
                            item.LyDoHuyDichVu = ycdvkt.LyDoHuyDichVu;
                        }

                        item.WillDelete = true;
                        item.TiemChung.XuatKhoDuocPhamChiTiet.WillDelete = true;

                        foreach (var xuatKhoViTri in item.TiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
                        {
                            xuatKhoViTri.WillDelete = true;
                        }

                        foreach (var mienGiamChiPhi in item.MienGiamChiPhis.Where(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                        {
                            mienGiamChiPhi.DaHuy = true;
                            mienGiamChiPhi.WillDelete = true;

                            var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiamChiPhi.SoTien;
                            if (giamSoTienMienGiam < 0)
                            {
                                giamSoTienMienGiam = 0;
                            }
                            item.SoTienMienGiam = giamSoTienMienGiam;
                        }
                    }
                }
            }
            /* */

            /* Kiểm tra vaccine */
            if (yeuCauKhamTiemChung.KhamSangLocTiemChung.KetLuan != LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem)
            {
                if (yeuCauKhamTiemChung.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(p => p.TiemChung != null && p.WillDelete != true && p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                {
                    throw new Exception(_localizationService.GetResource("TiemChung.KhongDongY.TamHoanTiemChung.ChongChiDinh.Vacxin"));
                }
            }
            /* */

            var yeuCauDichVuKyThuatCuoiCung = yeuCauKhamTiemChung.YeuCauTiepNhan.YeuCauDichVuKyThuats.Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                                                                                                   p.Id != 0)
                                                                                                       .OrderByDescending(p => p.Id)
                                                                                                       .FirstOrDefault();

            var yeuCauDichVuKyThuatCuoiCungId = yeuCauDichVuKyThuatCuoiCung != null ? yeuCauDichVuKyThuatCuoiCung.Id : 0;



            //var nhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>();
            //await _tiemChungService.XuLySoLuongChiDinhVacxinAsync(yeuCauKhamTiemChung.KhamSangLocTiemChung.YeuCauDichVuKyThuats, nhapKhoDuocPhamChiTiets);
            await _tiemChungService.XuLySoLuongChiDinhVacxinAsyncVer2(yeuCauKhamTiemChung.KhamSangLocTiemChung.YeuCauDichVuKyThuats);

            /* Cập nhật YCDVKT nếu mới tạo */
            if (yeuCauKhamTiemChung.KhamSangLocTiemChung.Id == 0)
            {
                yeuCauKhamTiemChung.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien;
                yeuCauKhamTiemChung.NhanVienThucHienId = currentUserId;
                yeuCauKhamTiemChung.ThoiDiemThucHien = DateTime.Now;
            }
            /* */

            // gọi hàm chung
            await _tiemChungService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauKhamTiemChung.YeuCauTiepNhan, false);
            await _tiemChungService.PrepareForAddDichVuAndUpdateAsync(yeuCauKhamTiemChung.YeuCauTiepNhan);

            //// cập nhật lại số lượng tồn
            //if (nhapKhoDuocPhamChiTiets.Any())
            //{
            //    await _khamBenhService.CapNhatSoLuongTonKhiGhiNhanVTTHThuocAsync(nhapKhoDuocPhamChiTiets, new List<NhapKhoVatTuChiTiet>());
            //}

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuKhamSangLocTiemChungVo();

            if (yeuCauKhamTiemChung.NoiTruPhieuDieuTriId != null)
            {
                decimal soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauKhamTiemChung.YeuCauTiepNhanId);
                //decimal chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauKhamTiemChung.YeuCauTiepNhanId).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                decimal chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauKhamTiemChung.YeuCauTiepNhanId).Result;

                chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
                chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                decimal soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauKhamTiemChung.YeuCauTiepNhanId);
                decimal soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauKhamTiemChung.YeuCauTiepNhanId);

                chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
                chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;
            }

            if (yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(p => p.Id == 0))
            {
                var yeuCauVuaThem = yeuCauKhamTiemChung.YeuCauTiepNhan.YeuCauDichVuKyThuats.Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                                                                                       (!yeuCauKhamTiemChungViewModel.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(p => p.Id == 0) || (x.Id > yeuCauDichVuKyThuatCuoiCungId)))
                                                                                           .ToList();

                chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVuaThem.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);
            }

            //kiểm tra kết luận có phù hợp
            //chiDinhDichVuResultVo.IsKhacKetLuanVoiBYT = !_tiemChungService.KiemTraKetLuanPhuHopVoiHuongDan(yeuCauKhamTiemChung);

            return Ok(chiDinhDichVuResultVo);
        }

        [HttpPost("XuLyBatDauKhamBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiemChungKhamSangLoc, DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult> XuLyBatDauKhamBenhNhan(HangDoiTiemChungInputViewModel hangDoiTiemChungInputView)
        {
            //HangDoiVacXinId là trường hợp thực hiện tiêm, hàng đợi sẽ load theo vacxin
            var hangDoiBatDauKham =
                _yeuCauDichVuKyThuatService.GetById(hangDoiTiemChungInputView.HangDoiVacXinId ?? hangDoiTiemChungInputView.HangDoiBatDauKhamId,
                    x => x.Include(y => y.PhongBenhVienHangDois)
                        .Include(y => y.KhamSangLocTiemChung).ThenInclude(y => y.YeuCauDichVuKyThuats).ThenInclude(y => y.PhongBenhVienHangDois));
            if (hangDoiBatDauKham == null 
                || (hangDoiTiemChungInputView.HangDoiVacXinId == null && hangDoiBatDauKham.LoaiDichVuKyThuat != LoaiDichVuKyThuat.SangLocTiemChung) 
                || (hangDoiTiemChungInputView.HangDoiVacXinId != null && hangDoiBatDauKham.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SangLocTiemChung))
            {
                return NotFound();
            }
            await _tiemChungService.KiemTraDatayeuCauTiemChungAsync(hangDoiTiemChungInputView.HangDoiBatDauKhamId, hangDoiBatDauKham.NoiThucHienId ?? 0, Enums.EnumTrangThaiHangDoi.ChoKham, hangDoiTiemChungInputView.HangDoiVacXinId);

            var coBenhNhanKhacDangKham =
                await _tiemChungService.KiemTraCoBenhNhanKhacDangKhamTiemChungTrongPhong(hangDoiTiemChungInputView.HangDoiDangKhamId, hangDoiBatDauKham.NoiThucHienId ?? 0, hangDoiTiemChungInputView.LoaiHangDoi);
            if (coBenhNhanKhacDangKham)
            {
                throw new ApiException(_localizationService.GetResource("TiemChung.BatDauKham.CoBenhNhanKhacDangKhamTrongPhong"));
            }

            if (hangDoiTiemChungInputView.HangDoiDangKhamId != null && hangDoiTiemChungInputView.HangDoiDangKhamId != 0)
            {
                await XuLyHoanThanhCongDoanKhamTiemChungHienTaiCuaBenhNhanAsync(hangDoiTiemChungInputView.HangDoiDangKhamId.Value, hangDoiTiemChungInputView.HoanThanhKham, hangDoiTiemChungInputView.HangDoiVacXinId);
            }

            var phongBenhVienHangDoiBatDauKham = new PhongBenhVienHangDoi();
            if (hangDoiTiemChungInputView.HangDoiVacXinId == null)
            {
                phongBenhVienHangDoiBatDauKham = hangDoiBatDauKham.PhongBenhVienHangDois
                    .Where(x => x.YeuCauKhamBenhId == null
                                && x.PhongBenhVienId == hangDoiBatDauKham.NoiThucHienId)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();
            }
            else
            {
                phongBenhVienHangDoiBatDauKham = hangDoiBatDauKham.PhongBenhVienHangDois
                    .Where(x => x.YeuCauKhamBenhId == null
                                && x.PhongBenhVienId == hangDoiBatDauKham.NoiThucHienId)
                    .OrderByDescending(x => x.TrangThai == EnumTrangThaiHangDoi.DangKham).ThenBy(x => x.Id)
                    .FirstOrDefault();
            }
            if (phongBenhVienHangDoiBatDauKham == null)
            {
                var soThuTuMax = await _tiemChungService.GetSoThuTuTiepTheoTrongHangDoiTiemChungAsync(hangDoiBatDauKham.NoiThucHienId.Value);
                phongBenhVienHangDoiBatDauKham = new PhongBenhVienHangDoi()
                {
                    YeuCauTiepNhanId = hangDoiBatDauKham.YeuCauTiepNhanId,
                    YeuCauDichVuKyThuatId = hangDoiBatDauKham.Id,
                    PhongBenhVienId = hangDoiBatDauKham.NoiThucHienId.Value,
                    TrangThai = EnumTrangThaiHangDoi.DangKham,
                    SoThuTu = soThuTuMax
                };
                hangDoiBatDauKham.PhongBenhVienHangDois.Add(phongBenhVienHangDoiBatDauKham);
            }
            else
            {
                phongBenhVienHangDoiBatDauKham.TrangThai = Enums.EnumTrangThaiHangDoi.DangKham;
            }

            await _yeuCauDichVuKyThuatService.UpdateAsync(hangDoiBatDauKham);
            return Ok();
        }

        [HttpPost("XuLyHoanThanhCongDoanKhamTiemChungHienTaiCuaBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiemChungKhamSangLoc, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult> XuLyHoanThanhCongDoanKhamTiemChungHienTaiCuaBenhNhanAsync(long yeuCauDichVuKyThuatId, bool hoanThanhKham = false, long? yeuCauDichVuKyThuatVacxinId = null)
        {
            await _tiemChungService.KiemTraDatayeuCauTiemChungAsync(yeuCauDichVuKyThuatId, 0, EnumTrangThaiHangDoi.DangKham, yeuCauDichVuKyThuatVacxinId);

            await _tiemChungService.XuLyHoanThanhCongDoanKhamTiemChungHienTaiCuaBenhNhan(yeuCauDichVuKyThuatId, hoanThanhKham, yeuCauDichVuKyThuatVacxinId);
            return Ok();
        }

        [HttpPost("ThemYeuCauDichVuKyThuatMultiSelect")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuCauDichVuKyThuatMultiselectAsync([FromBody] PhauThuatThuThuatChiDinhDichVuKyThuatMultiSelectViewModel yeuCauViewModel)
        {
            var yeuCauVo = yeuCauViewModel.Map<ChiDinhDichVuKyThuatMultiselectVo>();

            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            // kiểm tra nếu là dịch vụ thêm từ gói
            var dichVuChiDinhTheoGoiVo = new ChiDinhGoiDichVuTheoBenhNhanVo()
            {
                YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                NoiTruPhieuDieuTriId = yeuCauViewModel.PhieuDieuTriId
            };
            if (yeuCauViewModel.DichVuKyThuatTuGois != null && yeuCauViewModel.DichVuKyThuatTuGois.Any())
            {
                dichVuChiDinhTheoGoiVo.DichVus.AddRange(yeuCauViewModel.DichVuKyThuatTuGois
                    .Select(item => new ChiTietGoiDichVuChiDinhTheoBenhNhanVo()
                    {
                        Id = item.Id,
                        YeuCauGoiDichVuId = item.YeuCauGoiDichVuId ?? 0,
                        TenDichVu = item.TenDichVu,
                        ChuongTrinhGoiDichVuId = item.ChuongTrinhGoiDichVuId ?? 0,
                        ChuongTrinhGoiDichVuChiTietId = item.ChuongTrinhGoiDichVuChiTietId ?? 0,
                        DichVuBenhVienId = item.DichVuBenhVienId ?? 0,
                        NhomGoiDichVu = (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                        SoLuongSuDung = 1
                    }));

                var lstDichVuKyThuatDangChon = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.Select(x => x).ToList();
                foreach (var strId in lstDichVuKyThuatDangChon)
                {
                    var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(strId);
                    if (dichVuChiDinhTheoGoiVo.DichVus.Any(x => x.DichVuBenhVienId == itemObj.DichVuId))
                    {
                        var dichVuLoaiBo = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.FirstOrDefault(x => x == strId);
                        yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Remove(dichVuLoaiBo);
                    }
                }

                await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, dichVuChiDinhTheoGoiVo);
            }
            if (yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Any())
            {
                await _phauThuatThuThuatService.XuLyThemYeuCauDichVuKyThuatMultiselectAsync(yeuCauVo, yeuCauTiepNhanChiTiet);
            }

            await _tiemChungService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            if (yeuCauTiepNhanChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru || yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru == true)
            {
                decimal soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                //decimal chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanChiTiet.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                decimal chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhanChiTiet.Id).Result;

                chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
                chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                decimal soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                decimal soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

                chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
                chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;
            }

            if (yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Any())
            {
                var yeuCauVuaThem = yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats
                    .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                 && (yeuCauVo.YeuCauDichVuKyThuatCuoiCungId == 0 || (x.Id > yeuCauVo.YeuCauDichVuKyThuatCuoiCungId))
                                 && x.YeuCauGoiDichVuId == null)
                    .ToList();
                chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVuaThem.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan
                                                                                     && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan);
            }

            return chiDinhDichVuResultVo;
        }

        [HttpPost("ThemChiDinhGoiDichVuTheoBenhNhan")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.TiemChungKhamSangLoc, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemChiDinhGoiDichVuTheoBenhNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuTheoBenhNhanVo>();
            await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiemChungService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.IsVuotQuaBaoLanhGoi = yeuCauVo.IsVuotQuaBaoLanhGoi;
            return chiDinhDichVuResultVo;
        }

        [HttpPost("ThemChiDinhGoiDichVuVacxinTheoBenhNhan")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<TiemChungChiDinhDichVuViewModel>> ThemChiDinhGoiDichVuVacxinTheoBenhNhanAsync([FromBody] TiemChungChiDinhGoiDichVuTheoBenhNhanViewModel yeuCauViewModel)
        {
            //get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauVo = yeuCauViewModel.Map<TiemChungChiDinhGoiDichVuTheoBenhNhanVo>();
            await _tiemChungService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVu = new TiemChungChiDinhDichVuViewModel();
            chiDinhDichVu.IsVuotQuaBaoLanhGoi = yeuCauVo.IsVuotQuaBaoLanhGoi;

            var lstYeuCauGoiDichVuId =
                yeuCauVo.YeuCauDichVuKyThuatNews
                    .Where(x => x.YeuCauGoiDichVuId != null)
                    .Select(x => x.YeuCauGoiDichVuId.Value).Distinct().ToList();
            var lstTenGoiDichVu = await _tiemChungService.GetListTenGoiDichVu(lstYeuCauGoiDichVuId);

            foreach (var item in yeuCauVo.YeuCauDichVuKyThuatNews)
            {
                var vacXinMoiThem = item.ToModel<YeuCauKhamTiemChungViewModel>();

                if (vacXinMoiThem.YeuCauGoiDichVuId != null)
                {
                    vacXinMoiThem.TenGoiDichVu = lstTenGoiDichVu
                        .Where(x => x.KeyId == vacXinMoiThem.YeuCauGoiDichVuId.Value).Select(x => x.DisplayName)
                        .FirstOrDefault();
                }

                chiDinhDichVu.YeuCauDichVuKyThuats.Add(vacXinMoiThem);
            }

            return chiDinhDichVu;
        }

        [HttpPut("XoaDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.TiemChungKhamSangLoc, Enums.DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> XoaDichVuKyThuat(ChiDinhNgoaiTruCanXoaViewModel xoaDichVuViewModel)
        {
            var entity = _yeuCauDichVuKyThuatService.GetById(xoaDichVuViewModel.DichVuId);
            if (entity != null && (entity.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien || entity.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien))
            {
                throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.DichVuKyThuat.DaThucHien"));
            }

            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(entity.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaRiengDichVuKyThuatNgoaiTruByIdAsync(entity.YeuCauTiepNhanId);
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats)
            {
                if (item.Id == xoaDichVuViewModel.DichVuId)
                {
                    if (yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauDichVuKyThuatId == item.Id && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                        || yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Any(x => x.YeuCauDichVuKyThuatId == item.Id && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
                    {
                        throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaCoGhiNhanVTTHThuoc"));
                    }
                    item.WillDelete = true;
                    if (!string.IsNullOrEmpty(xoaDichVuViewModel.LyDoHuyDichVu))
                    {
                        if (item.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                        }
                        item.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
                        item.LyDoHuyDichVu = xoaDichVuViewModel.LyDoHuyDichVu;
                    }
                    else
                    {
                        //if (item.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && item.TaiKhoanBenhNhanChis.Any())
                        //{
                        //    throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                        //}

                        if (item.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan)
                        {
                            var coChi = await _khamBenhService.KiemTraTaiKhoanBenhNhanChiTheoDichVu(yeuCauKyThuatId: item.Id);
                            if (coChi)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                            }
                        }
                    }

                    //BVHD-3825
                    //var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                    var mienGiam = await _khamBenhService.GetMienGiamChiPhiTrongGoiTheoDichVu(yeuCauKyThuatId: item.Id);
                    if (mienGiam != null)
                    {
                        mienGiam.DaHuy = true;
                        mienGiam.WillDelete = true;

                        var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                        if (giamSoTienMienGiam < 0)
                        {
                            giamSoTienMienGiam = 0;
                        }
                        item.SoTienMienGiam = giamSoTienMienGiam;
                    }
                    break;
                }
            }

            await _tiemChungService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return Ok();
        }

        [HttpPut("CapNhatGhiChuCanLamSang")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.TiemChungKhamSangLoc, DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult> CapNhatGhiChuCanLamSangAsync(UpdateGhiChuCanLamSangVo updateVo)
        {
            await _khamBenhService.CapNhatGhiChuCanLamSangAsync(updateVo);
            return Ok();
        }

        [HttpPost("CapNhatGridItemDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.TiemChungKhamSangLoc, DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult> CapNhatGridItemDichVuKyThuatAsync(GridItemYeuCauDichVuKyThuatViewModel viewModel)
        {
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaRiengDichVuKyThuatNgoaiTruByIdAsync(viewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiCapNhatRiengDichVuKyThuatTiemChungByIdAsync(viewModel.YeuCauTiepNhanId);

            var flagUpdate = false;
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats)
            {
                if (item.Id == viewModel.YeuCauDichVuKyThuatId)
                {
                    flagUpdate = true;

                    long? yeuCauGoiDichVuKhuyenMaiId = null;
                    if (viewModel.IsUpdateLoaiGia)
                    {
                        if (viewModel.NhomGiaDichVuKyThuatBenhVienId != null && item.NhomGiaDichVuKyThuatBenhVienId != viewModel.NhomGiaDichVuKyThuatBenhVienId)
                        {
                            if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.DaThanhToan"));
                            }

                            item.NhomGiaDichVuKyThuatBenhVienId = viewModel.NhomGiaDichVuKyThuatBenhVienId.Value;
                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(item.DichVuKyThuatBenhVienId, viewModel.NhomGiaDichVuKyThuatBenhVienId.Value);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            //BVHD-3825
                            // kiểm tra nếu là dịch vụ khuyến mãi từ gói marketing
                            if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                            {
                                viewModel.IsSwapDichVuKhuyenMai = true;
                                viewModel.LaDichVuKhuyenMai = true;
                                yeuCauGoiDichVuKhuyenMaiId = item.MienGiamChiPhis
                                    .First(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true))
                                    .YeuCauGoiDichVuId.Value;
                            }

                            item.Gia = donGiaBenhVien;
                        }
                    }

                    if (viewModel.IsUpdateSoLan)
                    {
                        if (viewModel.SoLan == null || viewModel.SoLan == 0)
                        {
                            throw new ApiException(_localizationService.GetResource("DichVuKyThuat.SoLan.Range"));
                        }
                        // kiểm tra nếu là dịch vụ chỉ đinh từ gói marketing
                        if (item.YeuCauGoiDichVuId != null)
                        {
                            var soLuongConLai = await _khamBenhService.GetSoLuongConLaiDichVuKyThuatTrongGoiMarketingBenhNhanAsync(item.YeuCauGoiDichVuId.Value, item.DichVuKyThuatBenhVienId);
                            var soLuongKhaDung = soLuongConLai + item.SoLan;
                            if (soLuongKhaDung < viewModel.SoLan)
                            {
                                throw new ApiException(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), item.TenDichVu, soLuongKhaDung));
                            }
                        }
                        // kiểm tra nếu là dịch vụ khuyến mãi từ gói marketing
                        if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                        {
                            var yeuCauGoiId = item.MienGiamChiPhis
                                .First(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true))
                                .YeuCauGoiDichVuId.Value;
                            var soLuongConLai = await _khamBenhService.GetSoLuongConLaiDichVuKyThuatKhuyenMaiTrongGoiMarketingBenhNhanAsync(yeuCauGoiId, item.DichVuKyThuatBenhVienId);
                            var soLuongKhaDung = soLuongConLai + item.SoLan;
                            if (soLuongKhaDung < viewModel.SoLan)
                            {
                                throw new ApiException(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), item.TenDichVu, soLuongKhaDung));
                            }

                            viewModel.IsSwapDichVuKhuyenMai = true;
                            viewModel.LaDichVuKhuyenMai = true;
                            yeuCauGoiDichVuKhuyenMaiId = yeuCauGoiId;
                        }

                        item.SoLan = viewModel.SoLan != null ? viewModel.SoLan.Value : item.SoLan;
                    }

                    if (viewModel.IsUpdateNoiThucHien)
                    {
                        if (viewModel.NoiThucHienId == null || viewModel.NoiThucHienId == 0)
                        {
                            throw new ApiException(_localizationService.GetResource("ycdvcls.NoiThucHienId.Required"));
                        }

                        var queryInfo = new DropDownListRequestModel()
                        {
                            //Id = viewModel.NoiThucHienId.Value,
                            ParameterDependencies = "{NoiThucHienId:" + viewModel.NoiThucHienId.Value + "}",
                            Take = 50
                        };

                        var lstBacSiKhams = await _yeuCauKhamBenhService.GetBacSiKhams(queryInfo);

                        item.NoiThucHienId = viewModel.NoiThucHienId;

                        // nếu nơi thực hiện mới ko có bác sĩ đang chọn, thì tự dộng chọn bác sĩ đầu tiên
                        if (lstBacSiKhams.Any() && !lstBacSiKhams.Any(x => x.KeyId == viewModel.NguoiThucHienId))
                        {
                            item.NhanVienThucHienId = lstBacSiKhams.First().KeyId;
                        }
                        else
                        {
                            item.NhanVienThucHienId = null;
                        }
                    }

                    if (viewModel.IsUpdateNguoiThucHien)
                    {
                        item.NhanVienThucHienId = viewModel.NguoiThucHienId;
                    }

                    if (viewModel.IsUpdateBenhPhamXetNghiem)
                    {
                        item.BenhPhamXetNghiem = viewModel.BenhPhamXetNghiem;
                    }

                    if (viewModel.IsUpdateGioThucHien)
                    {
                        var ngayDieuTri = item.NoiTruPhieuDieuTri?.NgayDieuTri ?? DateTime.Now;
                        var gioDangKy = viewModel.GioBatDau ?? DateTime.Now;
                        item.ThoiDiemDangKy = ngayDieuTri.Date.AddSeconds(gioDangKy.Hour * 3600 + gioDangKy.Minute * 60); //viewModel.GioBatDau ?? DateTime.Now;
                    }

                    if (viewModel.IsUpdateDuocHuongBaoHiem)
                    {
                        item.DuocHuongBaoHiem = viewModel.DuocHuongBaoHiem ?? false;
                    }

                    if (viewModel.IsSwapDichVuGoi)
                    {
                        if (viewModel.LaDichVuTrongGoi == true)
                        {
                            var thongTin = new ThongTinDichVuTrongGoi()
                            {
                                BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                DichVuId = item.DichVuKyThuatBenhVienId,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                SoLuong = item.SoLan
                            };
                            await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                            item.Gia = thongTin.DonGia;
                            item.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                            item.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                            item.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;

                            if (item.MienGiamChiPhis.Any(x => x.DaHuy != true))
                            {
                                item.SoTienBaoHiemTuNhanChiTra = null;
                                item.SoTienMienGiam = null;
                                foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true))
                                {
                                    mienGiam.WillDelete = true;
                                }

                                foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                {
                                    congNo.WillDelete = true;
                                }
                            }
                        }
                        else
                        {
                            item.DonGiaTruocChietKhau = null;
                            item.DonGiaSauChietKhau = null;
                            item.YeuCauGoiDichVuId = null;

                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(item.DichVuKyThuatBenhVienId, item.NhomGiaDichVuKyThuatBenhVienId);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            item.Gia = donGiaBenhVien;
                        }
                    }

                    //BVHD-3825
                    if (viewModel.IsSwapDichVuKhuyenMai)
                    {
                        if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("DichVuKhuyenMai.TrangThaiYeuCauDichVu.DaThanhToan"));
                        }

                        if (viewModel.LaDichVuKhuyenMai == true)
                        {
                            var thongTin = new ThongTinDichVuTrongGoi()
                            {
                                BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                DichVuId = item.DichVuKyThuatBenhVienId,
                                NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                SoLuong = item.SoLan,
                                NhomGiaId = item.NhomGiaDichVuKyThuatBenhVienId
                            };

                            //dùng cho trường hợp cập nhật số lượng hoặc loại giá
                            if (viewModel.IsUpdateLoaiGia || viewModel.IsUpdateSoLan)
                            {
                                thongTin.YeuCauDichVuCapNhatSoLuongLoaiGiaId = item.Id;
                                thongTin.YeucauGoiDichVuKhuyenMaiId = yeuCauGoiDichVuKhuyenMaiId;
                            }

                            await _tiepNhanBenhNhanService.GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(thongTin);

                            if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true) && x.YeuCauGoiDichVuId != null))
                            {
                                //item.SoTienBaoHiemTuNhanChiTra = null;
                                //item.SoTienMienGiam = null;
                                foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true && (a.TaiKhoanBenhNhanThuId == null || a.TaiKhoanBenhNhanThu.DaHuy != true) && a.YeuCauGoiDichVuId != null))
                                {
                                    mienGiam.DaHuy = true;
                                    mienGiam.WillDelete = true;

                                    var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                    if (giamSoTienMienGiam < 0)
                                    {
                                        giamSoTienMienGiam = 0;
                                    }
                                    item.SoTienMienGiam = giamSoTienMienGiam;
                                }

                                //foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                //{
                                //    congNo.WillDelete = true;
                                //}
                            }

                            //trường hợp cập nhật số lượng hoặc loại giá thì giữ nguyên đơn giá chỉ định trước đó
                            if (!viewModel.IsUpdateLoaiGia && !viewModel.IsUpdateSoLan)
                            {
                                item.Gia = thongTin.DonGia;
                            }

                            var thanhTien = item.SoLan * item.Gia;
                            var thanhTienMienGiam = item.SoLan * thongTin.DonGiaKhuyenMai.Value;

                            var tongTienMienGiam = (thanhTien > thanhTienMienGiam) ? (thanhTien - thanhTienMienGiam) : 0;
                            item.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() + tongTienMienGiam;
                            item.MienGiamChiPhis.Add(new MienGiamChiPhi()
                            {
                                YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                SoTien = item.SoTienMienGiam.Value,
                                YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId
                            });
                        }
                        else
                        {
                            var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(item.DichVuKyThuatBenhVienId, item.NhomGiaDichVuKyThuatBenhVienId);
                            if (donGiaBenhVien == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                            }

                            item.Gia = donGiaBenhVien;
                            var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                            if (mienGiam != null)
                            {
                                mienGiam.DaHuy = true;
                                mienGiam.WillDelete = true;

                                var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                if (giamSoTienMienGiam < 0)
                                {
                                    giamSoTienMienGiam = 0;
                                }
                                item.SoTienMienGiam = giamSoTienMienGiam;
                            }
                        }
                    }
                    break;
                }
            }

            if (!flagUpdate)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            await _tiemChungService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return Ok();
        }

        [HttpPost("ThemYeuGoiDichVuThuongDung")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TiemChungKhamSangLoc, DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuGoiDichVuThuongDungAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            var yeuCauVo = yeuCauViewModel.Map<YeuCauThemGoiDichVuThuongDungVo>();
            await _khamBenhService.XuLyThemGoiDichVuThuongDungAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiemChungService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVo.YeuCauKhamBenhNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);
            return chiDinhDichVuResultVo;
        }

        [HttpPut("XuLyTiemChungQuayLaiChuaKhamAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult> XuLyTiemChungQuayLaiChuaKhamAsync(long yeuCauDichVuKyThuatId)
        {
            // xử lý quay lại chưa khám
            await _tiemChungService.XuLyTiemChungQuayLaiChuaKhamAsync(yeuCauDichVuKyThuatId);
            return Ok();
        }
        #endregion

        #region Chỉ định gói Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuCuaBenhNhanDataForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuCuaBenhNhanDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuCuaBenhNhanTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuCuaBenhNhanDataForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            var yeuCauGoiDichVu = _yeuCauGoiDichVuService.GetById(yeuCauGoiDichVuId);
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;
            var dichVuGiuongDaChiDinhs = await _dieuTriNoiTruService.GetThongTinSuDungDichVuGiuongTrongGoiAsync(benhNhanId ?? 0);

            var gridData = await _khamBenhService.GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo, false, dichVuGiuongDaChiDinhs);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Chỉ định gói vắcxin
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuVacxinCuaBenhNhanDataForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuVacxinCuaBenhNhanDataForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tiemChungService.GetGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuVacxinCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuVacxinCuaBenhNhanTotalPageForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tiemChungService.GetGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuVacxinCuaBenhNhanDataForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuVacxinCuaBenhNhanDataForGrid([FromBody] QueryInfo queryInfo)
        {
            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            var yeuCauGoiDichVu = _yeuCauGoiDichVuService.GetById(yeuCauGoiDichVuId);
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;
            var dichVuGiuongDaChiDinhs = await _tiemChungService.GetThongTinSuDungDichVuGiuongTrongGoiAsync(benhNhanId ?? 0);

            var gridData = await _tiemChungService.GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo, false, dichVuGiuongDaChiDinhs);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuVacxinCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.TiemChungKhamSangLoc)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuVacxinCuaBenhNhanTotalPageForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _tiemChungService.GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region In
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("InBanKiemTruocTiemChungDoiVoiTreEm")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.TiemChungThucHienTiem)]
        public async Task<ActionResult<string>> InBanKiemTruocTiemChungDoiVoiTreEm(InBanKiemTruocTiemChungViewModel inBanKiemTruocTiemChungViewModel)
        {
            var html = await _tiemChungService.InBanKiemTruocTiemChungDoiVoiTreEm(inBanKiemTruocTiemChungViewModel.YeuCauDichVuKyThuatKhamSangLocId, inBanKiemTruocTiemChungViewModel.Hosting);

            return html;
        }
        #endregion
    }
}
