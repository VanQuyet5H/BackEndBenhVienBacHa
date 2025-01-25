using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using Camino.Services.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.Users.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<UserUpdateExternalViewModel>))]
    public class UserUpdateExternalViewModelValidator : AbstractValidator<UserUpdateExternalViewModel>
    {
        public UserUpdateExternalViewModelValidator(ILocalizationService localizationService, IUserService userService)
        {


            RuleFor(x => x.Phone)

              .MustAsync(async (request, sdt, id) =>
              {
                  var val = await userService.CheckIsExistPhone(sdt.RemoveFormatPhone(), request.Id);
                  return val;
              }).WithMessage(localizationService.GetResource("NhanVien.SoDienThoai.Exists"));


            RuleFor(x => x.Email)
          .EmailAddress().When(email => email.Email != "").WithMessage(localizationService.GetResource("User.Email.WrongEmail"))

           .MustAsync(async (request, email, id) =>
           {
               var val = await userService.CheckIsExistEmail(email, request.Id);
               return val;
           }).WithMessage(localizationService.GetResource("NhanVien.Email.Exists"));

        }
    }
}
