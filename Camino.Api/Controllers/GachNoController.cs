using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.GachNo;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.GachNos;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.GachNos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.GachNo;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class GachNoController : CaminoBaseController
    {
        private IGachNoService _gachNoService;
        private ILocalizationService _iLocalizationService;
        private IUserAgentHelper _iAgentHelper;
        private IExcelService _iExcelService;
        public GachNoController(IGachNoService gachNoService,
            ILocalizationService iLocalizationService,
            IUserAgentHelper iAgentHelper,
            IExcelService iExcelService)
        {
            _gachNoService = gachNoService;
            _iLocalizationService = iLocalizationService;
            _iAgentHelper = iAgentHelper;
            _iExcelService = iExcelService;
        }

        #region grid
        [HttpPost("GetDataGachNoForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<GridDataSource>> GetDataGachNoForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _gachNoService.GetDataGachNoForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageGachNoForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageGachNoForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _gachNoService.GetTotalPageGachNoForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataCongTyBaoHiemTuNhanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<GridDataSource>> GetDataCongTyBaoHiemTuNhanForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _gachNoService.GetDataCongTyBaoHiemTuNhanForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageCongTyBaoHiemTuNhanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageCongTyBaoHiemTuNhanForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _gachNoService.GetTotalPageCongTyBaoHiemTuNhanForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataBenhNhanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<GridDataSource>> GetDataBenhNhanForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _gachNoService.GetDataBenhNhanForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageBenhNhanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageBenhNhanForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _gachNoService.GetTotalPageBenhNhanForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region get list
        [HttpPost("GetListLoaiThuChi")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<List<LookupItemCauHinhVo>>> GetListLoaiThuChiAsync([FromBody]DropDownListRequestModel model)
        {
            var listLoaiTuChi = await _gachNoService.GetListLoaiThuChiAsync(model);
            return Ok(listLoaiTuChi);
        }

        [HttpPost("GetListLoaiTienTe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<List<LookupLoaiTienTeItemVo>>> GetListLoaiTienTeAsync([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _gachNoService.GetListLoaiTienTeAsync(model);
            return Ok(listIcd);
        }

        [HttpPost("GetListDoiTuong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<List<LookupItemVo>>> GetListDoiTuongAsync([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _gachNoService.GetListDoiTuongAsync(model);
            return Ok(listIcd);
        }

        [HttpPost("GetListMaCongTyBaoHiemTuNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<List<LookupCongTyBHTNItemVo>>> GetListMaCongTyBaoHiemTuNhanAsync([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _gachNoService.GetListMaCongTyBaoHiemTuNhanAsync(model);
            return Ok(listIcd);
        }

        [HttpPost("GetListMaBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<List<LookupBenhNhanItemVo>>> GetListMaBenhNhanAsync([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _gachNoService.GetListMaBenhNhanAsync(model);
            return Ok(listIcd);
        }

        [HttpPost("GetListTaiKhoan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<List<LookupItemCauHinhVo>>> GetListTaiKhoanAsync([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _gachNoService.GetListTaiKhoanAsync(model);
            return Ok(listIcd);
        }

        [HttpPost("GetListSoTaiKhoanNganHang")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<List<LookupItemCauHinhVo>>> GetListSoTaiKhoanNganHangAsync([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _gachNoService.GetListSoTaiKhoanNganHangAsync(model);
            return Ok(listIcd);
        }

        [HttpPost("GetListLoaiChungTu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult<List<LookupItemVo>>> GetListLoaiChungTuAsync([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _gachNoService.GetListLoaiChungTuAsync(model);
            return Ok(listIcd);
        }

        [HttpPost("GetListBaoHiemTuNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu, Enums.DocumentType.BaoCaoCongNoCongTyBhtn)]
        public async Task<ActionResult<List<LookupItemTemplateVo>>> GetListBaoHiemTuNhanAsync([FromBody]DropDownListRequestModel model)
        {
            var listIcd = await _gachNoService.GetListBaoHiemTuNhanAsync(model);
            return Ok(listIcd);
        }
        #endregion

        #region xử lý thêm/xóa/sửa

        [HttpGet("GetThongTinThuNo")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<GachNoViewModel> GetThongTinThuNoAsync(long id)
        {
            var gachNoView = new GachNoViewModel();
            var kiemTraQuyenXacNhanNhapLieu = await _gachNoService.KiemTraQuyenXacNhanNhapLieuAsync();
            if (id != 0)
            {
                var gachNo =
                    await _gachNoService.GetByIdAsync(id, x => x.Include(a => a.BenhNhan).Include(b => b.CongTyBaoHiemTuNhan));
                gachNoView = gachNo.ToModel<GachNoViewModel>();
                if (gachNoView.TrangThai == Enums.TrangThaiGachNo.XacNhanNhapLieu)
                {
                    gachNoView.IsDisabledView = true;
                }
                if (gachNoView.CongTyBaoHiemTuNhan == null)
                {
                    gachNoView.CongTyBaoHiemTuNhan = new GachNoCongTyBaoHiemTuNhanViewModel();
                }

                if (gachNoView.BenhNhan == null)
                {
                    gachNoView.BenhNhan = new GachNoBenhNhanViewModel();
                }

                if (gachNo.TrangThai == Enums.TrangThaiGachNo.NhapLieu && kiemTraQuyenXacNhanNhapLieu)
                {
                    gachNoView.IsShowXacNhanNhapLieu = true;
                }
            }
            else
            {
                gachNoView.KyKeToan = DateTime.Now.ToString("yyyyMM");

                if (kiemTraQuyenXacNhanNhapLieu)
                {
                    gachNoView.TrangThai = Enums.TrangThaiGachNo.XacNhanNhapLieu;
                }
                else
                {
                    gachNoView.TrangThai = Enums.TrangThaiGachNo.NhapLieu;
                }

                gachNoView.NgayChungTu = gachNoView.NgayThucThu = DateTime.Now.Date;
                gachNoView.LoaiTienTe = Enums.LoaiTienTe.VND;
                gachNoView.TyGia = 1;
                gachNoView.LoaiChungTu = Enums.LoaiChungTu.BaoCoNganHang;

                var taiKhoans = await _gachNoService.GetListTaiKhoanAsync(new DropDownListRequestModel()
                {
                    Take = Int32.MaxValue
                });
                var taiKhoanTienVietNam = taiKhoans.FirstOrDefault(x => x.KeyId == "11201");
                if (taiKhoanTienVietNam != null)
                {
                    gachNoView.TaiKhoan = taiKhoanTienVietNam.KeyId;
                    gachNoView.TaiKhoanLoaiTien = taiKhoanTienVietNam.GhiChu;
                }
            }

            return gachNoView;
        }


        [HttpPost("XuLyTaoThongTinThuNo")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult> XuLyTaoThongTinThuNoAsync([FromBody] GachNoViewModel thuNoViewModel)
        {
            thuNoViewModel.SoChungTu = ResourceHelper.CreateSoChungTuGachNo();
            var gachNo = thuNoViewModel.ToEntity<GachNo>();
            await _gachNoService.AddAsync(gachNo);

            // 01/12/2021: cập nhật bỏ xử lý cập nhật số dư tài khoản người bệnh, chức năng này chỉ dùng để ghi nhận lại thông tin người dùng nhập vào
            // nếu có quyền xác nhận nhập liệu và loại đối tượng là người bệnh
            // thì cập nhật công nợ của người bệnh
            //var kiemTraQuyenXacNhanNhapLieu = await _gachNoService.KiemTraQuyenXacNhanNhapLieuAsync();
            //if (kiemTraQuyenXacNhanNhapLieu && gachNo.BenhNhanId != null)
            //{
            //    await _gachNoService.XuLyCapNhatCongNo(gachNo);
            //}
            return NoContent();
        }

        [HttpPut("XuLyCapNhatThongTinThuNo")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult> Put([FromBody] GachNoViewModel thuNoViewModel)
        {
            var gachNo = await _gachNoService.GetByIdAsync(thuNoViewModel.Id);
            if (gachNo.TrangThai == Enums.TrangThaiGachNo.XacNhanNhapLieu)
            {
                throw new ApiException(_iLocalizationService.GetResource("GachNo.TrangThai.DaXacNhanNhapLieu"));
            }

            thuNoViewModel.ToEntity(gachNo);
            await _gachNoService.UpdateAsync(gachNo);

            // xử lý xác nhận nhập liệu
            //var kiemTraQuyenXacNhanNhapLieu = await _gachNoService.KiemTraQuyenXacNhanNhapLieuAsync();
            //if (thuNoViewModel.IsXacNhanNhapLieu && kiemTraQuyenXacNhanNhapLieu && gachNo.BenhNhanId != null)
            //{
            //    await _gachNoService.XuLyCapNhatCongNo(gachNo);
            //}
            return NoContent();
        }

        [HttpPut("XuLyXacNhanNhapLieu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult> XuLyXacNhanNhapLieuAsync([FromBody] GachNoViewModel thuNoViewModel)
        {
            var gachNo = await _gachNoService.GetByIdAsync(thuNoViewModel.Id);
            if (gachNo.TrangThai == Enums.TrangThaiGachNo.XacNhanNhapLieu)
            {
                throw new ApiException(_iLocalizationService.GetResource("GachNo.TrangThai.DaXacNhanNhapLieu"));
            }

            thuNoViewModel.ToEntity(gachNo);
            gachNo.TrangThai = Enums.TrangThaiGachNo.XacNhanNhapLieu;
            await _gachNoService.UpdateAsync(gachNo);

            // xử lý xác nhận nhập liệu
            //if (gachNo.BenhNhanId != null)
            //{
            //    await _gachNoService.XuLyCapNhatCongNo(gachNo);
            //}
            return NoContent();
        }

        [HttpDelete("XuLyXoaThongTinThuNo")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<ActionResult> XuLyXoaThongTinThuNoAsync(long id)
        {
            var gachNo = await _gachNoService.GetByIdAsync(id);
            var currentUserId = _iAgentHelper.GetCurrentUserId();
            if (currentUserId != gachNo.CreatedById)
            {
                throw new ApiException(_iLocalizationService.GetResource("GachNo.NguoiTao.NotEqual"));
            }
            if (gachNo.TrangThai == Enums.TrangThaiGachNo.XacNhanNhapLieu)
            {
                throw new ApiException(_iLocalizationService.GetResource("GachNo.TrangThai.DaXacNhanNhapLieu"));
            }
            _gachNoService.DeleteById(id);
            return NoContent();
        }

        [HttpGet("GetLichSuGachNo")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CongNoBhtn, Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)]
        public async Task<List<GachNoHistoryVo>> GetLichSuGachNoAsync(long id)
        {
            var historys = await _gachNoService.GetLichSuGachNoAsync(id);
            return historys;
        }
        #endregion

        #region Báo cáo công nợ
        [HttpPost("GetBaoCaoGachNoCongTyBaoHiemTuNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoCongNoCongTyBhtn)]
        public async Task<ActionResult<GridDataSource>> GetBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _gachNoService.GetBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("GetChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoCongNoCongTyBhtn)]
        public async Task<ActionResult<GridDataSource>> GetChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _gachNoService.GetChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoCongNoCongTyBhtn)]
        public async Task<ActionResult<GridItem>> GetTotalChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var data = await _gachNoService.GetTotalChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(queryInfo);
            return Ok(data);
        }


        [HttpPost("ExportBaoCaoCongNoCongTyBaoHiemTuNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoCongNoCongTyBhtn)]
        public async Task<ActionResult> ExportBaoCaoCongNoCongTyBaoHiemTuNhanAsync(QueryInfo queryInfo)
        {
            var baoCaoCongTys = await _gachNoService.GetBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(queryInfo);
            BaoCaoGachNoCongTyBhtnGridVo congNoChung = null;
            if (baoCaoCongTys.DataSource.Data.Count > 2)
            {
                congNoChung = baoCaoCongTys.DataSource.Data.First(x => x.CongTyId == 0);
            }

            var baoCaoDatas = await _gachNoService.GetChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(queryInfo, true);
            if (baoCaoDatas == null || baoCaoDatas.Data.Count == 0)
            {
                return NoContent();
            }

            var bytes = _iExcelService.ExportBaoCaoCongNoCongTyBaoHiemTuNhan(baoCaoDatas.Data, congNoChung, queryInfo.AdditionalSearchString);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoCongNoCongTyBaoHiemTuNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }


        [HttpPost("ExportGachNo")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.CongNoBhtn)]
        public async Task<ActionResult> ExportGachNoAsync(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;

            var gridData = await _gachNoService.GetDataGachNoForGridAsync(queryInfo);
            var chucVuData = gridData.Data.Select(p => (GachNoGridVo)p).ToList();
            var excelData = chucVuData.Map<List<GachNoExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(GachNoExportExcel.SoChungTu), "SỐ CT"));
            lstValueObject.Add((nameof(GachNoExportExcel.TenLoaiDoiTuong), "Loại ĐT"));
            lstValueObject.Add((nameof(GachNoExportExcel.LoaiThuChi), "Loại T/C"));
            lstValueObject.Add((nameof(GachNoExportExcel.NgayChungTuDisplay), "Ngày CT"));
            lstValueObject.Add((nameof(GachNoExportExcel.TaiKhoan), "Tài khoản"));
            lstValueObject.Add((nameof(GachNoExportExcel.DienGiai), "Diễn giải"));
            lstValueObject.Add((nameof(GachNoExportExcel.VAT), "VAT(%)"));
            lstValueObject.Add((nameof(GachNoExportExcel.KhoanMucPhi), "Khoản mục phí"));
            lstValueObject.Add((nameof(GachNoExportExcel.TienHachToan), "Tiền HT"));
            lstValueObject.Add((nameof(GachNoExportExcel.TienThueHachToan), "Tiền thuế HT"));
            lstValueObject.Add((nameof(GachNoExportExcel.TongTienHachToan), "Tổng tiền HT"));
            lstValueObject.Add((nameof(GachNoExportExcel.SoHoaDon), "Số HĐ"));
            lstValueObject.Add((nameof(GachNoExportExcel.NgayHoaDonDisplay), "Ngày HĐ"));
            lstValueObject.Add((nameof(GachNoExportExcel.MaKhachHang), "Mã ĐT"));
            lstValueObject.Add((nameof(GachNoExportExcel.TenKhachHang), "Tên ĐT"));
            lstValueObject.Add((nameof(GachNoExportExcel.MaSoThue), "MST"));
            lstValueObject.Add((nameof(GachNoExportExcel.TenTrangThai), "Trạng thái"));

            var bytes = _iExcelService.ExportManagermentView(excelData, lstValueObject, "Gạch nợ");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GachNo" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}
