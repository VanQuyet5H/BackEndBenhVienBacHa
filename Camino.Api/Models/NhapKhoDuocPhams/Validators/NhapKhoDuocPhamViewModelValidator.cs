using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhapKhoDuocPhams;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoDuocPhams.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhapKhoDuocPhamViewModel>))]
    public class NhapKhoDuocPhamViewModelValidator : AbstractValidator<NhapKhoDuocPhamViewModel>
    {
        public NhapKhoDuocPhamViewModelValidator(ILocalizationService localizationService, INhapKhoDuocPhamService iNhapKhoDuocPhamService)
        {
            RuleFor(x => x.KhoDuocPhamId)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.KhoDuocPhamId.Required"));
            RuleFor(x => x.SoChungTu)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.SoChungTu.Required"));
            RuleFor(x => x.LoaiNhapKho)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.LoaiNhapKho.Required"));
            RuleFor(x => x.NguoiNhapId)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.NguoiNhapId.Required"));
            RuleFor(x => x.NgayNhap)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.NgayNhap.Required"));

            RuleFor(x => x.SoChungTu)
            .MustAsync(async (model, input, s) => {
                if (!string.IsNullOrEmpty(model.SoChungTu))
                    return !await iNhapKhoDuocPhamService.CheckSoChungTuAsync(model.SoChungTu, model.Id).ConfigureAwait(false);
                else
                    return true;
            })
            .WithMessage(localizationService.GetResource("NhapKhoDuocPham.SoChungTu.Exists"));
        }
    }
}

