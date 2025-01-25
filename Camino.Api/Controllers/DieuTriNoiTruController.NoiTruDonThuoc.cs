using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("GetDataForGridDanhSachNoiTruDonThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetDataForGridDanhSachNoiTruDonThuoc([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachNoiTruDonThuoc(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPageForGridDanhSachNoiTruDonThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetTotalPageForGridDanhSachNoiTruDonThuoc([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachNoiTruDonThuoc(queryInfo);
            return gridData;
        }

        [HttpPost("GetNoiTruDuocPhamInfoById")]
        public ActionResult GetNoiTruDuocPhamInfoById(ThongTinThuocNoiTruVo thongTinThuocVM)
        {
            var entity = _dieuTriNoiTruService.GetNoiTruDuocPhamInfoById(thongTinThuocVM);
            return Ok(entity);
        }

        #region CRUD DonThuocChiTiet
        [HttpPost("ThemNoiTruDonThuocChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemNoiTruDonThuocChiTiet(NoiTruDonThuocChiTietViewModel donThuocChiTietVM)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(donThuocChiTietVM.YeuCauTiepNhanId);

            NoiTruDonThuocChiTietVo donThuocChiTiet = new NoiTruDonThuocChiTietVo
            {
                YeuCauTiepNhanId = donThuocChiTietVM.YeuCauTiepNhanId,
                DuocPhamId = donThuocChiTietVM.DuocPhamId.Value,
                SoLuong = donThuocChiTietVM.SoLuong.Value,
                SoNgayDung = donThuocChiTietVM.SoNgayDung,
                ThoiGianDungSang = donThuocChiTietVM.ThoiGianDungSang,
                ThoiGianDungTrua = donThuocChiTietVM.ThoiGianDungTrua,
                ThoiGianDungChieu = donThuocChiTietVM.ThoiGianDungChieu,
                ThoiGianDungToi = donThuocChiTietVM.ThoiGianDungToi,
                DungSang = donThuocChiTietVM.SangDisplay.ToFloatFromFraction(),
                DungTrua = donThuocChiTietVM.TruaDisplay.ToFloatFromFraction(),
                DungChieu = donThuocChiTietVM.ChieuDisplay.ToFloatFromFraction(),
                DungToi = donThuocChiTietVM.ToiDisplay.ToFloatFromFraction(),
                LoaiKhoThuoc = (LoaiKhoThuoc)donThuocChiTietVM.LoaiKhoThuoc,
                GhiChu = donThuocChiTietVM.GhiChu,
            };
            if (!string.IsNullOrEmpty(donThuocChiTietVM.GhiChu))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.Thuoc,
                    GhiChu = donThuocChiTietVM.GhiChu
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            var error = await _dieuTriNoiTruService.ThemNoiTruDonThuocChiTiet(donThuocChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }

        [HttpPost("CapNhatNoiTruDonThuocChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatNoiTruDonThuocChiTiet(NoiTruDonThuocChiTietViewModel donThuocChiTietVM)
        {
            NoiTruDonThuocChiTietVo donThuocChiTiet = new NoiTruDonThuocChiTietVo
            {
                NoiTruDonThuocChiTietId = donThuocChiTietVM.Id,
                YeuCauTiepNhanId = donThuocChiTietVM.YeuCauTiepNhanId,
                DuocPhamId = donThuocChiTietVM.DuocPhamId.Value,
                SoLuong = donThuocChiTietVM.SoLuong.Value,
                SoNgayDung = donThuocChiTietVM.SoNgayDung,
                ThoiGianDungSang = donThuocChiTietVM.ThoiGianDungSang,
                ThoiGianDungTrua = donThuocChiTietVM.ThoiGianDungTrua,
                ThoiGianDungChieu = donThuocChiTietVM.ThoiGianDungChieu,
                ThoiGianDungToi = donThuocChiTietVM.ThoiGianDungToi,
                DungSang = donThuocChiTietVM.SangDisplay.ToFloatFromFraction(),
                DungTrua = donThuocChiTietVM.TruaDisplay.ToFloatFromFraction(),
                DungChieu = donThuocChiTietVM.ChieuDisplay.ToFloatFromFraction(),
                DungToi = donThuocChiTietVM.ToiDisplay.ToFloatFromFraction(),
                LoaiKhoThuoc = (LoaiKhoThuoc)donThuocChiTietVM.LoaiKhoThuoc,
                GhiChu = donThuocChiTietVM.GhiChu,
            };
            if (!string.IsNullOrEmpty(donThuocChiTietVM.GhiChu))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.Thuoc,
                    GhiChu = donThuocChiTietVM.GhiChu
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            var error = await _dieuTriNoiTruService.CapNhatNoiTruDonThuocChiTiet(donThuocChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);

            return NoContent();
        }

        [HttpPost("XoaNoiTruDonThuocChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaNoiTruDonThuocChiTiet(NoiTruDonThuocChiTietViewModel donThuocChiTietVM)
        {
            NoiTruDonThuocChiTietVo donThuocChiTiet = new NoiTruDonThuocChiTietVo
            {
                YeuCauTiepNhanId = donThuocChiTietVM.YeuCauTiepNhanId,
                NoiTruDonThuocChiTietId = donThuocChiTietVM.Id,
            };
            var error = await _dieuTriNoiTruService.XoaNoiTruDonThuocChiTiet(donThuocChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }
        #endregion

        [HttpPost("InDonThuocRaVien")]
        public ActionResult InDonThuocRaVien(InToaThuocRaVien inToaThuoc)//InToaThuoc
        {
            var result = _dieuTriNoiTruService.InDonThuocRaVien(inToaThuoc);
            return Ok(result);
        }
    }
}
