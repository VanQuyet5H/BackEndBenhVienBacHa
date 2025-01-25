using Camino.Api.Models.DichVuKhamBenh;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DichVuKhamBenhBenhViens;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKhamBenhBenhViens.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuKhamBenhBenhVienViewModel>))]
    public class DichVuKhamBenhBenhVienViewModelValidator : AbstractValidator<DichVuKhamBenhBenhVienViewModel>
    {
        public DichVuKhamBenhBenhVienViewModelValidator(ILocalizationService localizationService, IDichVuKhamBenhBenhVienService _dichVuKhamBenhBenhVienService, IValidator<DichVuKhamBenhBenhVienGiaBaoHiemViewModel> giaBaoHiemValidator, IValidator<DichVuKhamBenhBenhVienGiaBenhVienViewModel>giaBenhVienValidator)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));

            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .MustAsync(async (s, input, a) => !await _dichVuKhamBenhBenhVienService.IsExistsMaDichVuKhamBenhBenhVien(s.Id, input)).WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));
            RuleFor(x => x.DichVuKhamBenhId)
                .Must((s, d) => !(s.AnhXa != null && s.AnhXa.Value && s.DichVuKhamBenhId == null)).WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienId.Required"));

            RuleFor(x => x.KhoaPhongIds)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.KhoaPhongIds.Required"));
            RuleFor(x => x.NoiThucHienIds)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKhamBenhBenhVien.NoiThucHienIds.Required"));

            RuleForEach(x => x.DichVuKhamBenhBenhVienGiaBaoHiems).SetValidator(giaBaoHiemValidator);
            RuleForEach(x => x.DichVuKhamBenhBenhVienGiaBenhViens).SetValidator(giaBenhVienValidator);

        }
    }
}
