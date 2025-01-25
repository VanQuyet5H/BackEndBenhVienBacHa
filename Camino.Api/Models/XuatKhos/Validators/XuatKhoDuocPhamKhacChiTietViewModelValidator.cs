using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Services.Localization;
using Camino.Services.XuatKhoKhacs;
using FluentValidation;
using System;
namespace Camino.Api.Models.XuatKhos.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<XuatKhoKhacDuocPhamChiTietVo>))]

    public class XuatKhoDuocPhamKhacChiTietViewModelValidator : AbstractValidator<XuatKhoKhacDuocPhamChiTietVo>
    {
        public XuatKhoDuocPhamKhacChiTietViewModelValidator(ILocalizationService localizationService, IXuatKhoKhacService xuatKhoKhacServiceService)
        {
            RuleFor(x => x.SoLuongXuat)
                 .MustAsync(async (viewModel, soLuongTon, id) =>
                 {
                     var kiemTraSLTon = await xuatKhoKhacServiceService.CheckSoLuongTonDuocPham(viewModel.DuocPhamBenhVienId, viewModel.LaDuocPhamBHYT, viewModel.KhoXuatId, viewModel.SoLuongXuat, viewModel.SoLo, viewModel.HanSuDung);
                     return kiemTraSLTon;
                 })
                 .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));
        }
    }
}
