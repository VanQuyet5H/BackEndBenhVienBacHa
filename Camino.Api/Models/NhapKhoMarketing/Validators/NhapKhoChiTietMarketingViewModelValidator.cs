using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoMarketing.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DanhSachQuaTangDuocThem>))]
    public class NhapKhoChiTietMarketingViewModelValidator : AbstractValidator<DanhSachQuaTangDuocThem>
    {
        public NhapKhoChiTietMarketingViewModelValidator(ILocalizationService iLocalizationService)
        {
            RuleFor(x => x.SoLuongNhap)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.SoLuongNhap.Required"));
            RuleFor(x => x.DonGiaNhap)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.DonGiaNhap.Required"));
            RuleFor(x => x.NhaCungCap)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.NhaCungCap.Required"));
            RuleFor(x => x.QuaTangId)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.QuaTangId.Required"));

        }
    }
}
