using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.KhoDuocPhamViTri;
using Camino.Services.Localization;

namespace Camino.Api.Models.KhoDuocPhamViTri.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhoDuocPhamViTriViewModel>))]
    public class KhoDuocPhamViTriViewModelValidator : AbstractValidator<KhoDuocPhamViTriViewModel>
    {
    public KhoDuocPhamViTriViewModelValidator(ILocalizationService iLocalizationService, IKhoduocPhamViTriService iKhoDuocPhamViTriService)
    {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) =>
                {
                    var val = await iKhoDuocPhamViTriService.IsTenExists(ten, request.Id);
                    return val;
                }).WithMessage(iLocalizationService.GetResource("Common.Ten.Exists")).Length(0, 250).WithMessage(iLocalizationService
                    .GetResource("Common.Ten.Range"));
            RuleFor(x => x.KhoDuocPhamId)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.KhoDuocPhamId.Required"));
        }
    }
}
