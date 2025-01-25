using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.VatTuBenhViens;

namespace Camino.Api.Models.VatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<VatTuBenhViewModel>))]

    public class VatTuBenhVienModelValidator : AbstractValidator<VatTuBenhViewModel>
    {
        public VatTuBenhVienModelValidator(ILocalizationService localizationService, IVatTuBenhVienService vatTuBenhVienService)
        {
            RuleFor(x => x.MaVatTuBenhVien)
           .NotEmpty().WithMessage(localizationService.GetResource("VatTu.MaVatTuBenhVien.Required"))

           // BVHD-3472
           // mã bệnh viện tối thiểu 7 ký tự
           .Must((viewModel, input, d) => string.IsNullOrEmpty(input) || (!string.IsNullOrEmpty(input) && input.Length >= 7))
                .WithMessage(localizationService.GetResource("VatTuBenhVien.MaVatTuBenhVien.Length"))
           .MustAsync(async (viewModel, input, d) => !await vatTuBenhVienService.KiemTraTrungMaVatTuBenhVienAsync(viewModel.Id, input))
                .WithMessage(localizationService.GetResource("VatTuBenhVien.MaVatTuBenhVien.IsExists"));

            RuleFor(x => x.LoaiSuDung)
                .NotEmpty().WithMessage(localizationService.GetResource("VatTu.LoaiSuDung.Required"));
        }
    }
}
