using System;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.HopDongThauVatTuService;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.HopDongThauVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HopDongThauVatTuViewModel>))]
    public class HdtVatTuViewModelValidator : AbstractValidator<HopDongThauVatTuViewModel>
    {
        public HdtVatTuViewModelValidator(ILocalizationService localizationService, IHopDongThauVatTuService hdtVatTu, IValidator<HopDongThauVatTuChiTietViewModel> hopDongThauChiTietValidator)
        {
            RuleFor(p => p.NhaThauId)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.NhaThauId.Required"))
                .MustAsync(async (model, input, s) => await hdtVatTu
                    .CheckExist(model.Id, model.NhaThauId.GetValueOrDefault(), model.SoHopDong, model.SoQuyetDinh).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("HdtVatTu.Exists.NotAllow"));
        
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
            RuleFor(p => p.SoQuyetDinh)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.SoQuyetDinh.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("HopDongThauDuocPham.SoQuyetDinh.Range.50"));


            //BVHD-3472
            RuleFor(x => x.CongBo)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.CongBo.Required"));
            RuleFor(x => x.NgayHieuLuc)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.NgayHieuLuc.Required"));
            RuleFor(x => x.LoaiThau)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.LoaiThau.Required"));
            RuleFor(x => x.NhomThau)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.NhomThau.Required"));
            RuleFor(x => x.GoiThau)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.GoiThau.Required"));
            RuleFor(x => x.Nam)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.Nam.Required"));

            RuleForEach(x => x.HopDongThauVatTuChiTiets).SetValidator(hopDongThauChiTietValidator);
        }
    }
}
