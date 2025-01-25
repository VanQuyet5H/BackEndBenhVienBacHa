using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        //[HttpPost("GetDataForGridDanhSachThuocKhoLe")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetDataForGridDanhSachThuocKhoLe([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachThuocKhoLe(queryInfo);
        //    return gridData;
        //}

        //[HttpPost("GetTotalPageForGridDanhSachThuocKhoLe")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetTotalPageForGridDanhSachThuocKhoLe([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachThuocKhoLe(queryInfo);
        //    return gridData;
        //}

        [HttpPost("GetDataForGridDanhSachThuocKhoTong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetDataForGridDanhSachThuocKhoTong([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachThuocKhoTong(queryInfo);
            return gridData;
        }
        #region BVHD-3959
        [HttpPost("ApDungThoiGianDienBienThuocLoaiThuongTrongBVs")]
        public async Task<ActionResult> ApDungThoiGianDienBienThuocLoaiThuongTrongBVs([FromBody] ApDungThoiGianDienBienThuocVo apDungThoiGianDienBienVo)
        {
            var chiDinhDuocPhamIds = apDungThoiGianDienBienVo.DataGridDichVuChons.Where(o => o.ChecBoxItem).Select(o => o.Id).ToList();
            if (!chiDinhDuocPhamIds.Any())
            {
                throw new ApiException("Vui lòng chọn dược phẩm để áp dụng");
            }
            _dieuTriNoiTruService.ApDungThoiGianDienBienChiDinhDuocPham(chiDinhDuocPhamIds, apDungThoiGianDienBienVo.ThoiGianDienBien);
            return Ok(true);
        }
        #endregion
        [HttpPost("GetTotalPageForGridDanhSachThuocKhoTong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetTotalPageForGridDanhSachThuocKhoTong([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachThuocKhoTong(queryInfo);
            return gridData;
        }

        [HttpPost("GetDataForGridDanhSachThuocNgoai")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetDataForGridDanhSachThuocNgoai([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachThuocNgoai(queryInfo);
            return gridData;
        }

        [HttpPost("GetDataForGridDanhSachDichTruyenNgoai")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetDataForGridDanhSachDichTruyenNgoai([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachDichTruyenNgoai(queryInfo);
            return gridData;
        }

        [HttpPost("GetKho")]
        public async Task<ActionResult<ICollection<KhoLookupItemVo>>> GetKho(DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetKhoCurrentUser(model);
            return Ok(lookup);
        }

        [HttpPost("ThongTinDuocPham")]
        public ActionResult ThongTinDuocPham(ThongTinThuocDieuTriVo thongTinThuocVM)
        {
            var entity = _dieuTriNoiTruService.GetDuocPhamInfoById(thongTinThuocVM);
            return Ok(entity);
        }

        #region CRUD ThuocBenhVien
        [HttpPost("ThemThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemThuoc(DieuTriNoiTruPhieuDieuTriThuocViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            if (!string.IsNullOrEmpty(thuocViewModel.GhiChu))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.Thuoc,
                    GhiChu = thuocViewModel.GhiChu
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(thuocViewModel.YeuCauTiepNhanId, s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris));
            ThuocBenhVienVo donThuocChiTiet = new ThuocBenhVienVo
            {
                Id = thuocViewModel.Id,
                PhieuDieuTriHienTaiId = thuocViewModel.PhieuDieuTriHienTaiId.Value,
                KhoId = thuocViewModel.KhoId.Value,
                LaDuocPhamBHYT = thuocViewModel.LaDuocPhamBHYT,
                YeuCauTiepNhanId = thuocViewModel.YeuCauTiepNhanId,
                DuocPhamBenhVienId = thuocViewModel.DuocPhamBenhVienId.Value,
                SoLuong = thuocViewModel.SoLuong.Value,
                SoLanDungTrongNgay = thuocViewModel.SoLanDungTrongNgay,
                ThoiGianDungSang = thuocViewModel.ThoiGianDungSang,
                ThoiGianDungTrua = thuocViewModel.ThoiGianDungTrua,
                ThoiGianDungChieu = thuocViewModel.ThoiGianDungChieu,
                ThoiGianDungToi = thuocViewModel.ThoiGianDungToi,
                DungSang = thuocViewModel.DungSang.ToFloatFromFraction(),
                DungTrua = thuocViewModel.DungTrua.ToFloatFromFraction(),
                DungChieu = thuocViewModel.DungChieu.ToFloatFromFraction(),
                DungToi = thuocViewModel.DungToi.ToFloatFromFraction(),
                GhiChu = thuocViewModel.GhiChu,
                TocDoTruyen = thuocViewModel.TocDoTruyen,
                DonViTocDoTruyen = thuocViewModel.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = thuocViewModel.ThoiGianBatDauTruyen,
                CachGioTruyenDich = thuocViewModel.CachGioTruyenDich,
                LaDichTruyen = thuocViewModel.LaDichTruyen,
                KhongTinhPhi = thuocViewModel.KhongTinhPhi,
                LoaiKho = thuocViewModel.LoaiKho,
                TheTich = thuocViewModel.TheTich,
                SoLanTrenVien = thuocViewModel.SoLanTrenVien,
                LieuDungTrenNgay = thuocViewModel.LieuDungTrenNgay,
                CachGioDungThuoc = thuocViewModel.CachGioDungThuoc,
                SoThuTu = thuocViewModel.SoThuTu
            };

            //Xử lý thêm yêu cầu dược phẩm bệnh viện
            await _dieuTriNoiTruService.ThemThuoc(donThuocChiTiet, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            //await _dieuTriNoiTruService.XuLySoThuTu(donThuocChiTiet, yeuCauTiepNhan);
            return NoContent();
        }


        [HttpPost("CapNhatThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatThuoc(DieuTriNoiTruPhieuDieuTriThuocViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            if (!string.IsNullOrEmpty(thuocViewModel.GhiChu))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.Thuoc,
                    GhiChu = thuocViewModel.GhiChu
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(thuocViewModel.YeuCauTiepNhanId,
                    s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.NhapKhoDuocPhamChiTiets)
                    .ThenInclude(z => z.NhapKhoDuocPhams)
                        .Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.XuatKhoDuocPhamChiTiet).ThenInclude(z => z.XuatKhoDuocPhamChiTietViTris).ThenInclude(z => z.NhapKhoDuocPhamChiTiet).ThenInclude(z => z.NhapKhoDuocPhams)
            );
            ThuocBenhVienVo donThuocChiTiet = new ThuocBenhVienVo
            {
                Id = thuocViewModel.Id,
                PhieuDieuTriHienTaiId = thuocViewModel.PhieuDieuTriHienTaiId.Value,
                KhoId = thuocViewModel.KhoId.Value,
                LaDuocPhamBHYT = thuocViewModel.LaDuocPhamBHYT,
                YeuCauTiepNhanId = thuocViewModel.YeuCauTiepNhanId,
                DuocPhamBenhVienId = thuocViewModel.DuocPhamBenhVienId.Value,
                SoLuong = thuocViewModel.SoLuong.Value,
                SoLanDungTrongNgay = thuocViewModel.SoLanDungTrongNgay,
                ThoiGianDungSang = thuocViewModel.ThoiGianDungSang,
                ThoiGianDungTrua = thuocViewModel.ThoiGianDungTrua,
                ThoiGianDungChieu = thuocViewModel.ThoiGianDungChieu,
                ThoiGianDungToi = thuocViewModel.ThoiGianDungToi,
                DungSang = thuocViewModel.DungSang.ToFloatFromFraction(),
                DungTrua = thuocViewModel.DungTrua.ToFloatFromFraction(),
                DungChieu = thuocViewModel.DungChieu.ToFloatFromFraction(),
                DungToi = thuocViewModel.DungToi.ToFloatFromFraction(),
                GhiChu = thuocViewModel.GhiChu,
                TocDoTruyen = thuocViewModel.TocDoTruyen,
                DonViTocDoTruyen = thuocViewModel.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = thuocViewModel.ThoiGianBatDauTruyen,
                CachGioTruyenDich = thuocViewModel.CachGioTruyenDich,
                LaDichTruyen = thuocViewModel.LaDichTruyen,
                KhongTinhPhi = thuocViewModel.KhongTinhPhi,
                LoaiKho = thuocViewModel.LoaiKho,
                TheTich = thuocViewModel.TheTich,
                SoLanTrenVien = thuocViewModel.SoLanTrenVien,
                CachGioDungThuoc = thuocViewModel.CachGioDungThuoc,
                LieuDungTrenNgay = thuocViewModel.LieuDungTrenNgay,
                SoLanTrenMui = thuocViewModel.SoLanTrenMui,
                CachGioTiem = thuocViewModel.CachGioTiem,
            };

            if (thuocViewModel.LaTuTruc)
            {
                var error = await _dieuTriNoiTruService.CapNhatThuocChoTuTruc(donThuocChiTiet, yeuCauTiepNhan);
                if (!string.IsNullOrEmpty(error))
                    throw new ApiException(error);
                await XoaThuoc(thuocViewModel);
                await ThemThuoc(thuocViewModel);
            }
            else
            {
                //Xử lý thêm yêu cầu dược phẩm bệnh viện
                await _dieuTriNoiTruService.CapNhatThuoc(donThuocChiTiet, yeuCauTiepNhan);

                //Gọi hàm chung
                await _yeuCauTiepNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhan);
            }

            return NoContent();
        }


        [HttpPost("XoaThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaThuoc(DieuTriNoiTruPhieuDieuTriThuocViewModel thuocViewModel)
        {

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(thuocViewModel.YeuCauTiepNhanId,
                s => s.Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.XuatKhoDuocPhamChiTiet).ThenInclude(z => z.XuatKhoDuocPhamChiTietViTris).ThenInclude(z => z.NhapKhoDuocPhamChiTiet)
                .Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.YeuCauLinhDuocPhamChiTiets)
                );
            await _dieuTriNoiTruService.XoaThuoc(thuocViewModel.Id, yeuCauTiepNhan);
            await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }


        [HttpPost("SapXepTatCaThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SapXepTatCaThuoc(SapXepThuoc sapXepThuocVo)
        {

            //await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(sapXepThuocVo.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(sapXepThuocVo.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(sapXepThuocVo.YeuCauTiepNhanId, s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien));
            await _dieuTriNoiTruService.SapXepThuoc(sapXepThuocVo);
            //await _dieuTriNoiTruService.XuLySoThuTu(donThuocChiTiet, yeuCauTiepNhan);
            return NoContent();
        }


        [HttpPost("TangHoacGiamSTTDonThuocChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> TangHoacGiamSTTDonThuocChiTiet(ThuocBenhVienTangGiamSTTVo thuocViewModel)
        {
            //await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(thuocViewModel.YeuCauTiepNhanId, s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien));
            var error = await _dieuTriNoiTruService.TangHoacGiamSTTDonThuocChiTiet(thuocViewModel);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            return NoContent();
        }
        #endregion

        #region CRUDThuocNgoaiBenhVien
        [HttpPost("ThemThuocNgoaiBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemThuocNgoaiBenhVien(DieuTriNoiTruPhieuDieuTriThuocViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            ThuocBenhVienVo donThuocChiTiet = new ThuocBenhVienVo
            {
                Id = 0,
                PhieuDieuTriHienTaiId = thuocViewModel.PhieuDieuTriHienTaiId.Value,
                YeuCauTiepNhanId = thuocViewModel.YeuCauTiepNhanId,
                DuocPhamBenhVienId = thuocViewModel.DuocPhamBenhVienId.Value,
                SoLuong = thuocViewModel.SoLuong.Value,
                SoLanDungTrongNgay = thuocViewModel.SoLanDungTrongNgay,
                ThoiGianDungSang = thuocViewModel.ThoiGianDungSang,
                ThoiGianDungTrua = thuocViewModel.ThoiGianDungTrua,
                ThoiGianDungChieu = thuocViewModel.ThoiGianDungChieu,
                ThoiGianDungToi = thuocViewModel.ThoiGianDungToi,
                DungSang = thuocViewModel.DungSang.ToFloatFromFraction(),
                DungTrua = thuocViewModel.DungTrua.ToFloatFromFraction(),
                DungChieu = thuocViewModel.DungChieu.ToFloatFromFraction(),
                DungToi = thuocViewModel.DungToi.ToFloatFromFraction(),
                GhiChu = thuocViewModel.GhiChu,
                TocDoTruyen = thuocViewModel.TocDoTruyen,
                DonViTocDoTruyen = thuocViewModel.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = thuocViewModel.ThoiGianBatDauTruyen,
                CachGioTruyenDich = thuocViewModel.CachGioTruyenDich,
                LaDichTruyen = thuocViewModel.LaDichTruyen,
                TheTich = thuocViewModel.TheTich,

            };
            if (!string.IsNullOrEmpty(thuocViewModel.GhiChu))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.Thuoc,
                    GhiChu = thuocViewModel.GhiChu
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            await _dieuTriNoiTruService.ThemThuocNgoaiBenhVien(donThuocChiTiet);
            return NoContent();
        }

        [HttpPost("CapNhatThuocNgoaiBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatThuocNgoaiBenhVien(DieuTriNoiTruPhieuDieuTriThuocViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);

            ThuocBenhVienVo donThuocChiTiet = new ThuocBenhVienVo
            {
                Id = thuocViewModel.Id,
                PhieuDieuTriHienTaiId = thuocViewModel.PhieuDieuTriHienTaiId.Value,
                YeuCauTiepNhanId = thuocViewModel.YeuCauTiepNhanId,
                DuocPhamBenhVienId = thuocViewModel.DuocPhamBenhVienId.Value,
                SoLuong = thuocViewModel.SoLuong.Value,
                SoLanDungTrongNgay = thuocViewModel.SoLanDungTrongNgay,
                ThoiGianDungSang = thuocViewModel.ThoiGianDungSang,
                ThoiGianDungTrua = thuocViewModel.ThoiGianDungTrua,
                ThoiGianDungChieu = thuocViewModel.ThoiGianDungChieu,
                ThoiGianDungToi = thuocViewModel.ThoiGianDungToi,
                DungSang = thuocViewModel.DungSang.ToFloatFromFraction(),
                DungTrua = thuocViewModel.DungTrua.ToFloatFromFraction(),
                DungChieu = thuocViewModel.DungChieu.ToFloatFromFraction(),
                DungToi = thuocViewModel.DungToi.ToFloatFromFraction(),
                GhiChu = thuocViewModel.GhiChu,
                TocDoTruyen = thuocViewModel.TocDoTruyen,
                DonViTocDoTruyen = thuocViewModel.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = thuocViewModel.ThoiGianBatDauTruyen,
                CachGioTruyenDich = thuocViewModel.CachGioTruyenDich,
                LaDichTruyen = thuocViewModel.LaDichTruyen,
                TheTich = thuocViewModel.TheTich,

            };
            if (!string.IsNullOrEmpty(thuocViewModel.GhiChu))
            {
                var inputStringStoredVo = new InputStringStoredVo
                {
                    Loai = Enums.InputStringStoredKey.Thuoc,
                    GhiChu = thuocViewModel.GhiChu
                };
                await _yeuCauKhamBenhService.ThemGhiChuDonThuocHoacVatTuChiTiet(inputStringStoredVo);
            }
            await _dieuTriNoiTruService.CapNhatThuocNgoaiBenhVien(donThuocChiTiet);

            return NoContent();
        }

        [HttpPost("XoaThuocNgoaiBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaThuocNgoaiBenhVien(DieuTriNoiTruPhieuDieuTriThuocViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            await _dieuTriNoiTruService.XoaThuocNgoaiBenhVien(thuocViewModel.Id);
            return NoContent();
        }
        #endregion

        [HttpPost("CapNhatKhongTinhPhi")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatKhongTinhPhi(CapNhatKhongTinhPhi capNhatKhongTinhPhiVo)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(capNhatKhongTinhPhiVo.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(capNhatKhongTinhPhiVo.YeuCauTiepNhanId);
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(capNhatKhongTinhPhiVo.YeuCauTiepNhanId,
                s => s.Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens)
                      .Include(z => z.YeuCauVatTuBenhViens));
            await _dieuTriNoiTruService.CapNhatKhongTinhPhi(capNhatKhongTinhPhiVo, yeuCauTiepNhan);
            await _yeuCauTiepNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }

        #region InPhieuCongKhaiThuocVatTu
        [HttpPost("InPhieuCongKhaiThuocVatTu")]
        public ActionResult InPhieuCongKhaiThuocVatTu(InPhieuCongKhaiThuocVatTuReOrder inToaThuoc)//InPhieuCongKhaiThuocVatTu
        {
            var result = _dieuTriNoiTruService.InPhieuCongKhaiThuocVatTu(inToaThuoc);
            return Ok(result);
        }

        #endregion

        #region InPhieuThucHienThuocVatTu
        [HttpPost("InPhieuThucHienThuocVatTu")]
        public ActionResult InPhieuThucHienThuocVatTu(InPhieuThucHienThuocVatTu inToaThuoc)//InPhieuThucHienThuocVatTu
        {
            var result = _dieuTriNoiTruService.InPhieuThucHienThuocVatTu(inToaThuoc);
            return Ok(result);
        }

        #endregion

        [HttpPost("HoanTraDuocPhamTuBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> HoanTraDuocPhamTuBenhNhan(YeuCauTraDuocPhamTuBenhNhanChiTietViewModel yeuCauViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(yeuCauViewModel.YeuCauTiepNhanId);
            
            if (yeuCauViewModel.NgayYeuCau == null)
            {
                throw new ApiException("Ngày yêu cầu nhập.");
            }
            if (yeuCauViewModel.NhanVienYeuCauId == null)
            {
                throw new ApiException("Người trả yêu cầu nhập.");
            }
            if (yeuCauViewModel.NgayYeuCau != null && yeuCauViewModel.NgayYeuCau.Value.Date > DateTime.Now.Date)
            {
                throw new ApiException("Ngày trả phải trước hoặc là ngày hiện tại.");
            }
            foreach (var item in yeuCauViewModel.YeuCauDuocPhamBenhViens)
            {
                if (item.SoLuongTra == null)
                {
                    throw new ApiException("Số lượng cầu nhập.");
                }
            }
            var model = new YeuCauTraDuocPhamTuBenhNhanChiTietVo
            {
                Id = yeuCauViewModel.Id,
                //YeuCauDuocPhamBenhVienId = yeuCauViewModel.YeuCauDuocPhamBenhVienId,
                LaDichTruyen = yeuCauViewModel.LaDichTruyen,
                DuocPhamBenhVienId = yeuCauViewModel.DuocPhamBenhVienId,
                LaDuocPhamBHYT = yeuCauViewModel.LaDuocPhamBHYT,
                NgayYeuCau = yeuCauViewModel.NgayYeuCau.Value,
                NhanVienYeuCauId = yeuCauViewModel.NhanVienYeuCauId.Value,
                YeuCauDuocPhamBenhViens = yeuCauViewModel.YeuCauDuocPhamBenhViens
            };
            await _dieuTriNoiTruService.HoanTraDuocPhamTuBenhNhan(model);
            return NoContent();
        }

        [HttpPost("GetThongTinHoanTraThuocVo")]
        //[ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> GetThongTinHoanTraThuocVo(HoanTraThuocVo hoanTraThuocVo)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(hoanTraThuocVo.YeuCauTiepNhanId);
            var res = _dieuTriNoiTruService.GetThongTinHoanTraThuocVo(hoanTraThuocVo);
            return Ok(res);
        }

        //[HttpPost("GetThongTinHoanTraThuocVo")]
        ////[ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public async Task<ActionResult> GetThongTinHoanTraThuocVo(HoanTraThuocVo hoanTraThuocVo)
        //{
        //    await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(hoanTraThuocVo.YeuCauTiepNhanId);
        //    var res = await _dieuTriNoiTruService.GetThongTinHoanTraThuocVo(hoanTraThuocVo);
        //    return Ok(res);
        //}



        [HttpPost("GetDataForGridDanhSachThuocHoanTra")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDanhSachThuocHoanTra([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridDanhSachThuocHoanTra(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDanhSachThuocHoanTra")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDanhSachThuocHoanTra([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridDanhSachThuocHoanTra(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("InNoiTruPhieuDieuTriTuVanThuoc")]
        public ActionResult InNoiTruPhieuDieuTriTuVanThuoc(InNoiTruPhieuDieuTriTuVanThuoc inNoiTruPhieuDieuTriTuVanThuoc)//InNoiTruPhieuDieuTriTuVanThuoc
        {
            var result = _dieuTriNoiTruService.InNoiTruPhieuDieuTriTuVanThuoc(inNoiTruPhieuDieuTriTuVanThuoc);
            return Ok(result);
        }

        [HttpPost("GetNgayNhapVien")]
        public async Task<ActionResult> GetNgayNhapVien(long yeuCauTiepNhanId)
        {
            var res = await _dieuTriNoiTruService.GetNgayNhapVien(yeuCauTiepNhanId);
            return Ok(res);
        }

        [HttpPost("GetNoiTruKhoaChuyenDen")]
        public async Task<ActionResult> GetNoiTruKhoaChuyenDen(long yeuCauTiepNhanId)
        {
            var res = await _dieuTriNoiTruService.GetNoiTruKhoaChuyenDen(yeuCauTiepNhanId);
            return Ok(res);
        }

        [HttpPost("TaoNgayDieuTriVaApDungDonThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> TaoNgayDieuTriVaApDungDonThuoc(NoiTruDonThuocTongHopVo model)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(model.YeuCauTiepNhanId);
            // tạo ngày điều trị
            //model.Dates = await _dieuTriNoiTruService.IsRemoveExistsDate(model.YeuCauTiepNhanId, model.Dates, model.KhoaId);
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(model.YeuCauTiepNhanId,
                    s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien)
                          .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruThamKhamChanDoanKemTheos)
                          .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruPhieuDieuTriTuVanThuocs));
            await _dieuTriNoiTruService.CreateNewDateDieuTri(yeuCauTiepNhan, model.KhoaId, model.Dates.OrderBy(x => x.Date).ToList());
            // áp dụng đơn thuốc cho các ngày sau
            var result = await _dieuTriNoiTruService.ApDungDonThuocChoCacNgayDieuTriAsync(model, yeuCauTiepNhan);
            if (!result.ThanhCong)
            {
                return Ok(result);
            }
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            return Ok("OK");
        }

        [HttpPost("TaoNgayDieuTriVaApDungDonThuocConfirm")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> TaoNgayDieuTriVaApDungDonThuocConfirm(NoiTruDonThuocTongHopVo model)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(model.YeuCauTiepNhanId);
            // tạo ngày điều trị
            //model.Dates = await _dieuTriNoiTruService.IsRemoveExistsDate(model.YeuCauTiepNhanId, model.Dates, model.KhoaId);
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(model.YeuCauTiepNhanId,
                    s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien)
                          .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruThamKhamChanDoanKemTheos)
                          .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruPhieuDieuTriTuVanThuocs));
            await _dieuTriNoiTruService.CreateNewDateDieuTri(yeuCauTiepNhan, model.KhoaId, model.Dates.OrderBy(x => x.Date).ToList());
            // áp dụng đơn thuốc cho các ngày sau
            var error = await _dieuTriNoiTruService.ApDungDonThuocChoCacNgayDieuTriConfirmAsync(model, yeuCauTiepNhan);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }

        
    }
}
