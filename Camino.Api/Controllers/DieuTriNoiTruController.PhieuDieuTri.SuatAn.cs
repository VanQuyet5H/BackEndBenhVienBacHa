using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetBuaAn")]
        public Task<List<LookupItemVo>> GetBuaAn()
        {
            var listEnum = EnumHelper.GetListEnum<BuaAn>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();
            return Task.FromResult(result);
        }

        [HttpPost("GetDataForGridAsyncSuatAn")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncSuatAn([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridAsyncSuatAn(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncSuatAn")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncSuatAn([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridAsyncSuatAn(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetSuatAn")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetSuatAn(DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetSuatAn(model);
            return Ok(lookup);
        }       

        [HttpPost("GetDoiTuongSuatAn")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDoiTuongSuatAn(DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetDoiTuongSuatAn(model);
            return Ok(lookup);
        }

        [HttpPost("ThemYeuCauSuatAn")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemYeuCauSuatAn(ThemSuatAnViewModel suatAnViewModel)
        {
            var yeuCauTiepNhan = await _dieuTriNoiTruService.GetYeuCauTiepNhanWithIncludeUpdate(suatAnViewModel.YeuCauTiepNhanId);

            if (yeuCauTiepNhan != null && yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            yeuCauTiepNhan = await _dieuTriNoiTruService.ThemSuatAn(yeuCauTiepNhan, suatAnViewModel.NoiTruPhieuDieuTriId
                , suatAnViewModel.DoiTuongSuDung ?? DoiTuongSuDung.BenhNhan, suatAnViewModel.SoLan ?? 0, suatAnViewModel.DichVuKyThuatBenhVienId ?? 0 , suatAnViewModel.BuaAn);

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [HttpPost("CapNhatYeuCauSuatAn")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatYeuCauSuatAn(ThemSuatAnViewModel suatAnViewModel)
        {
            var yeuCauTiepNhan = await _dieuTriNoiTruService.GetYeuCauTiepNhanWithIncludeUpdate(suatAnViewModel.YeuCauTiepNhanId);
            var yeuCauDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == suatAnViewModel.Id);
            if (yeuCauDichVuKyThuat == null)
            {
                return NotFound();
            }
            yeuCauDichVuKyThuat.SoLan = suatAnViewModel.SoLan.GetValueOrDefault();
            yeuCauDichVuKyThuat.DoiTuongSuDung = suatAnViewModel.DoiTuongSuDung;
            yeuCauDichVuKyThuat.BuaAn = suatAnViewModel.BuaAn;
            await _yeuCauTiepNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [HttpPost("XoaYeuCauSuatAn")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaYeuCauSuatAn(long yctnId, long ycdvktId)
        {
            var yeuCauTiepNhan = await _dieuTriNoiTruService.GetYeuCauTiepNhanWithIncludeUpdate(yctnId);

            var ycdvkt = yeuCauTiepNhan.YeuCauDichVuKyThuats.FirstOrDefault(p => p.Id == ycdvktId);

            if (ycdvkt != null)
            {
                ycdvkt.WillDelete = true;
                await _dieuTriNoiTruService.XuLyXoaYLenhKhiXoaDichVuNoiTruAsync(EnumNhomGoiDichVu.DichVuKyThuat, ycdvkt.Id);
                await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            }

            return NoContent();
        }

        [HttpPost("InPhieuSuatAn")]
        public async Task<ActionResult<string>> InPhieuSuatAn(Core.Domain.ValueObject.DieuTriNoiTru.XacNhanInPhieuSuatAn xacNhanIn)
        {
            var phieuIn = await _dieuTriNoiTruService.InPhieuSuatAn(xacNhanIn);
            return phieuIn;
        }

    }
}
