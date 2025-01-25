using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DinhMucVatTuTonKhos;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DinhMucVatTuTonKhos.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DinhMucVatTuTonKhoViewModel>))]
    public class DinhMucVatTuTonKhoViewModelValidator : AbstractValidator<DinhMucVatTuTonKhoViewModel>
    {
        public DinhMucVatTuTonKhoViewModelValidator(ILocalizationService localizationService, IDinhMucVatTuTonKhoService dinhMucVatTuTonKhoService)
        {
            RuleFor(x => x.VatTuBenhVienId)
                .MustAsync(async (request, vatTuId, input) =>
                {
                    var checkExistsTen = await dinhMucVatTuTonKhoService.IsTenVatTuExists(vatTuId.GetValueOrDefault(), request.Id, request.KhoId.GetValueOrDefault());
                    return !checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Kho.TenVatTu.Exists"));
        }
    }
}
