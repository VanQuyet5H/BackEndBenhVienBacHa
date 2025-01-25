using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.XacNhanBHYTs;

namespace Camino.Api.Controllers
{
    public partial class XacNhanBHYTController : CaminoBaseController
    {        
        private readonly IBhytDvChuaHuongService _bhytDvChuaHuongService;
        private readonly IXacNhanBHYTService _xacNhanBhytService;
        private readonly IExcelService _excelService;
        private readonly IBhytDvHuongService _bhytDvHuongService;
        private readonly IBhytConfirmByDayService _bhytConfirmByDayService;
        private readonly ILocalizationService _localizationService;
        private readonly IBhytXacNhanNoiTruService _xnBhytNoiTruService;
        private readonly IBhytListNoiTruService _bhytListNoiTruService;
        private readonly IXacNhanBHYTNoiTruService _xnBhytNoiTruListService;
        private readonly IPdfService _pdfService;
        private readonly IXacNhanNoiTruVaNgoaiTruBHYTService _xacNhanNoiTruVaNgoaiTruBHYTService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;

        public XacNhanBHYTController
        (
            IBhytDvChuaHuongService bhytDvChuaHuongService,
            IXacNhanBHYTService xacNhanBhytService,
            IBhytDvHuongService bhytDvHuongService,
            IExcelService excelService,
            IBhytConfirmByDayService bhytConfirmByDayService,
            ILocalizationService localizationService,
            IBhytXacNhanNoiTruService xnBhytNoiTruService,
            IBhytListNoiTruService bhytListNoiTruService,
            IXacNhanBHYTNoiTruService xnBhytNoiTruListService,
            IPdfService pdfService,
            IXacNhanNoiTruVaNgoaiTruBHYTService xacNhanNoiTruVaNgoaiTruBHYTService,
            ITiepNhanBenhNhanService tiepNhanBenhNhanService
        )
        {
            _pdfService = pdfService;
            _bhytDvChuaHuongService = bhytDvChuaHuongService;
            _xacNhanBhytService = xacNhanBhytService;
            _bhytDvHuongService = bhytDvHuongService;
            _excelService = excelService;
            _bhytConfirmByDayService = bhytConfirmByDayService;
            _localizationService = localizationService;
            _xnBhytNoiTruService = xnBhytNoiTruService;
            _bhytListNoiTruService = bhytListNoiTruService;
            _xnBhytNoiTruListService = xnBhytNoiTruListService;
            _xacNhanNoiTruVaNgoaiTruBHYTService = xacNhanNoiTruVaNgoaiTruBHYTService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
        }
    }
}