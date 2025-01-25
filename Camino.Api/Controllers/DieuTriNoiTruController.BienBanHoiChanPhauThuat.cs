using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        #region biên bản hội chẩn pt
        [HttpPost("UpdateVaCreateBienBanHoiChanPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateVaCreateBienBanHoiChanPhauThuat([FromBody] BienBanHoiChanPhauThuatModel bienBanHoiChanPhauThuat)
        {
            if (bienBanHoiChanPhauThuat.Id == 0)
            {
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                var trichBienBanNoiTru = new BienBanHoiChanPhauThuatModel()
                {
                    YeuCauTiepNhanId = bienBanHoiChanPhauThuat.YeuCauTiepNhanId,
                    LoaiHoSoDieuTriNoiTru = bienBanHoiChanPhauThuat.LoaiHoSoDieuTriNoiTru,
                    ThongTinHoSo = bienBanHoiChanPhauThuat.ThongTinHoSo,
                    NhanVienThucHienId = nguoiDangLogin,
                    NoiThucHienId = noiThucHien,
                    ThoiDiemThucHien = DateTime.Now
                };
                var bbhc = trichBienBanNoiTru.ToEntity<NoiTruHoSoKhac>();
                if (bienBanHoiChanPhauThuat.FileChuKy.Any())
                {
                    foreach (var itemfileChuKy in bienBanHoiChanPhauThuat.FileChuKy)
                    {
                        var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                        {
                            //NoiTruHoSoKhacId = user.Id,
                            Ma = itemfileChuKy.Uid,
                            Ten = itemfileChuKy.Ten,
                            TenGuid = itemfileChuKy.TenGuid,
                            DuongDan = itemfileChuKy.DuongDan,
                            LoaiTapTin = itemfileChuKy.LoaiTapTin,
                            MoTa = itemfileChuKy.MoTa,
                            KichThuoc = itemfileChuKy.KichThuoc
                        };
                        _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                        bbhc.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                    }
                }
                await _noiDuTruHoSoKhacService.AddAsync(bbhc);
                return CreatedAtAction(nameof(Get), new { id = bbhc.Id }, bbhc.ToModel<BienBanHoiChanPhauThuatModel>());
            }
            else
            {
                var update = await _noiDuTruHoSoKhacService.GetByIdAsync(bienBanHoiChanPhauThuat.Id);
                var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
                var noiThucHien = _userAgentHelper.GetCurrentNoiLLamViecId();
                update.YeuCauTiepNhanId = bienBanHoiChanPhauThuat.YeuCauTiepNhanId;
                update.LoaiHoSoDieuTriNoiTru = bienBanHoiChanPhauThuat.LoaiHoSoDieuTriNoiTru;
                update.ThongTinHoSo = bienBanHoiChanPhauThuat.ThongTinHoSo;
                update.NhanVienThucHienId = nguoiDangLogin;
                update.NoiThucHienId = noiThucHien;
                update.ThoiDiemThucHien = DateTime.Now;
                if (update == null)
                {
                    return NotFound();
                }
                bienBanHoiChanPhauThuat.ToEntity<NoiTruHoSoKhac>();
                // remove list fileChuKy hiện tại
                foreach (var itemNoiTruHoSoKhacFileDinhKem in update.NoiTruHoSoKhacFileDinhKems.ToList())
                {
                    var soket = await _noiDuTruHoSoKhacFileDinhKemService.GetByIdAsync(itemNoiTruHoSoKhacFileDinhKem.Id);
                    if (soket == null)
                    {
                        return NotFound();
                    }
                    await _noiDuTruHoSoKhacFileDinhKemService.DeleteByIdAsync(soket.Id);
                }
                if (bienBanHoiChanPhauThuat.Id != 0)
                {
                    if (bienBanHoiChanPhauThuat.FileChuKy.Any())
                    {
                        foreach (var itemfileChuKy in bienBanHoiChanPhauThuat.FileChuKy)
                        {
                            var noiTruHoSoKhacFileDinhKem = new NoiTruHoSoKhacFileDinhKem()
                            {
                                Ma = itemfileChuKy.Uid,
                                Ten = itemfileChuKy.Ten,
                                TenGuid = itemfileChuKy.TenGuid,
                                DuongDan = itemfileChuKy.DuongDan,
                                LoaiTapTin = itemfileChuKy.LoaiTapTin,
                                MoTa = itemfileChuKy.MoTa,
                                KichThuoc = itemfileChuKy.KichThuoc
                            };
                            _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileDinhKem.DuongDan, noiTruHoSoKhacFileDinhKem.TenGuid);
                            update.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileDinhKem);
                        }
                    }

                }
                await _noiDuTruHoSoKhacService.UpdateAsync(update);
                return Ok(update.Id);
            }
            return null;
        }

        
        [HttpPost("GetThongTinPhieuBienBanHoiChanPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public PhieuBienBanHoiChanPhauThuatGridVo GetThongTinPhieuBienBanHoiChanPhauThuat(long yeuCauTiepNhanId)
        {
            var lookup = _noiTruHoSoKhacService.GetThongTinPhieuBienBanHoiChanPhauThuat(yeuCauTiepNhanId);
            return lookup;
        }

        [HttpPost("ViewNoiTruHoSoKhac")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public PhieuBienBanHoiChanPhauThuatGridVo ViewNoiTruHoSoKhac(long noiTruHoSoKhacId)
        {
            var lookup = _noiTruHoSoKhacService.ViewNoiTruHoSoKhac(noiTruHoSoKhacId);
            return lookup;
        }

        [HttpPost("GetDanhSachBienBanHoiChanPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public List<DanhSachBienBanHoiChanPhauThuat> GetDanhSachBienBanHoiChanPhauThuat(long yeuCauTiepNhanId)
        {
            var bienBanHoiChan =  _noiTruHoSoKhacService.DanhSachBienBanHoiPhauThuat(yeuCauTiepNhanId);
            return bienBanHoiChan;
        }
        #endregion

        [HttpGet("GetBienBanHoiChanPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<BienBanHoiChanPhauThuatViewModel> GetBienBanHoiChanPhauThuat(long yeuCauTiepNhanId)
        {
            var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat);

            if (noiTruHoSoKhac != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(noiTruHoSoKhac.Id);
                var bienBan = JsonConvert.DeserializeObject<BienBanHoiChanPhauThuatViewModel>(noiTruHoSoKhac.ThongTinHoSo);
                bienBan.IdNoiTruHoSo = noiTruHoSoKhac.Id;
                var noiTruHoSoConLai = await _noiTruHoSoKhacService.GetListNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat);

                bienBan.ListPhieu = new List<PhieuHoiChanViewModel>();

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (bienBan.FileChuKy != null && bienBan.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        bienBan.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        bienBan.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                        bienBan.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).DuongDan = fileChuKyEntity.DuongDanTmp;
                    }
                }

                foreach (var noiTruHoSo in noiTruHoSoConLai)
                {
                    var bienBanTruHoSo = JsonConvert.DeserializeObject<BienBanHoiChanPhauThuatViewModel>(noiTruHoSo.ThongTinHoSo);

                    var phieuHoiChanViewModel = new PhieuHoiChanViewModel
                    {
                        Id = noiTruHoSo.Id,
                        NgayHoiChan = noiTruHoSo.NgayHoiChan,
                        ChanDoan = bienBanTruHoSo.ChanDoan,
                        ThanhVienThamGia = bienBanTruHoSo.ThanhVienThamGia
                    };
                    bienBan.ListPhieu.Add(phieuHoiChanViewModel);
                }

                bienBan.ThongTinHoSo = noiTruHoSoKhac.ThongTinHoSo;

                return bienBan;
            }

            var bienBanHoiChanPhauThuat = new BienBanHoiChanPhauThuatViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            bienBanHoiChanPhauThuat.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            bienBanHoiChanPhauThuat.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            return bienBanHoiChanPhauThuat;
        }

        [HttpGet("GetBienBanHc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<BienBanHoiChanPhauThuatViewModel> GetBienBanHc(long phieuId, long yctnId)
        {
            var bienBanHoiChan = await _noiTruHoSoKhacService.GetByIdAsync(phieuId);

            if (bienBanHoiChan != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(bienBanHoiChan.Id);
                var bienBan = JsonConvert.DeserializeObject<BienBanHoiChanPhauThuatViewModel>(bienBanHoiChan.ThongTinHoSo);
                bienBan.IdNoiTruHoSo = bienBanHoiChan.Id;
                var noiTruHoSoConLai = await _noiTruHoSoKhacService.GetListNoiTruHoSoKhac(yctnId, Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat);

                bienBan.ListPhieu = new List<PhieuHoiChanViewModel>();

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (bienBan.FileChuKy != null && bienBan.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        bienBan.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        bienBan.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                        bienBan.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).DuongDan = fileChuKyEntity.DuongDanTmp;
                    }
                }

                foreach (var noiTruHoSo in noiTruHoSoConLai)
                {
                    var bienBanTruHoSo = JsonConvert.DeserializeObject<BienBanHoiChanPhauThuatViewModel>(noiTruHoSo.ThongTinHoSo);

                    var phieuHoiChanViewModel = new PhieuHoiChanViewModel
                    {
                        Id = noiTruHoSo.Id,
                        NgayHoiChan = noiTruHoSo.NgayHoiChan,
                        ChanDoan = bienBanTruHoSo.ChanDoan,
                        ThanhVienThamGia = bienBanTruHoSo.ThanhVienThamGia
                    };
                    bienBan.ListPhieu.Add(phieuHoiChanViewModel);
                }

                bienBan.ThongTinHoSo = bienBanHoiChan.ThongTinHoSo;

                return bienBan;
            }

            return new BienBanHoiChanPhauThuatViewModel();
        }

        [HttpPost("XoaBienBanHoiChanPhauThuat")]
        public async Task<ActionResult> XoaBienBanHoiChanPhauThuat(long id)
        {
            var bbhcpt = await _noiDuTruHoSoKhacService.GetByIdAsync(id, s => s.Include(k => k.NoiTruHoSoKhacFileDinhKems));
            if (bbhcpt == null)
            {
                return NotFound();
            }
            await _noiDuTruHoSoKhacService.DeleteByIdAsync(id);
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInBienBanHoiChanPhauThuat")]
        public async Task<ActionResult> PhieuInBienBanHoiChanPhauThuat(BangTheoDoiHoiTinhHttpParamsRequest phieuInBienBanHoiChan)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuInBienBanHoiChanPhauThuat(phieuInBienBanHoiChan);
            return Ok(phieuIns);
        }
        [HttpPost("GetListThongTinThanhVienThamGia")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListThongTinThanhVienThamGia([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _noiTruHoSoKhacService.GetListThongTinThanhVienThamGia(model);
            return Ok(lookup);
        }

    }
}
