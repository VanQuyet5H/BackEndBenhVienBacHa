using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel>))]
    public class KhamBenhYeuCauTiepNhanCongTyBaoHiemTuNhanViewModelValidator : AbstractValidator<KhamBenhYeuCauTiepNhanCongTyBaoHiemTuNhanViewModel>
    {
        public KhamBenhYeuCauTiepNhanCongTyBaoHiemTuNhanViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CongTyBaoHiemTuNhanId)
                .NotEmpty().WithMessage(localizationService.GetResource("themBaoHiemTuNhanModel.CongTyBaoHiemTuNhanId.Required"));
            RuleFor(x => x.NgayHetHan)
                .Must((model, input, s) => (model.NgayHieuLuc == null || (model.NgayHieuLuc.Value.Date < model.NgayHetHan.Value.Date)))
                .WithMessage(localizationService.GetResource("themBaoHiemTuNhanModel.NgayHetHan.MoreThanTuNgay")).When(p => p.NgayHetHan != null && p.NgayHetHan.Value.Date > DateTime.Now.Date)
                .GreaterThan(DateTime.Now.Date).WithMessage(localizationService.GetResource("themBaoHiemTuNhanModel.NgayHetHan.MoreThanDatetimeNow"));
        }
    }
}
