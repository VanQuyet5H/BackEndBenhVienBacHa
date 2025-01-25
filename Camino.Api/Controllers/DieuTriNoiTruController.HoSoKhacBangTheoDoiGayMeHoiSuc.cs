using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHoSoKhacBangTheoDoiGayMeHoiSuc")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacBangTheoDoiGayMeHoiSucViewModel>> GetThongTinHoSoKhacBangTheoDoiGayMeHoiSucAsync(long yeuCauTiepNhanId, long? hoSoKhacId)
        {
            var hoSoKhac = _dieuTriNoiTruService.GetThongTinHoSoKhacBangTheoDoiGayMeHoiSuc(yeuCauTiepNhanId, hoSoKhacId);

            var hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacBangTheoDoiGayMeHoiSucViewModel>() ?? new HoSoKhacBangTheoDoiGayMeHoiSucViewModel();

            if (hoSoKhac == null)
            {
                var currentUser = _userAgentHelper.GetCurrentUserId();
                hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
            }

            return hoSoKhacVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaThongTinHoSoKhacBangTheoDoiGayMeHoiSuc")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaThongTinHoSoKhacBangTheoDoiGayMeHoiSuc([FromBody] HoSoKhacBangTheoDoiGayMeHoiSucViewModel hoSoKhacBangTheoDoiGayMeHoiSucViewModel)
        {
            hoSoKhacBangTheoDoiGayMeHoiSucViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            hoSoKhacBangTheoDoiGayMeHoiSucViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            hoSoKhacBangTheoDoiGayMeHoiSucViewModel.ThoiDiemThucHien = DateTime.Now;

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(hoSoKhacBangTheoDoiGayMeHoiSucViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruHoSoKhacs)
                                                                                                                                           .ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            if (hoSoKhacBangTheoDoiGayMeHoiSucViewModel.Id == 0)
            {
                var hoSoKhac = hoSoKhacBangTheoDoiGayMeHoiSucViewModel.ToEntity<NoiTruHoSoKhac>();
                yeuCauTiepNhan.NoiTruHoSoKhacs.Add(hoSoKhac);
            }
            else
            {
                var hoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Single(p => p.Id == hoSoKhacBangTheoDoiGayMeHoiSucViewModel.Id);
                hoSoKhac = hoSoKhacBangTheoDoiGayMeHoiSucViewModel.ToEntity(hoSoKhac);

                foreach (var item in hoSoKhac.NoiTruHoSoKhacFileDinhKems)
                {
                    if (item.WillDelete != true)
                    {
                        await _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                    }
                }
            }

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [HttpDelete("XoaThongTinHoSoKhacBangTheoDoiGayMeHoiSuc")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaThongTinHoSoKhacBangTheoDoiGayMeHoiSuc(long yeuCauTiepNhanId, long hoSoKhacId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId, o => o.Include(p => p.NoiTruHoSoKhacs)
                                                                                                   .ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            var hoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Single(p => p.Id == hoSoKhacId);
            hoSoKhac.WillDelete = true;

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachBangTheoDoiGayMeHoiSuc")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachBangTheoDoiGayMeHoiSuc([FromBody] QueryInfo queryInfo)
        {
            return await _dieuTriNoiTruService.GetDanhSachBangTheoDoiGayMeHoiSuc(queryInfo);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachBangTheoDoiGayMeHoiSuc")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachBangTheoDoiGayMeHoiSuc([FromBody] QueryInfo queryInfo)
        {
            return await _dieuTriNoiTruService.GetTotalPagesDanhSachBangTheoDoiGayMeHoiSuc(queryInfo);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("InBangTheoDoiGayMeHoiSuc")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> InBangTheoDoiGayMeHoiSuc(long yeuCauTiepNhanId, long hoSoKhacId)
        {
            var html = await _dieuTriNoiTruService.InBangTheoDoiGayMeHoiSuc(yeuCauTiepNhanId, hoSoKhacId, false);

            return html;
        }
    }
}
