using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DuocPhamBenhVienPhanNhoms;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DuocPhamBenhVienPhanNhoms.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuocPhamBenhVienPhanNhomViewModel>))]
    public class DuocPhamBenhVienPhanNhomViewModelValidator : AbstractValidator<DuocPhamBenhVienPhanNhomViewModel>
    {
        public DuocPhamBenhVienPhanNhomViewModelValidator(ILocalizationService localizationService, IDuocPhamBenhVienPhanNhomService duocPhamBenhVienPhanNhom)
        {
            RuleFor(p => p.NhomChaId)
                .MustAsync(async (model, input, s) => await duocPhamBenhVienPhanNhom
                    .CheckChiDinhVong(model.Id, model.NhomChaId).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("DuocPhamBv.Vong.NotAllow"));
        }
    }
}
