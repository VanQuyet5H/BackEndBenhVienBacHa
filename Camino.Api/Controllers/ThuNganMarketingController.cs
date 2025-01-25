using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Api.Models.ThongTinMienGiamThuNgan;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhMucMarketing;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.BenhNhans;
using Camino.Services.BenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoaPhong;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public class ThuNganMarketingController : CaminoBaseController
    {
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IThuNganMarketingService _thuNganMarketingService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        private readonly IBenhVienService _benhVienService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IExcelService _excelService;
        private readonly IBenhNhanService _benhnhanService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;

        public ThuNganMarketingController(IYeuCauTiepNhanService yeuCauTiepNhanService,
                                IThuNganMarketingService thuNganMarketingService,
                                ITaiLieuDinhKemService taiLieuDinhKemService,
                                ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
                                IBenhVienService benhVienService,
                                IKhoaPhongService khoaPhongService,
                                IBenhNhanService benhnhanService,
                                IExcelService excelService,
                                ITiepNhanBenhNhanService tiepNhanBenhNhanService)
        {
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _thuNganMarketingService = thuNganMarketingService;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _khoaPhongService = khoaPhongService;
            _benhVienService = benhVienService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _excelService = excelService;
            _benhnhanService = benhnhanService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
        }

        #region Chuyển Gói Marketing       

        [HttpPost("GetListGoiMarketingChuyenGoi")]
        public ActionResult<ICollection<LookupItemTemplateVo>> GetListGoiMarketingChuyenGoi([FromBody]DropDownListRequestModel model)
        {
            var yeuCauGoiDichVuId = JsonConvert.DeserializeObject<GoiMarketingJson>(model.ParameterDependencies);
            var lookup = _thuNganMarketingService.GetListGoiMarketingChuyenGoi(model, yeuCauGoiDichVuId.YeuCauGoiDichVuId);
            return Ok(lookup);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinGoiDichVuMoiMuonChuyen/{chonGoiMarketing}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public List<DichVuTrongGoiMarketingModel> GetDataGoiDichVuChuaChuyen(long chonGoiMarketing)
        {
            return _thuNganMarketingService.GetThongTinGoiDichVuMoiMuonChuyen(chonGoiMarketing);
        }

        [HttpGet("GetThongTinGoiDichVuCuaBenhNhan/{yeuCauGoiDichVuId}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public ActionResult<List<DichVuTrongGoiMarketingModel>> GetThongTinDichVuTrongGoiBenhNhan(long yeuCauGoiDichVuId)
        {
            return Ok(_thuNganMarketingService.GetThongTinGoiDichVuCuaBenhNhan(yeuCauGoiDichVuId));
        }

        [HttpPost("LuuThongTinMuonChuyenGoiMoi")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult> LuuThongTinMuonChuyenGoiMoi(LuuThongTinChuyenGoiMoi thongTinChuyenGois)
        {
            await _thuNganMarketingService.LuuThongTinMuonChuyenGoiMoi(thongTinChuyenGois);
            return Ok();
        }

        #endregion

        #region Danh sách marketing

        [HttpPost("GetDataChuaQuyetToanMarketingForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataChuaQuyetToanMarketingForGridAsync([FromBody] ThuNganMarketingQueryInfo queryInfo)
        {
            var gridData = await _thuNganMarketingService.GetDanhSachChuaQuyetToanAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalChuaQuyetToanMarketingPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetTotalChuaQuyetToanMarketingPageForGridAsync([FromBody] ThuNganMarketingQueryInfo queryInfo)
        {
            var gridData = await _thuNganMarketingService.GetTotalPageDanhSachChuaQuyetToanAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("GetDataDaQuyetToanMarketingForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataDaQuyetToanMarketingForGridAsync([FromBody] ThuNganMarketingQueryInfo queryInfo)
        {
            var gridData = await _thuNganMarketingService.GetDanhSachDaQuyetToanAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalDaQuyetToanMarketingPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetTotalDaQuyetToanMarketingPageForGridAsync([FromBody] ThuNganMarketingQueryInfo queryInfo)
        {
            var gridData = await _thuNganMarketingService.GetTotalPageDanhSachDaQuyetToanAsync(queryInfo);
            return Ok(gridData);
        }


        #region GÓI DỊCH VỤ QUYẾT TOÁN

        [HttpGet("GetThongTinBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<ThongTinThuNganBenhNhanViewModel>> GetThongTinBenhNhan(long benhNhanId)
        {
            var yeuCauTiepNhan = _thuNganMarketingService.YeuCauTiepNhanCuoiBenhNhan(benhNhanId);
            if (yeuCauTiepNhan == null)
            {
                var benhNhan = _benhnhanService.GetById(benhNhanId);
                var thongTinBenhNhan = benhNhan.ToModel<ThongTinThuNganBenhNhanViewModel>();

                return Ok(thongTinBenhNhan);
            }
            else
            {
                var thongTinBenhNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauTiepNhan.Id, s => s.Include(cc => cc.CongTyUuDai)
                                                                                                   .Include(o => o.QuanHuyen)
                                                                                                   .Include(o => o.TinhThanh)
                                                                                                   .Include(o => o.PhuongXa)
                                                                                                   .Include(o => o.BenhNhan)
                                                                                                   .Include(o => o.DoiTuongUuDai)
                                                                                                   .Include(o => o.YeuCauTiepNhanTheBHYTs).ThenInclude(cc => cc.GiayMienCungChiTra)
                                                                                                   .Include(cc => cc.GiayChuyenVien)
                                                                                                   .Include(cc => cc.GiayMienGiamThem)
                                                                                                   .Include(cc => cc.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                                                                                                   .ThenInclude(cc => cc.CongTyBaoHiemTuNhan)
                                                                                                   .Include(cc => cc.BHYTGiayMienCungChiTra));

                var benhVien = await _benhVienService.GetBenhVienWithMaBenhVien(thongTinBenhNhan.BHYTMaDKBD);
                var model = thongTinBenhNhan.ToModel<ThongTinThuNganBenhNhanViewModel>();

                model.MaBN = thongTinBenhNhan.BenhNhan.MaBN;
                model.SoDienThoai = model.SoDienThoai.ApplyFormatPhone();
                model.BHYTDiaChi = benhVien != null ? benhVien.Ten : string.Empty;
                model.NgayThangNamSinh = DateHelper.DOBFormat(yeuCauTiepNhan?.NgaySinh, yeuCauTiepNhan?.ThangSinh, yeuCauTiepNhan?.NamSinh);

                if (thongTinBenhNhan.GiayChuyenVien != null)
                {
                    model.TaiLieuDinhKemGiayChuyenVien = new TaiLieuDinhKemGiayChuyenVien()
                    {
                        Ten = thongTinBenhNhan.GiayChuyenVien.Ten,
                        DuongDan = thongTinBenhNhan.GiayChuyenVien.DuongDan,
                        TenGuid = thongTinBenhNhan.GiayChuyenVien.TenGuid,
                        KichThuoc = thongTinBenhNhan.GiayChuyenVien.KichThuoc,
                        LoaiTapTin = (int)thongTinBenhNhan.GiayChuyenVien.LoaiTapTin,
                    };
                }

                if (thongTinBenhNhan.BHYTGiayMienCungChiTra != null)
                {
                    model.TaiLieuDinhKemGiayMiemCungChiTra = new TaiLieuDinhKemGiayMiemCungChiTra()
                    {
                        Ten = thongTinBenhNhan.BHYTGiayMienCungChiTra.Ten,
                        DuongDan = thongTinBenhNhan.BHYTGiayMienCungChiTra.DuongDan,
                        TenGuid = thongTinBenhNhan.BHYTGiayMienCungChiTra.TenGuid,
                        KichThuoc = thongTinBenhNhan.BHYTGiayMienCungChiTra.KichThuoc,
                        LoaiTapTin = (int)thongTinBenhNhan.BHYTGiayMienCungChiTra.LoaiTapTin,
                    };
                }

                if (thongTinBenhNhan.GiayMienGiamThem != null)
                {
                    model.TaiLieuDinhKemGiayMiemGiam = new TaiLieuDinhKemGiayMiemGiam
                    {
                        Ten = thongTinBenhNhan.GiayMienGiamThem.Ten,
                        Id = thongTinBenhNhan.GiayMienGiamThem.Id,
                        Ma = thongTinBenhNhan.GiayMienGiamThem.Ma,
                        DuongDan = thongTinBenhNhan.GiayMienGiamThem.DuongDan,
                        TenGuid = thongTinBenhNhan.GiayMienGiamThem.TenGuid,
                        KichThuoc = thongTinBenhNhan.GiayMienGiamThem.KichThuoc,
                        LoaiTapTin = (int)thongTinBenhNhan.GiayMienGiamThem.LoaiTapTin
                    };
                }

                model.DiaChi = thongTinBenhNhan.DiaChiDayDu;

                #region BVHD-3941
                model.CoYCTN = thongTinBenhNhan.Id != 0;
                if (thongTinBenhNhan.CoBHTN == true)
                {
                    model.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(thongTinBenhNhan.Id);
                }
                #endregion

                return Ok(model);
            }
        }

        [HttpPost("GetGoiDichVuTheoBenhNhan")]
        public ActionResult<List<LookupItemVo>> GetGoiDichVuTheoBenhNhan(long benhNhanId)
        {
            var benhNhan = _thuNganMarketingService.GetById(benhNhanId, c => c.Include(cc => cc.YeuCauGoiDichVus)
                                                         .Include(o => o.TaiKhoanBenhNhan).ThenInclude(cc => cc.TaiKhoanBenhNhanThus));
            var yeuCauGoiDichVus = benhNhan.YeuCauGoiDichVus.Select(c => new LookupItemVo { KeyId = c.Id, DisplayName = c.TenGoiDichVu }).ToList();
            return Ok(yeuCauGoiDichVus);
        }

        [HttpGet("GetDataGoiChuaThuQuyetToan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GoiChuaQuyetToanMarketingGridVo>> GetDataGoiChuaThuQuyetToan(long benhNhanId)
        {
            var gridData = await _thuNganMarketingService.GetGoiChuaQuyetToanTheoBenhNhanAsync(benhNhanId);
            return Ok(gridData);
        }

        [HttpGet("GetDataGoiDaThuQuyetToan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GoiDaQuyetToanMarketingGridVo>> GetDataGoiDaThuQuyetToan(long benhNhanId)
        {
            var gridData = await _thuNganMarketingService.GetGoiDaQuyetToanTheoBenhNhanAsync(benhNhanId);
            return Ok(gridData);
        }

        [HttpGet("GetThongTinDichVuTrongGoi/{yeuCauGoiDichVuId}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<ThongTinYeuCauGoiDichVu>> GetThongTinDichVuTrongGoi(long yeuCauGoiDichVuId)
        {
            return Ok(await _thuNganMarketingService.GetThongTinGoiDichVu(yeuCauGoiDichVuId));
        }

        [HttpPut("ThuTienBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult> ThuTienBenhNhan([FromBody] ThongTinThuTienGoi thongTinThuTienGoi)
        {
            var result = await _thuNganMarketingService.ThuTienGoiDichVu(thongTinThuTienGoi);
            if (string.IsNullOrEmpty(result.Item2)) return Ok(result.Item1);
            throw new ApiException(result.Item2);
        }

        [HttpPut("QuyetToanDichVuTrongGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<long?>> QuyetToanDichVuTrongGoi([FromBody] ThongTinYeuCauGoiDichVu thongTinQuyetToan)
        {
            return Ok(await _thuNganMarketingService.QuyetToanGoiDichVu(thongTinQuyetToan));
        }

        [HttpPut("HuyQuyetToanDichVuTrongGoi")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult> HuyQuyetToanDichVuTrongGoi([FromBody] HuyQuyetToanGoi thongTinQuyetToan)
        {
            await _thuNganMarketingService.HuyQuyetToanGoiDichVu(thongTinQuyetToan);
            return Ok();
        }

        [HttpGet("ThongTinPhieuThuMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult> ThongTinPhieuThuMarketing(long taiKhoanThuId, LoaiPhieuThuChiThuNgan loaiPhieuThuChiThuNgan)
        {
            return Ok(await _thuNganMarketingService.GetThongTinPhieuThu(taiKhoanThuId, loaiPhieuThuChiThuNgan));
        }

        [HttpPost("GetHtmlPhieuThuTamUngMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public ActionResult<List<PhieuThuNgan>> GetHtmlPhieuThuTamUngMarketing(long taiKhoanThuId, string hostingName)
        {
            var phieuThuNgans = new List<PhieuThuNgan>()
            {
              new PhieuThuNgan {
                    Html = _thuNganMarketingService.GetHtmlPhieuThuTamUngMarketing(taiKhoanThuId, hostingName),
                    TenFile = "PhieuThuTamUngMarketing",
                    PageOrientation = PageOrientationLandscape,
                    PageSize = PageSizeA5
              }
            };
            return Ok(phieuThuNgans);
        }

        [HttpPost("GetHtmlPhieuThuHoanUngMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public ActionResult<List<PhieuThuNgan>> GetHtmlPhieuThuHoanUngMarketing(long taiKhoanChiId, string hostingName)
        {
            var phieuThuNgans = new List<PhieuThuNgan>()
            {
              new PhieuThuNgan {
                    Html = _yeuCauTiepNhanService.GetHtmlPhieuHoanUngNgoaiTru(taiKhoanChiId, hostingName),
                    TenFile = "PhieuThuHoanUngMarketing",
                    PageOrientation = PageOrientationLandscape,
                    PageSize = PageSizeA5
              }
            };
            return Ok(phieuThuNgans);
        }

        [HttpPost("GetHtmlPhieuThuMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public ActionResult<List<PhieuThuNgan>> GetHtmlPhieuThuMarketing(long taiKhoanChiId, string hostingName)
        {
            var phieuThuNgans = new List<PhieuThuNgan>()
            {
              new PhieuThuNgan {
                    Html = _thuNganMarketingService.GetHtmlPhieuThuMarketing(taiKhoanChiId, hostingName),
                    TenFile = "PhieuThuMarketing",
                    PageOrientation = PageOrientationLandscape,
                    PageSize = PageSizeA5
              }
            };
            return Ok(phieuThuNgans);
        }

        [HttpPost("InBangKeSuDung")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public ActionResult<string> InBangKeSuDung(long goiDichVuId, string hostingName)
        {
            var bangKeSuDung = new List<PhieuThuNgan>()
            {
              new PhieuThuNgan {
                    Html = _thuNganMarketingService.GetHtmBangKeSuDung(goiDichVuId, hostingName),
                    TenFile = "BangKeSuDung",
                    PageOrientation = PageOrientationPortrait,
                    PageSize = PageSizeA5
              }
            };
            return Ok(bangKeSuDung);
        }

        [HttpPost("GetDataThongTinPhieuThuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.ThuNgan)]
        public async Task<ActionResult<GridDataSource>> GetDataThongTinPhieuThuForGrid([FromBody] ThongTinPhieuMarketingQueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryInfo = JsonConvert.DeserializeObject<ThongTinPhieuMarketingQueryInfo>(queryInfo.AdditionalSearchString);
            }

            if (!string.IsNullOrEmpty(queryInfo.FromDate) || !string.IsNullOrEmpty(queryInfo.ToDate))
            {
                DateTime denNgay;
                queryInfo.FromDate.TryParseExactCustom(out var tuNgay);
                if (string.IsNullOrEmpty(queryInfo.ToDate))
                {
                    denNgay = DateTime.Now;
                }
                else
                {
                    queryInfo.ToDate.TryParseExactCustom(out denNgay);
                }
                denNgay = denNgay.AddSeconds(59).AddMilliseconds(999);

                queryInfo.ThoiDiemTuFormat = tuNgay;
                queryInfo.ThoiDiemDenFormat = denNgay;
            }

            var gridData = await _thuNganMarketingService.GetDanhSachPhieuThuMarketingAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("HuyPhieuThuMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> HuyPhieuThuMarketing(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            thongTinHuyPhieuVo.LoaiPhieuThuChiThuNgan = huyPhieuViewModel.LoaiPhieuThuChiThuNgan;
            await _thuNganMarketingService.HuyPhieuThuGoiDichVu(thongTinHuyPhieuVo);
            return Ok();
        }

        [HttpPost("CapnhatNguoiThuHoiPhieuThuMarketing")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.ThuNgan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult CapnhatNguoiThuHoiPhieuThuMarketing(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            _yeuCauTiepNhanService.CapnhatNguoiThuHoiPhieuThu(thongTinHuyPhieuVo);
            return Ok();
        }

        #endregion

        private readonly string PageSizeA5 = "A5";
        private readonly string PageOrientationLandscape = "Landscape";
        private readonly string PageOrientationPortrait = "Portrait";

        #endregion

    }
}