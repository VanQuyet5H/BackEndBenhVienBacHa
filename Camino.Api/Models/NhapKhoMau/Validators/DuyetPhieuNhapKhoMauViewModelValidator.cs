using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;

namespace Camino.Api.Models.NhapKhoMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuyetPhieuNhapKhoMauViewModel>))]
    public class DuyetPhieuNhapKhoMauViewModelValidator : AbstractValidator<DuyetPhieuNhapKhoMauViewModel>
    {
        public DuyetPhieuNhapKhoMauViewModelValidator(IValidator<DuyetPhieuNhapKhoMauChiTietViewModel> nhapKhoMauChiTietValidator)
        {
            RuleForEach(x => x.DuyetNhapKhoMauChiTiets)
                .SetValidator(nhapKhoMauChiTietValidator);
        }
    }
}
