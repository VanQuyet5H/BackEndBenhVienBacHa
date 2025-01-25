using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DichVuKyThuatBenhVien;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DichVuKyThuatBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DichVuKyThuatBenhVienViewModel>))]
    public class DichVuKyThuatBenhVienViewModelValidator : AbstractValidator<DichVuKyThuatBenhVienViewModel>
    {
        public DichVuKyThuatBenhVienViewModelValidator(ILocalizationService localizationService, IDichVuKyThuatBenhVienService _dichVuKyThuatBenhVienService
            , IValidator<DichVuKyThuatBenhVienGiaBaoHiemViewModel> giaBaoHiemValidator
            , IValidator<DichVuKyThuatVuBenhVienGiaBenhVienViewModel> giaBenhVienValidator
            , IValidator<DichVuKyThuatBenhVienTiemChungViewModel> dichVuKyThuatBenhVienTiemChungValidator
            , IValidator<DinhMucDuocPhamVTYTTrongDichVuViewModel> dinhMucDuocPhamVTYTTrongDichVuValidator)
        {
            RuleFor(x => x.NhomDichVuBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.NhomDichVuBenhVienId.Required"));
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"));
                //.MustAsync(async (s,input,a) => !await _dichVuKyThuatBenhVienService.IsExistsMaDichVuKyThuatBenhVien(s.Id,input)).WithMessage(localizationService.GetResource("Common.Ma.Exists"));
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));
            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));
            RuleFor(x => x.DichVuKyThuatId)
                .Must((s, d) => !(s.AnhXa != null && s.AnhXa.Value && s.DichVuKyThuatId == null)).WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatId.Required"));
            RuleFor(x => x.NgayBatDau)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.NgayBatDau.Required"));

            RuleFor(x => x.KhoaPhongIds)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.KhoaPhongIds.Required"));
            RuleFor(x => x.NoiThucHienIds)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.NoiThucHienIds.Required"));

            RuleFor(x => x.DichVuKyThuatBenhVienTiemChung)
                .SetValidator(dichVuKyThuatBenhVienTiemChungValidator).When(s => s.LaVacxin == true);

            RuleForEach(x => x.DichVuKyThuatBenhVienGiaBaoHiems).SetValidator(giaBaoHiemValidator);
            RuleForEach(x => x.DichVuKyThuatVuBenhVienGiaBenhViens).SetValidator(giaBenhVienValidator);
            RuleForEach(x => x.DinhMucDuocPhamVTYTTrongDichVus).SetValidator(dinhMucDuocPhamVTYTTrongDichVuValidator);
        }
    }
}