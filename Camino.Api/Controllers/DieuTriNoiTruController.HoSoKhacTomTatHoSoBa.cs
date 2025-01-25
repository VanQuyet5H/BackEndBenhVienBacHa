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
        [HttpGet("GetTomTatHoSoBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<TomTatHoSoBenhAnViewModel> GetTomTatHoSoBenhAn(long yeuCauTiepNhanId)
        {
            var hoSoBenhAn = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn);

            if (hoSoBenhAn != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(hoSoBenhAn.Id);
                var hoSoBaViewModel = JsonConvert.DeserializeObject<TomTatHoSoBenhAnViewModel>(hoSoBenhAn.ThongTinHoSo);
                hoSoBaViewModel.IdNoiTruHoSo = hoSoBenhAn.Id;
                hoSoBaViewModel.IsCreated = false;
                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (hoSoBaViewModel.FileChuKy != null && hoSoBaViewModel.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        hoSoBaViewModel.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        hoSoBaViewModel.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                    }
                }

                return hoSoBaViewModel;
            }

            var newHoSoBa = new TomTatHoSoBenhAnViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            newHoSoBa.IsCreated = true;
            newHoSoBa.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            newHoSoBa.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            return newHoSoBa;
        }

        [HttpPost("UpdateTomTatHoSoBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateTomTatHoSoBenhAn([FromBody] TomTatHoSoBenhAnViewModel tomTatHoSoBenhAnViewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            tomTatHoSoBenhAnViewModel.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            tomTatHoSoBenhAnViewModel.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nhanVienThucHienId);
            var thongTinHoSo = JsonConvert.SerializeObject(tomTatHoSoBenhAnViewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (tomTatHoSoBenhAnViewModel.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(tomTatHoSoBenhAnViewModel.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (tomTatHoSoBenhAnViewModel.FileChuKy != null && tomTatHoSoBenhAnViewModel.FileChuKy.Any())
                {
                    foreach (var file in tomTatHoSoBenhAnViewModel.FileChuKy)
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
                        if (tomTatHoSoBenhAnViewModel.FileChuKy != null && tomTatHoSoBenhAnViewModel.FileChuKy.All(w => w.TenGuid != noiTruHoSoKhacKemFileEntity.TenGuid))
                        {
                            noiTruHoSoKhacKemFileEntity.WillDelete = true;
                        }
                    }
                }
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                return Ok(tomTatHoSoBenhAnViewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            if (tomTatHoSoBenhAnViewModel.FileChuKy.Any())
            {
                foreach (var file in tomTatHoSoBenhAnViewModel.FileChuKy)
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
            tomTatHoSoBenhAnViewModel.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            return Ok(tomTatHoSoBenhAnViewModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInTomTatHoSoBenhAn")]
        public async Task<ActionResult> PhieuInTomTatHoSoBenhAn(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIn = await _noiTruHoSoKhacService.PhieuInTomTatHoSoBenhAn(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIn);
        }
    }
}
