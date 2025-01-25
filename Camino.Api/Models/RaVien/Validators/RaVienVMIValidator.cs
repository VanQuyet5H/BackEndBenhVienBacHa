using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using System;

namespace Camino.Api.Models.RaVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<RaVienVM>))]
    public class RaVienVMIValidator : AbstractValidator<RaVienVM>
    {
        public RaVienVMIValidator(ILocalizationService localizationService, IDieuTriNoiTruService noiTruService)
        {
            RuleFor(x => x.KetQuaDieuTriId).NotEmpty().WithMessage("Vui lòng chọn kết quả điều trị");
            //RuleFor(x => x.GiaPhauThuatId).NotEmpty().WithMessage("Vui lòng chọn giải phẩu thuật");

            RuleFor(x => x.ThoiGianRaVien).NotEmpty().WithMessage("Vui lòng chọn thời gian ra viện")
                .MustAsync(async (model, input, s) => await noiTruService.KiemTraNgayRaVien(model.YeuCauTiepNhanId, input).ConfigureAwait(false))
                .WithMessage("Thời gian ra viện lớn hơn thời gian nhập viện");

            //BVHD-3692
            RuleFor(x => x.ThoiGianRaVien)
                        .Must((request, ThoiGianRaVien, id) =>
                        {
                            if (ThoiGianRaVien != null && ThoiGianRaVien.Value.Year > DateTime.Now.Year + 1)
                                return false;
                            return true;
                        }).WithMessage($"Năm ra viện không được lớn hơn năm {DateTime.Now.Year + 1}");


            RuleFor(x => x.HinhThucRaVienId).NotEmpty().WithMessage("Vui lòng chọn hình thức ra viện");

            RuleFor(x => x.NgayHienTaiKham)
                         .NotEmpty().WithMessage("Ngày hẹn tái khám bắt buộc nhập").When(p => p.HenTaiKham == true);

            RuleFor(x => x.NgayHienTaiKham)
                         .Must((request, NgayHienTaiKham, id) =>
                         {
                             if (NgayHienTaiKham != null && NgayHienTaiKham < request.ThoiGianRaVien)
                                 return false;
                             return true;
                         }).WithMessage("Ngày hẹn tái khám lớn hơn thời gian ra viện")
                         .When(p => p.HenTaiKham == true);


        }
    }
}
