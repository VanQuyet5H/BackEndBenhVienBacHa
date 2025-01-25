using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhapKhoMau;
using FluentValidation;

namespace Camino.Api.Models.NhapKhoMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuyetPhieuNhapKhoMauChiTietViewModel>))]
    public class DuyetPhieuNhapKhoMauChiTietViewModelValidator : AbstractValidator<DuyetPhieuNhapKhoMauChiTietViewModel>
    {
        public DuyetPhieuNhapKhoMauChiTietViewModelValidator(ILocalizationService localizationService, INhapKhoMauService _nhapKhoMauService)
        {
            RuleFor(x => x.DonGiaNhap)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.DonGiaNhap.Required"))
                .Must((viewModel,input) => input == null || input > 0).WithMessage(localizationService.GetResource("NhapKhoMau.DonGiaNhap.Range"))
                .MustAsync(async (viewModel, input, d) => await _nhapKhoMauService.KiemTraCapNhatGiaNhapKhoMauChiTietAsync(input, viewModel.DonGiaBaoHiem, viewModel.Id))
                    .WithMessage(localizationService.GetResource("NhapKhoMau.YeuCauTruyenMau.DaThanhToan"));
            RuleFor(x => x.DonGiaBaoHiem)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.DonGiaBaoHiem.Required"))
                .Must((viewModel, input) => input == null || input > 0).WithMessage(localizationService.GetResource("NhapKhoMau.DonGiaBaoHiem.Range"));
            RuleFor(x => x.HanSuDung)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.HanSuDung.Required"));
        }
    }
}
