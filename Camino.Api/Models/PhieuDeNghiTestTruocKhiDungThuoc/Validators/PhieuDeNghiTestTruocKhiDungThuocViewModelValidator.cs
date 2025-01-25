using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuDeNghiTestTruocKhiDungThuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ValiDatorPhieuDeNghiTestTruocKhiDung>))]
    public class PhieuDeNghiTestTruocKhiDungThuocViewModelValidator : AbstractValidator<ValiDatorPhieuDeNghiTestTruocKhiDung>
    {
        public PhieuDeNghiTestTruocKhiDungThuocViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {

            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.HoTen.Required"));
            RuleFor(x => x.NamSinh)
              .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NamSinh.Required")).MustAsync(async (request, ten, id) => {
                  var val = await dieuTriNoiTruService.KiemTraNamSinhHople(ten, request.Id);
                  return val;
              }).WithMessage(localizationService.GetResource("Common.NamSinh.Exists"));
            RuleFor(x => x.GioiTinh)
             .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.GioiTinh.Required"));
            RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.DiaChi.Required"));
        }
    }
}
