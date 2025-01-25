using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
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


        [HttpPost("GetTongKetBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> GetTongKetBenhAn(long yeuCauTiepNhanId)
        {
            var entity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId, s =>
                    s.Include(x => x.NoiTruBenhAn)
                    );
            var noiTruBenhAn = entity.NoiTruBenhAn;

            var result = new DieuTriNoiTruTongKetBenhAnViewModel();

            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinTongKetBenhAn))
            {
                result = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(noiTruBenhAn.ThongTinTongKetBenhAn);

                if (result.LanPhauThuats != null)
                {
                    foreach (var pttt in result.LanPhauThuats)
                    {
                        if (!string.IsNullOrEmpty(pttt.PTTTPhuongPhap))
                        {
                            var content = pttt.PTTTPhuongPhap.Split(";");
                            if (content.Any())
                            {
                                pttt.PTTT = content[0];
                                pttt.VoCam = content[1];
                            }
                        }
                    }
                }

            }
            else
            {
                result.gridPhauThuatThuThuat = GetDanhSachPTTT(yeuCauTiepNhanId);
                result.LanPhauThuats = GetDanhSachPTTT(yeuCauTiepNhanId);
            }

            return Ok(result);
        }

        [HttpPost("LuuTongKetBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuTongKetBenhAn([FromBody] DieuTriNoiTruTongKetBenhAnViewModel model)
        {

            //validate
            if (model.gridPhauThuatThuThuat != null)
            {
                foreach (var item in model.gridPhauThuatThuThuat)
                {
                    var modelCompare = model.gridPhauThuatThuThuat.Where(p => p != item).ToList();
                    if (modelCompare != null && modelCompare.Any(p => p.PTTTNgayGio == item.PTTTNgayGio
                                                    && (!string.IsNullOrEmpty(p.PTTT) && !string.IsNullOrEmpty(item.PTTT) && p.PTTT == item.PTTT)
                                                        || (!string.IsNullOrEmpty(p.VoCam) && !string.IsNullOrEmpty(item.VoCam) && p.VoCam == item.VoCam)))
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.DateTimePTTT.IsExists"));
                    }

                    if (item.PTTTNgayGio > DateTime.Now)
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.DateTimePTTT.MoreThanNow"));
                    }
                }
            }

            if (model.LanPhauThuats != null)
            {
                foreach (var item in model.LanPhauThuats)
                {
                    var modelCompare = model.LanPhauThuats.Where(p => p != item).ToList();
                    if (modelCompare != null && modelCompare.Any(p => p.PTTTNgayGio == item.PTTTNgayGio
                                                    && (!string.IsNullOrEmpty(p.PTTT) && !string.IsNullOrEmpty(item.PTTT) && p.PTTT == item.PTTT)
                                                        || (!string.IsNullOrEmpty(p.VoCam) && !string.IsNullOrEmpty(item.VoCam) && p.VoCam == item.VoCam)))
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.DateTimePTTT.IsExists"));
                    }

                    if (item.PTTTNgayGio > DateTime.Now)
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.DateTimePTTT.MoreThanNow"));
                    }
                }
            }
            
            var entity = await _dieuTriNoiTruService.GetByIdAsync(model.YeuCauTiepNhanId ?? 0, s => s.Include(x => x.NoiTruBenhAn));
            var noiTruBenhAn = entity.NoiTruBenhAn;

            if (entity != null && entity.NoiTruBenhAn != null && entity.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }


            if (model.DacDiemTreSoSinhs != null)
            {             
                foreach (var item in model.DacDiemTreSoSinhs)
                {
                    if (item.DeLuc < noiTruBenhAn.ThoiDiemNhapVien)
                    {
                        throw new ApiException(_localizationService.GetResource("BenhAnSoSinh.NgayThangNamSinh.BenhAnConSauNhapVienBAMe"));
                    }
                }
            }


            if (model.gridPhauThuatThuThuat != null && model.gridPhauThuatThuThuat.Any())
            {
                foreach (var pttt in model.gridPhauThuatThuThuat)
                {
                    if (!string.IsNullOrEmpty(pttt.VoCam) || !string.IsNullOrEmpty(pttt.PTTT))
                    {
                        pttt.PTTTPhuongPhap = pttt.PTTT + ";" + pttt.VoCam;
                    }
                }
            }


            noiTruBenhAn.ThongTinTongKetBenhAn = JsonConvert.SerializeObject(model);
            await _dieuTriNoiTruService.UpdateAsync(entity);

            var result = new DieuTriNoiTruTongKetBenhAnViewModel();

            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinTongKetBenhAn))
            {

                result = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(noiTruBenhAn.ThongTinTongKetBenhAn);

            }

            return Ok(result);
        }

        [HttpGet("GetThongTinTheoDoiSoSinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<List<DacDiemTreSoSinh>>> GetThongTinTheoDoiSoSinh(long yeuCauTiepNhanId)
        {
            var entity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId, s => s.Include(x => x.NoiTruBenhAn));

            var thongTinTheoDoiSoSinhs = new List<DacDiemTreSoSinh>();
            var noiTruBenhAn = entity.NoiTruBenhAn;

            var result = new DieuTriNoiTruTongKetBenhAnViewModel();

            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinTongKetBenhAn))
            {
                result = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(noiTruBenhAn.ThongTinTongKetBenhAn);
                thongTinTheoDoiSoSinhs = result.DacDiemTreSoSinhs.Where(c => c.YeuCauTiepNhanConId == null).ToList();
                return Ok(thongTinTheoDoiSoSinhs);
            }
            return Ok(thongTinTheoDoiSoSinhs);
        }

        [HttpPost("LuuThongTinSoSinhDuocChon")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuThongTinSoSinhDuocChon([FromBody] ThongTinTheoDoiSoSinhDuocChon model)
        {
            var entity = await _dieuTriNoiTruService.GetByIdAsync(model.YeuCauTiepNhanMeId, s => s.Include(x => x.NoiTruBenhAn));

            var thongTinTheoDoiSoSinhs = new List<DacDiemTreSoSinh>();
            var noiTruBenhAn = entity.NoiTruBenhAn;

            var result = new DieuTriNoiTruTongKetBenhAnViewModel();

            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinTongKetBenhAn))
            {
                result = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnViewModel>(noiTruBenhAn.ThongTinTongKetBenhAn);
                foreach (var dacDiemTreSoSinh in result.DacDiemTreSoSinhs)
                {
                    foreach (var chonThongTinSoSinh in model.DacDiemTreSoSinhs)
                    {
                        if (dacDiemTreSoSinh.Id == chonThongTinSoSinh.Id)
                        {
                            dacDiemTreSoSinh.YeuCauTiepNhanConId = model.YeuCauTiepNhanConId;
                        }
                    }
                }

                noiTruBenhAn.ThongTinTongKetBenhAn = JsonConvert.SerializeObject(result);
                await _dieuTriNoiTruService.UpdateAsync(entity);
            }
            return Ok();
        }

        private List<GridPhauThuatThuThuatViewModel> GetDanhSachPTTT(long yeuCauTiepNhanId)
        {
            var yctn = _dieuTriNoiTruService.GetById(yeuCauTiepNhanId, s =>
                   s.Include(x => x.NoiTruBenhAn)
                   .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                   .ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(x => x.PhuongPhapVoCam)
                   .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.YeuCauDichVuKyThuatTuongTrinhPTTT).ThenInclude(x => x.PhuongPhapVoCam)

                   .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
                   .ThenInclude(c => c.PhauThuatThuThuatEkipDieuDuongs)
                   .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(c => c.YeuCauDichVuKyThuatTuongTrinhPTTT)
                   .ThenInclude(c => c.PhauThuatThuThuatEkipBacSis)
                   );

            var lstModel = new List<GridPhauThuatThuThuatViewModel>();

            if (yctn.YeuCauNhapVien != null && yctn.YeuCauNhapVien.YeuCauKhamBenh != null
                    && yctn.YeuCauNhapVien.YeuCauKhamBenh.YeuCauTiepNhan != null
                    && yctn.YeuCauNhapVien.YeuCauKhamBenh.YeuCauTiepNhan.YeuCauDichVuKyThuats.Any(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT != null))
            {
                var lstTuongTrinh = yctn.YeuCauNhapVien.YeuCauKhamBenh.YeuCauTiepNhan
                    .YeuCauDichVuKyThuats.Where(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT != null)
                        .Select(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)
                        .ToList();

                foreach (var tt in lstTuongTrinh)
                {
                    lstModel.Add(new GridPhauThuatThuThuatViewModel
                    {
                        PTTTNgayGio = tt.ThoiDiemPhauThuat,
                        PTTT = tt.TenPhuongPhapPTTT,
                        VoCam = tt.PhuongPhapVoCam?.Ten,

                        PTTTBacSyGayMe = tt.PhauThuatThuThuatEkipBacSis.Any() ? tt.PhauThuatThuThuatEkipBacSis.Where(cc => cc.VaiTroBacSi == Enums.EnumVaiTroBacSi.GayMeTeChinh).LastOrDefault()?.NhanVienId : 0,
                        PTTTPhauThuatVien = tt.PhauThuatThuThuatEkipBacSis.Any() ? tt.PhauThuatThuThuatEkipBacSis.Where(cc => cc.VaiTroBacSi == Enums.EnumVaiTroBacSi.PhauThuatVienChinh).LastOrDefault()?.NhanVienId : 0,
                    });
                }
            }

            return lstModel;
        }

    }
}
