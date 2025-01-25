using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanViewModel>))]
    public class ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanViewModelValidator : AbstractValidator<ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanViewModel>
    {
        public ChiDinhEkipVaDichVuGiuongNoiTruTiepNhanViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.BacSiDieuTriId)
                .NotEmpty().WithMessage(localizationService.GetResource("NoiTruBenhAn.BacSiDieuTriId.Required"));
            RuleFor(x => x.DieuDuongId)
                .NotEmpty().WithMessage(localizationService.GetResource("NoiTruBenhAn.DieuDuongId.Required"));
            RuleFor(x => x.TuNgay)
                .NotEmpty().WithMessage(localizationService.GetResource("NoiTruBenhAn.TuNgay.Required"));


            RuleFor(x => x.DichVuGiuongId)
                .Must((model, input) => model.KhongCanChiDinhGiuong == true || input != null).WithMessage(localizationService.GetResource("NoiTruBenhAn.DichVuGiuongId.Required"));
            RuleFor(x => x.GiuongId)
                .Must((model, input) => (model.KhongCanChiDinhGiuong == true && (model.DichVuGiuongId == null || (model.DichVuGiuongId != null && input != null))) || input != null).WithMessage(localizationService.GetResource("NoiTruBenhAn.GiuongId.Required"));
            RuleFor(x => x.LoaiGiuong)
                .Must((model, input) => (model.KhongCanChiDinhGiuong == true && (model.DichVuGiuongId == null || (model.DichVuGiuongId != null && input != null))) || input != null).WithMessage(localizationService.GetResource("NoiTruBenhAn.LoaiGiuong.Required"));
            RuleFor(x => x.ThoiGianNhan)
                .Must((model, input) => (model.KhongCanChiDinhGiuong == true && (model.DichVuGiuongId == null || (model.DichVuGiuongId != null && input != null))) || input != null).WithMessage(localizationService.GetResource("NoiTruBenhAn.ThoiGianNhan.Required"));

        }
    }

}
