using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.GiayCamKetSuDungThuocNgoaiBH.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GiayCamKetSuDungThuocNgoaiBHViewModel>))]
    public class GiayCamKetSuDungThuocNgoaiBHViewModelValidator : AbstractValidator<GiayCamKetSuDungThuocNgoaiBHViewModel>
    {
        public GiayCamKetSuDungThuocNgoaiBHViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            RuleFor(x => x.NgayThucHien)
               .MustAsync(async (request, ten, id) => {
                   var val = dieuTriNoiTruService.KiemTraThoaDieuKien(request.YeuCauTiepNhanId, request.NgayThucHien, 3);
                   return val;
               }).WithMessage(localizationService.GetResource("Common.ThoiDiemKham.NotValue"));
        }
    }
}
