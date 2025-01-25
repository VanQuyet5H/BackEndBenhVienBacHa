using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NoiTruYeuCauTiepNhanTheBHYTViewModel>))]
    public class NoiTruYeuCauTiepNhanTheBHYTViewModelValidator : AbstractValidator<NoiTruYeuCauTiepNhanTheBHYTViewModel>
    {
        public NoiTruYeuCauTiepNhanTheBHYTViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.MaSoThe)
                .NotEmpty().WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTMaSoThe.Required"));
            RuleFor(x => x.MaDKBD)
                .NotEmpty().WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTMaDKBD.Required"));
            RuleFor(x => x.MucHuong)
                .NotEmpty().WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTMucHuong.Required"));
            RuleFor(x => x.DiaChi)
                .NotEmpty().WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTDiaChi.Required"));
            RuleFor(x => x.NgayHieuLuc)
                .NotEmpty().WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTNgayHieuLuc.Required"));
            RuleFor(x => x.NgayHetHan)
                .NotEmpty().WithMessage(localizationService.GetResource("ThongTinDoiTuongTiepNhan.BHYTNgayHetHan.Required"));
        }
    }
}
