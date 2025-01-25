using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<UpdateSoLuongItemGhiNhanVTTHThuocViewModel>))]
    public class UpdateSoLuongItemGhiNhanVTTHThuocViewModelValidator : AbstractValidator<UpdateSoLuongItemGhiNhanVTTHThuocViewModel>
    {
        public UpdateSoLuongItemGhiNhanVTTHThuocViewModelValidator(ILocalizationService localizationService, IKhamBenhService khamBenhService)
        {
            RuleFor(x => x.SoLuong)
                .Must((model, input) => input != null && input > 0).WithMessage(localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.SoLuong.Required"))
                .MustAsync(async (viewModel, input, d) =>
                {
                        var obj = new UpdateSoLuongItemGhiNhanVTTHThuocVo
                        {
                            SoLuong = viewModel.SoLuong ?? 0,
                            SoLuongBanDau = viewModel.SoLuongBanDau ?? 0,
                            LaDuocPham = viewModel.LaDuocPham ?? false,
                            VatTuThuocBenhVienId = viewModel.VatTuThuocBenhVienId ?? 0,
                            LaBHYT = viewModel.LaBHYT ?? false,
                            KhoId = viewModel.KhoId ?? 0
                        };
                        return await khamBenhService.KiemTraSoLuongTonCuaThuocVTTHHienTaiAsync(obj);
                }).WithMessage(localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
        }
    }
}
