using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BienBanHoiChanSuDungThuocCoDau.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<BienBanHoiChanSuDungThuocCoDauViewModel>))]
    public class BienBanHoiChanSuDungThuocCoDauViewModelValidator : AbstractValidator<BienBanHoiChanSuDungThuocCoDauViewModel>
    {
        public BienBanHoiChanSuDungThuocCoDauViewModelValidator(ILocalizationService iLocalizationService)
        {
            RuleFor(x => x.HoiChanLuc)
            .NotEmpty().WithMessage(iLocalizationService.GetResource("DieuTriNoiTru.HoiChanLuc.Required"));
            RuleFor(x => x. LanhDao)
           .NotEmpty().WithMessage(iLocalizationService.GetResource("DieuTriNoiTru.LanhDao.Required"));
            RuleFor(x => x.BsDieuTri)
           .NotEmpty().WithMessage(iLocalizationService.GetResource("DieuTriNoiTru.BsDieuTri.Required"));
        }
    }
}
