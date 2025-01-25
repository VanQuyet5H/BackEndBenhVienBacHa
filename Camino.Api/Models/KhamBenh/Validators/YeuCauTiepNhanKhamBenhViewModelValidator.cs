using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauTiepNhanKhamBenhViewModel>))]
    public class YeuCauTiepNhanKhamBenhViewModelValidator : AbstractValidator<YeuCauTiepNhanKhamBenhViewModel>
    {
        public YeuCauTiepNhanKhamBenhViewModelValidator(ILocalizationService localizationService, IValidator<KhamBenhBenhNhanViewModel> benhNhanValidator)
        {
            //RuleFor(x => x.KSKKetQuaCanLamSang)
            //    .Must((model, input) => model.IsKhamDoanTatCa != true || (model.IsKhamDoanTatCa == true && !string.IsNullOrEmpty(input)))
            //    .WithMessage(localizationService.GetResource("KhamDoan.KSKKetQuaCanLamSang.Required"));
            //RuleFor(x => x.KSKDanhGiaCanLamSang)
            //    .Must((model, input) => model.IsKhamDoanTatCa != true || (model.IsKhamDoanTatCa == true && !string.IsNullOrEmpty(input)))
            //    .WithMessage(localizationService.GetResource("KhamDoan.KSKNhanVienDanhGiaCanLamSang.Required"));
            RuleFor(x => x.PhanLoaiSucKhoeId)
                //.Must((model, input) => model.IsKhamDoanTatCa != true || (model.IsKhamDoanTatCa == true && input != null))
                .Must((model, input) => model.IsDuChuyenKhoaKhamSucKhoeChinh != true || (model.IsDuChuyenKhoaKhamSucKhoeChinh == true && input != null))
                .WithMessage(localizationService.GetResource("KhamDoan.PhanLoaiSucKhoeId.Required"));

            RuleFor(x => x.BenhNhan).SetValidator(benhNhanValidator);
        }
    }
}
