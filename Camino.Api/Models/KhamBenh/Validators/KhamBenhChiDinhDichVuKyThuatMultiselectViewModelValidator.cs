using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using Camino.Services.YeuCauKhamBenh;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhChiDinhDichVuKyThuatMultiselectViewModel>))]
    public class KhamBenhChiDinhDichVuKyThuatMultiselectViewModelValidator : AbstractValidator<KhamBenhChiDinhDichVuKyThuatMultiselectViewModel>
    {
        public KhamBenhChiDinhDichVuKyThuatMultiselectViewModelValidator(ILocalizationService localizationService, IYeuCauKhamBenhService yeuCauKhamBenhService)
        {
            RuleFor(x => x.DichVuKyThuatBenhVienChiDinhs)
                .NotEmpty().WithMessage(localizationService.GetResource("ChiDinhDichVuKyThuat.DichVuKyThuatBenhVienChiDinhs.Required"));
            RuleFor(x => x.NhomDichVuBenhVienId)
                .MustAsync(async (model,input,f) => await yeuCauKhamBenhService.KiemTraNhomDichVuChiDinhDichVuKyThuat(input, model.DichVuKyThuatBenhVienChiDinhs))
                    .WithMessage(localizationService.GetResource("ChiDinhDichVuKyThuat.NhomDichVuBenhVienId.Required"))
                .MustAsync( async (model,input,f) => await yeuCauKhamBenhService.KiemTraNhomDichVuDungTheoDichVuChiDinhDichVuKyThuat(input, model.DichVuKyThuatBenhVienChiDinhs))
                    .WithMessage(localizationService.GetResource("ChiDinhDichVuKyThuat.NhomDichVuBenhVienId.KhongDungTheoDichVu"));
        }
    }
}
