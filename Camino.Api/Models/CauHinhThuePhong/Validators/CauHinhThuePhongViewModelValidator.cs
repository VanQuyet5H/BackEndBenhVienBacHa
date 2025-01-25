using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.CauHinhThuePhong;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.CauHinhThuePhong.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<CauHinhThuePhongViewModel>))]
    public class CauHinhThuePhongViewModelValidator : AbstractValidator<CauHinhThuePhongViewModel>
    {
        public CauHinhThuePhongViewModelValidator(ILocalizationService localizationService, ICauHinhThuePhongService _cauHinhThuePhongService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .MustAsync(async (model, input, f) => await _cauHinhThuePhongService.KiemTraTrungTenAsync(model.Id, input))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));
            RuleFor(x => x.LoaiThuePhongPhauThuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("CauHinhThuePhong.LoaiThuePhongPhauThuatId.Required"));
            RuleFor(x => x.LoaiThuePhongNoiThucHienId)
                .NotEmpty().WithMessage(localizationService.GetResource("CauHinhThuePhong.LoaiThuePhongNoiThucHienId.Required"));
            RuleFor(x => x.BlockThoiGianTheoPhut)
                .NotEmpty().WithMessage(localizationService.GetResource("CauHinhThuePhong.BlockThoiGianTheoPhut.Required"));
            RuleFor(x => x.GiaThue)
                .NotEmpty().WithMessage(localizationService.GetResource("CauHinhThuePhong.GiaThue.Required"));
            RuleFor(x => x.PhanTramNgoaiGio)
                .NotEmpty().WithMessage(localizationService.GetResource("CauHinhThuePhong.PhanTramNgoaiGio.Required"));
            RuleFor(x => x.PhanTramLeTet)
                .NotEmpty().WithMessage(localizationService.GetResource("CauHinhThuePhong.PhanTramLeTet.Required"));
            RuleFor(x => x.GiaThuePhatSinh)
                .NotEmpty().WithMessage(localizationService.GetResource("CauHinhThuePhong.GiaThuePhatSinh.Required"));
            RuleFor(x => x.PhanTramPhatSinhNgoaiGio)
                .NotEmpty().WithMessage(localizationService.GetResource("CauHinhThuePhong.PhanTramPhatSinhNgoaiGio.Required"));
            RuleFor(x => x.PhanTramPhatSinhLeTet)
                .NotEmpty().WithMessage(localizationService.GetResource("CauHinhThuePhong.PhanTramPhatSinhLeTet.Required"));
        }
    }
}
