using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.Thuocs;

namespace Camino.Api.Models.Thuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ADRViewModel>))]

    public class ADRViewModelValidator : AbstractValidator<ADRViewModel>
    {
        public ADRViewModelValidator(ILocalizationService localizationService, IADRService aDRService)
        {

            RuleFor(x => x.TuongTacHauQua)
                .Length(0, 250).WithMessage(localizationService.GetResource("Thuoc.TuongTacHauQua.Range"))
                .NotEmpty().WithMessage(localizationService.GetResource("Thuoc.TuongTacHauQua.Required"));

            RuleFor(x => x.ThuocHoacHoatChat1Id)
               .NotEmpty().WithMessage(localizationService.GetResource("Thuoc.TenHoatChat1.Required")).MustAsync(async (viewModel, Id, input) =>
               {
                   var checkExistsTen = await aDRService.IsTenHoatChatExists(viewModel.ThuocHoacHoatChat1Id, viewModel.ThuocHoacHoatChat2Id, viewModel.Id);
                   return !checkExistsTen;
               })
                .WithMessage(localizationService.GetResource("Thuoc.TenHoatChat.Exists"));

            RuleFor(x => x.ThuocHoacHoatChat2Id)
              .NotEmpty().WithMessage(localizationService.GetResource("Thuoc.TenHoatChat2.Required")).MustAsync(async (viewModel, Id, input) =>
              {
                  var checkExistsTen = await aDRService.IsTenHoatChatExists(viewModel.ThuocHoacHoatChat1Id, viewModel.ThuocHoacHoatChat2Id, viewModel.Id);
                  return !checkExistsTen;
              })
                .WithMessage(localizationService.GetResource("Thuoc.TenHoatChat.Exists"));

            RuleFor(x => x.CachXuLy)
                .Length(0, 1000).WithMessage(localizationService.GetResource("Thuoc.CachXuLy.Range"));

            RuleFor(x => x.GhiChu)
                .Length(0, 1000).WithMessage(localizationService.GetResource("Thuoc.GhiChu.Range"));
        }
    }
}
