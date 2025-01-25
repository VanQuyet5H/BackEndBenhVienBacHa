using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.Voucher;
using FluentValidation;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.Voucher.Validators
{
    //[TransientDependency(ServiceType = typeof(IValidator<VoucherChiTietMienGiamViewModel>))]
    //public class VoucherChiTietMienGiamViewModelValidator : AbstractValidator<VoucherChiTietMienGiamViewModel>
    //{
    //    public VoucherChiTietMienGiamViewModelValidator(ILocalizationService localizationService, IVoucherService voucherService)
    //    {
    //        this.CascadeMode = CascadeMode.StopOnFirstFailure;

    //        RuleFor(p => p.DichVuId)
    //            .NotEqual(0).WithMessage(localizationService.GetResource("Tên DV null"));

    //        RuleFor(p => p.LoaiGiaId)
    //            .NotEqual(0).WithMessage(localizationService.GetResource("Loại giá null"));

    //        RuleFor(p => p.SoTienChietKhau)
    //            .GreaterThan(0).WithMessage(localizationService.GetResource("Tiền null"))
    //            .When(model => model.LoaiChietKhau == LoaiChietKhau.ChietKhauTheoSoTien);

    //        RuleFor(p => p.TiLeChietKhau)
    //            .Must((model, s) => model.TiLeChietKhau > 0 && model.TiLeChietKhau <= 100).WithMessage(localizationService.GetResource("Tỉ lệ null"))
    //            .When(model => model.LoaiChietKhau == LoaiChietKhau.ChietKhauTheoTiLe);
    //    }
    //}
}
