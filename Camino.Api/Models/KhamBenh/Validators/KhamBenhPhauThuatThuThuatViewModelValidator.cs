using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhPhauThuatThuThuatViewModel>))]
    public class KhamBenhPhauThuatThuThuatViewModelValidator : AbstractValidator<KhamBenhPhauThuatThuThuatViewModel>
    {
        public KhamBenhPhauThuatThuThuatViewModelValidator(IValidator<ListKhamBenhPhauThuatThuThuatViewModel> khamBenhPhauThuatThuThuatListValidator)
        {
            RuleForEach(x => x.ListPttt).SetValidator(khamBenhPhauThuatThuThuatListValidator);
        }
    }
}
