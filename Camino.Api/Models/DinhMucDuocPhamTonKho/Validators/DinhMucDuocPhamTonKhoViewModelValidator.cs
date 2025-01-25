using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.DinhMucDuocPhamTonKho;

namespace Camino.Api.Models.DinhMucDuocPhamTonKho.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DinhMucDuocPhamTonKhoViewModel>))]

    public class DinhMucDuocPhamTonKhoViewModelValidator : AbstractValidator<DinhMucDuocPhamTonKhoViewModel>
    {
        public DinhMucDuocPhamTonKhoViewModelValidator(ILocalizationService localizationService, IDinhMucDuocPhamTonKhoService dinhMucDuocPhamTonKhoService)
        {
            RuleFor(x => x.DuocPhamBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("Kho.TenDuocPham.Required")).MustAsync(async (request, duocPhamId, input) =>
                {
                    if (duocPhamId == null)
                    {
                        duocPhamId = 0;
                    }
                    var checkExistsTen = await dinhMucDuocPhamTonKhoService.IsTenExists(duocPhamId.Value, request.Id, request.KhoDuocPhamId.GetValueOrDefault());
                    return !checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Kho.TenDuocPham.Exists"));
            RuleFor(x => x.KhoDuocPhamId)
                .NotEmpty().WithMessage(localizationService.GetResource("Kho.KhoDuocPham.Required"));
            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));
        }
    }
}
