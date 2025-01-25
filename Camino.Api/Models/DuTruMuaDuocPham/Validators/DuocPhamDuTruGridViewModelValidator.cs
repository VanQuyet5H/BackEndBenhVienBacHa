using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauMuaDuTruDuocPham;
using FluentValidation;

namespace Camino.Api.Models.DuTruMuaDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuocPhamDuTruGridViewModel>))]

    public class DuocPhamDuTruGridViewModelValidator : AbstractValidator<DuocPhamDuTruGridViewModel>
    {
        public DuocPhamDuTruGridViewModelValidator(ILocalizationService localizationService, IYeuCauMuaDuTruDuocPhamService ycMuaDuocPhamService)
        {
            RuleFor(x => x.DuocPhamId)
               .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DuocPham.Required")).When(p => !p.IsThemDuocPham)
                .MustAsync(async (viewModel, soLuongTon, id) =>
                {
                    var kiemTraExists = await ycMuaDuocPhamService.CheckDuocPhamExists(viewModel.DuocPhamId, viewModel.LaDuocPhamBHYT, viewModel.YeuCauMuaThuocChiTietValidators);
                    return kiemTraExists;
                })
              .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.DuocPham.Exists")).When(p => !p.IsThemDuocPham);

            RuleFor(x => x.SoLuongDuTru)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.SoLuongDuTru.Required"));

            RuleFor(x => x.SoLuongDuKienSuDung)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.SoLuongDuKienSuDungTrongKy.Required"));


            RuleFor(x => x.Ten)
              .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).When(p => p.IsThemDuocPham);

            RuleFor(x => x.DuongDungId)
              .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.DuongDung.Required")).When(p => p.IsThemDuocPham);

            RuleFor(x => x.DonViTinhId)
              .NotEmpty().WithMessage(localizationService.GetResource("ThuocBenhVien.DonViTinh.Required")).When(p => p.IsThemDuocPham);
        }
    }
}
