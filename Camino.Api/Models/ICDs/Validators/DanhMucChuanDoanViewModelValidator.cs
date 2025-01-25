using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.ICDs;

namespace Camino.Api.Models.ICDs.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DanhMucChuanDoanViewModel>))]
    public class DanhMucChuanDoanViewModelValidator : AbstractValidator<DanhMucChuanDoanViewModel>
    {
        public DanhMucChuanDoanViewModelValidator(ILocalizationService localizationService, IDanhMucChuanDoanService danhMucChuanDoanService)
        {

            RuleFor(x => x.TenTiengViet)
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.TenTiengViet.Range"))
                .NotEmpty().WithMessage(localizationService.GetResource("Common.TenTiengViet.Required")).MustAsync(async (request, tenVi, id) =>
                {
                    var checkExistsTen = await danhMucChuanDoanService.IsTenViExists(tenVi, request.Id);
                    return checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.TenTiengViet.Exists"));

            RuleFor(x => x.TenTiengAnh)
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.TenTiengAnh.Range"))
                .NotEmpty().WithMessage(localizationService.GetResource("Common.TenTiengAnh.Required")).MustAsync(async (request, tenEng, id) =>
                {
                    var checkExistsTen = await danhMucChuanDoanService.IsTenEngExists(tenEng, request.Id);
                    return checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.TenTiengAnh.Exists"));
            RuleFor(x => x.GhiChu)
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.GhiChu.Range"));
        }
    }
}
