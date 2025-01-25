using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LinhThuongDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuyetYeuCauLinhDuocPhamViewModel>))]
    public class DuyetYeuCauLinhDuocPhamThuongViewModelValidator: AbstractValidator<DuyetYeuCauLinhDuocPhamViewModel>
    {
        public DuyetYeuCauLinhDuocPhamThuongViewModelValidator(ILocalizationService localizationService, IValidator<DuyetYeuCauLinhDuocPhamChiTietViewModel> chiTietValidator)
        {
            RuleFor(x => x.NguoiNhapKhoId).NotEmpty().WithMessage(localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.NguoiNhapKhoId.Required"));
            RuleFor(x => x.NguoiXuatKhoId).NotEmpty().WithMessage(localizationService.GetResource("DuyetYeuCauLinhDuocPhamThuong.NguoiXuatKhoId.Required"));

            RuleForEach(x => x.DuyetYeuCauLinhDuocPhamChiTiets).SetValidator(chiTietValidator);
        }
    }
}
