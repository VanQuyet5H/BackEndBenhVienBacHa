using Camino.Api.Auth;
using Camino.Api.Models.GiayCamKetSuDungThuocNgoaiBH;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
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

        [HttpGet("GetGiayCamKetSuDungThuocNgoaiBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<GiayCamKetSuDungThuocNgoaiBHViewModel> GetGiayCamKetSuDungThuocNgoaiBHYT(long yeuCauTiepNhanId)
        {
            var giayCamKetEntity = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetSuDungThuocNgoaiBHYT);

            if (giayCamKetEntity != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(giayCamKetEntity.Id);
                var giayCamKet = JsonConvert.DeserializeObject<GiayCamKetSuDungThuocNgoaiBHViewModel>(giayCamKetEntity.ThongTinHoSo);
                giayCamKet.IdNoiTruHoSo = giayCamKetEntity.Id;
                giayCamKet.YeuCauTiepNhanId = yeuCauTiepNhanId;

                return giayCamKet;
            }

            var giayCamKetKtMoi = new GiayCamKetSuDungThuocNgoaiBHViewModel();
            giayCamKetKtMoi.YeuCauTiepNhanId = yeuCauTiepNhanId;
            giayCamKetKtMoi.ChanDoan =  _dieuTriNoiTruService.GetChanDoanNhapVienGiayCamKetSuDungNgoaiBHYT(yeuCauTiepNhanId);
           
            return giayCamKetKtMoi;
        }

        [HttpPost("UpdateGiayCamKetSuDungThuocNgoaiBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateGiayCamKetSuDungThuocNgoaiBHYT([FromBody] GiayCamKetSuDungThuocNgoaiBHViewModel giayCamKetSuDungThuocNgoaiBHViewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var thongTinHoSo = JsonConvert.SerializeObject(giayCamKetSuDungThuocNgoaiBHViewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (giayCamKetSuDungThuocNgoaiBHViewModel.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(giayCamKetSuDungThuocNgoaiBHViewModel.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetSuDungThuocNgoaiBHYT;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                giayCamKetSuDungThuocNgoaiBHViewModel.CheckCreateOrCapNhat = true; // true là cập nhat
                return Ok(giayCamKetSuDungThuocNgoaiBHViewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetSuDungThuocNgoaiBHYT,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhacNew);
            giayCamKetSuDungThuocNgoaiBHViewModel.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            giayCamKetSuDungThuocNgoaiBHViewModel.CheckCreateOrCapNhat = false; // false là tao moires
            return Ok(giayCamKetSuDungThuocNgoaiBHViewModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInGiayCamKetSuDungThuocNgoaiBH")]
        public async Task<ActionResult> PhieuInGiayCamKetSuDungThuocNgoaiBH(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _dieuTriNoiTruService.PhieuInGiayCamKetSuDungNgoaiBHYT(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }
    }
}
