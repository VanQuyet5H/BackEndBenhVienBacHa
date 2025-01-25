using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;


namespace Camino.Api.Models.LichKhoaPhong.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LichKhoaPhongViewModel>))]
    public class LichKhoaPhongViewModelValidators : AbstractValidator<LichKhoaPhongViewModel>
    {
        public LichKhoaPhongViewModelValidators(ILocalizationService localizationService, 
                                                IValidator<NhanVienLichPhanCongViewModel> nhanVienLichPhanCongViewModelValidator)
        {
            RuleForEach(x => x.BacSis).SetValidator(nhanVienLichPhanCongViewModelValidator);
            RuleForEach(x => x.YTas).SetValidator(nhanVienLichPhanCongViewModelValidator);
        }
    }


}
