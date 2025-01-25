using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
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
        [HttpGet("GetGiayCamKetGayMeGiamDauTrongDeSauPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<GiayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel> GetGiayCamKetGayMeGiamDauTrongDeSauPhauThuat(long yeuCauTiepNhanId)
        {
            var giayCamKetEntity = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetGayMeGiamDauTrongDeSauPhauThuat);

            if (giayCamKetEntity != null)
            {
                var listNoiTruHoSoKhacFileDinhKem =
                    await _noiDuTruHoSoKhacFileDinhKemService.GetListFileDinhKem(giayCamKetEntity.Id);
                var giayCamKet = JsonConvert.DeserializeObject<GiayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel>(giayCamKetEntity.ThongTinHoSo);
                giayCamKet.IdNoiTruHoSo = giayCamKetEntity.Id;
                giayCamKet.YeuCauTiepNhanId = yeuCauTiepNhanId;

                if (giayCamKet.BSGMHSId != null)
                {
                    giayCamKet.BSGMHSText = _dieuTriNoiTruService.GetTenBS((long)giayCamKet.BSGMHSId);
                }

                return giayCamKet;
            }

            var giayCamKetKtMoi = new GiayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel();
            giayCamKetKtMoi.YeuCauTiepNhanId = yeuCauTiepNhanId;
            giayCamKetKtMoi.NgayThucHien = DateTime.Now;
            return giayCamKetKtMoi;
        }

        [HttpPost("UpdateGiayGiayCamKetGayMeGiamDauTrongDeSauPhauThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateGiayGiayCamKetGayMeGiamDauTrongDeSauPhauThuat([FromBody] GiayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel giayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var thongTinHoSo = JsonConvert.SerializeObject(giayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (giayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(giayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetGayMeGiamDauTrongDeSauPhauThuat;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = DateTime.Now;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();

                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                giayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel.CheckCreateOrCapNhat = true; // true là cập nhat
                return Ok(giayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetGayMeGiamDauTrongDeSauPhauThuat,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };

            await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhacNew);
            giayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            giayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel.CheckCreateOrCapNhat = false; // false là tao moires
            return Ok(giayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInGiayCamKetGayMeGiamDauTrongDeSauPhauThuat")]
        public async Task<ActionResult> PhieuInGiayCamKetGayMeGiamDauTrongDeSauPhauThuat(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _dieuTriNoiTruService.PhieuInGiayCamKetGayMeGiamDauTrongDeSauPhauThuat(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }
    }
}
