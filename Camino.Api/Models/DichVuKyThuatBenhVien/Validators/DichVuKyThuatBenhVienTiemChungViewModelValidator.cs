using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DichVuKyThuatBenhVien;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DichVuKyThuatBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuKyThuatBenhVienTiemChungViewModel>))]
    public class DichVuKyThuatBenhVienTiemChungViewModelValidator: AbstractValidator<DichVuKyThuatBenhVienTiemChungViewModel>
    {
        public DichVuKyThuatBenhVienTiemChungViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DuocPhamBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuatBenhVienTiemChung.DuocPhamBenhVienId.Required"));
        }
    }
    [TransientDependency(ServiceType = typeof(IValidator<DinhMucDuocPhamVTYTTrongDichVuViewModel>))]
    public class DinhMucDuocPhamVTYTTrongDichVuViewModelValidator : AbstractValidator<DinhMucDuocPhamVTYTTrongDichVuViewModel>
    {
        public DinhMucDuocPhamVTYTTrongDichVuViewModelValidator(ILocalizationService localizationService, IDichVuKyThuatBenhVienService dichVuKyThuatBenhVienService)
        {
            RuleFor(x => x.DuocPhamBenhVienId)
                 .MustAsync(async (request, ten, id) =>
                 {
                     var val = await dichVuKyThuatBenhVienService.KiemTraIsNull(request.LaDuocPham, request.DuocPhamBenhVienId, request.VatTuBenhVienId);
                     return val;
                 }).WithMessage(localizationService.GetResource("DichVuKyThuatBenhVienTiemChung.DuocPhamBenhVienId.Required"));
            RuleFor(x => x.SoLuong)
               .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuatBenhVienTiemChung.SoLuong.Required"))
               .MustAsync(async (request, ten, id) =>
               {
                   var val = await dichVuKyThuatBenhVienService.KiemTraSLTonDuocPhamVatTu((double)request.SoLuong, request.DuocPhamBenhVienId,request.VatTuBenhVienId);
                   return val;
               }).WithMessage(localizationService.GetResource("DichVuKyThuatBenhVienTiemChung.SoLuongTonKhongDu.Required"));


        }
    }
}
