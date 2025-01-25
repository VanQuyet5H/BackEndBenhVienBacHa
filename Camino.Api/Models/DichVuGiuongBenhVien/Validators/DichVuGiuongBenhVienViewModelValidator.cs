using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DichVuGiuongBenhVien;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DichVuGiuongBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuGiuongBenhVienViewModel>))]
    public class DichVuGiuongBenhVienViewModelValidator
        : AbstractValidator<DichVuGiuongBenhVienViewModel>
    {
        public DichVuGiuongBenhVienViewModelValidator(
            ILocalizationService localizationService
            , IDichVuGiuongBenhVienService _dichVuGiuongBenhVienService,
            IValidator<DichVuGiuongBenhVienGiaBaoHiemViewModel> giaBaoHiemValidator, IValidator<DichVuGiuongBenhVienGiaBenhVienViewModel> giaBenhVienValidator
        )
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .MustAsync(async (s, input, a) => !await _dichVuGiuongBenhVienService.IsExistsMaDichVuGiuongBenhVien(s.Id, input)).WithMessage(localizationService.GetResource("Common.Ma.Exists")); ;
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));

            RuleFor(x => x.DichVuGiuongId)
                .Must((s, d) => !(s.AnhXa != null && s.AnhXa.Value && s.DichVuGiuongId == null)).WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.DichVuGiuongId.Required"));

            RuleFor(x => x.LoaiGiuong)
                .NotEmpty().WithMessage(localizationService
                    .GetResource("DichVuGiuongBenhVien.LoaiGiuong.Required"));

            RuleFor(x => x.MoTa)
                .Length(0, 4000).WithMessage(localizationService
                    .GetResource("Common.MoTa.Range.4000"));

            //RuleFor(x => x.NoiThucHienIds)
            //    .NotEmpty().WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.NoiThucHienIds.Required"));
            //RuleFor(x => x.KhoaPhongIds)
            //    .NotEmpty().WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.KhoaPhongIds.Required"));

            RuleForEach(x => x.DichVuGiuongBenhVienGiaBaoHiems).SetValidator(giaBaoHiemValidator);
            RuleForEach(x => x.DichVuGiuongBenhVienGiaBenhViens).SetValidator(giaBenhVienValidator);
        }
    }
}
