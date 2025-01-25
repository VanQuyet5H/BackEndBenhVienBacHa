using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
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

        //[HttpPost("GetDataForGridDanhSachPhaThuocTiem")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetDataForGridDanhSachPhaThuocTiem([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachPhaThuocTiem(queryInfo);
        //    return gridData;
        //}

        //[HttpPost("GetTotalPageForGridDanhSachPhaThuocTiem")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetTotalPageForGridDanhSachPhaThuocTiem([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachPhaThuocTiem(queryInfo);
        //    return gridData;
        //}

        //[HttpPost("GetDataForGridDanhSachPhaThuocTiemNgoai")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetDataForGridDanhSachPhaThuocTiemNgoai([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachPhaThuocTiemNgoai(queryInfo);
        //    return gridData;
        //}

        //[HttpPost("GetTotalPageForGridDanhSachPhaThuocTiemNgoai")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetTotalPageForGridDanhSachPhaThuocTiemNgoai([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachPhaThuocTiemNgoai(queryInfo);
        //    return gridData;
        //}
        #endregion

        #region CRUD PhaThuocTiemBenhVien
        [HttpPost("ThemPhaThuocTiem")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemPhaThuocTiem(DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel thuocViewModel)
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

            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(thuocViewModel.YeuCauTiepNhanId, 
                s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien)
                .Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.NoiTruChiDinhPhaThuocTiem));

            PhaThuocTiemBenhVienVo donThuocChiTiet = new PhaThuocTiemBenhVienVo
            {
                Id = thuocViewModel.Id,
                PhieuDieuTriHienTaiId = thuocViewModel.PhieuDieuTriHienTaiId.Value,
                YeuCauTiepNhanId = thuocViewModel.YeuCauTiepNhanId,
                SoLanTrenNgay = thuocViewModel.SoLanTrenNgay,
                GhiChu = thuocViewModel.GhiChu,
                ThoiGianBatDauTiem = thuocViewModel.ThoiGianBatDauTiem,
                CachGioTiem = thuocViewModel.CachGioTiem,
                LaDichTruyen = thuocViewModel.LaDichTruyen,
                KhongTinhPhi = thuocViewModel.KhongTinhPhi,
                TheTich = thuocViewModel.TheTich,
                SoLanTrenMui = thuocViewModel.SoLanTrenMui,
                LieuDungTrenNgay = thuocViewModel.LieuDungTrenNgay,
                CachGioDungThuoc = thuocViewModel.CachGioDungThuoc,
                SoThuTu = thuocViewModel.SoThuTu,
                NoiTruChiDinhDuocPhams = thuocViewModel.NoiTruChiDinhDuocPhams
            };

            //Xử lý thêm yêu cầu dược phẩm bệnh viện
            await _dieuTriNoiTruService.ThemPhaThuocTiem(donThuocChiTiet, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }


        [HttpPost("CapNhatPhaThuocTiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatPhaThuocTiem(DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);

            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(thuocViewModel.YeuCauTiepNhanId,
                   s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.NhapKhoDuocPhamChiTiets).ThenInclude(z => z.NhapKhoDuocPhams)
                       .Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.XuatKhoDuocPhamChiTiet).ThenInclude(z => z.XuatKhoDuocPhamChiTietViTris).ThenInclude(z => z.NhapKhoDuocPhamChiTiet)
                        .Include(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.NhapKhoDuocPhamChiTiets).ThenInclude(z => z.NhapKhoDuocPhams)
                       .Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.NoiTruChiDinhDuocPhams).ThenInclude(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.NhapKhoDuocPhamChiTiets).ThenInclude(z => z.NhapKhoDuocPhams)
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
                CachGioTiem = thuocViewModel.CachGioTiem,
                LaDichTruyen = thuocViewModel.LaDichTruyen,
                KhongTinhPhi = thuocViewModel.KhongTinhPhi,
                TheTich = thuocViewModel.TheTich,
                SoLanTrenMui = thuocViewModel.SoLanTrenMui,
                LieuDungTrenNgay = thuocViewModel.LieuDungTrenNgay,
                CachGioDungThuoc = thuocViewModel.CachGioDungThuoc,
                NoiTruChiDinhDuocPhams = thuocViewModel.NoiTruChiDinhDuocPhams
            };
            await _dieuTriNoiTruService.CapNhatPhaThuocTiem(donThuocChiTiet, yeuCauTiepNhan);
            await _yeuCauTiepNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [HttpPost("XoaPhaThuocTiem")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaPhaThuocTiem(DieuTriNoiTruPhieuDieuTriPhaThuocTiemViewModel thuocViewModel)
        {

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);
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

        [HttpPost("CapNhatKhongTinhPhiTiemHoacTruyen")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatKhongTinhPhiTiemHoacTruyen(CapNhatKhongTinhPhiTiem capNhatKhongTinhPhiVo)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(capNhatKhongTinhPhiVo.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(capNhatKhongTinhPhiVo.YeuCauTiepNhanId);
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(capNhatKhongTinhPhiVo.YeuCauTiepNhanId,
                x => x.Include(a => a.NoiTruChiDinhDuocPhams)
                    .ThenInclude(a => a.YeuCauDuocPhamBenhViens));
            await _dieuTriNoiTruService.CapNhatKhongTinhPhiTiem(capNhatKhongTinhPhiVo, yeuCauTiepNhan);
            await _yeuCauTiepNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }

    }
}
