using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HoSoKhacGayMeGayTeViewModel>))]

    public class HoSoKhacGayMeGayTeViewModelValidator : AbstractValidator<HoSoKhacGayMeGayTeViewModel>
    {
        public HoSoKhacGayMeGayTeViewModelValidator(ILocalizationService localizationService, IValidator<ThongTinQuanHeThanNhanVo> thongTinQHTNValidator)
        {
            RuleFor(x => x.NhanVienThucHienId)
               .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NhanVienThucHienId.Required"));

            RuleFor(x => x.NhanVienGiaiThichId)
               .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NhanVienGiaiThichId.Required"));

            RuleFor(x => x.ThoiDiemThucHien)
            .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.ThoiDiemThucHien.Required"));

            RuleForEach(x => x.ThongTinQuanHeThanNhans).SetValidator(thongTinQHTNValidator);

        }
    }
}
