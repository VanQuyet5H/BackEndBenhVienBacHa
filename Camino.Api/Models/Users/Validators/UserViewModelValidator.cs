using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using Camino.Services.Users;
using FluentValidation;

namespace Camino.Api.Models.Users.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<UserViewModel>))]
    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator(ILocalizationService localizationService, IUserService _userService)
        {


            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage(localizationService.GetResource("User.Phone.Required"))
                 .NotNull().WithMessage(localizationService.GetResource("User.Phone.Required"))
              .MustAsync(async (request, sdt, id) =>
               {
                   var val = await _userService.CheckIsExistPhone(sdt.RemoveFormatPhone(), request.Id);
                   return val;
               }).WithMessage(localizationService.GetResource("User.Phone.Exists"));



            RuleFor(x => x.Email)
          .EmailAddress().When(email => email.Email != "").WithMessage(localizationService.GetResource("User.Email.WrongEmail"))

           .MustAsync(async (request, email, id) =>
           {
               var val = await _userService.CheckIsExistEmail(email,  request.Id);
               return val;
           }).WithMessage(localizationService.GetResource("User.Email.Exists"));

        }
    }
}
