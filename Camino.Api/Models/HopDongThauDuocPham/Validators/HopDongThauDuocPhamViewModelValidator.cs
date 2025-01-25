using System;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.HopDongThauDuocPhamService;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.HopDongThauDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HopDongThauDuocPhamViewModel>))]
    public class HopDongThauDuocPhamViewModelValidator : AbstractValidator<HopDongThauDuocPhamViewModel>
    {
        public HopDongThauDuocPhamViewModelValidator(ILocalizationService localizationService, IHopDongThauDuocPhamService hopDongThauDuocPhamService, IValidator<HopDongThauDuocPhamChiTietViewModel> validateHopDongThauDuocPhamChiTietViewModel)
        {
            RuleFor(x => x.NhaThauId)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.NhaThauId.Required"));

            //RuleFor(x => x.SoHopDong)
            //    .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.SoHopDong.Required"))
            //    .Length(0, 50).WithMessage(localizationService.GetResource("HopDongThauDuocPham.SoHopDong.Range.50"));

            RuleFor(x => x.SoQuyetDinh)
                   .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.SoQuyetDinh.Required"))
                   .Length(0, 50).WithMessage(localizationService.GetResource("HopDongThauDuocPham.SoQuyetDinh.Range.50"))
                   ;

            RuleFor(x => x.CongBo)
                   .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.CongBo.Required"));

            //RuleFor(x => x.NgayKy)
            //       .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.NgayKy.Required"));

            RuleFor(x => x.NgayHieuLuc)
                   .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.NgayHieuLuc.Required"));

            RuleFor(x => x.NgayHetHan)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.NgayHetHan.Required"))
                .Must((model, input, s) =>
                {
                    if (input == null) return true;

                    if (input.GetValueOrDefault().Date <= DateTime.Now.Date)
                    {
                        return false;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("HopDongThauDuocPham.NgayHetHan.NotValidate"))
                .Must((model, input, s) =>
                {
                    if (input == null) return true;

                    if (input.GetValueOrDefault().Date <= model.NgayHieuLuc.GetValueOrDefault().Date)
                    {
                        return false;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("HopDongThauDuocPham.NgayHetHan.GreatThanNgayHieuLuc.NotValidate"));

            RuleFor(x => x.LoaiThau)
                   .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.LoaiThau.Required"));

            RuleFor(x => x.LoaiThuocThau)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.LoaiThuocThau.Required"));

            RuleFor(x => x.NhomThau)
                   .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.NhomThau.Required"))
                   .Length(0, 50).WithMessage(localizationService.GetResource("HopDongThauDuocPham.NhomThau.Range.50"));

            RuleFor(x => x.GoiThau)
                   .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.GoiThau.Required"))
                   .Length(0, 50).WithMessage(localizationService.GetResource("HopDongThauDuocPham.GoiThau.Range.2"));

            RuleFor(x => x.Nam)
                   .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.Nam.Required"));

            // BVHD-3454
            RuleForEach(x => x.HopDongThauDuocPhamChiTiets).SetValidator(validateHopDongThauDuocPhamChiTietViewModel);
        }
    }
}
