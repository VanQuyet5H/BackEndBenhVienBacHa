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
    [TransientDependency(ServiceType = typeof(IValidator<PhieuNhapKhoMauChiTietViewModel>))]
    public class PhieuNhapKhoMauChiTietViewModelValidator : AbstractValidator<PhieuNhapKhoMauChiTietViewModel>
    {
        public PhieuNhapKhoMauChiTietViewModelValidator(ILocalizationService localizationService, INhapKhoMauService _nhapKhoMauService, IValidator<KetQuaXetNghiemKhac> ketQuaXetNghiemKhacValidator)
        {
            RuleFor(x => x.MauVaChePhamId)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.MauVaChePhamId.Required"));
            RuleFor(x => x.MaTuiMau)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.MaTuiMau.Required"))
                .MustAsync(async (viewModel,input,d) => !await _nhapKhoMauService.KiemTraTrungMaTuiMauAsync(input, viewModel.Id, viewModel.MaTuiMauDangNhaps))
                    .WithMessage(localizationService.GetResource("NhapKhoMau.MaTuiMau.IsExists"));
            RuleFor(x => x.NgaySanXuat)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.NgaySanXuat.Required"))
                .LessThanOrEqualTo(DateTime.Now.Date).WithMessage(localizationService.GetResource("NhapKhoMau.NgaySanXuat.Range"));
            RuleFor(x => x.HanSuDung)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.HanSuDung.Required"))
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage(localizationService.GetResource("NhapKhoMau.HanSuDung.Range"));
            RuleFor(x => x.YeuCauTruyenMauId)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.YeuCauTruyenMauId.Required"))
                .MustAsync(async (viewModel, input, d) => !await _nhapKhoMauService.KiemTraTrungYeuCauTruyenMauAsync(input, viewModel.Id, viewModel.YeuCauTruyenMauIdDangChons))
                .WithMessage(localizationService.GetResource("NhapKhoMau.YeuCauTruyenMauId.IsExists"));

            RuleForEach(x => x.KetQuaXetNghiemKhacs)
                .SetValidator(ketQuaXetNghiemKhacValidator);
        }
    }
}
