using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Camino.Api.Models.Error;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        [HttpPost("GetDataForGridTuVanThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public GridDataSource GetDataForGridTuVanThuoc([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetDataForGridTuVanThuoc(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPageForGridTuVanThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public GridDataSource GetTotalPageForGridTuVanThuoc([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetTotalPageForGridTuVanThuoc(queryInfo);
            return gridData;
        }

        [HttpPost("DuocPhamTuVans")]
        public async Task<ActionResult<ICollection<DuocPhamTuVanTemplate>>> DuocPhamTuVans(DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetDuocPhamTuVans(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("InfoTonKhoDuocPhamTuVan")]
        public ActionResult InfoTonKhoDuocPhamTuVan(ThongTinThuocTuVanVo thongTinThuocVM)
        {
            var entity = _yeuCauKhamBenhService.GetDuocPhamTuVanInfoById(thongTinThuocVM);
            return Ok(entity);
        }

        #region CRUD DonThuocChiTiet
        [HttpPost("ThemDonThuocTuVanSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult> ThemDonThuocTuVanSucKhoe(YeuCauKhamBenhDonThuocChiTietViewModel donThuocChiTietVM)
        {
            if (donThuocChiTietVM.IsKhamDoanTatCa != true)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            DonThuocChiTietVo donThuocChiTiet = new DonThuocChiTietVo
            {
                YeuCauTiepNhanId = donThuocChiTietVM.YeuCauTiepNhanId.Value,
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
            var error = await _yeuCauKhamBenhService.ThemDonThuocTuVanSucKhoe(donThuocChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }

        [HttpPost("CapNhatDonThuocTuVanSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult> CapNhatDonThuocTuVanSucKhoe(YeuCauKhamBenhDonThuocChiTietViewModel donThuocChiTietVM)
        {
            if (donThuocChiTietVM.IsKhamDoanTatCa != true)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            DonThuocChiTietVo donThuocChiTiet = new DonThuocChiTietVo
            {
                DonThuocChiTietId = donThuocChiTietVM.Id,
                YeuCauTiepNhanId = donThuocChiTietVM.YeuCauTiepNhanId.Value,
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
            var error = await _yeuCauKhamBenhService.CapNhatDonThuocTuVanSucKhoe(donThuocChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);

            return NoContent();
        }

        [HttpPost("XoaDonThuocTuVanSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult> XoaDonThuocTuVanSucKhoe(YeuCauKhamBenhDonThuocChiTietViewModel donThuocChiTietVM)
        {
            var checkDonThuocChiTiet = await _yeuCauKhamBenhService.KiemTraThuocTuVanSucKhoe(donThuocChiTietVM.Id);
            if (!checkDonThuocChiTiet)
            {
                throw new ApiException(_localizationService.GetResource("DonThuoc.NotExists"));
            }
            if (donThuocChiTietVM.IsKhamDoanTatCa != true)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(donThuocChiTietVM.YeuCauTiepNhanId.Value, s => s.Include(p => p.TuVanThuocKhamSucKhoes));
            var tuVanThuocSucKhoe = yeuCauTiepNhan.TuVanThuocKhamSucKhoes.Where(p => p.Id == donThuocChiTietVM.Id).FirstOrDefault();
            if (tuVanThuocSucKhoe == null)
            {
                return NotFound();
            }
            tuVanThuocSucKhoe.WillDelete = true;
            await _yeuCauTiepNhanService.UpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }
        #endregion


        [HttpPost("InTuVanThuoc")]
        public ActionResult InTuVanThuoc(InTuVanThuoc inTuVanThuoc)//InTuVanThuoc
        {
            var result = _yeuCauKhamBenhService.InTuVanThuoc(inTuVanThuoc);
            return Ok(result);
        }
    }
}
