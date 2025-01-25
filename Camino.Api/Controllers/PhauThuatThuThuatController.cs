using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.Localization;
using Camino.Services.KhamBenhs;
using Camino.Services.PhauThuatThuThuat;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.YeuCauTiepNhans;
using Camino.Services.Users;
using Camino.Services.DieuTriNoiTru;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController : CaminoBaseController
    {
        private readonly IPhauThuatThuThuatService _phauThuatThuThuatService;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        private readonly IPhongBenhVienHangDoiService _phongBenhVienHangDoiService;
        private readonly ITheoDoiSauPhauThuatThuThuatService _theoDoiSauPhauThuatThuThuatService;
        private readonly IYeuCauDichVuKyThuatTuongTrinhPTTTService _yeuCauDichVuKyThuatTuongTrinhPtttService;
        private readonly IYeuCauDichVuKyThuatService _yeuCauDichVuKyThuatService;
        private readonly IUserService _userService;
        private readonly IKhamTheoDoiService _khamTheoDoiService;
        private readonly IKhamTheoDoiBoPhanKhacService _khamTheoDoiBoPhanKhacService;
        private readonly IKhamBenhService _khamBenhService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IUserAgentHelper _userAgentHelper; 
        private readonly ITiepNhanBenhNhanService _yctnService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        private readonly IYeuCauGoiDichVuService _yeuCauGoiDichVuService;
        private readonly IThuNganNoiTruService _thuNganNoiTruService;
        private readonly ICauHinhService _cauHinhService;

        public PhauThuatThuThuatController(
            IPhauThuatThuThuatService phauThuatThuThuatService,
            ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
            IPhongBenhVienHangDoiService phongBenhVienHangDoiService,
            ITheoDoiSauPhauThuatThuThuatService theoDoiSauPhauThuatThuThuatService,
            IYeuCauDichVuKyThuatTuongTrinhPTTTService yeuCauDichVuKyThuatTuongTrinhPtttService,
            IYeuCauDichVuKyThuatService yeuCauDichVuKyThuatService,
            IKhamTheoDoiService khamTheoDoiService,
            IKhamTheoDoiBoPhanKhacService khamTheoDoiBoPhanKhacService,
            IKhamBenhService khamBenhService,
            ITiepNhanBenhNhanService tiepNhanBenhNhanService,
            ILocalizationService localizationService,
            IExcelService excelService,
            ITiepNhanBenhNhanService yctnService,
            IUserAgentHelper userAgentHelper,
            IYeuCauKhamBenhService yeuCauKhamBenhService,
            IYeuCauTiepNhanService yeuCauTiepNhanService,
            IUserService userService,
            IDieuTriNoiTruService dieuTriNoiTruService,
            IYeuCauGoiDichVuService yeuCauGoiDichVuService,
            IThuNganNoiTruService thuNganNoiTruService,
            ICauHinhService cauHinhService
        )
        {
            _phauThuatThuThuatService = phauThuatThuThuatService;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _phongBenhVienHangDoiService = phongBenhVienHangDoiService;
            _theoDoiSauPhauThuatThuThuatService = theoDoiSauPhauThuatThuThuatService;
            _yeuCauDichVuKyThuatTuongTrinhPtttService = yeuCauDichVuKyThuatTuongTrinhPtttService;
            _yeuCauDichVuKyThuatService = yeuCauDichVuKyThuatService;
            _khamTheoDoiService = khamTheoDoiService;
            _khamTheoDoiBoPhanKhacService = khamTheoDoiBoPhanKhacService;
            _khamBenhService = khamBenhService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
            _localizationService = localizationService;
            _excelService = excelService;
            _yctnService = yctnService;
            _userAgentHelper = userAgentHelper;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _userService = userService;
            _dieuTriNoiTruService = dieuTriNoiTruService;
            _yeuCauGoiDichVuService = yeuCauGoiDichVuService;
            _thuNganNoiTruService = thuNganNoiTruService;
            _cauHinhService = cauHinhService;
        }
    }
}