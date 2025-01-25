using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using FluentValidation;
namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DieuTriNoiTruPhieuDieuTriThuocViewModel>))]

    public class DieuTriNoiTruPhieuDieuTriThuocViewModelValidator : AbstractValidator<DieuTriNoiTruPhieuDieuTriThuocViewModel>
    {
        public DieuTriNoiTruPhieuDieuTriThuocViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoId).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.KhoId.Required"))
                 .When(x => x.KhuVuc == 1);

            RuleFor(x => x.DungSang)
                .Must((model, input, s) => string.IsNullOrEmpty(model.DungSang) || model.DungSang.IsNumberFraction())
                .WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.DungTrua)
               .Must((model, input, s) => string.IsNullOrEmpty(model.DungTrua) || model.DungTrua.IsNumberFraction())
               .WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.DungChieu)
               .Must((model, input, s) => string.IsNullOrEmpty(model.DungChieu) || model.DungChieu.IsNumberFraction())
               .WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.DungToi)
               .Must((model, input, s) => string.IsNullOrEmpty(model.DungToi) || model.DungToi.IsNumberFraction())
               .WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.SoLuong).NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"));

            RuleFor(x => x.DuocPhamBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DuocPham.Required"));

            RuleFor(x => x.ThoiGianBatDauTruyen)
               .NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.ThoiGianBatDauTruyen.Required"))
               .When(x => x.LaDichTruyen == true);

            RuleFor(x => x.CachGioTruyenDich).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.CachGioTruyenDich.Required"))
            .When(x => x.SoLanDungTrongNgay != null && x.SoLanDungTrongNgay > 1 && x.LaDichTruyen == true && !x.IsDelete);
        }
    }
}
