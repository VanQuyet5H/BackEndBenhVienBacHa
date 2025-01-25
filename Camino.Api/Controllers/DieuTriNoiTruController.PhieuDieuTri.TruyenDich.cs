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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {

        //[HttpPost("GetDataForGridDanhSachTruyenDich")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetDataForGridDanhSachTruyenDich([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachTruyenDich(queryInfo);
        //    return gridData;
        //}

        //[HttpPost("GetTotalPageForGridDanhSachTruyenDich")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        //public GridDataSource GetTotalPageForGridDanhSachTruyenDich([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachTruyenDich(queryInfo);
        //    return gridData;
        //}

        [HttpPost("GetDataForGridDanhSachTruyenDichKhoTong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetDataForGridDanhSachTruyenDichKhoTong([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachTruyenDichKhoTong(queryInfo);
            return gridData;
        }
        #region BVHD-3959
        [HttpPost("ApDungThoiGianDienBienThuocLoaiDichTruyenTrongBVs")]
        public async Task<ActionResult> ApDungThoiGianDienBienThuocLoaiDichTruyenTrongBVs([FromBody] ApDungThoiGianDienBienThuocDichTruyenVo apDungThoiGianDienBienVo)
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

        [HttpPost("GetTotalPageForGridDanhSachTruyenDichKhoTong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetTotalPageForGridDanhSachTruyenDichKhoTong([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachTruyenDichKhoTong(queryInfo);
            return gridData;
        }

        [HttpPost("KiemTraCoDonTruyenDich")]
        public async Task<ActionResult> KiemTraCoDonTruyenDich(long noiTruPhieuDieuTriId)
        {
            var entity = await _dieuTriNoiTruService.KiemTraCoDonTruyenDich(noiTruPhieuDieuTriId);
            return Ok(entity);
        }

        [HttpPost("GetDonViTocDoTruyen")]
        public ActionResult<ICollection<LookupItemVo>> GetDonViTocDoTruyen(DropDownListRequestModel model)
        {
            var lookup = _dieuTriNoiTruService.GetDonViTocDoTruyen(model);
            return Ok(lookup);
        }
        // Các func này  hiện tại ko sử dụng
        #region CRUD ThuocTruyenDichBenhVien
        [HttpPost("ThemThuocTruyenDich")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemThuocTruyenDich(DieuTriNoiTruPhieuDieuTriThuocTruyenDichViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);

            ThuocTruyenDichBenhVienVo donThuocChiTiet = new ThuocTruyenDichBenhVienVo
            {
                Id = thuocViewModel.Id,
                KhoId = thuocViewModel.KhoId.Value,
                LaDuocPhamBHYT = thuocViewModel.LaDuocPhamBHYT,
                YeuCauTiepNhanId = thuocViewModel.YeuCauTiepNhanId,
                DuocPhamBenhVienId = thuocViewModel.DuocPhamBenhVienId.Value,
                SoLuong = thuocViewModel.SoLuong.Value,
                SoLanDungTrongNgay = thuocViewModel.SoLanDungTrongNgay,
                GhiChu = thuocViewModel.GhiChu,
                TocDoTruyen = thuocViewModel.TocDoTruyen,
                DonViTocDoTruyen = thuocViewModel.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = thuocViewModel.ThoiGianBatDauTruyen,
                CachGioTruyenDich = thuocViewModel.CachGioTruyenDich,
                KhongTinhPhi = thuocViewModel.KhongTinhPhi
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

            //Xử lý thêm yêu cầu dược phẩm bệnh viện
            await _dieuTriNoiTruService.ThemThuocTruyenDich(donThuocChiTiet, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);


            return NoContent();
        }


        [HttpPost("CapNhatThuocTruyenDich")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatThuocTruyenDich(DieuTriNoiTruPhieuDieuTriThuocTruyenDichViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);

            var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);

            ThuocTruyenDichBenhVienVo donThuocChiTiet = new ThuocTruyenDichBenhVienVo
            {
                Id = thuocViewModel.Id,
                KhoId = thuocViewModel.KhoId.Value,
                LaDuocPhamBHYT = thuocViewModel.LaDuocPhamBHYT,
                YeuCauTiepNhanId = thuocViewModel.YeuCauTiepNhanId,
                DuocPhamBenhVienId = thuocViewModel.DuocPhamBenhVienId.Value,
                SoLuong = thuocViewModel.SoLuong.Value,
                SoLanDungTrongNgay = thuocViewModel.SoLanDungTrongNgay,
                GhiChu = thuocViewModel.GhiChu,
                TocDoTruyen = thuocViewModel.TocDoTruyen,
                DonViTocDoTruyen = thuocViewModel.DonViTocDoTruyen,
                ThoiGianBatDauTruyen = thuocViewModel.ThoiGianBatDauTruyen,
                CachGioTruyenDich = thuocViewModel.CachGioTruyenDich,
                KhongTinhPhi = thuocViewModel.KhongTinhPhi
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


            //Xử lý thêm yêu cầu dược phẩm bệnh viện
            await _dieuTriNoiTruService.CapNhatThuocTruyenDich(donThuocChiTiet, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }


        [HttpPost("XoaThuocTruyenDich")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaThuocTruyenDich(DieuTriNoiTruPhieuDieuTriThuocTruyenDichViewModel thuocViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thuocViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(thuocViewModel.YeuCauTiepNhanId);

            await _dieuTriNoiTruService.XoaThuoc(thuocViewModel.Id, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }
        #endregion

        
    }
}
