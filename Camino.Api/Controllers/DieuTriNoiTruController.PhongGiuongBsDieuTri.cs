using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Api.Models.Error;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpGet("IsDaChiDinhBacSiVaGiuong")]
        //[ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        //public ActionResult<bool> IsDaChiDinhBacSiVaGiuong(long yeuCauTiepNhanId)
        //{
        //    return _dieuTriNoiTruService.IsDaChiDinhBacSiVaGiuong(yeuCauTiepNhanId);
        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("IsDaChiDinhBacSi")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<bool> IsDaChiDinhBacSi(long yeuCauTiepNhanId)
        {
            return _dieuTriNoiTruService.IsDaChiDinhBacSi(yeuCauTiepNhanId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("IsDaChiDinhGiuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<bool> IsDaChiDinhGiuong(long yeuCauTiepNhanId)
        {
            return _dieuTriNoiTruService.IsDaChiDinhGiuong(yeuCauTiepNhanId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("IsDichVuGiuongAvailable")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<bool> IsDichVuGiuongAvailable(KiemTraDichVuCapGiuongViewModel kiemTraDichVuCapGiuongViewModel)
        {
            if (kiemTraDichVuCapGiuongViewModel.ThoiGianNhan == null || kiemTraDichVuCapGiuongViewModel.DichVuGiuongId == null)
            {
                return false;
            }

            return _dieuTriNoiTruService.IsDichVuGiuongAvailable(kiemTraDichVuCapGiuongViewModel.DichVuGiuongId.Value, kiemTraDichVuCapGiuongViewModel.ThoiGianNhan.Value);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("IsGiuongAvailable")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<bool> IsGiuongAvailable(KiemTraDichVuCapGiuongViewModel kiemTraDichVuCapGiuongViewModel)
        {
            if (kiemTraDichVuCapGiuongViewModel.ThoiGianNhan == null || kiemTraDichVuCapGiuongViewModel.GiuongBenhId == null)
            {
                return false;
            }

            return _dieuTriNoiTruService.IsGiuongAvailable(kiemTraDichVuCapGiuongViewModel.GiuongBenhId.Value, kiemTraDichVuCapGiuongViewModel.YeuCauDichVuGiuongBenhVienId, kiemTraDichVuCapGiuongViewModel.ThoiGianNhan, kiemTraDichVuCapGiuongViewModel.ThoiGianTra);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThoiDiemNhapVien")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<DateTime> GetThoiDiemNhapVien(long yeuCauTiepNhanId)
        {
            return _dieuTriNoiTruService.GetThoiDiemNhapVien(yeuCauTiepNhanId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChanDoanICD")]
        public async Task<ActionResult> GetChanDoanICD([FromBody] DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetChanDoanICD(model);
            return Ok(lookup);
        }

        #region Bác sĩ điều trị
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachEkipDieuTriForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachEkipDieuTriForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDanhSachEkipDieuTriForGrid(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachEkipDieuTriForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachEkipDieuTriForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPagesDanhSachEkipDieuTriForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ThemBacSiDieuTri")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemBacSiDieuTri([FromBody] EkipDieuTriViewModel ekipDieuTriViewModel)
        {
            var noiTruEkipDieuTri = ekipDieuTriViewModel.ToEntity<NoiTruEkipDieuTri>();

           
          

            var lstNoiTruEkipDieuTri = _dieuTriNoiTruService.XuLyThemBacSiDieuTri(noiTruEkipDieuTri);

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(noiTruEkipDieuTri.NoiTruBenhAnId, o => o.Include(p => p.NoiTruBenhAn)
                                                                                                                   .ThenInclude(p => p.NoiTruEkipDieuTris));
            // kết thúc bệnh án => không cho them
            if (yeuCauTiepNhan != null && yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            foreach (var item in lstNoiTruEkipDieuTri)
            {
                if (item.Id == 0)
                {
                    yeuCauTiepNhan.NoiTruBenhAn.NoiTruEkipDieuTris.Add(item);
                }
                else
                {
                    var ekipDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruEkipDieuTris.Single(c => c.Id == item.Id);
                    ekipDieuTri.DenNgay = item.DenNgay;
                }
            }

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaBacSiDieuTri")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaBacSiDieuTri([FromBody] EkipDieuTriViewModel ekipDieuTriViewModel)
        {
            var noiTruEkipDieuTri = ekipDieuTriViewModel.ToEntity<NoiTruEkipDieuTri>();
            var lstNoiTruEkipDieuTri = _dieuTriNoiTruService.XuLySuaBacSiDieuTri(noiTruEkipDieuTri);

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(noiTruEkipDieuTri.NoiTruBenhAnId, o => o.Include(p => p.NoiTruBenhAn)
                                                                                                                   .ThenInclude(p => p.NoiTruEkipDieuTris));

            foreach (var item in lstNoiTruEkipDieuTri)
            {
                if (item.Id == noiTruEkipDieuTri.Id)
                {
                    var ekipDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruEkipDieuTris.Single(c => c.Id == item.Id);
                    ekipDieuTri.NoiTruBenhAnId = item.NoiTruBenhAnId;
                    ekipDieuTri.BacSiId = item.BacSiId;
                    ekipDieuTri.DieuDuongId = item.DieuDuongId;
                    ekipDieuTri.NhanVienLapId = item.NhanVienLapId;
                    ekipDieuTri.KhoaPhongDieuTriId = item.KhoaPhongDieuTriId;
                    ekipDieuTri.TuNgay = item.TuNgay;
                    ekipDieuTri.DenNgay = item.DenNgay;
                }
                else
                {
                    var ekipDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruEkipDieuTris.Single(c => c.Id == item.Id);
                    ekipDieuTri.DenNgay = item.DenNgay;
                }
            }

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("XoaBacSiDieuTri")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaBacSiDieuTri(long yeuCauTiepNhanId, long noiTruEkipDieuTriId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId, o => o.Include(p => p.NoiTruBenhAn)
                                                                                                   .ThenInclude(p => p.NoiTruEkipDieuTris));


            var deletedEkipDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruEkipDieuTris.Single(c => c.Id == noiTruEkipDieuTriId);
            deletedEkipDieuTri.WillDelete = true;

            var lstNoiTruEkipDieuTri = _dieuTriNoiTruService.XuLyXoaBacSiDieuTri(deletedEkipDieuTri);
            foreach (var item in lstNoiTruEkipDieuTri)
            {
                var ekipDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruEkipDieuTris.Single(c => c.Id == item.Id);
                ekipDieuTri.DenNgay = item.DenNgay;
            }

            await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }
        #endregion

        #region Cấp giường
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachCapGiuongForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachCapGiuongForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDanhSachCapGiuongForGrid(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachCapGiuongForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachCapGiuongForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPagesDanhSachCapGiuongForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetDanhSachChiTietSuDungGiuongTheoNgayForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ChiTietSuDungGiuongTheoNgayViewModel> GetDanhSachChiTietSuDungGiuongTheoNgayForGrid(long yeuCauTiepNhanId)
        {
            var data = _dieuTriNoiTruService.GetDanhSachChiTietSuDungGiuongTheoNgayForGrid(yeuCauTiepNhanId);
            return Ok(_mapper.Map<ChiTietSuDungGiuongTheoNgayViewModel>(data));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaDanhSachChiTietSuDungGiuongTheoNgay")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaDanhSachChiTietSuDungGiuongTheoNgay(ChiTietSuDungGiuongTheoNgayViewModel chiTietSuDungGiuongTheoNgayViewModel, long yeuCauTiepNhanId)
        {
            
            foreach(var suDungGiuongTheoNgay in chiTietSuDungGiuongTheoNgayViewModel.SuDungGiuongTheoNgays)
            {
                foreach (var chiTietGiuongBenhVienChiPhi in suDungGiuongTheoNgay.ChiTietGiuongBenhVienChiPhis)
                {
                    if(chiTietGiuongBenhVienChiPhi.isCreated != true && chiTietGiuongBenhVienChiPhi.LoaiChiPhiGiuongBenh == null)
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.LoaiChiPhiGiuongBenh.Required"));
                    }
                    if (chiTietGiuongBenhVienChiPhi.DoiTuong == DoiTuongSuDung.BenhNhan && chiTietGiuongBenhVienChiPhi.GiuongChuyenDenId == null)
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.GiuongChuyenDen.Required"));
                    }
                    if (chiTietGiuongBenhVienChiPhi.DichVuGiuongId == null)
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.DichVuGiuong.Required"));
                    }
                    if (chiTietGiuongBenhVienChiPhi.SoLuong == null)
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.SoLuong.Required"));
                    }
                    if (chiTietGiuongBenhVienChiPhi.SoLuong == 0)
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.SoLuong.Min"));
                    }
                    if (chiTietGiuongBenhVienChiPhi.SoLuongGhep == null)
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.SoLuongGhep.Required"));
                    }
                    if (chiTietGiuongBenhVienChiPhi.DoiTuong == null)
                    {
                        throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.DoiTuong.Required"));
                    }
                }
            }
            

            //        RuleFor(p => p.LoaiChiPhiGiuongBenh)
            //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.LoaiChiPhiGiuongBenh.Required")).When(p => p.isCreated != true);

            //        RuleFor(p => p.GiuongChuyenDenId)
            //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.GiuongChuyenDen.Required")).When(p => p.DoiTuong == DoiTuongSuDung.BenhNhan);

            //        RuleFor(p => p.DichVuGiuongId)
            //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.DichVuGiuong.Required"));

            //        RuleFor(p => p.SoLuong)
            //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.SoLuong.Required"))
            //            .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.SoLuong.Min"));

            //        RuleFor(p => p.SoLuongGhep)
            //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.SoLuongGhep.Required"));

            //        RuleFor(p => p.DoiTuong)
            //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.DoiTuong.Required"));


            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(yeuCauTiepNhanId, o => o.Include(p => p.YeuCauDichVuGiuongBenhViens)
                                                                                        .Include(p => p.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(p => p.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(p => p.DuyetBaoHiemChiTiets)
                                                                                        .Include(p => p.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(p => p.DuyetBaoHiemChiTiets)
                                                                                        .Include(p => p.YeuCauTiepNhanTheBHYTs));

            var chiTietSuDungGiuong = _mapper.Map<ChiTietSuDungGiuongTheoNgayVo>(chiTietSuDungGiuongTheoNgayViewModel);

            await _dieuTriNoiTruService.SuaDanhSachChiTietSuDungGiuongTheoNgay(yeuCauTiepNhan, chiTietSuDungGiuong);

            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListGiuongChoChiTietSuDungTheoNgay")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<List<LookupItemVo>> GetListGiuongChoChiTietSuDungTheoNgay([FromBody] DropDownListRequestModel model)
        {
            var gridData = _dieuTriNoiTruService.GetListGiuongChoChiTietSuDungTheoNgay(model);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListDichVuGiuongBenhVien")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetListDichVuGiuongBenhVien([FromBody] DropDownListRequestModel model)
        {
            var gridData = await _dieuTriNoiTruService.GetListDichVuGiuongBenhVien(model);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetLoaiGiuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<LookupItemVo> GetLoaiGiuong(EnumLoaiGiuong loaiGiuong)
        {
            var data = _dieuTriNoiTruService.GetLoaiGiuong(loaiGiuong);
            return Ok(data);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListDoiTuongSuDung")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public List<LookupItemVo> GetListDoiTuongSuDung([FromBody] DropDownListRequestModel dropDownListRequestModel)
        {
            return _dieuTriNoiTruService.GetListDoiTuongSuDung(dropDownListRequestModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ThemCapGiuong")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemCapGiuong([FromBody] CapGiuongViewModel capGiuongViewModel)
        {
            //Xử lý lưu
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(capGiuongViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruBenhAn)
                                                                                                                      .Include(p => p.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.HoatDongGiuongBenhs)
                                                                                                                      .Include(p => p.YeuCauTiepNhanTheBHYTs)
                                                                                                                      .Include(x => x.BenhNhan).ThenInclude(y => y.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(t => t.ChuongTrinhGoiDichVuDichVuGiuongs)
                                                                                                                      .Include(x => x.BenhNhan).ThenInclude(y => y.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(t => t.ChuongTrinhGoiDichVuDichVuGiuongs));
            // kết thúc bệnh án => không cho them
            if (yeuCauTiepNhan != null && yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            //Xử lý kiểm tra giường: còn trống? có bao phòng?
            if (capGiuongViewModel.GiuongBenhId != null)
            {
                var giuongBenhTrong = new GiuongBenhTrongVo()
                {
                    GiuongBenhId = capGiuongViewModel.GiuongBenhId.Value,
                    BaoPhong = capGiuongViewModel.BaoPhong,
                    ThoiGianNhan = capGiuongViewModel.ThoiDiemBatDauSuDung.Value,
                    ThoiGianTra = capGiuongViewModel.ThoiDiemKetThucSuDung,
                    YeuCauTiepNhanNoiTruId = capGiuongViewModel.YeuCauTiepNhanId
                };

                await _dieuTriNoiTruService.KiemTraPhongChiDinhTiepNhanNoiTru(giuongBenhTrong);
            }

           

            var capGiuongVo = new CapGiuongVo()
            {
                Id = capGiuongViewModel.Id,
                YeuCauTiepNhanId = capGiuongViewModel.YeuCauTiepNhanId,
                DichVuGiuongBenhVienId = capGiuongViewModel.DichVuGiuongBenhVienId.Value,
                GiuongBenhId = capGiuongViewModel.GiuongBenhId,
                LoaiGiuong = capGiuongViewModel.LoaiGiuong,
                BaoPhong = capGiuongViewModel.BaoPhong,
                DoiTuongSuDung = capGiuongViewModel.DoiTuongSuDung,
                ThoiDiemBatDauSuDung = capGiuongViewModel.ThoiDiemBatDauSuDung.Value,
                ThoiDiemKetThucSuDung = capGiuongViewModel.ThoiDiemKetThucSuDung,
                GhiChu = capGiuongViewModel.GhiChu,
                YeuCauGoiDichVuId = capGiuongViewModel.YeuCauGoiDichVuId
            };

            await _dieuTriNoiTruService.XuLyThemCapGiuong(yeuCauTiepNhan, capGiuongVo);
            _dieuTriNoiTruService.XuLyThoiGianTraGiuong(yeuCauTiepNhan, capGiuongVo.ThoiDiemBatDauSuDung, capGiuongVo.ThoiDiemKetThucSuDung, capGiuongVo.DoiTuongSuDung, capGiuongVo.Id);

            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaCapGiuong")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaCapGiuong([FromBody] CapGiuongViewModel capGiuongViewModel)
        {
            //Xử lý kiểm tra đã quyết toán
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(capGiuongViewModel.YeuCauTiepNhanId, o => o.Include(p => p.NoiTruBenhAn)
                                                                                                                      .Include(p => p.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.HoatDongGiuongBenhs)
                                                                                                                      .Include(p => p.YeuCauTiepNhanTheBHYTs)
                                                                                                                      .Include(x => x.BenhNhan).ThenInclude(y => y.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(t => t.ChuongTrinhGoiDichVuDichVuGiuongs)
                                                                                                                      .Include(x => x.BenhNhan).ThenInclude(y => y.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(t => t.ChuongTrinhGoiDichVuDichVuGiuongs));

            if (yeuCauTiepNhan.NoiTruBenhAn?.DaQuyetToan == true)
            {
                throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.DaQuyetToan"));
            }

            // xử lý kiểm tra giường: còn trống? có bao phòng?
            if (capGiuongViewModel.GiuongBenhId != null)
            {
                var giuongBenhTrong = new GiuongBenhTrongVo()
                {
                    GiuongBenhId = capGiuongViewModel.GiuongBenhId.Value,
                    BaoPhong = capGiuongViewModel.BaoPhong,
                    ThoiGianNhan = capGiuongViewModel.ThoiDiemBatDauSuDung.Value,
                    ThoiGianTra = capGiuongViewModel.ThoiDiemKetThucSuDung,
                    YeuCauDichVuGiuongBenhVienId = capGiuongViewModel.Id
                };

                await _dieuTriNoiTruService.KiemTraPhongChiDinhTiepNhanNoiTru(giuongBenhTrong);
            }

            // xử lý lưu
            var capGiuongVo = new CapGiuongVo()
            {
                Id = capGiuongViewModel.Id,
                YeuCauTiepNhanId = capGiuongViewModel.YeuCauTiepNhanId,
                DichVuGiuongBenhVienId = capGiuongViewModel.DichVuGiuongBenhVienId.Value,
                GiuongBenhId = capGiuongViewModel.GiuongBenhId,
                LoaiGiuong = capGiuongViewModel.LoaiGiuong,
                BaoPhong = capGiuongViewModel.BaoPhong,
                DoiTuongSuDung = capGiuongViewModel.DoiTuongSuDung,
                ThoiDiemBatDauSuDung = capGiuongViewModel.ThoiDiemBatDauSuDung.Value,
                ThoiDiemKetThucSuDung = capGiuongViewModel.ThoiDiemKetThucSuDung,
                GhiChu = capGiuongViewModel.GhiChu,
                YeuCauGoiDichVuId = capGiuongViewModel.YeuCauGoiDichVuId
            };

            var oldDoiTuongSuDung = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(p => p.Id == capGiuongVo.Id)
                                                                              .Select(p => p.DoiTuongSuDung)
                                                                              .FirstOrDefault();

            await _dieuTriNoiTruService.XuLySuaHoatDongGiuong(yeuCauTiepNhan, capGiuongVo);

            var lstYeuCauDichVuGiuong = _dieuTriNoiTruService.XuLySuaCapGiuong(capGiuongVo, oldDoiTuongSuDung);

            //check giường cũ khi sửa có ảnh hưởng giường khác
            //foreach (var item in lstYeuCauDichVuGiuong)
            //{
            //    try
            //    {
            //        var giuongBenhTrongKhac = new GiuongBenhTrongVo()
            //        {
            //            GiuongBenhId = item.GiuongBenhId.GetValueOrDefault(),
            //            BaoPhong = item.BaoPhong,
            //            ThoiGianNhan = item.ThoiDiemBatDauSuDung.GetValueOrDefault(),
            //            ThoiGianTra = item.ThoiDiemKetThucSuDung,
            //            YeuCauDichVuGiuongBenhVienId = item.Id
            //        };

            //        await _dieuTriNoiTruService.KiemTraPhongChiDinhTiepNhanNoiTru(giuongBenhTrongKhac);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new ApiException($"{item.GiuongBenh?.Ten ?? ""} - {item.GiuongBenh?.PhongBenhVien?.Ten ?? ""}: {ex.Message}");
            //    }
            //}

            foreach (var item in lstYeuCauDichVuGiuong)
            {
                var tempCapGiuongVo = new CapGiuongVo()
                {
                    Id = item.Id,
                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                    DichVuGiuongBenhVienId = item.DichVuGiuongBenhVienId,
                    GiuongBenhId = item.GiuongBenhId.Value,
                    LoaiGiuong = item.LoaiGiuong,
                    BaoPhong = item.BaoPhong ?? false,
                    DoiTuongSuDung = item.DoiTuongSuDung,
                    ThoiDiemBatDauSuDung = item.ThoiDiemBatDauSuDung.Value,
                    ThoiDiemKetThucSuDung = item.ThoiDiemKetThucSuDung,
                    GhiChu = item.GhiChu
                };

                await _dieuTriNoiTruService.XuLySuaHoatDongGiuong(yeuCauTiepNhan, tempCapGiuongVo);
            }

            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("XoaCapGiuong")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaCapGiuong(long yeuCauTiepNhanId, long yeuCauDichVuGiuongBenhVienId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId, o => o.Include(p => p.YeuCauDichVuGiuongBenhViens).ThenInclude(p => p.HoatDongGiuongBenhs)
                .Include(p => p.YeuCauTiepNhanTheBHYTs)
                .Include(x => x.BenhNhan).ThenInclude(y => y.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(t => t.ChuongTrinhGoiDichVuDichVuGiuongs)
                .Include(x => x.BenhNhan).ThenInclude(y => y.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(t => t.ChuongTrinhGoiDichVuDichVuGiuongs));

            var deletedYeuCauDichVuGiuong = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Single(p => p.Id == yeuCauDichVuGiuongBenhVienId);
            deletedYeuCauDichVuGiuong.WillDelete = true;

            var lstYeuCauDichVuGiuong = _dieuTriNoiTruService.XuLyXoaCapGiuong(deletedYeuCauDichVuGiuong);

            //check giường cũ khi sửa có ảnh hưởng giường khác
            //foreach (var item in lstYeuCauDichVuGiuong)
            //{
            //    try
            //    {
            //        var giuongBenhTrongKhac = new GiuongBenhTrongVo()
            //        {
            //            GiuongBenhId = item.GiuongBenhId.GetValueOrDefault(),
            //            BaoPhong = item.BaoPhong,
            //            ThoiGianNhan = item.ThoiDiemBatDauSuDung.GetValueOrDefault(),
            //            ThoiGianTra = item.ThoiDiemKetThucSuDung,
            //            YeuCauDichVuGiuongBenhVienId = item.Id
            //        };

            //        await _dieuTriNoiTruService.KiemTraPhongChiDinhTiepNhanNoiTru(giuongBenhTrongKhac);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new ApiException($"{item.GiuongBenh?.Ten ?? ""} - {item.GiuongBenh?.PhongBenhVien?.Ten ?? ""}: {ex.Message}");
            //    }
            //}

            foreach (var item in lstYeuCauDichVuGiuong)
            {
                //var yeuCauDichVu = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Single(p => p.Id == item.Id);
                //yeuCauDichVu.ThoiDiemKetThucSuDung = item.ThoiDiemKetThucSuDung;

                var tempCapGiuongVo = new CapGiuongVo()
                {
                    Id = item.Id,
                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                    DichVuGiuongBenhVienId = item.DichVuGiuongBenhVienId,
                    GiuongBenhId = item.GiuongBenhId.Value,
                    LoaiGiuong = item.LoaiGiuong,
                    BaoPhong = item.BaoPhong ?? false,
                    DoiTuongSuDung = item.DoiTuongSuDung,
                    ThoiDiemBatDauSuDung = item.ThoiDiemBatDauSuDung.Value,
                    ThoiDiemKetThucSuDung = item.ThoiDiemKetThucSuDung,
                    GhiChu = item.GhiChu,
                    YeuCauGoiDichVuId = item.YeuCauGoiDichVuId
                };

                await _dieuTriNoiTruService.XuLySuaHoatDongGiuong(yeuCauTiepNhan, tempCapGiuongVo);
            }

            await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThoiDiemChiDinhGiuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ThoiDiemNhanGiuongVo> GetThoiDiemChiDinhGiuong(long yeuCauTiepNhanId)
        {
            var data = _dieuTriNoiTruService.GetThoiDiemChiDinhGiuong(yeuCauTiepNhanId);
            return Ok(data);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDonGiaDichVuGiuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ThongTinGiaDichVuGiuongVo> GetDonGiaDichVuGiuong(ThongTinGetGiaDichVuGiuongViewModel thongTinGetGiaDichVuGiuongViewModel)
        {
            var data = _dieuTriNoiTruService.GetDonGiaDichVuGiuong(thongTinGetGiaDichVuGiuongViewModel.YeuCauTiepNhanId, thongTinGetGiaDichVuGiuongViewModel.DichVuGiuongId, thongTinGetGiaDichVuGiuongViewModel.NgayPhatSinh, thongTinGetGiaDichVuGiuongViewModel.BaoPhong);
            return Ok(data);
        }
        #endregion

        #region Chuyển khoa
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetLastKhoaPhongDieuTri")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetLastKhoaPhongDieuTri(long yeuCauTiepNhanId)
        {
            var khoaPhongDieuTri = await _dieuTriNoiTruService.GetLastKhoaPhongDieuTri(yeuCauTiepNhanId);
            return Ok(khoaPhongDieuTri.ToModel<KhoaPhongDieuTriViewModel>());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachChuyenKhoaForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachChuyenKhoaForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDanhSachChuyenKhoaForGrid(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPagesDanhSachChuyenKhoaForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesDanhSachChuyenKhoaForGrid([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPagesDanhSachChuyenKhoaForGrid(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ThemKhoaPhongDieuTri")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemKhoaPhongDieuTri([FromBody] KhoaPhongDieuTriViewModel khoaPhongDieuTriViewModel)
        {
            if (_dieuTriNoiTruService.KiemTraTonTaiLichDieuTri(khoaPhongDieuTriViewModel.ThoiDiemVaoKhoa, khoaPhongDieuTriViewModel.NoiTruBenhAnId.GetValueOrDefault()) == false)
            {
                throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.NgayDieuTri.Existed"));
            }

            var noiTruKhoaPhongDieuTri = khoaPhongDieuTriViewModel.ToEntity<NoiTruKhoaPhongDieuTri>();
            var lstNoiTruKhoaPhongDieuTri = _dieuTriNoiTruService.XuLyThemKhoaPhongDieuTri(noiTruKhoaPhongDieuTri);

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(noiTruKhoaPhongDieuTri.NoiTruBenhAnId, o => o.Include(p => p.NoiTruBenhAn)
                                                                                                                        .ThenInclude(p => p.NoiTruKhoaPhongDieuTris));
            if (yeuCauTiepNhan!= null && yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            foreach (var item in lstNoiTruKhoaPhongDieuTri)
            {
                if (item.Id == 0)
                {
                    yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Add(item);
                }
                else
                {
                    var khoaPhongDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Single(c => c.Id == item.Id);
                    var khoaPhongVM = item.ToModel<KhoaPhongDieuTriViewModel>();

                    khoaPhongDieuTri = khoaPhongVM.ToEntity(khoaPhongDieuTri);
                }
            }

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("SuaKhoaPhongDieuTri")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> SuaKhoaPhongDieuTri([FromBody] KhoaPhongDieuTriViewModel khoaPhongDieuTriViewModel)
        {
            if (_dieuTriNoiTruService.KiemTraTonTaiLichDieuTri(khoaPhongDieuTriViewModel.ThoiDiemVaoKhoa, khoaPhongDieuTriViewModel.NoiTruBenhAnId.GetValueOrDefault()) == false)
            {
                throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.NgayDieuTri.Existed"));
            }

            var noiTruKhoaPhongDieuTri = khoaPhongDieuTriViewModel.ToEntity<NoiTruKhoaPhongDieuTri>();
            var lstNoiTruKhoaPhongDieuTri = _dieuTriNoiTruService.XuLySuaKhoaPhongDieuTri(noiTruKhoaPhongDieuTri);

            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(noiTruKhoaPhongDieuTri.NoiTruBenhAnId, o => o.Include(p => p.NoiTruBenhAn)
                                                                                                                        .ThenInclude(p => p.NoiTruKhoaPhongDieuTris));

            foreach (var item in lstNoiTruKhoaPhongDieuTri)
            {
                var khoaPhongDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Single(c => c.Id == item.Id);
                var khoaPhongVM = item.ToModel<KhoaPhongDieuTriViewModel>();

                khoaPhongDieuTri = khoaPhongVM.ToEntity(khoaPhongDieuTri);
            }

            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("XoaKhoaPhongDieuTri")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaKhoaPhongDieuTri(long yeuCauTiepNhanId, long noiTruKhoaPhongDieuTriId)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhanId, o => o.Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruKhoaPhongDieuTris)
                                                                                                   .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruPhieuDieuTris));


            var deletedKhoaPhongDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Single(c => c.Id == noiTruKhoaPhongDieuTriId);

            if (yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Any(p => p.NgayDieuTri.Date >= deletedKhoaPhongDieuTri.ThoiDiemVaoKhoa.Date && (deletedKhoaPhongDieuTri.ThoiDiemRaKhoa == null || p.NgayDieuTri.Date <= deletedKhoaPhongDieuTri.ThoiDiemRaKhoa.Value.Date)))
            {
                throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.Delete.NgayDieuTri.Existed"));
            }

            deletedKhoaPhongDieuTri.WillDelete = true;

            var lstKhoaPhongDieuTri = _dieuTriNoiTruService.XuLyXoaKhoaPhongDieuTri(deletedKhoaPhongDieuTri);
            foreach (var item in lstKhoaPhongDieuTri)
            {
                var khoaPhongDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Single(c => c.Id == item.Id);
                var khoaPhongVM = item.ToModel<KhoaPhongDieuTriViewModel>();

                khoaPhongDieuTri = khoaPhongVM.ToEntity(khoaPhongDieuTri);
            }

            await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetCurrentNoiTruKhoaPhongDieuTri")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<KhoaPhongDieuTriViewModel>> GetCurrentNoiTruKhoaPhongDieuTri(long noiTruBenhAnId)
        {
            var currentNoiTruKhoaPhongDieuTri = await _dieuTriNoiTruService.GetCurrentNoiTruKhoaPhongDieuTri(noiTruBenhAnId);

            if (currentNoiTruKhoaPhongDieuTri == null)
            {
                return new KhoaPhongDieuTriViewModel();
            }

            return currentNoiTruKhoaPhongDieuTri.ToModel<KhoaPhongDieuTriViewModel>();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetCurrentKhoaHienTaiBenhNhanChuyenDen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<KhoaPhongChuyenDen>> GetCurrentKhoaHienTaiBenhNhanChuyenDen(long noiTruBenhAnId)
        {
            return await _dieuTriNoiTruService.GetCurrentKhoaHienTaiBenhNhanChuyenDen(noiTruBenhAnId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachKhoaChuyenDen")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<List<LookupItemVo>>> GetDanhSachKhoaChuyenDen(DropDownListRequestModel model)
        {
            return await _dieuTriNoiTruService.GetDanhSachKhoaChuyenDen(model);
        }
        #endregion

        [HttpPost("KiemTraDichVuCoTrongGoi")]
        public bool KiemTraDichVuCoTrongGoi(long benhNhanId, long? dichVuGiuongBenhVienId)
        {
            return _dieuTriNoiTruService.KiemTraDichVuCoTrongGoi(benhNhanId, dichVuGiuongBenhVienId);
        }
    }
}