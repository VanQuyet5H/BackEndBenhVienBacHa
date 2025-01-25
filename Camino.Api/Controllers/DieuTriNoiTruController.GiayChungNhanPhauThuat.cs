using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.GiayChungNhanPhauThuat;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.TinhTrangRaVienHoSoKhacs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpGet("GetThongTinGiayChungNhanPhauThuat")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<InfoGiayChungNhanPhauThuatVo> GetThongTinGiayChungNhanPhauThuat(long yeuCauTiepNhanId)
        {
            var lookup = await _dieuTriNoiTruService.GetThongTinGiayChungNhanPhauThuat(yeuCauTiepNhanId);
            return lookup;
        }
       
        [HttpPost("LuuGiayChungNhanPhauThuat")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GiayChungNhanPhauThuatViewModel>> LuuGiayChungNhanPhauThuat([FromBody] GiayChungNhanPhauThuatViewModel giayChungNhanPhauThuatViewModel)
        {
            if (giayChungNhanPhauThuatViewModel.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new GiayChungNhanPhauThuatViewModel()
                {
                    YeuCauTiepNhanId = giayChungNhanPhauThuatViewModel.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = giayChungNhanPhauThuatViewModel.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = giayChungNhanPhauThuatViewModel.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var user = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                await _noiDuTruHoSoKhacService.AddAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<GiayChungNhanPhauThuatViewModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(giayChungNhanPhauThuatViewModel.Id);
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = giayChungNhanPhauThuatViewModel.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = giayChungNhanPhauThuatViewModel.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = giayChungNhanPhauThuatViewModel.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                giayChungNhanPhauThuatViewModel.ToEntity<NoiTruHoSoKhac>();

                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }
        [HttpPost("GetListDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan([FromBody]DropDownListRequestModel queryInfo,long yctnId)
        {
            var lookup = await _dieuTriNoiTruService.GetListDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan(queryInfo, yctnId);
            return Ok(lookup);
        }
        [HttpGet("GetInfoDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan")]
        public async Task<ActionResult<ICollection<InfoYeuCauDichVuKyThuatTheoBenNhanVo>>> GetInfoDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan( long ycdvktId)
        {
            var lookup = await _dieuTriNoiTruService.GetInfoDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan(ycdvktId);
            return Ok(lookup);
        }
        [HttpPost("GetListTinhTrangRaVienHoSoKhacs")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListTinhTrangRaVienHoSoKhacs([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dieuTriNoiTruService.GetListTinhTrangRaVienHoSoKhac(queryInfo);
            return Ok(lookup);
        }
        //[HttpPost("AddTinhTrangRaVien")]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        //public async Task<ActionResult> AddTinhTrangRaVien(TinhTrangRaVienHoSoKhacViewModel tinhTrang)
        //{
        //    if (!string.IsNullOrEmpty(tinhTrang.TinhTrang))
        //    {
        //        // kiểm tra table TinhTrangRaVienText  đã tồn tại chưa
        //        var kiemTraTonTai = _dieuTriNoiTruService.KiemTraTinhTrangExit(tinhTrang.TinhTrang);
        //        if (kiemTraTonTai == true)
        //        {
        //            var newTinhTrang = new TinhTrangRaVienHoSoKhac()
        //            {
        //                TenTinhTrangRaVien = tinhTrang.TinhTrang
        //            };
        //          var result = await _giayChungNhanPhauThuatService.AddAsync(newTinhTrang);
        //        }
        //    }
        //    return Ok();
        [HttpPost("AddTinhTrangRaVien")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<TinhTrangRaVienHoSoKhacViewModel>> AddTinhTrangRaVien([FromBody] TinhTrangRaVienHoSoKhacViewModel tinhTrang)
        {
            if (!string.IsNullOrEmpty(tinhTrang.TenTinhTrangRaVien))
            {
                // kiểm tra table TinhTrangRaVienText  đã tồn tại chưa
                var kiemTraTonTai = _dieuTriNoiTruService.KiemTraTinhTrangExit(tinhTrang.TenTinhTrangRaVien);
                if (kiemTraTonTai == true)
                {
                    var user = tinhTrang.ToEntity<TinhTrangRaVienHoSoKhac>();
                    _tinhTrangRaVienHoSoKhacService.Add(user);
                    return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToModel<TinhTrangRaVienHoSoKhacViewModel>());
                }
            }
            return Ok();
        }
        [HttpPost("InGiayChungNhanPhauThuat")]
        public async Task<ActionResult<string>> InGiayChungNhanNghiDuongThai([FromBody]GiayChungNhanPhauThuatQueryInfo xacNhanIn)
        {
            //XacNhanInPhieuGiayChungNhanNghiDuongThai
            var html = await _dieuTriNoiTruService.InGiayChungNhanPhauThuat(xacNhanIn.YeuCauTiepNhanId);
            return html;
        }
    }
}
