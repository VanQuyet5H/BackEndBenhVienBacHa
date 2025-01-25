using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.HopDongThauVatTuService;
using Camino.Services.Localization;
using Camino.Services.VatTuBenhViens;
using FluentValidation;

namespace Camino.Api.Models.HopDongThauVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HopDongThauVatTuChiTietViewModel>))]
    public class HopDongThauVatTuChiTietViewModelValidator : AbstractValidator<HopDongThauVatTuChiTietViewModel>
    {
        public HopDongThauVatTuChiTietViewModelValidator(ILocalizationService localizationService, IHopDongThauVatTuService hdtVatTu, IVatTuBenhVienService vatTuBenhVienService)
        {
            RuleFor(x => x.MaVatTuBenhVien)
                .NotEmpty().When(x => x.SuDungTaiBenhVien).WithMessage(localizationService.GetResource("HopDongThauVatTu.MaVatTuBenhVien.Required"))
                .Must((viewModel, input, d) => string.IsNullOrEmpty(input) || (!string.IsNullOrEmpty(input) && input.Length >= 7))
                    .When(x => x.SuDungTaiBenhVien)
                    .WithMessage(localizationService.GetResource("VatTuBenhVien.MaVatTuBenhVien.Length"))
                .MustAsync(async (viewModel, input, d) => !await vatTuBenhVienService.KiemTraTrungMaVatTuBenhVienAsync(viewModel.VatTuBenhVienId ?? 0, input, viewModel.MaVatTuTemps))
                    .When(x => x.SuDungTaiBenhVien)
                    .WithMessage(localizationService.GetResource("HopDongThauVatTu.MaVatTuBenhVien.Exists"));

            RuleFor(x => x.LoaiSuDungId)
                .NotEmpty().When(x => x.SuDungTaiBenhVien).WithMessage(localizationService.GetResource("HopDongThauVatTu.LoaiSuDungId.Reqiured"));

            RuleFor(x => x.VatTuId)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.VatTuId.Reqiured"));

            RuleFor(x => x.VatTuId)
                .MustAsync(async (viewModel, input, d) =>  await hdtVatTu.GetHieuLucVatTu(input ?? 0))
                    .When(x => x.VatTuId != null && x.VatTuId != 0 && (x.Id == 0 || (x.VatTuBenhVienId == 0 || x.VatTuBenhVienId == null)))
                    .WithMessage(localizationService.GetResource("HopDongThauVatTu.VatTuId.HetHieuLuc"));


            RuleFor(x => x.Gia)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.Gia.Reqiured"))
                .GreaterThan(0).WithMessage(localizationService.GetResource("HopDongThauVatTu.Gia.Range"));

            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauVatTu.SoLuong.Reqiured"))
                .GreaterThan(0).WithMessage(localizationService.GetResource("HopDongThauVatTu.SoLuong.Range"));
        }
    }
}
