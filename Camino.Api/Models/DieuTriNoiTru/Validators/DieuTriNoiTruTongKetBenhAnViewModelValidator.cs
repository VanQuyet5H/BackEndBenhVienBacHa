using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DieuTriNoiTruTongKetBenhAnViewModel>))]
    public class DieuTriNoiTruTongKetBenhAnViewModelValidator : AbstractValidator<DieuTriNoiTruTongKetBenhAnViewModel>
    {
        public DieuTriNoiTruTongKetBenhAnViewModelValidator(ILocalizationService localizationService, IValidator<GridPhauThuatThuThuatViewModel> gridPhauThuatThuThuatValidator)
        {
            RuleForEach(x => x.gridPhauThuatThuThuat)
                .SetValidator(gridPhauThuatThuThuatValidator);

            RuleFor(x => x.VaoBuongDeLuc)
                .Must((viewModel, input) => input == null || input != null && input <= DateTime.Now)
                .WithMessage(localizationService.GetResource("thongTinBenhAnSanKhoa.VaoBuongDeLuc.MoreThanNow"));
        }
    }
}
