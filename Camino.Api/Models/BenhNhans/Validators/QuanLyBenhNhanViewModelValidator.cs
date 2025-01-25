using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BenhNhans;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Helpers;

namespace Camino.Api.Models.BenhNhans.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<QuanLyBenhNhanViewModel>))]
    public class QuanLyBenhNhanViewModelValidator : AbstractValidator<QuanLyBenhNhanViewModel>
    {
        public QuanLyBenhNhanViewModelValidator(ILocalizationService iLocalizationService, IBenhNhanService benhNhanService)
        {
            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required"));

            RuleFor(x => x.BHYTMaSoThe)
                .Length(0, 20).WithMessage(iLocalizationService.GetResource("BenhNhan.BHYTMaSoThe.Range"))
                .NotEmpty().When(x => x.CoBHYT == true)
                .WithMessage(iLocalizationService.GetResource("Common.BHYTMaSoThe.Required"));

            //RuleFor(x => x.SoChungMinhThu)
            //   .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.SoCMT.Required"))
            //   .Must((model, input, s) => string.IsNullOrEmpty(input) || (!string.IsNullOrEmpty(input) && input.Length >= 9 && input.Length <= 12)).WithMessage(iLocalizationService.GetResource("Common.SoCMT.Length"));

            RuleFor(x => x.Email)
            .Must((model, s) => CommonHelper.IsMailValid(model.Email) == true).WithMessage(iLocalizationService.GetResource("Common.WrongEmail"));

            RuleFor(x => x.NguoiLienHeEmail)
            .Must((model, s) => CommonHelper.IsMailValid(model.NguoiLienHeEmail) == true).WithMessage(iLocalizationService.GetResource("Common.WrongEmail"));

            //RuleFor(x => x.NgayThangNamSinh)
            //    .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.NgaySinh.Required"))
            //       .Must((request, ngay, id) =>
            //       {
            //           if (ngay == null)
            //               return false;
            //           return true;
            //       }).WithMessage(iLocalizationService.GetResource("Common.NgaySinh.Required"))
            //       .Must((request, ngay, id) =>
            //        {
            //            if (ngay != null && ngay.Value.Date >= DateTime.Now.Date)
            //                return false;
            //            return true;
            //        }).WithMessage(iLocalizationService.GetResource("Common.NgaySinh.LessThanNow"));

            RuleFor(x => x.NamSinh)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("TiepNhanBenhNhan.NamSinh.Required"))
                .When(x => x.NgayThangNamSinh != null)
                ;

            RuleFor(x => x.NamSinh)
                .Must((model, s) => model.NamSinh <= DateTime.Now.Year)
                .WithMessage(iLocalizationService.GetResource("TiepNhanBenhNhan.NamSinhMoreThanNow.Required"))
                .When(x => x.NamSinh != null)
                ;

            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.Required"))
                .When(x => x.NamSinh == null)
                ;

            RuleFor(x => x.NgayThangNamSinh).Must((model, s) => model.NgayThangNamSinh.GetValueOrDefault() <= DateTime.Now.Date)
                .WithMessage(iLocalizationService.GetResource("TiepNhanBenhNhan.NgayThangNamSinh.DatetimeNow"))
                .When(x => x.NgayThangNamSinh != null);

            //RuleFor(x => x.SoDienThoai)
            //    .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.SoDienThoai.Required"))
            //         .Must((request, sdt) =>
            //         {
            //             if (sdt != null && sdt.Length < 10 && sdt.Length != 0)
            //                 return false;
            //             return true;
            //         }).WithMessage(iLocalizationService.GetResource("Common.SoDienThoai.Range"));

            //RuleFor(x => x.GioiTinh)
            //    .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Sex.Required"));

        }


    }
}
