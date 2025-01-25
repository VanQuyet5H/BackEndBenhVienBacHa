using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru
{
    [TransientDependency(ServiceType = typeof(IValidator<GiayChungSinhNewVo>))]
    public class GiayChungSinhNewValidator : AbstractValidator<GiayChungSinhNewVo>
    {
        public GiayChungSinhNewValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            RuleFor(x => x.So)
            .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.So.Required"));
           // .MustAsync(async (request, ten, id) =>
           // {
           //     var checkExistsTen = dieuTriNoiTruService.kiemTrungSoChungSinh(request.So, request.YeuCauTiepNhanId,request.NoiTruHoSoKhacGiayChungSinhId);
           //     return checkExistsTen;
           // })
           //.WithMessage(localizationService.GetResource("Common.SoChungSinhTrung.Required"));
            RuleFor(x => x.QuyenSo)
            .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.QuyenSo.Required"));
            RuleFor(x => x.CMND)
            .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.CMND.Required"));
            RuleFor(x => x.NgayCap)
            .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NgayCap.Required"));
            RuleFor(x => x.NoiCap)
           .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NoiCap.Required"));
            RuleFor(x => x.DuDinhDatTenCon)
           .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.DuDinhDatTenCon.Required"));
            //Ngày thực hiện mặc định là ngày hiện tại cho phép người dùng sửa, nhưng không được trước thời gian tiếp nhận và sau thời gian hiện tại
            RuleFor(x => x.NgayCapGiayChungSinh)
           .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NgayCapGiayChungSinh.Required"))
           .MustAsync(async (request, ten, id) =>
           {
               var checkExistsTen =  dieuTriNoiTruService.CheckNgayCapGiayChungSinh(request.YeuCauTiepNhanId, request.NgayCapGiayChungSinh);
               return checkExistsTen;
           })
           .WithMessage(localizationService.GetResource("Common.NgayCapGiayChungSinhKhongDuocTruocThoiGianThucHienVaThoiGianHienTai.Required"));
        }
    }
}
