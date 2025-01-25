using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LinhBuKSNK.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauKSNKBenhViensViewModel>))]

    public class YeuCauKSNKBenhVienViewModelValidator : AbstractValidator<YeuCauKSNKBenhViensViewModel>
    {
        public YeuCauKSNKBenhVienViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SLYeuCauLinhThucTe)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                 .Must((viewModel, soLuongTon, id) =>
                 {
                     if (viewModel.SLYeuCauLinhThucTe != null && viewModel.SLYeuCauLinhThucTe != 0 && viewModel.SLYeuCauLinhThucTe > viewModel.SLYeuCauLinhThucTeMax && viewModel.CheckBox)
                         return false;
                     return true;
                 })
                .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.OutRegulated"));
        }
    }
}
