using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BangKiemAnToanNBPT.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BangKiemAnToanNBPTViewModel>))]
    public class BangKiemAnToanNBPTViewModelValidator : AbstractValidator<BangKiemAnToanNBPTViewModel>
    {
        public BangKiemAnToanNBPTViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            RuleFor(x => x.NgayGioDuaBNDiPT)
              .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NgayGioDuaBNDiPT.Required"))
              .MustAsync(async (request, ten, id) => {
                  //Ngày giờ đưa NB đi phẫu thuật và ngày giờ dự định gây mê phải sau ngày thời gian tiếp nhận 3802
                  var val =  dieuTriNoiTruService.KiemTraThoaDieuKien(request.YeuCauTiepNhanId, request.NgayGioDuaBNDiPT,1);
                  return val;
              }).WithMessage(localizationService.GetResource("Common.NgayGioDuaBNDiPT.NotValue")); 
              
            //RuleFor(x => x.NgayGioDuDinhGayMe)
            //    .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NgayGioDuDinhGayMe.NgayHieuLuc"));
            RuleFor(x => x.ThoiDiemKham)
               .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.ThoiDiemKham.Required")).MustAsync(async (request, ten, id) => {
                   //Bắt điều kiện: Thời điểm khám: phải sau thời gian tiếp nhận NB và trước thời gian hiện tại
                   var val = dieuTriNoiTruService.KiemTraThoaDieuKien(request.YeuCauTiepNhanId, request.ThoiDiemKham, 2);
                   return val;
               }).WithMessage(localizationService.GetResource("Common.ThoiDiemKham.NotValue"));
        }
    }
}
