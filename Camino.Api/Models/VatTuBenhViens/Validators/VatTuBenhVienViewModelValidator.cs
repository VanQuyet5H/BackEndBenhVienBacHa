using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.VatTuBenhViens;

namespace Camino.Api.Models.VatTuBenhViens.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<VatTuBenhVienViewModel>))]
    public class VatTuBenhVienViewModelValidator : AbstractValidator<VatTuBenhVienViewModel>
    {
        public VatTuBenhVienViewModelValidator(ILocalizationService localizationService, IVatTuBenhVienService vatTuBenhVienService)
        {
            RuleFor(x => x.MaVatTuBenhVien)
                .NotEmpty().WithMessage(localizationService.GetResource("VatTuBenhVien.MaVatTuBenhVien.Required"))

               // BVHD-3472
                // mã bệnh viện tối thiểu 7 ký tự
                .Must((viewModel, input, d) => string.IsNullOrEmpty(input) || (!string.IsNullOrEmpty(input) && input.Length >= 7))
                .WithMessage(localizationService.GetResource("VatTuBenhVien.MaVatTuBenhVien.Length"))
                .MustAsync(async (viewModel, input, d) => !await vatTuBenhVienService.KiemTraTrungMaVatTuBenhVienAsync(viewModel.Id, input))
                .WithMessage(localizationService.GetResource("VatTuBenhVien.MaVatTuBenhVien.IsExists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));

            RuleFor(x => x.NhomVatTuId)
                .NotEmpty().WithMessage(localizationService.GetResource("VatTu.NhomVatTuId.Required"));

            RuleFor(x => x.TyLeBaoHiemThanhToan)
                .NotEmpty().WithMessage(localizationService.GetResource("VatTu.TyLeBaoHiemThanhToan.Required"));

            RuleFor(x => x.LoaiSuDung)
              .NotEmpty().WithMessage(localizationService.GetResource("VatTuBenhVien.LoaiSuDung.Required"));
        }
    }
}
