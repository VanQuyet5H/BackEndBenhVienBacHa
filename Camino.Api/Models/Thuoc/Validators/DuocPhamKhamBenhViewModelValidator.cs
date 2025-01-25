using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.Thuocs;
using FluentValidation;

namespace Camino.Api.Models.Thuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuocPhamKhamBenhViewModel>))]

    public class DuocPhamKhamBenhViewModelValidator : AbstractValidator<DuocPhamKhamBenhViewModel>
    {
        public DuocPhamKhamBenhViewModelValidator(ILocalizationService localizationService, IDuocPhamService thuocBenhVienService)
        {

            RuleFor(x => x.Ten)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));
               //.MustAsync(async (model, ten, id) =>
               //{
               //    var checkExistsTen = await thuocBenhVienService.IsTenExists(ten, model.Id);
               //    return checkExistsTen;
               //})
               //.WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.DuongDungId)
             .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.DuongDung.Required"));

            RuleFor(x => x.DonViTinhId)
              .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.DonViTinh.Required"));

            RuleFor(x => x.SoLuong)
              .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"));
        }
    }
}
