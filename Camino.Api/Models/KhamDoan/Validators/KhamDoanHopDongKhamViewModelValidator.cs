using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamDoanHopDongKhamViewModel>))]
    public class KhamDoanHopDongKhamViewModelValidator : AbstractValidator<KhamDoanHopDongKhamViewModel>
    {
        public KhamDoanHopDongKhamViewModelValidator(ILocalizationService localizationService,
                                                     IValidator<HopDongSucKhoeDiaDiemViewModel> diaDiemKhamViewModelValidator)
        {
            RuleFor(x => x.CongTyKhamSucKhoeId).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.CongTyKhamSucKhoeId.Required"));

            RuleFor(p => p.SoHopDong).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.SoHopDong.Required"));

            RuleFor(x => x.NgayHopDong).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.NgayHopDong.Required"));

            RuleFor(x => x.LoaiHopDong).NotNull().WithMessage(localizationService.GetResource("KhamDoan.LoaiHopDong.Required"));

            //RuleFor(x => x.SoNguoiKham).NotNull().WithMessage(localizationService.GetResource("KhamDoan.SoBenhNhan.Required"));

            RuleFor(x => x.GiaTriHopDong).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.GiaTriHopDong.Required"));

            RuleFor(x => x.NgayHieuLuc).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.NgayHieuLuc.Required"));

            //RuleFor(x => x.NgayHieuLuc).Must((model, s) => model.NgayKetThuc >= model.NgayHieuLuc)
            //                           .WithMessage(localizationService.GetResource("KhamDoan.NgayHieuLuc.GreaterThan"))
            //                           .When(x => x.NgayHieuLuc != null);

            //RuleFor(x => x.NgayKetThuc).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.NgayKetThuc.Required"));

            //RuleFor(x => x.NgayKetThuc).Must((model, s) =>  model.NgayKetThuc >= DateTime.Now)
            //                           .WithMessage(localizationService.GetResource("KhamDoan.NgayKetThuc.GreaterThanToDay"))
            //                           .When(x => x.NgayKetThuc != null);

            //RuleFor(x => x.NgayKetThuc).Must((model, s) => model.NgayHieuLuc <= model.NgayKetThuc)
            //                           .WithMessage(localizationService.GetResource("KhamDoan.NgayKetThuc.GreaterThan"))
            //                           .When(x => x.NgayKetThuc != null && x.NgayKetThuc >= DateTime.Now);

            //RuleFor(x => x.NguoiKyHopDong).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.NguoiKyHopDong.Required"));
            //RuleFor(x => x.ChucDanhNguoiKy).NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.ChucDanhNguoiKy.Required"));

            RuleForEach(x => x.HopDongKhamSucKhoeDiaDiems).SetValidator(diaDiemKhamViewModelValidator);
        }
    }
}
