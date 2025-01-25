using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Camino.Api.Models.Error;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Api.Models.Thuoc;
using System.Net;
using Camino.Core.Domain.Entities.Thuocs;
using System.Web;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {

        [HttpPost("GetDataForGridKetLuanDSCK")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public GridDataSource GetDataForGridKeToa([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetDataForGridKeToa(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPageForGridKetLuanDSCK")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public GridDataSource GetTotalPageForGridKeToa([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetTotalPageForGridKeToa(queryInfo);
            return gridData;
        }

        [HttpPost("ICD")]
        public async Task<ActionResult> ICD([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetICDs(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetLoiDanTheoICD")]
        public async Task<ActionResult> GetLoiDanTheoICD(long iCDId)
        {
            var lookup = await _yeuCauKhamBenhService.GetLoiDanTheoICD(iCDId);
            return Ok(lookup);
        }

        [HttpPost("ICDKhacs")]
        public async Task<ActionResult<ICDKhacsTemplateVo>> ICDKhacs([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetICDKhacs(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("MucDoDiUng")]
        public Task<string> MucDoDiUng(MucDoDiUngThuocVo mucDoVo)
        {
            var result = _yeuCauKhamBenhService.GetMucDoDiUng(mucDoVo);
            return result;
        }

        [HttpPost("GetNhanVienHoTongs")]
        public async Task<ActionResult<NhanVienHoTongTemplateVo>> GetNhanVienHoTongs(DropDownListRequestModel model)
        {
            var result = await _yeuCauKhamBenhService.GetNhanVienHoTongs(model);
            return Ok(result);
        }

        [HttpPost("GetDichVuKyThuatBenhViens")]
        public async Task<ActionResult<DichVuKyThuatTemplateVo>> GetDichVuKyThuatBenhViens(DropDownListRequestModel model)
        {
            var result = await _yeuCauKhamBenhService.GetDichVuKyThuatBenhViens(model);
            return Ok(result);
        }

        [HttpGet("ThoiGianDonThuoc")]
        public ActionResult<ICollection<string>> ThoiGianDonThuoc()
        {
            var lookup = _yeuCauKhamBenhService.GetThoiGianDonThuoc();
            return Ok(lookup);
        }

        [HttpPost("GetGhiChuDonThuocChiTietString")]
        public async Task<ActionResult<string>> GetGhiChuDonThuocChiTietString([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetGhiChuDonThuocChiTietString(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetLyDoNhapVienString")]
        public async Task<ActionResult<string>> GetLyDoNhapVienString([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetLyDoNhapVienString(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetGhiChuDonThuocChiTiet")]
        public ActionResult<ICollection<string>> GhiChuDonThuocChiTiet()
        {
            var lookup = _yeuCauKhamBenhService.GetGhiChuDonThuocChiTiet();
            return Ok(lookup);
        }

        [HttpPost("KhoaPhongNhapViens")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> KhoaPhongNhapViens([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetKhoaPhongNhapViens(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("BenhVienChuyenViens")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> BenhVienChuyenViens([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetBenhVienChuyenViens(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("LuuKetLuan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> LuuKetLuan(KetLuanKhamBenhViewModel KetLuanKhamBenhVM)
        {
            if (KetLuanKhamBenhVM.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(KetLuanKhamBenhVM.Id);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(KetLuanKhamBenhVM.Id);
                // trường hợp khám trong phòng khám
                KetLuanKhamBenhVM.NoiKetLuanId = _userAgentHelper.GetCurrentNoiLLamViecId();
            }

            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(KetLuanKhamBenhVM.YeuCauTiepNhanId.Value);
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(KetLuanKhamBenhVM.YeuCauTiepNhanId.Value, 
                a => a.Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhICDKhacs)
                             .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauDichVuKyThuats)
                             .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhLichSuTrangThais)
                             .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs)
                             .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonVTYTs)
                             .Include(x => x.YeuCauDichVuKyThuats)
                             .Include(x => x.YeuCauDuocPhamBenhViens)
                             .Include(x => x.YeuCauVatTuBenhViens)
                             .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));
            var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.Select(p => p).Where(p => p.Id == KetLuanKhamBenhVM.Id).FirstOrDefault();

            #region log 28/03/2022
            try
            {
                LogManager.GetCurrentClassLogger().Info(
                    $"LuuKetLuan phongBenhVienId{_userAgentHelper.GetCurrentNoiLLamViecId()}, yeuCauTiepNhanId{yeuCauTiepNhan?.Id ?? 0}, yeuCauKhamBenhId{yeuCauKhamBenh?.Id ?? 0}, currentUser{_userAgentHelper.GetCurrentUserId()}");
            }
            catch (Exception e)
            {

            }
            #endregion

            // trường hợp khám đoàn thì ko cần lưu tab chẩn đoán
            if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
            {
                var yeuCauKham = yeuCauTiepNhan.YeuCauKhamBenhs.First(p => p.Id == KetLuanKhamBenhVM.Id).ToModel<KetLuanKhamBenhViewModel>();
                return Ok(yeuCauKham);
            }

            if (KetLuanKhamBenhVM.YeuCauKhamBenhICDKhacs.Any())
            {
                var lstYeuCauKhamBenhICDKhacsId =
                KetLuanKhamBenhVM.YeuCauKhamBenhICDKhacs.Where(x => x.Id == 0)
                    .Select(x => x.ICDId).ToList();
                if (lstYeuCauKhamBenhICDKhacsId.Count() != lstYeuCauKhamBenhICDKhacsId.Distinct().Count())
                {
                    throw new ApiException(_localizationService.GetResource("KhamBenh.YeuCauKhamBenhICDKhac.IsExists"));
                }
            }
            if (yeuCauKhamBenh == null)
            {
                return NotFound();
            }
            if (KetLuanKhamBenhVM.CoInKeToa == true)
            {
                KetLuanKhamBenhVM.CoTaiKham = null;
                KetLuanKhamBenhVM.NgayTaiKham = null;
                KetLuanKhamBenhVM.GhiChuTaiKham = null;
                KetLuanKhamBenhVM.CoDieuTriNgoaiTru = null;
                KetLuanKhamBenhVM.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId = null;
                KetLuanKhamBenhVM.YeuCauDichVuKyThuat.SoLan = null;
                KetLuanKhamBenhVM.YeuCauDichVuKyThuat.ThoiDiemBatDauDieuTri = null;
            }
            KetLuanKhamBenhVM.BacSiKetLuanId = _userAgentHelper.GetCurrentUserId();
            // trường hợp khám tại chức năng khám bệnh đang khám
            if (KetLuanKhamBenhVM.NoiKetLuanId == null)
            {
                KetLuanKhamBenhVM.NoiKetLuanId = yeuCauKhamBenh.NoiDangKyId;
            }
            KetLuanKhamBenhVM.ToEntity(yeuCauKhamBenh);
            var yeuCauDichVuKyThuat = yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(dv => dv.YeuCauKhamBenhId == yeuCauKhamBenh.Id && dv.DieuTriNgoaiTru == true && dv.TrangThai
               != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).FirstOrDefault();
            var dangCoDieuTriNgoaiTru = yeuCauKhamBenh.CoDieuTriNgoaiTru == true && yeuCauDichVuKyThuat != null;

            if (KetLuanKhamBenhVM.CoKeToa == true)
            {
                if (!yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.Any() && !yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any())
                {
                    throw new ApiException(_localizationService.GetResource("KhamBenh.YeuCauKhamBenhDonThuocHoacVatTus.Required"));
                }
                foreach (var item in yeuCauKhamBenh.YeuCauKhamBenhDonThuocs)
                {
                    item.GhiChu = KetLuanKhamBenhVM.GhiChu;
                }
            }
            if (yeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                yeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangKham;
                yeuCauKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauKhamBenh.NoiThucHienId = yeuCauKhamBenh.NoiDangKyId; //_userAgentHelper.GetCurrentNoiLLamViecId();
                yeuCauKhamBenh.ThoiDiemThucHien = DateTime.Now;
                // lưu lịch sử
                var lichSuNew = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai,
                    MoTa = yeuCauKhamBenh.TrangThai.GetDescription(),
                };
                yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
            }
            #region Xử lý YeuCauDichVuKyThuat

            //kiem tra co cap nhat dieu tri ngoai tru
            //--them dtnt
            if (dangCoDieuTriNgoaiTru == false && KetLuanKhamBenhVM.CoDieuTriNgoaiTru == true)
            {
                //add
                KetLuanKhamBenhVM.YeuCauDichVuKyThuat.Id = 0;
                var entity = KetLuanKhamBenhVM.YeuCauDichVuKyThuat.ToEntity<YeuCauDichVuKyThuat>();
                entity.ThoiDiemDangKy = DateTime.Now;
                entity.ThoiDiemChiDinh = DateTime.Now;
                entity.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;
                entity.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                entity.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
                entity.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                entity.DieuTriNgoaiTru = true;
                yeuCauKhamBenh.CoDieuTriNgoaiTru = true;
                yeuCauKhamBenh.YeuCauDichVuKyThuats.Add(entity);
                await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            }
            //--huy dtnt
            else if (dangCoDieuTriNgoaiTru == true && KetLuanKhamBenhVM.CoDieuTriNgoaiTru == false)
            {
                //if ((yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yeuCauDichVuKyThuat.TaiKhoanBenhNhanChis.All(o => o.NhanVienThucHienId == (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong)) || yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                {
                    yeuCauKhamBenh.CoDieuTriNgoaiTru = null;
                    yeuCauDichVuKyThuat.WillDelete = true;
                    await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
                }
            }
            else if (yeuCauDichVuKyThuat != null && KetLuanKhamBenhVM.CoDieuTriNgoaiTru == false)
            {
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                {
                    yeuCauKhamBenh.CoDieuTriNgoaiTru = null;
                    yeuCauDichVuKyThuat.WillDelete = true;
                    await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
                }
            }
            //--edit dv dtng
            else if (dangCoDieuTriNgoaiTru == true && yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId != KetLuanKhamBenhVM.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId)
            {
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                {
                    yeuCauDichVuKyThuat.WillDelete = true;
                    yeuCauKhamBenh.CoDieuTriNgoaiTru = false;
                    await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
                    KetLuanKhamBenhVM.YeuCauDichVuKyThuat.Id = 0;
                    var entity = KetLuanKhamBenhVM.YeuCauDichVuKyThuat.ToEntity<YeuCauDichVuKyThuat>();
                    entity.ThoiDiemDangKy = DateTime.Now;
                    entity.ThoiDiemChiDinh = DateTime.Now;
                    entity.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;
                    entity.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                    entity.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
                    entity.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    entity.DieuTriNgoaiTru = true;
                    yeuCauKhamBenh.CoDieuTriNgoaiTru = true;
                    yeuCauKhamBenh.YeuCauDichVuKyThuats.Add(entity);
                    await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
                }
            }
            else if (dangCoDieuTriNgoaiTru == true && yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId == KetLuanKhamBenhVM.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId
                     && (yeuCauDichVuKyThuat.SoLan != KetLuanKhamBenhVM.YeuCauDichVuKyThuat.SoLan || yeuCauDichVuKyThuat.ThoiDiemBatDauDieuTri != KetLuanKhamBenhVM.YeuCauDichVuKyThuat.ThoiDiemBatDauDieuTri))
            {
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                {
                    yeuCauDichVuKyThuat.WillDelete = true;
                    yeuCauKhamBenh.CoDieuTriNgoaiTru = false;
                    await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
                    KetLuanKhamBenhVM.YeuCauDichVuKyThuat.Id = 0;
                    var entity = KetLuanKhamBenhVM.YeuCauDichVuKyThuat.ToEntity<YeuCauDichVuKyThuat>();
                    entity.ThoiDiemDangKy = DateTime.Now;
                    entity.ThoiDiemChiDinh = DateTime.Now;
                    entity.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;
                    entity.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                    entity.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
                    entity.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    entity.DieuTriNgoaiTru = true;
                    yeuCauKhamBenh.CoDieuTriNgoaiTru = true;
                    yeuCauKhamBenh.YeuCauDichVuKyThuats.Add(entity);
                    await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
                }
            }
            #endregion
            //else update yckb
            else
            {
                await _yeuCauTiepNhanService.UpdateAsync(yeuCauTiepNhan);
            }
            var result = yeuCauTiepNhan.YeuCauKhamBenhs.First(p => p.Id == KetLuanKhamBenhVM.Id).ToModel<KetLuanKhamBenhViewModel>();
            foreach (var item in yeuCauTiepNhan.YeuCauKhamBenhs.First(p => p.Id == KetLuanKhamBenhVM.Id).YeuCauKhamBenhICDKhacs)
            {
                var tenICD = await _yeuCauKhamBenhService.GetTenICD(item.ICDId);
                result.YeuCauKhamBenhICDKhacs.Add(new YeuCauKhamBenhICDKhacViewModel
                {
                    Id = item.Id,
                    YeuCauKhamBenhId = item.YeuCauKhamBenhId,
                    ICDId = item.ICDId,
                    GhiChu = item.GhiChu,
                    TenICD = tenICD
                });
            }

            if (!string.IsNullOrEmpty(KetLuanKhamBenhVM.LyDoNhapVien))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.LyDoNhapVien,
                    GhiChu = KetLuanKhamBenhVM.LyDoNhapVien
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            return Ok(result);
            //return NoContent();
        }

        #region CRUD DonThuocChiTiet
        [HttpPost("KiemTraThuocTrungBSKe")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> KiemTraThuocTrungBSKe(YeuCauKhamBenhDonThuocChiTietViewModel donThuocChiTietVM)
        {
            var kiemTraThuocTrungBSKe = new KiemTraThuocTrungBSKe
            {
                YeuCauTiepNhanId = donThuocChiTietVM.YeuCauTiepNhanId.Value,
                DuocPhamId = donThuocChiTietVM.DuocPhamId.Value
            };
            var kiemTra = await _yeuCauKhamBenhService.LaBacSiKeDon(kiemTraThuocTrungBSKe);
            return Ok(kiemTra);
        }

        [HttpPost("AddDonThuocChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> AddDonThuocChiTiet(YeuCauKhamBenhDonThuocChiTietViewModel donThuocChiTietVM)
        {
            if (donThuocChiTietVM.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            DonThuocChiTietVo donThuocChiTiet = new DonThuocChiTietVo
            {
                YeuCauKhamBenhId = donThuocChiTietVM.YeuCauKhamBenhId.Value,
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
                LieuDungTrenNgay = donThuocChiTietVM.LieuDungTrenNgayDisplay.ToFloatFromFraction(),
                SoLanTrenVien = donThuocChiTietVM.SoLanTrenVienDisplay.ToFloatFromFraction(),
                LoaiKhoThuoc = (LoaiKhoThuoc)donThuocChiTietVM.LoaiKhoThuoc,
                GhiChu = donThuocChiTietVM.GhiChu,
                SoThuTu = donThuocChiTietVM.SoThuTu
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
            var error = await _yeuCauKhamBenhService.ThemDonThuocChiTiet(donThuocChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);

            var mucTranChiPhi = await _yeuCauKhamBenhService.KiemTraMucTranChiPhi(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            if (!string.IsNullOrEmpty(mucTranChiPhi))
            {
                return Ok(mucTranChiPhi);
            }
            return NoContent();
        }

        [HttpPost("UpdateDonThuocChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> UpdateDonThuocChiTiet(YeuCauKhamBenhDonThuocChiTietViewModel donThuocChiTietVM)
        {
            //var checkDonThuocChiTiet = await _yeuCauKhamBenhService.CheckDonThuocChiTietExist(donThuocChiTietVM.Id);
            //if (checkDonThuocChiTiet == false)
            //{
            //    throw new ApiException(_localizationService.GetResource("DonThuoc.Deleted"));
            //}
            if (donThuocChiTietVM.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            DonThuocChiTietVo donThuocChiTiet = new DonThuocChiTietVo
            {
                DonThuocChiTietId = donThuocChiTietVM.Id,
                YeuCauKhamBenhId = donThuocChiTietVM.YeuCauKhamBenhId.Value,
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
                LieuDungTrenNgay = donThuocChiTietVM.LieuDungTrenNgayDisplay.ToFloatFromFraction(),
                SoLanTrenVien = donThuocChiTietVM.SoLanTrenVienDisplay.ToFloatFromFraction(),
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
            var error = await _yeuCauKhamBenhService.CapNhatDonThuocChiTiet(donThuocChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);

            var mucTranChiPhi = await _yeuCauKhamBenhService.KiemTraMucTranChiPhi(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            if (!string.IsNullOrEmpty(mucTranChiPhi))
            {
                return Ok(mucTranChiPhi);
            }
            return NoContent();
        }

        [HttpPost("DeleteDonThuocChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> DeleteDonThuocChiTiet(YeuCauKhamBenhDonThuocChiTietViewModel donThuocChiTietVM)
        {
            var checkDonThuocChiTiet = await _yeuCauKhamBenhService.CheckDonThuocChiTietExist(donThuocChiTietVM.Id);
            if (checkDonThuocChiTiet == false)
            {
                //("Thuốc này đã bị xóa trước đó.");
                throw new ApiException(_localizationService.GetResource("DonThuoc.Deleted"));
            }

            if (donThuocChiTietVM.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            DonThuocChiTietVo donThuocChiTiet = new DonThuocChiTietVo
            {
                YeuCauKhamBenhId = donThuocChiTietVM.YeuCauKhamBenhId.Value,
                DonThuocChiTietId = donThuocChiTietVM.Id,
            };
            var error = await _yeuCauKhamBenhService.XoaDonThuocChiTiet(donThuocChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }

        [HttpPost("TangHoacGiamSTTDonThuocChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> TangHoacGiamSTTDonThuocChiTiet(YeuCauKhamBenhDonThuocChiTietViewModel donThuocChiTietVM)
        {
            var checkDonThuocChiTiet = await _yeuCauKhamBenhService.CheckDonThuocChiTietExist(donThuocChiTietVM.Id);
            if (checkDonThuocChiTiet == false)
            {
                //("Thuốc này đã bị xóa trước đó.");
                throw new ApiException(_localizationService.GetResource("DonThuoc.Deleted"));
            }

            if (donThuocChiTietVM.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(donThuocChiTietVM.YeuCauKhamBenhId.Value);
            }
            DonThuocChiTietTangGiamSTTVo donThuocChiTiet = new DonThuocChiTietTangGiamSTTVo
            {
                YeuCauKhamBenhId = donThuocChiTietVM.YeuCauKhamBenhId.Value,
                DonThuocChiTietId = donThuocChiTietVM.Id,
                LaTangSTT = donThuocChiTietVM.LaTangSTT,
                LoaiDonThuoc  = donThuocChiTietVM.LoaiDonThuoc.GetValueOrDefault()
            };
            var error = await _yeuCauKhamBenhService.TangHoacGiamSTTDonThuocChiTiet(donThuocChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }
        #endregion

        #region Xóa toa thuốc nếu bỏ check có kê toa
        [HttpPost("XoaDonThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> XoaDonThuoc(YeuCauKhamBenhXoaDonThuoc yeuCauKhamBenhXoaDonThuocVM)
        {
            if (yeuCauKhamBenhXoaDonThuocVM.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(yeuCauKhamBenhXoaDonThuocVM.YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauKhamBenhXoaDonThuocVM.YeuCauKhamBenhId);
            }
            XoaDonThuocTheoYeuCauKhamBenhVo xoaDonThuocTheoYeuCauKhamBenh = new XoaDonThuocTheoYeuCauKhamBenhVo
            {
                YeuCauKhamBenhId = yeuCauKhamBenhXoaDonThuocVM.YeuCauKhamBenhId,
            };
            var error = await _yeuCauKhamBenhService.XoaDonThuocTheoYeuCauKhamBenh(xoaDonThuocTheoYeuCauKhamBenh);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }

        [HttpPost("KiemTraDonThuocDaXuatHayDaThanhToan")]
        public async Task<ActionResult> KiemTraDonThuocDaXuatHayDaThanhToan(YeuCauKhamBenhXoaDonThuoc yeuCauKhamBenhXoaDonThuocVM)
        {
            XoaDonThuocTheoYeuCauKhamBenhVo xoaDonThuocTheoYeuCauKhamBenh = new XoaDonThuocTheoYeuCauKhamBenhVo
            {
                YeuCauKhamBenhId = yeuCauKhamBenhXoaDonThuocVM.YeuCauKhamBenhId,
            };
            var error = await _yeuCauKhamBenhService.KiemTraDonThuocDaXuatHayDaThanhToan(xoaDonThuocTheoYeuCauKhamBenh);
            return Ok(error);
        }
        #endregion

        #region ThongTinDuocPhamVaVatTu

        [HttpPost("DuocPhams")]
        public async Task<ActionResult<ICollection<DuocPhamVaVatTuTemplate>>> GetListDuocPham(DropDownListRequestModel queryInfo, bool laNoiTruDuocPham = false)
        {
            //var lookup = await _yeuCauKhamBenhService.GetDuocPhamKeToaAsync(queryInfo);
            var lookup = await _yeuCauKhamBenhService.GetDuocPhamVaVatTuKeToaAsync(queryInfo, laNoiTruDuocPham);
            return Ok(lookup);
        }

        [HttpPost("InfoTonKhoDuocPham")]
        public ActionResult InfoTonKhoDuocPham(ThongTinThuocVo thongTinThuocVM)
        {
            var entity = _yeuCauKhamBenhService.GetDuocPhamInfoById(thongTinThuocVM);
            return Ok(entity);
        }

        [HttpPost("ThongTinTonKhoVatTu")]
        public ActionResult ThongTinTonKhoVatTu(ThongTinVatTuVo thongTinVatTuVo)
        {
            var entity = _yeuCauKhamBenhService.GetVatTuInfoById(thongTinVatTuVo);
            return Ok(entity);
        }
        #endregion

        #region CRUD VatTu
        [HttpPost("AddVatTuChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> AddVatTuChiTiet(YeuCauKhamBenhDonThuocChiTietViewModel vatTuVM)
        {
            if (vatTuVM.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(vatTuVM.YeuCauKhamBenhId.Value);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(vatTuVM.YeuCauKhamBenhId.Value);
            }
            VatTuChiTietVo vatTuChiTiet = new VatTuChiTietVo
            {
                YeuCauKhamBenhId = vatTuVM.YeuCauKhamBenhId.Value,
                VatTuId = vatTuVM.DuocPhamId.Value, // DuocPham va Vat Tu dùng chung 1 combobox
                SoLuong = vatTuVM.SoLuong.Value,
                GhiChu = vatTuVM.GhiChu,
            };
            if (!string.IsNullOrEmpty(vatTuVM.GhiChu))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.VatTu,
                    GhiChu = vatTuVM.GhiChu
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            var error = await _yeuCauKhamBenhService.ThemVatTuChiTiet(vatTuChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);

            var mucTranChiPhi = await _yeuCauKhamBenhService.KiemTraMucTranChiPhi(vatTuVM.YeuCauKhamBenhId.Value);
            if (!string.IsNullOrEmpty(mucTranChiPhi))
            {
                return Ok(mucTranChiPhi);
            }
            return NoContent();
        }

        [HttpPost("UpdateVatTuChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> UpdateVatTuChiTiet(YeuCauKhamBenhDonThuocChiTietViewModel vatTuVM)
        {
            //var checkVatTuChiTiet = await _yeuCauKhamBenhService.CheckVatTuChiTietExist(vatTuVM.Id);
            //if (checkVatTuChiTiet == false)
            //{
            //    throw new ApiException(_localizationService.GetResource("DonVTYT.VTYTDeleted"));
            //}
            if (vatTuVM.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(vatTuVM.YeuCauKhamBenhId.Value);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(vatTuVM.YeuCauKhamBenhId.Value);
            }

            VatTuChiTietVo vatTuChiTiet = new VatTuChiTietVo
            {
                DonVTYTChiTietId = vatTuVM.Id,
                YeuCauKhamBenhId = vatTuVM.YeuCauKhamBenhId.Value,
                VatTuId = vatTuVM.DuocPhamId.Value, // DuocPham va Vat Tu dùng chung 1 combobox
                SoLuong = vatTuVM.SoLuong.Value,
                GhiChu = vatTuVM.GhiChu,
            };
            if (!string.IsNullOrEmpty(vatTuVM.GhiChu))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.VatTu,
                    GhiChu = vatTuVM.GhiChu
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            var error = await _yeuCauKhamBenhService.CapNhatVatTuChiTiet(vatTuChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);

            var mucTranChiPhi = await _yeuCauKhamBenhService.KiemTraMucTranChiPhi(vatTuVM.YeuCauKhamBenhId.Value);
            if (!string.IsNullOrEmpty(mucTranChiPhi))
            {
                return Ok(mucTranChiPhi);
            }
            return NoContent();
        }

        [HttpPost("DeleteVatTuChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> DeleteVatTuChiTiet(YeuCauKhamBenhDonThuocChiTietViewModel vatTuVM)
        {
            var checkVatTuChiTiet = await _yeuCauKhamBenhService.CheckVatTuChiTietExist(vatTuVM.Id);
            if (checkVatTuChiTiet == false)
            {
                throw new ApiException(_localizationService.GetResource("DonVTYT.VTYTDeleted"));
            }

            if (vatTuVM.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(vatTuVM.YeuCauKhamBenhId.Value);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(vatTuVM.YeuCauKhamBenhId.Value);
            }
            VatTuChiTietVo vatTuChiTiet = new VatTuChiTietVo
            {
                DonVTYTChiTietId = vatTuVM.Id,
                YeuCauKhamBenhId = vatTuVM.YeuCauKhamBenhId.Value,
                VatTuId = vatTuVM.DuocPhamId.Value, // DuocPham va Vat Tu dùng chung 1 combobox
                SoLuong = vatTuVM.SoLuong.Value,
                GhiChu = vatTuVM.GhiChu,
            };
            var error = await _yeuCauKhamBenhService.XoaVatTuChiTiet(vatTuChiTiet);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }
        #endregion

        [HttpPost("GetDonGia")]
        public async Task<ActionResult<ThongTinDichVuKhamTiepTheo>> GetDonGia(ThongTinDichVuKhamTiepTheo thongTinDichVuKhamTiepTheo)
        {
            thongTinDichVuKhamTiepTheo.SoTienBHTTToanBo = await _cauhinhService.SoTienBHYTSeThanhToanToanBo();
            var entity = await _yeuCauKhamBenhService.GiaBenhVienAsync(thongTinDichVuKhamTiepTheo);
            return Ok(entity);
        }

        [HttpPost("KiemTraCoDonThuoc")]
        public async Task<ActionResult> KiemTraCoDonThuocs(long yeuCauKhamBenhId)
        {
            var res = await _yeuCauKhamBenhService.KiemTraCoDonThuoc(yeuCauKhamBenhId);
            return Ok(res);
        }

        [HttpPost("KiemTraCoDonThuocs2")]
        public async Task<ActionResult<bool>> KiemTraCoDonThuocs2(long yeuCauKhamBenhId)
        {
            var res = await _yeuCauKhamBenhService.KiemTraCoDonThuoc(yeuCauKhamBenhId);
            return Ok(res);
        }


        [HttpPost("KiemTraCoBHYT")]
        public async Task<ActionResult> KiemTraCoBHYT(long yeuTiepNhanId)
        {
            var res = await _yeuCauKhamBenhService.KiemTraCoBHYT(yeuTiepNhanId);
            return Ok(res);
        }

        [HttpPost("KiemTraDeChonLoaiThuoc")]
        public async Task<ActionResult<KiemTraThanhToan>> KiemTraDeChonLoaiThuoc(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            var res = await _yeuCauKhamBenhService.KiemTraDeChonLoaiThuoc(yeuCauTiepNhanId, yeuCauKhamBenhId);
            return Ok(res);
        }

        [HttpPost("InDonThuoc")]
        public ActionResult InDonThuoc(InToaThuocReOrder inToaThuoc)//InToaThuoc
        {
            var result = _yeuCauKhamBenhService.InDonThuocKhamBenh(inToaThuoc);
            return Ok(result);
        }

        [HttpPost("InVatTu")]
        public ActionResult InVatTu(InVatTuReOrder inVatTu)//InVatTu
        {
            var result = _yeuCauKhamBenhService.InVatTuKhamBenh(inVatTu);
            return Ok(result);
        }
        //================ TOA THUỐC MẪU
        #region gridToaThuocMau

        [HttpPost("GetDataForGridAsyncToaThuocMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncToaThuocMau([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncToaThuocMau(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncToaThuocMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncToaThuocMau([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridAsyncToaThuocMau(queryInfo);
            return Ok(gridData);
        }

        #endregion

        //================ TOA THUỐC MẪU CHI TIẾT

        #region gridToaThuocMauChiTietChild
        [HttpPost("GetDataForGridToaThuocMauChiTietChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public GridDataSource GetDataForGridToaThuocMauChiTietChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetDataForGridToaThuocMauChiTietChild(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPageForToaThuocMauChiTietChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public GridDataSource GetTotalPageForToaThuocMauChiTietChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetTotalPageForToaThuocMauChiTietChild(queryInfo);
            return gridData;
        }
        #endregion

        //================ LỊCH SỬ KÊ TOA

        #region gridLichSuKeToa

        [HttpPost("GetDataForGridAsyncLichSuKeToa")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncLichSuKeToa([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncLichSuKeToa(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncLichSuKeToa")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncLichSuKeToa([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridAsyncLichSuKeToa(queryInfo);
            return Ok(gridData);
        }

        #endregion

        //================ LỊCH SỬ KÊ TOA CHI TIẾT

        #region gridLichSuKeToaChild

        [HttpPost("GetDataForGridLichSuKeToaChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public GridDataSource GetDataForGridLichSuKeToaChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetDataForGridLichSuKeToaChild(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPageForLichSuKeToaChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public GridDataSource GetTotalPageForLichSuKeToaChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetTotalPageForLichSuKeToaChild(queryInfo);
            return gridData;
        }

        #endregion


        #region Xử lý sử dụng toa mẫu

        [HttpPost("ApDungToaMau")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> ApDungToaMauAsync(ApDungToaThuocMauVo toaMauVo)
        {
            var result = await _yeuCauKhamBenhService.ApDungToaMauAsync(toaMauVo);
            var mucTranChiPhi = await _yeuCauKhamBenhService.KiemTraMucTranChiPhi(toaMauVo.YeuCauKhamBenhHienTaiId);
            if (!string.IsNullOrEmpty(mucTranChiPhi))
            {
                result.MucTranChiPhi = mucTranChiPhi;
            }
            return Ok(result);
        }
        #endregion

        #region Xử lý sử dụng lịch sử kê toa

        [HttpPost("ApDungToaMauLichSuKeToa")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public async Task<ActionResult> ApDungToaMauLichSuKeToaAsync(ApDungToaThuocLichSuKhamBenhVo toaThuocVo)
        {
            var result = await _yeuCauKhamBenhService.ApDungToaThuocLichSuKhamBenhAsync(toaThuocVo);
            var mucTranChiPhi = await _yeuCauKhamBenhService.KiemTraMucTranChiPhi(toaThuocVo.YeuCauKhamBenhHienTaiId);
            if (!string.IsNullOrEmpty(mucTranChiPhi))
            {
                result.MucTranChiPhi = mucTranChiPhi;
            }
            return Ok(result);
        }

        [HttpPost("ApDungToaThuocConfirmAsync")]
        public async Task<ActionResult> ApDungToaThuocConfirmAsync(ApDungToaThuocConfirmVo apDungToaThuocVM)
        {
            var error = await _yeuCauKhamBenhService.ApDungToaThuocConfirmAsync(apDungToaThuocVM);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }
        #endregion

        #region grid ChanDoanBacSiKhac

        [HttpPost("GetChanDoanBacSiKhacDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetChanDoanBacSiKhacDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetChanDoanBacSiKhacDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetChanDoanBacSiKhacTotalPageForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetChanDoanBacSiKhacTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetChanDoanBacSiKhacTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region detail ChanDoanBacSiKhac
        [HttpPost("GetICDKhacDataForGridAsyncDetail")]
        public async Task<ActionResult<GridDataSource>> GetICDKhacDataForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetICDKhacDataForGridAsyncDetail(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetICDKhacTotalPageForGridAsyncDetail")]
        public async Task<ActionResult<GridDataSource>> GetICDKhacTotalPageForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetICDKhacTotalPageForGridAsyncDetail(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region grid BacSiKhacKeDon 

        [HttpPost("GetDonThuocBacSiKhacDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetDonThuocBacSiKhacDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDonThuocBacSiKhacDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDonThuocBacSiKhacTotalPageForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetDonThuocBacSiKhacTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDonThuocBacSiKhacTotalPageForGridAsync(queryInfo);
            return Ok(gridData);

        }
        #endregion

        #region  detail BacSiKhacKeDon 

        [HttpPost("GetDonThuocBacSiKhacDataForGridAsyncDetail")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public GridDataSource GetDonThuocBacSiKhacDataForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetDonThuocBacSiKhacDataForGridAsyncDetail(queryInfo);
            return gridData;
        }

        [HttpPost("GetDonThuocBacSiKhacTotalPageForGridAsyncDetail")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham)]
        public GridDataSource GetDonThuocBacSiKhacTotalPageForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetDonThuocBacSiKhacTotalPageForGridAsyncDetail(queryInfo);
            return gridData;
        }
        #endregion

        [HttpPost("FormatNumber")]
        public string FormatNumber(double? inputNumber)
        {
            var result = _yeuCauKhamBenhService.FormatNumber(inputNumber);
            return result;
        }

        [HttpPost("KiemTraDonThuocChiTietThanhToan")]
        public async Task<ActionResult> KiemTraDonThuocChiTietThanhToan(KiemTraXoaDonThuocThanhToan donThuocThanhToanVo)
        {
            var checkThuocChiTiet = await _yeuCauKhamBenhService.CheckDonThuocChiTietExist(donThuocThanhToanVo.DonThuocChiTietId.Value);
            if (checkThuocChiTiet == false)
            {
                throw new ApiException(_localizationService.GetResource("DonThuoc.Deleted"));
            }

            if (donThuocThanhToanVo.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(donThuocThanhToanVo.YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(donThuocThanhToanVo.YeuCauKhamBenhId);
            }
            var error = await _yeuCauKhamBenhService.KiemTraDonThuocChiTietThanhToan(donThuocThanhToanVo.DonThuocChiTietId.Value);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }

        #region VatTuYTGrid
        [HttpPost("GetVatTuYTDataForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetVatTuYTDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetVatTuYTDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetVatTuYTTotalPageForGridAsync")]
        public async Task<ActionResult<GridDataSource>> GetVatTuYTTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetVatTuYTTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("KiemTraVatTuChiTietThanhToan")]
        public async Task<ActionResult> KiemTraVatTuChiTietThanhToan(KiemTraXoaVatTuThanhToan vatTuThanhToanVo)
        {
            var checkVatTuChiTiet = await _yeuCauKhamBenhService.CheckVatTuChiTietExist(vatTuThanhToanVo.YeuCauKhamBenhDonVTYTChiTietId.Value);
            if (checkVatTuChiTiet == false)
            {
                throw new ApiException(_localizationService.GetResource("DonVTYT.VTYTDeleted"));
            }

            if (vatTuThanhToanVo.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(vatTuThanhToanVo.YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(vatTuThanhToanVo.YeuCauKhamBenhId);
            }

            var error = await _yeuCauKhamBenhService.KiemTraVatTuChiTietThanhToan(vatTuThanhToanVo.YeuCauKhamBenhDonVTYTChiTietId.Value);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }
        #endregion

        #region ThemDuocPhamKhongBHYT
        [HttpPost("ThemDuocPhamKhongBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<DuocPhamKhamBenhViewModel>> ThemDuocPhamKhongBHYT([FromBody] DuocPhamKhamBenhViewModel thuocBenhVienViewModel)
        {
            if (!await _thuocBenhVienService.CheckDuongDungAsync(thuocBenhVienViewModel.DuongDungId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DuongDung.NotExists"), (int)HttpStatusCode.BadRequest);
            if (!await _thuocBenhVienService.CheckDVTAsync(thuocBenhVienViewModel.DonViTinhId ?? 0))
                throw new ApiException(_localizationService.GetResource("DuocPham.DVT.NotExists"), (int)HttpStatusCode.BadRequest);

            thuocBenhVienViewModel.MaHoatChat = "Chưa nhập";
            thuocBenhVienViewModel.HoatChat = "Chưa nhập";
            thuocBenhVienViewModel.LoaiThuocHoacHoatChat = Enums.LoaiThuocHoacHoatChat.ThuocTanDuoc;
            thuocBenhVienViewModel.SoDangKy = await _yeuCauKhamBenhService.GetSoDangKyDuocPhamNgoaiBv();
            var thuocBenhVien = thuocBenhVienViewModel.ToEntity<DuocPham>();
            _thuocBenhVienService.Add(thuocBenhVien);

            DonThuocChiTietVo donThuocChiTiet = new DonThuocChiTietVo
            {
                YeuCauKhamBenhId = thuocBenhVienViewModel.YeuCauKhamBenhId,
                DuocPhamId = thuocBenhVien.Id,
                SoLuong = thuocBenhVienViewModel.SoLuong.Value,
                SoNgayDung = thuocBenhVienViewModel.SoNgayDung,
                ThoiGianDungSang = thuocBenhVienViewModel.ThoiGianDungSang,
                ThoiGianDungTrua = thuocBenhVienViewModel.ThoiGianDungTrua,
                ThoiGianDungChieu = thuocBenhVienViewModel.ThoiGianDungChieu,
                ThoiGianDungToi = thuocBenhVienViewModel.ThoiGianDungToi,
                DungSang = thuocBenhVienViewModel.SangDisplay.ToFloatFromFraction(),
                DungTrua = thuocBenhVienViewModel.TruaDisplay.ToFloatFromFraction(),
                DungChieu = thuocBenhVienViewModel.ChieuDisplay.ToFloatFromFraction(),
                DungToi = thuocBenhVienViewModel.ToiDisplay.ToFloatFromFraction(),
                LoaiKhoThuoc = LoaiKhoThuoc.ThuocNgoaiBenhVien,
                GhiChu = thuocBenhVienViewModel.GhiChu,
            };
            if (!string.IsNullOrEmpty(thuocBenhVienViewModel.GhiChu))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.Thuoc,
                    GhiChu = thuocBenhVienViewModel.GhiChu
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            var error = await _yeuCauKhamBenhService.ThemDonThuocChiTiet(donThuocChiTiet);
            return NoContent();
        }
        #endregion

        [HttpPost("GetPhuongPhapKyThuatDieuTri")]
        public string GetPhuongPhapKyThuatDieuTri(long yeuCauKhamBenhId)
        {
            var result = _yeuCauKhamBenhService.GetPhuongPhapKyThuatDieuTri(yeuCauKhamBenhId);
            return result;
        }
    }
}
