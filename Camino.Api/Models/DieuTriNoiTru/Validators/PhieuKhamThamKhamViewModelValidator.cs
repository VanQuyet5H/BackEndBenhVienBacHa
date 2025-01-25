using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuKhamThamKhamViewModel>))]
    public class PhieuKhamThamKhamViewModelValidator : AbstractValidator<PhieuKhamThamKhamViewModel>
    {
        public PhieuKhamThamKhamViewModelValidator(ILocalizationService localizationService, 
            IValidator<PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModel> noiTruThamKhamChanDoanKemTheoValidator
            , IValidator<PhieuThamKhamDienBienViewModel> phieuThamKhamDienBienViewModelValidator
            , IValidator<PhieuThamKhamKetQuaSinhHieuViewModel> ketQuaSHViewModelValidator
            , IValidator<ThoiGianDieuTriSoSinhViewModel> thoiGianDieuTriSoSinhViewModelsValidator)
        {
            //RuleFor(x => x.KetQuaSinhHieus).SetValidator(benhNhanValidator);
            RuleForEach(x => x.NoiTruThamKhamChanDoanKemTheos).SetValidator(noiTruThamKhamChanDoanKemTheoValidator);

            RuleForEach(x => x.DienBiens).SetValidator(phieuThamKhamDienBienViewModelValidator);
            RuleForEach(x => x.KetQuaSinhHieus).SetValidator(ketQuaSHViewModelValidator);

            RuleForEach(x => x.ThoiGianDieuTriSoSinhViewModels).SetValidator(thoiGianDieuTriSoSinhViewModelsValidator);
        }
    }
}
