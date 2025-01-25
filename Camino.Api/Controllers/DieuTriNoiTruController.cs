using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Camino.Services.BenhNhans;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.YeuCauTiepNhans;
using Camino.Services.Localization;
using Camino.Services.NoiDuTruHoSoKhac;
using Camino.Services.TiepNhanBenhNhan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.KhamBenhs;
using Camino.Services.PhauThuatThuThuat;
using Camino.Services.NoiDuTruHoSoKhacFileDinhKem;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.KetQuaSinhHieus;
using Camino.Services.NoiTruBenhAn;
using Camino.Services.Users;
using Camino.Services.ICDs;
using Camino.Services.NhanVien;
using AutoMapper;
using Camino.Services.BenhVien;
using Camino.Services.PhongBenhVien;
using Camino.Core.Domain.ValueObject;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Services.CauHinh;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.NoiDungMauLoiDanBacSi;
using Camino.Services.ToaThuocMau;
using Camino.Services.InputStringStored;
using Camino.Services.KhoaPhong;
using Camino.Core.Infrastructure;
using Camino.Services.DichVuKhamBenh;
using Camino.Services.DichVuKhamBenhBenhViens;
using Camino.Services.TinhTrangRaVienHoSoKhac;
using Camino.Core.Domain.Entities.BenhNhans;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class DieuTriNoiTruController : CaminoBaseController
    {
        private readonly IDieuTriNoiTruService _dieuTriNoiTruService;
        private readonly INoiDuTruHoSoKhacService _noiDuTruHoSoKhacService;
        private readonly INoiTruHoSoKhacService _noiTruHoSoKhacService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IExcelService _excelService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IKhamBenhService _khamBenhService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;
        private readonly ILocalizationService _localizationService;
        private readonly IPhauThuatThuThuatService _phauThuatThuThuatService;
        private readonly INoiDuTruHoSoKhacFileDinhKemService _noiDuTruHoSoKhacFileDinhKemService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IKetQuaSinhHieuService _ketQuaSinhHieuService;
        private readonly INoiTruBenhAnService _noiTruBenhAnService;
        private readonly IUserService _userService;
        private readonly IICDService _icdService;
        private readonly IDanhSachChoKhamService _danhSachChoKhamService;
        private readonly IPhongBenhVienService _phongBenhVienService;
        private readonly INhanVienService _nhanVienService;
        private IMapper _mapper;
        private readonly IBenhVienService _benhVienService;
        private readonly IThuNganNoiTruService _thuNganNoiTruService;
        private readonly IPdfService _pdfService;
        private readonly IYeuCauGoiDichVuService _yeuCauGoiDichVuService;
        private readonly ICauHinhService _cauHinhService;
        private readonly IYeuCauDichVuKyThuatService _yeuCauDichVuKyThuatService;
        private readonly INoiDungMauLoiDanBacSiService _noiDungMauLoiDanBacSiService;
        IToaThuocMauService _toaThuocMauService;
        private readonly IInputStringStoredService _inputStringStoredService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly IRoleService _roleService;
        private readonly ITinhTrangRaVienHoSoKhacService _tinhTrangRaVienHoSoKhacService;
        private readonly ILoggerManager _logger;
        private readonly IDichVuKhamBenhBenhVienService _dichVuKhamBenhBenhVienService;
        public DieuTriNoiTruController(
            IUserAgentHelper userAgentHelper,
            IDieuTriNoiTruService dieuTriNoiTruService,
            ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
            INoiTruHoSoKhacService noiTruHoSoKhacService,
            IYeuCauTiepNhanService yeuCauTiepNhanService,
            IExcelService excelService,
            IYeuCauKhamBenhService yeuCauKhamBenhService,
            ITiepNhanBenhNhanService tiepNhanBenhNhanService,
            ILocalizationService localizationService,
            IKhamBenhService khamBenhService,
            IPhauThuatThuThuatService phauThuatThuThuatService,
            INoiDuTruHoSoKhacService noiDuTruHoSoKhacService,
            ITaiLieuDinhKemService taiLieuDinhKemService,
            INoiDuTruHoSoKhacFileDinhKemService noiDuTruHoSoKhacFileDinhKemService,
            IKetQuaSinhHieuService ketQuaSinhHieuService,
            INoiTruBenhAnService noiTruBenhAnService,
            IUserService userService,
            IICDService icdService,
            IDanhSachChoKhamService danhSachChoKhamService,
            INhanVienService nhanVienService,
            IMapper mapper,
            IPdfService pdfService,
            IPhongBenhVienService phongBenhVienService,
            IBenhVienService benhVienService,
            IThuNganNoiTruService thuNganNoiTruService,
            IYeuCauGoiDichVuService yeuCauGoiDichVuService,
            ICauHinhService cauHinhService,
            IYeuCauDichVuKyThuatService yeuCauDichVuKyThuatService,
            INoiDungMauLoiDanBacSiService noiDungMauLoiDanBacSiService,
             IToaThuocMauService toaThuocMauService,
            IInputStringStoredService inputStringStoredService,
            IKhoaPhongService khoaPhongService,
            ILoggerManager logger,
            IRoleService roleService
            ,ITinhTrangRaVienHoSoKhacService tinhTrangRaVienHoSoKhacService
            , IDichVuKhamBenhBenhVienService dichVuKhamBenhBenhVienService
        )
        {
            _userAgentHelper = userAgentHelper;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _noiTruHoSoKhacService = noiTruHoSoKhacService;
            _dieuTriNoiTruService = dieuTriNoiTruService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _excelService = excelService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
            _khamBenhService = khamBenhService;
            _phauThuatThuThuatService = phauThuatThuThuatService;
            _noiDuTruHoSoKhacService = noiDuTruHoSoKhacService;
            _noiDuTruHoSoKhacFileDinhKemService = noiDuTruHoSoKhacFileDinhKemService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _ketQuaSinhHieuService = ketQuaSinhHieuService;
            _localizationService = localizationService;
            _noiTruBenhAnService = noiTruBenhAnService;
            _userService = userService;
            _icdService = icdService;
            _danhSachChoKhamService = danhSachChoKhamService;
            _nhanVienService = nhanVienService;
            _mapper = mapper;
            _phongBenhVienService = phongBenhVienService;
            _benhVienService = benhVienService;
            _thuNganNoiTruService = thuNganNoiTruService;
            _pdfService = pdfService;
            _yeuCauGoiDichVuService = yeuCauGoiDichVuService;
            _cauHinhService = cauHinhService;
            _yeuCauDichVuKyThuatService = yeuCauDichVuKyThuatService;
            _noiDungMauLoiDanBacSiService = noiDungMauLoiDanBacSiService;
            _toaThuocMauService = toaThuocMauService;
            _inputStringStoredService = inputStringStoredService;
            _khoaPhongService = khoaPhongService;
            _roleService = roleService;
            _tinhTrangRaVienHoSoKhacService = tinhTrangRaVienHoSoKhacService;
            _logger = logger;
            _dichVuKhamBenhBenhVienService = dichVuKhamBenhBenhVienService;
        }

        #region THÔNG TIN CHUNG BÊNH ÁN LookupItemVo        

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDoLot")]
        public ActionResult GetDoLot([FromBody]LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.DoLot>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTinhTrangOi")]
        public ActionResult GetTinhTrangOi([FromBody]LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.TinhTrangVoOi>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTinhTrangOiVo")]
        public ActionResult GetTinhTrangOiVo([FromBody]LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.VoOi>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGioiTinh")]
        public ActionResult GetGioiTinh([FromBody]LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumGioiTinh>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTrangThai")]
        public ActionResult GetTrangThai([FromBody]LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumTrangThaiSong>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetPhuongPhapDe")]
        public ActionResult GetPhuongPhapDe([FromBody]LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.PhuongPhapDe>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTangSinhMon")]
        public ActionResult GetTangSinhMon([FromBody]LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.TangSinhMon>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetCoTuCung")]
        public ActionResult GetCoTuCung([FromBody]LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.CoTuCung>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetKetQuaDieuTri")]
        public ActionResult GetKetQuaDieuTri([FromBody] LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumKetQuaDieuTri>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGiaiPhauThuat")]
        public ActionResult GetGiaiPhauThuat([FromBody] LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.LoaiGiaPhauThuat>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetHinhThucRaVien")]
        public ActionResult GetHinhThucRaVien([FromBody] LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumHinhThucRaVien>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("LyDoChuyenTuyen")]
        public ActionResult LyDoChuyenTuyen([FromBody] LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumLyDoChuyenTuyen>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetNguyenNhanTuVong")]
        public ActionResult GetNguyenNhanTuVong([FromBody] LookupQueryInfo model)
        {

            var listEnum = EnumHelper.GetListEnum<Enums.LyDoTuVong>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTuyenChuyen")]
        public ActionResult GetTuyenChuyen([FromBody] LookupQueryInfo model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.LoaiChuyenTuyen>();
            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            return Ok(result);
        }


        #endregion

        #region THÔNG TIN XỬ LÝ CHUNG 

        [HttpPost("ChonBenhAnMe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoBenhAnSoSinh)]
        public ActionResult<ICollection<ThongTinBenhAnMe>> ChonBenhAnMe([FromBody] DropDownListRequestModel model)
        {
            var lookup = _dieuTriNoiTruService.ChonBenhAnMe(model);
            return Ok(lookup);
        }

        [HttpPost("KhoaChuyenBenhAnSoSinhVe")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.TaoBenhAnSoSinh)]
        public ActionResult<ICollection<LookupItemVo>> KhoaChuyenBenhAnSoSinhVe([FromBody] DropDownListRequestModel model)
        {
            var lookup = _dieuTriNoiTruService.KhoaChuyenBenhAnSoSinhVe(model);
            return Ok(lookup);
        }

        [HttpGet("KiemTraPhieuDieuTriChuaThucHien")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult KiemTraPhieuDieuTriChuaThucHien(long yeuCauTiepNhanId)
        {
            var errors = _dieuTriNoiTruService.KiemTraThongTinKetThucBenhAn(yeuCauTiepNhanId);
            return Ok(errors);
        }


        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru, Enums.DocumentType.TaoBenhAnSoSinh)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ChiTietDieuTriNoiTruViewModel>> Get(long id)
        {
            var result = await _dieuTriNoiTruService.GetByIdAsync(id, s =>
                    s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)
                    .Include(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    .Include(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .Include(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuGiuongs)
                    .Include(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    .Include(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .Include(x => x.BenhNhan).ThenInclude(x => x.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuDichVuGiuongs)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)
                    .Include(x => x.YeuCauTiepNhanTheBHYTs)
                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
                    .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauTiepNhanMe).ThenInclude(x => x.NoiTruBenhAn)
                    .Include(x => x.YeuCauNhapVienCons).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.NoiTruBenhAn)
                    .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)

                    //BVHD-3960
                    .Include(x => x.HinhThucDen)
                    .Include(x => x.NoiGioiThieu)
                    );

            if (result == null)
            {
                return NotFound();
            }

            //Update cấp giường cho trẻ em
            var khoaSan = _cauHinhService.GetSetting("CauHinhNoiTru.KhoaPhuSan");
            long.TryParse(khoaSan?.Value, out long khoaPhuSanId);

            var today = DateTime.Now;

            var benhNhan = result.BenhNhan;
            var tuoiDisplay = string.Empty;
            if ((benhNhan.NgaySinh != null && benhNhan.NgaySinh.GetValueOrDefault() != 0) && (benhNhan.ThangSinh != null && benhNhan.ThangSinh.GetValueOrDefault() != 0) && (benhNhan.NamSinh != null && benhNhan.NamSinh.GetValueOrDefault() != 0))
            {
                var ngaySinh = benhNhan.NgaySinh < 10 ? $"0{benhNhan.NgaySinh}" : benhNhan.NgaySinh.ToString();
                var thangSinh = benhNhan.ThangSinh < 10 ? $"0{benhNhan.ThangSinh}" : benhNhan.ThangSinh.ToString();

                tuoiDisplay = $"{DateTime.Now.Year - result.BenhNhan.NamSinh} ({ngaySinh}/{thangSinh}/{benhNhan.NamSinh})";
            }
            else
            {
                tuoiDisplay = $"{DateTime.Now.Year - result.BenhNhan.NamSinh} ({benhNhan.NamSinh})";
            }

            var r = new ChiTietDieuTriNoiTruViewModel
            {
                Id = id,
                BenhNhanId = result.BenhNhanId,
                MaYeuCauTiepNhan = result.MaYeuCauTiepNhan,
                MaBenhNhan = result.BenhNhan.MaBN,
                HoTen = result.HoTen,
                Tuoi = tuoiDisplay,
                GioiTinh = result.GioiTinh.GetDescription(),
                DiaChi = result.DiaChiDayDu,
                NgheNghiep = result.NgheNghiep?.Ten,
                SoBenhAn = result.NoiTruBenhAn.SoBenhAn,
                LoaiBenhAn = result.NoiTruBenhAn.LoaiBenhAn.GetDescription(),
                LoaiBenhAnEnum = result.NoiTruBenhAn.LoaiBenhAn,
                Khoa = result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any() ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.LastOrDefault(p => DateTime.Now >= p.ThoiDiemVaoKhoa)?.KhoaPhongChuyenDen?.Ten : "",
                KhoaId = result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any() ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.LastOrDefault(p => DateTime.Now >= p.ThoiDiemVaoKhoa)?.KhoaPhongChuyenDenId : 0,
                BacSiDieuTri = result.NoiTruBenhAn.NoiTruEkipDieuTris.Any() ? result.NoiTruBenhAn.NoiTruEkipDieuTris
                                                                                    .Where(cc => cc.TuNgay <= today && (cc.DenNgay == null || cc.DenNgay.Value >= today))
                                                                                    .Select(s => s.BacSi.User.HoTen).Join(", ") : "",
                Giuong = result.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan && p.ThoiDiemBatDauSuDung <= today && (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today)) ? result.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan &&
                                                                                                                                                                        p.ThoiDiemBatDauSuDung <= today &&
                                                                                                                                                                        (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today))?
                                                                                                                                                            .GiuongBenh?.Ten : "",
                Phong = result.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan && p.ThoiDiemBatDauSuDung <= today && (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today)) ? result.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan &&
                                                                                                                                                                       p.ThoiDiemBatDauSuDung <= today &&
                                                                                                                                                                       (p.ThoiDiemKetThucSuDung == null || p.ThoiDiemKetThucSuDung >= today))?
                                                                                                                                                           .GiuongBenh?.PhongBenhVien?.Ten : "",

                //BVHD-3754
                //MucHuong = result.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                //        ? result.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                //            .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                //            .Select(a => a.MucHuong).FirstOrDefault() : (int?)null,
                MucHuong = result.CoBHYT == true ? result.BHYTMucHuong : (int?)null,

                CoBHYT = result.CoBHYT,
                TrangThaiId = (result.NoiTruBenhAn.DaQuyetToan == null || result.NoiTruBenhAn.DaQuyetToan == false) ? Enums.EnumTrangThaiDieuTriNoiTru.DangDieuTri
                    : (result.NoiTruBenhAn.HinhThucRaVien == Enums.EnumHinhThucRaVien.ChuyenVien ? Enums.EnumTrangThaiDieuTriNoiTru.ChuyenVien : Enums.EnumTrangThaiDieuTriNoiTru.DaRaVien),
                SoTaiKhoan = result.BenhNhan.MaBN,

                KetThucBenhAn = result.NoiTruBenhAn.ThoiDiemRaVien != null,
                DaQuyetToan = result.NoiTruBenhAn.DaQuyetToan,
                BenhAnMe = (result.YeuCauNhapVien?.YeuCauTiepNhanMeId != null ? new ThongTinBALink
                {
                    BenhAnId = result.YeuCauNhapVien?.YeuCauTiepNhanMeId,
                    SoBenhAn = result.YeuCauNhapVien?.YeuCauTiepNhanMe?.NoiTruBenhAn?.SoBenhAn
                } : null),
                BenhAnCons = result.YeuCauNhapVienCons?.SelectMany(o => o.YeuCauTiepNhans?.Select(p => new ThongTinBALink
                {
                    BenhAnId = p.Id,
                    SoBenhAn = p.NoiTruBenhAn?.SoBenhAn
                })).ToList(),

                IsDaChiDinhBacSi = result.NoiTruBenhAn.NoiTruEkipDieuTris.Any(),
                IsDaChiDinhGiuong = result.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan),

                // cập nhật 03/06/2021: lấy YeuCauTiepNhanId của ngoại trú -> hiện thông tin tin trong tab Dịch vụ chỉ định ngoại trú
                YeuCauTiepNhanNgoaiTruId = result.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhanId ?? 0,

                //BVHD-3800
                LaCapCuu = result.LaCapCuu ?? result.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu
            };
            //dich vu khuyen mai
            if ((result.BenhNhan.YeuCauGoiDichVus.Any() && result.BenhNhan.YeuCauGoiDichVus.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
                                                                                      z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                                                                                      z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()))
             || (result.BenhNhan.YeuCauGoiDichVuSoSinhs.Any() && result.BenhNhan.YeuCauGoiDichVuSoSinhs.Any(z => z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() ||
                                                                                       z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() ||
                                                                                       z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any())))
            {
                r.CoDichVuKhuyenMai = true;
            }
            //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
            var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(id).Result;
            var soTienDaTamUng = _thuNganNoiTruService.GetSoTienDaTamUngAsync(id).Result;
            r.SoDuTaiKhoan = soTienDaTamUng;
            r.SoTienConLai = soTienDaTamUng - chiPhiKhamChuaBenh;

            //BVHD-3243: cho phép hiển thị số tiền âm
            //r.SoTienConLai = r.SoTienConLai < 0 ? 0 : r.SoTienConLai;
            //Update cấp giường cho trẻ em
            r.KhoaPhuSanId = khoaPhuSanId;
            var currentNoiLLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var khoa = _phongBenhVienService.GetById(currentNoiLLamViecId);

            r.CungKhoaDangNhap = khoa != null && khoa.KhoaPhongId == r.KhoaId;

            #region get thông tin gói marketing của người bệnh
            if (result.BenhNhan.YeuCauGoiDichVus.Any() || result.BenhNhan.YeuCauGoiDichVuSoSinhs.Any())
            {
                var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanDataForGridAsync(new QueryInfo
                {
                    AdditionalSearchString = $"{result.BenhNhanId} ; false",
                    Take = Int32.MaxValue
                });

                var lstGoi = gridData.Data.Select(p => (GoiDichVuTheoBenhNhanGridVo)p).ToList();
                if (lstGoi.Any())
                {
                    r.GoiDichVus = lstGoi;
                }
            }


            #endregion

            #region BVHD-3941
            if (result.CoBHTN == true)
            {
                r.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(result.Id);
            }
            #endregion

            #region BVHD-3960
            r.TenHinhThucDen = result.HinhThucDen?.Ten;
            r.TenNoiGioiThieu = result.NoiGioiThieu?.Ten;
            if (result.HinhThucDenId != 0 && result.NoiGioiThieuId != 0)
            {
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);
                r.LaHinhThucDenGioiThieu = hinhThucDenGioiThieuId != 0 && result.HinhThucDenId == hinhThucDenGioiThieuId;
            }
            #endregion
            return Ok(r);
        }

        [HttpGet("KiemTraKetThucBenhAn/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<bool> KiemTraKetThucBenhAn(long id)
        {
            var yeuCauTiepNhan = _dieuTriNoiTruService.GetById(id,
                    s => s.Include(k => k.NoiTruBenhAn));
            var ktKetThucBenhAn = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien;
            return Ok(ktKetThucBenhAn);
        }

        [HttpGet("KiemTraKetBenhAnKhiThuTien/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<bool> KiemTraKetBenhAnKhiThuTien(long id)
        {
            var yeuCauTiepNhan = _dieuTriNoiTruService.GetById(id,
                    s => s.Include(k => k.NoiTruBenhAn));
            var ktKetThucBenhAn = yeuCauTiepNhan.NoiTruBenhAn.DaQuyetToan;
            return Ok(ktKetThucBenhAn);
        }

        [HttpPost("InPhieuThu")]
        public ActionResult<List<HtmlToPdfVo>> InPhieuThu(long yeuCauTiepNhanId, string hostingName)
        {
            var phieuThuNgans = new List<HtmlToPdfVo>();

            var footerHtml = @"<!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <script charset='utf-8'>

                function replaceParams() {
                  var url = window.location.href
                    .replace(/#$/, '');
                  var params = (url.split('?')[1] || '').split('&');
                  for (var i = 0; i < params.length; i++) {
                      var param = params[i].split('=');
                      var key = param[0];
                      var value = param[1] || '';
                      var regex = new RegExp('{' + key + '}', 'g');
                      document.body.innerText = document.body.innerText.replace(regex, value);
                  }
                }
                </script>
            </head>
            <body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}
            </body>
            </html>";

            var yeuCauTiepNhan = _dieuTriNoiTruService.GetById(yeuCauTiepNhanId, s => s.Include(k => k.NoiTruBenhAn).Include(k => k.TaiKhoanBenhNhanThus));

            if (yeuCauTiepNhan.NoiTruBenhAn != null &&
                yeuCauTiepNhan.NoiTruBenhAn.DaQuyetToan == true)
            {
                var taiKhoanbenhNhanThuIds = yeuCauTiepNhan.TaiKhoanBenhNhanThus.Where(c => c.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi)
                                                           .Select(c => c.Id).ToArray();
                if (taiKhoanbenhNhanThuIds.Length > 0)
                {
                    TaiKhoanBenhNhanThu taiKhoanBenhNhanThu = null; 
                    List<ChiPhiKhamChuaBenhNoiTruVo> tatCaDichVuKhamChuaBenh = null;
                    var htmlCoBHYT = _thuNganNoiTruService.GetHtmlBangKeCoBHYT(taiKhoanbenhNhanThuIds.Last(), hostingName, ref taiKhoanBenhNhanThu, ref tatCaDichVuKhamChuaBenh);

                    if (!string.IsNullOrEmpty(htmlCoBHYT))
                    {
                        var phieuBHYT = new HtmlToPdfVo()
                        {
                            Html = htmlCoBHYT,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                        };
                        phieuThuNgans.Add(phieuBHYT);
                    }

                    var bangKeKhamBenh = _thuNganNoiTruService.GetHtmlBangKe(taiKhoanbenhNhanThuIds.Last(), hostingName, ref taiKhoanBenhNhanThu, ref tatCaDichVuKhamChuaBenh);
                    if (!string.IsNullOrEmpty(bangKeKhamBenh))
                    {
                        var phieuThuNgan = new HtmlToPdfVo()
                        {
                            Html = bangKeKhamBenh,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                        };
                        phieuThuNgans.Add(phieuThuNgan);
                    }

                    var bkDaThuChuaQuyetToan = _thuNganNoiTruService.GetHtmlBangKeTrongGoiDv(taiKhoanbenhNhanThuIds.Last(), hostingName, ref taiKhoanBenhNhanThu, ref tatCaDichVuKhamChuaBenh);
                    if (!string.IsNullOrEmpty(bkDaThuChuaQuyetToan))
                    {
                        var pbkPhieuQT = new HtmlToPdfVo()
                        {
                            Html = bkDaThuChuaQuyetToan,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                        };
                        phieuThuNgans.Add(pbkPhieuQT);
                    }
                }
                //for (int i = 0; i < taiKhoanbenhNhanThuIds.Length; i++)
                //{
                //    var htmlCoBHYT = _thuNganNoiTruService.GetHtmlBangKeCoBHYT(taiKhoanbenhNhanThuIds[i], hostingName);

                //    if (!string.IsNullOrEmpty(htmlCoBHYT))
                //    {                      
                //        var phieuBHYT = new HtmlToPdfVo()
                //        {
                //            Html = htmlCoBHYT,
                //            FooterHtml = footerHtml,
                //            Bottom = 15,
                //        };
                //        phieuThuNgans.Add(phieuBHYT);
                //    }

                //    var bangKeKhamBenh = _thuNganNoiTruService.GetHtmlBangKe(taiKhoanbenhNhanThuIds[i], hostingName);
                //    if (!string.IsNullOrEmpty(bangKeKhamBenh))
                //    {
                //        var phieuThuNgan = new HtmlToPdfVo()
                //        {
                //            Html = bangKeKhamBenh,
                //            FooterHtml = footerHtml,
                //            Bottom = 15,
                //        };
                //        phieuThuNgans.Add(phieuThuNgan);
                //    }
                                       
                //    var bkDaThuChuaQuyetToan = _thuNganNoiTruService.GetHtmlBangKeTrongGoiDv(taiKhoanbenhNhanThuIds[i], hostingName);
                //    if (!string.IsNullOrEmpty(bkDaThuChuaQuyetToan))
                //    {
                //        var pbkPhieuQT = new HtmlToPdfVo()
                //        {
                //            Html = bkDaThuChuaQuyetToan,
                //            FooterHtml = footerHtml,
                //            Bottom = 15,
                //        };
                //        phieuThuNgans.Add(pbkPhieuQT);
                //    }

                //}
            }
            else
            {
                var htmlCoBHYT = _thuNganNoiTruService.GetHtmlBangKeCoBHYTChuaQuyetToan(yeuCauTiepNhanId, hostingName);
                if (!string.IsNullOrEmpty(htmlCoBHYT))
                {                  
                    var phieuBHYT = new HtmlToPdfVo()
                    {
                        Html = htmlCoBHYT,
                        FooterHtml = footerHtml,
                        Bottom = 15,
                    };
                    phieuThuNgans.Add(phieuBHYT);
                }

                var bangKeKhamBenh = _thuNganNoiTruService.GetHtmlBangKeChuaQuyetToan(yeuCauTiepNhanId, hostingName);
                if (!string.IsNullOrEmpty(bangKeKhamBenh))
                {
                    var phieuThuNgan = new HtmlToPdfVo()
                    {
                        Html = bangKeKhamBenh,
                        FooterHtml = footerHtml,
                        Bottom = 15,
                    };
                    phieuThuNgans.Add(phieuThuNgan);
                }

                var bangKeKhamBenhGoi = _thuNganNoiTruService.GetHtmlBangKeChuaQuyetToanTrongGoiDv(yeuCauTiepNhanId, hostingName);
                if (!string.IsNullOrEmpty(bangKeKhamBenhGoi))
                {
                    var bkPhieuGoi = new HtmlToPdfVo()
                    {
                        Html = bangKeKhamBenhGoi,
                        FooterHtml = footerHtml,
                        Bottom = 15,
                    };
                    phieuThuNgans.Add(bkPhieuGoi);
                }

            }

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(phieuThuNgans);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangKeChiPhi" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("XemBangKeChiPhiTheoKhoa")]
        public ActionResult<List<HtmlToPdfVo>> XemBangKeChiPhiTheoKhoa(long yeuCauTiepNhanId, string hostingName)
        {
            var footerHtml = @"<!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <script charset='utf-8'>

                function replaceParams() {
                  var url = window.location.href
                    .replace(/#$/, '');
                  var params = (url.split('?')[1] || '').split('&');
                  for (var i = 0; i < params.length; i++) {
                      var param = params[i].split('=');
                      var key = param[0];
                      var value = param[1] || '';
                      var regex = new RegExp('{' + key + '}', 'g');
                      document.body.innerText = document.body.innerText.replace(regex, value);
                  }
                }
                </script>
            </head>
            <body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}
            </body>
            </html>";

            var phieuThuNgans = new List<HtmlToPdfVo>();

            var yeuCauTiepNhan = _dieuTriNoiTruService.GetById(yeuCauTiepNhanId, s => s.Include(k => k.NoiTruBenhAn).Include(k => k.TaiKhoanBenhNhanThus));

            if (yeuCauTiepNhan.NoiTruBenhAn.DaQuyetToan == true)
            {
                var taiKhoanbenhNhanThuIds = yeuCauTiepNhan.TaiKhoanBenhNhanThus.Where(c => c.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi)
                                                           .Select(c => c.Id).ToArray();
                for (int i = 0; i < taiKhoanbenhNhanThuIds.Length; i++)
                {
                    var bangKeChiPhis = _thuNganNoiTruService.GetHtmlBangKeTheoKhoaChiDinh(taiKhoanbenhNhanThuIds[i], hostingName);
                    if (bangKeChiPhis.Any())
                    {
                        foreach (var item in bangKeChiPhis)
                        {
                            var phieuThuNgan = new HtmlToPdfVo()
                            {
                                Html = item,
                                FooterHtml = footerHtml,
                                Bottom = 15,
                            };
                            phieuThuNgans.Add(phieuThuNgan);
                        }
                    }
                }

            }
            else
            {               
                var bangKeChiPhis = _thuNganNoiTruService.GetHtmlBangChuaQuyetToanTheoKhoaChiDinh(yeuCauTiepNhanId, hostingName);
                if (bangKeChiPhis.Any())
                {
                    foreach (var item in bangKeChiPhis)
                    {
                        var phieuThuNgan = new HtmlToPdfVo()
                        {
                            Html = item,
                            FooterHtml = footerHtml,
                            Bottom = 15,
                        };
                        phieuThuNgans.Add(phieuThuNgan);
                    }
                }
            }

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(phieuThuNgans);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangKeChiPhiTheoKhoa" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }


        [HttpPost("GetFilePDFFromHtml")]
        public ActionResult GetFilePDFFromHtml(PhieuThuNgan htmlContent)
        {
            var footerHtml = @"<!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <script charset='utf-8'>

                function replaceParams() {
                  var url = window.location.href
                    .replace(/#$/, '');
                  var params = (url.split('?')[1] || '').split('&');
                  for (var i = 0; i < params.length; i++) {
                      var param = params[i].split('=');
                      var key = param[0];
                      var value = param[1] || '';
                      var regex = new RegExp('{' + key + '}', 'g');
                      document.body.innerText = document.body.innerText.replace(regex, value);
                  }
                }
                </script>
            </head>
            <body onload='replaceParams()' style='text-align: right;'>Trang {page}/{topage}
            </body>
            </html>";

            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = htmlContent.Html,
                FooterHtml = footerHtml,
                PageSize = htmlContent.PageSize,
                PageOrientation = htmlContent.PageOrientation,
            };
            var bytes = _pdfService.ExportFilePdfFromHtml(htmlToPdfVo);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");

        }

        #endregion
    }
}
