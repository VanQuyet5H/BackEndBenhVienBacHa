using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DuyetPhieuHoanTraThuocTuBns;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetTraVatTuTuBns;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.DuyetPhieuTraVatTuTuBns;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class DuyetTraVatTuTuBnController : CaminoBaseController
    {
        private readonly IDuyetPhieuTraVatTuTuBnService _duyetPhieuTraVatTuTuBenhNhanService;
        private readonly IExcelService _excelService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ILocalizationService _localizationService;

        public DuyetTraVatTuTuBnController(IDuyetPhieuTraVatTuTuBnService duyetPhieuTraVatTuTuBenhNhanService, IExcelService excelService, IUserAgentHelper userAgentHelper, ILocalizationService localizationService)
        {
            _excelService = excelService;
            _duyetPhieuTraVatTuTuBenhNhanService = duyetPhieuTraVatTuTuBenhNhanService;
            _userAgentHelper = userAgentHelper;
            _localizationService = localizationService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duyetPhieuTraVatTuTuBenhNhanService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duyetPhieuTraVatTuTuBenhNhanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncVatTuChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncVatTuChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duyetPhieuTraVatTuTuBenhNhanService.GetDataForGridAsyncVatTuChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncBenhNhanChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraVatTuTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncBenhNhanChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duyetPhieuTraVatTuTuBenhNhanService.GetDataForGridAsyncBenhNhanChild(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraVatTuTuBenhNhan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetPhieuHoanTraThuocTuBnViewModel>> Get(long id)
        {
            var duyetPhieuHoanTraVatTu = await _duyetPhieuTraVatTuTuBenhNhanService.GetByIdAsync(id,
                w => w.Include(q => q.KhoaHoanTra)
                    .Include(q => q.KhoTra)
                    .Include(q => q.NhanVienYeuCau).ThenInclude(q => q.User));

            if (duyetPhieuHoanTraVatTu == null)
            {
                return NotFound();
            }

            var duyetPhieuHoanTraResult = new DuyetPhieuHoanTraThuocTuBnViewModel
            {
                Id = duyetPhieuHoanTraVatTu.Id,
                GhiChu = duyetPhieuHoanTraVatTu.GhiChu,
                SoPhieu = duyetPhieuHoanTraVatTu.SoPhieu,
                KhoaHoanTraId = duyetPhieuHoanTraVatTu.KhoaHoanTraId,
                KhoaHoanTraDisplay = duyetPhieuHoanTraVatTu.KhoaHoanTra.Ten,
                HoanTraVeKhoId = duyetPhieuHoanTraVatTu.KhoTraId,
                HoanTraVeKhoDisplay = duyetPhieuHoanTraVatTu.KhoTra.Ten,
                NgayYeuCau = duyetPhieuHoanTraVatTu.NgayYeuCau,
                TinhTrang = duyetPhieuHoanTraVatTu.DuocDuyet,
                NguoiYeuCauId = duyetPhieuHoanTraVatTu.NhanVienYeuCauId,
                NguoiYeuCauDisplay = duyetPhieuHoanTraVatTu.NhanVienYeuCau.User.HoTen
            };

            return Ok(duyetPhieuHoanTraResult);
        }

        [HttpPost("ExportDuyetPhieuTraVatTuTuBn")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetTraVatTuTuBenhNhan)]
        public async Task<ActionResult> ExportDuyetPhieuTraVatTuTuBn(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = int.MaxValue;

            var gridData = await _duyetPhieuTraVatTuTuBenhNhanService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (DuyetTraVatTuTuBnVo)p).ToList();
            var dataExcel = data.Map<List<DuyetPhieuHoanTraVatTuTuBnExportExcel>>();

            queryInfo.Sort = new List<Sort> { new Sort { Field = "VatTu", Dir = "asc" } };

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id + "";
                var gridChildData = await _duyetPhieuTraVatTuTuBenhNhanService.GetDataForGridAsyncVatTuChild(queryInfo);
                var dataChild = gridChildData.Data.Select(p => (DuyetTraVatTuChiTietTuBnVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetPhieuHoanTraVatTuTuBnExportExcelChild>>();
                item.DuyetPhieuHoanTraVatTuTuBnExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetPhieuHoanTraVatTuTuBnExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(DuyetPhieuHoanTraVatTuTuBnExportExcel.KhoaHoanTraDisplay), "Khoa Hoàn Trả"),
                (nameof(DuyetPhieuHoanTraVatTuTuBnExportExcel.HoanTraVeKhoDisplay), "Hoàn Trả Về Kho"),
                (nameof(DuyetPhieuHoanTraVatTuTuBnExportExcel.NguoiYeuCauDisplay), "Người Yêu Cầu"),
                (nameof(DuyetPhieuHoanTraVatTuTuBnExportExcel.NgayYeuCauDisplay), "Ngày Yêu Cầu"),
                (nameof(DuyetPhieuHoanTraVatTuTuBnExportExcel.TinhTrangDisplay), "Tình Trạng"),
                (nameof(DuyetPhieuHoanTraVatTuTuBnExportExcel.NguoiDuyetDisplay), "Người Duyệt"),
                (nameof(DuyetPhieuHoanTraVatTuTuBnExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(DuyetPhieuHoanTraVatTuTuBnExportExcel.DuyetPhieuHoanTraVatTuTuBnExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Duyệt Phiếu Trả Vật Tư Từ Người Bệnh Nội Trú");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetPhieuTraVatTuTuBenhNhanNoiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DuyetTraVatTuTuBenhNhan)]
        public async Task<ActionResult> Delete(long id)
        {
            var yeuCauHoanTraVatTu = await _duyetPhieuTraVatTuTuBenhNhanService.GetByIdAsync(id, q =>
                q.Include(w => w.YeuCauTraVatTuTuBenhNhanChiTiets)
                    .ThenInclude(w => w.YeuCauVatTuBenhVien)
            );
            if (yeuCauHoanTraVatTu == null)
            {
                return NotFound();
            }

            await _duyetPhieuTraVatTuTuBenhNhanService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("DuyetTraVatTuTuBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetTraVatTuTuBenhNhan)]
        public async Task<ActionResult> DuyetTraVatTuTuBenhNhan([FromBody] ApproveRequestParam reqParam)
        {
            var yeuCauHoanTraVatTu = await _duyetPhieuTraVatTuTuBenhNhanService.GetByIdAsync(reqParam.Id,
                w => w.Include(p => p.YeuCauTraVatTuTuBenhNhanChiTiets)
                    .ThenInclude(p => p.YeuCauVatTuBenhVien)
                    .ThenInclude(p => p.XuatKhoVatTuChiTiet)
                    .ThenInclude(p => p.XuatKhoVatTuChiTietViTris).ThenInclude(p => p.NhapKhoVatTuChiTiet));
            yeuCauHoanTraVatTu.GhiChu = reqParam.GhiChu;
            yeuCauHoanTraVatTu.DuocDuyet = true;
            yeuCauHoanTraVatTu.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
            yeuCauHoanTraVatTu.NgayDuyet = DateTime.Now;

            foreach (var yeuCauTraVatTuTuBenhNhanChiTiet in yeuCauHoanTraVatTu.YeuCauTraVatTuTuBenhNhanChiTiets)
            {
                var soLuongDaXuatTheoKho = await _duyetPhieuTraVatTuTuBenhNhanService.GetSoLuongXuat(yeuCauTraVatTuTuBenhNhanChiTiet.VatTuBenhVienId, yeuCauTraVatTuTuBenhNhanChiTiet.LaVatTuBHYT, yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.HopDongThauVatTuId, reqParam.HoanTraVeKhoId);
                var listXuatKhoTra = new List<XuatKhoVatTuChiTietViTri>();

                var soLuongTra = yeuCauTraVatTuTuBenhNhanChiTiet.SoLuongTra;

                #region //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                var lstNhapChiTietChuaHoanTraHet = yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris
                    .GroupBy(x => new { x.NhapKhoVatTuChiTietId })
                    .Select(x => new
                    {
                        NhapKhoId = x.Key.NhapKhoVatTuChiTietId,
                        SoLuong = x.Sum(a => a.SoLuongXuat).MathRoundNumber(2)
                    }).Where(x => x.SoLuong > 0)
                    .Distinct().ToList();
                #endregion

                foreach (var xuatKhoDpChiTietViTri in yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet
                                                            //.XuatKhoVatTuChiTietViTris.Where(x => x.SoLuongXuat > 0).OrderByDescending(z => z.Id))

                                                            //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                                                            .XuatKhoVatTuChiTietViTris.Where(x => x.SoLuongXuat > 0 && lstNhapChiTietChuaHoanTraHet.Select(a => a.NhapKhoId).Contains(x.NhapKhoVatTuChiTietId))
                                                            .OrderByDescending(z => z.Id))
                {
                    if (soLuongTra <= soLuongDaXuatTheoKho)
                    {
                        if (soLuongTra == 0)
                        {
                            break;
                        }
                        var xuatKhoDpChiTietViTriTraPhieu = new XuatKhoVatTuChiTietViTri
                        {
                            Id = 0,
                            XuatKhoVatTuChiTietId = xuatKhoDpChiTietViTri.XuatKhoVatTuChiTietId,
                            NhapKhoVatTuChiTietId = xuatKhoDpChiTietViTri.NhapKhoVatTuChiTietId,
                            NgayXuat = DateTime.Now,
                            YeuCauTraVatTuTuBenhNhanChiTietId = yeuCauTraVatTuTuBenhNhanChiTiet.Id
                        };

                        #region //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                        var thongTinXuatHoanTraThucTe = lstNhapChiTietChuaHoanTraHet.FirstOrDefault(x => x.NhapKhoId == xuatKhoDpChiTietViTri.NhapKhoVatTuChiTietId);
                        var soLuongDaXuatChuaHoan = xuatKhoDpChiTietViTri.SoLuongXuat;
                        if (thongTinXuatHoanTraThucTe != null)
                        {
                            soLuongDaXuatChuaHoan = thongTinXuatHoanTraThucTe.SoLuong;
                        }
                        #endregion

                        //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                        //if (soLuongTra < xuatKhoDpChiTietViTri.SoLuongXuat || soLuongTra.AlmostEqual(xuatKhoDpChiTietViTri.SoLuongXuat))
                        if (soLuongTra < soLuongDaXuatChuaHoan || soLuongTra.AlmostEqual(soLuongDaXuatChuaHoan))
                        {
                            xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat -= soLuongTra;
                            //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                            //yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.SoLuongDaTra = yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.SoLuongDaTra != null ? yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.SoLuongDaTra + soLuongTra : soLuongTra;
                            //yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.SoLuong -= soLuongTra;
                            if (xuatKhoDpChiTietViTri.NhapKhoVatTuChiTiet != null)
                            {
                                xuatKhoDpChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= soLuongTra;
                            }

                            soLuongTra = 0;
                        }
                        else
                        {
                            //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                            //xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat -= xuatKhoDpChiTietViTri.SoLuongXuat;
                            //soLuongTra -= xuatKhoDpChiTietViTri.SoLuongXuat;
                            ////update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                            ////yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.SoLuongDaTra = yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.SoLuongDaTra != null ? yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.SoLuongDaTra + xuatKhoDpChiTietViTri.SoLuongXuat : xuatKhoDpChiTietViTri.SoLuongXuat;
                            ////yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.SoLuong -= xuatKhoDpChiTietViTri.SoLuongXuat;
                            //if (xuatKhoDpChiTietViTri.NhapKhoVatTuChiTiet != null)
                            //{
                            //    xuatKhoDpChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= xuatKhoDpChiTietViTri.SoLuongXuat;
                            //}

                            xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat = (xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat - soLuongDaXuatChuaHoan).MathRoundNumber(2);
                            soLuongTra = (soLuongTra - soLuongDaXuatChuaHoan).MathRoundNumber(2);
                            if (xuatKhoDpChiTietViTri.NhapKhoVatTuChiTiet != null)
                            {
                                xuatKhoDpChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat = (xuatKhoDpChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat - soLuongDaXuatChuaHoan).MathRoundNumber(2);
                            }

                        }
                        listXuatKhoTra.Add(xuatKhoDpChiTietViTriTraPhieu);
                    }
                }

                foreach (var xuatKhoTra in listXuatKhoTra)
                {
                    yeuCauTraVatTuTuBenhNhanChiTiet.YeuCauVatTuBenhVien.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Add(xuatKhoTra);
                }
            }

            await _duyetPhieuTraVatTuTuBenhNhanService.UpdateAsync(yeuCauHoanTraVatTu);

            return Ok();
        }
    }
}
