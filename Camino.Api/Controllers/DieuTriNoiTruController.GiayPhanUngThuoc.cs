using Camino.Api.Auth;
using Camino.Api.Models.GiayPhanUngThuoc;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController 
    {
        [HttpGet("GetGiayPhanUngThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<GiayPhanUngThuocViewModel> GetGiayPhanUngThuoc(long yeuCauTiepNhanId)
        {
            var giayCamKetKyThuatMoiEntity = await _noiTruHoSoKhacService.GetNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.GiayPhanUngThuoc);

            if (giayCamKetKyThuatMoiEntity != null)
            {
                var giayCamKet = JsonConvert.DeserializeObject<GiayPhanUngThuocViewModel>(giayCamKetKyThuatMoiEntity.ThongTinHoSo);
                giayCamKet.IdNoiTruHoSo = giayCamKetKyThuatMoiEntity.Id;
                if(giayCamKet.DieuDuongThucHienId != 0 && giayCamKet.DieuDuongThucHienId != null)
                {
                    giayCamKet.DieuDuongThucHienText = await _dieuTriNoiTruService.GetTenNhanVien(giayCamKet.DieuDuongThucHienId);
                }
                if (giayCamKet.BSDocPhanUngId != 0 && giayCamKet.BSDocPhanUngId != null)
                {
                    giayCamKet.BSDocPhanUngText = await _dieuTriNoiTruService.GetTenNhanVien(giayCamKet.BSDocPhanUngId);
                }
                return giayCamKet;
            }

            var giayCamKetKtMoi = new GiayPhanUngThuocViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            giayCamKetKtMoi.ChanDoan =  _noiTruHoSoKhacService.GetChanDoanNhapVien(yeuCauTiepNhanId);
            return giayCamKetKtMoi;
        }

        [HttpGet("GetGiayPhanUngThuocNew")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<GiayPhanUngThuocViewModel> GetGiayPhanUngThuocNew(long yeuCauTiepNhanId)
        {
            var giayCamKetKtMoi = new GiayPhanUngThuocViewModel();
            var nguoiDangLogin = _userAgentHelper.GetCurrentUserId();
            giayCamKetKtMoi.ChanDoan = _noiTruHoSoKhacService.GetChanDoanNhapVien(yeuCauTiepNhanId);
            return giayCamKetKtMoi;
        }
        [HttpPost("UpdateGiayPhanUngThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> UpdateGiayCamKetKyThuatMoi([FromBody] GiayPhanUngThuocViewModel GiayPhanUngThuocViewModel, long yeuCauTiepNhanId)
        {
            var nhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thongTinHoSo = JsonConvert.SerializeObject(GiayPhanUngThuocViewModel);
            var yctnEntity = await _dieuTriNoiTruService.GetByIdAsync(yeuCauTiepNhanId);

            if (GiayPhanUngThuocViewModel.IdNoiTruHoSo != null)
            {
                var noiTruHoSoKhac = await _noiTruHoSoKhacService.GetByIdAsync(GiayPhanUngThuocViewModel.IdNoiTruHoSo.GetValueOrDefault(),
                    w => w.Include(q => q.NoiTruHoSoKhacFileDinhKems));
                noiTruHoSoKhac.ThongTinHoSo = thongTinHoSo;
                noiTruHoSoKhac.LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayPhanUngThuoc;
                noiTruHoSoKhac.NhanVienThucHienId = nhanVienThucHienId;
                noiTruHoSoKhac.ThoiDiemThucHien = noiTruHoSoKhac.ThoiDiemThucHien;
                noiTruHoSoKhac.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
               
                //await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
                await _noiTruHoSoKhacService.UpdateAsync(noiTruHoSoKhac);
                GiayPhanUngThuocViewModel.CheckCreateOrCapNhat = true; // true là cập nhat
                return Ok(GiayPhanUngThuocViewModel);
            }

            var noiTruHoSoKhacNew = new NoiTruHoSoKhac
            {
                Id = 0,
                LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.GiayPhanUngThuoc,
                NhanVienThucHienId = nhanVienThucHienId,
                NoiThucHienId = noiThucHienId,
                ThoiDiemThucHien = DateTime.Now,
                YeuCauTiepNhanId = yeuCauTiepNhanId,
                ThongTinHoSo = thongTinHoSo
            };


            //await _tiepNhanBenhNhanService.PrepareForEditYeuCauTiepNhanAndUpdateAsync(yctnEntity);
            await _noiTruHoSoKhacService.AddAsync(noiTruHoSoKhacNew);
            GiayPhanUngThuocViewModel.IdNoiTruHoSo = noiTruHoSoKhacNew.Id;
            GiayPhanUngThuocViewModel.CheckCreateOrCapNhat = false; // false là tao moires
            return Ok(GiayPhanUngThuocViewModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("PhieuInGiayPhanUngthuoc")]
        public async Task<ActionResult> PhieuInGiayPhanUngThuoc(PhanUngThuocVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var phieuIns = await _noiTruHoSoKhacService.PhieuINPhanUngThuoc(dieuTriNoiTruVaServicesHttpParams);
            return Ok(phieuIns);
        }

        [HttpGet("GetDSGiayPhanUngThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<List<GiayPhanUngThuocInfo>> GetDSGiayPhanUngThuoc(long yeuCauTiepNhanId)
        {
            var giayPhanUngThuocInfos = new List<GiayPhanUngThuocInfo>();
         
            var giayCamKetKyThuatMoiEntitys = await _noiTruHoSoKhacService.GetDSNoiTruHoSoKhac(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.GiayPhanUngThuoc);

            if (giayCamKetKyThuatMoiEntitys != null)
            {
                foreach (var item in giayCamKetKyThuatMoiEntitys.ToList())
                {
                    var giayPhanUngThuocInfo = new GiayPhanUngThuocInfo();
                    var giay = JsonConvert.DeserializeObject<GiayPhanUngThuocViewModel>(item.ThongTinHoSo);
                    giayPhanUngThuocInfo.KetQuaText = giay.KetQuaText;
                    giayPhanUngThuocInfo.TenThuoc = giay.TenThuocText;

                    if (!string.IsNullOrEmpty(giay.ThoiGianThuPhanUngString))
                    {
                        DateTime thoigian = DateTime.Now;
                        DateTime.TryParseExact(giay.ThoiGianThuPhanUngString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out thoigian);
                        giayPhanUngThuocInfo.NgayPhanUngThuoc = thoigian.ApplyFormatDateTime();
                    }
                    giayPhanUngThuocInfo.Id = item.Id;
                    giayPhanUngThuocInfos.Add(giayPhanUngThuocInfo);
                }
            }
            return giayPhanUngThuocInfos;
        }

        [HttpGet("GetInFogiayPhanUngThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<GiayPhanUngThuocViewModel> GetGiayPhanUngThuocTheoId(long yeuCauTiepNhanId)
        {
            var giayCamKetKyThuatMoiEntity = await _noiTruHoSoKhacService.GetNoiTruHoSoKhacId(yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru.GiayPhanUngThuoc);
            var giayCamKet = JsonConvert.DeserializeObject<GiayPhanUngThuocViewModel>(giayCamKetKyThuatMoiEntity.ThongTinHoSo);
            giayCamKet.IdNoiTruHoSo = giayCamKetKyThuatMoiEntity.Id;
            return giayCamKet;
        }

        [HttpPost("GetListDuocPhamTheoTuTrucKhoaPhongBenhNhanDangNamNoiTru")]
        public async Task<ActionResult> GetListDuocPhamTheoTuTrucKhoaPhongBenhNhanDangNamNoiTru([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _noiTruHoSoKhacService.GetListDuocPhamTheoTuTrucKhoaPhongBenhNhanDangNamNoiTru(queryInfo);
            return Ok(lookup);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListKetQua")]
        public ActionResult GetListKetQua([FromBody] LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<KetQuaPhanUngThuocNoiTruHoSoKhac>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }
    }
}
