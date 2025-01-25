using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("UpdateBienBanCamKetPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateBienBanCamKetPhauThuat([FromBody] BienBanCamKetPhauThuatViewModel bienBanCamKetPhauThuat, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            bienBanCamKetPhauThuat.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            bienBanCamKetPhauThuat.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nhanVienThucHienId);
            var thongTinHoSo = JsonConvert.SerializeObject(bienBanCamKetPhauThuat);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (bienBanCamKetPhauThuat.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(bienBanCamKetPhauThuat.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetPhauThuat;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (bienBanCamKetPhauThuat.FileChuKy != null && bienBanCamKetPhauThuat.FileChuKy.Any())
                {
                    foreach (var file in bienBanCamKetPhauThuat.FileChuKy)
                    {
                        if (noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems.All(w => w.TenGuid != file.TenGuid))
                        {
                            var noiTruHoSoKhacFileChuKy = new NoiTruHoSoKhacFileDinhKem
                            {
                                Id = 0,
                                Ten = file.Ten,
                                DuongDan = file.DuongDan,
                                Ma = file.Uid,
                                KichThuoc = file.KichThuoc,
                                TenGuid = file.TenGuid,
                                NoiTruHoSoKhacId = noiTruHoSoKhac.Id,
                                LoaiTapTin = file.LoaiTapTin.GetValueOrDefault()
                            };
                            noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileChuKy);
                            await _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileChuKy.DuongDan, noiTruHoSoKhacFileChuKy.TenGuid);
                        }
                    }
                }
                if (noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems != null && noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems.Any())
                {
                    foreach (var noiTruHoSoKhacKemFileEntity in noiTruHoSoKhac.NoiTruHoSoKhacFileDinhKems)
                    {
                        if (bienBanCamKetPhauThuat.FileChuKy != null && bienBanCamKetPhauThuat.FileChuKy.All(w => w.TenGuid != noiTruHoSoKhacKemFileEntity.TenGuid))
                        {
                            noiTruHoSoKhacKemFileEntity.WillDelete = true;
                        }
                    }
                }
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                bienBanCamKetPhauThuat.CheckCreateOrUpdate = true; // update
                return Ok(bienBanCamKetPhauThuat);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetPhauThuat,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            if (bienBanCamKetPhauThuat.FileChuKy.Any())
            {
                foreach (var file in bienBanCamKetPhauThuat.FileChuKy)
                {
                    var noiTruHoSoKhacFileChuKy = new NoiTruHoSoKhacFileDinhKem
                    {
                        Id = 0,
                        Ma = file.Uid,
                        Ten = file.Ten,
                        DuongDan = file.DuongDan,
                        KichThuoc = file.KichThuoc,
                        TenGuid = file.TenGuid,
                        NoiTruHoSoKhacId = noiTruHoSoKhacNew.Id,
                        LoaiTapTin = file.LoaiTapTin.GetValueOrDefault()
                    };
                    noiTruHoSoKhacNew.NoiTruHoSoKhacFileDinhKems.Add(noiTruHoSoKhacFileChuKy);
                    await _taiLieuDinhKemService.LuuTaiLieuAsync(noiTruHoSoKhacFileChuKy.DuongDan, noiTruHoSoKhacFileChuKy.TenGuid);
                }
            }

            await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
            await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhacNew);
            bienBanCamKetPhauThuat.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            bienBanCamKetPhauThuat.CheckCreateOrUpdate = false; // tạo mới
            return Ok(bienBanCamKetPhauThuat);
        }

        [HttpGet("GetBienBanCamKetPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<BienBanCamKetPhauThuatViewModel> GetBienBanCamKetPhauThuat(long yeuCauTiepNhanId)
        {
            var bienBanHoiChan = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetPhauThuat);

            if (bienBanHoiChan != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(bienBanHoiChan.Id);
                var bienBan = JsonConvert.DeserializeObject<BienBanCamKetPhauThuatViewModel>(bienBanHoiChan.ThongTinHoSo);
                bienBan.IdNoiTruHoSo = bienBanHoiChan.Id;

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

                return bienBan;
            }

            var bienBanCamKet = new BienBanCamKetPhauThuatViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            bienBanCamKet.ChanDoan = await _noiTruHoSoKhacService.GetChanDoan(yeuCauTiepNhanId);
            bienBanCamKet.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            bienBanCamKet.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            return bienBanCamKet;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInBienBanCamKetPhauThuat")]
        public async Task<ActionResult> PhieuInBienBanCamKetPhauThuat(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuInBienBanCamKetPhauThuat(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }
    }
}
