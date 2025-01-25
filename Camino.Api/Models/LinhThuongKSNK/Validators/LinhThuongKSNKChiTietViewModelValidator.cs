using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhKSNK;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LinhThuongKSNK.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhThuongKNSKChiTietViewModel>))]

    public class LinhThuongKSNKChiTietViewModelValidator : AbstractValidator<LinhThuongKNSKChiTietViewModel>
    {
        public LinhThuongKSNKChiTietViewModelValidator(ILocalizationService localizationService, IYeuCauLinhKSNKService ycLinhThuongVatTuService)
        {
            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                .MustAsync(async (viewModel, soLuongTon, id) =>
                {
                    var kiemTraSLTon = await ycLinhThuongVatTuService.CheckSoLuongTonKSNK(viewModel.VatTuBenhVienId.Value, viewModel.SoLuong, viewModel.DuocDuyet, viewModel.SoLuongCoTheXuat, viewModel.KhoXuatId, viewModel.LaVatTuBHYT, viewModel.IsValidator,viewModel.LoaiDuocPhamHayVatTu);
                    return kiemTraSLTon;
                })
                .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));

        }
    }
}
