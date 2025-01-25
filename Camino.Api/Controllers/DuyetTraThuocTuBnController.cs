using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.DuyetPhieuHoanTraThuocTuBns;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.DuyetPhieuTraThuocTuBns;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class DuyetTraThuocTuBnController : CaminoBaseController
    {
        private readonly IDuyetPhieuTraThuocTuBnService _duyetPhieuTraThuocTuBenhNhanService;
        private readonly IExcelService _excelService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ILocalizationService _localizationService;

        public DuyetTraThuocTuBnController(IDuyetPhieuTraThuocTuBnService duyetPhieuTraThuocTuBenhNhanService, IExcelService excelService, IUserAgentHelper userAgentHelper, ILocalizationService localizationService)
        {
            _excelService = excelService;
            _duyetPhieuTraThuocTuBenhNhanService = duyetPhieuTraThuocTuBenhNhanService;
            _userAgentHelper = userAgentHelper;
            _localizationService = localizationService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duyetPhieuTraThuocTuBenhNhanService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duyetPhieuTraThuocTuBenhNhanService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDuocPhamChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDuocPhamChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duyetPhieuTraThuocTuBenhNhanService.GetDataForGridAsyncDuocPhamChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncBenhNhanChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraThuocTuBenhNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncBenhNhanChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _duyetPhieuTraThuocTuBenhNhanService.GetDataForGridAsyncBenhNhanChild(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DuyetTraThuocTuBenhNhan)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DuyetPhieuHoanTraThuocTuBnViewModel>> Get(long id)
        {
            var duyetPhieuHoanTraThuoc = await _duyetPhieuTraThuocTuBenhNhanService.GetByIdAsync(id,
                w => w.Include(q => q.KhoaHoanTra)
                    .Include(q => q.KhoTra)
                    .Include(q => q.YeuCauTraDuocPhamTuBenhNhanChiTiets).ThenInclude(q => q.YeuCauDuocPhamBenhVien).ThenInclude(q => q.NoiTruChiDinhDuocPham)
                    .Include(q => q.NhanVienYeuCau).ThenInclude(q => q.User));

            if (duyetPhieuHoanTraThuoc == null)
            {
                return NotFound();
            }

            var duyetPhieuHoanTraResult = new DuyetPhieuHoanTraThuocTuBnViewModel
            {
                Id = duyetPhieuHoanTraThuoc.Id,
                GhiChu = duyetPhieuHoanTraThuoc.GhiChu,
                SoPhieu = duyetPhieuHoanTraThuoc.SoPhieu,
                KhoaHoanTraId = duyetPhieuHoanTraThuoc.KhoaHoanTraId,
                KhoaHoanTraDisplay = duyetPhieuHoanTraThuoc.KhoaHoanTra.Ten,
                HoanTraVeKhoId = duyetPhieuHoanTraThuoc.KhoTraId,
                HoanTraVeKhoDisplay = duyetPhieuHoanTraThuoc.KhoTra.Ten,
                NgayYeuCau = duyetPhieuHoanTraThuoc.NgayYeuCau,
                TinhTrang = duyetPhieuHoanTraThuoc.DuocDuyet,
                NguoiYeuCauId = duyetPhieuHoanTraThuoc.NhanVienYeuCauId,
                NguoiYeuCauDisplay = duyetPhieuHoanTraThuoc.NhanVienYeuCau.User.HoTen,
                //LaDichTruyen = duyetPhieuHoanTraThuoc.YeuCauTraDuocPhamTuBenhNhanChiTiets.Select(x => x.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham.LaDichTruyen).FirstOrDefault(),
                LaDichTruyen = duyetPhieuHoanTraThuoc.YeuCauTraDuocPhamTuBenhNhanChiTiets.Select(x => x.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham?.LaDichTruyen ?? x.YeuCauDuocPhamBenhVien.LaDichTruyen).FirstOrDefault()
            };

            return Ok(duyetPhieuHoanTraResult);
        }

        [HttpPost("ExportDuyetPhieuTraThuocTuBn")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetTraThuocTuBenhNhan)]
        public async Task<ActionResult> ExportDuyetPhieuTraThuocTuBn(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = int.MaxValue;

            var gridData = await _duyetPhieuTraThuocTuBenhNhanService.GetDataForGridAsync(queryInfo, true);
            var data = gridData.Data.Select(p => (DuyetTraThuocTuBnVo)p).ToList();
            var dataExcel = data.Map<List<DuyetPhieuHoanTraThuocTuBnExportExcel>>();

            queryInfo.Sort = new List<Sort> { new Sort { Field = "DuocPham", Dir = "asc" } };

            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id + "";
                var gridChildData = await _duyetPhieuTraThuocTuBenhNhanService.GetDataForGridAsyncDuocPhamChild(queryInfo);
                var dataChild = gridChildData.Data.Select(p => (DuyetTraThuocChiTietTuBnVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<DuyetPhieuHoanTraThuocTuBnExportExcelChild>>();
                item.DuyetPhieuHoanTraThuocTuBnExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(DuyetPhieuHoanTraThuocTuBnExportExcel.SoPhieu), "Số Phiếu"),
                (nameof(DuyetPhieuHoanTraThuocTuBnExportExcel.KhoaHoanTraDisplay), "Khoa Hoàn Trả"),
                (nameof(DuyetPhieuHoanTraThuocTuBnExportExcel.HoanTraVeKhoDisplay), "Hoàn Trả Về Kho"),
                (nameof(DuyetPhieuHoanTraThuocTuBnExportExcel.NguoiYeuCauDisplay), "Người Yêu Cầu"),
                (nameof(DuyetPhieuHoanTraThuocTuBnExportExcel.NgayYeuCauDisplay), "Ngày Yêu Cầu"),
                (nameof(DuyetPhieuHoanTraThuocTuBnExportExcel.TinhTrangDisplay), "Tình Trạng"),
                (nameof(DuyetPhieuHoanTraThuocTuBnExportExcel.NguoiDuyetDisplay), "Người Duyệt"),
                (nameof(DuyetPhieuHoanTraThuocTuBnExportExcel.NgayDuyetDisplay), "Ngày Duyệt"),
                (nameof(DuyetPhieuHoanTraThuocTuBnExportExcel.DuyetPhieuHoanTraThuocTuBnExportExcelChild), "")
            };

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Duyệt Phiếu Trả Thuốc Từ Người Bệnh Nội Trú");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DuyetPhieuTraThuocTuBenhNhanNoiTru" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucNgheNghiep)]
        public async Task<ActionResult> Delete(long id)
        {
            var yeuCauHoanTraDuocPham = await _duyetPhieuTraThuocTuBenhNhanService.GetByIdAsync(id, q =>
                q.Include(w => w.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                .ThenInclude(w => w.YeuCauDuocPhamBenhVien)
            );
            if (yeuCauHoanTraDuocPham == null)
            {
                return NotFound();
            }

            await _duyetPhieuTraThuocTuBenhNhanService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("DuyetTraThuocTuBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DuyetTraThuocTuBenhNhan)]
        public async Task<ActionResult> DuyetTraThuocTuBenhNhan([FromBody] ApproveRequestParam reqParam)
        {
            var yeuCauHoanTraDuocPham = await _duyetPhieuTraThuocTuBenhNhanService.GetByIdAsync(reqParam.Id,
                w => w.Include(p => p.YeuCauTraDuocPhamTuBenhNhanChiTiets).ThenInclude(p => p.YeuCauDuocPhamBenhVien).ThenInclude(p => p.XuatKhoDuocPhamChiTiet).ThenInclude(p => p.XuatKhoDuocPhamChiTietViTris).ThenInclude(p => p.NhapKhoDuocPhamChiTiet)
                    .Include(p => p.YeuCauTraDuocPhamTuBenhNhanChiTiets).ThenInclude(p => p.YeuCauDuocPhamBenhVien).ThenInclude(p => p.NoiTruChiDinhDuocPham)
                );

            yeuCauHoanTraDuocPham.GhiChu = reqParam.GhiChu;
            yeuCauHoanTraDuocPham.DuocDuyet = true;
            yeuCauHoanTraDuocPham.NhanVienDuyetId = _userAgentHelper.GetCurrentUserId();
            yeuCauHoanTraDuocPham.NgayDuyet = DateTime.Now;


            //var yeuCauTraDuocPhamTuBenhNhanChiTietThuocs = yeuCauHoanTraDuocPham.YeuCauTraDuocPhamTuBenhNhanChiTiets.Where(z => z.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham.LaDichTruyen != true).ToList();
            var yeuCauTraDuocPhamTuBenhNhanChiTietThuocs = yeuCauHoanTraDuocPham.YeuCauTraDuocPhamTuBenhNhanChiTiets.Where(z => (z.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham?.LaDichTruyen ?? z.YeuCauDuocPhamBenhVien.LaDichTruyen) != true).ToList();

            if (yeuCauTraDuocPhamTuBenhNhanChiTietThuocs.Any())
            {
                foreach (var yeuCauTraDuocPhamTuBenhNhanChiTiet in yeuCauTraDuocPhamTuBenhNhanChiTietThuocs)
                {
                    var soLuongDaXuatTheoKho = await _duyetPhieuTraThuocTuBenhNhanService.
                        GetSoLuongXuat(yeuCauTraDuocPhamTuBenhNhanChiTiet.DuocPhamBenhVienId, yeuCauTraDuocPhamTuBenhNhanChiTiet.LaDuocPhamBHYT, yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.HopDongThauDuocPhamId, reqParam.HoanTraVeKhoId);
                    var listXuatKhoTra = new List<XuatKhoDuocPhamChiTietViTri>();
                    var soLuongTra = yeuCauTraDuocPhamTuBenhNhanChiTiet.SoLuongTra;

                    #region //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                    var lstNhapChiTietChuaHoanTraHet = yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien
                        .XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris
                        .GroupBy(x => new { x.NhapKhoDuocPhamChiTietId })
                        .Select(x => new
                        {
                            NhapKhoId = x.Key.NhapKhoDuocPhamChiTietId,
                            SoLuong = x.Sum(a => a.SoLuongXuat).MathRoundNumber(2)
                        }).Where(x => x.SoLuong > 0)
                        .Distinct().ToList();
                    #endregion

                    foreach (var xuatKhoDpChiTietViTri in yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet
                                                            //.XuatKhoDuocPhamChiTietViTris.Where(x => x.SoLuongXuat > 0)

                                                            //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                                                            .XuatKhoDuocPhamChiTietViTris.Where(x => x.SoLuongXuat > 0 && lstNhapChiTietChuaHoanTraHet.Select(a => a.NhapKhoId).Contains(x.NhapKhoDuocPhamChiTietId))
                                                            .OrderByDescending(z => z.Id))
                    {
                        if (soLuongTra <= soLuongDaXuatTheoKho)
                        {                           
                            if (soLuongTra == 0)
                            {
                                break;
                            }
                            var xuatKhoDpChiTietViTriTraPhieu = new XuatKhoDuocPhamChiTietViTri
                            {
                                Id = 0,
                                XuatKhoDuocPhamChiTietId = xuatKhoDpChiTietViTri.XuatKhoDuocPhamChiTietId,
                                NhapKhoDuocPhamChiTietId = xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTietId,
                                NgayXuat = DateTime.Now,
                                YeuCauTraDuocPhamTuBenhNhanChiTietId = yeuCauTraDuocPhamTuBenhNhanChiTiet.Id
                            };

                            #region //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                            var thongTinXuatHoanTraThucTe = lstNhapChiTietChuaHoanTraHet.FirstOrDefault(x => x.NhapKhoId == xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTietId);
                            var soLuongDaXuatChuaHoan = xuatKhoDpChiTietViTri.SoLuongXuat;
                            if (thongTinXuatHoanTraThucTe != null)
                            {
                                soLuongDaXuatChuaHoan = thongTinXuatHoanTraThucTe.SoLuong;
                            }
                            #endregion

                            //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                            //if (yeuCauTraDuocPhamTuBenhNhanChiTiet.SoLuongTra < xuatKhoDpChiTietViTri.SoLuongXuat || yeuCauTraDuocPhamTuBenhNhanChiTiet.SoLuongTra.AlmostEqual(xuatKhoDpChiTietViTri.SoLuongXuat))
                            if (soLuongTra < soLuongDaXuatChuaHoan || soLuongTra.AlmostEqual(soLuongDaXuatChuaHoan))
                            {
                                xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat -= soLuongTra;
                                if (xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet != null)
                                {
                                    xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= soLuongTra;
                                }
                                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                                //yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra = yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra != null ? yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra + soLuongTra : soLuongTra;
                                //yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuong -= soLuongTra;
                                //if (yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPhamId != null)
                                //{
                                //    yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham.SoLuong -= soLuongTra;
                                //}
                                soLuongTra = 0;
                            }
                            else
                            {
                                //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                                //xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //soLuongTra -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //if (xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet != null)
                                //{
                                //    xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //}
                                xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat = (xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat - soLuongDaXuatChuaHoan).MathRoundNumber(2);
                                soLuongTra = (soLuongTra - soLuongDaXuatChuaHoan).MathRoundNumber(2);
                                if (xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet != null)
                                {
                                    xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = (xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - soLuongDaXuatChuaHoan).MathRoundNumber(2);
                                }

                                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                                //yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra = yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra != null ? yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra + xuatKhoDpChiTietViTri.SoLuongXuat : xuatKhoDpChiTietViTri.SoLuongXuat;
                                //yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuong -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //if (yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPhamId != null)
                                //{
                                //    yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham.SoLuong -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //}
                            }
                            listXuatKhoTra.Add(xuatKhoDpChiTietViTriTraPhieu);

                        }
                        else
                        {
                            throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"), (int)HttpStatusCode.BadRequest);
                        }
                    }

                    foreach (var xuatKhoTra in listXuatKhoTra)
                    {
                        yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoTra);
                    }
                }
            }

            //var yeuCauTraDuocPhamTuBenhNhanChiTietDichTruyens = yeuCauHoanTraDuocPham.YeuCauTraDuocPhamTuBenhNhanChiTiets.Where(z => z.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham.LaDichTruyen == true).ToList();
            var yeuCauTraDuocPhamTuBenhNhanChiTietDichTruyens = yeuCauHoanTraDuocPham.YeuCauTraDuocPhamTuBenhNhanChiTiets.Where(z => (z.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham?.LaDichTruyen ?? z.YeuCauDuocPhamBenhVien.LaDichTruyen) == true).ToList();

            if (yeuCauTraDuocPhamTuBenhNhanChiTietDichTruyens.Any())
            {
                foreach (var yeuCauTraDuocPhamTuBenhNhanChiTiet in yeuCauTraDuocPhamTuBenhNhanChiTietDichTruyens)
                {
                    var soLuongDaXuatTheoKho = await _duyetPhieuTraThuocTuBenhNhanService.
                        GetSoLuongXuat(yeuCauTraDuocPhamTuBenhNhanChiTiet.DuocPhamBenhVienId, yeuCauTraDuocPhamTuBenhNhanChiTiet.LaDuocPhamBHYT, yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.HopDongThauDuocPhamId, reqParam.HoanTraVeKhoId);
                    var listXuatKhoTra = new List<XuatKhoDuocPhamChiTietViTri>();

                    var soLuongTra = yeuCauTraDuocPhamTuBenhNhanChiTiet.SoLuongTra;

                    #region //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                    var lstNhapChiTietChuaHoanTraHet = yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien
                        .XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris
                        .GroupBy(x => new { x.NhapKhoDuocPhamChiTietId })
                        .Select(x => new
                        {
                            NhapKhoId = x.Key.NhapKhoDuocPhamChiTietId,
                            SoLuong = x.Sum(a => a.SoLuongXuat).MathRoundNumber(2)
                        }).Where(x => x.SoLuong > 0)
                        .Distinct().ToList();
                    #endregion

                    foreach (var xuatKhoDpChiTietViTri in yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet
                                                            //.XuatKhoDuocPhamChiTietViTris.Where(x => x.SoLuongXuat > 0).OrderByDescending(z => z.Id))

                                                            //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                                                            .XuatKhoDuocPhamChiTietViTris.Where(x => x.SoLuongXuat > 0 && lstNhapChiTietChuaHoanTraHet.Select(a => a.NhapKhoId).Contains(x.NhapKhoDuocPhamChiTietId))
                                                            .OrderByDescending(z => z.Id))
                    {
                        if (soLuongTra <= soLuongDaXuatTheoKho)
                        {
                            if (soLuongTra == 0)
                            {
                                break;
                            }
                            var xuatKhoDpChiTietViTriTraPhieu = new XuatKhoDuocPhamChiTietViTri
                            {
                                Id = 0,
                                XuatKhoDuocPhamChiTietId = xuatKhoDpChiTietViTri.XuatKhoDuocPhamChiTietId,
                                NhapKhoDuocPhamChiTietId = xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTietId,
                                NgayXuat = DateTime.Now,
                                YeuCauTraDuocPhamTuBenhNhanChiTietId = yeuCauTraDuocPhamTuBenhNhanChiTiet.Id
                            };

                            #region //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                            var thongTinXuatHoanTraThucTe = lstNhapChiTietChuaHoanTraHet.FirstOrDefault(x => x.NhapKhoId == xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTietId);
                            var soLuongDaXuatChuaHoan = xuatKhoDpChiTietViTri.SoLuongXuat;
                            if (thongTinXuatHoanTraThucTe != null)
                            {
                                soLuongDaXuatChuaHoan = thongTinXuatHoanTraThucTe.SoLuong;
                            }
                            #endregion

                            //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                            //if (yeuCauTraDuocPhamTuBenhNhanChiTiet.SoLuongTra < xuatKhoDpChiTietViTri.SoLuongXuat || yeuCauTraDuocPhamTuBenhNhanChiTiet.SoLuongTra.AlmostEqual(xuatKhoDpChiTietViTri.SoLuongXuat))
                            if (soLuongTra < soLuongDaXuatChuaHoan || soLuongTra.AlmostEqual(soLuongDaXuatChuaHoan))
                            {
                                xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat -= soLuongTra;
                                if (xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet != null)
                                {
                                    xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= soLuongTra;
                                }
                                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                                //yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra = yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra != null ? yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra + soLuongTra : soLuongTra;
                                //yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuong -= soLuongTra;
                                //if (yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPhamId != null)
                                //{
                                //    yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham.SoLuong -= soLuongTra;
                                //}
                                soLuongTra = 0;
                            }
                            else
                            {
                                //Cập nhật 23/03/2022: fix bug hoàn trả khác số lô, HSD nhiều lần thì chưa trả số lượng về đúng NhapKhoChiTiet
                                //xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //soLuongTra -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //if (xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet != null)
                                //{
                                //    xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //}
                                xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat = (xuatKhoDpChiTietViTriTraPhieu.SoLuongXuat - soLuongDaXuatChuaHoan).MathRoundNumber(2);
                                soLuongTra = (soLuongTra - soLuongDaXuatChuaHoan).MathRoundNumber(2);
                                if (xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet != null)
                                {
                                    xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = (xuatKhoDpChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat - soLuongDaXuatChuaHoan).MathRoundNumber(2);
                                }

                                //update BVHD-3411: Khi hoàn trả thuốc/vật tư từ người bệnh (chưa cần duyệt phiếu hoàn trả) thì phần mềm ghi nhận số lượng và thành tiền còn lại sau khi hoàn trả
                                //yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra = yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra != null ? yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuongDaTra + xuatKhoDpChiTietViTri.SoLuongXuat : xuatKhoDpChiTietViTri.SoLuongXuat;
                                //yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.SoLuong -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //if (yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPhamId != null)
                                //{
                                //    yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.NoiTruChiDinhDuocPham.SoLuong -= xuatKhoDpChiTietViTri.SoLuongXuat;
                                //}
                            }
                            listXuatKhoTra.Add(xuatKhoDpChiTietViTriTraPhieu);

                        }
                        else
                        {
                            throw new ApiException(_localizationService.GetResource("DieuTriNoiTru.SoLuongTra.NotValid"), (int)HttpStatusCode.BadRequest);
                        }
                    }

                    foreach (var xuatKhoTra in listXuatKhoTra)
                    {
                        yeuCauTraDuocPhamTuBenhNhanChiTiet.YeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatKhoTra);
                    }
                }
            }

            await _duyetPhieuTraThuocTuBenhNhanService.UpdateAsync(yeuCauHoanTraDuocPham);

            return Ok();
        }
    }
}
