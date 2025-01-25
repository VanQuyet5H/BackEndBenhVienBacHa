using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DichVuKyThuat;
using Camino.Services.Localization;
using FluentValidation;
namespace Camino.Api.Models.DichVuKyThuatBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuKyThuatViewModel>))]

    public class DichVuKyThuatViewModelValidator : AbstractValidator<DichVuKyThuatViewModel>
    {
        public DichVuKyThuatViewModelValidator(ILocalizationService localizationService, IDichVuKyThuatService dichVuKyThuatService
            , IValidator<DichVuKyThuatThongTinGiaViewModel> dichVuKyThuatThongTinGiaValidator)
        {

            RuleFor(x => x.MaChung)
             .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
             .MustAsync(async (request, ma, id) =>
             {
                 return await dichVuKyThuatService.KiemTraTrungMaHoacTen(request.MaChung, request.Id, false);
             })
             .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.TenChung)
               .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
               .MustAsync(async (request, ten, id) =>
               {
                   return await dichVuKyThuatService.KiemTraTrungMaHoacTen(request.TenChung, request.Id, true);
               })
               .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.MaGia)
               .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuat.MaGia.Required"));

            RuleFor(x => x.NhomChiPhi)
              .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuat.NhomChiPhi.Required"));

            RuleFor(x => x.NhomDichVuKyThuatId)
              .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.NhomDichVuBenhVienId.Required"));

            RuleForEach(x => x.DichVuKyThuatThongTinGias).SetValidator(dichVuKyThuatThongTinGiaValidator);
        }
    }
}
