using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhapKhoDuocPhams;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoVatTus.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauNhapKhoVatTuViewModel>))]
    public class YeuCauNhapKhoVatTuViewModelValidator : AbstractValidator<YeuCauNhapKhoVatTuViewModel>
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
            //            RuleFor(x => x.NguoiGiaoId)
            //              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoVatTu.NguoiGiaoId.Required")).When(x => x.LoaiNguoiGiao == Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong);
            //            RuleFor(x => x.TenNguoiGiao)
            //              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoVatTu.TenNguoiGiao.Required")).When(x => x.LoaiNguoiGiao == Core.Domain.Enums.LoaiNguoiGiaoNhan.NgoaiHeThong);
        }
    }
}
