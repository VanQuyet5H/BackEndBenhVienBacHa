using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.XuatKhos.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThemDuocPham>))]
    public class ThemDuocPhamValidator : AbstractValidator<ThemDuocPham>
    {
        public ThemDuocPhamValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DuocPhamBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("ThemDuocPham.DuocPhamId.Required"));
            RuleFor(x => x.KhoId)
                .NotEmpty().WithMessage(localizationService.GetResource("ThemDuocPham.KhoId.Required"));
            RuleFor(x => x.SoLuongXuat)
                .Must((model, input, s) => model.SoLuongTon >= model.SoLuongXuat)
                .WithMessage(localizationService.GetResource("ThemDuocPham.SoLuongXuat.Range")).When(x => x.SoLuongXuat != null)
                .Must((model, input, s) => model.SoLuongXuat > 0)
                .WithMessage(localizationService.GetResource("ThemDuocPham.SoLuongXuat.MoreThan0")).When(x => x.SoLuongXuat != null)
                .NotEmpty().WithMessage(localizationService.GetResource("ThemDuocPham.soLuongXuat.Required"));

            //TODO update entity kho on 9/9/2020
            //RuleFor(x => x.DonGia)
            //    .NotEmpty().WithMessage(localizationService.GetResource("ThemDuocPham.DonGia.Required")).When(x => x.loaiKhoDuocPhamXuat == Enums.EnumLoaiKhoDuocPham.KhoTong 
            //                                                                                                && x.loaiXuatKho == Enums.XuatKhoDuocPham.XuatQuaKhoKhac);
            //RuleFor(x => x.VAT)
            //    .NotEmpty().WithMessage(localizationService.GetResource("ThemDuocPham.VAT.Required")).When(x => x.loaiKhoDuocPhamXuat == Enums.EnumLoaiKhoDuocPham.KhoTong
            //                                                                                                      && x.loaiXuatKho == Enums.XuatKhoDuocPham.XuatQuaKhoKhac);
            //RuleFor(x => x.ChietKhau)
            //    .NotEmpty().WithMessage(localizationService.GetResource("ThemDuocPham.ChietKhau.Required")).When(x => x.loaiKhoDuocPhamXuat == Enums.EnumLoaiKhoDuocPham.KhoTong
            //                                                                                                      && x.loaiXuatKho == Enums.XuatKhoDuocPham.XuatQuaKhoKhac);
            //RuleFor(x => x.SoLuongTon)
            //    .MustAsync(async (model, input, s) => model.SoLuongTon < model.SoLuongXuat).When(x => x.SoLuongTon != null).WithMessage(localizationService.GetResource("ThemDuocPham.SoLuongTon.Required"));
        }
    }
}