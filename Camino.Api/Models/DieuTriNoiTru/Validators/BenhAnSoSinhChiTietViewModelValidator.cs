using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BenhAnSoSinhChiTietViewModel>))]
    public class BenhAnSoSinhChiTietViewModelValidator : AbstractValidator<BenhAnSoSinhChiTietViewModel>
    {
        public BenhAnSoSinhChiTietViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            RuleFor(x => x.KhoaChuyenBenhAnSoSinhVeId).NotEmpty()
                .WithMessage(localizationService.GetResource("BenhAnSoSinh.KhoaChuyenBenhAnSoSinhVeId.Required"));

            RuleFor(x => x.TenBanDau)
                .Must((viewmodel, input) => !string.IsNullOrEmpty(input) || !string.IsNullOrEmpty(viewmodel.TenKhaiSinh))
                .WithMessage(localizationService.GetResource("BenhAnSoSinh.TenBenhNhan.Required"));

            RuleFor(x => x.NgayThangNamSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhAnSoSinh.NgayThangNamSinh.Required"));

            RuleFor(x => x.NgayThangNamSinh)
              .MustAsync(async (viewModel, input, f) => !await dieuTriNoiTruService.KiemTraNgaySinhConVaThoiGianNhapVienMe(viewModel.YeuCauTiepNhanId, (DateTime)viewModel.NgayThangNamSinh))
              .WithMessage(localizationService.GetResource("BenhAnSoSinh.NgayThangNamSinh.BenhAnConSauNhapVienBAMe"))
              .When(x => x.NgayThangNamSinh != null);

            RuleFor(x => x.SoDienThoai)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhAnSoSinh.SoDienThoai.Required"));
            RuleFor(x => x.GioiTinh)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhAnSoSinh.GioiTinh.Required"));

            RuleFor(x => x.YeuCauGoiDichVuId)
                .MustAsync(async (viewModel, input, f) => !await dieuTriNoiTruService.KiemTraYeuCauGoiDichVuDaSuDungAsync(input, null, false, viewModel.YeuCauTiepNhanId))
                .WithMessage(localizationService.GetResource("BenhAnSoSinh.YeuCauGoiDichVuId.DaSuDung"));
        }
    }
}
