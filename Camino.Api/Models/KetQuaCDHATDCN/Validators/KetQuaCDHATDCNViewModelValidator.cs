using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KetQuaCDHATDCN.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KetQuaCDHATDCNViewModel>))]
    public class KetQuaCDHATDCNViewModelValidator : AbstractValidator<KetQuaCDHATDCNViewModel>
    {
        public KetQuaCDHATDCNViewModelValidator(ILocalizationService localizationService, IValidator<ChiTietKetQuaCDHATDCNViewModel> chiTietValidator)
        {
            RuleFor(x => x.ThoiDiemThucHien)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.NgayThucHien.Required"))
                .LessThanOrEqualTo(DateTime.Now).WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.NgayThucHien.Range"));

            //Khách hàng yêu cầu bỏ cột này
            //RuleFor(x => x.ThoiDiemKetLuan)
            //    .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.ThoiDiemKetLuan.Required"))
            //    .LessThanOrEqualTo(DateTime.Now).WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.ThoiDiemKetLuan.Range"))
            //    .Must((viewModel, input) => input == null || viewModel.ThoiDiemThucHien == null || viewModel.ThoiDiemThucHien.Value <= input.Value)
            //        .WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.ThoiDiemKetLuan.GreaterThanThoiDiemThucHien"));

            //RuleFor(x => x.NhanVienThucHienId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.NguoiThucHienId.Required"));

            RuleFor(x => x.NhanVienKetLuanId)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaVaKetLuanMau.BacSiKetLuanId.Required"));

            RuleFor(x => x.ChiTietKetQuaObj)
                .SetValidator(chiTietValidator);
        }
    }
}
