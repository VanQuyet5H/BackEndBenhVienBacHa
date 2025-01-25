using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
namespace Camino.Api.Models.DichVuKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuKhamBenhThongTinGiaViewModel>))]

    public class DichVuKhamBenhThongTinGiaViewModelValidator : AbstractValidator<DichVuKhamBenhThongTinGiaViewModel>
    {
        public DichVuKhamBenhThongTinGiaViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Gia)
                   .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.Gia.Reqiured"));

            RuleFor(x => x.TuNgay)
                   .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.TuNgay.Required"));

            RuleFor(x => x.DenNgay)
              .Must((request, ten, id) =>
              {
                  if (request.TuNgay != null && request.DenNgay != null)
                  {
                      if (request.DenNgay < request.TuNgay)
                      {
                          return false;
                      }
                  }
                  return true;
              })
              .WithMessage(localizationService.GetResource("ChuongTrinhMarketing.DenNgayVoucher.NotGreaterThanTuNgay"));
        }
    }
}
