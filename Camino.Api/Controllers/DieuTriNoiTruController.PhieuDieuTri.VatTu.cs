using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
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
        [HttpPost("GetDataForGridDanhSachVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetDataForGridDanhSachVatTu([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachVatTu(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPageForGridDanhSachVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetTotalPageForGridDanhSachVatTu([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachVatTu(queryInfo);
            return gridData;
        }

        [HttpPost("GetDataForGridDanhSachVatTuKhoTong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetDataForGridDanhSachVatTuKhoTong([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetDataForGridDanhSachVatTuKhoTong(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPageForGridDanhSachVatTuKhoTong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public GridDataSource GetTotalPageForGridDanhSachVatTuKhoTong([FromBody]QueryInfo queryInfo)
        {
            var gridData = _dieuTriNoiTruService.GetTotalPageForGridDanhSachVatTuKhoTong(queryInfo);
            return gridData;
        }

        [HttpPost("KiemTraCoDonVT")]
        public async Task<ActionResult> KiemTraCoDonVT(long noiTruPhieuDieuTriId)
        {
            var entity = await _dieuTriNoiTruService.KiemTraCoDonVT(noiTruPhieuDieuTriId);
            return Ok(entity);
        }

        [HttpPost("GetDichVuKyThuatDaThem")]
        public async Task<ActionResult<DichVuKyThuatDaThemLookupItem>> GetDichVuKyThuatDaThem([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dieuTriNoiTruService.GetDichVuKyThuatDaThem(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetKhoVatTu")]
        public async Task<ActionResult<ICollection<KhoLookupItemVo>>> GetKhoVatTuCurrentUser(DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetKhoVatTuCurrentUser(model);
            return Ok(lookup);
        }

        [HttpPost("ThongTinVatTu")]
        public ActionResult ThongTinVatTu(ThongTinChiTietVatTuTonKhoPDT thongTinVatTuVo)
        {
            var entity = _dieuTriNoiTruService.GetVatTuInfoById(thongTinVatTuVo);
            return Ok(entity);
        }

        #region CRUD VatTuBenhVien
        [HttpPost("ThemVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemVatTu(DieuTriNoiTruPhieuDieuTriVatTuViewModel vatTuViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(vatTuViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(vatTuViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = _yeuCauTiepNhanService
                .GetById(vatTuViewModel.YeuCauTiepNhanId, s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris));
            VatTuBenhVienVo donVatTuChiTiet = new VatTuBenhVienVo
            {
                Id = vatTuViewModel.Id,
                PhieuDieuTriHienTaiId = vatTuViewModel.PhieuDieuTriHienTaiId.GetValueOrDefault(),
                YeuCauDichVuKyThuatId = vatTuViewModel.YeuCauDichVuKyThuatId,
                KhoId = vatTuViewModel.KhoId.Value,
                LaVatTuBHYT = vatTuViewModel.LaVatTuBHYT,
                YeuCauTiepNhanId = vatTuViewModel.YeuCauTiepNhanId,
                VatTuBenhVienId = vatTuViewModel.VatTuBenhVienId.Value,
                SoLuong = vatTuViewModel.SoLuong.Value,
                KhongTinhPhi = vatTuViewModel.KhongTinhPhi
            };

            //Xử lý thêm yêu cầu dược phẩm bệnh viện
            await _dieuTriNoiTruService.ThemVatTu(donVatTuChiTiet, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }


        [HttpPost("CapNhatVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatVatTu(DieuTriNoiTruPhieuDieuTriVatTuViewModel vatTuViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(vatTuViewModel.YeuCauTiepNhanId);

            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(vatTuViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(vatTuViewModel.YeuCauTiepNhanId, s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.XuatKhoVatTuChiTiet).ThenInclude(z => z.XuatKhoVatTuChiTietViTris));

            //var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(vatTuViewModel.YeuCauTiepNhanId,
            //s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.VatTuBenhVien).ThenInclude(z => z.NhapKhoVatTuChiTiets)
            //.ThenInclude(z => z.NhapKhoVatTu)
            //    .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.VatTuBenhVien).ThenInclude(z => z.XuatKhoVatTuChiTiets).ThenInclude(z => z.XuatKhoVatTuChiTietViTris).ThenInclude(z => z.NhapKhoVatTuChiTiet).ThenInclude(z => z.NhapKhoVatTu));

            VatTuBenhVienVo donVatTuChiTiet = new VatTuBenhVienVo
            {
                Id = vatTuViewModel.Id,
                Ids = vatTuViewModel.Ids,
                PhieuDieuTriHienTaiId = vatTuViewModel.PhieuDieuTriHienTaiId.GetValueOrDefault(),
                KhoId = vatTuViewModel.KhoId.Value,
                LaVatTuBHYT = vatTuViewModel.LaVatTuBHYT,
                YeuCauTiepNhanId = vatTuViewModel.YeuCauTiepNhanId,
                VatTuBenhVienId = vatTuViewModel.VatTuBenhVienId.Value,
                SoLuong = vatTuViewModel.SoLuong.Value,
                KhongTinhPhi = vatTuViewModel.KhongTinhPhi
            };

            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(vatTuViewModel.YeuCauTiepNhanId, s => s.Include(z => z.YeuCauVatTuBenhViens));

            var error = await _dieuTriNoiTruService.CapNhatVatTuChoTuTruc(donVatTuChiTiet, yeuCauTiepNhan);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            await XoaVatTu(vatTuViewModel);

            await ThemVatTu(vatTuViewModel);

            //if (vatTuViewModel.LaTuTruc)
            //{
            //    var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(vatTuViewModel.YeuCauTiepNhanId, s => s.Include(z => z.YeuCauVatTuBenhViens));

            //    var error = await _dieuTriNoiTruService.CapNhatVatTuChoTuTruc(donVatTuChiTiet, yeuCauTiepNhan);
            //    if (!string.IsNullOrEmpty(error))
            //        throw new ApiException(error);
            //    await XoaVatTu(vatTuViewModel);

            //    await ThemVatTu(vatTuViewModel);
            //}
            //else
            //{
            //    var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(vatTuViewModel.YeuCauTiepNhanId,
            //s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.VatTuBenhVien).ThenInclude(z => z.NhapKhoVatTuChiTiets)
            //.ThenInclude(z => z.NhapKhoVatTu)
            //    .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.VatTuBenhVien).ThenInclude(z => z.XuatKhoVatTuChiTiets).ThenInclude(z => z.XuatKhoVatTuChiTietViTris).ThenInclude(z => z.NhapKhoVatTuChiTiet).ThenInclude(z => z.NhapKhoVatTu));
            //    //Xử lý thêm yêu cầu dược phẩm bệnh viện
            //    await _dieuTriNoiTruService.CapNhatVatTu(donVatTuChiTiet, yeuCauTiepNhan);


            //    //Gọi hàm chung
            //    await _yeuCauTiepNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhan);
            //}

            return NoContent();
        }


        [HttpPost("XoaVatTu")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaVatTu(DieuTriNoiTruPhieuDieuTriVatTuViewModel vatTuViewModel)
        {

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(vatTuViewModel.YeuCauTiepNhanId);

            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(vatTuViewModel.YeuCauTiepNhanId, s => s.Include(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.XuatKhoVatTuChiTiet).ThenInclude(z => z.XuatKhoVatTuChiTietViTris).ThenInclude(z => z.NhapKhoVatTuChiTiet)
            .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.YeuCauLinhVatTuChiTiets)
            );

            //var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(vatTuViewModel.YeuCauTiepNhanId, s => s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.XuatKhoVatTuChiTiet).ThenInclude(z => z.XuatKhoVatTuChiTietViTris).ThenInclude(z => z.NhapKhoVatTuChiTiet)
            //.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.YeuCauLinhVatTuChiTiets)
            //);

            await _dieuTriNoiTruService.XoaVatTu(vatTuViewModel.Ids, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }
        #endregion

        [HttpPost("GetThongTinHoanTraVatTu")]
        //[ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> GetThongTinHoanTraVatTu(HoanTraVTVo hoanTraVTVo)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(hoanTraVTVo.YeuCauTiepNhanId);
            var res = await _dieuTriNoiTruService.GetThongTinHoanTraVatTu(hoanTraVTVo);
            return Ok(res);
        }

        [HttpPost("GetDataForGridDanhSachVatTuHoanTra")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDanhSachVatTuHoanTra([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridDanhSachVatTuHoanTra(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridDanhSachVatTuHoanTra")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridDanhSachVatTuHoanTra([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridDanhSachVatTuHoanTra(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("HoanTraVatTuTuBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> HoanTraVatTuTuBenhNhan(YeuCauTraVatTuTuBenhNhanChiTietViewModel yeuCauViewModel)
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
            foreach (var item in yeuCauViewModel.YeuCauVatTuBenhViens)
            {
                if (item.SoLuongTra == null)
                {
                    throw new ApiException("Số lượng cầu nhập.");
                }
            }
            var model = new YeuCauTraVatTuTuBenhNhanChiTietVo
            {
                Id = yeuCauViewModel.Id,
                VatTuBenhVienId = yeuCauViewModel.VatTuBenhVienId,
                LaVatTuBHYT = yeuCauViewModel.LaVatTuBHYT,
                NgayYeuCau = yeuCauViewModel.NgayYeuCau.Value,
                NhanVienYeuCauId = yeuCauViewModel.NhanVienYeuCauId.Value,
                SoLuong = yeuCauViewModel.SoLuong,
                SoLuongDaTra = yeuCauViewModel.SoLuongDaTra,
                SoLuongTra = yeuCauViewModel.SoLuongTra,
                YeuCauVatTuBenhViens = yeuCauViewModel.YeuCauVatTuBenhViens
            };
            await _dieuTriNoiTruService.HoanTraVatTuTuBenhNhan(model);
            return NoContent();
        }

        [HttpPost("TaoNgayDieuTriVaApDungDonVTYT")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> TaoNgayDieuTriVaApDungDonVTYT(NoiTruDonVTYTTongHopVo model)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(model.YeuCauTiepNhanId);
            // tạo ngày điều trị
            //model.Dates = await _dieuTriNoiTruService.IsRemoveExistsDate(model.YeuCauTiepNhanId, model.Dates, model.KhoaId);
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(model.YeuCauTiepNhanId,
                s =>
                s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.YeuCauVatTuBenhViens)
                         .ThenInclude(z => z.XuatKhoVatTuChiTiet).ThenInclude(z => z.XuatKhoVatTuChiTietViTris)
                 .Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.VatTuBenhVien)
                 .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruThamKhamChanDoanKemTheos)
                );
            await _dieuTriNoiTruService.CreateNewDateDieuTri(yeuCauTiepNhan, model.KhoaId, model.Dates.OrderBy(x => x.Date).ToList());
            // áp dụng đơn vtyt cho các ngày sau
            var result = await _dieuTriNoiTruService.ApDungDonVTYTChoCacNgayDieuTriAsync(model, yeuCauTiepNhan);
            if (!result.ThanhCong)
            {
                return Ok(result);
            }
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            return Ok("OK");
        }

        [HttpPost("TaoNgayDieuTriVaApDungDonVTYTConfirm")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> TaoNgayDieuTriVaApDungDonVTYTConfirm(NoiTruDonVTYTTongHopVo model)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(model.YeuCauTiepNhanId);
            // tạo ngày điều trị
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(model.YeuCauTiepNhanId,
               s =>
               s.Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.YeuCauVatTuBenhViens)
                        .ThenInclude(z => z.XuatKhoVatTuChiTiet).ThenInclude(z => z.XuatKhoVatTuChiTietViTris)
                .Include(z => z.NoiTruBenhAn).ThenInclude(z => z.NoiTruPhieuDieuTris).ThenInclude(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.VatTuBenhVien)
                .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris).ThenInclude(p => p.NoiTruThamKhamChanDoanKemTheos)
               );
            await _dieuTriNoiTruService.CreateNewDateDieuTri(yeuCauTiepNhan, model.KhoaId, model.Dates.OrderBy(x => x.Date).ToList());
            // áp dụng đơn vtyt cho các ngày sau
            var error = await _dieuTriNoiTruService.ApDungDonVTYTChoCacNgayDieuTriConfirmAsync(model, yeuCauTiepNhan);
            if (!string.IsNullOrEmpty(error))
                throw new ApiException(error);
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }
    }
}
