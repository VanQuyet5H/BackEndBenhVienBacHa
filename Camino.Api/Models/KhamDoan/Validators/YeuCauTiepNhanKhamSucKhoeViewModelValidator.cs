using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauTiepNhanKhamSucKhoeViewModel>))]
    public class YeuCauTiepNhanKhamSucKhoeViewModelValidator : AbstractValidator<YeuCauTiepNhanKhamSucKhoeViewModel>
    {
        public YeuCauTiepNhanKhamSucKhoeViewModelValidator(ILocalizationService localizationService, IValidator<ThongTinHanhChinhHopDongKhamSucKhoeNhanVienViewModel> thongTinHanhChinhValidator)
        {
            RuleFor(x => x.HopDongKhamSucKhoeNhanVien).SetValidator(thongTinHanhChinhValidator);
        }
    }
}
