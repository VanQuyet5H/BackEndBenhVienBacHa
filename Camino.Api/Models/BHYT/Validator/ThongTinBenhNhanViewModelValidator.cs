using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BHYT;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<ThongTinBenhNhanViewModel>))]
    public class ThongTinBenhNhanViewModelValidator : AbstractValidator<ThongTinBenhNhanViewModel>
    {
        public ThongTinBenhNhanViewModelValidator(ILocalizationService iLocalizationService, IBaoHiemYTeService _BHYTService, IValidator<HoSoChiTietDienBienBenhViewModel> dienbienBenhValidator, IValidator<HoSoChiTietThuocViewModel> hosothuocValidator, IValidator<HoSoChiTietDVKTViewModel> hosoDVKTValidator, IValidator<HoSoCanLamSangViewModel> canlamsangValidator)
        {
            RuleFor(x => x.MaLienKet)
           .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaLienKet.Required"));
            //.Must((request, mlk) =>
            // {
            //     if (mlk != null && mlk.Length < 100)
            //         return false;
            //     return true;
            // }).WithMessage(iLocalizationService.GetResource("BHYT.MaLienKet.Length"));


            //RuleFor(x => x.NgaySinh)
            //   .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.ThoiGian.Required"));

           // RuleFor(x => x.MaBenhNhan)
           //     .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaBenhNhan.Required"));

           // RuleFor(x => x.HoTen)
           //   .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.HoTen.Required"));

           // RuleFor(x => x.GioiTinh)
           //  .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.GioiTinh.Required"));

           // RuleFor(x => x.DiaChi)
           // .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.DiaChi.Required"));

           // RuleFor(x => x.MaThe)
           //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaThe.Required"))
           //.Must((request, mt) =>
           //{
           //    if (mt != null && mt.Length > 15 && mt.Length < 255 && mt.Length != 0)
           //        return false;
           //    return true;
           //}).WithMessage(iLocalizationService.GetResource("BHYT.MaLienKet.Length"));


           // RuleFor(x => x.MaCoSoKCBBanDau)
           //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaCoSoKCBBanDau.Required"))
           //  .Must((request, mt) =>
           //  {
           //      if (mt != null && mt.Length > 5 && mt.Length < 255 && mt.Length != 0)
           //          return false;
           //      return true;
           //  }).WithMessage(iLocalizationService.GetResource("BHYT.MaCoSoKCBBanDau.Length"))
           // .MustAsync(async (model, input, s) => !await _BHYTService.CheckValidMaDKBD(!string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input).ConfigureAwait(false))
           //     .WithMessage(iLocalizationService.GetResource("BHYT.MaCoSoKCBBanDau.Valid"));

           // RuleFor(x => x.GiaTriTheTu)
           //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.GiaTriTheTu.Required"))
           // .Length(8, 255).WithMessage(iLocalizationService.GetResource("BHYT.GiaTriTheTu.Length"));

            //.MustAsync(async (model, input, s) => !await _BHYTService.CheckValidGiaTriTheTu(!string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input).ConfigureAwait(false))
            //    .WithMessage(iLocalizationService.GetResource("BHYT.GiaTriTheTu.Valid"));

           // RuleFor(x => x.TenBenh)
           //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.TenBenh.Required"));

           // RuleFor(x => x.MaBenh)
           //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaBenh.Required"));
            //.Must((request, mt) =>
            //{
            //    if (mt != null && mt.Length < 255 && mt.Length != 0)
            //        return false;
            //    return true;
            //}).WithMessage(iLocalizationService.GetResource("BHYT.MaBenh.Length"));


           // RuleFor(x => x.LyDoVaoVien)
           //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.LyDoVaoVien.Required"));
            //   RuleFor(x => x.NgayVao)
            //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.NgayVao.Required"))
            //.Length(12).WithMessage(iLocalizationService.GetResource("BHYT.NgayVao.Length"));

            //RuleFor(x => x.NgayVaoTime)
            //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.NgayVao.Required"));
            //.MustAsync(async (model, input, s) => !await _BHYTService.CheckValidNgayVaoRa(input).ConfigureAwait(false))
            //.WithMessage(iLocalizationService.GetResource("BHYT.NgayVao.Valid"))
            //.LessThanOrEqualTo(x => x.NgayRaTime).WithMessage(iLocalizationService.GetResource("BHYT.NgayVao.LessThan"));

           // RuleFor(x => x.NgayRa)
           //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.NgayRa.Required"));
            //.Must((request, mt) =>
            //{
            //    if (mt != null && mt.Length < 12 && mt.Length != 0)
            //        return false;
            //    return true;
            //}).WithMessage(iLocalizationService.GetResource("BHYT.NgayRa.Length"));          

            //RuleFor(x => x.NgayRaTime)
            //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.NgayRa.Required"));
            //.MustAsync(async (model, input, s) => !await _BHYTService.CheckValidNgayVaoRa(input).ConfigureAwait(false))
            //   .WithMessage(iLocalizationService.GetResource("BHYT.NgayRa.Valid"));

           // RuleFor(x => x.SoNgayDieuTri)
           //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.SoNgayDieuTri.Required"));
            //.MustAsync(async (model, input, s) => !await _BHYTService.CheckValidSoNgayDieuTri(model.NgayVao, model.NgayRa, model.SoNgayDieuTri).ConfigureAwait(false))
            //    .WithMessage(iLocalizationService.GetResource("BHYT.SoNgayDieuTri.Valid"));

            //RuleFor(x => x.KetQuaDieuTri)
            //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.KetQuaDieuTri.Required"));

            //RuleFor(x => x.TinhTrangRaVien)
            //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.TinhTrangRaVien.Required"));

            //  RuleFor(x => x.TienBaoHiemThanhToan)
            //  .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.TienBaoHiemThanhToan.Required"));

            //  RuleFor(x => x.NamQuyetToan)
            //.NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.NamQuyetToan.Required"))
            //  .LessThanOrEqualTo(DateTime.Now.Year).WithMessage(iLocalizationService.GetResource("BHYT.NamQuyetToan.Less"));

            //  RuleFor(x => x.ThangQuyetToan)
            //   .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.ThangQuyetToan.Required"))
            //   .LessThanOrEqualTo(DateTime.Now.Month).WithMessage(iLocalizationService.GetResource("BHYT.ThangQuyetToan.Less"));

            //  RuleFor(x => x.TienTongChi)
            //   .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.TienTongChi.Required"));

            //  RuleFor(x => x.MaLoaiKCB)
            //   .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaLoaiKCB.Required"));

            //  RuleFor(x => x.MaKhoa)
            //   .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaKhoa.Required"))
            //   .Length(1, 15).WithMessage(iLocalizationService.GetResource("BHYT.MaKhoa.Length"));

            //  RuleFor(x => x.MaCSKCB)
            //   .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaCSKCB.Required"))
            //   .Length(5).WithMessage(iLocalizationService.GetResource("BHYT.MaCSKCB.Length"));


            //  RuleFor(x => x.GiaTriTheDen)
            //  .MustAsync(async (model, input, s) => !await _BHYTService.CheckValidGiaTriTheDen(!string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input, model.GiaTriTheTu).ConfigureAwait(false))
            //  .WithMessage(iLocalizationService.GetResource("BHYT.GiaTriTheDen.Valid"));

            //  RuleFor(x => x.MienCungChiTra)
            //  .Length(8).WithMessage(iLocalizationService.GetResource("BHYT.MienCungChiTra.Length")).When(x => !string.IsNullOrEmpty(x.MienCungChiTra));

            //  RuleFor(x => x.MaBenhKhac)
            //.MustAsync(async (model, input, s) => !await _BHYTService.CheckValidMaBenhKhac(input).ConfigureAwait(false))
            //.WithMessage(iLocalizationService.GetResource("BHYT.MaBenhKhac.Valid"));
            //  RuleFor(x => x.MaNoiChuyen)
            // .Length(5).WithMessage(iLocalizationService.GetResource("BHYT.MaNoiChuyen.Length")).When(x => !string.IsNullOrEmpty(x.MaNoiChuyen));

            //  RuleFor(x => x.NgayThanhToanTime)
            //.MustAsync(async (model, input, s) => !await _BHYTService.CheckValidNgayVaoRa(input).ConfigureAwait(false))
            //.WithMessage(iLocalizationService.GetResource("BHYT.NgayThanhToanTime.Valid"));
            //  RuleFor(x => x.NgayThanhToanTime)
            //  .GreaterThanOrEqualTo(x => x.NgayRaTime).WithMessage(iLocalizationService.GetResource("BHYT.NgayThanhToanTime.LessThan")).When(x => x.NgayThanhToanTime != null);

            // RuleFor(x => x.MaPhauThuatQuocTe)
            //.Length(3, 255).WithMessage(iLocalizationService.GetResource("BHYT.MaPhauThuatQuocTe.Length")).When(x => !string.IsNullOrEmpty(x.MaPhauThuatQuocTe));

            //RuleForEach(x => x.HoSoChiTietThuoc).SetValidator(hosothuocValidator);
            //RuleForEach(x => x.HoSoChiTietDVKT).SetValidator(hosoDVKTValidator);
            //RuleForEach(x => x.HoSoCanLamSang).SetValidator(canlamsangValidator);
            //RuleForEach(x => x.HoSoChiTietDienBienBenh).SetValidator(dienbienBenhValidator);
        }
    }
}
