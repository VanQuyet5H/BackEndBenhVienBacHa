using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.CauHinh;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.Cauhinh.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<CauHinhNguoiDuyetTheoNhomDichVuViewModel>))]
    public class CauHinhNguoiDuyetTheoNhomDichVuViewModelValidator : AbstractValidator<CauHinhNguoiDuyetTheoNhomDichVuViewModel>
    {
        public CauHinhNguoiDuyetTheoNhomDichVuViewModelValidator(ILocalizationService localizationService, ICauHinhNguoiDuyetTheoNhomDVService _cauHinhNguoiDuyetTheoNhomDVService)
        {
            //RuleFor(x => x.KeyId).NotEmpty().WithMessage(localizationService.GetResource("CauHinh.KeyId.Required"));
            RuleFor(x => x.NhanVienId).NotEmpty().WithMessage(localizationService.GetResource("CauHinhNguoiDuyetTheoNhomDV.Ten.Required"));
            RuleFor(x => x.NhomDichVuBenhVienId).NotEmpty().WithMessage(localizationService.GetResource("CauHinhNguoiDuyetTheoNhomDV.Nhom.Required"));
        }
    }
}
