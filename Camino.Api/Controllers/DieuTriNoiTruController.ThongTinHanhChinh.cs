using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Api.Models.YeuCauTiepNhan;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHanhChinh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<DieuTriNoiTruThongTinHanhChinhViewModel>> GetThongTinHanhChinh(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId, o => o.Include(p => p.BenhNhan)
                                                                                                   .Include(p => p.NgheNghiep)
                                                                                                   .Include(p => p.DanToc)
                                                                                                   .Include(p => p.QuocTich)
                                                                                                   .Include(p => p.NguoiLienHeQuanHeNhanThan)
                                                                                                   .Include(p => p.NoiTiepNhan)
                                                                                                   .Include(p => p.NoiGioiThieu)
                                                                                                   .Include(p => p.NoiTruBenhAn)
                                                                                                   .Include(p => p.YeuCauTiepNhanTheBHYTs));
            var thongTinRaVien = yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien != null ? JsonConvert.DeserializeObject<RaVien>(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien) : null;
            var thongTinHanhChinhVM = new DieuTriNoiTruThongTinHanhChinhViewModel
            {
                Id = yeuCauTiepNhan.Id,

                HoTen = yeuCauTiepNhan.HoTen,
                NgaySinh = yeuCauTiepNhan.NgaySinh,
                ThangSinh = yeuCauTiepNhan.ThangSinh,
                NamSinh = yeuCauTiepNhan.NamSinh,
                GioiTinh = yeuCauTiepNhan.GioiTinh,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                QuocTichId = yeuCauTiepNhan.QuocTichId,
                QuocTichDisplay = yeuCauTiepNhan.QuocTich?.QuocTich,
                DanTocId = yeuCauTiepNhan.DanTocId,
                DanTocDisplay = yeuCauTiepNhan.DanToc?.Ten,
                NgheNghiepId = yeuCauTiepNhan.NgheNghiepId,
                NgheNghiepDisplay = yeuCauTiepNhan.NgheNghiep?.Ten,
                NoiLamViec = yeuCauTiepNhan.NoiLamViec,
                NguoiLienHeHoTen = yeuCauTiepNhan.NguoiLienHeHoTen,
                NguoiLienHeQuanHeNhanThanId = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId,
                NguoiLienHeQuanHeNhanThanDisplay = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.Ten,
                SoDienThoai = yeuCauTiepNhan.SoDienThoai,
                SoDienThoaiDisplay = yeuCauTiepNhan.SoDienThoaiDisplay,
                //CoBHYT = yeuCauTiepNhan.CoBHYT == true ? true : yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Count > 0,
                //BHYTMaSoThe = yeuCauTiepNhan.BHYTMaSoThe,
                //BHYTNgayHieuLuc = yeuCauTiepNhan.BHYTNgayHieuLuc,
                //BHYTNgayHetHan = yeuCauTiepNhan.BHYTNgayHetHan,
                //BHYTMucHuong = yeuCauTiepNhan.BHYTMucHuong,
                TuyenKham = yeuCauTiepNhan.LyDoVaoVien == null ? null : yeuCauTiepNhan.LyDoVaoVien.GetDescription(),
                NhomMau = yeuCauTiepNhan.NhomMau,
                YeuToRh = yeuCauTiepNhan.YeuToRh,

                ThoiDiemTiepNhan = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemTaoBenhAn,
                NoiTiepNhanId = yeuCauTiepNhan.NoiTiepNhanId,
                NoiTiepNhanDisplay = yeuCauTiepNhan.NoiTiepNhan?.Ten,
                NoiGioiThieuId = yeuCauTiepNhan.NoiGioiThieuId,
                NoiGioiThieuDisplay = yeuCauTiepNhan.NoiGioiThieu?.Ten,
                ThoiDiemNhapVien = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien,
                ThoiDiemRaVien = thongTinRaVien != null ? thongTinRaVien.ThoiGianRaVien : null,
                SoLanVaoVienDoBenhNay = yeuCauTiepNhan.NoiTruBenhAn.SoLanVaoVienDoBenhNay,
                CoChuyenVien = yeuCauTiepNhan.NoiTruBenhAn.CoChuyenVien,
                LoaiChuyenTuyen = yeuCauTiepNhan.NoiTruBenhAn.LoaiChuyenTuyen,
                ChuyenDenBenhVienId = yeuCauTiepNhan.NoiTruBenhAn.ChuyenDenBenhVienId,
                ChuyenDenBenhVienDisplay = yeuCauTiepNhan.NoiTruBenhAn.ChuyenDenBenhVien?.Ten,
                HinhThucRaVien = yeuCauTiepNhan.NoiTruBenhAn.HinhThucRaVien,
                NgayTaiKham = thongTinRaVien != null ? thongTinRaVien.NgayHienTaiKham : null,
                CoThuThuat = yeuCauTiepNhan.NoiTruBenhAn.CoThuThuat,
                CoPhauThuat = yeuCauTiepNhan.NoiTruBenhAn.CoPhauThuat
            };

            if ((thongTinHanhChinhVM.NgaySinh != null && thongTinHanhChinhVM.NgaySinh.GetValueOrDefault() != 0) && (thongTinHanhChinhVM.ThangSinh != null && thongTinHanhChinhVM.ThangSinh.GetValueOrDefault() != 0) && (thongTinHanhChinhVM.NamSinh != null && thongTinHanhChinhVM.NamSinh.GetValueOrDefault() != 0))
            {
                var ngaySinh = thongTinHanhChinhVM.NgaySinh < 10 ? $"0{thongTinHanhChinhVM.NgaySinh}" : thongTinHanhChinhVM.NgaySinh.ToString();
                var thangSinh = thongTinHanhChinhVM.ThangSinh < 10 ? $"0{thongTinHanhChinhVM.ThangSinh}" : thongTinHanhChinhVM.ThangSinh.ToString();

                thongTinHanhChinhVM.NgaySinhDisplay = $"{ngaySinh}/{thangSinh}/{thongTinHanhChinhVM.NamSinh}";
            }
            else
            {
                //thongTinHanhChinhVM.NgaySinhDisplay = $"01/01/{thongTinHanhChinhVM.NamSinh}";
                thongTinHanhChinhVM.NgaySinhDisplay = $"{thongTinHanhChinhVM.NamSinh}";
            }

            //BVHD-3754
            //if (yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
            //{
            //    var yeuCauTiepNhanTheBHYTMoiNhat = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

            //    thongTinHanhChinhVM.CoBHYT = true;
            //    thongTinHanhChinhVM.BHYTMaSoThe = yeuCauTiepNhanTheBHYTMoiNhat.MaSoThe;
            //    thongTinHanhChinhVM.BHYTNgayHieuLuc = yeuCauTiepNhanTheBHYTMoiNhat.NgayHieuLuc;
            //    thongTinHanhChinhVM.BHYTNgayHetHan = yeuCauTiepNhanTheBHYTMoiNhat.NgayHetHan;
            //    thongTinHanhChinhVM.BHYTMucHuong = yeuCauTiepNhanTheBHYTMoiNhat.MucHuong;
            //}
            //else
            //{
            //    thongTinHanhChinhVM.CoBHYT = yeuCauTiepNhan.CoBHYT;
            //    thongTinHanhChinhVM.BHYTMaSoThe = yeuCauTiepNhan.BHYTMaSoThe;
            //    thongTinHanhChinhVM.BHYTNgayHieuLuc = yeuCauTiepNhan.BHYTNgayHieuLuc;
            //    thongTinHanhChinhVM.BHYTNgayHetHan = yeuCauTiepNhan.BHYTNgayHetHan;
            //    thongTinHanhChinhVM.BHYTMucHuong = yeuCauTiepNhan.BHYTMucHuong;
            //}

            if (yeuCauTiepNhan.CoBHYT == true)
            {
                thongTinHanhChinhVM.CoBHYT = yeuCauTiepNhan.CoBHYT;
                thongTinHanhChinhVM.BHYTMaSoThe = yeuCauTiepNhan.BHYTMaSoThe;
                thongTinHanhChinhVM.BHYTNgayHieuLuc = yeuCauTiepNhan.BHYTNgayHieuLuc;
                thongTinHanhChinhVM.BHYTNgayHetHan = yeuCauTiepNhan.BHYTNgayHetHan;
                thongTinHanhChinhVM.BHYTMucHuong = yeuCauTiepNhan.BHYTMucHuong;
            }

            return thongTinHanhChinhVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListNhomMau")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListNhomMau([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListNhomMau(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListYeuToRh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListYeuToRh([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListYeuToRh(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListLoaiChuyenTuyen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListLoaiChuyenTuyen([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListLoaiChuyenTuyen(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListHinhThucRaVien")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListHinhThucRaVien([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListHinhThucRaVien(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListBenhVien")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListBenhVien([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListBenhVien(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("CapNhatThongTinHanhChinh")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatThongTinHanhChinh([FromBody] DieuTriNoiTruThongTinHanhChinhViewModel dieuTriNoiTruThongTinHanhChinhViewModel)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(dieuTriNoiTruThongTinHanhChinhViewModel.Id, o => o.Include(p => p.BenhNhan)
                                                                                                                             .Include(p => p.NoiTruBenhAn));

            if (yeuCauTiepNhan != null && yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }
            yeuCauTiepNhan.NhomMau = dieuTriNoiTruThongTinHanhChinhViewModel.NhomMau;
            yeuCauTiepNhan.YeuToRh = dieuTriNoiTruThongTinHanhChinhViewModel.YeuToRh;
            yeuCauTiepNhan.NoiTruBenhAn.SoLanVaoVienDoBenhNay = dieuTriNoiTruThongTinHanhChinhViewModel.SoLanVaoVienDoBenhNay;

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }
    }
}
