using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.BaoCao;
using Camino.Api.Models.BaoCaoLuuKetQuaXetNghiemHangNgay;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoBienBanKiemKeDPVT;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDoanhThuKhamDoanTheoNhomDichVu;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDuTruSoLuongNguoiThucHienDvLSCLS;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoKetQuaKhamChuaBenh;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoThongKeDonThuoc;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoTongHopKetQuaKhamDoan;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoVienPhiThuTien;
using Camino.Core.Domain.ValueObject.BaoCaoLuuketQuaXeNghiemTrongNgay;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.DSDichVuNgoaiGoiKeToan;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Services.BaoCao;
using Camino.Services.BaoCaoKhamDoanHopDong;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.PhongBenhVien;
using Camino.Services.YeuCauKhamBenh;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class BaoCaoController : CaminoBaseController
    {
        private readonly IBaoCaoService _baoCaoService;
        private readonly IBaoCaoXetNghiemService _baoCaoXetNghiemService;
        private readonly IBaoCaoKhamDoanHopDongServices _baoCaoKhamDoanHopDongService;
        private readonly IExcelService _excelService;
        private readonly IPhongBenhVienService _phongBenhVienService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        private readonly IPdfService _pdfService;
        private readonly IBaoCaoKhamDoanTheoGiaThucTeServices _baoCaoKhamDoanTheoGiaThucTeServices;

        public BaoCaoController(
            IExcelService excelService,
            IBaoCaoService baoCaoService,
            IBaoCaoXetNghiemService baoCaoXetNghiemService,
            IPhongBenhVienService phongBenhVienService,
            IYeuCauKhamBenhService yeuCauKhamBenhService,
            IPdfService pdfService,
            IBaoCaoKhamDoanHopDongServices baoCaoKhamDoanHopDongService,
            IBaoCaoKhamDoanTheoGiaThucTeServices baoCaoKhamDoanTheoGiaThucTeServices
        )
        {
            _baoCaoService = baoCaoService;
            _excelService = excelService;
            _phongBenhVienService = phongBenhVienService;
            _baoCaoXetNghiemService = baoCaoXetNghiemService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _pdfService = pdfService;
            _baoCaoKhamDoanHopDongService = baoCaoKhamDoanHopDongService;
            _baoCaoKhamDoanTheoGiaThucTeServices = baoCaoKhamDoanTheoGiaThucTeServices;
        }
        #region GetDataForGridAsyncBaoCaoTongHopDoanhThuKhoaPhong
        // to do nam ho
        [HttpPost("GetBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTongHopDoanhThuTheoKhoaPhong)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync([FromBody] BaoCaoTongHopDoanhThuTheoKhoaPhongQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetTotalBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTongHopDoanhThuTheoKhoaPhong)]
        public async Task<ActionResult<GridItem>> GetTotalBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync([FromBody] BaoCaoTongHopDoanhThuTheoKhoaPhongQueryInfo queryInfo)
        {
            var data = await _baoCaoService.GetTotalBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync(queryInfo);
            return Ok(data);
        }
        #endregion

        #region GetDataForGridAsyncBaoCaoChiTietDoanhThuKhoaPhong

        // to do nam ho
        //BaoCaoChiTietDoanhThuTheoKhoaPhongMasterGridVo
        [HttpPost("GetBaoCaoChiTietDoanhThuTheoKhoaPhongForMasterGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoChiTietDoanhThuTheoKhoaPhong)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoChiTietDoanhThuTheoKhoaPhongForMasterGridAsync([FromBody] BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetBaoCaoChiTietDoanhThuTheoKhoaPhongForMasterGridAsync(queryInfo);
            return Ok(gridData);
        }
        //BaoCaoChiTietDoanhThuTheoKhoaPhongDetailGridVo
        [HttpPost("GetBaoCaoChiTietDoanhThuTheoKhoaPhongForDetailGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoChiTietDoanhThuTheoKhoaPhong)]
        public async Task<ActionResult<GridItem>> GetBaoCaoChiTietDoanhThuTheoKhoaPhongForDetailGridAsync(int khoaPhongId, string tuNgay, string denNgay, string KySoSanhTuNgay, string kySoSanhDenNgay)
        {
            DateTime TuNgayPart = DateTime.Now;
            DateTime DenNgaysPart = DateTime.Now;
            DateTime.TryParseExact(tuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
            DateTime.TryParseExact(denNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
            DateTime TuNgayPartKy = DateTime.Now;
            DateTime DenNgaysPartKy = DateTime.Now;
            DateTime.TryParseExact(KySoSanhTuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPartKy);
            DateTime.TryParseExact(kySoSanhDenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPartKy);
            var dateTimeFilterVo = new DateTimeFilterVo();
            var queryInfo = new BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo
            {
                LayTatCa = true,
                TuNgay = TuNgayPart,
                DenNgay = DenNgaysPart,
                KySoSanhTuNgay = TuNgayPartKy,
                KySoSanhDenNgay = DenNgaysPartKy,
                KhoaPhongId = khoaPhongId
            };
            var data = await _baoCaoService.GetBaoCaoChiTietDoanhThuTheoKhoaPhongForDetailGridAsync(queryInfo);
            return Ok(data);
        }
        #endregion

        #region GetDataForGridAsyncBaoCaoTongHopDoanhThuBacSi
        [HttpPost("GetBaoCaoTongHopDoanhThuTheoBacSiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTongHopDoanhThuTheoBacSi)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoTongHopDoanhThuTheoBacSiForGridAsync([FromBody] BaoCaoTongHopDoanhThuTheoBacSiQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetBaoCaoTongHopDoanhThuTheoBacSiForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalBaoCaoTongHopDoanhThuTheoBacSiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTongHopDoanhThuTheoBacSi)]
        public async Task<ActionResult<GridItem>> GetTotalBaoCaoTongHopDoanhThuTheoBacSiForGridAsync([FromBody] BaoCaoTongHopDoanhThuTheoBacSiQueryInfo queryInfo)
        {
            var data = await _baoCaoService.GetTotalBaoCaoTongHopDoanhThuTheoBacSiForGridAsync(queryInfo);
            return Ok(data);
        }
        #endregion

        #region Báo cáo chi tiet theo bác sĩ
        [HttpPost("GetBaoCaoChiTietDoanhThuTheoBacSiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoChiTietDoanhThuTheoBacSi)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoChiTietDoanhThuTheoBacSiForGridAsync([FromBody] BaoCaoDoanhThuTheoBacSiQueryInfo queryInfo)
        {
            //if (queryInfo.BacSiId == 0)
            //{
            //    queryInfo.LayTatCa = true;
            //}
            var gridData = await _baoCaoService.GetBaoCaoChiTietDoanhThuTheoBacSiForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalBaoCaoChiTietDoanhThuTheoBacSiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoChiTietDoanhThuTheoBacSi)]
        public async Task<ActionResult<GridItem>> GetTotalBaoCaoChiTietDoanhThuTheoBacSiForGridAsync([FromBody] BaoCaoDoanhThuTheoBacSiQueryInfo queryInfo)
        {
            // if(queryInfo.BacSiId == 0)
            //{
            //    queryInfo.LayTatCa = true;
            //}
            var data = await _baoCaoService.GetTotalBaoCaoChiTietDoanhThuTheoBacSiForGridAsync(queryInfo);
            return Ok(data);
        }
        [HttpPost("GetDanhSachBacSy")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDanhSachBacSy([FromBody] DropDownListRequestModel queryInfo)
        {
            //LookupItemVo lookupItemVo = new LookupItemVo { KeyId = 0, DisplayName = "Tất cả nhân viên" };
            //var lookup = await _baoCaoService.GetListBacSy(queryInfo);
            //lookup.Add(lookupItemVo);
            //return Ok(lookup.OrderBy(cc => cc.KeyId));
            var lookup = await _baoCaoService.GetListBacSy(queryInfo);
            return Ok(lookup);
        }
        #endregion

        #region Báo cáo thu tiền

        #region Báo cáo thu tiền viện phí

        [HttpPost("GetBaoCaoChiTietThuTienVienPhiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDanhSachThuTienVienPhi)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoChiTietThuTienVienPhiForGridAsync([FromBody] BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetBaoCaoChiTietThuTienVienPhiForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalBaoCaoChiTietThuTienVienPhiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDanhSachThuTienVienPhi)]
        public async Task<ActionResult<GridItem>> GetTotalBaoCaoChiTietThuTienVienPhiForGridAsync([FromBody] BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            var data = await _baoCaoService.GetTotalBaoCaoChiTietThuTienVienPhiForGridAsync(queryInfo);
            return Ok(data);
        }

        // to do nam ho mastter
        // Master ---
        [HttpPost("GetBaoCaoChiTietThuTienVienPhiForMasterGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDanhSachThuTienVienPhi)]
        public async Task<ActionResult> GetBaoCaoChiTietThuTienVienPhiForMasterGridAsync([FromBody] BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetBaoCaoChiTietThuTienVienPhiForMasterGridAsync(queryInfo);

            ICollection<BaoCaoChiTietThuVienPhiMasterGridVo> gridVos = (ICollection<BaoCaoChiTietThuVienPhiMasterGridVo>)gridData.Data;
            var totalItem = new TotalBaoCaoThuPhiVienPhiGridVo
            {
                TamUng = gridVos.Select(o => o.TotalTamUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HoanUng = gridVos.Select(o => o.TotalHoanUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                SoTienThu = gridVos.Select(o => o.TotalSoTienThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HuyThu = gridVos.Select(o => o.TotalHuyThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                CongNo = gridVos.Select(o => o.TotalCongNo.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TienMat = gridVos.Select(o => o.TotalTienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ChuyenKhoan = gridVos.Select(o => o.TotalChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                Pos = gridVos.Select(o => o.TotalPos.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoTienMat = gridVos.Select(o => o.TotalThuNoTienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoChuyenKhoan = gridVos.Select(o => o.TotalThuNoChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoPos = gridVos.Select(o => o.TotalThuNoPos.GetValueOrDefault()).DefaultIfEmpty(0).Sum()
            };

            return Ok(new { Data = gridData.Data, TotalRowCount = gridData.TotalRowCount, TotalRow = totalItem });
        }
        //// DeTail ---
        //[HttpPost("GetBaoCaoChiTietThuTienVienPhiForDeTailGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDanhSachThuTienVienPhi)]
        //public async Task<ActionResult<GridDataSource>> GetBaoCaoChiTietThuTienVienPhiForDeTailGridAsync( BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        //{
        //    var gridData = await _baoCaoService.GetBaoCaoChiTietThuTienVienPhiForDetailGridAsync(queryInfo);
        //    return Ok(gridData);
        //}
        // example DeTail ---
        [HttpPost("GetBaoCaoChiTietThuTienVienPhiForDeTailGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDanhSachThuTienVienPhi)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoChiTietThuTienVienPhiForDeTailGridAsync([FromBody] BaoCaoThuPhiVienPhiQueryInfoQueryInfo queryInfo)
        {
            if (queryInfo.AdditionalSearchString != null)
            {
                var stringObj = queryInfo.AdditionalSearchString.Split('-');
                queryInfo.NhanVienId = long.Parse(stringObj[2]);
                queryInfo.PhongBenhVienId = long.Parse(stringObj[3]);
                DateTime tuNgayPartKy = DateTime.Now;
                DateTime denNgaysPartKy = DateTime.Now;
                DateTime.TryParseExact(stringObj[0], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgayPartKy);
                DateTime.TryParseExact(stringObj[1], "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgaysPartKy);
                queryInfo.TuNgay = tuNgayPartKy;
                queryInfo.DenNgay = denNgaysPartKy;
            }

            var gridData = await _baoCaoService.GetBaoCaoChiTietThuTienVienPhiForDetailGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpGet("ExportBaoCaoChiTietThuTienVienPhi")]
        public ActionResult ExportBaoCaoChiTietThuTienVienPhi(int phongBenhVienId, int nhanVienId, string startDate, string endDate, string hosting)
        {
            DateTime TuNgayPart = DateTime.Now;
            DateTime DenNgaysPart = DateTime.Now;

            if (startDate != null)
            {
                startDate.TryParseExactCustom(out TuNgayPart);
                endDate.TryParseExactCustom(out DenNgaysPart);
            }

            var queryInfo = new BaoCaoThuPhiVienPhiQueryInfoQueryInfo
            {
                LayTatCa = true,
                PhongBenhVienId = phongBenhVienId,
                NhanVienId = nhanVienId,
                TuNgay = TuNgayPart,
                DenNgay = DenNgaysPart,
                TuNgayUTC = startDate,
                DenNgayUTC = endDate
            };
            var tenNhanVien = _baoCaoService.GetNameNhanVien(queryInfo.NhanVienId).Result;
            var tenKhoaPhong = _baoCaoService.GetNamePhongBenhVien(queryInfo.PhongBenhVienId).Result; // phong benh viện id => Khoa phòng bệnh viện id => k dùng 
            var baoCaoThuTienVienPhis = _baoCaoService.GetBaoCaoChiTietThuTienVienPhiForGridAsync(queryInfo).Result;

            ICollection<BaoCaoThuPhiVienPhiGridVo> gridVos = (ICollection<BaoCaoThuPhiVienPhiGridVo>)baoCaoThuTienVienPhis.Data;
            var totalItem = new TotalBaoCaoThuPhiVienPhiGridVo
            {
                TamUng = gridVos.Select(o => o.TamUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HoanUng = gridVos.Select(o => o.HoanUng.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                SoTienThu = gridVos.Select(o => o.SoTienThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                HuyThu = gridVos.Select(o => o.HuyThu.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                CongNo = gridVos.Select(o => o.CongNo.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TienMat = gridVos.Select(o => o.TienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ChuyenKhoan = gridVos.Select(o => o.ChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                Pos = gridVos.Select(o => o.Pos.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoTienMat = gridVos.Select(o => o.ThuNoTienMat.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoChuyenKhoan = gridVos.Select(o => o.ThuNoChuyenKhoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                ThuNoPos = gridVos.Select(o => o.ThuNoPos.GetValueOrDefault()).DefaultIfEmpty(0).Sum()
            };            

            //var datatoTal = _baoCaoService.GetTotalBaoCaoChiTietThuTienVienPhi(queryInfo).Result;
            if (baoCaoThuTienVienPhis == null || baoCaoThuTienVienPhis.Data.Count == 0)
                return NoContent();

            var bytes = _excelService.ExportBaoCaoThuTienVienPhi(baoCaoThuTienVienPhis.Data.Select(o => (BaoCaoThuPhiVienPhiGridVo)o).ToList(), queryInfo, tenNhanVien, tenKhoaPhong, hosting, totalItem);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoThuTienVienPhi" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #region Báo cáo thu tiền người bệnh

        [HttpPost("GetBaoCaoChiTietThuTienVienPhiBenhNhanForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoThuVienPhiBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoChiTietThuTienVienPhiBenhNhanForGridAsync([FromBody] BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetBaoCaoChiTietThuTienBenhNhanForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetTotalBaoCaoChiTietThuTienVienPhiBenhNhanForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoThuVienPhiBenhNhan)]
        public async Task<ActionResult<GridItem>> GetTotalBaoCaoChiTietThuTienVienPhiBenhNhanForGridAsync([FromBody] BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo queryInfo)
        {
            var data = await _baoCaoService.GetTotalBaoCaoChiTietThuTienBenhNhanForGridAsync(queryInfo);
            return Ok(data);
        }


        // to do nam ho
        [HttpGet("ExportBaoCaoChiTietThuTienBenhNhan")]
        public ActionResult ExportBaoCaoChiTietThuTienBenhNhan(int phongBenhVienId, int nhanVienId, string startDate, string endDate)
        {
            DateTime TuNgayPart = DateTime.Now;
            DateTime DenNgaysPart = DateTime.Now;

            if (startDate != null)
            {
                DateTime.TryParseExact(startDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
                DateTime.TryParseExact(endDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
            }

            var queryInfo = new BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo
            {
                LayTatCa = true,
                PhongBenhVienId = phongBenhVienId,
                NhanVienId = nhanVienId,
                TuNgay = TuNgayPart,
                DenNgay = DenNgaysPart
            };
            var tenNhanVien = _baoCaoService.GetNameNhanVien(queryInfo.NhanVienId).Result;
            var tenPhongBenhVien = _baoCaoService.GetNamePhongBenhVien(queryInfo.PhongBenhVienId).Result;
            var baoCaoThuTienBenhNhans = _baoCaoService.GetBaoCaoChiTietThuTienBenhNhanForGridAsync(queryInfo).Result;
            if (baoCaoThuTienBenhNhans == null || baoCaoThuTienBenhNhans.Data.Count == 0)
                return NoContent();
            var bytes = _excelService.ExportBaoCaoThuTienBenhNhan(baoCaoThuTienBenhNhans.Data.Select(o => (BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo)o).ToList(), queryInfo, tenNhanVien, tenPhongBenhVien);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoChiTietThuTienVienPhiBenhNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #endregion

        [HttpGet("ExportDetailedRevenueReportByDepartment")]
        public async Task<ActionResult> ExportDetailedRevenueReportByDepartment(string startDate, string endDate, string startDateKySoSanh, string endDateKySoSanh)
        {
            DateTime TuNgayPart = DateTime.Now;
            DateTime DenNgaysPart = DateTime.Now;
            DateTime.TryParseExact(startDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
            DateTime.TryParseExact(endDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
            DateTime TuNgayPartKy = DateTime.Now;
            DateTime DenNgaysPartKy = DateTime.Now;
            DateTime.TryParseExact(startDateKySoSanh, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPartKy);
            DateTime.TryParseExact(endDateKySoSanh, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPartKy);
            var dateTimeFilterVo = new DateTimeFilterVo();
            var queryInfo = new BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo
            {
                LayTatCa = true,
                TuNgay = TuNgayPart,
                DenNgay = DenNgaysPart,
                KySoSanhTuNgay = TuNgayPartKy,
                KySoSanhDenNgay = DenNgaysPartKy
            };

            dateTimeFilterVo.RangeDateTimeFilter = new RangeDateFilterVo
            {
                DateStart = TuNgayPart,
                DateEnd = DenNgaysPart
            };

            dateTimeFilterVo.RangeDateTimeSoSanh = new RangeDateFilterVo
            {
                DateStart = TuNgayPartKy,
                DateEnd = DenNgaysPartKy
            };

            var baoCaoChiTietDoanhThuTheoKhoaPhongGrid = _baoCaoService.GetBaoCaoChiTietDoanhThuTheoKhoaPhongForExportAsync(queryInfo).Result;

            var bytes = _excelService.ExportDetailedRevenueReportByDepartment(baoCaoChiTietDoanhThuTheoKhoaPhongGrid.Data.Select(o => (BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo)o).ToList(), dateTimeFilterVo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoChiTietDoanhThuTheoKhoaPhong" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpGet("ExportAggregateRevenueReportByDepartment")]
        public async Task<ActionResult> ExportAggregateRevenueReportByDepartment(string startDate, string endDate, string startDateKySoSanh, string endDateKySoSanh)
        {

            DateTime.TryParseExact(startDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime TuNgayPart);
            DateTime.TryParseExact(endDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime denNgayPart);

            DateTime.TryParseExact(startDateKySoSanh, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime TuNgayPartKy);
            DateTime.TryParseExact(endDateKySoSanh, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime DenNgaysPartKy);
            var dateTimeFilterVo = new DateTimeFilterVo();
            var queryInfo = new BaoCaoTongHopDoanhThuTheoKhoaPhongQueryInfo
            {
                LayTatCa = true,
                TuNgay = TuNgayPart,
                DenNgay = denNgayPart,
                KySoSanhTuNgay = TuNgayPartKy,
                KySoSanhDenNgay = DenNgaysPartKy
            };
            dateTimeFilterVo.RangeDateTimeFilter = new RangeDateFilterVo
            {
                DateStart = TuNgayPart,
                DateEnd = denNgayPart
            };

            dateTimeFilterVo.RangeDateTimeSoSanh = new RangeDateFilterVo
            {
                DateStart = TuNgayPartKy,
                DateEnd = DenNgaysPartKy
            };

            var baoCaoTongHopDoanhThuTheoKhoaPhongGrid = _baoCaoService.GetBaoCaoTongHopDoanhThuTheoKhoaPhongForGridAsync(
                queryInfo).Result;

            var bytes = _excelService.ExportAggregateRevenueReportByDepartment(baoCaoTongHopDoanhThuTheoKhoaPhongGrid.Data.Select(o => (BaoCaoTongHopDoanhThuTheoKhoaPhongGridVo)o).ToList(), dateTimeFilterVo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTongHopDoanhThuTheoKhoaPhong" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpGet("ExportConsolidatedSalesReportByDoctor")]
        public async Task<ActionResult> ExportConsolidatedSalesReportByDoctor(string startDate, string endDate)
        {
            DateTime TuNgayPart = DateTime.Now;
            DateTime DenNgaysPart = DateTime.Now;
            DateTime.TryParseExact(startDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
            DateTime.TryParseExact(endDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
            var dateTimeFilterVo = new DateTimeFilterVo();
            var queryInfo = new BaoCaoTongHopDoanhThuTheoBacSiQueryInfo
            {
                LayTatCa = true,
                TuNgay = TuNgayPart,
                DenNgay = DenNgaysPart,
                Take = 1000
            };

            DateTime startDateTime = Convert.ToDateTime(startDate);
            DateTime endDateTime = Convert.ToDateTime(endDate);
            dateTimeFilterVo.RangeDateTimeFilter = new RangeDateFilterVo
            {
                DateStart = startDateTime,
                DateEnd = endDateTime
            };

            var baoCaoTongHopDoanhThuTheoBacSiGrid = _baoCaoService.GetBaoCaoTongHopDoanhThuTheoBacSiForGridAsync(queryInfo).Result;

            var bytes = _excelService.ExportConsolidatedSalesReportToXlsx(baoCaoTongHopDoanhThuTheoBacSiGrid.Data.Select(o => (BaoCaoTongHopDoanhThuTheoBacSiGridVo)o).ToList(), dateTimeFilterVo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTongHopDoanhThuTheoBacSi" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpGet("ExportDetailedSalesReportByDoctor")]
        public async Task<ActionResult> ExportDetailedSalesReportByDoctor(string startDate, string endDate, int bacSiId)
        {
            DateTime TuNgayPart = DateTime.Now;
            DateTime DenNgaysPart = DateTime.Now;
            DateTime.TryParseExact(startDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
            DateTime.TryParseExact(endDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
            var dateTimeFilterVo = new DateTimeFilterVo();
            var queryInfo = new BaoCaoDoanhThuTheoBacSiQueryInfo
            {
                LayTatCa = true,
                TuNgay = TuNgayPart,
                DenNgay = DenNgaysPart,
                BacSiId = bacSiId
            };

            DateTime startDateTime = Convert.ToDateTime(startDate);
            DateTime endDateTime = Convert.ToDateTime(endDate);
            dateTimeFilterVo.RangeDateTimeFilter = new RangeDateFilterVo
            {
                DateStart = startDateTime,
                DateEnd = endDateTime
            };
            var tenBacSi = await _baoCaoService.GetNameBacSy(queryInfo.BacSiId);
            var baoCaoChiTietDoanhThuTheoBacSiGrid = _baoCaoService.GetBaoCaoChiTietDoanhThuTheoBacSiForGridAsync(queryInfo).Result;

            var bytes = _excelService.DetailedSalesReportByDoctor(baoCaoChiTietDoanhThuTheoBacSiGrid.Data.Select(o => (BaoCaoChiTietDoanhThuTheoBacSiGridVo)o).ToList(), dateTimeFilterVo, tenBacSi);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoChiTietDoanhThuTheoBacSi" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #region Báo cáo lưu trữ hồ sơ bệnh án 19/2/2021
        [HttpPost("GetBaoCaoLuuTruHoSoBenhAnForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuTruHoSoBenhAn)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoLuuTruHoSoBenhAnForGridAsync([FromBody] BaoCaoLuuHoSoBenhAnVo queryInfo)
        {
            var gridData = await _baoCaoService.GetBaoCaoLuuTruHoSoBenhAnForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region  In
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("InBaoCaoLuuTruHoSoBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuTruHoSoBenhAn)]
        public async Task<ActionResult> InBaoCaoLuuTruHoSoBenhAn(BaoCaoLuuHoSoBenhAnVo baoCaoLuuHoSoBenhAnVo)
        {
            var phieuIns = await _baoCaoService.InBaoCaoLuuTruHoSoBenhAn(baoCaoLuuHoSoBenhAnVo);
            return Ok(phieuIns);
        }
        #endregion
        #region import excel
        [HttpPost("ExportLuuTruHoSoBenhAns")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuTruHoSoBenhAn)]
        public async Task<ActionResult> ExportLuuTruHoSoBenhAns([FromBody] BaoCaoLuuHoSoBenhAnVo queryInfo)
        {
            var gridData = await _baoCaoService.GetBaoCaoLuuTruHoSoBenhAnForGridAsync(queryInfo);
            var luuTruHoSoBenhAnData = gridData.Data.Select(p => (BaoCaoLuuHoSoBenhAnGridVo)p).ToList();
            var excelData = luuTruHoSoBenhAnData.Map<List<LuuTruHoSoBenhAnExport>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LuuTruHoSoBenhAnExport.ThuTuSap), "Thứ tự sắp"));
            lstValueObject.Add((nameof(LuuTruHoSoBenhAnExport.SoLuuTru), "Số lưu trữ"));
            lstValueObject.Add((nameof(LuuTruHoSoBenhAnExport.HoTen), "Họ tên"));
            lstValueObject.Add((nameof(LuuTruHoSoBenhAnExport.GioiTinh), "Giới tính"));
            lstValueObject.Add((nameof(LuuTruHoSoBenhAnExport.Tuoi), "Tuổi"));
            lstValueObject.Add((nameof(LuuTruHoSoBenhAnExport.ThoiGianVaoVienString), "Thời gian vào viện"));
            lstValueObject.Add((nameof(LuuTruHoSoBenhAnExport.ThoiGianRaVienString), "Thời gian ra viện"));
            lstValueObject.Add((nameof(LuuTruHoSoBenhAnExport.ChanDoan), "Chẩn đoán"));
            lstValueObject.Add((nameof(LuuTruHoSoBenhAnExport.ICD), "ICD"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Báo cáo lưu trữ hồ sơ bệnh án", 2, "Báo cáo lưu trữ hồ sơ bệnh án");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoLuuTruHoSoBenhAn" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("ExportLuuTruHoSoBenhAn")]
        public async Task<ActionResult> ExportLuuTruHoSoBenhAn(BaoCaoLuuHoSoBenhAnVo baoCaoLuuHoSoBenhAnVo)
        {
            var gridData = await _baoCaoService.GetBaoCaoLuuTruHoSoBenhAnForGridAsync(baoCaoLuuHoSoBenhAnVo);
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }

            var datas = gridData.Data.Cast<BaoCaoLuuHoSoBenhAnGridVo>().ToList();
            var bytes = _baoCaoService.ExportBaoCaoLuuTruHoSoBenhAn(datas, baoCaoLuuHoSoBenhAnVo.NgayVaoVien, baoCaoLuuHoSoBenhAnVo.NgayVaoVien, baoCaoLuuHoSoBenhAnVo.KhoaId, baoCaoLuuHoSoBenhAnVo.Hosting);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBenhNhanKhamNgoaiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        #endregion
        #region Báo cáo người bệnh khám ngoại trú 22/2/2021

        [HttpPost("GetTaCaPhongTheoKhoa")]
        public async Task<ActionResult<ICollection<ChucDanhItemVo>>> GetListNhomChucDanh([FromBody] DropDownListRequestModel queryInfo)
        {
            var subId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);

            var lstPhongBenhVienWithKhoaPhong = await _phongBenhVienService.GetListPhongBenhVienByKhoaPhongId(subId, queryInfo);
            List<KhoaKhamTemplateVo> listKhoa = new List<KhoaKhamTemplateVo>();
            KhoaKhamTemplateVo objectKhoa = new KhoaKhamTemplateVo();
            objectKhoa.KeyId = 0;
            objectKhoa.DisplayName = "Tất cả phòng";
            objectKhoa.Ma = "TC";
            objectKhoa.Ten = "Tất cả phòng";
            listKhoa.Add(objectKhoa);
            var lookup = lstPhongBenhVienWithKhoaPhong.Select(item => new KhoaKhamTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();
            listKhoa.AddRange(lookup);
            return Ok(listKhoa);
        }
        [HttpPost("GetBaoCaoBenhNhanKhamNgoaiTruForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBenhNhanKhamNgoaiTru)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoBenhNhanKhamNgoaiTruForGridAsync([FromBody] BaoCaoBenhNhanKhamNgoaiTruQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBenhNhanKhamNgoaiTruForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region  In
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("InBaoCaoBenhNhanKhamNgoaiTru")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuTruHoSoBenhAn)]
        public async Task<ActionResult> InBaoCaoBenhNhanKhamNgoaiTru(BaoCaoBenhNhanKhamNgoaiTruVo BaoCaoBenhNhanKhamNgoaiTruVo)
        {
            var phieuIns = await _baoCaoService.InBaoCaoBenhNhanKhamNgoaiTru(BaoCaoBenhNhanKhamNgoaiTruVo);
            return Ok(phieuIns);
        }
        #endregion
        #region export
        //[HttpPost("ExportBaoCaoBenhNhanKhamNgoaiTru")]
        //public async Task<ActionResult> ExportBaoCaoBenhNhanKhamNgoaiTru(BaoCaoBenhNhanKhamNgoaiTruVo baoCaoBenhNhanKhamNgoaiTruVo)
        //{
        //    var gridData = await _baoCaoService.GetBaoCaoBenhNhanKhamNgoaiTruForGridAsync(baoCaoBenhNhanKhamNgoaiTruVo);
        //    if (gridData == null || gridData.Data.Count == 0)
        //    {
        //        return NoContent();
        //    }

        //    var datas = gridData.Data.Cast<BaoCaoBenhNhanKhamNgoaiTruGridVo>().ToList();
        //    var bytes = _baoCaoService.ExportBaoCaoBenhNhanKhamNgoaiTru(datas, baoCaoBenhNhanKhamNgoaiTruVo.TuNgay, baoCaoBenhNhanKhamNgoaiTruVo.DenNgay, baoCaoBenhNhanKhamNgoaiTruVo.PhongId, baoCaoBenhNhanKhamNgoaiTruVo.Hosting);
        //    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBenhNhanKhamNgoaiTru" + DateTime.Now.Year + ".xls");
        //    Response.ContentType = "application/vnd.ms-excel";

        //    return new FileContentResult(bytes, "application/vnd.ms-excel");
        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoBenhNhanKhamNgoaiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBenhNhanKhamNgoaiTru)]
        public async Task<ActionResult> ExportBaoCaoBenhNhanKhamNgoaiTru([FromBody] BaoCaoBenhNhanKhamNgoaiTruQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBenhNhanKhamNgoaiTruForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBenhNhanKhamNgoaiTru(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoXuatNhapTon" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        #endregion

        #region Báo cáo xuất nhập tồn
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataBaoCaoXuatNhapTonForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoXuatNhapTon)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoXuatNhapTonForGridAsync([FromBody] BaoCaoXuatNhapTonQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoXuatNhapTonForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataBaoCaoXuatNhapTonForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoXuatNhapTon)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoXuatNhapTonForGridAsyncChild([FromBody] BaoCaoXuatNhapTonQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoXuatNhapTonForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("InBaoCaoXuatNhapTon")]
        public ActionResult InBaoCaoXuatNhapTon(InBaoCaoXuatNhapTonVo inBaoCaoXuatNhapTon)//inBaoCaoXuatNhapTon
        {
            var result = _baoCaoService.InBaoCaoXuatNhapTon(inBaoCaoXuatNhapTon);
            return Ok(result);
        }

        [HttpPost("ExportBaoCaoXuatNhapTon")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoXuatNhapTon)]
        public async Task<ActionResult> ExportBaoCaoXuatNhapTon(BaoCaoXuatNhapTonQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoXuatNhapTonForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoXuatNhapTon(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoXuatNhapTon" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        #region Báo cáo xuất nhập tồn vật tư
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataBaoCaoXuatNhapTonVTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoXNTVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoXuatNhapTonVTForGridAsync([FromBody] BaoCaoXuatNhapTonVTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoXuatNhapTonVTForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataBaoCaoXuatNhapTonVTForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoXNTVatTu)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoXuatNhapTonVTForGridAsyncChild([FromBody] BaoCaoXuatNhapTonVTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoXuatNhapTonVTForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("InBaoCaoXuatNhapTonVT")]
        public ActionResult InBaoCaoXuatNhapTonVT(InBaoCaoXuatNhapTonVTVo inBaoCaoXuatNhapTon)//inBaoCaoXuatNhapTon
        {
            var result = _baoCaoService.InBaoCaoXuatNhapTonVT(inBaoCaoXuatNhapTon);
            return Ok(result);
        }

        [HttpPost("ExportBaoCaoXuatNhapTonVT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoXNTVatTu)]
        public async Task<ActionResult> ExportBaoCaoXuatNhapTonVT(BaoCaoXuatNhapTonVTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoXuatNhapTonVTForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoXuatNhapTonVT(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoXuatNhapTon" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo tiếp nhận người bệnh khám
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataBaoCaoTiepNhanBenhNhanKhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTiepNhanBenhNhanKham)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoTiepNhanBenhNhanKhamForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTiepNhanBenhNhanKhamForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalBaoCaoTiepNhanBenhNhanKhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTiepNhanBenhNhanKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalBaoCaoTiepNhanBenhNhanKhamForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalBaoCaoTiepNhanBenhNhanKhamForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("InBaoCaoTiepNhanBenhNhanKham")]
        public ActionResult InBaoCaoTiepNhanBenhNhanKham(InBaoCaoTNBenhNhanKhamVo inBaoCaoTiepNhanBenhNhanKham)//inBaoCaoTiepNhanBenhNhanKham
        {
            var result = _baoCaoService.InBaoCaoTiepNhanBenhNhanKham(inBaoCaoTiepNhanBenhNhanKham);
            return Ok(result);
        }

        [HttpPost("ExportBaoCaoTiepNhanBenhNhanKham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTiepNhanBenhNhanKham)]
        public async Task<ActionResult> ExportBaoCaoTiepNhanBenhNhanKham(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoTiepNhanBenhNhanKhamForGridAsync(queryInfo);
            var datas = gridData.Data.Cast<BaoCaoTNBenhNhanKhamGridVo>().ToList();
            var bytes = _baoCaoService.ExportBaoCaoTiepNhanBenhNhanKham(datas, queryInfo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTiepNhanBenhNhanKham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        #region Báo cáo kêt quả chữa bệnh 24/2/2021
        [HttpPost("GetDataTemplateBaoCaoKetQuaKhamChuaBenhAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoKetQuaKhamChuaBenh)]
        public string GetDataTemplateBaoCaoKetQuaKhamChuaBenhAsync(BaoCaoKetQuaKhamChuaBenhVo baoCaoKetQuaKhamChuaBenhVo)
        {
            var html = _baoCaoService.GetDataTemplateBaoCaoKetQuaKhamChuaBenhAsync(baoCaoKetQuaKhamChuaBenhVo);
            return html;
        }

        [HttpPost("ExportBaoCaoKetQuaKhamChuaBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoKetQuaKhamChuaBenh)]
        public async Task<ActionResult> ExportBaoCaoKetQuaKhamChuaBenhAsync(BaoCaoKetQuaKhamChuaBenhViewModel viewModel)
        {
            DataTongHopBaoCaoKetQuaKhamChuaBenhVo datas = new DataTongHopBaoCaoKetQuaKhamChuaBenhVo();
            //var datas = gridData.Data.Cast<DataTongHopBaoCaoKetQuaKhamChuaBenhVo>().ToList();
            var info = new BaoCaoKetQuaKhamChuaBenhVo()
            {
                TuNgay = viewModel.TuNgay,
                DenNgay = viewModel.DenNgay,
                Hosting = viewModel.Hosting
            };
            var bytes = _baoCaoService.ExportBaoCaoKetQuaKhamChuaBenhAsync(datas, info);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoKetQuaKhamChuaBenh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #region In 
        [HttpPost("InBaoCaoKetQuaKhamChuaBenh")]
        public async Task<ActionResult<string>> InBaoCaoKetQuaKhamChuaBenh([FromBody] BaoCaoKetQuaKhamChuaBenhVo baoCaoKetQuaKhamChuaBenhVo)
        {
            var html = await _baoCaoService.InBaoCaoKetQuaKhamChuaBenh(baoCaoKetQuaKhamChuaBenhVo);
            return html;
        }
        #endregion
        #endregion
        #region báo cáo viện phi thu tiền 3/3/2021
        [HttpPost("GetBaoCaoThuTienVienPhiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoVienPhiThuTien)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoThuTienVienPhiForGridAsync([FromBody] BaoCaoVienPhiThuTienVo queryInfo)
        {
            var gridData = await _baoCaoService.GetBaoCaoThuTienVienPhiForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetBaoCaoThuTienVienPhiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoVienPhiThuTien)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoThuTienVienPhiChildForGridAsync(int nhanVienId, string tuNgay, string denNgay)
        {
            DateTime TuNgayPart = DateTime.Now;
            DateTime DenNgaysPart = DateTime.Now;
            DateTime.TryParseExact(tuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out TuNgayPart);
            DateTime.TryParseExact(denNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DenNgaysPart);
            var dateTimeFilterVo = new DateTimeFilterVo();
            var queryInfo = new BaoCaoVienPhiThuTienVo
            {
                //LayTatCa = true,
                TuNgay = TuNgayPart,
                DenNgay = DenNgaysPart,
                NhanVienThuNganId = nhanVienId
            };
            var gridData = await _baoCaoService.GetBaoCaoThuTienVienPhiChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #region In 
        [HttpPost("InBaoCaoVienPhiThuTien")]
        public async Task<ActionResult<string>> InBaoCaoVienPhiThuTien([FromBody] BaoCaoVienPhiThuTienVo baoCaoVienPhiThuTienVo)
        {
            var html = await _baoCaoService.InBaoCaoVienPhiThuTien(baoCaoVienPhiThuTienVo);
            return html;
        }
        #endregion
        #endregion
        #region báo cáo thống kê đơn thuốc

        [HttpPost("GetBaoCaoThongKeDonThuocForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoThongKeDonThuoc)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoThongKeDonThuocForGridAsync(BaoCaoThongKeDonThuocVo baoCaoThongKeDonThuocVo)
        {
            var gridData = await _baoCaoService.GetBaoCaoThongKeDonThuocForGridAsync(baoCaoThongKeDonThuocVo);
            return Ok(gridData);
        }
        [HttpPost("ExportThongKeDonThuocs")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoThongKeDonThuoc)]
        public async Task<ActionResult> ExportThongKeDonThuocs(BaoCaoThongKeDonThuocVo baoCaoThongKeDonThuocVo)
        {
            var gridData = await _baoCaoService.GetBaoCaoThongKeDonThuocForGridAsync(baoCaoThongKeDonThuocVo);
            var chucDanhData = gridData.Data.Select(p => (BaoCaoThongKeDonThuocGridVo)p).ToList();
            var excelData = chucDanhData.Map<List<ThongKeDonThuocExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.MaYT), "Mã Y Tế"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.SoBenhAn), "Số Bệnh Án"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.HoVaTen), "Họ Và Tên"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.NamSinh), "Năm Sinh"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.GioiTinh), "Giới Tính"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.DiaChi), "Địa Chỉ"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.MaBHYT), "Mã BHYT"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.MaBenh), "Mã Bệnh"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.NgayVaoString), "Ngày Vào"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.NgayRaString), "Ngày Ra"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.ChanDoan), "Chẩn Đoán"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.TienSuBenh), "Tiền Sử Bệnh"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.KhoaRa), "Khoa Ra"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.TrangThai), "Trạng Thái"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.BsKeToa), "BS Kê Toa"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.TenThuoc), "Tên Thuốc"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.HamLuong), "Hàm Lượng"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.SoLuong), "Số Lượng"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.Sang), "Sáng"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.Trua), "Trưa"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.Chieu), "Chiều"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.Tra), "Trả"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.GhiChu), "Ghi Chú"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.KhoPhat), "Kho Phát"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.BHYT), "BHYT"));
            lstValueObject.Add((nameof(ThongKeDonThuocExportExcel.NgayDuyetPhieuString), "Ngày Duyệt Phiếu"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Thống Kê Đơn Thuốc");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=ThongKeDonThuoc" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("ExportThongKeDonThuoc")]
        public async Task<ActionResult> ExportThongKeDonThuoc(BaoCaoThongKeDonThuocVo baoCaoThongKeDonThuocVo)
        {
            var gridData = await _baoCaoService.GetBaoCaoThongKeDonThuocForGridAsync(baoCaoThongKeDonThuocVo);
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }

            var datas = gridData.Data.Cast<BaoCaoThongKeDonThuocGridVo>().ToList();
            var bytes = _baoCaoService.ExportBaoCaoThongKeDonThuoc(datas, baoCaoThongKeDonThuocVo.TuNgay, baoCaoThongKeDonThuocVo.DenNgay, baoCaoThongKeDonThuocVo.Hosting);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoThongKeDonThuoc" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion


        #region Báo cáo doanh thu dịch vu theo nhóm

        [HttpPost("GetBaoCaoDoanhThuDichVuTheoNhomBenhVienForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDoanhThuTheoNhomDichVu)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoDoanhThuDichVuTheoNhomBenhVienForGridAsync([FromBody] BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoDoanhThuTheoNhomDichVu3961Async(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoDoanhThuBenhVienTheoNhom")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuTheoNhomDichVu)]
        public ActionResult ExportBaoCaoDoanhThuBenhVienTheoNhom(BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo queryInfo)
        {
            var gridData = _baoCaoService.GetDataBaoCaoDoanhThuTheoNhomDichVu3961Async(queryInfo).Result;
            var bytes = _excelService.ExportBaoCaoDoanhThuTheoNhomBenhVien(gridData, queryInfo);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDoanhThuTheoNhomDichVu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion


        #region Báo cáo tồn kho
        [HttpPost("GetDataBaoCaoTonKhoForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTonKho)]
        public async Task<ActionResult> GetDataBaoCaoTonKhoForGridAsync(BaoCaoTonKhoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTonKhoForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("ExportBaoCaoTonKho")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTonKho)]
        public async Task<ActionResult> ExportBaoCaoTonKho(BaoCaoTonKhoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTonKhoForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTonKho(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTonKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region báo cáo luu xet nghiem hang ngay

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuKetQuaXetNghiemHangNgay)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayForGridAsync([FromBody] QueryInfo queryInfo)
        {
            BaoCaoKetQuaXetNghiemQueryInfo baoCaoKetQuaXetNghiemQueryInfo = new BaoCaoKetQuaXetNghiemQueryInfo();
            if (queryInfo.AdditionalSearchString != null && queryInfo.AdditionalSearchString != "")
            {
                var jsonString = JsonConvert.DeserializeObject<BaoCaoKetQuaXetNghiemQueryInfo>(queryInfo.AdditionalSearchString);
                if (jsonString.TuNgay != null && jsonString.DenNgay != null)
                {
                    DateTime tuNgay = DateTime.Now;
                    DateTime.TryParseExact(jsonString.TuNgayUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
                    DateTime denNgay = DateTime.Now;
                    DateTime.TryParseExact(jsonString.DenNgayUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                    baoCaoKetQuaXetNghiemQueryInfo = jsonString;
                    baoCaoKetQuaXetNghiemQueryInfo.TuNgay = tuNgay;
                    baoCaoKetQuaXetNghiemQueryInfo.DenNgay = denNgay;

                    baoCaoKetQuaXetNghiemQueryInfo.Skip = queryInfo.Skip;
                    baoCaoKetQuaXetNghiemQueryInfo.Take = queryInfo.Take;
                    var grid = await _baoCaoXetNghiemService.GetDataBaoCaoLuuKetQuaXetNghiemHangNgayForGridAsync(baoCaoKetQuaXetNghiemQueryInfo, false);
                    return Ok(grid);
                }
            }
            return null;


        }
        [HttpPost("GetDataBaoCaoLuuKetQuaXetNghiemHangNgayTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuKetQuaXetNghiemHangNgay)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayTotalPageForGridAsync(string Json)
        {
            BaoCaoKetQuaXetNghiemQueryInfo baoCaoKetQuaXetNghiemQueryInfo = new BaoCaoKetQuaXetNghiemQueryInfo();
            if (Json != null || Json != "")
            {
                var jsonString = JsonConvert.DeserializeObject<BaoCaoKetQuaXetNghiemQueryInfo>(Json);
                DateTime tuNgay = DateTime.Now;
                DateTime.TryParseExact(jsonString.TuNgayUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
                DateTime denNgay = DateTime.Now;
                DateTime.TryParseExact(jsonString.DenNgayUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                baoCaoKetQuaXetNghiemQueryInfo = jsonString;
                baoCaoKetQuaXetNghiemQueryInfo.TuNgay = tuNgay;
                baoCaoKetQuaXetNghiemQueryInfo.DenNgay = denNgay;
            }
            var grid = await _baoCaoXetNghiemService.GetDataBaoCaoLuuKetQuaXetNghiemHangNgayTotalPageForGridAsync(baoCaoKetQuaXetNghiemQueryInfo);
            return grid;
        }
        [HttpPost("GetListBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuKetQuaXetNghiemHangNgay)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListBHYT([FromBody] DropDownListRequestModel queryInfo)
        {
            var lookup = await _baoCaoXetNghiemService.GetListBHYT(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetListKSK")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuKetQuaXetNghiemHangNgay)]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKSK([FromBody] DropDownListRequestModel queryInfo)
        {
            var lookup = await _baoCaoXetNghiemService.GetListKSK(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("ExportBaoCaoLuuKetQuaXetNghiemHangNgay")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuKetQuaXetNghiemHangNgay)]
        public async Task<ActionResult> ExportBaoCaoLuuKetQuaXetNghiemHangNgay(string Json)
        {
            BaoCaoKetQuaXetNghiemQueryInfo baoCaoKetQuaXetNghiemQueryInfo = new BaoCaoKetQuaXetNghiemQueryInfo();
            if (Json != null || Json != "")
            {
                var jsonString = JsonConvert.DeserializeObject<BaoCaoKetQuaXetNghiemQueryInfo>(Json);
                baoCaoKetQuaXetNghiemQueryInfo = jsonString;
                DateTime tuNgay = DateTime.Now;
                DateTime.TryParseExact(jsonString.TuNgayUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
                DateTime denNgay = DateTime.Now;
                DateTime.TryParseExact(jsonString.DenNgayUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                baoCaoKetQuaXetNghiemQueryInfo.TuNgay = tuNgay;
                baoCaoKetQuaXetNghiemQueryInfo.DenNgay = denNgay;
            }
            var gridData = await _baoCaoXetNghiemService.GetDataBaoCaoLuuKetQuaXetNghiemHangNgayForGridAsync(baoCaoKetQuaXetNghiemQueryInfo, true);
            if (gridData == null || gridData.Data.Count == 0)
            {
                return NoContent();
            }

            var datas = gridData.Data.Cast<BaoCaoLuuketQuaXeNghiemTrongNgayGridVo>().ToList();
            var bytes = _baoCaoService.ExportBaoCaoLuuKetQuaXetNghiemHangNgay(datas, baoCaoKetQuaXetNghiemQueryInfo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoLuuKetQuaXetNghiemHangNgay" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("GetListPhongBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuKetQuaXetNghiemHangNgay)]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> GetListPhongBenhVienAsync(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauKhamBenhService.GetListPhongBenhVienAsync(model);

            var defaulTatCa = new LookupItemTemplateVo
            {
                KeyId = 0,
                DisplayName = "Tất cả",
                Ma = "TC",
                Ten = "Tất cả"
            };
            lookup.Add(defaulTatCa);

            if (model.Query != null && model.Query != "")
            {
                //var th = lookup.Select(s => s.DisplayName.RemoveDiacritics().ToLower().Contains(model.Query.RemoveDiacritics().ToLower())).ToList();
                lookup = lookup.Where(p => (p.DisplayName != null
                                            && p.DisplayName.RemoveVietnameseDiacritics().ToLower().Trim().Contains(model.Query.RemoveVietnameseDiacritics().ToLower().Trim())
                || (p.Ma != null
                    && p.Ma.RemoveVietnameseDiacritics().ToLower().Trim().Contains(model.Query.RemoveVietnameseDiacritics().ToLower().Trim())))).ToList();
            }
            return Ok(lookup.OrderBy(s => s.KeyId));
        }
        [HttpPost("XuLyInBaoCaoLuuKetQuaXetNghiemHangNgayAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuKetQuaXetNghiemHangNgay)]
        public async Task<ActionResult<string>> XuLyInBaoCaoLuuKetQuaXetNghiemHangNgayAsync(string Json)
        {
            BaoCaoKetQuaXetNghiemQueryInfo baoCaoKetQuaXetNghiemQueryInfo = new BaoCaoKetQuaXetNghiemQueryInfo();
            if (Json != null || Json != "")
            {
                var jsonString = JsonConvert.DeserializeObject<BaoCaoKetQuaXetNghiemQueryInfo>(Json);
                DateTime tuNgay = DateTime.Now;
                DateTime.TryParseExact(jsonString.TuNgayUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out tuNgay);
                DateTime denNgay = DateTime.Now;
                DateTime.TryParseExact(jsonString.DenNgayUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out denNgay);
                baoCaoKetQuaXetNghiemQueryInfo = jsonString;
                baoCaoKetQuaXetNghiemQueryInfo.TuNgay = tuNgay;
                baoCaoKetQuaXetNghiemQueryInfo.DenNgay = denNgay;
            }
            var gridData = await _baoCaoXetNghiemService.GetDataBaoCaoLuuKetQuaXetNghiemHangNgayForGridAsync(baoCaoKetQuaXetNghiemQueryInfo, true);
            var datas = gridData.Data.Cast<BaoCaoLuuketQuaXeNghiemTrongNgayGridVo>().ToList();
            var phieuIn = await _baoCaoXetNghiemService.XuLyInBaoCaoLuuKetQuaXetNghiemHangNgayAsync(baoCaoKetQuaXetNghiemQueryInfo, datas);
            return phieuIn;
            //var phieuInPdf = new PhieuInNhanVienKhamSucKhoeViewModel()
            //{
            //    Html = phieuIn,
            //    TenFile = "BaoCaoLuuKetQuaXetNghiem",
            //};
            //return GetFilePDFFromHtml(phieuInPdf);
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

        #region Báo cáo Người Bệnh Làm Xét Nghiệm
        [HttpPost("GetDataBaoCaoBenhNhanLamXetNghiemForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBenhNhanLamXetNghiem)]
        public async Task<ActionResult> GetDataBaoCaoBenhNhanLamXetNghiemForGridAsync(BaoCaoBenhNhanLamXetNghiemQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBenhNhanLamXetNghiemForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoBenhNhanLamXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBenhNhanLamXetNghiem)]
        public async Task<ActionResult> ExportBaoCaoBenhNhanLamXetNghiem(BaoCaoBenhNhanLamXetNghiemQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBenhNhanLamXetNghiemForGridAsync(queryInfo, true);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBenhNhanLamXetNghiem(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBenhNhanLamXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion Báo cáo Người Bệnh Làm Xét Nghiệm

        #region Báo cáo Số Xét Nghiệm Sàng Lọc Hiv 
        [HttpPost("GetDataBaoCaoSoXetNghiemSangLocHivForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoSoXetNghiemSangLocHiv)]
        public async Task<ActionResult> GetDataBaoCaoSoXetNghiemSangLocHivForGridAsync(BaoCaoSoXetNghiemSangLocHivQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoSoXetNghiemSangLocHivForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoSoXetNghiemSangLocHiv")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoSoXetNghiemSangLocHiv)]
        public async Task<ActionResult> ExportBaoCaoSoXetNghiemSangLocHiv(BaoCaoSoXetNghiemSangLocHivQueryInfo queryInfo)
        {
            //for test
            //queryInfo.DichVuKyThuatBenhVienId = 3798;
            //for Export
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoSoXetNghiemSangLocHivForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoSoXetNghiemSangLocHiv(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoSoXetNghiemSangLocHiv" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion Báo cáo Số Xét Nghiệm Sàng Lọc Hiv

        #region Báo cáo Tổng Hợp Số Lượng Xét Nghiệm Theo Thời Gian
        [HttpPost("GetDataBaoCaoTongHopSoLuongXetNghiemTheoThoiGianForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopSoLuongXetNghiemTheo)]
        public async Task<ActionResult> GetDataBaoCaoTongHopSoLuongXetNghiemTheoThoiGianForGridAsync(BaoCaoTongHopSoLuongXetNghiemTheoThoiGianQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTongHopSoLuongXetNghiemTheoThoiGianForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportBaoCaoTongHopSoLuongXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopSoLuongXetNghiemTheo)]
        public async Task<ActionResult> ExportBaoCaoTongHopSoLuongXetNghiem(BaoCaoTongHopSoLuongXetNghiemTheoThoiGianQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTongHopSoLuongXetNghiemTheoThoiGianForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTongHopSoLuongXetNghiemTheoThoiGian(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTongHopSoLuongXetNghiemTheoThoiGian" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion Báo cáo Tổng Hợp Số Lượng Xét Nghiệm Theo Thời Gian
        #region báo cáo tổng hợp kết qua KSk 08072021
        [HttpPost("ExportBaoCaoTongHopKetQuaKSK")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopKetQuaKSK)]
        public async Task<ActionResult> ExportBaoCaoTongHopKetQuaKSK([FromBody]BaoCaoTongHopKetQuaKhamDoanQueryInfoQueryInfo queryInfo)
        {
            if (queryInfo.HopDongId != null && queryInfo.CongTyId != null)
            {
                var objModel = new ModelVoNhanVien
                {
                    HopDongId = (long)queryInfo.HopDongId,
                    CongTyId = (long)queryInfo.CongTyId,
                    ToDate = queryInfo.ToDate,
                    FromDate = queryInfo.FromDate
                };
                var gridData = await _baoCaoKhamDoanHopDongService.ListDichVu(objModel);

                var gridDataTheoNhanVien = await _baoCaoKhamDoanHopDongService.ListDichVuNhanVien(objModel);
                var getTenCongTy = _baoCaoKhamDoanHopDongService.GetNameCongTy(objModel.CongTyId);
                var getHopDong = _baoCaoKhamDoanHopDongService.GetNameHopDongKhamSucKhoe(objModel.HopDongId);
                byte[] bytes = null;
                if (gridData != null)
                {
                    //gridData là tat ca dịch vu cong ty hop dong disctin
                    //bc
                    bytes = _baoCaoKhamDoanHopDongService.ExportBaoCaoTongHopKetQuaKSK(gridData, gridDataTheoNhanVien, getTenCongTy, getHopDong);
                }
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTongHopKetQuaKSK" + DateTime.Now.Year + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                return new FileContentResult(bytes, "application/vnd.ms-excel");
            }
            return null;

        }
        [HttpPost("GetHopDongKhamSucKhoes")]
        public async Task<ActionResult> GetHopDongKhamSucKhoe([FromBody]DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false)
        {
            var lookup = await _baoCaoKhamDoanHopDongService.GetHopDongKhamSucKhoe(queryInfo, LaHopDongKetThuc);
            return Ok(lookup);
        }

        [HttpPost("GetCongTys")]
        public async Task<ActionResult> GetCongTy([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _baoCaoKhamDoanHopDongService.GetCongTy(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetHTMLBaoCaoTongHopKetQuaKSK")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopKetQuaKSK)]
        public async Task<string> GetHTMLBaoCaoTongHopKetQuaKSK([FromBody]BaoCaoTongHopKetQuaKhamDoanQueryInfoQueryInfo queryInfo)
        {
            if (queryInfo.HopDongId != null && queryInfo.CongTyId != null)
            {
                var objModel = new ModelVoNhanVien
                {
                    HopDongId = (long)queryInfo.HopDongId,
                    CongTyId = (long)queryInfo.CongTyId,
                    ToDate = queryInfo.ToDate,
                    FromDate = queryInfo.FromDate
                };
                var gridData = await _baoCaoKhamDoanHopDongService.ListDichVu(objModel);

                var gridDataTheoNhanVien = await _baoCaoKhamDoanHopDongService.ListDichVuNhanVien(objModel);
                var getHTMLBaoCaoTongHopKetQuaKSK = await _baoCaoKhamDoanHopDongService.GetHTMLBaoCaoTongHopKetQuaKSK(gridData, gridDataTheoNhanVien);
                return getHTMLBaoCaoTongHopKetQuaKSK;
            }
            return null;

        }
        #endregion


        #region Báo cáo sổ phúc trình phẫu thuật/ thủ thuật
        [HttpPost("GetDataBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoSoPhucTrinhPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetTotalPageBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoSoPhucTrinhPhauThuatThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoSoPhucTrinhPhauThuatThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoSoPhucTrinhPhauThuatThuThuat)]
        public async Task<ActionResult> ExportBaoCaoSoPhucTrinhPhauThuatThuThuat([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetBaoCaoSoPhucTrinhPhauThuatThuThuatForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoSoPhucTrinhPhauThuatThuThuatGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoSoPhucTrinhPhauThuatThuThuat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        #region Báo cáo Hoạt Động Khoa Khám Bệnh
        [HttpPost("GetDataBaoCaoHoatDongKhoaKhamBenhForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoHoatDongKhoaKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoHoatDongKhoaKhamBenhForGridAsync(BaoCaoHoaDongKhoaKhamBenhQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoHoatDongKhoaKhamBenhForGridAsync(queryInfo);
            return Ok(grid);
        }

        [HttpPost("ExportBaoCaoHoatDongKhoaKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongKhoaKhamBenh)]
        public async Task<ActionResult> ExportBaoCaoHoatDongKhoaKhamBenh(BaoCaoHoaDongKhoaKhamBenhQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongKhoaKhamBenhForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoHoatDongKhoaKhamBenh(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHoatDongKhoaKhamBenh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo Thống Kê Số Lượng Thủ Thuật
        [HttpPost("GetDataBaoCaoThongKeSoLuongThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoSoLuongThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoThongKeSoLuongThuThuat(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoThongKeSoLuongThuThuatForGridAsync(queryInfo);
            return Ok(grid);
        }

        [HttpPost("GetTotalBaoCaoThongKeSoLuongThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoSoLuongThuThuat)]
        public async Task<ActionResult<GridDataSource>> GetTotalBaoCaoThongKeSoLuongThuThuatAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalBaoCaoThongKeSoLuongThuThuatAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoThongKeSoLuongThuThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoSoLuongThuThuat)]
        public async Task<ActionResult> ExportBaoCaoThongKeSoLuongThuThuat(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoThongKeSoLuongThuThuatForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoThongKeSoLuongThuThuat(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoThongKeSoLuongThuThuat" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion


        #region Báo cáo tiếp nhận bệnh phẩm

        [HttpPost("GetListNhomDoiTuong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhomDoiTuong([FromBody] DropDownListRequestModel model)
        {
            //var lookup = await _khoaPhongService.GetListKhoaPhongAll(model);
            //var lookup = new List<LookupItemVo>();
            //var item1 = new LookupItemVo
            //{
            //    KeyId = 1,
            //    DisplayName = "Tất cả"
            //};
            //var item2 = new LookupItemVo
            //{
            //    KeyId = 2,
            //    DisplayName = "Không theo đoàn"
            //};
            //var item3 = new LookupItemVo
            //{
            //    KeyId = 3,
            //    DisplayName = "Đoàn ABC..."
            //};
            //lookup.Add(item1);
            //lookup.Add(item2);
            //lookup.Add(item3);
            return Ok(_baoCaoService.GetListDoan(model));
        }


        [HttpPost("GetDataBaoCaoTiepNhanBenhPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTiepNhanBenhPham)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoTiepNhanBenhPhamForGridAsync(BaoCaoTiepNhanBenhPhamQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoTiepNhanBenhPhamForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetTotalPageBaoCaoTiepNhanBenhPhamForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTiepNhanBenhPham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBaoCaoTiepNhanBenhPhamForGridAsync([FromBody] BaoCaoTiepNhanBenhPhamQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalBaoCaoTiepNhanBenhPhamForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoTiepNhanBenhPham")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTiepNhanBenhPham)]
        public async Task<ActionResult> ExportBaoCaoTiepNhanBenhPham([FromBody] BaoCaoTiepNhanBenhPhamQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTiepNhanBenhPhamForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTiepNhanBenhPhamGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTiepNhanBenhPham" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo tồn kho - xét nghiệm

        [HttpPost("GetTatCaKhoTheoKhoaXetNghiems")]
        public ActionResult<ICollection<LookupItemVo>> GetTatCaKhoTheoKhoaXetNghiems([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = _baoCaoService.GetTatCaKhoTheoKhoaXetNghiems(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoTonKhoXetNghiemForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTonKhoXN)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoTonKhoXetNghiemForGridAsync(BaoCaoTonKhoXetNghiemQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoTonKhoXetNghiemForGridAsync(queryInfo);
            return Ok(grid);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoTonKhoXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTonKhoXN)]
        public async Task<ActionResult> ExportBaoCaoTonKhoXetNghiem([FromBody] BaoCaoTonKhoXetNghiemQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTonKhoXetNghiemForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTonKhoXetNghiemGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTonKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #region Báo cáo tồn kho - kế toán
        [HttpPost("GetDataBaoCaoTonKhoKTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTonKhoKT)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoTonKhoKTForGridAsync(BaoCaoTonKhoKTQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoTonKhoKTForGridAsync(queryInfo);
            return grid;
        }


        [HttpPost("GetDataTotalPageBaoCaoTonKhoKTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTonKhoKT)]
        public async Task<ActionResult<GridDataSource>> GetDataTotalPageBaoCaoTonKhoKTForGridAsync([FromBody] BaoCaoTonKhoKTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataTotalPageBaoCaoTonKhoKTForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoTonKhoKT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTonKhoKT)]
        public async Task<ActionResult> ExportBaoCaoTonKhoKT([FromBody] BaoCaoTonKhoKTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTonKhoKTForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTonKhoKTGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTonKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo lưu trữ hồ sơ bệnh án

        [HttpPost("GetDataLuuTruHoSoBenhAnForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoLuuTruHoSoBenhAn)]
        public async Task<ActionResult<GridDataSource>> GetDataLuuTruHoSoBenhAnForGridAsync([FromBody] BaoCaoLuuTruHoSoBenhAnQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoLuuTruHoSoBenhAnForGridAsync(queryInfo);
            return Ok(grid);

        }

        [HttpPost("ExportBaoCaoLuuTruHoSoBenhAn")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoLuuTruHoSoBenhAn)]
        public async Task<ActionResult> ExportBaoCaoLuuTruHoSoBenhAn(BaoCaoLuuTruHoSoBenhAnQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoLuuTruHoSoBenhAnForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoLuuTruHoSoBenhAn(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoLuuTruHoSoBenhAn" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetKhoaKhamNoiTruCoTatCa")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.YeuCauTiepNhan, Enums.DocumentType.BaoCaoLuuTruHoSoBenhAn)]
        public async Task<ActionResult> GetKhoaKhamNoiTruCoTatCa([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _baoCaoService.GetKhoaKhamNoiTruAsync(queryInfo);
            if (lookup.Count > 0)
            {
                lookup.Insert(0, new LookupItemTemplateVo()
                {
                    KeyId = 0,
                    Ma = "",
                    Ten = "Toàn viện",
                    DisplayName = "Toàn viện"
                });
            }
            return Ok(lookup);
        }
        #endregion

        #region Báo cáo dược tình hình xuất nội bộ
        [HttpPost("GetDataBaoCaoDuocTinhHinhXuatNoiBoForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDuocTinhHinhXuatNoiBo)]
        public async Task<ActionResult> GetDataBaoCaoDuocTinhHinhXuatNoiBoForGridAsync(BaoCaoDuocTinhHinhXuatNoiBoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoDuocTinhHinhXuatNoiBoForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoDuocTinhHinhXuatNoiBo")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDuocTinhHinhXuatNoiBo)]
        public async Task<ActionResult> ExportBaoCaoDuocTinhHinhXuatNoiBo(BaoCaoDuocTinhHinhXuatNoiBoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoDuocTinhHinhXuatNoiBoForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoDuocTinhHinhXuatNoiBo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDuocTinhHinhXuatNoiBo" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("GetKhos")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhos([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoService.GetKhoWithoutKhoLe(queryInfo);
            return Ok(result);
        }
        #endregion

        #region Báo cáo hiệu quả công việc

        [HttpPost("GetDataBaoCaoHieuQuaCongViec")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHieuQuaCongViec)]
        public async Task<ActionResult> GetDataBaoCaoHieuQuaCongViec(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoHieuQuaCongViecForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoHieuQuaCongViec")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHieuQuaCongViec)]
        public async Task<ActionResult> ExportBaoCaoHieuQuaCongViec(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoHieuQuaCongViecForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoHieuQuaCongViec(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHieuQuaCongViec" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo tình hình trả nhà cung cấp

        [HttpPost("GetKhoDuocPhamLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoDuocPhamLookupAsync([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoService.GetKhoDuocPhamLookupAsync(queryInfo);
            return Ok(result);
        }


        [HttpPost("GetDataBaoCaoTinhHinhTraNCCForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhTraNCC)]
        public async Task<ActionResult> GetDataBaoCaoTinhHinhTraNCCForGridAsync(BaoCaoTinhHinhTraNCCQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTinhHinhTraNCCForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoTinhHinhTraNCC")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhTraNCC)]
        public async Task<ActionResult> ExportBaoCaoTinhHinhTraNCC(BaoCaoTinhHinhTraNCCQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTinhHinhTraNCCForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTinhHinhTraNCC(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTinhHinhTraNCC" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo biên bản kiểm kê - kế toán

        [HttpPost("GetTatCaKhoaBaoCaoKiemKeKTLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTatCaKhoaBaoCaoKiemKeKTLookupAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetTatCaKhoaBaoCaoKiemKeKTs(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetTatCakhoTheoKhoaBaoCaoKiemKeKTLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTatCakhoTheoKhoaBaoCaoKiemKeKTLookupAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var lstKhoaIdDaChon = JsonConvert.DeserializeObject<KhoaDaChonVo>(queryInfo.ParameterDependencies);
            var result = await _baoCaoService.GetTatCaKhoTheoKhoaBaoCaoKiemKeKTs(queryInfo, lstKhoaIdDaChon.KhoaId);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoBienBanKiemKeKTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBienBanKiemKeKT)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoBienBanKiemKeKTForGridAsync(BaoCaoBienBanKiemKeKTQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoBienBanKiemKeKTForGridAsync(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoBienBanKiemKeKT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBienBanKiemKeKT)]
        public async Task<ActionResult> ExportBaoCaoBienBanKiemKeKT([FromBody] BaoCaoBienBanKiemKeKTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBienBanKiemKeKTForGridAsync(queryInfo, true);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBienBanKiemKeKTGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBienBanKiemKe" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpPost("ExportBaoCaoBienBanKiemKeKT")]
        //[ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBienBanKiemKeKT)]
        //public async Task<ActionResult> ExportBaoCaoBienBanKiemKeKT([FromBody] BaoCaoBienBanKiemKeKTQueryInfo queryInfo)
        //{
        //    byte[] bytes = _baoCaoService.ExportBaoCaoBienBanKiemKe28092021GridVo(queryInfo);
        //    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBienBanKiemKe" + DateTime.Now.Year + ".xls");
        //    Response.ContentType = "application/vnd.ms-excel";

        //    return new FileContentResult(bytes, "application/vnd.ms-excel");
        //}

        #endregion

        #region Báo cáo bảng kê phiếu xuất kho
        [HttpPost("GetDataBaoCaoBangKePhieuXuatKhoForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBangKePhieuXuatKho)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoBangKePhieuXuatKhoForGridAsync(BaoCaoBangKePhieuXuatKhoQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoBangKePhieuXuatKhoForGridAsync(queryInfo);

            var baoCaoPhieuXuatKhoVo = new BaoCaoBangKePhieuXuatKhoVo();
            var baoCaoPhieuXuatKhoGridVos = new List<BaoCaoBangKePhieuXuatKhoGridVo>();

            if (grid.Data != null)
            {
                var datas = grid.Data.Cast<BaoCaoBangKePhieuXuatKhoGridVo>().ToList();


                var listNhom = datas.GroupBy(s => new { s.PhieuXuatId }).Select(s => new BaoCaoBangKePhieuXuatKhoGroupVo
                {
                    PhieuXuatId = s.First().PhieuXuatId
                }).ToList();


                foreach (var item in listNhom)
                {
                    baoCaoPhieuXuatKhoGridVos.AddRange(datas.Where(d => d.PhieuXuatId == item.PhieuXuatId).ToList());
                }

                baoCaoPhieuXuatKhoVo.TotalRowCount = baoCaoPhieuXuatKhoGridVos.Count();
                baoCaoPhieuXuatKhoVo.Data = baoCaoPhieuXuatKhoGridVos.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
                baoCaoPhieuXuatKhoVo.ListGroupTheoFileExecls = baoCaoPhieuXuatKhoGridVos.Skip(queryInfo.Skip).Take(queryInfo.Take).GroupBy(s => new { s.PhieuXuatId }).Select(s => new BaoCaoBangKePhieuXuatKhoGroupVo
                {
                    PhieuXuatId = s.First().PhieuXuatId
                }).ToList();
            }
            return Ok(baoCaoPhieuXuatKhoVo);
        }

        //need remove
        [HttpPost("GetDataTotalPageBaoCaoBangKePhieuXuatKhoForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBangKePhieuXuatKho)]
        public async Task<ActionResult<GridDataSource>> GetDataTotalPageBaoCaoBangKePhieuXuatKhoForGridAsync([FromBody] BaoCaoBangKePhieuXuatKhoQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoBangKePhieuXuatKhoForGridAsync(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoBangKePhieuXuatKho")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBangKePhieuXuatKho)]
        public async Task<ActionResult> ExportBaoCaoBangKePhieuXuatKho([FromBody] BaoCaoBangKePhieuXuatKhoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBangKePhieuXuatKhoForGridAsync(queryInfo, true);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBangKePhieuXuatKhoGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBangKePhieuXuatKho" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        
        #region Báo cáo tình hình nhập ncc chi tiết

        [HttpPost("GetKhoDuocPhamBaoCaoTinhHinhNhapNCCChiTietLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoDuocPhamBaoCaoTinhHinhNhapNCCChiTietLookupAsync([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoService.GetKhoDuocPhamBaoCaoTinhHinhNhapNCCChiTietLookupAsync(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoTinhHinhNhapNCCChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhNhapNCCChiTiet)]
        public async Task<ActionResult> GetDataBaoCaoTinhHinhNhapNCCChiTietForGridAsync(BaoCaoTinhHinhNhapNCCChiTietQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTinhHinhNhapNCCChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportBaoCaoTinhHinhNhapNCCChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhNhapNCCChiTiet)]
        public async Task<ActionResult> ExportBaoCaoTinhHinhNhapNCCChiTiet(BaoCaoTinhHinhNhapNCCChiTietQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTinhHinhNhapNCCChiTietForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTinhHinhNhapNCCChiTiet(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTinhHinhNhapNCCChiTiet" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo dược chi tiết xuất nội bộ
        [HttpPost("GetDataBaoCaoDuocChiTietXuatNoiBoForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhNhapNCCChiTiet)]
        public async Task<ActionResult> GetDataBaoCaoDuocChiTietXuatNoiBoForGridAsync(BaoCaoDuocChiTietXuatNoiBoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoDuocChiTietXuatNoiBoForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoDuocChiTietXuatNoiBo")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhNhapNCCChiTiet)]
        public async Task<ActionResult> ExportBaoCaoDuocChiTietXuatNoiBo(BaoCaoDuocChiTietXuatNoiBoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoDuocChiTietXuatNoiBoForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoDuocChiTietXuatNoiBo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDuocChiTietXuatNoiBo" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo chi tiết miễn phí trốn viện

        [HttpPost("GetDataBaoCaoChiTietMienPhiTronVienForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoChiTietMienPhiTronVien)]
        public async Task<ActionResult> GetDataBaoCaoChiTietMienPhiTronVienForGridAsync(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoChiTietMienPhiTronVienForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalBaoCaoChiTietMienPhiTronVienForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoChiTietMienPhiTronVien)]
        public async Task<ActionResult> GetTotalBaoCaoChiTietMienPhiTronVienForGridAsync(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalBaoCaoChiTietMienPhiTronVienForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoChiTietMienPhiTronVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoChiTietMienPhiTronVien)]
        public async Task<ActionResult> ExportBaoCaoChiTietMienPhiTronVien(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoChiTietMienPhiTronVienForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoChiTietMienPhiTronVien(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoChiTietMienPhiTronVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetTongCongChiPhiMienPhiTronVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoChiTietMienPhiTronVien)]
        public async Task<ActionResult<BaoCaoChiTietMienPhiTronVienGridVo>> GetTongCongChiPhiMienPhiTronVienAsync(QueryInfo queryInfo)
        {
            var tongCong = new BaoCaoChiTietMienPhiTronVienGridVo()
            {
                GiamPhi = 0,
                MienPhi = 0,
                TongGiamPhiMienPhi = 0
            };

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoChiTietMienPhiTronVienForGridAsync(queryInfo);
            var datas = (ICollection<BaoCaoChiTietMienPhiTronVienGridVo>)gridData.Data;

            tongCong.GiamPhi = datas.Sum(x => x.GiamPhi);
            tongCong.MienPhi = datas.Sum(x => x.MienPhi);
            tongCong.TongGiamPhiMienPhi = datas.Sum(s => s.TongGiamPhiMienPhi);
            return Ok(tongCong);
        }
        #endregion

        #region Báo cáo tình hình nhập từ nhà cung cấp

        [HttpPost("GetKhoBaoCaoTinhHinhNhapTuNCCLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoBaoCaoTinhHinhNhapTuNCCLookupAsync([FromBody] LookupQueryInfo queryInfo)
        {
            //var result = new List<LookupItemVo>()
            //{
            //    new LookupItemVo() {KeyId = 0 , DisplayName="Toàn viện"},
            //    new LookupItemVo() {KeyId = 1 , DisplayName="Kho 1"},
            //    new LookupItemVo() {KeyId = 2 , DisplayName="Kho 2"},
            //    new LookupItemVo() {KeyId = 3 , DisplayName="Kho 3"},
            //    new LookupItemVo() {KeyId = 4 , DisplayName="Kho 4"},
            //    new LookupItemVo() {KeyId = 5 , DisplayName="Kho 5"}
            //};
            var result = await _baoCaoService.GetKhoBaoCaoTinhHinhNhapTuNCCLookupAsync(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoTinhHinhNhapTuNCCForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTinhHinhNhapTuNhaCungCap)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoTinhHinhNhapTuNCCForGridAsync(BaoCaoTinhHinhNhapTuNhaCungCapQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoTinhHinhNhapTuNhaCungCapForGridAsync(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoTinhHinhNhapTuNCC")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTinhHinhNhapTuNhaCungCap)]
        public async Task<ActionResult> ExportBaoCaoTinhHinhNhapTuNCC([FromBody] BaoCaoTinhHinhNhapTuNhaCungCapQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTinhHinhNhapTuNhaCungCapForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBangTinhHinhNhapTuNhaCungCapGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTinhHinhNhapTuNCC" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #region Báo cáo tổng hợp doanh thu thai sản đã sinh
        [HttpPost("GetDataBaoCaoTongHopDoanhThuThaiSanDaSinhForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopDoanhThuThaiSanDaSinh)]
        public async Task<ActionResult> GetDataBaoCaoTongHopDoanhThuThaiSanDaSinhForGridAsync(BaoCaoTongHopDoanhThuThaiSanDaSinhQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTongHopDoanhThuThaiSanDaSinhForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoTongHopDoanhThuThaiSanDaSinh")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopDoanhThuThaiSanDaSinh)]
        public async Task<ActionResult> ExportBaoCaoTongHopDoanhThuThaiSanDaSinh(BaoCaoTongHopDoanhThuThaiSanDaSinhQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTongHopDoanhThuThaiSanDaSinhForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTongHopDoanhThuThaiSanDaSinh(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTongHopDoanhThuThaiSanDaSinh" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion


        #region Báo cáo sổ chi tiết vật tư hàng hóa

        [HttpPost("GetKhoHangHoa")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoHangHoa([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoService.GetKhoHangHoa(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetKhoDuocPhamVatTuTheoKhoHangHoa")]
        public async Task<ActionResult<ICollection<LookupItemDuocPhamHoacVatTuVo>>> GetKhoDuocPhamVatTuTheoKhoHangHoa([FromBody] DropDownListRequestModel queryInfo)
        {
            var lstKhoIdDaChon = JsonConvert.DeserializeObject<KhoDaChonVo>(queryInfo.ParameterDependencies);
            var result = await _baoCaoService.GetKhoDuocPhamVatTuTheoKhoHangHoa(queryInfo, lstKhoIdDaChon.KhoId);

            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoSoChiTietVatTuHangHoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoSoChiTietVatTuHangHoa)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoSoChiTietVatTuHangHoaForGridAsync(BaoCaoTheKhoSoChiTietVatTuHangHoaQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoTheKhoSoChiTietVatTuHangHoaForGridAsync(queryInfo);
            return grid;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoBangTheKhoSoChiTietVatTuHangHoa")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoSoChiTietVatTuHangHoa)]
        public async Task<ActionResult> ExportBaoCaoBangTheKhoSoChiTietVatTuHangHoa([FromBody] BaoCaoTheKhoSoChiTietVatTuHangHoaQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTheKhoSoChiTietVatTuHangHoaForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBangTheKhoSoChiTietVatTuHangHoaGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoSoChiTietVatTuHangHoa" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo thuốc sắp hết hạn
        [HttpPost("GetDataBaoCaoThuocSapHetHanForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoThuocSapHetHanDung)]
        public async Task<ActionResult> GetDataBaoCaoThuocSapHetHanForGridAsync(BaoCaoThuocSapHetHanQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoThuocSapHetHanForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoThuocSapHetHan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoThuocSapHetHanDung)]
        public async Task<ActionResult> ExportBaoCaoThuocSapHetHan(BaoCaoThuocSapHetHanQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoThuocSapHetHanForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoThuocSapHetHan(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoThuocSapHetHan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo doanh thu khám đoàn theo nhóm dịch vụ
        [HttpPost("GetDataBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDV)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync(queryInfo);
            return grid;
        }


        [HttpPost("GetDataTotalPageBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDV)]
        public async Task<ActionResult<GridDataSource>> GetDataTotalPageBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataTotalPageBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync(queryInfo);
            return Ok(gridData);
        }



        [HttpPost("GetTongCongDoanhThuKhamDoanTheoNhomDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDV)]
        public async Task<ActionResult<BaoCaoDoanhThuKhamDoanTheoNhomDVGridVo>> GetTongCongDoanhThuKhamDoanTheoNhomDichVuAsync(QueryInfo queryInfo)
        {
            var tongCong = new BaoCaoDoanhThuKhamDoanTheoNhomDVGridVo();

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync(queryInfo, true);
            var datas = (ICollection<BaoCaoDoanhThuKhamDoanTheoKhoaPhongGridVo>)gridData.Data;

            tongCong.KhamBenh = datas.Sum(a => a.KhamBenh ?? 0);
            tongCong.XetNghiem = datas.Sum(a => a.XetNghiem ?? 0);
            tongCong.NoiSoi = datas.Sum(a => a.NoiSoi ?? 0);
            tongCong.NoiSoiTMH = datas.Sum(a => a.NoiSoiTMH ?? 0);
            tongCong.SieuAm = datas.Sum(a => a.SieuAm ?? 0);
            tongCong.XQuang = datas.Sum(a => a.XQuang ?? 0);
            tongCong.CTScan = datas.Sum(a => a.CTScan ?? 0);
            tongCong.MRI = datas.Sum(a => a.MRI ?? 0);
            tongCong.DienTimDienNao = datas.Sum(a => a.DienTimDienNao ?? 0);
            tongCong.TDCNDoLoangXuong = datas.Sum(a => a.TDCNDoLoangXuong ?? 0);
            tongCong.DVKhac = datas.Sum(a => a.DVKhac ?? 0);
            return Ok(tongCong);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoDoanhThuKhamDoanTheoNhomDV")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDV)]
        public async Task<ActionResult> ExportBaoCaoDoanhThuKhamDoanTheoNhomDV([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoDoanhThuKhamDoanTheoNhomDVForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoDoanhThuKhamDoanTheoNhomDVGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDoanhThuKhamDoanTheoNhomDV" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo tổng hợp đăng ký gói dịch vụ

        [HttpPost("GetDataBaoCaoTongHopDangKyGoiDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopDangKyGoiDichVu)]
        public async Task<ActionResult> GetDataBaoCaoTongHopDangKyGoiDichVu(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTongHopDangKyGoiDichVuForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalBaoCaoTongHopDangKyGoiDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopDangKyGoiDichVu)]
        public async Task<ActionResult> GetTotalBaoCaoTongHopDangKyGoiDichVu(QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetTotalBaoCaoTongHopDangKyGoiDichVuForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoTongHopDangKyGoiDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTongHopDangKyGoiDichVu)]
        public async Task<ActionResult> ExportBaoCaoTongHopDangKyGoiDichVu(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoTongHopDangKyGoiDichVuForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoTongHopDangKyGoiDichVu(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTongHopDangKyGoiDichVu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo bảng kê xuất thuốc theo bệnh nhân
        [HttpPost("GetDataBaoCaoBangKeXuatThuocTheoBenhNhanForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBangKeXuatThuocTheoBenhNhan)]
        public async Task<ActionResult> GetDataBaoCaoBangKeXuatThuocTheoBenhNhanForGridAsync(BaoCaoBangKeXuatThuocTheoBenhNhanQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBangKeXuatThuocTheoBenhNhanForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoBangKeXuatThuocTheoBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBangKeXuatThuocTheoBenhNhan)]
        public async Task<ActionResult> ExportBaoCaoBangKeXuatThuocTheoBenhNhan(BaoCaoBangKeXuatThuocTheoBenhNhanQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBangKeXuatThuocTheoBenhNhanForGridAsync(queryInfo, true);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBangKeXuatThuocTheoBenhNhan(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BangKeXuatThuocTheoBenhNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion


        #region Báo cáo bảng kê chi tiết ttcn
        [HttpPost("GetDataBaoCaoBangKeChiTietTTCNForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBangKeChiTietTTCN)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoBangKeChiTietTTCNForGridAsync(BaoCaoBangKeChiTietTTCNQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoBangKeChiTietTTCNForGridAsync(queryInfo);
            return Ok(grid);
        }

        [HttpPost("ExportBaoCaoBangKeChiTietTTCN")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBangKeChiTietTTCN)]
        public async Task<ActionResult> ExportBaoCaoBangKeChiTietTTCN(BaoCaoBangKeChiTietTTCNQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBangKeChiTietTTCNForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBangKeChiTietTTCN(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBangKeChiTietTTCN" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo doanh thu khám đoàn theo khoa phòng
        [HttpPost("GetDataBaoCaoDoanhThuKhamDoanTheoKhoaPhongForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoKP)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoDoanhThuKhamDoanTheoKhoaPhongForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoDoanhThuKhamDoanTheoKhoaPhongForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetDataTotalPageBaoCaoDoanhThuKhamDoanTheoKhoaPhongForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoKP)]
        public async Task<ActionResult<GridDataSource>> GetDataTotalPageBaoCaoDoanhThuKhamDoanTheoKhoaPhongForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataTotalPageBaoCaoDoanhThuKhamDoanTheoKhoaPhongForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoDoanhThuKhamDoanTheoKhoaPhong")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoKP)]
        public async Task<ActionResult> ExportBaoCaoDoanhThuKhamDoanTheoKhoaPhong([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoDoanhThuKhamDoanTheoKhoaPhongForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoDoanhThuKhamDoanTheoKhoaPhongGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDoanhThuKhamDoanTheoKhoaPhong" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo chi tiết hoa hồng của người giới thiệu
        [HttpPost("GetDataBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoChiTietHoaHongCuaNguoiGT)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGridAsync(QueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGrid3961Async(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetDataTotalPageBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoChiTietHoaHongCuaNguoiGT)]
        public async Task<ActionResult<GridDataSource>> GetDataTotalPageBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataTotalPageBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoChiTietHoaHongCuaNguoiGioiThieu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoChiTietHoaHongCuaNguoiGT)]
        public async Task<ActionResult> ExportBaoCaoChiTietHoaHongCuaNguoiGioiThieu([FromBody] QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGrid3961Async(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoChiTietHoaHongCuaNguoiGioiThieu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetNoiGioiThieuDaCoNguoiBenh")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNoiGioiThieuDaCoNguoiBenhAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetNoiGioiThieuDaCoNguoiBenhAsync(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetTonGcongHoaHongCuaNguoiGioiThieu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoChiTietHoaHongCuaNguoiGT)]
        public async Task<ActionResult<BaoCaoChiTietMienPhiTronVienGridVo>> GetTonGcongHoaHongCuaNguoiGioiThieuAsync(QueryInfo queryInfo)
        {
            var tongCong = new BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo()
            {
                SoTienDV = 0
            };

            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoService.GetDataBaoCaoChiTietHoaHongCuaNguoiGioiThieuForGrid3961Async(queryInfo);
            var datas = (ICollection<BaoCaoChiTietHoaHongCuaNguoiGioiThieuGridVo>)gridData.Data;

            tongCong.SoTienDV = datas.Sum(x => x.SoTienDV);
            return Ok(tongCong);
        }
        #endregion

        #region Báo cáo cam kết tự nguyện sử dụng thuốc dv ngoài bhyt
        [HttpPost("GetDataBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoCamKetSuDungThuocNgoaiBHYT)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync(BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetDataTotalPageBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoCamKetSuDungThuocNgoaiBHYT)]
        public async Task<ActionResult<GridDataSource>> GetDataTotalPageBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync([FromBody] BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataTotalPageBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoCamKetSuDungThuocNgoaiBHYT)]
        public async Task<ActionResult> ExportBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYT([FromBody] BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYTGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoCamKetTuNguyenSuDungThuocDVNgoaiBHYT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion


        #region Báo cáo hoạt động cls

        [HttpPost("GetDataBaoCaoHoatDongCLSMauThucTeForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongCls)]
        public async Task<ActionResult> GetDataBaoCaoHoatDongCLSMauThucTeForGridAsync(BaoCaoHoatDongClsQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongCLSMauThucTeForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataBaoCaoHoatDongCLSMauCucQuanLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongCls)]
        public async Task<ActionResult> GetDataBaoCaoHoatDongCLSMauCucQuanLyForGridAsync(BaoCaoHoatDongClsQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongCLSMauCucQuanLyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("ExportBaoCaoHoatDongCLSMauThucTe")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongCls)]
        public async Task<ActionResult> ExportBaoCaoHoatDongCLSMauThucTe(BaoCaoHoatDongClsQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongCLSMauThucTeForGridAsync(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoHoatDongCLSMauThucTe(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHoatDongCls" + DateTime.Now.Year + ".xls");

            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportBaoCaoHoatDongCLSMauCucQuanLy")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongCls)]
        public async Task<ActionResult> ExportBaoCaoHoatDongCLSMauCucQuanLy(BaoCaoHoatDongClsQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongCLSMauCucQuanLyForGridAsync(queryInfo);

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoHoatDongCLSMauCucQuanLy(gridData, queryInfo);

            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHoatDongCls" + DateTime.Now.Year + ".xls");

            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo số liệu tính thời gian sử dụng dv của khách hàng
        [HttpPost("GetDataBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoSoLieuThoiGianSuDungDV)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync(BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync(queryInfo);
            return grid;
        }


        [HttpPost("GetDataTotalPageBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoSoLieuThoiGianSuDungDV)]
        public async Task<ActionResult<GridDataSource>> GetDataTotalPageBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync([FromBody] BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataTotalPageBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoSoLieuTinhThoiGianSuDungDVCuaKH")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoSoLieuThoiGianSuDungDV)]
        public async Task<ActionResult> ExportBaoCaoSoLieuTinhThoiGianSuDungDVCuaKH([FromBody] BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoSoLieuTinhThoiGianSuDungDVCuaKHGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoSoLieuTinhThoiGianSuDungDVCuaKH" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        #endregion

        #region Báo cáo kế toán nhập xuất tồn chi tiết

        [HttpPost("GetTatCakhoaLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTatCakhoaLookupAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var result = await _baoCaoService.GetTatCaKhoas(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetTatCakhoTheoKhoaLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetTatCakhoTheoKhoaLookupAsync([FromBody] DropDownListRequestModel queryInfo)
        {
            var lstKhoaIdDaChon = JsonConvert.DeserializeObject<KhoaDaChonVo>(queryInfo.ParameterDependencies);
            var result = await _baoCaoService.GetTatCaKhoTheoKhoas(queryInfo, lstKhoaIdDaChon.KhoaId);
            return Ok(result);
        }

        [HttpPost("GetDataBaoCaoKTNhapXuatTonChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoKTNhapXuatTonChiTiet)]
        public async Task<ActionResult> GetDataBaoCaoKTNhapXuatTonChiTietForGridAsync(BaoCaoKTNhapXuatTonChiTietQueryInfo queryInfo)
        {
            //var gridData = await _baoCaoService.GetDataBaoCaoKTNhapXuatTonChiTietForGridAsync(queryInfo);
            //return Ok(gridData);
            //need update
            return Ok();
        }

        [HttpPost("ExportBaoCaoKTNhapXuatTonChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoKTNhapXuatTonChiTiet)]
        public async Task<ActionResult> ExportBaoCaoKTNhapXuatTonChiTiet(BaoCaoKTNhapXuatTonChiTietQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoKTNhapXuatTonChiTietForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoKTNhapXuatTonChiTiet(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=tBaoCaoKTNhapXuatTonChiTiet" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("GetDataBaoCaoKeToanNhapXuatTonChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoKTNhapXuatTonChiTiet)]
        public async Task<ActionResult> GetDataBaoCaoKeToanNhapXuatTonChiTietForGridAsync(BaoCaoKTNhapXuatTonChiTietQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoKTNhapXuatTonChiTietForGridAsync(queryInfo);

            var baoCaoKTNhapXuatTonVo = new BaoCaoKTNhapXuatTonChiTietVo();
            var baoCaoKTNhapXuatTonGridVos = new List<BaoCaoKTNhapXuatTonChiTietGridVo>();


            // total page sum 
            if (gridData.Data != null)
            {  
                var datas = gridData.Data.Cast<BaoCaoKTNhapXuatTonChiTietGridVo>().ToList();
                baoCaoKTNhapXuatTonVo.DataSumPageTotal = datas;

                var lstKhoa = datas.GroupBy(s => new { Khoa = s.Kho }).Select(s => new KhoaGroupBaoCaoKTNhapXuatTonChiTietVo
                {
                    Khoa = s.First().Kho
                }).OrderBy(p => p.Khoa).ToList();

                var listNhom = datas.GroupBy(s => new { Khoa = s.Kho, s.Nhom }).Select(s => new NhomGroupBaoCaoKTNhapXuatTonChiTietVo
                {
                    Khoa = s.First().Kho,
                    Nhom = s.First().Nhom
                }).OrderBy(p => p.Khoa).ToList();


                foreach (var item in listNhom)
                {
                    baoCaoKTNhapXuatTonGridVos.AddRange(datas.Where(d => d.Nhom == item.Nhom && d.Kho == item.Khoa).ToList());
                }

                baoCaoKTNhapXuatTonVo.TotalRowCount = baoCaoKTNhapXuatTonGridVos.Count();
                baoCaoKTNhapXuatTonVo.Data = baoCaoKTNhapXuatTonGridVos.Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
                baoCaoKTNhapXuatTonVo.ListGroupTheoFileExecls = baoCaoKTNhapXuatTonGridVos.Skip(queryInfo.Skip).Take(queryInfo.Take)
               .GroupBy(s => new { Khoa = s.Kho, s.Nhom }).Select(s => new NhomGroupBaoCaoKTNhapXuatTonChiTietVo
               {
                   Khoa = s.First().Kho,
                   Nhom = s.First().Nhom
               }).OrderBy(p => p.Khoa).ToList();
            }


            return Ok(baoCaoKTNhapXuatTonVo);
        }
        [HttpPost("GetTotalSumBaoCaoKeToanNhapXuatTonChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoKTNhapXuatTonChiTiet)]
        public async Task<ActionResult> GetTotalSumBaoCaoKeToanNhapXuatTonChiTietForGridAsync(BaoCaoKTNhapXuatTonChiTietQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoKTNhapXuatTonChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Báo cáo bảng kê giao hóa đơn sang phòng kế toán
        [HttpPost("GetDataBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBangKeGiaoHoaDonSangPKT)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync(BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync(queryInfo);
            return Ok(grid);
        }


        [HttpPost("GetDataTotalPageBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBangKeGiaoHoaDonSangPKT)]
        public async Task<ActionResult<GridDataSource>> GetDataTotalPageBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync([FromBody] BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataTotalPageBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoBangKeGiaoHoaDonSangPKT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBangKeGiaoHoaDonSangPKT)]
        public async Task<ActionResult> ExportBaoCaoBangKeGiaoHoaDonSangPKT([FromBody] BaoCaoBangKeGiaoHoaDonSangPKTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBangKeGiaoHoaDonSangPKTForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBangKeGiaoHoaDonSangPKTGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBangKeGiaoHoaDonSangPKT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo hoạt động nội trú
        [HttpPost("GetKhoNhanVienLookupAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoNhanVienLookupAsync([FromBody] LookupQueryInfo queryInfo)
        {
            var result = await _baoCaoService.GetKhoNhanVienLookupAsync(queryInfo);
            return Ok(result);
        }

        [HttpPost("GetListNoiDieuTri")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNoiDieuTri([FromBody] DropDownListRequestModel model)
        {
            var lookup = new List<LookupItemVo>();
            var item1 = new LookupItemVo
            {
                KeyId = 0,
                DisplayName = "Điều Trị Nội Trú Sở Y Tế"
            };
            var item2 = new LookupItemVo
            {
                KeyId = 1,
                DisplayName = "Điều Trị Nội Trú Tại Bệnh Viện"
            };
            lookup.Add(item1);
            lookup.Add(item2);
            return Ok(lookup);
        }

        [HttpPost("GetDataBaoCaoHoatDongNoiTruForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoHoatDongNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoHoatDongNoiTruForGridAsync(BaoCaoHoatDongNoiTruQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoHoatDongNoiTruForGridAsync(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoHoatDongNoiTru")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoHoatDongNoiTru)]
        public async Task<ActionResult> ExportBaoCaoHoatDongNoiTruGridVo([FromBody] BaoCaoHoatDongNoiTruQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoHoatDongNoiTruForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoHoatDongNoiTruGridVo(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoHoatDongNoiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region Báo cáo biên bản kiểm kê dược vt
        [HttpPost("GetDataBaoCaoBienBanKiemKeDPVTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoBienBanKiemKeDuocVT)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoBienBanKiemKeDPVTForGridAsync(BaoCaoBienBanKiemKeDPVTQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoBienBanKiemKeDPVTForGridAsync(queryInfo);
            return Ok(grid);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoBienBanKiemKeDPVT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoBienBanKiemKeDuocVT)]
        public async Task<ActionResult> ExportBaoCaoBienBanKiemKeDPVT([FromBody] BaoCaoBienBanKiemKeDPVTQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoBienBanKiemKeDPVTForGridAsync(queryInfo, true);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportBaoCaoBienBanKiemKeDPVT(gridData, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoBienBanKiemKe" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion

        #region báo cáo dự trù số lượng người thực hiện dv ls -cls 22/10/2021
        [HttpPost("ExportBaoCaoDuTruSoLuongNguoiThucHienDvLSCLS")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanThongKeSoNguoiKhamSucKhoeLSCLS)]
        public async Task<ActionResult> ExportBaoCaoDuTruSoLuongNguoiThucHienDvLSCLS([FromBody]BaoCaoTongHopKetQuaKhamDoanQueryInfoQueryInfo queryInfo)
        {
            if (queryInfo.HopDongId != null && queryInfo.CongTyId != null)
            {
              
                var gridData = await _baoCaoKhamDoanHopDongService.ListDichVuBenhNhanDangKy(queryInfo);

                byte[] bytes = null;
                if (gridData != null)
                {
                    //gridData là tat ca dịch vu cong ty hop dong disctin
                    //bc BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo
                    
                    bytes = _baoCaoKhamDoanHopDongService.ExportBaoCaoDuTruSLNguoiThucHienDichVu(gridData.Data.Cast<BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo>().ToList());
                }
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDuTruSLNguoiThucHien" + DateTime.Now.Year + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                return new FileContentResult(bytes, "application/vnd.ms-excel");
            }
            return null;

        }
        [HttpPost("GetHTMLDuTruSoLuongNguoiThucHienDvLSCLS")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.KhamDoanThongKeSoNguoiKhamSucKhoeLSCLS)]
        public async Task<ActionResult<GridDataSource>>  GetHTMLDuTruSoLuongNguoiThucHienDvLSCLS([FromBody]BaoCaoTongHopKetQuaKhamDoanQueryInfoQueryInfo queryInfo)
        {
            if (queryInfo.HopDongId != null && queryInfo.CongTyId != null)
            {
                var gridData = await _baoCaoKhamDoanHopDongService.ListDichVuBenhNhanDangKy(queryInfo);

                return Ok(gridData);
            }
            return null;

        }
        #endregion

        #region Báo cáo dịch vu ngoài gói 2/11/2021 

    
        [HttpPost("GetBaoCaoChiTietThuTienDichVuNgoaiGoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDichVuPhatSinhNgoaiGoiCuaKeToan)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoChiTietThuTienDichVuNgoaiGoiForGridAsync([FromBody] BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo queryInfo)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.GetAllForBaoCaoBenhNhanKhamDicVuNgoaiGoi(queryInfo);
            int stt = 1;
            var listExel = new List<BaoCaoDVNgoaiGoiKeToanExelGridVo>();
            foreach (var vb in gridData)
            {
                foreach (var dv in vb.TenDichVus)
                {
                    listExel.Add(new BaoCaoDVNgoaiGoiKeToanExelGridVo
                    {
                        TenDichVu = dv.TenDichVu,
                        DonGiaBenhVien = dv.DonGiaBenhVien,
                        DonGiaMoi = dv.DonGiaMoi,
                        SoTienDuocMienGiam = dv.SoTienDuocMienGiam,
                        SoTienThucThu = dv.SoTienThucThu,
                        BHYT = vb.BHYT,
                        ChiTietBHYTs = vb.ChiTietBHYTs,
                        ChiTietCongNoTuNhans = vb.ChiTietCongNoTuNhans,
                        CongNoCaNhan = vb.CongNoCaNhan,
                        CongNoTuNhan = vb.CongNoTuNhan,
                        CoTiemChung = vb.CoTiemChung,
                        DataPhieuChis = vb.DataPhieuChis,
                        GioiTinh = vb.GioiTinh,
                        GoiDichVu = vb.GoiDichVu,
                        HoTen = vb.HoTen,
                        Id = vb.Id,
                        LaPhieuHuy = vb.LaPhieuHuy,
                        LoaiChiTienBenhNhan = vb.LoaiChiTienBenhNhan,
                        LoaiThuTienBenhNhan = vb.LoaiThuTienBenhNhan,
                        LyDo = vb.LyDo,
                        MaNguoiBenh = vb.MaNguoiBenh,
                        MaNhanVien = vb.MaNhanVien,
                        MaTiepNhan = vb.MaTiepNhan,
                        NamSinh = vb.NamSinh,
                        NgayBienLai = vb.NgayBienLai,

                        NguoiGioiThieu = vb.NguoiGioiThieu,
                        SoBienLai = vb.SoBienLai,
                        SoBienLaiRemoveSpecial=vb.SoBienLaiRemoveSpecial,
                        SoHoaDon = vb.SoHoaDon,
                        SoHoaDonChiTiet = vb.SoHoaDonChiTiet,
                        SoPhieuThuGhiNo = vb.SoPhieuThuGhiNo,
                        SoTienThuChuyenKhoan = vb.SoTienThuChuyenKhoan,
                        SoTienThuPos = vb.SoTienThuPos,
                        SoTienThuTamUng = vb.SoTienThuTamUng,
                        SoTienThuTienMat = vb.SoTienThuTienMat,
                        STT = stt,
                        TenDichVus = vb.TenDichVus,
                        TongChiPhiBNTT = vb.TongChiPhiBNTT,
                        Voucher = vb.Voucher,

                    });
                    stt++;
                }
            }
            var countTask =  listExel.Count();
            var queryTask = listExel.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            return Ok(listExel);
        }

        [HttpPost("ExportBaoCaoChiTietThuTienDichVuNgoaiGoi")]
        public ActionResult ExportBaoCaoChiTietThuTienDichVuNgoaiGoi(BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo queryInfo)
        {
            var tenCongTy = _baoCaoKhamDoanHopDongService.GetTenCongTy((long)queryInfo.HopDongId);
            var baoCao = _baoCaoKhamDoanHopDongService.GetAllForBaoCaoBenhNhanKhamDicVuNgoaiGoi(queryInfo).Result;

            var datatoTal = _baoCaoKhamDoanHopDongService.GetTotalBaoCaoChiTietThuTienDichVuNgoaiGoiForGridAsync(queryInfo);
            if (baoCao == null || baoCao.Count == 0)
                return NoContent();

            var bytes = _baoCaoKhamDoanHopDongService.ExportBaoCaoChiTietThuTienDichVuNgoaiGoi(baoCao.Select(o => (BaoCaoDVNgoaiGoiKeToanGridVo)o).ToList(), queryInfo, tenCongTy, datatoTal.Result);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDanhSachDichVuNgoaiGoiCuaKeToan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetTotalBaoCaoChiTietThuTienDichVuNgoaiGoiForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDichVuPhatSinhNgoaiGoiCuaKeToan)]
        public async Task<ActionResult<GridItem>> GetTotalBaoCaoChiTietThuTienDichVuNgoaiGoiForGridAsync([FromBody] BaoCaoDVNgoaiGoiKeToanQueryInfoQueryInfo queryInfo)
        {
            var data = await _baoCaoKhamDoanHopDongService.GetTotalBaoCaoChiTietThuTienDichVuNgoaiGoiForGridAsync(queryInfo);
            return Ok(data);
        }
        [HttpPost("GetHopDongKhamDoan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDichVuPhatSinhNgoaiGoiCuaKeToan)]
        public async Task<ActionResult<GridItem>> GetHopDongKhamDoan([FromBody] DropDownListRequestModel queryInfo)
        {
            var data = await _baoCaoKhamDoanHopDongService.GetHopDongKhamDoan(queryInfo);
            return Ok(data);
        }
        #endregion

        #region Báo cáo tra cứu dữ liệu

        [HttpPost("GetDataBaoCaoTraCuuDuLieuForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoTraCuuDuLieu)]
        public async Task<ActionResult<GridDataSource>> GetDataBaoCaoTraCuuDuLieuForGridAsync(BaoCaoTraCuuDuLieuQueryInfo queryInfo)
        {
            var grid = await _baoCaoService.GetDataBaoCaoTraCuuDuLieuForGridAsync(queryInfo);
            return grid;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ExportBaoCaoTraCuuDuLieu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoTraCuuDuLieu)]
        public async Task<ActionResult> ExportBaoCaoTraCuuDuLieu([FromBody] BaoCaoTraCuuDuLieuQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataBaoCaoTraCuuDuLieuForGridAsync(queryInfo);
            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExporBaoCaoTraCuuDuLieuGridVo(gridData, queryInfo);
            }

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoTraCuuDuLieu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion
        #region BÁO CÁO DOANH THU KHÁM ĐOÀN THEO NHÓM DỊCH VỤ
        [HttpPost("GetDataBCDoanhThuKhamDoanTheoNhomDichVuChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDichVu)]
        public async Task<ActionResult<GridDataSource>> GetDataBCDoanhThuKhamDoanTheoNhomDichVuChiTiet([FromBody]BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo bc)
        {
            if (!string.IsNullOrEmpty(bc.AdditionalSearchString))
            {
                var json = JsonConvert.DeserializeObject<BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo>(bc.AdditionalSearchString);
                bc.ToDate = json.ToDate;
                bc.FromDate = json.FromDate;
                bc.Skip = json.Skip;
                bc.Take = json.Take;
                var gridData = await _baoCaoKhamDoanHopDongService.BaoCaoDoanhThuKhamDoanTheoNhomDichVu(bc);
                var data = gridData.Data.Cast<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo>().Where(d => d.HopDongId == json.HopDongId && d.CongTyId == json.CongTyId);

                return new GridDataSource { Data = data.ToArray(), TotalRowCount = data.Count() };
            }
            return null;
        }
        [HttpPost("GetDataTotalBCDoanhThuKhamDoanTheoNhomDichVuChiTiet")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDichVu)]
        public async Task<ActionResult<GridDataSource>> GetDataTotalBCDoanhThuKhamDoanTheoNhomDichVuChiTiet([FromBody]BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo bc)
        {
            
            if (!string.IsNullOrEmpty(bc.AdditionalSearchString))
            {
                var json = JsonConvert.DeserializeObject<BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo>(bc.AdditionalSearchString);
                bc.ToDate = json.ToDate;
                bc.FromDate = json.FromDate;
                bc.Skip = json.Skip;
                bc.Take = json.Take;
                var gridData = await _baoCaoKhamDoanHopDongService.BaoCaoDoanhThuKhamDoanTheoNhomDichVu(bc);
                var data = gridData.Data.Cast<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo>().Where(d => d.HopDongId == json.HopDongId && d.CongTyId == json.CongTyId);

                return new GridDataSource { TotalRowCount = data.Count() };
            }
            return null;
        }



        [HttpPost("GetDataBCDoanhThuKhamDoanTheoNhomDichVuToTalGroups")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDichVu)]
        public async Task<ActionResult<GridDataSource>> GetDataBCDoanhThuKhamDoanTheoNhomDichVuToTalGroups([FromBody]BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo bc)
        {
            var gridData = await _baoCaoKhamDoanHopDongService.BaoCaoDoanhThuKhamDoanTheoNhomDichVu(bc);

            var list = gridData.Data.Cast<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo>().GroupBy(d => d.TenCongTy)
                 .Select(d => new TotalTienDichVuKhamDoanGridVo
                 {
                     TenCongTy = d.First().TenCongTy,
                     SoTienDVKhamBenhs = d.Sum(g => g.SoTienDVKhamBenh),
                     SoTienDVXetNghiems = d.Sum(g => g.SoTienDVXetNghiem),
                     SoTienDVTDCNNoiSoiTMHs = d.Sum(g => g.SoTienDVTDCNNoiSoiTMH),
                     SoTienDVThamDoChucNangNoiSois = d.Sum(g => g.SoTienDVThamDoChucNangNoiSoi),
                     SoTienDVCDHASieuAms = d.Sum(g => g.SoTienDVCDHASieuAm),
                     SoTienDVCDHAXQuangThuongXQuangSoHoas = d.Sum(g => g.SoTienDVCDHAXQuangThuongXQuangSoHoa),
                     SoTienDVCTScans = d.Sum(g => g.SoTienDVCTScan),
                     SoTienDVMRIs = d.Sum(g => g.SoTienDVMRI),
                     SoTienDVDienTimDienNaos = d.Sum(g => g.SoTienDVDienTimDienNao),
                     SoTienDVTDCNDoLoangXuongs = d.Sum(g => g.SoTienDVTDCNDoLoangXuong),
                     SoTienDVKhacs = d.Sum(g => g.SoTienDVKhac),
                     TongCongs = d.Sum(g => g.TongCong),

                     SoTienDVKhamBenhNGs = d.Sum(g => g.SoTienDVKhamBenhNG),
                     SoTienDVXetNghiemNGs = d.Sum(g => g.SoTienDVXetNghiemNG),
                     SoTienDVTDCNNoiSoiTMHNGs = d.Sum(g => g.SoTienDVTDCNNoiSoiTMHNG),
                     SoTienDVThamDoChucNangNoiSoiNGs = d.Sum(g => g.SoTienDVThamDoChucNangNoiSoiNG),
                     SoTienDVCDHASieuAmNGs = d.Sum(g => g.SoTienDVCDHASieuAmNG),
                     SoTienDVCDHAXQuangThuongXQuangSoHoaNGs = d.Sum(g => g.SoTienDVCDHAXQuangThuongXQuangSoHoaNG),
                     SoTienDVCTScanNGs = d.Sum(g => g.SoTienDVCTScanNG),
                     SoTienDVMRINGs = d.Sum(g => g.SoTienDVMRING),
                     SoTienDVDienTimDienNaoNGs = d.Sum(g => g.SoTienDVDienTimDienNaoNG),
                     SoTienDVTDCNDoLoangXuongNGs = d.Sum(g => g.SoTienDVTDCNDoLoangXuongNG),
                     SoTienDVKhacNGs = d.Sum(g => g.SoTienDVKhacNG),
                     SoTienDVPhauThuatNGs = d.Sum(g => g.SoTienDVPhauThuatNG),
                     SoTienDVThuThuatNGs = d.Sum(g => g.SoTienDVThuThuatNG),
                     TongCongNGs = d.Sum(g => g.TongCongNG),
                     HopDongId = d.First().HopDongId,
                     CongTyId = d.First().CongTyId
                 }).ToList();
            return new GridDataSource { Data = list.ToArray(), TotalRowCount = gridData.TotalRowCount };
        }
        [HttpPost("ExportBCDoanhThuKhamDoanTheoNhomDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuKhamDoanTheoNhomDichVu)]
        public async Task<ActionResult> ExportBCDoanhThuKhamDoanTheoNhomDichVu(BaoCaoDoanhThuKhamDoanTheoNhomDichVuQueryInfo bcInfo)
        {
            bcInfo.Skip = 0;
            bcInfo.Take = Int32.MaxValue;
            var baoCao = await _baoCaoKhamDoanHopDongService.BaoCaoDoanhThuKhamDoanTheoNhomDichVu(bcInfo);
            if (baoCao == null)
                return NoContent();
                
            var bytes = _baoCaoKhamDoanHopDongService.ExportBaoCaoDoanhThuKhamDoanTheoNhomDichVu(baoCao.Data.Cast<BaoCaoDoanhThuKhamDoanTheoNhomDichVuGridVo>().ToList(), bcInfo);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDoanhThuKhamDoanTheoNhomDichVu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("GetCongTyDoanhThuKhamDoanTheoDichVu")]
        public async Task<ActionResult> GetCongTyDoanhThuKhamDoanTheoDichVu([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _baoCaoKhamDoanHopDongService.GetCongTy(queryInfo);
            lookup.Insert(0, new LookupItemTemplateVo
            {
                KeyId = 0,
                DisplayName = "Tất cả"
            });
            return Ok(lookup);
        }
        [HttpPost("GetHopDongKhamSucKhoeDoanhThuKhamDoanTheoDichVu")]
        public async Task<ActionResult> GetHopDongKhamSucKhoeDoanhThuKhamDoanTheoDichVu([FromBody]DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false)
        {
            var lookup = await _baoCaoKhamDoanHopDongService.GetHopDongKhamSucKhoe(queryInfo, LaHopDongKetThuc);
            lookup.Insert(0, new LookupItemHopDingKhamSucKhoeTemplateVo
            {
                KeyId = 0,
                DisplayName = "Tất cả"
            });
            return Ok(lookup);
        }
        #endregion
        #region DOANH THU CHIA THEO KHOA PHÒNG 
        [HttpPost("GetDataDoanhThuChiaTheoKhoaPhongForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BCDTChiaTheoPhong)] 
        public async Task<ActionResult> GetDataDoanhThuChiaTheoKhoaPhongForGridAsync([FromBody] BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataDoanhThuChiaTheoKhoaPhongForGridAsync(queryInfo);

            var totalItem = new SumBaoCaoDoanhThuChiaTheoKhoaPhongGridVo
            {
                TotalDoanhThuKhachLe = gridData.Select(o => o.DoanhThuKhachLe.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuDoan = gridData.Select(o => o.DoanhThuDoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuBaoHiemYTe = gridData.Select(o => o.DoanhThuBaoHiemYTe.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuCoDinh = gridData.Select(o => o.DoanhThuCoDinh.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuThuocVaVTYT = gridData.Select(o => o.DoanhThuThuocVaVTYT.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuSuDungPhongMo = gridData.Select(o => o.DoanhThuSuDungPhongMo.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuLuongBacSiPartime = gridData.Select(o => o.DoanhThuLuongBacSiPartime.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuTienDien = gridData.Select(o => o.DoanhThuTienDien.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuSuatAn = gridData.Select(o => o.DoanhThuSuatAn.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuTongCong = gridData.Select(o => o.DoanhThuTongCong.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
            };

            return Ok(new { Data = gridData, TotalRow = totalItem });
        }

        [HttpPost("GetTotalDoanhThuChiaTheoKhoaPhongForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BCDTChiaTheoPhong)]
        public async Task<ActionResult<GridItem>> GetTotalDoanhThuChiaTheoKhoaPhongForGridAsync([FromBody] BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo queryInfo)
        {
            var data = await _baoCaoService.GetTotalBaoCaoDoanhThuChiaTheoKhoaPhongForGridAsync(queryInfo);
            return Ok(data);
        }

        [HttpPost("ExportDoanhThuChiaTheoKhoaPhong")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BCDTChiaTheoPhong)]
        public async Task<ActionResult> ExportDoanhThuChiaTheoKhoaPhong([FromBody]BaoCaoDoanhThuChiaTheoKhoaPhongQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataDoanhThuChiaTheoKhoaPhongForGridAsync(queryInfo);            

            var total = new SumBaoCaoDoanhThuChiaTheoKhoaPhongGridVo
            {
                TotalDoanhThuKhachLe = gridData.Select(o => o.DoanhThuKhachLe.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuDoan = gridData.Select(o => o.DoanhThuDoan.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuBaoHiemYTe = gridData.Select(o => o.DoanhThuBaoHiemYTe.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuCoDinh = gridData.Select(o => o.DoanhThuCoDinh.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuThuocVaVTYT = gridData.Select(o => o.DoanhThuThuocVaVTYT.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuSuDungPhongMo = gridData.Select(o => o.DoanhThuSuDungPhongMo.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuLuongBacSiPartime = gridData.Select(o => o.DoanhThuLuongBacSiPartime.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuTienDien = gridData.Select(o => o.DoanhThuTienDien.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuSuatAn = gridData.Select(o => o.DoanhThuSuatAn.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuTongCong = gridData.Select(o => o.DoanhThuTongCong.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
            };

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportDoanhThuChiaTheoKhoaPhong(gridData, queryInfo, total);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDoanhThuChiaTheoKhoaPhong" + DateTime.Now.Year + ".xls");

            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
        #region BÁO CÁO TỔNG HỢP DOANH THU THEO NGUỒN BỆNH NHÂN
        [HttpPost("GetDataTongHopDoanhThuTheoNguonBenhNhanForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BCDTTongHopDoanhThuTheoNguonBenhNhan)]
        public async Task<ActionResult> GetDataTongHopDoanhThuTheoNguonBenhNhanForGridAsync([FromBody] BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo queryInfo)
        {
            var gridData = await _baoCaoService.GetDataTongHopDoanhThuTheoNguonBenhNhanForGridAsync(queryInfo);

            var allDatas = gridData.Data.Cast<BaoCaoTongHopDoanhThuTheoNguonBenhNhanGridVo>().ToList();

            var totalItem =  new SumTongHopDoanhThuTheoNguonBenhNhanGridVo
            {
                TotalSoLuongKhachHang = allDatas.Select(o => o.SoLuongKhachHang).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuTheoGiaNiemYet = allDatas.Select(o => o.DoanhThuTheoGiaNiemYet.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalMienGiam = allDatas.Select(o => o.MienGiam.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalBaoHiemChiTra = allDatas.Select(o => o.BaoHiemChiTra.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
                TotalDoanhThuThucTeDuocThuTuKhachHang = allDatas.Select(o => o.DoanhThuThucTeDuocThuTuKhachHang.GetValueOrDefault()).DefaultIfEmpty(0).Sum(),
            };

            return Ok(new { Data = gridData.Data, TotalRowCount = gridData.TotalRowCount, TotalRow = totalItem });
        }


        [HttpPost("GetTotalBaoCaoDoanhThuongHopDoanhThuTheoNguonBenhNhanForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BCDTTongHopDoanhThuTheoNguonBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalBaoCaoDoanhThuongHopDoanhThuTheoNguonBenhNhanForGridAsync([FromBody] BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo queryInfo)
        {
            var data = await _baoCaoService.GetTotalBaoCaoDoanhThuongHopDoanhThuTheoNguonBenhNhanForGridAsync(queryInfo);
            return Ok(data);
        }

        [HttpPost("ExportDoanhThuongHopDoanhThuTheoNguonBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BCDTTongHopDoanhThuTheoNguonBenhNhan)]
        public async Task<ActionResult> ExportDoanhThuongHopDoanhThuTheoNguonBenhNhan([FromBody]BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo queryInfo)
        {
            // all bỏ  phân trang skip take
            queryInfo.Skip = 0;
            queryInfo.Take = int.MaxValue;
            var gridData = await _baoCaoService.GetDataTongHopDoanhThuTheoNguonBenhNhanForGridAsync(queryInfo);
            //var total = await _baoCaoService.GetTotalBaoCaoDoanhThuongHopDoanhThuTheoNguonBenhNhanForGridAsync(queryInfo);

            var allDatas = gridData.Data.Cast<BaoCaoTongHopDoanhThuTheoNguonBenhNhanGridVo>().ToList();

            byte[] bytes = null;
            if (gridData != null)
            {
                bytes = _baoCaoService.ExportDoanhThuongHopDoanhThuTheoNguonBenhNhan(allDatas, queryInfo);
            }
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDoanhThuongHopDoanhThuTheoNguonBenhNhan" + DateTime.Now.Year + ".xls");

            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
