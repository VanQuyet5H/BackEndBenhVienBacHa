using Camino.Api.Infrastructure.Auth;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using Camino.Services.Users;
using Camino.Services.YeuCauLinhDuocPham;
using Camino.Services.YeuCauLinhKSNK;
using Camino.Services.YeuCauLinhVatTu;

namespace Camino.Api.Controllers
{

    public partial class YeuCauLinhKSNKController : CaminoBaseController
    {
        private readonly IYeuCauLinhKSNKService _yeuCauLinhKSNKService;
        private readonly IYeuCauLinhDuocPhamService _yeuCauLinhDuocPhamService;
        private readonly IYeuCauLinhVatTuService _yeuCauLinhVatTuService;
        private readonly IYeuCauVatTuBenhVienService _yeuCauVatTuBenhVienService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IExcelService _excelService;
        public YeuCauLinhKSNKController(
          IYeuCauLinhKSNKService yeuCauLinhKSNKService,
          ILocalizationService localizationService,
          IUserService userService,
          IUserAgentHelper userAgentHelper,
          IYeuCauVatTuBenhVienService yeuCauVatTuBenhVienService,
          IExcelService excelService,
          IYeuCauLinhDuocPhamService yeuCauLinhDuocPhamService,
          IYeuCauLinhVatTuService yeuCauLinhVatTuService)
        {
            _yeuCauLinhKSNKService = yeuCauLinhKSNKService;
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _userService = userService;
            _yeuCauVatTuBenhVienService = yeuCauVatTuBenhVienService;
            _excelService = excelService;
            _yeuCauLinhDuocPhamService = yeuCauLinhDuocPhamService;
            _yeuCauLinhVatTuService = yeuCauLinhVatTuService;
        }
    }
}
