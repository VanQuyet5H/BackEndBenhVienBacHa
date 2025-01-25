using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinHoSoKhacGiayChuyenTuyen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<HoSoKhacGiayChuyenTuyenViewModel>> GetThongTinHoSoKhacGiayChuyenTuyenAsync(long yeuCauTiepNhanId)
        {
            var hoSoKhac = _dieuTriNoiTruService.GetThongTinHoSoKhacGiayChuyenTuyen(yeuCauTiepNhanId);

            var hoSoKhacVM = hoSoKhac?.ToModel<HoSoKhacGiayChuyenTuyenViewModel>() ?? new HoSoKhacGiayChuyenTuyenViewModel();

            if (hoSoKhac == null)
            {
                var currentUser = _userAgentHelper.GetCurrentUserId();
                hoSoKhacVM.NhanVienThucHienDisplay = await _noiTruHoSoKhacService.GetNguoiThucHien(currentUser);
                hoSoKhacVM.ThoiDiemThucHien = DateTime.Now;
            }

            return hoSoKhacVM;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaThongTinHoSoKhacGiayChuyenTuyen")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaThongTinHoSoKhacGiayChuyenTuyen([FromBody] HoSoKhacGiayChuyenTuyenViewModel hoSoKhacGiayChuyenTuyenViewModel)
        {
            hoSoKhacGiayChuyenTuyenViewModel.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            hoSoKhacGiayChuyenTuyenViewModel.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            hoSoKhacGiayChuyenTuyenViewModel.ThoiDiemThucHien = DateTime.Now;

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(hoSoKhacGiayChuyenTuyenViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruHoSoKhacs)
                                                                                                                                    .ThenInclude(p => p.NoiTruHoSoKhacFileDinhKems));

            if (hoSoKhacGiayChuyenTuyenViewModel.Id == 0)
            {
                var hoSoKhac = hoSoKhacGiayChuyenTuyenViewModel.ToEntity<NoiTruHoSoKhac>();
                yeuCauTiepNhan.NoiTruHoSoKhacs.Add(hoSoKhac);
            }
            else
            {
                var hoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Single(p => p.Id == hoSoKhacGiayChuyenTuyenViewModel.Id);
                hoSoKhac = hoSoKhacGiayChuyenTuyenViewModel.ToEntity(hoSoKhac);
            }

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetHuongDieuTri")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<string> GetHuongDieuTri(long yeuCauTiepNhanId)
        {
            var entity = _dieuTriNoiTruService.GetById(yeuCauTiepNhanId, o => o.Include(x => x.NoiTruBenhAn));

            var noiTruBenhAn = entity.NoiTruBenhAn;

            if(noiTruBenhAn == null)
            {
                return string.Empty;
            }

            var result = new DieuTriNoiTruTongKetBenhAnViewModel();

            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinTongKetBenhAn))
            {
                result = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(noiTruBenhAn.ThongTinTongKetBenhAn);
            }

            return Ok(result.HuongDieuTri);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetPhuongPhapSuDungTrongDieuTri")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<string> GetPhuongPhapSuDungTrongDieuTri(long yeuCauTiepNhanId)
        {
            var entity = _dieuTriNoiTruService.GetById(yeuCauTiepNhanId, o => o.Include(x => x.NoiTruBenhAn));

            var noiTruBenhAn = entity.NoiTruBenhAn;

            if (noiTruBenhAn == null)
            {
                return string.Empty;
            }

            var result = new DieuTriNoiTruTongKetBenhAnViewModel();

            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinTongKetBenhAn))
            {
                result = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(noiTruBenhAn.ThongTinTongKetBenhAn);
            }

            return Ok(result.PhuongPhapDieuTri);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetTinhTrangNguoiBenh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<string> GetTinhTrangNguoiBenh(long yeuCauTiepNhanId)
        {
            var entity = _dieuTriNoiTruService.GetById(yeuCauTiepNhanId, o => o.Include(x => x.NoiTruBenhAn));

            var noiTruBenhAn = entity.NoiTruBenhAn;

            if (noiTruBenhAn == null)
            {
                return string.Empty;
            }

            var result = new DieuTriNoiTruTongKetBenhAnViewModel();

            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinTongKetBenhAn))
            {
                result = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(noiTruBenhAn.ThongTinTongKetBenhAn);
            }

            return Ok(result.TinhTrangNguoiBenhKhiRaVien);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetKetQuaXNCLS")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<string> GetKetQuaXNCLS(long yeuCauTiepNhanId)
        {
            var entity = _dieuTriNoiTruService.GetById(yeuCauTiepNhanId, o => o.Include(x => x.NoiTruBenhAn));

            var noiTruBenhAn = entity.NoiTruBenhAn;

            if (noiTruBenhAn == null)
            {
                return string.Empty;
            }

            var result = new DieuTriNoiTruTongKetBenhAnViewModel();

            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinTongKetBenhAn))
            {
                result = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(noiTruBenhAn.ThongTinTongKetBenhAn);
            }
            //string chuoiKetQuaTheoCLS="";
            ////return Ok($"XN máu: {result.XNMau} - XN tế bào: {result.XNTeBao} - XN BLGP: {result.XNBLGP} - X quang: {result.XQuang} - Siêu âm: {result.SieuAm} - Các XN khác: {result. chuoiKetQuaTheoCLS += "XN máu:" + result.XNMau;}");
            //if (result.XNMau != null )
            //{
            //    chuoiKetQuaTheoCLS += "XN máu:" + result.XNMau + "  ";
            //}
            //if ( result.XNTeBao != null )
            //{
            //    chuoiKetQuaTheoCLS += "XN tế bào:" + result.XNTeBao + "  ";
            //}
            //if ( result.XNBLGP != null)
            //{
            //    chuoiKetQuaTheoCLS += "XN BLGP:" + result.XNBLGP + "  ";
            //}
            //if ( result.XQuang != null )
            //{
            //    chuoiKetQuaTheoCLS += "X quang:" + result.XQuang + "  ";
            //}
            //if (result.SieuAm != null)
            //{
            //    chuoiKetQuaTheoCLS += "Siêu âm:" + result.SieuAm + "  ";
            //}
            //if (result.XNKhac != null)
            //{
            //    chuoiKetQuaTheoCLS += "Các XN khác:" + result.XNKhac + "  ";
            //}
            //return Ok(chuoiKetQuaTheoCLS);

            var lstXN = new List<string>();

            if (!string.IsNullOrEmpty(result.XNMau)) lstXN.Add($"XN máu: {result.XNMau}");
            if (!string.IsNullOrEmpty(result.XNTeBao)) lstXN.Add($"XN tế bào: {result.XNTeBao}");
            if (!string.IsNullOrEmpty(result.XNBLGP)) lstXN.Add($"XN BLGP: {result.XNBLGP}");
            if (!string.IsNullOrEmpty(result.XQuang)) lstXN.Add($"X quang: {result.XQuang}");
            if (!string.IsNullOrEmpty(result.SieuAm)) lstXN.Add($"Siêu âm: {result.SieuAm}");
            if (!string.IsNullOrEmpty(result.XNKhac)) lstXN.Add($"Các XN khác: {result.XNKhac}");

            return Ok(string.Join(" - ", lstXN));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetChanDoan")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<string> GetChanDoan(long yeuCauTiepNhanId)
        {
            return _dieuTriNoiTruService.GetChanDoan(yeuCauTiepNhanId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("InGiayChuyenTuyen")]
        [ClaimRequirement(SecurityOperation.Process, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> InGiayChuyenTuyen(long yeuCauTiepNhanId)
        {
            var html = await _dieuTriNoiTruService.InGiayChuyenTuyen(yeuCauTiepNhanId, false);

            return html;
        }
    }
}