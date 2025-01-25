using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KetQuaNhomXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.BenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KetQuaNhomXetNghiem;
using Camino.Services.KhamBenhs;
using Camino.Services.NhomDichVuBenhVien;
using Camino.Services.TaiLieuDinhKem;
using Camino.Services.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models;
using Camino.Api.Models.Error;
using Camino.Api.Models.KetQuaCDHATDCN;
using Camino.Api.Models.XetNghiem;
using Camino.Core.Domain.Entities.KetQuaVaKetLuanMaus;
using Camino.Services.KetQuaVaKetLuanMau;
using Camino.Services.Localization;
using System.Drawing;
using Camino.Services.NhanVien;
using Camino.Services.Users;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Services.TiepNhanBenhNhan;

namespace Camino.Api.Controllers
{
    public class CanLamSangController : CaminoBaseController
    {
        private readonly IYeuCauTiepNhanService _yeuCauTiepNhanService;
        private readonly IBenhVienService _benhVienService;
        private readonly IYeuCauDichVuKyThuatService _yeuCauDichVuKyThuatService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private readonly IExcelService _excelService;
        private readonly INhomDichVuBenhVienService _nhomDichVuBenhVienRepository;
        private readonly IKetQuaNhomXetNghiemService _ketQuaNhomXetNghiemRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IKetQuaVaKetLuanMauService _ketQuaVaKetLuanMauService;
        private readonly ILocalizationService _localizationService;
        private readonly IPdfService _pdfService;
        private readonly INhanVienService _nhanVienService;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;

        public CanLamSangController(IYeuCauTiepNhanService yeuCauTiepNhanService, ITaiLieuDinhKemService taiLieuDinhKemService,
                IUserAgentHelper userAgentHelper,
               INhomDichVuBenhVienService nhomDichVuBenhVienRepository, IKetQuaNhomXetNghiemService ketQuaNhomXetNghiemRepository
            , IBenhVienService benhVienService, IYeuCauDichVuKyThuatService yeuCauDichVuKyThuatService, IExcelService excelService,
                IKetQuaVaKetLuanMauService ketQuaVaKetLuanMauService,
                ILocalizationService localizationService,
                IPdfService pdfService,
                INhanVienService nhanVienService,
                ITiepNhanBenhNhanService tiepNhanBenhNhanService)
        {
            _yeuCauTiepNhanService = yeuCauTiepNhanService;
            _benhVienService = benhVienService;
            _yeuCauDichVuKyThuatService = yeuCauDichVuKyThuatService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _excelService = excelService;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _ketQuaNhomXetNghiemRepository = ketQuaNhomXetNghiemRepository;
            _userAgentHelper = userAgentHelper;
            _ketQuaVaKetLuanMauService = ketQuaVaKetLuanMauService;
            _localizationService = localizationService;
            _pdfService = pdfService;
            _nhanVienService = nhanVienService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
        }

        #region Thông tin kết quả cận lâm sàng

        #region Danh Sách tất cả của cận lâm sàng

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
          ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetCanLamSangDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetCanLamSangTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion        

        #region Câp nhật kết quả cận lâm sàng

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ThongTinThuNganBenhNhanViewModel>> Get(long id)
        {
            var thongTinBenhNhan = await _yeuCauTiepNhanService.GetByIdAsync(id,
                s => s.Include(cc => cc.CongTyUuDai)
                    .Include(o => o.QuanHuyen)
                    .Include(o => o.TinhThanh)
                    .Include(o => o.PhuongXa)
                    .Include(o => o.BenhNhan)
                    .Include(o => o.DoiTuongUuDai)
                    .Include(cc => cc.GiayChuyenVien)
                    .Include(cc => cc.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                    .ThenInclude(cc => cc.CongTyBaoHiemTuNhan)
                    .Include(cc => cc.BHYTGiayMienCungChiTra)
                    .Include(a => a.NoiTruBenhAn)

                    //BVHD-3800
                    .Include(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
                );
            var model = thongTinBenhNhan.ToModel<ThongTinThuNganBenhNhanViewModel>();
            model.DiaChi = thongTinBenhNhan.DiaChiDayDu;
            model.MaBN = thongTinBenhNhan.BenhNhan.MaBN;
            model.SoDienThoai = model.SoDienThoai.ApplyFormatPhone();

            model.SoBenhAn = thongTinBenhNhan.NoiTruBenhAn != null ? thongTinBenhNhan.NoiTruBenhAn.SoBenhAn : null;

            //BVHD-3800
            model.LaCapCuu = thongTinBenhNhan.LaCapCuu ?? thongTinBenhNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;

            #region BVHD-3941
            if (thongTinBenhNhan.CoBHTN == true)
            {
                model.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(thongTinBenhNhan.Id).Result;
            }
            #endregion

            return Ok(model);
        }

        [HttpPost("TimKiemThongTinCLS")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<KetQuaCLS> TimKiemThongTinCLS(TimKiemThongTinBenhNhan timKiemThongTinBenhNhan)
        {
            var ketQuaCLS = _yeuCauTiepNhanService.GetCanLamSangIdByMaBNVaMaTT(timKiemThongTinBenhNhan);
            return Ok(ketQuaCLS);
        }

        [HttpGet("GetThongTinCanLamSang/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<DanhSachCanLamSangVo> GetThongTinCanLamSang(long id)
        {
            var danhSachCanLamSangs = _yeuCauTiepNhanService.GetThongTinCanLamSang(id);
            return Ok(danhSachCanLamSangs);
        }

        [HttpPost("CapNhatKetQuaCanLamSang")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CanLamSang)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<bool> CapNhatKetQuaCanLamSang([FromBody]DanhSachCanLamSangVo thongTinCanLamSangs)
        {
            var nhanVienketLuan = _userAgentHelper.GetCurrentUserId();

            if (thongTinCanLamSangs != null)
            {
                //Trường hợp này dùng cho thăm dò chức năng và chuẩn đoán hình ảnh.
                if (thongTinCanLamSangs.DanhSachCanLamSangs.Any())
                {
                    for (int i = 0; i < thongTinCanLamSangs.DanhSachCanLamSangs.Count; i++)
                    {

                        var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(thongTinCanLamSangs.DanhSachCanLamSangs[i].DichVuKyhuatId, c => c.Include(cc => cc.FileKetQuaCanLamSangs));
                        if (yeuCauDichVuKyThuat != null && thongTinCanLamSangs.DanhSachCanLamSangs[i].GiayKetQuaLamSang != null ||
                            !String.IsNullOrEmpty(thongTinCanLamSangs.DanhSachCanLamSangs[i].GhiChu))
                        {
                            yeuCauDichVuKyThuat.KetLuan = thongTinCanLamSangs.DanhSachCanLamSangs[i].GhiChu;
                            yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;

                            yeuCauDichVuKyThuat.NhanVienKetLuanId = nhanVienketLuan;
                            yeuCauDichVuKyThuat.NhanVienThucHienId = thongTinCanLamSangs.DanhSachCanLamSangs[i].NhanVienThucHienId;

                            //xóa hinh củ update hinh mới vào
                            _yeuCauTiepNhanService.DeleteFileKetQuaCanLamSang(yeuCauDichVuKyThuat.FileKetQuaCanLamSangs.ToList());

                            if (yeuCauDichVuKyThuat != null && thongTinCanLamSangs.DanhSachCanLamSangs[i].GiayKetQuaLamSang != null)
                            {
                                //todo need update FileKetQuaCanLamSangs
                                foreach (var giayKetQua in thongTinCanLamSangs.DanhSachCanLamSangs[i].GiayKetQuaLamSang)
                                {
                                    if (giayKetQua != null)
                                    {
                                        yeuCauDichVuKyThuat.FileKetQuaCanLamSangs.Add(new Core.Domain.Entities.FileKetQuaCanLamSangs.FileKetQuaCanLamSang
                                        {
                                            Ma = Guid.NewGuid().ToString(),
                                            Ten = giayKetQua.Ten,
                                            TenGuid = giayKetQua.TenGuid,
                                            DuongDan = giayKetQua.DuongDan,
                                            KichThuoc = giayKetQua.KichThuoc,
                                            LoaiTapTin = (Enums.LoaiTapTin)giayKetQua.LoaiTapTin,
                                        });
                                        _taiLieuDinhKemService.LuuTaiLieuAsync(giayKetQua.DuongDan, giayKetQua.TenGuid);
                                    }
                                }
                            }
                            _yeuCauDichVuKyThuatService.Update(yeuCauDichVuKyThuat);
                        }
                    }

                }

                //Trường hợp này dùng cho nhóm xét nghiệm
                if (thongTinCanLamSangs.KetQuaNhomXetNghiems.Any())
                {

                    for (int i = 0; i < thongTinCanLamSangs.KetQuaNhomXetNghiems.Count; i++)
                    {
                        if (thongTinCanLamSangs.KetQuaNhomXetNghiems[i].NhomDanhSachXetNghiem.Any())
                        {
                            //xóa file nhóm cận lâm sàng
                            _ketQuaNhomXetNghiemRepository.DeleteFileNhomXetNghiems(thongTinCanLamSangs.YeuCauTiepNhanId, thongTinCanLamSangs.KetQuaNhomXetNghiems[i].NhomDichVuKyThuatId);

                            var ketQuaNhomXetNghiem = new KetQuaNhomXetNghiem();
                            ketQuaNhomXetNghiem.KetLuan = thongTinCanLamSangs.KetQuaNhomXetNghiems[i].KetLuan;
                            ketQuaNhomXetNghiem.NhomDichVuBenhVienId = thongTinCanLamSangs.KetQuaNhomXetNghiems[i].NhomDichVuKyThuatId;
                            ketQuaNhomXetNghiem.YeuCauTiepNhanId = thongTinCanLamSangs.YeuCauTiepNhanId;
                            ketQuaNhomXetNghiem.NhanVienKetLuanId = nhanVienketLuan;
                            ketQuaNhomXetNghiem.ThoiDiemKetLuan = DateTime.Now;

                            //todo need update FileKetQuaCanLamSangs
                            if (thongTinCanLamSangs.KetQuaNhomXetNghiems[i].GiayKetQuaNhomCanLamSang != null)
                            {
                                foreach (var item in thongTinCanLamSangs.KetQuaNhomXetNghiems[i].GiayKetQuaNhomCanLamSang)
                                {
                                    ketQuaNhomXetNghiem.FileKetQuaCanLamSangs.Add(new Core.Domain.Entities.FileKetQuaCanLamSangs.FileKetQuaCanLamSang
                                    {
                                        Ma = Guid.NewGuid().ToString(),
                                        Ten = item.Ten,
                                        TenGuid = item.TenGuid,
                                        DuongDan = item.DuongDan,
                                        KichThuoc = (long)item.KichThuoc,
                                        LoaiTapTin = (Enums.LoaiTapTin)item.LoaiTapTin,
                                    });
                                    _taiLieuDinhKemService.LuuTaiLieuAsync(item.DuongDan, item.TenGuid);
                                }
                            }
                            _ketQuaNhomXetNghiemRepository.Add(ketQuaNhomXetNghiem);

                        }

                        if (thongTinCanLamSangs.KetQuaNhomXetNghiems[i].GiayKetQuaNhomCanLamSang != null || !String.IsNullOrEmpty(thongTinCanLamSangs.KetQuaNhomXetNghiems[i].KetLuan))
                        {
                            for (int j = 0; j < thongTinCanLamSangs.KetQuaNhomXetNghiems[i].NhomDanhSachXetNghiem.Count; j++)
                            {
                                var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(thongTinCanLamSangs.KetQuaNhomXetNghiems[i].NhomDanhSachXetNghiem[j].DichVuId);
                                yeuCauDichVuKyThuat.KetLuan = thongTinCanLamSangs.KetQuaNhomXetNghiems[i].KetLuan;
                                yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;

                                yeuCauDichVuKyThuat.NhanVienKetLuanId = nhanVienketLuan;
                                yeuCauDichVuKyThuat.NhanVienThucHienId = thongTinCanLamSangs.KetQuaNhomXetNghiems[i].NhanVienThucHienId;

                                _yeuCauDichVuKyThuatService.Update(yeuCauDichVuKyThuat);
                            }

                        }
                    }
                }

                return true;
            }
            return false;

        }

        #endregion

        #region Danh Sách tất cả danh sách lịch sử cận lâm sàng

        [HttpGet("GetThongTinKLichSuCanLamSang/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuCanLamSang)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<DanhSachCanLamSangVo> GetThongTinKLichSuCanLamSang(long id)
        {
            var danhSachCanLamSangs = _yeuCauTiepNhanService.GetThongTinKLichSuCanLamSang(id);
            return Ok(danhSachCanLamSangs);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataLichSuCanLamSangForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuCanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetDataLichSuCanLamSangForGridAsync
          ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetLichSuCanLamSangDaCoKetQuaDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalLichSuCanLamSangPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuCanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetTotalLichSuCanLamSangPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetLichSuCanLamSangCanLamSangDaCoKetQuaTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataChiTietLichSuCanLamSangForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuCanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetDataChiTietLichSuCanLamSangForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetDataChiTietLichSuCanLamSangForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalChiTietLichSuCanLamSangPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.LichSuCanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetTotalChiTietLichSuCanLamSangPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetTotalChiTietLichSuCanLamSangPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Danh sách nội dung mẫu
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNoiDungMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNoiDungMauAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetDataForGridNoiDungMauAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridNoiDungMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNoiDungMauAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauTiepNhanService.GetTotalPageForGridNoiDungMauAsync(queryInfo);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpPost("GetDataForGridKetLuanMau")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        //public async Task<ActionResult<GridDataSource>> GetDataForGridKetLuanMauAsync
        //    ([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = await _yeuCauTiepNhanService.GetDataForGridNoiDungMauAsync(queryInfo);
        //    return Ok(gridData);
        //}

        //[HttpPost("GetTotalPageForGridKetLuanMau")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        //public async Task<ActionResult<GridDataSource>> GetTotalPageForGridKetLuanMauAsync
        //    ([FromBody]QueryInfo queryInfo)
        //{
        //    var gridData = await _yeuCauTiepNhanService.GetTotalPageForGridNoiDungMauAsync(queryInfo);
        //    return Ok(gridData);
        //}
        #endregion

        #region Xuất Excel Cho Cận Lâm Sàng

        [HttpPost("ExportCanLamSang")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult> ExportCanLamSang([FromBody]QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;

            var gridData = await _yeuCauTiepNhanService.GetCanLamSangDataForGridAsync(queryInfo);
            var danhSachCanLamSangs = gridData.Data.Select(p => (KetQuaCDHATDCNTimKiemGridVo)p).ToList();
            var dataExcel = danhSachCanLamSangs.Map<List<CanLamSangExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(CanLamSangExportExcel.MaYeuCauTiepNhan), "Mã TN"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.HoTen), "Họ Tên"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.GioiTinh), "Giới Tính"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.NgayThangNam), "Năm Sinh"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.ChiDinh), "Chỉ Định"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.ChuanDoanDisplay), "Chẩn Đoán"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.BacSiCDDisplay), "Bác Sĩ CĐ"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.KetLuanDisplay), "Kết Luận"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.BacSiKetLuanDisplay), "Bác Sĩ KL"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.KyThuatVien1Display), "KTV1"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.DoiTuong), "Đối Tượng"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.TenTrangThai), "Trạng Thái"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.NgayChiDinhDisplay), "Ngày Chỉ Định"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.NgayThucHienDisplay), "Ngày Thực Hiện"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.NoiChiDinh), "Nơi Chỉ Định"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.MaBN), "Mã NB"));
            lstValueObject.Add((nameof(CanLamSangExportExcel.SoBenhAn), "Số BA"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Kết quả CĐHA, TDCN", 2, "Kết quả CĐHA, TDCN");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KetQuaCDHATDCN" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportLichSuCanLamSang")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuCanLamSang)]
        public async Task<ActionResult> ExportLichSuCanLamSang([FromBody]QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            GridDataSource gridData = await _yeuCauTiepNhanService.GetLichSuCanLamSangDaCoKetQuaDataForGridAsync(queryInfo);
            var dsLSCanLamSangs = gridData.Data.Select(p => (KetQuaCDHATDCNLichSuGridVo)p).ToList();
            var dataExcel = dsLSCanLamSangs.Map<List<LichSuCanLamSangExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(LichSuCanLamSangExportExcel.MaYeuCauTiepNhan), "Mã TN"));
            lstValueObject.Add((nameof(LichSuCanLamSangExportExcel.SoBenhAn), "Số BA"));
            lstValueObject.Add((nameof(LichSuCanLamSangExportExcel.MaBN), "Mã BN"));
            lstValueObject.Add((nameof(LichSuCanLamSangExportExcel.HoTen), "Họ Tên"));
            lstValueObject.Add((nameof(LichSuCanLamSangExportExcel.GioiTinh), "Giới Tính"));
            lstValueObject.Add((nameof(LichSuCanLamSangExportExcel.DiaChi), "Địa Chỉ"));
            lstValueObject.Add((nameof(LichSuCanLamSangExportExcel.SoDienThoai), "Số Điện Thoại"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Lịch Sử Kết Quả CĐHA, TDCN", 2, "Lịch Sử Kết Quả CĐHA, TDCN");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuKetQuaCDHATDCN" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #endregion

        #region lookup
        [HttpPost("GetListNoiDungMau")]
        public async Task<ActionResult<ICollection<NoiDungMauLookupItemVo>>> GetListNoiDungMauAsync(DropDownListRequestModel model)
        {
            var lookup = await _ketQuaVaKetLuanMauService.GetListNoiDungMauAsync(model);
            return Ok(lookup);
        }

        //[HttpPost("GetListKetLuanMau")]
        //public async Task<ActionResult<ICollection<NoiDungMauLookupItemVo>>> GetListKetLuanMauAsync(DropDownListRequestModel model)
        //{
        //    var lookup = await _ketQuaVaKetLuanMauService.GetListNoiDungMauAsync(model);
        //    return Ok(lookup);
        //}

        [HttpPost("GetListKyThuatDichVuKyThuat")]
        public async Task<ActionResult<string>> GetListKyThuatDichVuKyThuatAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauTiepNhanService.GetListKyThuatDichVuKyThuatAsync(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetListKyThuatDichVuKyThuatTheoTiepNhan")]
        public async Task<ActionResult<YeuCauKyThuatCDHALookupItemVo>> GetListKyThuatDichVuKyThuatTheoTiepNhan([FromBody]DropDownListRequestModel queryInfo)
        {
            var ketQuaCDHATDCNViewModel = JsonConvert.DeserializeObject<KetQuaCDHATDCNViewModel>(queryInfo.ParameterDependencies);
            var lookup = await _yeuCauTiepNhanService.GetListKyThuatDichVuKyThuatTheoTiepNhan(queryInfo, ketQuaCDHATDCNViewModel.YeuCauTiepNhanId);
            return Ok((bool)ketQuaCDHATDCNViewModel.ChoKetQua ? lookup.OrderByDescending(c => c.TrangThaiDangThucHien) : lookup.OrderBy(c => c.TrangThaiDangThucHien));
        }

        [HttpGet("TrangThaiYeuCauDichVuKyThuat")]
        public async Task<ActionResult<TrangThaiYeuCauKyThuat>> TrangThaiYeuCauDichVuKyThuat(long yeuCauDichVuKyThuatId)
        {
            var trangThaiYeuCauDVkyThuat = _yeuCauTiepNhanService.TrangThaiYeuCauDichVuKyThuat(yeuCauDichVuKyThuatId);
            return Ok(trangThaiYeuCauDVkyThuat);
        }

        #endregion

        #region thêm/xóa/sửa kết quả và kết luận mẫu

        [HttpGet("GetThongTinNoiDungMau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<KetQuaVaKetLuanMauViewModel>> GetThongTinNoiDungMauAsync(long id)
        {
            var result = await _ketQuaVaKetLuanMauService.GetByIdAsync(id);
            return result.ToModel<KetQuaVaKetLuanMauViewModel>();
        }

        [HttpPost("LuuThongTinNoiDungMau")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult> LuuThongTinNoiDungMauAsync([FromBody] KetQuaVaKetLuanMauViewModel viewModel)
        {
            if (viewModel.Id != 0)
            {
                var noiDungMau = await _ketQuaVaKetLuanMauService.GetByIdAsync(viewModel.Id);
                viewModel.ToEntity(noiDungMau);
                await _ketQuaVaKetLuanMauService.UpdateAsync(noiDungMau);
            }
            else
            {
                var noiDungMau = viewModel.ToEntity<KetQuaVaKetLuanMau>();
                await _ketQuaVaKetLuanMauService.AddAsync(noiDungMau);
            }

            return Ok();
        }

        [HttpDelete("XoaThongTinNoiDungMau")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<KetQuaVaKetLuanMauViewModel>> XoaThongTinNoiDungMauAsync(long id)
        {
            var result = await _ketQuaVaKetLuanMauService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            await _ketQuaVaKetLuanMauService.DeleteByIdAsync(id);
            return Ok();
        }

        #endregion

        #region thêm/xóa/sửa kết quả

        [HttpGet("GetThongTinChungBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public ActionResult<KetQuaCDHATDCNViewModel> GetThongTinChungBenhNhan(long id)
        {
            #region Cập nhật 27/12/2022
            //var result = _yeuCauTiepNhanService.GetById(id, x => x.Include(z => z.NoiTruBenhAn)
            //                                                      .Include(z => z.BenhNhan)
            //                                                .Include(y => y.YeuCauDichVuKyThuats).ThenInclude(y => y.NhanVienChiDinh).ThenInclude(z => z.User)
            //                                                .Include(y => y.YeuCauDichVuKyThuats).ThenInclude(y => y.NoiChiDinh)
            //                                                .Include(y => y.YeuCauDichVuKyThuats).ThenInclude(y => y.YeuCauKhamBenh).ThenInclude(z => z.Icdchinh)
            //                                                .Include(y => y.YeuCauDichVuKyThuats).ThenInclude(y => y.NoiTruPhieuDieuTri).ThenInclude(z => z.ChanDoanChinhICD)
            //                                                .Include(y => y.YeuCauDichVuKyThuats).ThenInclude(y => y.NhomDichVuBenhVien)
            //                                                .Include(y => y.YeuCauDichVuKyThuats).ThenInclude(y => y.FileKetQuaCanLamSangs)
            //                                                .Include(y => y.YeuCauDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien));
            var result = _yeuCauTiepNhanService.GetThongTinChungBenhNhan(id);
            #endregion

            var viewModel = result.ToModel<KetQuaCDHATDCNViewModel>();
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var dsYCDVKT = result.YeuCauDichVuKyThuats.Where(c => c.NoiThucHienId == noiThucHienId && (c.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || c.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh));

            viewModel.ThongTinHanhChinhBacSiChiDinh = String.Join("; ", dsYCDVKT.Select(c => c.NhanVienChiDinh.User.HoTen).Distinct().ToArray());
            viewModel.ThongTinHanhChinhNgayChiDinh = String.Join("; ", dsYCDVKT.Select(c => c.ThoiDiemChiDinh.ApplyFormatDateTimeSACH()).Distinct().ToArray());
            viewModel.ThongTinHanhChinhNoiChiDinh = String.Join("; ", dsYCDVKT.Select(c => c.NoiChiDinh.Ten + (!string.IsNullOrEmpty(c.NoiChiDinh.Tang) ? "(" + c.NoiChiDinh.Tang + ")" : "")).Distinct().ToArray());

            viewModel.ThongTinHanhChinhChanDoan = dsYCDVKT.Any(c => c.NoiTruPhieuDieuTri != null) ?
                String.Join(";", dsYCDVKT.Select(c => c.NoiTruPhieuDieuTri.ChanDoanChinhICD?.TenTiengViet).Distinct().ToArray()) :
                String.Join(";", dsYCDVKT.Select(c => c.YeuCauKhamBenh?.Icdchinh?.TenTiengViet).Distinct().ToArray());
            //viewModel.ThongTinHanhChinhChiDinh = String.Join("; ", dsYCDVKT.Select(c => c.TenDichVu).Distinct().ToArray());

            #region BVHD-3941
            if (result.CoBHTN == true)
            {
                viewModel.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(result.Id).Result;
            }
            #endregion

            return viewModel;
        }

        [HttpGet("GetThongTinKetQua")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public ActionResult<KetQuaCDHATDCNViewModel> GetThongTinKetQuaAsync(long id)
        {
            var result = _yeuCauDichVuKyThuatService.GetById(id
                , x => x.Include(y => y.YeuCauTiepNhan).ThenInclude(z => z.NoiTruBenhAn)
                                                                                         .Include(y => y.YeuCauTiepNhan).ThenInclude(z => z.BenhNhan)
                                                                                         .Include(y => y.NhanVienChiDinh).ThenInclude(z => z.User)
                                                                                         .Include(y => y.NoiChiDinh)
                                                                                         .Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.Icdchinh)
                                                                                         .Include(y => y.NoiTruPhieuDieuTri).ThenInclude(z => z.ChanDoanChinhICD)
                                                                                         .Include(y => y.NhomDichVuBenhVien)
                                                                                         .Include(y => y.FileKetQuaCanLamSangs)
                                                                                         .Include(y => y.DichVuKyThuatBenhVien).ThenInclude(c => c.DichVukyThuatBenhVienMauKetQua)
                 //BVHD-3800
                 .Include(y => y.YeuCauTiepNhan).ThenInclude(y => y.YeuCauNhapVien).ThenInclude(y => y.YeuCauKhamBenh).ThenInclude(y => y.YeuCauTiepNhan));

            var viewModel = result.ToModel<KetQuaCDHATDCNViewModel>();
            viewModel.ChiTietKetQuaObj.InKemAnh = viewModel.ChiTietKetQuaObj.InKemAnh || viewModel.DichVuCoInKetQuaKemHinhAnh;

            var userId = _userAgentHelper.GetCurrentUserId();
            var nhanVienDangLogin = _nhanVienService.GetById(userId, x => x.Include(y => y.ChucDanh).Include(y => y.User));

            if (viewModel.NhanVienThucHienId == null || viewModel.NhanVienKetLuanId == null)
            {
                var laBacSi = nhanVienDangLogin?.ChucDanh?.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSi || nhanVienDangLogin?.ChucDanh?.NhomChucDanhId == (int)Enums.EnumNhomChucDanh.BacSiDuPhong;
                if (laBacSi)
                {
                    viewModel.NhanVienKetLuanId = viewModel.NhanVienKetLuanId ?? userId;
                }
                else
                {
                    viewModel.NhanVienThucHienId = viewModel.NhanVienThucHienId ?? userId;
                }
            }

            viewModel.NguoiLuuId = userId;
            viewModel.NguoiLuuTen = nhanVienDangLogin.User.HoTen;

            //viewModel.NhanVienThucHienId = viewModel.NhanVienThucHienId ?? _userAgentHelper.GetCurrentUserId();
            viewModel.YeuCauTiepNhanId = result.YeuCauTiepNhanId;

            if (string.IsNullOrEmpty(viewModel.ChiTietKetQuaObj.TenKetQua))
            {
                viewModel.ThoiDiemThucHien = result.ThoiDiemThucHien ?? DateTime.Now;

                viewModel.ChiTietKetQuaObj.TenKetQua = result.NhomDichVuBenhVien.Ten != null ? result.NhomDichVuBenhVien.Ten : result.DichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua?.TenKetQuaMau;
                viewModel.ChiTietKetQuaObj.KetQua = viewModel.ChiTietKetQuaObj.KetQua != null ? viewModel.ChiTietKetQuaObj.KetQua : result.DichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua?.KetQua;
                viewModel.ChiTietKetQuaObj.KetLuan = viewModel.ChiTietKetQuaObj.KetLuan != null ? viewModel.ChiTietKetQuaObj.KetLuan : result.DichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua?.KetLuan;

                viewModel.ChiTietKetQuaObj.KyThuat = result.DichVuKyThuatBenhVien.TenKyThuat;
            }
            viewModel.DichVuKyThuatBenhVienId = result.DichVuKyThuatBenhVienId;

            //BVHD-3800
            viewModel.LaCapCuu = result.YeuCauTiepNhan.LaCapCuu ?? result.YeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu;
            return viewModel;
        }


        [HttpPut("KiemTraLuuNoiDungKetQua")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CanLamSang)]
        public ActionResult KiemTraLuuNoiDungKetQuaAsync([FromBody]KetQuaCDHATDCNViewModel viewModel)
        {
            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(viewModel.Id, x => x.Include(y => y.FileKetQuaCanLamSangs)
                                                                                                               .Include(y => y.YeuCauTiepNhan)
                                                                                                               .Include(y => y.DichVuKyThuatBenhVien));

            //if (yeuCauDichVuKyThuat.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat) return Ok();
            //if (yeuCauDichVuKyThuat.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
            //{
            //    throw new ApiException(_localizationService.GetResource("KetQuaVaKetLuanMau.YeuCauTiepNhan.DaHoanThanh"));
            //}

            if (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            {
                throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiYeuCauDichVuKyThuat.DaHuy"));
            }

            viewModel.ToEntity(yeuCauDichVuKyThuat);
            _yeuCauTiepNhanService.KiemTraLuuNoiDungKetQuaAsync(yeuCauDichVuKyThuat, viewModel.ChiTietKetQuaObj.KyThuat);

            return Ok();
        }
        #endregion

        #region In phiếu

        [HttpPost("XuLyInPhieuKetQua")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.DanhSachDieuTriNoiTru, Enums.DocumentType.LichSuKhamBenh)]
        public ActionResult<string> XuLyInPhieuKetQuaAsync(PhieuInKetQuaInfoVo phieuInKetQuaInfoVo)
        {
            var phieuIn = _yeuCauTiepNhanService.XuLyInPhieuKetQuaAsync(phieuInKetQuaInfoVo);
            return phieuIn;
        }

        //Khách hàng muốn in theo ý mình phiếu này
        [HttpPost("XuLyInPhieuKetQuaTheoYeuCau")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.DanhSachDieuTriNoiTru, Enums.DocumentType.LichSuKhamBenh)]
        public ActionResult<string> XuLyInPhieuKetQuaTheoYeuCau(CauHinhHinhVo cauHinhHinhVo)
        {
            var phieuIn = _yeuCauTiepNhanService.XuLyInPhieuKetQuaTheoYeuCauAsync(cauHinhHinhVo);
            return phieuIn;
        }

        [HttpPost("XuLyInPhieuKetQuaTheoYeuCauMotLan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.DanhSachDieuTriNoiTru, Enums.DocumentType.LichSuKhamBenh)]
        public ActionResult<string> XuLyInPhieuKetQuaTheoYeuCauMotLan(CauHinhHinhVo cauHinhHinhVo)
        {
            var phieuIn = _yeuCauTiepNhanService.XuLyInPhieuKetQuaTheoYeuCauAsync(cauHinhHinhVo);
            var htmlToPdfVo = new HtmlToPdfVo
            {
                Html = phieuIn,
                Zoom=cauHinhHinhVo.Zoom
            };
            var bytes = _pdfService.ExportFilePdfFromHtml(htmlToPdfVo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KetQuaCLS" + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";

            return new FileContentResult(bytes, "application/pdf");
        }

        #endregion

        #region pdf
        [HttpPost("GetFilePDFFromHtml")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang, Enums.DocumentType.KhamBenh, Enums.DocumentType.KhamBenhDangKham, Enums.DocumentType.DanhSachDieuTriNoiTru, Enums.DocumentType.LichSuKhamBenh)]
        public ActionResult GetFilePDFFromHtml(LayMauXetNghiemFileKetQuaViewModel htmlContent)
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
                FooterHtml = footerHtml
            };
            var bytes = _pdfService.ExportFilePdfFromHtml(htmlToPdfVo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
            Response.ContentType = "application/pdf";
            return new FileContentResult(bytes, "application/pdf");
        }
        #endregion

        #region Hủy kết quả CLS

        [HttpPost("HuyKetQuaCDHA")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult> HuyKetQuaCDHA(long id)
        {
            var result = await _yeuCauDichVuKyThuatService.GetByIdAsync(id, x => x.Include(y => y.FileKetQuaCanLamSangs).Include(c => c.YeuCauTiepNhan).ThenInclude(c => c.NoiTruBenhAn));
            if (result == null)
            {
                return NotFound();
            }

            if (result.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
            {
                throw new ApiException(_localizationService.GetResource("CanLamSan.YeuCauTiepNhan.RaVienKhongHuyKetQua"));
            }

            result.DataKetQuaCanLamSang = null;
            result.ThoiDiemThucHien = null;
            result.NhanVienThucHienId = null;
            result.NhanVienKetLuanId = null;
            result.MayTraKetQuaId = null;
            foreach (var item in result.FileKetQuaCanLamSangs)
            {
                item.WillDelete = true;
            }
            result.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            await _yeuCauDichVuKyThuatService.UpdateAsync(result);
            return NoContent();
        }
        #endregion
    }
}