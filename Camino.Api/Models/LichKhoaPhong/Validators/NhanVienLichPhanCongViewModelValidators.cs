using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.LichPhanCongNgoaiTru;
using Camino.Services.Localization;
using FluentValidation;


namespace Camino.Api.Models.LichKhoaPhong.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhanVienLichPhanCongViewModel>))]
    public class NhanVienLichPhanCongViewModelValidators : AbstractValidator<NhanVienLichPhanCongViewModel>
    {
        public NhanVienLichPhanCongViewModelValidators(ILocalizationService localizationService, ILichPhanCongNgoaiTruService lichPhanCongService)
        {
            RuleFor(x => x.TenNhanVien)
                // .NotEmpty().WithMessage(localizationService.GetResource("LichPhanCong.NhanVien.Required"))
                .MustAsync(async (model, input, s) => !await lichPhanCongService.IsNhanVienIdExists(model.ListIdValidator, model.NhanVienId).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("LichPhanCong.NhanVienCaTruc.Exists"));
        }
    }
}
