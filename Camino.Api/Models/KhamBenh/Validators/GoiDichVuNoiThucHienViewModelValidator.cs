using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GoiDichVuNoiThucHienViewModel>))]
    public class GoiDichVuNoiThucHienViewModelValidator : AbstractValidator<GoiDichVuNoiThucHienViewModel>
    {
        public GoiDichVuNoiThucHienViewModelValidator(IValidator<GoiDichVuChiTietNoiThucHienViewModel> chiTietNoiThucHienValidator)
        {
            RuleForEach(x => x.GoiDichVuChiTietNoiThucHiens).SetValidator(chiTietNoiThucHienValidator);
        }
    }
}
