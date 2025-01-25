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
   
    [TransientDependency(ServiceType = typeof(IValidator<KSNKGridViewModel>))]

    public class KSNKGridViewModelValidator : AbstractValidator<KSNKGridViewModel>
    {
        public KSNKGridViewModelValidator(ILocalizationService localizationService, IYeuCauLinhKSNKService ycLinhThuongKSNKService)
        {
            RuleFor(x => x.VatTuBenhVienId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongKSNK.TenKSNK.Required"))
               .MustAsync(async (viewModel, soLuongTon, id) =>
               {
                   var kiemTraExists = await ycLinhThuongKSNKService.CheckKSNKExists(viewModel.VatTuBenhVienId, viewModel.LaVatTuBHYT, viewModel.VatTuBenhViens, viewModel.LoaiDuocPhamHayVatTu);
                   return kiemTraExists;
               })
                .WithMessage(localizationService.GetResource("LinhThuongKSNK.TenKSNK.Exists"))
                ;

            RuleFor(x => x.SLYeuCau)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                .MustAsync(async (viewModel, soLuongTon, id) =>
                {
                    var kiemTraSLTon = await ycLinhThuongKSNKService.CheckSoLuongTonKSNKGridVo(viewModel.VatTuBenhVienId, viewModel.SLYeuCau, viewModel.KhoXuatId, viewModel.LaVatTuBHYT, viewModel.LoaiDuocPhamHayVatTu);
                    return kiemTraSLTon;
                })
                .WithMessage(localizationService.GetResource("YeuCauLinhDuocPham.SLYeuCau.NotValid"));
        }
    }
}
