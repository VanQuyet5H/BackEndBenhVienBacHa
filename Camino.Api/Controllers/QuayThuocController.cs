using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhNhans;
using Camino.Api.Models.General;
using Camino.Api.Models.PhieuInTienQuayThuoc;
using Camino.Api.Models.QuayThuoc;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.BenhNhans;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.QuayThuoc;
using Camino.Services.TiepNhanBenhNhan;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class QuayThuocController : CaminoBaseController
    {
        private IQuayThuocService _quayThuocService;
        private ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        private IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private IQuayThuocLichSuXuatThuocService _quayThuocLichSuXuatThuocService;
        private ITiepNhanBenhNhanService _tiepNhanBenhNhanService;
        private readonly IExcelService _excelService;
        private readonly IPdfService _pdfService;
        private IQuayThuocLichSuHuyBanThuocService _quayThuocLichSuHuyBanThuocService;
        private readonly ILocalizationService _localizationService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;

        public QuayThuocController(IQuayThuocService quayThuocService,
                                   ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
                                   IYeuCauTiepNhanService yeuCauTiepNhanService,
                                   IQuayThuocLichSuXuatThuocService quayThuocLichSuXuatThuocService,
                                   ITiepNhanBenhNhanService tiepNhanBenhNhanService,
                                   IPdfService pdfService,
                                   IExcelService excelService,
                                   ILocalizationService localizationService,
                                   IYeuCauKhamBenhService yeuCauKhamBenhService,
        IQuayThuocLichSuHuyBanThuocService quayThuocLichSuHuyBanThuocService)
        {
            _quayThuocService = quayThuocService;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _quayThuocLichSuXuatThuocService = quayThuocLichSuXuatThuocService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
            _excelService = excelService;
            _pdfService = pdfService;
            _quayThuocLichSuHuyBanThuocService = quayThuocLichSuHuyBanThuocService;
            _localizationService = localizationService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
        }

        [HttpPost("NguoiBenhKhongMuaDonThuoc")]
        public async Task<ActionResult> NguoiBenhKhongMuaDonThuoc(NguoiBenhKhongMuaDonThuocViewModel model)
        {
            await _quayThuocService.NguoiBenhKhongMuaDonThuoc(model.YeuCauTiepNhanId);
            return NoContent();
        }

        #region Cập Nhật Xem trước phiếu thuốc

        [HttpPost("XemTruocBangKeThuoc")]
        public async Task<ActionResult<string>> XemTruocBangKeThuoc([FromBody] XemTruocBangKeThuoc xemTruocBangKeThuoc)
        {
            var list = new List<HtmlToPdfVo>();

            var htmlInBangKe = await _quayThuocService.XemTruocBangKeThuoc(xemTruocBangKeThuoc);
            list.Add(obj(htmlInBangKe, "A4", "Portrait"));

            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=XemTruocBangKeThuoc" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }

        #endregion


        #region Cập nhật 12/04/2021
        [HttpPost("GetSoPhieuQuayThuoc/{yeuCauTiepNhanId}")]
        public async Task<ActionResult<ICollection<ThongTinPhieuThuQuayThuocVo>>> GetSoPhieuNgoaiTru([FromBody]DropDownListRequestModel queryInfo, long yeuCauTiepNhanId)
        {
            var lookup = await _quayThuocService.GetSoPhieu(queryInfo, yeuCauTiepNhanId);
            return Ok(lookup);
        }

        [HttpGet("GetThongTinPhieuThu/{soPhieu}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ThongTinPhieuThu> GetThongTinPhieuThu(long soPhieu)
        {
            var phieuThu = _quayThuocService.GetThongTinPhieuThuQuayThuoc(soPhieu);
            return Ok(phieuThu);
        }

        [HttpPost("HuyPhieuThuThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.QuayThuoc)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult HuyPhieuThuThuoc(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            thongTinHuyPhieuVo.LoaiPhieuThuChiThuNgan = huyPhieuViewModel.LoaiPhieuThuChiThuNgan;
            _quayThuocService.HuyPhieuThu(thongTinHuyPhieuVo);
            return Ok();
        }

        [HttpPost("CapnhatNguoiThuHoiPhieuThuThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.QuayThuoc)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult CapnhatNguoiThuHoiPhieuThuThuoc(ThongTinHuyPhieuViewModel huyPhieuViewModel)
        {
            var thongTinHuyPhieuVo = huyPhieuViewModel.Map<ThongTinHuyPhieuVo>();
            _quayThuocService.CapnhatNguoiThuHoiPhieuThuThuoc(thongTinHuyPhieuVo);

            return Ok();
        }


        #endregion

        [HttpPost("TimKiemBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public ActionResult TimKiemBenhNhan(string search)
        {
            var result = _quayThuocService.FindQuayThuoc(search);
            return Ok(result);
        }
        [HttpPost("GetThongTinBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetThongTinBenhNhanAsync(long benhNhanId, long tiepNhanId)
        {
            var thongTinYeuCauTiepNhan = _yeuCauTiepNhanService.GetById(tiepNhanId, s => s.Include(o => o.BenhNhan).Include(o => o.TinhThanh).Include(o => o.QuanHuyen).Include(o => o.PhuongXa));
            var result = thongTinYeuCauTiepNhan.ToModel<BenhNhanViewModel>();
            if (thongTinYeuCauTiepNhan != null)
            {
                result.MaYeuCauTiepNhan = thongTinYeuCauTiepNhan.MaYeuCauTiepNhan;
            }
            result.SoDienThoai = result.SoDienThoai.ApplyFormatPhone();
            result.GioiTinhHienThi = result.GioiTinh.GetDescription();
            result.DiaChiDayDu = thongTinYeuCauTiepNhan.DiaChiDayDu;
            result.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(tiepNhanId);

            #region BVHD-3941
            if (thongTinYeuCauTiepNhan?.CoBHTN == true)
            {
                result.TenCongTyBaoHiemTuNhan = await _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(thongTinYeuCauTiepNhan?.Id ?? 0);
            }
            #endregion

            return Ok(result);
        }
        [HttpPost("GetThongTinBenhNhan1")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetThongTinBenhNhan1Async(long maBN)
        {
            var result = _quayThuocService.GetThongTinBenhNhan(maBN);
            return Ok(result);
        }
        [HttpPost("GetThongTinBenhNhanTN")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetThongTinBenhNhanTN(long maTN)
        {
            var result = _quayThuocService.GetThongTinBenhNhanTN(maTN);
            return Ok(result);
        }
        [HttpPost("CheckBenhNhanExistBaoHiemTuNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public ActionResult CheckBenhNhanExistBaoHiemTuNhan(long yctnId)
        {
            var thongTin = _quayThuocService.CheckBenhNhanExistBaoHiemTuNhan(yctnId);

            return Ok(thongTin);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridBaoHiemTuNhanAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo, long yCTiepNhanId)
        {
            var gridData = await _quayThuocService.GetDataForGridAsync(queryInfo, yCTiepNhanId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo, long benhNhanId)
        {
            var gridData = await _quayThuocService.GetTotalPageForGridAsync(queryInfo, benhNhanId);
            return Ok(gridData);
        }

        [HttpPost("GetDuocPham")]
        public async Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPham(DropDownListRequestModel queryInfo)
        {
            var listEnum = await _quayThuocService.GetDuocPhamVaVatTuAsync(queryInfo);
            return listEnum;
        }

        [HttpPost("GetDuocPhamDinhMuc")]
        public async Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamDinhMuc(DropDownListRequestModel queryInfo)
        {
            var listEnum = await _quayThuocService.GetDuocPhamVaVatTuDinhMucAsync(queryInfo);
            return listEnum;
        }

        [HttpPost("GetTenBenhNhan")]
        public async Task<string> GetTenBenhNhan(long benhNhanId)
        {
            var listEnum = await _quayThuocService.GetTenBenhNhan(benhNhanId);
            return listEnum;
        }
        [HttpPost("GetTenBacSI")]
        public async Task<string> GetTenBacSI(long Id)
        {
            var name = await _quayThuocService.GetTenBacSI(Id);
            return name;
        }
        [HttpPost("ThuTienKhachVangLai")]
        public async Task<ActionResult<KetQuaThemThanhToanDonThuocVo>> ThuTienKhachVangLai(
            [FromBody] KhachVangLaiChoThanhToanViewModel khachVangLaiChoThanhToan)
        {
            var result = await _quayThuocService.ThanhToanDonThuocKhachVanLai(MapKhachVangLaiChoThanhToanVo(khachVangLaiChoThanhToan));
            return Ok(result);
        }
        private KhachVangLaiThanhToanDonThuocVo MapKhachVangLaiChoThanhToanVo(KhachVangLaiChoThanhToanViewModel khachVangLaiChoThanhToan)
        {
            var khachVangLaiChoThanhToanVo = new KhachVangLaiThanhToanDonThuocVo
            {
                DanhSachDonThuoc = new List<KhachVangLaiThuocChoThanhToanVo>(),
                ThongTinKhach = new KhachVangLaiThongTinHanhChinhVo
                {
                    HoTen = khachVangLaiChoThanhToan.ThongTinKhach.HoTen,
                    BenhNhanId = khachVangLaiChoThanhToan.ThongTinKhach.BenhNhanId ?? 0,
                    SoDienThoai = khachVangLaiChoThanhToan.ThongTinKhach.SoDienThoai,
                    DiaChi = khachVangLaiChoThanhToan.ThongTinKhach.DiaChi,
                    GioiTinh = khachVangLaiChoThanhToan.ThongTinKhach.GioiTinh,
                    NamSinh = khachVangLaiChoThanhToan.ThongTinKhach.NamSinh,
                    PhuongXaId = khachVangLaiChoThanhToan.ThongTinKhach.PhuongXaId,
                    QuanHuyenId = khachVangLaiChoThanhToan.ThongTinKhach.QuanHuyenId,
                    TinhThanhId = khachVangLaiChoThanhToan.ThongTinKhach.TinhThanhId
                },
                ThongTinThuChi = new KhachVangLaiThongTinDonThuocVo
                {
                    POS = khachVangLaiChoThanhToan.ThongTinThuChi.POS ?? 0,
                    TienMat = khachVangLaiChoThanhToan.ThongTinThuChi.TienMat ?? 0,
                    ChuyenKhoan = khachVangLaiChoThanhToan.ThongTinThuChi.ChuyenKhoan ?? 0,
                    SoTienCongNo = khachVangLaiChoThanhToan.ThongTinThuChi.SoTienCongNo ?? 0,

                    NgayThu = khachVangLaiChoThanhToan.ThongTinThuChi.NgayThu,
                    NoiDungThu = khachVangLaiChoThanhToan.ThongTinThuChi.NoiDungThu,
                    GhiChu = khachVangLaiChoThanhToan.ThongTinThuChi.GhiChu
                }
            };

            foreach (var donThuoc in khachVangLaiChoThanhToan.DanhSachDonThuoc)
            {
                khachVangLaiChoThanhToanVo.DanhSachDonThuoc.Add(new KhachVangLaiThuocChoThanhToanVo
                {
                    STT = donThuoc.STT,
                    HanSuDung = donThuoc.HanSuDung,
                    DuocPhamId = donThuoc.DuocPhamId ?? 0,
                    MaHoatChat = donThuoc.MaHoatChat,
                    ViTri = donThuoc.ViTri,
                    DonViTinh = donThuoc.DonViTinh,
                    SoLuongTon = donThuoc.SoLuongTon ?? 0,
                    BacSiKeToa = donThuoc.BacSiKeToa,
                    DonGia = donThuoc.DonGia ?? 0,
                    HanSuDungHientThi = donThuoc.HanSuDungHientThi,
                    NhapKhoDuocPhamChiTietId = donThuoc.NhapKhoDuocPhamChiTietId ?? 0,
                    SoLuongMua = donThuoc.SoLuongMua ?? 0,
                    SoLuongToa = donThuoc.SoLuongToa ?? 0,
                    Solo = donThuoc.Solo,
                    TenDuocPham = donThuoc.TenDuocPham,
                    TenHoatChat = donThuoc.TenHoatChat,
                    ThanhTien = donThuoc.ThanhTien ?? 0,
                    LoaiDuocPhamHoacVatTu = donThuoc.LoaiDuocPhamHoacVatTu
                });
            }
            return khachVangLaiChoThanhToanVo;
        }

        [HttpPost("ThuTienKhachVangLaiVaXuatThuoc")]
        public async Task<ActionResult<KetQuaThemThanhToanDonThuocVo>> ThuTienKhachVangLaiVaXuatThuoc([FromBody] KhachVangLaiChoThanhToanViewModel khachVangLaiChoThanhToan)
        {
            var result = await _quayThuocService.ThanhToanDonThuocKhachVanLai(MapKhachVangLaiChoThanhToanVo(khachVangLaiChoThanhToan), true);
            return Ok(result);
        }

        [HttpPost("GetBenhNhanForNavigation")]
        public async Task<ActionResult<KhachVangLaiNavigateVo>> GetBenhNhanForNavigation(long taiKhoanThuId)
        {
            var benhNhan = await _quayThuocService.GetBenhNhan(taiKhoanThuId);
            return Ok(benhNhan);
        }

        [HttpPost("ThemThanhToanDonThuoc")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<KetQuaThemThanhToanDonThuocVo>> ThemThanhToanDonThuoc(
            [FromBody] ThongTinDonThuocVO thongTinDonThuocViewModel)
        {
            var result = await _quayThuocService.ThanhToanDonThuoc(thongTinDonThuocViewModel);
            return Ok(result);
        }


        [HttpPost("ThuTienVaXuatThuoc")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<KetQuaThemThanhToanDonThuocVo>> ThuTienVaXuatThuoc([FromBody] ThongTinDonThuocVO thongTinDonThuocViewModel)
        {
            var result = await _quayThuocService.ThanhToanDonThuoc(thongTinDonThuocViewModel, true);
            return Ok(result);
        }

        [HttpPost("XuatDonThuocBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuayThuoc, Enums.DocumentType.DanhSachDonThuocChoCapThuocBHYT)]
        public async Task<ActionResult> XuatDonThuocBHYT(
            [FromBody] DanhSachChoXuatThuocVO thongTinDonThuocViewModel)
        {
            var result = await _quayThuocService.XacNhanXuatDonThuoc(thongTinDonThuocViewModel);
            return Ok(result);
        }
        [HttpPost("GetThongTinDuocPham")]
        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetThongTinDuocPham(long duocPhamId, long loaiDuocPhamHoacVatTu)
        {
            var result = await _quayThuocService.GetThongTinDuocPham(duocPhamId, loaiDuocPhamHoacVatTu);
            return result;
        }
        [HttpPost("GetDataDonThuoc")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetDataDonThuoc(long tiepNhanId)
        {
            var result = await _quayThuocService.GetDanhSachThuocChoThanhToan(tiepNhanId);
            return Ok(result);
        }

        [HttpGet("GetListCongTyBaoHiemTuNhans")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetListCongTyBaoHiemTuNhans(long tiepNhanId)
        {
            var result = await _quayThuocService.GetListCongTyBaoHiemTuNhans(tiepNhanId);
            return Ok(result);
        }

        [HttpPost("GetDanhSachXuatThuocBHYT")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetDanhSachXuatThuocBHYT(long tiepNhanId)
        {
            var result = await _quayThuocService.GetDanhSachThuocChoXuatThuocBHYT(tiepNhanId);
            return Ok(result);
        }
        [HttpPost("GetDanhSachXuatThuocKhongBHYT")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetDanhSachXuatThuocKhongBHYT(long tiepNhanId)
        {
            var result = await _quayThuocService.GetDanhSachThuocChoXuatThuocKhongBHYT(tiepNhanId);
            return Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridMuaThuocAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridMuaThuocAsync([FromBody]QueryInfo queryInfo)
        {

            return Ok();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridMuaThuocAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridMuaThuocAsync([FromBody]QueryInfo queryInfo)
        {

            return Ok();
        }
        //
        #region list đơn thuốc trong ngày
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridTimBenhNhanAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public ActionResult<GridDataSource> GetDataForGridTimBenhNhanAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _quayThuocService.GetDataForGridTimBenhNhanAsync(queryInfo, false);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridTimBenhNhanAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public ActionResult<GridDataSource> GetTotalPageForGridTimBenhNhanAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _quayThuocService.GetTotalPageForGridTimBenhNhanAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetDanhSachThuocBenhNhanChild")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachThuocBenhNhanChild([FromBody]QueryInfo queryInfo)
        {
            var result = await _quayThuocService.GetDanhSachThuocBenhNhanChild(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataForGridChildDTTNAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildDTTNAsync([FromBody]QueryInfo queryInfo)
        {
            var result = await _quayThuocService.GetDanhSachThuocBenhNhan(queryInfo);
            return Ok(result);
        }
        [HttpPost("InThuocBenhNhan")]
        public async Task<ActionResult<string>> InThuocBenhNhan([FromBody]XacNhanInThuocBenhNhan xacNhanInThuocBenhNhan)
        {
            var html = string.Empty;
            if (string.IsNullOrEmpty(xacNhanInThuocBenhNhan.LoaiDonThuoc))
                html = _quayThuocService.InPhieuVatTu(xacNhanInThuocBenhNhan);
            else
                html = await _quayThuocService.InPhieuThuTienThuoc(xacNhanInThuocBenhNhan);
            return html;
        }

        #endregion
        ///Toa thuoc cu

        [HttpPost("GetDonthuocChiTietCu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<List<ThongTinDuocPhamQuayThuocCuVo>> GetDonthuocChiTietCu(ListViewModel model)
        {
            var gridData = await _quayThuocService.GetDonThuocChiTietCu(model.ToaThuoc);
            return gridData;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridToaThuocCuAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridToaThuocCuAsync([FromBody]QueryInfo queryInfo, long tiepNhanId)
        {
            var gridData = await _quayThuocService.GetDataForGridToaThuocCuAsync(queryInfo, tiepNhanId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridToaThuocCuAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridMuaThuocAsync([FromBody]QueryInfo queryInfo, long tiepNhanId)
        {
            var gridData = await _quayThuocService.GetTotalPageForGridToaThuocCuAsync(queryInfo, tiepNhanId);
            return Ok(gridData);
        }
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        ///
        [HttpPost("GetHinhThucThanhToanPhiDescription")]
        public List<LookupItemVo> GetHinhThucThanhToanPhiDescription(long benhNhanId)
        {
            var list = Enum.GetValues(typeof(Enums.HinhThucThanhToanPhi)).Cast<Enum>();
            var result = list.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();

            var thongTin = _quayThuocService.CheckBenhNhanExistBaoHiemTuNhan(benhNhanId);
            if (thongTin == null || thongTin.Count == 0)
            {
                result.RemoveAt(3);
            }
            return result;
        }

        [HttpPost("GetHinhThucThanhToanChoKhachVangLai")]
        public List<LookupItemVo> GetHinhThucThanhToanChoKhachVangLai()
        {
            var list = Enum.GetValues(typeof(Enums.HinhThucThanhToanPhi)).Cast<Enum>();
            var result = list.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).ToList();
            return result;
        }

        [HttpPost("XacNhanIn")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<string>> XacNhanIn([FromBody] XacNhanIn xacNhanIn)
        {
            //var idDonThuocBN = xacNhanIn.id; // value default = 0

            var list = new List<HtmlToPdfVo>();
            var hostingName = xacNhanIn.Hosting;
            var tenfile = "";
            if (xacNhanIn.InBangKe && !xacNhanIn.InPhieuThu)
            {
                var htmlInBangKe = await _quayThuocService.InBaoCaoToaThuocAsync(xacNhanIn.TaiKhoanBenhNhanThuId, xacNhanIn.InBangKe, false, hostingName);
                list.Add(obj(htmlInBangKe, "A4", "Portrait"));
                tenfile = "BangKe";
            }
            if (!xacNhanIn.InBangKe && xacNhanIn.InPhieuThu)
            {
                var htmlPhieuThu = await _quayThuocService.InBaoCaoToaThuocAsync(xacNhanIn.TaiKhoanBenhNhanThuId, false, xacNhanIn.InPhieuThu, hostingName);
                list.Add(obj(htmlPhieuThu, "A5", "Landscape"));
                tenfile = "PhieuThu";
            }
            if (xacNhanIn.InBangKe && xacNhanIn.InPhieuThu)
            {
                var htmlInBangKe = await _quayThuocService.InBaoCaoToaThuocAsync(xacNhanIn.TaiKhoanBenhNhanThuId, xacNhanIn.InBangKe, false, hostingName);
                list.Add(obj(htmlInBangKe, "A4", "Portrait"));
                var htmlPhieuThu = await _quayThuocService.InBaoCaoToaThuocAsync(xacNhanIn.TaiKhoanBenhNhanThuId, false, xacNhanIn.InPhieuThu, hostingName);
                list.Add(obj(htmlPhieuThu, "A5", "Landscape"));
                tenfile = "BangKePhieuThu";
            }
            if (!xacNhanIn.InBangKe && !xacNhanIn.InPhieuThu)
            {
                return "true";
            }
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + tenfile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");

        }
        private HtmlToPdfVo obj(string html, string pageSize, string pageOrientation)
        {
            var list = new List<HtmlToPdfVo>();
            PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();
            var footerHtml = @"<!DOCTYPE html>
              <html><head><script>
                function subst() {
                   var vars={};
                    var x=window.location.search.substring(1).split('&');
                    for (var i in x) {var z=x[i].split('=',2);vars[z[0]] = unescape(z[1]);}
                    var x=['frompage','topage','page','webpage','section','subsection','subsubsection'];
                    for (var i in x) {
                        var y = document.getElementsByClassName(x[i]);
                        for (var j=0; j<y.length; ++j) y[j].textContent = vars[x[i]];
                        ";
            footerHtml += @"}}
              </script></head><body style='border:0; margin: 0;' onload='subst()'>
                <table style='width: 100%'>   
                   <tr>";
            if (htmlContent.TenFile == "BangKeVaBangThuTien")
            {
                footerHtml += @"<td><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Ghi ch&#250;: Ph&#226;n lo&#7841;i kh&#225;m s&#7913;c kh&#7887;e: Lo&#7841;i 1: R&#7845;t kh&#7887;e; Lo&#7841;i 2: Kh&#7887;e; Lo&#7841;i 3: Trung b&#236;nh; Lo&#7841;i 4: Y&#7871;u; Lo&#7841;i 5: R&#7845;t y&#7871;u.</b></td>";
            }
            else
            {
                footerHtml += @"<td class='section'></td>";
            }
            footerHtml += @"<td style='text-align:right'>
                    Trang <span class='page'></span>/<span class='topage'></span>
                  </td>
                </tr>
              </table>
              </body></html>";
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = html,
                FooterHtml = footerHtml,
                PageSize = pageSize,
                PageOrientation = pageOrientation,
            };
            return htmlToPdfVo;
        }
        [HttpPost("XacNhanInThuocCoBhyt")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<string>> XacNhanInThuocCoBhyt(
            [FromBody]XacNhanInThuocBhyt xacNhanIn)
        {
            var htmlBangThuTienThuoc = await _quayThuocService.XacNhanInThuocCoBhyt(xacNhanIn);
            var list = new List<HtmlToPdfVo>();
            PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();
            var footerHtml = @"<!DOCTYPE html>
              <html><head><script>
                function subst() {
                   var vars={};
                    var x=window.location.search.substring(1).split('&');
                    for (var i in x) {var z=x[i].split('=',2);vars[z[0]] = unescape(z[1]);}
                    var x=['frompage','topage','page','webpage','section','subsection','subsubsection'];
                    for (var i in x) {
                        var y = document.getElementsByClassName(x[i]);
                        for (var j=0; j<y.length; ++j) y[j].textContent = vars[x[i]];
                        ";
            footerHtml += @"}}
              </script></head><body style='border:0; margin: 0;' onload='subst()'>
                <table style='width: 100%'>   
                   <tr>";
            if (htmlContent.TenFile == "BangKeVaBangThuTien")
            {
                footerHtml += @"<td><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Ghi ch&#250;: Ph&#226;n lo&#7841;i kh&#225;m s&#7913;c kh&#7887;e: Lo&#7841;i 1: R&#7845;t kh&#7887;e; Lo&#7841;i 2: Kh&#7887;e; Lo&#7841;i 3: Trung b&#236;nh; Lo&#7841;i 4: Y&#7871;u; Lo&#7841;i 5: R&#7845;t y&#7871;u.</b></td>";
            }
            else
            {
                footerHtml += @"<td class='section'></td>";
            }
            footerHtml += @"<td style='text-align:right'>
                    Trang <span class='page'></span>/<span class='topage'></span>
                  </td>
                </tr>
              </table>
              </body></html>";
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = htmlBangThuTienThuoc,
                FooterHtml = footerHtml,
                PageSize = "A4",
                PageOrientation = "Landscape",
            };
            list.Add(htmlToPdfVo);
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + "ThuTienThuocBaoHiemYTe" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }


        [HttpPost("CheckValidationForQuayThuocKhachVangLai")]
        public ActionResult CheckValidationForQuayThuocKhachVangLai([FromBody]KhachVangLaiViewModel validationCheck)
        {
            return Ok();
        }

        [HttpPost("CheckValidationForQuayThuoc")]
        public ActionResult CheckValidationForQuayThuoc([FromBody]ThuTienMuaDuocPhamViewModel validationCheck)
        {
            return Ok();
        }

        [HttpPost("GetDanhSachChoXuatThuocKhachVangLai")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetDanhSachChoXuatThuocKhachVangLai(long benhNhanId)
        {
            var result = await _quayThuocService.GetDanhSachChoXuatThuocKhachVangLai(benhNhanId);
            return Ok(result);
        }

        /// tỉnh quận phường
        [HttpPost("GetTinhThanh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetTinhThanh(DropDownListRequestModel model)
        {
            var lookup = await _quayThuocService.GetTinhThanh(model);
            return Ok(lookup);
        }

        [HttpPost("GetQuanHuyen")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetQuanHuyen(DropDownListRequestModel model)
        {
            var lookup = await _quayThuocService.GetQuanHuyen(model);
            return Ok(lookup);
        }

        [HttpPost("GetPhuongXa")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetPhuongXa(DropDownListRequestModel model)
        {
            var lookup = await _quayThuocService.GetPhuongXa(model);
            return Ok(lookup);
        }
        [HttpPost("GetTinhHuyen")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> GetTinhHuyen(long phuongXaId)
        {
            var lookup = await _quayThuocService.GetTinhHuyen(phuongXaId);
            return Ok(lookup);
        }
        // excel export
        [HttpPost("ExportDonThuocTrongNgay")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.QuayThuoc)]
        public async Task<ActionResult> ExportDonThuocTrongNgay(QueryInfo queryInfo)
        {
            var gridData = _quayThuocService.GetDataForGridTimBenhNhanAsync(queryInfo, true);
            var donThuocTrongNgay = gridData.Data.Select(p => (DonThuocThanhToanGridVo)p).ToList();
            queryInfo.Sort[0].Dir = "asc";
            queryInfo.Sort[0].Field = "Id";
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }
            var datas = gridData.Data.Cast<DonThuocThanhToanGridVo>().ToList();
            foreach (var data in datas)
            {
                if (data.DateStart != null || data.DateEnd != null)
                {
                    queryInfo.AdditionalSearchString = data.Id.ToString() + '-' + data.DateStart + '-' + data.DateEnd;
                }
                else
                {
                    queryInfo.AdditionalSearchString = data.Id.ToString();
                }

                var gridDataChildNhaThuoc = await _quayThuocService.GetDanhSachThuocBenhNhanChild(queryInfo);
                data.ListChilDonThuocTrongNgay = gridDataChildNhaThuoc.Data.Cast<DonThuocCuaBenhNhanGridVo>().ToList();
            }
            var bytes = _quayThuocService.ExportDanhSachDonThuocTrongNgay(datas);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DanhSachDonThuocTrongNgay" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #region lịch sử xuất thuốc 12/8/2020

        #region  DANH SÁCH LỊCH SỬ XUẤT THUỐC 07/09/2020

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachLichSuXuatThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachLichSuXuatThuoc([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuXuatThuocService.GetDanhSachLichSuXuatThuoc(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachLichSuXuatThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachLichSuXuatThuoc([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuXuatThuocService.GetTotalDanhSachLichSuXuatThuoc(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDanhSachLichSuXuatThuocVatTuBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc, Enums.DocumentType.LichSuXuatThuocCapThuocBHYT)]
        public ActionResult GetDanhSachLichSuXuatThuocVatTuBHYT(long phieuXuatId, LoaiDuocPhamVatTu loaiDuocPhamVatTu)
        {
            var result = _quayThuocLichSuXuatThuocService.GetDanhSachLichSuXuatThuocVatTuBHYT(phieuXuatId, loaiDuocPhamVatTu);
            return Ok(result);
        }

        [HttpPost("GetDanhSachLichSuXuatThuocVatTukhongBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc, Enums.DocumentType.LichSuXuatThuocCapThuocBHYT)]
        public ActionResult GetDanhSachLichSuXuatThuocVatTukhongBHYT(long phieuXuatId, LoaiDuocPhamVatTu loaiDuocPhamVatTu)
        {
            var result = _quayThuocLichSuXuatThuocService.GetDanhSachLichSuXuatThuocVatTukhongBHYT(phieuXuatId, loaiDuocPhamVatTu);
            return Ok(result);
        }

        [HttpPost("XacNhanInThuocVatTuCoBhytXuatThuoc")]
        public async Task<ActionResult<string>> XacNhanInThuocVatTuCoBhytXuatThuoc([FromBody]XacNhanInThuocVatTu xacNhanIn)
        {
            var htmlBangThuTienThuoc = await _quayThuocLichSuXuatThuocService.XacNhanInThuocVatTuCoBhytXuatThuoc(xacNhanIn, true);
            var list = new List<HtmlToPdfVo>();

            PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();
            var footerHtml = @"<!DOCTYPE html>
              <html><head><script>
                function subst() {
                   var vars={};
                    var x=window.location.search.substring(1).split('&');
                    for (var i in x) {var z=x[i].split('=',2);vars[z[0]] = unescape(z[1]);}
                    var x=['frompage','topage','page','webpage','section','subsection','subsubsection'];
                    for (var i in x) {
                        var y = document.getElementsByClassName(x[i]);
                        for (var j=0; j<y.length; ++j) y[j].textContent = vars[x[i]];
                        ";
            footerHtml += @"}}
              </script></head><body style='border:0; margin: 0;' onload='subst()'>
                <table style='width: 100%'>   
                   <tr>";
            if (htmlContent.TenFile == "BangKeVaBangThuTien")
            {
                footerHtml += @"<td><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Ghi ch&#250;: Ph&#226;n lo&#7841;i kh&#225;m s&#7913;c kh&#7887;e: Lo&#7841;i 1: R&#7845;t kh&#7887;e; Lo&#7841;i 2: Kh&#7887;e; Lo&#7841;i 3: Trung b&#236;nh; Lo&#7841;i 4: Y&#7871;u; Lo&#7841;i 5: R&#7845;t y&#7871;u.</b></td>";
            }
            else
            {
                footerHtml += @"<td class='section'></td>";
            }
            footerHtml += @"<td style='text-align:right'>
                    Trang <span class='page'></span>/<span class='topage'></span>
                  </td>
                </tr>
              </table>
              </body></html>";
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = htmlBangThuTienThuoc,
                FooterHtml = footerHtml,
                PageSize = "A4",
                PageOrientation = "Landscape",
            };
            list.Add(htmlToPdfVo);
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + "ThuTienThuocBaoHiemYTe" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }

        [HttpPost("XacNhanInThuocVatTuKhongBhytXuatThuoc")]
        public async Task<ActionResult<string>> XacNhanInThuocVatTuKhongBhytXuatThuoc([FromBody]XacNhanInThuocVatTu xacNhanIn)
        {
            var htmlBangThuTienThuoc = await _quayThuocLichSuXuatThuocService.XacNhanInThuocVatTuCoBhytXuatThuoc(xacNhanIn, false);
            var list = new List<HtmlToPdfVo>();

            PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();
            var footerHtml = @"<!DOCTYPE html>
              <html><head><script>
                function subst() {
                   var vars={};
                    var x=window.location.search.substring(1).split('&');
                    for (var i in x) {var z=x[i].split('=',2);vars[z[0]] = unescape(z[1]);}
                    var x=['frompage','topage','page','webpage','section','subsection','subsubsection'];
                    for (var i in x) {
                        var y = document.getElementsByClassName(x[i]);
                        for (var j=0; j<y.length; ++j) y[j].textContent = vars[x[i]];
                        ";
            footerHtml += @"}}
              </script></head><body style='border:0; margin: 0;' onload='subst()'>
                <table style='width: 100%'>   
                   <tr>";
            if (htmlContent.TenFile == "BangKeVaBangThuTien")
            {
                footerHtml += @"<td><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Ghi ch&#250;: Ph&#226;n lo&#7841;i kh&#225;m s&#7913;c kh&#7887;e: Lo&#7841;i 1: R&#7845;t kh&#7887;e; Lo&#7841;i 2: Kh&#7887;e; Lo&#7841;i 3: Trung b&#236;nh; Lo&#7841;i 4: Y&#7871;u; Lo&#7841;i 5: R&#7845;t y&#7871;u.</b></td>";
            }
            else
            {
                footerHtml += @"<td class='section'></td>";
            }
            footerHtml += @"<td style='text-align:right'>
                    Trang <span class='page'></span>/<span class='topage'></span>
                  </td>
                </tr>
              </table>
              </body></html>";
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = htmlBangThuTienThuoc,
                FooterHtml = footerHtml,
                PageSize = "A4",
                PageOrientation = "Landscape",
            };
            list.Add(htmlToPdfVo);
            var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + "ThuTienThuocBaoHiemYTe" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }


        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridLichSuXuatThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridLichSuXuatThuoc([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuXuatThuocService.GetDanhSachLichSuXuatThuoc(queryInfo, false);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridLichSuXuatThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuQuayThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridLichSuXuatThuoc([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _quayThuocLichSuXuatThuocService.GetTotalDanhSachLichSuXuatThuoc(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetFilePDFFromHtml")]
        public ActionResult GetFilePDFFromHtml(SettingPhieuThu htmlContent)
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

        [HttpPost("CheckLoaiYCTN")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.QuayThuoc)]
        public async Task<bool> CheckLoaiYCTN(long tiepNhanId)
        {
            var lookup = _quayThuocService.LoaiYCTN(tiepNhanId);
            return lookup;
        }


    }
}