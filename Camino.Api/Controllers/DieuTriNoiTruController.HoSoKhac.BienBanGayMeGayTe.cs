using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("ThemHoacCapNhatGayMeGayTe")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemHoacCapNhatGayMeGayTe(HoSoKhacGayMeGayTeViewModel hoSoKhacViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(hoSoKhacViewModel.YeuCauTiepNhanId);
            if (hoSoKhacViewModel.Id == 0) // Them
            {
                hoSoKhacViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetGayMeGayTe;
                foreach (var item in hoSoKhacViewModel.NoiTruHoSoKhacFileDinhKems)
                {
                    item.Ma = Guid.NewGuid().ToString();
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                }
                var noiTruHoSoKhac = hoSoKhacViewModel.ToEntity<NoiTruHoSoKhac>();
                await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhac);
                var thongTinHoSo = new HoSoKhacGayMeGayTeViewModel();
                if (!string.IsNullOrEmpty(noiTruHoSoKhac.ThongTinHoSo))
                {
                    thongTinHoSo = JsonConvert.DeserializeObject<HoSoKhacGayMeGayTeViewModel>(noiTruHoSoKhac.ThongTinHoSo);
                    if (thongTinHoSo != null && thongTinHoSo.ThongTinQuanHeThanNhans.Any())
                    {
                        foreach (var item in thongTinHoSo.ThongTinQuanHeThanNhans)
                        {
                            item.TenQuanHeThanNhan = _dieuTriNoiTruService.GetTenQuanHeThanNhan(item.QuanHeThanNhanId);
                        }
                    }
                }
                var resul = new
                {
                    noiTruHoSoKhac.Id,
                    noiTruHoSoKhac.LastModified,
                    thongTinHoSo?.ThongTinQuanHeThanNhans
                };
                return Ok(resul);
            }
            else // Update
            {
                var noiTruHoSoKhac = _noiTruHoSoKhacService.GetById(hoSoKhacViewModel.Id, s => s.Include(p => p.NoiTruHoSoKhacFileDinhKems));
                hoSoKhacViewModel.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetGayMeGayTe;
                foreach (var item in hoSoKhacViewModel.NoiTruHoSoKhacFileDinhKems)
                {
                    item.Ma = Guid.NewGuid().ToString();
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                }
                hoSoKhacViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                hoSoKhacViewModel.ToEntity(noiTruHoSoKhac);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                var thongTinHoSo = new HoSoKhacGayMeGayTeViewModel();
                if (!string.IsNullOrEmpty(noiTruHoSoKhac.ThongTinHoSo))
                {
                    thongTinHoSo = JsonConvert.DeserializeObject<HoSoKhacGayMeGayTeViewModel>(noiTruHoSoKhac.ThongTinHoSo);
                    if (thongTinHoSo != null && thongTinHoSo.ThongTinQuanHeThanNhans.Any())
                    {
                        foreach (var item in thongTinHoSo.ThongTinQuanHeThanNhans)
                        {
                            item.TenQuanHeThanNhan = _dieuTriNoiTruService.GetTenQuanHeThanNhan(item.QuanHeThanNhanId);
                        }
                    }
                }
                var resul = new
                {
                    noiTruHoSoKhac.Id,
                    noiTruHoSoKhac.LastModified,
                    thongTinHoSo?.ThongTinQuanHeThanNhans
                };
                return Ok(resul);
            }
        }

        [HttpGet("GetHoSoKhacGayMe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacGayMeGayTeViewModel>> GetHoSoKhacGayMe(long id)
        {
            var ycTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(id, s => s.Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                                                 .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NoiThucHien)
                                                                                 .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));
            var hoSoGayMe = ycTiepNhan.NoiTruHoSoKhacs.FirstOrDefault(p => p.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetGayMeGayTe);
            if (hoSoGayMe == null)
            {
                return NoContent();
            }

            var model = hoSoGayMe.ToModel<HoSoKhacGayMeGayTeViewModel>();
            var thongTinHoSo = JsonConvert.DeserializeObject<HoSoKhacGayMeGayTeViewModel>(hoSoGayMe.ThongTinHoSo);
            model.TenNhanVienGiaiThich = _dieuTriNoiTruService.GetTenNhanVienGiaiThich(thongTinHoSo.NhanVienGiaiThichId.GetValueOrDefault());
            model.NhanVienGiaiThichId = thongTinHoSo.NhanVienGiaiThichId;
            model.ThongTinQuanHeThanNhans = thongTinHoSo.ThongTinQuanHeThanNhans;
            model.ThoiDiemThucHienDisplay = model.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH();
            foreach (var item in model.ThongTinQuanHeThanNhans)
            {
                item.TenQuanHeThanNhan = _dieuTriNoiTruService.GetTenQuanHeThanNhan(item.QuanHeThanNhanId);
            }
            return Ok(model);
        }

        [HttpPost("InBienBanGayMeGayTe")]
        public ActionResult InBienBanGayMeGayTe(long noiTruHoSoKhacId)
        {
            var result = _dieuTriNoiTruService.InBienBanGayMeGayTe(noiTruHoSoKhacId);
            return Ok(result);
        }

        [HttpPost("ThongTinNhanVienDangNhap")]
        public async Task<ActionResult> ThongTinNhanVienDangNhap()
        {
            var result = await _dieuTriNoiTruService.ThongTinNhanVienDangNhap();
            return Ok(result);
        }
        [HttpPost("CheckThongTinBenhAnDaKetThuc")]
        public async Task<ActionResult> CheckThongTinBenhAnDaKetThuc(long id)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(id);
            return NoContent();
        }
    }
}
