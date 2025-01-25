using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DuocPhamBenhVien.Validators
{
    //[TransientDependency(ServiceType = typeof(IValidator<DuocPhamBenhVienGiaBaoHiemViewModel>))]
    //public class DuocPhamBenhVienGiaBaoHiemViewModelValidator : AbstractValidator<DuocPhamBenhVienGiaBaoHiemViewModel>
    //{
    //    public DuocPhamBenhVienGiaBaoHiemViewModelValidator(ILocalizationService localizationService)
    //    {
    //        RuleFor(x => x.Gia)
    //            .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVien.Gia.Required"));
    //        RuleFor(x => x.TiLeBaoHiemThanhToan)
    //         .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVienGiaBaoHiem.TiLeBaoHiemThanhToan.Required"));
    //        RuleFor(x => x.TuNgay)
    //            .NotEmpty().WithMessage(localizationService.GetResource("DuocPhamBenhVien.TuNgay.Required"));
    //    }
    //}
}
