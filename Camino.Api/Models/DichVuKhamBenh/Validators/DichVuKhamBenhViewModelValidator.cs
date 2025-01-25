using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.DichVuKhamBenh;

namespace Camino.Api.Models.DichVuKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuKhamBenhViewModel>))]
    public class DichVuKhamBenhViewModelValidator : AbstractValidator<DichVuKhamBenhViewModel>
    {
        public DichVuKhamBenhViewModelValidator(ILocalizationService localizationService, IDichVuKhamBenhService dichVuKhamBenhService, IValidator<DichVuKhamBenhThongTinGiaViewModel> dichVuKhamBenhThongTinGiaValidator)
        {
            RuleFor(x => x.MaChung)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required")).MustAsync(async (request, ten, id) =>
               {
                   return await dichVuKhamBenhService.KiemTraTrungMaHoacTen(request.MaChung, request.Id, false);
               })
               .WithMessage(localizationService.GetResource("Common.Ma.Exists"));


            RuleFor(x => x.TenChung)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) =>
                {
                    return await dichVuKhamBenhService.KiemTraTrungMaHoacTen(request.TenChung, request.Id, true);
                })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.MaTT37)
              .NotEmpty().WithMessage(localizationService.GetResource("Common.MaTT37.Required"));

            RuleFor(x => x.HangBenhVien)
             .NotEmpty().WithMessage(localizationService.GetResource("Common.HangBenhVien.Required"));

            RuleForEach(x => x.DichVuKhamBenhThongTinGias).SetValidator(dichVuKhamBenhThongTinGiaValidator);

        }
    }
}
