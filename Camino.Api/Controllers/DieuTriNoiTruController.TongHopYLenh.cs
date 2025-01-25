using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.TongHopYLenh;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        #region Grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridTongHopYLenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TongHopYLenh, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridTongHopYLenhAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridTongHopYLenhAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridTongHopYLenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TongHopYLenh, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridTongHopYLenhAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridTongHopYLenhAsync(queryInfo);
            return Ok(gridData);
        }


        #endregion

        #region Get data
        [HttpPost("GetTongHopYLenhThongTinHanhChinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TongHopYLenh, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<TongHopYLenhThongTinHanhChinhVo>> GetTongHopYLenhThongTinHanhChinhAsync(ChiTietYLenhQueryInfoVo queryInfo)
        {
            var thongTinHanhChinh = await _dieuTriNoiTruService.GetTongHopYLenhThongTinHanhChinhAsync(queryInfo);
            if (thongTinHanhChinh == null)
            {
                throw new ApiException(_localizationService.GetResource("TongHopYLenh.PhieuDieuTri.NotExists"));
            }

            #region BVHD-3941
            if (thongTinHanhChinh.CoBaoHiemTuNhan == true)
            {
                thongTinHanhChinh.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(thongTinHanhChinh.YeuCauTiepNhanId);
            }
            #endregion

            return thongTinHanhChinh;
        }

        [HttpPost("GetThongTinChiTietYLenhPhieuDieuTri")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TongHopYLenh, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<TongHopYLenhPhieuDieuTriVo>> GetThongTinChiTietYLenhPhieuDieuTriAsync(ChiTietYLenhQueryInfoVo queryInfo)
        {
            var thongTinPhieuDieuTri = await _dieuTriNoiTruService.GetThongTinChiTietYLenhPhieuDieuTriAsync(queryInfo);
            //if (thongTinPhieuDieuTri == null)
            //{
            //    throw new ApiException(_localizationService.GetResource("TongHopYLenh.PhieuDieuTri.NotExists"));
            //}
            return thongTinPhieuDieuTri;
        }

        [HttpGet("KiemTraPhieuDieuTriNgayHienTaiTheoYeuCauTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TongHopYLenh, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<long> KiemTraPhieuDieuTriNgayHienTaiTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId,
                x => x.Include(a => a.NoiTruBenhAn).ThenInclude(y => y.NoiTruPhieuDieuTris));
            if (yeuCauTiepNhan == null || yeuCauTiepNhan.NoiTruBenhAn == null || yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.All(x => x.NgayDieuTri.Date != DateTime.Now.Date))
            {
                throw new ApiException(_localizationService.GetResource("TongHopYLenh.PhieuDieuTri.NotExists"));
            }

            return yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.First(x => x.NgayDieuTri.Date == DateTime.Now.Date).Id;
        }
        #endregion

        #region Kiểm tra data
        [HttpPost("KiemTraPhieuDieuTriNoiTruByNgayDieuTri")]
        public async Task<ActionResult> KiemTraPhieuDieuTriNoiTruByNgayDieuTriAsync([FromBody]TongHopYLenhKiemTraPhieuDieuTriTheoNgayViewModel thongTin)
        {
            await _dieuTriNoiTruService.KiemTraPhieuDieuTriNoiTruByNgayDieuTriAsync(thongTin.NoiTruBenhAnId, thongTin.NgayDieuTri, thongTin.YeuCauTiepNhanId);
            //if (result == null)
            //{
            //    throw new ApiException(_localizationService.GetResource("TongHopYLenh.PhieuDieuTri.NotExists"));
            //}

            return Ok();
        }

        [HttpPost("KiemTraThemYLenh")]
        public async Task<ActionResult<TongHopYLenhDienBienVo>> KiemTraThemYLenhAsync([FromBody]TongHopYLenhThemMoiViewModel thongTinYLenh)
        {
            var gioYLenhTemp = thongTinYLenh.GioYLenh.Value - ((thongTinYLenh.GioYLenh.Value % 3600) % 60);
            var newDienBien = new TongHopYLenhDienBienVo()
            {
                DienBien = thongTinYLenh.DienBien,
                GioYLenh = gioYLenhTemp
            };

            DateTime.TryParseExact(thongTinYLenh.NgayYLenh, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var ngayYLenh);
            if (thongTinYLenh.GioThucHien != null)
            {
                thongTinYLenh.ThoiDiemXacNhanThucHien = ngayYLenh.AddSeconds(thongTinYLenh.GioThucHien.Value);// + TimeSpan.FromSeconds(thongTinYLenh.GioThucHien.Value);

            }

            var nhanVien = await _userService.GetCurrentUser();
            var newChiTiet = new TongHopYLenhDienBienChiTietVo()
            {
                //NoiTruPhieuDieuTriId = thongTinYLenh.PhieuDieuTriId,
                NoiTruBenhAnId = thongTinYLenh.NoiTruBenhAnId,
                MoTaYLenh = thongTinYLenh.MoTaYLenh,
                GioYLenh = gioYLenhTemp,
                NhanVienChiDinhId = nhanVien.Id,
                NhanVienChiDinhDisplay = nhanVien.HoTen,
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                NhanVienXacNhanThucHienId = thongTinYLenh.NhanVienXacNhanThucHienId,
                NhanVienXacNhanThucHienDisplay = thongTinYLenh.NhanVienXacNhanThucHienDisplay,
                ThoiDiemXacNhanThucHien = thongTinYLenh.ThoiDiemXacNhanThucHien,
                GioThucHien = thongTinYLenh.GioThucHien,
                XacNhanThucHien = thongTinYLenh.XacNhanThucHien,
                NgayThucHien = ngayYLenh.Date,
            };
            newDienBien.TongHopYLenhDienBienChiTiets.Add(newChiTiet);

            return newDienBien;
        }

        [HttpPut("KiemTraLuuDienBienYLenh")]
        public async Task<ActionResult> KiemTraLuuDienBienYLenhAsync([FromBody]TongHopYLenhPhieuDieuTriViewModel phieuDieuTri)
        {
            return Ok();
        }

        [HttpGet("GetThongTinYLenhNhanVienDangLogin")]
        public TongHopYLenhThemMoiViewModel GetThongTinYLenhNhanVienDangLogin()
        {
            var now = DateTime.Now;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            return new TongHopYLenhThemMoiViewModel()
            {
                NhanVienXacNhanThucHienId = currentUserId,


                //Cập nhật 06/06/2022: Cập nhật giờ thực hiện -> đổi từ chỉ nhập giờ thành nhập ngày giờ (Áp dụng cho trường hợp check vào xác nhận thực hiện trên grid)
                //GioThucHien = now.Hour * 3600 + now.Minute * 60,
                ThoiDiemXacNhanThucHien = now
            };
        }
        #endregion

        #region Thêm/xóa/sửa
        [HttpPut("XuLyLuuDienBienYLenh")]
        public async Task<ActionResult> XuLyLuuDienBienYLenhAsync([FromBody]TongHopYLenhPhieuDieuTriVo phieuDieuTri)
        {
            //if (!phieuDieuTri.TongHopYLenhDienBiens.Any())
            //{
            //    throw new ApiException(_localizationService.GetResource("TongHopYLenh.TongHopYLenhDienBiens.Required"));
            //}
            await _dieuTriNoiTruService.XuLyLuuDienBienYLenhAsync(phieuDieuTri);
            return Ok();
        }

        #endregion

        #region In phiếu/ xuất excel
        [HttpPost("InPhieuChamSoc")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TongHopYLenh, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<string>> XuLyGuiMauXetNghiemAsync([FromBody]InPhieuChamSocVo detail)
        {
            var phieuChamSoc = await _dieuTriNoiTruService.InPhieuChamSocAsyncVer2(detail);
            if (string.IsNullOrEmpty(phieuChamSoc))
            {
                throw new ApiException(_localizationService.GetResource("TongHopYLenh.InPhieuChamSoc.IsEmpty"));
            }
            return phieuChamSoc;
        }


        #endregion
    }
}
