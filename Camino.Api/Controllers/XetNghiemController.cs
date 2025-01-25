using Camino.Services.DuyetKetQuaXetNghiems;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.XetNghiem;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class XetNghiemController : CaminoBaseController
    {
        private readonly IXetNghiemService _xetNghiemService;
        private readonly IExcelService _excelService;
        private ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private IDuyetKetQuaXetNghiemService _duyetKetQuaXetNghiemService;
        private IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private IPdfService _pdfService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;

        public XetNghiemController(
            IXetNghiemService xetNghiemService,
            IExcelService excelService,
            ILocalizationService localizationService,
            IUserAgentHelper userAgentHelper,
            IYeuCauKhamBenhService yeuCauKhamBenhService,
            IDuyetKetQuaXetNghiemService duyetKetQuaXetNghiemService,
            IYeuCauTiepNhanService yeuCauTiepNhanService,
            IPdfService pdfService,
            ITaiLieuDinhKemService taiLieuDinhKemService,
            ITiepNhanBenhNhanService tiepNhanBenhNhanService
        )
        {
            _xetNghiemService = xetNghiemService;
            _excelService = excelService;
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _duyetKetQuaXetNghiemService = duyetKetQuaXetNghiemService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _pdfService = pdfService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
        }
    }
}
