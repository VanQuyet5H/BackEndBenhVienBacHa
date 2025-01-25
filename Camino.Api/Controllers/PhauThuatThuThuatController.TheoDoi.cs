using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamTheoDoi;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        //[HttpGet("GetKetQuaSinhHieus")]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        //public async Task<ActionResult> GetKetQuaSinhHieus(long yeuCauTiepNhanId)
        //{

        //}

        [HttpGet("GetKhamTheoDois")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> GetKhamTheoDois(long theoDoiSauPhauThuatThuThuatId)
        {
            var khamTheoDois = await _khamTheoDoiService.GetKhamTheoDoisByTheoDoiSauPhauThuatThuThuat(theoDoiSauPhauThuatThuThuatId);

            foreach(var item in khamTheoDois)
            {
                // get template khám theo dịch vụ khám
                if (string.IsNullOrEmpty(item.ThongTinKhamTheoDoiTemplate))
                {
                    item.ThongTinKhamTheoDoiTemplate = await _phauThuatThuThuatService.GetTemplateKhamTheoDoi();
                }
            }

            return Ok(khamTheoDois.Select(p => p.ToModel<KhamTheoDoiViewModel>()).ToList());
        }

        [HttpGet("GetKhamTheoDoi")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KhamTheoDoiViewModel>> GetKhamTheoDoi(long id)
        {
            var khamTheoDoi = await _khamTheoDoiService.GetByIdAsync(id, k => k.Include(p => p.KhamTheoDoiBoPhanKhacs));

            if (khamTheoDoi == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(khamTheoDoi.ThongTinKhamTheoDoiTemplate))
            {
                khamTheoDoi.ThongTinKhamTheoDoiTemplate = await _phauThuatThuThuatService.GetTemplateKhamTheoDoi();
            }

            return Ok(khamTheoDoi.ToModel<KhamTheoDoiViewModel>());
        }

        [HttpPost("AddKhamTheoDoi")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<KhamTheoDoiViewModel>> AddKhamTheoDoi([FromBody] KhamTheoDoiViewModel khamTheoDoiViewModel)
        {
            //Kiểm tra lần khám trước
            var checkPreviousKhamTheoDoi = await _phauThuatThuThuatService.CheckPreviousKhamTheoDoi(khamTheoDoiViewModel.TheoDoiSauPhauThuatThuThuatId);
            if(!checkPreviousKhamTheoDoi)
            {
                throw new ApiException(_localizationService.GetResource("PTTT.TheoDoi.KhamTheoDoiTruoc.ThoiDiemKetThuc.Required"));
            }

            var khamTheoDoi = khamTheoDoiViewModel.ToEntity<KhamTheoDoi>();

            await _khamTheoDoiService.AddAsync(khamTheoDoi);

            var khamTheoDoiRes = await _khamTheoDoiService.GetByIdAsync(khamTheoDoi.Id);

            var actionName = nameof(GetKhamTheoDoi);

            return CreatedAtAction(
                actionName,
                new { id = khamTheoDoi.Id },
                khamTheoDoiRes.ToModel<KhamTheoDoiViewModel>()
            );
        }

        [HttpPost("UpdateKhamTheoDoi")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<KhamTheoDoiViewModel>> UpdateKhamTheoDoi([FromBody] KhamTheoDoiViewModel khamTheoDoiViewModel)
        {
            var khamTheoDoi = await _khamTheoDoiService.GetByIdAsync(khamTheoDoiViewModel.Id);
            
            if(khamTheoDoi == null)
            {
                return NotFound();
            }

            khamTheoDoiViewModel.ToEntity(khamTheoDoi);

            await _khamTheoDoiService.UpdateAsync(khamTheoDoi);

            //KhamTheoDoiBoPhanKhac
            List<long> khamTheoDoiBoPhanKhacIds = new List<long>();

            foreach(var item in khamTheoDoiViewModel.KhamTheoDoiBoPhanKhacs)
            {
                if (item.Id == 0)
                {
                    var khamTheoDoiBoPhanKhac = item.ToEntity<KhamTheoDoiBoPhanKhac>();
                    await _khamTheoDoiBoPhanKhacService.AddAsync(khamTheoDoiBoPhanKhac);
                    item.Id = khamTheoDoiBoPhanKhac.Id;
                }

                khamTheoDoiBoPhanKhacIds.Add(item.Id);
            }

            await _khamTheoDoiBoPhanKhacService.DeleteKhamTheoDoiBoPhanKhacs(khamTheoDoiBoPhanKhacIds, khamTheoDoi.Id);

            return Ok(khamTheoDoi);
        }

        [HttpPost("BenhNhanTuVongKhiTheoDoi")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> BenhNhanTuVongKhiTheoDoi(TuongTrinhTuVongViewModel tuongTrinhTuVongViewModel)
        {
            await _phauThuatThuThuatService.BenhNhanTuVongKhiTheoDoi(tuongTrinhTuVongViewModel.YeuCauTiepNhanId ?? 0, tuongTrinhTuVongViewModel.TheoDoiSauPhauThuatThuThuatId ?? 0, tuongTrinhTuVongViewModel.NhanVienKetLuanId ?? 0, tuongTrinhTuVongViewModel.PhongBenhVienId ?? 0, tuongTrinhTuVongViewModel.TgTuVong, tuongTrinhTuVongViewModel.TuVong);
            return NoContent();
        }
    }
}