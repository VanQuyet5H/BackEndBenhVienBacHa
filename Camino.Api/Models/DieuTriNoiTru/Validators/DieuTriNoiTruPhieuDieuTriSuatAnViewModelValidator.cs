using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThemSuatAnViewModel>))]
    public class DieuTriNoiTruPhieuDieuTriSuatAnViewModelValidator : AbstractValidator<ThemSuatAnViewModel>
    {
        public DieuTriNoiTruPhieuDieuTriSuatAnViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DichVuKyThuatBenhVienId).NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));

            RuleFor(x => x.SoLan).NotEmpty().WithMessage(localizationService.GetResource("Common.SoLuongNhap.Required"));

            RuleFor(x => x.DoiTuongSuDung).NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.DoiTuong.Required"));
        }
    }
}
