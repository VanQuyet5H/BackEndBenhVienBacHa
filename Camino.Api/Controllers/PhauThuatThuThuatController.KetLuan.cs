using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        [HttpGet("IsExistDoubleKetLuanVaTheoDoi")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<bool> IsExistDoubleKetLuanVaTheoDoi(long yeuCauTiepNhanId, long phongBenhVienId)
        {
            return await _phauThuatThuThuatService.IsExistDoubleKetLuanVaTheoDoi(yeuCauTiepNhanId, phongBenhVienId);
        }

        [HttpGet("CheckHasPhauThuat")]
        public async Task<bool> CheckHasPhauThuat(long noiThucHienId, long yctnId, bool isExistTheoDoi)
        {
            return await _phauThuatThuThuatService.CheckHasPhauThuat(noiThucHienId, yctnId, isExistTheoDoi);
        }

        [HttpGet("LaPhauThuat")]
        public async Task<bool> LaPhauThuat(long ycdvktId)
        {
            var ycdvktEntity = await _yeuCauDichVuKyThuatService.GetByIdAsync(ycdvktId);

            return _phauThuatThuThuatService.IsPhauThuat(ycdvktEntity.DichVuKyThuatBenhVienId);
        }

        [HttpGet("ThuThuatCoBacSi")]
        public async Task<bool> ThuThuatCoBacSi(long ycdvktId)
        {
            var phauThuatThuThuat = await _yeuCauDichVuKyThuatService.GetByIdAsync(ycdvktId,
                w => w.Include(q => q.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(q => q.PhauThuatThuThuatEkipBacSis)
                    .Include(q => q.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(q => q.PhauThuatThuThuatEkipDieuDuongs));

            if (phauThuatThuThuat.YeuCauDichVuKyThuatTuongTrinhPTTT == null)
            {
                return false;
            }

            return phauThuatThuThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipBacSis.Any() ||
                   phauThuatThuThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.PhauThuatThuThuatEkipDieuDuongs.Any();
        }

        [HttpGet("GetTheoDoiSauPhauThuatThuThuatByYeuCauTiepNhan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> GetTheoDoiSauPhauThuatThuThuatByYeuCauTiepNhan(long yeuCauTiepNhanId, bool? isTuongTrinhLai)
        {
            var theoDoiSauPhauThuatThuThuat = await _theoDoiSauPhauThuatThuThuatService.GetTheoDoiSauPhauThuatThuThuatByYeuCauTiepNhan(yeuCauTiepNhanId, isTuongTrinhLai);

            return Ok(theoDoiSauPhauThuatThuThuat.ToModel<TheoDoiSauPhauThuatThuThuatViewModel>());
        }

        [HttpPost("TheoDoiSauPhauThuatThuThuat")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> TheoDoiSauPhauThuatThuThuat([FromBody] TheoDoiSauPhauThuatThuThuatViewModel theoDoiSauPhauThuatThuThuatViewModel)
        {
            var isHoanThanhTatCa = await _phauThuatThuThuatService.KiemTraTatCaYeuCauDichVuKyThuatPTTT(theoDoiSauPhauThuatThuThuatViewModel.YeuCauTiepNhanId, theoDoiSauPhauThuatThuThuatViewModel.PhongBenhVienId);

            if (!isHoanThanhTatCa)
            {
                throw new ApiException(_localizationService.GetResource("PTTT.KetLuan.YeuCauDichVuKyThuats.NotDone"));
            }

            var theoDoiSauPhauThuatThuThuat = theoDoiSauPhauThuatThuThuatViewModel.ToEntity<TheoDoiSauPhauThuatThuThuat>();

            if (theoDoiSauPhauThuatThuThuat.Id != 0)
            {
                var entity = await _theoDoiSauPhauThuatThuThuatService.GetByIdAsync(theoDoiSauPhauThuatThuThuat.Id);

                if (entity == null)
                {
                    return NotFound();
                }

                theoDoiSauPhauThuatThuThuatViewModel.ToEntity(entity);
                await _theoDoiSauPhauThuatThuThuatService.UpdateAsync(entity);
            }
            else
            {
                theoDoiSauPhauThuatThuThuat.ThoiDiemBatDauTheoDoi = DateTime.Now;
                await _theoDoiSauPhauThuatThuThuatService.AddAsync(theoDoiSauPhauThuatThuThuat);
            }

            //Thay đổi trang thái
            await _phauThuatThuThuatService.CapNhatTheoDoiPhauThuatThuThuatChoYeuCauDichVuKyThuat(theoDoiSauPhauThuatThuThuatViewModel.YeuCauTiepNhanId, theoDoiSauPhauThuatThuThuat.Id, null, theoDoiSauPhauThuatThuThuatViewModel.NhanVienKetLuanId, theoDoiSauPhauThuatThuThuatViewModel.PhongBenhVienId, false, theoDoiSauPhauThuatThuThuatViewModel.ThoiDiemKetThucTheoDoi);

            return Ok(await _theoDoiSauPhauThuatThuThuatService.GetByIdAsync(theoDoiSauPhauThuatThuThuat.Id));
        }

        [HttpPost("ChuyenGiaoSauPhauThuatThuThuat")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> ChuyenGiaoSauPhauThuatThuThuat([FromBody] TheoDoiSauPhauThuatThuThuatViewModel theoDoiSauPhauThuatThuThuatViewModel)
        {
            var isHoanThanhTatCa = await _phauThuatThuThuatService.KiemTraTatCaYeuCauDichVuKyThuatPTTT(theoDoiSauPhauThuatThuThuatViewModel.YeuCauTiepNhanId, theoDoiSauPhauThuatThuThuatViewModel.PhongBenhVienId);

            if (!isHoanThanhTatCa)
            {
                throw new ApiException(_localizationService.GetResource("PTTT.KetLuan.YeuCauDichVuKyThuats.NotDone"));
            }

            //Thêm TheoDoiSauPhauThuatThuThuat
            theoDoiSauPhauThuatThuThuatViewModel.BacSiPhuTrachTheoDoiId = theoDoiSauPhauThuatThuThuatViewModel.BacSiPhuTrachTheoDoiId == 0 ? null : theoDoiSauPhauThuatThuThuatViewModel.BacSiPhuTrachTheoDoiId;
            theoDoiSauPhauThuatThuThuatViewModel.DieuDuongPhuTrachTheoDoiId = theoDoiSauPhauThuatThuThuatViewModel.DieuDuongPhuTrachTheoDoiId == 0 ? null : theoDoiSauPhauThuatThuThuatViewModel.DieuDuongPhuTrachTheoDoiId;

            var theoDoiSauPhauThuatThuThuat = theoDoiSauPhauThuatThuThuatViewModel.ToEntity<TheoDoiSauPhauThuatThuThuat>();

            if (theoDoiSauPhauThuatThuThuat.Id != 0)
            {
                //Kiểm tra lần khám đã lưu chưa
                var checkPreviousKhamTheoDoi = await _phauThuatThuThuatService.CheckPreviousKhamTheoDoi(theoDoiSauPhauThuatThuThuat.Id);
                if (!checkPreviousKhamTheoDoi)
                {
                    throw new ApiException(_localizationService.GetResource("PTTT.TheoDoi.KhamTheoDoiTruoc.ThoiDiemKetThuc.Required"));
                }

                var entity = await _theoDoiSauPhauThuatThuThuatService.GetByIdAsync(theoDoiSauPhauThuatThuThuat.Id);

                if (entity == null)
                {
                    return NotFound();
                }

                //Cho phép tự nhập do có thể nhập sau khi theo dõi
                //theoDoiSauPhauThuatThuThuatViewModel.ThoiDiemKetThucTheoDoi = DateTime.Now;

                if(theoDoiSauPhauThuatThuThuat.ThoiDiemKetThucTheoDoi == null || theoDoiSauPhauThuatThuThuat.ThoiDiemKetThucTheoDoi > DateTime.Now)
                {
                    theoDoiSauPhauThuatThuThuatViewModel.ThoiDiemKetThucTheoDoi = DateTime.Now;
                }

                theoDoiSauPhauThuatThuThuatViewModel.ToEntity(entity);
                await _theoDoiSauPhauThuatThuThuatService.UpdateAsync(entity);
            }
            else
            {
                await _theoDoiSauPhauThuatThuThuatService.AddAsync(theoDoiSauPhauThuatThuThuat);
            }
            //await _theoDoiSauPhauThuatThuThuatService.AddAsync(theoDoiSauPhauThuatThuThuat);

            //Thay đổi trang thái
            await _phauThuatThuThuatService.ChuyenGiaoSauPhauThuatThuThuat(theoDoiSauPhauThuatThuThuatViewModel.YeuCauTiepNhanId, theoDoiSauPhauThuatThuThuat.Id, theoDoiSauPhauThuatThuThuatViewModel.NhanVienKetLuanId, theoDoiSauPhauThuatThuThuatViewModel.PhongBenhVienId, theoDoiSauPhauThuatThuThuatViewModel.IsChuyenGiaoTuTuongTrinh, theoDoiSauPhauThuatThuThuatViewModel.ThoiDiemKetThucTheoDoi);

            return Ok(await _theoDoiSauPhauThuatThuThuatService.GetByIdAsync(theoDoiSauPhauThuatThuThuat.Id));
        }

        [HttpPost("KhongTuongTrinh")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> KhongTuongTrinh([FromBody] KhongTuongTrinhViewModel khongTuongTrinhViewModel)
        {
            await _phauThuatThuThuatService.KhongTuongTrinh(khongTuongTrinhViewModel.YeuCauTiepNhanId, khongTuongTrinhViewModel.YeuCauDichVuKyThuatId, khongTuongTrinhViewModel.PhongBenhVienId);

            return Ok();
        }

        [HttpPost("HoanThanhTuongTrinhLai")]
        public async Task<ActionResult> HoanThanhTuongTrinhLai(long phongBenhVienId, long yeuCauTiepNhanId)
        {
            await _phauThuatThuThuatService.HoanThanhTuongTrinhLai(phongBenhVienId, yeuCauTiepNhanId);

            return Ok();
        }
    }
}