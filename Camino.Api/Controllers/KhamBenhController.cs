using Camino.Services.BenhNhans;
using Camino.Services.KetQuaSinhHieus;
using Camino.Services.KhamBenhs;
using Camino.Services.YeuCauTiepNhans;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.Helpers;
using Camino.Services.DichVuKhamBenhBenhViens;
using Camino.Services.CauHinh;
using Camino.Services.DichVuKyThuatBenhVien;
using Camino.Services.Localization;
using Camino.Services.DichVuGiuongBenhVien;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.YeucCauKhamBenhChanDoanPhanBiet;
using Camino.Services.ICDs;
using Camino.Services.YeuCauKhamBenhKhamBoPhanKhac;
using Camino.Services.BenhVien;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.YeuCauKhambenhBoPhanTonThuong;
using Camino.Services.ExportImport;
using Camino.Services.KhamDoan;
using Camino.Services.Thuocs;
using Wkhtmltopdf.NetCore;
using Camino.Services.KetQuaXetNghiem;
using Camino.Services.DuyetKetQuaXetNghiems;
using Camino.Services.PhauThuatThuThuat;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.Users;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController : CaminoBaseController
    {
        private readonly IChuanDoanService _ChuanDoanService;
        private readonly IBenhVienService _benhVienService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;
        private readonly IKhamBenhService _khamBenhService;
        private readonly IYeuCauKhamBenhKhamBoPhanKhacService _yeuCauKhamBenhKhamBoPhanKhacService;
        private readonly IYeuCauKhamBenhChanDoanPhanBietService _yeuCauKhamBenhChanDoanPhanBietService;
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IDanhSachChoKhamService _danhSachChoKhamService;
        private readonly IBenhNhanTienSuBenhService _benhNhanTienSuBenhService;
        private readonly IBenhNhanDiUngThuocService _benhNhanDiUngThuocService;
        private readonly ICauHinhService _cauhinhService;
        private readonly IYeuCauKhamBenhTrieuChungService _yeuCauKhamBenhTrieuChungService;
        private readonly IYeuCauKhamBenhICDKhacService _yeuCauKhamBenhICDKhacService;
        private readonly IDichVuKhamBenhBenhVienService _dichVuKhamBenhBenhVienService;
        private readonly IYeuCauKhamBenhChuanDoanService _yeuCauKhamBenhChuanDoanService;
        private readonly IKetQuaSinhHieuService _ketQuaSinhHieuService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IYeuCauKhambenhBoPhanTonThuongService _yeuCauKhambenhBoPhanTonThuongService;
        private readonly IYeuCauDichVuKyThuatService _yeuCauDichVuKyThuatService;
        private readonly IYeuCauDichVuKyThuatTuongTrinhPTTTService _yeuCauDichVuKyThuatTuongTrinhPtttService;
        private readonly IYeuCauTuongTrinhService _yeuCauTuongTrinhService;
        private readonly IPhongBenhVienHangDoiService _phongBenhVienHangDoiService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanServiceService;
        private readonly IYeuCauDichVuGiuongBenhService _yeuCauDichVuGiuongBenhService;
        private readonly IYeuCauVatTuBenhVienService _yeuCauVatTuBenhVienService;
        private readonly IYeuCauGoiDichVuService _yeuCauGoiDichVuService;
        private readonly IYeuCauDuocPhamBenhVienService _yeuCauDuocPhamBenhVienService;
        private readonly IYeuCauKhamBenhDonThuocService _yeuCauKhamBenhDonThuocService;
        private readonly IYeuCauKhamBenhDonThuocChiTietService _yeuCauKhamBenhDonThuocChiTietService;
        private readonly ILocalizationService _localizationService;
        private readonly IDichVuKyThuatBenhVienService _dichVuKyThuatBenhVienService;
        private readonly IDichVuGiuongBenhVienService _dichVuGiuongBenhVienService;
        private readonly ITaiKhoanBenhNhanService _taikhoanBenhNhanService;
        private readonly IExcelService _excelService;
        private readonly IDuocPhamService _thuocBenhVienService;
        private readonly ICauHinhService _cauHinhService;
        private readonly IKhamDoanService _khamDoanService;
        private readonly IKetQuaXetNghiemService _ketQuaXetNghiemService;
        private IDuyetKetQuaXetNghiemService _duyetKetQuaXetNghiemService;
        private IPdfService _pdfService;
        readonly IGeneratePdf _generatePdf;
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        private readonly IPhauThuatThuThuatService _phauThuatThuThuatService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IRoleService _roleService;
        public KhamBenhController(
            IKhamBenhService khamBenhService,
            IYeuCauTiepNhanService yeuCauTiepNhanService,
            IDanhSachChoKhamService danhSachChoKhamService,
            IBenhNhanTienSuBenhService benhNhanTienSuBenhService,
            IBenhNhanDiUngThuocService benhNhanDiUngThuocService,
            IYeuCauKhamBenhTrieuChungService yeuCauKhamBenhTrieuChungService,
            IYeuCauKhamBenhICDKhacService yeuCauKhamBenhICDKhacService,
            IDichVuKhamBenhBenhVienService dichVuKhamBenhBenhVienService,
            IYeuCauKhamBenhChuanDoanService yeuCauKhamBenhChuanDoanService,
            IKetQuaSinhHieuService ketQuaSinhHieuService,
            IYeuCauKhamBenhService yeuCauKhamBenhService,
            IYeuCauDichVuKyThuatService yeuCauDichVuKyThuatService,
            IPhongBenhVienHangDoiService phongBenhVienHangDoiService,
            IUserAgentHelper userAgentHelper,
            ICauHinhService cauhinhService,
            IYeuCauDichVuGiuongBenhService yeuCauDichVuGiuongBenhService,
            IYeuCauVatTuBenhVienService yeuCauVatTuBenhVienService,
            IYeuCauGoiDichVuService yeuCauGoiDichVuService,
            IYeuCauDuocPhamBenhVienService yeuCauDuocPhamBenhVienService,
            IYeuCauKhamBenhDonThuocService yeuCauKhamBenhDonThuocService,
            IYeuCauKhamBenhDonThuocChiTietService yeuCauKhamBenhDonThuocChiTietService,
            ILocalizationService localizationService,
            IDichVuKyThuatBenhVienService dichVuKyThuatBenhVienService,
            IDichVuGiuongBenhVienService dichVuGiuongBenhVienService,
            ITiepNhanBenhNhanService tiepNhanBenhNhanServiceService,
            IYeuCauKhamBenhKhamBoPhanKhacService yeuCauKhamBenhKhamBoPhanKhacService,
            IYeuCauKhamBenhChanDoanPhanBietService yeuCauKhamBenhChanDoanPhanBietService,
            ITiepNhanBenhNhanService tiepNhanBenhNhanService,
            IBenhVienService benhVienService,
            IYeuCauTuongTrinhService yeuCauTuongTrinhService,
            IYeuCauDichVuKyThuatTuongTrinhPTTTService yeuCauDichVuKyThuatTuongTrinhPtttService,
            IYeuCauKhambenhBoPhanTonThuongService yeuCauKhambenhBoPhanTonThuongService,
            ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
            IExcelService excelService,
            IDuocPhamService thuocBenhVienService,
            ICauHinhService cauHinhService,
            IPdfService pdfService, IGeneratePdf generatePdf,
            IKhamDoanService khamDoanService,
            IKetQuaXetNghiemService ketQuaXetNghiemService,
            IDuyetKetQuaXetNghiemService duyetKetQuaXetNghiemService,
            IDieuTriNoiTruService dieuTriNoiTruService,
            IPhauThuatThuThuatService phauThuatThuThuatService,
            ITaiLieuDinhKemService taiLieuDinhKemService,
            IRoleService roleService
        )
        {
            _khamBenhService = khamBenhService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _danhSachChoKhamService = danhSachChoKhamService;
            _benhNhanTienSuBenhService = benhNhanTienSuBenhService;
            _benhNhanDiUngThuocService = benhNhanDiUngThuocService;
            _yeuCauKhamBenhTrieuChungService = yeuCauKhamBenhTrieuChungService;
            _yeuCauKhamBenhICDKhacService = yeuCauKhamBenhICDKhacService;
            _dichVuKhamBenhBenhVienService = dichVuKhamBenhBenhVienService;
            _yeuCauKhamBenhChuanDoanService = yeuCauKhamBenhChuanDoanService;
            _ketQuaSinhHieuService = ketQuaSinhHieuService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _yeuCauDichVuKyThuatService = yeuCauDichVuKyThuatService;
            _phongBenhVienHangDoiService = phongBenhVienHangDoiService;
            _userAgentHelper = userAgentHelper;
            _cauhinhService = cauhinhService;
            _yeuCauDichVuGiuongBenhService = yeuCauDichVuGiuongBenhService;
            _yeuCauVatTuBenhVienService = yeuCauVatTuBenhVienService;
            _yeuCauGoiDichVuService = yeuCauGoiDichVuService;
            _yeuCauDuocPhamBenhVienService = yeuCauDuocPhamBenhVienService;
            _yeuCauKhamBenhDonThuocService = yeuCauKhamBenhDonThuocService;
            _yeuCauKhamBenhDonThuocChiTietService = yeuCauKhamBenhDonThuocChiTietService;
            _localizationService = localizationService;
            _dichVuKyThuatBenhVienService = dichVuKyThuatBenhVienService;
            _dichVuGiuongBenhVienService = dichVuGiuongBenhVienService;
            _tiepNhanBenhNhanServiceService = tiepNhanBenhNhanServiceService;
            _yeuCauKhamBenhKhamBoPhanKhacService = yeuCauKhamBenhKhamBoPhanKhacService;
            _yeuCauKhamBenhChanDoanPhanBietService = yeuCauKhamBenhChanDoanPhanBietService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
            _benhVienService = benhVienService;
            _yeuCauTuongTrinhService = yeuCauTuongTrinhService;
            _yeuCauDichVuKyThuatTuongTrinhPtttService = yeuCauDichVuKyThuatTuongTrinhPtttService;
            _yeuCauKhambenhBoPhanTonThuongService = yeuCauKhambenhBoPhanTonThuongService;
            _taikhoanBenhNhanService = taiKhoanBenhNhanService;
            _excelService = excelService;
            _thuocBenhVienService = thuocBenhVienService;
            _cauHinhService = cauHinhService;
            _pdfService = pdfService;
            _generatePdf = generatePdf;
            _khamDoanService = khamDoanService;
            _ketQuaXetNghiemService = ketQuaXetNghiemService;
            _duyetKetQuaXetNghiemService = duyetKetQuaXetNghiemService;
            _dieuTriNoiTruService = dieuTriNoiTruService;
            _phauThuatThuThuatService = phauThuatThuThuatService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _roleService = roleService;
        }
    }
}