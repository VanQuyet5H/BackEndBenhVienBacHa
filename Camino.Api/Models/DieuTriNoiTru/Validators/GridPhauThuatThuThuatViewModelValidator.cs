using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GridPhauThuatThuThuatViewModel>))]
    public class GridPhauThuatThuThuatViewModelValidator : AbstractValidator<GridPhauThuatThuThuatViewModel>
    {
        public GridPhauThuatThuThuatViewModelValidator(ILocalizationService localizationService)
        {

        }
    }
}
