using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiepNhanDichVuChiDinhViewModel>))]
    public class TiepNhanDichVuChiDinhViewModelValidator : AbstractValidator<TiepNhanDichVuChiDinhViewModel>
    {
        public TiepNhanDichVuChiDinhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DichVuBenhVienId).NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.DichVuBenhVienId.Required"));
            RuleFor(x => x.LoaiGiaId).NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.LoaiGiaId.Required"));
            RuleFor(x => x.SoLan).NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.SoLan.Required"));
            RuleFor(x => x.DonGiaMoi).NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.DonGiaMoi.Required"));
            RuleFor(x => x.NoiThucHienId).NotEmpty()
                .WithMessage(localizationService.GetResource("TiepNhanKhamDoan.NoiThucHienId.Required"));
        }
    }
}
