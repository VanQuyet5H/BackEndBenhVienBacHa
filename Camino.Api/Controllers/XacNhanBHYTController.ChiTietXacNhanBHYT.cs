using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BHYT;
using Camino.Api.Models.YeuCauTiepNhan;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// include all APIs connected to provide bhyt information and its detail
namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController
    {
        #region LoadThongTinHanhChinhVaThongTinBaoHiemYTe
        [HttpPost("GetById")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBHYT)]
        public async Task<ActionResult<YeuCauTiepNhanViewModel>> GetById(long id)
        {
            var yeuCauTiepNhan = await _xacNhanBhytService.GetByIdAsync(id,
                s => s.Include(k => k.DoiTuongUuDai)
                    .Include(k => k.CongTyUuDai)
                    .Include(k => k.PhuongXa)
                    .Include(k => k.QuanHuyen)
                    .Include(k => k.TinhThanh)
                    .Include(k => k.GiayChuyenVien)
                    .Include(k => k.BHYTGiayMienCungChiTra)
                    .Include(k => k.YeuCauKhamBenhs).ThenInclude(o => o.BenhVienChuyenVien)
                    .Include(k => k.BenhNhan).Include(k => k.NoiChuyen));

            if (yeuCauTiepNhan == null)
            {
                return NotFound();
            }

            var resultData = yeuCauTiepNhan.ToModel<YeuCauTiepNhanViewModel>();
            resultData.LyDoVaoVienDisplay = yeuCauTiepNhan.LyDoVaoVien.GetDescription();

            if (resultData.BHYTMaDKBD != null)
            {
                var noiDkbd = _xacNhanBhytService.GetNoiDKBD(resultData.BHYTMaDKBD);
                resultData.DKBD = noiDkbd;
            }

            if (resultData.GiayChuyenVienId != null)
            {
                var giayChuyenVien = _xacNhanBhytService.GetGiayChuyenVien(resultData.GiayChuyenVienId);
                resultData.GiayChuyenVienDisplay = giayChuyenVien;
            }

            if (resultData.BHYTGiayMienCungChiTraId != null)
            {
                var giayMienCungChiTra = _xacNhanBhytService.GetGiayMienCungChiTra(resultData.BHYTGiayMienCungChiTraId);
                resultData.GiayMienCungChiTraDisplay = giayMienCungChiTra;
            }

            if (resultData.BHYTNgayHieuLuc != null)
            {
                var ngayHieuLucFormat = resultData.BHYTNgayHieuLuc.Value.ApplyFormatDate();
                resultData.BHYTngayHieuLucStr = ngayHieuLucFormat;
            }

            if (resultData.BHYTNgayHetHan != null)
            {
                var ngayHetHanFormat = resultData.BHYTNgayHetHan.Value.ApplyFormatDate();
                resultData.BHYTngayHetHanStr = ngayHetHanFormat;
            }

            var bvChuyenViens = yeuCauTiepNhan.YeuCauKhamBenhs.Where(f => f.CoChuyenVien != false).Select(c => c.BenhVienChuyenVien?.Ten).ToList();
            resultData.NoiChuyenDi = bvChuyenViens != null ? string.Join("; ", bvChuyenViens) : string.Empty;

            resultData.DiaChi = yeuCauTiepNhan.DiaChiDayDu;

            #region BVHD-3941
            if (yeuCauTiepNhan.CoBHTN == true)
            {
                resultData.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(yeuCauTiepNhan.Id);
            }
            #endregion

            return Ok(resultData);
        }

        [HttpGet("GetByIdNoiTruAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytNoiTru)]
        public async Task<ActionResult<BhytNoiTruChiTietViewModel>> GetByIdNoiTruAsync(long yctnId)
        {
            var yeuCauTiepNhan = await _xacNhanBhytService.GetByIdAsync(yctnId,
                s => s.Include(k => k.DoiTuongUuDai)
                    .Include(k => k.CongTyUuDai)
                    .Include(k => k.PhuongXa)
                    .Include(k => k.QuanHuyen)
                    .Include(k => k.TinhThanh)
                    .Include(k => k.BenhNhan)
                    .Include(k => k.GiayChuyenVien)
                    .Include(k => k.YeuCauTiepNhanTheBHYTs).ThenInclude(k => k.GiayMienCungChiTra));

            if (yeuCauTiepNhan == null)
            {
                return NotFound();
            }

            var resultData = new BhytNoiTruChiTietViewModel
            {
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                HoTen = yeuCauTiepNhan.HoTen,
                GioiTinh = yeuCauTiepNhan.GioiTinh,
                MaBn = yeuCauTiepNhan.BenhNhan.MaBN,
                NamSinh = yeuCauTiepNhan.NamSinh,
                SoDienThoai = yeuCauTiepNhan.SoDienThoai,
                MaYeuCauTiepNhan = yeuCauTiepNhan.MaYeuCauTiepNhan,
                Email = yeuCauTiepNhan.Email,
                CongTyUuDai = yeuCauTiepNhan.CongTyUuDai?.Ten,
                DoiTuongUuDai = yeuCauTiepNhan.DoiTuongUuDai?.Ten,
                LyDoVaoVien = yeuCauTiepNhan.LyDoVaoVien?.GetDescription(),
                GiayChuyenVien = yeuCauTiepNhan.GiayChuyenVien?.Ten,
                GiayChuyenVienId = yeuCauTiepNhan.GiayChuyenVienId,
                Id = yeuCauTiepNhan.Id
            };

            foreach (var theBhyt in yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs)
            {
                resultData.ThongTinBhytNoiTrus.Add(new ThongTinBhytNoiTruViewModel
                {
                    BhytNgayHetHanStr = theBhyt.NgayHetHan != null ? theBhyt.NgayHetHan.GetValueOrDefault().ApplyFormatDate() : string.Empty,
                    BhytMaSoThe = theBhyt.MaSoThe,
                    BhytMucHuong = theBhyt.MucHuong,
                    BhytNgayHieuLucStr = theBhyt.NgayHieuLuc.ApplyFormatDate(),
                    Dkbd = theBhyt.MaDKBD,
                    GiayMienCungChiTraId = theBhyt.GiayMienCungChiTraId,
                    GiayMienCungChiTra = theBhyt.GiayMienCungChiTra?.Ten,
                    Id = theBhyt.Id
                });
            }

            #region BVHD-3941
            if (yeuCauTiepNhan.CoBHTN == true)
            {
                resultData.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(yeuCauTiepNhan.Id);
            }
            #endregion

            return Ok(resultData);
        }
        #endregion

        #region Document
        [HttpPost("GetDocument")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBHYT)]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.XacNhanBhytDaHoanThanh)]
        public async Task<ActionResult> GetDocument(long idFile, int type)
        {
            if (type == 1)
            {
                // giấy chuyển viện
                var document = await _xacNhanBhytService.GetDocumentChoGiayChuyenVien(idFile);
                return Ok(document);
            }
            if (type == 2)
            {
                // giấy miễn cùng chi trả
                var document = await _xacNhanBhytService.GetDocumentChoGiayMienCungChiTra(idFile);
                return Ok(document);
            }

            return Ok();
        }
        #endregion
    }
}
