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
        [HttpGet("GetGiayCamKetKyThuatMoi")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<GiayCamKetKyThuatMoiViewModel> GetGiayCamKetKyThuatMoi(long yeuCauTiepNhanId)
        {
            var giayCamKetKyThuatMoiEntity = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetKyThuatMoi);

            if (giayCamKetKyThuatMoiEntity != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(giayCamKetKyThuatMoiEntity.Id);
                var giayCamKet = JsonConvert.DeserializeObject<GiayCamKetKyThuatMoiViewModel>(giayCamKetKyThuatMoiEntity.ThongTinHoSo);
                giayCamKet.IdNoiTruHoSo = giayCamKetKyThuatMoiEntity.Id;

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

                return giayCamKet;
            }

            var giayCamKetKtMoi = new GiayCamKetKyThuatMoiViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            giayCamKetKtMoi.ChanDoan = await _noiTruHoSoKhacService.GetChanDoan(yeuCauTiepNhanId);
            giayCamKetKtMoi.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            giayCamKetKtMoi.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nguoiDangLogin);
            return giayCamKetKtMoi;
        }

        [HttpPost("UpdateGiayCamKetKyThuatMoi")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateGiayCamKetKyThuatMoi([FromBody] GiayCamKetKyThuatMoiViewModel giayCamKetKyThuatMoiViewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            giayCamKetKyThuatMoiViewModel.NgayThucHienReadonly = DateTime.Now.ApplyFormatDateTime();
            giayCamKetKyThuatMoiViewModel.NguoiThucHienReadonly = await _noiTruHoSoKhacService.GetNguoiThucHien(nhanVienThucHienId);
            var thongTinHoSo = JsonConvert.SerializeObject(giayCamKetKyThuatMoiViewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (giayCamKetKyThuatMoiViewModel.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(giayCamKetKyThuatMoiViewModel.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetKyThuatMoi;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (giayCamKetKyThuatMoiViewModel.FileChuKy != null && giayCamKetKyThuatMoiViewModel.FileChuKy.Any())
                {
                    foreach (var file in giayCamKetKyThuatMoiViewModel.FileChuKy)
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
                        if (giayCamKetKyThuatMoiViewModel.FileChuKy != null && giayCamKetKyThuatMoiViewModel.FileChuKy.All(w => w.TenGuid != noiTruHoSoKhacKemFileEntity.TenGuid))
                        {
                            noiTruHoSoKhacKemFileEntity.WillDelete = true;
                        }
                    }
                }
                await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                giayCamKetKyThuatMoiViewModel.CheckCreateOrCapNhat = true; // true là cập nhat
                return Ok(giayCamKetKyThuatMoiViewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetKyThuatMoi,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            if (giayCamKetKyThuatMoiViewModel.FileChuKy.Any())
            {
                foreach (var file in giayCamKetKyThuatMoiViewModel.FileChuKy)
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
            giayCamKetKyThuatMoiViewModel.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            giayCamKetKyThuatMoiViewModel.CheckCreateOrCapNhat = false; // false là tao moires
            return Ok(giayCamKetKyThuatMoiViewModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInGiayCamKetKyThuatMoi")]
        public async Task<ActionResult> PhieuInGiayCamKetKyThuatMoi(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuInGiayCamKetKyThuatMoi(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }
    }
}
