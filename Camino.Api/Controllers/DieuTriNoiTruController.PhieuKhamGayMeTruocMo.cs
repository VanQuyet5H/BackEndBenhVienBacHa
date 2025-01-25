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
        [HttpPost("UpdatePhieuKhamGayMeTruocMo")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdatePhieuKhamGayMeTruocMo([FromBody] PhieuKhamGayMeTruocMoViewModel phieuKhamGayMeTruocMoViewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            phieuKhamGayMeTruocMoViewModel.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            phieuKhamGayMeTruocMoViewModel.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nhanVienThucHienId);
            var thongTinHoSo = JsonConvert.SerializeObject(phieuKhamGayMeTruocMoViewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (phieuKhamGayMeTruocMoViewModel.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(phieuKhamGayMeTruocMoViewModel.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuKhamGayMeTruocMo;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (phieuKhamGayMeTruocMoViewModel.FileChuKy != null && phieuKhamGayMeTruocMoViewModel.FileChuKy.Any())
                {
                    foreach (var file in phieuKhamGayMeTruocMoViewModel.FileChuKy)
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
                        if (phieuKhamGayMeTruocMoViewModel.FileChuKy != null && phieuKhamGayMeTruocMoViewModel.FileChuKy.All(w => w.TenGuid != noiTruHoSoKhacKemFileEntity.TenGuid))
                        {
                            noiTruHoSoKhacKemFileEntity.WillDelete = true;
                        }
                    }
                }
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                return Ok(phieuKhamGayMeTruocMoViewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuKhamGayMeTruocMo,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            if (phieuKhamGayMeTruocMoViewModel.FileChuKy.Any())
            {
                foreach (var file in phieuKhamGayMeTruocMoViewModel.FileChuKy)
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
            phieuKhamGayMeTruocMoViewModel.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            return Ok(phieuKhamGayMeTruocMoViewModel);
        }

        [HttpGet("GetPhieuKhamGayMeTruocMo")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<PhieuKhamGayMeTruocMoViewModel> GetPhieuKhamGayMeTruocMo(long yeuCauTiepNhanId)
        {
            var bangKiemAnToan = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.PhieuKhamGayMeTruocMo);

            if (bangKiemAnToan != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(bangKiemAnToan.Id);
                var bangKiem = JsonConvert.DeserializeObject<PhieuKhamGayMeTruocMoViewModel>(bangKiemAnToan.ThongTinHoSo);
                bangKiem.IdNoiTruHoSo = bangKiemAnToan.Id;

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (bangKiem.FileChuKy != null && bangKiem.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        bangKiem.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        bangKiem.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                    }
                }

                return bangKiem;
            }

            var bangKiemPhauThuat = new PhieuKhamGayMeTruocMoViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            bangKiemPhauThuat.ChanDoan = await _noiTruHoSoKhacService.GetChanDoan(yeuCauTiepNhanId);
            bangKiemPhauThuat.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            bangKiemPhauThuat.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            return bangKiemPhauThuat;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInPhieuKhamGayMeTruocMo")]
        public async Task<ActionResult> PhieuInPhieuKhamGayMeTruocMo(PhieuDieuTriVaServicesHttpParams phieuKhamGayMeTruocMoHttpParams)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuInPhieuKhamGayMeTruocMo(phieuKhamGayMeTruocMoHttpParams);
            return Ok(phieuIns);
        }
    }
}
