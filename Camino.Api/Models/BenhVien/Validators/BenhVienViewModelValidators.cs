using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BenhVien;
using Camino.Services.Localization;
using FluentValidation;


namespace Camino.Api.Models.BenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BienVienViewModel>))]
    public class BenhVienViewModelValidators : AbstractValidator<BienVienViewModel>
    {
        public BenhVienViewModelValidators(ILocalizationService iLocalizationService, IBenhVienService benhVienService)
        {
            RuleFor(x => x.Ma)
               .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required"))
               .Length(0, 20).WithMessage(iLocalizationService.GetResource("Common.Ma.Range"))
                .Must((model, s) => !benhVienService.CheckMaSoBenhVienExits(model.Ma, model.Id))
                .WithMessage(iLocalizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(iLocalizationService.GetResource("Common.Ten.Range"))
                .Must((model, s) => !benhVienService.CheckTenBenhVienExits(model.Ten, model.Id,true))
                    .WithMessage(iLocalizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.TenVietTat)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.TenVietTat.Required"))
                .Length(0, 50).WithMessage(iLocalizationService.GetResource("Common.TenVietTat.Range"))
                .Must((model, s) => !benhVienService.CheckTenBenhVienExits(model.TenVietTat, model.Id,false))
                    .WithMessage(iLocalizationService.GetResource("Common.TenVietTat.Exists"));

            RuleFor(x => x.DiaChi)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.DiaChi.Required"))
                .Length(0, 250).WithMessage(iLocalizationService.GetResource("Common.DiaChi.Range"));

            RuleFor(x => x.DonViHanhChinhId)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.DonViHanhChinh.Required"));

            RuleFor(x => x.LoaiBenhVienId)
             .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.LoaiBenhVien.Required"));

            RuleFor(x => x.SoDienThoaiLanhDao)
                .Length(0, 20).WithMessage(iLocalizationService.GetResource("Common.SoDienThoai.Range"))
                .Must((model,d) => string.IsNullOrEmpty(model.SoDienThoaiLanhDao) || (!string.IsNullOrEmpty(model.SoDienThoaiLanhDao) && model.SoDienThoaiLanhDao.Length >= 10 && model.SoDienThoaiLanhDao.Length <= 20)).WithMessage(iLocalizationService.GetResource("BenhVien.SoDienThoaiLanhDao.LessRange"));

            RuleFor(x => x.HangBenhVien)
             .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.HangBenhVien.Required"));

            RuleFor(x => x.TuyenChuyenMonKyThuat)
             .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.TuyenChuyenMonKyThuat.Required"));
        }
    }
}
