using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhapKhoDuocPhams;
using FluentValidation;
using System;


namespace Camino.Api.Models.NhapKhoKSNKs.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauNhapKhoKSNKViewModel>))]
    public class YeuCauNhapKhoVatTuViewModelValidator : AbstractValidator<YeuCauNhapKhoKSNKViewModel>
    {
        public YeuCauNhapKhoVatTuViewModelValidator(ILocalizationService localizationService, IYeuCauNhapKhoDuocPhamService yeuCauNhapKhoDuocPhamService)
        {
            RuleFor(x => x.SoChungTu)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoVatTu.SoChungTu.Required"))
                .Must((req, soChungTu, id) => soChungTu != "0000000").WithMessage(localizationService.GetResource("NhapKho.SoChungTu.Invalid"));
            RuleFor(x => x.NguoiNhapId)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoVatTu.NguoiNhapId.Required"));
            RuleFor(x => x.NgayNhap)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoVatTu.NgayNhap.Required"));
            RuleFor(x => x.KyHieuHoaDon)
            .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.KyHieuHoaDon.Required"));
            RuleFor(x => x.NgayHoaDon)
             .MustAsync(async (viewModel, ngayTiepNhan, id) =>
             {
                 var kiemTraNgayHoaDon = await yeuCauNhapKhoDuocPhamService.KiemTraNgayHoaDon(viewModel.NgayHoaDon, DateTime.Now); // dùng chung funtion bên yeuCauNhapKhoDuocPhamService
                 return kiemTraNgayHoaDon;
             })
             .WithMessage(localizationService.GetResource("NhapKhoVaTuNgayHoaDon.LessThanOrEqualTo"));
            
        }
    }
}
