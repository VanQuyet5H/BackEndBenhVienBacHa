using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {

        #region tạm đóng

        //[HttpPost("GetDataForGridDanhSachPhaThuocTruyen")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetDataForGridDanhSachPhaThuocTruyen([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachPhaThuocTruyen(queryInfo);
        //    return gridData;
        //}

        //[HttpPost("GetTotalPageForGridDanhSachPhaThuocTruyen")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetTotalPageForGridDanhSachPhaThuocTruyen([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachPhaThuocTruyen(queryInfo);
        //    return gridData;
        //}
        #endregion


        #region CRUD PhaThuocTruyenBenhVien
        [HttpPost("ThemPhaThuocTruyen")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemPhaThuocTruyen(DieuTriNoiTruPhieuDieuTriPhaThuocTruyenViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);
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
                s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien)
                .Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.NoiTruChiDinhPhaThuocTruyen));

            PhaThuocTruyenBenhVienVo donThuocChiTiet = new PhaThuocTruyenBenhVienVo
            {
                Id = thuocViewModel.Id,
                PhieuDieuTriHienTaiId = thuocViewModel.PhieuDieuTriHienTaiId.Value,
                YeuCauTiepNhanId = thuocViewModel.YeuCauTiepNhanId,
                SoLanTrenNgay = thuocViewModel.SoLanTrenNgay,
                GhiChu = thuocViewModel.GhiChu,
                ThoiGianBatDauTruyen = thuocViewModel.ThoiGianBatDauTruyen,
                CachGioTruyen = thuocViewModel.CachGioTruyen,
                LaDichTruyen = thuocViewModel.LaDichTruyen,
                KhongTinhPhi = thuocViewModel.KhongTinhPhi,
                TheTich = thuocViewModel.TheTich,
                LieuDungTrenNgay = thuocViewModel.LieuDungTrenNgay,
                CachGioDungThuoc = thuocViewModel.CachGioDungThuoc,
                DonViTocDoTruyen = thuocViewModel.DonViTocDoTruyen,
                TocDoTruyen = thuocViewModel.TocDoTruyen,
                NoiTruChiDinhDuocPhams = thuocViewModel.NoiTruChiDinhDuocPhams
            };

            //Xử lý thêm yêu cầu dược phẩm bệnh viện
            await _dieuTriNoiTruService.ThemPhaThuocTruyen(donThuocChiTiet, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }


        [HttpPost("CapNhatPhaThuocTruyen")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatPhaThuocTruyen(DieuTriNoiTruPhieuDieuTriPhaThuocTruyenViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(thuocViewModel.YeuCauTiepNhanId,
                   s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.NhapKhoDuocPhamChiTiets)
                       .Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.XuatKhoDuocPhamChiTiet).ThenInclude(z => z.XuatKhoDuocPhamChiTietViTris).ThenInclude(z => z.NhapKhoDuocPhamChiTiet)
                       .Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.NhapKhoDuocPhamChiTiets).ThenInclude(z => z.NhapKhoDuocPhams)
                       .Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.NoiTruChiDinhPhaThuocTiem)
                       .Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.NoiTruChiDinhPhaThuocTruyen)

            );
            PhaThuocTiemBenhVienVo donThuocChiTiet = new PhaThuocTiemBenhVienVo
            {
                Id = thuocViewModel.Id,
                PhieuDieuTriHienTaiId = thuocViewModel.PhieuDieuTriHienTaiId.Value,
                YeuCauTiepNhanId = thuocViewModel.YeuCauTiepNhanId,
                SoLanTrenNgay = thuocViewModel.SoLanTrenNgay,
                GhiChu = thuocViewModel.GhiChu,
                ThoiGianBatDauTiem = thuocViewModel.ThoiGianBatDauTiem,
                CachGioTruyen = thuocViewModel.CachGioTruyen,
                LaDichTruyen = thuocViewModel.LaDichTruyen,
                KhongTinhPhi = thuocViewModel.KhongTinhPhi,
                TheTich = thuocViewModel.TheTich,
                TocDoTruyen = thuocViewModel.TocDoTruyen,
                ThoiGianBatDauTruyen = thuocViewModel.ThoiGianBatDauTruyen,
                LieuDungTrenNgay = thuocViewModel.LieuDungTrenNgay,
                CachGioDungThuoc = thuocViewModel.CachGioDungThuoc,
                NoiTruChiDinhDuocPhams = thuocViewModel.NoiTruChiDinhDuocPhams
            };
            await _dieuTriNoiTruService.CapNhatPhaThuocTiem(donThuocChiTiet, yeuCauTiepNhan);
            await _yeuCauTiepNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [HttpPost("XoaPhaThuocTruyen")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaPhaThuocTruyen(DieuTriNoiTruPhieuDieuTriPhaThuocTruyenViewModel thuocViewModel)
        {

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(thuocViewModel.YeuCauTiepNhanId,
                                s => s.Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.XuatKhoDuocPhamChiTiet).ThenInclude(z => z.XuatKhoDuocPhamChiTietViTris).ThenInclude(z => z.NhapKhoDuocPhamChiTiet)
                                .Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.YeuCauLinhDuocPhamChiTiets)
                                );
            await _dieuTriNoiTruService.XoaThuoc(thuocViewModel.Id, yeuCauTiepNhan);
            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }


        #endregion

    }
}
