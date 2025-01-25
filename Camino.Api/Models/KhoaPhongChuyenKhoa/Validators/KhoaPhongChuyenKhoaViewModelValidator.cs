using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhoaPhongChuyenKhoa.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhoaPhongChuyenKhoaViewModel>))]
    public class KhoaPhongChuyenKhoaViewModelValidator : AbstractValidator<KhoaPhongChuyenKhoaViewModel>
    {
        public KhoaPhongChuyenKhoaViewModelValidator(ILocalizationService iLocalizationService)
        {

        }
    }
}
