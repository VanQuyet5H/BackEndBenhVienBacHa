using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.HopDongThauDuocPhamService;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.DuocPhamBenhVien;

namespace Camino.Api.Models.HopDongThauDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HopDongThauDuocPhamChiTietViewModel>))]
    public class HopDongThauDuocPhamChiTietViewModelValidator : AbstractValidator<HopDongThauDuocPhamChiTietViewModel>
    {
        public HopDongThauDuocPhamChiTietViewModelValidator(ILocalizationService localizationService, IHopDongThauDuocPhamService hopDongThauDuocPhamService, IDuocPhamBenhVienService duocPhamBenhVienService)
        {
            //RuleFor(x => x.MaDuocPhamBenhVien)
            //    .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.MaDuocPhamBenhVien.Required"))
            //.MustAsync(async (model, input, s) => !await hopDongThauDuocPhamService.IsMaDuocPhamBVExists(model.MaDuocPhamBenhVien,0).ConfigureAwait(false))
            //.WithMessage(localizationService.GetResource("HopDongThauDuocPham.MaDuocPhamBenhVien.Exists"))

            // BVHD-3454
            // mã bệnh viện tối thiểu 7 ký tự
            RuleFor(x => x.MaDuocPhamBenhVien)
                .NotEmpty().When(x => x.SuDungTaiBenhVien).WithMessage(localizationService.GetResource("HopDongThauDuocPham.MaDuocPhamBenhVien.Required"))
            .Must((viewModel, input, d) => string.IsNullOrEmpty(input) || (!string.IsNullOrEmpty(input) && input.Length >= 7)).When(x => x.SuDungTaiBenhVien)
                .WithMessage(localizationService.GetResource("DuocPhamBenhVien.MaDuocPhamBenhVien.Length"))
            .MustAsync(async (viewModel, input, d) => !await duocPhamBenhVienService.KiemTraTrungMaDuocPhamBenhVienAsync(viewModel.DuocPhamBenhVienId ?? 0, input, viewModel.MaDuocPhamTemps))
                .When(x => x.SuDungTaiBenhVien)
                .WithMessage(localizationService.GetResource("HopDongThauDuocPham.MaDuocPhamBenhVien.Exists"));

            RuleFor(x => x.DuocPhamBenhVienPhanNhomChaId)
                .NotEmpty().When(x => x.SuDungTaiBenhVien)
                    .WithMessage(localizationService.GetResource("HopDongThauDuocPham.DuocPhamBenhVienPhanNhomChaId.Reqiured"));

            RuleFor(x => x.DuocPhamId)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.DuocPhamId.Reqiured"))
                .MustAsync(async (viewModel, input, d) => await hopDongThauDuocPhamService.GetHieuLucDuocPham(input.Value))
                    .When(x => x.DuocPhamId != null && x.DuocPhamId != 0 && (x.DuocPhamBenhVienId == 0 || x.DuocPhamBenhVienId == null))
                    .WithMessage(localizationService.GetResource("HopDongThauDuocPham.DuocPhamId.HetHieuLuc"));

            RuleFor(x => x.Gia)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.Gia.Reqiured"))
                .GreaterThan(0).WithMessage(localizationService.GetResource("HopDongThauDuocPham.Gia.Range"));

            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("HopDongThauDuocPham.SoLuong.Reqiured"))
                .GreaterThan(0).WithMessage(localizationService.GetResource("HopDongThauDuocPham.SoLuong.Range"));
        }
    }
}
