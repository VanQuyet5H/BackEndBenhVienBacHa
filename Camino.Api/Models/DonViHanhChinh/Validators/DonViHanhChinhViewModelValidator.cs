using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.DonViHanhChinh;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DonViHanhChinh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DonViHanhChinhViewModel>))]
    public class DonViHanhChinhModelValidator : AbstractValidator<DonViHanhChinhViewModel>
    {

        public DonViHanhChinhModelValidator(ILocalizationService localizationService, IDonViHanhChinhService donViHanhChinhService)
        {
            RuleFor(a => a.CapHanhChinh)
                .NotNull().WithMessage(localizationService.GetResource("DonViHanhChinh.CapHanhChinh.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("DonViHanhChinh.CapHanhChinh.Required"));

            When(a => a.CapHanhChinh.HasValue, () =>
            {
                RuleFor(a => a.Ma)
                   .NotNull().WithMessage(localizationService.GetResource("DonViHanhChinh.Ma.Required"))
                   .NotEmpty().WithMessage(localizationService.GetResource("DonViHanhChinh.Ma.Required"))
                   .Must((request, ma, id) =>
                   {
                       if (!string.IsNullOrEmpty(ma))
                       {
                           if (request.CapHanhChinh == CapHanhChinh.TinhThanhPho)
                               return donViHanhChinhService.CheckExistMaCapTinhThanhPho(ma, request.Id);
                           if (request.TrucThuocThanhPhoId.HasValue && request.CapHanhChinh == CapHanhChinh.QuanHuyen)
                               return donViHanhChinhService.CheckExistMaCapQuanHuyen(request.TrucThuocThanhPhoId.Value, ma, request.Id);
                           if (request.TrucThuocQuanHuyenId.HasValue && request.CapHanhChinh == CapHanhChinh.PhuongXa)
                               return donViHanhChinhService.CheckExistMaCapPhuongXa(request.TrucThuocQuanHuyenId.Value, ma, request.Id);
                       }
                       return true;
                   }).WithMessage(localizationService.GetResource("DonViHanhChinh.Ma.Exist"));

                RuleFor(a => a.Ten)
                    .NotNull().WithMessage(localizationService.GetResource("DonViHanhChinh.Ten.Required"))
                    .NotEmpty().WithMessage(localizationService.GetResource("DonViHanhChinh.Ten.Required"));
            });

            When(a => a.CapHanhChinh.HasValue && a.CapHanhChinh == CapHanhChinh.QuanHuyen, () =>
            {
                RuleFor(b => b.TrucThuocThanhPhoId)
                    .NotNull().WithMessage(localizationService.GetResource("DonViHanhChinh.TrucThuocThanhPhoId.Required"))
                    .NotEmpty().WithMessage(localizationService.GetResource("DonViHanhChinh.TrucThuocThanhPhoId.Required"));
            });

            When(a => a.CapHanhChinh.HasValue && a.CapHanhChinh == CapHanhChinh.PhuongXa, () =>
            {
                RuleFor(b => b.TrucThuocThanhPhoId)
                    .NotNull().WithMessage(localizationService.GetResource("DonViHanhChinh.TrucThuocThanhPhoId.Required"))
                    .NotEmpty().WithMessage(localizationService.GetResource("DonViHanhChinh.TrucThuocThanhPhoId.Required"));

                RuleFor(b => b.TrucThuocQuanHuyenId)
                    .NotNull().WithMessage(localizationService.GetResource("DonViHanhChinh.TrucThuocQuanHuyenId.Required"))
                    .NotEmpty().WithMessage(localizationService.GetResource("DonViHanhChinh.TrucThuocQuanHuyenId.Required"));
            });
        }
    }
}
