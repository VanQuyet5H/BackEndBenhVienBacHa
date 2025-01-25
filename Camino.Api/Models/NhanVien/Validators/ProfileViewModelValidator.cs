using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using Camino.Services.NhanVien;
using Camino.Services.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhanVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ProfileNhanVien>))]
    public class ProfileViewModelValidator : AbstractValidator<ProfileNhanVien>
    {
        public ProfileViewModelValidator(ILocalizationService localizationService, INhanVienService _nhanvienSevice, IUserService _userService)
        {
            RuleFor(x => x.Password)
               .MinimumLength(6).WithMessage(localizationService.GetResource("Common.Password.Range.Min"))
               .MaximumLength(100).WithMessage(localizationService.GetResource("Common.Password.Range")).When(p => !string.IsNullOrEmpty(p.Password));

            RuleFor(x => x.PasswordConfirm)              
                .Must((model, input, s) =>
                {
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        return true;
                    }
                    else  if ((model.Password != model.PasswordConfirm))
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("NhanVien.PasswordConfirm.DontMatchPassword"));

            RuleFor(x => x.HoTen)
              .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));

            RuleFor(x => x.NgaySinh)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.NgaySinh.Required"))
               .Must((request, ngay, id) =>
               {
                   if (ngay == null)
                       return false;
                   return true;
               }).WithMessage(localizationService.GetResource("Common.NgaySinh.Required"))
                .Must((request, ngay, id) =>
                {
                    if (ngay != null && ngay.Value.Date >= DateTime.Now.Date)
                        return false;
                    return true;
                }).WithMessage(localizationService.GetResource("Common.NgaySinh.LessThanNow"));

            RuleFor(x => x.NgayHetHopDong)
                 .Must((request, ngay, id) =>
                 {
                     if (request.NgayKyHopDong == null && ngay != null)
                         return false;
                     return true;
                 }).WithMessage(localizationService.GetResource("Common.NgayKyHopDong.NotCurent"))
                .Must((request, ngay, id) =>
                {
                    if (request.NgayKyHopDong != null)
                        if (ngay != null && ngay.Value.Date <= request.NgayKyHopDong.Value.Date)
                            return false;
                    return true;
                }).WithMessage(localizationService.GetResource("Common.NgayHetHopDong.LessThanNgayKyHopDong"));


            RuleFor(x => x.SoDienThoai)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.SoDienThoai.Required"))
                .Must((request, sdt) =>
                {
                    if (sdt != null && sdt.Length < 10 && sdt.Length != 0)
                        return false;
                    return true;
                }).WithMessage(localizationService.GetResource("Common.SoDienThoai.Range"))
               .MustAsync(async (request, sdt, id) =>
               {
                   var val = await _nhanvienSevice.CheckIsExistPhone(sdt.RemoveFormatPhone(), request.Id);
                   return val;
               }).WithMessage(localizationService.GetResource("Common.SoDienThoai.Exists"));


            //RuleFor(x => x.DiaChi)
            // .NotEmpty().WithMessage(localizationService.GetResource("Common.DiaChi.Required"));

            RuleFor(x => x.Email)
            .EmailAddress().When(email => email.Email != "").WithMessage(localizationService.GetResource("Common.WrongEmailFormat"))
            .MustAsync(async (request, email, id) =>
            {
                var val = await _nhanvienSevice.CheckIsExistEmail(email, request.Id);
                return val;
            }).WithMessage(localizationService.GetResource("Common.Email.Exists"))
             .NotEmpty().WithMessage(localizationService.GetResource("Auth.ForgotPassword.Email.Required"));

            //RuleFor(x => x.SoCMT)
            //     .MustAsync(async (request, cmt, id) =>
            //     {
            //         var val = await _nhanvienSevice.CheckIsExistChungMinh(cmt, request.Id);
            //         return val;
            //     }).When(x => !string.IsNullOrWhiteSpace(x.SoCMT)).WithMessage(localizationService.GetResource("Common.SoCMT.Exists"))
            //       .NotEmpty().WithMessage(localizationService.GetResource("Common.SoCMT.Required"))
            //       .Must((model, input, s) => string.IsNullOrEmpty(input) || (!string.IsNullOrEmpty(input) && input.Length >= 9 && input.Length <= 12)).WithMessage(localizationService.GetResource("Common.SoCMT.Length"));


            RuleFor(x => x.GioiTinh)
             .NotEmpty().WithMessage(localizationService.GetResource("Common.Sex.Required"));

            RuleFor(x => x.KhoaPhongIds)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.KhoaPhongIds.Required"));

            //RuleFor(x => x.MaChungChiHanhNghe)
            //   .MustAsync(async (model, input, s) => !await _nhanvienSevice.CheckMaChungChiAsync(model.MaChungChiHanhNghe, model.Id).ConfigureAwait(false))
            //   .WithMessage(localizationService.GetResource("Common.MaChungChiHanhNghe.Exists"));


        }
    }
}
