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
        [HttpGet("GetGiayKhamChuaBenhTheoYc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<GiayKhamChuaBenhTheoYeuCauViewModel> GetGiayKhamChuaBenhTheoYc(long yeuCauTiepNhanId)
        {
            var giayKhamChuaBenhTheoYcEntity = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.GiayKhamChuaBenhTheoYc);

            if (giayKhamChuaBenhTheoYcEntity != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(giayKhamChuaBenhTheoYcEntity.Id);
                var giayCamKet = JsonConvert.DeserializeObject<GiayKhamChuaBenhTheoYeuCauViewModel>(giayKhamChuaBenhTheoYcEntity.ThongTinHoSo);
                giayCamKet.IdNoiTruHoSo = giayKhamChuaBenhTheoYcEntity.Id;

                foreach (var fileChuKyEntity in listNoiTruHoSoKhacFileDinhKem)
                {
                    if (giayCamKet.FileChuKy != null && giayCamKet.FileChuKy.Any(w => w.TenGuid == fileChuKyEntity.TenGuid))
                    {
                        giayCamKet.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Id =
                            fileChuKyEntity.Id;
                        giayCamKet.FileChuKy.First(w => w.TenGuid == fileChuKyEntity.TenGuid).Uid =
                            fileChuKyEntity.Uid;
                    }
                }
                if(giayKhamChuaBenhTheoYcEntity.Id != null)
                {
                    giayCamKet.NoiTruHoSoKhacId = giayKhamChuaBenhTheoYcEntity.Id;
                }
                return giayCamKet;
            }

            var giayKhamChuaBenhTheoYcMoi = new GiayKhamChuaBenhTheoYeuCauViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            giayKhamChuaBenhTheoYcMoi.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            giayKhamChuaBenhTheoYcMoi.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            return giayKhamChuaBenhTheoYcMoi;
        }

        [HttpPost("UpdateGiayKhamChuaBenhTheoYc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateGiayKhamChuaBenhTheoYc([FromBody] GiayKhamChuaBenhTheoYeuCauViewModel giayKhamChuaBenhTheoYcViewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            giayKhamChuaBenhTheoYcViewModel.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            giayKhamChuaBenhTheoYcViewModel.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nhanVienThucHienId);
            var thongTinHoSo = JsonConvert.SerializeObject(giayKhamChuaBenhTheoYcViewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (giayKhamChuaBenhTheoYcViewModel.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(giayKhamChuaBenhTheoYcViewModel.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayKhamChuaBenhTheoYc;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (giayKhamChuaBenhTheoYcViewModel.FileChuKy != null && giayKhamChuaBenhTheoYcViewModel.FileChuKy.Any())
                {
                    foreach (var file in giayKhamChuaBenhTheoYcViewModel.FileChuKy)
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
                        if (giayKhamChuaBenhTheoYcViewModel.FileChuKy != null && giayKhamChuaBenhTheoYcViewModel.FileChuKy.All(w => w.TenGuid != noiTruHoSoKhacKemFileEntity.TenGuid))
                        {
                            noiTruHoSoKhacKemFileEntity.WillDelete = true;
                        }
                    }
                }
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                giayKhamChuaBenhTheoYcViewModel.IsSaveOrUpdate = true; // true là update
                return Ok(giayKhamChuaBenhTheoYcViewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayKhamChuaBenhTheoYc,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            if (giayKhamChuaBenhTheoYcViewModel.FileChuKy.Any())
            {
                foreach (var file in giayKhamChuaBenhTheoYcViewModel.FileChuKy)
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
            giayKhamChuaBenhTheoYcViewModel.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            giayKhamChuaBenhTheoYcViewModel.IsSaveOrUpdate = false; // false là create
            return Ok(giayKhamChuaBenhTheoYcViewModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInGiayKhamChuaBenhTheoYc")]
        public async Task<ActionResult> PhieuInGiayKhamChuaBenhTheoYc(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuInGiayKhamChuaBenhTheoYc(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }
    }
}
