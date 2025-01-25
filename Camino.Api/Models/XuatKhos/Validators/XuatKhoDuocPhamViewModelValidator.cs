using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Services.Localization;
using FluentValidation;
using System;

namespace Camino.Api.Models.XuatKhos.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<XuatKhoDuocPhamViewModel>))]
    public class XuatKhoDuocPhamViewModelValidator : AbstractValidator<XuatKhoDuocPhamViewModel>
    {
        public XuatKhoDuocPhamViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoDuocPhamXuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.KhoDuocPhamXuatId.Required"));

            RuleFor(x => x.KhoDuocPhamNhapId)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.KhoDuocPhamNhapId.Required")).When(p => !p.IsXuatKhac);

            RuleFor(x => x.NgayXuat)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.NgayXuat.Required"));

            RuleFor(x => x.NgayXuat)
                 .Must((model, s) => (model.NgayXuat != null && model.NgayXuat < DateTime.Now) || model.NgayXuat == null).WithMessage(localizationService.GetResource("XuatKho.NgayXuat.MoreThanNow"));

            //RuleFor(x => x.SoPhieu)
            //    .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.SoPhieu.Required"));
            //RuleFor(x => x.LoaiXuatKho)
            //    .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.LoaiXuatKho.Required"));
            RuleFor(x => x.NguoiXuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.NguoiXuatId.Required"));

//            RuleFor(x => x.NguoiNhanId)
//                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.NguoiNhanId.Required"));

            //RuleFor(x => x.LoaiNguoiNhan)
            //    .Must((model, s) => model.LoaiNguoiNhan != null && model.LoaiNguoiNhan == Enums.LoaiNguoiGiaoNhan.TrongHeThong)
            //    .WithMessage(localizationService.GetResource("XuatKho.LoaiNguoiNhan.MustTypeOne")).When(x => x.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatQuaKhoKhac);

            RuleFor(x => x.LyDoXuatKho)
                .NotEmpty().WithMessage(localizationService.GetResource("XuatKho.LyDoXuatKho.Required"));

            //RuleFor(x => x.KhoDuocPhamNhapId)
            //    .Must((model, s) => model.KhoDuocPhamNhapId != model.KhoDuocPhamXuatId)
            //    .WithMessage(localizationService.GetResource("XuatKho.KhoDuocPhamNhapId.OtherKhoDuocPhamXuat")).When(x => x.KhoDuocPhamNhapId != null && x.KhoDuocPhamXuatId != 0);

        }
    }
}