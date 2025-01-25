using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DieuTriNoiTruPhieuDieuTriThuocTruyenDichViewModel>))]

    public class DieuTriNoiTruPhieuDieuTriThuocTruyenDichViewModelValidator : AbstractValidator<DieuTriNoiTruPhieuDieuTriThuocTruyenDichViewModel>
    {
        public DieuTriNoiTruPhieuDieuTriThuocTruyenDichViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoId).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.KhoId.Required"));

            //RuleFor(x => x.ThoiGianBatDauTruyen).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.ThoiGianBatDauTruyen.Required"));

            RuleFor(x => x.SoLuong).NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"));

            //RuleFor(x => x.CachGioTruyenDich).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.CachGioTruyenDich.Required"))
            //    .When(x => x.SoLuong != null);

            RuleFor(x => x.ThoiGianBatDauTruyen)
               .NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.ThoiGianBatDauTruyen.Required"))
               .When(x => x.LaDichTruyen == true);

            RuleFor(x => x.CachGioTruyenDich).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.CachGioTruyenDich.Required"))
            .When(x => x.SoLanDungTrongNgay != null && x.SoLanDungTrongNgay > 1 && x.LaDichTruyen == true && !x.IsDelete);

            RuleFor(x => x.DuocPhamBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DuocPham.Required"));
        }
    }
}
